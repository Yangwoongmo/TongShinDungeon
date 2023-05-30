using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionController : IBattleActionObserver
{    
    private Player player;
    private IMonsterHitHandler monsterHitHandler;
    private IWarriorHitHandler warriorHitHandler;

    private WarriorStatus currentWarriorActionType = WarriorStatus.IDLE;
    private MonsterStatus currentMonsterActionType = MonsterStatus.IDLE;

    private bool isParryingTriggered = false;
    private bool isAttackBlocked = false;

    public void Setup(Player player, IMonsterHitHandler monsterHitHandler, IWarriorHitHandler warriorHitHandler)
    {
        this.player = player;
        this.monsterHitHandler = monsterHitHandler;
        this.warriorHitHandler = warriorHitHandler;
    }

    public void RegisterWarriorAction(WarriorStatus actionType)
    {
        currentWarriorActionType = actionType;
        ResetAttackBlockStatus();
    }

    public void UnregisterWarriorAction()
    {
        currentWarriorActionType = WarriorStatus.IDLE;
        ResetAttackBlockStatus();
    }

    public bool FireWarriorAttack(int damage, int pp)
    {
        int realDamage = damage;
        int realPp = pp;

        switch (currentWarriorActionType)
        {
            case WarriorStatus.ATTACK:     
                if (currentMonsterActionType == MonsterStatus.STRONG_ATTACK)
                {
                    realDamage = 0;
                    realPp = 0;
                }
                else if ((IsParryingAvailable() && currentMonsterActionType == MonsterStatus.WEAK_ATTACK) || isParryingTriggered)
                {
                    realDamage = 0;
                    realPp = GetParryingPp(pp);
                    isAttackBlocked = true;
                }
                break;
            case WarriorStatus.CLASH:
                if ((IsParryingAvailable() && currentMonsterActionType == MonsterStatus.WEAK_ATTACK) || isParryingTriggered)
                {
                    realPp = GetParryingPp(pp);
                    isAttackBlocked = true;
                }
                else if ((IsParryingAvailable() && currentMonsterActionType == MonsterStatus.STRONG_ATTACK) || isParryingTriggered)
                {
                    realDamage = 0;
                    realPp = GetParryingPp(pp);
                    isAttackBlocked = true;
                }
                break;
            default:
                return false;
        }

        bool isParryingSuccess = isAttackBlocked;
        isParryingTriggered = false;
        currentWarriorActionType = WarriorStatus.IDLE;
        monsterHitHandler.HitFromWarrior(realDamage, realPp);
       
        return isParryingSuccess;
    }

    public void RegisterMonsterAction(MonsterStatus actionType)
    {
        currentMonsterActionType = actionType;
        ResetAttackBlockStatus();
    }

    public void UnregisterMonsterAction()
    {
        currentMonsterActionType = MonsterStatus.IDLE;
        ResetAttackBlockStatus();
    }

    public void FireMonsterAttack(int damage, PlayerDirection attackDirection)
    {
        int realDamage = damage;

        switch (currentMonsterActionType)
        {
            case MonsterStatus.WEAK_ATTACK:
                if (CanAvoidMonsterAttack(attackDirection))
                {
                    realDamage = 0;
                }
                else if (
                    (IsParryingAvailable() && (currentWarriorActionType == WarriorStatus.ATTACK || currentWarriorActionType == WarriorStatus.CLASH)) ||
                    isAttackBlocked    
                )
                {
                    realDamage = 0;
                    isParryingTriggered = true;
                }
                break;
            case MonsterStatus.STRONG_ATTACK:
                if (CanAvoidMonsterAttack(attackDirection))
                {
                    realDamage = 0;
                }
                else if ((IsParryingAvailable() && currentWarriorActionType == WarriorStatus.CLASH) || isAttackBlocked)
                {
                    realDamage = 0;
                    isParryingTriggered = true;
                }
                break;
            default:
                return;
        }

        isAttackBlocked = false;
        currentMonsterActionType = MonsterStatus.IDLE;
        warriorHitHandler.Hit(realDamage);
    }

    private bool IsParryingAvailable()
    {
        return player.IsWarriorSkillAvailable(Player.WarriorSkill.PARRYNIG);
    }

    private int GetParryingPp(int pp)
    {
        return Mathf.RoundToInt(pp * 1.5f * player.GetPlayerPpDamageMultiplier());
    }

    private bool CanAvoidMonsterAttack(PlayerDirection direction)
    {
        switch (direction)
        {
            case PlayerDirection.CENTER:
                return currentWarriorActionType == WarriorStatus.LEFT_AVOID || currentWarriorActionType == WarriorStatus.RIGHT_AVOID;
            case PlayerDirection.LEFT:
                return currentWarriorActionType == WarriorStatus.RIGHT_AVOID;
            case PlayerDirection.RIGHT:
                return currentWarriorActionType == WarriorStatus.LEFT_AVOID;
            case PlayerDirection.BOTH:
                return currentWarriorActionType != WarriorStatus.LEFT_AVOID && currentWarriorActionType != WarriorStatus.RIGHT_AVOID;
            default:
                return false;
        }
    }

    private void ResetAttackBlockStatus()
    {
        isParryingTriggered = false;
        isAttackBlocked = false;
    }
}
