using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Wall
{
    private const string DoorOpenAnimationKey = "open";
    private const string OppositeDoorOpenAnimationKey = "openOpposite";

    [SerializeField] private bool isOppositeSide;
    [SerializeField] private bool isLocked;
    [SerializeField] private Animator doorAnimator;

    public override void SetDoorPassable(bool isPassable)
    {
        base.SetDoorPassable(isPassable);
        isLocked = false;
    }

    public void OpenDoor()
    {
        StartCoroutine(OpenDoorCoroutine());
    }

    public bool IsDoorLocked()
    {
        return isLocked;
    }

    private IEnumerator OpenDoorCoroutine()
    {
        string animationKey;
        if (isOppositeSide)
        {
            animationKey = OppositeDoorOpenAnimationKey;
        }
        else
        {
            animationKey = DoorOpenAnimationKey;
        }

        doorAnimator.SetBool(animationKey, true);

        yield return new WaitForSeconds(0.5f);

        doorAnimator.SetBool(animationKey, false);
    }
}
