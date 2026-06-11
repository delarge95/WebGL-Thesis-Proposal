using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;
using WebGL.Core.Managers;

namespace WebGL.Core.Utils
{
    public enum MigrationBenchmarkTargetKind
    {
        AutoInteractivePart,
        AutoDroneRoot,
        CurrentSelection,
        NamedObject,
        TaggedObject,
        ExplicitTransform
    }

    [Serializable]
    public sealed class MigrationBenchmarkPreset
    {
        public string presetId = "Preset";
        [TextArea] public string description = string.Empty;
        public MigrationBenchmarkTargetKind targetKind = MigrationBenchmarkTargetKind.AutoDroneRoot;
        public string targetName = string.Empty;
        public string targetTag = string.Empty;
        public Transform explicitTarget;
        public float horizontalAngle = 35f;
        public float verticalAngle = 20f;
        public float distance = 8f;
        public float settleSeconds = 1.5f;
        public bool selectTargetDuringInteractive = true;
    }

    [Serializable]
    public sealed class MigrationBenchmarkRunResult
    {
        public string presetId;
        public string scenario;
        public int repetitionIndex;
        public string targetPath;
        public WebGLProfilerSessionSummary session;
    }

    [Serializable]
    public sealed class MigrationBenchmarkAggregateResult
    {
        public string presetId;
        public string scenario;
        public int completedRuns;
        public float averageDurationSeconds;
        public float averageFps;
        public float averageFrameMs;
        public float worstFrameMs;
        public float lowestSampledFps;
        public float averageHeapMb;
        public long peakHeapMb;
    }

    [Serializable]
    public sealed class MigrationBenchmarkReport
    {
        public string reportId;
        public string generatedAtUtc;
        public string unityVersion;
        public string platform;
        public string scenePath;
        public int repetitionsPerScenario;
        public float idleDurationSeconds;
        public float interactiveDurationSeconds;
        public MigrationBenchmarkRunResult[] runs;
        public MigrationBenchmarkAggregateResult[] aggregates;
        public string[] notes;
    }

    public class MigrationBenchmarkRunner : MonoBehaviour
    {
        [Header("Execution")]
        [SerializeField] private bool autoRunOnStart;
        [SerializeField] private bool createProfilerIfMissing = true;

        [Header("Scenarios")]
        [SerializeField] private bool runIdleScenario = true;
        [SerializeField] private bool runInteractiveScenario = true;
        [SerializeField] private int repetitionsPerScenario = 3;
        [SerializeField] private float idleDurationSeconds = 30f;
        [SerializeField] private float interactiveDurationSeconds = 60f;
        [SerializeField] private float cooldownBetweenRunsSeconds = 1.5f;

        [Header("Output")]
        [SerializeField] private bool emitPrettyJsonToConsole = true;
        [SerializeField] private bool writeJsonReportWhenPossible = true;

        [Header("Presets")]
        [SerializeField] private MigrationBenchmarkPreset[] presets = Array.Empty<MigrationBenchmarkPreset>();

        private Coroutine _runCoroutine;
        private WebGLProfiler _profiler;
        private OrbitCameraController _cameraController;
        private SelectionManager _selectionManager;
        private ViewModeManager _viewModeManager;
        private CrossSectionManager _crossSectionManager;
        private ExplodedViewManager _explodedViewManager;
        private MigrationBenchmarkReport _lastReport;

        public MigrationBenchmarkReport LastReport => _lastReport;
        public bool IsRunning => _runCoroutine != null;

        private IEnumerator Start()
        {
            if (!autoRunOnStart)
            {
                yield break;
            }

            yield return null;
            yield return null;
            RunBenchmark();
        }

        private void Reset()
        {
            presets = BuildDefaultPresets();
        }

