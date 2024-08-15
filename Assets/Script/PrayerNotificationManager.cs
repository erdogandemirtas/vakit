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

        // Örnek namaz vakitleri (Yerel saatlere göre ayarlama yapýlmalý)
        DateTime[] examplePrayerTimes =
        {
            DateTime.Now.AddSeconds(10), // Örnek: Ýmsak için 10 saniye sonra
            DateTime.Now.AddSeconds(20), // Örnek: Güneþ için 20 saniye sonra
            DateTime.Now.AddSeconds(30), // Örnek: Öðle için 30 saniye sonra
            DateTime.Now.AddSeconds(40), // Örnek: Ýkindi için 40 saniye sonra
            DateTime.Now.AddSeconds(50), // Örnek: Akþam için 50 saniye sonra
            DateTime.Now.AddSeconds(60)  // Örnek: Yatsý için 60 saniye sonra
        };

        SchedulePrayerNotifications(examplePrayerTimes);
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
        if (prayerTimes == null || prayerTimes.Length != prayerNames.Length)
        {
            Debug.LogError("Invalid prayerTimes array.");
            return;
        }

        // Önceki bildirimleri temizle
        AndroidNotificationCenter.CancelAllNotifications();

        // Her bir namaz vakti için bildirim oluþtur
        for (int i = 0; i < prayerTimes.Length; i++)
        {
            // Bildirim zamanýný yerel saat diliminde ayarla
            var localPrayerTime = prayerTimes[i].ToLocalTime();

            // Kalan süreyi hesapla
            TimeSpan remainingTime = localPrayerTime - DateTime.Now;

            // Kalan süre 0 olduðunda bildirim gönder
            if (remainingTime.TotalSeconds <= 0)
            {
                SendNotification(prayerNames[i]);
            }
            else
            {
                // Kalan süreyi sýfýrlayarak bildirim göndermek için zamanla
                ScheduleNotification(prayerNames[i], localPrayerTime);
            }
        }
    }

    private void SendNotification(string prayerName)
    {
        var notification = new AndroidNotification()
        {
            Title = $"{prayerName} Vakti Geldi",
            Text = $"{prayerName} vakti geldi, lütfen namazýnýzý kýlmayý unutmayýn.",
            FireTime = DateTime.Now, // Hemen gönder
            SmallIcon = "icon", // Optional: Set a small icon for the notification
            LargeIcon = "icon_large" // Optional: Set a large icon for the notification
        };

        AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
    }

    private void ScheduleNotification(string prayerName, DateTime fireTime)
    {
        var notification = new AndroidNotification()
        {
            Title = $"{prayerName} Vakti Geldi",
            Text = $"{prayerName} vakti geldi, lütfen namazýnýzý kýlmayý unutmayýn.",
            FireTime = fireTime, // Belirlenen zamana göre ayarlama
            SmallIcon = "icon", // Optional: Set a small icon for the notification
            LargeIcon = "icon_large" // Optional: Set a large icon for the notification
        };

        AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
    }
}
