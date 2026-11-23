using UnityEngine;

[CreateAssetMenu(fileName = "NewWaterJug", menuName = "Inventory/Water Jug Item")]
public class WaterJugItemDefinition : ItemDefinition
{
    public enum JugSize
    {
        FiveLiter,
        ThreeLiter
    }

    [Header("Jug Settings")]
    public JugSize jugSize;

    [HideInInspector] public float currentFillRatio = 0f;

    public override void Use(GameObject player)
    {
        WaterJugPuzzleManager puzzleManager = FindObjectOfType<WaterJugPuzzleManager>();
        if (puzzleManager != null)
        {
            puzzleManager.UseJugFromInventory(jugSize, player);
        }
        else
        {
            Debug.LogWarning("No WaterJugPuzzleManager active in this room!");
        }
    }
}