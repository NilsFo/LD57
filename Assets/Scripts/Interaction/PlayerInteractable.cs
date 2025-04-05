using Tumble2.Character;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractable : MonoBehaviour
{
    [Header("Properties")] public NetworkVariable<bool> isInteractable = new NetworkVariable<bool>(true);
    public bool interactableWhileCarryingParcel = false;

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
        onPlayerInteract.Invoke();
    }
}