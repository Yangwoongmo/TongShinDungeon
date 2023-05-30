using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleActionObserver
{
    public void RegisterWarriorAction(WarriorStatus actionType);
    public void UnregisterWarriorAction();
    public bool FireWarriorAttack(int damage, int pp);
    public void RegisterMonsterAction(MonsterStatus actionType);
    public void UnregisterMonsterAction();
    public void FireMonsterAttack(int damage, PlayerDirection attackDirection);
}
