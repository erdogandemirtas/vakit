using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Start()
    {
        // FPS sýnýrýný burada belirleyin, örneðin 60 FPS
        Application.targetFrameRate = 60;
    }
}
