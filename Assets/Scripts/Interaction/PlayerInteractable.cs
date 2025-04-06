using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractable : MonoBehaviour
{

    private GameState gameState;

    [Header("Properties")] public bool isInteractable = true;

    [Header("UI Prompt")] public string interactionPrompt = "";
    public bool HasInteractionPrompt => !string.IsNullOrEmpty(interactionPrompt);

    [Header("Events")] public UnityEvent onPlayerInteract;

    private void Start()
    {
        gameState = FindFirstObjectByType<GameState>();

        if (onPlayerInteract == null)
        {
            onPlayerInteract = new UnityEvent();
        }
    }

    void Update()
    {
        //isInteractable = gameState.playerState == GameState.PLAYER_STATE.WALKING ||
        //gameState.playerState == GameState.PLAYER_STATE.HOSE;
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