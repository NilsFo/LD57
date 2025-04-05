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
        
        private int _id;
        
        [SerializeField]
        private List<GameObject> neighborNodes;
        
        public int ID
        {
            get => _id;
            set => _id = value;
        }

        public void AddNeighbor(GameObject start)
        {
            neighborNodes.Add(start);
        }

        public void AddAsPartialEdgeNode()
        {
            GraphScript graph = gameState.GetComponent<GraphScript>();
            graph.AddPartialNodeForEdge(gameObject);
        }
        
        private void Start()
        {
            gameState = FindFirstObjectByType<GameState>();
        }
    }
}
