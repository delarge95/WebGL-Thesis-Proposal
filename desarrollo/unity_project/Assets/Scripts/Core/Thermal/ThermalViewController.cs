using System;
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
        public string NormalizedPartId;
        public string SubpieceId;
        public Renderer Renderer;
        public Transform Transform;
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
        [SerializeField] private float defaultBandHalfWidth = 0.07f;
        [SerializeField] private float criticalBandHalfWidth = 0.12f;
        [SerializeField] private float passiveBandHalfWidth = 0.035f;
        [SerializeField] private float neutralTemperatureDeadbandC = 0.4f;
        [SerializeField] private float neutralActivationRampC = 2.0f;

        [Header("Debug")]
        [SerializeField] private bool logBindingSummary;

        private readonly List<ThermalRendererBinding> bindings = new List<ThermalRendererBinding>();
        private readonly Dictionary<string, ThermalRendererBinding> bindingsById = new Dictionary<string, ThermalRendererBinding>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Bounds> canonicalBoundsById = new Dictionary<string, Bounds>(StringComparer.OrdinalIgnoreCase);

        private ThermalSimulationManager thermalSimulation;
        private ViewModeManager viewModeManager;
        private UIDocument uiDocument;
        private Label thermalMinLabel;
        private Label thermalMaxLabel;
        private VisualElement thermalGradientElement;
        private Texture2D thermalLegendTexture;
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
            TryBindLegendUI();
        }

        private void OnEnable()
        {
            AttachManagers();
            TryBindLegendUI();
        }

        private void OnDisable()
        {
            DetachManagers();
            ClearLegendBinding();
        }

        private void Update()
        {
            AttachManagers();

            if (!isThermalModeActive)
            {
                return;
            }

            TryBindLegendUI();

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
            bindingsById.Clear();
            canonicalBoundsById.Clear();

            HashSet<Renderer> seen = new HashSet<Renderer>();
            List<Renderer> renderers = DroneRenderResolver.CollectManagedRenderers();
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || !seen.Add(renderer))
                {
                    continue;
                }

                ExplodablePart part = DroneRenderResolver.ResolveCanonicalPart(renderer.transform);
                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = renderer.GetComponentInParent<PartRenderCategory>();
                }

                ThermalSurfaceProfile profile = part != null
                    ? part.GetComponent<ThermalSurfaceProfile>()
                    : renderer.GetComponentInParent<ThermalSurfaceProfile>();
                string partId = profile != null
                    ? profile.ResolvePartId(part)
                    : DroneRenderResolver.ResolveThermalSourceId(renderer, part);
                string normalizedPartId = NormalizePartId(partId);

                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                Vector3 localExtents = meshFilter != null && meshFilter.sharedMesh != null
                    ? meshFilter.sharedMesh.bounds.extents
                    : Vector3.one * 0.15f;

                if (localExtents.sqrMagnitude <= 0.0001f)
                {
                    localExtents = Vector3.one * 0.15f;
                }

                Vector3 dominantAxis = ResolveDominantAxis(localExtents, out float dominantExtent);
                ThermalRendererBinding binding = new ThermalRendererBinding
                {
                    PartId = partId,
                    NormalizedPartId = normalizedPartId,
                    SubpieceId = category != null ? category.SubpieceId : string.Empty,
                    Renderer = renderer,
                    Transform = renderer.transform,
                    Data = part != null ? part.Data : null,
                    Profile = profile,
                    PropertyBlock = new MaterialPropertyBlock(),
                    LocalExtents = localExtents,
                    DominantAxis = dominantAxis,
                    DominantExtent = dominantExtent,
                };

                bindings.Add(binding);
                if (!string.IsNullOrWhiteSpace(normalizedPartId))
                {
                    if (canonicalBoundsById.TryGetValue(normalizedPartId, out Bounds existingBounds))
                    {
                        existingBounds.Encapsulate(renderer.bounds);
                        canonicalBoundsById[normalizedPartId] = existingBounds;
                    }
                    else
                    {
                        canonicalBoundsById[normalizedPartId] = renderer.bounds;
                    }
                }

                if (!string.IsNullOrWhiteSpace(normalizedPartId) && !bindingsById.ContainsKey(normalizedPartId))
                {
                    bindingsById.Add(normalizedPartId, binding);
                }
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

                TryBindLegendUI();
                RefreshThermalVisuals();
            }
            else
            {
                UpdateLegendUI(displayMinTemperatureC, displayMaxTemperatureC);
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
            UpdateLegendUI(minDisplay, maxDisplay);

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

                float visualTemperatureC = ResolveVisualTemperature(binding, temperatureC, ambientTemperatureC);
                float thermalActivity = ResolveThermalActivity(visualTemperatureC, ambientTemperatureC);
                if (thermalActivity <= 0f)
                {
                    visualTemperatureC = ambientTemperatureC;
                }

                float normalizedTemperature = Mathf.InverseLerp(minDisplay, maxDisplay, visualTemperatureC);
                float bandHalfWidth = ResolveBandHalfWidth(binding) * thermalActivity;
                float minBand = Mathf.Clamp01(normalizedTemperature - bandHalfWidth);
                float maxBand = Mathf.Clamp01(normalizedTemperature + bandHalfWidth);
                float shaderMode = thermalActivity > 0f ? ResolveShaderMode(binding) : 0f;
                Vector3 hotspot = ResolveHotspot(binding);
                Vector3 direction = ResolveDirection(binding, hotspot);
                float spread = ResolveSpread(binding);
                float edgeCooling = ResolveEdgeCooling(binding) * thermalActivity;
                float baseVariation = ResolveBaseVariation(binding) * thermalActivity;
                float propagation = Mathf.Lerp(1f, ResolvePropagation(binding, normalizedTemperature), thermalActivity);

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

            if (IsCanonicalPlatePart(binding.NormalizedPartId))
            {
                return ThermalSurfacePattern.Radial;
            }

            ThermalSurfacePattern canonicalPattern = ResolveCanonicalPattern(binding);
            if (canonicalPattern != ThermalSurfacePattern.Automatic)
            {
                return canonicalPattern;
            }

            string partType = binding.Data != null
                ? (binding.Data.partType ?? string.Empty).ToLowerInvariant()
                : string.Empty;

            string partName = binding.Data != null
                ? (binding.Data.partName ?? string.Empty).ToLowerInvariant()
                : string.Empty;

            if (partType.Contains("motor") || partName.Contains("motor") || partType.Contains("battery") || partName.Contains("battery"))
            {
                return ThermalSurfacePattern.Radial;
            }

            if (partType.Contains("esc") || partType.Contains("arm") || partType.Contains("frame") || partType.Contains("landing"))
            {
                return ThermalSurfacePattern.Axial;
            }

            if (partType.Contains("flightcontroller") || partType.Contains("powermodule") || partType.Contains("pdb"))
            {
                return ThermalSurfacePattern.Radial;
            }

            return ThermalSurfacePattern.Uniform;
        }

        private Vector3 ResolveHotspot(ThermalRendererBinding binding)
        {
            if (binding.Profile != null)
            {
                return binding.Profile.HotspotLocal;
            }

            if (TryResolveCanonicalHotspot(binding, out Vector3 hotspot))
            {
                return hotspot;
            }

            if (ResolvePattern(binding) == ThermalSurfacePattern.Axial)
            {
                return -binding.DominantAxis * binding.DominantExtent * 0.85f;
            }

            return Vector3.zero;
        }

        private Vector3 ResolveDirection(ThermalRendererBinding binding, Vector3 hotspot)
        {
            if (binding.Profile != null && binding.Profile.DirectionLocal.sqrMagnitude > 0.0001f)
            {
                return binding.Profile.DirectionLocal.normalized;
            }

            if (TryResolveCanonicalDirection(binding, hotspot, out Vector3 direction))
            {
                return direction;
            }

            return binding.DominantAxis;
        }

        private float ResolveSpread(ThermalRendererBinding binding)
        {
            if (binding.Profile != null && binding.Profile.Spread > 0f)
            {
                return binding.Profile.Spread;
            }

            if (IsCanonicalMotorPart(binding.NormalizedPartId))
            {
                return binding.DominantExtent * 1.15f + 0.015f;
            }

            if (IsCanonicalEscPart(binding.NormalizedPartId))
            {
                return binding.DominantExtent * 1.35f + 0.025f;
            }

            if (IsCanonicalArmPart(binding.NormalizedPartId))
            {
                return binding.DominantExtent * 2.15f + 0.05f;
            }

            if (IsCanonicalBatteryPart(binding.NormalizedPartId))
            {
                return MaxComponent(binding.LocalExtents) * 1.4f + 0.03f;
            }

            if (IsCanonicalElectronicsCore(binding.NormalizedPartId))
            {
                return MaxComponent(binding.LocalExtents) * 1.25f + 0.02f;
            }

            if (IsBatteryRailPart(binding.NormalizedPartId))
            {
                return binding.DominantExtent * 1.7f + 0.03f;
            }

            ThermalSurfacePattern pattern = ResolvePattern(binding);
            if (pattern == ThermalSurfacePattern.Radial)
            {
                return MaxComponent(binding.LocalExtents) * 1.2f + 0.02f;
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

            if (IsCanonicalMotorPart(binding.NormalizedPartId))
            {
                return 0.18f;
            }

            if (IsCanonicalEscPart(binding.NormalizedPartId))
            {
                return 0.24f;
            }

            if (IsCanonicalArmPart(binding.NormalizedPartId))
            {
                return 0.32f;
            }

            if (IsCanonicalBatteryPart(binding.NormalizedPartId))
            {
                return 0.12f;
            }

            if (IsCanonicalPlatePart(binding.NormalizedPartId))
            {
                return 0.28f;
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

            if (IsCanonicalPlatePart(binding.NormalizedPartId))
            {
                return 0.03f;
            }

            if (IsCanonicalArmPart(binding.NormalizedPartId) || IsBatteryRailPart(binding.NormalizedPartId))
            {
                return 0.04f;
            }

            if (IsCanonicalMotorPart(binding.NormalizedPartId) || IsCanonicalEscPart(binding.NormalizedPartId))
            {
                return 0.085f;
            }

            if (IsCanonicalBatteryPart(binding.NormalizedPartId) || IsCanonicalElectronicsCore(binding.NormalizedPartId))
            {
                return 0.055f;
            }

            return binding.Data != null && binding.Data.isThermallyCritical ? 0.06f : 0.03f;
        }

        private float ResolvePropagation(ThermalRendererBinding binding, float normalizedTemperature)
        {
            float basePropagation = binding.Profile != null ? binding.Profile.Propagation : 1f;
            float temperatureBlend = Mathf.Lerp(0.45f, 1f, normalizedTemperature);

            if (IsCanonicalArmPart(binding.NormalizedPartId) || IsBatteryRailPart(binding.NormalizedPartId))
            {
                return Mathf.Clamp(basePropagation * (temperatureBlend + 0.2f), 0.35f, 1.75f);
            }

            if (IsCanonicalMotorPart(binding.NormalizedPartId) || IsCanonicalEscPart(binding.NormalizedPartId))
            {
                return Mathf.Clamp(basePropagation * (temperatureBlend + 0.1f), 0.35f, 1.6f);
            }

            float criticalBoost = binding.Data != null && binding.Data.isThermallyCritical ? 0.15f : 0f;
            return Mathf.Clamp(basePropagation * temperatureBlend + criticalBoost, 0.25f, 1.5f);
        }

        private bool TryBindLegendUI()
        {
            if (thermalMinLabel != null && thermalMaxLabel != null && thermalGradientElement != null && thermalMinLabel.panel != null && thermalMaxLabel.panel != null)
            {
                return true;
            }

            if (uiDocument == null)
            {
                uiDocument = FindFirstObjectByType<UIDocument>();
                if (uiDocument == null)
                {
                    return false;
                }
            }

            VisualElement root = uiDocument.rootVisualElement;
            if (root == null)
            {
                return false;
            }

            thermalMinLabel = root.Q<Label>("ThermalMinTemp");
            thermalMaxLabel = root.Q<Label>("ThermalMaxTemp");
            thermalGradientElement = root.Q<VisualElement>("ThermalLegendGradient");
            return thermalMinLabel != null && thermalMaxLabel != null && thermalGradientElement != null;
        }

        private void ClearLegendBinding()
        {
            uiDocument = null;
            thermalMinLabel = null;
            thermalMaxLabel = null;
            thermalGradientElement = null;
        }

        private void UpdateLegendUI(float minDisplay, float maxDisplay)
        {
            if (!TryBindLegendUI())
            {
                return;
            }

            thermalMinLabel.text = FormatTemperature(minDisplay);
            thermalMaxLabel.text = FormatTemperature(maxDisplay);
            ApplyLegendGradient();
        }

        private static string FormatTemperature(float value)
        {
            return $"{Mathf.RoundToInt(value)}\u00B0C";
        }

        private void ApplyLegendGradient()
        {
            if (thermalGradientElement == null)
            {
                return;
            }

            if (thermalLegendTexture == null)
            {
                thermalLegendTexture = BuildLegendTexture();
            }

            thermalGradientElement.style.backgroundImage = new StyleBackground(thermalLegendTexture);
#pragma warning disable CS0618
            thermalGradientElement.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
#pragma warning restore CS0618
        }

        private static Texture2D BuildLegendTexture()
        {
            const int height = 256;
            Texture2D texture = new Texture2D(1, height, TextureFormat.RGBA32, false, true);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.name = "ThermalLegendGradientRuntime";

            for (int y = 0; y < height; y++)
            {
                float t = y / (float)(height - 1);
                texture.SetPixel(0, y, EvaluateLegendColor(t));
            }

            texture.Apply(false, false);
            return texture;
        }

        private static Color EvaluateLegendColor(float t)
        {
            Color cold = new Color(0.05f, 0.0f, 0.35f, 1f);
            Color mid = new Color(1f, 0.5f, 0f, 1f);
            Color hot = new Color(1f, 0.92f, 0.08f, 1f);
            Color whiteHot = Color.white;

            if (t < 0.33f)
            {
                return Color.Lerp(cold, mid, t / 0.33f);
            }

            if (t < 0.66f)
            {
                return Color.Lerp(mid, hot, (t - 0.33f) / 0.33f);
            }

            return Color.Lerp(hot, whiteHot, (t - 0.66f) / 0.34f);
        }

        private ThermalSurfacePattern ResolveCanonicalPattern(ThermalRendererBinding binding)
        {
            if (IsCanonicalMotorPart(binding.NormalizedPartId) || IsCanonicalBatteryPart(binding.NormalizedPartId) || IsCanonicalElectronicsCore(binding.NormalizedPartId))
            {
                return ThermalSurfacePattern.Radial;
            }

            if (IsCanonicalEscPart(binding.NormalizedPartId) || IsCanonicalArmPart(binding.NormalizedPartId) || IsBatteryRailPart(binding.NormalizedPartId))
            {
                return ThermalSurfacePattern.Axial;
            }

            if (IsCanonicalUniformPart(binding.NormalizedPartId))
            {
                return ThermalSurfacePattern.Uniform;
            }

            return ThermalSurfacePattern.Automatic;
        }

        private bool TryResolveCanonicalHotspot(ThermalRendererBinding binding, out Vector3 hotspot)
        {
            hotspot = Vector3.zero;

            if (IsCanonicalMotorPart(binding.NormalizedPartId) || IsCanonicalBatteryPart(binding.NormalizedPartId) || IsCanonicalElectronicsCore(binding.NormalizedPartId))
            {
                return true;
            }

            if (TryGetCanonicalSuffix(binding.NormalizedPartId, out string suffix))
            {
                if (IsCanonicalArmPart(binding.NormalizedPartId))
                {
                    if (TryResolveNeighborHotspot(binding, new[]
                    {
                        $"x500v2_motor_{suffix}",
                        $"x500v2_esc_{suffix}"
                    }, out hotspot))
                    {
                        return true;
                    }

                    hotspot = -binding.DominantAxis * binding.DominantExtent * 0.85f;
                    return true;
                }

                if (IsCanonicalEscPart(binding.NormalizedPartId))
                {
                    if (TryResolveNeighborHotspot(binding, new[]
                    {
                        $"x500v2_motor_{suffix}",
                        $"x500v2_arm_{suffix}"
                    }, out hotspot))
                    {
                        return true;
                    }

                    hotspot = -binding.DominantAxis * binding.DominantExtent * 0.6f;
                    return true;
                }
            }

            if (IsBatteryRailPart(binding.NormalizedPartId))
            {
                if (TryResolveNeighborHotspot(binding, new[]
                {
                    "x500v2_battery",
                    "x500v2_bottom_plate"
                }, out hotspot))
                {
                    return true;
                }

                hotspot = -binding.DominantAxis * binding.DominantExtent * 0.55f;
                return true;
            }

            if (IsCanonicalPlatePart(binding.NormalizedPartId))
            {
                if (TryResolveDynamicNeighborHotspot(binding, out hotspot))
                {
                    return true;
                }

                hotspot = Vector3.zero;
                return true;
            }

            return false;
        }

        private bool TryResolveCanonicalDirection(ThermalRendererBinding binding, Vector3 hotspot, out Vector3 direction)
        {
            direction = Vector3.zero;

            if (IsCanonicalArmPart(binding.NormalizedPartId) || IsCanonicalEscPart(binding.NormalizedPartId) || IsBatteryRailPart(binding.NormalizedPartId))
            {
                Vector3 towardCenter = -hotspot;
                if (towardCenter.sqrMagnitude > 0.0001f)
                {
                    direction = towardCenter.normalized;
                    return true;
                }

                direction = binding.DominantAxis;
                return true;
            }

            return false;
        }

        private bool TryResolveNeighborHotspot(ThermalRendererBinding binding, IEnumerable<string> neighborIds, out Vector3 hotspot)
        {
            hotspot = Vector3.zero;
            if (binding.Transform == null)
            {
                return false;
            }

            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (string neighborId in neighborIds)
            {
                if (!bindingsById.TryGetValue(NormalizePartId(neighborId), out ThermalRendererBinding neighbor) || neighbor.Renderer == null)
                {
                    continue;
                }

                Vector3 neighborCenter = TryGetCanonicalBoundsCenter(NormalizePartId(neighborId), out Vector3 resolvedCenter)
                    ? resolvedCenter
                    : neighbor.Renderer.bounds.center;
                Vector3 localPoint = binding.Transform.InverseTransformPoint(neighborCenter);
                sum += localPoint;
                count++;
            }

            if (count == 0)
            {
                return false;
            }

            hotspot = ClampLocalPointToExtents(sum / count, binding.LocalExtents * 0.95f);
            return true;
        }

        private bool TryResolveDynamicNeighborHotspot(ThermalRendererBinding binding, out Vector3 hotspot)
        {
            hotspot = Vector3.zero;
            if (thermalSimulation == null || binding.Transform == null)
            {
                return false;
            }

            if (!thermalSimulation.TryGetHottestLinkedNeighbor(binding.PartId, out string neighborId, out _))
            {
                return false;
            }

            if (!bindingsById.TryGetValue(NormalizePartId(neighborId), out ThermalRendererBinding neighbor) || neighbor.Renderer == null)
            {
                return false;
            }

            Vector3 neighborCenter = TryGetCanonicalBoundsCenter(NormalizePartId(neighborId), out Vector3 resolvedCenter)
                ? resolvedCenter
                : neighbor.Renderer.bounds.center;
            Vector3 localPoint = binding.Transform.InverseTransformPoint(neighborCenter);
            hotspot = ClampLocalPointToExtents(localPoint, binding.LocalExtents * 0.95f);
            return true;
        }

        private bool TryGetCanonicalBoundsCenter(string normalizedPartId, out Vector3 center)
        {
            if (!string.IsNullOrWhiteSpace(normalizedPartId) &&
                canonicalBoundsById.TryGetValue(normalizedPartId, out Bounds bounds))
            {
                center = bounds.center;
                return true;
            }

            center = Vector3.zero;
            return false;
        }

        private static Vector3 ClampLocalPointToExtents(Vector3 point, Vector3 extents)
        {
            return new Vector3(
                Mathf.Clamp(point.x, -extents.x, extents.x),
                Mathf.Clamp(point.y, -extents.y, extents.y),
                Mathf.Clamp(point.z, -extents.z, extents.z));
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

        private static string NormalizePartId(string partId)
        {
            return string.IsNullOrWhiteSpace(partId)
                ? string.Empty
                : partId.Trim().ToLowerInvariant();
        }

        private static bool TryGetCanonicalSuffix(string normalizedPartId, out string suffix)
        {
            suffix = string.Empty;
            if (string.IsNullOrWhiteSpace(normalizedPartId) || normalizedPartId.Length < 3)
            {
                return false;
            }

            string candidate = normalizedPartId.Substring(normalizedPartId.Length - 2).ToUpperInvariant();
            if (candidate == "FL" || candidate == "FR" || candidate == "BL" || candidate == "BR")
            {
                suffix = candidate;
                return true;
            }

            return false;
        }

        private static bool IsCanonicalMotorPart(string normalizedPartId)
        {
            return normalizedPartId.StartsWith("x500v2_motor_", StringComparison.Ordinal);
        }

        private static bool IsCanonicalEscPart(string normalizedPartId)
        {
            return normalizedPartId.StartsWith("x500v2_esc_", StringComparison.Ordinal);
        }

        private static bool IsCanonicalArmPart(string normalizedPartId)
        {
            return normalizedPartId.StartsWith("x500v2_arm_", StringComparison.Ordinal);
        }

        private static bool IsCanonicalBatteryPart(string normalizedPartId)
        {
            return string.Equals(normalizedPartId, "x500v2_battery", StringComparison.Ordinal);
        }

        private static bool IsBatteryRailPart(string normalizedPartId)
        {
            return string.Equals(normalizedPartId, "x500v2_rails_battery", StringComparison.Ordinal);
        }

        private static bool IsCanonicalElectronicsCore(string normalizedPartId)
        {
            return string.Equals(normalizedPartId, "x500v2_pdb", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_power_module", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_pixhawk6c", StringComparison.Ordinal);
        }

        private static bool IsCanonicalPlatePart(string normalizedPartId)
        {
            return string.Equals(normalizedPartId, "x500v2_bottom_plate", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_top_plate", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_platform_board", StringComparison.Ordinal);
        }

        private static bool IsCanonicalUniformPart(string normalizedPartId)
        {
            return IsCanonicalPlatePart(normalizedPartId)
                || string.Equals(normalizedPartId, "x500v2_landing_gear", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_gps_m10", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_telemetry_radio", StringComparison.Ordinal)
                || string.Equals(normalizedPartId, "x500v2_rc_receiver", StringComparison.Ordinal)
                || normalizedPartId.StartsWith("x500v2_prop_", StringComparison.Ordinal);
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

        private static float MaxComponent(Vector3 value)
        {
            return Mathf.Max(value.x, Mathf.Max(value.y, value.z));
        }

        private float ResolveVisualTemperature(ThermalRendererBinding binding, float solverTemperatureC, float ambientTemperatureC)
        {
            float solverDelta = Mathf.Max(0f, solverTemperatureC - ambientTemperatureC);
            float contactDelta = ResolveContactVisualDelta(binding, ambientTemperatureC);
            float resolvedDelta = Mathf.Max(solverDelta, contactDelta);
            float subpieceScale = ResolveSubpieceThermalScale(binding);
            return ambientTemperatureC + resolvedDelta * subpieceScale;
        }

        private float ResolveThermalActivity(float visualTemperatureC, float ambientTemperatureC)
        {
            float deltaC = Mathf.Max(0f, visualTemperatureC - ambientTemperatureC);
            if (deltaC <= neutralTemperatureDeadbandC)
            {
                return 0f;
            }

            float ramp = Mathf.Max(neutralActivationRampC, 0.01f);
            return Mathf.Clamp01((deltaC - neutralTemperatureDeadbandC) / ramp);
        }

        private float ResolveContactVisualDelta(ThermalRendererBinding binding, float ambientTemperatureC)
        {
            if (thermalSimulation == null)
            {
                return 0f;
            }

            bool isStructuralCarrier = IsCanonicalPlatePart(binding.NormalizedPartId)
                || IsCanonicalArmPart(binding.NormalizedPartId)
                || IsBatteryRailPart(binding.NormalizedPartId)
                || string.Equals(binding.NormalizedPartId, "x500v2_landing_gear", StringComparison.Ordinal);

            if (!isStructuralCarrier ||
                !thermalSimulation.TryGetHottestLinkedNeighbor(binding.PartId, out _, out float neighborTemperatureC))
            {
                return 0f;
            }

            float neighborDelta = Mathf.Max(0f, neighborTemperatureC - ambientTemperatureC);
            float transferScale = IsCanonicalPlatePart(binding.NormalizedPartId) ? 0.42f : 0.34f;
            return neighborDelta * transferScale;
        }

        private static float ResolveSubpieceThermalScale(ThermalRendererBinding binding)
        {
            string subpiece = (binding.SubpieceId ?? string.Empty).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(subpiece))
            {
                return 1f;
            }

            if (binding.NormalizedPartId == "x500v2_pixhawk6c")
            {
                if (subpiece.Contains("pcb") || subpiece.Contains("imu"))
                {
                    return 1f;
                }

                if (subpiece.Contains("mianke") || subpiece.Contains("dike"))
                {
                    return 0.58f;
                }

                if (subpiece.Contains("bm06b"))
                {
                    return 0.72f;
                }
            }

            if (binding.NormalizedPartId == "x500v2_power_module")
            {
                if (subpiece.Contains("pcb") || subpiece.Contains("pm06"))
                {
                    return 1f;
                }

                if (subpiece.Contains("xt60"))
                {
                    return 0.82f;
                }
            }

            if (subpiece.Contains("huan-guijiao") || subpiece.Contains("jiao-eva"))
            {
                return 0.45f;
            }

            if (subpiece.Contains("carbon-fiber") || subpiece.Contains("plate") || subpiece.Contains("guan-cheng"))
            {
                return 0.92f;
            }

            return 1f;
        }
    }
}


