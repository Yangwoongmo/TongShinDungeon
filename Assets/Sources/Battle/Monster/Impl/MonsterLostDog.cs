using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLostDog : Monster
{
    private const string Pattern1AnimationKey = "pattern1";

    public override void StartPattern(bool fromStun)
    {
        StartCoroutine(AttackPatternCoroutine());
    }

    public override void ResetAllAnimation()
    {
        base.ResetAllAnimation();

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
    }

    private void Pattern1()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern1Coroutine());
    }

    private IEnumerator Pattern1Coroutine()
    {
        monsterAnimator.SetBool(Pattern1AnimationKey, true);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, 4);
        }

        yield return new WaitForSeconds(0.8f);
        currentStatus = MonsterStatus.WEAK_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(10, PlayerDirection.CENTER);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator AttackPatternCoroutine()
    {
        Idle();

        while (!IsDead())
        {
            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);
        }
    }
}
