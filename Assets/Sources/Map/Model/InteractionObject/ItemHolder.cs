using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : InteractionObject
{
    private const string ItemDisappearAnimationKey = "itemDisappear";

    [SerializeField] private int itemId;
    [SerializeField] private int gold;
    [SerializeField] private MeshRenderer itemView;
    [SerializeField] private bool isFixedItem;
    private float x;
    private float z;
    private float width;
    private ItemFactory factory = new ItemFactory();
    private InventoryItem item;

    private bool isInitialized = false;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        StartCoroutine(ItemObtainCoroutine(playerEventHandler));
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return x >= this.x - width && x <= this.x + width && z >= this.z - width && z <= this.z + width;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        if (isSelfCameraMode)
        {
            ResetItemHolder();
        }
        else
        {
            StartCoroutine(InteractWithPlayerCoroutine());
            StartCoroutine(RotateToSyncWithPlayerRotationCoroutine());
        }
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        return null;
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        // Do nothing
    }

    public void SetItemId(int itemId)
    {
        this.itemId = itemId;
    }

    public void SetGold(int gold)
    {
        this.gold = gold;
    }

    public void StartObservePlayer()
    {
        if (!isInitialized)
        {
            return;
        }

        this.gameObject.SetActive(true);
        mediator.AddPlayerStatusChangeObserver(this);

        StartCoroutine(InteractWithPlayerCoroutine());
        StartCoroutine(RotateToSyncWithPlayerRotationCoroutine());
    }

    public void ResetObserver()
    {
        if (!isInitialized)
        {
            return;
        }

        StopAllCoroutines();
        mediator.RemovePlayerStatusChangeObserver(this);
        this.gameObject.GetComponent<Animator>().SetBool(ItemDisappearAnimationKey, false);
        this.gameObject.SetActive(false);
    }

    public void SetItemInfo(int itemId, int gold, bool isFixedItem)
    {
#if UNITY_EDITOR
        this.itemId = itemId;
        this.gold = gold;
        this.isFixedItem = isFixedItem;
#endif
    }

    protected override void SetMediator()
    {
        mediator = this.gameObject.transform.parent.parent.GetComponent<PlayerInteractionMediator>();
        mediator.AddPlayerStatusChangeObserver(this);
    }

    protected override void SetStorage()
    {
        storage = this.gameObject.transform.parent.parent.GetComponent<InteractionObjectCountStorage>();
        AddItemToStorage();
    }

    private void AddItemToStorage()
    {
        if (!(item is KeyItem))
        {
            return;
        }

        KeyItem localKeyItem = item as KeyItem;
        if (localKeyItem.IsPlantType())
        {
            storage.AddPlantItem(localKeyItem.GetItemName());
        }
        else
        {
            storage.AddKeyItem();
        }
    }

    private void RemoveItemFromStorage()
    {
        if (!(item is KeyItem))
        {
            return;
        }

        KeyItem localKeyItem = item as KeyItem;
        if (localKeyItem.IsPlantType())
        {
            storage.RemovePlantItem(localKeyItem.GetItemName());
        }
        else
        {
            storage.RemoveKeyItem();
        }
    }

    private void Start()
    {
        if (itemId > 0)
        {
            CreateAndSetupItem(itemId);
        }
        else
        {
            itemView.material = Resources.Load("Materials/gold") as Material;
        }
        
        x = this.gameObject.transform.position.x;
        z = this.gameObject.transform.position.z;
        width = this.gameObject.transform.lossyScale.x * 5;

        StartCoroutine(InteractWithPlayerCoroutine());
        StartCoroutine(RotateToSyncWithPlayerRotationCoroutine());

        SetStorage();

        isInitialized = true;
    }

    private IEnumerator InteractWithPlayerCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => IsPlayerInActiveArea(playerPosition.localPosition.x, playerPosition.localPosition.z));
            mediator.SendPlayerInInteractionAreaEvent(DoInteraction);
        }
    }

    private IEnumerator RotateToSyncWithPlayerRotationCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => playerPosition.localEulerAngles.y != this.gameObject.transform.localEulerAngles.y);
            this.gameObject.transform.localEulerAngles = new Vector3(
                this.gameObject.transform.localEulerAngles.x, 
                playerPosition.localEulerAngles.y, 
                this.gameObject.transform.localEulerAngles.z
            );
        }
    }

    private void CreateAndSetupItem(int itemId)
    {
        item = factory.createItem(itemId);
        Material holderMaterial;

        if (item is ExpendableItem)
        {
            holderMaterial = Resources.Load("Materials/expendable") as Material;
        }
        else
        {
            holderMaterial = Resources.Load("Materials/" + item.GetItemId()) as Material;
        }

        itemView.material = holderMaterial;
    }

    private IEnumerator ItemObtainCoroutine(PlayerEventHandler playerEventHandler)
    {
        this.gameObject.GetComponent<Animator>().SetBool(ItemDisappearAnimationKey, true);
        yield return new WaitForSeconds(0.1f);

        if (itemId > 0)
        {
            playerEventHandler.ObtainItem(item);
        }
        else
        {
            playerEventHandler.ObtainGold(gold);
        }

        ResetItemHolder();

        mediator.RemovePlayerStatusChangeObserver(this);
        RemoveItemFromStorage();

        if (isFixedItem)
        {
            string id = this.gameObject.transform.parent.GetComponent<Floor>().GetId();
            MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);
        }
    }

    private void ResetItemHolder()
    {
        StopAllCoroutines();
        this.gameObject.GetComponent<Animator>().SetBool(ItemDisappearAnimationKey, false);
        this.gameObject.SetActive(false);
    }
}
