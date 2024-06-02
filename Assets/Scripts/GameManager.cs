using System;
using System.Collections;
using System.Collections.Generic;
using Dan.Main;
using DefaultNamespace;
using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public Data currentData;
    private NotificationSystem _notificationSystem;

    [Header("Ads")] [Space(10)] [SerializeField]
    private string _androidGameId;

    [SerializeField] private string _iosGameId;
    [SerializeField] private RewardAds _ads;
    public Canvas adsCanvas;
    public bool isTesting;
    private string _gameId;

    public Leaderboard LeaderboardsRef;
    public AnalyticsSystem AnalyticsSystemRef;
    public delegate void AdDelegate();

    public event AdDelegate OnAdEnded;

    #region Singleton

    public static GameManager Instance;
    [SerializeField] private Animator _animator;
    public bool detectMouseGameInput = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }

        _animator = GetComponent<Animator>();
        LeaderboardsRef = GetComponentInChildren<Leaderboard>();
        AnalyticsSystemRef.Initialize();

#if UNITY_IOS
        _gameId = _iosGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
        _gameId = _androidGameId;
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
            Advertisement.Initialize(_gameId, isTesting, this);
    }

    #endregion


    private void Start()
    {
        if (DataController.HasDataSaved())
            currentData = DataController.Load<Data>();
        else
            DataController.SaveNew(new Data());

        _notificationSystem = GetComponentInChildren<NotificationSystem>();
        _notificationSystem.Initialize();
        SceneManager.LoadScene("StartScene");
    }

    public void LoadGameScene()
    {
        _animator.SetTrigger("LoadGameScene");
    }

    public void EventLoadScene()
    {
        SceneManager.LoadScene("GameScene");
        _animator.SetTrigger("StartGameScene");
        detectMouseGameInput = true;
        _ads.LoadAd();
    }

    public void LoadHomeScene()
    {
        _animator.SetTrigger("LoadHomeScene");
    }

    public void EventHomeScene()
    {
        SceneManager.LoadScene("StartScene");
        _animator.SetTrigger("StartHomeScene");
        detectMouseGameInput = false;
    }

    [ContextMenu("Play Rewarded Ad")]
    public void ShowAd(bool obligatory)
    {
        detectMouseGameInput = false;
        if (obligatory)
        {
            _ads.ShowAd();
        }
        else
        {
            adsCanvas.gameObject.SetActive(true);
            // StartCoroutine(WaitToHideRestorePosCanvasAd());
        }
    }

    public void IgnoreAd()
    {
        adsCanvas.gameObject.SetActive(false);
        detectMouseGameInput = true;
        AnalyticsSystemRef.AdReaction(false);
    }

    private IEnumerator WaitToHideRestorePosCanvasAd()
    {
        yield return new WaitForSeconds(6.5f);
        adsCanvas.gameObject.SetActive(false);
        yield return null;
    }

    public void SaveBall(Transform trf)
    {
        var position = trf.position;
        var rotation = trf.rotation;
        currentData.XPos = position.x;
        currentData.YPos = position.y;
        currentData.ZPos = position.z;
        currentData.XRot = rotation.eulerAngles.x;
        currentData.YRot = rotation.eulerAngles.y;
        currentData.ZRot = rotation.eulerAngles.z;
        DataController.Save(currentData);
    }

    public void SaveTime(string time)
    {
        currentData.Time = time;
        DataController.Save(currentData);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialized correctly");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"ERROR- Ads Initialization failed, {message}");
    }

    public void InvokeAdEndedEvent()
    {
        detectMouseGameInput = true;
        OnAdEnded?.Invoke();
    }
}