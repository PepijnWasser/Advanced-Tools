using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    public int m_CurrentFps;
    const string display = "{0} FPS";

    [SerializeField]
    private TMP_Text m_Text;

    public UnityEvent<List<int>> onStopCapturing;

    bool shouldCaptureFPS = false;

    float secondsToCapture;
    float secondCounter;

    List<int> fpsMomentsCaptured = new List<int>();

    [SerializeField]
    string pathName;

    private void Start()
    {

    }


    private void Update()
    {
        if (shouldCaptureFPS)
        {
            secondCounter += Time.deltaTime;
            if (secondCounter > secondsToCapture)
            {
                StopCapture();
            }
            else
            {
                CaptureFPS();
            }
        }
    }

    public void StartCapture(float _secondsToCapture)
    {
        secondsToCapture = _secondsToCapture;
        secondCounter = 0;
        fpsMomentsCaptured.Clear();
        shouldCaptureFPS = true;
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    void CaptureFPS()
    {
        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            m_Text.text = string.Format(display, m_CurrentFps);
            Debug.Log(m_CurrentFps);
            fpsMomentsCaptured.Add(m_CurrentFps);
        }
    }

    void StopCapture()
    {
        shouldCaptureFPS = false;
        onStopCapturing?.Invoke(fpsMomentsCaptured);
        SafeFPS(fpsMomentsCaptured);
    }

    void SafeFPS(List<int> _fpsMomentsCaptured)
    {
        string path = Application.dataPath + "/FPSCaptures/" + pathName + "/";
        string data = "";
        foreach(int moment in _fpsMomentsCaptured)
        {
            data = data + moment + "\n\n";
        }

        if(data != null && data != "")
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);

                DateTime currentDateTime = DateTime.Now;
                string currentDate = currentDateTime.Month + "_" + currentDateTime.Day + "_" + currentDateTime.Hour + "_" + currentDateTime.Minute + "_" + currentDateTime.Second;
                File.WriteAllText(path + currentDate + ".txt", data);
            }
        }
    }
}
