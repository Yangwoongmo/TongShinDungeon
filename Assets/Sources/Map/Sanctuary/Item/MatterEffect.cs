using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatterEffect : MonoBehaviour
{
    private const string FadeAnimationKey = "fade";

    [SerializeField] private Text effectText;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform effectPool;

    public void StartEffect(string text, Transform effectPool)
    {
        gameObject.SetActive(true);

        this.effectPool = effectPool;
        effectText.text = text;
        animator.SetBool(FadeAnimationKey, true);
    }

    public void ResetEffect()
    {
        animator.SetBool(FadeAnimationKey, false);
        gameObject.SetActive(false);
        transform.SetParent(effectPool);
    }
}
