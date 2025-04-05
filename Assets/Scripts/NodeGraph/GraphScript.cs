using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class GraphScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefabEdges;

        [SerializeField] 
        private List<GameObject> nodes;
        
        [SerializeField]
        private List<GameObject> edges;

        [SerializeField] 
        private GameObject partialNodeForEdge;
        
        private GraphEdge AddEdge(GameObject startNode, GameObject endNode)
        {
            var instance = Instantiate(prefabEdges, transform);
            edges.Add(instance);
            
            var edge = instance.GetComponent<GraphEdge>();
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
            //if (comp ==null) return false;
            if (partialNodeForEdge == null)
            {
                partialNodeForEdge = node;
                //return true;
            }
            AddEdge(partialNodeForEdge, node);
            partialNodeForEdge = null;
            //return true;
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
            Vector3 nearestPoint = Vector3.positiveInfinity;
            foreach (var instance in edges)
            {
                var edge = instance.GetComponent<GraphEdge>();
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
