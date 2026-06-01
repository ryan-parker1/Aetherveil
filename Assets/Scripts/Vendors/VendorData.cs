using System;
using System.Collections.Generic;
using UnityEngine;

// One entry in a vendor's stock
[Serializable]
public class VendorItem
{
    public ItemData item;

    [Tooltip("Leave at 0 to use the item's baseValue automatically.")]
    public int priceOverride;

    // The actual buy price shown in the shop
    public int BuyPrice => priceOverride > 0 ? priceOverride : item.baseValue;

    // Players sell back at half the buy price
    public int SellPrice => Mathf.Max(1, BuyPrice / 2);
}

[CreateAssetMenu(
    fileName = "New Vendor",
    menuName = "MMO/Vendor"
)]
public class VendorData : ScriptableObject
{
    public string vendorName;

    public List<VendorItem> itemsForSale;
}
