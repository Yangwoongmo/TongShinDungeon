using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterReward
{
    [SerializeField] private float probability;
    [SerializeField] private int itemId;
    [SerializeField] private int minGold;
    [SerializeField] private int maxGold;

    public float GetProbability()
    {
        return probability;
    }

    public (int, int) GetReward()
    {
        int rewardGold = Random.Range(minGold, maxGold + 1);
        return (itemId, rewardGold);
    }
}
