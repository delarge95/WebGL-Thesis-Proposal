using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

namespace WebGL.Core.Thermal
{
    internal sealed class ThermalRendererBinding
    {
        public string PartId;
        public Renderer Renderer;
        public DronePartData Data;
        public ThermalSurfaceProfile Profile;
        public MaterialPropertyBlock PropertyBlock;
        public Vector3 LocalExtents;
        public Vector3 DominantAxis;
        public float DominantExtent;
    }

    public class ThermalViewController : Singleton<ThermalViewController>
    {
        [Header("Display")]
        [SerializeField] private float displayMinTemperatureC = 20f;
        [SerializeField] private float displayMaxTemperatureC = 100f;
        [SerializeField] private float refreshRateHz = 12f;
        [SerializeField] private float defaultBandHalfWidth = 0.08f;
        [SerializeField] private float criticalBandHalfWidth = 0.14f;
        [SerializeField] private float passiveBandHalfWidth = 0.05f;

        [Header("Debug")]
        [SerializeField] private bool logBindingSummary;

        private readonly List<ThermalRendererBinding> bindings = new List<ThermalRendererBinding>();

        private ThermalSimulationManager thermalSimulation;
        private ViewModeManager viewModeManager;
        private bool isThermalModeActive;
        private float refreshTimer;

        private static readonly int MinTempId = Shader.PropertyToID("_MinTemp");
        private static readonly int MaxTempId = Shader.PropertyToID("_MaxTemp");
        private static readonly int ThermalModeId = Shader.PropertyToID("_ThermalMode");
        private static readonly int ThermalHotspotId = Shader.PropertyToID("_ThermalHotspotOS");
        private static readonly int ThermalDirectionId = Shader.PropertyToID("_ThermalDirectionOS");
        private static readonly int ThermalSpreadId = Shader.PropertyToID("_ThermalSpread");
        private static readonly int ThermalEdgeCoolingId = Shader.PropertyToID("_ThermalEdgeCooling");
        private static readonly int ThermalBaseVariationId = Shader.PropertyToID("_ThermalBaseVariation");
        private static readonly int ThermalPropagationId = Shader.PropertyToID("_ThermalPropagation");

        private void Start()
        {
            RebuildBindings();
            AttachManagers();
        }

        private void OnEnable()
        {
            AttachManagers();
        }

        private void OnDisable()
        {
            DetachManagers();
        }

        private void Update()
        {
            AttachManagers();

            if (!isThermalModeActive)
            {
                return;
            }

            float targetInterval = 1f / Mathf.Max(refreshRateHz, 1f);
            refreshTimer += Time.unscaledDeltaTime;
            if (refreshTimer < targetInterval)
            {
                return;
            }

            refreshTimer = 0f;
            RefreshThermalVisuals();
        }

        [ContextMenu("Rebuild Thermal Renderer Bindings")]
        public void RebuildBindings()
        {
            bindings.Clear();

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (ExplodablePart part in parts)
            {
                if (part == null)
                {
                    continue;
                }

                Renderer renderer = part.GetComponent<Renderer>();
                if (renderer == null)
                {
                    continue;
                }

                MeshFilter meshFilter = part.GetComponent<MeshFilter>();
                Vector3 localExtents = meshFilter != null && meshFilter.sharedMesh != null
                    ? meshFilter.sharedMesh.bounds.extents
                    : Vector3.one * 0.15f;

                if (localExtents.sqrMagnitude <= 0.0001f)
                {
                    localExtents = Vector3.one * 0.15f;
                }

                Vector3 dominantAxis = ResolveDominantAxis(localExtents, out float dominantExtent);
                ThermalSurfaceProfile profile = part.GetComponent<ThermalSurfaceProfile>();
                string partId = profile != null ? profile.ResolvePartId(part) : ResolvePartId(part);

                bindings.Add(new ThermalRendererBinding
                {
                    PartId = partId,
                    Renderer = renderer,
                    Data = part.Data,
                    Profile = profile,
                    PropertyBlock = new MaterialPropertyBlock(),
                    LocalExtents = localExtents,
                    DominantAxis = dominantAxis,
                    DominantExtent = dominantExtent,
                });
            }

            if (logBindingSummary)
            {
                Debug.Log($"[ThermalViewController] Cached {bindings.Count} renderer bindings.");
            }
        }

        private void AttachManagers()
        {
            if (viewModeManager == null)
            {
                viewModeManager = ViewModeManager.Instance;
                if (viewModeManager != null)
                {
                    viewModeManager.OnModeChanged += HandleViewModeChanged;
                    isThermalModeActive = viewModeManager.CurrentMode == ViewMode.Thermal;
                }
            }

            if (thermalSimulation == null)
            {
                thermalSimulation = ThermalSimulationManager.Instance;
            }
        }

