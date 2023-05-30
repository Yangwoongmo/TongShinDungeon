using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagicianHitHandler
{
    public bool Hit(int damage, MonsterStatus attackType, PlayerDirection direction = PlayerDirection.CENTER);
}
