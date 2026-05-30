using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryWindow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I key pressed!");

            inventoryWindow.SetActive(
                !inventoryWindow.activeSelf
            );
        }
    }
}