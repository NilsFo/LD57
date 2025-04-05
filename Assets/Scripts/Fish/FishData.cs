using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "LD57/New Fish", order = 0)]
public class FishData : ScriptableObject
{
    [Header("Names")] public string albumName;

    [Header("Visuals")] public Sprite albumSprite;

    private void OnValidate()
    {
    }
}