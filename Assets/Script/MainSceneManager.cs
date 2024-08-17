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
    public TextMeshProUGUI districtButtonText; // Buton metnini gösterecek TextMeshProUGUI bileþeni

    private DateTime nextPrayerTime;

    void Start()
    {
        // Uygulamanýn ilk kez çalýþtýrýlýp çalýþtýrýlmadýðýný kontrol et
        if (!PlayerPrefs.HasKey("AppInitialized"))
        {
            // Uygulama ilk kez çalýþtýrýlýyor, CitySelection sahnesine yönlendir
            SceneManager.LoadScene("CitySelection");

            // Uygulamanýn ilk kez baþlatýldýðýný belirten anahtarý ayarla
            PlayerPrefs.SetInt("AppInitialized", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // Uygulama daha önce çalýþtýrýlmýþ, normal iþlemlere devam et
            ShowPrayerTimes(); // Namaz saatlerini göster
            StartCoroutine(UpdateRemainingTime()); // Kalan süreyi güncelle
            SetDistrictButtonText(); // Seçilen bölgeyi buton metnine yazdýr
        }

        // Önceden kaydedilmiþ namaz vakitlerini kontrol et
        string prayerTimesJson = PlayerPrefs.GetString("PrayerTimes", null);
        if (!string.IsNullOrEmpty(prayerTimesJson))
        {
            PrayerTimes savedPrayerTimes = JsonConvert.DeserializeObject<PrayerTimes>(prayerTimesJson);
            if (savedPrayerTimes != null)
            {
                PrayerTimeManager.currentPrayerTimes = savedPrayerTimes;
                ShowPrayerTimes(); // Kaydedilmiþ namaz vakitlerini göster
            }
        }
    }

    public void ShowPrayerTimes()
    {
        var prayerTimes = PrayerTimeManager.GetCurrentPrayerTimes(); // Güncel namaz saatlerini al

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
            nextPrayerTime = GetNextPrayerTime(now); // nextPrayerTime burada atanýyor
            UpdateTimeRemaining();
        }
        else
        {
            Debug.LogError("Namaz saatleri mevcut deðil.");
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

        // Eðer tüm namaz vakitleri geçmiþse, ertesi günün ilk vakti olan Ýmsak'ý döndür
        return DateTime.Parse(prayerTimes.Imsak).AddDays(1);
    }

    private string GetNextPrayerName()
    {
        DateTime now = DateTime.Now;
        var prayerTimes = PrayerTimeManager.GetCurrentPrayerTimes();

        if (prayerTimes != null)
        {
            if (now < DateTime.Parse(prayerTimes.Imsak)) return "Ýmsak";
            if (now < DateTime.Parse(prayerTimes.Gunes)) return "Güneþ";
            if (now < DateTime.Parse(prayerTimes.Ogle)) return "Öðle";
            if (now < DateTime.Parse(prayerTimes.Ikindi)) return "Ýkindi";
            if (now < DateTime.Parse(prayerTimes.Aksam)) return "Akþam";
            if (now < DateTime.Parse(prayerTimes.Yatsi)) return "Yatsý";
        }

        return "Ýmsak"; // Eðer tüm namaz vakitleri geçmiþse ertesi günün Ýmsak vaktini döndür
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

        // Sonraki vaktin ismi ve kalan süreyi göster
        timeRemainingText.text = $"{nextPrayerName} Vaktine: {FormatTimeSpan(timeRemaining)}";
    }

    private IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            UpdateTimeRemaining();
            yield return new WaitForSeconds(1); // Her saniye güncelle
        }
    }

    private void SetDistrictButtonText()
    {
        string selectedDistrict = PlayerPrefs.GetString("SelectedDistrict", "Seçilen Bölge");
        districtButtonText.text = selectedDistrict;
    }
}
