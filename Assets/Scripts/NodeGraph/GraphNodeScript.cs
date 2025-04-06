using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace NodeGraph
{
    public class GraphNode : MonoBehaviour
    {
        private GameState gameState;

        [SerializeField] private int id;

        [SerializeField] private List<GameObject> neighborNodes;

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
            print("Adding node as partial edge node");
            NodeGraph nodeGraph = gameState.GetComponent<NodeGraph>();
            nodeGraph.AddPartialNodeForEdge(gameObject);
        }

        private void Awake()
        {
            gameState = FindFirstObjectByType<GameState>();
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            gameState = FindFirstObjectByType<GameState>();
            NodeGraph nodeGraph = gameState.GetComponent<NodeGraph>();
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, nodeGraph.GetLeashDistance());
        }

#endif
    }
}