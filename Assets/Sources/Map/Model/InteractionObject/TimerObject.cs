using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerObject : TriggerObject
{
    [SerializeField] private float timeToStopTimer;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        base.DoInteraction(playerEventHandler);

        Invoke("StopTrigger", timeToStopTimer);
    }

    public void SetupTimerInfo(Thorn[] thorns, IronBar[] ironBars, float timer)
    {
#if UNITY_EDITOR
        base.SetupTriggerInfo(thorns, ironBars);
        timeToStopTimer = timer;
#endif
    }
}
