using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifyButton : InteractionObject
{
    private const string InteractionMessage = "정화";

    [SerializeField] private SanctuaryPortal[] portals;
    [SerializeField] private bool isAlreadyPurified;
    private bool isPurified = false;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        if (isPurified)
        {
            return;
        }

        isPurified = true;
        storage.RemoveSanctuaryPortal();

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].SetPurified(true);
        }

        string id = this.gameObject.transform.parent.GetComponent<SanctuaryPortal>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);

        playerEventHandler.ShowAdventureDialog("I03");
        playerEventHandler.SaveDataOnPurify(portals[0].GetPortalId());
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        if (!isPurified)
        {
            return InteractionMessage;
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
        isPurified = true;
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].SetPurified(true);
        }
    }

    protected override void SetMediator()
    {
        // Do Nothing
    }

    protected override void SetStorage()
    {
        if (isPurified)
        {
            return;
        }
        storage = this.gameObject.transform.parent.parent.parent.GetComponent<InteractionObjectCountStorage>();
        storage.AddSanctuaryPortal();
    }

    private void Start()
    {
        SetStorage();
    }

    private void Awake()
    {
        if (isAlreadyPurified)
        {
            isPurified = true;
            for (int i = 0; i < portals.Length; i++)
            {
                portals[i].SetPurified(true);
            }
        }
    }
}
