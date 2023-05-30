using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnicalTextBook : KeyItem
{
    public TechnicalTextBook()
    {
        this.itemId = 104;
        this.itemName = "기술교본1";
        this.itemDescription = "숙련된 전사의 찌르기는 받아치는 것이 거의 불가능하며 큰 타격을 줄 수 있다.";
        this.isPermanent = true;
        this.isPlantType = false;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Not Implemented Yet
    }
}
