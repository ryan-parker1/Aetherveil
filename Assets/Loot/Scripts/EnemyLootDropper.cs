using FishNet;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(LootTable))]
public class EnemyLootDropper : MonoBehaviour
{
    [Tooltip("Prefab with LootPickup + NetworkObject + Collider (trigger).")]
    [SerializeField] private GameObject lootPickupPrefab;

    private Health    health;
    private LootTable lootTable;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        health    = GetComponent<Health>();
        lootTable = GetComponent<LootTable>();
    }

    private void OnEnable()  { health.OnDeath += DropLoot; }
    private void OnDisable() { health.OnDeath -= DropLoot; }

    // -------------------------------------------------------------------------

    private void DropLoot()
    {
        bool offline = !InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted;

        if (!offline && !InstanceFinder.IsServerStarted)
            return; // non-server clients do nothing — server handles drops

        var drops = lootTable.RollLootItems();

        foreach ((ItemData item, int qty) in drops)
        {
            if (offline)
            {
                // Offline: add directly to the local player's inventory.
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player?.GetComponent<Inventory>()?.AddItem(item, qty);
                Debug.Log($"Looted {qty}x {item.itemName} (offline)");
                continue;
            }

            // Multiplayer: spawn a world pickup that any player can claim.
            if (lootPickupPrefab == null)
            {
                Debug.LogWarning("EnemyLootDropper: lootPickupPrefab not assigned.");
                continue;
            }

            // Scatter drops slightly so stacked items are individually pickable.
            Vector3 offset = new Vector3(
                Random.Range(-0.6f, 0.6f), 0f, Random.Range(-0.6f, 0.6f)
            );
            Vector3 spawnPos = transform.position + offset;

            GameObject pickupObj = Instantiate(lootPickupPrefab, spawnPos, Quaternion.identity);
            LootPickup pickup = pickupObj.GetComponent<LootPickup>();
            pickup.Initialize(item.itemName, qty);

            InstanceFinder.ServerManager.Spawn(pickupObj);
        }
    }
}
