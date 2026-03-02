using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Adaptive resolution scaling for WebGL.
    /// Monitors FPS and adjusts URP renderScale to maintain target framerate.
    /// Uses reflection to avoid hard dependency on URP assembly (Core.asmdef has no URP ref).
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

        // Reflection-based access to URP renderScale (avoids assembly dependency)
        private RenderPipelineAsset _pipelineAsset;
        private PropertyInfo _renderScaleProp;

        public float CurrentScale => currentScale;

        private void Start()
        {
            _pipelineAsset = GraphicsSettings.currentRenderPipeline;
            if (_pipelineAsset == null)
            {
                Debug.LogWarning("[QualityManager] No render pipeline asset — adaptive resolution disabled.");
                enabled = false;
                return;
            }

            // Find renderScale property via reflection (works with URP without compile-time dependency)
            _renderScaleProp = _pipelineAsset.GetType().GetProperty("renderScale",
                BindingFlags.Public | BindingFlags.Instance);

            if (_renderScaleProp == null)
            {
                Debug.LogWarning("[QualityManager] Pipeline asset has no renderScale property — disabled.");
                enabled = false;
                return;
            }

            currentScale = (float)_renderScaleProp.GetValue(_pipelineAsset);
            _lastFrameCount = Time.frameCount;
            _lastCheckTime = Time.realtimeSinceStartup;
            InvokeRepeating(nameof(CheckPerformance), checkInterval, checkInterval);
            Debug.Log($"[QualityManager] Started — initial renderScale: {currentScale:0.00}");
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
                _renderScaleProp.SetValue(_pipelineAsset, currentScale);
                Debug.Log($"[QualityManager] FPS: {fps:0.0} → renderScale: {currentScale:0.00}");
            }
        }
    }
}
