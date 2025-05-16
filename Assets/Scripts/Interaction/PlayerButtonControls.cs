using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerButtonControls : MonoBehaviour
{
    private PlayerInteraction _interaction;
    private GamepadInputDetector _gamepadInputDetector;

    private void Start()
    {
        _gamepadInputDetector = FindFirstObjectByType<GamepadInputDetector>();
        _interaction = FindFirstObjectByType<PlayerInteraction>();
    }

    private void Update()
    {
        bool interactionRequested = false;

        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                interactionRequested = gamepad.buttonWest.wasPressedThisFrame;
            }
        }
        else
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                interactionRequested = keyboard.eKey.wasPressedThisFrame;
            }
        }

        if (interactionRequested)
        {
            var inFocus = _interaction.GetInteractableInFocus();
            if (inFocus != null)
            {
                inFocus.Interact();
            }
            else
            {
                print("nichts im focus");
            }
        }
    }
}