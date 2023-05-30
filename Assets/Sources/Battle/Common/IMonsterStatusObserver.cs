using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterStatusObserver
{
    public void MonsterHitDamage();
    public void MonsterHitPp();
    public void MonsterRecoverPp();
    public void MonsterAttackWarning(string animationKey, int value);
    public void ResetMonsterWarning(string animationKey);
}
