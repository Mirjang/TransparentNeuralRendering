using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TextureDisplay : MonoBehaviour
{

    public RawImage[] images;

    private RenderTexture[] textures; 

    // Start is called before the first frame update
    void Start()
    {
        textures = new RenderTexture[images.Length]; 
        for (int i = 0; i< images.Length; ++i)
        {
            textures[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            images[i].texture = textures[i]; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void drawImage(int slot, Texture tex)
    {
        if (slot >= textures.Length)
            return;
        Graphics.CopyTexture(tex, textures[slot]); 
    }

}
