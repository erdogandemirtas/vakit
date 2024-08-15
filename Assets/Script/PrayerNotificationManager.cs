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

        // �rnek namaz vakitleri (Yerel saatlere g�re ayarlama yap�lmal�)
        DateTime[] examplePrayerTimes =
        {
            DateTime.Now.AddSeconds(10), // �rnek: �msak i�in 10 saniye sonra
            DateTime.Now.AddSeconds(20), // �rnek: G�ne� i�in 20 saniye sonra
            DateTime.Now.AddSeconds(30), // �rnek: ��le i�in 30 saniye sonra
            DateTime.Now.AddSeconds(40), // �rnek: �kindi i�in 40 saniye sonra
            DateTime.Now.AddSeconds(50), // �rnek: Ak�am i�in 50 saniye sonra
            DateTime.Now.AddSeconds(60)  // �rnek: Yats� i�in 60 saniye sonra
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

        // �nceki bildirimleri temizle
        AndroidNotificationCenter.CancelAllNotifications();

        // Her bir namaz vakti i�in bildirim olu�tur
        for (int i = 0; i < prayerTimes.Length; i++)
        {
            // Bildirim zaman�n� yerel saat diliminde ayarla
            var localPrayerTime = prayerTimes[i].ToLocalTime();

            // Kalan s�reyi hesapla
            TimeSpan remainingTime = localPrayerTime - DateTime.Now;

            // Kalan s�re 0 oldu�unda bildirim g�nder
            if (remainingTime.TotalSeconds <= 0)
            {
                SendNotification(prayerNames[i]);
            }
            else
            {
                // Kalan s�reyi s�f�rlayarak bildirim g�ndermek i�in zamanla
                ScheduleNotification(prayerNames[i], localPrayerTime);
            }
        }
    }

    private void SendNotification(string prayerName)
    {
        var notification = new AndroidNotification()
        {
            Title = $"{prayerName} Vakti Geldi",
            Text = $"{prayerName} vakti geldi, l�tfen namaz�n�z� k�lmay� unutmay�n.",
            FireTime = DateTime.Now, // Hemen g�nder
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
            Text = $"{prayerName} vakti geldi, l�tfen namaz�n�z� k�lmay� unutmay�n.",
            FireTime = fireTime, // Belirlenen zamana g�re ayarlama
            SmallIcon = "icon", // Optional: Set a small icon for the notification
            LargeIcon = "icon_large" // Optional: Set a large icon for the notification
        };

        AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
    }
}
