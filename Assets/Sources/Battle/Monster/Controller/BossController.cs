using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private const string PhaseChangeFadeInAnimationKey = "fadeIn";
    private const string PhaseChangeFadeOutAnimationKey = "fadeOut";

    [SerializeField] private Animator phaseChangeEffectUIAnimator;

    private string bossUIText;
    private float typingSpeed = 0.05f;
    private bool isStarting = false;
    private bool isPhaseChanging = false;

    public bool IsStarting => isStarting;
    public bool IsPhaseChanging => isPhaseChanging;

    public void BossBattleStartAnimation()
    {
        isStarting = true;
        StartCoroutine(BossBattleStartUICoroutine());
    }

    public void BossPhaseChangeUIAnimation()
    {
        isPhaseChanging = true;
        StartCoroutine(BossPhaseChangeUICoroutine());
    }

    private IEnumerator BossBattleStartUICoroutine()
    {
        GameObject bossPhaseTextObject = phaseChangeEffectUIAnimator.gameObject.transform.GetChild(0).gameObject;
        bossUIText = "보스 등장 텍스트";

        phaseChangeEffectUIAnimator.SetTrigger(PhaseChangeFadeInAnimationKey);

        yield return new WaitForSeconds(0.5f);

        bossPhaseTextObject.SetActive(true);
        StartCoroutine(BossPhaseChangeTextEffect(bossUIText));

        yield return new WaitForSeconds(2f);

        phaseChangeEffectUIAnimator.SetBool(PhaseChangeFadeOutAnimationKey, true);

        yield return new WaitForSeconds(1f);

        phaseChangeEffectUIAnimator.SetBool(PhaseChangeFadeOutAnimationKey, false);
        bossPhaseTextObject.SetActive(false);

        isStarting = false;
    }

    private IEnumerator BossPhaseChangeUICoroutine()
    {
        GameObject bossPhaseTextObject = phaseChangeEffectUIAnimator.gameObject.transform.GetChild(0).gameObject;
        bossUIText = "Test Text";

        phaseChangeEffectUIAnimator.SetTrigger(PhaseChangeFadeInAnimationKey);

        yield return new WaitForSeconds(0.5f);

        bossPhaseTextObject.SetActive(true);
        StartCoroutine(BossPhaseChangeTextEffect(bossUIText));

        yield return new WaitForSeconds(3.5f);

        phaseChangeEffectUIAnimator.SetBool(PhaseChangeFadeOutAnimationKey, true);

        yield return new WaitForSeconds(1f);

        phaseChangeEffectUIAnimator.SetBool(PhaseChangeFadeOutAnimationKey, false);
        bossPhaseTextObject.SetActive(false);
        isPhaseChanging = false;
    }

    private IEnumerator BossPhaseChangeTextEffect(string text)
    {
        Text phaseChangeText = phaseChangeEffectUIAnimator.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        phaseChangeText.text = "";

        foreach (var letter in text)
        {
            phaseChangeText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
