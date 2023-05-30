using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warrior : MonoBehaviour, IWarriorHitHandler
{
    private const string LeftAvoidAnimationKey = "leftAvoid";
    private const string RightAvoidAnimationKey = "rightAvoid";
    private const string NormalAttackAnimationKey = "normalAttack";
    private const string PokeAnimationKey = "poke";
    private const string HitAnimationKey = "damage";
    private const string DieAnimationKey = "die";
    private const string AppearAnimationKey = "appear";
    private const string DisappearAnimationKey = "disappear";
    private const string ParryingEffectAnimationKey = "parrying";
    private const string PreparePoseAnimationkey = "prepare";

    [SerializeField] private WarriorStatus currentStatus;
    [SerializeField] private Animator warriorAnimator;
    [SerializeField] private Animator parryingAnimator;

    private IWarriorStatusObserver warriorStatusObserver;
    private IBattleActionObserver actionObserver;
    private Coroutine currentWarriorActionCoroutine;

    private int animationPhase = 1;

    private int hitCount = 0;
    private int hitDamage = 0;

    [SerializeField] private bool isAttackReinforced = false;

    public void SetWarriorStatusObserver(IWarriorStatusObserver observer)
    {
        this.warriorStatusObserver = observer;
    }

    public void SetBattleActionObserver(IBattleActionObserver observer)
    {
        actionObserver = observer;
    }

    public void Idle()
    {
        currentStatus = WarriorStatus.IDLE;
        if (actionObserver != null)
        {
            actionObserver.UnregisterWarriorAction();
        }    
    }

    public void Die()
    {
        ResetWarrior();
        currentStatus = WarriorStatus.DEATH;
        actionObserver.UnregisterWarriorAction();
        warriorAnimator.SetBool(DieAnimationKey, true);
    }

    public void Appear()
    {
        StartCoroutine(AppearCoroutine());
        ResetWarriorActionCoroutine();
        if (actionObserver != null)
        {
            actionObserver.UnregisterWarriorAction();
        }
    }

    public void Disappear()
    {
        warriorAnimator.SetBool(DisappearAnimationKey, true);
    }

    public void ResetWarrior()
    {
        StopAllCoroutines();
        ResetWarriorActionCoroutine();
        ResetAllAnimation();
        animationPhase = 1;
        hitDamage = 0;
        hitCount = 0;
        Idle();
        parryingAnimator.gameObject.SetActive(false);
        isAttackReinforced = false;
        if (actionObserver != null)
        {
            actionObserver.UnregisterWarriorAction();
        }
    }

    public void LeftAvoid()
    {
        if (!CanInvokeNextAction())
        {
            return;
        }

        currentWarriorActionCoroutine = StartCoroutine(LeftAvoidCoroutine());
    }

    public void RightAvoid()
    {
        if (!CanInvokeNextAction())
        {
            return;
        }

        currentWarriorActionCoroutine = StartCoroutine(RightAvoidCoroutine());
    }

    public bool Hit(int damage)
    {
        if (currentStatus == WarriorStatus.INVINCIBLE || currentStatus == WarriorStatus.DEATH || damage == 0)
        {
            return false;
        }

        if (currentWarriorActionCoroutine != null)
        {
            StopCoroutine(currentWarriorActionCoroutine);
        }
        ResetWarriorActionCoroutine();
        ResetAllAnimation();

        actionObserver.UnregisterWarriorAction();
        currentWarriorActionCoroutine = StartCoroutine(HitCoroutine());
        isAttackReinforced = false;
        bool isDead = false;

        hitCount++;
        hitDamage += damage;

        if (warriorStatusObserver != null)
        {
            isDead = !warriorStatusObserver.WarriorHitDamage(damage);
        }

        if (currentStatus != WarriorStatus.DEATH && isDead)
        {
            Debug.Log("Warrior is Dead");
            Die();
        }
        return false;
    }

    public void NormalAttack(int str, int dex, bool isParryingAvailable, bool isPrepareAvailable)
    {
        if (currentStatus == WarriorStatus.DEATH || currentWarriorActionCoroutine != null)
        {
            return;
        }

        currentWarriorActionCoroutine = StartCoroutine(NormalAttackCoroutine(str, dex, isParryingAvailable, isPrepareAvailable));
    }

    public bool Poke(int str, int dex, bool isParryingAvailable, bool isClashAvailable)
    {
        if (!CanInvokeNextAction())
        {
            return false;
        }

        currentWarriorActionCoroutine = StartCoroutine(PokeCoroutine(str, dex, isParryingAvailable, isClashAvailable));
        return true;
    }

    public Coroutine GetWarriorAttackCoroutine()
    {
        return currentWarriorActionCoroutine;
    }

    public WarriorBattleStatistic GetStatistic()
    {
        return new WarriorBattleStatistic(hitCount, hitDamage);
    }

    public void SetWarriorInvicible()
    {
        currentStatus = WarriorStatus.INVINCIBLE;
    } 

    public bool IsWarriorInAction()
    {
        return currentWarriorActionCoroutine != null;
    }

    private IEnumerator HitCoroutine()
    {
        currentStatus = WarriorStatus.INVINCIBLE;
        warriorAnimator.SetInteger(HitAnimationKey, animationPhase);

        yield return new WaitForSeconds(0.6f);
        currentStatus = WarriorStatus.IDLE;
        warriorAnimator.SetInteger(HitAnimationKey, -animationPhase);

        ResetWarriorActionCoroutine();
    }

    private IEnumerator LeftAvoidCoroutine()
    {
        int type = Random.Range(1, 3);
        actionObserver.RegisterWarriorAction(WarriorStatus.LEFT_AVOID);
        currentStatus = WarriorStatus.LEFT_AVOID;
        warriorAnimator.SetInteger(LeftAvoidAnimationKey, type);

        yield return new WaitForSeconds(0.5f);
        warriorAnimator.SetInteger(LeftAvoidAnimationKey, animationPhase + 3);
        actionObserver.UnregisterWarriorAction();
        currentStatus = WarriorStatus.IDLE;

        yield return new WaitForSeconds(0.1f);

        ResetWarriorActionCoroutine();
    }

    private IEnumerator RightAvoidCoroutine()
    {
        int type = Random.Range(1, 3);
        actionObserver.RegisterWarriorAction(WarriorStatus.RIGHT_AVOID);
        currentStatus = WarriorStatus.RIGHT_AVOID; 
        warriorAnimator.SetInteger(RightAvoidAnimationKey, type);  

        yield return new WaitForSeconds(0.5f);
        warriorAnimator.SetInteger(RightAvoidAnimationKey, animationPhase + 3);
        actionObserver.UnregisterWarriorAction();
        currentStatus = WarriorStatus.IDLE;

        yield return new WaitForSeconds(0.1f);
        
        ResetWarriorActionCoroutine();
    }

    private IEnumerator NormalAttackCoroutine(int str, int dex, bool isParryingAvailable, bool isPrepareAvailable)
    {
        animationPhase++;
        if (animationPhase > 3)
        {
            animationPhase = 1;
        }
        warriorAnimator.SetBool(NormalAttackAnimationKey, true);

        yield return new WaitForSeconds(0.15f);
        currentStatus = WarriorStatus.ATTACK;
        actionObserver.RegisterWarriorAction(currentStatus);

        yield return new WaitForSeconds(0.15f);

        (int reinforcedStr, int reinforcedDex) = CalculateReinforcedAttack(str, dex);
        isAttackReinforced = false;

        bool parryingResult = actionObserver.FireWarriorAttack(reinforcedStr, reinforcedDex);
        if (parryingResult)
        {
            StartCoroutine(ShowParryingEffectCoroutine());
        }

        currentStatus = WarriorStatus.IDLE;

        yield return new WaitForSeconds(0.4f);
        
        currentStatus = WarriorStatus.AFTER_DELAY;
        warriorAnimator.SetBool(PreparePoseAnimationkey, isPrepareAvailable);

        yield return new WaitForSeconds(0.1f);
        isAttackReinforced = isPrepareAvailable;

        yield return new WaitForSeconds(0.1f);
        warriorAnimator.SetBool(NormalAttackAnimationKey, false);
        warriorAnimator.SetBool(PreparePoseAnimationkey, false);

        StopAttackAction();
    }

    private IEnumerator PokeCoroutine(int str, int dex, bool isParryingAvailable, bool isClashAvailable)
    {
        warriorAnimator.SetBool(PokeAnimationKey, true);

        yield return new WaitForSeconds(0.2f);
        if (isClashAvailable)
        {
            currentStatus = WarriorStatus.CLASH;
        } 
        else
        {
            currentStatus = WarriorStatus.ATTACK;
        }
        actionObserver.RegisterWarriorAction(currentStatus);
        
        yield return new WaitForSeconds(0.4f);

        (int reinforcedStr, int reinforcedDex) = CalculateReinforcedAttack(str * 3, dex * 4);
        isAttackReinforced = false;

        bool parryingResult = actionObserver.FireWarriorAttack(reinforcedStr, reinforcedDex);
        if (parryingResult)
        {
            StartCoroutine(ShowParryingEffectCoroutine());
        }

        currentStatus = WarriorStatus.AFTER_DELAY;

        yield return new WaitForSeconds(0.4f);
        StopAttackAction();
    }

    private IEnumerator AppearCoroutine()
    {
        warriorAnimator.SetBool(AppearAnimationKey, true);
        yield return new WaitForSeconds(0.5f);
        warriorAnimator.SetBool(AppearAnimationKey, false);
    }

    private void ResetAllAnimation()
    {
        warriorAnimator.SetInteger(LeftAvoidAnimationKey, 0);
        warriorAnimator.SetInteger(RightAvoidAnimationKey, 0);
        warriorAnimator.SetInteger(HitAnimationKey, 0);
        warriorAnimator.SetBool(NormalAttackAnimationKey, false);
        warriorAnimator.SetBool(PokeAnimationKey, false);
        warriorAnimator.SetBool(DieAnimationKey, false);
        warriorAnimator.SetBool(AppearAnimationKey, false);
        warriorAnimator.SetBool(DisappearAnimationKey, false);
        warriorAnimator.SetBool(PreparePoseAnimationkey, false);

        parryingAnimator.SetBool(ParryingEffectAnimationKey, false);
    }

    private void ResetWarriorActionCoroutine()
    {
        currentWarriorActionCoroutine = null;
    }

    private void Start()
    {
        Idle();
    }

    private bool CanInvokeNextAction()
    {
        if (currentStatus == WarriorStatus.DEATH)
        {
            return false;
        }

        if (currentWarriorActionCoroutine == null)
        {
            return true;
        }
        
        if (currentStatus == WarriorStatus.AFTER_DELAY)
        {
            StopCoroutine(currentWarriorActionCoroutine);
            StopAttackAction();
            return true;
        }

        return false;
    }

    private void StopAttackAction()
    {
        currentStatus = WarriorStatus.IDLE;
        ResetAllAnimation();

        ResetWarriorActionCoroutine();
    }

    private IEnumerator ShowParryingEffectCoroutine()
    {
        parryingAnimator.gameObject.SetActive(true);
        parryingAnimator.SetBool(ParryingEffectAnimationKey, true);

        yield return new WaitForSeconds(0.35f);
        parryingAnimator.SetBool(ParryingEffectAnimationKey, false);
        parryingAnimator.gameObject.SetActive(false);
    }

    private (int, int) CalculateReinforcedAttack(int str, int dex)
    {
        float multiplier = isAttackReinforced ? 1.2f : 1f;
        return (Mathf.RoundToInt(str * multiplier), Mathf.RoundToInt(dex * multiplier));
    }
}
