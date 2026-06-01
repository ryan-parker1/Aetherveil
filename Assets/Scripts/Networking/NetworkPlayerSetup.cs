using FishNet.Object;
using UnityEngine;

// Attach to the Player prefab alongside NetworkObject.
// Runs once when the player spawns on a client and handles
// per-ownership setup: camera assignment and component gating.
public class NetworkPlayerSetup : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerController playerController =
            GetComponent<PlayerController>();

        NPCInteractor npcInteractor =
            GetComponent<NPCInteractor>();

        if (IsOwner)
        {
            // Point the scene camera at this player
            CameraController cam =
                FindAnyObjectByType<CameraController>();
            if (cam != null)
                cam.SetTarget(transform);

            if (playerController != null)
                playerController.enabled = true;

            if (npcInteractor != null)
                npcInteractor.enabled = true;

            // Wire up UI systems that need player references
            EquipmentUI equipmentUI =
                FindAnyObjectByType<EquipmentUI>();
            if (equipmentUI != null)
                equipmentUI.SetEquipmentManager(
                    GetComponent<EquipmentManager>()
                );

            HotbarUI hotbarUI =
                FindAnyObjectByType<HotbarUI>();
            if (hotbarUI != null)
                hotbarUI.SetAbilityController(
                    GetComponent<AbilityController>()
                );
        }
        else
        {
            // Disable input-driven components on other players
            if (playerController != null)
                playerController.enabled = false;

            if (npcInteractor != null)
                npcInteractor.enabled = false;
        }
    }
}
