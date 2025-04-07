using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "LD57/New Fish", order = 0)]
public class FishData : ScriptableObject
{
    [Header("Names")] public string displayName;

    [Header("Visuals")] public Texture2D albumSprite;
    public int pixelPerMeter = 32;


    private void OnValidate()
    {
    }
}