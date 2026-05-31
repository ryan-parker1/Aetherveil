using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField]
    private GameObject equipmentWindow;

    [SerializeField]
    private EquipmentManager equipmentManager;

    [SerializeField]
    private Image headSlot;

    [SerializeField]
    private Image chestSlot;

    [SerializeField]
    private Image gloveSlot;

    [SerializeField]
    private Image weaponSlot;

    [SerializeField]
    private Image legSlot;

    [SerializeField]
    private Image feetSlot;

    private void Start()
    {
        equipmentManager.OnEquipmentChanged += Refresh;
    }

    private void OnDestroy()
    {
        equipmentManager.OnEquipmentChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        equipmentWindow.SetActive(
            !equipmentWindow.activeSelf
        );

        if (equipmentWindow.activeSelf)
        {
            Refresh();
        }
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
            EquipmentSlot.Glove,
            gloveSlot
        );

        UpdateSlot(
            EquipmentSlot.Weapon,
            weaponSlot
        );

        UpdateSlot(
            EquipmentSlot.Legs,
            legSlot
        );

        UpdateSlot(
            EquipmentSlot.Feet,
            feetSlot
        );
    }

    private void UpdateSlot(
        EquipmentSlot slot,
        Image image
    )
    {

        if (image == null)
        {
            Debug.LogError(
                slot + " image is not assigned!"
            );

            return;
        }
        
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