using UnityEngine;

public class PlayerInputDelegator : MonoBehaviour
{
    [Header("Config")] public bool useGamepadOverKBM = false;

    [Header("Player Elements")] public CharacterMovement characterMovement;
    public MouseLook mouseLook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        useGamepadOverKBM = false;
    }

    // Update is called once per frame
    void Update()
    {
        characterMovement.useGamepadOverKBM = useGamepadOverKBM;
        mouseLook.useGamepadOverKBM = useGamepadOverKBM;
    }

    public void SetUseGameoadOverKBM(bool gamepad)
    {
        useGamepadOverKBM = gamepad;
    }
}