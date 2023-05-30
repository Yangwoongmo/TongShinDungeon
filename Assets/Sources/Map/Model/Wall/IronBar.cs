using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBar : Wall
{
    private const string OpenIronBarAnimationKey = "open";
    private const string OpenIronBarFastAnimationKey = "openFast";
    private const string CloseIronBarAnimationKey = "close";
    [SerializeField] private Animator ironBarAnimator;
    [SerializeField] private bool openInDefault;
    [SerializeField] private bool isActivatedByTrigger = false;

    public void ToggleIronBar()
    {
        if (isPassable)
        {
            CloseIronBar();
        }
        else
        {
            OpenIronBar();
        }
    }

    public void OpenIronBar()
    {
        if (isPassable)
        {
            return;
        }

        StartCoroutine(OpenIronBarCoroutine());
    }

    public void CloseIronBar()
    {
        if (!isPassable)
        {
            return;
        }

        SetDoorPassable(false);
        ironBarAnimator.SetBool(CloseIronBarAnimationKey, true);
        ironBarAnimator.SetBool(OpenIronBarAnimationKey, false);

        if (!isActivatedByTrigger)
        {
            UpdateObjectState();
        }
    }

    public override bool UpdateChangeWithoutAnimationIfNeed()
    {
        if (base.UpdateChangeWithoutAnimationIfNeed())
        {
            if (!openInDefault)
            {
                ironBarAnimator.SetBool(OpenIronBarFastAnimationKey, true);
                SetDoorPassable(true);
            }
            return true;
        }
        else
        {
            if (openInDefault)
            {
                ironBarAnimator.SetBool(OpenIronBarFastAnimationKey, true);
                SetDoorPassable(true);
            }
            return false;
        }
    }

    public void SetupIronBarInfo(bool openInDefault, bool isActivatedByTrigger)
    {
#if UNITY_EDITOR
        this.openInDefault = openInDefault;
        this.isActivatedByTrigger = isActivatedByTrigger;
#endif
    }

    private IEnumerator OpenIronBarCoroutine()
    {
        ironBarAnimator.SetBool(OpenIronBarAnimationKey, true);
        ironBarAnimator.SetBool(CloseIronBarAnimationKey, false);
        yield return new WaitForSeconds(0.5f);

        SetDoorPassable(true);
        
        if (!isActivatedByTrigger)
        {
            UpdateObjectState();
        }
    }
}
