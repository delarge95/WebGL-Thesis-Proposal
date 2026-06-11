using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using System;
using WebGL.Core.Data;

namespace WebGL.Core.Content
{
    public class ExplodedViewManager : Singleton<ExplodedViewManager>
    {
        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float explosionFactor = 0f;
        [SerializeField] private float animationSpeed = 2f;
        [SerializeField] private bool useEasing = true;
        [SerializeField] private bool staggerByPriority = true;
        [Range(0.2f, 1f)]
        [SerializeField] private float priorityTravelWindow = 0.55f;
        [Range(0f, 0.35f)]
        [SerializeField] private float mainPartsStartDelay = 0.08f;
        [Header("Radial Layout")]
        [SerializeField] private bool useDroneCenterRadialLayout = true;
        [SerializeField] private string radialCenterPartId = "x500v2_bottom_plate";
        [SerializeField] private float radialDistanceScale = 0.28f;
        [SerializeField] private float minRadialDistance = 0.35f;
        [SerializeField] private float maxRadialDistance = 1.85f;
        [SerializeField] private float centerFallbackDistance = 0.45f;
        [Header("Radial Subpieces")]
        [SerializeField] private bool usePerRendererRadialOffsetsForArmSubpieces = true;
        [SerializeField] private float perRendererRadialOffsetStrength = 1f;
        [SerializeField] private float maxPerRendererRadialOffset = 1.4f;
        [Header("Physical Disassembly")]
        [Range(0.05f, 0.45f)]
        [SerializeField] private float fastenerTravelWindow = 0.22f;
        [SerializeField] private float screwAxisDistance = 0.72f;
        [SerializeField] private float nutAxisDistance = 0.46f;
        [SerializeField] private float spacerAxisDistance = 0.38f;
        [Header("Final Layout Clearance")]
        [SerializeField] private float minimumFinalBoundsSeparation = 0.16f;
        [SerializeField] private int finalSeparationIterations = 10;
        [SerializeField] private float finalSeparationStrength = 0.85f;

        private const float ArmAttachmentCarrierDistance = 0.52f;
        private const float ArmJibiCarrierDistance = 0.46f;
        private const float ArmJibiCarrierAxialWeight = 0.76f;
        private const float RailBatteryTravelStart = 0.30f;
        private const float RailBatteryTravelEnd = 0.90f;
        private const float TopPlateVerticalDistance = 0.72f;
        private const float PlatformBoardVerticalDistance = 0.92f;
        private const float PixhawkVerticalDistance = 1.10f;
        private const float PowerModuleVerticalDistance = 0.96f;
        private const float GpsVerticalDistance = 1.36f;
        private const float TelemetryVerticalDistance = 1.16f;
        private const float RailBatteryVerticalDistance = 0.92f;
        private const float ArmRadialDistanceMultiplier = 0.5f;
        private const float LandingGearVerticalDistance = 0.65f;
        private const float MotorLiftDistance = 1.10f;
        private const float PropellerInitialLiftDistance = 1.36f;
        private const float PropellerExtraLiftDistance = 0.64f;
        private const float MotorLiftStart = 0.18f;
        private const float MotorLiftEnd = 0.52f;
        private const float PropellerInitialLiftStart = 0.00f;
        private const float PropellerInitialLiftEnd = 0.24f;
        private const float PropellerExtraLiftStart = 0.24f;
        private const float PropellerExtraLiftEnd = 0.70f;

        private List<ExplodablePart> parts = new List<ExplodablePart>();
        private readonly List<string> activeCategoryFilters = new List<string>();
        private readonly Dictionary<ExplodablePart, ExplodablePart> motionParentByPart = new Dictionary<ExplodablePart, ExplodablePart>();
        private float currentFactor = 0f;
        private float targetFactor = 0f;
        private Coroutine _animCoroutine;
        private int minExplosionPriority;
        private int maxExplosionPriority;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Find all explodable parts in the scene
            RebuildCache();
            
            // Subscribe to state changes
            EventBus.Subscribe<StateChangedEvent>(OnStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<StateChangedEvent>(OnStateChanged);
        }

        private void OnStateChanged(StateChangedEvent evt)
        {
            if (evt.NewState == AppState.ExplodedView)
            {
                AudioManager.Instance?.PlayExplosionSound();
            }
        }

        private IEnumerator AnimateExplosion()
        {
            while (Mathf.Abs(currentFactor - targetFactor) > 0.001f)
            {
                if (useEasing)
                    currentFactor = Mathf.Lerp(currentFactor, targetFactor, Time.deltaTime * animationSpeed);
                else
                    currentFactor = Mathf.MoveTowards(currentFactor, targetFactor, Time.deltaTime * animationSpeed * 0.5f);

                UpdateAllParts();
                yield return null;
            }

            currentFactor = targetFactor;
            UpdateAllParts();
            _animCoroutine = null;
        }

        private void UpdateAllParts()
        {
            foreach (var part in parts)
            {
                if (part != null && !HasSameCanonicalExplodableAncestor(part))
                {
                    float partFactor = ResolvePartFactor(part, currentFactor);
                    if (motionParentByPart.TryGetValue(part, out ExplodablePart motionParent) &&
                        motionParent != null)
                    {
                        partFactor = ResolvePartFactor(motionParent, currentFactor);
                    }

                    part.UpdateExplosion(partFactor, currentFactor);
                }
            }
            // Force physics broadphase to recognize new collider positions.
            // Parts are static (no Rigidbody), so Unity won't auto-sync transforms.
            Physics.SyncTransforms();
        }

        public void SetExplosionFactor(float value)
        {
            explosionFactor = Mathf.Clamp01(value);
            targetFactor = explosionFactor;

            // Only animate if there is an actual change to avoid
            // resetting parts to zero before ExplodablePart.Start() initializes them
            if (Mathf.Abs(currentFactor - targetFactor) > 0.001f)
            {
                if (_animCoroutine != null) StopCoroutine(_animCoroutine);
                _animCoroutine = StartCoroutine(AnimateExplosion());
            }

            // Notify about change
            EventBus.Publish(new ViewModeChangedEvent(explosionFactor > 0.1f));
        }

        public float GetExplosionFactor() => explosionFactor;
        public float GetCurrentFactor() => currentFactor;
        public bool HasActiveCategoryFilters => !FiltersShowAll(activeCategoryFilters);

        public void RebuildCache()
        {
            parts.Clear();
            parts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));

            // Ensure every part has captured its base local pose before first explosion update.
            foreach (ExplodablePart part in parts)
            {
                if (part != null)
                {
                    part.Initialize();
                }
            }

