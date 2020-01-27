using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanObjectsLocal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(var r in RenderOptions.getInstance().getVisibleObjects())
        {
            r.gameObject.transform.position = Vector3.zero;
            r.gameObject.transform.rotation = Quaternion.identity; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RenderOptions.getInstance().framesSinceStart < RenderOptions.getInstance().startFrame)
        {
            return;
        }
        else if (RenderOptions.getInstance().framesSinceStart - RenderOptions.getInstance().startFrame >= RenderOptions.getInstance().numFrames)
        {
            RenderOptions.getInstance().OnSceneFinish();
            return;
        }

    }
}
