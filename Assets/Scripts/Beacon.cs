using NodeGraph;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public NodeButton myNodeButton;

    public BeaconTerminal myTerminal;
    public GraphNode myNode;
    private GameState gameState;
    private MusicManager musicManager;
    public AudioClip hosePickUpClip;

    public bool terminalFinishedLoading;
    public bool isInNetwork;
    public bool activeOnStart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        musicManager = FindFirstObjectByType<MusicManager>();
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

        if (gameState.playerState == GameState.PLAYER_STATE.WALKING)
        {
            if (gameState.isCarryingHose)
            {
                if (gameState.hoseStartBeacon != this)
                {
                    myNodeButton.interactable.isInteractable = true;
                    myNodeButton.interactable.interactionPrompt = "Attach oxygen hose";
                }
            }
            else
            {
                if (terminalFinishedLoading)
                {
                    myNodeButton.interactable.isInteractable = true;
                    myNodeButton.interactable.interactionPrompt = "Take oxygen hose";
                }
            }
        }
    }

    public void OnPlayerInteract()
    {
        print("Beacon player interact");
        if (gameState.playerState == GameState.PLAYER_STATE.WALKING)
        {
            if (gameState.isCarryingHose)
            {
                if (isInNetwork)
                    return;
                JoinNetwork();
                musicManager.CreateAudioClip(hosePickUpClip, transform.position);
                gameState.isCarryingHose = false;
                print("Attached the hose.");
                FindFirstObjectByType<NodeGraph.NodeGraph>()
                    .AddEdge(myNode.gameObject, gameState.hoseStartBeacon.myNode.gameObject);
                return;
            }

            if (!gameState.isCarryingHose)
            {
                gameState.isCarryingHose = true;
                gameState.hoseStartBeacon = this;
                musicManager.CreateAudioClip(hosePickUpClip, transform.position);
                print("Picked up a hose.");
                return;
            }
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