using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoRepository
{
    private static PlayerInfoRepository instance;

    private const string PlayerDataPrefKey = "player_data";

    private PlayerInfoRepository()
    {
        // Private constructor for singleTon instance.
    }

    public static PlayerInfoRepository GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerInfoRepository();
        }

        return instance;
    }

    public void SavePlayerData(Player player)
    {
        // TODO: Maybe needs some encryption?
        string json = JsonUtility.ToJson(player);
        PlayerPrefs.SetString(PlayerDataPrefKey, json);
    }

    public Player LoadPlayerData()
    {
        string data = PlayerPrefs.GetString(PlayerDataPrefKey, "");
        if (data.Length == 0)
        {
            return new Player(true);
        }
        else
        {
            return JsonUtility.FromJson<Player>(data);
        }
    }
}
