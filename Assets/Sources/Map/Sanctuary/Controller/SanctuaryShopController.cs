using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanctuaryShopController : SanctuaryCharacterDialogController
{
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private GameObject rotateButtons;
    [SerializeField] private GameObject shopMainButton;
    [SerializeField] private GameObject shopDialogButton;
    [SerializeField] private ShopInventoryController shopInventoryController;

    public override void SetEnable(bool isEnabled)
    {
        base.SetEnable(isEnabled);
        shopRoot.SetActive(isEnabled);
    }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        shopInventoryController.SetPlayer(player);
    }

    public override void SetSaveDataCallback(Action saveDataCallback)
    {
        base.SetSaveDataCallback(saveDataCallback);
        shopInventoryController.SetSaveDataCallback(saveDataCallback);
    }

    public void OpenShoppingList()
    {
        shopInventoryController.OpenShoppingList();
        ChangeShoppingListVisibility(true);
    }

    public void CloseShoppingList()
    {
        shopInventoryController.CloseShoppingList();
        ChangeShoppingListVisibility(false);
    }

    public void ShowSanctuaryDeathDialog(SanctuaryDeathStatistics statistics)
    {
        dialogController.ShowSanctuaryDeathDialog(statistics);
    }

    private void ChangeShoppingListVisibility(bool isVisible)
    {
        rotateButtons.SetActive(!isVisible);
        inventoryButton.SetActive(!isVisible);
        shopMainButton.SetActive(!isVisible);
        shopDialogButton.SetActive(!isVisible);
    }
}
