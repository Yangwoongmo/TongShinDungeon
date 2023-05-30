using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePrototypeScene : MonoBehaviour
{
    [SerializeField] private BattlePrototypeSceneController controller;

    public void OnNormalAttackClick()
    {
        controller.OnNormalAttackClick();
    }
    public void OnAvoidClick(int direction)
    {
        controller.OnAvoidClick((PlayerDirection)direction);
    }
    public void OnWarriorSkillClick()
    {
        controller.OnWarriorSkillClick();
    }
    public void OnBlinkClick()
    {
        controller.OnBlinkClick();
    }
    public void OnSpellClick(int buttonNumber)
    {
        controller.OnSpellClick(buttonNumber);
    }
    public void OnCompleteCastingClick()
    {
        controller.OnCompleteCastingClick();
    }
}
