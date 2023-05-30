using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Warrior warrior;
    private Player player;

    public void LeftAvoid()
    {
        warrior.LeftAvoid();
    }

    public void RightAvoid()
    {
        warrior.RightAvoid();
    }

    public void NormalAttack()
    {
        warrior.NormalAttack(player.GetStrength(), player.GetDexterity(), IsParryingAvailable(), player.IsWarriorSkillAvailable(Player.WarriorSkill.PREPARE_POSE));
    }

    public bool PokeClick()
    {
        return warrior.Poke(player.GetStrength(), player.GetDexterity(), IsParryingAvailable(), player.IsWarriorSkillAvailable(Player.WarriorSkill.CLASH));
    }

    public Warrior GetWarrior()
    {
        return warrior;
    }

    public Coroutine GetWarriorAttackCoroutine()
    {
        return warrior.GetWarriorAttackCoroutine();
    }

    public void SetWarriorStatusObserver(IWarriorStatusObserver observer)
    {
        warrior.SetWarriorStatusObserver(observer);
    }

    public void SetBattleActionObserver(IBattleActionObserver observer)
    {
        warrior.SetBattleActionObserver(observer);
    }

    public void ResetWarrior()
    {
        warrior.ResetWarrior();
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public WarriorBattleStatistic GetStatistic()
    {
        return warrior.GetStatistic();
    }

    public bool IsWarriorInAction()
    {
        return warrior.IsWarriorInAction();
    }

    private bool IsParryingAvailable()
    {
        return player.IsWarriorSkillAvailable(Player.WarriorSkill.PARRYNIG);
    }

    public void SetWarriorInvicible()
    {
        warrior.SetWarriorInvicible();
    }
}
