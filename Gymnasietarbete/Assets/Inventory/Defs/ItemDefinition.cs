using UnityEngine;

public abstract class ItemDefinition : ScriptableObject
{
    public string Name;
    public Sprite icon;
    public bool isStackable = true;
    public bool removeOnUse = true; // consumeables
    public string tooltipDescription;
    public string useButtonText = "Use";

    public virtual void Use(GameObject player)
    {
    }
}