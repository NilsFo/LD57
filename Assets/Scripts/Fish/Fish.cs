using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [Header("Identity")] public FishData data;
    public SpriteRenderer mySpriteRenderer;

    private GameState _gameState;
    private KnownFish _knownFish;

    [Header("I am an individual and i have individual traits")]
    public float temporalOffset = 0;
    public Vector3 spacialOffset = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _knownFish = FindFirstObjectByType<KnownFish>();
    }

    // Update is called once per frame
    void Update()
    {
        mySpriteRenderer.sprite = data.albumSprite;
    }

    public void OnPhotoTaken()
    {
        _knownFish.RegisterFish(data);
    }
}