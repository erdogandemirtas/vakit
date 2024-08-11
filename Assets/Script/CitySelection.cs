using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CitySelection : MonoBehaviour
{
    public TMP_Dropdown cityDropdown;

    private List<KeyValuePair<string, string>> cities = new List<KeyValuePair<string, string>>()
    {
        new KeyValuePair<string, string>("Se�iniz", ""),
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
        // Dropdown men�s�n� temizle ve �ehirleri ekle
        cityDropdown.ClearOptions();
        List<string> cityNames = new List<string>();

        foreach (var city in cities)
        {
            cityNames.Add(city.Key);
        }

        // "Se�iniz" her zaman ilk s�rada olacak �ekilde ekle
        cityNames.Sort();
        cityNames.Remove("Se�iniz");
        cityNames.Insert(0, "Se�iniz");

        cityDropdown.AddOptions(cityNames);

        // Varsay�lan olarak "Se�iniz" g�sterilir
        cityDropdown.value = 0;

        // �ehir se�ildi�inde OnCitySelected metodunu �a��r
        cityDropdown.onValueChanged.AddListener(OnCitySelected);
    }

    private void OnCitySelected(int index)
    {
        // "Se�iniz" se�ene�i hari�, kullan�c� bir �ehir se�tiyse i�lem yap�l�r
        if (index > 0)
        {
            // "Se�iniz" birinci s�rada oldu�u i�in index - 1
            string selectedCity = cities[index - 0].Value;
            Debug.Log("Se�ilen �ehir: " + selectedCity); // Log ile se�ilen �ehri kontrol edin

            PlayerPrefs.SetString("SelectedCity", selectedCity);
            PlayerPrefs.Save();

            // Ana sahneye ge�i�
            Debug.Log("Sahneye ge�iliyor..."); // Log ile sahne ge�i�ini kontrol edin
            SceneManager.LoadScene("MainScene"); // Ana sahnenizin ad�
        }
    }
}
