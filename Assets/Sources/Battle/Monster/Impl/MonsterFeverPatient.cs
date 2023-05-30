using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFeverPatient : Monster
{
    private const string Pattern1AnimationKey = "pattern1";
    private const string Pattern2AnimationKey = "pattern2";
    private const string Pattern3AnimationKey = "pattern3";

    private int[] successiveCountArray;

    public override void StartPattern(bool fromStun)
    {
        if (!fromStun)
        {
            successiveCountArray = new int[2] { 0, 0 };
        }
        StartCoroutine(AttackPatternCoroutine());
    }

    public override void ResetAllAnimation()
    {
        base.ResetAllAnimation();

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        monsterAnimator.SetInteger(Pattern2AnimationKey, 0);
        monsterAnimator.SetInteger(Pattern3AnimationKey, 0);
    }

    private void Pattern1()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern1Coroutine());
    }

    private void Pattern2()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern2Coroutine());
    }

    private void Pattern3()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern3Coroutine());
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
        actionObserver.FireMonsterAttack(30, PlayerDirection.CENTER);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator Pattern2Coroutine()
    {
        int attackDirection = Random.Range(1, 3);

        monsterAnimator.SetInteger(Pattern2AnimationKey, attackDirection);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, attackDirection);
        }

        yield return new WaitForSeconds(0.8f);
        currentStatus = MonsterStatus.STRONG_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(50, (PlayerDirection)attackDirection);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetInteger(Pattern2AnimationKey, 0);
        isPatternEnd = true;
    }

    private IEnumerator Pattern3Coroutine()
    {
        int firstAttackDirection = Random.Range(1, 3);
        int secondAttackDirection = 3 - firstAttackDirection;
        monsterAnimator.SetInteger(Pattern3AnimationKey, firstAttackDirection);

        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, firstAttackDirection + 10);
        }

        yield return new WaitForSeconds(0.4f);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, secondAttackDirection + 10);
        }

        yield return new WaitForSeconds(0.5f);
        currentStatus = MonsterStatus.STRONG_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(35, (PlayerDirection)firstAttackDirection);

        yield return new WaitForSeconds(0.55f);
        currentStatus = MonsterStatus.STRONG_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(35, (PlayerDirection)secondAttackDirection);

        yield return new WaitForSeconds(0.35f);
        monsterAnimator.SetInteger(Pattern3AnimationKey, 0);
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

            float rand = Random.Range(0f, 1f);
            if ((rand <= 0.5f && successiveCountArray[0] < 1) ||
                (rand > 0.5f && successiveCountArray[1] >= 1))
            {
                successiveCountArray[0] += 1;
                successiveCountArray[1] = 0;

                Pattern1();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                successiveCountArray[1] += 1;
                successiveCountArray[0] = 0;

                Pattern2();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();
                yield return new WaitForSeconds(1f);
            }

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);

            Pattern3();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);
        }
    }
}
