using UnityEngine;

public class MainMenuControls : MonoBehaviour
{
    private GameState _gameState;
    public GameObject mainCameraHolder;
    public GameObject playerHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
        playerHolder.gameObject.SetActive(false);
        mainCameraHolder.gameObject.SetActive(true);
        _gameState.gameState = GameState.GAME_STATE.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartGame()
    {
        playerHolder.gameObject.SetActive(true);
        mainCameraHolder.gameObject.SetActive(false);
        _gameState.gameState = GameState.GAME_STATE.PLAYING;
    }
}