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


public class RenderTextureToTexture2dConverter : MonoBehaviour
{
    public RenderTexture rgbRenderTexture;
    public RenderTexture depthRenderTexture;

    private Texture2D rgbTexture;
    private Texture2D depthTexture;

    public GameObject rgbDebugObject;
    public GameObject depthDebugObject;

    public CopyMode copyMode;
    float secondCounter = 0;

    private void Start()
    {
        rgbTexture = new Texture2D(rgbRenderTexture.width, rgbRenderTexture.height);
        depthTexture = new Texture2D(depthRenderTexture.width, depthRenderTexture.height);

        rgbDebugObject.GetComponent<Renderer>().material.SetTexture("_MainTex", rgbTexture);
        depthDebugObject.GetComponent<Renderer>().material.SetTexture("_MainTex", depthTexture);
    }

    private void Update()
    {
        if(copyMode == CopyMode.readPixels)
        {
            CopyRenderTextureToTexture2DReadPixels(rgbRenderTexture, rgbTexture);
            CopyRenderTextureToTexture2DReadPixels(depthRenderTexture, depthTexture);
        }
        else
        {
            secondCounter += Time.deltaTime;
            if (secondCounter > 2)
            {
                secondCounter = 0;

                CopyRenderTextureToTexture2DAsync(rgbRenderTexture, rgbTexture);
                CopyRenderTextureToTexture2DAsync(depthRenderTexture, depthTexture);
            }
        }
    }

    void CopyRenderTextureToTexture2DReadPixels(RenderTexture rt, Texture2D texture)
    {
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
    }

    async Task CopyRenderTextureToTexture2DAsync(RenderTexture rt, Texture2D texture)
    {

        AsyncGPUReadback.Request(rt, 0, (AsyncGPUReadbackRequest asyncAction) =>
        {
            texture.SetPixelData(asyncAction.GetData<byte>(), 0);
            texture.Apply();
        });
    }

   public enum CopyMode
   {
        readPixels,
        gpuAsyncCallback,
   }

    private struct EncodeImageJob : IJob
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<uint> Input;

        public uint Width;
        public uint Height;
        public int Quality;

        public NativeList<byte> Output;

        public unsafe void Execute()
        {
            NativeArray<byte> temp = ImageConversion.EncodeNativeArrayToJPG(
                Input, GraphicsFormat.R8G8B8_UNorm, Width, Height, 0, Quality);

            Output.Resize(temp.Length, NativeArrayOptions.UninitializedMemory);

            void* internalPtr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(temp);
            void* outputPtr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(Output);
            UnsafeUtility.MemCpy(outputPtr, internalPtr, temp.Length * UnsafeUtility.SizeOf<byte>());

            temp.Dispose();
        }
    }
}
