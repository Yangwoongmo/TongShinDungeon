using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : StewIngredient
{
    public Meat()
    {
        this.itemId = 106;
        this.itemName = "고기";
        this.itemDescription = "몬스터의 고기는 아니다";
        this.isPermanent = true;
        this.isPlantType = false;
        this.recipes.effectTexts = new string[1] { "근력 +1" };
        this.price = 100;
        this.itemStock = 5;
        this.stockPrice = 20;
    }

    public override void UpdatePlayerStatus(Player player, int selectIndex = 0)
    {
        int addStrength = 1;
        player.IncreaseStrength(addStrength);
        player.UseItem(itemId);
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
    }
}
