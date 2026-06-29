using UnityEngine;

public class PortfolioLinkInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Press E to open link";
    [SerializeField] private string url = "";

    public string InteractionPrompt => interactionPrompt;

    public void Interact(PlayerInteractor interactor)
    {
        PortfolioUrlOpener.OpenUrl(url);
    }
}
