using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

public class SampleImages : MonoBehaviour
{
    public float minRadius = .8f;
    public float maxRadius = 1.2f;

    public float minAngleY = 0; 
    public float maxAngleY = 180;
    public float minAngleX = -90;
    public float maxAngleX = 90;

    public Vector3 centerRadius = Vector3.one;
    public bool smoothPath = false;
    public float smoothSpeed = 50f;
    public float smoothYSteps = 15.0f; 
    public bool saveImages = true;

    public GameObject camPrefab;
    public Transform lookAtTarget;

    private GameObject cam; 
    private CustomRender render;

    public bool useDebugSphere = true; 
    public GameObject debugSphere; 

    private int frameCounter = 0; 

    private float smoothAngX, smoothAngY;
    private float smoothDirX = 1.0f, smoothDirY = 1.0f;
    private float smoothYStep = 0.0f; 
    private float smoothPathStartTime = 0.0f;
    private float goDirX = 1.0f; 
    // Start is called before the first frame update
    void Start()
    {

        cam = Instantiate(camPrefab);
        render = cam.GetComponentInChildren<CustomRender>();
        
        smoothAngX = minAngleX;
        smoothAngY = minAngleY;
        smoothPathStartTime = Time.time; 
    }

    // Update is called once per frame
    void Update()
    {
        //wait some frames to init
        if (RenderOptions.getInstance().framesSinceStart < RenderOptions.getInstance().startFrame)
        {
            return; 
        }
        //there should only be one of these objects in the scene, taking all the frames and then quitting
        if (frameCounter >= RenderOptions.getInstance().numFrames)
        {
            RenderOptions.getInstance().OnSceneFinish(); 
            return;
        }

        float r, angX, angY;
        if (smoothPath)
        {
            r = (minRadius + maxRadius) / 2.0f;
            angX = smoothAngX;
            angY = smoothAngY;
            smoothAngY += smoothSpeed * smoothDirX * goDirX;
            if ((smoothAngY > maxAngleY || smoothAngY < minAngleY) && goDirX > 0)
            {
                smoothAngY = Mathf.Clamp(smoothAngY, minAngleY, maxAngleY);
                smoothDirX *= -1.0f;
                goDirX = 0.0f;
            }

            if (goDirX < 1)
            {                         
                float yStep = smoothSpeed * smoothDirY;
                smoothYStep += yStep; 
                if(smoothYStep > smoothYSteps)
                {
                    yStep = smoothYStep - smoothYSteps;
                    smoothYStep = 0;
                    goDirX = 1.0f; 
                }
                smoothAngX += yStep; 

            }

            if(smoothAngX > maxAngleX || smoothAngY > maxAngleY)
            {
                RenderOptions.getInstance().numFrames = frameCounter;
                return; 
            }

        }
        else
        {
            r = Random.Range(minRadius, maxRadius);
            angX = Random.Range(minAngleX, maxAngleX);
            angY = Random.Range(minAngleY, maxAngleY);
        }





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
