using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [SerializeField]
    private List<LootEntry> lootEntries = new List<LootEntry>();

    /// <summary>
    /// Legacy offline path: rolls loot and adds items directly to an inventory.
    /// </summary>
    public void RollLoot(Inventory inventory)
    {
        foreach ((ItemData item, int qty) in RollLootItems())
        {
            inventory.AddItem(item, qty);
            Debug.Log($"Looted {qty}x {item.itemName}");
        }
    }

    /// <summary>
    /// Returns the items (and quantities) that dropped this roll.
    /// Used by EnemyLootDropper in multiplayer to spawn LootPickup objects.
    /// </summary>
    public List<(ItemData item, int quantity)> RollLootItems()
    {
        var drops = new List<(ItemData, int)>();

        foreach (LootEntry entry in lootEntries)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
            drops.Add((entry.item, qty));
        }

        return drops;
    }
}
