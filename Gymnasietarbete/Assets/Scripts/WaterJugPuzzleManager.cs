using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterJugPuzzleManager : MonoBehaviour
{
    [Header("Station Trigger Zones (2D Colliders)")]
    public Collider2D fountainZone;
    public Collider2D drainZone;
    public Collider2D scaleZone;

    [Header("Inventory Items")]
    public WaterJugItemDefinition jug5ItemDef;
    public WaterJugItemDefinition jug3ItemDef;

    [Header("Scale & Exit Door")]
    public Transform scalePlate;
    public GameObject exitDoor;
    public float dropPerLiter = 0.2f;

    [Header("Optional Visual Water Fills & Status Text")]
    public Image waterFill5L;
    public Image waterFill3L;
    public TextMeshProUGUI statusText;

    private int jug5 = 0;
    private int jug3 = 0;
    private bool jug5OnScale = false;
    private bool jug3OnScale = false;
    private bool solved = false;
    private float scaleStartY;

    private void Start()
    {
        if (scalePlate != null) scaleStartY = scalePlate.position.y;
        UpdateItemDescriptions();
        SetStatus("Simon says: 'Use your jugs to place EXACTLY 4L on the scale!'");
    }

    private void Update()
    {
        if (waterFill5L != null)
            waterFill5L.fillAmount = Mathf.Lerp(waterFill5L.fillAmount, jug5 / 5f, Time.deltaTime * 10f);

        if (waterFill3L != null)
            waterFill3L.fillAmount = Mathf.Lerp(waterFill3L.fillAmount, jug3 / 3f, Time.deltaTime * 10f);
    }

    public void UseJugFromInventory(WaterJugItemDefinition.JugSize size, GameObject player)
    {
        if (solved)
        {
            SetStatus("Puzzle is already solved!");
            return;
        }

        Collider2D[] playerCols = player.GetComponentsInChildren<Collider2D>();
        if (playerCols == null || playerCols.Length == 0)
        {
            SetStatus($"ERROR: No Collider2D found on {player.name}!");
            return;
        }

        if (fountainZone == null)
        {
            SetStatus("ERROR: fountainZone is NULL in WaterJugPuzzleManager!");
            return;
        }

        bool atFountain = IsTouchingZone(playerCols, fountainZone);
        bool atDrain = IsTouchingZone(playerCols, drainZone);
        bool atScale = IsTouchingZone(playerCols, scaleZone);

        bool is5L = (size == WaterJugItemDefinition.JugSize.FiveLiter);

        if (atFountain)
        {
            if (is5L)
            {
                if (jug5OnScale) SetStatus("Take the 5L jug off the scale first!");
                else if (jug5 == 5) SetStatus("The 5L jug is already full!");
                else
                {
                    jug5 = 5;
                    SetStatus($"Filled 5L Jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
                }
            }
            else
            {
                if (jug3OnScale) SetStatus("Take the 3L jug off the scale first!");
                else if (jug3 == 3) SetStatus("The 3L jug is already full!");
                else
                {
                    jug3 = 3;
                    SetStatus($"Filled 3L Jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
                }
            }
        }
        else if (atDrain)
        {
            if (is5L)
            {
                if (jug5OnScale) SetStatus("Take the 5L jug off the scale first!");
                else if (jug5 == 0) SetStatus("The 5L jug is already empty!");
                else
                {
                    jug5 = 0;
                    SetStatus($"Emptied 5L Jug. (5L: {jug5}/5 | 3L: {jug3}/3)");
                }
            }
            else
            {
                if (jug3OnScale) SetStatus("Take the 3L jug off the scale first!");
                else if (jug3 == 0) SetStatus("The 3L jug is already empty!");
                else
                {
                    jug3 = 0;
                    SetStatus($"Emptied 3L Jug. (5L: {jug5}/5 | 3L: {jug3}/3)");
                }
            }
        }
        else if (atScale)
        {
            if (is5L) jug5OnScale = !jug5OnScale;
            else jug3OnScale = !jug3OnScale;

            CheckScale();
        }
        else
        {
            // Shows distance to fountain on-screen so you immediately see if Z-depth or position is off
            float dist = Vector2.Distance(player.transform.position, fountainZone.bounds.center);
            SetStatus($"Not at a station (Dist to fountain: {dist:F1}m) -> Trying to pour...");
            TryPourBetweenJugs(is5L);
        }

        UpdateItemDescriptions();
    }

    private bool IsTouchingZone(Collider2D[] playerCols, Collider2D zone)
    {
        if (zone == null) return false;

        foreach (var col in playerCols)
        {
            if (col == null || !col.enabled) continue;

            // Ignore Z-axis differences by comparing 2D min/max bounds directly
            Bounds a = col.bounds;
            Bounds b = zone.bounds;
            bool overlapX = a.min.x <= b.max.x && a.max.x >= b.min.x;
            bool overlapY = a.min.y <= b.max.y && a.max.y >= b.min.y;

            if (overlapX && overlapY) return true;
        }
        return false;
    }

    private void TryPourBetweenJugs(bool from5To3)
    {
        if (jug5OnScale || jug3OnScale)
        {
            SetStatus("Can't pour! Take both jugs off the scale first.");
            return;
        }

        if (from5To3)
        {
            if (jug5 == 0)
            {
                SetStatus("Can't pour! Your 5L jug is empty.");
                return;
            }
            if (jug3 == 3)
            {
                SetStatus("Can't pour! The 3L jug is already full.");
                return;
            }

            int move = Mathf.Min(jug5, 3 - jug3);
            jug5 -= move;
            jug3 += move;
            SetStatus($"Poured {move}L into the 3L jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
        }
        else
        {
            if (jug3 == 0)
            {
                SetStatus("Can't pour! Your 3L jug is empty.");
                return;
            }
            if (jug5 == 5)
            {
                SetStatus("Can't pour! The 5L jug is already full.");
                return;
            }

            int move = Mathf.Min(jug3, 5 - jug5);
            jug3 -= move;
            jug5 += move;
            SetStatus($"Poured {move}L into the 5L jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
        }
    }

    private void CheckScale()
    {
        int totalWeight = (jug5OnScale ? jug5 : 0) + (jug3OnScale ? jug3 : 0);

        if (scalePlate != null)
        {
            Vector3 pos = scalePlate.position;
            pos.y = scaleStartY - (totalWeight * dropPerLiter);
            scalePlate.position = pos;
        }

        if (totalWeight == 4)
        {
            solved = true;
            SetStatus("CLICK! Exactly 4 Liters! Simon says: 'Impressive... the door is open.'");
            if (exitDoor != null) exitDoor.SetActive(false);
        }
        else
        {
            SetStatus($"Scale reads {totalWeight}L. (Needs 4L)");
        }
    }

    private void UpdateItemDescriptions()
    {
        if (jug5ItemDef != null)
        {
            jug5ItemDef.currentFillRatio = jug5 / 5f;
            jug5ItemDef.tooltipDescription = $"Ancient 5-Liter clay jug.\nCurrently holding: <b>{jug5} / 5 Liters</b>{(jug5OnScale ? "\n(Currently on Scale)" : "")}";
        }

        if (jug3ItemDef != null)
        {
            jug3ItemDef.currentFillRatio = jug3 / 3f;
            jug3ItemDef.tooltipDescription = $"Ancient 3-Liter clay jug.\nCurrently holding: <b>{jug3} / 3 Liters</b>{(jug3OnScale ? "\n(Currently on Scale)" : "")}";
        }

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.Refresh();
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log(msg);
    }
}