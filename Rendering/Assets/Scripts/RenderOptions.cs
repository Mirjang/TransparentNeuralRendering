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
    public enum ActionOnFinish { None, Exit, LoadScene };

    public ActionOnFinish actionOnFinish = ActionOnFinish.Exit;

    public int numDepthPeelLayers = 8;
    public int startDepthLayer = 0;

    public string experiment_name = "";
    public string phase_name = ""; 
    public string outputDir = "";
    public bool isTrainSet = true; 
    public bool logOutputVerbose = true; 

    public bool renderRGBUnity = true;
    public bool renderTransparent = true;
    public bool renderWorldPos = true; 
    public bool deleteDirIfExists = false;
    public int maxActiveWriteThreads = 1; 
    public int framesSinceStart = 0; 
    public Material default_t;
    public bool savePoses = true; 
    private int numVisibleObjects;

    private List<Renderer> visibleObjects = new List<Renderer>(); 

    public int camerIDCounter = 0;
    public int frameIdCounter = 0; 
    // Start is called before the first frame update
    void Awake()
    {
        if(experiment_name == "")
        {
            experiment_name = SceneManager.GetActiveScene().name + "_" + System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"); 
        }
        if (phase_name == "")
        {
            phase_name = (isTrainSet ? "train" : "test");
        }
        if (outputDir == "")
        {

            if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                outputDir = Application.dataPath + "/mnt/raid/patrickradner/datasets/" + experiment_name + "/" + phase_name + "/";
            }
            else
            {                
                //outputDir = Application.dataPath + "/../../Datasets/" + experiment_name + "/" + (isTrainSet ? "train/" : "test/");
                outputDir = "D:\\datasets/" + experiment_name + "/" + phase_name + "/";
            }
        }
        else
        {
            outputDir = outputDir + "/" + experiment_name + "/" + phase_name + "/";
        }
        parseOptionsFromFile();

        if(!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
        {
            Debug.LogError("Scalar Int render target format not supported!"); 
        }
        if(SystemInfo.supportedRenderTargetCount < 5)
        {
            Debug.LogError("System cannot support 5 simultaneous render targets!"); 
        }
        if(SystemInfo.graphicsShaderLevel < 50)
        {
            Debug.LogError("System only supports shader level: " + SystemInfo.graphicsShaderLevel); 
        }

        Debug.Log(SceneManager.GetActiveScene().name); 
        Debug.Log("Output Dir: " + outputDir);
        if (System.IO.Directory.Exists(outputDir))
        {
            var pose_file = outputDir + "camera_pose.txt";

            if (deleteDirIfExists || !System.IO.File.Exists(pose_file))
            {
                System.IO.Directory.Delete(outputDir, true);
                System.IO.Directory.CreateDirectory(outputDir);

            }
            else if (GameObject.FindObjectOfType<RenderFromFileMultiProcessing>() != null)
            {
            }
            else
            {
                var prevPoses = System.IO.File.ReadAllLines(pose_file);
                int nPrevFrames = 0; 

                foreach(var line in prevPoses)
                {
                    if (line.Split(' ').Length >= 6)
                        ++nPrevFrames; 
                }
                frameIdCounter = nPrevFrames;
                framesSinceStart = nPrevFrames;

                Debug.Log("Continuing exitsing dataset: " + nPrevFrames + "/" + numFrames); 
            }

        }
        else
        {
            System.IO.Directory.CreateDirectory(outputDir);
        }

        assignIDtoObjects(); 
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        ++framesSinceStart; 
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

    //private void OnDestroy()
    //{
    //    OutputManager.getInstance().flushAll();     
    //}

    private void assignIDtoObjects(bool continueDataset = false)
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
        visibleObjects.Clear(); 
        foreach (var name in names)
        {
            var visibleObject = dict[name];
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            if (visibleObject.HasPropertyBlock())
                visibleObject.GetPropertyBlock(prop);
            prop.SetInt("_ObjectID", id);
            visibleObject.SetPropertyBlock(prop);
            visibleObjects.Add(visibleObject); 
            ++id;
        }

        numVisibleObjects = id - 1;
        Shader.SetGlobalInt("_MaxVisObjects", numVisibleObjects);
        Shader.SetGlobalInt("WorldPosTextureRes", Screen.width);

        names.Insert(0, "none"); 

        if(continueDataset)
        {
            var prev_names = System.IO.File.ReadAllLines(outputDir + "object_names.txt");
            Debug.Assert(prev_names.Length >= name.Length);
            int i = 0; 
            foreach(var line in names)
            {
                Debug.Assert(line.Equals(names[i]));
                ++i; 
            }
        }
        else
        {
            System.IO.File.WriteAllLines(outputDir + "object_names.txt", names);
        }

    }

    public int getNumVisibleObjects()
    {
        return numVisibleObjects; 
    }

    public List<Renderer> getVisibleObjects()
    {
        return visibleObjects; 
    }

    private static RenderOptions instance = null;
    public static RenderOptions getInstance()
    {
        return instance;
    }


    public void OnSceneFinish()
    {
      //  OutputManager.getInstance().flushAll();


        if (actionOnFinish == ActionOnFinish.Exit)
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        if (actionOnFinish == ActionOnFinish.LoadScene)
        {
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            }
        }

    }

}
