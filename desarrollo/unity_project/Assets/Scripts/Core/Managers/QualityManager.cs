using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Adaptive resolution scaling for WebGL.
    /// Monitors FPS and adjusts URP renderScale to maintain target framerate.
    /// ScalableBufferManager is NOT supported in WebGL — uses URP asset directly.
    /// </summary>
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
        private UniversalRenderPipelineAsset _urpAsset;

        public float CurrentScale => currentScale;

        private void Start()
        {
            _urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (_urpAsset == null)
            {
                Debug.LogWarning("[QualityManager] No URP asset found — adaptive resolution disabled.");
                enabled = false;
                return;
            }

            currentScale = _urpAsset.renderScale;
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
            float prevScale = currentScale;

            if (fps < targetFPS - 5)
            {
                currentScale = Mathf.Max(minScale, currentScale - 0.1f);
            }
            else if (fps > targetFPS + 10 && currentScale < maxScale)
            {
                currentScale = Mathf.Min(maxScale, currentScale + 0.05f);
            }

            if (!Mathf.Approximately(prevScale, currentScale))
            {
                _urpAsset.renderScale = currentScale;
                Debug.Log($"[QualityManager] FPS: {fps:0.0} → renderScale: {currentScale:0.00}");
            }
        }
    }
}
