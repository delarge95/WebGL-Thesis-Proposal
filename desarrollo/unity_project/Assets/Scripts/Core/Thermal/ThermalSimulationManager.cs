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
        Propeller,
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

    internal sealed class ThermalBridgeCandidate
    {
        public string PresentPartId;
        public float Conductance;
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
        [SerializeField] private string defaultContactGraphResourcePath = "ThermalCanonicalContactGraph";

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
        private bool usingFallbackLinks;

        public bool IsReady => isReady;
        public float AmbientTemperatureC => ambientTemperatureC;
        public IReadOnlyList<ThermalPartSnapshot> DebugSnapshot => debugSnapshot;
        public bool IsUsingFallbackLinks => usingFallbackLinks;

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
            ResolveContactGraph();
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
            ResolveContactGraph();

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

            AddRendererThermalSources();
            BuildLinks();
            ResetTemperatures();

            isReady = nodes.Count > 0;
            EventBus.Publish(new ThermalSimulationReadyEvent(isReady, nodes.Count));

            if (logInitialization)
            {
                string graphMode = usingFallbackLinks ? "fallback-links" : "contact-graph";
                Debug.Log($"[ThermalSimulationManager] Ready={isReady} Nodes={nodes.Count} Links={links.Count} Mode={graphMode}");
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
            if (!TryGetNode(partId, out ThermalNodeRuntime node))
            {
                return false;
            }

            temperatureC = node.CurrentTemperatureC;
            return true;
        }

        public bool TryGetHottestLinkedNeighbor(string partId, out string neighborPartId, out float temperatureC)
        {
            neighborPartId = string.Empty;
            temperatureC = ambientTemperatureC;

            if (!TryGetNode(partId, out ThermalNodeRuntime node))
            {
                return false;
            }

            bool found = false;
            foreach (ThermalLinkRuntime link in links)
            {
                ThermalNodeRuntime neighbor = null;
                if (link.A == node)
                {
                    neighbor = link.B;
                }
                else if (link.B == node)
                {
                    neighbor = link.A;
                }

                if (neighbor == null || (found && neighbor.CurrentTemperatureC <= temperatureC))
                {
                    continue;
                }

                found = true;
                neighborPartId = neighbor.PartId;
                temperatureC = neighbor.CurrentTemperatureC;
            }

            return found;
        }

        public float GetNormalizedTemperature(string partId)
        {
            if (!TryGetNode(partId, out ThermalNodeRuntime node))
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
            ThermalSourceClass sourceClass = DetermineSourceClass(data, partId, part.gameObject.name);
            float peakTemp = ResolvePeakTemperature(data, sourceClass);
            float hoverTemp = ResolveHoverTemperature(data, peakTemp, sourceClass);
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

        private void AddRendererThermalSources()
        {
            HashSet<string> allowedSources = BuildRuntimeThermalSourceSet();
            List<Renderer> renderers = DroneRenderResolver.CollectManagedRenderers();
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = renderer.GetComponentInParent<PartRenderCategory>();
                }

                string sourceId = category != null ? category.ThermalSourcePartId : string.Empty;
                if (string.IsNullOrWhiteSpace(sourceId) || TryGetNode(sourceId, out _))
                {
                    continue;
                }

                if (allowedSources.Count > 0 && !allowedSources.Contains(sourceId))
                {
                    continue;
                }

                ThermalNodeRuntime node = CreateSyntheticNode(sourceId, category, renderer);
                if (node == null || string.IsNullOrWhiteSpace(node.PartId))
                {
                    continue;
                }

                nodes.Add(node);
                nodesById[node.PartId] = node;
            }
        }

        private HashSet<string> BuildRuntimeThermalSourceSet()
        {
            HashSet<string> allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (contactGraph != null && contactGraph.nodeIds != null)
            {
                foreach (string nodeId in contactGraph.nodeIds)
                {
                    if (!string.IsNullOrWhiteSpace(nodeId))
                    {
                        allowed.Add(nodeId);
                    }
                }
            }

            allowed.Add("x500v2_battery");
            allowed.Add("x500v2_prop_FL");
            allowed.Add("x500v2_prop_FR");
            allowed.Add("x500v2_prop_BL");
            allowed.Add("x500v2_prop_BR");
            return allowed;
        }

        private ThermalNodeRuntime CreateSyntheticNode(string sourceId, PartRenderCategory category, Renderer renderer)
        {
            string rendererName = renderer != null ? renderer.name : string.Empty;
            string descriptor = $"{sourceId} {rendererName} {category?.SubpieceId} {category?.PrimaryCategory} {category?.AuxiliaryCategory}";
            ThermalSourceClass sourceClass = DetermineSourceClass(null, sourceId, descriptor);
            float peakTemp = ResolvePeakTemperature(null, sourceClass);
            float hoverTemp = ResolveHoverTemperature(null, peakTemp, sourceClass);
            string materialType = renderer != null && renderer.sharedMaterial != null ? renderer.sharedMaterial.name : string.Empty;

            return new ThermalNodeRuntime
            {
                PartId = sourceId,
                PartName = string.IsNullOrWhiteSpace(rendererName) ? sourceId : rendererName,
                Data = null,
                MaterialType = materialType,
                SourceClass = sourceClass,
                CurrentTemperatureC = ambientTemperatureC,
                EquilibriumTemperatureC = ambientTemperatureC,
                MinTemperatureC = 20f,
                HoverTemperatureC = hoverTemp,
                PeakTemperatureC = peakTemp,
                WarmupSeconds = EstimateWarmupSeconds(sourceClass),
                CoolingRate = EstimateCoolingRate(null, sourceClass),
                Exposure = EstimateExposure(null, sourceClass),
                SourceWeight = EstimateSourceWeight(null, sourceClass),
                ConductionScale = 1f,
                IsCritical = sourceClass == ThermalSourceClass.Motor
                    || sourceClass == ThermalSourceClass.Battery
                    || sourceClass == ThermalSourceClass.ElectronicsHigh,
            };
        }

        private void BuildLinks()
        {
            bool hasAuthoritativeGraph = contactGraph != null
                && contactGraph.links != null
                && contactGraph.links.Count > 0;

            usingFallbackLinks = !hasAuthoritativeGraph;

            if (hasAuthoritativeGraph)
            {
                foreach (ThermalContactLinkData linkData in contactGraph.links)
                {
                    float conductance = EstimateConductanceFromGraph(linkData);
                    TryAddLink(linkData.fromPartId, linkData.toPartId, conductance);
                }

                AddBridgeLinksForMissingGraphNodes();
                AddSupplementalRuntimeLinks();
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
            AddSupplementalRuntimeLinks();
        }

        private void ResolveContactGraph()
        {
            if (contactGraph != null && contactGraph.links != null && contactGraph.links.Count > 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(defaultContactGraphResourcePath))
            {
                return;
            }

            ThermalContactGraphAsset loadedGraph = Resources.Load<ThermalContactGraphAsset>(defaultContactGraphResourcePath);
            if (loadedGraph != null)
            {
                contactGraph = loadedGraph;
            }
        }

        private void AddPerArmFallbackLinks(string suffix)
        {
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_esc_{suffix}", 0.05f);
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_prop_{suffix}", 0.018f);
            TryAddLink($"x500v2_prop_{suffix}", $"x500v2_arm_{suffix}", 0.006f);
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_arm_{suffix}", 0.08f);
            TryAddLink($"x500v2_esc_{suffix}", $"x500v2_arm_{suffix}", 0.09f);
            TryAddLink($"x500v2_arm_{suffix}", "x500v2_bottom_plate", 0.05f);
            TryAddLink($"x500v2_arm_{suffix}", "x500v2_top_plate", 0.04f);
        }

        private bool TryAddLink(string fromPartId, string toPartId, float baseConductance)
        {
            if (string.IsNullOrWhiteSpace(fromPartId) || string.IsNullOrWhiteSpace(toPartId))
            {
                return false;
            }

            if (!TryGetNode(fromPartId, out ThermalNodeRuntime a) || !TryGetNode(toPartId, out ThermalNodeRuntime b))
            {
                return false;
            }

            foreach (ThermalLinkRuntime existing in links)
            {
                bool sameDirection = existing.A == a && existing.B == b;
                bool invertedDirection = existing.A == b && existing.B == a;
                if (sameDirection || invertedDirection)
                {
                    return false;
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

            return true;
        }

        private void AddBridgeLinksForMissingGraphNodes()
        {
            if (contactGraph == null || contactGraph.links == null || contactGraph.links.Count == 0)
            {
                return;
            }

            var candidatesByMissingNode = new Dictionary<string, List<ThermalBridgeCandidate>>(StringComparer.OrdinalIgnoreCase);
            foreach (ThermalContactLinkData linkData in contactGraph.links)
            {
                bool hasFrom = TryGetNode(linkData.fromPartId, out _);
                bool hasTo = TryGetNode(linkData.toPartId, out _);
                if (hasFrom == hasTo)
                {
                    continue;
                }

                string missingPartId = hasFrom ? linkData.toPartId : linkData.fromPartId;
                string presentPartId = hasFrom ? linkData.fromPartId : linkData.toPartId;
                if (string.IsNullOrWhiteSpace(missingPartId) || string.IsNullOrWhiteSpace(presentPartId))
                {
                    continue;
                }

                if (!candidatesByMissingNode.TryGetValue(missingPartId, out List<ThermalBridgeCandidate> candidates))
                {
                    candidates = new List<ThermalBridgeCandidate>();
                    candidatesByMissingNode[missingPartId] = candidates;
                }

                candidates.Add(new ThermalBridgeCandidate
                {
                    PresentPartId = presentPartId,
                    Conductance = EstimateConductanceFromGraph(linkData),
                });
            }

            foreach (KeyValuePair<string, List<ThermalBridgeCandidate>> entry in candidatesByMissingNode)
            {
                List<ThermalBridgeCandidate> candidates = entry.Value;
                for (int i = 0; i < candidates.Count; i++)
                {
                    for (int j = i + 1; j < candidates.Count; j++)
                    {
                        float bridgeConductance = Mathf.Min(candidates[i].Conductance, candidates[j].Conductance) * 0.45f;
                        TryAddLink(candidates[i].PresentPartId, candidates[j].PresentPartId, bridgeConductance);
                    }
                }
            }
        }

        private void AddSupplementalRuntimeLinks()
        {
            // The final FBX intentionally suppresses some documented proxies (PDB, ESCs, RC).
            // These direct links preserve the physical conduction paths through real nearby geometry.
            TryAddLink("x500v2_power_module", "x500v2_bottom_plate", 0.045f);
            TryAddLink("x500v2_power_module", "x500v2_top_plate", 0.028f);
            TryAddLink("x500v2_power_module", "x500v2_rails_battery", 0.024f);
            TryAddLink("x500v2_pixhawk6c", "x500v2_top_plate", 0.07f);
            TryAddLink("x500v2_platform_board", "x500v2_top_plate", 0.04f);
            TryAddLink("x500v2_platform_board", "x500v2_gps_m10", 0.022f);
            TryAddLink("x500v2_bottom_plate", "x500v2_top_plate", 0.04f);
            TryAddLink("x500v2_bottom_plate", "x500v2_rails_battery", 0.045f);
            TryAddLink("x500v2_rails_battery", "x500v2_battery", 0.055f);
            TryAddLink("x500v2_battery", "x500v2_power_module", 0.025f);
            TryAddLink("x500v2_battery", "x500v2_bottom_plate", 0.018f);
            TryAddLink("x500v2_bottom_plate", "x500v2_landing_gear", 0.035f);

            AddSupplementalPropellerLinks("FL");
            AddSupplementalPropellerLinks("FR");
            AddSupplementalPropellerLinks("BL");
            AddSupplementalPropellerLinks("BR");
        }

        private void AddSupplementalPropellerLinks(string suffix)
        {
            TryAddLink($"x500v2_motor_{suffix}", $"x500v2_prop_{suffix}", 0.018f);
            TryAddLink($"x500v2_prop_{suffix}", $"x500v2_arm_{suffix}", 0.006f);
        }

        private bool TryGetNode(string partId, out ThermalNodeRuntime node)
        {
            if (!string.IsNullOrWhiteSpace(partId) && nodesById.TryGetValue(partId, out node))
            {
                return true;
            }

            foreach (KeyValuePair<string, ThermalNodeRuntime> entry in nodesById)
            {
                if (string.Equals(entry.Key, partId, StringComparison.OrdinalIgnoreCase))
                {
                    node = entry.Value;
                    return true;
                }
            }

            node = null;
            return false;
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

        private static float ResolvePeakTemperature(DronePartData data, ThermalSourceClass sourceClass)
        {
            if (data == null)
            {
                switch (sourceClass)
                {
                    case ThermalSourceClass.Motor:
                        return 95f;
                    case ThermalSourceClass.Propeller:
                        return 31f;
                    case ThermalSourceClass.Esc:
                        return 70f;
                    case ThermalSourceClass.Battery:
                        return 55f;
                    case ThermalSourceClass.ElectronicsHigh:
                        return 68f;
                    case ThermalSourceClass.ElectronicsLow:
                        return 45f;
                    default:
                        return 60f;
                }
            }

            float peak = data.thermalPeak > 0f ? data.thermalPeak : data.operatingTempMax;
            if (peak <= 0f)
            {
                peak = data.operatingTemp > 0f ? data.operatingTemp : 60f;
            }

            if (sourceClass == ThermalSourceClass.Propeller)
            {
                peak = Mathf.Clamp(peak, 25f, 32f);
            }

            if (sourceClass == ThermalSourceClass.Battery)
            {
                peak = Mathf.Max(peak, 55f);
            }

            return Mathf.Max(peak, 25f);
        }

        private static float ResolveHoverTemperature(DronePartData data, float peakTemperature, ThermalSourceClass sourceClass)
        {
            if (data != null && data.thermalHover > 0f)
            {
                float hover = data.thermalHover;
                if (sourceClass == ThermalSourceClass.Propeller)
                {
                    hover = Mathf.Clamp(hover, 21.5f, 24f);
                }

                if (sourceClass == ThermalSourceClass.Battery)
                {
                    hover = Mathf.Max(hover, 35f);
                }

                return Mathf.Min(hover, peakTemperature);
            }

            if (sourceClass == ThermalSourceClass.Propeller)
            {
                return Mathf.Min(24f, peakTemperature);
            }

            if (sourceClass == ThermalSourceClass.Battery)
            {
                return Mathf.Min(35f, peakTemperature);
            }

            return Mathf.Lerp(20f, peakTemperature, 0.6f);
        }

        private static float EstimateWarmupSeconds(ThermalSourceClass sourceClass)
        {
            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 10f;
                case ThermalSourceClass.Propeller:
                    return 18f;
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
            if (sourceClass == ThermalSourceClass.Propeller)
            {
                return 0.34f;
            }

            if (data != null && data.thermalExposure > 0f)
            {
                return defaultCoolingRate * Mathf.Lerp(0.75f, 1.5f, data.thermalExposure);
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 0.16f;
                case ThermalSourceClass.Propeller:
                    return 0.2f;
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
                if (sourceClass == ThermalSourceClass.Propeller)
                {
                    return Mathf.Min(data.thermalExposure, 0.25f);
                }

                return Mathf.Clamp01(data.thermalExposure);
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 0.9f;
                case ThermalSourceClass.Propeller:
                    return 0.22f;
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
                if (sourceClass == ThermalSourceClass.Propeller)
                {
                    return Mathf.Min(data.thermalSourceWeight, 0.08f);
                }

                return data.thermalSourceWeight;
            }

            switch (sourceClass)
            {
                case ThermalSourceClass.Motor:
                    return 1.0f;
                case ThermalSourceClass.Propeller:
                    return 0.06f;
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

        private static ThermalSourceClass DetermineSourceClass(DronePartData data, string fallbackId = "", string fallbackName = "")
        {
            string id = data != null ? (data.id ?? string.Empty).ToLowerInvariant() : (fallbackId ?? string.Empty).ToLowerInvariant();
            string partType = data != null ? (data.partType ?? string.Empty).ToLowerInvariant() : string.Empty;
            string partName = data != null ? (data.partName ?? string.Empty).ToLowerInvariant() : string.Empty;
            string category = data != null ? data.category.ToString().ToLowerInvariant() : string.Empty;
            string fallback = (fallbackName ?? string.Empty).ToLowerInvariant();
            string combined = $"{id} {partType} {partName} {category} {fallback}";

            if (id.StartsWith("x500v2_prop_", StringComparison.Ordinal) ||
                partType.Contains("propeller") ||
                partName.Contains("propeller") ||
                fallback.Contains("propeller") ||
                fallback.Contains("helice"))
            {
                return ThermalSourceClass.Propeller;
            }

            if (partType.Contains("motor") || partName.Contains("motor"))
            {
                return ThermalSourceClass.Motor;
            }

            if (partType.Contains("esc"))
            {
                return ThermalSourceClass.Esc;
            }

            bool isBatteryMount = id.Contains("rails_battery")
                || combined.Contains("rail system")
                || combined.Contains("battery mount")
                || combined.Contains("battery-mounting")
                || combined.Contains("battery-pad");
            if (!isBatteryMount && (partType.Contains("battery") || partName.Contains("battery") || id.Contains("battery") || combined.Contains("lipo")))
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

        // V002 â€” Wolfram Verification: These are deliberately compressed conductivity scales
        // (0.18â€“1.8) not real k ratios (0.017â€“24.1 normalized to steel).
        // Real values: Cu=390, Al=167, Steel=16.2, CF=2.5, FR4=0.30, LiPo=0.50, Nylon=0.28 W/(mÂ·K).
        // Compression prevents metallic parts from reaching equilibrium in 1 frame while
        // insulating parts show imperceptible gradients. Prioritizes visual credibility over
        // numerical accuracy, consistent with the V1 scope.
        // See: desarrollo/docs/sistema_termico/wolfram_verificaciones.md#V002
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
