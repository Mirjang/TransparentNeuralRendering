using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class RenderFromFile : MonoBehaviour
{
    public TextAsset file;

    public GameObject camPrefab;

    private GameObject cam;
    private CustomRender render;

    public string[] poses;
    public int frameCounter = 0;


    public enum ActionOnFinish { None, Exit, LoadScene };

    public ActionOnFinish actionOnFinish = ActionOnFinish.Exit;
    public bool saveImages = true;

    // Start is called before the first frame update
    void Start()
    {

        poses = file.text.Split('\n');
        cam = Instantiate(camPrefab);
        render = cam.GetComponentInChildren<CustomRender>();
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
        if (frameCounter >= poses.Length)
        {
            RenderOptions.getInstance().OnSceneFinish();         
            return;
        }
        else
        {

            string[] lineElems = poses[frameCounter].Split(' ');

            float[] elems = new float[6];

            bool success = true; 
            int i = 0; 
            foreach(var s in lineElems)
            {
                success &= float.TryParse(s, out elems[i++]);
                if (i > 6) break; 
            }

            success &= i == 6; 

            if (success)
            {
                Vector3 pos = new Vector3(elems[0], elems[1], elems[2]);
                Vector3 fwd = new Vector3(elems[3], elems[4], elems[5]);
                cam.transform.position = pos;
                cam.transform.forward = fwd;
                render.RenderImage(saveImages);
            }

        }
        ++frameCounter;

    }


}
