using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class PhotoCamera : MonoBehaviour
{
    public float maxRange = 10f;
    public float paddingPercent = 0.1f;
    public Camera cam;
    private LayerMask _photoLayerMask;
    private GamepadInputDetector _gamepadInputDetector;

    public List<GameObject> viewmodelsRaised;
    public List<GameObject> viewmodelsLowered;
    public Animator camAnim;
    private GameState _gameState;

    public AudioClip photoSound;

    public Light cameraFlash;
    public float cameraFlashIntensity = 2000f;

    public float cameraCooldown = 1f;
    private float _cameraCooldown = 0;

    public Volume cameraPostProcessing;

    public float addFov = -5f;
    private float _baseFov;

    public enum PhotoCameraState : UInt16
    {
        Idle,
        Transition,
        Raised
    }

    public PhotoCameraState state;
    public float minFocus = 1f, maxFocus = 50f;
    private float _currentFocus = 0.5f;
    private float _targetFocus = 0.5f;
    public float focusRange = 0.08f;

    public float CurrentFocus => _currentFocus;

    public UnityEvent onBlurryPhoto;

    private void Start()
    {
        _gamepadInputDetector = FindFirstObjectByType<GamepadInputDetector>();
        _gameState = FindFirstObjectByType<GameState>();
        _photoLayerMask = LayerMask.GetMask("Default", "Entities");
        _baseFov = cam.fieldOfView;
    }

    private void Update()
    {
        _cameraCooldown -= Time.deltaTime;

        bool inputTakePhoto = false;
        bool inputRaise = false;

        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                inputTakePhoto = gamepad.rightTrigger.wasPressedThisFrame;
                inputRaise = gamepad.buttonSouth.wasPressedThisFrame
                             || gamepad.buttonEast.wasPressedThisFrame
                             || gamepad.leftTrigger.wasPressedThisFrame;
            }
        }
        else
        {
            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                inputTakePhoto = mouse.leftButton.wasPressedThisFrame;
                inputRaise = mouse.rightButton.wasPressedThisFrame;
            }
        }

        if (inputTakePhoto &&
            state == PhotoCameraState.Raised &&
            _gameState.gameState == GameState.GAME_STATE.PLAYING)
        {
            if (_cameraCooldown <= 0)
            {
                _cameraCooldown = cameraCooldown;
                TakePhoto();
            }
        }

        if (inputRaise &&
            state == PhotoCameraState.Raised &&
            _gameState.gameState == GameState.GAME_STATE.PLAYING)
        {
            Lower();
        }

        if ((inputTakePhoto || inputRaise) &&
            state == PhotoCameraState.Idle &&
            _gameState.gameState == GameState.GAME_STATE.PLAYING)
        {
            Raise();
        }

        if (state == PhotoCameraState.Raised && _gameState.gameState == GameState.GAME_STATE.PLAYING)
        {
            UpdateDOF();
        }
    }

    public void Raise()
    {
        state = PhotoCameraState.Transition;
        camAnim.SetTrigger("Aim");
        Invoke(nameof(RaiseFinish), 10f / 30f);
    }

    public void RaiseFinish()
    {
        state = PhotoCameraState.Raised;
        _gameState.playerState = GameState.PLAYER_STATE.CAMERA;
        foreach (var viewmodel in viewmodelsLowered)
        {
            viewmodel.SetActive(false);
        }

        foreach (var viewmodel in viewmodelsRaised)
        {
            viewmodel.SetActive(true);
        }
    }

    public void Lower()
    {
        state = PhotoCameraState.Transition;
        camAnim.SetTrigger("Unaim");
        Invoke(nameof(LowerFinish), 10f / 30f);

        _gameState.playerState = GameState.PLAYER_STATE.WALKING;
        foreach (var viewmodel in viewmodelsLowered)
        {
            viewmodel.SetActive(true);
        }

        foreach (var viewmodel in viewmodelsRaised)
        {
            viewmodel.SetActive(false);
        }
    }

    public void LowerFinish()
    {
        state = PhotoCameraState.Idle;
    }

    public void UpdateDOF()
    {
        float deltaFocus = 0f;

        bool inputScrollDown = false;
        bool inputScrollUp = false;
        bool fastScroll = false;
        float scroll = 0;

        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                DpadControl dpadControl = gamepad.dpad;
                if (dpadControl != null)
                {
                    inputScrollDown = dpadControl.down.wasPressedThisFrame || dpadControl.left.wasPressedThisFrame;
                    inputScrollUp = dpadControl.up.wasPressedThisFrame || dpadControl.right.wasPressedThisFrame;

                    if (dpadControl.left.wasPressedThisFrame || dpadControl.right.wasPressedThisFrame)
                    {
                        fastScroll = true;
                    }
                }
            }
        }
        else
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (mouse != null)
            {
                scroll = Mouse.current.scroll.ReadValue()[1];
            }

            if (keyboard != null)
            {
                inputScrollDown = keyboard.fKey.wasPressedThisFrame;
                inputScrollUp = keyboard.rKey.wasPressedThisFrame;
            }
        }

        if (scroll > 0 || inputScrollUp)
        {
            deltaFocus = 0.05f;
        }
        else if (scroll < 0 || inputScrollDown)
        {
            deltaFocus = -0.05f;
        }

        if (fastScroll)
        {
            deltaFocus *= 3;
        }

        // if (deltaFocus == 0f)
        //     return;

        _targetFocus = Mathf.Clamp(_targetFocus + deltaFocus, 0, 1);
        _currentFocus = Mathf.MoveTowards(_currentFocus, _targetFocus, Time.deltaTime);
        float focusDist = Mathf.Lerp(minFocus, maxFocus, (_currentFocus * _currentFocus));

        UnityEngine.Rendering.Universal.DepthOfField dof;

        if (!cameraPostProcessing.sharedProfile.TryGet(out dof)) return;

        dof.focusDistance.Override(focusDist);
        cam.fieldOfView = Mathf.Lerp(_baseFov, _baseFov + addFov, _currentFocus);
    }

    public bool IsInFocus(Vector3 point)
    {
        var dist = Vector3.Distance(transform.position, point);

        var focusDistLower = Mathf.LerpUnclamped(minFocus, maxFocus, ((_currentFocus - focusRange) *
                                                                      (_currentFocus - focusRange)));
        var focusDistUpper = Mathf.LerpUnclamped(minFocus, maxFocus, (_currentFocus + focusRange) *
                                                                     (_currentFocus + focusRange));
        var inFocus = dist >= focusDistLower && dist <= focusDistUpper;

        /*if (!inFocus)
            Debug.Log("Not in Focus: " + dist + " away, should be between " + focusDistLower + " and " + focusDistUpper);
        else
        {
            Debug.Log("Is in Focus!!: " + dist + " away, can be between " + focusDistLower + " and " + focusDistUpper);
        }*/
        return inFocus;
    }

    public void TakePhoto()
    {
        int width = Mathf.FloorToInt(cam.pixelWidth * (1 - 2 * paddingPercent));
        int height = Mathf.FloorToInt(cam.pixelHeight * (1 - 2 * paddingPercent));
        var pixelStep = 32;
        List<GameObject> hits = new List<GameObject>();
        for (int x = Mathf.FloorToInt(cam.pixelWidth * paddingPercent); x < width; x += pixelStep)
        {
            for (int y = Mathf.FloorToInt(cam.pixelHeight * paddingPercent); y < height; y += pixelStep)
            {
                var r = cam.ScreenPointToRay(new Vector2(x, y));
                var foundObject = ShootPhotoRay(r);
                if (foundObject == null)
                    continue;
                if (hits.Contains(foundObject))
                    continue;
                hits.Add(foundObject);
            }
        }

        Debug.Log("Photoshoot hit " + hits.Count + " Entities");

        bool anyBlurry = false, anyCaptured = false;

        foreach (GameObject hit in hits)
        {
            PhotoListener listener = hit.GetComponentInChildren<PhotoListener>();
            if (listener != null)
            {
                if (IsInFocus(listener.transform.position))
                {
                    listener.OnPhotoTaken();
                    anyCaptured = true;
                }
                else
                {
                    anyBlurry = true;
                    listener.OnPhotoOutOfFocus();
                }
            }
        }

        if (anyBlurry && !anyCaptured)
        {
            Debug.Log("Blurry Photo taken");
            onBlurryPhoto.Invoke();
        }

        Debug.Log("Playing Photo Sound");
        // Play Sound
        FindFirstObjectByType<MusicManager>().CreateAudioClip(photoSound, Vector3.zero);

        Debug.Log("Flashing Camera");
        // Flash
        cameraFlash.intensity = cameraFlashIntensity;
        cameraFlash.DOIntensity(0f, 0.2f);
    }

    private GameObject ShootPhotoRay(Ray r)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(r, out hit, maxRange, _photoLayerMask))

        {
            Debug.DrawRay(transform.position, r.direction * hit.distance, Color.yellow, .5f);
            if (hit.collider.tag.Equals("Photogenic"))
                return hit.collider.gameObject;
        }

        Debug.DrawRay(transform.position, r.direction * maxRange, Color.white, .5f);
        return null;
    }
}