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
        _gameState.onPlayerStateChanged.AddListener(UpdateState);
    }

    // Update is called once per frame
    void UpdateState(GameState.PLAYER_STATE playerState)
    {
        if (playerState == GameState.PLAYER_STATE.HOSE)
        {
            _anim.SetBool(Equipped, true);
        }
        else
        {
            _anim.SetBool(Equipped, false);
        }
    }
}
