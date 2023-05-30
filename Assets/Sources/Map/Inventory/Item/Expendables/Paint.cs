using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : ExpendableItem
{
    public Paint()
    {
        this.itemId = 6;
        this.itemName = "페인트";
        this.itemDescription = "페인트보다는 적절한 설정이 있었을 겁니다.";
        this.isPermanent = false;
        this.canUseFromInventory = false;
        this.price = 60;
        this.itemStock = 6;
        this.stockPrice = 0;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Do Nothing
    }
}
