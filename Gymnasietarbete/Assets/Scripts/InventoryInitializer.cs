using UnityEngine;

public class InventoryInitializer : MonoBehaviour
{
    public InventoryUI inventoryUI;

    private void Start()
    {
        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
        }

        if (inventoryUI != null)
        {
            inventoryUI.SetPlayerTransform(transform);
        }
    }
}
