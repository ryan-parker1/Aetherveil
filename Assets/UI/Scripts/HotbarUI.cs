using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField]
    private AbilityController abilityController;

    [SerializeField]
    private GameObject abilitySlotPrefab;

    // Called by NetworkPlayerSetup when the local player spawns
    public void SetAbilityController(AbilityController controller)
    {
        abilityController = controller;
        GenerateHotbar();
    }

    private void Start()
    {
        // AbilityController is on the Player spawned by FishNet
        // — wait until it exists
        if (abilityController == null)
            abilityController =
                FindAnyObjectByType<AbilityController>();

        if (abilityController != null)
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