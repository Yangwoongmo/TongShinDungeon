using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : InteractionObject
{
    private const string PressSwitchAnimationKey = "pressSwitch";
    private const string PressSwitchFastAnimationKey = "pressSwitchFast";
    private const string RaiseSwitchAnimationKey = "raiseSwitch";
    private const string InteractionMessageLeverDown = "내리기";
    private const string InteractionMessageLeverRaise = "올리기";

    [SerializeField] private Animator switchAnimator;
    [SerializeField] private IronBar[] targetIronBars;
    [SerializeField] private bool isForMultipleUsage;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private Switch[] syncedSwitchList;
    [SerializeField] private Switch[] ironBarAssignedSwitchList;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        PressSwitch(true, playerEventHandler);
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        if (!isPressed)
        {
            return InteractionMessageLeverDown;
        }
        else
        {
            if (isForMultipleUsage)
            {
                return InteractionMessageLeverRaise;
            }
            else
            {
                return null;
            }
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
        isPressed = !isPressed;
    }

    public void SetupSwitchInfo(
        IronBar[] targetIronBars,
        bool isForMultipleUsage,
        bool isPressed,
        Switch[] syncedSwitchList,
        Switch[] ironBarAssignedSwitchList
    )
    {
#if UNITY_EDITOR
        this.targetIronBars = new IronBar[targetIronBars.Length];
        for (int i = 0; i < targetIronBars.Length; i++)
        {
            this.targetIronBars[i] = targetIronBars[i];
        }

        this.isForMultipleUsage = isForMultipleUsage;
        this.isPressed = isPressed;

        this.syncedSwitchList = new Switch[syncedSwitchList.Length];
        for (int i = 0; i < syncedSwitchList.Length; i++)
        {
            this.syncedSwitchList[i] = syncedSwitchList[i];
        }

        this.ironBarAssignedSwitchList = new Switch[ironBarAssignedSwitchList.Length];
        for (int i = 0; i < ironBarAssignedSwitchList.Length; i++)
        {
            this.ironBarAssignedSwitchList[i] = ironBarAssignedSwitchList[i];
        }
#endif
    }

    protected override void SetMediator()
    {
        // Do Nothing
    }

    private void Start()
    {
        if (isPressed)
        {
            switchAnimator.SetBool(PressSwitchFastAnimationKey, true);
        }
    }

    private void PressSwitch(bool isSyncNeeded, PlayerEventHandler playerEventHandler)
    {
        if (isPressed && !isForMultipleUsage)
        {
            return;
        }

        isPressed = !isPressed;
        switchAnimator.SetBool(PressSwitchAnimationKey, isPressed);
        switchAnimator.SetBool(RaiseSwitchAnimationKey, !isPressed);

        string id = this.gameObject.transform.parent.GetChild(1).GetComponent<Wall>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);

        if (isSyncNeeded)
        {
            playerEventHandler.ShowAdventureDialog("B00");

            for (int i = 0; i < syncedSwitchList.Length; i++)
            {
                syncedSwitchList[i].PressSwitch(false, playerEventHandler);
            }
        }

        if (ironBarAssignedSwitchList.Length > 0)
        {
            bool canOpenIronBar = isPressed;

            for (int i = 0; i < ironBarAssignedSwitchList.Length; i++)
            {
                canOpenIronBar &= ironBarAssignedSwitchList[i].isPressed;
            }

            if (canOpenIronBar)
            {
                for (int i = 0; i < targetIronBars.Length; i++)
                {
                    targetIronBars[i].ToggleIronBar();
                }
            }
        }
        else
        {
            for (int i = 0; i < targetIronBars.Length; i++)
            {
                targetIronBars[i].ToggleIronBar();
            }
        }
    }
}
