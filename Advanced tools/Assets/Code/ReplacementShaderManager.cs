using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacementShaderManager : MonoBehaviour
{
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.SetReplacementShader(Shader.Find("Custom/DepthShader"), "");
    }

    
}
