using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Ensures smooth performance on Quest 3 hardware.
/// Monitors frame rate, manages optimization, and logs metrics.
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    public float targetFrameRate = 72f;
    public float checkInterval = 2f;
    private float timer;
    private int frameCount;
    private float lastFps;

    private void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        timer = 0f;
        frameCount = 0;
    }

    private void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;
        if (timer >= checkInterval)
        {
            lastFps = frameCount / timer;
            Debug.Log($"[PerformanceMonitor] FPS: {lastFps:F1}");
            AdjustQuality(lastFps);
            timer = 0f;
            frameCount = 0;
        }
    }

    private void AdjustQuality(float fps)
    {
        if (fps < targetFrameRate - 10)
        {
            // Lower quality if possible
            if (QualitySettings.GetQualityLevel() > 0)
            {
                QualitySettings.DecreaseLevel(true);
                Debug.Log("[PerformanceMonitor] Lowered quality level");
            }
        }
        else if (fps > targetFrameRate + 10)
        {
            // Raise quality if possible
            if (QualitySettings.GetQualityLevel() < QualitySettings.names.Length - 1)
            {
                QualitySettings.IncreaseLevel(true);
                Debug.Log("[PerformanceMonitor] Increased quality level");
            }
        }
    }
} 