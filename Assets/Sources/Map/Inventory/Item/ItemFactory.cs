using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    public InventoryItem createItem(int itemId)
    {
        switch (itemId)
        {
            case 1: return new HolyWater();
            case 2: return new HealPortion();
            // Cat/MousePortion are deleted
            case 5: return new HearthStone();
            case 6: return new Paint();

            case 101: return new Mandragora();
            case 102: return new ScreamWeed();
            case 103: return new BloodFlower();
            case 104: return new TechnicalTextBook();
            case 105: return new SpellBook();
            case 106: return new Meat();
            case 107: return new Potato();
            case 108: return new Onion();

            default: return null;
        }
    }
}
