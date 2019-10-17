using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    public float alpha = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

        foreach( var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;

            Material mat = new Material(RenderOptions.getInstance().default_t);
            changeAlpha(mat, alpha);
            mat.SetTexture("_MainTex", renderer.material.GetTexture("_MainTex"));
            renderer.material = mat; 
            renderer.enabled = true; 
        }

        
    }


    void changeAlpha(Material mat, float alpha)
    {
        var c = mat.color;
        c.a = alpha;
        mat.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
