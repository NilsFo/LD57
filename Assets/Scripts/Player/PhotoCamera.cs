using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class PhotoCamera : MonoBehaviour
{
    public float maxRange = 10f;
    public float paddingPercent = 0.1f;
    private Camera _cam;
    private LayerMask _photoLayerMask;

    public Animator camAnim;

    public enum PhotoCameraState
    {
        Idle, Transition, Raised
    }
    public PhotoCameraState state;

    private void Start()
    {
        _photoLayerMask = LayerMask.GetMask("Default", "Entities");
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && state == PhotoCameraState.Raised)
        {
            TakePhoto();
        }
        
        if ((Mouse.current.rightButton.wasPressedThisFrame) &&
            state == PhotoCameraState.Raised)
        {
            Lower();
        }
        
        if ((Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) &&
            state == PhotoCameraState.Idle)
        {
            Raise();
        }
    }

    public void Raise()
    {
        state = PhotoCameraState.Transition;
        camAnim.SetTrigger("Aim");
        Invoke(nameof(RaiseFinish), 10f/60f);
    }

    public void RaiseFinish()
    {
        state = PhotoCameraState.Raised;
    }

    public void Lower()
    {
        state = PhotoCameraState.Transition;
        camAnim.SetTrigger("Unaim");
        Invoke(nameof(LowerFinish), 10f/60f);
    }
    
    public void LowerFinish()
    {
        state = PhotoCameraState.Idle;
    }

    public void TakePhoto()
    {
        int width = Mathf.FloorToInt(_cam.pixelWidth * (1 - 2 * paddingPercent));
        int height = Mathf.FloorToInt(_cam.pixelHeight * (1- 2 * paddingPercent));
        var pixelStep = 32;
        List<GameObject> hits = new List<GameObject>();
        for (int x = Mathf.FloorToInt(_cam.pixelWidth * paddingPercent); x < width; x += pixelStep)
        {
            for (int y = Mathf.FloorToInt(_cam.pixelHeight * paddingPercent); y < height; y += pixelStep)
            {
                var r = _cam.ScreenPointToRay(new Vector2(x, y));
                var foundObject = ShootPhotoRay(r);
                if(foundObject == null)
                    continue;
                if(hits.Contains(foundObject))
                    continue;
                hits.Add(foundObject);
            }
        }
        
        Debug.Log("Photoshoot hit " + hits.Count + " Entities");
        
        foreach (GameObject hit in hits)
        {
            PhotoListener listener = hit.GetComponentInChildren<PhotoListener>();
            if (listener != null)
            {
                listener.OnPhotoTaken();
            }
        }
    }

    private GameObject ShootPhotoRay(Ray r)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(r, out hit, maxRange, _photoLayerMask))

        {
            Debug.DrawRay(transform.position, r.direction * hit.distance, Color.yellow, .5f);
            if(hit.collider.tag.Equals("Photogenic"))
                return hit.collider.gameObject;
        }
        Debug.DrawRay(transform.position, r.direction * maxRange, Color.white, .5f);
        return null;
    }
}