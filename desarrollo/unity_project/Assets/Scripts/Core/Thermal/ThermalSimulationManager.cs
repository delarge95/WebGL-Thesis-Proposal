using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Events;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

namespace WebGL.Core.Thermal
{
    public enum ThermalSourceClass
    {
        Passive,
        StructuralSink,
        ElectronicsLow,
        ElectronicsHigh,
        Motor,
        Esc,
        Battery
    }

    [Serializable]
    public class ThermalPartSnapshot
    {
        public string partId;
        public string partName;
        public float currentTemperatureC;
        public float normalizedTemperature;
        public float equilibriumTemperatureC;
        public ThermalSourceClass sourceClass;
        public bool isCritical;
    }

    internal sealed class ThermalNodeRuntime
    {
        public string PartId;
        public string PartName;
        public DronePartData Data;
        public ThermalSourceClass SourceClass;
        public float CurrentTemperatureC;
        public float EquilibriumTemperatureC;
        public float MinTemperatureC;
        public float HoverTemperatureC;
        public float PeakTemperatureC;
        public float WarmupSeconds;
        public float CoolingRate;
        public float Exposure;
        public float SourceWeight;
        public float ConductionScale;
        public bool IsCritical;
        public string MaterialType;

        public bool IsHeatSource
        {
            get
            {
                return SourceClass == ThermalSourceClass.Motor
                    || SourceClass == ThermalSourceClass.Esc
                    || SourceClass == ThermalSourceClass.Battery
                    || SourceClass == ThermalSourceClass.ElectronicsHigh
                    || SourceClass == ThermalSourceClass.ElectronicsLow;
            }
        }
    }

    internal sealed class ThermalLinkRuntime
    {
        public ThermalNodeRuntime A;
        public ThermalNodeRuntime B;
        public float EffectiveConductance;
    }

    public class ThermalSimulationManager : Singleton<ThermalSimulationManager>
    {
        [Header("Simulation")]
        [SerializeField] private bool autoInitializeOnStart = true;
        [SerializeField] private bool simulateInBackground = true;
        [SerializeField] private float ambientTemperatureC = 20f;
        [SerializeField] private float simulationTimeScale = 4f;
        [SerializeField] private float targetStepHz = 20f;
        [SerializeField] private float idleLoadFloor = 0.2f;
        [SerializeField] private float hoverLoadFloor = 0.45f;

        [Header("Transport Tuning")]
        [SerializeField] private float defaultCoolingRate = 0.08f;
        [SerializeField] private float defaultExposure = 0.5f;
        [SerializeField] private float criticalConductionBoost = 0.03f;

        [Header("Graph")]
        [SerializeField] private ThermalContactGraphAsset contactGraph;

        [Header("Debug")]
        [SerializeField] private bool logInitialization = true;
        [SerializeField] private List<ThermalPartSnapshot> debugSnapshot = new List<ThermalPartSnapshot>();

        private readonly Dictionary<string, ThermalNodeRuntime> nodesById = new Dictionary<string, ThermalNodeRuntime>();
        private readonly List<ThermalNodeRuntime> nodes = new List<ThermalNodeRuntime>();
        private readonly List<ThermalLinkRuntime> links = new List<ThermalLinkRuntime>();
        private readonly Dictionary<ThermalNodeRuntime, float> workingDelta = new Dictionary<ThermalNodeRuntime, float>();

        private DroneStateController droneState;
        private float currentLoadFactor = 0.35f;
        private float stepAccumulator;
        private bool isReady;

        public bool IsReady => isReady;
        public float AmbientTemperatureC => ambientTemperatureC;
        public IReadOnlyList<ThermalPartSnapshot> DebugSnapshot => debugSnapshot;

        public event Action ThermalStepCompleted;

        private void Start()
        {
            if (autoInitializeOnStart)
            {
                RebuildRuntime();
            }
        }

        private void OnEnable()
        {
            AttachDroneState();
        }

        private void OnDisable()
        {
            DetachDroneState();
        }

        private void Update()
        {
            if (!simulateInBackground || !isReady || nodes.Count == 0)
            {
                return;
            }

            float scaledDelta = Time.deltaTime * Mathf.Max(simulationTimeScale, 0.01f);
            float targetStep = 1f / Mathf.Max(targetStepHz, 1f);
            stepAccumulator += scaledDelta;

            while (stepAccumulator >= targetStep)
            {
                StepSimulation(targetStep);
                stepAccumulator -= targetStep;
            }
        }

