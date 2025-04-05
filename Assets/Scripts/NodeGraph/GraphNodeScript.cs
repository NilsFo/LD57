using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NodeGraph
{
    public class GraphNode: MonoBehaviour
    {
        [SerializeField]
        private GameState gameState;
        
        [SerializeField]
        private int id;
        
        [SerializeField]
        private List<GameObject> neighborNodes;
        
        public int ID
        {
            get => id;
            set => id = value;
        }

        public void AddNeighbor(GameObject start)
        {
            neighborNodes.Add(start);
        }

        public void AddAsPartialEdgeNode()
        {
            NodeGraph nodeGraph = gameState.GetComponent<NodeGraph>();
            nodeGraph.AddPartialNodeForEdge(gameObject);
        }
        
        private void Start()
        {
            gameState = FindFirstObjectByType<GameState>();
        }
    }
}
