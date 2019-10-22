using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCamera : MonoBehaviour
{
    public CustomRender recordingCamera; 
    public float moveSpeed = 1.0f;
    public float lookSpeed = 3.0f; 
    public float fastModeMultiplier = 2.0f;
    public float xClampMin = -80;
    public float xClampMax = 80;
    private Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fastMode = Input.GetKey(KeyCode.LeftShift)? fastModeMultiplier : 1.0f;

        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, xClampMin, xClampMax);
        var qr = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, qr, lookSpeed * Time.deltaTime); //Smooth Update

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * fastMode * Time.deltaTime; 
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * moveSpeed * fastMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * fastMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * fastMode * Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            recordingCamera.RenderImage(); 
        }


    }
}
