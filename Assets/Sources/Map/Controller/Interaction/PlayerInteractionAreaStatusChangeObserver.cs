using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInteractionAreaStatusChangeObserver : MonoBehaviour
{
    public delegate void DoInteraction(PlayerEventHandler handler);

    public abstract void OnPlayerInInteractionAreaStatusChanged(DoInteraction interaction);
}
