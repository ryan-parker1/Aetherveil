using UnityEngine;

public class EquipmentWindowController : MonoBehaviour
{
    [SerializeField]
    private GameObject equipmentWindow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            equipmentWindow.SetActive(
                !equipmentWindow.activeSelf
            );
        }
    }
}