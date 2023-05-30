using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : InteractionObject
{
    [SerializeField] protected List<Thorn> thornList;
    [SerializeField] protected List<IronBar> ironBarList;
    private bool isAlreadyTriggered = false;
    private bool isTerminated = false;
    private PlayerEventHandler handler;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        if (isTerminated || isAlreadyTriggered)
        {
            return;
        }

        isAlreadyTriggered = true;
        for (int i = 0; i < thornList.Count; i++)
        {
            thornList[i].StartThorn();
        }

        for (int i = 0; i < ironBarList.Count; i++)
        {
            ironBarList[i].CloseIronBar();
        }

        playerEventHandler.EnableSelfCameraButton(false);
        playerEventHandler.EnableSettingMenuButton(false);
        handler = playerEventHandler;
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
        isTerminated = true;
    }

    public virtual void StopTrigger()
    {
        if (isTerminated)
        {
            return;
        }
        isTerminated = true;

        for (int i = 0; i < thornList.Count; i++)
        {
            thornList[i].TerminateThorn();
        }

        for (int i = 0; i < ironBarList.Count; i++)
        {
            ironBarList[i].OpenIronBar();
        }

        string id = gameObject.GetComponent<Floor>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);

        handler.EnableSelfCameraButton(true);
        handler.EnableSettingMenuButton(true);
    }

    public void SetupTriggerInfo(Thorn[] thorns, IronBar[] ironBars)
    {
#if UNITY_EDITOR
        this.thornList = new List<Thorn>();
        this.thornList.Clear();
        this.thornList.AddRange(thorns);

        this.ironBarList = new List<IronBar>();
        this.ironBarList.Clear();
        this.ironBarList.AddRange(ironBars);
#endif
    }

    protected override void SetMediator()
    {
        // Do nothing
    }
}