            RecalculatePriorityRange();
            ConfigureRadialExplosionTargets();
            UpdateAllParts();
        }

        public void SetCategoryFilters(List<string> activeCategories)
        {
            activeCategoryFilters.Clear();
            if (activeCategories != null)
            {
                activeCategoryFilters.AddRange(activeCategories);
            }

            ApplyCategoryFiltersToRenderers();

            if (PartVisibilityManager.Instance != null && PartVisibilityManager.Instance.HasAnyIsolationActive)
            {
                PartVisibilityManager.Instance.ReapplyCurrentIsolationVisuals();
            }
        }

        public void ReapplyActiveCategoryFilters()
        {
            if (!HasActiveCategoryFilters)
            {
                return;
            }

            ApplyCategoryFiltersToRenderers();
        }

        public bool ShouldRendererPassCategoryFilters(Renderer renderer, ExplodablePart part)
        {
            return FiltersShowAll(activeCategoryFilters) ||
                   RendererMatchesFilters(renderer, part, activeCategoryFilters);
        }

        private void ApplyCategoryFiltersToRenderers()
        {
            foreach (var part in parts)
            {
                if (part == null) continue;

                part.gameObject.SetActive(true);

                Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
                bool anyVisible = false;

                foreach (Renderer renderer in renderers)
                {
                    if (renderer == null) continue;

                    bool visible = ShouldRendererPassCategoryFilters(renderer, part);
                    if (visible &&
                        FastenerInspectionManager.Instance != null &&
                        FastenerInspectionManager.Instance.ShouldSuppressProxyRenderer(renderer))
                    {
                        visible = false;
                    }

                    LodVisibilityUtility.ApplyRendererEnabled(renderer, visible);
                    anyVisible |= visible;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = visible;
                    }
                }

                if (!anyVisible)
                {
                    DisableRootColliders(part.transform);
                }
            }
        }

        private static bool FiltersShowAll(IReadOnlyList<string> filters)
        {
            if (filters == null || filters.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < filters.Count; i++)
            {
                if (string.Equals(filters[i], "ALL", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RendererMatchesFilters(Renderer renderer, ExplodablePart part, IReadOnlyList<string> activeCategories)
        {
            PartRenderCategory renderCategory = renderer.GetComponent<PartRenderCategory>();
            if (renderCategory != null)
            {
                return renderCategory.MatchesAny(activeCategories);
            }

            if (part == null || part.Data == null)
            {
                return false;
            }

            foreach (string category in activeCategories)
            {
                if (string.Equals(category, part.Data.category.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void DisableRootColliders(Transform root)
        {
            if (root == null)
            {
                return;
            }

            foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
            {
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        public void RegisterPart(ExplodablePart part)
        {
            if (!parts.Contains(part))
            {
                parts.Add(part);
                RecalculatePriorityRange();
                ConfigureRadialExplosionTargets();
                
                // Sync visual mode state via the canonical ViewModeManager
                if (ViewModeManager.Instance != null &&
                    ViewModeManager.Instance.CurrentMode != ViewMode.Realistic)
                {
                    // ViewModeManager already handles material application —
                    // trigger a re-apply so the new part gets the current shader.
                    ViewModeManager.Instance.SetViewMode(ViewModeManager.Instance.CurrentMode);
                }
            }
        }

        public void UnregisterPart(ExplodablePart part)
        {
            parts.Remove(part);
            RecalculatePriorityRange();
            ConfigureRadialExplosionTargets();
        }

        private void ConfigureRadialExplosionTargets()
        {
            if (!useDroneCenterRadialLayout || parts.Count == 0)
            {
                return;
            }

            if (!TryResolveExplosionCenter(out Vector3 center))
            {
                return;
            }

            Dictionary<string, List<ExplodablePart>> groupedPartsById = BuildGroupedPartsById();
            Dictionary<string, ExplodablePart> partsById = BuildPartsById(groupedPartsById);
            Dictionary<ExplodablePart, Bounds> boundsByPart = BuildBoundsByPart();
            Dictionary<string, Bounds> boundsById = BuildBoundsById(groupedPartsById);
            Dictionary<string, Vector3> primaryWorldOffsetsById = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
            motionParentByPart.Clear();

            foreach (KeyValuePair<string, List<ExplodablePart>> kvp in groupedPartsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart part))
                {
                    continue;
                }

                if (part == null || IsFastenerPart(part) || IsDependentPropulsionPart(part))
                {
                    continue;
                }

                Vector3 partCenter = boundsById.TryGetValue(kvp.Key, out Bounds bounds)
                    ? bounds.center
                    : part.transform.position;
                Vector3 outward = partCenter - center;
                float centerDistance = outward.magnitude;
                float explodeDistance = ResolveRadialDistance(part, centerDistance);

                if (outward.sqrMagnitude <= 0.0001f)
                {
                    outward = ResolveCenterFallbackDirection(part);
                    explodeDistance = Mathf.Max(explodeDistance, centerFallbackDistance);
                }

                Vector3 physicalOutward = ResolveMainPartDisassemblyDirection(part, outward, center);
                Vector3 primaryOffset = ConstrainMainPartOffset(kvp.Key, physicalOutward.normalized * explodeDistance);
                ConfigurePrimaryExplosionTargets(kvp.Value, primaryOffset);
                if (!string.IsNullOrWhiteSpace(kvp.Key))
                {
                    primaryWorldOffsetsById[kvp.Key] = primaryOffset;
                }

                ConfigurePerRendererRadialOffsets(kvp.Value, center, outward.normalized, explodeDistance);
            }

            EnforceMinimumFinalSeparation(partsById, boundsById, primaryWorldOffsetsById);
            EnforceMinimumTimelineSeparation(partsById, boundsById, primaryWorldOffsetsById);
            foreach (KeyValuePair<string, Vector3> kvp in primaryWorldOffsetsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart adjustedPart) ||
                    adjustedPart == null ||
                    IsFastenerPart(adjustedPart) ||
                    IsDependentPropulsionPart(adjustedPart))
                {
                    continue;
                }

                Vector3 adjustedOffset = ConstrainMainPartOffset(kvp.Key, kvp.Value);
                if (adjustedOffset.sqrMagnitude > 0.0001f)
                {
                    if (groupedPartsById.TryGetValue(kvp.Key, out List<ExplodablePart> groupedParts))
                    {
                        ConfigurePrimaryExplosionTargets(groupedParts, adjustedOffset);
                    }
                    else
                    {
                        ConfigurePrimaryExplosionTarget(adjustedPart, adjustedOffset);
                    }
                }
            }

            foreach (ExplodablePart part in parts)
            {
                if (part == null || !IsDependentPropulsionPart(part))
                {
                    continue;
                }

                if (!TryResolvePropulsionArmParentPart(part, partsById, boundsByPart, out ExplodablePart armPart) ||
                    armPart == null ||
                    armPart.Data == null ||
                    !primaryWorldOffsetsById.TryGetValue(armPart.Data.id, out Vector3 armOffset))
                {
                    Vector3 partCenter = TryGetPartWorldBounds(part, out Bounds bounds)
                        ? bounds.center
                        : part.transform.position;
                    Vector3 fallbackOutward = partCenter - center;
                    if (fallbackOutward.sqrMagnitude <= 0.0001f)
                    {
                        fallbackOutward = ResolveCenterFallbackDirection(part);
                    }

                    float fallbackDistance = ResolveRadialDistance(part, fallbackOutward.magnitude);
                    part.ConfigureRuntimeExplosionTarget(
                        ResolveMainPartDisassemblyDirection(part, fallbackOutward, center),
                        fallbackDistance);
                    continue;
                }

                bool isProp = part.Data != null &&
                              part.Data.id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase);
                motionParentByPart[part] = armPart;
                Vector3 sharedLiftDirection = ResolveDependentPropulsionSharedLiftDirection(part, armOffset);
                if (isProp)
                {
                    part.ConfigureRuntimeCompositeExplosionTarget(
                        armOffset,
                        sharedLiftDirection * PropellerInitialLiftDistance,
                        PropellerInitialLiftStart,
                        PropellerInitialLiftEnd,
                        Vector3.up * PropellerExtraLiftDistance,
                        PropellerExtraLiftStart,
                        PropellerExtraLiftEnd);
                }
                else
                {
                    part.ConfigureRuntimeCompositeExplosionTarget(
                        armOffset,
                        sharedLiftDirection * MotorLiftDistance,
                        MotorLiftStart,
                        MotorLiftEnd);
                }
            }

            foreach (ExplodablePart part in parts)
            {
                if (part == null || !IsFastenerPart(part))
                {
                    continue;
                }

                Vector3 parentWorldOffset = Vector3.zero;
                Vector3 carrierWorldOffset = Vector3.zero;
                ExplodablePart motionParentPart = null;
                bool followsArmCarrier = false;
                bool followsArmJibiCarrier = false;
                string fastenerToken = BuildFastenerToken(part.transform);
                string fastenerParentId = ResolveFastenerParentPartId(part);
                bool isRailBatteryFastener = IsRailBatteryParentId(fastenerParentId);
                bool isLongArmCapScrewM3x38 = IsLongArmCapScrewM3x38Token(fastenerToken);
                bool isArmFlangeNutM3 = IsArmFlangeNutM3Token(fastenerToken);
                bool skipsArmCarrier = isArmFlangeNutM3;
                bool isExplicitRailRubberGrommet = false;
                Vector3 railRubberZOffset = Vector3.zero;
                bool railRubberPositiveZ = false;

                if (IsRailRubberGrommetFastenerToken(fastenerToken) &&
                    partsById.TryGetValue("x500v2_rails_battery", out ExplodablePart railBatteryPart) &&
                    railBatteryPart != null &&
                    railBatteryPart.Data != null &&
                    primaryWorldOffsetsById.TryGetValue(railBatteryPart.Data.id, out Vector3 railBatteryOffset))
                {
                    motionParentPart = railBatteryPart;
                    parentWorldOffset = railBatteryOffset;
                    motionParentByPart[part] = railBatteryPart;
                    railRubberZOffset = ResolveRailRubberGrommetZOffset(part.transform, railBatteryPart, fastenerToken, out railRubberPositiveZ);
                    isExplicitRailRubberGrommet = railRubberZOffset.sqrMagnitude > 0.0001f;
                }
                else if (isLongArmCapScrewM3x38 &&
                    TryResolveArmMotionParentForFastener(part, partsById, boundsByPart, out ExplodablePart longScrewArmParent) &&
                    longScrewArmParent != null &&
                    longScrewArmParent.Data != null &&
                    primaryWorldOffsetsById.TryGetValue(longScrewArmParent.Data.id, out Vector3 longScrewArmOffset))
                {
                    motionParentPart = longScrewArmParent;
                    parentWorldOffset = longScrewArmOffset;
                    carrierWorldOffset = ResolveArmJibiCarrierOffset(longScrewArmParent, part, center);
                    motionParentByPart[part] = longScrewArmParent;
                    followsArmJibiCarrier = true;
                }
                else if (!skipsArmCarrier &&
                    TryResolveArmMotionParentForFastener(part, partsById, boundsByPart, out ExplodablePart directArmParent) &&
                    directArmParent != null &&
                    directArmParent.Data != null &&
                    primaryWorldOffsetsById.TryGetValue(directArmParent.Data.id, out Vector3 directArmOffset))
                {
                    motionParentPart = directArmParent;
                    parentWorldOffset = directArmOffset;
                    carrierWorldOffset = ResolveArmAttachmentCarrierOffset(directArmParent, center);
                    motionParentByPart[part] = directArmParent;
                    followsArmCarrier = true;
                }
                else if (TryResolveFastenerMotionParentPart(part, partsById, boundsByPart, out ExplodablePart parentPart) &&
                    parentPart != null)
                {
                    motionParentPart = ResolvePrimaryMotionParentForFastener(parentPart, partsById, boundsByPart);
                    if (motionParentPart != null &&
                        motionParentPart.Data != null &&
                        primaryWorldOffsetsById.TryGetValue(motionParentPart.Data.id, out Vector3 resolvedParentOffset))
                    {
                        parentWorldOffset = resolvedParentOffset;
                        motionParentByPart[part] = motionParentPart;
                    }
                }

                bool isInvertedArmM25x6Screw = IsInvertedArmM25x6Screw(part.transform, fastenerToken);
                Vector3 fastenerDirection = IsRubberGrommetToken(fastenerToken) && parentWorldOffset.sqrMagnitude > 0.0001f
                    ? Vector3.ProjectOnPlane(parentWorldOffset, Vector3.up).normalized
                    : ResolveFastenerWorldDirection(part.transform, center);
                if (fastenerDirection.sqrMagnitude < 0.0001f)
                {
                    fastenerDirection = ResolveFastenerWorldDirection(part.transform, center);
                }
                float fastenerDistance = ResolveFastenerDistance(part.transform);
                float secondaryStart = 0f;
                float secondaryEnd = fastenerTravelWindow;
                Vector3 tertiaryWorldOffset = Vector3.zero;
                float tertiaryStart = 0f;
                float tertiaryEnd = 1f;
                bool usesTertiaryOffset = false;
                if (isInvertedArmM25x6Screw)
                {
                    fastenerDirection = Vector3.down;
                    fastenerDistance = 0.24f;
                    secondaryStart = 0.00f;
                    secondaryEnd = fastenerTravelWindow;
                }
                else if (IsMotorCapScrewM3x6Token(fastenerToken))
                {
                    fastenerDirection = ResolveFastenerRelativeDirection(part.transform, motionParentPart, center, Vector3.up, 0.78f);
                    fastenerDistance = 0.68f;
                    secondaryStart = 0.24f;
                    secondaryEnd = 0.52f;
                }
                else if (isLongArmCapScrewM3x38)
                {
                    fastenerDirection = Vector3.up;
                    fastenerDistance = 1.08f;
                    secondaryStart = 0.00f;
                    secondaryEnd = 0.24f;
                }
                else if (isExplicitRailRubberGrommet)
                {
                    fastenerDirection = Vector3.up;
                    fastenerDistance = 0f;
                    secondaryStart = 0f;
                    secondaryEnd = 1f;
                    tertiaryWorldOffset = railRubberZOffset;
                    tertiaryStart = railRubberPositiveZ ? 0.42f : 0.38f;
                    tertiaryEnd = railRubberPositiveZ ? 0.98f : 0.96f;
                    usesTertiaryOffset = true;
                }
                else if (isRailBatteryFastener && IsCountersunkM3x16Token(fastenerToken))
                {
                    fastenerDirection = Vector3.down;
                    fastenerDistance = 0.48f;
                    secondaryStart = 0.00f;
                    secondaryEnd = 0.22f;
                    tertiaryWorldOffset = ResolveFrameForwardWorldDirection(part.transform, motionParentPart) * 0.46f;
                    tertiaryStart = 0.44f;
                    tertiaryEnd = 0.78f;
                    usesTertiaryOffset = true;
                }
                else if (isRailBatteryFastener &&
                         (IsSelfLockNutM25Token(fastenerToken) || IsNylonStandoffM25x5Token(fastenerToken)))
                {
                    fastenerDirection = Vector3.down;
                    fastenerDistance = IsNylonStandoffM25x5Token(fastenerToken) ? 0.34f : 0.28f;
                    secondaryStart = 0.00f;
                    secondaryEnd = 0.26f;
                }
                else if (isRailBatteryFastener && IsCapScrewM25x12Token(fastenerToken))
                {
                    fastenerDirection = Vector3.up;
                    fastenerDistance = 0.58f;
                    secondaryStart = 0.00f;
                    secondaryEnd = 0.24f;
                }
                else if (isArmFlangeNutM3)
                {
                    fastenerDirection = Vector3.down;
                    fastenerDistance = 0.12f;
                    secondaryStart = 0.00f;
                    secondaryEnd = 0.22f;
                }

                if (followsArmJibiCarrier)
                {
                    part.ConfigureRuntimeCompositeExplosionTarget(
                        parentWorldOffset,
                        0f,
                        1f,
                        fastenerDirection.normalized * fastenerDistance,
                        secondaryStart,
                        secondaryEnd,
                        carrierWorldOffset,
                        0.62f,
                        0.92f);
                    part.ConfigureCompositeTimelineSource(true, false);
                    part.ConfigureCompositeTertiaryEaseOut(true);
                }
                else if (usesTertiaryOffset)
                {
                    part.ConfigureRuntimeCompositeExplosionTarget(
                        parentWorldOffset,
                        0f,
                        1f,
                        fastenerDirection.normalized * fastenerDistance,
                        secondaryStart,
                        secondaryEnd,
                        tertiaryWorldOffset,
                        tertiaryStart,
                        tertiaryEnd);
                    part.ConfigureCompositeTimelineSource(true, false);
                }
                else if (followsArmCarrier)
                {
                    Vector3 resolvedCarrierOffset = carrierWorldOffset;
                    if (isInvertedArmM25x6Screw)
                    {
                        // These screws are mounted head-down below HMX5V-DIGAI-DIANJIZUO-MUJU:
                        // first they back out downward, then they keep following the lower cover.
                        resolvedCarrierOffset += Vector3.down * 0.52f;
                    }

                    part.ConfigureRuntimeCompositeExplosionTarget(
                        parentWorldOffset + resolvedCarrierOffset,
                        fastenerDirection.normalized * fastenerDistance,
                        secondaryStart,
                        secondaryEnd);
                }
                else
                {
                    part.ConfigureRuntimeCompositeExplosionTarget(
                        parentWorldOffset,
                        fastenerDirection.normalized * fastenerDistance,
                        secondaryStart,
                        secondaryEnd);
                }
            }
        }

        private static void ConfigurePrimaryExplosionTarget(ExplodablePart part, Vector3 worldOffset)
        {
            if (part == null)
            {
                return;
            }

            if (worldOffset.sqrMagnitude < 0.0001f)
            {
                part.ConfigureRuntimeExplosionTarget(Vector3.up, 0f);
                return;
            }

            if (IsFixedMotherPart(part))
            {
                part.ConfigureRuntimeExplosionTarget(Vector3.up, 0f);
                return;
            }

            if (IsRailBatteryPart(part))
            {
                part.ConfigureRuntimeCompositeExplosionTarget(
                    worldOffset,
                    RailBatteryTravelStart,
                    RailBatteryTravelEnd,
                    Vector3.zero,
                    0f,
                    1f);
                return;
            }

            part.ConfigureRuntimeExplosionTarget(worldOffset.normalized, worldOffset.magnitude);
        }

        private static void ConfigurePrimaryExplosionTargets(IEnumerable<ExplodablePart> groupedParts, Vector3 worldOffset)
        {
            if (groupedParts == null)
            {
                return;
            }

            foreach (ExplodablePart groupedPart in groupedParts)
            {
                ConfigurePrimaryExplosionTarget(groupedPart, worldOffset);
            }
        }

        private void ConfigurePerRendererRadialOffsets(
            IEnumerable<ExplodablePart> groupedParts,
            Vector3 center,
            Vector3 parentOutward,
            float parentDistance)
        {
            if (groupedParts == null)
            {
                return;
            }

            foreach (ExplodablePart groupedPart in groupedParts)
            {
                ConfigurePerRendererRadialOffsets(groupedPart, center, parentOutward, parentDistance);
            }
        }

        private void ConfigurePerRendererRadialOffsets(
            ExplodablePart part,
            Vector3 center,
            Vector3 parentOutward,
            float parentDistance)
        {
            if (!usePerRendererRadialOffsetsForArmSubpieces ||
                part == null ||
                part.Data == null ||
                !part.Data.id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Vector3 planarParentOutward = Vector3.ProjectOnPlane(parentOutward, Vector3.up);
            if (planarParentOutward.sqrMagnitude < 0.0001f)
            {
                planarParentOutward = parentOutward;
            }

            Vector3 parentDisplacement = planarParentOutward.sqrMagnitude > 0.0001f
                ? planarParentOutward.normalized * parentDistance
                : Vector3.zero;

            foreach (Renderer renderer in part.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || renderer.transform == null || renderer.transform == part.transform)
                {
                    continue;
                }

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category != null &&
                    string.Equals(category.AuxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string normalizedName = SelectionHierarchy.NormalizeToken(renderer.transform.name);
                if (HasExplicitArmAuxiliaryMotion(normalizedName))
                {
                    continue;
                }

                Vector3 childOutward = Vector3.ProjectOnPlane(renderer.bounds.center - center, Vector3.up);
                if (childOutward.sqrMagnitude < 0.0001f)
                {
                    continue;
                }

                Vector3 desiredDisplacement = childOutward.normalized * parentDistance;
                Vector3 offsetWorld = (desiredDisplacement - parentDisplacement) * Mathf.Max(0f, perRendererRadialOffsetStrength);
                float maxOffset = Mathf.Max(0f, maxPerRendererRadialOffset);
                if (maxOffset > 0f && offsetWorld.magnitude > maxOffset)
                {
                    offsetWorld = offsetWorld.normalized * maxOffset;
                }

                if (offsetWorld.sqrMagnitude < 0.000001f)
                {
                    continue;
                }

                Transform offsetParent = renderer.transform.parent;
                Vector3 localOffset = offsetParent != null
                    ? offsetParent.InverseTransformVector(offsetWorld)
                    : offsetWorld;
                if (localOffset.sqrMagnitude < 0.000001f)
                {
                    continue;
                }

                AuxiliaryExplodeOffset offset = renderer.transform.GetComponent<AuxiliaryExplodeOffset>();
                if (offset == null)
                {
                    offset = renderer.transform.gameObject.AddComponent<AuxiliaryExplodeOffset>();
                    offset.Configure(Vector3.up, 0f, 1f);
                }

                offset.ConfigureRadialCorrection(localOffset);
            }
        }

        private static bool HasExplicitArmAuxiliaryMotion(string normalizedName)
        {
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return false;
            }

            return normalizedName.Contains("propeller") ||
                   normalizedName.Contains("x500v2-prop") ||
                   normalizedName.Contains("dj-2216") ||
                   normalizedName.Contains("kv880") ||
                   normalizedName.Contains("ban-dj-dian-f2") ||
                   normalizedName.Contains("hmx5v-zuo-dj-muju") ||
                   normalizedName.Contains("hmx5v-digai-dianjizuo-muju") ||
                   normalizedName.Contains("hmx5v-jibi-jia-muju") ||
                   normalizedName.Contains("jia-guan") ||
                   normalizedName.Contains("hmx5v-guan-dingwei") ||
                   normalizedName.Contains("huan-guijiao") ||
                   normalizedName.Contains("rubber-grommet");
        }

        private bool TryResolveExplosionCenter(out Vector3 center)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null || part.Data == null)
                {
                    continue;
                }

                if (!string.Equals(part.Data.id, radialCenterPartId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (TryGetPartWorldBounds(part, out Bounds centerBounds))
                {
                    center = centerBounds.center;
                    return true;
                }

                center = part.transform.position;
                return true;
            }

            bool found = false;
            Bounds aggregate = default;
            foreach (ExplodablePart part in parts)
            {
                if (part == null || part.Data == null || part.Data.category == PartCategory.Fasteners)
                {
                    continue;
                }

                if (!TryGetPartWorldBounds(part, out Bounds bounds))
                {
                    continue;
                }

                if (!found)
                {
                    aggregate = bounds;
                    found = true;
                }
                else
                {
                    aggregate.Encapsulate(bounds);
                }
            }

            center = found ? aggregate.center : Vector3.zero;
            return found;
        }

        private static bool TryGetPartWorldBounds(ExplodablePart part, out Bounds bounds)
        {
            bounds = default;
            if (part == null)
            {
                return false;
            }

            string partId = part.Data != null ? part.Data.id : string.Empty;
            bool found = false;
            foreach (Renderer renderer in part.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                {
                    continue;
                }

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category != null)
                {
                    if (!string.IsNullOrWhiteSpace(partId) &&
                        !string.Equals(category.CanonicalPartId, partId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (string.Equals(category.AuxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found;
        }

        private float ResolveRadialDistance(ExplodablePart part, float centerDistance)
        {
            if (part != null && part.Data != null)
            {
                string id = part.Data.id ?? string.Empty;
                if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase))
                {
                    return 0f;
                }

                if (string.Equals(id, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase)) return TopPlateVerticalDistance;
                if (string.Equals(id, "x500v2_platform_board", StringComparison.OrdinalIgnoreCase)) return PlatformBoardVerticalDistance;
                if (string.Equals(id, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase)) return PixhawkVerticalDistance;
                if (string.Equals(id, "x500v2_pdb", StringComparison.OrdinalIgnoreCase)) return PowerModuleVerticalDistance;
                if (string.Equals(id, "x500v2_power_module", StringComparison.OrdinalIgnoreCase)) return PowerModuleVerticalDistance;
                if (string.Equals(id, "x500v2_gps_m10", StringComparison.OrdinalIgnoreCase)) return GpsVerticalDistance;
                if (string.Equals(id, "x500v2_telemetry_radio", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "x500v2_rc_receiver", StringComparison.OrdinalIgnoreCase))
                {
                    return TelemetryVerticalDistance;
                }

                if (string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase)) return RailBatteryVerticalDistance * 0.72f;
                if (string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase)) return RailBatteryVerticalDistance;
                if (string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase)) return LandingGearVerticalDistance;
            }

            float authoredDistance = part != null && part.Data != null ? Mathf.Max(0f, part.Data.explosionDistance) : 0f;
            float distanceFromCenter = centerDistance * radialDistanceScale;
            float resolved = Mathf.Max(authoredDistance, distanceFromCenter, minRadialDistance);
            if (part != null &&
                part.Data != null &&
                !string.IsNullOrWhiteSpace(part.Data.id) &&
                part.Data.id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase))
            {
                resolved *= ArmRadialDistanceMultiplier;
            }

            return Mathf.Min(resolved, maxRadialDistance);
        }

        private static Vector3 ResolveCenterFallbackDirection(ExplodablePart part)
        {
            if (part != null && part.Data != null)
            {
                string id = part.Data.id ?? string.Empty;
                if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase))
                {
                    return Vector3.zero;
                }

                if (string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase))
                {
                    return Vector3.down;
                }

                if (part.Data.explosionDirection.sqrMagnitude > 0.0001f)
                {
                    return part.Data.explosionDirection.normalized;
                }
            }

            return Vector3.up;
        }

        private static bool IsFastenerPart(ExplodablePart part)
        {
            if (part == null)
            {
                return false;
            }

            if (part.Data != null && part.Data.category == PartCategory.Fasteners)
            {
                return true;
            }

            return part.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase) ||
                   part.GetComponent<FastenerRuntimeMarker>() != null;
        }

        private static bool HasSameCanonicalExplodableAncestor(ExplodablePart part)
        {
            if (part == null || part.Data == null || string.IsNullOrWhiteSpace(part.Data.id))
            {
                return false;
            }

            string partId = part.Data.id;
            Transform current = part.transform.parent;
            while (current != null)
            {
                ExplodablePart ancestor = current.GetComponent<ExplodablePart>();
                if (ancestor != null &&
                    ancestor.Data != null &&
                    string.Equals(ancestor.Data.id, partId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private static bool IsDependentPropulsionPart(ExplodablePart part)
        {
            if (part == null || part.Data == null || string.IsNullOrWhiteSpace(part.Data.id))
            {
                return false;
            }

            return part.Data.id.StartsWith("x500v2_motor_", StringComparison.OrdinalIgnoreCase) ||
                   part.Data.id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryResolvePropulsionArmParentPart(
            ExplodablePart propulsionPart,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart,
            out ExplodablePart armPart)
        {
            armPart = null;
            if (propulsionPart == null ||
                propulsionPart.Data == null ||
                string.IsNullOrWhiteSpace(propulsionPart.Data.id) ||
                partsById == null)
            {
                return false;
            }

            string id = propulsionPart.Data.id;
            string suffix = string.Empty;
            if (id.StartsWith("x500v2_motor_", StringComparison.OrdinalIgnoreCase))
            {
                suffix = id.Substring("x500v2_motor_".Length);
            }
            else if (id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase))
            {
                suffix = id.Substring("x500v2_prop_".Length);
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                return false;
            }

            ExplodablePart suffixArm = null;
            partsById.TryGetValue("x500v2_arm_" + suffix.ToUpperInvariant(), out suffixArm);
            if (TryResolveNearestArmPart(propulsionPart, boundsByPart, out ExplodablePart nearestArm))
            {
                armPart = nearestArm;
                return true;
            }

            armPart = suffixArm;
            return armPart != null;
        }

        private static bool TryResolveNearestArmPart(
            ExplodablePart part,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart,
            out ExplodablePart nearestArm)
        {
            nearestArm = null;
            if (part == null || boundsByPart == null)
            {
                return false;
            }

            Vector3 point = TryGetPartWorldBounds(part, out Bounds partBounds)
                ? partBounds.center
                : part.transform.position;
            float bestScore = float.PositiveInfinity;
            foreach (KeyValuePair<ExplodablePart, Bounds> kvp in boundsByPart)
            {
                ExplodablePart candidate = kvp.Key;
                if (candidate == null ||
                    candidate.Data == null ||
                    string.IsNullOrWhiteSpace(candidate.Data.id) ||
                    !candidate.Data.id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                float score = DistanceToBounds(point, kvp.Value);
                if (score < bestScore)
                {
                    bestScore = score;
                    nearestArm = candidate;
                }
            }

            return nearestArm != null;
        }

        private static Vector3 ResolveDependentPropulsionSecondaryDirection(
            ExplodablePart part,
            Vector3 armWorldOffset,
            bool isProp)
        {
            Vector3 radial = Vector3.ProjectOnPlane(armWorldOffset, Vector3.up);
            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = part != null ? Vector3.ProjectOnPlane(part.transform.position, Vector3.up) : Vector3.forward;
            }

            Vector3 vertical = Vector3.up;
            float verticalWeight = isProp ? 0.92f : 0.82f;
            Vector3 blended = radial.normalized * (1f - verticalWeight) + vertical * verticalWeight;
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : vertical;
        }

        private static Vector3 ResolveDependentPropulsionSharedLiftDirection(ExplodablePart part, Vector3 armWorldOffset)
        {
            Vector3 radial = Vector3.ProjectOnPlane(armWorldOffset, Vector3.up);
            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = part != null ? Vector3.ProjectOnPlane(part.transform.position, Vector3.up) : Vector3.forward;
            }

            Vector3 vertical = Vector3.up;
            const float verticalWeight = 0.88f;
            Vector3 blended = radial.normalized * (1f - verticalWeight) + vertical * verticalWeight;
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : vertical;
        }

        private static ExplodablePart ResolvePrimaryMotionParentForFastener(
            ExplodablePart parentPart,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart)
        {
            if (parentPart == null)
            {
                return null;
            }

            if (!IsDependentPropulsionPart(parentPart))
            {
                return parentPart;
            }

            return TryResolvePropulsionArmParentPart(parentPart, partsById, boundsByPart, out ExplodablePart armPart) &&
                   armPart != null
                ? armPart
                : parentPart;
        }

        private static bool TryResolveArmMotionParentForFastener(
            ExplodablePart fastenerPart,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart,
            out ExplodablePart armPart)
        {
            armPart = null;
            if (fastenerPart == null || partsById == null)
            {
                return false;
            }

            string token = BuildFastenerToken(fastenerPart.transform);
            if (TryResolveArmFromKnownFastenerToken(token, partsById, out armPart))
            {
                return true;
            }

            string parentId = ResolveFastenerParentPartId(fastenerPart);
            if (TryResolveArmFromCanonicalParentId(parentId, partsById, out armPart))
            {
                return true;
            }

            if ((IsInvertedArmM25x6Screw(fastenerPart.transform, token) ||
                 IsMotorCapScrewM3x6Token(token) ||
                 IsArmFlangeNutM3Token(token)) &&
                TryResolveNearestArmPart(fastenerPart, boundsByPart, out armPart))
            {
                return true;
            }

            return false;
        }

        private static bool IsArmPart(ExplodablePart part)
        {
            return part != null &&
                   part.Data != null &&
                   !string.IsNullOrWhiteSpace(part.Data.id) &&
                   part.Data.id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRailBatteryPart(ExplodablePart part)
        {
            return part != null &&
                   part.Data != null &&
                   string.Equals(part.Data.id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRailBatteryParentId(string parentId)
        {
            return string.Equals(parentId, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFixedMotherPart(ExplodablePart part)
        {
            return part != null &&
                   part.Data != null &&
                   string.Equals(part.Data.id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsVerticalOnlyMotherPartId(string id)
        {
            return string.Equals(id, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_platform_board", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_pdb", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_power_module", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_gps_m10", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_telemetry_radio", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_rc_receiver", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase);
        }

        private static Vector3 ResolveVerticalOnlyMotherDirection(string id)
        {
            if (string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.down;
            }

            return Vector3.up;
        }

        private static Vector3 ConstrainMainPartOffset(string id, Vector3 offset)
        {
            if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.zero;
            }

            if (!IsVerticalOnlyMotherPartId(id))
            {
                return offset;
            }

            Vector3 direction = ResolveVerticalOnlyMotherDirection(id);
            float distance = Mathf.Max(0f, Vector3.Dot(offset, direction));
            if (string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase))
            {
                distance = Mathf.Min(distance, LandingGearVerticalDistance);
            }

            return direction * distance;
        }

        private static bool TryResolveArmFromKnownFastenerToken(
            string token,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            out ExplodablePart armPart)
        {
            armPart = null;
            if (string.IsNullOrWhiteSpace(token) || partsById == null)
            {
                return false;
            }

            string suffix = string.Empty;
            if (HasFastenerIndex(token, "cap-screw-m25x10", 1, 2) ||
                HasFastenerIndex(token, "cap-screw-m25x12", 1, 1) ||
                HasFastenerIndex(token, "cap-screw-m25x12", 5, 6) ||
                HasFastenerIndex(token, "cap-screw-m25x6", 9, 12) ||
                HasFastenerIndex(token, "cap-screw-m3x38", 1, 4) ||
                HasFastenerIndex(token, "cap-screw-m3x6", 1, 4) ||
                HasFastenerIndex(token, "flange-nut-m3", 1, 4))
            {
                suffix = "FL";
            }
            else if (HasFastenerIndex(token, "cap-screw-m25x10", 3, 4) ||
                     HasFastenerIndex(token, "cap-screw-m25x12", 7, 8) ||
                     HasFastenerIndex(token, "cap-screw-m25x6", 13, 16) ||
                     HasFastenerIndex(token, "cap-screw-m3x38", 5, 8) ||
                     HasFastenerIndex(token, "cap-screw-m3x6", 5, 8) ||
                     HasFastenerIndex(token, "flange-nut-m3", 5, 8))
            {
                suffix = "FR";
            }
            else if (HasFastenerIndex(token, "cap-screw-m25x10", 5, 6) ||
                     HasFastenerIndex(token, "cap-screw-m25x12", 9, 10) ||
                     HasFastenerIndex(token, "cap-screw-m25x6", 17, 20) ||
                     HasFastenerIndex(token, "cap-screw-m3x38", 9, 12) ||
                     HasFastenerIndex(token, "cap-screw-m3x6", 9, 12) ||
                     HasFastenerIndex(token, "flange-nut-m3", 9, 12))
            {
                suffix = "BR";
            }
            else if (HasFastenerIndex(token, "cap-screw-m25x10", 7, 8) ||
                     HasFastenerIndex(token, "cap-screw-m25x12", 11, 12) ||
                     HasFastenerIndex(token, "cap-screw-m25x6", 21, 24) ||
                     HasFastenerIndex(token, "cap-screw-m3x38", 13, 16) ||
                     HasFastenerIndex(token, "cap-screw-m3x6", 13, 16) ||
                     HasFastenerIndex(token, "flange-nut-m3", 13, 16))
            {
                suffix = "BL";
            }

            return !string.IsNullOrWhiteSpace(suffix) &&
                   partsById.TryGetValue("x500v2_arm_" + suffix, out armPart) &&
                   armPart != null;
        }

        private static bool TryResolveArmFromCanonicalParentId(
            string parentId,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            out ExplodablePart armPart)
        {
            armPart = null;
            if (string.IsNullOrWhiteSpace(parentId) || partsById == null)
            {
                return false;
            }

            if (parentId.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase))
            {
                return partsById.TryGetValue(parentId, out armPart) && armPart != null;
            }

            string suffix = string.Empty;
            if (parentId.StartsWith("x500v2_motor_", StringComparison.OrdinalIgnoreCase))
            {
                suffix = parentId.Substring("x500v2_motor_".Length);
            }
            else if (parentId.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase))
            {
                suffix = parentId.Substring("x500v2_prop_".Length);
            }

            return !string.IsNullOrWhiteSpace(suffix) &&
                   partsById.TryGetValue("x500v2_arm_" + suffix.ToUpperInvariant(), out armPart) &&
                   armPart != null;
        }

        private static Vector3 ResolveFastenerRelativeDirection(
            Transform fastener,
            ExplodablePart motionParentPart,
            Vector3 fallbackCenter,
            Vector3 axialDirection,
            float axialWeight)
        {
            Vector3 parentCenter = motionParentPart != null && TryGetPartWorldBounds(motionParentPart, out Bounds parentBounds)
                ? parentBounds.center
                : fallbackCenter;
            Vector3 fastenerCenter = fastener != null ? ResolveTransformWorldCenter(fastener) : parentCenter;
            Vector3 radial = Vector3.ProjectOnPlane(fastenerCenter - parentCenter, Vector3.up);
            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = Vector3.ProjectOnPlane(fastenerCenter - fallbackCenter, Vector3.up);
            }

            Vector3 resolvedRadial = radial.sqrMagnitude > 0.0001f ? radial.normalized : Vector3.zero;
            Vector3 resolvedAxial = axialDirection.sqrMagnitude > 0.0001f ? axialDirection.normalized : Vector3.up;
            Vector3 blended = resolvedRadial * Mathf.Clamp01(1f - axialWeight) + resolvedAxial * Mathf.Clamp01(axialWeight);
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : resolvedAxial;
        }

        private static Vector3 ResolveTransformWorldCenter(Transform transform)
        {
            if (transform == null)
            {
                return Vector3.zero;
            }

            Renderer renderer = transform.GetComponentInChildren<Renderer>(true);
            return renderer != null ? renderer.bounds.center : transform.position;
        }

        private Dictionary<string, List<ExplodablePart>> BuildGroupedPartsById()
        {
            Dictionary<string, List<ExplodablePart>> lookup = new Dictionary<string, List<ExplodablePart>>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < parts.Count; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null || part.Data == null || string.IsNullOrWhiteSpace(part.Data.id))
                {
                    continue;
                }

                string partId = part.Data.id;
                if (!lookup.TryGetValue(partId, out List<ExplodablePart> group))
                {
                    group = new List<ExplodablePart>();
                    lookup.Add(partId, group);
                }

                group.Add(part);
            }

            return lookup;
        }

        private Dictionary<string, ExplodablePart> BuildPartsById()
        {
            return BuildPartsById(BuildGroupedPartsById());
        }

        private static Dictionary<string, ExplodablePart> BuildPartsById(IReadOnlyDictionary<string, List<ExplodablePart>> groupedParts)
        {
            Dictionary<string, ExplodablePart> lookup = new Dictionary<string, ExplodablePart>(StringComparer.OrdinalIgnoreCase);
            if (groupedParts == null)
            {
                return lookup;
            }

            foreach (KeyValuePair<string, List<ExplodablePart>> kvp in groupedParts)
            {
                ExplodablePart representative = SelectRepresentativePart(kvp.Value);
                if (representative != null && !string.IsNullOrWhiteSpace(kvp.Key))
                {
                    lookup[kvp.Key] = representative;
                }
            }

            return lookup;
        }

        private static ExplodablePart SelectRepresentativePart(IReadOnlyList<ExplodablePart> groupedParts)
        {
            if (groupedParts == null || groupedParts.Count == 0)
            {
                return null;
            }

            ExplodablePart best = null;
            float bestScore = float.NegativeInfinity;
            for (int i = 0; i < groupedParts.Count; i++)
            {
                ExplodablePart candidate = groupedParts[i];
                if (candidate == null)
                {
                    continue;
                }

                float score = 0f;
                if (!HasSameCanonicalExplodableAncestor(candidate))
                {
                    score += 10000f;
                }

                Renderer[] renderers = candidate.GetComponentsInChildren<Renderer>(true);
                score += renderers != null ? renderers.Length * 0.1f : 0f;
                if (TryGetPartWorldBounds(candidate, out Bounds bounds))
                {
                    Vector3 size = bounds.size;
                    float volume = Mathf.Max(0.000001f, size.x * size.y * size.z);
                    score += Mathf.Log10(1f + volume) * 10f;
                }

                if (best == null || score > bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            return best;
        }

        private Dictionary<ExplodablePart, Bounds> BuildBoundsByPart()
        {
            Dictionary<ExplodablePart, Bounds> lookup = new Dictionary<ExplodablePart, Bounds>();
            for (int i = 0; i < parts.Count; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null || part.Data == null || IsFastenerPart(part))
                {
                    continue;
                }

                if (TryGetPartWorldBounds(part, out Bounds bounds))
                {
                    lookup[part] = bounds;
                }
            }

            return lookup;
        }

        private static Dictionary<string, Bounds> BuildBoundsById(IReadOnlyDictionary<string, List<ExplodablePart>> groupedParts)
        {
            Dictionary<string, Bounds> lookup = new Dictionary<string, Bounds>(StringComparer.OrdinalIgnoreCase);
            if (groupedParts == null)
            {
                return lookup;
            }

            foreach (KeyValuePair<string, List<ExplodablePart>> kvp in groupedParts)
            {
                bool found = false;
                Bounds aggregate = default;
                List<ExplodablePart> group = kvp.Value;
                if (group == null)
                {
                    continue;
                }

                for (int i = 0; i < group.Count; i++)
                {
                    ExplodablePart part = group[i];
                    if (part == null || part.Data == null || IsFastenerPart(part))
                    {
                        continue;
                    }

                    if (!TryGetPartWorldBounds(part, out Bounds bounds))
                    {
                        continue;
                    }

                    if (!found)
                    {
                        aggregate = bounds;
                        found = true;
                    }
                    else
                    {
                        aggregate.Encapsulate(bounds);
                    }
                }

                if (found)
                {
                    lookup[kvp.Key] = aggregate;
                }
            }

            return lookup;
        }

        private static float DistanceToBounds(Vector3 point, Bounds bounds)
        {
            Vector3 closest = bounds.ClosestPoint(point);
            return Vector3.Distance(point, closest);
        }

        private void EnforceMinimumFinalSeparation(
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<string, Bounds> boundsById,
            Dictionary<string, Vector3> offsetsById)
        {
            if (partsById == null ||
                boundsById == null ||
                offsetsById == null ||
                minimumFinalBoundsSeparation <= 0f)
            {
                return;
            }

            List<string> layoutIds = new List<string>();
            foreach (KeyValuePair<string, Vector3> kvp in offsetsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart part) ||
                    part == null ||
                    part.Data == null ||
                    !boundsById.ContainsKey(kvp.Key) ||
                    IsFastenerPart(part) ||
                    IsDependentPropulsionPart(part))
                {
                    continue;
                }

                layoutIds.Add(kvp.Key);
            }

            int iterations = Mathf.Clamp(finalSeparationIterations, 0, 12);
            float padding = Mathf.Max(0f, minimumFinalBoundsSeparation);
            float strength = Mathf.Clamp01(finalSeparationStrength);
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                bool changed = false;
                for (int i = 0; i < layoutIds.Count; i++)
                {
                    string idA = layoutIds[i];
                    if (!partsById.TryGetValue(idA, out ExplodablePart a) || a == null || a.Data == null)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < layoutIds.Count; j++)
                    {
                        string idB = layoutIds[j];
                        if (!partsById.TryGetValue(idB, out ExplodablePart b) || b == null || b.Data == null)
                        {
                            continue;
                        }

                        if (!offsetsById.TryGetValue(idA, out Vector3 offsetA) ||
                            !offsetsById.TryGetValue(idB, out Vector3 offsetB) ||
                            !boundsById.TryGetValue(idA, out Bounds boundsA) ||
                            !boundsById.TryGetValue(idB, out Bounds boundsB))
                        {
                            continue;
                        }

                        Bounds finalA = TranslateBounds(boundsA, offsetA);
                        Bounds finalB = TranslateBounds(boundsB, offsetB);
                        Bounds paddedA = ExpandBounds(finalA, padding);
                        Bounds paddedB = ExpandBounds(finalB, padding);
                        if (!paddedA.Intersects(paddedB))
                        {
                            continue;
                        }

                        Vector3 separationAxis = finalA.center - finalB.center;
                        if (separationAxis.sqrMagnitude < 0.0001f)
                        {
                            separationAxis = offsetA - offsetB;
                        }
                        if (separationAxis.sqrMagnitude < 0.0001f)
                        {
                            separationAxis = Vector3.up;
                        }

                        separationAxis.Normalize();
                        float push = Mathf.Max(padding, EstimateOverlapPush(paddedA, paddedB)) * 0.5f * strength;
                        bool aLocked = IsExplosionLayoutLocked(a);
                        bool bLocked = IsExplosionLayoutLocked(b);
                        if (!aLocked)
                        {
                            offsetsById[idA] = ConstrainMainPartOffset(idA, offsetA + separationAxis * push);
                        }
                        if (!bLocked)
                        {
                            offsetsById[idB] = ConstrainMainPartOffset(idB, offsetB - separationAxis * push);
                        }
                        changed = true;
                    }
                }

                if (!changed)
                {
                    break;
                }
            }
        }

        private void EnforceMinimumTimelineSeparation(
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<string, Bounds> boundsById,
            Dictionary<string, Vector3> offsetsById)
        {
            if (partsById == null ||
                boundsById == null ||
                offsetsById == null ||
                minimumFinalBoundsSeparation <= 0f)
            {
                return;
            }

            List<string> layoutIds = new List<string>();
            foreach (KeyValuePair<string, Vector3> kvp in offsetsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart part) ||
                    part == null ||
                    part.Data == null ||
                    !boundsById.ContainsKey(kvp.Key) ||
                    IsFastenerPart(part) ||
                    IsDependentPropulsionPart(part))
                {
                    continue;
                }

                layoutIds.Add(kvp.Key);
            }

            if (layoutIds.Count < 2)
            {
                return;
            }

            float[] sampleFactors = { 0.30f, 0.40f, 0.50f, 0.62f, 0.74f, 0.86f };
            float padding = Mathf.Max(0f, minimumFinalBoundsSeparation * 0.75f);
            float strength = Mathf.Clamp01(finalSeparationStrength) * 0.45f;
            int iterations = Mathf.Clamp(finalSeparationIterations / 2, 1, 6);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                bool changed = false;
                for (int s = 0; s < sampleFactors.Length; s++)
                {
                    float sample = sampleFactors[s];
                    for (int i = 0; i < layoutIds.Count; i++)
                    {
                        string idA = layoutIds[i];
                        if (!partsById.TryGetValue(idA, out ExplodablePart a) ||
                            a == null ||
                            !offsetsById.TryGetValue(idA, out Vector3 targetOffsetA) ||
                            !boundsById.TryGetValue(idA, out Bounds baseBoundsA))
                        {
                            continue;
                        }

                        for (int j = i + 1; j < layoutIds.Count; j++)
                        {
                            string idB = layoutIds[j];
                            if (!partsById.TryGetValue(idB, out ExplodablePart b) ||
                                b == null ||
                                !offsetsById.TryGetValue(idB, out Vector3 targetOffsetB) ||
                                !boundsById.TryGetValue(idB, out Bounds baseBoundsB))
                            {
                                continue;
                            }

                            Bounds sampleA = TranslateBounds(baseBoundsA, EvaluateLayoutOffset(a, targetOffsetA, sample));
                            Bounds sampleB = TranslateBounds(baseBoundsB, EvaluateLayoutOffset(b, targetOffsetB, sample));
                            Bounds paddedA = ExpandBounds(sampleA, padding);
                            Bounds paddedB = ExpandBounds(sampleB, padding);
                            if (!paddedA.Intersects(paddedB))
                            {
                                continue;
                            }

                            Vector3 separationAxis = sampleA.center - sampleB.center;
                            if (separationAxis.sqrMagnitude < 0.0001f)
                            {
                                separationAxis = targetOffsetA - targetOffsetB;
                            }
                            if (separationAxis.sqrMagnitude < 0.0001f)
                            {
                                separationAxis = Vector3.up;
                            }

                            separationAxis.Normalize();
                            float push = Mathf.Max(padding, EstimateOverlapPush(paddedA, paddedB)) * strength;
                            bool aLocked = IsExplosionLayoutLocked(a);
                            bool bLocked = IsExplosionLayoutLocked(b);
                            if (!aLocked)
                            {
                                offsetsById[idA] = ConstrainMainPartOffset(idA, targetOffsetA + separationAxis * push);
                            }
                            if (!bLocked)
                            {
                                offsetsById[idB] = ConstrainMainPartOffset(idB, targetOffsetB - separationAxis * push);
                            }

                            changed = true;
                        }
                    }
                }

                if (!changed)
                {
                    break;
                }
            }
        }

        private Vector3 EvaluateLayoutOffset(ExplodablePart part, Vector3 targetOffset, float globalFactor)
        {
            if (targetOffset.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            if (IsArmPart(part))
            {
                return targetOffset * ResolvePartFactor(part, globalFactor);
            }

            if (IsRailBatteryPart(part))
            {
                float rail = SmoothStep(Mathf.InverseLerp(RailBatteryTravelStart, RailBatteryTravelEnd, Mathf.Clamp01(globalFactor)));
                return targetOffset * rail;
            }

            return targetOffset * ResolvePartFactor(part, globalFactor);
        }

        private static bool IsExplosionLayoutLocked(ExplodablePart part)
        {
            if (part == null || part.Data == null)
            {
                return false;
            }

            string id = part.Data.id ?? string.Empty;
            return string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase);
        }

        private static Bounds TranslateBounds(Bounds bounds, Vector3 offset)
        {
            bounds.center += offset;
            return bounds;
        }

        private static Bounds ExpandBounds(Bounds bounds, float padding)
        {
            bounds.Expand(padding * 2f);
            return bounds;
        }

        private static float EstimateOverlapPush(Bounds a, Bounds b)
        {
            float overlapX = Mathf.Min(a.max.x, b.max.x) - Mathf.Max(a.min.x, b.min.x);
            float overlapY = Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y);
            float overlapZ = Mathf.Min(a.max.z, b.max.z) - Mathf.Max(a.min.z, b.min.z);
            float minOverlap = Mathf.Min(Mathf.Max(0f, overlapX), Mathf.Max(0f, overlapY), Mathf.Max(0f, overlapZ));
            return Mathf.Max(minOverlap, 0.01f);
        }

        private static float ScoreMotionParentCandidate(Vector3 point, Bounds bounds)
        {
            float surfaceDistance = DistanceToBounds(point, bounds);
            float centerDistance = Vector3.Distance(point, bounds.center);
            return surfaceDistance + centerDistance * 0.015f;
        }

        private static bool TryResolveFastenerParentPart(
            ExplodablePart fastenerPart,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            out ExplodablePart parentPart)
        {
            parentPart = null;
            if (fastenerPart == null || partsById == null)
            {
                return false;
            }

            string parentId = ResolveFastenerParentPartId(fastenerPart);
            if (string.IsNullOrWhiteSpace(parentId) ||
                string.Equals(parentId, "x500v2_fastener_group", StringComparison.OrdinalIgnoreCase) ||
                parentId.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return partsById.TryGetValue(parentId, out parentPart) && parentPart != null;
        }

        private static bool TryResolveFastenerMotionParentPart(
            ExplodablePart fastenerPart,
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart,
            out ExplodablePart parentPart)
        {
            parentPart = null;
            if (fastenerPart == null || partsById == null || boundsByPart == null)
            {
                return false;
            }

            TryResolveFastenerParentPart(fastenerPart, partsById, out ExplodablePart explicitParent);
            Vector3 point = TryGetPartWorldBounds(fastenerPart, out Bounds fastenerBounds)
                ? fastenerBounds.center
                : fastenerPart.transform.position;

            float explicitScore = explicitParent != null && boundsByPart.TryGetValue(explicitParent, out Bounds explicitBounds)
                ? ScoreMotionParentCandidate(point, explicitBounds)
                : float.PositiveInfinity;

            ExplodablePart nearest = null;
            float bestScore = float.PositiveInfinity;
            foreach (KeyValuePair<ExplodablePart, Bounds> kvp in boundsByPart)
            {
                ExplodablePart candidate = kvp.Key;
                if (candidate == null || candidate == fastenerPart || IsFastenerPart(candidate))
                {
                    continue;
                }

                string id = candidate.Data != null ? candidate.Data.id ?? string.Empty : string.Empty;
                if (id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                float score = ScoreMotionParentCandidate(point, kvp.Value);
                if (explicitParent != null && candidate == explicitParent)
                {
                    score *= 0.9f;
                }

                if (score < bestScore)
                {
                    bestScore = score;
                    nearest = candidate;
                }
            }

            if (nearest == null && explicitParent == null)
            {
                return false;
            }

            // Prefer the spatial contact parent when it is materially closer. This
            // fixes screws whose metadata is canonical, but whose head actually
            // rides with another plate during the visual disassembly.
            if (nearest != null &&
                (explicitParent == null || bestScore <= explicitScore * 0.75f || bestScore < 0.12f))
            {
                parentPart = nearest;
                return true;
            }

            parentPart = explicitParent != null ? explicitParent : nearest;
            return parentPart != null;
        }

        private static string ResolveFastenerParentPartId(ExplodablePart fastenerPart)
        {
            if (fastenerPart == null)
            {
                return string.Empty;
            }

            FastenerRuntimeMarker marker = fastenerPart.GetComponent<FastenerRuntimeMarker>();
            if (marker != null && !string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId))
            {
                return marker.ParentCanonicalPartId;
            }

            if (fastenerPart.Data != null &&
                fastenerPart.Data.HasFastenerMetadata &&
                !string.IsNullOrWhiteSpace(fastenerPart.Data.fastenerMetadata.parentCanonicalPartId))
            {
                return fastenerPart.Data.fastenerMetadata.parentCanonicalPartId;
            }

            PartRenderCategory category = fastenerPart.GetComponent<PartRenderCategory>();
            if (category != null && !string.IsNullOrWhiteSpace(category.CanonicalPartId))
            {
                return category.CanonicalPartId;
            }

            category = fastenerPart.GetComponentInChildren<PartRenderCategory>(true);
            return category != null ? category.CanonicalPartId : string.Empty;
        }

        private Vector3 ResolveMainPartDisassemblyDirection(ExplodablePart part, Vector3 rawOutward, Vector3 center)
        {
            Vector3 planarOutward = Vector3.ProjectOnPlane(rawOutward, Vector3.up);
            if (planarOutward.sqrMagnitude < 0.0001f)
            {
                planarOutward = rawOutward;
            }

            Vector3 radial = planarOutward.sqrMagnitude > 0.0001f ? planarOutward.normalized : Vector3.zero;
            if (part == null || part.Data == null)
            {
                return radial.sqrMagnitude > 0.0001f ? radial : Vector3.up;
            }

            string id = part.Data.id ?? string.Empty;
            string type = part.Data.partType ?? string.Empty;
            string normalized = (id + " " + type).ToLowerInvariant();

            if (id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase))
            {
                return BlendDirections(radial, Vector3.up, 0.75f);
            }

            if (id.StartsWith("x500v2_motor_", StringComparison.OrdinalIgnoreCase))
            {
                return BlendDirections(radial, Vector3.up, 0.68f);
            }

            if (id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase))
            {
                if (TryResolveArmEndpointAxis(part, center, out Vector3 armAxis))
                {
                    return armAxis;
                }

                return radial.sqrMagnitude > 0.0001f ? radial : ResolveCenterFallbackDirection(part);
            }

            if (string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.down;
            }

            if (string.Equals(id, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_platform_board", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_pdb", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_power_module", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_gps_m10", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_telemetry_radio", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_rc_receiver", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("gps") ||
                normalized.Contains("telemetry") ||
                normalized.Contains("radio"))
            {
                return Vector3.up;
            }

            if (string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.down;
            }

            if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.zero;
            }

            return radial.sqrMagnitude > 0.0001f ? radial : ResolveCenterFallbackDirection(part);
        }

        private static bool TryResolveArmEndpointAxis(ExplodablePart part, Vector3 center, out Vector3 axis)
        {
            axis = Vector3.zero;
            if (part == null)
            {
                return false;
            }

            Vector3 bestPoint = Vector3.zero;
            float bestScore = float.NegativeInfinity;
            foreach (Renderer renderer in part.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || renderer.transform == null)
                {
                    continue;
                }

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category != null &&
                    string.Equals(category.AuxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string normalized = SelectionHierarchy.NormalizeToken(renderer.transform.name);
                Vector3 planarOffset = Vector3.ProjectOnPlane(renderer.bounds.center - center, Vector3.up);
                if (planarOffset.sqrMagnitude < 0.0001f)
                {
                    continue;
                }

                float semanticBonus = 0f;
                if (normalized.Contains("dj-2216") || normalized.Contains("kv880"))
                {
                    semanticBonus = 1000f;
                }
                else if (normalized.Contains("propeller") || normalized.Contains("x500v2-prop"))
                {
                    semanticBonus = 750f;
                }
                else if (normalized.Contains("hmx5v-zuo-dj-muju") ||
                         normalized.Contains("hmx5v-digai-dianjizuo-muju") ||
                         normalized.Contains("ban-dj-dian-f2"))
                {
                    semanticBonus = 250f;
                }

                float score = semanticBonus + planarOffset.sqrMagnitude;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = renderer.bounds.center;
                }
            }

            axis = Vector3.ProjectOnPlane(bestPoint - center, Vector3.up);
            if (axis.sqrMagnitude < 0.0001f)
            {
                return false;
            }

            axis.Normalize();
            return true;
        }

        private float ResolveFastenerDistance(Transform fastener)
        {
            string token = BuildFastenerToken(fastener);

            if (IsNutToken(token))
            {
                return nutAxisDistance;
            }

            if (IsRubberGrommetToken(token))
            {
                return 0.16f;
            }

            if (token.Contains("standoff") || token.Contains("nilongzhu"))
            {
                return spacerAxisDistance;
            }

            return screwAxisDistance;
        }

        private static Vector3 ResolveFastenerWorldDirection(Transform fastener, Vector3 center)
        {
            if (fastener == null)
            {
                return Vector3.up;
            }

            string token = BuildFastenerToken(fastener);
            Vector3 axis = ResolveLikelyFastenerAxis(fastener);
            Vector3 away = Vector3.ProjectOnPlane(fastener.position - center, Vector3.up);
            if (away.sqrMagnitude < 0.0001f)
            {
                away = fastener.position - center;
            }
            if (away.sqrMagnitude < 0.0001f)
            {
                away = Vector3.up;
            }
            away.Normalize();

            bool isNut = IsNutToken(token);
            if (IsRubberGrommetToken(token))
            {
                return away;
            }

            bool isSpacer = token.Contains("standoff") || token.Contains("nilongzhu");
            if (axis.sqrMagnitude < 0.0001f)
            {
                return isNut ? Vector3.down : Vector3.up;
            }

            axis.Normalize();
            float verticalDot = Vector3.Dot(axis, Vector3.up);
            if (Mathf.Abs(verticalDot) > 0.35f)
            {
                if (isNut)
                {
                    return verticalDot > 0f ? -axis : axis;
                }

                if (isSpacer)
                {
                    return Vector3.Dot(fastener.position - center, Vector3.up) >= 0f
                        ? (verticalDot > 0f ? axis : -axis)
                        : (verticalDot > 0f ? -axis : axis);
                }

                return verticalDot > 0f ? axis : -axis;
            }

            return Vector3.Dot(axis, away) >= 0f ? axis : -axis;
        }

        private static Vector3 ResolveFrameForwardWorldDirection(Transform member, ExplodablePart anchor)
        {
            Transform root = anchor != null && anchor.transform != null
                ? anchor.transform.root
                : member != null ? member.root : null;
            Vector3 center = anchor != null && TryGetPartWorldBounds(anchor, out Bounds anchorBounds)
                ? anchorBounds.center
                : (anchor != null ? anchor.transform.position : Vector3.zero);

            if (root != null)
            {
                if (TryGetNamedRendererBounds(root, "bottom-plate-x500-v5", out Bounds bottomBounds))
                {
                    center = bottomBounds.center;
                }
                else if (TryGetRendererBounds(root, out Bounds rootBounds))
                {
                    center = rootBounds.center;
                }

                if (TryGetNamedRendererBounds(root, "zhijia-camera-intel", out Bounds cameraBounds))
                {
                    Vector3 cameraFront = Vector3.ProjectOnPlane(cameraBounds.center - center, Vector3.up);
                    if (cameraFront.sqrMagnitude > 0.0001f)
                    {
                        return cameraFront.normalized;
                    }
                }
            }

            Vector3 fallback = member != null
                ? Vector3.ProjectOnPlane(member.position - center, Vector3.up)
                : Vector3.forward;
            return fallback.sqrMagnitude > 0.0001f ? fallback.normalized : Vector3.forward;
        }

        private static bool TryGetNamedRendererBounds(Transform root, string normalizedNameToken, out Bounds bounds)
        {
            bounds = default;
            if (root == null || string.IsNullOrWhiteSpace(normalizedNameToken))
            {
                return false;
            }

            bool found = false;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                string normalized = SelectionHierarchy.NormalizeToken(renderer.transform.name);
                if (!normalized.Contains(normalizedNameToken))
                {
                    continue;
                }

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found;
        }

        private static bool TryGetRendererBounds(Transform root, out Bounds bounds)
        {
            bounds = default;
            if (root == null)
            {
                return false;
            }

            bool found = false;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == root)
                {
                    continue;
                }

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found;
        }

        private static Vector3 ResolveLikelyFastenerAxis(Transform fastener)
        {
            Renderer renderer = fastener != null ? fastener.GetComponentInChildren<Renderer>(true) : null;
            MeshFilter meshFilter = renderer != null ? renderer.GetComponent<MeshFilter>() : fastener.GetComponentInChildren<MeshFilter>(true);
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return fastener != null ? fastener.up : Vector3.up;
            }

            Vector3 size = meshFilter.sharedMesh.bounds.size;
            Vector3 localAxis = Vector3.up;
            if (size.x >= size.y && size.x >= size.z)
            {
                localAxis = Vector3.right;
            }
            else if (size.z >= size.x && size.z >= size.y)
            {
                localAxis = Vector3.forward;
            }

            Transform axisTransform = meshFilter.transform != null ? meshFilter.transform : fastener;
            return axisTransform.TransformDirection(localAxis);
        }

        private static bool IsNutToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return token.Contains("nut") ||
                   token.Contains("luomao") ||
                   token.Contains("lm-m3") ||
                   token.Contains("zslm") ||
                   token.Contains("falan");
        }

        private static bool IsRubberGrommetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return token.Contains("grommet") ||
                   token.Contains("rubber") ||
                   token.Contains("huan-guijiao");
        }

        private static bool IsRailRubberGrommetFastenerToken(string token)
        {
            if (!IsRubberGrommetToken(token))
            {
                return false;
            }

            return HasFastenerIndexAnyFamily(
                token,
                new[] { "rubber-grommet", "rubber-gromet", "huan-guijiao", "grommet" },
                2,
                8);
        }

        private static Vector3 ResolveRailRubberGrommetZOffset(
            Transform fastener,
            ExplodablePart railBatteryPart,
            string token,
            out bool positiveZ)
        {
            positiveZ = false;
            if (fastener == null || railBatteryPart == null)
            {
                return Vector3.zero;
            }

            Vector3 modelZ = -ResolveFrameForwardWorldDirection(fastener, railBatteryPart);
            if (modelZ.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            modelZ.Normalize();
            Vector3 center = TryGetPartWorldBounds(railBatteryPart, out Bounds railBounds)
                ? railBounds.center
                : railBatteryPart.transform.position;
            Vector3 planarOffset = Vector3.ProjectOnPlane(fastener.position - center, Vector3.up);
            if (planarOffset.sqrMagnitude > 0.0001f)
            {
                positiveZ = Vector3.Dot(planarOffset, modelZ) >= 0f;
            }
            else if (HasFastenerIndexAnyFamily(token, new[] { "rubber-grommet", "rubber-gromet", "huan-guijiao", "grommet" }, 4, 8))
            {
                positiveZ = true;
            }

            return (positiveZ ? modelZ : -modelZ) * 1.70f;
        }

        private static string BuildFastenerToken(Transform fastener)
        {
            string normalized = SelectionHierarchy.NormalizeToken(fastener != null ? fastener.name : string.Empty);
            FastenerRuntimeMarker marker = fastener != null ? fastener.GetComponent<FastenerRuntimeMarker>() : null;
            AppendNormalizedToken(ref normalized, marker != null ? marker.SceneTypeKey : string.Empty);
            AppendNormalizedToken(ref normalized, marker != null ? marker.FastenerInstanceId : string.Empty);
            AppendNormalizedToken(ref normalized, marker != null ? marker.FastenerFamilyId : string.Empty);

            ExplodablePart part = fastener != null ? fastener.GetComponent<ExplodablePart>() : null;
            DronePartData data = part != null ? part.Data : null;
            AppendNormalizedToken(ref normalized, data != null ? data.id : string.Empty);
            AppendNormalizedToken(ref normalized, data != null ? data.partName : string.Empty);

            FastenerMetadata metadata = data != null && data.HasFastenerMetadata ? data.fastenerMetadata : null;
            AppendNormalizedToken(ref normalized, metadata != null ? metadata.instanceId : string.Empty);
            AppendNormalizedToken(ref normalized, metadata != null ? metadata.sceneObjectName : string.Empty);
            AppendNormalizedToken(ref normalized, metadata != null ? metadata.sceneTypeKey : string.Empty);
            AppendNormalizedToken(ref normalized, metadata != null ? metadata.familyId : string.Empty);
            AppendNormalizedToken(ref normalized, metadata != null ? metadata.blenderName : string.Empty);
            return normalized;
        }

        private static void AppendNormalizedToken(ref string token, string rawValue)
        {
            string normalizedValue = SelectionHierarchy.NormalizeToken(rawValue);
            if (string.IsNullOrWhiteSpace(normalizedValue))
            {
                return;
            }

            token = string.IsNullOrWhiteSpace(token) ? normalizedValue : token + "-" + normalizedValue;
        }

        private static bool IsInvertedArmM25x6Screw(Transform fastener, string token)
        {
            if (IsInvertedArmM25x6ScrewToken(token))
            {
                return true;
            }

            string normalizedName = SelectionHierarchy.NormalizeToken(fastener != null ? fastener.name : string.Empty);
            FastenerRuntimeMarker marker = fastener != null ? fastener.GetComponent<FastenerRuntimeMarker>() : null;
            string markerToken = marker != null
                ? SelectionHierarchy.NormalizeToken(marker.SceneTypeKey) + "-" +
                  SelectionHierarchy.NormalizeToken(marker.FastenerInstanceId)
                : string.Empty;
            return IsInvertedArmM25x6ScrewToken(normalizedName + "-" + markerToken);
        }

        private static bool IsInvertedArmM25x6ScrewToken(string token)
        {
            return HasFastenerIndexAnyFamily(
                token,
                new[]
                {
                    "cap-screw-m25x6",
                    "cap-screw-m25-6",
                    "cap-screw-m2-5x6",
                    "gb70-m25-6",
                    "m25x6"
                },
                11, 12, 15, 16, 19, 20, 23, 24);
        }

        private static bool IsMotorCapScrewM3x6Token(string token)
        {
            return HasFastenerIndexAnyFamily(token, new[] { "cap-screw-m3x6", "cap-screw-m3-6", "gb70-m3-6", "m3x6" }, 1, 16);
        }

        private static bool IsArmFlangeNutM3Token(string token)
        {
            return HasFastenerIndexAnyFamily(token, new[] { "flange-nut-m3", "zslm-m3-falan", "m3-falan" }, 1, 16);
        }

        private static bool IsLongArmCapScrewM3x38Token(string token)
        {
            return HasFastenerIndexAnyFamily(token, new[] { "cap-screw-m3x38", "cap-screw-m3-38", "gb70-m3-38", "m3x38" }, 1, 16);
        }

        private static bool IsCountersunkM3x16Token(string token)
        {
            return ContainsAnyFastenerToken(token, "countersunk-m3x16", "countersunk-m3-16", "m3x16", "m3-16-chen");
        }

        private static bool IsSelfLockNutM25Token(string token)
        {
            return ContainsAnyFastenerToken(token, "self-lock-nut-m25", "self-lock-nut-m2-5", "zslm-m25", "nut-m25");
        }

        private static bool IsNylonStandoffM25x5Token(string token)
        {
            return ContainsAnyFastenerToken(token, "nylon-standoff-m25x5", "nylon-standoff-m2-5x5", "nilongzhu-m25-5", "standoff-m25x5");
        }

        private static bool IsCapScrewM25x12Token(string token)
        {
            return ContainsAnyFastenerToken(token, "cap-screw-m25x12", "cap-screw-m25-12", "cap-screw-m2-5x12", "gb70-m25-12", "m25x12");
        }

        private static bool ContainsAnyFastenerToken(string token, params string[] needles)
        {
            if (string.IsNullOrWhiteSpace(token) || needles == null)
            {
                return false;
            }

            for (int i = 0; i < needles.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(needles[i]) && token.Contains(needles[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasFastenerIndexAnyFamily(string token, string[] familyTokens, int minIndex, int maxIndex)
        {
            if (familyTokens == null)
            {
                return false;
            }

            for (int i = 0; i < familyTokens.Length; i++)
            {
                if (HasFastenerIndex(token, familyTokens[i], minIndex, maxIndex))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasFastenerIndexAnyFamily(string token, string[] familyTokens, params int[] indices)
        {
            if (familyTokens == null || indices == null)
            {
                return false;
            }

            for (int i = 0; i < familyTokens.Length; i++)
            {
                if (HasFastenerIndex(token, familyTokens[i], indices))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasFastenerIndex(string token, string familyToken, int minIndex, int maxIndex)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(familyToken) ||
                !token.Contains(familyToken))
            {
                return false;
            }

            for (int index = minIndex; index <= maxIndex; index++)
            {
                string suffix = "-" + index.ToString("000", System.Globalization.CultureInfo.InvariantCulture);
                if (token.Contains(familyToken + suffix) || token.EndsWith(suffix, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 ResolveArmAttachmentCarrierOffset(ExplodablePart armPart, Vector3 center)
        {
            if (TryResolveArmEndpointAxis(armPart, center, out Vector3 axis))
            {
                return axis * ArmAttachmentCarrierDistance;
            }

            if (armPart != null &&
                armPart.Data != null &&
                armPart.Data.explosionDirection.sqrMagnitude > 0.0001f)
            {
                Vector3 fallback = Vector3.ProjectOnPlane(armPart.Data.explosionDirection, Vector3.up);
                if (fallback.sqrMagnitude > 0.0001f)
                {
                    return fallback.normalized * ArmAttachmentCarrierDistance;
                }
            }

            return Vector3.zero;
        }

        private static Vector3 ResolveArmJibiCarrierOffset(ExplodablePart armPart, ExplodablePart fastenerPart, Vector3 center)
        {
            Vector3 axis = Vector3.zero;
            if (armPart != null && fastenerPart != null)
            {
                Vector3 armCenter = TryGetPartWorldBounds(armPart, out Bounds armBounds)
                    ? armBounds.center
                    : armPart.transform.position;
                Vector3 fastenerCenter = TryGetPartWorldBounds(fastenerPart, out Bounds fastenerBounds)
                    ? fastenerBounds.center
                    : fastenerPart.transform.position;
                axis = Vector3.ProjectOnPlane(fastenerCenter - armCenter, Vector3.up);
            }

            if (axis.sqrMagnitude < 0.0001f && TryResolveArmEndpointAxis(armPart, center, out Vector3 endpointAxis))
            {
                axis = endpointAxis;
            }

            if (axis.sqrMagnitude < 0.0001f &&
                armPart != null &&
                armPart.Data != null &&
                armPart.Data.explosionDirection.sqrMagnitude > 0.0001f)
            {
                axis = Vector3.ProjectOnPlane(armPart.Data.explosionDirection, Vector3.up);
            }

            if (axis.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            float lateralWeight = Mathf.Clamp01(1f - ArmJibiCarrierAxialWeight);
            float axialWeight = Mathf.Clamp01(ArmJibiCarrierAxialWeight);
            float blendMagnitude = Mathf.Sqrt(lateralWeight * lateralWeight + axialWeight * axialWeight);
            float planarDistance = blendMagnitude > 0.0001f
                ? ArmJibiCarrierDistance * (lateralWeight / blendMagnitude)
                : 0f;

            return axis.normalized * planarDistance;
        }

        private static bool HasFastenerIndex(string token, string familyToken, params int[] indices)
        {
            if (indices == null)
            {
                return false;
            }

            for (int i = 0; i < indices.Length; i++)
            {
                if (HasFastenerIndex(token, familyToken, indices[i], indices[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 BlendDirections(Vector3 radial, Vector3 axial, float axialWeight)
        {
            Vector3 resolvedRadial = radial.sqrMagnitude > 0.0001f ? radial.normalized : Vector3.zero;
            Vector3 resolvedAxial = axial.sqrMagnitude > 0.0001f ? axial.normalized : Vector3.up;
            Vector3 blended = resolvedRadial * Mathf.Clamp01(1f - axialWeight) + resolvedAxial * Mathf.Clamp01(axialWeight);
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : resolvedAxial;
        }

        private void RecalculatePriorityRange()
        {
            bool found = false;
            minExplosionPriority = 0;
            maxExplosionPriority = 0;

            foreach (ExplodablePart part in parts)
            {
                if (part == null || part.Data == null || part.Data.category == WebGL.Core.Data.PartCategory.Fasteners)
                {
                    continue;
                }

                int priority = part.Data.explosionPriority;
                if (!found)
                {
                    minExplosionPriority = priority;
                    maxExplosionPriority = priority;
                    found = true;
                    continue;
                }

                minExplosionPriority = Mathf.Min(minExplosionPriority, priority);
                maxExplosionPriority = Mathf.Max(maxExplosionPriority, priority);
            }
        }

        private float ResolvePartFactor(ExplodablePart part, float globalFactor)
        {
            float clampedFactor = Mathf.Clamp01(globalFactor);
            if (!staggerByPriority || part == null || part.Data == null || maxExplosionPriority <= minExplosionPriority)
            {
                return SmoothStep(clampedFactor);
            }

            if (part.Data.category == WebGL.Core.Data.PartCategory.Fasteners)
            {
                float fastenerFactor = Mathf.InverseLerp(0f, Mathf.Clamp(fastenerTravelWindow, 0.05f, 0.45f), clampedFactor);
                return SmoothStep(fastenerFactor);
            }

            float priority01 = Mathf.InverseLerp(minExplosionPriority, maxExplosionPriority, part.Data.explosionPriority);
            float startDelay = Mathf.Clamp01(mainPartsStartDelay);
            float travelWindow = Mathf.Clamp(priorityTravelWindow, 0.2f, 1f);
            float availableDelay = Mathf.Max(0f, 1f - startDelay - travelWindow);
            float start = startDelay + priority01 * availableDelay;
            float end = Mathf.Min(1f, start + travelWindow);
            float localFactor = Mathf.InverseLerp(start, end, clampedFactor);
            return SmoothStep(localFactor);
        }

        private static float SmoothStep(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
