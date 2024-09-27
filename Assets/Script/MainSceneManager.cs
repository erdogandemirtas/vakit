using Newtonsoft.Json;
using static PrayerTimeManager;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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
    public TextMeshProUGUI districtButtonText;

    private DateTime nextPrayerTime;
    public static PrayerTimes currentPrayerTimes;
    public static PrayerTimes GetCurrentPrayerTimes() => currentPrayerTimes;

    private void Start()
    {
        string selectedDistrictId = PlayerPrefs.GetString("SelectedDistrictId", null);
        DateTime lastCheckedDate = GetLastCheckedDate();

        // E�er bir il�e se�ilmi�se
        if (!string.IsNullOrEmpty(selectedDistrictId))
        {
            // Bug�n�n tarihi
            DateTime today = DateTime.Now.Date;

            // E�er bug�n�n tarihi, son kontrol edilen tarihten farkl�ysa yeni veriyi �ek
            if (today > lastCheckedDate)
            {
                StartCoroutine(GetPrayerTimes(selectedDistrictId));
            }
            else
            {
                LoadPrayerTimes(); // Kaydedilen verileri y�kle
            }

            StartCoroutine(UpdateRemainingTime());
            SetDistrictButtonText();
        }
        else
        {
            SceneManager.LoadScene("CitySelection");
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

                try
                {
                    var prayerTimesList = JsonConvert.DeserializeObject<List<PrayerTimes>>(jsonResponse);
                    if (prayerTimesList != null && prayerTimesList.Count > 0)
                    {
                        // Eski verileri sil
                        PlayerPrefs.DeleteKey("PrayerTimes");
                        PlayerPrefs.Save(); // Silme i�lemini kesinle�tir

                        // Yeni veriyi kaydet
                        PlayerPrefs.SetString("PrayerTimes", jsonResponse);
                        PlayerPrefs.Save(); // Kaydetme i�lemini kesinle�tir

                        LoadPrayerTimes(); // Yeni veriyi y�kle
                        SetLastCheckedDate(DateTime.Now); // Son veri �ekim tarihini g�ncelle
                    }
                    else
                    {
                        Debug.LogError("Namaz vakitleri listesi bo� veya null.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"JSON deserialization hatas�: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"Web iste�i ba�ar�s�z: {webRequest.error}");
                LoadPrayerTimes(); // Hata durumunda eski verileri y�kle
            }
        }
    }

    private void LoadPrayerTimes()
    {
        string prayerTimesJson = PlayerPrefs.GetString("PrayerTimes", null);
        Debug.Log($"Loaded prayer times JSON: {prayerTimesJson}");

        // E�er JSON bo�sa veya ge�ersizse, veriyi g�ncellemeye �al��
        if (string.IsNullOrEmpty(prayerTimesJson) || !TryLoadPrayerTimes(prayerTimesJson))
        {
            // Ge�erli bir veri yoksa g�ncel verileri �ek
            string selectedDistrictId = PlayerPrefs.GetString("SelectedDistrictId", null);
            if (!string.IsNullOrEmpty(selectedDistrictId))
            {
                StartCoroutine(GetPrayerTimes(selectedDistrictId));
            }
            else
            {
                Debug.LogError("Se�ilen il�e yok. L�tfen bir il�e se�in.");
                SceneManager.LoadScene("CitySelection");
            }
        }
    }

    private bool TryLoadPrayerTimes(string prayerTimesJson)
    {
        if (!string.IsNullOrEmpty(prayerTimesJson))
        {
            List<PrayerTimes> savedPrayerTimes = JsonConvert.DeserializeObject<List<PrayerTimes>>(prayerTimesJson);
            if (savedPrayerTimes != null && savedPrayerTimes.Count > 0)
            {
                ShowPrayerTimesForToday(savedPrayerTimes);
                return true; // Ba�ar�l� y�klendi
            }
            else
            {
                Debug.LogError("Kaydedilen namaz vakitleri bo� veya null.");
            }
        }
        else
        {
            Debug.LogError("Namaz vakitleri bulunamad�.");
        }
        return false; // Ba�ar�s�z y�klendi
    }

    private void ShowPrayerTimesForToday(List<PrayerTimes> prayerTimesList)
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
            MainSceneManager.currentPrayerTimes = currentPrayerTimes;
            imsakText.text = currentPrayerTimes.Imsak;
            gunesText.text = currentPrayerTimes.Gunes;
            ogleText.text = currentPrayerTimes.Ogle;
            ikindiText.text = currentPrayerTimes.Ikindi;
            aksamText.text = currentPrayerTimes.Aksam;
            yatsiText.text = currentPrayerTimes.Yatsi;
            currentDateText.text = $"{currentPrayerTimes.MiladiTarihUzun}\n{currentPrayerTimes.HicriTarihUzun}";

            nextPrayerTime = GetNextPrayerTime(DateTime.Now);
            UpdateTimeRemaining();
        }
        else
        {
            Debug.LogError("Bug�nk� namaz vakitleri bulunamad�.");
        }
    }

    private DateTime GetLastCheckedDate()
    {
        string lastCheckedDateStr = PlayerPrefs.GetString("LastCheckedDate", DateTime.MinValue.ToString("yyyy-MM-dd"));
        if (DateTime.TryParse(lastCheckedDateStr, out DateTime lastCheckedDate))
        {
            return lastCheckedDate;
        }
        return DateTime.MinValue;
    }

    private void SetLastCheckedDate(DateTime date)
    {
        PlayerPrefs.SetString("LastCheckedDate", date.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }

    private DateTime GetNextPrayerTime(DateTime currentTime)
    {
        var prayerTimes = MainSceneManager.GetCurrentPrayerTimes();
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

        return DateTime.Parse(prayerTimes.Imsak).AddDays(1);
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    private string GetNextPrayerName()
    {
        DateTime now = DateTime.Now;
        var prayerTimes = MainSceneManager.GetCurrentPrayerTimes();

        if (prayerTimes != null)
        {
            if (now < DateTime.Parse(prayerTimes.Imsak)) return "�msak";
            if (now < DateTime.Parse(prayerTimes.Gunes)) return "G�ne�";
            if (now < DateTime.Parse(prayerTimes.Ogle)) return "��le";
            if (now < DateTime.Parse(prayerTimes.Ikindi)) return "�kindi";
            if (now < DateTime.Parse(prayerTimes.Aksam)) return "Ak�am";
            if (now < DateTime.Parse(prayerTimes.Yatsi)) return "Yats�";
        }

        return "�msak";
    }

    private void UpdateTimeRemaining()
    {
        DateTime now = DateTime.Now;
        TimeSpan timeRemaining = nextPrayerTime - now;

        if (timeRemaining <= TimeSpan.Zero)
        {
            nextPrayerTime = GetNextPrayerTime(now);
            timeRemaining = nextPrayerTime - now;
        }

        string nextPrayerName = GetNextPrayerName();
        timeRemainingText.text = $"{nextPrayerName} Vaktine {FormatTimeSpan(timeRemaining)}";
    }

    private IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            UpdateTimeRemaining();
            yield return new WaitForSeconds(1);
        }
    }

    private void SetDistrictButtonText()
    {
        string selectedDistrict = PlayerPrefs.GetString("SelectedDistrict", "Se�ilen B�lge");
        districtButtonText.text = selectedDistrict;
    }
    public void OnDistrictSelectionButtonClicked()
    {
        // Eski verileri temizle
        ClearOldPrayerTimes();

        // �l�e se�imini ger�ekle�tirin (burada kendi il�e se�im mant���n�z� ekleyebilirsiniz)
        string selectedDistrictId = "YeniSe�ilen�l�eId"; // �rnek: Kullan�c�dan se�ilen il�e ID'si al�nmal�
        string selectedDistrictName = "Yeni Se�ilen �l�e"; // �rnek: Kullan�c�dan se�ilen il�e ismi al�nmal�

        // Se�ilen il�e verisini kaydet
        PlayerPrefs.SetString("SelectedDistrictId", selectedDistrictId);
        PlayerPrefs.SetString("SelectedDistrict", selectedDistrictName);
        PlayerPrefs.Save();

        // Yeni verileri y�kle
        StartCoroutine(GetPrayerTimes(selectedDistrictId));
    }

    private void ClearOldPrayerTimes()
    {
        PlayerPrefs.DeleteKey("PrayerTimes"); // Eski namaz vakitlerini sil
        PlayerPrefs.DeleteKey("LastCheckedDate"); // Son kontrol tarihini sil
        PlayerPrefs.Save(); // De�i�iklikleri kaydet
    }

    [System.Serializable]
    public class PrayerTimes
    {
        public string Imsak;
        public string Gunes;
        public string Ogle;
        public string Ikindi;
        public string Aksam;
        public string Yatsi;
        public string HicriTarihUzun;
        public string MiladiTarihKisa;
        public string MiladiTarihUzun;
    }
}
