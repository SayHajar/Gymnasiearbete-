using UnityEngine;

public class FireSource : MonoBehaviour, IInteractable
{
    public string displayName = "Campfire";
    public FireVisuals fire;

    public bool IsLit => fire != null && fire.IsLit;

    public string InteractionPrompt => IsLit ? displayName + " (lit)" : "Light " + displayName;

    public bool CanInteract(PlayerInteractor player) => fire != null;

    public void Interact(PlayerInteractor player)
    {
        if (player == null || fire == null) return;

        // Burning fire -> let the player light their own torch from it
        if (IsLit)
        {
            if (player.equipSystem != null && player.equipSystem.TryLightEquipped())
                player.ShowMessage("You light your torch from the " + displayName + ".");
            else
                player.ShowMessage("The " + displayName + " is already burning.");
            return;
        }

        // Unlit -> the player needs a lit fire item in hand
        if (player.HasLitFire)
        {
            fire.SetLit(true);
            player.ShowMessage("You light the " + displayName + ".");
            // Hook game events here: unlock a door, trigger an objective...
        }
        else
        {
            player.ShowMessage("You need a lit torch for that.");
        }
    }
}