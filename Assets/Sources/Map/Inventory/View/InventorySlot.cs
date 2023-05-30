using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Text numberOfItemText;
    [SerializeField] private Image itemIcon;
    private InventoryItem item;
    private int numberOfItem;

    public void SetInventoryItem(InventoryItem item)
    {
        this.item = item;
        this.itemIcon.sprite = Resources.Load<Sprite>("Image/ItemIcon/" + item.GetItemId().ToString()) as Sprite;
    }

    public InventoryItem GetInventoryItem()
    {
        return this.item;
    }

    public void SetNumberOfItem(int number)
    {
        this.numberOfItem = number;
        this.numberOfItemText.text = number.ToString();
    }

    public int GetNumberOfItem()
    {
        return this.numberOfItem;
    }
}
