using UnityEngine;
using UnityEngine.Rendering;

using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class QualityManager : Singleton<QualityManager>
    {
        [Header("Settings")]
        [SerializeField] private float targetFPS = 30f;
        [SerializeField] private float checkInterval = 2f;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.0f;

        private float currentScale = 1.0f;
        private int _lastFrameCount;
        private float _lastCheckTime;

        private void Start()
        {
            _lastFrameCount = Time.frameCount;
            _lastCheckTime = Time.realtimeSinceStartup;
            InvokeRepeating(nameof(CheckPerformance), checkInterval, checkInterval);
        }

        private void CheckPerformance()
        {
            int frames = Time.frameCount - _lastFrameCount;
            float elapsed = Time.realtimeSinceStartup - _lastCheckTime;
            if (elapsed <= 0f) return;

            float fps = frames / elapsed;
            AdjustQuality(fps);

            _lastFrameCount = Time.frameCount;
            _lastCheckTime = Time.realtimeSinceStartup;
        }

        private void AdjustQuality(float fps)
        {
            if (fps < targetFPS - 5)
            {
                // Decrease resolution
                currentScale = Mathf.Max(minScale, currentScale - 0.1f);
                // ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
                Debug.Log($"[QualityManager] Low FPS ({fps:0.0}). Reducing scale to {currentScale}");
            }
            else if (fps > targetFPS + 10 && currentScale < maxScale)
            {
                // Increase resolution
                currentScale = Mathf.Min(maxScale, currentScale + 0.05f);
                // ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
                Debug.Log($"[QualityManager] Good FPS ({fps:0.0}). Increasing scale to {currentScale}");
            }
        }
    }
}
