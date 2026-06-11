using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;
using WebGL.Core.Managers;

namespace WebGL.Core.Utils
{
    [Serializable]
    public struct WebGLProfilerSessionSummary
    {
        public string sessionId;
        public string scenario;
        public string startedAtUtc;
        public string endedAtUtc;
        public float durationSeconds;
        public float appUptimeAtSessionStartSeconds;
        public float appUptimeAtSessionEndSeconds;
        public int sampleCount;
        public float averageFps;
        public float averageFrameMs;
        public float worstFrameMs;
        public float lowestSampledFps;
        public float averageHeapMb;
        public long peakHeapMb;
        public float averageAllocatedMemoryMb;
        public long peakAllocatedMemoryMb;
        public float averageReservedMemoryMb;
        public long peakReservedMemoryMb;
        public int screenWidth;
        public int screenHeight;
        public float screenDpi;
        public string platform;
        public string deviceType;
        public string deviceModel;
        public string operatingSystem;
        public string browserUrl;
        public string unityVersion;
        public string appVersion;
        public string sceneName;
        public string qualityLevel;
        public float renderScale;
        public string graphicsDeviceName;
        public string graphicsDeviceType;
        public int systemMemoryMb;
        public int graphicsMemoryMb;
        public string viewMode;
        public float explosionFactor;
        public bool crossSectionEnabled;
        public int rendererCount;
        public int enabledRendererCount;
        public int meshCount;
        public long estimatedTriangleCount;
    }

    /// <summary>
    /// Lightweight runtime profiler for the WebGL build.
    /// It records scenario-based sessions that can be copied as JSON/CSV from inside the app.
    /// </summary>
    public class WebGLProfiler : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void X500V2DownloadTextFile(string fileName, string mimeType, string content);
#endif

        [Header("Sampling")]
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private bool logToConsole;
        [SerializeField] private int maxCompletedSessions = 32;

        [Header("Overlay")]
        [SerializeField] private bool showOnScreen;
        [SerializeField] private KeyCode toggleKey = KeyCode.F8;

        [Header("Session Defaults")]
        [SerializeField] private bool autoBeginSessionOnStart = true;
        [SerializeField] private string defaultScenario = "scene_base";

        private int _lastFrameCount;
        private float _lastTime;
        private float _currentFps;
        private float _currentMs;
        private long _heapMB;
        private long _allocatedMB;
        private long _reservedMB;
        private bool _overlayExpanded;
        private Vector2 _sessionsScroll;
        private string _lastExportMessage = string.Empty;

        private readonly List<WebGLProfilerSessionSummary> _completedSessions = new List<WebGLProfilerSessionSummary>();
        private bool _sessionActive;
        private string _activeSessionId = string.Empty;
        private string _activeScenario = string.Empty;
        private DateTime _activeSessionStartedAtUtc;
        private double _sessionStartRealtime;
        private int _sessionSampleCount;
        private double _sessionTotalFps;
        private double _sessionTotalFrameMs;
        private double _sessionTotalHeapMb;
        private double _sessionTotalAllocatedMb;
        private double _sessionTotalReservedMb;
        private float _sessionWorstFrameMs;
        private float _sessionLowestFps;
        private long _sessionPeakHeapMb;
        private long _sessionPeakAllocatedMb;
        private long _sessionPeakReservedMb;

        private const float WarningFps = 30f;
        private const float CriticalFps = 20f;

        private static readonly string[] QuickScenarios =
        {
            "scene_base",
            "selection_isolate",
            "explode",
            "cut",
            "thermal_studio",
            "mobile_free"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureProfilerExists()
        {
            if (FindObjectsByType<WebGLProfiler>(FindObjectsSortMode.None).Length > 0)
            {
                return;
            }

            GameObject host = new GameObject("_WebGLProfiler");
            WebGLProfiler profiler = host.AddComponent<WebGLProfiler>();
            profiler.showOnScreen = false;
            DontDestroyOnLoad(host);
        }

        private void Start()
        {
            showOnScreen = false;
            _overlayExpanded = false;
            _lastTime = Time.realtimeSinceStartup;
            _lastFrameCount = Time.frameCount;
            InvokeRepeating(nameof(SamplePerformance), updateInterval, updateInterval);

            if (autoBeginSessionOnStart)
            {
                BeginSession(BuildSessionId(defaultScenario), defaultScenario);
            }
        }

        private void Update()
        {
            if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
            {
                ToggleCapturePanel();
            }
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
            _allocatedMB = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            _reservedMB = Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);

            CaptureSessionSample();

            if (logToConsole)
            {
                Debug.Log(BuildCurrentMetricsLine());
            }

            _lastFrameCount = Time.frameCount;
            _lastTime = Time.realtimeSinceStartup;
        }