        [ContextMenu("Run Migration Benchmark")]
        public void RunBenchmark()
        {
            if (_runCoroutine != null)
            {
                Debug.LogWarning("[MigrationBenchmark] A benchmark run is already in progress.");
                return;
            }

            _runCoroutine = StartCoroutine(RunBenchmarkCoroutine());
        }

        [ContextMenu("Stop Migration Benchmark")]
        public void StopBenchmark()
        {
            if (_runCoroutine == null)
            {
                return;
            }

            StopCoroutine(_runCoroutine);
            _runCoroutine = null;
            RestoreDefaultRuntimeState(null);
            Debug.LogWarning("[MigrationBenchmark] Benchmark run stopped.");
        }

        [ContextMenu("Populate Default Presets")]
        public void PopulateDefaultPresets()
        {
            presets = BuildDefaultPresets();
        }

        private IEnumerator RunBenchmarkCoroutine()
        {
            List<string> notes = new List<string>();
            EnsureDependencies(notes);

            if (_profiler == null)
            {
                notes.Add("WebGLProfiler could not be resolved.");
                Debug.LogError("[MigrationBenchmark] WebGLProfiler is required to run benchmarks.");
                _runCoroutine = null;
                yield break;
            }

            if (presets == null || presets.Length == 0)
            {
                presets = BuildDefaultPresets();
                notes.Add("Benchmark presets were empty and were replaced with defaults.");
            }

            BenchmarkStateSnapshot initialState = CaptureCurrentRuntimeState();
            List<MigrationBenchmarkRunResult> runResults = new List<MigrationBenchmarkRunResult>();

            try
            {
                _profiler.ClearCompletedSessions();

                for (int presetIndex = 0; presetIndex < presets.Length; presetIndex++)
                {
                    MigrationBenchmarkPreset preset = presets[presetIndex];
                    Transform target = ResolveTarget(preset, notes);
                    string targetPath = GetTransformPath(target);

                    if (target == null)
                    {
                        notes.Add($"Preset '{preset.presetId}' skipped because no benchmark target could be resolved.");
                        continue;
                    }

                    if (runIdleScenario)
                    {
                        yield return RunScenario(preset, target, targetPath, "Idle", idleDurationSeconds, RunIdleScenario, runResults);
                    }

                    if (runInteractiveScenario)
                    {
                        yield return RunScenario(preset, target, targetPath, "Interactive", interactiveDurationSeconds, RunInteractiveScenario, runResults);
                    }
                }
            }
            finally
            {
                RestoreRuntimeState(initialState);
            }

            _lastReport = BuildReport(runResults, notes);
            string reportJson = JsonUtility.ToJson(_lastReport, true);

            if (emitPrettyJsonToConsole)
            {
                Debug.Log($"[MigrationBenchmark] Completed benchmark run:\n{reportJson}");
            }

            WriteReportToDisk(reportJson, notes);

            Debug.Log($"[MigrationBenchmark] Finished benchmark report '{_lastReport.reportId}' with {runResults.Count} run(s).");
            _runCoroutine = null;
        }

        private IEnumerator RunScenario(
            MigrationBenchmarkPreset preset,
            Transform target,
            string targetPath,
            string scenarioName,
            float durationSeconds,
            Func<MigrationBenchmarkPreset, Transform, float, IEnumerator> scenarioRoutine,
            List<MigrationBenchmarkRunResult> runResults)
        {
            for (int repetition = 0; repetition < Mathf.Max(1, repetitionsPerScenario); repetition++)
            {
                PrepareForScenario(preset, target);
                yield return WaitRealtime(Mathf.Max(0.1f, preset.settleSeconds));

                string sessionId = $"{SanitizeId(preset.presetId)}_{scenarioName.ToLowerInvariant()}_{repetition + 1}";
                _profiler.BeginSession(sessionId);

                yield return scenarioRoutine(preset, target, durationSeconds);

                WebGLProfilerSessionSummary summary = _profiler.EndSession();
                runResults.Add(new MigrationBenchmarkRunResult
                {
                    presetId = preset.presetId,
                    scenario = scenarioName,
                    repetitionIndex = repetition + 1,
                    targetPath = targetPath,
                    session = summary
                });

                RestoreDefaultRuntimeState(target);
                yield return WaitRealtime(Mathf.Max(0f, cooldownBetweenRunsSeconds));
            }
        }

