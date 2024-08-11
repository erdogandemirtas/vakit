using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NamazVakitleri : MonoBehaviour
{
    public Dropdown cityDropdown;
    public Text imsakText;
    public Text gunesText;
    public Text oglenText;
    public Text ikindiText;
    public Text aksamText;
    public Text yatsiText;

    private string baseApiUrl = "https://api.aladhan.com/v1/timingsByCity";
    private List<KeyValuePair<string, string>> cities = new List<KeyValuePair<string, string>>()
    {
        new KeyValuePair<string, string>("Adana", "Adana"),
        new KeyValuePair<string, string>("Adýyaman", "Adýyaman"),
        new KeyValuePair<string, string>("Afyonkarahisar", "Afyonkarahisar"),
        new KeyValuePair<string, string>("Aðrý", "Aðrý"),
        new KeyValuePair<string, string>("Aksaray", "Aksaray"),
        new KeyValuePair<string, string>("Amasya", "Amasya"),
        new KeyValuePair<string, string>("Ankara", "Ankara"),
        new KeyValuePair<string, string>("Antalya", "Antalya"),
        new KeyValuePair<string, string>("Ardahan", "Ardahan"),
        new KeyValuePair<string, string>("Artvin", "Artvin"),
        new KeyValuePair<string, string>("Aydýn", "Aydýn"),
        new KeyValuePair<string, string>("Balýkesir", "Balýkesir"),
        new KeyValuePair<string, string>("Bartýn", "Bartýn"),
        new KeyValuePair<string, string>("Batman", "Batman"),
        new KeyValuePair<string, string>("Bayburt", "Bayburt"),
        new KeyValuePair<string, string>("Bilecik", "Bilecik"),
        new KeyValuePair<string, string>("Bingöl", "Bingöl"),
        new KeyValuePair<string, string>("Bitlis", "Bitlis"),
        new KeyValuePair<string, string>("Bolu", "Bolu"),
        new KeyValuePair<string, string>("Burdur", "Burdur"),
        new KeyValuePair<string, string>("Bursa", "Bursa"),
        new KeyValuePair<string, string>("Çanakkale", "Çanakkale"),
        new KeyValuePair<string, string>("Çankýrý", "Çankýrý"),
        new KeyValuePair<string, string>("Çorum", "Çorum"),
        new KeyValuePair<string, string>("Denizli", "Denizli"),
        new KeyValuePair<string, string>("Diyarbakýr", "Diyarbakýr"),
        new KeyValuePair<string, string>("Düzce", "Düzce"),
        new KeyValuePair<string, string>("Edirne", "Edirne"),
        new KeyValuePair<string, string>("Elazýð", "Elazýð"),
        new KeyValuePair<string, string>("Erzincan", "Erzincan"),
        new KeyValuePair<string, string>("Erzurum", "Erzurum"),
        new KeyValuePair<string, string>("Eskiþehir", "Eskiþehir"),
        new KeyValuePair<string, string>("Gaziantep", "Gaziantep"),
        new KeyValuePair<string, string>("Giresun", "Giresun"),
        new KeyValuePair<string, string>("Gümüþhane", "Gümüþhane"),
        new KeyValuePair<string, string>("Hakkâri", "Hakkâri"),
        new KeyValuePair<string, string>("Hatay", "Hatay"),
        new KeyValuePair<string, string>("Iðdýr", "Iðdýr"),
        new KeyValuePair<string, string>("Isparta", "Isparta"),
        new KeyValuePair<string, string>("Ýstanbul", "Ýstanbul"),
        new KeyValuePair<string, string>("Ýzmir", "Ýzmir"),
        new KeyValuePair<string, string>("Kahramanmaraþ", "Kahramanmaraþ"),
        new KeyValuePair<string, string>("Karabük", "Karabük"),
        new KeyValuePair<string, string>("Karaman", "Karaman"),
        new KeyValuePair<string, string>("Kars", "Kars"),
        new KeyValuePair<string, string>("Kastamonu", "Kastamonu"),
        new KeyValuePair<string, string>("Kayseri", "Kayseri"),
        new KeyValuePair<string, string>("Kilis", "Kilis"),
        new KeyValuePair<string, string>("Kýrýkkale", "Kýrýkkale"),
        new KeyValuePair<string, string>("Kýrklareli", "Kýrklareli"),
        new KeyValuePair<string, string>("Kýrþehir", "Kýrþehir"),
        new KeyValuePair<string, string>("Kocaeli", "Kocaeli"),
        new KeyValuePair<string, string>("Konya", "Konya"),
        new KeyValuePair<string, string>("Kütahya", "Kütahya"),
        new KeyValuePair<string, string>("Malatya", "Malatya"),
        new KeyValuePair<string, string>("Manisa", "Manisa"),
        new KeyValuePair<string, string>("Mardin", "Mardin"),
        new KeyValuePair<string, string>("Mersin", "Mersin"),
        new KeyValuePair<string, string>("Muðla", "Muðla"),
        new KeyValuePair<string, string>("Muþ", "Muþ"),
        new KeyValuePair<string, string>("Nevþehir", "Nevþehir"),
        new KeyValuePair<string, string>("Niðde", "Niðde"),
        new KeyValuePair<string, string>("Ordu", "Ordu"),
        new KeyValuePair<string, string>("Osmaniye", "Osmaniye"),
        new KeyValuePair<string, string>("Rize", "Rize"),
        new KeyValuePair<string, string>("Sakarya", "Sakarya"),
        new KeyValuePair<string, string>("Samsun", "Samsun"),
        new KeyValuePair<string, string>("Þanlýurfa", "Þanlýurfa"),
        new KeyValuePair<string, string>("Siirt", "Siirt"),
        new KeyValuePair<string, string>("Sinop", "Sinop"),
        new KeyValuePair<string, string>("Sivas", "Sivas"),
        new KeyValuePair<string, string>("Þýrnak", "Þýrnak"),
        new KeyValuePair<string, string>("Tekirdað", "Tekirdað"),
        new KeyValuePair<string, string>("Tokat", "Tokat"),
        new KeyValuePair<string, string>("Trabzon", "Trabzon"),
        new KeyValuePair<string, string>("Tunceli", "Tunceli"),
        new KeyValuePair<string, string>("Uþak", "Uþak"),
        new KeyValuePair<string, string>("Van", "Van"),
        new KeyValuePair<string, string>("Yalova", "Yalova"),
        new KeyValuePair<string, string>("Yozgat", "Yozgat"),
        new KeyValuePair<string, string>("Zonguldak", "Zonguldak")
    };

    private void Start()
    {
        // Þehirler dropdown'ýna eklenir
        cityDropdown.ClearOptions();
        List<string> cityNames = new List<string>();

        foreach (var city in cities)
        {
            cityNames.Add(city.Key);
        }

        cityDropdown.AddOptions(cityNames);

        // Önceki þehir tercihini yükle
        LoadCitySelection();

        // Seçim deðiþtiðinde güncelle
        cityDropdown.onValueChanged.AddListener(delegate {
            SaveCitySelection(cities[cityDropdown.value].Value);
            GetNamazVakitleri(cities[cityDropdown.value].Value);
        });
    }

    private void LoadCitySelection()
    {
        string savedCity = PlayerPrefs.GetString("SelectedCity", cities[0].Value); // Varsayýlan olarak ilk þehir
        int cityIndex = cities.FindIndex(city => city.Value == savedCity);
        if (cityIndex >= 0)
        {
            cityDropdown.value = cityIndex;
            GetNamazVakitleri(savedCity);
        }
    }

    private void SaveCitySelection(string cityName)
    {
        PlayerPrefs.SetString("SelectedCity", cityName);
        PlayerPrefs.Save();
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
            }
            else
            {
                Debug.LogError("API Ýsteði Baþarýsýz: " + www.error);
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
                imsakText.text = jsonData.data.timings.Fajr;  // Ýmsak verisini Fajr yerine alabilirsiniz, düzeltebilirsiniz.
                gunesText.text = jsonData.data.timings.Sunrise;
                oglenText.text = jsonData.data.timings.Dhuhr;
                ikindiText.text = jsonData.data.timings.Asr;
                aksamText.text = jsonData.data.timings.Maghrib;
                yatsiText.text = jsonData.data.timings.Isha;
            }
            else
            {
                Debug.LogError("Geçersiz JSON Yapýsý");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JSON Ayrýþtýrma Hatasý: " + ex.Message);
        }
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
