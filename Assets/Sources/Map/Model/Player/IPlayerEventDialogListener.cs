using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerEventDialogListener
{
    public void ObtainWarriorSkill(Player.WarriorSkill skill);
    public void IncreaseMagicianSpellCount();
}
