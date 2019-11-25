using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SampleImages : MonoBehaviour
{
    public float minRadius = .8f;
    public float maxRadius = 1.2f;

    public float minAngleY = 0; 
    public float maxAngleY = 180;
    public float minAngleX = -90;
    public float maxAngleX = 90;

    public Vector3 centerRadius = Vector3.one; 

    public bool saveImages = true;
    public bool exitOnFinish = true; 

    public GameObject camPrefab;
    public Transform lookAtTarget;

    private GameObject cam; 
    private CustomRender render;

    public bool useDebugSphere = true; 
    public GameObject debugSphere; 

    private int frameCounter = 0; 

    // Start is called before the first frame update
    void Start()
    {
        cam = Instantiate(camPrefab);
        render = cam.GetComponentInChildren<CustomRender>();
    }

    // Update is called once per frame
    void Update()
    {
        //wait some frames to init
        if (Time.frameCount < RenderOptions.getInstance().startFrame)
        {
            return; 
        }
        //there should only be one of these objects in the scene, taking all the frames and then quitting
        if (frameCounter >= RenderOptions.getInstance().numFrames)
        {
            if(exitOnFinish)
            {
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            }
            return; 
        }


        float r = Random.Range(minRadius, maxRadius);
        float angX = Random.Range(minAngleX, maxAngleX);
        float angY = Random.Range(minAngleY, maxAngleY);

        cam.transform.rotation = transform.rotation;
        cam.transform.Rotate(angX, angY, 0f);
        cam.transform.position = transform.position - r * cam.transform.forward.normalized;

        if(lookAtTarget)
        {
            var p = lookAtTarget.position + Vector3.Scale(Random.insideUnitSphere, centerRadius); 

            cam.transform.LookAt(p); 
        }
        if(useDebugSphere)
        {
            GameObject g = Instantiate(debugSphere, cam.transform.position, cam.transform.rotation);
        }
        render.RenderImage(saveImages); 

        ++frameCounter; 
    }
}
