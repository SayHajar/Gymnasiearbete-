using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int capacity = 8;      // keep at 8 so every slot has a number key
    public int maxStack = 10;

    [System.Serializable]
    public class Slot
    {
        public ItemData item;
        public int count;
    }

    public List<Slot> slots = new List<Slot>();
    public int selectedIndex = -1;             // currently equipped hotbar slot

    public event Action OnChanged;             // UI listens to this

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        int remaining = amount;

        // 1) top up existing stacks
        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i].item == item && slots[i].count < maxStack)
            {
                int take = Mathf.Min(maxStack - slots[i].count, remaining);
                slots[i].count += take;
                remaining -= take;
            }
        }

        // 2) new stacks while there is room
        while (remaining > 0 && slots.Count < capacity)
        {
            int take = Mathf.Min(maxStack, remaining);
            slots.Add(new Slot { item = item, count = take });
            remaining -= take;
        }

        OnChanged?.Invoke();
        return remaining == 0;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        for (int i = slots.Count - 1; i >= 0 && amount > 0; i--)
        {
            if (slots[i].item != item) continue;

            int take = Mathf.Min(slots[i].count, amount);
            slots[i].count -= take;
            amount -= take;

            if (slots[i].count <= 0)
            {
                slots.RemoveAt(i);
                if (selectedIndex == i) SetSelected(-1);
                else if (selectedIndex > i) SetSelected(selectedIndex - 1);
            }
        }
        OnChanged?.Invoke();
    }

    public bool HasItem(ItemData item) => slots.Exists(s => s.item == item);

    public Slot GetSlot(int index)
        => (index >= 0 && index < slots.Count) ? slots[index] : null;

    public void SetSelected(int index)
    {
        selectedIndex = index;
        OnChanged?.Invoke();
    }
}