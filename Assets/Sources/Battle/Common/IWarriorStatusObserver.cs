using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWarriorStatusObserver
{
    /**
     * Return false when hp <= 0
     */
    public bool WarriorHitDamage(int damage);
}
