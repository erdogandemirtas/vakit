using UnityEngine;
using UnityEngine.InputSystem; // Yeni Giriþ Sistemi için ekle
using UnityEngine.SceneManagement; // Sahne yönetimi için ekle

public class ScreenApplication : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Android'de uygulamayý arka plana alma
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
        }
    }

    private void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    // CitySelection sahnesine geçiþ yapacak yöntem
    public void LoadCitySelectionScene()
    {
        SceneManager.LoadScene("CitySelection");
    }
}