        private void DetachManagers()
        {
            if (viewModeManager != null)
            {
                viewModeManager.OnModeChanged -= HandleViewModeChanged;
                viewModeManager = null;
            }

            thermalSimulation = null;
        }

        private void HandleViewModeChanged(ViewMode mode)
        {
            isThermalModeActive = mode == ViewMode.Thermal;
            refreshTimer = 0f;

            if (isThermalModeActive)
            {
                if (bindings.Count == 0)
                {
                    RebuildBindings();
                }

                RefreshThermalVisuals();
            }
        }

        private void RefreshThermalVisuals()
        {
            if (bindings.Count == 0)
            {
                RebuildBindings();
            }

            float ambientTemperatureC = thermalSimulation != null
                ? thermalSimulation.AmbientTemperatureC
                : displayMinTemperatureC;

            float minDisplay = Mathf.Min(displayMinTemperatureC, ambientTemperatureC);
            float maxDisplay = Mathf.Max(displayMaxTemperatureC, minDisplay + 1f);

            // Sync Legend UI
            var uiDoc = Object.FindFirstObjectByType<UnityEngine.UIElements.UIDocument>();
            if (uiDoc != null && uiDoc.rootVisualElement != null)
            {
                var minLabel = uiDoc.rootVisualElement.Q<UnityEngine.UIElements.Label>("ThermalMinTemp");
                var maxLabel = uiDoc.rootVisualElement.Q<UnityEngine.UIElements.Label>("ThermalMaxTemp");
                if (minLabel != null) minLabel.text = $"{Mathf.RoundToInt(minDisplay)}°C";
                if (maxLabel != null) maxLabel.text = $"{Mathf.RoundToInt(maxDisplay)}°C";
            }

            foreach (ThermalRendererBinding binding in bindings)
            {
                if (binding.Renderer == null)
                {
                    continue;
                }

                float temperatureC = ambientTemperatureC;
                if (thermalSimulation != null && thermalSimulation.IsReady)
                {
                    thermalSimulation.TryGetTemperature(binding.PartId, out temperatureC);
                }

                float normalizedTemperature = Mathf.InverseLerp(minDisplay, maxDisplay, temperatureC);
                float bandHalfWidth = ResolveBandHalfWidth(binding);
                float minBand = Mathf.Clamp01(normalizedTemperature - bandHalfWidth);
                float maxBand = Mathf.Clamp01(normalizedTemperature + bandHalfWidth);
                float shaderMode = ResolveShaderMode(binding);
                Vector3 hotspot = ResolveHotspot(binding);
                Vector3 direction = ResolveDirection(binding);
                float spread = ResolveSpread(binding);
                float edgeCooling = ResolveEdgeCooling(binding);
                float baseVariation = ResolveBaseVariation(binding);
                float propagation = ResolvePropagation(binding, normalizedTemperature);

                MaterialPropertyBlock block = binding.PropertyBlock ?? new MaterialPropertyBlock();
                binding.Renderer.GetPropertyBlock(block);
                block.SetFloat(MinTempId, minBand);
                block.SetFloat(MaxTempId, maxBand);
                block.SetFloat(ThermalModeId, shaderMode);
                block.SetVector(ThermalHotspotId, new Vector4(hotspot.x, hotspot.y, hotspot.z, 0f));
                block.SetVector(ThermalDirectionId, new Vector4(direction.x, direction.y, direction.z, 0f));
                block.SetFloat(ThermalSpreadId, spread);
                block.SetFloat(ThermalEdgeCoolingId, edgeCooling);
                block.SetFloat(ThermalBaseVariationId, baseVariation);
                block.SetFloat(ThermalPropagationId, propagation);
                binding.Renderer.SetPropertyBlock(block);
                binding.PropertyBlock = block;
            }
        }

        private float ResolveBandHalfWidth(ThermalRendererBinding binding)
        {
            if (binding.Data != null && binding.Data.isThermallyCritical)
            {
                return criticalBandHalfWidth;
            }

            if (IsHeatSource(binding.Data))
            {
                return Mathf.Lerp(defaultBandHalfWidth, criticalBandHalfWidth, 0.35f);
            }

            return passiveBandHalfWidth;
        }

        private float ResolveShaderMode(ThermalRendererBinding binding)
        {
            switch (ResolvePattern(binding))
            {
                case ThermalSurfacePattern.Radial:
                    return 1f;
                case ThermalSurfacePattern.Axial:
                    return 2f;
                default:
                    return 0f;
            }
        }

