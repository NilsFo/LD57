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

    [Header("States")] public GAME_STATE gameState;
    public PLAYER_STATE playerState;
    private GAME_STATE _gameState;
    private PLAYER_STATE _playerState;

    public bool isCarryingHose = false;
    private bool _isCarryingHose = false;

    public Beacon hoseStartBeacon;

    [Header("Read Only Lists")] public List<BeaconTerminal> allBeaconTerminals;
    public List<FishData> debugReachableFish;

    public UnityEvent<GAME_STATE> onGameStateChanged;
    public UnityEvent<PLAYER_STATE> onPlayerStateChanged;
    public UnityEvent onHoseStateChanged;

    public Camera mainCameraCache;
    public MouseLook mouseLook;
    private CharacterMovement _movement;
    private MusicManager _musicManager;
    private GamepadInputDetector _gamepadInputDetector;

    public CharacterMovement PlayerMovementController => _movement;

    public float msgTetherExceededTimer = 5;
    public float _msgTetherExceededTimer = 5;

    private void Awake()
    {
        mainCameraCache = Camera.main;
        mouseLook = FindFirstObjectByType<MouseLook>();
        _movement = FindFirstObjectByType<CharacterMovement>();
        _musicManager = FindFirstObjectByType<MusicManager>();
        _gamepadInputDetector = FindFirstObjectByType<GamepadInputDetector>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isCarryingHose = false;
        isCarryingHose = false;
    }

    public void PlayTutorial()
    {
        print("Setting up tutorial.");
        var messageSystem = FindFirstObjectByType<MessageSystem>();
        messageSystem.EnqueueMessage("Explore and take pictures\nof the local wildlife".ToUpper());
        messageSystem.EnqueueMessage("Connect oxygen supply beacons".ToUpper());
        messageSystem.EnqueueMessage("You are tethered to the beacons".ToUpper());
    }

    // Update is called once per frame
    void Update()
    {
        _msgTetherExceededTimer -= Time.deltaTime;

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
                SetCursorLockState(false);
                mouseLook.enabled = false;
                _movement.inputDisabled = true;
                break;
            case GAME_STATE.PAUSED:
                SetCursorLockState(false);
                mouseLook.enabled = false;
                _movement.inputDisabled = true;
                break;
            case GAME_STATE.PLAYING:
                SetCursorLockState(true);
                mouseLook.enabled = true;
                _movement.inputDisabled = false;
                break;
            case GAME_STATE.CREDITS:
                SetCursorLockState(false);
                mouseLook.enabled = false;
                _movement.inputDisabled = true;
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
        Gamepad gamepad = Gamepad.current;

        bool pauseRequested = false;
        bool helpRequested = false;

        if (_gamepadInputDetector.isGamePad)
        {
            if (gamepad != null)
            {
                pauseRequested = gamepad.startButton.wasPressedThisFrame;
                helpRequested = gamepad.selectButton.wasPressedThisFrame;
            }
        }
        else if (keyboard != null)
        {
            pauseRequested = keyboard.tabKey.wasPressedThisFrame;
            helpRequested = keyboard.hKey.wasPressedThisFrame;
        }

        // Pause menu
        if (pauseRequested)
        {
            if (gameState == GAME_STATE.PAUSED)
            {
                gameState = GAME_STATE.PLAYING;
            }
            else if (gameState == GAME_STATE.PLAYING)
            {
                gameState = GAME_STATE.PAUSED;
            }
        }

        if (helpRequested)
        {
            if (gameState == GAME_STATE.PLAYING)
            {
                PlayTutorial();
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
            _musicManager.Stop();
        }
    }

    public void SetCursorLockState(bool locked)
    {
        if (_gamepadInputDetector.isGamePad)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

    internal void OnTetherLengthExceeded()
    {
        if (_msgTetherExceededTimer < 0)
        {
            _msgTetherExceededTimer = msgTetherExceededTimer;
            var messageSystem = FindFirstObjectByType<MessageSystem>();
            messageSystem.EnqueueMessage("Tether range exceeded".ToUpper());
        }
    }
}