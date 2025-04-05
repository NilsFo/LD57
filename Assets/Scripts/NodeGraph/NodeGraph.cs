using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class NodeGraph : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefabEdges;

        [SerializeField] 
        private List<GameObject> nodes;
        
        [SerializeField]
        private List<GraphEdge> edges;

        [SerializeField] 
        private GameObject partialNodeForEdge;
        
        private GraphEdge AddEdge(GameObject startNode, GameObject endNode)
        {
            var instance = Instantiate(prefabEdges, transform);
            var edge = instance.GetComponent<GraphEdge>();
            edges.Add(edge);
            edge.AddNodes(startNode, endNode);
            
            var start = startNode.GetComponent<GraphNode>();
            var end = startNode.GetComponent<GraphNode>();
            start.AddNeighbor(endNode);
            end.AddNeighbor(startNode);
            
            return edge;
        }
        
        public void AddPartialNodeForEdge(GameObject node)
        {
            GraphNode comp = node.GetComponent<GraphNode>();
            if (comp ==null) return;
            if (partialNodeForEdge == null)
            {
                partialNodeForEdge = node;
                return;
            }
            if (node == partialNodeForEdge)
            {
                return;
            }
            AddEdge(partialNodeForEdge, node);
            partialNodeForEdge = null;
        }

        public bool RemovePartialNodeForEdge()
        {
            if (partialNodeForEdge == null)
            {
                return false;
            }

            partialNodeForEdge = null;
            return true;
        }
        
        public Vector3 FindNearestPoint(Vector3 point)
        {
            var diff = point - nodes[0].transform.position;
            Vector3 nearestPoint = diff.normalized * 5;
            foreach (var edge in edges)
            {
                var len = edge.Direction.magnitude;
                var v = point - edge.StartPoint;
                var d = Vector3.Dot(v, edge.Direction);
                d = Mathf.Clamp(d, 0f, len);
                var resultPoint =  edge.StartPoint + edge.Direction * d;
                float distance = (resultPoint - point).magnitude;
                if(distance < (nearestPoint - point).magnitude)
                {
                    nearestPoint = resultPoint;
                }
            }
            return nearestPoint;
        }
        
    }
}
