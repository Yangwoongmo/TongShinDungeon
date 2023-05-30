using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour, IMonsterStatusObserver
{
    private const string MonsterUIAppearAnimationKey = "uiAppear";
    private const string MonsterHideAnimationKey = "hide";

    [SerializeField] private Image hpBar;
    [SerializeField] private Image ppBar;
    [SerializeField] private Text monsterName;
    [SerializeField] private Canvas monsterAttackWarningCanvas;
    [SerializeField] private Animator monsterAttackWarningAnimator;
    [SerializeField] private Animator monsterUIAnimator;
    private Monster monster;

    public void MonsterAttackWarning(string animationKey, int value)
    {
        StartCoroutine(MonsterAttackWarningCoroutine(animationKey, value));
    }

    public void MonsterHitDamage()
    {
        hpBar.fillAmount = monster.GetHpPercentage();
    }

    public void MonsterHitPp()
    {
        ppBar.fillAmount = monster.GetPpPercentage();
    }

    public void MonsterRecoverPp()
    {
        ppBar.fillAmount = monster.GetPpPercentage();
    }

    public void StartMonsterPattern()
    {
        monster.StartPattern();
    }

    public Monster GetMonster()
    {
        return monster;
    }

    public void ChangeMonsterSpriteVisibility(bool isVisible)
    {
        if (monster != null)
        {
            monster.ChangeMonsterSpriteVisibility(isVisible);
        }
    }

    public void SetMonster(Monster monster)
    {
        this.monster = monster;
        this.monster.InitializeMonster();
        monsterName.text = this.monster.GetMonsterName();
        monsterName.color = this.monster.GetMonsterNameColor();
        this.monster.SetMonsterStatusObserver(this);
        
        hpBar.fillAmount = 1;
        ppBar.fillAmount = 0;
        monsterAttackWarningCanvas.gameObject.SetActive(true);
        SetWarningCanvaseVisibility(true);
        monsterUIAnimator.SetBool(MonsterUIAppearAnimationKey, true);     
    }

    public void ResetMonster()
    {
        this.monster = null;
        ResetAttackWarningObjects();
        monsterUIAnimator.SetBool(MonsterUIAppearAnimationKey, false);
    }

    public void HideMonster()
    {
        Animator monsterAnimator = this.monster.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
        monsterAnimator.SetBool(MonsterHideAnimationKey, true);
        this.monster.gameObject.SetActive(false);
        monsterAnimator.SetBool(MonsterHideAnimationKey, false);
        StopMonster();
        monsterUIAnimator.SetBool(MonsterUIAppearAnimationKey, false);
        ResetAttackWarningObjects();
    }

    public void StopMonster()
    {
        this.monster.StopMonster();
        this.monster = null;
        ResetAttackWarningObjects();
    }

    public void ResetMonsterWarning(string animationKey)
    {
        monsterAttackWarningAnimator.SetInteger(animationKey, -1);
    }

    public void SetWarningCanvaseVisibility(bool isVisible)
    {
        monsterAttackWarningCanvas.enabled = isVisible;
    }

    public void SetBattleActionObserver(IBattleActionObserver observer)
    {
        monster.SetBattleActionObserver(observer);
    }

    private IEnumerator MonsterAttackWarningCoroutine(string animationKey, int value)
    {
        monsterAttackWarningAnimator.SetInteger(animationKey, value);
        yield return new WaitForSeconds(0.35f);
        monsterAttackWarningAnimator.SetInteger(animationKey, -1);
    }

    private void ResetAttackWarningObjects()
    {
        monsterAttackWarningCanvas.gameObject.SetActive(false);
        Transform child = monsterAttackWarningCanvas.gameObject.transform.GetChild(0);
        for (int i = 0; i < child.childCount; i++)
        {
            child.GetChild(i).gameObject.SetActive(false);
        }
    }
}
