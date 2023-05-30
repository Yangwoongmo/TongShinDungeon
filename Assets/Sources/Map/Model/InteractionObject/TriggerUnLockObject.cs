using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerUnLockObject : TriggerObject
{
    [SerializeField] private TriggerObject triggerObject;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        triggerObject.StopTrigger();
    }

    public void SetTriggerObject(TriggerObject trigger)
    {
#if UNITY_EDITOR
        triggerObject = trigger;
#endif
    }
}
