using UnityEngine;
using UnityEngine.Splines;

public class SchoolOfFish : MonoBehaviour
{

    public FishData fishData;
    public SplineContainer mySpline;
    private GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();

        ApplyDataToChildren();
        RegisterToTerminals();
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
        foreach (BeaconTerminal terminal in gameState.allBeaconTerminals)
        {
            float beaconDiscoveryRange = terminal.fishDiscoveryRange;
            if (Vector3.Distance(transform.position, terminal.transform.position) < beaconDiscoveryRange)
            {
                terminal.AddNearbyFish(fishData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}