using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMark : InteractionObject
{
    private const string InteractionMarkingMessage = "마킹";
    private const string InteractionRemoveMarkingMessage = "지우기";

    [SerializeField] protected GameObject marking;
    private bool isMarked = false;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        if (!isMarked)
        {
            if (!playerEventHandler.UsePaint())
            {
                return;
            }
            marking.SetActive(true);
        }
        else
        {
            marking.SetActive(false);
        }

        isMarked = !isMarked;

        string id = this.gameObject.transform.parent.GetComponent<Wall>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        if (checker.CanMarkWall())
        {
            return isMarked ? InteractionRemoveMarkingMessage : InteractionMarkingMessage;
        }
        else
        {
            return null;
        }
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return false;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        // Do Nothing
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        isMarked = true;
        marking.SetActive(true);
    }

    protected override void SetMediator()
    {
        // Do Nothing
    }
}
