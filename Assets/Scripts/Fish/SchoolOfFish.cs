using System;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SchoolOfFish : MonoBehaviour
{
    public FishData fishData;
    private GameState _gameState;
    public GameObject fishPrefab;

    [Header("Config")] public bool staticMovement = false;
    public bool singleton = false;

    [Header("Movement")] public SplineContainer myContainer;
    public float moveSpeed = 1;
    private float _pathLength;

    [Header("School size")] public Fish leaderFish;
    public int numberOfOtherFishMin = 5;
    public int numberOfOtherFishMax = 10;
    public float randomSpacialDistanceJitter = 3f;
    public float randomTemporalDistanceJitter = 0.1f;
    [Range(1, 10)] public int schoolMirrors = 1;

    [Header("Readonly Debug")] [SerializeField]
    public List<School> schools;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pathLength = myContainer.CalculateLength();
        _gameState = FindFirstObjectByType<GameState>();

        if (singleton)
        {
            schoolMirrors = 1;
        }

        schools = new List<School>();
        schools.Clear();
        schools.Add(new School());

        // initial random progress
        if (!singleton || !staticMovement)
        {
            schools[0].progress = Random.Range(0f, _pathLength);
        }

        schools[0].fish.Add(leaderFish);

        if (!singleton)
        {
            for (int i = 0; i < schoolMirrors; i++)
            {
                GenerateMoreFish(i);

                float offset = (float)i / (float)schoolMirrors;
                schools[i].progress = schools[0].progress + offset * _pathLength;
            }
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

    public void GenerateMoreFish(int schoolIndex)
    {
        int moreCount = Random.Range(numberOfOtherFishMin, numberOfOtherFishMax);
        for (int i = 0; i < moreCount; i++)
        {
            NextFish(schoolIndex);
        }
    }

    private void NextFish(int schoolIndex)
    {
        GameObject newFishObj = Instantiate(fishPrefab, transform);
        Fish newFish = newFishObj.GetComponent<Fish>();

        newFish.temporalOffset = Random.Range(randomTemporalDistanceJitter * -1, 0);
        newFish.spacialOffset = new Vector3(
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter),
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter),
            Random.Range(randomSpacialDistanceJitter * -1, randomSpacialDistanceJitter)
        );

        if (schools.Count <= schoolIndex || schools[schoolIndex] == null)
        {
            schools.Insert(schoolIndex, new School());
        }

        schools[schoolIndex].fish.Add(newFish);
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
        if (myContainer == null)
        {
            return;
        }

        float directionFactor = 1.0f;

        foreach (School school in schools)
        {
            float progress = school.progress + (moveSpeed * Time.deltaTime) * directionFactor;

            foreach (Fish fish in school.fish)
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

            progress = progress % _pathLength;
            school.progress = progress;
        }
    }

    [Serializable]
    public class School
    {
        public List<Fish> fish;
        public float progress;

        public School()
        {
            fish = new List<Fish>();
            progress = 0;
        }
    }
}