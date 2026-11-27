using UnityEngine;

/// <summary>
/// Siktar och kastar en sten. Läggs på spelaren automatiskt av Stone.Use()
/// första gången man klickar "Use"/"Throw" på en sten i inventoryt.
/// </summary>
public class StoneThrower : MonoBehaviour
{
    public static bool IsAiming { get; private set; }

    private Stone stone;
    private Inventory inventory;
    private Camera cam;
    private LineRenderer preview;
    private float charge01;
    private int beginFrame;

    public void Begin(Stone stone, Inventory inventory)
    {
        if (stone == null || inventory == null) return;

        this.stone = stone;
        this.inventory = inventory;
        cam = Camera.main;

        charge01 = 0f;
        beginFrame = Time.frameCount;
        IsAiming = true;

        // Stäng inventoryt så att musen kan användas för att sikta
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.CloseUI();

        EnsurePreview();
        DrawPreview(AimDirection());
    }

    private void Update()
    {
        if (!IsAiming || stone == null) return;

        // Ignorera klicket som startade kastet (samma frame som "Use"-knappen)
        if (Time.frameCount == beginFrame) return;

        if (Input.GetMouseButton(0))
            charge01 = Mathf.Min(charge01 + Time.deltaTime / Mathf.Max(0.01f, stone.chargeTime), 1f);

        var dir = AimDirection();
        DrawPreview(dir);

        if (Input.GetMouseButtonUp(0))
        {
            var power = Mathf.Lerp(stone.minPower, stone.maxPower, charge01);
            inventory.ThrowItem(stone, dir, power, stone.worldPrefab, stone.gravityScale, stone.spin);
            End();
            return;
        }

        // Avbryt: stenen ligger kvar i inventoryt
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            End();
    }

    private Vector2 AimDirection()
    {
        if (cam == null) return Vector2.right;

        var mouseWorld = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        var d = mouseWorld - (Vector2)transform.position;
        return d.sqrMagnitude < 0.0001f ? Vector2.right : d.normalized;
    }

    private void EnsurePreview()
    {
        if (preview != null) return;

        var go = new GameObject("StoneThrowPreview");
        preview = go.AddComponent<LineRenderer>();
        preview.useWorldSpace = true;
        preview.positionCount = 0;
        preview.startWidth = 0.09f;
        preview.endWidth = 0.03f;
        preview.numCapVertices = 2;
        preview.startColor = new Color(1f, 1f, 1f, 0.85f);
        preview.endColor = new Color(1f, 1f, 1f, 0.05f);
        preview.sortingOrder = 100;

        var shader = Shader.Find("Sprites/Default");
        if (shader != null)
            preview.material = new Material(shader);
    }

    private void DrawPreview(Vector2 dir)
    {
        if (preview == null || stone == null) return;

        var power = Mathf.Lerp(stone.minPower, stone.maxPower, charge01);
        var origin = (Vector2)transform.position + dir * 0.5f;
        var v0 = dir * power;
        var g = Physics2D.gravity * stone.gravityScale;

        const int steps = 26;
        const float step = 0.055f;

        preview.positionCount = steps;
        for (var i = 0; i < steps; i++)
        {
            var t = i * step;
            preview.SetPosition(i, origin + v0 * t + 0.5f * g * t * t);
        }
    }

    private void End()
    {
        IsAiming = false;
        stone = null;
        charge01 = 0f;
        if (preview != null) preview.positionCount = 0;
    }

    private void OnDisable()
    {
        if (IsAiming) End();
    }

    private void OnDestroy()
    {
        if (IsAiming) End();
    }
}
