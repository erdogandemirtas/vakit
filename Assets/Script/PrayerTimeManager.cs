using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class DropdownOption
{
    public string DisplayName { get; set; }
    public string Id { get; set; }
    public OptionType Type { get; set; }

    public enum OptionType
    {
        Country,
        City,
        District
    }
}

public class PrayerTimeManager : MonoBehaviour
{
    public TMP_Dropdown combinedDropdown;
    private Dictionary<string, string> countryIdMap = new Dictionary<string, string>();
    private Dictionary<string, string> cityIdMap = new Dictionary<string, string>();
    private Dictionary<string, string> districtIdMap = new Dictionary<string, string>();
    private string selectedCountry;
    private string selectedCity;
    private string selectedDistrict;

    public TMP_Text labelText;

    public static PrayerTimes currentPrayerTimes; // Private yerine public yap�yoruz

    public static PrayerTimes GetCurrentPrayerTimes() => currentPrayerTimes; // Eri�im y�ntemi ekliyoruz

    void Start()
    {
        StartCoroutine(GetCountries());
        combinedDropdown.onValueChanged.AddListener(OnDropdownChanged); // Listener'� burada ekleyin

        // Varsay�lan de�eri Label'a yazd�r
        combinedDropdown.onValueChanged.AddListener(delegate {
            labelText.text = combinedDropdown.options[combinedDropdown.value].text;
        });
    }

    public void OnDropdownChanged(int index)
    {
        if (index >= 0 && index < combinedDropdown.options.Count)
        {
            string selectedOption = combinedDropdown.options[index].text;
            labelText.text = selectedOption; // Se�ilen de�eri Label'a yazd�r�n

            if (index == 0) return; // "Se�iniz" se�ene�i se�ildiyse i�leme yapma

            DropdownOption selectedDropdownOption = options.Find(option => option.DisplayName == selectedOption);

            if (selectedDropdownOption != null)
            {
                switch (selectedDropdownOption.Type)
                {
                    case DropdownOption.OptionType.Country:
                        selectedCountry = selectedOption;
                        string countryId = selectedDropdownOption.Id;
                        StartCoroutine(GetCities(countryId));
                        break;

                    case DropdownOption.OptionType.City:
                        selectedCity = selectedOption;
                        string cityId = selectedDropdownOption.Id;
                        StartCoroutine(GetDistricts(cityId));
                        break;

                    case DropdownOption.OptionType.District:
                        selectedDistrict = selectedOption;
                        string districtId = selectedDropdownOption.Id;
                        Debug.Log($"Se�ilen il�e: {selectedDistrict}, ID: {districtId}");
                        PlayerPrefs.SetString("SelectedDistrict", selectedDistrict);
                        StartCoroutine(GetPrayerTimes(districtId));
                        break;
                }
            }
        }
    }

    private List<DropdownOption> options = new List<DropdownOption>();

    private IEnumerator GetCountries()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://ezanvakti.herokuapp.com/ulkeler"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = webRequest.downloadHandler.text;
                var countries = JsonConvert.DeserializeObject<List<Country>>(jsonResponse);

                combinedDropdown.ClearOptions();
                options.Clear();

                combinedDropdown.options.Add(new TMP_Dropdown.OptionData("�lke Se�iniz"));

                foreach (var country in countries)
                {
                    var option = new DropdownOption
                    {
                        DisplayName = country.UlkeAdi,
                        Id = country.UlkeID,
                        Type = DropdownOption.OptionType.Country
                    };
                    options.Add(option);
                    combinedDropdown.options.Add(new TMP_Dropdown.OptionData(country.UlkeAdi));
                }

                // Varsay�lan de�eri Label'a yazd�r
                if (combinedDropdown.options.Count > 0)
                {
                    labelText.text = combinedDropdown.options[0].text;
                }
            }
            else
            {
                Debug.LogError("Hata: " + webRequest.error);
            }
        }
    }

    private IEnumerator GetCities(string countryId)
    {
        string url = $"https://ezanvakti.herokuapp.com/sehirler/{countryId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = webRequest.downloadHandler.text;
                var cities = JsonConvert.DeserializeObject<List<City>>(jsonResponse);

                combinedDropdown.ClearOptions();
                options.Clear();

                combinedDropdown.options.Add(new TMP_Dropdown.OptionData("�ehir Se�iniz"));

                foreach (var city in cities)
                {
                    var option = new DropdownOption
                    {
                        DisplayName = city.SehirAdi,
                        Id = city.SehirID,
                        Type = DropdownOption.OptionType.City
                    };
                    options.Add(option);
                    combinedDropdown.options.Add(new TMP_Dropdown.OptionData(city.SehirAdi));
                }

                // Varsay�lan de�eri Label'a yazd�r
                if (combinedDropdown.options.Count > 0)
                {
                    labelText.text = combinedDropdown.options[0].text;
                }
            }
            else
            {
                Debug.LogError("Hata: " + webRequest.error);
            }
        }
    }

    private IEnumerator GetDistricts(string cityId)
    {
        string url = $"https://ezanvakti.herokuapp.com/ilceler/{cityId}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = webRequest.downloadHandler.text;
                var districts = JsonConvert.DeserializeObject<List<District>>(jsonResponse);

                combinedDropdown.ClearOptions();
                options.Clear();

                combinedDropdown.options.Add(new TMP_Dropdown.OptionData("�l�e Se�iniz"));

                foreach (var district in districts)
                {
                    var option = new DropdownOption
                    {
                        DisplayName = district.IlceAdi,
                        Id = district.IlceID,
                        Type = DropdownOption.OptionType.District
                    };
                    options.Add(option);
                    combinedDropdown.options.Add(new TMP_Dropdown.OptionData(district.IlceAdi));
                }

                // Varsay�lan de�eri Label'a yazd�r
                if (combinedDropdown.options.Count > 0)
                {
                    labelText.text = combinedDropdown.options[0].text;
                }
            }
            else
            {
                Debug.LogError("Hata: " + webRequest.error);
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

                        // Namaz vakitlerini PlayerPrefs ile kaydedin
                        PlayerPrefs.SetString("PrayerTimes", JsonConvert.SerializeObject(currentPrayerTimes));
                        PlayerPrefs.SetString("LastUpdateDate", DateTime.Now.ToString("yyyy-MM-dd")); // Son g�ncellemeyi kaydedin

                        LoadNextScene();
                    }
                    else
                    {
                        Debug.LogError("G�n�n tarihi i�in namaz saatleri bulunamad�.");
                    }
                }
                else
                {
                    Debug.LogError("Namaz saatleri verisi bo�.");
                }
            }
            else
            {
                Debug.LogError("Hata: " + webRequest.error);
            }
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    [System.Serializable]
    public class Country
    {
        public string UlkeAdi;
        public string UlkeID;
    }

    [System.Serializable]
    public class City
    {
        public string SehirAdi;
        public string SehirID;
    }

    [System.Serializable]
    public class District
    {
        public string IlceAdi;
        public string IlceID;
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
        // public string AyinSekliURL;
        // public float GreenwichOrtalamaZamani;
        // public string GunesBatis;
        // public string GunesDogus;
        // public string HicriTarihKisa;
        // public string HicriTarihKisaIso8601;
        public string HicriTarihUzun;
        // public string HicriTarihUzunIso8601;
        public string MiladiTarihKisa;
        // public string MiladiTarihKisaIso8601;
        public string MiladiTarihUzun;
        // public string MiladiTarihUzunIso8601;
        // public string KibleSaati;
    }
}