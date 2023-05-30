using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Player: ISerializationCallbackReceiver
{
    [SerializeField] private Direction direction;
    [SerializeField] private int gold = 0;

    [SerializeField] private int warriorMaxHp = 100;
    [SerializeField] private int currentWarriorHp = 100;

    [SerializeField] private int strength = 10;
    [SerializeField] private int dexterity = 10;
    [SerializeField] private int intelligence = 10;

    [SerializeField] private float healCoefficient = 1f;
    [SerializeField] private float playerPpDamageMultiplier = 1f;

    [SerializeField] private float awakeMagicDamageAddition = 0f;
    [SerializeField] private float stunMagicDamageAddition = 0f;

    // Members for using serialization to save/load data.
    [SerializeField] private List<int> _itemIds = new List<int>();
    [SerializeField] private List<int> _itemCounts = new List<int>();

    [SerializeField] private bool[] warriorObtainedSkillList = new bool[4] { false, true, false, false };
    [SerializeField] private int magicianSpellCount = 3;

    [SerializeField] private int maxHollyWaterCount = 3;

    // Pair of itemId and itemCount
    private readonly (int, int)[] InitialItemIds = new (int, int)[2] { (1, 3), (6, 5) };
    private List<List<(InventoryItem, int)>> inventory;
    private ItemFactory itemFactory;

    public Player(bool isInitialLoad = false)
    {
        InitializeProperties();

        if (isInitialLoad)
        {
            for (int i = 0; i < InitialItemIds.Length; i++)
            {
                ObtainItem(itemFactory.createItem(InitialItemIds[i].Item1), InitialItemIds[i].Item2);
            }
        }
    }

    public float GetPlayerPpDamageMultiplier()
    {
        return playerPpDamageMultiplier;
    }

    public void SetPlayerPpDamageMultiplier(float playerPp)
    {
        playerPpDamageMultiplier = playerPp;
    }

    public float GetAwakeMagicDamageAddition()
    {
        return awakeMagicDamageAddition;
    }

    public void SetAwakeMagicDamageAddition(float awakeMagicDamageAddition)
    {
        this.awakeMagicDamageAddition = awakeMagicDamageAddition;
    }

    public float GetStunMagicDamageAddition()
    {
        return stunMagicDamageAddition;
    }

    public void SetStunMagicDamageAddition(float stunMagicDamageAddition)
    {
        this.stunMagicDamageAddition = stunMagicDamageAddition;
    }

    public void OnBeforeSerialize()
    {
        if (inventory == null)
        {
            return;
        }

        _itemIds.Clear();
        _itemCounts.Clear();

        for (int i = 0; i < inventory.Count; i++)
        {
            List<(InventoryItem, int)> inventoryTab = inventory[i];
            for (int j = 0; j < inventoryTab.Count; j++)
            {
                _itemIds.Add(inventoryTab[j].Item1.GetItemId());
                _itemCounts.Add(inventoryTab[j].Item2);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        InitializeProperties();

        for (int i = 0; i < _itemIds.Count; i++)
        {
            if (i >= _itemCounts.Count)
            {
                return;
            }

            ObtainItem(itemFactory.createItem(_itemIds[i]), _itemCounts[i]);
        }
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
    }

    public List<List<(InventoryItem, int)>> GetInventory()
    {
        return this.inventory;
    }

    public int GetStrength()
    {
        return strength;
    }

    public void IncreaseStrength(int strength)
    {
        this.strength += strength;
    }

    public int GetDexterity()
    {
        return dexterity;
    }

    public void SetDexterity(int dex)
    {
        dexterity = dex;
    }

    public int GetIntelligence()
    {
        return intelligence;
    }

    public void IncreaseIntelligence(int intelligence)
    {
        this.intelligence += intelligence;
    }

    public bool IsWarriorSkillAvailable(WarriorSkill skill)
    {
        return warriorObtainedSkillList[(int)skill];
    }

    public void ClearInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].Clear();
        }
    }
         
    public int GetGold()
    {
        return this.gold;
    }

    public void DecreaseGold(int gold)
    {
        this.gold -= gold;
    }

    public void RestoreHollyWater()
    {
        int hollyWaterIndex = FindItemIndex(0, 1);
        inventory[0][hollyWaterIndex] = (inventory[0][hollyWaterIndex].Item1, maxHollyWaterCount);
    }

    public void ObtainItem(InventoryItem item, int itemCount = 1)
    {
        int itemCategory = 1;
        if (item is ExpendableItem)
        {
            itemCategory = 0;
        }
        int index = FindItemIndex(itemCategory, item.GetItemId());

        if (index < 0)
        {
            inventory[itemCategory].Add((item, itemCount));
        }
        else
        {
            inventory[itemCategory][index] = (inventory[itemCategory][index].Item1, inventory[itemCategory][index].Item2 + itemCount);
        }

        int hollyWaterIndex = FindItemIndex(0, 1);
        if (inventory[0][hollyWaterIndex].Item2 > maxHollyWaterCount)
        {
            maxHollyWaterCount = inventory[0][hollyWaterIndex].Item2;
        }
    } 

    public void ObtainGold(int gold)
    {
        this.gold += gold;
    }

    public bool HasItemInInventory(int itemId)
    {
        int itemCategory = GetItemCategoryById(itemId);
        return FindItemIndex(itemCategory, itemId) >= 0;
    }

    public int GetItemCount(int itemId)
    {
        int itemCategory = GetItemCategoryById(itemId);
        int itemIndex = FindItemIndex(itemCategory, itemId);
        return inventory[itemCategory][itemIndex].Item2;
    }

    public int FindItemIndex(int category, int itemId)
    {
        for (int i = 0; i < inventory[category].Count; i++)
        {
            if (inventory[category][i].Item1.GetItemId() == itemId)
            {
                return i;
            }
        }
        return -1;
    }

    public void UpdateWarriorHp(int amount)
    {
        currentWarriorHp += amount;

        if (currentWarriorHp > warriorMaxHp)
        {
            currentWarriorHp = warriorMaxHp;
        }

        if (currentWarriorHp < 0)
        {
            currentWarriorHp = 0;
        }
    }

    public float GetWarriorHpPercentage()
    {
        return (float)currentWarriorHp / (float)warriorMaxHp;
    }

    public int GetWarriorMaxHp()
    {
        return warriorMaxHp;
    }

    public void IncreaseWarriorHP(int addHp)
    {
        warriorMaxHp += addHp;
        currentWarriorHp += addHp;
    }

    public float GetHealCoefficient()
    {
        return this.healCoefficient;
    }

    public int GetMagicianSpellCount()
    {
        return magicianSpellCount;
    }

    public bool UseItem(InventoryItem item)
    {
        int tab = 0;
        if (item is KeyItem)
        {
            tab = 1;
        }

        for (int i = 0; i < inventory[tab].Count; i++)
        {
            if (inventory[tab][i].Item1.GetItemId() == item.GetItemId())
            {
                int itemCount = inventory[tab][i].Item2;
                if (itemCount == 0)
                {
                    return false;
                }

                itemCount--;
                inventory[tab][i] = (item, itemCount);

                if (itemCount <= 0 && !item.IsPermanent())
                {
                    inventory[tab].RemoveAt(i);
                }
                return true;
            }
        }

        return false;
    }

    public bool UseItem(int itemId)
    {
        InventoryItem item = itemFactory.createItem(itemId);
        return UseItem(item);
    }

    public void UpdateStatus(int maxHp, int str, int dex, int intelligence, int skills, int gold, int spellCount)
    {
        warriorMaxHp = maxHp;
        currentWarriorHp = maxHp;
        strength = str;
        dexterity = dex;
        this.intelligence = intelligence;
        this.gold = gold;
        magicianSpellCount = spellCount;
        for (int i = 0; i < warriorObtainedSkillList.Length; i++)
        {
            warriorObtainedSkillList[i] = i < skills;
        }
    }

    public int GetSkillCount()
    {
        for (int i = 0; i < warriorObtainedSkillList.Length; i++)
        {
            if (!warriorObtainedSkillList[i])
            {
                return i;
            }
        }
        return warriorObtainedSkillList.Length;
    }

    public void RestoreFullHp()
    {
        currentWarriorHp = warriorMaxHp;
    }

    public void ObtainWarriorSkill(WarriorSkill skill)
    {
        warriorObtainedSkillList[(int)skill] = true;
    }

    public void IncreaseMagicianSpellCount()
    {
        magicianSpellCount++;
    }

    private void InitializeProperties()
    {
        inventory = new List<List<(InventoryItem, int)>>();
        itemFactory = new ItemFactory();

        List<(InventoryItem, int)> expendables = new List<(InventoryItem, int)>();
        inventory.Add(expendables);

        List<(InventoryItem, int)> keyItems = new List<(InventoryItem, int)>();
        inventory.Add(keyItems);
    }

    private int GetItemCategoryById(int itemId)
    {
        return itemId / 100 != 0 ? 1 : 0;
    }

    public enum WarriorSkill
    {
        SMITE = 0,
        PARRYNIG,
        PREPARE_POSE,
        CLASH
    }
}
