using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject slotPrefab;

    [SerializeField] private Transform gridParent;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        Debug.Log("Refreshing Inventory UI");

        if (inventory == null)
        {
            Debug.LogError("Inventory reference is missing!");
            return;
        }

        Debug.Log("Inventory Slots: " + inventory.Slots.Count);

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot slot in inventory.Slots)
        {
            GameObject obj =
                Instantiate(slotPrefab, gridParent);

            obj.GetComponent<InventorySlotUI>()
                .Setup(slot);
        }
    }
}