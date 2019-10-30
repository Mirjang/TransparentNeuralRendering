using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMultiView : MonoBehaviour
{



    public int setupTimeFrames = 3;
    private int frameCtr = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(frameCtr == setupTimeFrames)
        {
            foreach(var cr in GameObject.FindObjectsOfType<CustomRender>())
            {
                cr.RenderImage(true); 
            }
        }

        ++frameCtr; 
    }
}
