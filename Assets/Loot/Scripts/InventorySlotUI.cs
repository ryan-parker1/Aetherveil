using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    private InventorySlot slot;
    [SerializeField] private Image icon;

    [SerializeField] private TextMeshProUGUI quantityText;

    public void Setup(InventorySlot slot)
    {
        this.slot = slot;

        icon.sprite = slot.Item.icon;

        quantityText.text =
            slot.Quantity > 1
                ? slot.Quantity.ToString()
                : "";
    }

    public void OnClick()
    {
        if (!slot.Item.isEquipment)
            return;

        EquipmentManager equipmentManager =
            FindFirstObjectByType<EquipmentManager>();

        if (equipmentManager == null)
            return;

        equipmentManager.Equip(slot.Item);

        Debug.Log(
            "Equipped from inventory: "
            + slot.Item.itemName
        );
    }
}