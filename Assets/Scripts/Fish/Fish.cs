using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [Header("Identity")] public FishData data;
    public SpriteRenderer mySpriteRenderer;

    [Header("Movement")] public SplineContainer myContainer;
    public float moveSpeed = 1;
    private float _pathLength;

    public float progress = 0;
    public int currentStop = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pathLength = myContainer.CalculateLength();

        mySpriteRenderer.sprite = data.albumSprite;
    }

    // Update is called once per frame
    void Update()
    {
        progress += (moveSpeed * Time.deltaTime);
        progress = progress % _pathLength;

        if (myContainer != null)
        {
            myContainer.Evaluate(progress / _pathLength, out var pos, out var tangent, out _);
            transform.position = pos;
        }
    }
}