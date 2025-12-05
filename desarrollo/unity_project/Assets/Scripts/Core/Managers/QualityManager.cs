using UnityEngine;
using UnityEngine.Rendering;

namespace WebGL.Core.Managers
{
    public class QualityManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float targetFPS = 30f;
        [SerializeField] private float checkInterval = 2f;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.0f;

        private float currentScale = 1.0f;
        private float timeSinceLastCheck = 0f;
        private float frameCount = 0f;
        private float dt = 0f;

        private void Start()
        {
            // Initial setting
            ScalableBufferManager.ResizeBuffers(maxScale, maxScale);
        }

        private void Update()
        {
            // Calculate FPS
            frameCount++;
            dt += Time.unscaledDeltaTime;

            if (dt > checkInterval)
            {
                float fps = frameCount / dt;
                AdjustQuality(fps);

                frameCount = 0;
                dt = 0;
            }
        }

        private void AdjustQuality(float fps)
        {
            if (fps < targetFPS - 5)
            {
                // Decrease resolution
                currentScale = Mathf.Max(minScale, currentScale - 0.1f);
                ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
                Debug.Log($"[QualityManager] Low FPS ({fps:0.0}). Reducing scale to {currentScale}");
            }
            else if (fps > targetFPS + 10 && currentScale < maxScale)
            {
                // Increase resolution
                currentScale = Mathf.Min(maxScale, currentScale + 0.05f);
                ScalableBufferManager.ResizeBuffers(currentScale, currentScale);
                Debug.Log($"[QualityManager] Good FPS ({fps:0.0}). Increasing scale to {currentScale}");
            }
        }
    }
}
