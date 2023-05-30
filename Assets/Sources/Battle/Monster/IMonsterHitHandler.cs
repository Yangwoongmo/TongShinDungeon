using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterHitHandler
{
    // Returns isAttackBlocked which means parrying success.
    public bool HitFromWarrior(int damage, int pp);
    public void HitFromMagician(float[] damageList, float awakeAddition, float stunAddition);
}
