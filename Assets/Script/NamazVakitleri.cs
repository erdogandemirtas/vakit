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
    public TextMeshProUGUI selectedCityText; // Se�ilen �ehir i�in Text bile�eni
    public TextMeshProUGUI dateText; // Tarih i�in Text bile�eni
    public Button backButton;

    private string baseApiUrl = "https://api.aladhan.com/v1/timingsByCity";
    private List<DateTime> prayerTimes = new List<DateTime>();
    private string[] prayerNames = { "�msak", "G�ne�", "��le", "�kindi", "Ak�am", "Yats�" };

    private void Start()
    {
        string savedCity = PlayerPrefs.GetString("SelectedCity", "");
        if (string.IsNullOrEmpty(savedCity))
        {
            SceneManager.LoadScene("CitySelection");
            return;
        }

        selectedCityText.text = $"{savedCity}"; // �ehir bilgisini ekranda g�ster
        dateText.text = $"{DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("tr-TR"))}"; // Tarih bilgisini ekranda g�ster
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
                Debug.LogError("API �ste�i Ba�ar�s�z: " + www.error);
                remainingTimeText.text = "Namaz vakitlerini y�klerken bir hata olu�tu.";
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

                // �msak
                if (DateTime.TryParseExact(timings.Fajr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz �msak tarihi format�: " + timings.Fajr);
                }

                // G�ne�
                if (DateTime.TryParseExact(timings.Sunrise, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz G�ne� tarihi format�: " + timings.Sunrise);
                }

                // ��le
                if (DateTime.TryParseExact(timings.Dhuhr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz ��le tarihi format�: " + timings.Dhuhr);
                }

                // �kindi
                if (DateTime.TryParseExact(timings.Asr, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz �kindi tarihi format�: " + timings.Asr);
                }

                // Ak�am
                if (DateTime.TryParseExact(timings.Maghrib, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz Ak�am tarihi format�: " + timings.Maghrib);
                }

                // Yats�
                if (DateTime.TryParseExact(timings.Isha, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    utcDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
                    prayerTimes.Add(utcDateTime);
                }
                else
                {
                    Debug.LogError("Ge�ersiz Yats� tarihi format�: " + timings.Isha);
                }

                // Sonu�lar� UI elemanlar�na aktar
                imsakText.text = prayerTimes.Count > 0 ? prayerTimes[0].ToString("HH:mm") : "";
                gunesText.text = prayerTimes.Count > 1 ? prayerTimes[1].ToString("HH:mm") : "";
                oglenText.text = prayerTimes.Count > 2 ? prayerTimes[2].ToString("HH:mm") : "";
                ikindiText.text = prayerTimes.Count > 3 ? prayerTimes[3].ToString("HH:mm") : "";
                aksamText.text = prayerTimes.Count > 4 ? prayerTimes[4].ToString("HH:mm") : "";
                yatsiText.text = prayerTimes.Count > 5 ? prayerTimes[5].ToString("HH:mm") : "";
            }
            else
            {
                Debug.LogError("Ge�ersiz JSON Yap�s�");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON Ayr��t�rma Hatas�: " + ex.Message);
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
                Text = $"{prayerNames[i]} vakti geldi. L�tfen namaz�n�z� k�lmay� unutmay�n.",
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
                timeToShow = "T�m vakitler ge�mi�";
            }

            remainingTimeText.text = timeToShow;

            yield return new WaitForSeconds(60f); // G�ncellemeyi her dakika yap
        }
    }

    private string GetRemainingTime(DateTime now, DateTime prayerTime)
    {
        TimeSpan remainingTime = prayerTime - now;
        if (remainingTime.TotalSeconds < 0)
        {
            return "Ge�mi�";
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
