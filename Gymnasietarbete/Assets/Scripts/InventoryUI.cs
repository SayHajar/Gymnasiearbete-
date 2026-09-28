using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Font font;

    readonly List<Image> slotImages = new List<Image>();
    readonly List<Image> icons = new List<Image>();
    readonly List<Text> labels = new List<Text>();
    readonly List<Text> counts = new List<Text>();

    readonly Color normalColor = new Color(0f, 0f, 0f, 0.55f);
    readonly Color selectedColor = new Color(1f, 0.75f, 0.2f, 0.85f);

    void Start()
    {
        if (!inventory) inventory = FindFirstObjectByType<Inventory>();
        if (!inventory) { enabled = false; return; }
        if (!font) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Build();
        inventory.OnChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    void Build()
    {
        var canvasGo = new GameObject("InventoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
        }

        var panel = new GameObject("Hotbar", typeof(Image), typeof(HorizontalLayoutGroup)).GetComponent<RectTransform>();
        panel.SetParent(canvas.transform, false);
        panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0f);
        panel.pivot = new Vector2(0.5f, 0f);
        panel.anchoredPosition = new Vector2(0f, 15f);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.2f);

        var layout = panel.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 6f;
        layout.padding = new RectOffset(6, 6, 6, 6);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        for (int i = 0; i < inventory.capacity; i++)
        {
            int index = i;

            var slot = new GameObject("Slot_" + i, typeof(Image), typeof(Button)).GetComponent<RectTransform>();
            slot.SetParent(panel, false);
            slot.GetComponent<Image>().color = normalColor;
            var le = slot.gameObject.AddComponent<LayoutElement>();
            le.preferredWidth = 58f; le.preferredHeight = 58f;

            var iconGo = new GameObject("Icon", typeof(Image));
            var iconRt = iconGo.GetComponent<RectTransform>();
            iconRt.SetParent(slot, false);
            iconRt.anchorMin = Vector2.zero; iconRt.anchorMax = Vector2.one;
            iconRt.offsetMin = new Vector2(10f, 14f); iconRt.offsetMax = new Vector2(-10f, -6f);
            var icon = iconGo.GetComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.enabled = false;

            var labelGo = new GameObject("Label", typeof(Text));
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.SetParent(slot, false);
            labelRt.anchorMin = labelRt.anchorMax = new Vector2(0.5f, 0.5f);
            labelRt.sizeDelta = new Vector2(56f, 30f);
            var label = labelGo.GetComponent<Text>();
            label.font = font; label.fontSize = 12;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white; label.raycastTarget = false; label.text = "";

            var countGo = new GameObject("Count", typeof(Text));
            var countRt = countGo.GetComponent<RectTransform>();
            countRt.SetParent(slot, false);
            countRt.anchorMin = countRt.anchorMax = new Vector2(1f, 0f);
            countRt.pivot = new Vector2(1f, 0f);
            countRt.anchoredPosition = new Vector2(-3f, 2f);
            countRt.sizeDelta = new Vector2(30f, 20f);
            var count = countGo.GetComponent<Text>();
            count.font = font; count.fontSize = 14; count.fontStyle = FontStyle.Bold;
            count.alignment = TextAnchor.LowerRight;
            count.color = Color.white; count.raycastTarget = false; count.text = "";

            slot.GetComponent<Button>().onClick.AddListener(() =>
                FindFirstObjectByType<EquipSystem>()?.ToggleEquip(index));

            slotImages.Add(slot.GetComponent<Image>());
            icons.Add(icon); labels.Add(label); counts.Add(count);
        }
    }

    void Refresh()
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            var slot = inventory.GetSlot(i);
            slotImages[i].color = (inventory.selectedIndex == i) ? selectedColor : normalColor;

            if (slot != null && slot.item != null)
            {
                bool hasIcon = slot.item.icon != null;
                icons[i].enabled = hasIcon;
                icons[i].sprite = slot.item.icon;
                labels[i].text = hasIcon ? "" : slot.item.displayName;
                counts[i].text = slot.count > 1 ? "x" + slot.count : "";
            }
            else
            {
                icons[i].enabled = false;
                labels[i].text = "";
                counts[i].text = "";
            }
        }
    }
}