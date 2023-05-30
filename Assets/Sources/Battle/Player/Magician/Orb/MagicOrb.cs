using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrb : MonoBehaviour
{
    private const string MagicOrbAppearAnimationKey = "appear";
    private const string MagicOrbIdleAnimationKey = "idle";
    private const string MagicOrbReinforceAnimationKey = "reinforce";
    private const string MagicOrbReinforceIdleAnimationKey = "idle";
    private const string MagicOrbShootAnimationKey = "shoot";

    [SerializeField] private Animator orbAnimator;
    [SerializeField] private Animator[] reinforceAnimator;
    [SerializeField] private Animator orbShootAnimator;
    private Element orbElement = Element.NONE;
    private int reinforceCount = 0;

    private Action onCameraChangeCallback;
    
    public void SetupMagicOrb(Element element, Action onCameraChange)
    {
        onCameraChangeCallback = onCameraChange;
        orbElement = element;
        orbAnimator.gameObject.SetActive(true);
        orbAnimator.SetInteger(MagicOrbAppearAnimationKey, (int)orbElement);
        StartCoroutine(OrbIdleCoroutine());
    }

    public Element GetElement()
    {
        return orbElement;
    }

    public void ReinforceOrb(Element element)
    {
        if (orbElement != element || reinforceCount > 0)
        {
            return;
        }

        reinforceCount++;

        for (int i = 0; i < 2; i++)
        {
            reinforceAnimator[i].gameObject.SetActive(true);
            reinforceAnimator[i].SetInteger(MagicOrbReinforceAnimationKey, (int)element);
            StartCoroutine(ReinforceIdleCoroutine());
        }
    }

    public float GetOrbDamage()
    {
        if (orbElement == Element.NONE)
        {
            return 0f;
        }
        else
        {
            return 3 * (1 + 0.5f * reinforceCount);
        } 
    }

    public void ShootOrb()
    {
        orbAnimator.speed = 0;
        orbShootAnimator.gameObject.SetActive(true);
        orbShootAnimator.SetInteger(MagicOrbShootAnimationKey, (int)orbElement);
        orbElement = Element.NONE;
        reinforceCount = 0;
    }

    public void HideOrb()
    {
        orbAnimator.SetInteger(MagicOrbAppearAnimationKey, 0);
        orbAnimator.SetBool(MagicOrbIdleAnimationKey, false);

        for (int i = 0; i < reinforceAnimator.Length; i++)
        {
            reinforceAnimator[i].SetInteger(MagicOrbReinforceAnimationKey, 0);
            reinforceAnimator[i].SetBool(MagicOrbReinforceIdleAnimationKey, false);
            reinforceAnimator[i].gameObject.SetActive(false);
        }

        orbAnimator.gameObject.SetActive(false);
        orbAnimator.speed = 1;
    }

    public void ChangeCameraMode()
    {
        onCameraChangeCallback.Invoke();
    }

    public void ResetOrbEffect()
    {
        orbShootAnimator.SetInteger(MagicOrbShootAnimationKey, 0);
    }

    public void ResetTotalMagicOrb()
    {
        HideOrb();
        ResetOrbEffect();

        orbElement = Element.NONE;
        reinforceCount = 0;
    }

    private IEnumerator OrbIdleCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        orbAnimator.SetBool(MagicOrbIdleAnimationKey, true);
        orbAnimator.SetInteger(MagicOrbAppearAnimationKey, 0);
    }

    private IEnumerator ReinforceIdleCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < 2; i++)
        {
            reinforceAnimator[i].SetBool(MagicOrbReinforceIdleAnimationKey, true);
        }
    }
}
