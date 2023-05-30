using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KeyItem : InventoryItem
{
    protected bool isPlantType;

    public bool IsPlantType()
    {
        return isPlantType;
    }
}
