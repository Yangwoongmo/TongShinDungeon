using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewItem : MonoBehaviour
{
    [SerializeField] private Text itemName;
    [SerializeField] private Text cookButtonText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private Button cookButton;

    private StewIngredient ingredient;
    private int itemCount;

    public void SetStewItem(StewIngredient ingredient, int itemCount)
    {
        this.ingredient = ingredient;
        this.itemCount = itemCount;

        itemName.text = ingredient.GetItemName();
        if (ingredient.IsPlantType())
        {
            cookButtonText.text = "조리";
        }
        else
        {
            cookButtonText.text = "넣기";
        }

        itemIconImage.sprite = 
            Resources.Load<Sprite>("Image/ItemIcon/" + ingredient.GetItemId().ToString());
    }

    public void SetCookButtonClickListener(Action onClickListener)
    {
        cookButton.onClick.RemoveAllListeners();
        cookButton.onClick.AddListener(() => onClickListener.Invoke());
    }

    public void CookIngredient(Player player, int selectedIndex = 0)
    {
        ingredient.UpdatePlayerStatus(player, selectedIndex);
        itemCount--;
    }

    public int GetItemCount()
    {
        return itemCount;
    }

    public StewIngredient GetIngredient()
    {
        return ingredient;
    }
}