        public void RebuildRuntime()
        {
            AttachDroneState();

            nodes.Clear();
            nodesById.Clear();
            links.Clear();
            workingDelta.Clear();
            debugSnapshot.Clear();

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (ExplodablePart part in parts)
            {
                ThermalNodeRuntime node = CreateNode(part);
                if (node == null || string.IsNullOrWhiteSpace(node.PartId))
                {
                    continue;
                }

                nodes.Add(node);
                nodesById[node.PartId] = node;
            }

            BuildLinks();
            ResetTemperatures();

            isReady = nodes.Count > 0;
            EventBus.Publish(new ThermalSimulationReadyEvent(isReady, nodes.Count));

            if (logInitialization)
            {
                Debug.Log($"[ThermalSimulationManager] Ready={isReady} Nodes={nodes.Count} Links={links.Count}");
            }
        }

        public void ResetTemperatures()
        {
            foreach (ThermalNodeRuntime node in nodes)
            {
                node.CurrentTemperatureC = ambientTemperatureC;
                node.EquilibriumTemperatureC = ambientTemperatureC;
            }

            RefreshDebugSnapshot();
            EventBus.Publish(new ThermalSimulationResetEvent(ambientTemperatureC));
        }

        public void SetAmbientTemperature(float temperatureC)
        {
            ambientTemperatureC = temperatureC;
        }

        public bool TryGetTemperature(string partId, out float temperatureC)
        {
            temperatureC = ambientTemperatureC;
            if (!nodesById.TryGetValue(partId, out ThermalNodeRuntime node))
            {
                return false;
            }

            temperatureC = node.CurrentTemperatureC;
            return true;
        }

        public float GetNormalizedTemperature(string partId)
        {
            if (!nodesById.TryGetValue(partId, out ThermalNodeRuntime node))
            {
                return 0f;
            }

            return NormalizeTemperature(node.CurrentTemperatureC, node.MinTemperatureC, node.PeakTemperatureC);
        }

        private void StepSimulation(float deltaTime)
        {
            workingDelta.Clear();
            foreach (ThermalNodeRuntime node in nodes)
            {
                workingDelta[node] = 0f;
            }

            foreach (ThermalNodeRuntime node in nodes)
            {
                float equilibrium = ComputeEquilibriumTemperature(node);
                node.EquilibriumTemperatureC = equilibrium;

                float tau = Mathf.Max(node.WarmupSeconds, 0.2f);
                float sourceBlend = 1f - Mathf.Exp(-deltaTime / tau);
                float sourceDelta = (equilibrium - node.CurrentTemperatureC) * sourceBlend * node.SourceWeight;
                workingDelta[node] += sourceDelta;

                float coolingDelta = (ambientTemperatureC - node.CurrentTemperatureC) * node.CoolingRate * node.Exposure * deltaTime;
                workingDelta[node] += coolingDelta;
            }

            foreach (ThermalLinkRuntime link in links)
            {
                float delta = (link.B.CurrentTemperatureC - link.A.CurrentTemperatureC) * link.EffectiveConductance * deltaTime;
                workingDelta[link.A] += delta;
                workingDelta[link.B] -= delta;
            }

            foreach (ThermalNodeRuntime node in nodes)
            {
                float minClamp = Mathf.Min(ambientTemperatureC, node.MinTemperatureC);
                float maxClamp = Mathf.Max(node.PeakTemperatureC + 15f, ambientTemperatureC + 5f);
                node.CurrentTemperatureC = Mathf.Clamp(node.CurrentTemperatureC + workingDelta[node], minClamp, maxClamp);
            }

            RefreshDebugSnapshot();
            ThermalStepCompleted?.Invoke();
        }

        private ThermalNodeRuntime CreateNode(ExplodablePart part)
        {
            if (part == null)
            {
                return null;
            }

            DronePartData data = part.Data;
            string partId = data != null && !string.IsNullOrWhiteSpace(data.id) ? data.id : part.gameObject.name;
            string materialType = data != null ? data.materialType : string.Empty;
            ThermalSourceClass sourceClass = DetermineSourceClass(data);
            float peakTemp = ResolvePeakTemperature(data);
            float hoverTemp = ResolveHoverTemperature(data, peakTemp);
            float warmupSeconds = data != null && data.thermalWarmupSeconds > 0f
                ? data.thermalWarmupSeconds
                : EstimateWarmupSeconds(sourceClass);

            return new ThermalNodeRuntime
            {
                PartId = partId,
                PartName = data != null && !string.IsNullOrWhiteSpace(data.partName) ? data.partName : part.gameObject.name,
                Data = data,
                MaterialType = materialType,
                SourceClass = sourceClass,
                CurrentTemperatureC = ambientTemperatureC,
                EquilibriumTemperatureC = ambientTemperatureC,
                MinTemperatureC = ResolveMinTemperature(data),
                HoverTemperatureC = hoverTemp,
                PeakTemperatureC = peakTemp,
                WarmupSeconds = warmupSeconds,
                CoolingRate = EstimateCoolingRate(data, sourceClass),
                Exposure = EstimateExposure(data, sourceClass),
                SourceWeight = EstimateSourceWeight(data, sourceClass),
                ConductionScale = data != null && data.thermalConductionScale > 0f ? data.thermalConductionScale : 1f,
                IsCritical = data != null && data.isThermallyCritical,
            };
        }

