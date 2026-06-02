using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    [SerializeField] private Transform gridParent;

    // Inventory lives on the Player prefab spawned by FishNet.
    // Find it lazily the first time it is needed.
    private Inventory inventory;

    private void OnEnable()
    {
        TryCacheInventory();

        if (inventory != null)
            inventory.OnInventoryChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    private void TryCacheInventory()
    {
        if (inventory != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            inventory = player.GetComponent<Inventory>();
    }

    public void Refresh()
    {
        // Try to find inventory again in case it wasn't ready during OnEnable.
        TryCacheInventory();

        if (inventory == null)
        {
            Debug.LogWarning("InventoryUI: player not spawned yet — inventory unavailable.");
            return;
        }

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (InventorySlot slot in inventory.Slots)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            obj.GetComponent<InventorySlotUI>()?.Setup(slot);
        }
    }
}
