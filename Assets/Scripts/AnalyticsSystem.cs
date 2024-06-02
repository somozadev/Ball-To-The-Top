using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

namespace DefaultNamespace
{
    public class AnalyticsSystem : MonoBehaviour
    {
        private bool _consent;

        public async void Initialize()
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Analytics system initialized correctly");
            AskForConsent();
        }

        private void AskForConsent()
        {
            //not sure how 2
            //if given: 
            _consent = true;
            ConsentGiven();
        }

        private void ConsentGiven()
        {
            if (_consent)
                AnalyticsService.Instance.StartDataCollection();
        }

        public void AdReaction(bool adWatched)
        {
            if (_consent) Analytics.CustomEvent(adWatched ? "AdWatched" : "AdSkipped");
        }
    }
}