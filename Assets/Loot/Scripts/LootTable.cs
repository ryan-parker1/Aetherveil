using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [SerializeField]
    private List<LootEntry> lootEntries =
        new List<LootEntry>();

    public void RollLoot(Inventory inventory)
    {
        foreach (LootEntry entry in lootEntries)
        {
            float roll =
                Random.Range(0f, 100f);

            if (roll > entry.dropChance)
                continue;

            int quantity =
                Random.Range(
                    entry.minQuantity,
                    entry.maxQuantity + 1
                );

            inventory.AddItem(
                entry.item,
                quantity
            );

            Debug.Log(
                $"Looted {quantity}x {entry.item.itemName}"
            );
        }
    }
}