using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public ItemData item;
    [Min(1)] public int amount = 1;

    public string InteractionPrompt =>
        item == null ? "?" : "Pick up " + item.displayName + (amount > 1 ? " x" + amount : "");

    public bool CanInteract(PlayerInteractor player) => item != null;

    public void Interact(PlayerInteractor player)
    {
        if (player.inventory == null) return;

        if (player.inventory.AddItem(item, amount))
        {
            player.ShowMessage("+ " + item.displayName + (amount > 1 ? " x" + amount : ""));
            Destroy(gameObject);
        }
        else
        {
            player.ShowMessage("Inventory is full!");
        }
    }

    // Optional helper for later (e.g. dropping items)
    public static void Spawn(ItemData item, Vector3 position, int amount = 1)
    {
        if (item == null || item.pickupPrefab == null)
        {
            Debug.LogWarning("PickupItem.Spawn: item or pickupPrefab missing.");
            return;
        }
        var go = Instantiate(item.pickupPrefab, position, Quaternion.identity);
        var pickup = go.GetComponent<PickupItem>();
        if (pickup == null) pickup = go.AddComponent<PickupItem>();
        pickup.item = item;
        pickup.amount = amount;
    }
}