        private IEnumerator RunIdleScenario(MigrationBenchmarkPreset preset, Transform target, float durationSeconds)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < durationSeconds)
            {
                ApplyCameraPose(preset, target, 0f, true);
                yield return null;
            }
        }

        private IEnumerator RunInteractiveScenario(MigrationBenchmarkPreset preset, Transform target, float durationSeconds)
        {
            bool selectionApplied = false;
            float startTime = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - startTime < durationSeconds)
            {
                float elapsed = Time.realtimeSinceStartup - startTime;
                float progress = durationSeconds > 0f ? Mathf.Clamp01(elapsed / durationSeconds) : 1f;

                ApplyCameraPose(preset, target, progress, false);

                if (!selectionApplied && preset.selectTargetDuringInteractive && _selectionManager != null)
                {
                    _selectionManager.SelectObject(target);
                    selectionApplied = true;
                }

                ApplyInteractiveViewMode(progress);
                ApplyInteractiveCrossSection(progress);
                ApplyInteractiveExplosion(progress);

                yield return null;
            }
        }

        private void EnsureDependencies(List<string> notes)
        {
            _profiler = FindFirstSceneObject<WebGLProfiler>();
            if (_profiler == null && createProfilerIfMissing)
            {
                GameObject profilerHost = new GameObject("MigrationBenchmarkProfiler");
                _profiler = profilerHost.AddComponent<WebGLProfiler>();
                notes.Add("WebGLProfiler was created automatically for benchmark execution.");
            }

            _cameraController = OrbitCameraController.Instance != null
                ? OrbitCameraController.Instance
                : FindFirstSceneObject<OrbitCameraController>();
            _selectionManager = SelectionManager.Instance != null
                ? SelectionManager.Instance
                : FindFirstSceneObject<SelectionManager>();
            _viewModeManager = ViewModeManager.Instance != null
                ? ViewModeManager.Instance
                : FindFirstSceneObject<ViewModeManager>();
            _crossSectionManager = CrossSectionManager.Instance != null
                ? CrossSectionManager.Instance
                : FindFirstSceneObject<CrossSectionManager>();
            _explodedViewManager = ExplodedViewManager.Instance != null
                ? ExplodedViewManager.Instance
                : FindFirstSceneObject<ExplodedViewManager>();

            if (_cameraController == null)
            {
                notes.Add("OrbitCameraController was not found. Camera automation will be skipped.");
            }

            if (_selectionManager == null)
            {
                notes.Add("SelectionManager was not found. Selection interactions will be skipped.");
            }

            if (_viewModeManager == null)
            {
                notes.Add("ViewModeManager was not found. Analyze/Thermal transitions will be skipped.");
            }

            if (_crossSectionManager == null)
            {
                notes.Add("CrossSectionManager was not found. Cut interactions will be skipped.");
            }

            if (_explodedViewManager == null)
            {
                notes.Add("ExplodedViewManager was not found. Explode interactions will be skipped.");
            }
        }

        private BenchmarkStateSnapshot CaptureCurrentRuntimeState()
        {
            return new BenchmarkStateSnapshot
            {
                selection = _selectionManager != null ? _selectionManager.CurrentSelection : null,
                viewMode = _viewModeManager != null ? _viewModeManager.CurrentMode : ViewMode.Realistic,
                crossSectionEnabled = _crossSectionManager != null && _crossSectionManager.IsEnabled,
                axis1 = _crossSectionManager != null ? _crossSectionManager.Axis1 : CrossSectionAxis.Y,
                axis2 = _crossSectionManager != null ? _crossSectionManager.Axis2 : CrossSectionAxis.X,
                position1 = _crossSectionManager != null ? _crossSectionManager.Position1 : 0f,
                position2 = _crossSectionManager != null ? _crossSectionManager.Position2 : 0f,
                plane2Active = _crossSectionManager != null && _crossSectionManager.Plane2Active,
                inverted1 = _crossSectionManager != null && _crossSectionManager.Inverted1,
                inverted2 = _crossSectionManager != null && _crossSectionManager.Inverted2,
                explosionFactor = _explodedViewManager != null ? _explodedViewManager.GetExplosionFactor() : 0f
            };
        }

        private void RestoreRuntimeState(BenchmarkStateSnapshot state)
        {
            RestoreDefaultRuntimeState(state.selection);

            if (_viewModeManager != null && _viewModeManager.CurrentMode != state.viewMode)
            {
                _viewModeManager.SetViewMode(state.viewMode);
            }

            if (_explodedViewManager != null)
            {
                _explodedViewManager.SetExplosionFactor(state.explosionFactor);
            }

            if (_crossSectionManager != null)
            {
                _crossSectionManager.DisableCrossSection();
                _crossSectionManager.SetAxis1(state.axis1);
                _crossSectionManager.SetPosition1(state.position1);
                _crossSectionManager.SetInverted1(state.inverted1);
                _crossSectionManager.SetPlane2Active(state.plane2Active);
                _crossSectionManager.SetAxis2(state.axis2);
                _crossSectionManager.SetPosition2(state.position2);
                _crossSectionManager.SetInverted2(state.inverted2);

                if (state.crossSectionEnabled)
                {
                    _crossSectionManager.EnableCrossSection();
                }
                else
                {
                    _crossSectionManager.DisableCrossSection();
                }
            }

            if (_selectionManager != null)
            {
                if (state.selection != null)
                {
                    _selectionManager.SelectObject(state.selection);
                }
                else
                {
                    _selectionManager.Deselect();
                }
            }

            if (_cameraController != null)
            {
                _cameraController.ResetView();
            }
        }

        private void PrepareForScenario(MigrationBenchmarkPreset preset, Transform target)
        {
            RestoreDefaultRuntimeState(target);
            ApplyCameraPose(preset, target, 0f, true);
        }

        private void RestoreDefaultRuntimeState(Transform target)
        {
            if (_selectionManager != null)
            {
                _selectionManager.Deselect();
            }

            if (_viewModeManager != null && _viewModeManager.CurrentMode != ViewMode.Realistic)
            {
                _viewModeManager.SetViewMode(ViewMode.Realistic);
            }

            if (_crossSectionManager != null)
            {
                _crossSectionManager.DisableCrossSection();
                _crossSectionManager.SetPlane2Active(false);
                _crossSectionManager.SetAxis1(CrossSectionAxis.Y);
                _crossSectionManager.SetAxis2(CrossSectionAxis.X);
                _crossSectionManager.SetPosition1(0f);
                _crossSectionManager.SetPosition2(0f);
                _crossSectionManager.SetInverted1(false);
                _crossSectionManager.SetInverted2(false);
            }

            if (_explodedViewManager != null)
            {
                _explodedViewManager.SetExplosionFactor(0f);
            }

            if (_cameraController != null)
            {
                _cameraController.ResetView();
                if (target != null)
                {
                    _cameraController.FocusOnObject(target);
                }
            }
        }

        private void ApplyCameraPose(MigrationBenchmarkPreset preset, Transform target, float progress, bool immediate)
        {
            if (_cameraController == null || target == null)
            {
                return;
            }

            if (immediate)
            {
                _cameraController.FocusOnObject(target);
            }

            float orbitOffset = immediate ? 0f : Mathf.Sin(progress * Mathf.PI * 2f) * 18f;
            float verticalOffset = immediate ? 0f : Mathf.Sin(progress * Mathf.PI) * 6f;
            float distanceOffset = immediate ? 0f : Mathf.Sin(progress * Mathf.PI * 2f) * 0.08f;

            _cameraController.SetAngles(
                preset.horizontalAngle + orbitOffset,
                preset.verticalAngle + verticalOffset,
                immediate);

            _cameraController.SetDistance(
                preset.distance * (1f + distanceOffset),
                immediate);
        }

        private void ApplyInteractiveViewMode(float progress)
        {
            if (_viewModeManager == null)
            {
                return;
            }

            ViewMode desiredMode;
            if (progress < 0.33f)
            {
                desiredMode = ViewMode.Blueprint;
            }
            else if (progress < 0.66f)
            {
                desiredMode = ViewMode.Thermal;
            }
            else
            {
                desiredMode = ViewMode.Realistic;
            }

            if (_viewModeManager.CurrentMode != desiredMode)
            {
                _viewModeManager.SetViewMode(desiredMode);
            }
        }

        private void ApplyInteractiveCrossSection(float progress)
        {
            if (_crossSectionManager == null)
            {
                return;
            }

            if (!_crossSectionManager.IsEnabled)
            {
                _crossSectionManager.EnableCrossSection();
            }

            _crossSectionManager.SetAxis1(CrossSectionAxis.Y);
            _crossSectionManager.SetPosition1(Mathf.Lerp(-0.35f, 0.35f, Mathf.PingPong(progress * 2f, 1f)));

            bool enableSecondPlane = progress > 0.5f;
            _crossSectionManager.SetPlane2Active(enableSecondPlane);
            if (enableSecondPlane)
            {
                _crossSectionManager.SetAxis2(CrossSectionAxis.X);
                _crossSectionManager.SetPosition2(Mathf.Lerp(-0.2f, 0.2f, (progress - 0.5f) / 0.5f));
            }
        }

        private void ApplyInteractiveExplosion(float progress)
        {
            if (_explodedViewManager == null)
            {
                return;
            }

            _explodedViewManager.SetExplosionFactor(Mathf.Sin(progress * Mathf.PI) * 0.35f);
        }

        private Transform ResolveTarget(MigrationBenchmarkPreset preset, List<string> notes)
        {
            switch (preset.targetKind)
            {
                case MigrationBenchmarkTargetKind.AutoInteractivePart:
                    return FindAutoInteractiveTarget();
                case MigrationBenchmarkTargetKind.AutoDroneRoot:
                    return FindAutoDroneRoot();
                case MigrationBenchmarkTargetKind.CurrentSelection:
                    return _selectionManager != null ? _selectionManager.CurrentSelection : null;
                case MigrationBenchmarkTargetKind.NamedObject:
                    return ResolveNamedObject(preset.targetName, notes);
                case MigrationBenchmarkTargetKind.TaggedObject:
                    return ResolveTaggedObject(preset.targetTag, notes);
                case MigrationBenchmarkTargetKind.ExplicitTransform:
                    return preset.explicitTarget;
                default:
                    return null;
            }
        }

        private Transform FindAutoInteractiveTarget()
        {
            if (_selectionManager != null && _selectionManager.CurrentSelection != null)
            {
                return _selectionManager.CurrentSelection;
            }

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            if (parts.Length > 0)
            {
                return parts[0].transform;
            }

            return FindAutoDroneRoot();
        }

        private Transform FindAutoDroneRoot()
        {
            string[] preferredNames = { "x500v2_Drone", "ProceduralDrone", "DroneAssembler" };
            for (int i = 0; i < preferredNames.Length; i++)
            {
                GameObject named = GameObject.Find(preferredNames[i]);
                if (named != null)
                {
                    return named.transform;
                }
            }

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            if (parts.Length > 0)
            {
                return parts[0].transform.root;
            }

            Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            if (renderers.Length > 0)
            {
                return renderers[0].transform.root;
            }

            return null;
        }

        private static Transform ResolveNamedObject(string objectName, List<string> notes)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                notes.Add("NamedObject preset skipped because targetName is empty.");
                return null;
            }

            GameObject match = GameObject.Find(objectName.Trim());
            if (match == null)
            {
                notes.Add($"NamedObject preset could not find '{objectName}'.");
                return null;
            }

            return match.transform;
        }

        private static Transform ResolveTaggedObject(string targetTag, List<string> notes)
        {
            if (string.IsNullOrWhiteSpace(targetTag))
            {
                notes.Add("TaggedObject preset skipped because targetTag is empty.");
                return null;
            }

            try
            {
                GameObject match = GameObject.FindGameObjectWithTag(targetTag.Trim());
                if (match == null)
                {
                    notes.Add($"TaggedObject preset could not find tag '{targetTag}'.");
                    return null;
                }

                return match.transform;
            }
            catch (UnityException)
            {
                notes.Add($"TaggedObject preset uses a tag that is not defined: '{targetTag}'.");
                return null;
            }
        }

        private static string GetTransformPath(Transform target)
        {
            if (target == null)
            {
                return "<null>";
            }

            Stack<string> segments = new Stack<string>();
            Transform current = target;
            while (current != null)
            {
                segments.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", segments.ToArray());
        }

        private MigrationBenchmarkReport BuildReport(List<MigrationBenchmarkRunResult> runResults, List<string> notes)
        {
            List<MigrationBenchmarkAggregateResult> aggregates = new List<MigrationBenchmarkAggregateResult>();
            HashSet<string> aggregateKeys = new HashSet<string>();

            for (int i = 0; i < runResults.Count; i++)
            {
                MigrationBenchmarkRunResult run = runResults[i];
                string key = $"{run.presetId}::{run.scenario}";
                if (!aggregateKeys.Add(key))
                {
                    continue;
                }

                List<MigrationBenchmarkRunResult> matchingRuns = new List<MigrationBenchmarkRunResult>();
                for (int j = 0; j < runResults.Count; j++)
                {
                    MigrationBenchmarkRunResult candidate = runResults[j];
                    if (candidate.presetId == run.presetId && candidate.scenario == run.scenario)
                    {
                        matchingRuns.Add(candidate);
                    }
                }

                aggregates.Add(BuildAggregate(run.presetId, run.scenario, matchingRuns));
            }

            return new MigrationBenchmarkReport
            {
                reportId = $"benchmark_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                generatedAtUtc = DateTime.UtcNow.ToString("o"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                scenePath = SceneManager.GetActiveScene().path,
                repetitionsPerScenario = Mathf.Max(1, repetitionsPerScenario),
                idleDurationSeconds = idleDurationSeconds,
                interactiveDurationSeconds = interactiveDurationSeconds,
                runs = runResults.ToArray(),
                aggregates = aggregates.ToArray(),
                notes = notes.ToArray()
            };
        }

        private static MigrationBenchmarkAggregateResult BuildAggregate(
            string presetId,
            string scenario,
            List<MigrationBenchmarkRunResult> runs)
        {
            MigrationBenchmarkAggregateResult aggregate = new MigrationBenchmarkAggregateResult
            {
                presetId = presetId,
                scenario = scenario,
                completedRuns = runs.Count
            };

            if (runs.Count == 0)
            {
                return aggregate;
            }

            float totalDuration = 0f;
            float totalFps = 0f;
            float totalFrameMs = 0f;
            float totalHeapMb = 0f;
            float worstFrameMs = 0f;
            float lowestFps = float.MaxValue;
            long peakHeapMb = 0L;

            for (int i = 0; i < runs.Count; i++)
            {
                WebGLProfilerSessionSummary session = runs[i].session;
                totalDuration += session.durationSeconds;
                totalFps += session.averageFps;
                totalFrameMs += session.averageFrameMs;
                totalHeapMb += session.averageHeapMb;
                worstFrameMs = Mathf.Max(worstFrameMs, session.worstFrameMs);
                lowestFps = Mathf.Min(lowestFps, session.lowestSampledFps);
                peakHeapMb = Math.Max(peakHeapMb, session.peakHeapMb);
            }

            aggregate.averageDurationSeconds = totalDuration / runs.Count;
            aggregate.averageFps = totalFps / runs.Count;
            aggregate.averageFrameMs = totalFrameMs / runs.Count;
            aggregate.worstFrameMs = worstFrameMs;
            aggregate.lowestSampledFps = lowestFps == float.MaxValue ? 0f : lowestFps;
            aggregate.averageHeapMb = totalHeapMb / runs.Count;
            aggregate.peakHeapMb = peakHeapMb;
            return aggregate;
        }

        private void WriteReportToDisk(string reportJson, List<string> notes)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (!writeJsonReportWhenPossible || string.IsNullOrWhiteSpace(reportJson))
            {
                return;
            }

            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string runtimeReportsRoot = Path.Combine(projectRoot, "Reports", "MigrationMetrics", "runtime");
            Directory.CreateDirectory(runtimeReportsRoot);

            string fileName = $"{_lastReport.reportId}.json";
            string fullPath = Path.Combine(runtimeReportsRoot, fileName);
            File.WriteAllText(fullPath, reportJson);
            Debug.Log($"[MigrationBenchmark] Wrote report to '{fullPath}'.");
#else
            notes.Add("JSON report file output is not available in WebGL runtime.");
#endif
        }

        private static T FindFirstSceneObject<T>() where T : UnityEngine.Object
        {
            T[] matches = FindObjectsByType<T>(FindObjectsSortMode.None);
            return matches.Length > 0 ? matches[0] : null;
        }

        private static IEnumerator WaitRealtime(float seconds)
        {
            if (seconds <= 0f)
            {
                yield break;
            }

            yield return new WaitForSecondsRealtime(seconds);
        }

        private static string SanitizeId(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return "preset";
            }

            return rawValue
                .Trim()
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .ToLowerInvariant();
        }

        private static MigrationBenchmarkPreset[] BuildDefaultPresets()
        {
            return new[]
            {
                new MigrationBenchmarkPreset
                {
                    presetId = "Preset A",
                    description = "Close inspection of an interactive part.",
                    targetKind = MigrationBenchmarkTargetKind.AutoInteractivePart,
                    horizontalAngle = 30f,
                    verticalAngle = 18f,
                    distance = 3.5f,
                    settleSeconds = 1.25f,
                    selectTargetDuringInteractive = true
                },
                new MigrationBenchmarkPreset
                {
                    presetId = "Preset B",
                    description = "Whole drone at medium distance.",
                    targetKind = MigrationBenchmarkTargetKind.AutoDroneRoot,
                    horizontalAngle = 35f,
                    verticalAngle = 18f,
                    distance = 8f,
                    settleSeconds = 1.5f,
                    selectTargetDuringInteractive = true
                },
                new MigrationBenchmarkPreset
                {
                    presetId = "Preset C",
                    description = "Whole drone at long distance.",
                    targetKind = MigrationBenchmarkTargetKind.AutoDroneRoot,
                    horizontalAngle = 45f,
                    verticalAngle = 24f,
                    distance = 14f,
                    settleSeconds = 1.75f,
                    selectTargetDuringInteractive = false
                }
            };
        }

        [Serializable]
        private struct BenchmarkStateSnapshot
        {
            public Transform selection;
            public ViewMode viewMode;
            public bool crossSectionEnabled;
            public CrossSectionAxis axis1;
            public CrossSectionAxis axis2;
            public float position1;
            public float position2;
            public bool plane2Active;
            public bool inverted1;
            public bool inverted2;
            public float explosionFactor;
        }
    }
}
