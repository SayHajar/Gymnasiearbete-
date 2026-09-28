public interface IInteractable
{
    string InteractionPrompt { get; }
    bool CanInteract(PlayerInteractor player);
    void Interact(PlayerInteractor player);
}