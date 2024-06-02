using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class AndroidNotifications : MonoBehaviour
{
#if UNITY_ANDROID
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
    }

    public void RegisterNotificationChannel()
    {
        var chanel = new AndroidNotificationChannel
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "The top needs you"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(chanel);
    }

    public void SendNotification(string title, string text, int fireTimeInHours)
    {
        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = System.DateTime.Now.AddHours(fireTimeInHours),
            SmallIcon = "default_icon",
            LargeIcon = "default_icon_large"
        };
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
#endif
}