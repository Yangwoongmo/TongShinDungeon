using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerController : MonoBehaviour
{
    [SerializeField] private BattlePlayer battlePlayer;
    [SerializeField] private WarriorController warriorController;
    [SerializeField] private MagicianController magicianController;

    public void SetMonsterHitHandler(IMonsterHitHandler handler)
    {
        magicianController.SetMonsterHitHandler(handler);
    }

    public WarriorController GetWarriorController()
    {
        return warriorController;
    }
    public MagicianController GetMagicianController()
    {
        return magicianController;
    }

    public void AppearCharacters()
    {
        warriorController.GetWarrior().gameObject.SetActive(true);
        warriorController.GetWarrior().Appear();
        magicianController.GetMagician().gameObject.SetActive(true);
    }

    public void DisappearCharacters()
    {
        StartCoroutine(WarriorDisappearCoroutine());
        magicianController.GetMagician().gameObject.SetActive(false);
    }

    public void DisappearCharactersWithoutAnimation()
    {
        warriorController.GetWarrior().gameObject.SetActive(false);
        magicianController.GetMagician().gameObject.SetActive(false);
    }

    public void SetPlayer(Player player)
    {
        warriorController.SetPlayer(player);
    }

    public WarriorBattleStatistic GetWarriorBattleStatistic()
    {
        return warriorController.GetStatistic();
    }

    public bool IsWarriorInAction()
    {
        return warriorController.IsWarriorInAction();
    }

    private IEnumerator WarriorDisappearCoroutine()
    {
        warriorController.GetWarrior().Disappear();
        yield return new WaitForSeconds(1.5f);
        warriorController.GetWarrior().gameObject.SetActive(false);
    }
}
