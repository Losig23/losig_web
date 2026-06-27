using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("UI")]
    [SerializeField] private InteractionPromptUI promptUI;

    private IInteractable currentInteractable;

    private void Reset()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        UpdateCurrentInteractable();

        if (currentInteractable != null && WasInteractPressed())
        {
            currentInteractable.Interact(this);
        }
    }

    private void UpdateCurrentInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null)
        {
            promptUI?.Hide();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayers, QueryTriggerInteraction.Collide))
        {
            currentInteractable = FindInteractable(hit.collider);
        }

        if (currentInteractable != null)
        {
            promptUI?.Show(currentInteractable.InteractionPrompt);
        }
        else
        {
            promptUI?.Hide();
        }
    }

    private static IInteractable FindInteractable(Collider sourceCollider)
    {
        MonoBehaviour[] behaviours = sourceCollider.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
            {
                return interactable;
            }
        }

        return null;
    }

    private static bool WasInteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            return Keyboard.current.eKey.wasPressedThisFrame;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.E);
#else
        return false;
#endif
    }
}
