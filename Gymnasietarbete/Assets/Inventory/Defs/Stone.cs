using UnityEngine;

[CreateAssetMenu(fileName = "Stone", menuName = "Scriptable Objects/Stone")]
public class Stone : ItemDefinition
{
    [Header("Throw Settings")]
    public ItemWorldObj worldPrefab;   // ItemWorldObj.prefab
    public float minPower = 6f;
    public float maxPower = 18f;
    public float chargeTime = 0.7f;    // sekunder för full kraft
    public float gravityScale = 1.6f;  // hur tung bågen känns
    public float spin = 540f;          // rotation i luften

    public override void Use(GameObject player)
    {
        if (player == null)
        {
            Debug.LogWarning("[Stone] Use anropades utan spelare");
            return;
        }

        if (worldPrefab == null)
        {
            Debug.LogWarning("[Stone] worldPrefab är inte satt på Stone-asseten");
            return;
        }

        var inventory = player.GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("[Stone] Spelaren saknar Inventory-komponent");
            return;
        }

        var thrower = player.GetComponent<StoneThrower>();
        if (thrower == null)
            thrower = player.AddComponent<StoneThrower>();

        thrower.Begin(this, inventory);
    }
}
