using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CitySlider : MonoBehaviour
{
    public Slider citySlider;
    public TextMeshProUGUI cityText; // Slider'ýn yanýnda þehir adýný göstermek için bir Text bileþeni
    public Button selectButton; // Seçim butonu

    private List<string> cities = new List<string>()
    {
        "Seçiniz", "Adana", "Adýyaman", "Afyonkarahisar", "Aðrý", "Aksaray", "Amasya",
        "Ankara", "Antalya", "Ardahan", "Artvin", "Aydýn", "Balýkesir", "Bartýn",
        "Batman", "Bayburt", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur",
        "Bursa", "Çanakkale", "Çankýrý", "Çorum", "Denizli", "Diyarbakýr", "Düzce",
        "Edirne", "Elazýð", "Erzincan", "Erzurum", "Eskiþehir", "Gaziantep", "Giresun",
        "Gümüþhane", "Hakkâri", "Hatay", "Iðdýr", "Isparta", "Ýstanbul", "Ýzmir",
        "Kahramanmaraþ", "Karabük", "Karaman", "Kars", "Kastamonu", "Kayseri", "Kilis",
        "Kýrýkkale", "Kýrklareli", "Kýrþehir", "Kocaeli", "Konya", "Kütahya", "Malatya",
        "Manisa", "Mardin", "Mersin", "Muðla", "Muþ", "Nevþehir", "Niðde", "Ordu",
        "Osmaniye", "Rize", "Sakarya", "Samsun", "Þanlýurfa", "Siirt", "Sinop",
        "Sivas", "Þýrnak", "Tekirdað", "Tokat", "Trabzon", "Tunceli", "Uþak",
        "Van", "Yalova", "Yozgat", "Zonguldak"
    };

    private void Start()
    {
        // Slider ayarlarýný yapýlandýr
        citySlider.minValue = 0;
        citySlider.maxValue = cities.Count - 1;
        citySlider.value = 0;

        // Ýlk þehir adýný göster
        UpdateCityText();

        // Slider deðiþtiðinde þehir adýný güncelle
        citySlider.onValueChanged.AddListener(delegate { UpdateCityText(); });

        // Butona týklandýðýnda sahneye geçiþ yap
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

        // Seçilen þehir bilgisini kaydet
        PlayerPrefs.SetString("SelectedCity", selectedCity);
        PlayerPrefs.Save();

        // Þehir seçildikten sonra sahneye geçiþ
        if (index > 0) // "Seçiniz" seçili deðilse geçiþ yap
        {
            Debug.Log("Sahneye geçiliyor...");
            SceneManager.LoadScene("MainScene"); // Ana sahnenizin adý
        }
    }
}