        private string BuildCurrentMetricsLine()
        {
            return $"[Profiler] FPS: {_currentFps:F1} | Frame: {_currentMs:F2}ms | Managed: {_heapMB}MB | Alloc: {_allocatedMB}MB | Reserved: {_reservedMB}MB | {GetPerformanceStatus()}";
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
            _sessionTotalAllocatedMb += _allocatedMB;
            _sessionTotalReservedMb += _reservedMB;

            if (_sessionSampleCount == 1)
            {
                _sessionWorstFrameMs = _currentMs;
                _sessionLowestFps = _currentFps;
                _sessionPeakHeapMb = _heapMB;
                _sessionPeakAllocatedMb = _allocatedMB;
                _sessionPeakReservedMb = _reservedMB;
                return;
            }

            _sessionWorstFrameMs = Mathf.Max(_sessionWorstFrameMs, _currentMs);
            _sessionLowestFps = Mathf.Min(_sessionLowestFps, _currentFps);
            _sessionPeakHeapMb = Math.Max(_sessionPeakHeapMb, _heapMB);
            _sessionPeakAllocatedMb = Math.Max(_sessionPeakAllocatedMb, _allocatedMB);
            _sessionPeakReservedMb = Math.Max(_sessionPeakReservedMb, _reservedMB);
        }

        private void OnGUI()
        {
            if (!showOnScreen)
            {
                return;
            }

            GUI.depth = -1000;
            if (!_overlayExpanded)
            {
                DrawCollapsedOverlay();
                return;
            }

            DrawExpandedOverlay();
        }

        private void DrawCollapsedOverlay()
        {
            float width = Mathf.Min(210f, Mathf.Max(150f, Screen.width - 20f));
            Rect rect = new Rect(10f, 10f, width, 42f);
            string rec = _sessionActive ? " REC" : string.Empty;
            string label = $"PERF {_currentFps:F0} FPS{rec}";
            if (GUI.Button(rect, label))
            {
                _overlayExpanded = true;
            }
        }

