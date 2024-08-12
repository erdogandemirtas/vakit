using UnityEngine;
using Unity.Notifications.Android;
using System;

public class PrayerNotificationManager : MonoBehaviour
{
    private string[] prayerNames = { "Ýmsak", "Güneþ", "Öðle", "Ýkindi", "Akþam", "Yatsý" };

    private void Start()
    {
        // Bildirim kanalýný oluþtur
        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        var notificationChannel = new AndroidNotificationChannel()
        {
            Id = "prayer_channel",
            Name = "Prayer Notifications",
            Importance = Importance.High,
            Description = "Notification channel for prayer times."
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
    }

    public void SchedulePrayerNotifications(DateTime[] prayerTimes)
    {
        // Önceki bildirimleri temizle
        AndroidNotificationCenter.CancelAllNotifications();

        // Her bir namaz vakti için bildirim oluþtur
        for (int i = 0; i < prayerTimes.Length; i++)
        {
            // Bildirim zamanýný yerel saat diliminde ayarla
            var localPrayerTime = prayerTimes[i].ToLocalTime();

            var notification = new AndroidNotification()
            {
                Title = $"{prayerNames[i]} Vakti Geldi",
                Text = $"{prayerNames[i]} vakti geldi, lütfen namazýnýzý kýlmayý unutmayýn.",
                FireTime = localPrayerTime, // Yerel zamana göre ayarlama
                SmallIcon = "small_icon" // Ýkonu buraya ekleyin
            };
            AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
        }
    }
}
