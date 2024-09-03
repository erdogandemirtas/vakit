using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Start()
    {
        // FPS sýnýrýný burada belirleyin, örneðin 120 FPS
        Application.targetFrameRate = 120;
    }
}
