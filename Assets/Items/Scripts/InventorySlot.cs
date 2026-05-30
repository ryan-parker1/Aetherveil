[System.Serializable]
public class InventorySlot
{
    public ItemData Item;

    public int Quantity;

    public InventorySlot(
        ItemData item,
        int quantity
    )
    {
        Item = item;
        Quantity = quantity;
    }
}