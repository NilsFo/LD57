using UnityEngine;

namespace Tumble2.Character
{
    public class FallReset : MonoBehaviour
    {
        private GameState _gameState;
        private Rigidbody _rb;
        public float thresholdY = -100;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gameState = FindObjectOfType<GameState>();
        }

        private void Update()
        {
            if (transform.position.y < thresholdY)
            {
                transform.position = _gameState.worldOrigin.transform.position;

                if (_rb != null)
                {
                    _rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
}