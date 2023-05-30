using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryDialogController : ExploreDialogController, IDialogStatusListener
{
    [SerializeField] private GameObject[] actionButtons;

    protected override DialogSceneType sceneType => DialogSceneType.SANCTUARY;

    private string pendingSequenceDialogId = "";
    private int pendingSequence = 0;
    private IPlayerEventDialogListener listener;

    private DialogFileAndStatusManager dialogManager = DialogFileAndStatusManager.GetInstance();
    private Queue<SanctuaryNormalDialog> normalDialogQueue;

    private List<SanctuaryNormalDialog> excessiveDialogList;
    private int excessiveDialogHead = 0;

    public void SetPlayerEventDialogListener(IPlayerEventDialogListener listener)
    {
        this.listener = listener;
    }

    public void LoadNormalDialogQueue(int progress, CharacterType type)
    {
        normalDialogQueue = dialogManager.GetSanctuaryNormalDialogList(progress, type);
    }

    public void LoadExcessiveDialogList(CharacterType type)
    {
        excessiveDialogList = dialogManager.GetSanctuaryExcessiveDialogList(type);
        excessiveDialogHead = 0;

        ShuffleExcessiveList();
    }

    public void ShowNormalDialg()
    {
        if (!IsWaitingOrFading())
        {
            SkipDialog();
            return;
        }

        bool isExceedDialog = normalDialogQueue == null || normalDialogQueue.Count == 0;
        if (isExceedDialog)
        {
            if (excessiveDialogHead >= excessiveDialogList.Count)
            {
                ShuffleExcessiveList();
                excessiveDialogHead = 0;
            }

            DialogModel excessiveDialog = excessiveDialogList[excessiveDialogHead++].GetCurrentDialog(0).Item1;
            ShowDialogForSelfCamera(new List<DialogModel>() { excessiveDialog });
            return;
        }

        if (pendingSequenceDialogId == null || pendingSequenceDialogId.Length == 0)
        {
            ShowNormalDialogWithNewSequence();
            return;
        }

        SanctuaryNormalDialog dialog = normalDialogQueue.Peek();
        if (dialog == null)
        {
            ShowNormalDialogWithNewSequence();
            return;
        }

        (DialogModel targetDialog, int nextSequence) = dialog.GetCurrentDialog(pendingSequence);
        if (nextSequence > 0)
        {
            pendingSequence = nextSequence;
        }
        else
        {
            pendingSequence = 0;
            pendingSequenceDialogId = "";
            normalDialogQueue.Dequeue();
        }

        ShowDialogForSelfCamera(new List<DialogModel>() { targetDialog });
    }

    public void ShowSanctuaryDeathDialog(SanctuaryDeathStatistics statistics)
    {
        if (!IsWaitingOrFading())
        {
            SkipDialog();
            return;
        }

        string dialogFileId;
        if (statistics.GetDeathCount() == 1)
        {
            dialogFileId = "0001";
        }
        else
        {
            dialogFileId = "0002";
        }

        SanctuaryNormalDialog dialog = dialogManager.GetSanctuaryDeathDialog(dialogFileId);
        (DialogModel targetDialog, int nextSequence) = dialog.GetCurrentDialog(0);

        ShowDialogForSelfCamera(new List<DialogModel>() { targetDialog });
    }

    public bool ShowSanctuaryEventDialog(string dialogId)
    {
        if (!IsWaitingOrFading())
        {
            SkipDialog();
            return false;
        }

        (EventDialog, int) dialog = dialogManager.GetSanctuaryEventDialog(dialogId);
        EventDialog targetDialog = dialog.Item1;

        if (targetDialog == null)
        {
            return true;
        }

        SetPendingEventDialog(targetDialog);
        ShowEventDialogForSelfCamera(0);

        ChangeActionButtonVisibility(true);

        OnEventDialogComplete(dialog.Item2);
        return true;
    }

    public void OnDialogComplete()
    {
        // Do nothing
    }

    public void OnSelfCameraModeDialogComplete(bool needToRotateCamera)
    {
        ChangeActionButtonVisibility(false);
    }

    private void ChangeActionButtonVisibility(bool isOnEventDialog)
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].SetActive(!isOnEventDialog);
        }
        confirmButton.SetActive(isOnEventDialog);
    }

    private void OnEventDialogComplete(int open)
    {
        if (listener == null)
        {
            return;
        }

        switch (open)
        {
            case 0:
                listener.ObtainWarriorSkill(Player.WarriorSkill.SMITE);
                break;
            case 1:
                listener.IncreaseMagicianSpellCount();
                break;
        }
    }

    private void ShowNormalDialogWithNewSequence()
    {
        SanctuaryNormalDialog selectedDialog = normalDialogQueue.Peek();

        (DialogModel targetDialog, int nextSequence) = selectedDialog.GetCurrentDialog(0);
        if (nextSequence > 0)
        {
            pendingSequenceDialogId = selectedDialog.GetId();
            pendingSequence = nextSequence;
        }
        else
        {
            normalDialogQueue.Dequeue();
        }

        dialogManager.UseDialog(selectedDialog.GetId());
        ShowDialogForSelfCamera(new List<DialogModel>() { targetDialog });
    }

    private void ShuffleExcessiveList()
    {
        List<SanctuaryNormalDialog> tmpList = new List<SanctuaryNormalDialog>();
        tmpList.AddRange(excessiveDialogList);
        excessiveDialogList.Clear();

        int totalCount = tmpList.Count;
        for (int i = 0; i < totalCount; i++)
        {
            int random = Random.Range(0, tmpList.Count);
            excessiveDialogList.Add(tmpList[random]);
            tmpList.RemoveAt(random);
        }
    }

    private void Start()
    {
        SetDialogStatusListener(this);
    }
}
