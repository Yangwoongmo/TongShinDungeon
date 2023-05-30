using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryInfoRepository
{
    private static SanctuaryInfoRepository instance;

    private const string PurifiedPortalPrefKeyPrefix = "purified_portal/";
    private const string LatestEnteredPortalIdPrefKey = "latest_entered_portal_id";
    private const string StartPortalIdPrefKey = "start_portal_id";
    private const string HasSanctuarySceneVisitedKey = "has_sanctuary_scene_visited";
    private const string SanctuaryShopItemPrefKey = "sanctuary_shop_item";
    private const string SanctuaryDeathStatisticsPrefKey = "sanctuary_death_statistics";
    private const string EnterByPlayerDeath = "enter_by_player_death";
    private const char PurifiedPortalIdDelemeter = ',';
    private const int TotalStage = 1;

    private SanctuaryInfoRepository()
    {

    }

    public static SanctuaryInfoRepository GetInstance()
    {
        if (instance == null)
        {
            instance = new SanctuaryInfoRepository();
        }

        return instance;
    }

    public void SavePurifiedPortal(int stageId, int portalId)
    {
        int targetStageId = Mathf.Min(stageId, 1);

        string prefKey = PurifiedPortalPrefKeyPrefix + targetStageId;
        string purifiedIds = PlayerPrefs.GetString(prefKey, "");

        List<int> idList = GetPurifedPortalList(targetStageId);

        int targetPortalId = Mathf.Max(0, stageId - 1);
        if (idList.Contains(targetPortalId))
        {
            return;
        }

        string nextIdString = purifiedIds.Length == 0 ? targetPortalId.ToString() : PurifiedPortalIdDelemeter + targetPortalId.ToString();
        PlayerPrefs.SetString(prefKey, purifiedIds + nextIdString);
    }

    public List<int> GetPurifedPortalList(int stageId)
    {
        string prefKey = PurifiedPortalPrefKeyPrefix + stageId;
        string purifiedIds = PlayerPrefs.GetString(prefKey, "");
        string[] idList = purifiedIds.Split(PurifiedPortalIdDelemeter);

        List<string> sortedIdList = new List<string>();
        if (idList[0].Length > 0)
        {
            sortedIdList.AddRange(idList);
            sortedIdList.Sort();
        }

        return sortedIdList.ConvertAll(id => int.Parse(id));
    }

    public List<List<int>> GetTotalPurifiedPortalList()
    {
        List<List<int>> totalList = new List<List<int>>();

        for (int i = 1; i <= TotalStage; i++)
        {
            totalList.Add(GetPurifedPortalList(i));
        }

        return totalList;
    }

    public void SaveLatestEnteredPortalId(int stageId, int portalId)
    {
        if (stageId < 0 || portalId < 0)
        {
            return;
        }

        PlayerPrefs.SetString(LatestEnteredPortalIdPrefKey, stageId.ToString() + PurifiedPortalIdDelemeter + portalId.ToString());
    }

    public (int, int) GetLatestEnteredPortalId()
    {
        string id = PlayerPrefs.GetString(LatestEnteredPortalIdPrefKey, "");
        if (id.Length == 0)
        {
            return (-1, -1);
        }

        PlayerPrefs.DeleteKey(LatestEnteredPortalIdPrefKey);

        string[] idList = id.Split(PurifiedPortalIdDelemeter);
        return (int.Parse(idList[0]), int.Parse(idList[1]));
    }

    public void SaveStartPortalId(int portalId)
    {
        PlayerPrefs.SetString(StartPortalIdPrefKey, portalId.ToString());
    }

    public int GetStartPortalId()
    {
        string id = PlayerPrefs.GetString(StartPortalIdPrefKey, "");
        PlayerPrefs.DeleteKey(StartPortalIdPrefKey);

        return id.Length == 0 ? -1 : int.Parse(id);
    }

    public void SetHasSanctuarySceneVisited()
    {
        PlayerPrefs.SetInt(HasSanctuarySceneVisitedKey, 1);
    }

    public bool HasSanctuarySceneVisited()
    {
        return PlayerPrefs.GetInt(HasSanctuarySceneVisitedKey, 0) > 0;
    }

    public void SetSanctuaryShopItemStock(string itemId, int itemStock, int itemPrice)
    {
        PlayerPrefs.SetString(SanctuaryShopItemPrefKey + itemId, itemStock.ToString() + PurifiedPortalIdDelemeter + itemPrice.ToString());
    }

    public (int, int) GetSanctuaryShopItemStock(int itemId)
    {
        string itemInfo = PlayerPrefs.GetString(SanctuaryShopItemPrefKey + itemId);
        string[] itemsInfo = itemInfo.Split(PurifiedPortalIdDelemeter);
        return (int.Parse(itemsInfo[0]), int.Parse(itemsInfo[1]));
    }

    public bool HasSanctuaryShopItemKey(int itemId)
    {
        return PlayerPrefs.HasKey(SanctuaryShopItemPrefKey + itemId);
    }

    public SanctuaryDeathStatistics GetSanctuaryDeathStatistics()
    {
        string json = PlayerPrefs.GetString(SanctuaryDeathStatisticsPrefKey, "");
        if (json.Length > 0)
        {
            return JsonUtility.FromJson<SanctuaryDeathStatistics>(json);
        }
        else
        {
            return new SanctuaryDeathStatistics(0, 0);
        }
    }

    public void IncreaseDeathCount()
    {
        SanctuaryDeathStatistics statistics = GetSanctuaryDeathStatistics();

        statistics.IncreaseDeathCount();

        SetSanctuaryDeathStatistics(statistics);
    }

    public void IncreaseProgress()
    {
        SanctuaryDeathStatistics statistics = GetSanctuaryDeathStatistics();

        statistics.IncreaseProgress();

        SetSanctuaryDeathStatistics(statistics);
    }

    public void ClearIsEnterByDeathValue()
    {
        PlayerPrefs.SetInt(EnterByPlayerDeath, 0);
    }

    public void SetIsEnterByDeath()
    {
        PlayerPrefs.SetInt(EnterByPlayerDeath, 1);
    }

    public bool IsEnterByDeathValue()
    {
        return PlayerPrefs.GetInt(EnterByPlayerDeath, 0) > 0;
    }

    private void SetSanctuaryDeathStatistics(SanctuaryDeathStatistics statistics)
    {
        string json = JsonUtility.ToJson(statistics);
        PlayerPrefs.SetString(SanctuaryDeathStatisticsPrefKey, json);
    }
}
