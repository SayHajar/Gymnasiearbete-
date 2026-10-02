using UnityEngine;

[CreateAssetMenu(fileName = "Torch", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName = "Item";
    public Sprite icon;                 // hotbar icon (optional)
    [TextArea] public string description;

    [Header("Pickup")]
    public GameObject pickupPrefab;     // what represents it on the ground / drops

    [Header("Equipping")]
    public bool equippable = false;
    public GameObject equipPrefab;      // spawned in the hand (torch with fire + light)

    [Header("Flags")]
    public bool isFire = false;         // can light FireSources / Burnables (a torch)
}