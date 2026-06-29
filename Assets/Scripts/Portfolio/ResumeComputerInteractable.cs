using UnityEngine;

public class ResumeComputerInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Press E to view resume";
    [SerializeField] private string resumeFileName = "Resume.pdf";
    [SerializeField] private string webFallbackUrl = "";

    public string InteractionPrompt => interactionPrompt;

    public void Interact(PlayerInteractor interactor)
    {
        OpenResume();
    }

    private void OpenResume()
    {
        PortfolioUrlOpener.OpenStreamingAsset(resumeFileName, webFallbackUrl);
    }
}
