using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem
{
    protected int itemId;
    protected string itemName;
    protected string imageSrc;
    protected string itemDescription;
    protected bool isPermanent;
    protected int price;
    protected int itemStock;
    protected int stockPrice;

    public abstract void UseItemFromInventory(ItemUseListener listener); 

    public int GetItemId()
    {
        return itemId;
    }

    public string GetItemName()
    {
        return itemName;
    } 

    public string GetItemDescription()
    {
        return itemDescription;
    }

    public bool IsPermanent()
    {
        return isPermanent;
    }

    public int GetPrice()
    {
        return price;
    }

    public void SetPrice(int itemPrice)
    {
        price = itemPrice;
    }

    public int GetItemStock()
    {
        return itemStock;
    }

    public void SetItemStock(int stock)
    {
        itemStock = stock;
    }

    public int GetStockPrice()
    {
        return stockPrice;
    }
}
