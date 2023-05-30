using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : MonoBehaviour, IMagicianHitHandler
{
    private IMonsterHitHandler monsterHitHandler;

    public void SetMonsterHitHandler(IMonsterHitHandler handler)
    {
        this.monsterHitHandler = handler;
    }

    public void Die()
    {
        // TODO : Add Game Over
        Debug.Log("GAME OVER");
    }

    public bool Hit(int damage, MonsterStatus attackType, PlayerDirection direction)
    {
        return false;
    }

    public void SpellAttack(float[] damageList, float awakeAddition, float stunAddition)
    {   
        monsterHitHandler.HitFromMagician(damageList, awakeAddition, stunAddition);
    }
}
