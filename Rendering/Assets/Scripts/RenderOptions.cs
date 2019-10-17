using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton storing all the options 
public class RenderOptions : MonoBehaviour
{


    public bool useCustomGlobalAlpha = true;
    public float globalAlha = .5f;

    public int numFrames = 1;
    public int startFrame = 2;

    public int numDepthPeelLayers = 8; 


    public string outputDir = "";

    public bool renderRGBOpaque = true;
    public bool renderUVOpaque = true;
    public bool renderTransparent = true;

    public Material default_t;

    private int camerIDCounter = 0; 
    // Start is called before the first frame update
    void Awake()
    {

        if (outputDir == "")
            outputDir = Application.dataPath + "/../";
        parseOptionsFromFile();

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getIncrementalCameraId()
    {
        return camerIDCounter++; 
    }

    //TODO
    private void parseOptionsFromFile()
    {

    }

    private static RenderOptions instance = null;
    public static RenderOptions getInstance()
    {
        return instance;
    }

}
