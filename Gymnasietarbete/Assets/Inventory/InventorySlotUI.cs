using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public Button useButton;
    public TextMeshProUGUI useButtonText;
    public Button dropButton;
    public GameObject buttonsPanel;

    public GameObject tooltipContainer; // make sure this and its childred is not raycast target !!!
    public TextMeshProUGUI tooltipTextBox;
    private Inventory inventory;
    private ItemWorldObj itemWorldObjPrefab;
    private Transform playerTransform;
    private InventorySlot slot;

    private InventoryUI inventoryUI;

    private void Awake()
    {
        if (buttonsPanel)
            buttonsPanel.SetActive(false);
        if (tooltipContainer)
            tooltipContainer.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonsPanel)
            buttonsPanel.SetActive(true);
        if (tooltipContainer)
            tooltipContainer.SetActive(true);
        DisplayInfo(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonsPanel)
            buttonsPanel.SetActive(false);
        if (tooltipContainer)
            tooltipContainer.SetActive(false);
        DisplayInfo(false);
    }

    public void Init(InventoryUI inventoryUI, Inventory inventory, ItemWorldObj itemWorldObjPrefab, Transform playerTransform)
    {
        this.inventory = inventory;
        this.itemWorldObjPrefab = itemWorldObjPrefab;
        this.playerTransform = playerTransform;
        this.inventoryUI = inventoryUI;
    }

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;
        icon.sprite = slot.item.icon;
        amountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
        useButtonText.text = slot.item.useButtonText;
        tooltipTextBox.text = $"<b>{slot.item.Name}</b>\n\n{slot.item.tooltipDescription}";
    }

    public void DisplayInfo(bool show)
    {
        if(show)
        {
            inventoryUI.itemName.text = slot.item.Name;
            inventoryUI.itemDesctiption.text = slot.item.tooltipDescription;
            inventoryUI.itemPreviewImage.sprite = slot.item.icon;
        }
        else
        {
            inventoryUI.itemName.text = string.Empty;
            inventoryUI.itemDesctiption.text = string.Empty;
            inventoryUI.itemPreviewImage.sprite = null;
        }
    }

    public void OnUseButton()
    {
        if (playerTransform)
        {
            var player = playerTransform.gameObject;
            if (player)
            {
                slot.item.Use(player);
                if (slot.item.removeOnUse && inventory != null) inventory.RemoveItem(slot.item);
            }
        }
    }

    public void OnDropButton()
    {
        if (inventory != null && itemWorldObjPrefab != null && playerTransform != null)
        {
            // offset drop pos so item doesnt get picked up immediately and funi fling effect too
            var dropOffset = playerTransform.right * 1.5f + playerTransform.up * 0.5f;
            var dropPosition = playerTransform.position + dropOffset;
            inventory.DropItem(slot.item, dropPosition, itemWorldObjPrefab);
        }
    }
}