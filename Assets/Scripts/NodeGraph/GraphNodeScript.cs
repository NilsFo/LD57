using UnityEngine;

namespace NodeGraph
{
    public class GraphNode: MonoBehaviour
    {
        private int _id;
        
        [SerializeField]
        private GraphNode[] neighborNodes;
        
        public int ID
        {
            get => _id;
            set => _id = value;
        }
        
    }
}
