using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModeSettingController : MonoBehaviour
{
    [SerializeField] private GameObject settingScreen;
    [SerializeField] private InputField maxHp;
    [SerializeField] private InputField str;
    [SerializeField] private InputField dex;
    [SerializeField] private InputField intelligence;
    [SerializeField] private InputField skills;
    [SerializeField] private InputField golds;
    [SerializeField] private InputField spellCount;

    private Player player;

    public void OpenSetting(Player player)
    {
        this.player = player;
        settingScreen.SetActive(true);
        maxHp.text = player.GetWarriorMaxHp().ToString();
        str.text = player.GetStrength().ToString();
        dex.text = player.GetDexterity().ToString();
        intelligence.text = player.GetIntelligence().ToString();
        skills.text = player.GetSkillCount().ToString();
        golds.text = player.GetGold().ToString();
        spellCount.text = player.GetMagicianSpellCount().ToString();
    }

    public void CancelChange()
    {
        settingScreen.SetActive(false);
    }

    public void SaveChange()
    {
        int updateMaxHp = int.Parse(maxHp.text);
        int updateSkills = int.Parse(skills.text);
        int updateStr = int.Parse(str.text);
        int updateDex = int.Parse(dex.text);
        int updateInt = int.Parse(intelligence.text);
        int updateGold = int.Parse(golds.text);
        int updateSpellCount = int.Parse(spellCount.text);

        player.UpdateStatus(updateMaxHp, updateStr, updateDex, updateInt, updateSkills, updateGold, updateSpellCount);

        settingScreen.SetActive(false);
    }
}
