using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onion : StewIngredient
{
    public Onion()
    {
        this.itemId = 108;
        this.itemName = "양파";
        this.itemDescription = "양파의 재배 역사는 4천년 이상이다";
        this.isPermanent = true;
        this.isPlantType = false;
        this.recipes.effectTexts = new string[1] { "지력 +1" };
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
