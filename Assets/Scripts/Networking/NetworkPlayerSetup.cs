using FishNet.Object;
using UnityEngine;

// Attach to the Player prefab alongside NetworkObject.
// Runs once when the player spawns on a client and handles
// per-ownership setup: camera assignment and component gating.
public class NetworkPlayerSetup : NetworkBehaviour
{
    [Header("Owner-Only Components")]
    [Tooltip("These components are disabled for all non-owner clients.")]
    [SerializeField] private MonoBehaviour[] ownerOnlyComponents;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            // Point the scene camera at this player
            CameraController cam =
                FindAnyObjectByType<CameraController>();

            if (cam != null)
                cam.SetTarget(transform);

            // Enable all owner-only components (they start disabled on prefab)
            foreach (MonoBehaviour comp in ownerOnlyComponents)
                if (comp != null) comp.enabled = true;
        }
        else
        {
            // Disable input-driven components on other players
            foreach (MonoBehaviour comp in ownerOnlyComponents)
                if (comp != null) comp.enabled = false;
        }
    }
}
