using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlinkPoint
{
    public Floor blinkFloor;
    public Vector3 blinkRotation;
    public Direction blinkDirection;

    public BlinkPoint(Floor f, Vector3 r, Direction d)
    {
        blinkFloor = f;
        blinkRotation = r;
        blinkDirection = d;
    }
}

[System.Serializable]
public class SubStageData
{
    [SerializeField] private RandomMonsterDistributor monsterDistributor;
    [SerializeField] private RandomItemDistributor itemDistributor;

    public void CleanupMonsters()
    {
        monsterDistributor.CleanupAllSpawans();
    }

    public void DistributeMonsters()
    {
        monsterDistributor.DistributeMonsters();
    }

    public void ShuffleMonsterForBlink(int group, int spawn)
    {
        monsterDistributor.ShuffleMonsterForBlink(group, spawn);
    }

    public void CleanupItems()
    {
        itemDistributor.CleanupItemHolders();
    }

    public void DistributeItems()
    {
        itemDistributor.DistributeItems();
    }
}