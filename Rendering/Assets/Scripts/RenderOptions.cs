using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

//Singleton storing all the options 
public class RenderOptions : MonoBehaviour
{
    public enum TextureOutputMode { DontWrite, PNG, EXR, Binary};

    public TextureOutputMode textureOutputMode = TextureOutputMode.PNG;

    public Texture2D.EXRFlags exrCompression = Texture2D.EXRFlags.CompressZIP;

    public bool useCustomGlobalAlpha = true;
    public float globalAlpha = .5f;
    
    public int numFrames = 1;
    public int startFrame = 2;

    public int numDepthPeelLayers = 8;

    public string experiment_name = ""; 

    public string outputDir = "";
    public bool isTrainSet = true; 
    public bool logOutputVerbose = true; 

    public bool renderRGBUnity = true;
    public bool renderUVOpaque = false;
    public bool renderTransparent = true;


    public Material default_t;

    private int numVisibleObjects; 

    public int camerIDCounter = 0;
    public int frameIdCounter = 0; 
    // Start is called before the first frame update
    void Awake()
    {
        if(experiment_name == "")
        {
            experiment_name = SceneManager.GetActiveScene().name + "_" + System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"); 
        }
        if (outputDir == "")
        {
            outputDir = Application.dataPath + "/../../Datasets/" + experiment_name + "/" + (isTrainSet?"train/":"test/");
        }
        else
        {
            outputDir = outputDir + "/" + experiment_name + "/" + (isTrainSet ? "train/" : "test/"); 
        }
        parseOptionsFromFile();

        if(!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
        {
            Debug.LogError("Scalar Int render target format not supported!"); 
        }
        if(SystemInfo.supportedRenderTargetCount < 4)
        {
            Debug.LogError("System cannot support 4 simultaneous render targets!"); 
        }


        Debug.Log("Output Dir: " + outputDir);
        if (System.IO.Directory.Exists(outputDir))
        {
            System.IO.Directory.Delete(outputDir, true);

        }
        System.IO.Directory.CreateDirectory(outputDir);

        assignIDtoObjects(); 
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

    public int getIncrementaFrameId()
    {
        return frameIdCounter++;
    }

    //TODO
    private void parseOptionsFromFile()
    {

    }


    private void assignIDtoObjects()
    {

        //disable unnecessary renderers and enable proxy renderer if present
        DisableRenderers.disableAllRenderers();

        Dictionary<string, Renderer> dict = new Dictionary<string, Renderer>(); 

        foreach (var visibleObject in FindObjectsOfType<Renderer>())
        {
            if (visibleObject.enabled && visibleObject.gameObject.activeSelf && visibleObject.gameObject.layer <4) // layer >=4 -> ui elements/indicator spheres
            {
                dict.Add(visibleObject.gameObject.name, visibleObject); 
            }
        }


        List<string> names = new List<string>(dict.Keys);
        names.Sort();
        int id = 1;

        foreach (var name in names)
        {
            var visibleObject = dict[name];
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            if (visibleObject.HasPropertyBlock())
                visibleObject.GetPropertyBlock(prop);
            prop.SetInt("_ObjectID", id);
            visibleObject.SetPropertyBlock(prop);
            ++id;
        }

        numVisibleObjects = id - 1;
        Shader.SetGlobalInt("_MaxVisObjects", numVisibleObjects);

        names.Insert(0, "none"); 
        System.IO.File.WriteAllLines(outputDir + "object_names.txt", names); 

    }

    public int getNumVisibleObjects()
    {
        return numVisibleObjects; 
    }

    private static RenderOptions instance = null;
    public static RenderOptions getInstance()
    {
        return instance;
    }

}
