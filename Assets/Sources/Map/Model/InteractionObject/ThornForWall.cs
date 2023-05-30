using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornForWall : InteractionObject
{
    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        playerEventHandler.GetDamage(20);
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return false;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        if (isSelfCameraMode)
        {
            // Stop moving
            this.gameObject.SetActive(false);
        }
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        // Do nothing
    }

    public override bool IsTrap()
    {
        return true;
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        return null;
    }

    protected override void SetMediator()
    {
        mediator = this.gameObject.transform.parent.parent.GetComponent<PlayerInteractionMediator>();
        mediator.AddPlayerStatusChangeObserver(this);
    }
}
