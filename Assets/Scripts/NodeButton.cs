using UnityEngine;

public class NodeButton : MonoBehaviour
{
    private GameState gameState;
    public PlayerInteractable interactable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
    }
}