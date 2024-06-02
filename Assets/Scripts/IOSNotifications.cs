using UnityEngine;
#if UNITY_IOS
using System.Collections;
using Unity.Notifications.iOS;
#endif

public class IOSNotifications : MonoBehaviour
{
#if UNITY_IOS
    public IEnumerator RequestAuthorization()
    {
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }
    }

    public void SendNotification(string title, string subtitle, string body, int fireTimeInHours)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(fireTimeInHours, 0, 0),
            Repeats = false
        };
        var notification = new iOSNotification()
        {
            Identifier = "default_notification",
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Badge),
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger
        };
        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif
}