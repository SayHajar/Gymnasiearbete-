using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    public event Action OnInventoryChanged;

    public void AddItem(ItemDefinition itemDef, int amount = 1)
    {
        if (itemDef.isStackable)
        {
            var slot = slots.Find(s => s.item == itemDef);
            if (slot != null)
                slot.amount += amount;
            else
                slots.Add(new InventorySlot(itemDef, amount));
        }
        else
        {
            for (var i = 0; i < amount; i++)
                slots.Add(new InventorySlot(itemDef, 1));
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"Item added to inventory: {itemDef.Name}");
    }

    public void RemoveItem(ItemDefinition itemDef, int amount = 1)
    {
        var slot = slots.Find(s => s.item == itemDef);
        if (slot != null)
        {
            slot.amount -= amount;
            if (slot.amount <= 0)
                slots.Remove(slot);
            OnInventoryChanged?.Invoke();
        }
    }

    // Drop one item at a position
    public void DropItem(ItemDefinition itemDef, Vector3 dropPosition, ItemWorldObj itemWorldObjPrefab)
    {
        RemoveItem(itemDef);
        ItemWorldObj.SpawnItemWorldObj(dropPosition, itemDef, 1, itemWorldObjPrefab, true);
    }
}