using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitManager : MonoBehaviour
{
    Camera camera;
    public Material depthMaterial;

    public RenderTexture depthTexture;
    public RenderTexture rgbTexture;


    void Start()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, depthTexture, depthMaterial);
        Graphics.Blit(source, rgbTexture);
        Graphics.Blit(source, dest);
    }
}
