using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

public enum CopyMode
{
    readPixels,
    gpuAsyncCallbackAsync,
    gpuAsyncCallbackSync
}

public class RenderTextureToTexture2dConverter : MonoBehaviour
{
    [SerializeField]
    private RenderTexture rgbRenderTexture;
    [SerializeField]
    private RenderTexture depthRenderTexture;

    private Texture2D rgbTexture;
    private Texture2D depthTexture;

    [SerializeField]
    private GameObject rgbDisplay;
    [SerializeField]
    private GameObject depthDisplay;

    [SerializeField]
    private CopyMode copyMode;

    private void Start()
    {
        rgbTexture = new Texture2D(rgbRenderTexture.width, rgbRenderTexture.height);
        depthTexture = new Texture2D(depthRenderTexture.width, depthRenderTexture.height);

        rgbDisplay.GetComponent<Renderer>().material.SetTexture("_MainTex", rgbTexture);
        depthDisplay.GetComponent<Renderer>().material.SetTexture("_MainTex", depthTexture);
    }

    private void Update()
    {
        if(copyMode == CopyMode.readPixels)
        {
            CopyRTToT2DReadPixels(rgbRenderTexture, rgbTexture);
            CopyRTToT2DReadPixels(depthRenderTexture, depthTexture);

            rgbTexture.name = "RGBReadpixel";
            depthTexture.name = "DepthReadpixel";
        }
        else if(copyMode == CopyMode.gpuAsyncCallbackAsync)
        {
            CopyRTToT2DGPUAsyncCallbackAsync(rgbRenderTexture, rgbTexture);
            CopyRTToT2DGPUAsyncCallbackAsync(depthRenderTexture, depthTexture);

            rgbTexture.name = "RGBCallbackAsync";
            depthTexture.name = "DepthCallbackAsync";
        }
        else if (copyMode == CopyMode.gpuAsyncCallbackSync)
        {
            CopyRTToT2DGPUAsyncCallbackSync(rgbRenderTexture, rgbTexture);
            CopyRTToT2DGPUAsyncCallbackSync(depthRenderTexture, depthTexture);

            rgbTexture.name = "RGBCallbackSync";
            depthTexture.name = "DepthCallbackSync";
        }
    }

    void CopyRTToT2DReadPixels(RenderTexture rt, Texture2D texture)
    {
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
    }

    void CopyRTToT2DGPUAsyncCallbackSync(RenderTexture rt, Texture2D texture)
    {
        AsyncGPUReadbackRequest asyncAction = AsyncGPUReadback.Request(rt, 0);
        asyncAction.WaitForCompletion();
        if (asyncAction.hasError)
        {
            Debug.Log("Error loading data");
        }
        else
        {
            texture.SetPixelData(asyncAction.GetData<byte>(), 0);
            texture.Apply();
        }
    }

    void CopyRTToT2DGPUAsyncCallbackAsync(RenderTexture rt, Texture2D texture)
    {
        AsyncGPUReadback.Request(rt, 0, (AsyncGPUReadbackRequest asyncAction) =>
        {
            if (asyncAction.hasError)
            {
                Debug.Log("Error loading data");
            }
            else
            {
                texture.SetPixelData(asyncAction.GetData<byte>(), 0);
                texture.Apply();
            }
        });
    }
}
