using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryScene : MonoBehaviour, ISettingMenuClickListener
{
    private const float PlayerUnitRotateSpeed = 0.033f * 72;
    private const int MaxRotationState = 4;

    private readonly float[] CameraRotationForEachState = 
        new float[MaxRotationState + 1] { 90f, 145f, 240f, -30f, 30f };

    [SerializeField] private GameObject sanctuaryCamera;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private SanctuaryController[] sanctuaryControllers;
    [SerializeField] private ExploreHeaderController headerController;

    private Coroutine currentMovingCoroutine;

    private bool isRotateClicked = false;
    private int rotationState = 0;
    private RotateDirection inputRotateDirection = RotateDirection.NONE;

    private Player player;
    PlayerManager manager = PlayerManager.GetInstance();
    StartPointDataManager startPointManager = StartPointDataManager.GetInstance();
    SanctuaryInfoRepository sanctuaryInfoRepository = SanctuaryInfoRepository.GetInstance();

    public void OnRightRotate(bool withoutAnimation = false)
    {
        OnRotateButtonClick(RotateDirection.RIGHT, withoutAnimation);
    }

    public void OnLeftRotate(bool withoutAnimation = false)
    {
        OnRotateButtonClick(RotateDirection.LEFT, withoutAnimation);
    }

    public void RotateButtonUp()
    {
        isRotateClicked = false;
        inputRotateDirection = RotateDirection.NONE;
    }

    private void OnRotateButtonClick(RotateDirection direction, bool withoutAnimation = false)
    {
        isRotateClicked = true;
        inputRotateDirection = direction;

        if (currentMovingCoroutine != null)
        {
            return;
        }

        for (int i = 0; i < sanctuaryControllers.Length; i++)
        {
            sanctuaryControllers[i].SetEnable(false);
        }

        currentMovingCoroutine = StartCoroutine(RotateButtonClickCoroutine(direction, withoutAnimation));
    }

    public void OnOpenInventoryClick()
    {
        if (currentMovingCoroutine != null || isRotateClicked)
        {
            return;
        }

        inventoryController.OpenInventory();
    }

    public void OnClickOpenSettingMenu()
    {
        // Do nothing
    }

    public void OnClickCloseSettingMenu()
    {
        // Do nothing
    }

    public void OnClickSaveData()
    {
        manager.SavePlayerData();
        headerController.StartSaveTextFadeAnimation();
    }

    public void OnClickSaveDataAndQuit()
    {
        OnClickSaveData();
        Application.Quit();
    }

    private void ChangeButtonActiveState(int nextRotationState)
    {
        rotationState = nextRotationState;
        sanctuaryControllers[rotationState].SetEnable(true);
    }

    private int GetNextRotateState(RotateDirection direction)
    {
        int nextRotateState = rotationState + (int)direction;
        if (nextRotateState < 0)
        {
            nextRotateState = MaxRotationState;
        }
        else if (nextRotateState > MaxRotationState)
        {
            nextRotateState = 0;
        }

        return nextRotateState;
    }

    private float GetNextCameraRotation(float startAngle, int nextRotateState, RotateDirection direction)
    {
        float targetAngle = CameraRotationForEachState[nextRotateState];
        switch (direction)
        {
            case RotateDirection.RIGHT:
                if (targetAngle < startAngle)
                {
                    targetAngle += 360;
                }
                break;
            case RotateDirection.LEFT:
                if (targetAngle > startAngle)
                {
                    targetAngle -= 360;
                }
                break;
            default:
                break;
        }

        return targetAngle;
    }

    private IEnumerator RotateButtonClickCoroutine(RotateDirection direction, bool withoutAnimation = false)
    {
        float currentLerpDistance = 0.0f;

        int nextRotateState = GetNextRotateState(direction);
        float startAngle = CameraRotationForEachState[rotationState];
        float targetAngle = GetNextCameraRotation(startAngle, nextRotateState, direction);
        float cameraXAngle = sanctuaryCamera.transform.localEulerAngles.x;

        float rotationSpeed = PlayerUnitRotateSpeed / Mathf.Abs(targetAngle - startAngle);

        if (!withoutAnimation)
        {
            while (true)
            {
                if (currentLerpDistance > 1.0f)
                {
                    break;
                }
                currentLerpDistance += rotationSpeed;
                sanctuaryCamera.transform.localEulerAngles =
                    Vector3.Lerp(new Vector3(cameraXAngle, startAngle, 0), new Vector3(cameraXAngle, targetAngle, 0), currentLerpDistance);
                yield return new WaitForFixedUpdate();
            }
        }

        sanctuaryCamera.transform.localEulerAngles = new Vector3(cameraXAngle, targetAngle, 0);
        ChangeButtonActiveState(nextRotateState);

        yield return new WaitForSeconds(0.1f);
        currentMovingCoroutine = null;
        sanctuaryControllers[rotationState].SetButtonClickAvailable(!isRotateClicked);

        if (isRotateClicked)
        {
            OnRotateButtonClick(inputRotateDirection);
        }
    }

    private void StartSanctuaryDeathDialog()
    {
        OnLeftRotate(true);
        RotateButtonUp();

        SanctuaryShopController shopController = sanctuaryControllers[rotationState] as SanctuaryShopController;
        shopController.ShowSanctuaryDeathDialog(sanctuaryInfoRepository.GetSanctuaryDeathStatistics());
    }

    private void Start()
    {
        player = manager.GetPlayer();
        player.RestoreFullHp();
        player.RestoreHollyWater();
        manager.SavePlayerData();
        MapObjectStatusManager.GetInstance().SetStageId(-1);
        sanctuaryInfoRepository.SetHasSanctuarySceneVisited();

        inventoryController.SetPlayer(player);
        inventoryController.SetEnableUseItem(false);

        sanctuaryControllers[0].SetEnable(true);
        sanctuaryControllers[0].SetButtonClickAvailable(true);

        headerController.SetPlayer(player);
        headerController.SetSettingMenuClickListener(this);

        if (startPointManager.NeedToSaveAfterSceneTransition())
        {
            startPointManager.SetNeedToSaveAfterTransitionDone(false);
            headerController.StartSaveTextFadeAnimation();
        }

        for (int i = 0; i < sanctuaryControllers.Length; i++)
        {
            sanctuaryControllers[i].SetPlayer(player);
            sanctuaryControllers[i].SetSaveDataCallback(OnClickSaveData);
        }

        if (sanctuaryInfoRepository.IsEnterByDeathValue())
        {
            StartSanctuaryDeathDialog();
            sanctuaryInfoRepository.ClearIsEnterByDeathValue();
        }
    }

    private enum RotateDirection
    {
        LEFT = -1,
        RIGHT = 1,
        NONE = 0
    }
}
