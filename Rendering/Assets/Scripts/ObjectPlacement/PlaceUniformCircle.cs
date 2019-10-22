using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUniformCircle : MonoBehaviour
{
    public int numObjects = 2;
    public float radius = 1.0f;
    public float maxAngle = 360.0f;
    public GameObject prefab;
    public Transform lookAtTarget;


    private void Awake()
    {
        Transform t = transform;

        for (int i = 0; i < numObjects; ++i)
        {
            Vector3 forward = transform.forward;


            GameObject o = Instantiate(prefab, transform.position, transform.rotation, transform);
            o.transform.Rotate(0.0f, maxAngle / numObjects * i, 0, 0f);
            o.transform.position = transform.position + radius * o.transform.forward.normalized;

            if (lookAtTarget)
            {
                o.transform.LookAt(lookAtTarget);
            }
            else
            {
                o.transform.LookAt(transform);
            }

        }



    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
