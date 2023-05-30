using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Text priceText;
    [SerializeField] private Image itemIconImage;

    private InventoryItem item;

    public void SetItem(int itemId)
    {
        ItemFactory factory = new ItemFactory();
        item = factory.createItem(itemId);

        itemIconImage.sprite = Resources.Load<Sprite>("Image/ItemIcon/" + item.GetItemId().ToString());
        priceText.text = item.GetPrice().ToString();
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    public void SetItemPriceAndStock(int price, int stock)
    {
        if (price < 0 || stock < 0)
        {
            return;
        }

        item.SetItemStock(stock);
        item.SetPrice(price);

        if (stock <= 0)
        {
            priceText.text = "없음";
        }
        else
        {
            priceText.text = price.ToString();
        }
    }
}
