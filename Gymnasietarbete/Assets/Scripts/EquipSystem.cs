using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    public Inventory inventory;

    [Header("Hand (auto-created if empty)")]
    public Transform handSocket;
    public Vector2 handOffset = new Vector2(0.45f, 0.1f);

    public Equippable Current => current;
    Equippable current;

    Rigidbody2D rb;
    float facing = 1f;   // +1 = right, -1 = left (world)

    public bool HasLitFire =>
        current != null && current.item != null && current.item.isFire && current.IsLit;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        if (!inventory) inventory = GetComponent<Inventory>();
        if (!inventory) inventory = FindFirstObjectByType<Inventory>();

        if (!handSocket)
        {
            handSocket = new GameObject("Hand").transform;
            handSocket.SetParent(transform, false);
        }
        UpdateHandSide();
    }

    void Update()
    {
        // Read facing from horizontal velocity — works no matter how
        // your PlayerMovement script flips the sprite
        if (rb != null)
        {
            float vx = rb.linearVelocity.x;   // Unity 6 API (older Unity: rb.velocity.x)
            if (vx > 0.05f) facing = 1f;
            else if (vx < -0.05f) facing = -1f;
        }
        UpdateHandSide();

#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null)
        {
            if (kb.digit1Key.wasPressedThisFrame) ToggleEquip(0);
            else if (kb.digit2Key.wasPressedThisFrame) ToggleEquip(1);
            else if (kb.digit3Key.wasPressedThisFrame) ToggleEquip(2);
            else if (kb.digit4Key.wasPressedThisFrame) ToggleEquip(3);
            else if (kb.digit5Key.wasPressedThisFrame) ToggleEquip(4);
            else if (kb.digit6Key.wasPressedThisFrame) ToggleEquip(5);
            else if (kb.digit7Key.wasPressedThisFrame) ToggleEquip(6);
            else if (kb.digit8Key.wasPressedThisFrame) ToggleEquip(7);
            else if (kb.qKey.wasPressedThisFrame) Unequip();
        }
#else
        for (int i = 0; i < 8; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) ToggleEquip(i);
        if (Input.GetKeyDown(KeyCode.Q)) Unequip();
#endif
    }

    // Puts the torch on the facing side. Also compensates if your
    // movement script flips the player via localScale.
    void UpdateHandSide()
    {
        if (handSocket == null) return;
        float rootSign = transform.lossyScale.x < 0f ? -1f : 1f;
        handSocket.localPosition = new Vector3(handOffset.x * facing * rootSign, handOffset.y, 0f);
        handSocket.localScale = new Vector3(facing * rootSign, 1f, 1f);
    }

    public void ToggleEquip(int uiIndex)
    {
        if (inventory == null || handSocket == null) return;

        var slot = inventory.GetSlot(uiIndex);
        if (slot == null || slot.item == null) return;

        // pressing the same slot again = unequip
        if (current != null && current.uiIndex == uiIndex) { Unequip(); return; }

        if (!slot.item.equippable || slot.item.equipPrefab == null)
        {
            PlayerInteractor.Instance?.ShowMessage(slot.item.displayName + " can't be equipped.");
            return;
        }

        Unequip();

        var go = Instantiate(slot.item.equipPrefab, handSocket);
        go.transform.localPosition = Vector3.zero;
        current = go.GetComponent<Equippable>();
        if (current == null) current = go.AddComponent<Equippable>();

        current.item = slot.item;
        current.uiIndex = uiIndex;
        inventory.SetSelected(uiIndex);

        PlayerInteractor.Instance?.ShowMessage("Equipped " + slot.item.displayName + ".");
    }

    public void Unequip()
    {
        if (current != null) { Destroy(current.gameObject); current = null; }
        if (inventory != null && inventory.selectedIndex != -1)
            inventory.SetSelected(-1);
    }

    // FireSource calls this: light my unlit torch from a burning fire
    public bool TryLightEquipped()
    {
        if (current != null && current.item != null && current.item.isFire && !current.IsLit)
        {
            current.SetLit(true);
            return true;
        }
        return false;
    }
}