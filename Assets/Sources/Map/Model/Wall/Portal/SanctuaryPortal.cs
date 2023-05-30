using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryPortal : Portal
{
    private const string SanctuaryGlowAnimationKey = "purify";

    [SerializeField] private MeshRenderer[] portalWhites;
    [SerializeField] private Material[] portalWhiteMaterials;
    [SerializeField] private int portalId;
    [SerializeField] private Animator portalGlowAnimator;
    private bool isPurified = false;

    public override bool MoveToTargetArea(PlayerEventHandler eventHandler)
    {
        if (!isPurified)
        {
            return false;
        }
        eventHandler.MoveToTargetScene(targetSceneName, PlayerEventHandler.PortalType.SANCTUARY, portalId);
        return true;
    }

    public void SetPurified(bool isPurified)
    {
        this.isPurified = isPurified;
        for (int i = 0; i < portalWhites.Length; i++)
        {
            portalWhites[i].material = isPurified ? portalWhiteMaterials[1] : portalWhiteMaterials[0];
        }

        if (isPurified)
        {
            portalGlowAnimator.SetTrigger(SanctuaryGlowAnimationKey);
        }
    }

    public int GetPortalId()
    {
        return portalId;
    }

    public void SetupPortalInfo()
    {
#if UNITY_EDITOR
        targetSceneName = "SanctuaryScene";
        portalId = 0;
#endif
    }
}
