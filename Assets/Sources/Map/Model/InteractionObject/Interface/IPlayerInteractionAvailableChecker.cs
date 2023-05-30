using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractionAvailableChecker
{
    public bool CanUnlockDoor();

    public bool CanMarkWall();
}
