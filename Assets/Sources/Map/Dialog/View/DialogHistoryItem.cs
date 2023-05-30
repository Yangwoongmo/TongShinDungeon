using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogHistoryItem : MonoBehaviour
{
    private readonly Vector2 playerBubblePosition = new Vector2(0, 0);
    private readonly Vector2 characterBubblePosition = new Vector2(165, 0);

    [SerializeField] private Image dialogIcon;
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private DialogBubble dialogBubble;
    [SerializeField] private RectTransform itemTransform;

    public void CreateDialogHistoryItem(
        string dialog, 
        DialogBubbleType bubbleType, 
        DialogHistoryIconType iconType,
        bool isPlayerDialog
    )
    {
        if (isPlayerDialog)
        {
            dialogBubble.ShowDialog(dialog, bubbleType, playerBubblePosition, true);
        }
        else
        {
            dialogBubble.ShowDialog(dialog, bubbleType, characterBubblePosition, true);
        }
        
        if (iconType != DialogHistoryIconType.NONE)
        {
            dialogIcon.enabled = true;
            dialogIcon.sprite = iconSprites[(int)iconType];
        }
        else
        {
            dialogIcon.enabled = false;
        }

        AdjustItemHeightAndBubblePosition();
    }

    private void AdjustItemHeightAndBubblePosition()
    {
        (float width, float height) = dialogBubble.GetBubbleSize();

        float iconHeight = dialogIcon.enabled ? dialogIcon.rectTransform.rect.height : 0;
        float maxHeight = Mathf.Max(height, iconHeight);
        itemTransform.sizeDelta = new Vector2(width, maxHeight);

        dialogBubble.UpdateBubblePositionYBy((maxHeight - height) / 2);
    }

    public enum DialogHistoryIconType
    {
        WARRIOR,
        MAGICIAN,
        NONE
    }
}
