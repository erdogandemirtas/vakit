using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using static PrayerTimeManager;

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
        DateTime lastCheckedDate = GetLastCheckedDate();
        DateTime today = DateTime.Now.Date;

        // G�n de�i�ti mi kontrol et
        if (lastCheckedDate < today)
        {
            // G�n de�i�mi�, il�e se�im ekran�na y�nlendir
            // Ancak �nce se�ilmi� il�eyi kullanarak namaz vakitlerini g�ncelle
            string selectedDistrictId = PlayerPrefs.GetString("SelectedDistrictId", null);
            if (!string.IsNullOrEmpty(selectedDistrictId))
            {
                StartCoroutine(GetPrayerTimes(selectedDistrictId));
                ShowPrayerTimes(); // Namaz saatlerini g�ster
                StartCoroutine(UpdateRemainingTime()); // Kalan s�reyi g�ncelle
                SetDistrictButtonText(); // Se�ilen b�lgeyi buton metnine yazd�r
            }
            else
            {
                // �l�e se�ilmemi�se il�e se�im ekran�na y�nlendir
                SceneManager.LoadScene("CitySelection");
            }

            // Son kontrol edilen tarihi g�ncelle
            SetLastCheckedDate(today);
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
    private IEnumerator GetPrayerTimes(string districtId)
    {
        string url = $"https://ezanvakti.herokuapp.com/vakitler/{districtId}";
        Debug.Log($"Fetching prayer times from URL: {url}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Fetched prayer times from web: {jsonResponse}");

                var prayerTimesList = JsonConvert.DeserializeObject<List<PrayerTimes>>(jsonResponse);

                if (prayerTimesList != null && prayerTimesList.Count > 0)
                {
                    DateTime todayDate = DateTime.Now.Date;
                    PrayerTimes currentPrayerTimes = null;

                    foreach (var prayerTimes in prayerTimesList)
                    {
                        if (DateTime.TryParseExact(prayerTimes.MiladiTarihKisa, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime prayerDate))
                        {
                            if (prayerDate.Date == todayDate)
                            {
                                currentPrayerTimes = prayerTimes;
                                break;
                            }
                        }
                        else
                        {
                            Debug.LogError($"Tarih format� uyumsuz: {prayerTimes.MiladiTarihKisa}");
                        }
                    }

                    if (currentPrayerTimes != null)
                    {
                        PrayerTimeManager.currentPrayerTimes = currentPrayerTimes;
                        PlayerPrefs.SetString("PrayerTimes", JsonConvert.SerializeObject(currentPrayerTimes));
                        PlayerPrefs.Save();
                        ShowPrayerTimes();
                    }
                    else
                    {
                        Debug.LogError("Bug�nk� namaz vakitleri bulunamad�.");
                    }
                }
                else
                {
                    Debug.LogError("Namaz vakitleri listesi bo� veya null.");
                }
            }
            else
            {
                Debug.LogError($"Web iste�i ba�ar�s�z: {webRequest.error}");
            }
        }
    }

    private DateTime GetLastCheckedDate()
    {
        // Son kontrol edilen tarihi PlayerPrefs'ten al
        string lastCheckedDateStr = PlayerPrefs.GetString("LastCheckedDate", DateTime.MinValue.ToString("yyyy-MM-dd"));
        return DateTime.Parse(lastCheckedDateStr);
    }

    private void SetLastCheckedDate(DateTime date)
    {
        // Son kontrol edilen tarihi PlayerPrefs'e kaydet
        PlayerPrefs.SetString("LastCheckedDate", date.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
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

            currentDateText.text = $"{prayerTimes.MiladiTarihUzun}\n{prayerTimes.HicriTarihUzun}";

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

        if (timeRemaining <= TimeSpan.Zero) // Vakit geldiyse
        {
            nextPrayerTime = GetNextPrayerTime(now);
            timeRemaining = nextPrayerTime - now; // Yeni vakit i�in kalan s�reyi hesapla
        }

        // Sonraki namaz vaktinin ismini belirle
        string nextPrayerName = GetNextPrayerName();

        // Sonraki vaktin ismi ve kalan s�reyi g�ster
        timeRemainingText.text = $"{nextPrayerName} Vaktine {FormatTimeSpan(timeRemaining)}";
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
