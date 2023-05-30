using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorBattleStatistic
{
    private int totalHitCount;
    private int totalHitDamage;

    public WarriorBattleStatistic(int hitCount, int hitDamage)
    {
        totalHitDamage = hitDamage;
        totalHitCount = hitCount;
    }

    public int GetTotalHitCount()
    {
        return totalHitCount;
    }

    public int GetTotalHitDamage()
    {
        return totalHitDamage;
    }
}
