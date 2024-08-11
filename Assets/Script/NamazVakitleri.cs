using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using System.Globalization;
using TMPro;

public class NamazVakitleri : MonoBehaviour
{
    public TextMeshProUGUI imsakText;
    public TextMeshProUGUI gunesText;
    public TextMeshProUGUI oglenText;
    public TextMeshProUGUI ikindiText;
    public TextMeshProUGUI aksamText;
    public TextMeshProUGUI yatsiText;
    public TextMeshProUGUI remainingTimeText;
    public TextMeshProUGUI selectedCityText; // Seçilen þehir için Text bileþeni
    public TextMeshProUGUI dateText; // Tarih için Text bileþeni
    public Button backButton;

    private string baseApiUrl = "https://api.aladhan.com/v1/timingsByCity";
    private List<DateTime> prayerTimes = new List<DateTime>();
    private string[] prayerNames = { "Ýmsak", "Güneþ", "Öðle", "Ýkindi", "Akþam", "Yatsý" };

    private void Start()
    {
        string savedCity = PlayerPrefs.GetString("SelectedCity", "");
        if (string.IsNullOrEmpty(savedCity))
        {
            SceneManager.LoadScene("CitySelection");
            return;
        }

        selectedCityText.text = $"{savedCity}"; // Þehir bilgisini ekranda göster
        dateText.text = $"{DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("tr-TR"))}"; // Tarih bilgisini ekranda göster
        GetNamazVakitleri(savedCity);

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonPressed);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void GetNamazVakitleri(string cityName)
    {
        string apiUrl = $"{baseApiUrl}?city={cityName}&country=Turkey&method=13";
        StartCoroutine(FetchNamazVakitleri(apiUrl));
    }

    private IEnumerator FetchNamazVakitleri(string apiUrl)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                ParseJson(jsonResponse);
                SchedulePrayerNotifications();
                StartCoroutine(UpdateRemainingTime());
            }
            else
            {
                Debug.LogError("API Ýsteði Baþarýsýz: " + www.error);
                remainingTimeText.text = "Namaz vakitlerini yüklerken bir hata oluþtu.";
            }
        }
    }

    private void ParseJson(string json)
    {
        try
        {
            AladhanResponse jsonData = JsonUtility.FromJson<AladhanResponse>(json);
            if (jsonData != null && jsonData.data != null && jsonData.data.timings != null)
            {
                var timings = jsonData.data.timings;
                prayerTimes.Clear();

                string[] timeFormats = { "HH:mm:ss", "HH:mm" };

                DateTime parsedDateTime;
                DateTime utcDateTime;

                // Ýmsak
                if (DateTime.TryParseExact(timings.Fajr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Ýmsak tarihi formatý: " + timings.Fajr);
                }

                // Güneþ
                if (DateTime.TryParseExact(timings.Sunrise, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Güneþ tarihi formatý: " + timings.Sunrise);
                }

                // Öðle
                if (DateTime.TryParseExact(timings.Dhuhr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Öðle tarihi formatý: " + timings.Dhuhr);
                }

                // Ýkindi
                if (DateTime.TryParseExact(timings.Asr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Ýkindi tarihi formatý: " + timings.Asr);
                }

                // Akþam
                if (DateTime.TryParseExact(timings.Maghrib, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Akþam tarihi formatý: " + timings.Maghrib);
                }

                // Yatsý
                if (DateTime.TryParseExact(timings.Isha, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Geçersiz Yatsý tarihi formatý: " + timings.Isha);
                }

                // Sonuçlarý UI elemanlarýna aktar
                imsakText.text = prayerTimes.Count > 0 ? prayerTimes[0].ToString("HH:mm") : "";
                gunesText.text = prayerTimes.Count > 1 ? prayerTimes[1].ToString("HH:mm") : "";
                oglenText.text = prayerTimes.Count > 2 ? prayerTimes[2].ToString("HH:mm") : "";
                ikindiText.text = prayerTimes.Count > 3 ? prayerTimes[3].ToString("HH:mm") : "";
                aksamText.text = prayerTimes.Count > 4 ? prayerTimes[4].ToString("HH:mm") : "";
                yatsiText.text = prayerTimes.Count > 5 ? prayerTimes[5].ToString("HH:mm") : "";
            }
            else
            {
                Debug.LogError("Geçersiz JSON Yapýsý");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON Ayrýþtýrma Hatasý: " + ex.Message);
        }
    }

    private void SchedulePrayerNotifications()
    {
        var notificationChannel = new AndroidNotificationChannel()
        {
            Id = "prayer_channel",
            Name = "Prayer Notifications",
            Importance = Importance.High,
            Description = "Notification channel for prayer times."
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);

        for (int i = 0; i < prayerTimes.Count; i++)
        {
            var notification = new AndroidNotification()
            {
                Title = $"{prayerNames[i]} Vakti Geldi",
                Text = $"{prayerNames[i]} vakti geldi. Lütfen namazýnýzý kýlmayý unutmayýn.",
                FireTime = prayerTimes[i],
                SmallIcon = "icon"
            };
            AndroidNotificationCenter.SendNotification(notification, "prayer_channel");
        }
    }

    private IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            DateTime now = DateTime.UtcNow; // UTC'yi kullan
            string timeToShow = "";

            for (int i = 0; i < prayerTimes.Count; i++)
            {
                if (now < prayerTimes[i])
                {
                    timeToShow = $"{prayerNames[i]} vakti: {GetRemainingTime(now, prayerTimes[i])}";
                    break;
                }
            }

            if (string.IsNullOrEmpty(timeToShow))
            {
                timeToShow = "Tüm vakitler geçmiþ";
            }

            remainingTimeText.text = timeToShow;

            yield return new WaitForSeconds(60f); // Güncellemeyi her dakika yap
        }
    }

    private string GetRemainingTime(DateTime now, DateTime prayerTime)
    {
        TimeSpan remainingTime = prayerTime - now;
        if (remainingTime.TotalSeconds < 0)
        {
            return "Geçmiþ";
        }
        return $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}";
    }

    private void OnBackButtonPressed()
    {
        SceneManager.LoadScene("CitySelection");
    }

    [System.Serializable]
    public class AladhanResponse
    {
        public Data data;

        [System.Serializable]
        public class Data
        {
            public Timings timings;

            [System.Serializable]
            public class Timings
            {
                public string Fajr;
                public string Sunrise;
                public string Dhuhr;
                public string Asr;
                public string Maghrib;
                public string Isha;
            }
        }
    }
}
