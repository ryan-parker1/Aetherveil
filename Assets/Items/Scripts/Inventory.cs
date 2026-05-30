using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int maxSlots = 20;

    [SerializeField]
    private List<InventorySlot> slots =
        new List<InventorySlot>();

    public IReadOnlyList<InventorySlot> Slots => slots;
    public event Action OnInventoryChanged;

    public bool AddItem(
        ItemData item,
        int quantity = 1)
    {
        if (item.stackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.Item == item)
                {
                    slot.Quantity += quantity;

                    Debug.Log(
                        $"Added {quantity} {item.itemName}"
                    );
                    OnInventoryChanged?.Invoke();

                    return true;
                }
            }
        }

        if (slots.Count >= maxSlots)
        {
            Debug.Log("Inventory Full");

            return false;
        }

        slots.Add(
            new InventorySlot(
                item,
                quantity
            )
        );

        Debug.Log(
            $"Added {item.itemName}"
        );

        OnInventoryChanged?.Invoke();

        return true;
    }

    public bool RemoveItem(
        ItemData item,
        int quantity = 1)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.Item != item)
                continue;

            slot.Quantity -= quantity;

            if (slot.Quantity <= 0)
            {
                slots.Remove(slot);
            }

            OnInventoryChanged?.Invoke();
            
            return true;
        }

        return false;
    }
}