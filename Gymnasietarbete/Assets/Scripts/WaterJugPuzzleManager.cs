using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterJugPuzzleManager : MonoBehaviour
{
    [Header("Station Trigger Zones (2D Colliders)")]
    public Collider2D fountainZone;
    public Collider2D pourZone;
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
        SetStatus("Simon says: 'Use your jugs at the stations to place EXACTLY 4L on the scale!'");
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
        if (solved) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return;

        bool is5L = (size == WaterJugItemDefinition.JugSize.FiveLiter);

        if (fountainZone != null && playerCol.IsTouching(fountainZone))
        {
            if (is5L && !jug5OnScale)
            {
                jug5 = 5;
                SetStatus($"Filled 5L Jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
            else if (!is5L && !jug3OnScale)
            {
                jug3 = 3;
                SetStatus($"Filled 3L Jug! (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
        }
        else if (pourZone != null && playerCol.IsTouching(pourZone))
        {
            if (jug5OnScale || jug3OnScale)
            {
                SetStatus("Take both jugs off the scale before pouring!");
                return;
            }

            if (is5L)
            {
                int move = Mathf.Min(jug5, 3 - jug3);
                jug5 -= move;
                jug3 += move;
                SetStatus($"Poured 5L into 3L! (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
            else
            {
                int move = Mathf.Min(jug3, 5 - jug5);
                jug3 -= move;
                jug5 += move;
                SetStatus($"Poured 3L into 5L! (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
        }
        else if (drainZone != null && playerCol.IsTouching(drainZone))
        {
            if (is5L && !jug5OnScale)
            {
                jug5 = 0;
                SetStatus($"Emptied 5L Jug. (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
            else if (!is5L && !jug3OnScale)
            {
                jug3 = 0;
                SetStatus($"Emptied 3L Jug. (5L: {jug5}/5 | 3L: {jug3}/3)");
            }
        }
        else if (scaleZone != null && playerCol.IsTouching(scaleZone))
        {
            if (is5L) jug5OnScale = !jug5OnScale;
            else jug3OnScale = !jug3OnScale;

            CheckScale();
        }
        else
        {
            SetStatus($"You examine the {(is5L ? $"5L Jug ({jug5}/5L)" : $"3L Jug ({jug3}/3L)")}. Walk up to a station to use it!");
        }

        UpdateItemDescriptions();
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