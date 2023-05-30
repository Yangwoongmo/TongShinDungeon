using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInteractionStatusChangeObserver : MonoBehaviour
{
    public abstract void OnPlayerScreenModeChanged(bool isSelfCameraMode);
    public abstract void SetPlayerPositionTransform(Transform position);
}
