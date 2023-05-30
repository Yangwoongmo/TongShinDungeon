using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryClickListener
{
    public void OpenInventory();
    public void CloseInventory();
    public void SelectInventoryTab(int tab);
    public void CloseItemDescription();
    public void SortInventory();
}
