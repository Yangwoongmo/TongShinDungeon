using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureDialogObject : InteractionObject
{
    [SerializeField] private string dialogId;
    [SerializeField] private int dialogIndex;
    private bool isAlreadyUsed = false;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        isAlreadyUsed = true;
        playerEventHandler.ShowAdventureDialog(dialogId, dialogIndex);
        this.gameObject.SetActive(false);

        string id = this.gameObject.transform.parent.GetComponent<Floor>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        return null;
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return false;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        // Do nothing
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        isAlreadyUsed = true;
        this.gameObject.SetActive(false);
    }

    public bool IsAlreadyUsed()
    {
        return isAlreadyUsed;
    }

    protected override void SetMediator()
    {
        // Do nothing
    }
}
