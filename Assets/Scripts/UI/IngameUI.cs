using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{

    private GameState gameState;
    private KnownFish knownFish;

    public TMP_Text objectiveTF;

    [Header("GameStateUIs")]
    public GameObject gamplayHolder;
    public GameObject mainMenuHolder;
    public GameObject pauseMenuHolder;


    public GameObject walkingHolder;
    public GameObject photoModeHolder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        knownFish = FindFirstObjectByType<KnownFish>();
    }

    // Update is called once per frame
    void Update()
    {
        objectiveTF.text = "Fish: " + knownFish.KnownFishCount + "/" + knownFish.AllFishCount;

        switch (gameState.gameState)
        {
            case GameState.GAME_STATE.PLAYING:
                break;
            default:
                break;
        }


        switch (gameState.playerState)
        {
            case GameState.PLAYER_STATE.CAMERA:
                photoModeHolder.SetActive(true);
                walkingHolder.SetActive(false);
                break;
            default:
                photoModeHolder.SetActive(false);
                walkingHolder.SetActive(true);
                break;
        }

        switch (gameState.gameState)
        {
            case GameState.GAME_STATE.ERROR:
                gamplayHolder.SetActive(false);
                mainMenuHolder.SetActive(false);
                pauseMenuHolder.SetActive(false);
                break;
            case GameState.GAME_STATE.MAIN_MENU:
                gamplayHolder.SetActive(false);
                mainMenuHolder.SetActive(true);
                pauseMenuHolder.SetActive(false);
                break;
            case GameState.GAME_STATE.PAUSED:
                gamplayHolder.SetActive(false);
                mainMenuHolder.SetActive(false);
                pauseMenuHolder.SetActive(true);
                break;
            case GameState.GAME_STATE.PLAYING:
                gamplayHolder.SetActive(true);
                mainMenuHolder.SetActive(false);
                pauseMenuHolder.SetActive(false);
                break;
        }
    }
}
