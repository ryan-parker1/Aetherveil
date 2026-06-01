using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Attach to each row prefab in the vendor item list
public class VendorSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;

    private System.Action onActionClicked;

    public void SetupBuySlot(VendorItem vendorItem, System.Action onBuy)
    {
        if (vendorItem.item.icon != null)
            itemIcon.sprite = vendorItem.item.icon;

        itemNameText.text = vendorItem.item.itemName;
        priceText.text    = vendorItem.BuyPrice + "g";

        actionButtonText.text = "Buy";
        onActionClicked       = onBuy;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onActionClicked?.Invoke());
    }

    public void SetupSellSlot(InventorySlot inventorySlot, System.Action onSell)
    {
        int sellPrice = Mathf.Max(1, inventorySlot.Item.baseValue / 2);

        if (inventorySlot.Item.icon != null)
            itemIcon.sprite = inventorySlot.Item.icon;

        itemNameText.text = inventorySlot.Item.itemName +
            (inventorySlot.Quantity > 1 ? " x" + inventorySlot.Quantity : "");
        priceText.text    = sellPrice + "g";

        actionButtonText.text = "Sell";
        onActionClicked       = onSell;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onActionClicked?.Invoke());
    }
}
