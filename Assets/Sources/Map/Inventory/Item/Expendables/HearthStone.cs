using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthStone : ExpendableItem
{
    public HearthStone()
    {
        this.itemId = 5;
        this.itemName = "귀환석";
        this.itemDescription = "성스러운 곳으로 돌아갑니다.";
        this.isPermanent = false;
        this.canUseFromInventory = true;
        this.price = 100;
        this.itemStock = 5;
        this.stockPrice = 0;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        listener.GoToSanctuary();
    }
}
