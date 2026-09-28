using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public static PlayerInteractor Instance { get; private set; }

    [Header("References (auto-found if empty)")]
    public Inventory inventory;
    public EquipSystem equipSystem;

    [Header("Interaction (2D)")]
    public float interactRange = 1.6f;
    public Vector2 centerOffset = new Vector2(0f, 0.25f);
    public LayerMask interactMask = ~0;

    [Header("UI (auto-created if empty)")]
    public Text promptText;
    public Text messageText;

    IInteractable target;
    float messageTimer;

    public bool HasLitFire => equipSystem != null && equipSystem.HasLitFire;

    void Awake() => Instance = this;

    void Start()
    {
        if (!inventory) inventory = FindFirstObjectByType<Inventory>();
        if (!equipSystem) equipSystem = FindFirstObjectByType<EquipSystem>();
        if (!promptText || !messageText) BuildUI();
    }

    void Update()
    {
        UpdateTarget();

        bool pressed;
#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        pressed = kb != null && kb.eKey.wasPressedThisFrame;
#else
        pressed = Input.GetKeyDown(KeyCode.E);
#endif
        if (pressed && target != null && target.CanInteract(this))
            target.Interact(this);

        if (messageTimer > 0f && messageText)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f) messageText.text = "";
        }
    }

    void UpdateTarget()
    {
        target = null;
        if (promptText) promptText.text = "";

        Vector2 center = (Vector2)transform.position + centerOffset;
        var hits = Physics2D.OverlapCircleAll(center, interactRange, interactMask);

        float bestDist = float.MaxValue;
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float d = ((Vector2)hit.transform.position - center).sqrMagnitude;
            if (d < bestDist) { bestDist = d; target = interactable; }
        }

        if (target != null && promptText)
            promptText.text = "[E] " + target.InteractionPrompt;
    }

    // Shows the interaction range in the Scene view when Player is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + centerOffset, interactRange);
    }

    public void ShowMessage(string msg)
    {
        if (!messageText) return;
        messageText.text = msg;
        messageTimer = 2.5f;
    }

    void BuildUI()
    {
        var canvasGo = new GameObject("InteractCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        promptText = CreateText(canvas.transform, "PromptText", new Vector2(0.5f, 0.85f), 26, Color.white);
        messageText = CreateText(canvas.transform, "MessageText", new Vector2(0.5f, 0.4f), 22, new Color(1f, 0.9f, 0.45f));
    }

    Text CreateText(Transform parent, string textName, Vector2 anchor, int size, Color color)
    {
        var go = new GameObject(textName, typeof(Text));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(900f, 40f);

        var t = go.GetComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.fontStyle = FontStyle.Bold;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        t.text = "";
        return t;
    }
}