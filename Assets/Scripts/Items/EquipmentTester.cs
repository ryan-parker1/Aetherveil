using UnityEngine;

public class EquipmentTester : MonoBehaviour
{
    [SerializeField]
    private ItemData testItem;

    [SerializeField]
    private ItemData secondTestItem;

    private EquipmentManager equipmentManager;

    private void Awake()
    {
        equipmentManager =
            GetComponent<EquipmentManager>();
    }

   private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            equipmentManager.Equip(
                testItem
            );
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            equipmentManager.Equip(
                secondTestItem
            );
        }
    }
}