using UnityEngine;

public class Equippable : MonoBehaviour
{
    public ItemData item;                 // filled by EquipSystem
    [HideInInspector] public int uiIndex = -1;

    public FireVisuals fire;              // the flame + light on this item

    public bool IsLit => fire != null && fire.IsLit;

    public void SetLit(bool lit)
    {
        if (fire != null) fire.SetLit(lit);
    }
}