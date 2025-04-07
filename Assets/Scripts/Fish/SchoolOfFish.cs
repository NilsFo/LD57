using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SchoolOfFish : MonoBehaviour
{

    public FishData fishData;
    private GameState _gameState;
    public GameObject FishPrefab;

    [Header("Config")]
    public bool staticMovement = false;
    public bool singleton = false;

    [Header("Movement")] public SplineContainer myContainer;
    public float moveSpeed = 1;
    private float _pathLength;
    public bool reversedDirection;

    public float progress = 0;

    [Header("School size")]
    public Fish leaderFish;
    public int numberOfOtherFishMin = 5;
    public int numberOfOtherFishMax = 10;
    public float randomSpacialDistanceJitter = 3f;
    public float randomTemporalDistanceJitter = 0.1f;
    public List<Fish> fishInScool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pathLength = myContainer.CalculateLength();
        _gameState = FindFirstObjectByType<GameState>();
        fishInScool.Clear();

        fishInScool.Add(leaderFish);
        if (!singleton)
        {
            GenerateMoreFish();
        }

        if (staticMovement)
        {
            myContainer = null;
        }

        ApplyDataToChildren();
        RegisterToTerminals();

        if (staticMovement)
        {
            leaderFish.billBoard.billboardStrength = 1.0f;
        }
    }

    void OnEnable()
    {
        ApplyDataToChildren();
    }

    public void GenerateMoreFish()
    {
        int moreCount = Random.Range(numberOfOtherFishMin, numberOfOtherFishMax);
        for (int i = 0; i < moreCount; i++)
        {
            NextFish();
        }
    }

    private void NextFish()
    {
        GameObject newFishObj = Instantiate(FishPrefab, transform);
        Fish newFish = newFishObj.GetComponent<Fish>();

        newFish.temporalOffset = Random.Range(randomTemporalDistanceJitter * -1, 0);
        newFish.spacialOffset = new Vector3(
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter),
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter),
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter)
        );

        fishInScool.Add(newFish);
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

    public void RegisterToTerminals()
    {
        bool beaconFound = false;
        foreach (BeaconTerminal terminal in _gameState.allBeaconTerminals)
        {
            float beaconDiscoveryRange = terminal.fishDiscoveryRange;
            if (Vector3.Distance(transform.position, terminal.transform.position) < beaconDiscoveryRange)
            {
                terminal.AddNearbyFish(fishData);
                beaconFound = true;
            }
        }

        if (!beaconFound)
        {
            Debug.LogWarning("Fish school has no beacon nearby", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float directionFactor = 1.0f;
        if (reversedDirection)
        {
            directionFactor = -1;
        }
        progress += (moveSpeed * Time.deltaTime) * directionFactor;

        if (myContainer != null)
        {
            foreach (Fish fish in fishInScool)
            {
                float individualProgress = progress + fish.temporalOffset;
                individualProgress = individualProgress % _pathLength;

                myContainer.Evaluate(individualProgress / _pathLength, out var pos, out var tangent, out _);

                Vector3 newPos = new Vector3(
                    pos.x + fish.spacialOffset.x,
                    pos.y + fish.spacialOffset.y,
                    pos.z + fish.spacialOffset.z
                );
                fish.transform.position = newPos;
                fish.transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
            }
        }
        progress = progress % _pathLength;
    }
}