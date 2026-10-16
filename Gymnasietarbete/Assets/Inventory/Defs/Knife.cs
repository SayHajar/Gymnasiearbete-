using UnityEngine;

[CreateAssetMenu(fileName = "Knife", menuName = "Scriptable Objects/Knife")]
public class Knife : ItemDefinition
{
    public override void Use(GameObject player)
    {
        Debug.Log("knife was used");    
    }
    
}
