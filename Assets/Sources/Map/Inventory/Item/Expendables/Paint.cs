using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : ExpendableItem
{
    public Paint()
    {
        this.itemId = 6;
        this.itemName = "����Ʈ";
        this.itemDescription = "����Ʈ���ٴ� ������ ������ �־��� �̴ϴ�.";
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
