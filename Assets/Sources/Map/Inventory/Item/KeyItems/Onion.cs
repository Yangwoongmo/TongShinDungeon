using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onion : StewIngredient
{
    public Onion()
    {
        this.itemId = 108;
        this.itemName = "����";
        this.itemDescription = "������ ��� ����� 4õ�� �̻��̴�";
        this.isPermanent = true;
        this.isPlantType = false;
        this.recipes.effectTexts = new string[1] { "���� +1" };
        this.price = 100;
        this.itemStock = 5;
        this.stockPrice = 20;
    }

    public override void UpdatePlayerStatus(Player player, int selectIndex = 0)
    {
        int addInteligence = 1;
        player.IncreaseIntelligence(addInteligence);
        player.UseItem(itemId);
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
    }
}
