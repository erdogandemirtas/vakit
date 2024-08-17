using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using static PrayerTimeManager;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public TextMeshProUGUI imsakText;
    public TextMeshProUGUI gunesText;
    public TextMeshProUGUI ogleText;
    public TextMeshProUGUI ikindiText;
    public TextMeshProUGUI aksamText;
    public TextMeshProUGUI yatsiText;
    public TextMeshProUGUI currentDateText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI districtButtonText; // Buton metnini g�sterecek TextMeshProUGUI bile�eni

    private DateTime nextPrayerTime;

    void Start()
    {
        // Uygulaman�n ilk kez �al��t�r�l�p �al��t�r�lmad���n� kontrol et
        if (!PlayerPrefs.HasKey("AppInitialized"))
        {
            // Uygulama ilk kez �al��t�r�l�yor, CitySelection sahnesine y�nlendir
            SceneManager.LoadScene("CitySelection");

            // Uygulaman�n ilk kez ba�lat�ld���n� belirten anahtar� ayarla
            PlayerPrefs.SetInt("AppInitialized", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // Uygulama daha �nce �al��t�r�lm��, normal i�lemlere devam et
            ShowPrayerTimes(); // Namaz saatlerini g�ster
            StartCoroutine(UpdateRemainingTime()); // Kalan s�reyi g�ncelle
            SetDistrictButtonText(); // Se�ilen b�lgeyi buton metnine yazd�r
        }

        // �nceden kaydedilmi� namaz vakitlerini kontrol et
        string prayerTimesJson = PlayerPrefs.GetString("PrayerTimes", null);
        if (!string.IsNullOrEmpty(prayerTimesJson))
        {
            PrayerTimes savedPrayerTimes = JsonConvert.DeserializeObject<PrayerTimes>(prayerTimesJson);
            if (savedPrayerTimes != null)
            {
                PrayerTimeManager.currentPrayerTimes = savedPrayerTimes;
                ShowPrayerTimes(); // Kaydedilmi� namaz vakitlerini g�ster
            }
        }
    }

    public void ShowPrayerTimes()
    {
        var prayerTimes = PrayerTimeManager.GetCurrentPrayerTimes(); // G�ncel namaz saatlerini al

        if (prayerTimes != null)
        {
            imsakText.text = prayerTimes.Imsak;
            gunesText.text = prayerTimes.Gunes;
            ogleText.text = prayerTimes.Ogle;
            ikindiText.text = prayerTimes.Ikindi;
            aksamText.text = prayerTimes.Aksam;
            yatsiText.text = prayerTimes.Yatsi;

            currentDateText.text = prayerTimes.MiladiTarihUzun;

            DateTime now = DateTime.Now;
            nextPrayerTime = GetNextPrayerTime(now); // nextPrayerTime burada atan�yor
            UpdateTimeRemaining();
        }
        else
        {
            Debug.LogError("Namaz saatleri mevcut de�il.");
        }
    }

    private DateTime GetNextPrayerTime(DateTime currentTime)
    {
        var prayerTimes = PrayerTimeManager.GetCurrentPrayerTimes();
        if (prayerTimes == null) return currentTime;

        var prayerTimesList = new List<DateTime>
    {
        DateTime.Parse(prayerTimes.Imsak),
        DateTime.Parse(prayerTimes.Gunes),
        DateTime.Parse(prayerTimes.Ogle),
        DateTime.Parse(prayerTimes.Ikindi),
        DateTime.Parse(prayerTimes.Aksam),
        DateTime.Parse(prayerTimes.Yatsi)
    };

        foreach (var prayerTime in prayerTimesList)
        {
            if (currentTime < prayerTime)
            {
                return prayerTime;
            }
        }

        // E�er t�m namaz vakitleri ge�mi�se, ertesi g�n�n ilk vakti olan �msak'� d�nd�r
        return DateTime.Parse(prayerTimes.Imsak).AddDays(1);
    }

    private string GetNextPrayerName()
    {
        DateTime now = DateTime.Now;
        var prayerTimes = PrayerTimeManager.GetCurrentPrayerTimes();

        if (prayerTimes != null)
        {
            if (now < DateTime.Parse(prayerTimes.Imsak)) return "�msak";
            if (now < DateTime.Parse(prayerTimes.Gunes)) return "G�ne�";
            if (now < DateTime.Parse(prayerTimes.Ogle)) return "��le";
            if (now < DateTime.Parse(prayerTimes.Ikindi)) return "�kindi";
            if (now < DateTime.Parse(prayerTimes.Aksam)) return "Ak�am";
            if (now < DateTime.Parse(prayerTimes.Yatsi)) return "Yats�";
        }

        return "�msak"; // E�er t�m namaz vakitleri ge�mi�se ertesi g�n�n �msak vaktini d�nd�r
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    private void UpdateTimeRemaining()
    {
        DateTime now = DateTime.Now;
        TimeSpan timeRemaining = nextPrayerTime - now;

        // Sonraki namaz vaktinin ismini belirle
        string nextPrayerName = GetNextPrayerName();

        // Sonraki vaktin ismi ve kalan s�reyi g�ster
        timeRemainingText.text = $"{nextPrayerName} Vaktine: {FormatTimeSpan(timeRemaining)}";
    }

    private IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            UpdateTimeRemaining();
            yield return new WaitForSeconds(1); // Her saniye g�ncelle
        }
    }

    private void SetDistrictButtonText()
    {
        string selectedDistrict = PlayerPrefs.GetString("SelectedDistrict", "Se�ilen B�lge");
        districtButtonText.text = selectedDistrict;
    }
}
