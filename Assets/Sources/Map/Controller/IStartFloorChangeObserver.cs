using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStartFloorChangeObserver
{
    public void OnStartFloorChanged(Floor floor);

    public void OnBlinkPointChanged(Floor floor);
}
