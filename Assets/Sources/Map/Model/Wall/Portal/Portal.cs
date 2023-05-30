using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Portal : Wall
{
    [SerializeField] protected string targetSceneName;

    public abstract bool MoveToTargetArea(PlayerEventHandler eventHandler);
}
