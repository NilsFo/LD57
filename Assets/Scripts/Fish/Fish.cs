using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    public SplineContainer myContainer;
    public float moveSpeed = 1;
    public float railPos;
    private float _railLength;

    public float progress = 0;
    public int currentStop = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _railLength = myContainer.CalculateLength();
    }

    // Update is called once per frame
    void Update()
    {
        progress += (moveSpeed * Time.deltaTime);
        progress = progress % _railLength;

        if (myContainer != null)
        {
            myContainer.Evaluate(progress / _railLength, out var pos, out var tangent, out _);
            transform.position = pos;
        }
    }
}