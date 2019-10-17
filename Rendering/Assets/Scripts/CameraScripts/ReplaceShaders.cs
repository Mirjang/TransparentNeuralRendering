using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceShaders : MonoBehaviour
{

    public Shader UV_Peel; 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Camera c in GetComponentsInChildren<Camera>())
        {
            c.SetReplacementShader(UV_Peel, "");
        }
    }

}
