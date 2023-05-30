using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject: PlayerInteractionStatusChangeObserver
{
    protected Transform playerPosition;
    protected PlayerInteractionMediator mediator;
    protected InteractionObjectCountStorage storage;

    public abstract void DoInteraction(PlayerEventHandler playerEventHandler);

    public abstract bool IsPlayerInActiveArea(float x, float z);

    public abstract void UpdateObjectStateWithoutAnimation();

    public override void SetPlayerPositionTransform(Transform position)
    {
        this.playerPosition = position;
    }

    public virtual bool IsTrap()
    {
        return false;
    }

    public abstract string GetInteractionMessage(IPlayerInteractionAvailableChecker checker);

    protected abstract void SetMediator();

    protected virtual void SetStorage()
    {
        // Do Nothing
    }

    private void Awake()
    {
        SetMediator();
    }
}
