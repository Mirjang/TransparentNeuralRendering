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
            case RenderOptions.TextureOutputMode.EXR:
                writeTexturesToEXR(rgb, uvs, masks, frameID);
                break;
            default:
                break;
        }

        Debug.Log("Wrote frame: " + frameID); 
    }

    private void writeTexturesToEXR(RenderTexture rgb, RenderTexture[] uvs, RenderTexture[] masks, int frameID)
    {
        writeTextureToEXR(rgb, "rgb", frameID);
        for (int i = 0; i < uvs.Length; ++i)
        {
            writeTextureToEXR(uvs[i], "uv_" + i, frameID);
        }
        for (int i = 0; i < masks.Length; ++i)
        {
            writeTextureToEXR(masks[i], "mask_" + i, frameID);
        }

        if (masks.Length != uvs.Length)
        {
            Debug.LogWarning("len(UVs) != len(masks) -- should provide per pixel per layer segmentation in multi-object scenes");
        }

    }


    private void writeTextureToEXR(RenderTexture rt, string name, int frameID)
    {
        Texture2D tex = getFloatTextureFormRenderTexture(rt);
        var blob = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat | RenderOptions.getInstance().exrCompression);
        string filename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_" + name + ".exr";

        File.WriteAllBytes(filename, blob);
        if(RenderOptions.getInstance().logOutputVerbose)
            Debug.Log("Wrote: " + filename);

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
        if (RenderOptions.getInstance().logOutputVerbose)
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

    private Texture2D getFloatTextureFormRenderTexture(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = old;
        return tex;
    }



    private void writeTexturesToBinary(RenderTexture rgb, RenderTexture[] uvs, RenderTexture[] masks, int frameID)
    {
        int H = Screen.height, W = Screen.width; 
        string rgbFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_rgb.bin";
        string uvFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_uv.bin";
        string maskFilename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + frameID + "_mask.bin";

        Texture2D texRGB = getTextureFormRenderTexture(rgb);
        var imgRGB = texRGB.GetRawTextureData();

        byte[] rgbOut = new byte[H * W * 3];
        for (int y = 0; y < H; ++y)
        {
            for (int x = 0; x < W; ++x)
            {
                Color pixel = texRGB.GetPixel(x, y);

                int index = (y * H + x)*3;
                rgbOut[index] = (byte)(pixel.r * 255);
                rgbOut[index+1] = (byte)(pixel.g * 255);
                rgbOut[index+2] = (byte)(pixel.b * 255);

            }
        }


        File.WriteAllBytes(rgbFilename, imgRGB);
        if (RenderOptions.getInstance().logOutputVerbose)
            Debug.Log("Wrote: " + rgbFilename);

        byte[] uvOut = new byte[H*W*2 * uvs.Length * 4];

        for(int layer = 0; layer < uvs.Length; ++layer)
        {
            var texUV = getTextureFormRenderTexture(uvs[layer]);
            for (int y = 0; y < H; ++y)
            {
                for (int x = 0; x < W; ++x)
                {
                    Color pixel = texUV.GetPixel(x, y);

                    int index = (layer * H * W + y * H + x) *8; //4... sizeof float, 2 for u and v coord
                    System.Buffer.BlockCopy(System.BitConverter.GetBytes(pixel.r), 0, uvOut, index, 4);
                    System.Buffer.BlockCopy(System.BitConverter.GetBytes(pixel.g), 0, uvOut, index + 4, 4);
                }
            }
        }

        File.WriteAllBytes(uvFilename, uvOut);
        if (RenderOptions.getInstance().logOutputVerbose)
            Debug.Log("Wrote: " + uvFilename);

        byte[] maskOut = new byte[H*W * 1 * uvs.Length *1];

        int numVisibleObjects = RenderOptions.getInstance().getNumVisibleObjects(); 
        for (int layer = 0; layer < masks.Length; ++layer)
        {
            var texMask = getTextureFormRenderTexture(masks[layer]);
            for (int y = 0; y < H; ++y)
            {
                for (int x = 0; x < W; ++x)
                {
                    Color pixel = texMask.GetPixel(x, y);

                    int index = (layer * H * W + y * H + x);
                    byte objectId = (byte) Mathf.RoundToInt(pixel.r * numVisibleObjects);
                    maskOut[index] = objectId; 
                }
            }
        }
        File.WriteAllBytes(maskFilename, maskOut);
        if (RenderOptions.getInstance().logOutputVerbose)
            Debug.Log("Wrote: " + maskFilename);
    }

}
