using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [Header("Identity")] public FishData data;
    public Renderer mySpriteRenderer;
    public SpriteRenderer debugSprite;

    //private GameState _gameState;
    private KnownFish _knownFish;

    [Header("I am an individual and i have individual traits")]
    public float temporalOffset = 0;
    public Vector3 spacialOffset = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_gameState = FindFirstObjectByType<GameState>();
        _knownFish = FindFirstObjectByType<KnownFish>();

        debugSprite.gameObject.SetActive(false);
        mySpriteRenderer.gameObject.SetActive(true);

        mySpriteRenderer.material.mainTexture = data.albumSprite;
        var sizeX = (float)data.albumSprite.width / (float)data.pixelPerMeter;
        var sizeY = (float)data.albumSprite.height / (float)data.pixelPerMeter;
        mySpriteRenderer.transform.localScale = new Vector3(sizeX, sizeY, 1);
        
        var box = GetComponent<BoxCollider>();
        box.size = new Vector3(sizeX, sizeY, 1);
    }

    public void OnPhotoTaken()
    {
        _knownFish.RegisterFish(data);
    }
}