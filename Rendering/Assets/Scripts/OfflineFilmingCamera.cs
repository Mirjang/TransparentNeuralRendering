using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class OfflineFilmingCamera : MonoBehaviour
{
    public CustomRender recordingCamera;
    public float frameRate = 30;
    public bool writeImages = true; 
    public GameObject recordingIndicator;
    public GameObject writeIndicator;
    public Text frames; 
    public float moveSpeed = 1.0f;
    public float lookSpeed = 3.0f; 
    public float fastModeMultiplier = 2.0f;
    public float xClampMin = -80;
    public float xClampMax = 80;
    private Vector2 rotation = Vector2.zero;

    private Queue<Vector3> positions = new Queue<Vector3>();
    private Queue<Quaternion> rotations = new Queue<Quaternion>();
    public bool recording = false;

    private float lastFrameTime; 


    // Start is called before the first frame update
    void Start()
    {
        lastFrameTime = Time.time;
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
        if (Input.GetKey(KeyCode.R))
        {
            transform.position += transform.up * moveSpeed * fastMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.position -= transform.up * moveSpeed * fastMode * Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            recording = !recording;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            recording = false;
            positions.Clear();
            rotations.Clear(); 
        }

        if (recordingIndicator)
        {
            recordingIndicator.SetActive(recording);
        }
        if (writeIndicator)
        {
            writeIndicator.SetActive(positions.Count > 0 && !recording);
        }
        if(frames)
        {
            frames.text = "Frames: " + positions.Count; 
        }


        if (recording)
        {
            float t = Time.time; 
            if(lastFrameTime + 1.0f/frameRate < t)
            {
                positions.Enqueue(recordingCamera.gameObject.transform.position);
                rotations.Enqueue(recordingCamera.gameObject.transform.rotation);
                lastFrameTime = t; 
            }
        }


        if(positions.Count > 0 && !recording) // just store one imaage per frame, so the programm doesnt freeze up completely
        {
            var pos = positions.Dequeue();
            var rot = rotations.Dequeue();
            recordingCamera.transform.position = pos;
            recordingCamera.transform.rotation = rot;
            recordingCamera.RenderImage(writeImages); 
        }


    }
}