        private void BuildLinks()
        {
            if (contactGraph != null && contactGraph.links != null && contactGraph.links.Count > 0)
            {
                foreach (ThermalContactLinkData linkData in contactGraph.links)
                {
                    float conductance = EstimateConductanceFromGraph(linkData);
                    TryAddLink(linkData.fromPartId, linkData.toPartId, conductance);
                }
                return;
            }

            AddPerArmFallbackLinks("FL");
            AddPerArmFallbackLinks("FR");
            AddPerArmFallbackLinks("BL");
            AddPerArmFallbackLinks("BR");

            TryAddLink("x500v2_bottom_plate", "x500v2_top_plate", 0.02f);
            TryAddLink("x500v2_bottom_plate", "x500v2_pdb", 0.08f);
            TryAddLink("x500v2_pdb", "x500v2_power_module", 0.07f);
            TryAddLink("x500v2_top_plate", "x500v2_pixhawk6c", 0.07f);
            TryAddLink("x500v2_top_plate", "x500v2_platform_board", 0.03f);
            TryAddLink("x500v2_platform_board", "x500v2_gps_m10", 0.03f);
            TryAddLink("x500v2_top_plate", "x500v2_telemetry_radio", 0.02f);
            TryAddLink("x500v2_top_plate", "x500v2_rc_receiver", 0.02f);
            TryAddLink("x500v2_bottom_plate", "x500v2_rails_battery", 0.04f);
            TryAddLink("x500v2_rails_battery", "x500v2_battery", 0.05f);
            TryAddLink("x500v2_bottom_plate", "x500v2_landing_gear", 0.03f);
        }

        private void AddPerArmFallbackLinks(string suffix)
        {
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_esc_{suffix}", 0.05f);
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_arm_{suffix}", 0.08f);
            TryAddLink($"x500v2_esc_{suffix}", $"x500v2_arm_{suffix}", 0.09f);
            TryAddLink($"x500v2_arm_{suffix}", "x500v2_bottom_plate", 0.05f);
            TryAddLink($"x500v2_arm_{suffix}", "x500v2_top_plate", 0.04f);
        }

        private void TryAddLink(string fromPartId, string toPartId, float baseConductance)
        {
            if (string.IsNullOrWhiteSpace(fromPartId) || string.IsNullOrWhiteSpace(toPartId))
            {
                return;
            }

            if (!nodesById.TryGetValue(fromPartId, out ThermalNodeRuntime a) || !nodesById.TryGetValue(toPartId, out ThermalNodeRuntime b))
            {
                return;
            }

            foreach (ThermalLinkRuntime existing in links)
            {
                bool sameDirection = existing.A == a && existing.B == b;
                bool invertedDirection = existing.A == b && existing.B == a;
                if (sameDirection || invertedDirection)
                {
                    return;
                }
            }

            float materialScale = 0.5f * (EstimateMaterialConductivityScale(a.MaterialType) + EstimateMaterialConductivityScale(b.MaterialType));
            float criticalBoost = (a.IsCritical || b.IsCritical) ? criticalConductionBoost : 0f;
            float conductance = Mathf.Max(0.001f, baseConductance * materialScale * a.ConductionScale * b.ConductionScale + criticalBoost);

            links.Add(new ThermalLinkRuntime
            {
                A = a,
                B = b,
                EffectiveConductance = conductance,
            });
        }

        private float ComputeEquilibriumTemperature(ThermalNodeRuntime node)
        {
            if (!node.IsHeatSource)
            {
                return ambientTemperatureC;
            }

            float effectiveLoad = EvaluateEffectiveLoad();
            if (effectiveLoad <= 0f)
            {
                return ambientTemperatureC;
            }

            if (effectiveLoad <= hoverLoadFloor)
            {
                float hoverT = Mathf.Clamp01(effectiveLoad / Mathf.Max(hoverLoadFloor, 0.01f));
                return Mathf.Lerp(ambientTemperatureC, node.HoverTemperatureC, Smooth01(hoverT));
            }

            float peakT = Mathf.InverseLerp(hoverLoadFloor, 1f, Mathf.Clamp01(effectiveLoad));
            return Mathf.Lerp(node.HoverTemperatureC, node.PeakTemperatureC, Smooth01(peakT));
        }

