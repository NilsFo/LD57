using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public enum GAME_STATE : UInt16
    {
        MAIN_MENU,
        PLAYING,
        PAUSED,
        ERROR
    }


    public enum PLAYER_STATE : UInt16
    {
        CAMERA,
        HOSE,
        ERROR
    }


    public Transform worldOrigin;

    [Header("States")]
    public GAME_STATE gameState;
    public PLAYER_STATE playerState;
    private GAME_STATE _gameState;
    private PLAYER_STATE _playerState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == PLAYER_STATE.ERROR)
        {
            Debug.LogError("PLAYER STATE IS ERROR");
            return;
        }
        if (gameState == GAME_STATE.ERROR)
        {
            Debug.LogError("GAME STATE IS ERROR");
            return;
        }

        // ####################
        // PLAYER STATE
        switch (playerState)
        {
            case PLAYER_STATE.CAMERA:
                break;
            case PLAYER_STATE.HOSE:
                break;
            default:
                break;
        }

        // ####################
        // GAME STATE
        switch (gameState)
        {
            case GAME_STATE.MAIN_MENU:
                break;
            case GAME_STATE.PAUSED:
                break;
            case GAME_STATE.PLAYING:
                break;
            default:
                break;
        }

    }

    public void onPlayerStateChanged()
    {
        print("New player state: " + playerState);
        switch (playerState)
        {
            case PLAYER_STATE.CAMERA:
                break;
            case PLAYER_STATE.HOSE:
                break;
            default:
                playerState = PLAYER_STATE.ERROR;
                break;
        }
    }

    public void onGameStateChanged()
    {
        print("New game state: " + gameState);
        switch (gameState)
        {
            case GAME_STATE.MAIN_MENU:
                break;
            case GAME_STATE.PAUSED:
                break;
            case GAME_STATE.PLAYING:
                break;
            default:
                gameState = GAME_STATE.ERROR;
                break;
        }
    }
}
