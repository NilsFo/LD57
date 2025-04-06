using NodeGraph;
using UnityEngine;

public class Beacon : MonoBehaviour
{

    public NodeButton myNodeButton;

    public BeaconTerminal myTerminal;
    public GraphNode myNode;
    private GameState gameState;

    public bool terminalFinishedLoading;
    public bool isInNetwork;
    public bool activeOnStart = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        if (activeOnStart)
        {
            myTerminal.StartLoading();
            JoinNetwork();
        }
    }

    // Update is called once per frame
    void Update()
    {
        myNodeButton.interactable.isInteractable = false;

        if (gameState.playerState == GameState.PLAYER_STATE.HOSE)
        {
            myNodeButton.interactable.isInteractable = true;
            myNodeButton.interactable.interactionPrompt = "Attach oxygen hose";
        }

        if (gameState.playerState == GameState.PLAYER_STATE.WALKING)
        {
            if (terminalFinishedLoading)
            {
                myNodeButton.interactable.isInteractable = true;
                myNodeButton.interactable.interactionPrompt = "Take oxygen hose";
            }
        }
    }

    public void OnPlayerInteract()
    {
        print("Beacon player interact");

        if (gameState.playerState == GameState.PLAYER_STATE.HOSE)
        {
            if (isInNetwork)
                return;
            JoinNetwork();
            gameState.playerState=GameState.PLAYER_STATE.WALKING;
            print("Attached the hose.");
            FindFirstObjectByType<NodeGraph.NodeGraph>().AddEdge(myNode.gameObject, gameState.hoseStartBeacon.myNode.gameObject);
            return;
        }

        if (gameState.playerState == GameState.PLAYER_STATE.WALKING)
        {
            gameState.playerState = GameState.PLAYER_STATE.HOSE;
            gameState.hoseStartBeacon=this;
            print("Picked up a hose.");
            return;
        }
    }

    public void OnTerminalFinishedLoading()
    {
        print("Beacon terminal finished loading");
        terminalFinishedLoading = true;
    }

    public void JoinNetwork()
    {
        print("Beacon requested to join network");
        if (!isInNetwork)
        {
            isInNetwork = true;
            OnNetworkJoin();
        }
    }

    private void OnNetworkJoin()
    {
        print("Beacon joined network");
        isInNetwork = true;
        myTerminal.StartLoading();
    }
}
