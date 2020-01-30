using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSkybox : MonoBehaviour
{
    public Material skybox; 
    // Start is called before the first frame update
    void Start()
    {
        addAndEnableSkybox(); 
    }


    void addAndEnableSkybox()
    {
        foreach (var cam in FindObjectsOfType<Camera>())
        {
            if (cam.gameObject.GetComponent<Skybox>() == null)
            {
                cam.gameObject.AddComponent<Skybox>();
                cam.gameObject.GetComponent<Skybox>().material = skybox;
            }
            else
                cam.GetComponent<Skybox>().enabled = true; 
            cam.clearFlags = CameraClearFlags.Skybox;

        }
    }

    void disableSkybox()
    {
        foreach (var cam in FindObjectsOfType<Camera>())
        {
            if (cam.gameObject.GetComponent<Skybox>())
            {
                cam.gameObject.GetComponent<Skybox>().enabled = false; 
                cam.clearFlags = CameraClearFlags.SolidColor;
            }
        }
    }

    //pointless overkill solution
    public static void setAllEnabled(bool enabled)
    {
        foreach (var script in FindObjectsOfType<AddSkybox>())
        {
            if (enabled)
                script.addAndEnableSkybox();
            else
                script.disableSkybox(); 
        }
    }

}
