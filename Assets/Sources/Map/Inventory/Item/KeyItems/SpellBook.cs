using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : KeyItem
{
    public SpellBook()
    {
        this.itemId = 105;
        this.itemName = "마법서1";
        this.itemDescription = "지혜는 호기심에서 나온다.";
        this.isPermanent = true;
        this.isPlantType = false;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Not Implemented Yet
    }
}
