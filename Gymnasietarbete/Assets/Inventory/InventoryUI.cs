using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

{
    public Image itemPreviewImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesctiption;

    public Inventory inventory;
    public Transform slotContainer;
    public GameObject slotPrefab;
    public ItemWorldObj itemWorldObjPrefab;
    private Transform playerTransform; // this is set programmatically !!!

    private void Start()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
            Refresh();
        }
    }

    private void OnDisable()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void SetPlayerTransform(Transform t)
    {
        playerTransform = t;
        if (inventory != null)
            Refresh();
    }

    public void SetInventory(Inventory inv)
    {
        if (inv == null)
        {
            Debug.LogWarning("[InventoryUI] SetInventory called with null");
            return;
        }

        // If the same inventory instance  just Refresh to rebuild UI for new scene
        if (inventory == inv)
        {
            Refresh();
            return;
        }

        // unsubscribe from old inventory if there was one
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;

        inventory = inv;
        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (slotContainer == null) return;

        for (var i = slotContainer.childCount - 1; i >= 0; i--)
        {
            var child = slotContainer.GetChild(i);
            if (child != null)
                Destroy(child.gameObject);
        }

        if (inventory == null) return;

        foreach (var slot in inventory.slots)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            var slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.Init(this, inventory, itemWorldObjPrefab, playerTransform);
            slotUI.SetSlot(slot);
            slotUI.useButton.onClick.AddListener(slotUI.OnUseButton);
            slotUI.dropButton.onClick.AddListener(slotUI.OnDropButton);
        }
    }
}