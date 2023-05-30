using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLantern : Monster
{
    private const string Pattern1AnimationKey = "pattern1";

    public override void StartPattern(bool fromStun)
    {
        StartCoroutine(AttackPatternCoroutine());
    }

    public override void ResetAllAnimation()
    {
        base.ResetAllAnimation();

        monsterAnimator.SetInteger(Pattern1AnimationKey, 0);
    }

    private void Pattern1()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern1Coroutine());
    }

    private IEnumerator Pattern1Coroutine()
    {
        int attackDirection = Random.Range(1, 3);

        monsterAnimator.SetInteger(Pattern1AnimationKey, attackDirection);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, attackDirection);
        }

        yield return new WaitForSeconds(0.8f);
        currentStatus = MonsterStatus.STRONG_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(40, (PlayerDirection)attackDirection);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetInteger(Pattern1AnimationKey, 0);
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
            yield return new WaitForSeconds(1.6f);
        }
    }
}
