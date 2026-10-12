using System;

[Serializable]
public class InventorySlot
{
    public ItemDefinition item;
    public int amount;

    public InventorySlot(ItemDefinition item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}