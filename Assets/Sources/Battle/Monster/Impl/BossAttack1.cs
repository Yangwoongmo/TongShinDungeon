using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack1 : MonsterBloodStarvedBeast
{
    public override void StartPattern(bool fromStun)
    {
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
            Pattern1();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            yield return new WaitForSeconds(0.5f);

            Pattern2();
            yield return new WaitUntil(() => isPatternEnd);

            Idle();

            yield return new WaitForSeconds(1f);
        }
    }
}
