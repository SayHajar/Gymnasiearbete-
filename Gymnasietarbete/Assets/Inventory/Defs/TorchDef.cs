using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "TorchDef", menuName = "Scriptable Objects/TorchDef")]
public class TorchDef : ItemDefinition
{
    override public void Use(GameObject player)
    {
        if(player == null)
        {
            Debug.LogError("Player is null in TorchDef.Use");
            return;
        }
        
        Light2D light = player.GetComponentInChildren<Light2D>();

        if (light == null)
        {
            Debug.LogError("No Light2D component found in children of player");
            return;
        }

        Debug.Log("Torch used by player");
        light.enabled = !light.enabled;
    }
}
