using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogHistoryList : MonoBehaviour
{
    private readonly DialogBubbleType characterFirstBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE, DialogBubbleCornerType.ROUND },
            DialogBubbleColorType.CHARACTER,
            DialogBubbleArrowType.NONE,
            DialogBubblePositionAlignType.NONE
        );

    private readonly DialogBubbleType characterBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.RECTANGLE, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE, DialogBubbleCornerType.ROUND },
            DialogBubbleColorType.CHARACTER,
            DialogBubbleArrowType.NONE,
            DialogBubblePositionAlignType.NONE
        );

    private readonly DialogBubbleType playerFirstBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE },
            DialogBubbleColorType.MINE,
            DialogBubbleArrowType.NONE,
            DialogBubblePositionAlignType.NONE
        );

    private readonly DialogBubbleType playerBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE },
            DialogBubbleColorType.MINE,
            DialogBubbleArrowType.NONE,
            DialogBubblePositionAlignType.NONE
        );

    [SerializeField] private GameObject dialogHistoryItemPrefab;
    [SerializeField] private RectTransform historyItemParent;
    [SerializeField] private VerticalLayoutGroup itemList;

    private DialogHistoryModel currentModel = null;

    public void CreateDialogHistoryList(DialogHistoryModel model)
    {
        if (currentModel != null && !currentModel.IsHistoryUpdated())
        {
            for (int i = 0; i < model.GetDialogHistories().Count; i++)
            {
                historyItemParent.GetChild(i).gameObject.SetActive(true);
            }
            return;
        }

        currentModel = model;

        List<string> histories = model.GetDialogHistories();
        DialogHistoryModel.DialogSpeakerType speakerType = model.GetSpeakerType();

        if (speakerType != DialogHistoryModel.DialogSpeakerType.PLAYER)
        {
            itemList.childAlignment = TextAnchor.UpperLeft;
        }
        else
        {
            itemList.childAlignment = TextAnchor.UpperRight;
        }

        for (int i = 0; i < histories.Count; i++)
        {
            bool isFirstBubble = i == 0;
            DialogBubbleType bubbleType;
            if (speakerType != DialogHistoryModel.DialogSpeakerType.PLAYER)
            {
                bubbleType = isFirstBubble ? characterFirstBubbleType : characterBubbleType;
            }
            else
            {
                bubbleType = isFirstBubble ? playerFirstBubbleType : playerBubbleType;
            }

            bool isLastBubble = i == histories.Count - 1;
            DialogHistoryItem.DialogHistoryIconType iconType;
            if (isLastBubble)
            {
                switch (speakerType)
                {
                    case DialogHistoryModel.DialogSpeakerType.WARRIOR:
                        iconType = DialogHistoryItem.DialogHistoryIconType.WARRIOR;
                        break;
                    case DialogHistoryModel.DialogSpeakerType.MAGICIAN:
                        iconType = DialogHistoryItem.DialogHistoryIconType.MAGICIAN;
                        break;
                    default:
                        iconType = DialogHistoryItem.DialogHistoryIconType.NONE;
                        break;
                }
            }
            else
            {
                iconType = DialogHistoryItem.DialogHistoryIconType.NONE;
            }

            if (historyItemParent.childCount <= i)
            {
                Instantiate(dialogHistoryItemPrefab, historyItemParent);
            }

            DialogHistoryItem item = historyItemParent.GetChild(i).gameObject.GetComponent<DialogHistoryItem>();
            item.gameObject.SetActive(true);
            item.CreateDialogHistoryItem(histories[i], bubbleType, iconType, speakerType == DialogHistoryModel.DialogSpeakerType.PLAYER);

            LayoutRebuilder.ForceRebuildLayoutImmediate(historyItemParent);
        }

        currentModel.SetAllHistoryUsed();
    }

    public void HideDialogHistoryList()
    {
        CleanupList();
        this.gameObject.SetActive(false);
    }

    private void CleanupList()
    {
        for (int i = 0; i < historyItemParent.childCount; i++)
        {
            historyItemParent.GetChild(i).gameObject.SetActive(false);
        }
    }
}

public class DialogHistoryModel
{
    private DialogSpeakerType speakerType;

    /**
     * Should not update history directly.
     * Only update histories by [AddDialogHistoryIfPossible].
     */
    private List<string> dialogHistories;

    private bool isHistoryUpdated;

    public DialogHistoryModel(DialogSpeakerType speakerType, List<string> histories)
    {
        this.speakerType = speakerType;
        dialogHistories = new List<string>();
        dialogHistories.AddRange(histories);
        isHistoryUpdated = true;
    }

    public DialogSpeakerType GetSpeakerType()
    {
        return speakerType;
    }

    public List<string> GetDialogHistories()
    {
        return dialogHistories;
    }

    public bool AddDialogHistoryIfPossible(DialogSpeakerType speakerType, string dialog)
    {
        if (this.speakerType != speakerType)
        {
            return false;
        }

        dialogHistories.Add(dialog);
        isHistoryUpdated = true;
        return true;
    }

    public bool IsHistoryUpdated()
    {
        return isHistoryUpdated;
    }

    public void SetAllHistoryUsed()
    {
        isHistoryUpdated = false;
    }

    public enum DialogSpeakerType
    {
        WARRIOR,
        MAGICIAN,
        PLAYER
    }
}
