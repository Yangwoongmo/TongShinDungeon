using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ExploreDialogController : MonoBehaviour
{
    private const string DialogFadeOutKey = "dialogFadeOut";

    private readonly DialogBubbleType warriorSelfCameraBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND },
            DialogBubbleColorType.CHARACTER,
            DialogBubbleArrowType.LEFT,
            DialogBubblePositionAlignType.LEFT_TOP
        );

    private readonly DialogBubbleType magicianSelfCameraBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND },
            DialogBubbleColorType.CHARACTER,
            DialogBubbleArrowType.RIGHT,
            DialogBubblePositionAlignType.RIGHT_TOP
        );

    private readonly DialogBubbleType exploreBubbleType =
        new DialogBubbleType(
            new DialogBubbleCornerType[4] { DialogBubbleCornerType.ROUND, DialogBubbleCornerType.ROUND, DialogBubbleCornerType.RECTANGLE, DialogBubbleCornerType.ROUND },
            DialogBubbleColorType.CHARACTER,
            DialogBubbleArrowType.NONE,
            DialogBubblePositionAlignType.LEFT_BOTTOM
        );

    // Temporary position data
    private readonly Vector2 warriorSelfCameraBubblePosition = new Vector2(-545, 953);
    private readonly Vector2 magicianSelfCameraBubblePosition = new Vector2(545, 953);
    private readonly Vector2 exploreBubblePosition = new Vector2(-515, -400f);

    // For explore mode.
    [SerializeField] private Animator dialogFadeOut;
    [SerializeField] public GameObject exploreDialogUI;
    [SerializeField] private DialogBubble dialogBubble;
    [SerializeField] private Canvas exploreDialogCanvas;

    [SerializeField] protected GameObject confirmButton;
    [SerializeField] private CharacterPoseController poseController;
    [SerializeField] private Button[] selectionButtons;

    [SerializeField] private GameObject[] eventDialogButtonList;
    [SerializeField] private GameObject dialogHistoryUI;
    [SerializeField] private Transform dialogHistoryList;
    [SerializeField] private GameObject dialogHistoryListPrefab;

    protected virtual DialogSceneType sceneType => DialogSceneType.DUNGEON;

    private IDialogStatusListener listener;
    private bool isSkipped = false;
    private bool isWaitingOrFading = true;

    private DialogFileAndStatusManager dialogManager = DialogFileAndStatusManager.GetInstance();

    private List<EnteranceDialog> entranceDialogList = null;

    private EventDialog pendingEventDialog = null;

    private List<DialogHistoryModel> eventDialogHistoryQueue = new List<DialogHistoryModel>();

    private IEnumerator currentEventDialogTypingCoroutine = null;

    public void SetDialogStatusListener(IDialogStatusListener listener)
    {
        this.listener = listener;
    }

    public void SkipDialog()
    {
        isSkipped = true;
    }

    public void HideDialog()
    {
        isWaitingOrFading = true;
        isSkipped = false;
        exploreDialogUI.SetActive(false);
        StopAllCoroutines();

        dialogBubble.HideDialog();

        dialogFadeOut.SetBool(DialogFadeOutKey, false);
    }

    public void ShowAdventureDialog(string id, int dialogIndex = 0)
    {
        DialogModel dialog = dialogManager.GetTargetAdventureDialog(id, dialogIndex);

        if (dialog == null)
        {
            return;
        }

        ShowDialog(dialog);
    }

    public void ShowDialogForSelfCamera(List<DialogModel> dialogList, bool needConfirmButton = false)
    {
        SetupSelfCameraUiAndFace(dialogList[0]);
        if (needConfirmButton)
        {
            confirmButton.SetActive(true);
        }
        StartCoroutine(TypeDialogTextForSelfCameraCoroutine(dialogList));
    }

    public void ShowEventDialogForSelfCamera(int moduleId, string dialogId = null)
    {
        if (dialogId != null && pendingEventDialog == null)
        {
            pendingEventDialog = FindEventDialogById(dialogId);
        }

        if (pendingEventDialog == null)
        {
            return;
        }

        SetupSelfCameraUiAndFace(pendingEventDialog.GetNextDialogModule(moduleId)[0]);
        currentEventDialogTypingCoroutine = TypeDialogTextForSelfCameraCoroutine(pendingEventDialog.GetNextDialogModule(moduleId), 0f, () =>
        {
            ShowNextSelectionOrFinish(moduleId);
        }, true);
        StartCoroutine(currentEventDialogTypingCoroutine);
        SetEventDialogButtonVisibility(true);
    }

    public void ShowSelfCameraEnterDialog(int progress, int stageId, float playerHpPercentage, int monsterKill)
    {
        if (entranceDialogList == null)
        {
            LoadEntranceDialogList();
            if (entranceDialogList == null)
            {
                return;
            }
        }

        List<EnteranceDialog> dialogCandidates = new List<EnteranceDialog>();
        int hpPoint = GetHpPoint(playerHpPercentage);
        int monsterKillPoint = GetMonsterKillPoint(monsterKill);
        int maxUseCount = int.MaxValue;

        for (int i = 0; i < entranceDialogList.Count; i++)
        {
            EnteranceDialog dialog = entranceDialogList[i];
            if (dialog.CanUseDialog(progress, stageId, hpPoint, monsterKillPoint))
            {
                int useCount = dialogManager.GetDialogUseCount(dialog.GetId());
                if (maxUseCount < useCount)
                {
                    continue;
                }
                
                if (maxUseCount > useCount)
                {
                    maxUseCount = useCount;
                    dialogCandidates.Clear();
                }

                dialogCandidates.Add(dialog);
            }
        }

        if (dialogCandidates.Count == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, dialogCandidates.Count);
        EnteranceDialog selectedDialog = dialogCandidates[randomIndex];

        dialogManager.UseDialog(selectedDialog.GetId());
        List<DialogModel> targetDialog = new List<DialogModel>() { selectedDialog.GetDialog() };

        if (poseController != null)
        {
            poseController.SetCameraHolder(selectedDialog.GetCameraHolder());
        }
        ShowDialogForSelfCamera(targetDialog);
    }

    public void ShowEventDialog(string id)
    {
        EventDialog targetDialog = FindEventDialogById(id);

        if (targetDialog == null)
        {
            return;
        }

        pendingEventDialog = targetDialog;
        
        ShowDialog(targetDialog.GetFirstDialog(), false);
    }

    public bool IsWaitingOrFading()
    {
        return isWaitingOrFading;
    }

    public void SetPoseAndFaceForEnteranceWithoutDialog(float hpPercentage)
    {
        if (poseController == null)
        {
            return;
        }

        int random = UnityEngine.Random.Range(0, 2);
        poseController.SetCameraHolder(random == 0 ? CharacterType.WARRIOR : CharacterType.MAGICIAN);

        if (hpPercentage > 0.95)
        {
            poseController.SetPoseAndFace(2, 1, 2, 1);
        }
        else if (hpPercentage >= 0.8)
        {
            poseController.SetPoseAndFace(1, 1, 1, 1);
        }
        else if (hpPercentage >= 0.3)
        {
            poseController.SetPoseAndFace(3, 2, 3, 2);
        }
        else
        {
            poseController.SetPoseAndFace(4, 3, 4, 3);
        }
    }

    public void ShowDialogHistory()
    {
        if (currentEventDialogTypingCoroutine != null)
        {
            StopCoroutine(currentEventDialogTypingCoroutine);
        }

        dialogHistoryUI.SetActive(true);
        exploreDialogCanvas.enabled = false;

        for (int i = 0; i < eventDialogHistoryQueue.Count; i++)
        {
            if (dialogHistoryList.childCount <= i)
            {
                Instantiate(dialogHistoryListPrefab, dialogHistoryList);
            }

            DialogHistoryList historyList = dialogHistoryList.GetChild(i).GetComponent<DialogHistoryList>();
            historyList.gameObject.SetActive(true);
            historyList.CreateDialogHistoryList(eventDialogHistoryQueue[i]);
        }

        RectTransform scrollRectTransform = dialogHistoryList.GetComponent<RectTransform>();

        // To caculate exact height of scroll area
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectTransform);
        float currentScrollX = scrollRectTransform.anchoredPosition.x;
        scrollRectTransform.anchoredPosition = new Vector3(currentScrollX, scrollRectTransform.rect.height, 0);
    }

    public void HideDialogHistory()
    {
        dialogHistoryUI.SetActive(false);
        for (int i = 0; i < dialogHistoryList.childCount; i++)
        {
            DialogHistoryList historyList = dialogHistoryList.GetChild(i).GetComponent<DialogHistoryList>();
            historyList.HideDialogHistoryList();
        }

        exploreDialogCanvas.enabled = true;
        if (currentEventDialogTypingCoroutine != null)
        {
            StartCoroutine(currentEventDialogTypingCoroutine);
        }
    }

    protected void SetPendingEventDialog(EventDialog dialog)
    {
        this.pendingEventDialog = dialog;
    }

    private void ShowDialog(DialogModel dialog, bool needToFadeOut = true)
    {
        HideDialog();

        exploreDialogUI.SetActive(true);
        dialogBubble.ShowDialog(dialog.GetMessage(), exploreBubbleType, exploreBubblePosition);

        isWaitingOrFading = false;

        StartCoroutine(DialogTypingCoroutine(dialog.GetMessage(), needToFadeOut));
    }

    private void SetupSelfCameraUiAndFace(DialogModel firstDialog)
    {
        HideDialog();
        exploreDialogUI.SetActive(true);
        if (poseController != null)
        {
            poseController.SetPoseAndFace(
                firstDialog.GetWarriorFaceType(),
                firstDialog.GetWarriorPoseType(),
                firstDialog.GetMagicianFaceType(),
                firstDialog.GetMagicianPoseType()
            );
        }
        isWaitingOrFading = false;
    }

    private void ClearSelectionButtons()
    {
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitingForNextDialogModuleCoroutine(int moduleId)
    {
        for (int n = 0; n < 200; n++)
        {
            if (isSkipped)
            {
                isSkipped = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        currentEventDialogTypingCoroutine = TypeDialogTextForSelfCameraCoroutine(pendingEventDialog.GetNextDialogModule(moduleId), 0f, () => { 
            ShowNextSelectionOrFinish(moduleId); 
        }, true);
        StartCoroutine(currentEventDialogTypingCoroutine);
    }

    private void ShowNextSelectionOrFinish(int moduleId)
    {
        EventDialogSelection selection = pendingEventDialog.GetNextSelection("module" + moduleId);
        if (selection == null)
        {
            StartCoroutine(EventDialogCompleteCoroutine(true));
            return;
        }

        switch (selection.GetSelectionType())
        {
            case EventDialogSelection.SelectionType.CONT:
                SetEventDialogButtonVisibility(true);
                int targetModuleId = int.Parse(selection.GetOptions()[0].Item2[1].ToString());
                StartCoroutine(WaitingForNextDialogModuleCoroutine(targetModuleId));
                break;
            case EventDialogSelection.SelectionType.OPTION:
                SetEventDialogButtonVisibility(false);
                StartCoroutine(ShowCurrentSelectionCoroutine(selection));
                break;
        }
    }

    private IEnumerator ShowCurrentSelectionCoroutine(EventDialogSelection selection)
    {
        yield return new WaitForSeconds(1f);

        List<(string, string)> options = selection.GetOptions();
        for (int i = 0; i < options.Count; i++)
        {
            (string, string) option = options[i];
            selectionButtons[i].transform.GetChild(0).GetComponent<Text>().text = option.Item1;
            selectionButtons[i].onClick.RemoveAllListeners();
            selectionButtons[i].onClick.AddListener(() =>
            {
                ClearSelectionButtons();
                enqueueDialogHistory(CharacterType.PLAYER, option.Item1);

                string nextTarget = option.Item2;
                if (nextTarget[0] == 'm')
                {
                    int targetModuleId = int.Parse(nextTarget[1].ToString());
                    currentEventDialogTypingCoroutine = TypeDialogTextForSelfCameraCoroutine(pendingEventDialog.GetNextDialogModule(targetModuleId), 1f, () => {
                        ShowNextSelectionOrFinish(targetModuleId);
                    }, true);
                    StartCoroutine(currentEventDialogTypingCoroutine);
                    SetEventDialogButtonVisibility(true);
                }
                else
                {
                    char targetSelectionId = nextTarget[1];
                    StartCoroutine(ShowCurrentSelectionCoroutine(pendingEventDialog.GetNextSelection("selection" + targetSelectionId)));
                }
            });
            selectionButtons[i].gameObject.SetActive(true);
        }
    }

    private IEnumerator TypeDialogTextForSelfCameraCoroutine(
        List<DialogModel> dialogList, 
        float delayTime = 0f, 
        Action onModuleFinishCallback = null,
        bool needToStoreHistory = false
    )
    {
        yield return new WaitForSeconds(0.15f + delayTime);

        StringBuilder builder = new StringBuilder();
        isSkipped = false;
        for (int i = 0; i < dialogList.Count; i++)
        {
            if (poseController != null)
            {
                poseController.SetPoseAndFace(
                    dialogList[i].GetWarriorFaceType(), 
                    dialogList[i].GetWarriorPoseType(), 
                    dialogList[i].GetMagicianFaceType(), 
                    dialogList[i].GetMagicianPoseType()
                );
            }
            dialogBubble.HideDialog();

            bool isSpeakerWarrior = IsSpeakerWarrior(dialogList[i]);
            string message = dialogList[i].GetMessage();

            // TODO: Adjust dialog bubble type and position according to sceneType
            if (isSpeakerWarrior)
            {
                dialogBubble.ShowDialog(message, warriorSelfCameraBubbleType, warriorSelfCameraBubblePosition);
            }
            else
            {
                dialogBubble.ShowDialog(message, magicianSelfCameraBubbleType, magicianSelfCameraBubblePosition);
            }

            if (needToStoreHistory)
            {
                enqueueDialogHistory(dialogList[i].GetSpeaker(), message);
            }

            builder.Clear();

            float delay = dialogList[i].GetDelayPerOneCharacter() > 0 ? dialogList[i].GetDelayPerOneCharacter() : 0.1f;

            for (int k = 0; k < message.Length; k++)
            {
                builder.Append(message[k]);
                dialogBubble.SetShowingDialogText(builder.ToString());
                int iterateCount = isSkipped ? 1 : Mathf.RoundToInt(delay / 0.01f);

                for (int n = 0; n < iterateCount; n++)
                {
                    yield return new WaitForFixedUpdate();
                    if (isSkipped)
                    {
                        break;
                    }
                }
            }

            isSkipped = false;

            for (int n = 0; n < 200; n++)
            {
                if (isSkipped)
                {
                    isSkipped = false;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        if (onModuleFinishCallback != null)
        {
            onModuleFinishCallback.Invoke();
            currentEventDialogTypingCoroutine = null;
            yield break;
        }
        
        if (listener != null)
        {
            listener.OnSelfCameraModeDialogComplete();
        }

        StartCoroutine(EventDialogCompleteCoroutine());
    }

    private void enqueueDialogHistory(CharacterType characterType, string message)
    {
        DialogHistoryModel.DialogSpeakerType speakerType;
        switch (characterType)
        {
            case CharacterType.WARRIOR:
                speakerType = DialogHistoryModel.DialogSpeakerType.WARRIOR;
                break;
            case CharacterType.MAGICIAN:
                speakerType = DialogHistoryModel.DialogSpeakerType.MAGICIAN;
                break;
            case CharacterType.PLAYER:
                speakerType = DialogHistoryModel.DialogSpeakerType.PLAYER;
                break;
            default:
                speakerType = DialogHistoryModel.DialogSpeakerType.WARRIOR;
                break;
        }

        int currentHistoryCount = eventDialogHistoryQueue.Count;
        if (currentHistoryCount == 0 || !eventDialogHistoryQueue[currentHistoryCount - 1].AddDialogHistoryIfPossible(speakerType, message))
        {
            DialogHistoryModel history = new DialogHistoryModel(speakerType, new List<string>() { message });
            eventDialogHistoryQueue.Add(history);
        }
    }

    private IEnumerator EventDialogCompleteCoroutine(bool needToRotateCamera = false)
    {
        isWaitingOrFading = true;
        SetEventDialogButtonVisibility(false);

        pendingEventDialog = null;
        currentEventDialogTypingCoroutine = null;
        eventDialogHistoryQueue.Clear();

        if (listener != null)
        {
            listener.OnSelfCameraModeDialogComplete(needToRotateCamera);
        }

        dialogFadeOut.SetBool(DialogFadeOutKey, true);
        yield return new WaitForSeconds(0.5f);
        dialogFadeOut.SetBool(DialogFadeOutKey, false);

        HideDialog();
    }

    private IEnumerator DialogTypingCoroutine(string str, bool needToFadeOut)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= str.Length; i++)
        {
            dialogBubble.SetShowingDialogText(str.Substring(0, i));
            yield return new WaitForSeconds(0.1f);
        }

        if (!needToFadeOut)
        {
            listener.OnDialogComplete();
            yield break;
        }

        isWaitingOrFading = true;
        yield return new WaitForSeconds(1.0f);
        dialogFadeOut.SetBool(DialogFadeOutKey, true);
        yield return new WaitForSeconds(0.5f);
        dialogFadeOut.SetBool(DialogFadeOutKey, false);
        HideDialog();
    }

    private bool IsSpeakerWarrior(DialogModel dialog)
    {
        return dialog.GetSpeaker() == CharacterType.WARRIOR;
    }

    private int GetHpPoint(float hpPercentage)
    {
        if (hpPercentage > 0.95)
        {
            return 1;
        } 
        else if (hpPercentage >= 0.8)
        {
            return 2;
        }
        else if (hpPercentage >= 0.3)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    private int GetMonsterKillPoint(int monsterKill)
    {
        if (monsterKill > 10)
        {
            return 4;
        }
        else if (monsterKill > 5)
        {
            return 3;
        }
        else if (monsterKill > 0)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private void SetEventDialogButtonVisibility(bool isVisible)
    {
        for (int i = 0; i < eventDialogButtonList.Length; i++)
        {
            eventDialogButtonList[i].SetActive(isVisible);
        }
    }

    private void LoadEntranceDialogList()
    {
        entranceDialogList = dialogManager.GetSelfCameraEnteranceDialogList();
    }

    private void Start()
    {
        LoadEntranceDialogList();
    }

    private EventDialog FindEventDialogById(string id)
    {
        EventDialog targetDialog = dialogManager.GetEventDialog(id, "Event");

        if (targetDialog == null)
        {
            return null;
        }

        if (poseController != null)
        {
            poseController.SetCameraHolder(targetDialog.GetCameraHolder());
        }

        return targetDialog;
    }

    protected enum DialogSceneType
    {
        DUNGEON,
        SANCTUARY
    }
}
