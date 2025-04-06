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

    // Update is called once per frame
    void Update()
    {
        if(gameState.playerState==GameState.PLAYER_STATE.HOSE){
            interactable.interactionPrompt="Attach oxygen hose";
        }else{
            interactable.interactionPrompt="Take oxygen hose";
        }
    }
}
