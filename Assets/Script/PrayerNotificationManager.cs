using UnityEngine;
using Unity.Notifications.Android;
using System;

public class PrayerNotificationManager : MonoBehaviour
{
    private string[] prayerNames = { "�msak", "G�ne�", "��le", "�kindi", "Ak�am", "Yats�" };

    private void Start()
    {
        // Bildirim kanal�n� olu�tur
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
        // �nceki bildirimleri temizle
        AndroidNotificationCenter.CancelAllNotifications();

        // Her bir namaz vakti i�in bildirim olu�tur
        for (int i = 0; i < prayerTimes.Length; i++)
        {
            // Bildirim zaman�n� yerel saat diliminde ayarla
            var localPrayerTime = prayerTimes[i].ToLocalTime();

            var notification = new AndroidNotification()
            {
                Title = $"{prayerNames[i]} Vakti Geldi",
                Text = $"{prayerNames[i]} vakti geldi, l�tfen namaz�n�z� k�lmay� unutmay�n.",
                FireTime = localPrayerTime, // Yerel zamana g�re ayarlama
                SmallIcon = "small_icon" // �konu buraya ekleyin
            };
            AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
        }
    }
}
