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
        new KeyValuePair<string, string>("Ad�yaman", "Ad�yaman"),
        new KeyValuePair<string, string>("Afyonkarahisar", "Afyonkarahisar"),
        new KeyValuePair<string, string>("A�r�", "A�r�"),
        new KeyValuePair<string, string>("Aksaray", "Aksaray"),
        new KeyValuePair<string, string>("Amasya", "Amasya"),
        new KeyValuePair<string, string>("Ankara", "Ankara"),
        new KeyValuePair<string, string>("Antalya", "Antalya"),
        new KeyValuePair<string, string>("Ardahan", "Ardahan"),
        new KeyValuePair<string, string>("Artvin", "Artvin"),
        new KeyValuePair<string, string>("Ayd�n", "Ayd�n"),
        new KeyValuePair<string, string>("Bal�kesir", "Bal�kesir"),
        new KeyValuePair<string, string>("Bart�n", "Bart�n"),
        new KeyValuePair<string, string>("Batman", "Batman"),
        new KeyValuePair<string, string>("Bayburt", "Bayburt"),
        new KeyValuePair<string, string>("Bilecik", "Bilecik"),
        new KeyValuePair<string, string>("Bing�l", "Bing�l"),
        new KeyValuePair<string, string>("Bitlis", "Bitlis"),
        new KeyValuePair<string, string>("Bolu", "Bolu"),
        new KeyValuePair<string, string>("Burdur", "Burdur"),
        new KeyValuePair<string, string>("Bursa", "Bursa"),
        new KeyValuePair<string, string>("�anakkale", "�anakkale"),
        new KeyValuePair<string, string>("�ank�r�", "�ank�r�"),
        new KeyValuePair<string, string>("�orum", "�orum"),
        new KeyValuePair<string, string>("Denizli", "Denizli"),
        new KeyValuePair<string, string>("Diyarbak�r", "Diyarbak�r"),
        new KeyValuePair<string, string>("D�zce", "D�zce"),
        new KeyValuePair<string, string>("Edirne", "Edirne"),
        new KeyValuePair<string, string>("Elaz��", "Elaz��"),
        new KeyValuePair<string, string>("Erzincan", "Erzincan"),
        new KeyValuePair<string, string>("Erzurum", "Erzurum"),
        new KeyValuePair<string, string>("Eski�ehir", "Eski�ehir"),
        new KeyValuePair<string, string>("Gaziantep", "Gaziantep"),
        new KeyValuePair<string, string>("Giresun", "Giresun"),
        new KeyValuePair<string, string>("G�m��hane", "G�m��hane"),
        new KeyValuePair<string, string>("Hakk�ri", "Hakk�ri"),
        new KeyValuePair<string, string>("Hatay", "Hatay"),
        new KeyValuePair<string, string>("I�d�r", "I�d�r"),
        new KeyValuePair<string, string>("Isparta", "Isparta"),
        new KeyValuePair<string, string>("�stanbul", "�stanbul"),
        new KeyValuePair<string, string>("�zmir", "�zmir"),
        new KeyValuePair<string, string>("Kahramanmara�", "Kahramanmara�"),
        new KeyValuePair<string, string>("Karab�k", "Karab�k"),
        new KeyValuePair<string, string>("Karaman", "Karaman"),
        new KeyValuePair<string, string>("Kars", "Kars"),
        new KeyValuePair<string, string>("Kastamonu", "Kastamonu"),
        new KeyValuePair<string, string>("Kayseri", "Kayseri"),
        new KeyValuePair<string, string>("Kilis", "Kilis"),
        new KeyValuePair<string, string>("K�r�kkale", "K�r�kkale"),
        new KeyValuePair<string, string>("K�rklareli", "K�rklareli"),
        new KeyValuePair<string, string>("K�r�ehir", "K�r�ehir"),
        new KeyValuePair<string, string>("Kocaeli", "Kocaeli"),
        new KeyValuePair<string, string>("Konya", "Konya"),
        new KeyValuePair<string, string>("K�tahya", "K�tahya"),
        new KeyValuePair<string, string>("Malatya", "Malatya"),
        new KeyValuePair<string, string>("Manisa", "Manisa"),
        new KeyValuePair<string, string>("Mardin", "Mardin"),
        new KeyValuePair<string, string>("Mersin", "Mersin"),
        new KeyValuePair<string, string>("Mu�la", "Mu�la"),
        new KeyValuePair<string, string>("Mu�", "Mu�"),
        new KeyValuePair<string, string>("Nev�ehir", "Nev�ehir"),
        new KeyValuePair<string, string>("Ni�de", "Ni�de"),
        new KeyValuePair<string, string>("Ordu", "Ordu"),
        new KeyValuePair<string, string>("Osmaniye", "Osmaniye"),
        new KeyValuePair<string, string>("Rize", "Rize"),
        new KeyValuePair<string, string>("Sakarya", "Sakarya"),
        new KeyValuePair<string, string>("Samsun", "Samsun"),
        new KeyValuePair<string, string>("�anl�urfa", "�anl�urfa"),
        new KeyValuePair<string, string>("Siirt", "Siirt"),
        new KeyValuePair<string, string>("Sinop", "Sinop"),
        new KeyValuePair<string, string>("Sivas", "Sivas"),
        new KeyValuePair<string, string>("��rnak", "��rnak"),
        new KeyValuePair<string, string>("Tekirda�", "Tekirda�"),
        new KeyValuePair<string, string>("Tokat", "Tokat"),
        new KeyValuePair<string, string>("Trabzon", "Trabzon"),
        new KeyValuePair<string, string>("Tunceli", "Tunceli"),
        new KeyValuePair<string, string>("U�ak", "U�ak"),
        new KeyValuePair<string, string>("Van", "Van"),
        new KeyValuePair<string, string>("Yalova", "Yalova"),
        new KeyValuePair<string, string>("Yozgat", "Yozgat"),
        new KeyValuePair<string, string>("Zonguldak", "Zonguldak")
    };

    private void Start()
    {
        // �ehirler dropdown'�na eklenir
        cityDropdown.ClearOptions();
        List<string> cityNames = new List<string>();

        foreach (var city in cities)
        {
            cityNames.Add(city.Key);
        }

        cityDropdown.AddOptions(cityNames);

        // �nceki �ehir tercihini y�kle
        LoadCitySelection();

        // Se�im de�i�ti�inde g�ncelle
        cityDropdown.onValueChanged.AddListener(delegate {
            SaveCitySelection(cities[cityDropdown.value].Value);
            GetNamazVakitleri(cities[cityDropdown.value].Value);
        });
    }

    private void LoadCitySelection()
    {
        string savedCity = PlayerPrefs.GetString("SelectedCity", cities[0].Value); // Varsay�lan olarak ilk �ehir
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
                Debug.LogError("API �ste�i Ba�ar�s�z: " + www.error);
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
                imsakText.text = jsonData.data.timings.Fajr;  // �msak verisini Fajr yerine alabilirsiniz, d�zeltebilirsiniz.
                gunesText.text = jsonData.data.timings.Sunrise;
                oglenText.text = jsonData.data.timings.Dhuhr;
                ikindiText.text = jsonData.data.timings.Asr;
                aksamText.text = jsonData.data.timings.Maghrib;
                yatsiText.text = jsonData.data.timings.Isha;
            }
            else
            {
                Debug.LogError("Ge�ersiz JSON Yap�s�");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JSON Ayr��t�rma Hatas�: " + ex.Message);
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
