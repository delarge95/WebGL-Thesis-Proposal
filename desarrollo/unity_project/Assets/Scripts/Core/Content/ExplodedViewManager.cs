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
        [SerializeField] private float minimumFinalBoundsSeparation = 0.12f;
        [SerializeField] private int finalSeparationIterations = 8;
        [SerializeField] private float finalSeparationStrength = 0.75f;

        private List<ExplodablePart> parts = new List<ExplodablePart>();
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
            Dictionary<string, ExplodablePart> partsById = BuildPartsById();
            foreach (var part in parts)
            {
                if (part != null)
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
            foreach (var part in parts)
            {
                if (part == null) continue;

                part.gameObject.SetActive(true);

                Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
                bool showAll = activeCategories == null || activeCategories.Count == 0 || activeCategories.Contains("ALL");
                bool anyVisible = false;

                foreach (Renderer renderer in renderers)
                {
                    if (renderer == null) continue;

                    bool visible = showAll || RendererMatchesFilters(renderer, part, activeCategories);
                    renderer.enabled = visible;
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

            Dictionary<string, ExplodablePart> partsById = BuildPartsById();
            Dictionary<ExplodablePart, Bounds> boundsByPart = BuildBoundsByPart();
            Dictionary<string, Vector3> primaryWorldOffsetsById = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
            motionParentByPart.Clear();

            foreach (ExplodablePart part in parts)
            {
                if (part == null || IsFastenerPart(part) || IsDependentPropulsionPart(part))
                {
                    continue;
                }

                Vector3 partCenter = TryGetPartWorldBounds(part, out Bounds bounds)
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

                Vector3 physicalOutward = ResolveMainPartDisassemblyDirection(part, outward);
                part.ConfigureRuntimeExplosionTarget(physicalOutward, explodeDistance);
                if (part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id))
                {
                    primaryWorldOffsetsById[part.Data.id] = physicalOutward.normalized * explodeDistance;
                }

                ConfigurePerRendererRadialOffsets(part, center, outward.normalized, explodeDistance);
            }

            EnforceMinimumFinalSeparation(partsById, boundsByPart, primaryWorldOffsetsById);
            foreach (KeyValuePair<string, Vector3> kvp in primaryWorldOffsetsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart adjustedPart) ||
                    adjustedPart == null ||
                    IsFastenerPart(adjustedPart) ||
                    IsDependentPropulsionPart(adjustedPart))
                {
                    continue;
                }

                Vector3 adjustedOffset = kvp.Value;
                if (adjustedOffset.sqrMagnitude > 0.0001f)
                {
                    adjustedPart.ConfigureRuntimeExplosionTarget(adjustedOffset.normalized, adjustedOffset.magnitude);
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
                        ResolveMainPartDisassemblyDirection(part, fallbackOutward),
                        fallbackDistance);
                    continue;
                }

                bool isProp = part.Data != null &&
                              part.Data.id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase);
                motionParentByPart[part] = armPart;
                Vector3 secondaryDirection = ResolveDependentPropulsionSecondaryDirection(part, armOffset, isProp);
                float secondaryDistance = isProp ? 1.72f : 1.34f;
                float start = isProp ? 0.00f : 0.18f;
                float end = isProp ? 0.24f : 0.50f;
                part.ConfigureRuntimeCompositeExplosionTarget(
                    armOffset,
                    secondaryDirection * secondaryDistance,
                    start,
                    end);
            }

            foreach (ExplodablePart part in parts)
            {
                if (part == null || !IsFastenerPart(part))
                {
                    continue;
                }

                Vector3 parentWorldOffset = Vector3.zero;
                if (TryResolveFastenerMotionParentPart(part, partsById, boundsByPart, out ExplodablePart parentPart) &&
                    parentPart != null &&
                    parentPart.Data != null &&
                    primaryWorldOffsetsById.TryGetValue(parentPart.Data.id, out Vector3 resolvedParentOffset))
                {
                    parentWorldOffset = resolvedParentOffset;
                    motionParentByPart[part] = parentPart;
                }

                string fastenerToken = BuildFastenerToken(part.transform);
                Vector3 fastenerDirection = IsRubberGrommetToken(fastenerToken) && parentWorldOffset.sqrMagnitude > 0.0001f
                    ? Vector3.ProjectOnPlane(parentWorldOffset, Vector3.up).normalized
                    : ResolveFastenerWorldDirection(part.transform, center);
                if (fastenerDirection.sqrMagnitude < 0.0001f)
                {
                    fastenerDirection = ResolveFastenerWorldDirection(part.transform, center);
                }
                float fastenerDistance = ResolveFastenerDistance(part.transform);
                part.ConfigureRuntimeCompositeExplosionTarget(
                    parentWorldOffset,
                    fastenerDirection.normalized * fastenerDistance,
                    0f,
                    fastenerTravelWindow);
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
                if (normalizedName.Contains("jia-guan") ||
                    normalizedName.Contains("hmx5v-guan-dingwei") ||
                    normalizedName.Contains("huan-guijiao") ||
                    normalizedName.Contains("rubber-grommet"))
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
            float authoredDistance = part != null && part.Data != null ? Mathf.Max(0f, part.Data.explosionDistance) : 0f;
            float distanceFromCenter = centerDistance * radialDistanceScale;
            float resolved = Mathf.Max(authoredDistance, distanceFromCenter, minRadialDistance);
            return Mathf.Min(resolved, maxRadialDistance);
        }

        private static Vector3 ResolveCenterFallbackDirection(ExplodablePart part)
        {
            if (part != null && part.Data != null)
            {
                string id = part.Data.id ?? string.Empty;
                if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase))
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

        private Dictionary<string, ExplodablePart> BuildPartsById()
        {
            Dictionary<string, ExplodablePart> lookup = new Dictionary<string, ExplodablePart>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < parts.Count; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null || part.Data == null || string.IsNullOrWhiteSpace(part.Data.id))
                {
                    continue;
                }

                lookup[part.Data.id] = part;
            }

            return lookup;
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

        private static float DistanceToBounds(Vector3 point, Bounds bounds)
        {
            Vector3 closest = bounds.ClosestPoint(point);
            return Vector3.Distance(point, closest);
        }

        private void EnforceMinimumFinalSeparation(
            IReadOnlyDictionary<string, ExplodablePart> partsById,
            IReadOnlyDictionary<ExplodablePart, Bounds> boundsByPart,
            Dictionary<string, Vector3> offsetsById)
        {
            if (partsById == null ||
                boundsByPart == null ||
                offsetsById == null ||
                minimumFinalBoundsSeparation <= 0f)
            {
                return;
            }

            List<ExplodablePart> layoutParts = new List<ExplodablePart>();
            foreach (KeyValuePair<string, Vector3> kvp in offsetsById)
            {
                if (!partsById.TryGetValue(kvp.Key, out ExplodablePart part) ||
                    part == null ||
                    part.Data == null ||
                    !boundsByPart.ContainsKey(part) ||
                    IsFastenerPart(part) ||
                    IsDependentPropulsionPart(part))
                {
                    continue;
                }

                layoutParts.Add(part);
            }

            int iterations = Mathf.Clamp(finalSeparationIterations, 0, 12);
            float padding = Mathf.Max(0f, minimumFinalBoundsSeparation);
            float strength = Mathf.Clamp01(finalSeparationStrength);
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                bool changed = false;
                for (int i = 0; i < layoutParts.Count; i++)
                {
                    ExplodablePart a = layoutParts[i];
                    if (a == null || a.Data == null)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < layoutParts.Count; j++)
                    {
                        ExplodablePart b = layoutParts[j];
                        if (b == null || b.Data == null)
                        {
                            continue;
                        }

                        if (!offsetsById.TryGetValue(a.Data.id, out Vector3 offsetA) ||
                            !offsetsById.TryGetValue(b.Data.id, out Vector3 offsetB) ||
                            !boundsByPart.TryGetValue(a, out Bounds boundsA) ||
                            !boundsByPart.TryGetValue(b, out Bounds boundsB))
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
                            offsetsById[a.Data.id] = offsetA + separationAxis * push;
                        }
                        if (!bLocked)
                        {
                            offsetsById[b.Data.id] = offsetB - separationAxis * push;
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

        private static bool IsExplosionLayoutLocked(ExplodablePart part)
        {
            return part != null &&
                   part.Data != null &&
                   string.Equals(part.Data.id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase);
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

        private Vector3 ResolveMainPartDisassemblyDirection(ExplodablePart part, Vector3 rawOutward)
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
                return radial.sqrMagnitude > 0.0001f ? radial : ResolveCenterFallbackDirection(part);
            }

            if (string.Equals(id, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_platform_board", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_power_module", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_gps_m10", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(id, "x500v2_battery", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("gps") ||
                normalized.Contains("battery"))
            {
                return BlendDirections(radial, Vector3.up, 0.82f);
            }

            if (string.Equals(id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase))
            {
                return BlendDirections(radial, Vector3.down, 0.86f);
            }

            if (string.Equals(id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase))
            {
                return BlendDirections(radial, Vector3.down, 0.72f);
            }

            if (string.Equals(id, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase))
            {
                return BlendDirections(radial, Vector3.down, 0.55f);
            }

            return radial.sqrMagnitude > 0.0001f ? radial : ResolveCenterFallbackDirection(part);
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

        private static string BuildFastenerToken(Transform fastener)
        {
            string normalized = SelectionHierarchy.NormalizeToken(fastener != null ? fastener.name : string.Empty);
            FastenerRuntimeMarker marker = fastener != null ? fastener.GetComponent<FastenerRuntimeMarker>() : null;
            string typeKey = marker != null ? SelectionHierarchy.NormalizeToken(marker.SceneTypeKey) : string.Empty;
            return normalized + "-" + typeKey;
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
