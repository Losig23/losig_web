using System;
using System.IO;
using UnityEngine;

public class ResumeComputerInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Press E to view resume";
    [SerializeField] private string resumeFileName = "Resume.pdf";

    public string InteractionPrompt => interactionPrompt;

    public void Interact(PlayerInteractor interactor)
    {
        OpenResume();
    }

    private void OpenResume()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string url = $"{Application.streamingAssetsPath}/{Uri.EscapeDataString(resumeFileName)}";
        Application.OpenURL(url);
#else
        string resumePath = Path.Combine(Application.streamingAssetsPath, resumeFileName);

        if (!File.Exists(resumePath))
        {
            Debug.LogWarning($"Resume PDF was not found at: {resumePath}");
            return;
        }

        Application.OpenURL(new Uri(resumePath).AbsoluteUri);
#endif
    }
}
