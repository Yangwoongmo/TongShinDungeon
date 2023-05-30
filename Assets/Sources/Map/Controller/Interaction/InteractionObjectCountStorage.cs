using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectCountStorage : MonoBehaviour
{
    private int keyItemsCount = 0;
    private int lockedDoorCount = 0;
    private int sanctuaryPortalCount = 0;
    private Dictionary<string, int> plantItemCountDictionary = new Dictionary<string, int>();

    public int GetKeyItemsCount()
    {
        return keyItemsCount;
    }

    public int GetLockedDoorCount()
    {
        return lockedDoorCount;
    }

    public int GetSanctuaryPortalCount()
    {
        return sanctuaryPortalCount;
    }

    public Dictionary<string, int> GetPlantItemDict()
    {
        return plantItemCountDictionary;
    }

    public void AddKeyItem()
    {
        keyItemsCount++;
    }

    public void RemoveKeyItem()
    {
        keyItemsCount--;
        if (keyItemsCount < 0)
        {
            keyItemsCount = 0;
        }
    }

    public void AddLockedDoor()
    {
        lockedDoorCount++;
    }

    public void RemoveLockedDoor()
    {
        lockedDoorCount--;
        if (lockedDoorCount < 0)
        {
            lockedDoorCount = 0;
        }
    }

    public void AddPlantItem(string itemName)
    {
        int currentItemCount = 0;
        if (plantItemCountDictionary.ContainsKey(itemName))
        {
            currentItemCount = plantItemCountDictionary[itemName];
        }

        plantItemCountDictionary[itemName] = currentItemCount + 1;
    }

    public void RemovePlantItem(string itemName)
    {
        if (!plantItemCountDictionary.ContainsKey(itemName))
        {
            return;
        }

        int currentItemCount = plantItemCountDictionary[itemName];
        
        if (currentItemCount > 1)
        {
            plantItemCountDictionary[itemName] = currentItemCount - 1;
        }
        else
        {
            plantItemCountDictionary.Remove(itemName);
        }
    }

    public void AddSanctuaryPortal()
    {
        sanctuaryPortalCount++;
    }

    public void RemoveSanctuaryPortal()
    {
        // TRICKY: SanctuaryPortal has 2 IO for front/back door per 1 portal.
        // So we need to subtract 2 count per purify interact.
        sanctuaryPortalCount -= 2;
        if (sanctuaryPortalCount < 0)
        {
            sanctuaryPortalCount = 0;
        }
    }
}
