using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExpendableItem : InventoryItem
{
    protected bool canUseFromInventory;

    public bool CanUserFromInventory()
    {
        return canUseFromInventory;
    }
}
