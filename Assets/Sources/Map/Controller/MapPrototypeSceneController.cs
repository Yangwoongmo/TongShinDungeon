using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MapPrototypeSceneController : 
    MonoBehaviour, 
    IPlayerButtonClickListener, 
    IDialogStatusListener,
    ICurrentFloorProvider,
    IStartFloorChangeObserver
{
    private const string CameraModeChangeAnimationKey = "cameraModeChange";
    private readonly float[] blinkPointRotationYByDirection = new float[4] { -90f, 0f, 90f, 180f };

    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject frontCameraUIButton;
    [SerializeField] private GameObject selfCameraUIButton;
    [SerializeField] private GameObject warriorChractorSprite;
    [SerializeField] private GameObject magicianChractorSprite;
    [SerializeField] private GameObject cameraChangeEffectObject;
    [SerializeField] private BlinkPoint startFloor;
    [SerializeField] private PlayerInteractionController playerInteractionController;
    [SerializeField] private Animator selfCameraChangeAnimator;
    // Blocking all kind of UI input
    [SerializeField] private GameObject dimLayer;
    [SerializeField] private Animator playerCameraAnimator;
    [SerializeField] private Direction startDirection;
    [SerializeField] private InventoryController inventoryController;

    [SerializeField] private SubStageData[] subStageDataList;
    [SerializeField] private Transform mapParent;
    [SerializeField] private GameObject mapDecorations;
    [SerializeField] private BlinkPoint[] portalPoints;
    [SerializeField] private int[] portalSubStageIds;

    [SerializeField] private ExploreHeaderController headerController;
    [SerializeField] private TutorialExploreController tutorialController;

    [SerializeField] private GameObject[] visibilityChangeTargetViewListForDialog;

    [SerializeField] private StartFloorChangeMediator startFloorMediator;

    private SanctuaryInfoRepository sanctuaryInfoRepository = SanctuaryInfoRepository.GetInstance();
    private Player player;
    private Floor currentFloor;
    private Vector3 targetPosition = Vector3.zero;
    private Coroutine currentMovingCoroutine;
    private float playerCameraXRotation = CameraTransformConst.CameraXRotation;
    private float playerMovingSpeed = 0.02f;
    private float playerRotateSpeed = 0.033f;
    private float currentLerpDistance = 0.0f;
    private float smoothCameraTransitionSpeed = 0.033f;
    private float inputRotateAngle = 0;

    //private bool isMoving = false;
    private bool fowardIsDown = false;
    private bool rotateIsDown = false;
    private bool isSelfCameraMode = false;

    private bool isBattle = false;
    private BlinkPoint currentBlinkPoint;

    private bool forceCameraChange = false;

    private Vector3 defaultFrontCameraRotation;
    private Vector3 battleWarriorCameraRotation;

    private int currentSubStageId = 0;

    private StartPointDataManager startPointManager = StartPointDataManager.GetInstance();

    /*
        현재 축 방향을 수정하지 않고 캐릭터 방향을 수정한 상태라. z+축(foward) : 왼쪽, x+축(right) : 앞
    */

    public void OnForwardDown()
    {
        fowardIsDown = true;

        if (currentMovingCoroutine != null)
        {
            return;
        }
        playerInteractionController.ResetInteractionButton();
        currentLerpDistance = 0.0f;

        int direction = (int)player.GetDirection();
        Wall wall = currentFloor.GetWall(direction);

        if (!CheckPositionValidity(direction, wall))
        {
            if (wall != null)
            {
                InteractionObject interaction = wall.GetInteractionObject();
                if (interaction != null && interaction.IsTrap())
                {
                    playerInteractionController.InteractWithObject(interaction);
                }
                else if (wall is Door && (wall as Door).IsDoorLocked())
                {
                    playerInteractionController.ShowAdventureDialog("I01");
                }
                else if (wall is IronBar)
                {
                    playerInteractionController.ShowAdventureDialog("I02");
                }
            }
            currentMovingCoroutine = StartCoroutine(ComebackMovingCoroutine());
        }
        else
        {
            Floor nextFloor = currentFloor.GetFloor(direction);
            Vector3 nextPosition = nextFloor.gameObject.transform.localPosition;
            targetPosition = new Vector3(nextPosition.x, playerObject.transform.localPosition.y, nextPosition.z);
            currentMovingCoroutine = StartCoroutine(FowardMovingCoroutine());
            if (wall != null && wall is Door)
            {
                (wall as Door).OpenDoor();
            }
            currentFloor = nextFloor;
        }
    }

    public void OnForwardUp()
    {
        fowardIsDown = false;
    }

    public void OnRotateDown(float angle)
    {
        rotateIsDown = true;
        inputRotateAngle = angle;

        if (currentMovingCoroutine != null)
        {
            return;
        }
        playerInteractionController.ResetInteractionButton();
        currentLerpDistance = 0.0f;

        float currentAngle = playerObject.transform.localEulerAngles.y;

        currentMovingCoroutine = StartCoroutine(RotateMovingCoroutine(inputRotateAngle));
        int currentDirection = (int)player.GetDirection();
        int nextDirection = currentDirection;
        if (angle > 0)
        {
            nextDirection = currentDirection + 1;
            if (nextDirection > 3)
            {
                nextDirection = 0;
            }
        }
        else
        {
            nextDirection = currentDirection - 1;
            if (nextDirection < 0)
            {
                nextDirection = 3;
            }
        }
        player.SetDirection((Direction)nextDirection);
    }

    public void OnRotateUp()
    {
        rotateIsDown = false;
        inputRotateAngle = 0;
    }

    public void OnPlayerCameraChangeClick(bool isForTutorialOpening = false)
    {
        if (IsPlayerMoving())
        {
            return;
        }

        if (isBattle)
        {
            playerInteractionController.ChangeCamera(playerCamera);
            return;
        }

        bool isWaitingOrFading = playerInteractionController.IsDialogWaitingOrFading();
        playerInteractionController.HideDialog();

        StartCoroutine(SelfCameraModeChangeCoroutine(isWaitingOrFading, !isSelfCameraMode, isForTutorialOpening));

        Vector3 vec;
        if (!isSelfCameraMode)
        {
            vec = new Vector3(playerCameraXRotation, CameraTransformConst.ExploreSelfCameraYRotation);
        }
        else
        {
            vec = new Vector3(playerCameraXRotation, CameraTransformConst.ExploreDefaultCameraYRotation);
        }

        UpdateIsSelfCameraMode(!isSelfCameraMode);
        UpdateIsMapExploring();
        frontCameraUIButton.SetActive(!isSelfCameraMode);
        playerInteractionController.ToggleInteractionObjects(isSelfCameraMode, currentSubStageId);
        warriorChractorSprite.SetActive(isSelfCameraMode);
        magicianChractorSprite.SetActive(isSelfCameraMode);
        selfCameraUIButton.SetActive(isSelfCameraMode);
        playerCamera.transform.localRotation = Quaternion.Euler(vec);
    }

    public void OnClickIntuition()
    {
        SetSelfCameraModeButtonsVisibility(false);
        playerInteractionController.ShowDialogForSelfCameraMode(true, currentSubStageId);
    }

    public void OnClickResearch()
    {
        SetSelfCameraModeButtonsVisibility(false);
        playerInteractionController.ShowDialogForSelfCameraMode(false, currentSubStageId);
    }

    public void OnClickDialogConfirm()
    {
        playerInteractionController.SkipDialog();
    }

    public void OnResetDataClick()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void OnMoveToStage3Click()
    {
        SceneManager.LoadScene("Stage3Scene");
    }

    public void OnSelfCameraModeDialogComplete(bool needToRotateCamera)
    {
        if (needToRotateCamera)
        {
            OnPlayerCameraChangeClick();
        }
        SetSelfCameraModeButtonsVisibility(true);
        headerController.SetHeaderVisibility(true);

        if (tutorialController != null)
        {
            tutorialController.ShowFirstMovingButtonIfNeed();
        }
    }

    public void OnDialogComplete()
    {
        StartCoroutine(ForceRotateCameraForDialogCoroutine());
    }

    public BlinkPoint GetCurrentFloor()
    {
        return new BlinkPoint(currentFloor, playerObject.transform.localEulerAngles, player.GetDirection());
    }

    public BlinkPoint GetCurrentBlinkPoint()
    {
        return currentBlinkPoint;
    }

    public void OnStartFloorChanged(Floor floor)
    {
        currentSubStageId = portalSubStageIds[0];
        
        BlinkPoint startPoint = startPointManager.GetStartPoint().ToBlinkPoint(floor);
        MoveToFloor(startPoint);
        CheckForwardInteractionObeject();

        targetPosition = playerObject.transform.localPosition;

        startPointManager.SetInitialStartFloorLoadDone();
    }

    public void OnBlinkPointChanged(Floor floor)
    {
        currentBlinkPoint = startPointManager.GetBlinkPoint().ToBlinkPoint(floor);

        startPointManager.SetInitialBlinkPointLoadDone();
    }

    public void ClearPortalPointInfos(int totalPortalCount)
    {
#if UNITY_EDITOR
        portalPoints = new BlinkPoint[totalPortalCount];
        portalSubStageIds = new int[totalPortalCount];
        for (int i = 0; i < portalSubStageIds.Length; i++)
        {
            portalSubStageIds[i] = 0;
        }
#endif
    }

    public void AddPortalPointFloor(int index, Floor floor, Direction startDirection)
    {
#if UNITY_EDITOR
        float rotationY = blinkPointRotationYByDirection[(int)startDirection];
        BlinkPoint point = new BlinkPoint(floor, new Vector3(0, rotationY, 0), startDirection);
        portalPoints[index] = point;
#endif
    }

    public void SetMapParent(Transform mapParent)
    {
#if UNITY_EDITOR
        this.mapParent = mapParent;
#endif
    }

    public void SetStartFloor(Floor startFloor, Direction startDirection)
    {
#if UNITY_EDITOR
        Vector3 rotation = new Vector3(0, blinkPointRotationYByDirection[(int)startDirection], 0);
        this.startFloor = new BlinkPoint(
            startFloor,
            rotation,
            startDirection
        );
#endif
    }

    public void SetStartFloorMediator(StartFloorChangeMediator mediator)
    {
#if UNITY_EDITOR
        this.startFloorMediator = mediator;
#endif
    }

    private void Awake()
    {
        player = PlayerManager.GetInstance().GetPlayer();

        currentLerpDistance = 1.0f;

        inventoryController.SetPlayer(player);
        inventoryController.SetItemUseListener(playerInteractionController);

        playerInteractionController.SetPlayer(player);
        playerInteractionController.SetDialogStatusListener(this);
        playerInteractionController.SetStageTransitionCallback(MoveToSubStage);
        playerInteractionController.SetCurrentFloorProvider(this);

        headerController.SetPlayer(player);
        headerController.SetSettingMenuClickListener(playerInteractionController);

        defaultFrontCameraRotation = new Vector3(playerCamera.transform.localEulerAngles.x, CameraTransformConst.ExploreDefaultCameraYRotation, 0);
        battleWarriorCameraRotation = new Vector3(playerCamera.transform.localEulerAngles.x, CameraTransformConst.BattleWarriorCameraYRotation, 0);

        StartPointData startPointData = startPointManager.GetStartPoint();
        if (!startPointManager.IsInitialStartFloorLoad() || 
            !startPointManager.IsInitialBlinkPointLoad() ||
            startPointData == null ||
            startPointData.GetStageId() != playerInteractionController.GetStageId()
        )
        {
            int startPortalId = sanctuaryInfoRepository.GetStartPortalId();
            if (startPortalId < 0)
            {
                currentBlinkPoint = startFloor;
            }
            else
            {
                currentBlinkPoint = portalPoints[startPortalId];
                currentSubStageId = portalSubStageIds[startPortalId];
            }

            MoveToFloor(currentBlinkPoint);
            targetPosition = playerObject.transform.localPosition;
            CheckForwardInteractionObeject();

            startPointManager.SetInitialStartFloorLoadDone();
            startPointManager.SetInitialBlinkPointLoadDone();

            if (startPointManager.NeedToSaveAfterSceneTransition())
            {
                playerInteractionController.OnClickSaveData();
                startPointManager.SetNeedToSaveAfterTransitionDone(false);
            }
        }
        else
        {
            startFloorMediator.SetStartFloorChangeObserver(this);
        }
    }

    private void Start()
    {
        subStageDataList[currentSubStageId].DistributeMonsters();
        subStageDataList[currentSubStageId].DistributeItems();
        mapParent.GetChild(currentSubStageId).gameObject.SetActive(true);
        mapDecorations.SetActive(true);

        if (IsTutorial())
        {
            StartTutorialOpeningDialog();
        }
    }

    private IEnumerator FowardMovingCoroutine()
    {
        Vector3 startPosition = playerObject.transform.localPosition;

        //isMoving = true;
        while (true)
        {
            if (currentLerpDistance > 1.0f)
            {
                break;
            }
            currentLerpDistance += playerMovingSpeed;
            playerObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }
        playerObject.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        //isMoving = false;
        ResetMovingCoroutine();

        yield return new WaitForSeconds(0.1f);
        if (fowardIsDown)
        {
            OnForwardDown();
        }
    }

    private IEnumerator ComebackMovingCoroutine()
    {
        /*
            현재 축 방향을 수정하지 않고 캐릭터 방향을 수정한 상태라. z+축(foward) : 왼쪽, x+축(right) : 앞
        */
        Vector3 startPosition = playerObject.transform.localPosition;
        Vector3 DestinationPosition = playerObject.transform.right * (5.14f);

        //isMoving = true;
        while (true)
        {
            if (currentLerpDistance > 0.14f)
            {
                break;
            }
            currentLerpDistance += playerMovingSpeed;
            playerObject.transform.localPosition = Vector3.Lerp(startPosition, startPosition + DestinationPosition, currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }

        int direction = (int)player.GetDirection();
        Wall wall = currentFloor.GetWall(direction);

        if (playerInteractionController.CheckForwardPortal(wall))
        {
            yield break;
        }

        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (currentLerpDistance < 0.0f)
            {
                break;
            }
            currentLerpDistance -= playerMovingSpeed;
            playerObject.transform.localPosition = Vector3.Lerp(startPosition, startPosition + DestinationPosition, currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }
        playerObject.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        //isMoving = false;
        ResetMovingCoroutine();

        yield return new WaitForSeconds(0.1f);
        if (fowardIsDown)
        {
            OnForwardDown();
        }
    }

    private IEnumerator RotateMovingCoroutine(float angle)
    {
        float startAngle = playerObject.transform.localEulerAngles.y;

        //isMoving = true;
        while (true)
        {
            if (currentLerpDistance > 1.0f)
            {
                break;
            }
            currentLerpDistance += playerRotateSpeed;
            playerObject.transform.localEulerAngles = Vector3.Lerp(new Vector3(0, startAngle, 0), new Vector3(0, startAngle + angle, 0), currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }
        playerObject.transform.localEulerAngles = new Vector3(0, startAngle + angle, 0);
        //isMoving = false;
        ResetMovingCoroutine();

        yield return new WaitForSeconds(0.1f);
        if (rotateIsDown)
        {
            OnRotateDown(inputRotateAngle);
        }

    }

    private IEnumerator SelfCameraModeChangeCoroutine(bool isWaitingOrFading, bool isSelfCameraMode, bool isForTutorialOpening = false)
    {
        if (!isForTutorialOpening)
        {
            cameraChangeEffectObject.SetActive(true);
            selfCameraChangeAnimator.SetBool(CameraModeChangeAnimationKey, true);
        }
        
        playerCameraAnimator.SetBool(CameraAnimationKeyConst.SelfCameraAnimationKey, isSelfCameraMode);

        if (isForTutorialOpening)
        {
            playerInteractionController.ShowDialogForSelfCameraModeForceChange(0, "0001");
            SetSelfCameraModeButtonsVisibility(false);
        }
        else if (forceCameraChange && isSelfCameraMode)
        {
            forceCameraChange = false;
            playerInteractionController.ShowDialogForSelfCameraModeForceChange();
            SetSelfCameraModeButtonsVisibility(false);
        }
        else if (isWaitingOrFading && isSelfCameraMode)
        {
            playerInteractionController.ShowSelfCameraEnterDialog();
        }
        else if (isSelfCameraMode)
        {
            playerInteractionController.SetPoseAndFaceForEnterance();
        }

        yield return new WaitForSeconds(0.25f);

        selfCameraChangeAnimator.SetBool(CameraModeChangeAnimationKey, false);
        cameraChangeEffectObject.SetActive(false);
    }

    private void ResetMovingCoroutine()
    {
        CheckBlinkPoint();
        CheckForwardInteractionObeject();
        CheckCurrentDialogEvent();
        CheckCurrentTriggerObject();
        CheckForwardMonster();
        CheckCurrentTutorialEventFloor();

        currentMovingCoroutine = null;
    }

    private void CheckBlinkPoint()
    {
        if (!currentFloor.IsBlinkPoint())
        {
            return;
        }

        currentBlinkPoint = new BlinkPoint(currentFloor, playerObject.transform.localEulerAngles, player.GetDirection());
        currentFloor.SetBlinkPoint(false);
    }

    private bool CheckPositionValidity(int direction, Wall wall)
    {
        return currentFloor.GetFloor(direction) != null && (wall == null || wall.IsPassable());
    }

    private void CheckCurrentDialogEvent()
    {
        InteractionObject interaction = currentFloor.GetInteractionObject();
        if (interaction == null)
        {
            return;
        }

        if (interaction is DialogEventObject)
        {
            DialogEventObject dialogEvent = interaction as DialogEventObject;
            if (dialogEvent.IsAlreadyUsed())
            {
                return;
            }

            dialogEvent.DoInteraction(playerInteractionController);

            ResetActions();

            forceCameraChange = true;
            headerController.SetHeaderVisibility(false);
        }
        else if (interaction is AdventureDialogObject)
        {
            AdventureDialogObject dialogObject = interaction as AdventureDialogObject;
            if (dialogObject.IsAlreadyUsed())
            {
                return;
            }

            dialogObject.DoInteraction(playerInteractionController);
        }
    }

    private void CheckCurrentTriggerObject()
    {
        InteractionObject triggerObject = currentFloor.GetTriggerObject();
        if (triggerObject == null)
        {
            return;
        }

        triggerObject.DoInteraction(playerInteractionController);
    }

    private void CheckForwardInteractionObeject()
    {
        int direction = (int)player.GetDirection();
        Wall wall = currentFloor.GetWall(direction);
        InteractionObject interaction;
        if (wall != null && (interaction = wall.GetInteractionObject()) != null)
        {
            playerInteractionController.CheckForwardInteractionObeject(interaction);
        }
        else
        {
            playerInteractionController.ResetInteractionButton();
        }
    }

    private void CheckForwardMonster()
    {
        int direction = (int)player.GetDirection();

        if (currentFloor.IsWallExist(direction))
        {
            return;
        }

        Floor nextFloor = currentFloor.GetFloor(direction);

        if (nextFloor != null && nextFloor.IsMonsterFloor())
        {
            ResetActions();

            UpdateIsBattle(true);
            UpdateIsMapExploring();

            SmoothCameraTransitionBetweenBattleAndExplore(true);
            playerCameraAnimator.SetBool(CameraAnimationKeyConst.BattleCameraTransitionAnimationKey, true);
            playerInteractionController.InteractWithObject(nextFloor.GetInteractionObject());
            headerController.SetHeaderVisibility(false);

            StartCoroutine(FinishBattleCoroutine(nextFloor));
        }
    }

    private void CheckCurrentTutorialEventFloor()
    {
        int tutorialButtonIndex = currentFloor.GetTutorialButtonShowIndex();
        if (tutorialController != null && tutorialButtonIndex > 0)
        {
            tutorialController.ShowMovingButtonAt(tutorialButtonIndex);
        }
    }

    private bool IsPlayerMoving()
    {
        return fowardIsDown || rotateIsDown || currentMovingCoroutine != null;
    }

    private void ResetActions()
    {
        StopAllCoroutines();
        dimLayer.SetActive(true);
        rotateIsDown = false;
        fowardIsDown = false;
        currentMovingCoroutine = null;
    }

    /**
     * TODO: Separate method for each condition.
     */
    private IEnumerator FinishBattleCoroutine(Floor monsterSpawn)
    {
        yield return new WaitUntil(() => playerInteractionController.IsBattleFinished());
        dimLayer.SetActive(true);

        if (playerInteractionController.IsMonsterDead())
        {
            if (playerInteractionController.IsMagicianCameraMode())
            {
                yield return new WaitForSeconds(1f);
                OnPlayerCameraChangeClick();
            }

            yield return new WaitForSeconds(0.3f);
            playerInteractionController.FinishBattleWithMonsterDeath();
            SmoothCameraTransitionBetweenBattleAndExplore(false);

            yield return new WaitForSeconds(0.5f);
            playerCameraAnimator.SetBool(CameraAnimationKeyConst.BattleCameraTransitionAnimationKey, false);
            (monsterSpawn.GetInteractionObject() as MonsterSpawn).GetReward();
            monsterSpawn.SetMonsterFloor(false);
            monsterSpawn.transform.GetChild(monsterSpawn.transform.childCount - 1).gameObject.SetActive(false);
        }
        else if (playerInteractionController.IsPlayerDead())
        {
            playerInteractionController.FinishBattleWithPlayerDeath();
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            playerInteractionController.FinishBattleWithBlink();

            yield return new WaitForSeconds(1.5f);

            playerCamera.transform.localRotation = Quaternion.Euler(new Vector3(playerCameraXRotation, CameraTransformConst.ExploreDefaultCameraYRotation));

            MoveToFloor(currentBlinkPoint);

            playerInteractionController.PlayAnimationAfterBlink();
            MonsterSpawn spawn = monsterSpawn.GetInteractionObject() as MonsterSpawn;
            subStageDataList[currentSubStageId].ShuffleMonsterForBlink(spawn.GetGroupIndex(), spawn.GetSpawnIndex());

            yield return new WaitForSeconds(1f);
        }
        dimLayer.SetActive(false);
        UpdateIsBattle(false);
        headerController.SetHeaderVisibility(true);
    }

    private void MoveToFloor(BlinkPoint movePoint)
    {
        this.currentFloor = movePoint.blinkFloor;
        Vector3 movePosition = movePoint.blinkFloor.transform.localPosition;
        playerObject.transform.localPosition = new Vector3(
            movePosition.x,
            playerObject.transform.localPosition.y,
            movePosition.z
            );

        playerObject.transform.localRotation = Quaternion.Euler(movePoint.blinkRotation);
        player.SetDirection(movePoint.blinkDirection);
    }

    private void UpdateIsMapExploring()
    {
        playerInteractionController.SetExploringMap(!isBattle && !isSelfCameraMode);
    }

    private void UpdateIsSelfCameraMode(bool isSelfCameraMode)
    {
        this.isSelfCameraMode = isSelfCameraMode;
        UpdateIsMapExploring();
    }

    private void UpdateIsBattle(bool isBattle)
    {
        this.isBattle = isBattle;
        UpdateIsMapExploring();
    }

    private void MoveToSubStage(int portalId)
    {
        subStageDataList[currentSubStageId].CleanupMonsters();
        subStageDataList[currentSubStageId].CleanupItems();
        mapParent.GetChild(currentSubStageId).gameObject.SetActive(false);

        currentSubStageId = portalSubStageIds[portalId];

        mapParent.GetChild(currentSubStageId).gameObject.SetActive(true);
        MoveToFloor(portalPoints[portalId]);

        subStageDataList[currentSubStageId].DistributeMonsters();
        subStageDataList[currentSubStageId].DistributeItems();

        ResetMovingCoroutine();
    }

    private IEnumerator ForceRotateCameraForDialogCoroutine()
    {
        yield return new WaitForSeconds(1f);

        OnPlayerCameraChangeClick();
        dimLayer.SetActive(false);
    }

    private void SetSelfCameraModeButtonsVisibility(bool isVisible)
    {
        for (int i = 0; i < visibilityChangeTargetViewListForDialog.Length; i++)
        {
            visibilityChangeTargetViewListForDialog[i].SetActive(isVisible);
        }
    }

    private void SmoothCameraTransitionBetweenBattleAndExplore(bool isBattle)
    {
        if (defaultFrontCameraRotation == null || battleWarriorCameraRotation == null)
        {
            return;
        }

        StartCoroutine(SmoothCameraTransitionCoroutine(isBattle));
    }

    private bool IsTutorial()
    {
        return tutorialController != null;
    }

    private void StartTutorialOpeningDialog()
    {
        OnPlayerCameraChangeClick(true);
    }

    private IEnumerator SmoothCameraTransitionCoroutine(bool isBattle)
    {
        Vector3 toRotation = isBattle ? battleWarriorCameraRotation : defaultFrontCameraRotation;
        Vector3 fromRotation = !isBattle ? battleWarriorCameraRotation : defaultFrontCameraRotation;

        float currentPoint = 0.0f;
        playerCameraAnimator.SetBool(CameraAnimationKeyConst.WarriorCameraTransitionAnimationKey, isBattle);

        while (currentPoint < 1)
        {
            currentPoint += smoothCameraTransitionSpeed;
            playerCamera.transform.localEulerAngles = Vector3.Lerp(fromRotation, toRotation, currentPoint);

            yield return new WaitForFixedUpdate();
        }

        playerCamera.transform.localEulerAngles = toRotation;
    }
}