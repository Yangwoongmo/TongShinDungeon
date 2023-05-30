using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExploreController : MonoBehaviour
{
    private const string ButtonFadeAnimationKey = "fade";

    [SerializeField] private Animator[] buttonFadeAnimators;

    private bool isFirstButtonShown = false;
    private WaitForSeconds waitForShowingFirstButton = new WaitForSeconds(0.5f);

    public void ShowMovingButtonAt(int index)
    {
        buttonFadeAnimators[index].gameObject.SetActive(true);
        buttonFadeAnimators[index].SetBool(ButtonFadeAnimationKey, true);
    }

    public void ShowFirstMovingButtonIfNeed()
    {
        if (isFirstButtonShown)
        {
            return;
        }

        isFirstButtonShown = true;
        StartCoroutine(ShowFirstMovingButtonCoroutine());
    }

    private IEnumerator ShowFirstMovingButtonCoroutine()
    {
        yield return waitForShowingFirstButton;
        ShowMovingButtonAt(0);
    }
}
