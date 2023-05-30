using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IMonsterHitHandler
{
    protected const string StunAnimationKey = "stun";
    protected const string DieAnimationKey = "die";
    protected const string AttackWarningAnimationKey = "warning";
    protected const string FakeAttackWarningAnimationKey = "fake";

    [SerializeField] protected string monsterName;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int hpRecovery;
    [SerializeField] protected int maxPp;
    [SerializeField] protected float ppRecovery;
    [SerializeField] protected float[] weakElement = new float[4];
    [SerializeField] protected MonsterStatus currentStatus;
    [SerializeField] protected Animator monsterAnimator;
    [SerializeField] protected List<MonsterReward> rewardList;
    [SerializeField] protected SpriteRenderer monsterSprite;
    [SerializeField] private Color nameColor;
    [SerializeField] private int monsterId;
    [SerializeField] private SpriteRenderer dieSprite;
    [SerializeField] private SpriteRenderer[] visibilityChangeRequiredSprites;
    protected int hp;
    protected float pp;
    protected bool isPatternEnd = true;
    protected IMonsterStatusObserver monsterStatusObserver;
    protected IBattleActionObserver actionObserver;

    protected virtual float monsterSpawnCord1 => 10f;
    protected virtual float monsterSpawnCord2 => -0.18f;

    public abstract void StartPattern(bool fromStun = false);

    public virtual void Idle()
    {
        currentStatus = MonsterStatus.IDLE;
        actionObserver.UnregisterMonsterAction();
    }

    public virtual void ResetAllAnimation()
    {
        monsterAnimator.SetBool(StunAnimationKey, false);
        monsterAnimator.SetBool(DieAnimationKey, false);

        ResetAttackWarningAnimations();
    }

    public virtual void Stun()
    {
        StopAllCoroutines();
        ResetAllAnimation();

        currentStatus = MonsterStatus.STUN;
        actionObserver.UnregisterMonsterAction();
        monsterAnimator.SetBool(StunAnimationKey, true);

        StartCoroutine(RecoverPpCoroutine());
    }

    public virtual void Die()
    {
        StopMonster(true);
        currentStatus = MonsterStatus.DEATH;
        actionObserver.UnregisterMonsterAction();
    }

    public virtual bool HitFromWarrior(int damage, int pp)
    {
        CalculateHpAndPp(damage, pp);
        return true;
    }

    public virtual void HitFromMagician(float[] damageList, float awakeAddition, float stunAddition)
    {
        int realDamage = 0;
        float realAwakeAddition = currentStatus != MonsterStatus.STUN ? awakeAddition : 0;
        float realStunAddition = currentStatus == MonsterStatus.STUN ? stunAddition : 0;

        for (int i = 0; i < damageList.Length; i++)
        {
            realDamage += Mathf.RoundToInt(damageList[i] * weakElement[i + 1] * (1 + realAwakeAddition + realStunAddition));
        }
        CalculateHpAndPp(realDamage, 0);
    }

    public virtual void PlayDeathAnimation()
    {
        dieSprite.sprite = monsterSprite.sprite;
        monsterAnimator.SetBool(DieAnimationKey, true);
    }

    public virtual void StopMonster(bool byMonsterDeath = false)
    {
        StopAllCoroutines();
        if (byMonsterDeath)
        {
            PlayDeathAnimation();
            ResetAttackWarningAnimations();
            
        }
        else
        {
            ResetAllAnimation();
        }
        isPatternEnd = true;
        currentStatus = MonsterStatus.IDLE;
        actionObserver.UnregisterMonsterAction();
        this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void ChangeMonsterSpriteVisibility(bool isVisible)
    {
        for (int i = 0; i < visibilityChangeRequiredSprites.Length; i++)
        {
            visibilityChangeRequiredSprites[i].enabled = isVisible;
        }
        monsterSprite.maskInteraction = 
            isVisible ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
    }

    public bool IsDead()
    {
        return hp <= 0;
    }

    public bool IsStun()
    {
        return pp >= maxPp;
    }

    public (float, float) GetMonsterSpawnXZPosition(float playerRotation)
    {
        float[] monsterPositionX = new float[4] { -monsterSpawnCord1, monsterSpawnCord2, monsterSpawnCord1, -monsterSpawnCord2 };
        float[] monsterPositionZ = new float[4] { monsterSpawnCord2, monsterSpawnCord1, -monsterSpawnCord2, -monsterSpawnCord1 };

        int minimumRotation = (int)playerRotation % 360;
        if (minimumRotation < 0)
        {
            minimumRotation += 360;
        }

        return (monsterPositionX[minimumRotation / 90], monsterPositionZ[minimumRotation / 90]);
    }

    public void SetMonsterStatusObserver(IMonsterStatusObserver observer)
    {
        this.monsterStatusObserver = observer;
    }

    public void SetBattleActionObserver(IBattleActionObserver observer)
    {
        actionObserver = observer;
    }

    public float GetHpPercentage()
    {
        return (float)hp / (float)maxHp;
    }

    public float GetPpPercentage()
    {
        return (float)pp / (float)maxPp;
    }

    public string GetMonsterName()
    {
        return monsterName;
    }

    public int GetMonsterId()
    {
        return monsterId;
    }

    public void InitializeMonster()
    {
        this.hp = maxHp;
        this.pp = 0;
    }

    public List<MonsterReward> GetRewardList()
    {
        return rewardList;
    }

    public Color GetMonsterNameColor()
    {
        return nameColor;
    }

    protected virtual void HitDamage(int damage)
    {
        if (currentStatus == MonsterStatus.DEATH)
        {
            return;
        }
        
        this.hp -= damage;

        if (this.hp <= 0)
        {
            this.hp = 0;
        }

        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterHitDamage();
        }
    }

    protected void HitPp(int pp)
    {
        if (currentStatus == MonsterStatus.STUN)
        {
            return;
        }

        this.pp += pp;
        if (this.pp >= maxPp)
        {
            this.pp = maxPp;
        }

        if (monsterStatusObserver != null)
        {
            monsterStatusObserver.MonsterHitPp();
        }
    }

    private void ResetAttackWarningAnimations()
    {
        monsterStatusObserver.ResetMonsterWarning(AttackWarningAnimationKey);
        monsterStatusObserver.ResetMonsterWarning(FakeAttackWarningAnimationKey);
    }

    private void CalculateHpAndPp(int damage, int pp)
    {
        HitDamage(damage);
        HitPp(pp);

        if (currentStatus != MonsterStatus.DEATH && IsDead())
        {
            Die();
            return;
        }

        if (currentStatus != MonsterStatus.STUN && IsStun())
        {
            Stun();
        }
    }

    private IEnumerator RecoverPpCoroutine()
    {
        while (pp > 0)
        {
            yield return new WaitForSeconds(0.1f);
            pp -= ppRecovery;
            if (monsterStatusObserver != null)
            {
                monsterStatusObserver.MonsterRecoverPp();
            }
        }

        pp = 0;
        currentStatus = MonsterStatus.IDLE;
        monsterAnimator.SetBool(StunAnimationKey, false);

        StartPattern(true);
    }
}
