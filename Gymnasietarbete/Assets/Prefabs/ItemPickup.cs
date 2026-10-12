using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private ItemWorldObj itemWorldObj;
    private Inventory playerInventory;
    private bool hasPickedUp = false;

    private void Start()
    {
        itemWorldObj = GetComponent<ItemWorldObj>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasPickedUp || !itemWorldObj.CanPickup())
            return;

        if (collision.CompareTag("Player"))
        {
            playerInventory = collision.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                var (itemDef, amount) = itemWorldObj.GetItem();
                playerInventory.AddItem(itemDef, amount);
                hasPickedUp = true;
                itemWorldObj.DestroySelf();
            }
        }
    }
}
