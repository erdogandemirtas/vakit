using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Start()
    {
        // FPS s�n�r�n� burada belirleyin, �rne�in 60 FPS
        Application.targetFrameRate = 60;
    }
}
