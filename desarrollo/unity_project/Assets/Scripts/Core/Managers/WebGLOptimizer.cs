using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class WebGLOptimizer : PersistentSingleton<WebGLOptimizer>
    {
        [Header("Settings")]
        [SerializeField] private bool autoOptimize = true;
        [SerializeField] private bool reduceTextureQuality = false;
        [SerializeField] private bool disableShadowsOnMobile = true;
        [SerializeField] private float targetFrameRate = 60f;

        [Header("Memory Management")]
        [SerializeField] private bool enableMemoryWarnings = true;
        [SerializeField] private int memoryWarningThresholdMB = 512;
        [SerializeField] private int memoryCriticalThresholdMB = 800;

        private bool isWebGL = false;
        private bool isMobile = false;

        protected override void Awake()
        {
            base.Awake();
            
            #if UNITY_WEBGL
            isWebGL = true;
            #endif

            // Detect mobile via User Agent in WebGL or platform in Editor
            #if UNITY_WEBGL && !UNITY_EDITOR
            // Would need JavaScript interop for proper mobile detection
            isMobile = false;
            #else
            isMobile = Application.isMobilePlatform;
            #endif

            if (autoOptimize) ApplyOptimizations();
        }

        private void Start()
        {
            // Set target frame rate
            Application.targetFrameRate = (int)targetFrameRate;
            
            // WebGL specific optimizations
            if (isWebGL)
            {
                // Reduce GC allocations
                System.GC.Collect();
                
                // Disable VSync for WebGL (browser controls it)
                QualitySettings.vSyncCount = 0;
            }
        }

        private void ApplyOptimizations()
        {
            Debug.Log("[WebGLOptimizer] Applying optimizations...");

            // Texture optimizations
            if (reduceTextureQuality || isMobile)
            {
                QualitySettings.globalTextureMipmapLimit = 1; // Use second mip level
            }

            // Shadow optimizations for mobile
            if (disableShadowsOnMobile && isMobile)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                Debug.Log("[WebGLOptimizer] Shadows disabled for mobile");
            }

            // LOD optimizations
            QualitySettings.lodBias = isWebGL ? 1.5f : 1f;

            // Particle optimizations
            QualitySettings.particleRaycastBudget = isWebGL ? 64 : 256;

            Debug.Log($"[WebGLOptimizer] Platform: WebGL={isWebGL}, Mobile={isMobile}");
        }

        private void Update()
        {
            if (!enableMemoryWarnings) return;

            // Check memory periodically
            if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
            {
                CheckMemory();
            }
        }

        private void CheckMemory()
        {
            long memoryMB = System.GC.GetTotalMemory(false) / (1024 * 1024);

            if (memoryMB > memoryCriticalThresholdMB)
            {
                Debug.LogWarning($"[WebGLOptimizer] CRITICAL: Memory at {memoryMB}MB. Forcing GC.");
                System.GC.Collect();
                Resources.UnloadUnusedAssets();

                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification("High memory usage detected", 5f);
                }
            }
            else if (memoryMB > memoryWarningThresholdMB)
            {
                Debug.LogWarning($"[WebGLOptimizer] Warning: Memory at {memoryMB}MB");
            }
        }

        public void ForceGarbageCollection()
        {
            Debug.Log("[WebGLOptimizer] Forcing garbage collection...");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public void SetQualityPreset(int level)
        {
            QualitySettings.SetQualityLevel(level, true);
            Debug.Log($"[WebGLOptimizer] Quality set to: {QualitySettings.names[level]}");
        }
    }
}
