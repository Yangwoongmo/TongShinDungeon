using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICurrentFloorProvider
{
    public BlinkPoint GetCurrentFloor();

    public BlinkPoint GetCurrentBlinkPoint();
}
