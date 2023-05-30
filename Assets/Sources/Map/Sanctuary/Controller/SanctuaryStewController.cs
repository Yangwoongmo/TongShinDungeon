using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanctuaryStewController : SanctuaryController
{
    [SerializeField] private GameObject stewButtons;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private GameObject rotateButton;
    [SerializeField] private GameObject stewStandardButtonUI;
    [SerializeField] private GameObject stewEffeckCheckUI;
    [SerializeField] private GameObject stewInsertMatterUI;
    [SerializeField] private Text stewStrText;
    [SerializeField] private Text stewDexText;
    [SerializeField] private Text stewHealthText;

    [SerializeField] private StewInventoryController inventoryController;

    private Player player;

    public override void SetEnable(bool isEnabled)
    {
        base.SetEnable(isEnabled);
        stewButtons.SetActive(isEnabled);
    }

    public override void SetPlayer(Player player)
    {
        this.player = player;
        inventoryController.SetPlayer(player);
    }

    public override void SetSaveDataCallback(Action saveDataCallback)
    {
        base.SetSaveDataCallback(saveDataCallback);
        inventoryController.SetSaveDataCallback(saveDataCallback);
    }

    public void OnClickInsertMatterButton()
    {
        rotateButton.SetActive(false);
        inventoryButton.SetActive(false);
        stewStandardButtonUI.SetActive(false);
        stewInsertMatterUI.SetActive(true);
        stewEffeckCheckUI.SetActive(false);

        inventoryController.OpenStewItemList();
    }

    public void OnClickStewStatusCheckButton()
    {
        rotateButton.SetActive(false);
        inventoryButton.SetActive(false);
        stewStandardButtonUI.SetActive(false);
        stewEffeckCheckUI.SetActive(true);
        SetStatusText();
    }

    public void FinishInsertingMatter()
    {
        stewEffeckCheckUI.SetActive(false);
        stewInsertMatterUI.SetActive(false);
        stewStandardButtonUI.SetActive(true);
        inventoryButton.SetActive(true);
        rotateButton.SetActive(true);

        inventoryController.CloseStewItemList();
    }

    public void CloseStatusCheckUI()
    {
        stewEffeckCheckUI.SetActive(false);
        stewInsertMatterUI.SetActive(false);
        stewStandardButtonUI.SetActive(true);
        inventoryButton.SetActive(true);
        rotateButton.SetActive(true);
    }

    public void SetStatusText()
    {
        stewStrText.text = "근력 : " + player.GetStrength();
        stewDexText.text = "손재주 : " + player.GetDexterity();
        stewHealthText.text = "체력 : " + player.GetWarriorMaxHp();
        //stewIntelligentText.text = "지력 : " + player.GetIntelligence();
    }
}
