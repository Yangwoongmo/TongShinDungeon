using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapObjectStatusManager
{
    private static MapObjectStatusManager instance;

    private const string StateChangedObjectIdPrefKeyPrefix = "state_changed_object_list_";
    private const int TotalStageCount = 6;
    private List<HashSet<string>> stateChangeObjectIdSetList;
    private int stageId = -1;

    private MapObjectStatusManager()
    {
    }

    public static MapObjectStatusManager GetInstance()
    {
        if (instance == null)
        {
            instance = new MapObjectStatusManager();
        }
        return instance;
    }

    public void UpdateObjectStatus(string objectId)
    {
        LoadStateChangedObjectIdSetListIfNeed();
        if (stageId < 0)
        {
            return;
        }

        HashSet<string> idSet = stateChangeObjectIdSetList[stageId];

        if (idSet.Contains(objectId))
        {
            idSet.Remove(objectId);
        }
        else
        {
            idSet.Add(objectId);
        }
    }

    public void SaveObjectChangeState()
    {
        LoadStateChangedObjectIdSetListIfNeed();

        for (int i = 0; i < TotalStageCount; i++)
        {
            HashSet<string> idSet = stateChangeObjectIdSetList[i];
            StringBuilder idStringBuilder = new StringBuilder("");
            
            foreach (string id in idSet)
            {
                idStringBuilder.Append(id + ",");
            }
            idStringBuilder.Remove(idStringBuilder.Length - 1, 1);

            PlayerPrefs.SetString(StateChangedObjectIdPrefKeyPrefix + i, idStringBuilder.ToString());
        }
    }

    public bool HasObjectStateChanged(string objectId)
    {
        LoadStateChangedObjectIdSetListIfNeed();
        if (stageId < 0)
        {
            return false;
        }

        return stateChangeObjectIdSetList[stageId].Contains(objectId);
    }

    private void LoadStateChangedObjectIdSetListIfNeed()
    {
        if (stateChangeObjectIdSetList != null && stateChangeObjectIdSetList.Count > 0)
        {
            return;
        }

        stateChangeObjectIdSetList = new List<HashSet<string>>();
        for (int i = 0; i < TotalStageCount; i++)
        {
            stateChangeObjectIdSetList.Add(ParseIdSet(i));
        }
    }

    public void SetStageId(int stageId)
    {
        this.stageId = stageId;
    }

    private HashSet<string> ParseIdSet(int mapId)
    {
        string[] idList = PlayerPrefs.GetString(StateChangedObjectIdPrefKeyPrefix + mapId).Split(',');
        HashSet<string> idSet = new HashSet<string>(idList);
       
        return idSet;
    }
}
