using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Recipes
{
    public string[] recipeNames;
    public string[] recipeDescriptions;
    public string[] effectTexts;
}

public abstract class StewIngredient : KeyItem
{
    protected Recipes recipes;

    public Recipes GetRecipes => recipes;

    public abstract void UpdatePlayerStatus(Player player, int selectIndex = 0);
}
