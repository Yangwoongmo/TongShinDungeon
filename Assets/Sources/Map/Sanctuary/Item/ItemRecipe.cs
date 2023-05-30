using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRecipe : MonoBehaviour
{
    [SerializeField] private Text recipeNameText;
    [SerializeField] private Text recipeDescriptionsText;

    private StewIngredient item;

    public void SetItem(StewIngredient item)
    {
        this.item = item;
    }

    public void SetRecipe(int recipeCount)
    {
        recipeNameText.text = item.GetRecipes.recipeNames[recipeCount];
        recipeDescriptionsText.text = item.GetRecipes.recipeDescriptions[recipeCount];
    }
}
