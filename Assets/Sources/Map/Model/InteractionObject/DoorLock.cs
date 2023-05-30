using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : InteractionObject
{
    private const string UnlockDoorAnimationKey = "unlock";
    private const string UnlockDoorFastAnimationKey = "unlockFast";
    private const string InteractionMessage = "열기";

    [SerializeField] private Animator doorLockAnimator;
    [SerializeField] private Door[] lockedDoors;
    private bool isOpened = false;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        if (isOpened)
        {
            return;
        }
        isOpened = true;
        storage.RemoveLockedDoor();
        StartCoroutine(UnlockDoorCoroutine());

        playerEventHandler.ShowAdventureDialog("B01");

        string id = this.gameObject.transform.parent.GetComponent<Door>().GetId();
        MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        if (checker.CanUnlockDoor() && !isOpened)
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
        // Do nothing
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        isOpened = true;
        doorLockAnimator.SetBool(UnlockDoorFastAnimationKey, true);

        for (int i = 0; i < lockedDoors.Length; i++)
        {
            lockedDoors[i].SetDoorPassable(true);
        }
    }

    public void SetOpened(bool isOpened)
    {
        this.isOpened = isOpened;
    }

    protected override void SetMediator()
    {
        // Do Nothing
    }

    protected override void SetStorage()
    {
        if (isOpened)
        {
            return;
        }
        storage = this.gameObject.transform.parent.parent.parent.GetComponent<InteractionObjectCountStorage>();
        storage.AddLockedDoor();
    }

    private void Start()
    {
        SetStorage();
    }

    private IEnumerator UnlockDoorCoroutine()
    {
        doorLockAnimator.SetBool(UnlockDoorAnimationKey, true);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < lockedDoors.Length; i++)
        {
            lockedDoors[i].SetDoorPassable(true);
        }
    }
}
