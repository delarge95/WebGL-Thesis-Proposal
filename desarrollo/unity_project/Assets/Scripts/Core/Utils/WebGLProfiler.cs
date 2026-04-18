using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Utils
{
    [Serializable]
    public struct WebGLProfilerSessionSummary
    {
        public string sessionId;
        public string startedAtUtc;
        public float durationSeconds;
        public int sampleCount;
        public float averageFps;
        public float averageFrameMs;
        public float worstFrameMs;
        public float lowestSampledFps;
        public float averageHeapMb;
        public long peakHeapMb;
    }

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
        [SerializeField] private int maxCompletedSessions = 32;

        private int _lastFrameCount;
        private float _lastTime;
        private float _currentFps;
        private float _currentMs;
        private long _heapMB;

        private readonly List<WebGLProfilerSessionSummary> _completedSessions = new List<WebGLProfilerSessionSummary>();
        private bool _sessionActive;
        private string _activeSessionId = string.Empty;
        private DateTime _activeSessionStartedAtUtc;
        private double _sessionStartRealtime;
        private int _sessionSampleCount;
        private double _sessionTotalFps;
        private double _sessionTotalFrameMs;
        private double _sessionTotalHeapMb;
        private float _sessionWorstFrameMs;
        private float _sessionLowestFps;
        private long _sessionPeakHeapMb;

        private const float WarningFps = 30f;
        private const float CriticalFps = 20f;

        private void Start()
        {
            _lastTime = Time.realtimeSinceStartup;
            _lastFrameCount = Time.frameCount;
            InvokeRepeating(nameof(SamplePerformance), updateInterval, updateInterval);
        }

        private void SamplePerformance()
        {
            int frames = Time.frameCount - _lastFrameCount;
            float elapsed = Time.realtimeSinceStartup - _lastTime;
            if (elapsed <= 0f)
            {
                return;
            }

            _currentFps = frames / elapsed;
            _currentMs = _currentFps > 0f ? 1000f / _currentFps : 0f;
            _heapMB = GC.GetTotalMemory(false) / (1024 * 1024);

            CaptureSessionSample();

            if (logToConsole)
            {
                string status = GetPerformanceStatus();
                Debug.Log($"[Profiler] FPS: {_currentFps:F1} | Frame: {_currentMs:F2}ms | Heap: {_heapMB}MB | {status}");
            }

            _lastFrameCount = Time.frameCount;
            _lastTime = Time.realtimeSinceStartup;
        }

        private string GetPerformanceStatus()
        {
            if (_currentFps >= 55f)
            {
                return "GOOD";
            }

            if (_currentFps >= WarningFps)
            {
                return "OK";
            }

            if (_currentFps >= CriticalFps)
            {
                return "WARNING";
            }

            return "CRITICAL";
        }

        private void CaptureSessionSample()
        {
            if (!_sessionActive)
            {
                return;
            }

            _sessionSampleCount++;
            _sessionTotalFps += _currentFps;
            _sessionTotalFrameMs += _currentMs;
            _sessionTotalHeapMb += _heapMB;

            if (_sessionSampleCount == 1)
            {
                _sessionWorstFrameMs = _currentMs;
                _sessionLowestFps = _currentFps;
                _sessionPeakHeapMb = _heapMB;
                return;
            }

            _sessionWorstFrameMs = Mathf.Max(_sessionWorstFrameMs, _currentMs);
            _sessionLowestFps = Mathf.Min(_sessionLowestFps, _currentFps);
            _sessionPeakHeapMb = Math.Max(_sessionPeakHeapMb, _heapMB);
        }

        private void OnGUI()
        {
            if (!showOnScreen)
            {
                return;
            }

            GUI.Box(new Rect(10, 10, 200, 70), string.Empty);

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            if (_currentFps < CriticalFps)
            {
                style.normal.textColor = Color.red;
            }
            else if (_currentFps < WarningFps)
            {
                style.normal.textColor = Color.yellow;
            }
            else
            {
                style.normal.textColor = Color.green;
            }

            GUI.Label(new Rect(15, 15, 190, 20), $"FPS: {_currentFps:F1}", style);

            style.normal.textColor = Color.white;
            GUI.Label(new Rect(15, 35, 190, 20), $"Frame: {_currentMs:F2}ms", style);
            GUI.Label(new Rect(15, 55, 190, 20), $"Heap: {_heapMB}MB", style);
        }

        public void BeginSession(string sessionId)
        {
            _activeSessionId = string.IsNullOrWhiteSpace(sessionId)
                ? $"session_{DateTime.UtcNow:yyyyMMdd_HHmmss}"
                : sessionId.Trim();

            _activeSessionStartedAtUtc = DateTime.UtcNow;
            _sessionActive = true;
            _sessionStartRealtime = Time.realtimeSinceStartupAsDouble;
            _sessionSampleCount = 0;
            _sessionTotalFps = 0d;
            _sessionTotalFrameMs = 0d;
            _sessionTotalHeapMb = 0d;
            _sessionWorstFrameMs = 0f;
            _sessionLowestFps = 0f;
            _sessionPeakHeapMb = 0L;
        }

        public WebGLProfilerSessionSummary EndSession()
        {
            if (!_sessionActive)
            {
                return default;
            }

            _sessionActive = false;

            WebGLProfilerSessionSummary summary = new WebGLProfilerSessionSummary
            {
                sessionId = _activeSessionId,
                startedAtUtc = _activeSessionStartedAtUtc.ToString("o"),
                durationSeconds = (float)Math.Max(Time.realtimeSinceStartupAsDouble - _sessionStartRealtime, 0d),
                sampleCount = _sessionSampleCount,
                averageFps = _sessionSampleCount > 0 ? (float)(_sessionTotalFps / _sessionSampleCount) : _currentFps,
                averageFrameMs = _sessionSampleCount > 0 ? (float)(_sessionTotalFrameMs / _sessionSampleCount) : _currentMs,
                worstFrameMs = _sessionSampleCount > 0 ? _sessionWorstFrameMs : _currentMs,
                lowestSampledFps = _sessionSampleCount > 0 ? _sessionLowestFps : _currentFps,
                averageHeapMb = _sessionSampleCount > 0 ? (float)(_sessionTotalHeapMb / _sessionSampleCount) : _heapMB,
                peakHeapMb = _sessionSampleCount > 0 ? _sessionPeakHeapMb : _heapMB
            };

            _completedSessions.Add(summary);
            while (_completedSessions.Count > Mathf.Max(1, maxCompletedSessions))
            {
                _completedSessions.RemoveAt(0);
            }

            _activeSessionId = string.Empty;
            return summary;
        }

        public bool TryGetLastCompletedSession(out WebGLProfilerSessionSummary sessionSummary)
        {
            if (_completedSessions.Count == 0)
            {
                sessionSummary = default;
                return false;
            }

            sessionSummary = _completedSessions[_completedSessions.Count - 1];
            return true;
        }

        public IReadOnlyList<WebGLProfilerSessionSummary> GetCompletedSessions()
        {
            return _completedSessions;
        }

        public void ClearCompletedSessions()
        {
            _completedSessions.Clear();
        }

        public string BuildCompletedSessionsJson()
        {
            return JsonUtility.ToJson(new CompletedSessionsWrapper
            {
                sessions = _completedSessions
            }, true);
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

        [Serializable]
        private sealed class CompletedSessionsWrapper
        {
            public List<WebGLProfilerSessionSummary> sessions = new List<WebGLProfilerSessionSummary>();
        }
    }
}
