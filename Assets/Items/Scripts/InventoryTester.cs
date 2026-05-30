using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField]
    private ItemData testItem;

    private Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            inventory.AddItem(
                testItem,
                1
            );
        }
    }
}