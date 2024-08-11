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
        new KeyValuePair<string, string>("Seçiniz", ""),
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
        // Dropdown menüsünü temizle ve þehirleri ekle
        cityDropdown.ClearOptions();
        List<string> cityNames = new List<string>();

        foreach (var city in cities)
        {
            cityNames.Add(city.Key);
        }

        // "Seçiniz" her zaman ilk sýrada olacak þekilde ekle
        cityNames.Sort();
        cityNames.Remove("Seçiniz");
        cityNames.Insert(0, "Seçiniz");

        cityDropdown.AddOptions(cityNames);

        // Varsayýlan olarak "Seçiniz" gösterilir
        cityDropdown.value = 0;

        // Þehir seçildiðinde OnCitySelected metodunu çaðýr
        cityDropdown.onValueChanged.AddListener(OnCitySelected);
    }

    private void OnCitySelected(int index)
    {
        // "Seçiniz" seçeneði hariç, kullanýcý bir þehir seçtiyse iþlem yapýlýr
        if (index > 0)
        {
            // "Seçiniz" birinci sýrada olduðu için index - 1
            string selectedCity = cities[index - 0].Value;
            Debug.Log("Seçilen Þehir: " + selectedCity); // Log ile seçilen þehri kontrol edin

            PlayerPrefs.SetString("SelectedCity", selectedCity);
            PlayerPrefs.Save();

            // Ana sahneye geçiþ
            Debug.Log("Sahneye geçiliyor..."); // Log ile sahne geçiþini kontrol edin
            SceneManager.LoadScene("MainScene"); // Ana sahnenizin adý
        }
    }
}
