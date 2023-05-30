using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPortion : ExpendableItem
{
    public HealPortion()
    {
        this.itemId = 2;
        this.itemName = "회복약";
        this.itemDescription = "복용자의 몸을 강제로 복구시킨다.\n끔찍한 맛이 난다.";
        this.isPermanent = false;
        this.canUseFromInventory = true;
        this.price = 30;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        listener.HealPlayer(30, false, false);
    }
}
