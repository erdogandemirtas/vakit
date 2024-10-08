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

    public List<DropdownOption> options = new List<DropdownOption>();

    public IEnumerator GetCountries()
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

    public IEnumerator GetCities(string countryId)
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

    public IEnumerator GetDistricts(string cityId)
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

    public IEnumerator GetPrayerTimes(string districtId)
    {
        // Sadece se�ilen il�e ID'sini sakla
        PlayerPrefs.SetString("SelectedDistrictId", districtId);
        PlayerPrefs.Save();

        // Sonraki sahneye git
        LoadNextScene();

        yield return null; // Coroutine i�in d�n�� de�eri
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
}