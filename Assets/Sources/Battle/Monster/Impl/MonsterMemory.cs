using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMemory : Monster
{
    private const string Pattern1AnimationKey = "pattern1";
    private const string Pattern2AnimationKey = "pattern2";
    private const string Pattern3AnimationKey = "pattern3";

    private int[] successiveCountArray;
    private int controlBlockAccessCount;

    public override void StartPattern(bool fromStun)
    {
        if (!fromStun)
        {
            successiveCountArray = new int[2] { 0, 0 };
            controlBlockAccessCount = 0;
        }
        StartCoroutine(AttackPatternCoroutine());
    }

    public override void ResetAllAnimation()
    {
        base.ResetAllAnimation();

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        monsterAnimator.SetInteger(Pattern2AnimationKey, 0);
        monsterAnimator.SetBool(Pattern3AnimationKey, false);
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
        actionObserver.FireMonsterAttack(30, (PlayerDirection)attackDirection);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetInteger(Pattern2AnimationKey, 0);
        isPatternEnd = true;
    }

    private IEnumerator Pattern3Coroutine()
    {
        monsterAnimator.SetBool(Pattern3AnimationKey, true);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, 3);
        }

        yield return new WaitForSeconds(0.8f);
        currentStatus = MonsterStatus.STRONG_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(50, PlayerDirection.BOTH);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetBool(Pattern3AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator AttackPatternCoroutine()
    {
        Idle();

        while (!IsDead())
        {
            controlBlockAccessCount++;
            if (controlBlockAccessCount % 4 > 0)
            {
                float rand = Random.Range(0f, 1f);
                if ((rand <= 0.6f && successiveCountArray[0] < 2) ||
                    (rand > 0.6f && successiveCountArray[1] >= 1))
                {
                    successiveCountArray[0] += 1;
                    successiveCountArray[1] = 0;

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                }
                else
                {
                    successiveCountArray[1] += 1;
                    successiveCountArray[0] = 0;

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(1.6f);
                }
            }
            else
            {
                Pattern3();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
