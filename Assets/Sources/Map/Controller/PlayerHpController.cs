using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpController: IWarriorStatusObserver
{
    private Player player;
    private Image warriorHpUI;

    private Image inventoryWarriorHpUI;
    private List<Sprite> hpBarSprites;
    private Text hpPercentageText;

    public PlayerHpController(
        Player player, 
        Image warriorHpUI, 
        Image inventoryWarriorHpUI, 
        List<Sprite> hpBarSprites,
        Text hpPercentageText
    )
    {
        this.player = player;
        this.warriorHpUI = warriorHpUI;
        this.inventoryWarriorHpUI = inventoryWarriorHpUI;
        this.hpBarSprites = new List<Sprite>();
        this.hpBarSprites.AddRange(hpBarSprites);
        this.hpPercentageText = hpPercentageText;

        UpdateWarriorHp();
    }

    public void UpdateWarriorHp(int amount = 0)
    {
        player.UpdateWarriorHp(amount);
        float warriorHpPercentage = player.GetWarriorHpPercentage();
        
        int hpBarSpriteIndex;
        if (warriorHpPercentage > 0.66f)
        {
            hpBarSpriteIndex = 0;
        }
        else if (warriorHpPercentage > 0.33f)
        {
            hpBarSpriteIndex = 1;
        }
        else
        {
            hpBarSpriteIndex = 2;
        }

        warriorHpUI.sprite = hpBarSprites[hpBarSpriteIndex];
        hpPercentageText.text = Mathf.RoundToInt(warriorHpPercentage * 100).ToString() + "%";
    }

    public bool WarriorHitDamage(int damage)
    {
        UpdateWarriorHp(damage * (-1));
        return player.GetWarriorHpPercentage() > 0;
    }

    public bool IsPlayerDead()
    {
        return player.GetWarriorHpPercentage() == 0f;
    }

    public float GetWarriorHpPercentage()
    {
        return player.GetWarriorHpPercentage();
    }

    public int GetWarriorMaxHp()
    {
        return player.GetWarriorMaxHp();
    }

    public void RestorePlayerFullHp()
    {
        player.RestoreFullHp();
    }
}
