using UnityEngine;

public class HoseViewModel : MonoBehaviour
{
    private static readonly int Equipped = Animator.StringToHash("equipped");
    private GameState _gameState;

    private Animator _anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
        _gameState = FindFirstObjectByType<GameState>();
        _gameState.onHoseStateChanged.AddListener(UpdateState);
    }

    // Update is called once per frame
    void UpdateState()
    {
        if (_gameState.isCarryingHose)
        {
            _anim.SetBool(Equipped, true);
        }
        else
        {
            _anim.SetBool(Equipped, false);
        }
    }
}