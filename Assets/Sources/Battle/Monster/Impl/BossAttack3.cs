using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack3 : MonsterBloodStarvedBeast
{
    private List<int> boxPatternList;

    public override void StartPattern(bool fromStun)
    {
        if (!fromStun)
        {
            boxPatternList = new List<int>() { 0, 1, 2 };
        }
        StartCoroutine(AttackPatternCoroutine(fromStun));
    }

    protected override void HitDamage(int damage)
    {
        return;
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
            if(boxPatternList.Count == 0)
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
                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(0.5f);
                    break;
                case 1:
                    Pattern1();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();

                    Pattern3();
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

                    Pattern2();
                    yield return new WaitUntil(() => isPatternEnd);

                    Idle();
                    yield return new WaitForSeconds(1f);
                    break;
            }
        }
    }
}
