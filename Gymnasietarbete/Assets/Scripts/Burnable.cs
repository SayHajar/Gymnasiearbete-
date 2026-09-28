using UnityEngine;

public class Burnable : MonoBehaviour, IInteractable
{
    public string prompt = "Burn";
    public GameObject objectToBurn;

    public string InteractionPrompt => prompt;
    public bool CanInteract(PlayerInteractor player) => objectToBurn != null;

    public void Interact(PlayerInteractor player)
    {
        if (!player.HasLitFire)
        {
            player.ShowMessage("You need a lit torch for that.");
            return;
        }
        player.ShowMessage("It burns away.");
        if (objectToBurn != null) Destroy(objectToBurn);
        // or: play particles, disable a collider, open a door...
    }
}