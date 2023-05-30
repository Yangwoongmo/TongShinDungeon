using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPointDataManager
{
    private static StartPointDataManager instance;

    private StartPointRepository repository = StartPointRepository.GetInstance();

    private StartPointData startPoint;
    private StartPointData blinkPoint;

    private bool isInitialStartFloorLoad = true;
    private bool isInitialBlinkPointLoad = true;
    private bool needToSaveAfterSceneTransition = false;

    private StartPointDataManager()
    {
    }

    public static StartPointDataManager GetInstance()
    {
        if (instance == null)
        {
            instance = new StartPointDataManager();
        }

        return instance;
    }

    public void SaveCurrentFloorAndBlinkPoint(
        int stageId,
        BlinkPoint currentFloor,
        BlinkPoint blinkPoint
    )
    {
        repository.SaveCurrentFloorAndBlinkPoint(stageId, currentFloor, blinkPoint);
    }

    public void ClearStartFloorData()
    {
        repository.ClearStartFloorData();
    }

    public StartPointData GetStartPoint()
    {
        if (startPoint == null)
        {
            startPoint = repository.GetStartPoint();
            if (startPoint == null)
            {
                isInitialStartFloorLoad = false;
            }
        }

        return startPoint;
    }

    public StartPointData GetBlinkPoint()
    {
        if (blinkPoint == null)
        {
            blinkPoint = repository.GetBlinkPoint();
            if (blinkPoint == null)
            {
                isInitialBlinkPointLoad = false;
            }
        }

        return blinkPoint;
    }

    public bool IsInitialStartFloorLoad()
    {
        return isInitialStartFloorLoad && GetStartPoint() != null;
    }

    public bool IsInitialBlinkPointLoad()
    {
        return isInitialBlinkPointLoad && GetBlinkPoint() != null;
    }

    public void SetInitialStartFloorLoadDone()
    {
        isInitialStartFloorLoad = false;
    }

    public void SetInitialBlinkPointLoadDone()
    {
        isInitialBlinkPointLoad = false;
    }

    public bool NeedToSaveAfterSceneTransition()
    {
        return needToSaveAfterSceneTransition;
    }

    public void SetNeedToSaveAfterTransitionDone(bool needSave)
    {
        needToSaveAfterSceneTransition = needSave;
    }
}