        private void DrawExpandedOverlay()
        {
            float width = Mathf.Min(460f, Mathf.Max(300f, Screen.width - 20f));
            float height = Mathf.Min(620f, Mathf.Max(360f, Screen.height - 20f));
            GUILayout.BeginArea(new Rect(10f, 10f, width, height), "Performance Capture", GUI.skin.window);

            GUILayout.BeginHorizontal();
            GUILayout.Label(_sessionActive ? $"Recording: {_activeScenario}" : "Idle");
            if (GUILayout.Button("Hide", GUILayout.Width(70f)))
            {
                HideCapturePanel();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
            GUILayout.Label(BuildCurrentMetricsLine());
            GUILayout.Label($"Uptime: {Time.realtimeSinceStartup:F1}s | Quality: {GetQualityLevelName()} | Scale: {GetCurrentRenderScale():F2}");
            GUILayout.Label($"Screen: {Screen.width}x{Screen.height} | DPI: {Screen.dpi:F0} | Platform: {Application.platform}");
            GUILayout.Label($"Device: {SystemInfo.deviceType} | RAM: {SystemInfo.systemMemorySize}MB | VRAM: {SystemInfo.graphicsMemorySize}MB");

            GUILayout.Space(6f);
            GUILayout.Label("Start scenario session");
            for (int i = 0; i < QuickScenarios.Length; i += 2)
            {
                GUILayout.BeginHorizontal();
                DrawScenarioButton(QuickScenarios[i]);
                if (i + 1 < QuickScenarios.Length)
                {
                    DrawScenarioButton(QuickScenarios[i + 1]);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUI.enabled = _sessionActive;
            if (GUILayout.Button("Stop + Save Session"))
            {
                WebGLProfilerSessionSummary summary = EndSession();
                _lastExportMessage = $"Saved {summary.sessionId}";
            }
            GUI.enabled = true;
            if (GUILayout.Button("Clear Sessions"))
            {
                ClearCompletedSessions();
                _lastExportMessage = "Cleared sessions";
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUI.enabled = _completedSessions.Count > 0 || _sessionActive;
            if (GUILayout.Button("Download JSON + CSV"))
            {
                DownloadAllSessions();
            }
            GUI.enabled = true;

            if (!string.IsNullOrEmpty(_lastExportMessage))
            {
                GUILayout.Label(_lastExportMessage);
            }

            GUILayout.Space(6f);
            GUILayout.Label($"Completed sessions: {_completedSessions.Count}");
            _sessionsScroll = GUILayout.BeginScrollView(_sessionsScroll, GUILayout.Height(Mathf.Max(90f, height - 400f)));
            for (int i = _completedSessions.Count - 1; i >= 0; i--)
            {
                WebGLProfilerSessionSummary s = _completedSessions[i];
                GUILayout.Label($"{s.scenario} | {s.averageFps:F1} FPS | {s.averageFrameMs:F1} ms | low {s.lowestSampledFps:F1} | {s.durationSeconds:F1}s");
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void DrawScenarioButton(string scenario)
        {
            if (GUILayout.Button(scenario))
            {
                BeginSession(BuildSessionId(scenario), scenario);
                _lastExportMessage = $"Recording {scenario}";
            }
        }

        public void BeginScenarioSession(string scenario)
        {
            BeginSession(BuildSessionId(scenario), scenario);
        }

        public void ShowCapturePanel()
        {
            showOnScreen = true;
            _overlayExpanded = true;
        }

        public void HideCapturePanel()
        {
            showOnScreen = false;
            _overlayExpanded = false;
        }

        public void ToggleCapturePanel()
        {
            if (showOnScreen)
            {
                HideCapturePanel();
                return;
            }

            ShowCapturePanel();
        }

        public void BeginSession(string sessionId)
        {
            BeginSession(sessionId, defaultScenario);
        }

        public void BeginSession(string sessionId, string scenario)
        {
            if (_sessionActive)
            {
                EndSession();
            }

            _activeScenario = string.IsNullOrWhiteSpace(scenario)
                ? defaultScenario
                : scenario.Trim();

            _activeSessionId = string.IsNullOrWhiteSpace(sessionId)
                ? BuildSessionId(_activeScenario)
                : sessionId.Trim();

            _activeSessionStartedAtUtc = DateTime.UtcNow;
            _sessionActive = true;
            _sessionStartRealtime = Time.realtimeSinceStartupAsDouble;
            _sessionSampleCount = 0;
            _sessionTotalFps = 0d;
            _sessionTotalFrameMs = 0d;
            _sessionTotalHeapMb = 0d;
            _sessionTotalAllocatedMb = 0d;
            _sessionTotalReservedMb = 0d;
            _sessionWorstFrameMs = 0f;
            _sessionLowestFps = 0f;
            _sessionPeakHeapMb = 0L;
            _sessionPeakAllocatedMb = 0L;
            _sessionPeakReservedMb = 0L;
        }

        public WebGLProfilerSessionSummary EndSession()
        {
            if (!_sessionActive)
            {
                return default;
            }

            _sessionActive = false;

            WebGLProfilerSessionSummary summary = BuildSessionSummary();
            _completedSessions.Add(summary);
            while (_completedSessions.Count > Mathf.Max(1, maxCompletedSessions))
            {
                _completedSessions.RemoveAt(0);
            }

            _activeSessionId = string.Empty;
            _activeScenario = string.Empty;
            return summary;
        }

        private WebGLProfilerSessionSummary BuildSessionSummary()
        {
            WebGLProfilerSessionSummary summary = new WebGLProfilerSessionSummary
            {
                sessionId = _activeSessionId,
                scenario = _activeScenario,
                startedAtUtc = _activeSessionStartedAtUtc.ToString("o"),
                endedAtUtc = DateTime.UtcNow.ToString("o"),
                durationSeconds = (float)Math.Max(Time.realtimeSinceStartupAsDouble - _sessionStartRealtime, 0d),
                appUptimeAtSessionStartSeconds = (float)_sessionStartRealtime,
                appUptimeAtSessionEndSeconds = Time.realtimeSinceStartup,
                sampleCount = _sessionSampleCount,
                averageFps = _sessionSampleCount > 0 ? (float)(_sessionTotalFps / _sessionSampleCount) : _currentFps,
                averageFrameMs = _sessionSampleCount > 0 ? (float)(_sessionTotalFrameMs / _sessionSampleCount) : _currentMs,
                worstFrameMs = _sessionSampleCount > 0 ? _sessionWorstFrameMs : _currentMs,
                lowestSampledFps = _sessionSampleCount > 0 ? _sessionLowestFps : _currentFps,
                averageHeapMb = _sessionSampleCount > 0 ? (float)(_sessionTotalHeapMb / _sessionSampleCount) : _heapMB,
                peakHeapMb = _sessionSampleCount > 0 ? _sessionPeakHeapMb : _heapMB,
                averageAllocatedMemoryMb = _sessionSampleCount > 0 ? (float)(_sessionTotalAllocatedMb / _sessionSampleCount) : _allocatedMB,
                peakAllocatedMemoryMb = _sessionSampleCount > 0 ? _sessionPeakAllocatedMb : _allocatedMB,
                averageReservedMemoryMb = _sessionSampleCount > 0 ? (float)(_sessionTotalReservedMb / _sessionSampleCount) : _reservedMB,
                peakReservedMemoryMb = _sessionSampleCount > 0 ? _sessionPeakReservedMb : _reservedMB
            };

            AppendEnvironmentSnapshot(ref summary);
            AppendRuntimeStateSnapshot(ref summary);
            AppendGeometrySnapshot(ref summary);
            return summary;
        }

        private static void AppendEnvironmentSnapshot(ref WebGLProfilerSessionSummary summary)
        {
            summary.screenWidth = Screen.width;
            summary.screenHeight = Screen.height;
            summary.screenDpi = Screen.dpi;
            summary.platform = Application.platform.ToString();
            summary.deviceType = SystemInfo.deviceType.ToString();
            summary.deviceModel = SystemInfo.deviceModel;
            summary.operatingSystem = SystemInfo.operatingSystem;
            summary.browserUrl = Application.absoluteURL;
            summary.unityVersion = Application.unityVersion;
            summary.appVersion = Application.version;
            summary.sceneName = SceneManager.GetActiveScene().name;
            summary.qualityLevel = GetQualityLevelName();
            summary.renderScale = GetCurrentRenderScale();
            summary.graphicsDeviceName = SystemInfo.graphicsDeviceName;
            summary.graphicsDeviceType = SystemInfo.graphicsDeviceType.ToString();
            summary.systemMemoryMb = SystemInfo.systemMemorySize;
            summary.graphicsMemoryMb = SystemInfo.graphicsMemorySize;
        }

        private static void AppendRuntimeStateSnapshot(ref WebGLProfilerSessionSummary summary)
        {
            summary.viewMode = ViewModeManager.Instance != null
                ? ViewModeManager.Instance.CurrentMode.ToString()
                : "unknown";

            summary.explosionFactor = ExplodedViewManager.Instance != null
                ? ExplodedViewManager.Instance.GetExplosionFactor()
                : 0f;

            summary.crossSectionEnabled = CrossSectionManager.Instance != null && CrossSectionManager.Instance.IsEnabled;
        }

        private static void AppendGeometrySnapshot(ref WebGLProfilerSessionSummary summary)
        {
            Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            HashSet<Mesh> uniqueMeshes = new HashSet<Mesh>();
            long triangles = 0L;
            int enabledRenderers = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (renderer.enabled && renderer.gameObject.activeInHierarchy)
                {
                    enabledRenderers++;
                }

                Mesh mesh = ResolveRendererMesh(renderer);
                if (mesh == null || !uniqueMeshes.Add(mesh))
                {
                    continue;
                }

                triangles += EstimateTriangleCount(mesh);
            }

            summary.rendererCount = renderers.Length;
            summary.enabledRendererCount = enabledRenderers;
            summary.meshCount = uniqueMeshes.Count;
            summary.estimatedTriangleCount = triangles;
        }

        private static Mesh ResolveRendererMesh(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer skinned)
            {
                return skinned.sharedMesh;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            return meshFilter != null ? meshFilter.sharedMesh : null;
        }

        private static long EstimateTriangleCount(Mesh mesh)
        {
            if (mesh == null)
            {
                return 0L;
            }

            long total = 0L;
            for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
            {
                total += mesh.GetIndexCount(submesh) / 3L;
            }

            return total;
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

        public string BuildLastCompletedSessionJson()
        {
            return TryGetLastCompletedSession(out WebGLProfilerSessionSummary summary)
                ? JsonUtility.ToJson(summary, true)
                : "{}";
        }

        public string BuildCompletedSessionsJson()
        {
            return JsonUtility.ToJson(new CompletedSessionsWrapper
            {
                sessions = _completedSessions
            }, true);
        }

        public string BuildCompletedSessionsCsv()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("sessionId,scenario,startedAtUtc,endedAtUtc,durationSeconds,sampleCount,averageFps,averageFrameMs,worstFrameMs,lowestSampledFps,averageHeapMb,peakHeapMb,averageAllocatedMemoryMb,peakAllocatedMemoryMb,averageReservedMemoryMb,peakReservedMemoryMb,screenWidth,screenHeight,platform,deviceType,deviceModel,operatingSystem,sceneName,qualityLevel,renderScale,viewMode,explosionFactor,crossSectionEnabled,rendererCount,enabledRendererCount,meshCount,estimatedTriangleCount,graphicsDeviceName,graphicsDeviceType,systemMemoryMb,graphicsMemoryMb,browserUrl");

            for (int i = 0; i < _completedSessions.Count; i++)
            {
                WebGLProfilerSessionSummary s = _completedSessions[i];
                builder
                    .Append(Csv(s.sessionId)).Append(',')
                    .Append(Csv(s.scenario)).Append(',')
                    .Append(Csv(s.startedAtUtc)).Append(',')
                    .Append(Csv(s.endedAtUtc)).Append(',')
                    .Append(s.durationSeconds.ToString("F3")).Append(',')
                    .Append(s.sampleCount).Append(',')
                    .Append(s.averageFps.ToString("F3")).Append(',')
                    .Append(s.averageFrameMs.ToString("F3")).Append(',')
                    .Append(s.worstFrameMs.ToString("F3")).Append(',')
                    .Append(s.lowestSampledFps.ToString("F3")).Append(',')
                    .Append(s.averageHeapMb.ToString("F3")).Append(',')
                    .Append(s.peakHeapMb).Append(',')
                    .Append(s.averageAllocatedMemoryMb.ToString("F3")).Append(',')
                    .Append(s.peakAllocatedMemoryMb).Append(',')
                    .Append(s.averageReservedMemoryMb.ToString("F3")).Append(',')
                    .Append(s.peakReservedMemoryMb).Append(',')
                    .Append(s.screenWidth).Append(',')
                    .Append(s.screenHeight).Append(',')
                    .Append(Csv(s.platform)).Append(',')
                    .Append(Csv(s.deviceType)).Append(',')
                    .Append(Csv(s.deviceModel)).Append(',')
                    .Append(Csv(s.operatingSystem)).Append(',')
                    .Append(Csv(s.sceneName)).Append(',')
                    .Append(Csv(s.qualityLevel)).Append(',')
                    .Append(s.renderScale.ToString("F3")).Append(',')
                    .Append(Csv(s.viewMode)).Append(',')
                    .Append(s.explosionFactor.ToString("F3")).Append(',')
                    .Append(s.crossSectionEnabled ? "true" : "false").Append(',')
                    .Append(s.rendererCount).Append(',')
                    .Append(s.enabledRendererCount).Append(',')
                    .Append(s.meshCount).Append(',')
                    .Append(s.estimatedTriangleCount).Append(',')
                    .Append(Csv(s.graphicsDeviceName)).Append(',')
                    .Append(Csv(s.graphicsDeviceType)).Append(',')
                    .Append(s.systemMemoryMb).Append(',')
                    .Append(s.graphicsMemoryMb).Append(',')
                    .Append(Csv(s.browserUrl))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private void DownloadAllSessions()
        {
            if (_sessionActive)
            {
                EndSession();
            }

            if (_completedSessions.Count == 0)
            {
                _lastExportMessage = "No saved sessions to download";
                return;
            }

            string baseFileName = BuildExportBaseFileName();
            string jsonFileName = $"{baseFileName}.json";
            string csvFileName = $"{baseFileName}.csv";

            string json = BuildCompletedSessionsJson();
            string csv = BuildCompletedSessionsCsv();

            DownloadTextFile(jsonFileName, "application/json;charset=utf-8", json);
            DownloadTextFile(csvFileName, "text/csv;charset=utf-8", csv);

            _lastExportMessage = $"Downloaded JSON + CSV ({_completedSessions.Count} session(s))";
            Debug.Log($"[Profiler] Exported {jsonFileName} and {csvFileName}");
        }

        private static void DownloadTextFile(string fileName, string mimeType, string content)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            X500V2DownloadTextFile(fileName, mimeType, content ?? string.Empty);
#else
            string root = Path.Combine(Application.persistentDataPath, "WebGLProfilerExports");
            Directory.CreateDirectory(root);
            string fullPath = Path.Combine(root, fileName);
            File.WriteAllText(fullPath, content ?? string.Empty, Encoding.UTF8);
            Debug.Log($"[Profiler] Wrote export to {fullPath}");
#endif
        }

        private string BuildExportBaseFileName()
        {
            string scenario = _completedSessions.Count == 1
                ? _completedSessions[0].scenario
                : "all_sessions";
            string device = string.IsNullOrWhiteSpace(SystemInfo.deviceModel)
                ? SystemInfo.deviceType.ToString()
                : SystemInfo.deviceModel;

            return string.Join("_", new[]
            {
                "x500v2_perf",
                SanitizeFilePart(scenario),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"),
                SanitizeFilePart(Application.platform.ToString()),
                SanitizeFilePart(device)
            });
        }

        private static string SanitizeFilePart(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return "unknown";
            }

            StringBuilder builder = new StringBuilder(rawValue.Length);
            for (int i = 0; i < rawValue.Length; i++)
            {
                char c = char.ToLowerInvariant(rawValue[i]);
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    builder.Append(c);
                }
                else if (c == '-' || c == '_' || c == '.')
                {
                    builder.Append(c);
                }
                else if (char.IsWhiteSpace(c) || c == '/' || c == '\\' || c == ':' || c == ',')
                {
                    builder.Append('-');
                }
            }

            string clean = builder.ToString().Trim('-', '_', '.');
            while (clean.Contains("--"))
            {
                clean = clean.Replace("--", "-");
            }

            return string.IsNullOrWhiteSpace(clean) ? "unknown" : clean;
        }

        private static string Csv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        private static string BuildSessionId(string scenario)
        {
            string cleanScenario = string.IsNullOrWhiteSpace(scenario)
                ? "session"
                : scenario.Trim().Replace(" ", "_").Replace("/", "_").Replace("\\", "_").ToLowerInvariant();
            return $"{cleanScenario}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        }

        private static string GetQualityLevelName()
        {
            string[] names = QualitySettings.names;
            int index = QualitySettings.GetQualityLevel();
            return names != null && index >= 0 && index < names.Length
                ? names[index]
                : index.ToString();
        }

        private static float GetCurrentRenderScale()
        {
            RenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline;
            if (pipelineAsset == null)
            {
                return 1f;
            }

            PropertyInfo renderScaleProp = pipelineAsset.GetType().GetProperty(
                "renderScale",
                BindingFlags.Public | BindingFlags.Instance);

            if (renderScaleProp == null)
            {
                return 1f;
            }

            object value = renderScaleProp.GetValue(pipelineAsset);
            return value is float scale ? scale : 1f;
        }

        public float GetCurrentFps() => _currentFps;
        public float GetFrameTimeMs() => _currentMs;
        public long GetHeapMB() => _heapMB;
        public long GetAllocatedMemoryMB() => _allocatedMB;
        public long GetReservedMemoryMB() => _reservedMB;
        public bool IsSessionActive() => _sessionActive;

        [Serializable]
        private sealed class CompletedSessionsWrapper
        {
            public List<WebGLProfilerSessionSummary> sessions = new List<WebGLProfilerSessionSummary>();
        }
    }
}
