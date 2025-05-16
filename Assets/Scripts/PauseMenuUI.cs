using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class PauseMenuUI : MonoBehaviour
{
    public List<FishDatabaseEntry> fishPoster;

    private GameState _gameState;
    private MouseLook _mouseLook;
    private CharacterController _controller;
    public TMP_Text descriptionTF;
    private KnownFish _knownFish;
    private GamepadInputDetector _gamepadInputDetector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gamepadInputDetector = FindFirstObjectByType<GamepadInputDetector>();
        _gameState = FindFirstObjectByType<GameState>();
        _knownFish = FindFirstObjectByType<KnownFish>();
        _mouseLook = FindFirstObjectByType<MouseLook>();
        _controller = FindFirstObjectByType<CharacterController>();

        _gamepadInputDetector.onSwitchToGamePad.AddListener(OnSwitchToGamePad);
        _gamepadInputDetector.onSwitchToKeyBoard.AddListener(OnSwitchToKeyBoard);

        InitSelection();
    }

    private void OnEnable()
    {
        InitSelection();
    }

    private void OnDisable()
    {
    }

    private void InitSelection()
    {
        UnselectAllEntries();

        if (_gamepadInputDetector != null && _gamepadInputDetector.isGamePad)
        {
            fishPoster[0].isHovered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Reading gamepad inputs
        bool inputGamepadLeft = false;
        bool inputGamepadRight = false;
        bool inputGamepadUp = false;
        bool inputGamepadDown = false;
        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                DpadControl dPad = gamepad.dpad;
                if (dPad != null)
                {
                    inputGamepadLeft = dPad.left.wasPressedThisFrame;
                    inputGamepadRight = dPad.right.wasPressedThisFrame;
                    inputGamepadUp = dPad.up.wasPressedThisFrame;
                    inputGamepadDown = dPad.down.wasPressedThisFrame;
                }
            }

            // making a default selection
            bool hasAnythingSelected = false;
            foreach (var poster in fishPoster)
            {
                if (poster.isHovered)
                {
                    hasAnythingSelected = true;
                }
            }

            if (!hasAnythingSelected)
            {
                fishPoster[0].isHovered = true;
            }
        }

        int selectedIndex = -1;
        for (var i = 0; i < fishPoster.Count; i++)
        {
            FishDatabaseEntry poster = fishPoster[i];
            if (poster.isHovered)
            {
                if (selectedIndex == -1)
                {
                    selectedIndex = i;
                }
                else
                {
                    poster.isHovered = false;
                }
            }
        }

        // applying gamepad input
        int oldSelectedIndex = selectedIndex;
        if (inputGamepadRight)
        {
            selectedIndex += 1;
        }

        if (inputGamepadLeft)
        {
            selectedIndex -= 1;
        }

        if (inputGamepadUp)
        {
            selectedIndex -= 6;
        }

        if (inputGamepadDown)
        {
            selectedIndex += 6;
        }

        //print("new selected index: " + selectedIndex + " -> " + selectedIndex % 18
        //      + " [old: " + oldSelectedIndex + "]");
        while (selectedIndex < 0)
        {
            selectedIndex += 18;
        }

        selectedIndex = selectedIndex % 18;

        if (selectedIndex != oldSelectedIndex && oldSelectedIndex != -1)
        {
            fishPoster[oldSelectedIndex].isHovered = false;
            fishPoster[selectedIndex].isHovered = true;
        }

        FishData selectedData = null;
        if (selectedIndex != -1)
        {
            selectedData = fishPoster[selectedIndex].fishData;
        }

        // Updating UI based on selection
        var description = "Mouse over species to learn more.";
        if (selectedData != null)
        {
            if (_knownFish.IsKnown(selectedData))
            {
                description = selectedData.displayName + ": " + selectedData.whereFindText;
            }
            else
            {
                description = selectedData.whereFindText;
            }
        }

        descriptionTF.text = description;
    }

    public void OnContinuePressed()
    {
        _gameState.gameState = GameState.GAME_STATE.PLAYING;
    }

    public void OnRestartPressed()
    {
        _gameState.BackToMenu();
    }

    public void UnselectAllEntries()
    {
        foreach (FishDatabaseEntry databaseEntry in fishPoster)
        {
            databaseEntry.isHovered = false;
        }
    }

    private void OnSwitchToKeyBoard()
    {
        InitSelection();
    }

    private void OnSwitchToGamePad()
    {
        InitSelection();
    }
}