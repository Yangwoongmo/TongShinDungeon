using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicianController : MonoBehaviour
{
    [SerializeField] Magician magician;

    public void SpellAttack(float[] damageList, float awakeAddition, float stunAddition)
    {
        magician.SpellAttack(damageList, awakeAddition, stunAddition);
    }

    public bool HitMagician(int damage, MonsterStatus attackType, PlayerDirection direction)
    {
        return magician.Hit(damage, attackType, direction);
    }

    public void SetMonsterHitHandler(IMonsterHitHandler handler)
    {
        magician.SetMonsterHitHandler(handler);
    }

    public Magician GetMagician()
    {
        return magician;
    }
}
