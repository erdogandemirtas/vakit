using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public TextMeshProUGUI selectedCityText;
    public TextMeshProUGUI dateText;
    public Button backButton;

    private string baseApiUrl = "https://api.aladhan.com/v1/timingsByCity";
    private List<DateTime> prayerTimes = new List<DateTime>();
    private string[] prayerNames = { "�msak", "G�ne�", "��le", "�kindi", "Ak�am", "Yats�" };

    private void Start()
    {
        // Ekran�n kapanmas�n� engellemek i�in
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        string savedCity = PlayerPrefs.GetString("SelectedCity", "");
        if (string.IsNullOrEmpty(savedCity))
        {
            SceneManager.LoadScene("CitySelection");
            return;
        }

        selectedCityText.text = $"{savedCity}";
        dateText.text = $"{DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("tr-TR"))}";
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

                // Her vakti ekleyin
                AddPrayerTime(timings.Imsak, timeFormats);
                AddPrayerTime(timings.Sunrise, timeFormats);
                AddPrayerTime(timings.Dhuhr, timeFormats);
                AddPrayerTime(timings.Asr, timeFormats);
                AddPrayerTime(timings.Maghrib, timeFormats);
                AddPrayerTime(timings.Isha, timeFormats);

                // Sonu�lar� UI elemanlar�na aktar
                UpdatePrayerTimesUI();
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

    private void AddPrayerTime(string timeString, string[] timeFormats)
    {
        DateTime parsedDateTime;
        if (DateTime.TryParseExact(timeString, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
        {
            // Yerel zaman olarak sakla
            prayerTimes.Add(DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Local));
        }
        else
        {
            Debug.LogError($"Ge�ersiz tarih format�: {timeString}");
        }
    }

    private void UpdatePrayerTimesUI()
    {
        imsakText.text = prayerTimes.Count > 0 ? prayerTimes[0].ToString("HH:mm") : "";
        gunesText.text = prayerTimes.Count > 1 ? prayerTimes[1].ToString("HH:mm") : "";
        oglenText.text = prayerTimes.Count > 2 ? prayerTimes[2].ToString("HH:mm") : "";
        ikindiText.text = prayerTimes.Count > 3 ? prayerTimes[3].ToString("HH:mm") : "";
        aksamText.text = prayerTimes.Count > 4 ? prayerTimes[4].ToString("HH:mm") : "";
        yatsiText.text = prayerTimes.Count > 5 ? prayerTimes[5].ToString("HH:mm") : "";
    }

    private IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            DateTime now = DateTime.Now; // �u anki zaman� yerel saat diliminde al
            string timeToShow = "";

            for (int i = 0; i < prayerTimes.Count; i++)
            {
                DateTime prayerTimeLocal = prayerTimes[i];

                if (now < prayerTimeLocal)
                {
                    timeToShow = $"{prayerNames[i]} vaktine: {GetRemainingTime(now, prayerTimeLocal)}";
                    break;
                }
            }

            if (string.IsNullOrEmpty(timeToShow))
            {
                timeToShow = "T�m vakitler ge�mi�";
            }

            remainingTimeText.text = timeToShow;

            yield return new WaitForSeconds(1f);
        }
    }

    private string GetRemainingTime(DateTime now, DateTime prayerTimeLocal)
    {
        TimeSpan remainingTime = prayerTimeLocal - now;
        Debug.Log($"Kalan S�re: {remainingTime}");
        if (remainingTime.TotalSeconds < 0)
        {
            return "Ge�mi�";
        }
        return $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}";
    }

    private void OnBackButtonPressed()
    {
        PlayerPrefs.DeleteKey("SelectedCity");
        SceneManager.LoadScene("CitySelection");
    }

    private void OnDestroy()
    {
        // Ekran kapanma ayar�n� varsay�lan hale d�nd�rmek i�in
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}

[Serializable]
public class AladhanResponse
{
    public AladhanData data;
}

[Serializable]
public class AladhanData
{
    public Timings timings;
}

[Serializable]
public class Timings
{
    public string Imsak;
    public string Sunrise;
    public string Dhuhr;
    public string Asr;
    public string Maghrib;
    public string Isha;
}
