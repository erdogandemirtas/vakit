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

        // Eðer bir ilçe seçilmiþse
        if (!string.IsNullOrEmpty(selectedDistrictId))
        {
            // Bugünün tarihi
            DateTime today = DateTime.Now.Date;

            // Eðer bugünün tarihi, son kontrol edilen tarihten farklýysa yeni veriyi çek
            if (today > lastCheckedDate)
            {
                StartCoroutine(GetPrayerTimes(selectedDistrictId));
            }
            else
            {
                LoadPrayerTimes(); // Kaydedilen verileri yükle
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
                        PlayerPrefs.Save(); // Silme iþlemini kesinleþtir

                        // Yeni veriyi kaydet
                        PlayerPrefs.SetString("PrayerTimes", jsonResponse);
                        PlayerPrefs.Save(); // Kaydetme iþlemini kesinleþtir

                        LoadPrayerTimes(); // Yeni veriyi yükle
                        SetLastCheckedDate(DateTime.Now); // Son veri çekim tarihini güncelle
                    }
                    else
                    {
                        Debug.LogError("Namaz vakitleri listesi boþ veya null.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"JSON deserialization hatasý: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"Web isteði baþarýsýz: {webRequest.error}");
                LoadPrayerTimes(); // Hata durumunda eski verileri yükle
            }
        }
    }

    private void LoadPrayerTimes()
    {
        string prayerTimesJson = PlayerPrefs.GetString("PrayerTimes", null);
        Debug.Log($"Loaded prayer times JSON: {prayerTimesJson}");

        // Eðer JSON boþsa veya geçersizse, veriyi güncellemeye çalýþ
        if (string.IsNullOrEmpty(prayerTimesJson) || !TryLoadPrayerTimes(prayerTimesJson))
        {
            // Geçerli bir veri yoksa güncel verileri çek
            string selectedDistrictId = PlayerPrefs.GetString("SelectedDistrictId", null);
            if (!string.IsNullOrEmpty(selectedDistrictId))
            {
                StartCoroutine(GetPrayerTimes(selectedDistrictId));
            }
            else
            {
                Debug.LogError("Seçilen ilçe yok. Lütfen bir ilçe seçin.");
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
                return true; // Baþarýlý yüklendi
            }
            else
            {
                Debug.LogError("Kaydedilen namaz vakitleri boþ veya null.");
            }
        }
        else
        {
            Debug.LogError("Namaz vakitleri bulunamadý.");
        }
        return false; // Baþarýsýz yüklendi
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
                Debug.LogError($"Tarih formatý uyumsuz: {prayerTimes.MiladiTarihKisa}");
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
            Debug.LogError("Bugünkü namaz vakitleri bulunamadý.");
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
            if (now < DateTime.Parse(prayerTimes.Imsak)) return "Ýmsak";
            if (now < DateTime.Parse(prayerTimes.Gunes)) return "Güneþ";
            if (now < DateTime.Parse(prayerTimes.Ogle)) return "Öðle";
            if (now < DateTime.Parse(prayerTimes.Ikindi)) return "Ýkindi";
            if (now < DateTime.Parse(prayerTimes.Aksam)) return "Akþam";
            if (now < DateTime.Parse(prayerTimes.Yatsi)) return "Yatsý";
        }

        return "Ýmsak";
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
        string selectedDistrict = PlayerPrefs.GetString("SelectedDistrict", "Seçilen Bölge");
        districtButtonText.text = selectedDistrict;
    }
    public void OnDistrictSelectionButtonClicked()
    {
        // Eski verileri temizle
        ClearOldPrayerTimes();

        // Ýlçe seçimini gerçekleþtirin (burada kendi ilçe seçim mantýðýnýzý ekleyebilirsiniz)
        string selectedDistrictId = "YeniSeçilenÝlçeId"; // Örnek: Kullanýcýdan seçilen ilçe ID'si alýnmalý
        string selectedDistrictName = "Yeni Seçilen Ýlçe"; // Örnek: Kullanýcýdan seçilen ilçe ismi alýnmalý

        // Seçilen ilçe verisini kaydet
        PlayerPrefs.SetString("SelectedDistrictId", selectedDistrictId);
        PlayerPrefs.SetString("SelectedDistrict", selectedDistrictName);
        PlayerPrefs.Save();

        // Yeni verileri yükle
        StartCoroutine(GetPrayerTimes(selectedDistrictId));
    }

    private void ClearOldPrayerTimes()
    {
        PlayerPrefs.DeleteKey("PrayerTimes"); // Eski namaz vakitlerini sil
        PlayerPrefs.DeleteKey("LastCheckedDate"); // Son kontrol tarihini sil
        PlayerPrefs.Save(); // Deðiþiklikleri kaydet
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
