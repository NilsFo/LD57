using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{

    private GameState gameState;
    private KnownFish knownFish;

    public TMP_Text objectiveTF;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        knownFish = FindFirstObjectByType<KnownFish>();
    }

    // Update is called once per frame
    void Update()
    {
        objectiveTF.text = "Fish: " + knownFish.KnownFishCount + "/" + knownFish.AllFishCount;
    }
}
