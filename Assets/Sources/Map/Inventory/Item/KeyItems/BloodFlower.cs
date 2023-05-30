using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFlower : KeyItem
{
    public BloodFlower()
    {
        this.itemId = 103;
        this.itemName = "혈화";
        this.itemDescription = "피를 먹고 자라는 식물입니다.";
        this.isPermanent = true;
        this.isPlantType = true;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Not Implemented Yet
    }
}
