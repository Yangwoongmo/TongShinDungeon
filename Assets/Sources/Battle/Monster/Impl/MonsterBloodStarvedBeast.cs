using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBloodStarvedBeast : Monster
{
    private const string Pattern1AnimationKey = "pattern1";
    private const string Pattern2LeftAnimationKey = "pattern2Left";
    private const string Pattern2RightAnimationKey = "pattern2Right";
    private const string Pattern3AnimationKey = "pattern3";
    private const string Phase2AnimationKey = "phase2";

    private bool isPhase2 = false;
    private int[] successiveCountArray;
    private int controlBlockAccessCount;

    public bool GetIsPhase2 => isPhase2;

    public override void StartPattern(bool fromStun)
    {
        if (!fromStun)
        {
            successiveCountArray = new int[3] { 0, 0, 0 };
            controlBlockAccessCount = 0;
        }

        if (isPhase2)
        {
            StartCoroutine(Phase2AttackPatternCoroutine(fromStun));
        }
        else
        {
            StartCoroutine(AttackPatternCoroutine(fromStun));
        }
    }

    public override void ResetAllAnimation()
    {
        base.ResetAllAnimation();

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        monsterAnimator.SetBool(Pattern2LeftAnimationKey, false);
        monsterAnimator.SetBool(Pattern2RightAnimationKey, false);
        monsterAnimator.SetBool(Pattern3AnimationKey, false);
    }

    public override void StopMonster(bool byMonsterDeath = false)
    {
        base.StopMonster(byMonsterDeath);
        isPhase2 = false;
    }

    public void BossPhaseChangeAnimation()
    {
        StartCoroutine(BossPhaseChangeAnimationCoroutine());
    }

    protected override void HitDamage(int damage)
    {
        base.HitDamage(damage);
        if (GetHpPercentage() <= 0.5f)
        {
            if (isPhase2)
            {
                return;
            }
            StopMonster();
            isPhase2 = true;
            // TODO : Some animation effect
        }
    }

    protected void Pattern1()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern1Coroutine());
    }

    protected void Pattern2()
    {
        isPatternEnd = false;
        StartCoroutine(Pattern2Coroutine());
    }

    protected void Pattern3()
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

        yield return new WaitForSeconds(0.15f);

        monsterAnimator.SetBool(Pattern1AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator Pattern2Coroutine()
    {
        int attackDirection = Random.Range(1, 3);
        string pattern2Direction = attackDirection == 1 ? Pattern2LeftAnimationKey : Pattern2RightAnimationKey;

        monsterAnimator.SetBool(pattern2Direction, true);
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

        yield return new WaitForSeconds(0.15f);

        monsterAnimator.SetBool(pattern2Direction, false);
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
        actionObserver.FireMonsterAttack(40, PlayerDirection.BOTH);

        yield return new WaitForSeconds(0.15f);

        monsterAnimator.SetBool(Pattern3AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator AttackPatternCoroutine(bool fromStun)
    {
        Idle();
        if (!fromStun)
        {
            yield return new WaitForSeconds(1f);
        }

        while (!IsDead())
        {
            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            yield return new WaitForSeconds(0.5f);

            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern3();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(1f);

            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            Pattern3();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(0.5f);

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(0.5f);

            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            yield return new WaitForSeconds(0.5f);

            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Phase2AttackPatternCoroutine(bool fromStun)
    {
        Idle();
        if (!fromStun)
        {
            yield return new WaitForSeconds(1f);
        }

        while (!IsDead())
        {
            controlBlockAccessCount++;
            if (controlBlockAccessCount % 5 > 0 || fromStun)
            {
                float rand = Random.Range(0f, 1f);
                if ((rand < 0.33 && successiveCountArray[0] == 0) ||
                    (successiveCountArray[1] > 0 && successiveCountArray[2] > 0))
                {
                    SelectSuccessiveAction(0);

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern3();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(2f);
                }
                else if ((rand < 0.66 && successiveCountArray[1] == 0) ||
                        (successiveCountArray[0] > 0 && successiveCountArray[2] > 0))
                {
                    SelectSuccessiveAction(1);

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern3();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    SelectSuccessiveAction(2);

                    Pattern3();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern3();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(2f);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);

                Pattern1();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern2();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern3();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern2();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern1();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern3();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern2();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                Pattern2();
                yield return new WaitUntil(() => isPatternEnd);

                Idle();

                yield return new WaitForSeconds(3f);
            }
        }
    }

    private IEnumerator BossPhaseChangeAnimationCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        monsterAnimator.SetTrigger(Phase2AnimationKey);

        yield return new WaitForSeconds(3.5f);

        monsterSprite.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(2f);

        StartPattern(false);
    }

    private void SelectSuccessiveAction(int index)
    {
        for (int i = 0; i < successiveCountArray.Length; i++)
        {
            if (i == index)
            {
                successiveCountArray[i] += 1;
            }
            else
            {
                successiveCountArray[i] = 0;
            }
        }
    }
}
