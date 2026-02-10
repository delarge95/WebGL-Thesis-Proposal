using UnityEngine;

namespace WebGL.Core.Utils
{
    /// <summary>
    /// Lightweight WebGL performance profiler.
    /// Displays FPS, frame time, and heap size in console.
    /// Use for testing WebGL builds.
    /// </summary>
    public class WebGLProfiler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private bool logToConsole = true;
        [SerializeField] private bool showOnScreen = false;
        
        private int _frames = 0;
        private float _lastTime = 0f;
        private float _currentFps = 0f;
        private float _currentMs = 0f;
        private long _heapMB = 0;
        
        // Performance thresholds
        private const float WarningFps = 30f;
        private const float CriticalFps = 20f;

        private void Update()
        {
            _frames++;
            
            if (Time.time - _lastTime >= updateInterval)
            {
                float elapsed = Time.time - _lastTime;
                _currentFps = _frames / elapsed;
                _currentMs = 1000f / _currentFps;
                _heapMB = System.GC.GetTotalMemory(false) / (1024 * 1024);
                
                if (logToConsole)
                {
                    string status = GetPerformanceStatus();
                    Debug.Log($"[Profiler] FPS: {_currentFps:F1} | Frame: {_currentMs:F2}ms | Heap: {_heapMB}MB | {status}");
                }
                
                _frames = 0;
                _lastTime = Time.time;
            }
        }

        private string GetPerformanceStatus()
        {
            if (_currentFps >= 55f) return "✓ GOOD";
            if (_currentFps >= WarningFps) return "⚠ OK";
            if (_currentFps >= CriticalFps) return "⚠ WARNING";
            return "✘ CRITICAL";
        }

        private void OnGUI()
        {
            if (!showOnScreen) return;
            
            // Background box
            GUI.Box(new Rect(10, 10, 200, 70), "");
            
            // Style for text
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            
            // Set color based on FPS
            if (_currentFps < CriticalFps)
                style.normal.textColor = Color.red;
            else if (_currentFps < WarningFps)
                style.normal.textColor = Color.yellow;
            else
                style.normal.textColor = Color.green;
            
            GUI.Label(new Rect(15, 15, 190, 20), $"FPS: {_currentFps:F1}", style);
            
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(15, 35, 190, 20), $"Frame: {_currentMs:F2}ms", style);
            GUI.Label(new Rect(15, 55, 190, 20), $"Heap: {_heapMB}MB", style);
        }

        /// <summary>
        /// Get current FPS for external monitoring.
        /// </summary>
        public float GetCurrentFps() => _currentFps;
        
        /// <summary>
        /// Get current frame time in milliseconds.
        /// </summary>
        public float GetFrameTimeMs() => _currentMs;
        
        /// <summary>
        /// Get managed heap size in MB.
        /// </summary>
        public long GetHeapMB() => _heapMB;
    }
}