        private float EvaluateEffectiveLoad()
        {
            if (droneState == null)
            {
                return Mathf.Clamp01(currentLoadFactor);
            }

            switch (droneState.CurrentState)
            {
                case DroneState.Off:
                case DroneState.ShuttingDown:
                    return 0f;
                case DroneState.StartingUp:
                    return Mathf.Max(0.1f, idleLoadFloor * 0.5f);
                case DroneState.Idle:
                    return Mathf.Max(currentLoadFactor, idleLoadFloor);
                case DroneState.Flying:
                    return Mathf.Max(currentLoadFactor, hoverLoadFloor);
                default:
                    return Mathf.Clamp01(currentLoadFactor);
            }
        }

        private void RefreshDebugSnapshot()
        {
            debugSnapshot.Clear();
            foreach (ThermalNodeRuntime node in nodes)
            {
                debugSnapshot.Add(new ThermalPartSnapshot
                {
                    partId = node.PartId,
                    partName = node.PartName,
                    currentTemperatureC = node.CurrentTemperatureC,
                    normalizedTemperature = NormalizeTemperature(node.CurrentTemperatureC, node.MinTemperatureC, node.PeakTemperatureC),
                    equilibriumTemperatureC = node.EquilibriumTemperatureC,
                    sourceClass = node.SourceClass,
                    isCritical = node.IsCritical,
                });
            }

            debugSnapshot.Sort((left, right) => right.currentTemperatureC.CompareTo(left.currentTemperatureC));
        }

        private void AttachDroneState()
        {
            if (droneState != null)
            {
                return;
            }

            droneState = DroneStateController.Instance;
            if (droneState == null)
            {
                return;
            }

            currentLoadFactor = droneState.SystemLoadFactor;
            droneState.OnStateChanged += HandleDroneStateChanged;
            droneState.OnSystemLoadChanged += HandleSystemLoadChanged;
        }

        private void DetachDroneState()
        {
            if (droneState == null)
            {
                return;
            }

            droneState.OnStateChanged -= HandleDroneStateChanged;
            droneState.OnSystemLoadChanged -= HandleSystemLoadChanged;
            droneState = null;
        }

        private void HandleDroneStateChanged(DroneState _)
        {
            stepAccumulator = 0f;
        }

        private void HandleSystemLoadChanged(float newLoad)
        {
            currentLoadFactor = Mathf.Clamp01(newLoad);
            stepAccumulator = 0f;
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }

        private static float NormalizeTemperature(float current, float min, float peak)
        {
            float max = Mathf.Max(min + 0.1f, peak);
            return Mathf.InverseLerp(min, max, current);
        }

        private static float ResolveMinTemperature(DronePartData data)
        {
            if (data != null && data.operatingTempMin < data.operatingTempMax)
            {
                return data.operatingTempMin;
            }

            return 20f;
        }

        private static float ResolvePeakTemperature(DronePartData data)
        {
            if (data == null)
            {
                return 60f;
            }

            float peak = data.thermalPeak > 0f ? data.thermalPeak : data.operatingTempMax;
            if (peak <= 0f)
            {
                peak = data.operatingTemp > 0f ? data.operatingTemp : 60f;
            }

            return Mathf.Max(peak, 25f);
        }

        private static float ResolveHoverTemperature(DronePartData data, float peakTemperature)
        {
            if (data != null && data.thermalHover > 0f)
            {
                return Mathf.Min(data.thermalHover, peakTemperature);
            }

            return Mathf.Lerp(20f, peakTemperature, 0.6f);
        }

        private static float EstimateWarmupSeconds(ThermalSourceClass sourceClass)
        {
            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 10f;
                case ThermalSourceClass.Esc:
                    return 6f;
                case ThermalSourceClass.Battery:
                    return 15f;
                case ThermalSourceClass.ElectronicsHigh:
                    return 8f;
                case ThermalSourceClass.ElectronicsLow:
                    return 4f;
                case ThermalSourceClass.StructuralSink:
                    return 20f;
                default:
                    return 12f;
            }
        }

