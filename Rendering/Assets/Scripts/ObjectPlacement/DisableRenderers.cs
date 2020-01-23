using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DisableRenderers : MonoBehaviour
{

    public bool disableSelf = false;
    public bool useBoundingBox = false; 

    public GameObject proxy;

   

    private void Awake()
    {
        if(useBoundingBox)
        {
            if (GetComponent<BoxCollider>() == null)
                gameObject.AddComponent<BoxCollider>(); 

            Debug.Assert(proxy == null);
            proxy = Instantiate(FindObjectOfType<CustomRender>().boxProxy, transform.position, transform.rotation, transform);
            proxy.GetComponent<ProxyCube>().SetDims(GetComponent<BoxCollider>().bounds.extents);
            proxy.transform.localScale *= 0.25f;
            Debug.Log(GetComponent<BoxCollider>().bounds.extents);
        }


        if (proxy)
        {
            proxy.name = gameObject.name + "_proxy"; 
            disableSelf = true;
            proxy.SetActive(true);
            proxy.GetComponent<Renderer>().enabled = true;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (disableSelf && GetComponent<Renderer>())
        {
            GetComponent<Renderer>().enabled = true;
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }

        if (proxy)
        {
            proxy.GetComponent<Renderer>().enabled = false;
        }

    }

    void disableRenderers()
    {
        
        if (disableSelf && GetComponent<Renderer>())
        {
            GetComponent<Renderer>().enabled = false;
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false; 
        }

        if (!disableSelf && GetComponent<Renderer>()) //GetComponentsInChildren also returns self?  
        {
            GetComponent<Renderer>().enabled = true;
        }

        if (proxy)
        {
            proxy.GetComponent<Renderer>().enabled = true;
        }

    }


    public static void disableAllRenderers()
    {
        foreach (DisableRenderers script in FindObjectsOfType<DisableRenderers>())
        {
            if(script.enabled)
                script.disableRenderers();
        }
    }

}
