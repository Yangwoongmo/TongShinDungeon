using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlePlayerButtonClickListener
{
    void OnChangeCameraClick();
    void OnNormalAttackClick();
    void OnAvoidClick(PlayerDirection direction);
    void OnWarriorSkillClick();
    void OnBlinkClick();
    void OnSpellClick(int buttonNumber);
    void OnCompleteCastingClick();
    void OnHolyWaterTouchDown();
    void OnHolyWaterTouchUp();
}
