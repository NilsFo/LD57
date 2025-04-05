using UnityEngine;
using UnityEngine.Splines;

public class SchoolOfFish : MonoBehaviour
{

    public FishData fishData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyDataToChildren();
    }

    private void ApplyDataToChildren()
    {
        if (fishData == null) return;

        Fish[] fishChildren = GetComponentsInChildren<Fish>();
        foreach (Fish fish in fishChildren)
        {
            fish.data = fishData;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}