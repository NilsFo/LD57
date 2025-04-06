using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardScaled : MonoBehaviour
{

    public Transform target;
    public float scale = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            target = Camera.main?.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(target.position, Vector3.up);
        var dist = (target.position - transform.position).magnitude;
        transform.localScale = dist * scale * Vector3.one;
    }
}
