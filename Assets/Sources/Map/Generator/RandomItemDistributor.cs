using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemDistributor : MonoBehaviour
{
    [SerializeField] private ItemGroup[] randomItemGroups;
    [SerializeField] private ItemHolder[] fixItems;

    private MapObjectStatusManager manager = MapObjectStatusManager.GetInstance();

    public void CleanupItemHolders()
    {
        for (int i = 0; i < randomItemGroups.Length; i++)
        {
            ItemGroup itemGroup = randomItemGroups[i];
            ItemHolder[] itemList = itemGroup.GetItemList();

            for (int j = 0; j < itemList.Length; j++)
            {
                itemList[j].ResetObserver();
            }
        }
    }

    public void DistributeItems()
    {
        for (int i = 0; i < randomItemGroups.Length; i++)
        {
            ItemGroup itemGroup = randomItemGroups[i];
            ItemHolder[] itemList = itemGroup.GetItemList();
            int[] itemIds = itemGroup.GetItemIds();
            int[] golds = itemGroup.GetGolds();

            List<int> indexPool = new List<int>();
            for (int j = 0; j < itemList.Length; j++)
            {
                indexPool.Add(j);
            }

            for (int k = 0; k < itemIds.Length; k++)
            {
                int selectedIndex = Random.Range(0, indexPool.Count);
                ItemHolder selectedItem = itemList[indexPool[selectedIndex]];
                indexPool.Remove(selectedIndex);

                selectedItem.SetItemId(itemIds[k]);
                selectedItem.SetGold(golds[k]);

                selectedItem.StartObservePlayer();
                selectedItem.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < fixItems.Length; i++)
        {
            string id = fixItems[i].transform.parent.GetComponent<Floor>().GetId();
            if (manager.HasObjectStateChanged(id))
            {
                continue;
            }

            fixItems[i].StartObservePlayer();
            fixItems[i].gameObject.SetActive(true);
        }
    }

    public void ClearItemGroups(int randomSize, int fixedSize)
    {
#if UNITY_EDITOR
        randomItemGroups = new ItemGroup[randomSize];
        fixItems = new ItemHolder[fixedSize];
#endif
    }

    public void AddItemHolderToGroup(
        int groupId, 
        bool isFixedItem, 
        ItemHolder[] itemHolders,
        int[] itemIds,
        int[] golds
    )
    {
#if UNITY_EDITOR
        if (isFixedItem)
        {
            fixItems[groupId] = itemHolders[0];
        }
        else
        {
            int[] groupItemIds = itemIds.Length > 0 ? itemIds : new int[golds.Length];
            int[] groupGolds = golds.Length > 0 ? golds : new int[itemIds.Length];
            randomItemGroups[groupId] = new ItemGroup(itemHolders, groupItemIds, groupGolds);
        }
#endif
    }
}

[System.Serializable]
public class ItemGroup
{
    [SerializeField] private ItemHolder[] itemList;
    [SerializeField] private int[] itemIds;
    [SerializeField] private int[] golds;

    public ItemGroup(ItemHolder[] itemList, int[] itemIds, int[] golds)
    {
        this.itemList = new ItemHolder[itemList.Length];
        for (int i = 0; i < itemList.Length; i++)
        {
            this.itemList[i] = itemList[i];
        }

        this.itemIds = new int[itemIds.Length];
        for (int i = 0; i < itemIds.Length; i++)
        {
            this.itemIds[i] = itemIds[i];
        }

        this.golds = new int[golds.Length];
        for (int i = 0; i < golds.Length; i++)
        {
            this.golds[i] = golds[i];
        }
    }

    public ItemHolder[] GetItemList()
    {
        return itemList;
    }

    public int[] GetItemIds()
    {
        return itemIds;
    }

    public int[] GetGolds()
    {
        return golds;
    }
}
