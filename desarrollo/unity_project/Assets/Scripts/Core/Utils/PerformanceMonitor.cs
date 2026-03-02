using UnityEngine;
using WebGL.Core.Managers;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class PerformanceMonitor : Singleton<PerformanceMonitor>
    {
        [Header("Settings")]
        [SerializeField] private bool showOverlay = false;
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;

        [Header("Thresholds")]
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private int warningFPS = 30;
        // [SerializeField] private int criticalFPS = 15;

        private float deltaTime = 0f;
        private float fps = 0f;
        private float timer = 0f;

        // Memory tracking
        private long lastMemory = 0;
        private long peakMemory = 0;

        // Draw call tracking
        // private int lastDrawCalls = 0;
        // private int lastTriangles = 0;

        public float FPS => fps;
        public long MemoryMB => System.GC.GetTotalMemory(false) / (1024 * 1024);
        public long PeakMemoryMB => peakMemory / (1024 * 1024);

        private void Update()
        {
            // Toggle overlay
            if (Input.GetKeyDown(toggleKey))
            {
                showOverlay = !showOverlay;
            }

            // Calculate FPS
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            
            timer += Time.unscaledDeltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                fps = 1f / deltaTime;
                
                // Track memory
                lastMemory = System.GC.GetTotalMemory(false);
                if (lastMemory > peakMemory) peakMemory = lastMemory;

                // Auto quality adjustment
                if (ServiceLocator.TryGet<QualityManager>(out _))
                {
                    // QualityManager handles its own adjustment via ServiceLocator
                }
            }
        }

        private void OnGUI()
        {
            if (!showOverlay) return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 14;
            style.padding = new RectOffset(10, 10, 5, 5);

            // Background
            GUI.Box(new Rect(10, 10, 200, 120), "");

            // FPS with color coding
            Color fpsColor = fps >= targetFPS ? Color.green : (fps >= warningFPS ? Color.yellow : Color.red);
            style.normal.textColor = fpsColor;
            GUI.Label(new Rect(15, 15, 190, 20), $"FPS: {fps:F1}", style);

            // Memory
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(15, 35, 190, 20), $"Memory: {MemoryMB} MB", style);
            GUI.Label(new Rect(15, 55, 190, 20), $"Peak: {PeakMemoryMB} MB", style);

            // Quality level
            GUI.Label(new Rect(15, 75, 190, 20), $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}", style);

            // Resolution scale - Disabled for Unity 6 compatibility
            // float scale = UnityEngine.Rendering.ScalableBufferManager.heightScaleFactor;
            float scale = 1.0f;
            GUI.Label(new Rect(15, 95, 190, 20), $"Resolution: {(scale * 100):F0}%", style);

            // Hotkey hint
            style.fontSize = 10;
            style.normal.textColor = new Color(1, 1, 1, 0.5f);
            GUI.Label(new Rect(15, 115, 190, 15), "Press F3 to hide", style);
        }

        public string GetPerformanceReport()
        {
            return $"FPS: {fps:F1}\n" +
                   $"Memory: {MemoryMB} MB (Peak: {PeakMemoryMB} MB)\n" +
                   $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\n" +
                   $"Resolution Scale: 100%"; // Fixed for Unity 6
        }
    }
}
