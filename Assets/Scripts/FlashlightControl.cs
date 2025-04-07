using System;
using DG.Tweening;
using UnityEngine;

public class FlashlightControl : MonoBehaviour
{
    public Light flashlight;
    public float intensity;

    private void Start()
    {
        if(flashlight != null)
            intensity = flashlight.intensity;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CharacterController>();
        if (player == null)
            return;
        if (flashlight == null)
            return;
        Invoke(nameof(FlashlightOn), 3f);
    }

    public void FlashlightOn()
    {
        flashlight.gameObject.SetActive(true);
        flashlight.intensity = 0;
        flashlight.DOIntensity(intensity, 0.2f);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<CharacterController>();
        if (player == null)
            return;

        if (flashlight == null)
            return;
        
        flashlight.DOIntensity(0f, 0.5f);
    }
}
