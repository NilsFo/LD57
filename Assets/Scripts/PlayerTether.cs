using System;
using UnityEngine;

public class PlayerTether : MonoBehaviour
{
    public Transform ropeStart, ropeEnd;
    private GameState gameState;
    private NodeGraph.NodeGraph nodeGraph;

    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        nodeGraph = gameState.GetComponent<NodeGraph.NodeGraph>();
    }

    // Update is called once per frame
    void Update()
    {
        ropeStart.transform.position = nodeGraph.FindNearestPoint(transform.position);
    }
}