using System;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public Color fogColor;
    private Color prevColor;
    public float fogDensity;
    private float prevDensity;

    public float envLighting = 1.6f;
    private float prevLighting;

    public float transitionTime;
    private float _t;

    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CharacterController>();
        if (player == null)
            return;
        prevColor = RenderSettings.fogColor;
        prevDensity = RenderSettings.fogDensity;
        prevLighting = RenderSettings.ambientIntensity;
        triggered = true;
    }

    private void Update()
    {
        if (triggered)
        {
            _t += Time.deltaTime;
            if (_t > transitionTime)
            {
                _t = transitionTime;
                triggered = false;
            }

            var t = _t / transitionTime;

            RenderSettings.fogColor = Color.Lerp(prevColor, fogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(prevDensity, fogDensity, t);
            RenderSettings.ambientIntensity = Mathf.Lerp(prevLighting, envLighting, t);
        }
    }
}