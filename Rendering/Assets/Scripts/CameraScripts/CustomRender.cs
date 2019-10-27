using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering; 

[RequireComponent(typeof(Camera))]
public class CustomRender : MonoBehaviour
{
    public Shader uvShader;
    public Shader depthPeelShader;
    public Shader blendShader;


    public TextureDisplay display = null;

    private RenderTexture opaqueTexture = null;
    private RenderTexture[] depthPeelBuffers = new RenderTexture[2]; // use prev depth buffer as mask for next depth peeling pass
    private Material blendMat = null;
    private Material copyMat = null;

    private int cameraID = -1;

    private  Camera cam; 
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cameraID = RenderOptions.getInstance().getIncrementalCameraId(); 
        blendMat = new Material(blendShader);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.frameCount >= RenderOptions.getInstance().startFrame && Time.frameCount < RenderOptions.getInstance().startFrame + RenderOptions.getInstance().numFrames)
        //{
        //    RenderImage(); 
        //}
        //if(display)
        //{
        //    RenderImage(false); 
        //}
    }

    public void RenderImage(bool write = true)
    {
        RenderTexture rgb = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

        if (RenderOptions.getInstance().renderRGBUnity)
        {
            RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            cam.targetTexture = rgb;
            cam.backgroundColor = new Color(0, 0, 0, 0);

            cam.Render();

        }
        //if (RenderOptions.getInstance().renderUVOpaque)
        //{
        //    RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        //    camera.targetTexture = rt;
        //    camera.RenderWithShader(uvShader, "");
        //    if (write)
        //        writeTextureToPng(rt, "uv");
        //    rt.Release();
        //}
        if (RenderOptions.getInstance().renderTransparent)
        {
            RenderTexture[] colorBuffers = new RenderTexture[RenderOptions.getInstance().numDepthPeelLayers];
            RenderTexture[] uvBuffers = new RenderTexture[RenderOptions.getInstance().numDepthPeelLayers];
            RenderTexture[] maskBuffers = new RenderTexture[RenderOptions.getInstance().numDepthPeelLayers];

            colorBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            uvBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            depthPeelBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            depthPeelBuffers[1] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            RenderTexture depthBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            colorBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            uvBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RenderBuffer[] renderTargets = new RenderBuffer[4]; // rgb, uv, mask and depth

            cam.backgroundColor = new Color(1, 1, 1, 1); 

            for (int i = 0; i < RenderOptions.getInstance().numDepthPeelLayers; i++)
            {
                colorBuffers[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                uvBuffers[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                maskBuffers[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

                renderTargets[0] = colorBuffers[i].colorBuffer;
                renderTargets[1] = uvBuffers[i].colorBuffer;
                renderTargets[2] = maskBuffers[i].colorBuffer; 
                renderTargets[3] = depthPeelBuffers[i%2].colorBuffer;
                cam.SetTargetBuffers(renderTargets, depthBuffer.depthBuffer);
                Shader.SetGlobalTexture("_PrevDepthTex", depthPeelBuffers[1-i%2]);
                cam.RenderWithShader(depthPeelShader, null);
            }

            ////Blending
            //for (int i = RenderOptions.getInstance().numDepthPeelLayers - 1; i >= 0; i--)
            //{
            //    RenderTexture tmpImage = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            //    blendMat.SetTexture("_LayerTex", colorBuffers[i]);
            //    Graphics.Blit(finalImage, tmpImage, blendMat, 1);
            //    rgb = tmpImage;
            //}


            RenderTexture.ReleaseTemporary(depthBuffer);
            RenderTexture.ReleaseTemporary(depthPeelBuffers[0]);
            RenderTexture.ReleaseTemporary(depthPeelBuffers[1]);

            if (write)
            {
                outputTextures(rgb, uvBuffers, maskBuffers);
            }

            for (int i = 0; i < RenderOptions.getInstance().numDepthPeelLayers; i++)
            {
                if (display)
                {
                    display.drawImage(i, uvBuffers[i]); 
                }
                RenderTexture.ReleaseTemporary(colorBuffers[i]);
                RenderTexture.ReleaseTemporary(uvBuffers[i]);
                RenderTexture.ReleaseTemporary(maskBuffers[i]);

            }
        }

        rgb.Release(); 

    }

    private void outputTextures(RenderTexture rgb, RenderTexture[] uvs, RenderTexture[] masks)
    {
        int frameID = RenderOptions.getInstance().getIncrementaFrameId(); 

        switch (RenderOptions.getInstance().textureOutputMode)
        {
            case RenderOptions.TextureOutputMode.PNG:
                writeTexturesToPng(rgb, uvs, masks, frameID); 
                break;
            case RenderOptions.TextureOutputMode.Binary:
                writeTexturesToBinary(rgb, uvs, masks, frameID); 
                break;
            default:
                break;
        }
    }


    private void writeTexturesToPng(RenderTexture rgb, RenderTexture[] uvs, RenderTexture[] masks, int frameID)
    {
        writeTextureToPng(rgb, "rgb", frameID); 
        for(int i = 0; i< uvs.Length; ++i )
        {
            writeTextureToPng(uvs[i], "uv_" + i, frameID); 
        }
        for (int i = 0; i < masks.Length; ++i)
        {
            writeTextureToPng(masks[i], "mask_" + i, frameID);
        }

        if (masks.Length != uvs.Length)
        {
            Debug.LogWarning("len(UVs) != len(masks) -- should provide per pixel per layer segmentation in multi-object scenes"); 
        }

    }


    private void writeTextureToPng(RenderTexture rt, string name, int frameID)
    {
        Texture2D tex = getTextureFormRenderTexture(rt); 
        var blob = tex.EncodeToPNG();
        string filename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_" + name + ".png";

        File.WriteAllBytes(filename, blob);
        Debug.Log("Wrote: " + filename);

    }

    private Texture2D getTextureFormRenderTexture(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = old;
        return tex; 
    }

    private void writeTexturesToBinary(RenderTexture rgb, RenderTexture[] uvs, RenderTexture[] masks, int frameID)
    {
        string rgbFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_rgb.png";
        string uvFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_uv.png";
        string maskFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_mask.png";

        Texture2D texRGB = getTextureFormRenderTexture(rgb);
        


        //Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        //RenderTexture old = RenderTexture.active;
        //RenderTexture.active = rt;
        //tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        //tex.Apply();
        //RenderTexture.active = old;

        //var blob = tex.EncodeToPNG();

        //File.WriteAllBytes(filename, blob);
       // Debug.Log("Wrote: " + filename);
    }

}
