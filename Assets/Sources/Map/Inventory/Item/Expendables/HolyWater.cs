using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyWater : ExpendableItem
{
    public HolyWater()
    {
        this.itemId = 1;
        this.itemName = "성수";
        this.itemDescription = "영혼을 정화해 통신 상태를 회복한다.\n성역에 들르면 물이 채워진다.";
        this.isPermanent = true;
        this.canUseFromInventory = true;
        this.price = 1000;
        this.itemStock = 3;
        this.stockPrice = 0;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        listener.HealPlayer(30, true, true);
    }
}
