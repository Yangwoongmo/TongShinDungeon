using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : StewIngredient
{
    public Potato()
    {
        this.itemId = 107;
        this.itemName = "����";
        this.itemDescription = "���ڴ� �������� ���ϴ� ��Ȳ�۹��̴�";
        this.isPermanent = true;
        this.isPlantType = false;
        this.recipes.effectTexts = new string[1] { "�ִ� ü�� +10" };
        this.price = 100;
        this.itemStock = 5;
        this.stockPrice = 20;
    }

    public override void UpdatePlayerStatus(Player player, int selectIndex = 0)
    {
        int addHealth = 10;
        player.IncreaseWarriorHP(addHealth);
        player.UseItem(itemId);
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
    }
}
