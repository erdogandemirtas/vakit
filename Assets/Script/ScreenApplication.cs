using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in ekle

public class ScreenApplication : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Android'de uygulamay� arka plana alma
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

    // CitySelection sahnesine ge�i� yapacak y�ntem
    public void LoadCitySelectionScene()
    {
        SceneManager.LoadScene("CitySelection");
    }
}
