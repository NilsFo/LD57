using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    public Transform target;

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
        //transform.Rotate(0,180,0);
    }
}
