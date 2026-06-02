using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// A world-space item pickup spawned by the server when an enemy dies.
/// Any player who walks over it can claim it:
///   1. Owner client's OnTriggerEnter fires → sends ServerRpc
///   2. Server validates the claim, sends TargetRpc to that client, despawns pickup
///   3. Client receives TargetRpc and adds the item to their local Inventory
///
/// Item identity is passed as a name string and resolved via GameRegistry
/// so ScriptableObject references don't need to cross the network.
/// </summary>
public class LootPickup : NetworkBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer pickupRenderer;
    [SerializeField] private Color    pickupColour = Color.yellow;

    // Set by EnemyLootDropper before ServerManager.Spawn() is called.
    private string _itemName;
    private int    _quantity;
    private bool   _claimed;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        // Auto-find renderer on this GameObject if not assigned in Inspector.
        if (pickupRenderer == null)
            pickupRenderer = GetComponent<Renderer>();

        if (pickupRenderer != null)
            pickupRenderer.material.color = pickupColour;
    }

    /// <summary>Called on the server before Spawn() to set the item payload.</summary>
    public void Initialize(string itemName, int quantity)
    {
        _itemName = itemName;
        _quantity = quantity;
    }

    // -------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"LootPickup trigger entered by: {other.gameObject.name} (tag: {other.tag})");

        if (!other.CompareTag("Player")) return;

        // In offline mode (no FishNet) add the item directly.
        if (!InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted)
        {
            GrantOffline(other.gameObject);
            return;
        }

        // In multiplayer only the owning client sends the claim RPC,
        // preventing every client from firing it simultaneously.
        NetworkObject playerNet = other.GetComponent<NetworkObject>();
        if (playerNet == null || !playerNet.IsOwner) return;

        Debug.Log($"LootPickup: sending ServerClaim for {_itemName}");
        ServerClaim(playerNet);
    }

    // -------------------------------------------------------------------------

    [ServerRpc(RequireOwnership = false)]
    private void ServerClaim(NetworkObject claimingPlayer)
    {
        if (_claimed) return;   // already taken by someone else
        _claimed = true;

        TargetGrantLoot(claimingPlayer.Owner, _itemName, _quantity);

        // Despawn so no other client can claim it.
        ServerManager.Despawn(gameObject);
    }

    [TargetRpc]
    private void TargetGrantLoot(NetworkConnection conn, string itemName, int quantity)
    {
        GameRegistry registry = FindAnyObjectByType<GameRegistry>();
        if (registry == null)
        {
            Debug.LogWarning("LootPickup: GameRegistry not found.");
            return;
        }

        ItemData item = registry.GetItem(itemName);
        if (item == null)
        {
            Debug.LogWarning($"LootPickup: item '{itemName}' not in GameRegistry.");
            return;
        }

        Inventory inventory = FindAnyObjectByType<Inventory>();
        if (inventory == null) return;

        inventory.AddItem(item, quantity);
        Debug.Log($"Looted {quantity}x {item.itemName}");
    }

    // -------------------------------------------------------------------------

    private void GrantOffline(GameObject player)
    {
        GameRegistry registry = FindAnyObjectByType<GameRegistry>();
        if (registry == null) return;

        ItemData item = registry.GetItem(_itemName);
        if (item == null) return;

        player.GetComponent<Inventory>()?.AddItem(item, _quantity);
        Debug.Log($"Looted {_quantity}x {item.itemName} (offline)");

        Destroy(gameObject);
    }
}
