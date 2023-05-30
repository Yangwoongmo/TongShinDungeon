using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFloorChangeMediator : MonoBehaviour
{
    private IStartFloorChangeObserver observer;

    public void SetStartFloorChangeObserver(IStartFloorChangeObserver observer)
    {
        this.observer = observer;
    }

    public void RemoveStartFloorChangeObserver()
    {
        observer = null;
    }

    public void SetStartFloor(Floor floor)
    {
        if (observer != null)
        {
            observer.OnStartFloorChanged(floor);
        }
    }

    public void SetBlinkPoint(Floor floor)
    {
        if (observer != null)
        {
            observer.OnBlinkPointChanged(floor);
        }
    }
}
