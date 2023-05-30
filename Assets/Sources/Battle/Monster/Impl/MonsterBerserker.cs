using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBerserker : Monster
{
    private const string Pattern1AnimationKey = "pattern1";
    private const string Pattern2AnimationKey = "pattern2";
    private const string Pattern3AnimationKey = "pattern3";

    private List<int> boxPatternList;

    public override void StartPattern(bool fromStun)
    {
        if (!fromStun)
        {
            boxPatternList = new List<int>() { 0, 1, 2 };
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
        actionObserver.FireMonsterAttack(50, (PlayerDirection)attackDirection);

        yield return new WaitForSeconds(0.35f);

        monsterAnimator.SetInteger(Pattern2AnimationKey, 0);
        isPatternEnd = true;
    }

    private IEnumerator Pattern3Coroutine()
    {
        monsterAnimator.SetBool(Pattern3AnimationKey, true);

        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, 14);
        }

        yield return new WaitForSeconds(0.4f);
        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterAttackWarning(AttackWarningAnimationKey, 14);
        }

        yield return new WaitForSeconds(0.5f);
        currentStatus = MonsterStatus.WEAK_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(35, PlayerDirection.CENTER);

        yield return new WaitForSeconds(0.55f);
        currentStatus = MonsterStatus.WEAK_ATTACK;
        actionObserver.RegisterMonsterAction(currentStatus);

        yield return new WaitForSeconds(0.05f);
        currentStatus = MonsterStatus.IDLE;
        actionObserver.FireMonsterAttack(35, PlayerDirection.CENTER);

        yield return new WaitForSeconds(0.35f);
        monsterAnimator.SetBool(Pattern3AnimationKey, false);
        isPatternEnd = true;
    }

    private IEnumerator AttackPatternCoroutine()
    {
        Idle();

        while (!IsDead())
        {
            if (boxPatternList.Count == 0)
            {
                boxPatternList.Add(0);
                boxPatternList.Add(1);
                boxPatternList.Add(2);
            }

            int rand = Random.Range(0, boxPatternList.Count);
            int nextPattern = boxPatternList[rand];
            boxPatternList.RemoveAt(rand);

            switch (nextPattern)
            {
                case 0:
                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(1f);
                    break;
                case 1:
                    Pattern3();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(1f);
                    break;
                case 2:
                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(1f);
                    break;
            }
        }
    }
}
