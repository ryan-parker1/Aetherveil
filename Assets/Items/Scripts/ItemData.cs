using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "MMO/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Basic")]
    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Item Info")]
    public ItemRarity rarity;

    public bool stackable = true;

    public int maxStackSize = 99;

    public bool isEquipment;

    public EquipmentSlot equipmentSlot;

    public int bonusHealth;

    public int bonusDamage;

    [Header("Economy")]
    [Tooltip("Base gold value. Vendors sell at this price, players sell back at half.")]
    public int baseValue;
}