using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    public Transform target;

    public float billboardStrength = 0.5f;
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
        var dir = (target.position - transform.position).normalized;
        var billboardRot = Quaternion.LookRotation(dir, Vector3.up);
        var actualRotation = transform.parent.rotation;
        transform.rotation = Quaternion.Lerp(actualRotation, billboardRot, billboardStrength);

        var dirN = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        var forwardN = Vector3.ProjectOnPlane(transform.parent.right, Vector3.up).normalized;
        var flip = Mathf.Sign(Vector3.Dot(dirN, forwardN));
        transform.localScale = new Vector3(flip, 1, 1);

        //transform.Rotate(0,180,0);
    }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            return;
        }

        Handles.color = Color.white;
        Handles.DrawLine(transform.position, target.transform.position);
    }
    
#endif

}
