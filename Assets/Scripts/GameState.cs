using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
        WALKING,
        CAMERA,
        ERROR
    }


    public Transform worldOrigin;

    [Header("States")]
    public GAME_STATE gameState;
    public PLAYER_STATE playerState;
    private GAME_STATE _gameState;
    private PLAYER_STATE _playerState;

    public bool isCarryingHose = false;
    private bool _isCarryingHose = false;

    public Beacon hoseStartBeacon;

    [Header("Read Only Lists")]
    public List<BeaconTerminal> allBeaconTerminals;

    public UnityEvent<GAME_STATE> onGameStateChanged;
    public UnityEvent<PLAYER_STATE> onPlayerStateChanged;
    public UnityEvent onHoseStateChanged;

    public Camera mainCameraCache;

    private void Awake()
    {
        mainCameraCache = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isCarryingHose = false;
        isCarryingHose = false;
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

        if (_gameState != gameState)
        {
            GameStateChanged();
            _gameState = gameState;
        }
        if (_playerState != playerState)
        {
            PlayerStateChanged();
            _playerState = playerState;
        }

        // ####################
        // PLAYER STATE
        switch (playerState)
        {
            case PLAYER_STATE.CAMERA:
                break;
            case PLAYER_STATE.WALKING:
                break;
            default:
                break;
        }

        // ####################
        // GAME STATE
        switch (gameState)
        {
            case GAME_STATE.MAIN_MENU:

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GAME_STATE.PAUSED:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GAME_STATE.PLAYING:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            default:
                break;
        }

        if (_isCarryingHose != isCarryingHose)
        {
            HoseStateChanged();
            _isCarryingHose=isCarryingHose;
        }
    }

    public void HoseStateChanged()
    {
        print("Is carrying hose: " + isCarryingHose);

        if (!isCarryingHose)
        {
            hoseStartBeacon=null;
        }

        onHoseStateChanged.Invoke();
    }

    public void PlayerStateChanged()
    {
        print("New player state: " + playerState);
        switch (playerState)
        {
            case PLAYER_STATE.CAMERA:
                break;
            case PLAYER_STATE.WALKING:
                break;
            default:
                playerState = PLAYER_STATE.ERROR;
                break;
        }
        onPlayerStateChanged.Invoke(playerState);

        if (!isCarryingHose)
        {
            hoseStartBeacon = null;
        }
    }

    public void GameStateChanged()
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

    public Camera GetCamera()
    {
        return mainCameraCache;
    }

    private void LateUpdate()
    {
        mainCameraCache = Camera.main;
    }
}
