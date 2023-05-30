using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventoryController : MonoBehaviour
{
    private const string OpenInventoryAnimationKey = "inventoryOpen";
    private readonly int[] shopItemIdList = new int[] { 106, 107, 108, 1, 5, 6 };

    [SerializeField] private GameObject shopItemSlot;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject itemDescription;
    [SerializeField] private Text descriptionBody;
    [SerializeField] private Text itemName;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text playerGoldText;
    [SerializeField] private Animator inventoryAnimator;
    [SerializeField] private Transform itemScrollContentTransform;
    [SerializeField] private Button itemPurchaseButton;
    [SerializeField] private Text itemPurchaseButtonText;
    [SerializeField] private Text itemPriceText;

    private Player player;
    private bool isAnimating = false;

    private PlayerManager manager = PlayerManager.GetInstance();
    private SanctuaryInfoRepository repository = SanctuaryInfoRepository.GetInstance();

    private Action saveDataCallback;

    public void OpenShoppingList()
    {
        SelectInventoryTab();
        inventoryCanvas.SetActive(true);

        StartCoroutine(OpenShoppingListCoroutine());
    }

    public void CloseShoppingList()
    {
        StartCoroutine(CloseShoppingListCoroutine());
    }

    public void CloseItemDescription()
    {
        if (isAnimating)
        {
            return;
        }
        itemDescription.SetActive(false);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetSaveDataCallback(Action saveDataCallback)
    {
        this.saveDataCallback = saveDataCallback;
    }

    public void PurchaseItem(ShopItem shopItem)
    {
        if (isAnimating)
        {
            return;
        }

        InventoryItem item = shopItem.GetItem();

        if (player.GetGold() < item.GetPrice() || item.GetItemStock() <= 0)
        {
            return;
        }
        player.DecreaseGold(item.GetPrice());
        player.ObtainItem(item);
        saveDataCallback.Invoke();

        int updatedStock = item.GetItemStock() - 1;
        int updatedPrice = item.GetPrice() + item.GetStockPrice();
        shopItem.SetItemPriceAndStock(updatedPrice, updatedStock);
        repository.SetSanctuaryShopItemStock(item.GetItemId().ToString(), updatedStock, updatedPrice);

        UpdateGoldText();
        UpdateDescriptionPurchaseAndPriceText(item);
    }

    private void SelectInventoryTab()
    {
        if (isAnimating)
        {
            return;
        }
        CloseItemDescription();

        float currentScrollX = itemScrollContentTransform.GetComponent<RectTransform>().anchoredPosition.x;
        itemScrollContentTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentScrollX, 0, 0);
    }

    private IEnumerator OpenShoppingListCoroutine()
    {
        if (isAnimating)
        {
            yield break;
        }
        SetupItemTab();
        UpdateGoldText();

        isAnimating = true;
        yield return new WaitForSeconds(0.5f);

        isAnimating = false;
    }

    private IEnumerator CloseShoppingListCoroutine()
    {
        if (isAnimating)
        {
            yield break;
        }

        inventoryAnimator.SetBool(OpenInventoryAnimationKey, false);
        isAnimating = true;
        yield return new WaitForSeconds(0.5f);

        isAnimating = false;

        CloseItemDescription();
        ResetItems();
        inventoryCanvas.SetActive(false);
    }

    private void Start()
    {
        SetupItemTab();
    }

    private void OpenItemDescription(ShopItem shopItem)
    {
        if (isAnimating)
        {
            return;
        }
        InventoryItem item = shopItem.GetItem();
        itemImage.sprite = Resources.Load<Sprite>("Image/ItemIcon/" + item.GetItemId().ToString()) as Sprite;
        itemName.text = item.GetItemName();
        descriptionBody.text = item.GetItemDescription();
        UpdateDescriptionPurchaseAndPriceText(item);

        itemPurchaseButton.onClick.RemoveAllListeners();
        itemPurchaseButton.onClick.AddListener(() => PurchaseItem(shopItem));
        itemDescription.gameObject.SetActive(true);
    }

    private void UpdateDescriptionPurchaseAndPriceText(InventoryItem item)
    {
        if (item.GetItemStock() <= 0)
        {
            itemPurchaseButtonText.text = "품절";
            itemPriceText.text = "없음";
        }
        else
        {
            itemPurchaseButtonText.text = "구매";
            itemPriceText.text = item.GetPrice().ToString();
        }
    }

    private void SetupItemTab()
    {
        int currentSlotCount = itemScrollContentTransform.childCount;

        for (int i = 0; i < currentSlotCount; i++)
        {
            itemScrollContentTransform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < shopItemIdList.Length; i++)
        {
            int itemId = shopItemIdList[i];
            (int itemStock, int itemPrice) = repository.HasSanctuaryShopItemKey(itemId) ? repository.GetSanctuaryShopItemStock(itemId) : (-1, -1);

            if (itemStock == 0)
            {
                continue;
            }

            ShopItem shopItem;
            if (currentSlotCount >= i + 1)
            {
                shopItem = itemScrollContentTransform.GetChild(i).gameObject.GetComponent<ShopItem>();
                shopItem.gameObject.SetActive(true);
            }
            else
            {
                shopItem = UnityEngine.Object.Instantiate<GameObject>(this.shopItemSlot, itemScrollContentTransform).GetComponent<ShopItem>();
            }
            
            shopItem.SetItem(itemId);
            shopItem.SetItemPriceAndStock(itemPrice, itemStock);

            Button shopItemButton = shopItem.gameObject.GetComponent<Button>();
            shopItemButton.onClick.RemoveAllListeners();
            shopItemButton.onClick.AddListener(() => OpenItemDescription(shopItem));
        }
    }

    private void ResetItems()
    {
        for (int i = 0; i < itemScrollContentTransform.childCount; i++)
        {
            itemScrollContentTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void UpdateGoldText()
    {
        playerGoldText.text = string.Format("{0:#,###0}", player.GetGold());
    }

    private enum InventoryTab
    {
        EXPENDABLES,
        KEY_ITEMS,
        MEMOS
    }
}
