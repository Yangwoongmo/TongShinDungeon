using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewInventoryController : MonoBehaviour
{
    private const string OpenInventoryAnimationKey = "inventoryOpen";
    private const int InventoryTabCount = 3;

    [SerializeField] private Transform[] itemTabs = new Transform[3];
    [SerializeField] private Transform[] itemScrollAreas = new Transform[3];

    [SerializeField] private GameObject stewItemPrefab;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject recipeSelection;

    [SerializeField] private Text[] recipeNames;
    [SerializeField] private Text[] recipeDescriptions;
    [SerializeField] private Button[] cookIngredientButtons;

    [SerializeField] private Animator inventoryAnimator;
    [SerializeField] private Image[] inventoryTabButtons;
    [SerializeField] private Sprite[] inventoryTabSprites;

    [SerializeField] private Transform showMatterEffectZone;
    [SerializeField] private Transform matterEffectPool;
    [SerializeField] private GameObject usingMatterEffectPrefab;

    private Player player;
    private bool isAnimating = false;

    private PlayerManager manager = PlayerManager.GetInstance();
    private Action saveDataCallback;

    public void OpenStewItemList()
    {
        SelectInventoryTab(0);
        inventoryCanvas.SetActive(true);

        StartCoroutine(OpenStewItemListCoroutine());
    }

    public void CloseStewItemList()
    {
        StartCoroutine(CloseStewItemListCoroutine());
    }

    public void SelectInventoryTab(int tab)
    {
        if (isAnimating)
        {
            return;
        }
        CloseRecipeSelection();

        for (int i = 0; i < itemTabs.Length; i++)
        {
            if (i != tab)
            {
                itemTabs[i].gameObject.SetActive(false);
                inventoryTabButtons[i].sprite = inventoryTabSprites[1];
            }
            else
            {
                Transform scrollContentTransform = itemScrollAreas[i];
                float currentScrollX = scrollContentTransform.GetComponent<RectTransform>().anchoredPosition.x;
                scrollContentTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentScrollX, 0, 0);

                itemTabs[i].gameObject.SetActive(true);
                inventoryTabButtons[i].sprite = inventoryTabSprites[0];
            }
        }
    }

    public void CloseRecipeSelection()
    {
        if (isAnimating)
        {
            return;
        }
        recipeSelection.SetActive(false);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetSaveDataCallback(Action saveDataCallback)
    {
        this.saveDataCallback = saveDataCallback;
    }

    private IEnumerator OpenStewItemListCoroutine()
    {
        if (isAnimating)
        {
            yield break;
        }
        ReRenderingInventory();

        isAnimating = true;
        yield return new WaitForSeconds(0.5f);

        isAnimating = false;
    }

    private IEnumerator CloseStewItemListCoroutine()
    {
        if (isAnimating)
        {
            yield break;
        }

        inventoryAnimator.SetBool(OpenInventoryAnimationKey, false);
        isAnimating = true;
        yield return new WaitForSeconds(0.5f);

        isAnimating = false;

        CloseRecipeSelection();
        for (int i = 0; i < InventoryTabCount; i++)
        {
            ResetItems(i);
        }
        inventoryCanvas.SetActive(false);
    }

    private void Start()
    {
        ReRenderingInventory();
    }

    private void OpenRecipeSelection(StewItem item)
    {
        if (isAnimating)
        {
            return;
        }

        StewIngredient ingredient = item.GetIngredient();
        if (!ingredient.IsPlantType())
        {
            return;
        }

        Recipes recipes = ingredient.GetRecipes;
        for (int i = 0; i < 2; i++)
        {
            recipeNames[i].text = recipes.recipeNames[i];
            recipeDescriptions[i].text = recipes.recipeDescriptions[i];

            cookIngredientButtons[i].onClick.RemoveAllListeners();
            cookIngredientButtons[i].onClick.AddListener(() =>
            {
                item.CookIngredient(player, i);
                saveDataCallback.Invoke();

                CloseRecipeSelection();
                ReRenderingInventory();
            });
        }
        
        recipeSelection.SetActive(true);
    }

    private void SetupItemTabs(int tab)
    {
        Transform itemTransform = itemScrollAreas[tab];
        List<(InventoryItem, int)> keyItems = player.GetInventory()[1];
        List<(InventoryItem, int)> stewItems = new List<(InventoryItem, int)>();

        for (int i = 0; i < keyItems.Count; i++)
        {
            if (keyItems[i].Item2 <= 0)
            {
                continue;
            }

            KeyItem item = keyItems[i].Item1 as KeyItem;

            if (!(item is StewIngredient))
            {
                continue;
            }

            if ((tab == (int)StewInventoryTab.PLANTS && !item.IsPlantType()) || 
                (tab == (int)StewInventoryTab.INGREDIENTS && item.IsPlantType()))
            {
                continue;
            }

            stewItems.Add(keyItems[i]);
        }
        int currentSlotCount = itemTransform.childCount;

        for (int i = 0; i < itemTransform.childCount; i++)
        {
            itemTransform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < stewItems.Count; i++)
        {
            StewIngredient item = stewItems[i].Item1 as StewIngredient;
            int itemCount = stewItems[i].Item2;

            StewItem stewItem;
            if (currentSlotCount >= i + 1)
            {
                stewItem = itemTransform.GetChild(i).gameObject.GetComponent<StewItem>();
                stewItem.gameObject.SetActive(true);
            }
            else
            {
                stewItem = Instantiate<GameObject>(stewItemPrefab, itemTransform).GetComponent<StewItem>();
            }

            stewItem.SetStewItem(item, itemCount);
            stewItem.SetCookButtonClickListener(() => TryCookIngredients(stewItem));
        }
    }

    private void TryCookIngredients(StewItem item)
    {
        StewIngredient ingredient = item.GetIngredient();
        if (!ingredient.IsPlantType())
        {
            AppearMatterEffect(ingredient);

            item.CookIngredient(player);
            saveDataCallback.Invoke();

            if (item.GetItemCount() <= 0)
            {
                ReRenderingInventory();
            }
        }
        else
        {
            OpenRecipeSelection(item);
        }
    }

    private void ResetItems(int tab)
    {
        for (int i = 0; i < itemScrollAreas[tab].childCount; i++)
        {
            itemScrollAreas[tab].GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ReRenderingInventory()
    {
        for (int i = 0; i < InventoryTabCount; i++)
        {
            SetupItemTabs(i);
        }
    }

    private void AppearMatterEffect(StewIngredient item)
    {
        string text = item.GetRecipes.effectTexts[0];
        MatterEffect matterEffect;

        if (matterEffectPool.childCount > 0)
        {
            matterEffect = matterEffectPool.GetChild(0).GetComponent<MatterEffect>();
            matterEffect.transform.SetParent(showMatterEffectZone);
        }
        else
        {
            GameObject effectText = Instantiate(usingMatterEffectPrefab, showMatterEffectZone);
            matterEffect = effectText.GetComponent<MatterEffect>();
        }

        matterEffect.StartEffect(text, matterEffectPool);
    }

    private enum StewInventoryTab
    {
        ALL,
        PLANTS,
        INGREDIENTS
    }
}
