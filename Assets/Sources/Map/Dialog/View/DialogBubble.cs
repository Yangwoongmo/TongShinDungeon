using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBubble : MonoBehaviour
{
    [SerializeField] private Text internalBodyText;
    [SerializeField] private Text showingBodyText;

    [SerializeField] private Image bodyBackground;
    [SerializeField] private VerticalLayoutGroup bodyLayoutGroup;
    [SerializeField] private Image[] spacings;
    [SerializeField] private Sprite[] boxImages;

    [SerializeField] private Image[] corners;
    [SerializeField] private List<TwoDimensionListForInspector<Sprite>> cornerSprites;

    [SerializeField] private Image[] bubbleArrows;

    [SerializeField] private RectTransform bubbleTransform;

    public void ShowDialog(string dialogText, DialogBubbleType bubbleType, Vector2 bubblePosition, bool showWithText = false)
    {
        internalBodyText.text = dialogText;
        showingBodyText.text = "";
        SetupDialogBubbleView(bubbleType, bubblePosition, showWithText);
    }

    public void HideDialog()
    {
        showingBodyText.text = "";
        internalBodyText.text = "";

        bodyBackground.enabled = false;
        for (int i = 0; i < 4; i++)
        {
            corners[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            spacings[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            bubbleArrows[i].gameObject.SetActive(false);
        }
    }

    public void SetShowingDialogText(string dialogText)
    {
        showingBodyText.text = dialogText;
    }

    public (float, float) GetBubbleSize()
    {
        float totalWidth = internalBodyText.rectTransform.rect.width + corners[0].rectTransform.rect.width * 2 + bodyLayoutGroup.padding.right * 2;
        float totalHeight = internalBodyText.rectTransform.rect.height + bodyLayoutGroup.padding.top * 2;
        return (totalWidth, totalHeight);
    }

    public void UpdateBubblePositionYBy(float amount)
    {
        bubbleTransform.localPosition = new Vector3(bubbleTransform.localPosition.x, bubbleTransform.localPosition.y + amount, 0);
    }

    private void SetupDialogBubbleView(DialogBubbleType bubbleType, Vector2 bubblePosition, bool showWithText)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(internalBodyText.rectTransform);

        bodyBackground.enabled = true;
        bodyBackground.sprite = boxImages[bubbleType.GetColorType()];

        float width = internalBodyText.rectTransform.rect.width;
        float height = internalBodyText.rectTransform.rect.height;

        showingBodyText.rectTransform.sizeDelta = internalBodyText.rectTransform.sizeDelta;

        float cornerSize = corners[0].rectTransform.rect.height;
        float bodyPaddingTop = bodyLayoutGroup.padding.top;
        float bodyPaddingRight = bodyLayoutGroup.padding.right;

        // Corner (LeftTop, RightTop, LeftBottom, RightBottom) setup

        float topYPosition = height / 2 - cornerSize / 2 + bodyPaddingTop;
        float rightXPosition = width / 2 + cornerSize / 2 + bodyPaddingRight;

        float[] cornerPositionX = new float[4] { -rightXPosition, rightXPosition, -rightXPosition, rightXPosition };
        float[] cornerPositionY = new float[4] { topYPosition, topYPosition, -topYPosition, -topYPosition };

        for (int i = 0; i < 4; i++)
        {
            corners[i].rectTransform.localPosition = new Vector3(cornerPositionX[i], cornerPositionY[i], 0);
            corners[i].sprite = cornerSprites[i].GetList()[bubbleType.GetCornerType(i) * 2 + bubbleType.GetColorType()];
            corners[i].gameObject.SetActive(true);
        }

        // Spacing (Left, Right) setup

        float spacingSizeY = height - cornerSize * 2 + bodyPaddingTop * 2;

        for (int i = 0; i < 2; i++)
        {
            spacings[i].rectTransform.localPosition = new Vector3(cornerPositionX[i], 0, 0);
            spacings[i].rectTransform.sizeDelta = new Vector2(50, spacingSizeY);
            spacings[i].sprite = boxImages[bubbleType.GetColorType()];
            spacings[i].gameObject.SetActive(true);
        }

        // Arrow (Left, Right) setup

        float arrowSize = bubbleArrows[0].rectTransform.rect.width;
        float[] arrowPositionX = new float[2] { -rightXPosition + arrowSize, rightXPosition - arrowSize };
        int arrowType = bubbleType.GetArrowType();
        for (int i = 0; i < 2; i++)
        {
            bubbleArrows[i].gameObject.SetActive(arrowType == i + 1);
            bubbleArrows[i].rectTransform.localPosition = new Vector3(arrowPositionX[i], -topYPosition - arrowSize, 0);
        }

        // Bubble positioning

        float bubbleLeftPositionX = width / 2 + cornerSize + bodyPaddingRight;
        float bubbleBottomPositionY = height / 2 + bodyPaddingTop;
        float[] bubblePositionX = new float[5] {
            0,
            bubbleLeftPositionX,
            -bubbleLeftPositionX,
            bubbleLeftPositionX,
            -bubbleLeftPositionX,
        };
        float[] bubblePositionY = new float[5] {
            0,
            bubbleBottomPositionY,
            bubbleBottomPositionY,
            -bubbleBottomPositionY,
            -bubbleBottomPositionY
        };
        int alignType = bubbleType.GetAlignType();

        // Adjust for resolution change
        float adjustedBubblePosition = bubblePosition.y;
        if (alignType > 0)
        {
            adjustedBubblePosition = 
                (Mathf.Abs(bubblePosition.y) - ResolutionUtils.GetCanvasHeightDifferenceFromReference() / 2f) * Mathf.Sign(bubblePosition.y);
        }
            
        bubbleTransform.localPosition = new Vector3(bubblePosition.x + bubblePositionX[alignType], adjustedBubblePosition + bubblePositionY[alignType], 0);

        if (showWithText)
        {
            showingBodyText.text = internalBodyText.text;
        }
    }
}

public enum DialogBubbleCornerType
{
    ROUND,
    RECTANGLE
}

public enum DialogBubbleColorType
{
    CHARACTER,
    MINE
}

public enum DialogBubbleArrowType
{
    NONE,
    LEFT,
    RIGHT
}

public enum DialogBubblePositionAlignType
{
    NONE,
    LEFT_BOTTOM,
    RIGHT_BOTTOM,
    LEFT_TOP,
    RIGHT_TOP
}

public class DialogBubbleType
{
    private List<DialogBubbleCornerType> cornerTypes;
    private DialogBubbleColorType colorType;
    private DialogBubbleArrowType arrowType;
    private DialogBubblePositionAlignType alignType;

    public DialogBubbleType(
        DialogBubbleCornerType[] cornerTypes,
        DialogBubbleColorType colorType,
        DialogBubbleArrowType arrowType,
        DialogBubblePositionAlignType alignType
    )
    {
        this.cornerTypes = new List<DialogBubbleCornerType>();
        this.cornerTypes.AddRange(cornerTypes);
        this.colorType = colorType;
        this.arrowType = arrowType;
        this.alignType = alignType;
    }

    public int GetCornerType(int index)
    {
        return (int)cornerTypes[index];
    }

    public int GetColorType()
    {
        return (int)colorType;
    }

    public int GetArrowType()
    {
        return (int)arrowType;
    }

    public int GetAlignType()
    {
        return (int)alignType;
    }
}