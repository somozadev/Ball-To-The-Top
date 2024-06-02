using System;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationSystem : MonoBehaviour
{
#if UNITY_ANDROID
    [SerializeField] private AndroidNotifications _androidNotifications;
#endif
#if UNITY_IOS
        [SerializeField] private IOSNotifications _iosNotifications;
#endif

    private const string Title = "The top is waiting!";
    private const string Text = "Come back to reach the top of the hill with the best time possible!";
    private const string Subtitle = "Come back";
    private const string Body = "To reach the top of the hill with the best time possible!";

    private void Awake()
    {
#if UNITY_ANDROID
        _androidNotifications = GetComponentInChildren<AndroidNotifications>();
#endif
#if UNITY_IOS
        _iosNotifications = GetComponentInChildren<IOSNotifications>();
#endif
    }

    public void Initialize()
    {
#if UNITY_ANDROID
        _androidNotifications.RequestAuthorization();
        _androidNotifications.RegisterNotificationChannel();
#endif
#if UNITY_IOS
            StartCoroutine(_iosNotifications.RequestAuthorization());
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) return;
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
        _androidNotifications.SendNotification(Title, Text, Random.Range(35,45));
#endif
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        _iosNotifications.SendNotification(Title, Subtitle, Body, Random.Range(35,45));
#endif
    }
}