        private float EstimateCoolingRate(DronePartData data, ThermalSourceClass sourceClass)
        {
            if (data != null && data.thermalExposure > 0f)
            {
                return defaultCoolingRate * Mathf.Lerp(0.75f, 1.5f, data.thermalExposure);
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 0.16f;
                case ThermalSourceClass.Esc:
                    return 0.12f;
                case ThermalSourceClass.StructuralSink:
                    return 0.07f;
                case ThermalSourceClass.Battery:
                    return 0.05f;
                case ThermalSourceClass.ElectronicsHigh:
                    return 0.06f;
                case ThermalSourceClass.ElectronicsLow:
                    return 0.05f;
                default:
                    return defaultCoolingRate;
            }
        }

        private float EstimateExposure(DronePartData data, ThermalSourceClass sourceClass)
        {
            if (data != null && data.thermalExposure > 0f)
            {
                return Mathf.Clamp01(data.thermalExposure);
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 0.9f;
                case ThermalSourceClass.Esc:
                    return 0.75f;
                case ThermalSourceClass.StructuralSink:
                    return 0.7f;
                case ThermalSourceClass.Battery:
                    return 0.45f;
                case ThermalSourceClass.ElectronicsHigh:
                    return 0.4f;
                case ThermalSourceClass.ElectronicsLow:
                    return 0.35f;
                default:
                    return defaultExposure;
            }
        }

        private static float EstimateSourceWeight(DronePartData data, ThermalSourceClass sourceClass)
        {
            if (data != null && data.thermalSourceWeight > 0f)
            {
                return data.thermalSourceWeight;
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 1.0f;
                case ThermalSourceClass.Esc:
                    return 0.95f;
                case ThermalSourceClass.Battery:
                    return 0.8f;
                case ThermalSourceClass.ElectronicsHigh:
                    return 0.75f;
                case ThermalSourceClass.ElectronicsLow:
                    return 0.55f;
                default:
                    return 0.0f;
            }
        }

        private static ThermalSourceClass DetermineSourceClass(DronePartData data)
        {
            string partType = data != null ? (data.partType ?? string.Empty).ToLowerInvariant() : string.Empty;
            string partName = data != null ? (data.partName ?? string.Empty).ToLowerInvariant() : string.Empty;

            if (partType.Contains("motor") || partName.Contains("motor"))
            {
                return ThermalSourceClass.Motor;
            }

            if (partType.Contains("esc"))
            {
                return ThermalSourceClass.Esc;
            }

            if (partType.Contains("battery"))
            {
                return ThermalSourceClass.Battery;
            }

            if (partType.Contains("flightcontroller") || partType.Contains("powermodule") || partType.Contains("pdb"))
            {
                return ThermalSourceClass.ElectronicsHigh;
            }

            if (partType.Contains("gps") || partType.Contains("radio") || partType.Contains("receiver"))
            {
                return ThermalSourceClass.ElectronicsLow;
            }

            if (partType.Contains("frame") || partType.Contains("arm") || partType.Contains("landing"))
            {
                return ThermalSourceClass.StructuralSink;
            }

            return ThermalSourceClass.Passive;
        }

        private static float EstimateMaterialConductivityScale(string materialType)
        {
            if (string.IsNullOrWhiteSpace(materialType))
            {
                return 1f;
            }

            string normalized = materialType.ToLowerInvariant();
            if (normalized.Contains("copper"))
            {
                return 1.8f;
            }

            if (normalized.Contains("alumin"))
            {
                return 1.4f;
            }

            if (normalized.Contains("steel") || normalized.Contains("acero"))
            {
                return 1.0f;
            }

            if (normalized.Contains("carbon") || normalized.Contains("fibra"))
            {
                return 0.65f;
            }

            if (normalized.Contains("fr4") || normalized.Contains("pcb"))
            {
                return 0.4f;
            }

            if (normalized.Contains("lipo") || normalized.Contains("litio"))
            {
                return 0.18f;
            }

            if (normalized.Contains("plastic") || normalized.Contains("nylon") || normalized.Contains("silicone") || normalized.Contains("silicona"))
            {
                return 0.2f;
            }

            return 0.6f;
        }

        private static float EstimateConductanceFromGraph(ThermalContactLinkData linkData)
        {
            const float Cm2ToM2 = 1e-4f;
            const float MmToM = 1e-3f;

            float contactAreaM2 = Mathf.Max(linkData.contactAreaCm2, 0.1f) * Cm2ToM2;
            float pathLengthM = Mathf.Max(linkData.pathLengthMm, 0.1f) * MmToM;
            float conductionScale = Mathf.Max(linkData.conductionScale, 0f);

            return Mathf.Max(0.001f, conductionScale * contactAreaM2 / pathLengthM);
        }
    }
}
