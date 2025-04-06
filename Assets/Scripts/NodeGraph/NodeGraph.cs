using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeGraph
{
    public class NodeGraph : MonoBehaviour
    {
        [SerializeField] private GameObject prefabEdges;

        [SerializeField] private List<GameObject> nodes;

        [SerializeField] private List<GraphEdge> edges;

        [SerializeField] private GameObject partialNodeForEdge;

public int leashDistance = 30;

        private void Start()
        {
            var localNodes = FindObjectsByType<GraphNode>(FindObjectsSortMode.InstanceID);
            nodes = localNodes.OrderBy(node => node.ID).Select(node => node.gameObject).ToList();
        }

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
            if (comp == null) return;
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

        public int GetLeashDistance()
        {

            return leashDistance;
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
            Vector3 nearestPoint = nodes[0].transform.position;
            foreach (var edge in edges)
            {
                var resultPoint = NearestPointOnLine(edge.StartPoint, edge.EndPoint, point);

                float distance = (resultPoint - point).magnitude;
                if (distance < (nearestPoint - point).magnitude)
                {
                    nearestPoint = resultPoint;
                }
            }

            return nearestPoint;
        }

        public Vector3 FindNearestBorderPoint(Vector3 point)
        {
            var nearest = FindNearestPoint(point);
            var diff = (point - nearest);
            if (diff.magnitude > GetLeashDistance())
            {
                return diff.normalized * GetLeashDistance() + nearest;
            }

            return point;
        }

        public static Vector3 NearestPointOnLine(Vector3 linePnt1, Vector3 linePnt2, Vector3 pnt)
        {
            var lineDir = linePnt2 - linePnt1;
            var len = lineDir.magnitude;
            lineDir.Normalize(); //this needs to be a unit vector
            var v = pnt - linePnt1;
            var d = Vector3.Dot(v, lineDir);

            d = Mathf.Clamp(d, 0f, len);
            return linePnt1 + lineDir * d;
        }
    }
}