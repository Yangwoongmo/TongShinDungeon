using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerInteractionController : 
    PlayerInteractionAreaStatusChangeObserver, 
    PlayerEventHandler, 
    IPlayerInteractionAvailableChecker, 
    ItemUseListener,
    ISettingMenuClickListener
{
    private const string BloodEffectAnimationKey = "bloodEffect";
    private const string DungeonFadeAnimationKey = "dungeon";
    private const string SanctuaryFadeAnimationKey = "sanctuary";
    private const string ItemObtainEffectAnimationKey = "itemObtain";
    private const string PlayerDeadAnimationKey = "playerDead";

    [SerializeField] private int stageId;
    [SerializeField] private PlayerInteractionMediator[] mediators;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator bloodEffectAnimator;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Text interactionButtonText;
    [SerializeField] private Animator sceneFadeAnimator;
    [SerializeField] private Image warriorHpUI;
    [SerializeField] private Image inventoryWarriorHpUI;
    [SerializeField] private BattlePrototypeSceneController battleController;
    [SerializeField] private Animator itemObtainEffectAnimator;
    [SerializeField] private Text itemObtainText;
    [SerializeField] private ExploreDialogController dialogController;
    [SerializeField] private InteractionObjectCountStorage[] storages;
    [SerializeField] private List<Sprite> hpBarSprites;
    [SerializeField] private Text hpPercentageText;
    [SerializeField] private Button selfCameraButton;
    [SerializeField] private ExploreHeaderController headerController;

    private bool canIgnoreTrap = false;
    private Player player;
    private PlayerHpController hpController;
    private bool isExploringMap = true;

    private float lastNoPaintAlertTime = -1;

    private Coroutine currentItemObtainCoroutine;

    private PlayerSelfCameraDialogGenerator dialogGenerator;

    // Properties for progress of game. This properties can be saved/loaded from local db later.
    private int mapProgress = 0;
    private int monsterKillPoint = 0;

    private Action<int> OnStageTransitionCallback;

    private PlayerManager manager = PlayerManager.GetInstance();
    private MapObjectStatusManager objectManager = MapObjectStatusManager.GetInstance();
    private EventDialogRepository eventDialogRepository = EventDialogRepository.GetInstance();
    private SanctuaryInfoRepository sanctuaryInfoRepository = SanctuaryInfoRepository.GetInstance();
    private StartPointDataManager startPointDataManager = StartPointDataManager.GetInstance();

    private ICurrentFloorProvider provider;

    public int GetStageId()
    {
        return stageId;
    }

    public void SetDialogStatusListener(IDialogStatusListener listener)
    {
        dialogController.SetDialogStatusListener(listener);
    }

    public void SetStageTransitionCallback(Action<int> callback)
    {
        OnStageTransitionCallback = callback;
    }

    public void GetDamage(int damage)
    {
        if (canIgnoreTrap)
        {
            return;
        }

        hpController.UpdateWarriorHp(damage * (-1));

        if (hpController.IsPlayerDead())
        {
            handlePlayerDeath();
        }

        StartCoroutine(PlayerDamageCoroutine());
        StartCoroutine(IgnoreTrapCoroutine(1.5f));

        dialogController.ShowAdventureDialog("I00");
    }

    public override void OnPlayerInInteractionAreaStatusChanged(DoInteraction interaction)
    {
        interaction(this);
    }

    public void ToggleInteractionObjects(bool isSelfCameraMode, int id)
    {
        mediators[id].SendPlayerScreenModeChangeEvent(isSelfCameraMode);
    }

    public void InteractWithObject(InteractionObject interaction)
    {
        interaction.DoInteraction(this);
    }

    public void ObtainItem(InventoryItem item)
    {
        if (currentItemObtainCoroutine != null)
        {
            StopCoroutine(currentItemObtainCoroutine);
            itemObtainEffectAnimator.gameObject.SetActive(false);
        }
        
        itemObtainText.text = item.GetItemName() + " 획득";
        currentItemObtainCoroutine = StartCoroutine(ItemObatainEffectCoroutine());
        player.ObtainItem(item);

        SetNeedToShowEventDialogByItem(item.GetItemId());
    }

    public void ObtainGold(int gold)
    {
        if (currentItemObtainCoroutine != null)
        {
            StopCoroutine(currentItemObtainCoroutine);
            itemObtainEffectAnimator.gameObject.SetActive(false);
        }
        
        itemObtainText.text = string.Format("{0:#,###}", gold) + "G 획득";
        currentItemObtainCoroutine = StartCoroutine(ItemObatainEffectCoroutine());
        player.ObtainGold(gold);
    }

    public void EnableSelfCameraButton(bool isEnable)
    {
        selfCameraButton.interactable = isEnable;
    }

    public void EnableSettingMenuButton(bool isEnable)
    {
        headerController.SetHeaderVisibility(isEnable);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        battleController.SetPlayer(player);
    }

    public void SetCurrentFloorProvider(ICurrentFloorProvider provider)
    {
        this.provider = provider;
    }

    public void MoveToTargetScene(string targetSceneName, PlayerEventHandler.PortalType type, int portalId)
    {
        startPointDataManager.SetNeedToSaveAfterTransitionDone(true);

        switch (type)
        {
            case PlayerEventHandler.PortalType.DUNGEON:
                sanctuaryInfoRepository.SaveStartPortalId(portalId);
                sceneFadeAnimator.SetBool(DungeonFadeAnimationKey, true);
                break;
            case PlayerEventHandler.PortalType.SANCTUARY:
                manager.SavePlayerData();
                objectManager.SaveObjectChangeState();
                sanctuaryInfoRepository.SaveLatestEnteredPortalId(stageId, portalId);
                startPointDataManager.ClearStartFloorData();
                sceneFadeAnimator.SetBool(SanctuaryFadeAnimationKey, true);
                break;
            default:
                return;
        }

        StartCoroutine(SceneTransitionCoroutine(targetSceneName));
    }

    public void MoveToNextSubStage(int portalId)
    {
        StartCoroutine(StageTransitionCoroutine(portalId));
    }

    public bool CanUnlockDoor()
    {
        // Maybe there can be special condition(like key or item?) to unlock door
        return true;
    }

    public bool CanMarkWall()
    {
        return stageId > 0;
    }

    public void CheckForwardInteractionObeject(InteractionObject interaction)
    {
        string message = interaction.GetInteractionMessage(this);

        if (message != null)
        {
            interactionButton.enabled = true;
            interactionButton.onClick.RemoveAllListeners();
            interactionButton.onClick.AddListener(() => { 
                InteractWithObject(interaction);
                CheckForwardInteractionObeject(interaction);
            });
            interactionButtonText.text = message;
        }
        else
        {
            ResetInteractionButton();
        }
    }

    public void ResetInteractionButton()
    {
        interactionButton.enabled = false;
        interactionButton.onClick.RemoveAllListeners();
        interactionButtonText.text = "상호작용";
    }

    public bool CheckForwardPortal(Wall wall)
    {
        if (wall != null && wall is Portal)
        {
            return (wall as Portal).MoveToTargetArea(this);
        }

        return false;
    }

    public void EncountMonster(Monster monster)
    {
        battleController.PrepareBattle(monster);
    }

    public void ChangeCamera(GameObject camera)
    {
        battleController.OnChangeCameraClick();
    }

    public bool IsMonsterDead()
    {
        return battleController.IsMonsterDead();
    }

    public bool IsPlayerDead()
    {
        return hpController.IsPlayerDead();
    }

    public void FinishBattleWithMonsterDeath()
    {
        WarriorBattleStatistic statistic = battleController.GetWarriorBattleStatistic();
        int monsterId = battleController.GetMonsterId();

        battleController.FinishBattleWithMonsterDeath();

        string dialogId = null;
        float hitPercentage = (float)statistic.GetTotalHitDamage() / (float)hpController.GetWarriorMaxHp();

        if (statistic.GetTotalHitCount() == 0)
        {
            dialogId = "F00";
        }
        else if (hitPercentage >= 0.5)
        {
            dialogId = "F01";
        }
        else if (monsterId == 2)
        {
            float probability = Random.Range(0f, 1f);
            if (probability <= 0.1f)
            {
                dialogId = "F10";
            }
        } else if (monsterId == 1)
        {
            dialogId = "F90";
        }

        if (dialogId != null)
        {
            dialogController.ShowAdventureDialog(dialogId);
        }
    }

    public void FinishBattleWithPlayerDeath()
    {
        battleController.FinishBattleWithPlayerDeath();
        handlePlayerDeath();
    }

    public void FinishBattleWithBlink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    public void SetExploringMap(bool isExploringMap)
    {
        this.isExploringMap = isExploringMap;
    }

    public bool IsMagicianCameraMode()
    {
        return battleController.IsMagicianCameraMode();
    }

    public bool IsBlinked()
    {
        return battleController.IsBlinked();
    }

    public bool IsBattleFinished()
    {
        return IsMonsterDead() || IsPlayerDead() || IsBlinked();
    }

    public void PlayAnimationAfterBlink()
    {
        battleController.AppearButtonsAfterBlink();
        StartCoroutine(PlayerBlinkAnimationCoroutine());
    }

    private void handlePlayerDeath()
    {
        manager.SavePlayerData();
        objectManager.SaveObjectChangeState();
        startPointDataManager.ClearStartFloorData();
        sanctuaryInfoRepository.IncreaseDeathCount();
        sanctuaryInfoRepository.SetIsEnterByDeath();

        sceneFadeAnimator.SetBool(PlayerDeadAnimationKey, true);

        if (stageId > 0)
        {
            StartCoroutine(SceneTransitionCoroutine("SanctuaryScene", false, true));
        }
        else
        {
            StartCoroutine(SceneTransitionCoroutine("TutorialScene", true, true));
        }
    }

    public void HealPlayer(int heal, bool isPercentage, bool withCoefficient)
    {
        int healAmount = isPercentage ? Mathf.RoundToInt((float)player.GetWarriorMaxHp() * heal / 100) : heal;
        int healAmountWithCoefficent = withCoefficient ? Mathf.RoundToInt((float)healAmount * player.GetHealCoefficient()) : healAmount;
        hpController.UpdateWarriorHp(healAmountWithCoefficent);
    }

    public bool UsePaint()
    {
        if (HasPaint())
        {
            player.UseItem(new Paint());

            if (!HasPaint())
            {
                lastNoPaintAlertTime = -1;
            } 
            return true;
        }

        float currentTime = Time.time;
        if (lastNoPaintAlertTime < 0 || currentTime - lastNoPaintAlertTime > 15)
        {
            lastNoPaintAlertTime = currentTime;
            dialogController.ShowAdventureDialog("B02");
        }
        
        return false;
    }

    public void ShowEventDialog(string dialogId)
    {
        dialogController.ShowEventDialog(dialogId);
    }

    public void ShowAdventureDialog(string dialogId, int dialogIndex = 0)
    {
        dialogController.ShowAdventureDialog(dialogId, dialogIndex);
    }

    public void UpdateMapProgress()
    {
        mapProgress++;
        sanctuaryInfoRepository.IncreaseProgress();
    }

    public void SaveDataOnPurify(int portalId)
    {
        manager.SavePlayerData();
        objectManager.SaveObjectChangeState();
        sanctuaryInfoRepository.SavePurifiedPortal(stageId, portalId);
    }

    public void ShowDialogForSelfCameraMode(bool isForWarrior, int id)
    {
        if (isForWarrior)
        {
            dialogController.ShowDialogForSelfCamera(dialogGenerator.GenerateWarriorDialog(storages[id]), true);
        }
        else
        {
            dialogController.ShowDialogForSelfCamera(dialogGenerator.GenerateMagicianDialog(storages[id]), true);
        }
    }

    public void ShowDialogForSelfCameraModeForceChange(int moduleId = 1, string dialogId = null)
    {
        dialogController.ShowEventDialogForSelfCamera(moduleId, dialogId);
    }

    public void ShowSelfCameraEnterDialog()
    {
        dialogController.ShowSelfCameraEnterDialog(mapProgress, stageId, hpController.GetWarriorHpPercentage(), monsterKillPoint);
    }

    public bool IsDialogWaitingOrFading()
    {
        return dialogController.IsWaitingOrFading();
    }

    public void HideDialog()
    {
        dialogController.HideDialog();
    }

    public void SkipDialog()
    {
        dialogController.SkipDialog();
    }

    public void SetPoseAndFaceForEnterance()
    {
        dialogController.SetPoseAndFaceForEnteranceWithoutDialog(hpController.GetWarriorHpPercentage());
    }

    public void GoToSanctuary()
    {
        MoveToTargetScene("SanctuaryScene", PlayerEventHandler.PortalType.SANCTUARY, -1);
    }

    public void SetMediatorAndStorage(GameObject mapRoot)
    {
#if UNITY_EDITOR
        mediators = new PlayerInteractionMediator[1];
        mediators[0] = mapRoot.GetComponent<PlayerInteractionMediator>();

        storages = new InteractionObjectCountStorage[1];
        storages[0] = mapRoot.GetComponent<InteractionObjectCountStorage>();
#endif
    }

    public void OnClickOpenSettingMenu()
    {
        ToggleInteractionObjects(true, 0);
    }

    public void OnClickCloseSettingMenu()
    {
        ToggleInteractionObjects(false, 0);
    }

    public void OnClickSaveData()
    {
        manager.SavePlayerData();
        objectManager.SaveObjectChangeState();

        startPointDataManager.SaveCurrentFloorAndBlinkPoint(
            stageId, 
            provider.GetCurrentFloor(), 
            provider.GetCurrentBlinkPoint()
        );

        headerController.StartSaveTextFadeAnimation();
    }

    public void OnClickSaveDataAndQuit()
    {
        OnClickSaveData();
        Application.Quit();
    }

    private void Start()
    {
        for (int i = 0; i < mediators.Length; i++)
        {
            mediators[i].SetPlayerAreaStatusObserver(this);
        }

        hpController = new PlayerHpController(player, warriorHpUI, inventoryWarriorHpUI, hpBarSprites, hpPercentageText);
        battleController.SetWarriorStatusObserver(hpController);

        dialogGenerator = new PlayerSelfCameraDialogGenerator();

        eventDialogRepository.SetNeedToShowDilaog("T0001", CharacterType.MERCHANT);
    }

    private bool HasPaint()
    {
        // ItemId of Paint is 6
        // If there are more cases for using itemId directly,
        // we can make enum or static function for ItemId of each items
        return player.HasItemInInventory(6);
    }

    private void SetNeedToShowEventDialogByItem(int itemId)
    {
        switch (itemId)
        {
            case 104:
                eventDialogRepository.SetNeedToShowDilaog("W0001", CharacterType.WARRIOR);
                break;
            case 105:
                eventDialogRepository.SetNeedToShowDilaog("M0001", CharacterType.MAGICIAN);
                break;
            default:
                break;
        }
    } 

    private IEnumerator PlayerDamageCoroutine()
    {
        playerAnimator.SetBool(CameraAnimationKeyConst.VibrationAnimationKey, true);
        bloodEffectAnimator.SetBool(BloodEffectAnimationKey, true);
        yield return new WaitForSeconds(0.3f);
        playerAnimator.SetBool(CameraAnimationKeyConst.VibrationAnimationKey, false);
        bloodEffectAnimator.SetBool(BloodEffectAnimationKey, false);
    }

    private IEnumerator IgnoreTrapCoroutine(float remainingTime)
    {
        canIgnoreTrap = true;
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => isExploringMap);

            remainingTime -= 0.5f;
        }
        
        canIgnoreTrap = false;
    }

    private IEnumerator SceneTransitionCoroutine(string targetSceneName, bool restoreBeforeTransition = false, bool isPlayerDead = false)
    {
        float time = isPlayerDead ? 1.0f : 0.6f;
        yield return new WaitForSeconds(time);

        if (restoreBeforeTransition)
        {
            hpController.RestorePlayerFullHp();
            manager.SavePlayerData();
        }
        SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator StageTransitionCoroutine(int portalId)
    {
        sceneFadeAnimator.SetBool(DungeonFadeAnimationKey, true);
        yield return new WaitForSeconds(0.6f);

        OnStageTransitionCallback.Invoke(portalId);
        sceneFadeAnimator.SetBool(DungeonFadeAnimationKey, false);
    }

    private IEnumerator BlinkCoroutine()
    {
        yield return new WaitForSeconds(1f);
        battleController.FinishBattleWithBlink();
    }

    private IEnumerator PlayerBlinkAnimationCoroutine()
    {
        playerAnimator.SetBool(CameraAnimationKeyConst.BlinkAnimationKey, true);
        playerAnimator.SetBool(CameraAnimationKeyConst.WarriorCameraTransitionAnimationKey, false);
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool(CameraAnimationKeyConst.BlinkAnimationKey, false);
        playerAnimator.SetBool(CameraAnimationKeyConst.BattleCameraTransitionAnimationKey, false);
        playerAnimator.SetBool(CameraAnimationKeyConst.MagicianCameraTransitionAnimationKey, false);
    }

    private IEnumerator ItemObatainEffectCoroutine()
    {
        itemObtainEffectAnimator.gameObject.SetActive(true);
        itemObtainEffectAnimator.SetBool(ItemObtainEffectAnimationKey, true);

        yield return new WaitForSeconds(1.2f);

        itemObtainEffectAnimator.SetBool(ItemObtainEffectAnimationKey, false);
        itemObtainEffectAnimator.gameObject.SetActive(false);
    }
}
