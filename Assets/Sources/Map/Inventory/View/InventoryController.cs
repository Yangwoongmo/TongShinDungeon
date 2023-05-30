using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour, IInventoryClickListener
{
    private const string OpenInventoryAnimationKey = "inventoryOpen";
    // Except memo
    private const int InventoryTabCount = 2;

    [SerializeField] private Transform[] itemTabs = new Transform[3];
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject itemDescription;
    [SerializeField] private Text descriptionBody;
    [SerializeField] private Text itemName;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemUseButton;
    [SerializeField] private Text playerGoldText;
    [SerializeField] private Animator inventoryAnimator;
    [SerializeField] private Image[] inventoryTabButtons;
    [SerializeField] private Sprite[] inventoryTabSprites;

    private Player player;
    private bool isAnimating = false;
    private ItemUseListener listener;

    private bool canUseItem = true;

    public void OpenInventory()
    {
        SelectInventoryTab(0);
        inventoryCanvas.SetActive(true);

        StartCoroutine(OpenInventoryCoroutine());
    }

    public void CloseInventory()
    {
        StartCoroutine(CloseInventoryCoroutine());
    }

    public void SelectInventoryTab(int tab)
    {
        if (isAnimating)
        {
            return;
        }
        CloseItemDescription();

        for (int i = 0; i < itemTabs.Length; i++)
        {
            if (i != tab)
            {
                itemTabs[i].gameObject.SetActive(false);
                inventoryTabButtons[i].sprite = inventoryTabSprites[1];
            }
            else
            {
                Transform scrollContentTransform = itemTabs[i].GetChild(0).GetChild(0);
                float currentScrollX = scrollContentTransform.GetComponent<RectTransform>().anchoredPosition.x;
                scrollContentTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentScrollX, 0, 0);

                itemTabs[i].gameObject.SetActive(true);
                inventoryTabButtons[i].sprite = inventoryTabSprites[0];
            }
        }
    }

    public void CloseItemDescription()
    {
        if (isAnimating)
        {
            return;
        }
        itemUseButton.gameObject.SetActive(false);
        itemDescription.SetActive(false);
    }

    public void SortInventory()
    {
        if (isAnimating)
        {
            return;
        }
        for (int i = 0; i < InventoryTabCount; i++)
        {
            player.GetInventory()[i].Sort(delegate ((InventoryItem, int) itemA, (InventoryItem, int) itemB)
            {
                return itemA.Item1.GetItemId() - itemB.Item1.GetItemId();
            });
        }

        ReRenderingInventory();
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SetItemUseListener(ItemUseListener listener)
    {
        this.listener = listener;
    }

    public void SetEnableUseItem(bool isEnabled)
    {
        canUseItem = isEnabled;
    }

    public void UseItem(InventoryItem item, bool needToUpdateInventoryUi = true)
    {
        if (!canUseItem)
        {
            return;
        }

        if (isAnimating)
        {
            return;
        }

        if (!player.UseItem(item))
        {
            return;
        }

        if (listener != null)
        {
            item.UseItemFromInventory(listener);
        }

        if (needToUpdateInventoryUi)
        {
            ReRenderingInventory();
            CloseItemDescription();
        }
    }

    private IEnumerator OpenInventoryCoroutine()
    {
        if (isAnimating)
        {
            yield break;
        }
        ReRenderingInventory();
        playerGoldText.text = string.Format("{0:#,###0}", player.GetGold());

        isAnimating = true;
        yield return new WaitForSeconds(0.5f);

        isAnimating = false;
    }

    private IEnumerator CloseInventoryCoroutine()
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

    private void OpenItemDescription(InventorySlot slot)
    {
        if (isAnimating)
        {
            return;
        }
        InventoryItem item = slot.GetInventoryItem();
        itemImage.sprite = Resources.Load<Sprite>("Image/ItemIcon/" + item.GetItemId().ToString()) as Sprite;
        itemName.text = item.GetItemName();
        descriptionBody.text = item.GetItemDescription();
        if (NeedToActiveUseButton(item))
        {
            itemUseButton.gameObject.SetActive(true);
            itemUseButton.onClick.RemoveAllListeners();
            itemUseButton.onClick.AddListener(() => UseItem(item));
        }
        itemDescription.gameObject.SetActive(true);
    }

    private bool NeedToActiveUseButton(InventoryItem item)
    {
        return item is ExpendableItem && (item as ExpendableItem).CanUserFromInventory() && canUseItem;
    }

    private void SetupItemTabs(int tab)
    {
        Transform itemTransform = this.itemTabs[tab].GetChild(0).GetChild(0);
        List<(InventoryItem, int)> Items = player.GetInventory()[tab];
        int currentSlotCount = itemTransform.childCount;

        for (int i = 0; i < itemTransform.childCount; i++)
        {
            itemTransform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < Items.Count; i++)
        {
            (InventoryItem item, int number) itemDetail = Items[i];
            InventorySlot slot;
            if (currentSlotCount >= i + 1)
            {
                slot = itemTransform.GetChild(i).gameObject.GetComponent<InventorySlot>();
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Object.Instantiate<GameObject>(this.inventorySlot, itemTransform).GetComponent<InventorySlot>();
            }
            slot.SetInventoryItem(itemDetail.item);
            slot.SetNumberOfItem(itemDetail.number);
            Button slotButton = slot.gameObject.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => OpenItemDescription(slot));
        }
    }

    private void ResetItems(int tab)
    {
        for (int i = 0; i < itemTabs[tab].GetChild(0).GetChild(0).childCount; i++)
        {
            itemTabs[tab].GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ReRenderingInventory()
    {
        for (int i = 0; i < InventoryTabCount; i++)
        {
            SetupItemTabs(i);
        }
    }

    private enum InventoryTab
    {
        EXPENDABLES,
        KEY_ITEMS,
        MEMOS
    }
}
