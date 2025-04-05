using UnityEngine;

namespace NodeGraph
{
    public class GraphEdge: MonoBehaviour
    {
        [SerializeField]
        GraphNode[] nodes;
        
        private Vector3 _direction;
        
        public GraphNode[] Nodes
        {
            get => nodes;
            set => nodes = value;
        }

        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public Vector3 StartPoint
        {
            get => nodes[0].transform.position;
        }
        
        public Vector3 EndPoint
        {
            get => nodes[1].transform.position;
        }

        public GraphNode[] AddNodes(GraphNode startNode, GraphNode endNode)
        {
            nodes = new [] { startNode, endNode };
            CalcDirection();
            return nodes;
        }
        
        private void CalcDirection()
        {
            if (nodes == null) return;
            _direction = nodes[0].transform.position - nodes[1].transform.position;
            _direction.Normalize();
        }

        public void SwapDirection()
        {
            nodes = new[]
            {
                nodes[1],
                nodes[0]
            };
            CalcDirection();
        }
    }
}
