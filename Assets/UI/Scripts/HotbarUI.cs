using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField]
    private AbilityController abilityController;

    [SerializeField]
    private GameObject abilitySlotPrefab;

    private void Start()
    {
        GenerateHotbar();
    }

    private void GenerateHotbar()
    {
        foreach (AbilityData ability
            in abilityController.Abilities)
        {
            GameObject slot =
                Instantiate(
                    abilitySlotPrefab,
                    transform
                );

            AbilitySlotUI slotUI =
                slot.GetComponent<AbilitySlotUI>();

            slotUI.Setup(
                ability,
                abilityController
            );
        }
    }
}