        private ThermalSurfacePattern ResolvePattern(ThermalRendererBinding binding)
        {
            if (binding.Profile != null && binding.Profile.Pattern != ThermalSurfacePattern.Automatic)
            {
                return binding.Profile.Pattern;
            }

            string partType = binding.Data != null
                ? (binding.Data.partType ?? string.Empty).ToLowerInvariant()
                : string.Empty;

            string partName = binding.Data != null
                ? (binding.Data.partName ?? string.Empty).ToLowerInvariant()
                : string.Empty;

            if (partType.Contains("motor") || partName.Contains("motor"))
            {
                return ThermalSurfacePattern.Radial;
            }

            if (partType.Contains("battery") || partName.Contains("battery"))
            {
                return ThermalSurfacePattern.Radial;
            }

            if (partType.Contains("flightcontroller") || partType.Contains("powermodule") || partType.Contains("pdb"))
            {
                return ThermalSurfacePattern.Radial;
            }

            if (partType.Contains("esc") || partType.Contains("arm") || partType.Contains("frame") || partType.Contains("landing"))
            {
                return ThermalSurfacePattern.Axial;
            }

            return ThermalSurfacePattern.Uniform;
        }

        private Vector3 ResolveHotspot(ThermalRendererBinding binding)
        {
            if (binding.Profile != null)
            {
                return binding.Profile.HotspotLocal;
            }

            if (ResolvePattern(binding) == ThermalSurfacePattern.Axial)
            {
                return -binding.DominantAxis * binding.DominantExtent * 0.9f;
            }

            return Vector3.zero;
        }

        private Vector3 ResolveDirection(ThermalRendererBinding binding)
        {
            if (binding.Profile != null && binding.Profile.DirectionLocal.sqrMagnitude > 0.0001f)
            {
                return binding.Profile.DirectionLocal.normalized;
            }

            return binding.DominantAxis;
        }

        private float ResolveSpread(ThermalRendererBinding binding)
        {
            if (binding.Profile != null && binding.Profile.Spread > 0f)
            {
                return binding.Profile.Spread;
            }

            ThermalSurfacePattern pattern = ResolvePattern(binding);
            if (pattern == ThermalSurfacePattern.Radial)
            {
                return binding.DominantExtent * 1.25f + 0.02f;
            }

            if (pattern == ThermalSurfacePattern.Axial)
            {
                return binding.DominantExtent * 2f + 0.04f;
            }

            return binding.DominantExtent + 0.02f;
        }

        private float ResolveEdgeCooling(ThermalRendererBinding binding)
        {
            if (binding.Profile != null)
            {
                return binding.Profile.EdgeCooling;
            }

            if (binding.Data != null && binding.Data.thermalExposure > 0f)
            {
                return Mathf.Lerp(0.08f, 0.35f, binding.Data.thermalExposure);
            }

            return 0.22f;
        }

        private float ResolveBaseVariation(ThermalRendererBinding binding)
        {
            if (binding.Profile != null)
            {
                return binding.Profile.BaseVariation;
            }

            return binding.Data != null && binding.Data.isThermallyCritical ? 0.16f : 0.08f;
        }

        private float ResolvePropagation(ThermalRendererBinding binding, float normalizedTemperature)
        {
            float basePropagation = binding.Profile != null ? binding.Profile.Propagation : 1f;
            float criticalBoost = binding.Data != null && binding.Data.isThermallyCritical ? 0.2f : 0f;
            return Mathf.Clamp(basePropagation * Mathf.Lerp(0.45f, 1f, normalizedTemperature) + criticalBoost, 0.25f, 2f);
        }

        private static string ResolvePartId(ExplodablePart part)
        {
            DronePartData data = part != null ? part.Data : null;
            if (data != null && !string.IsNullOrWhiteSpace(data.id))
            {
                return data.id;
            }

            return part != null ? part.gameObject.name : string.Empty;
        }

        private static bool IsHeatSource(DronePartData data)
        {
            if (data == null)
            {
                return false;
            }

            string partType = (data.partType ?? string.Empty).ToLowerInvariant();
            string partName = (data.partName ?? string.Empty).ToLowerInvariant();
            return partType.Contains("motor")
                || partType.Contains("esc")
                || partType.Contains("battery")
                || partType.Contains("flightcontroller")
                || partType.Contains("powermodule")
                || partType.Contains("pdb")
                || partName.Contains("motor")
                || partName.Contains("battery");
        }

        private static Vector3 ResolveDominantAxis(Vector3 extents, out float dominantExtent)
        {
            if (extents.x >= extents.y && extents.x >= extents.z)
            {
                dominantExtent = Mathf.Max(extents.x, 0.05f);
                return Vector3.right;
            }

            if (extents.y >= extents.x && extents.y >= extents.z)
            {
                dominantExtent = Mathf.Max(extents.y, 0.05f);
                return Vector3.up;
            }

            dominantExtent = Mathf.Max(extents.z, 0.05f);
            return Vector3.forward;
        }
    }
}