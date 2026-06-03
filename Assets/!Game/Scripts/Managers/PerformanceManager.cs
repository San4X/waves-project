using System;
using TMPro;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateCd;
    [SerializeField] private int targetFps;
    private float _updateTimer;
    private float _fpsAvg;
    private int _frameCounter;
    
    private void Awake()
    {
        Application.targetFrameRate = targetFps;
    }
    
    private void Update()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        _fpsAvg += fps;
        _frameCounter++;
        
        if (_updateTimer < updateCd)
        {
            _updateTimer += Time.unscaledDeltaTime;
            return;
        }
        _updateTimer = 0;
        
        fpsText.text = "FPS " + (int)_fpsAvg / _frameCounter;
        _fpsAvg = 0;
        _frameCounter = 0;
    }
}
