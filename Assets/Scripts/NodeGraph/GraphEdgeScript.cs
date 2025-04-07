using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

namespace NodeGraph
{
    public class GraphEdge : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] nodes;

        [SerializeField]
        private Rope rope;

        [SerializeField]
        private GameObject ropeStartNode;

        [SerializeField]
        private GameObject ropeEndNode;

        [SerializeField]
        private float slag;

        private Vector3 _direction;

        public GameObject[] Nodes
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

        public GameObject[] AddNodes(GameObject startNode, GameObject endNode)
        {
            nodes = new[] { startNode, endNode };

            try
            {
                ropeStartNode.transform.position = startNode.transform.position;
                ropeEndNode.transform.position = endNode.transform.position;

                rope.ropeLength = (startNode.transform.position - endNode.transform.position).magnitude + slag;
                rope.RecalculateRope();
            }
            catch (System.Exception)
            {
                Debug.LogWarning("ERROR CALCULATING ROPE!");
            }

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
