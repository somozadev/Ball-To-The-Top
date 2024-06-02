using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GetHeightController : MonoBehaviour
{
    private float _initialHeightPosY;
    [SerializeField] private Transform targetHeightTracked;
    [SerializeField] private float heightMultiplier = 10;
    public float targetHeight;

    [SerializeField] private int AdTriggerDistance = 30;

    [SerializeField] private float _lastNFrameHeight;
    private float _elapsedTime;
    private float _checkWaitTime = 1.5f;

    private void Start()
    {
        if (!targetHeightTracked) Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnAdEnded += RestoreDueAdReward;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnAdEnded -= RestoreDueAdReward;
    }

    private void Update()
    {
        targetHeight = Mathf.RoundToInt(targetHeightTracked.position.y * heightMultiplier);
        if (targetHeight < 0) targetHeight = 0f; 
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _checkWaitTime)
        {
            CheckHeightDiff();
            _elapsedTime = 0f;
            _lastNFrameHeight = targetHeight;
        }
    }

    private void CheckHeightDiff()
    {
        if (_lastNFrameHeight - targetHeight >= AdTriggerDistance)
        {
            GameManager.Instance.ShowAd(false);
        }
    }
    private void RestoreDueAdReward()
    {
        Debug.LogWarning("Player position restored");
        targetHeightTracked.position = targetHeightTracked.GetComponent<BallController>().GetLastStablePositionFromLastTerrain();
        targetHeightTracked.rotation = quaternion.identity;
    }
}