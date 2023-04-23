using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FPSCounter))]
public class FPSCounterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FPSCounter fpsCounter = (FPSCounter)target;
        if (GUILayout.Button("StartCapturing"))
        {
            fpsCounter.StartCapture(20);
        }
    }
}
