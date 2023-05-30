using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPointRepository
{
    private static StartPointRepository instance;
    private const string StartPointPrefKey = "start_point";
    private const string BlinkPointPrefKey = "blink_point";

    private StartPointRepository()
    {
    }

    public static StartPointRepository GetInstance()
    {
        if (instance == null)
        {
            instance = new StartPointRepository();
        }

        return instance;
    }

    public void SaveCurrentFloorAndBlinkPoint(
        int stageId,
        BlinkPoint currentFloor, 
        BlinkPoint blinkPoint
    )
    {
        StartPointData startFloor = new StartPointData(stageId, currentFloor);
        StartPointData blinkFloor = new StartPointData(stageId, blinkPoint);

        PlayerPrefs.SetString(StartPointPrefKey, JsonUtility.ToJson(startFloor));
        PlayerPrefs.SetString(BlinkPointPrefKey, JsonUtility.ToJson(blinkFloor));
    }

    public void ClearStartFloorData()
    {
        PlayerPrefs.DeleteKey(StartPointPrefKey);
        PlayerPrefs.DeleteKey(BlinkPointPrefKey);
    }

    public StartPointData GetStartPoint()
    {
        return GetPointByKey(StartPointPrefKey);
    }

    public StartPointData GetBlinkPoint()
    {
        return GetPointByKey(BlinkPointPrefKey);
    }

    private StartPointData GetPointByKey(string key)
    {
        string value = PlayerPrefs.GetString(key, "");
        if (value.Length == 0)
        {
            return null;
        }
        else
        {
            return JsonUtility.FromJson<StartPointData>(value);
        }
    }
}

[System.Serializable]
public class StartPointData
{
    [SerializeField] private int stageId;
    [SerializeField] private string floorId;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Direction direction;

    public StartPointData(int stageId, BlinkPoint startPoint)
    {
        this.stageId = stageId;
        floorId = startPoint.blinkFloor.GetId();
        rotation = startPoint.blinkRotation;
        direction = startPoint.blinkDirection;
    }

    public int GetStageId()
    {
        return stageId;
    }

    public string GetFloorId()
    {
        return floorId;
    }

    public BlinkPoint ToBlinkPoint(Floor floor)
    {
        return new BlinkPoint(floor, rotation, direction);
    }
}
