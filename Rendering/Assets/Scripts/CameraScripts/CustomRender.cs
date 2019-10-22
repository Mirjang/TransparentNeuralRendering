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
    public Shader initShader; 


    public Renderer display = null;

    private RenderTexture opaqueTexture = null;
    private RenderTexture[] depthPeelBuffers = new RenderTexture[2]; // use prev depth buffer as mask for next depth peeling pass
    private Material blendMat = null;

    private int cameraID = -1;
    private int frameID = 0; 

    private new Camera camera; 
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        cameraID = RenderOptions.getInstance().getIncrementalCameraId(); 
        blendMat = new Material(blendShader);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount >= RenderOptions.getInstance().startFrame && Time.frameCount < RenderOptions.getInstance().startFrame + RenderOptions.getInstance().numFrames)
        {
            RenderImage(); 
        }
    }

    public void RenderImage()
    {
        if (RenderOptions.getInstance().renderRGBUnity)
        {
            RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            camera.targetTexture = rt;
            camera.Render();
            writeRenderTextureToFile(rt, "rgb");
        }
        if (RenderOptions.getInstance().renderUVOpaque)
        {
            RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            camera.targetTexture = rt;
            camera.RenderWithShader(uvShader, "");
            writeRenderTextureToFile(rt, "uv");
        }
        if (RenderOptions.getInstance().renderTransparent)
        {
            RenderTexture[] colorBuffers = new RenderTexture[RenderOptions.getInstance().numDepthPeelLayers];
            RenderTexture[] uvBuffers = new RenderTexture[RenderOptions.getInstance().numDepthPeelLayers];

            colorBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            uvBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            depthPeelBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            depthPeelBuffers[1] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            RenderTexture depthBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            colorBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            uvBuffers[0] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.clearFlags = CameraClearFlags.Color;

            RenderBuffer[] renderTargets = new RenderBuffer[3]; // rgb, uv and depth


            //innital pass
            //renderTargets[0] = colorBuffers[0].colorBuffer;
            //renderTargets[1] = uvBuffers[0].colorBuffer;
            //renderTargets[2] = depthPeelBuffers[0].colorBuffer;
            //camera.SetTargetBuffers(renderTargets, depthBuffer.depthBuffer);
            //camera.RenderWithShader(initShader, null);


            for (int i = 0; i < RenderOptions.getInstance().numDepthPeelLayers; i++)
            {
                colorBuffers[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                uvBuffers[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                renderTargets[0] = colorBuffers[i].colorBuffer;
                renderTargets[1] = uvBuffers[i].colorBuffer;
                renderTargets[2] = depthPeelBuffers[i % 2].colorBuffer;
                camera.SetTargetBuffers(renderTargets, depthBuffer.depthBuffer);
                Shader.SetGlobalTexture("_PrevDepthTex", depthPeelBuffers[1 - i % 2]);
                camera.RenderWithShader(depthPeelShader, null);


                //   writeRenderTextureToFile(depthPeelBuffers[i % 2], "d_" + (i));
            }

            RenderTexture finalImage = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);


            //Blending
            for (int i = RenderOptions.getInstance().numDepthPeelLayers - 1; i >= 0; i--)
            {
                RenderTexture tmpImage = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                blendMat.SetTexture("_LayerTex", colorBuffers[i]);
                Graphics.Blit(finalImage, tmpImage, blendMat, 1);
                finalImage = tmpImage;
            }


            writeRenderTextureToFile(finalImage, "tRGB");


            RenderTexture.ReleaseTemporary(finalImage);
            RenderTexture.ReleaseTemporary(depthBuffer);
            RenderTexture.ReleaseTemporary(depthPeelBuffers[0]);
            RenderTexture.ReleaseTemporary(depthPeelBuffers[1]);
            for (int i = 0; i < RenderOptions.getInstance().numDepthPeelLayers; i++)
            {
                writeRenderTextureToFile(uvBuffers[i], "uvp_" + i);


                RenderTexture.ReleaseTemporary(colorBuffers[i]);
                RenderTexture.ReleaseTemporary(uvBuffers[i]);
            }

        }
        ++frameID;

    }

    private void writeRenderTextureToFile(RenderTexture rt, string name)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture old = RenderTexture.active; 
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply(); 
        RenderTexture.active = old; 
        
        var blob = tex.EncodeToPNG();
        string filename = RenderOptions.getInstance().outputDir + cameraID.ToString() + "_" + frameID + "_" + name + ".png"; 
        File.WriteAllBytes(filename, blob);
        Debug.Log("Wrote: " + filename);
    }


}
