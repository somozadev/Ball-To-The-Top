using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int _minOrthoSize = 6;
    private int _maxOrthoSize = 12;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _noise;
    public Camera cameraRef;

    private void Awake()
    {
        cameraRef = Camera.main;
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void LerpZoomOut(float value)
    {
        _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(_minOrthoSize, _maxOrthoSize, value);
    }

    public void Shake()
    {
        StartCoroutine(ProcessShake(.25f, .25f, .15f));
    }

    private IEnumerator ProcessShake(float shakeIntensity = 1f, float frequencyGain = 1f, float shakeTiming = 0.5f)
    {
        Noise(shakeIntensity, frequencyGain);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }

    private void Noise(float amplitudeGain, float frequencyGain)
    {
        _noise.m_AmplitudeGain = amplitudeGain;
        _noise.m_FrequencyGain = frequencyGain;
    }

    public void LerpZoomIn()
    {
        if (Math.Abs(_virtualCamera.m_Lens.OrthographicSize - _minOrthoSize) > 0.02f)
            StartCoroutine(Lerp());
    }

    private IEnumerator Lerp()
    {
        var elapsed_time = 0f;
        var initialSize = _virtualCamera.m_Lens.OrthographicSize;
        while (elapsed_time <= 1f)
        {
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(initialSize, _minOrthoSize, elapsed_time / 1f);
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}