using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ReplacementShaderManager : MonoBehaviour
{
    Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.SetReplacementShader(Shader.Find("Custom/ReplacementDepth"), "");
    }  
}
