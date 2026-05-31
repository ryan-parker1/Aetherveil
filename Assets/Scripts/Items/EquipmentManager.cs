using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentManager : MonoBehaviour
{
    public event Action OnEquipmentChanged;
    private CombatStats stats;

    private Dictionary<EquipmentSlot, ItemData>
        equippedItems =
            new Dictionary<EquipmentSlot, ItemData>();

    public bool Equip(ItemData item)
    {
        if (!item.isEquipment)
        {
            Debug.Log(
                item.itemName +
                " is not equipment."
            );

            return false;
        }

        if (equippedItems.ContainsKey(
                item.equipmentSlot))
        {
            Unequip(item.equipmentSlot);
        }

        equippedItems[item.equipmentSlot] = item;

        OnEquipmentChanged?.Invoke();

        stats.AddHealthBonus(item.bonusHealth);

        stats.AddDamageBonus(item.bonusDamage);

        Debug.Log(
            "Equipped: " +
            item.itemName
        );

        Debug.Log(
            "Total Damage: " +
            stats.TotalDamage
        );

        Debug.Log(
            "Total Health: " +
            stats.TotalHealth
        );

        return true;
    }

    public ItemData GetEquippedItem(
        EquipmentSlot slot)
    {
        equippedItems.TryGetValue(
            slot,
            out ItemData item
        );

        return item;
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (!equippedItems.TryGetValue(
            slot,
            out ItemData item))
        {
            return;
        }

        stats.RemoveHealthBonus(
            item.bonusHealth
        );

        stats.RemoveDamageBonus(
            item.bonusDamage
        );

        equippedItems.Remove(slot);

        Debug.Log(
            "Unequipped: " +
            item.itemName
        );
    }

    private void Awake()
    {
        stats = GetComponent<CombatStats>();
    }
}