using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField]
    private EquipmentManager equipmentManager;

    [SerializeField]
    private Image headSlot;

    [SerializeField]
    private Image chestSlot;

    [SerializeField]
    private Image weaponSlot;

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        UpdateSlot(
            EquipmentSlot.Head,
            headSlot
        );

        UpdateSlot(
            EquipmentSlot.Chest,
            chestSlot
        );

        UpdateSlot(
            EquipmentSlot.Weapon,
            weaponSlot
        );
    }

    private void UpdateSlot(
        EquipmentSlot slot,
        Image image
    )
    {
        ItemData item =
            equipmentManager.GetEquippedItem(slot);

        if (item != null)
        {
            image.sprite = item.icon;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color =
                new Color(
                    1,
                    1,
                    1,
                    0.2f
                );
        }
    }
}