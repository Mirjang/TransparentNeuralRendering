using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageOutput : MonoBehaviour
{
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
}
