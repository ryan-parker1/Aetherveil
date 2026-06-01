using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject vendorWindow;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI vendorNameText;
    [SerializeField] private Button closeButton;

    [Header("Tabs")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;

    [Header("Item List")]
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject vendorSlotPrefab;

    [Header("Footer")]
    [SerializeField] private TextMeshProUGUI playerGoldText;

    public bool IsOpen => vendorWindow.activeSelf;

    private VendorData currentVendor;
    private PlayerGold playerGold;
    private Inventory playerInventory;

    private void Start()
    {
        vendorWindow.SetActive(false);

        closeButton.onClick.AddListener(Close);
        buyTabButton.onClick.AddListener(ShowBuyTab);
        sellTabButton.onClick.AddListener(ShowSellTab);
    }

    public void OpenShop(VendorData vendor)
    {
        currentVendor  = vendor;
        playerGold     = FindAnyObjectByType<PlayerGold>();
        playerInventory = FindAnyObjectByType<Inventory>();

        vendorNameText.text = vendor.vendorName;
        vendorWindow.SetActive(true);

        if (playerGold != null)
        {
            playerGold.OnGoldChanged -= RefreshGoldDisplay;
            playerGold.OnGoldChanged += RefreshGoldDisplay;
        }

        isBuyTab = true;
        RefreshGoldDisplay();
        PopulateBuyList();
    }

    public void Close()
    {
        vendorWindow.SetActive(false);

        if (playerGold != null)
            playerGold.OnGoldChanged -= RefreshGoldDisplay;
    }

    private void ShowBuyTab()
    {
        isBuyTab = true;
        PopulateBuyList();
    }

    private void ShowSellTab()
    {
        isBuyTab = false;
        PopulateSellList();
    }

    private void PopulateBuyList()
    {
        ClearList();

        if (currentVendor == null) return;

        foreach (VendorItem vendorItem in currentVendor.itemsForSale)
        {
            VendorItem captured = vendorItem; // capture for lambda

            GameObject slot = Instantiate(vendorSlotPrefab, itemListParent);
            VendorSlotUI slotUI = slot.GetComponent<VendorSlotUI>();

            slotUI.SetupBuySlot(captured, () => BuyItem(captured));
        }
    }

    private void PopulateSellList()
    {
        ClearList();

        if (playerInventory == null) return;

        foreach (InventorySlot slot in playerInventory.Slots)
        {
            InventorySlot captured = slot; // capture for lambda

            GameObject slotObj = Instantiate(vendorSlotPrefab, itemListParent);
            VendorSlotUI slotUI = slotObj.GetComponent<VendorSlotUI>();

            slotUI.SetupSellSlot(captured, () => SellItem(captured));
        }
    }

    private void BuyItem(VendorItem vendorItem)
    {
        if (playerGold == null || playerInventory == null) return;

        if (!playerGold.SpendGold(vendorItem.BuyPrice))
        {
            Debug.Log("Cannot afford: " + vendorItem.item.itemName);
            return;
        }

        playerInventory.AddItem(vendorItem.item);
        Debug.Log("Bought: " + vendorItem.item.itemName);
    }

    private void SellItem(InventorySlot slot)
    {
        if (playerGold == null || playerInventory == null) return;

        int sellPrice = Mathf.Max(1, slot.Item.baseValue / 2);

        playerInventory.RemoveItem(slot.Item);
        playerGold.AddGold(sellPrice);

        Debug.Log("Sold: " + slot.Item.itemName + " for " + sellPrice + "g");

        // Refresh the sell list so the sold item disappears
        PopulateSellList();
    }

    private void RefreshGoldDisplay()
    {
        if (playerGold != null)
            playerGoldText.text = "Gold: " + playerGold.Gold + "g";
    }

    private void ClearList()
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);
    }
}
