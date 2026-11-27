using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    public event Action OnInventoryChanged;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !StoneThrower.IsAiming && InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ToggleUI();
        }
    }


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

    // Throw one item with a direction and a force. The item is removed first when the throw happens.
    public bool ThrowItem(ItemDefinition itemDef, Vector2 direction, float power, ItemWorldObj itemWorldObjPrefab,
        float gravityScale = 1f, float spin = 360f)
    {
        if (itemDef == null || itemWorldObjPrefab == null) return false;
        if (!slots.Exists(s => s.item == itemDef)) return false;

        RemoveItem(itemDef);

        var dir = direction.sqrMagnitude < 0.0001f ? Vector2.right : direction.normalized;
        var spawnPosition = (Vector2)transform.position + dir * 0.6f;
        ItemWorldObj.SpawnThrown(spawnPosition, itemDef, 1, itemWorldObjPrefab, dir * power, gravityScale, spin);
        return true;
    }
}