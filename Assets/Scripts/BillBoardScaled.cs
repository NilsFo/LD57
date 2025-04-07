using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BillBoardScaled : MonoBehaviour
{
    public Transform target;
    public float scale = 0.01f;

    public enum ScaleType
    {
        Linear,
        Square,
        InverseSquare,
        SquareRoot,
        Exponential
    }

    public ScaleType scaleType;
    private GameState _gameState;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        target = _gameState.GetCamera().transform;
        transform.LookAt(target.position, Vector3.up);

        var dist = (target.position - transform.position).magnitude;

        switch (scaleType)
        {
            case ScaleType.Linear:
                transform.localScale = dist * scale * Vector3.one;
                break;
            case ScaleType.Square:
                transform.localScale = dist * dist * scale * Vector3.one;
                break;
            case ScaleType.InverseSquare:
                transform.localScale = 1 / (dist * dist) * scale * Vector3.one;
                break;
            case ScaleType.SquareRoot:
                transform.localScale = Mathf.Sqrt(dist) * scale * Vector3.one;
                break;
            case ScaleType.Exponential:
                transform.localScale = Mathf.Exp(dist) * scale * Vector3.one;
                break;
        }
    }
}