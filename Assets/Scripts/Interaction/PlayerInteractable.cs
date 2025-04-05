using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractable : MonoBehaviour
{
    [Header("Properties")] public bool isInteractable = true;

    [Header("UI Prompt")] public string interactionPrompt = "";
    public bool HasInteractionPrompt => !string.IsNullOrEmpty(interactionPrompt);

    [Header("Events")] public UnityEvent onPlayerInteract;

    private void Start()
    {
        if (onPlayerInteract == null)
        {
            onPlayerInteract = new UnityEvent();
        }
    }

    private void Awake()
    {
    }

    public void Interact()
    {
        if (isInteractable)
            onPlayerInteract.Invoke();
    }
}