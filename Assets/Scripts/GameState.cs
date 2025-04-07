using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{

    public enum GAME_STATE : UInt16
    {
        MAIN_MENU,
        PLAYING,
        PAUSED,
        ERROR,
        CREDITS
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
    public List<FishData> debugReachableFish;

    public UnityEvent<GAME_STATE> onGameStateChanged;
    public UnityEvent<PLAYER_STATE> onPlayerStateChanged;
    public UnityEvent onHoseStateChanged;

    public Camera mainCameraCache;
    public MouseLook mouseLook;
    public CharacterMovement movement;
    private MusicManager musicManager;

    private void Awake()
    {
        mainCameraCache = Camera.main;
        mouseLook = FindFirstObjectByType<MouseLook>();
        movement = FindFirstObjectByType<CharacterMovement>();
        musicManager = FindFirstObjectByType<MusicManager>();
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
                mouseLook.enabled = false;
                movement.inputDisabled = true;
                break;
            case GAME_STATE.PAUSED:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                mouseLook.enabled = false;
                movement.inputDisabled = true;
                break;
            case GAME_STATE.PLAYING:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mouseLook.enabled = true;
                movement.inputDisabled = false;
                break;
            default:
                break;
        }

        if (_isCarryingHose != isCarryingHose)
        {
            HoseStateChanged();
            _isCarryingHose = isCarryingHose;
        }

        Keyboard keyboard = Keyboard.current;
        // Pause menu

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            if (gameState == GAME_STATE.PAUSED)
            {
                gameState = GAME_STATE.PLAYING;
            }else
            if (gameState == GAME_STATE.PLAYING)
            {
                gameState = GAME_STATE.PAUSED;
            }
        }
    }

    public void HoseStateChanged()
    {
        print("Is carrying hose: " + isCarryingHose);

        if (!isCarryingHose)
        {
            hoseStartBeacon = null;
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
            case GAME_STATE.CREDITS:
                break;
            default:
                gameState = GAME_STATE.ERROR;
                break;
        }

        if (_gameState == GAME_STATE.MAIN_MENU)
        {
            musicManager.Stop();
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

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
