using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class RewardAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string androidAdUnitId;
        [SerializeField] private string iosAdUnitId;

        private string adUnitId;
        public UnityEvent OnAdEnded;

        private void Awake()
        {
#if UNITY_IOS
                adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
#endif
        }

        public void LoadAd()
        {
            Advertisement.Load(adUnitId, this);
        }

        public void ShowAd()
        {
            Advertisement.Show(adUnitId, this);
            LoadAd();
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("ad clicked!");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (placementId == adUnitId && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Debug.Log("Give reward");
                GameManager.Instance.InvokeAdEndedEvent();
                GameManager.Instance.adsCanvas.gameObject.SetActive(false);
                GameManager.Instance.AnalyticsSystemRef.AdReaction(true);
            }
        }
    }
}