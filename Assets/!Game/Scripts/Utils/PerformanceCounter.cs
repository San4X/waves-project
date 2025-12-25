using System;
using TMPro;
using UnityEngine;

public class PerformanceCounter : MonoBehaviour
{
    private TextMeshProUGUI _fpsText;
    [SerializeField] private float updateCd;
    private float _updateTimer;
    private float _fpsAvg;
    private int _frameCounter;


    private void Awake()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        _fpsAvg += fps;
        _frameCounter++;
        
        if (_updateTimer < updateCd)
        {
            _updateTimer += Time.deltaTime;
            return;
        }
        _updateTimer = 0;
        
        _fpsText.text = "FPS " + (int)_fpsAvg / _frameCounter;
        _fpsAvg = 0;
        _frameCounter = 0;
    }
}
