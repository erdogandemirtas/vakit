using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CitySlider : MonoBehaviour
{
    public Slider citySlider;
    public TextMeshProUGUI cityText; // Slider'�n yan�nda �ehir ad�n� g�stermek i�in bir Text bile�eni
    public Button selectButton; // Se�im butonu

    private List<string> cities = new List<string>()
    {
        "Se�iniz", "Adana", "Ad�yaman", "Afyonkarahisar", "A�r�", "Aksaray", "Amasya",
        "Ankara", "Antalya", "Ardahan", "Artvin", "Ayd�n", "Bal�kesir", "Bart�n",
        "Batman", "Bayburt", "Bilecik", "Bing�l", "Bitlis", "Bolu", "Burdur",
        "Bursa", "�anakkale", "�ank�r�", "�orum", "Denizli", "Diyarbak�r", "D�zce",
        "Edirne", "Elaz��", "Erzincan", "Erzurum", "Eski�ehir", "Gaziantep", "Giresun",
        "G�m��hane", "Hakk�ri", "Hatay", "I�d�r", "Isparta", "�stanbul", "�zmir",
        "Kahramanmara�", "Karab�k", "Karaman", "Kars", "Kastamonu", "Kayseri", "Kilis",
        "K�r�kkale", "K�rklareli", "K�r�ehir", "Kocaeli", "Konya", "K�tahya", "Malatya",
        "Manisa", "Mardin", "Mersin", "Mu�la", "Mu�", "Nev�ehir", "Ni�de", "Ordu",
        "Osmaniye", "Rize", "Sakarya", "Samsun", "�anl�urfa", "Siirt", "Sinop",
        "Sivas", "��rnak", "Tekirda�", "Tokat", "Trabzon", "Tunceli", "U�ak",
        "Van", "Yalova", "Yozgat", "Zonguldak"
    };

    private void Start()
    {
        // Slider ayarlar�n� yap�land�r
        citySlider.minValue = 0;
        citySlider.maxValue = cities.Count - 1;
        citySlider.value = 0;

        // �lk �ehir ad�n� g�ster
        UpdateCityText();

        // Slider de�i�ti�inde �ehir ad�n� g�ncelle
        citySlider.onValueChanged.AddListener(delegate { UpdateCityText(); });

        // Butona t�kland���nda sahneye ge�i� yap
        selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    private void UpdateCityText()
    {
        int index = Mathf.RoundToInt(citySlider.value);
        cityText.text = cities[index];
    }

    private void OnSelectButtonClick()
    {
        int index = Mathf.RoundToInt(citySlider.value);
        string selectedCity = cities[index];

        // Se�ilen �ehir bilgisini kaydet
        PlayerPrefs.SetString("SelectedCity", selectedCity);
        PlayerPrefs.Save();

        // �ehir se�ildikten sonra sahneye ge�i�
        if (index > 0) // "Se�iniz" se�ili de�ilse ge�i� yap
        {
            Debug.Log("Sahneye ge�iliyor...");
            SceneManager.LoadScene("MainScene"); // Ana sahnenizin ad�
        }
    }
}
