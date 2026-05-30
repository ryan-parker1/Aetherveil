using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public ItemData item;

    [Range(0f, 100f)]
    public float dropChance = 50f;

    public int minQuantity = 1;

    public int maxQuantity = 1;
}