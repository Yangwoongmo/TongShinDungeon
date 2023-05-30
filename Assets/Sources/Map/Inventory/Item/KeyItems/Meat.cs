using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : StewIngredient
{
    public Meat()
    {
        this.itemId = 106;
        this.itemName = "���";
        this.itemDescription = "������ ���� �ƴϴ�";
        this.isPermanent = true;
        this.isPlantType = false;
        this.recipes.effectTexts = new string[1] { "�ٷ� +1" };
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
