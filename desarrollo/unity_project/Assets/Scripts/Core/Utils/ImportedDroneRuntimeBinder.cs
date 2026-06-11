using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Events;
using WebGL.Core.Managers;
using WebGL.Core.Thermal;

namespace WebGL.Core.Utils
{
    [DisallowMultipleComponent]
    public class ImportedDroneRuntimeBinder : MonoBehaviour
    {
        private const string FastenerGroupId = "x500v2_fastener_group";
        private const string MiscGroupId = "x500v2_misc_group";

        [Header("Runtime Binding")]
        [SerializeField] private bool bindOnStart = true;
        [SerializeField] private bool rebuildHotspotsWhenReady = true;
        [SerializeField] private bool logBindingSummary;

        private Coroutine bindRoutine;

        private void Start()
        {
            if (!bindOnStart)
            {
                return;
            }

            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
            }

            bindRoutine = StartCoroutine(BindNextFrame());
        }

        [ContextMenu("Bind Imported Drone Runtime")]
        public void BindRuntimeNow()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
            }

            bindRoutine = StartCoroutine(BindNextFrame());
        }

        private IEnumerator BindNextFrame()
        {
            yield return null;
            BindRuntime();
            bindRoutine = null;
        }

        private void BindRuntime()
        {
            Transform droneRoot = ResolveDroneRoot();
            if (droneRoot == null)
            {
                Debug.LogWarning("[ImportedDroneRuntimeBinder] No se encontro el root del dron importado.");
                return;
            }

            FastenerRegistry registry = EnsureFastenerRuntimeSystems(droneRoot);
            RepairImportedDrone(droneRoot, registry);
            Transform[] propellers = FindPropellers(droneRoot);

            DroneStateController.Instance?.ConfigureRuntimeBindings(droneRoot, propellers);
            ExplodedViewManager.Instance?.RebuildCache();
            PartVisibilityManager.Instance?.RebuildCache();
            PartCatalogManager.Instance?.RefreshPartsList();
            CrossSectionManager.Instance?.RefreshTargetObject();

            ViewModeManager viewMode = ViewModeManager.Instance;
            if (viewMode != null)
            {
                viewMode.RebuildCache();
                viewMode.ReapplyCurrentMode(true);
            }

            ThermalSimulationManager.Instance?.RebuildRuntime();
            ThermalViewController.Instance?.RebuildBindings();
            int partCount = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None).Length;

            ValidateFastenerIdentityInvariants();

            if (rebuildHotspotsWhenReady)
            {
                EventBus.Publish(new ImportedDroneRuntimeBoundEvent(droneRoot.name, propellers.Length, partCount));
            }

            if (logBindingSummary)
            {
                Debug.Log($"[ImportedDroneRuntimeBinder] Bound root={droneRoot.name} propellers={propellers.Length} parts={partCount}");
            }
        }

        private FastenerRegistry EnsureFastenerRuntimeSystems(Transform droneRoot)
        {
            if (droneRoot == null)
            {
                return null;
            }

            FastenerRegistry registry = FindFirstObjectByType<FastenerRegistry>();
            if (registry == null)
            {
                registry = droneRoot.gameObject.AddComponent<FastenerRegistry>();
            }

            registry.RebuildRegistry();

            if (FindFirstObjectByType<FastenerInspectionManager>() == null)
            {
                droneRoot.gameObject.AddComponent<FastenerInspectionManager>();
            }

            return registry;
        }

        private void RepairImportedDrone(Transform droneRoot, FastenerRegistry registry)
        {
            Dictionary<string, ExplodablePart> anchorsById = BuildAnchorMap(droneRoot);
            RestoreMisclassifiedStructuralFasteners(droneRoot, anchorsById);
            MovePrimitiveFastenersToRootGroup(droneRoot);
            ReparentTopLevelOrphans(droneRoot, anchorsById);
            ReparentNestedOrphansByType(droneRoot, anchorsById, registry);

            List<ExplodablePart> anchors = CollectRuntimeAnchors(droneRoot);
            for (int i = 0; i < anchors.Count; i++)
            {
                ExplodablePart anchor = anchors[i];
                string anchorId = ResolveAnchorId(anchor);
                ConfigureAnchor(anchorId, anchor, registry);
            }

            SealLooseFastenerMetadata(droneRoot, registry);
        }

        private static List<ExplodablePart> CollectRuntimeAnchors(Transform droneRoot)
        {
            List<ExplodablePart> anchors = new List<ExplodablePart>();
            if (droneRoot == null)
            {
                return anchors;
            }

            foreach (ExplodablePart part in droneRoot.GetComponentsInChildren<ExplodablePart>(true))
            {
                if (part == null)
                {
                    continue;
                }

                string partId = ResolveAnchorId(part);
                if (string.IsNullOrWhiteSpace(partId) ||
                    partId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                anchors.Add(part);
            }

            return anchors;
        }

        private static string ResolveAnchorId(ExplodablePart anchor)
        {
            if (anchor == null)
            {
                return string.Empty;
            }

            return anchor.Data != null && !string.IsNullOrWhiteSpace(anchor.Data.id)
                ? anchor.Data.id
                : anchor.name;
        }

        private static Dictionary<string, ExplodablePart> BuildAnchorMap(Transform droneRoot)
        {
            Dictionary<string, ExplodablePart> anchors = new Dictionary<string, ExplodablePart>(StringComparer.OrdinalIgnoreCase);
            if (droneRoot == null)
            {
                return anchors;
            }

            foreach (ExplodablePart part in droneRoot.GetComponentsInChildren<ExplodablePart>(true))
            {
                if (part == null)
                {
                    continue;
                }

                string partId = part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id)
                    ? part.Data.id
                    : part.name;

                if (!string.IsNullOrWhiteSpace(partId) &&
                    !partId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase) &&
                    !anchors.ContainsKey(partId))
                {
                    anchors.Add(partId, part);
                }
            }

            return anchors;
        }

        private static void ReparentTopLevelOrphans(Transform droneRoot, IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (droneRoot == null || anchorsById == null || anchorsById.Count == 0)
            {
                return;
            }

            List<Transform> directChildren = new List<Transform>();
            foreach (Transform child in droneRoot)
            {
                directChildren.Add(child);
            }

            foreach (Transform child in directChildren)
            {
                if (child == null ||
                    child.GetComponent<ExplodablePart>() != null ||
                    string.Equals(child.name, FastenerGroupId, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(child.name, MiscGroupId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string expectedAnchorId = SelectionHierarchy.ResolveCanonicalPartId(child, droneRoot);
                if (!string.IsNullOrWhiteSpace(expectedAnchorId) &&
                    anchorsById.TryGetValue(expectedAnchorId, out ExplodablePart expectedAnchor) &&
                    expectedAnchor != null &&
                    !child.IsChildOf(expectedAnchor.transform))
                {
                    child.SetParent(expectedAnchor.transform, true);
                    continue;
                }

                Transform syntheticGroupAnchor = ResolveOrCreateSyntheticGroupAnchor(droneRoot, child.name);
                if (syntheticGroupAnchor != null && !child.IsChildOf(syntheticGroupAnchor))
                {
                    child.SetParent(syntheticGroupAnchor, true);
                    continue;
                }

                ExplodablePart prefixedAnchor = ResolveAnchorFromNamePrefix(child.name, anchorsById);
                if (prefixedAnchor != null && !child.IsChildOf(prefixedAnchor.transform))
                {
                    child.SetParent(prefixedAnchor.transform, true);
                    continue;
                }

                Renderer renderer = child.GetComponentInChildren<Renderer>(true);
                if (renderer == null)
                {
                    continue;
                }

                ExplodablePart bestAnchor = ResolveBestAnchor(child, renderer, anchorsById);
                if (bestAnchor == null || child.IsChildOf(bestAnchor.transform))
                {
                    continue;
                }

                child.SetParent(bestAnchor.transform, true);
            }
        }

        private static void RestoreMisclassifiedStructuralFasteners(
            Transform droneRoot,
            IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (droneRoot == null || anchorsById == null || anchorsById.Count == 0)
            {
                return;
            }

            Transform[] transforms = droneRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate == null || !IsMisclassifiedStructuralFastener(candidate.name))
                {
                    continue;
                }

                string parentId = ResolveStructuralNonFastenerParentCanonicalId(candidate, droneRoot);
                if (string.IsNullOrWhiteSpace(parentId) ||
                    !anchorsById.TryGetValue(parentId, out ExplodablePart parentPart) ||
                    parentPart == null)
                {
                    Debug.LogWarning($"[ImportedDroneRuntimeBinder] Falso fastener estructural sin padre canonico confiable: {candidate.name}");
                    continue;
                }

                candidate.SetParent(parentPart.transform, true);
                candidate.name = FastenerNamingUtility.ExtractSceneTypeKey(candidate.name);

                FastenerRuntimeMarker marker = candidate.GetComponent<FastenerRuntimeMarker>();
                if (marker != null)
                {
                    DestroyComponent(marker);
                }

                PartRenderCategory category = candidate.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = candidate.gameObject.AddComponent<PartRenderCategory>();
                }

                string primaryCategory = parentPart.Data != null ? parentPart.Data.category.ToString() : "SkeletonAirframe";
                string subpieceId = SelectionHierarchy.ResolveSubpieceId(candidate, parentId);
                if (string.IsNullOrWhiteSpace(subpieceId))
                {
                    subpieceId = ResolveStructuralNonFastenerSubpieceId(candidate.name);
                }

                category.Configure(parentId, primaryCategory, string.Empty, parentId, subpieceId);
            }
        }

        private static void MovePrimitiveFastenersToRootGroup(Transform droneRoot)
        {
            if (droneRoot == null)
            {
                return;
            }

            Transform rootGroup = EnsureDirectChildGroup(droneRoot, FastenerGroupId);
            Transform[] transforms = droneRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate == null ||
                    candidate == droneRoot ||
                    candidate == rootGroup ||
                    !candidate.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (IsMisclassifiedStructuralFastener(candidate.name))
                {
                    continue;
                }

                if (candidate.parent != rootGroup)
                {
                    candidate.SetParent(rootGroup, true);
                }
            }
        }

        private static Transform EnsureDirectChildGroup(Transform droneRoot, string groupId)
        {
            if (droneRoot == null || string.IsNullOrWhiteSpace(groupId))
            {
                return null;
            }

            foreach (Transform child in droneRoot)
            {
                if (child != null && string.Equals(child.name, groupId, StringComparison.OrdinalIgnoreCase))
                {
                    child.gameObject.SetActive(true);
                    return child;
                }
            }

            GameObject group = new GameObject(groupId);
            Transform groupTransform = group.transform;
            groupTransform.SetParent(droneRoot, false);
            groupTransform.localPosition = Vector3.zero;
            groupTransform.localRotation = Quaternion.identity;
            groupTransform.localScale = Vector3.one;
            return groupTransform;
        }

        private static void SealLooseFastenerMetadata(Transform droneRoot, FastenerRegistry registry)
        {
            if (droneRoot == null || registry == null)
            {
                return;
            }

            Transform rootGroup = EnsureDirectChildGroup(droneRoot, FastenerGroupId);
            if (rootGroup == null)
            {
                return;
            }

            Transform[] transforms = rootGroup.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate == null ||
                    candidate == rootGroup ||
                    !candidate.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                FastenerMetadata metadata = registry.ResolveMetadata(candidate);
                if (metadata == null || string.IsNullOrWhiteSpace(metadata.instanceId))
                {
                    continue;
                }

                registry.SealMarker(candidate, metadata);

                PartRenderCategory category = candidate.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = candidate.gameObject.AddComponent<PartRenderCategory>();
                }

                string parentCanonicalId = string.IsNullOrWhiteSpace(metadata.parentCanonicalPartId)
                    ? FastenerGroupId
                    : metadata.parentCanonicalPartId;
                category.Configure(parentCanonicalId, "Fasteners", "Fasteners", parentCanonicalId);
            }
        }

        private static bool IsMisclassifiedStructuralFastener(string rawName)
        {
            return SelectionHierarchy.IsKnownStructuralNonFastenerName(rawName);
        }

        private static string ResolveStructuralNonFastenerParentCanonicalId(Transform candidate, Transform droneRoot)
        {
            if (candidate == null)
            {
                return string.Empty;
            }

            string resolved = SelectionHierarchy.ResolveCanonicalPartId(candidate, droneRoot);
            if (!string.IsNullOrWhiteSpace(resolved))
            {
                return resolved;
            }

            string normalized = SelectionHierarchy.NormalizeToken(candidate.name);
            if (normalized.Contains("gpsv5-zhijia-luomao"))
            {
                return "x500v2_gps_m10";
            }

            if (normalized.Contains("hmx5v-guan-dingwei"))
            {
                return ResolveQuadrantArmParentCanonicalId(candidate, droneRoot);
            }

            return string.Empty;
        }

        private static string ResolveStructuralNonFastenerSubpieceId(string rawName)
        {
            string normalized = SelectionHierarchy.NormalizeToken(rawName);
            if (normalized.Contains("hmx5v-guan-dingwei")) return "hmx5v-guan-dingwei";
            if (normalized.Contains("carbon-fiber-tube300")) return "carbon-fiber-tube300";
            if (normalized.Contains("jia-guan")) return "jia-guan";
            if (normalized.Contains("huan-guijiao") || normalized.Contains("rubber-grommet")) return "huan-guijiao";
            if (normalized.Contains("gpsv5-zhijia-luomao")) return "gpsv5-zhijia-luomao";
            return string.Empty;
        }

        private static string ResolveQuadrantArmParentCanonicalId(Transform candidate, Transform droneRoot)
        {
            if (candidate == null)
            {
                return string.Empty;
            }

            string explicitSuffix = ResolveStructuralInstanceSuffix(candidate.name);
            if (!string.IsNullOrWhiteSpace(explicitSuffix))
            {
                return "x500v2_arm_" + explicitSuffix;
            }

            Renderer renderer = candidate.GetComponentInChildren<Renderer>(true);
            Vector3 position = renderer != null ? renderer.bounds.center : candidate.position;
            string worldSuffix = ResolveQuadrantSuffixFromWorld(position, droneRoot);
            return string.IsNullOrWhiteSpace(worldSuffix) ? string.Empty : "x500v2_arm_" + worldSuffix;
        }

        private static string ResolveStructuralInstanceSuffix(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            return SelectionHierarchy.ResolveKnownArmInstanceQuadrantSuffix(rawName);
        }

        private static string ResolveQuadrantSuffixFromWorld(Vector3 worldPosition, Transform root)
        {
            Vector3 front = Vector3.forward;
            Vector3 right = Vector3.right;
            float dominantSize = 1f;
            Vector3 center;
            if (root != null && TryResolveDroneReferenceFrame(root, out Vector3 frameCenter, out front, out right, out dominantSize))
            {
                center = frameCenter;
            }
            else
            {
                center = root != null && TryComputeWorldCenter(root, out Vector3 rootCenter) ? rootCenter : Vector3.zero;
            }

            Vector3 offset = Vector3.ProjectOnPlane(worldPosition - center, Vector3.up);
            if (offset.sqrMagnitude < 0.0000001f)
            {
                return string.Empty;
            }

            if (front.sqrMagnitude < 0.0001f || right.sqrMagnitude < 0.0001f)
            {
                front = Vector3.forward;
                right = Vector3.right;
                dominantSize = Mathf.Max(dominantSize, 1f);
            }

            float frontDot = Vector3.Dot(offset, front.normalized);
            float rightDot = Vector3.Dot(offset, right.normalized);
            float deadband = Mathf.Max(0.0005f, dominantSize * 0.01f);
            if (Mathf.Abs(frontDot) < deadband && Mathf.Abs(rightDot) < deadband)
            {
                return string.Empty;
            }

            if (Mathf.Abs(rightDot) < deadband)
            {
                rightDot = -deadband;
            }

            return (frontDot >= 0f ? "F" : "B") + (rightDot < 0f ? "L" : "R");
        }

        private static bool TryComputeWorldCenter(Transform root, out Vector3 center)
        {
            center = Vector3.zero;
            if (root == null)
            {
                return false;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            Bounds bounds = default;
            bool hasBounds = false;
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == root)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!hasBounds)
            {
                return false;
            }

            center = bounds.center;
            return true;
        }

        private static bool TryResolveDroneReferenceFrame(
            Transform root,
            out Vector3 center,
            out Vector3 front,
            out Vector3 right,
            out float dominantSize)
        {
            center = Vector3.zero;
            front = Vector3.forward;
            right = Vector3.right;
            dominantSize = 1f;

            if (root == null)
            {
                return false;
            }

            bool hasCenter = TryGetNamedRendererBounds(root, "bottom-plate-x500-v5", out Bounds bottomBounds);
            bool hasAggregate = TryGetRendererBounds(root, out Bounds aggregateBounds);
            if (hasCenter)
            {
                center = bottomBounds.center;
                dominantSize = Mathf.Max(bottomBounds.size.x, bottomBounds.size.z, 1f);
            }
            else if (hasAggregate)
            {
                center = aggregateBounds.center;
                dominantSize = Mathf.Max(aggregateBounds.size.x, aggregateBounds.size.z, 1f);
            }
            else
            {
                return false;
            }

            if (TryGetNamedRendererBounds(root, "zhijia-camera-intel", out Bounds cameraBounds))
            {
                Vector3 cameraFront = Vector3.ProjectOnPlane(cameraBounds.center - center, Vector3.up);
                if (cameraFront.sqrMagnitude > 0.000001f)
                {
                    front = cameraFront.normalized;
                    right = Vector3.Cross(Vector3.up, front).normalized;
                    return right.sqrMagnitude > 0.0001f;
                }
            }

            return hasCenter || hasAggregate;
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

        private static void DestroyComponent(Component component)
        {
            if (component == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(component);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }

        private static void ReparentNestedOrphansByType(
            Transform droneRoot,
            IReadOnlyDictionary<string, ExplodablePart> anchorsById,
            FastenerRegistry registry)
        {
            if (droneRoot == null || anchorsById == null || anchorsById.Count == 0)
            {
                return;
            }

            foreach (Renderer renderer in droneRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                {
                    continue;
                }

                Transform member = renderer.transform;
                if (member == null || member.GetComponent<ExplodablePart>() != null)
                {
                    continue;
                }

                ExplodablePart currentAnchor = member.GetComponentInParent<ExplodablePart>();
                string currentAnchorId = currentAnchor != null && currentAnchor.Data != null
                    ? currentAnchor.Data.id
                    : (currentAnchor != null ? currentAnchor.name : string.Empty);

                string expectedAnchorId = SelectionHierarchy.ResolveCanonicalPartId(member, droneRoot, currentAnchorId);
                if (string.IsNullOrWhiteSpace(expectedAnchorId))
                {
                    continue;
                }

                bool canCorrectArmQuadrant =
                    IsArmCanonicalAnchorId(currentAnchorId) &&
                    IsArmCanonicalAnchorId(expectedAnchorId);
                bool canCorrectLandingFamily =
                    string.Equals(expectedAnchorId, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase) &&
                    IsLandingGearMemberName(member.name);

                if (!canCorrectArmQuadrant &&
                    !canCorrectLandingFamily &&
                    IsStableCanonicalAnchorId(currentAnchorId) &&
                    !string.Equals(currentAnchorId, expectedAnchorId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!anchorsById.TryGetValue(expectedAnchorId, out ExplodablePart expectedAnchor) || expectedAnchor == null)
                {
                    continue;
                }

                if (string.Equals(currentAnchorId, expectedAnchorId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                SealInheritedFastenerMarker(member, currentAnchor, registry);

                if (!member.IsChildOf(expectedAnchor.transform))
                {
                    member.SetParent(expectedAnchor.transform, true);
                }
            }
        }

        private static void SealInheritedFastenerMarker(Transform member, ExplodablePart currentAnchor, FastenerRegistry registry)
        {
            if (member == null || currentAnchor == null || registry == null)
            {
                return;
            }

            DronePartData anchorData = currentAnchor.Data;
            bool isFastenerAnchor = (anchorData != null && anchorData.category == PartCategory.Fasteners) ||
                                    currentAnchor.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase);
            if (!isFastenerAnchor)
            {
                return;
            }

            FastenerMetadata metadata = anchorData != null && anchorData.HasFastenerMetadata
                ? anchorData.fastenerMetadata
                : registry.ResolveMetadata(currentAnchor.transform, anchorData);
            if (metadata == null || string.IsNullOrWhiteSpace(metadata.instanceId))
            {
                return;
            }

            registry.SealMarker(member, metadata);
        }

        private static Transform ResolveOrCreateSyntheticGroupAnchor(Transform droneRoot, string candidateName)
        {
            if (droneRoot == null || string.IsNullOrWhiteSpace(candidateName))
            {
                return null;
            }

            string groupId = ResolveSyntheticGroupIdFromName(candidateName);
            if (string.IsNullOrWhiteSpace(groupId))
            {
                return null;
            }

            Transform existing = droneRoot.Find(groupId);
            if (existing != null)
            {
                return existing;
            }

            GameObject group = new GameObject(groupId);
            Transform groupTransform = group.transform;
            groupTransform.SetParent(droneRoot, true);
            groupTransform.localScale = Vector3.one;
            groupTransform.position = droneRoot.position;
            return groupTransform;
        }

        private static string ResolveSyntheticGroupIdFromName(string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                return string.Empty;
            }

            if (IsRecognizedFastenerName(candidateName))
            {
                return FastenerGroupId;
            }

            if (candidateName.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase))
            {
                return FastenerGroupId;
            }

            if (candidateName.IndexOf("x500v2_fastener.", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return FastenerGroupId;
            }

            if (candidateName.StartsWith("x500v2_misc.", StringComparison.OrdinalIgnoreCase))
            {
                return MiscGroupId;
            }

            if (candidateName.IndexOf("x500v2_misc.", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return MiscGroupId;
            }

            return string.Empty;
        }

        private static ExplodablePart ResolveAnchorFromNamePrefix(string candidateName, IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (string.IsNullOrWhiteSpace(candidateName) || anchorsById == null || anchorsById.Count == 0)
            {
                return null;
            }

            if (anchorsById.TryGetValue(candidateName, out ExplodablePart exact))
            {
                return exact;
            }

            int dot = candidateName.IndexOf('.');
            if (dot > 0)
            {
                string prefix = candidateName.Substring(0, dot);
                if (anchorsById.TryGetValue(prefix, out ExplodablePart byPrefix))
                {
                    return byPrefix;
                }
            }

            foreach (KeyValuePair<string, ExplodablePart> kvp in anchorsById)
            {
                if (candidateName.StartsWith(kvp.Key + ".", StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        private static ExplodablePart ResolveBestAnchor(Transform candidate, Renderer renderer, IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (candidate == null || anchorsById == null || anchorsById.Count == 0)
            {
                return null;
            }

            string lowerName = candidate.name.ToLowerInvariant();
            string suffix = ResolveQuadrantSuffix(lowerName);
            string expectedAnchorId = ResolveExpectedAnchorIdFromName(candidate.name, string.Empty);
            if (!string.IsNullOrWhiteSpace(expectedAnchorId) &&
                anchorsById.TryGetValue(expectedAnchorId, out ExplodablePart expectedAnchor) &&
                expectedAnchor != null)
            {
                return expectedAnchor;
            }

            Vector3 candidateCenter = renderer != null ? renderer.bounds.center : candidate.position;

            float bestScore = float.MaxValue;
            ExplodablePart bestAnchor = null;

            foreach (KeyValuePair<string, ExplodablePart> kvp in anchorsById)
            {
                ExplodablePart anchor = kvp.Value;
                if (anchor == null)
                {
                    continue;
                }

                float score = Vector3.Distance(candidateCenter, anchor.transform.position);
                string anchorId = kvp.Key.ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(suffix) && anchorId.EndsWith("_" + suffix, StringComparison.Ordinal))
                {
                    score -= 2f;
                }

                if (lowerName.Contains("prop") && anchorId.Contains("prop")) score -= 2.0f;
                if (lowerName.Contains("motor") && anchorId.Contains("motor")) score -= 1.8f;
                if (lowerName.Contains("esc") && anchorId.Contains("esc")) score -= 1.7f;
                if (lowerName.Contains("arm") && anchorId.Contains("arm")) score -= 1.4f;
                if (lowerName.Contains("battery") && anchorId.Contains("battery")) score -= 1.4f;
                if (lowerName.Contains("plate") && anchorId.Contains("plate")) score -= 1.0f;
                if (lowerName.Contains("landing") && anchorId.Contains("landing")) score -= 1.0f;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestAnchor = anchor;
                }
            }

            return bestAnchor;
        }

        private static void ConfigureAnchor(string anchorId, ExplodablePart anchor, FastenerRegistry registry)
        {
            if (anchor == null)
            {
                return;
            }

            CalibrateExplosionPreset(anchorId, anchor);
            EnsureSelectableLayer(anchor.transform);
            bool anchorIsFastener = IsFastenerAnchor(anchor);

            foreach (Renderer renderer in anchor.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                {
                    continue;
                }

                ExplodablePart rendererOwner = renderer.GetComponentInParent<ExplodablePart>();
                if (rendererOwner != null && rendererOwner != anchor &&
                    !ShouldConfigureNestedRendererForAnchor(anchor, rendererOwner, renderer.transform))
                {
                    continue;
                }

                EnsureSelectionCollider(renderer);

                string auxiliaryCategory = InferAuxiliaryCategory(renderer.transform.name);
                string thermalSourceId = InferThermalSourcePartId(renderer.transform.name, anchorId);
                string primaryCategory = InferDisplayCategory(renderer.transform.name, anchor.Data != null ? anchor.Data.category.ToString() : string.Empty, thermalSourceId);

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = renderer.gameObject.AddComponent<PartRenderCategory>();
                }
                string subpieceId = SelectionHierarchy.ResolveSubpieceId(renderer.transform, anchorId);
                category.Configure(anchorId, primaryCategory, auxiliaryCategory, thermalSourceId, subpieceId);

                // Fasteners can be visually re-parented under their mother piece so the
                // clicked renderer is not always the same transform that owns the runtime
                // marker. Seal instance metadata on the visible renderer too, so selection
                // and isolation can resolve the exact fastener instance instead of
                // collapsing back to the associated fastener group.
                if (registry != null &&
                    string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) &&
                    SelectionHierarchy.IsPrimitiveFastenerSource(renderer.transform))
                {
                    FastenerMetadata visibleFastenerMetadata = registry.ResolveMetadata(renderer.transform);
                    if (visibleFastenerMetadata != null && !string.IsNullOrWhiteSpace(visibleFastenerMetadata.instanceId))
                    {
                        registry.SealMarker(renderer.transform, visibleFastenerMetadata);
                    }
                }

                if (!anchorIsFastener && renderer.transform != anchor.transform)
                {
                    ConfigureAuxiliaryExplode(renderer.transform, anchor, auxiliaryCategory);
                }
            }

            SealFastenerMetadata(anchor, registry);
            anchor.Initialize();
        }

        private static bool IsFastenerAnchor(ExplodablePart anchor)
        {
            if (anchor == null)
            {
                return false;
            }

            DronePartData data = anchor.Data;
            return (data != null && data.category == PartCategory.Fasteners) ||
                   anchor.GetComponent<FastenerRuntimeMarker>() != null ||
                   anchor.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ShouldConfigureNestedRendererForAnchor(ExplodablePart anchor, ExplodablePart rendererOwner, Transform rendererTransform)
        {
            if (!IsLandingGearAnchor(anchor) || rendererOwner == null || rendererTransform == null)
            {
                return false;
            }

            string ownerId = rendererOwner.Data != null ? rendererOwner.Data.id : rendererOwner.name;
            return string.Equals(ownerId, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase) ||
                   IsLandingGearMemberName(rendererTransform.name);
        }

        private static void SealFastenerMetadata(ExplodablePart anchor, FastenerRegistry registry)
        {
            if (anchor == null)
            {
                return;
            }

            DronePartData data = anchor.Data;
            if (!IsFastenerAnchor(anchor))
            {
                return;
            }

            FastenerMetadata metadata = data != null && data.HasFastenerMetadata
                ? data.fastenerMetadata
                : registry != null ? registry.ResolveMetadata(anchor.transform, data) : null;

            if (metadata == null)
            {
                metadata = new FastenerMetadata
                {
                    instanceId = FastenerNamingUtility.SanitizeId(anchor.name),
                    sceneObjectName = anchor.name,
                    sceneTypeKey = FastenerNamingUtility.ExtractSceneTypeKey(anchor.name),
                    isInspectable = false,
                    fallbackReason = "No fastener catalog metadata is available for this scene object yet."
                };
            }

            if (data != null && !data.HasFastenerMetadata)
            {
                data.fastenerMetadata = metadata;
            }

            if (registry != null)
            {
                registry.SealMarker(anchor.transform, metadata);
                return;
            }

            FastenerRuntimeMarker marker = anchor.GetComponent<FastenerRuntimeMarker>();
            if (marker == null)
            {
                marker = anchor.gameObject.AddComponent<FastenerRuntimeMarker>();
            }

            marker.Configure(
                metadata.familyId,
                metadata.instanceId,
                metadata.sceneTypeKey,
                metadata.parentCanonicalPartId,
                metadata.isInspectable,
                metadata.fallbackReason,
                SelectionHierarchy.IsPrimitiveFastenerSource(anchor.transform));
        }

        private static void EnsureSelectableLayer(Transform root)
        {
            int selectableLayer = LayerMask.NameToLayer("SelectablePart");
            if (root == null || selectableLayer < 0)
            {
                return;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = selectableLayer;
            }
        }

        private static void EnsureSelectionCollider(Renderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                foreach (BoxCollider staleBox in renderer.GetComponents<BoxCollider>())
                {
                    DestroyComponent(staleBox);
                }

                MeshCollider meshCollider = renderer.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    meshCollider = renderer.gameObject.AddComponent<MeshCollider>();
                }

                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                return;
            }

            if (renderer.GetComponent<Collider>() != null)
            {
                return;
            }

            BoxCollider collider = renderer.gameObject.AddComponent<BoxCollider>();
            Bounds localBounds = TransformWorldBoundsToLocal(renderer.bounds, renderer.transform);
            collider.center = localBounds.center;
            collider.size = localBounds.size;
        }

        private static Bounds TransformWorldBoundsToLocal(Bounds worldBounds, Transform target)
        {
            if (target == null)
            {
                return worldBounds;
            }

            Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            Vector3 center = worldBounds.center;
            Vector3 extents = worldBounds.extents;

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 worldCorner = center + Vector3.Scale(extents, new Vector3(x, y, z));
                        Vector3 localCorner = target.InverseTransformPoint(worldCorner);
                        min = Vector3.Min(min, localCorner);
                        max = Vector3.Max(max, localCorner);
                    }
                }
            }

            Bounds localBounds = new Bounds((min + max) * 0.5f, max - min);
            Vector3 size = localBounds.size;
            size.x = Mathf.Max(size.x, 0.0001f);
            size.y = Mathf.Max(size.y, 0.0001f);
            size.z = Mathf.Max(size.z, 0.0001f);
            localBounds.size = size;
            return localBounds;
        }

        private static void ConfigureAuxiliaryExplode(Transform member, ExplodablePart anchor, string auxiliaryCategory)
        {
            if (member == null || anchor == null)
            {
                return;
            }

            if (TryResolveCompositeAuxiliaryProfile(
                    member,
                    anchor,
                    auxiliaryCategory,
                    out Vector3 primaryDirection,
                    out float primaryDistance,
                    out float primaryStart,
                    out float primaryEnd,
                    out Vector3 secondaryDirection,
                    out float secondaryDistance,
                    out float secondaryStart,
                    out float secondaryEnd,
                    out bool compositeUsesGlobalTiming))
            {
                AuxiliaryExplodeOffset compositeOffset = member.GetComponent<AuxiliaryExplodeOffset>();
                if (compositeOffset == null)
                {
                    compositeOffset = member.gameObject.AddComponent<AuxiliaryExplodeOffset>();
                }

                compositeOffset.ConfigureSequencedComposite(
                    primaryDirection,
                    primaryDistance,
                    primaryStart,
                    primaryEnd,
                    secondaryDirection,
                    secondaryDistance,
                    secondaryStart,
                    secondaryEnd,
                    compositeUsesGlobalTiming);
                return;
            }

            if (TryResolvePhysicalAuxiliaryProfile(
                    member,
                    anchor,
                    auxiliaryCategory,
                    out Vector3 physicalLocalDirection,
                    out float physicalDistance,
                    out float sequenceStart,
                    out float sequenceEnd,
                    out bool useGlobalTiming))
            {
                AuxiliaryExplodeOffset physicalOffset = member.GetComponent<AuxiliaryExplodeOffset>();
                if (physicalOffset == null)
                {
                    physicalOffset = member.gameObject.AddComponent<AuxiliaryExplodeOffset>();
                }

                physicalOffset.ConfigureSequenced(
                    physicalLocalDirection,
                    physicalDistance,
                    sequenceStart,
                    sequenceEnd,
                    useGlobalTiming);
                return;
            }

            if (string.IsNullOrWhiteSpace(auxiliaryCategory))
            {
                AuxiliaryExplodeOffset existing = member.GetComponent<AuxiliaryExplodeOffset>();
                if (existing != null)
                {
                    existing.Initialize();
                }
                return;
            }

            Vector3 localDirection = member.localPosition.sqrMagnitude > 0.0001f
                ? member.localPosition.normalized
                : ResolveExplosionDirection(anchor).normalized;

            float distance = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ? 0.06f : 0.03f;
            float lead = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ? 1.35f : 1.12f;

            AuxiliaryExplodeOffset offset = member.GetComponent<AuxiliaryExplodeOffset>();
            if (offset == null)
            {
                offset = member.gameObject.AddComponent<AuxiliaryExplodeOffset>();
            }

            offset.Configure(localDirection, distance, lead);
        }

        private static bool TryResolveCompositeAuxiliaryProfile(
            Transform member,
            ExplodablePart anchor,
            string auxiliaryCategory,
            out Vector3 primaryLocalDirection,
            out float primaryDistance,
            out float primaryStart,
            out float primaryEnd,
            out Vector3 secondaryLocalDirection,
            out float secondaryDistance,
            out float secondaryStart,
            out float secondaryEnd,
            out bool useGlobalTiming)
        {
            primaryLocalDirection = Vector3.down;
            primaryDistance = 0f;
            primaryStart = 0f;
            primaryEnd = 1f;
            secondaryLocalDirection = Vector3.zero;
            secondaryDistance = 0f;
            secondaryStart = 0f;
            secondaryEnd = 1f;
            useGlobalTiming = true;

            if (member == null || anchor == null)
            {
                return false;
            }

            string normalized = SelectionHierarchy.NormalizeToken(member.name);
            string token = BuildFastenerProfileToken(member);
            bool isFastener = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ||
                              SelectionHierarchy.IsPrimitiveFastenerSource(member) ||
                              member.GetComponent<FastenerRuntimeMarker>() != null;

            Vector3 zuoCarrierWorldDirection = ResolveArmAxisWorldDirection(member, anchor);
            const float zuoCarrierDistance = 0.52f;
            const float zuoCarrierStart = 0.42f;
            const float zuoCarrierEnd = 1.00f;
            const float lowerCoverStart = 0.42f;

            if (IsArmAnchor(anchor) && normalized.Contains("hmx5v-zuo-dj-muju"))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.zero,
                    0f,
                    0f,
                    1f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsArmAnchor(anchor) && normalized.Contains("hmx5v-digai-dianjizuo-muju"))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.down,
                    0.46f,
                    0.30f,
                    0.62f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsArmAnchor(anchor) && IsPropellerSubpieceToken(normalized))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.up,
                    1.72f,
                    0.20f,
                    0.52f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsArmAnchor(anchor) && IsMotorSubpieceToken(normalized))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.up,
                    1.34f,
                    0.22f,
                    0.54f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsArmAnchor(anchor) && normalized.Contains("ban-dj-dian-f2"))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.up,
                    0.52f,
                    0.30f,
                    0.58f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            bool isInvertedArmM25x6Screw = IsInvertedArmM25x6Screw(member, token);

            if (isFastener && IsRailBatteryAnchor(anchor) && IsCountersunkM3x16Token(token))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.down,
                    0.48f,
                    0.00f,
                    0.22f,
                    ResolveFrameForwardWorldDirection(member, anchor),
                    0.46f,
                    0.44f,
                    0.78f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (isFastener && IsArmAnchor(anchor) && IsLongArmCapScrewM3x38Token(token))
            {
                ExplodablePart owningPart = member.GetComponentInParent<ExplodablePart>();
                if (owningPart != null && owningPart != anchor && IsFastenerAnchor(owningPart))
                {
                    ConfigureCompositeLocalDirections(
                        member,
                        Vector3.zero,
                        0f,
                        0f,
                        1f,
                        Vector3.zero,
                        0f,
                        0f,
                        1f,
                        out primaryLocalDirection,
                        out primaryDistance,
                        out primaryStart,
                        out primaryEnd,
                        out secondaryLocalDirection,
                        out secondaryDistance,
                        out secondaryStart,
                        out secondaryEnd);
                    return true;
                }

                const float jibiCarrierDistance = 0.46f;
                const float jibiCarrierAxialWeight = 0.76f;
                float jibiLateralWeight = Mathf.Clamp01(1f - jibiCarrierAxialWeight);
                float jibiBlendMagnitude = Mathf.Sqrt(
                    jibiLateralWeight * jibiLateralWeight +
                    jibiCarrierAxialWeight * jibiCarrierAxialWeight);
                float jibiPlanarDistance = jibiBlendMagnitude > 0.0001f
                    ? jibiCarrierDistance * (jibiLateralWeight / jibiBlendMagnitude)
                    : 0f;

                ConfigureCompositeLocalDirections(
                    member,
                    ResolveArmAxisWorldDirection(member, anchor),
                    jibiPlanarDistance,
                    0.62f,
                    0.92f,
                    Vector3.up,
                    1.08f,
                    0.00f,
                    0.24f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (isFastener && IsArmAnchor(anchor) && IsMotorCapScrewM3x6Token(token))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    zuoCarrierWorldDirection,
                    zuoCarrierDistance,
                    zuoCarrierStart,
                    zuoCarrierEnd,
                    Vector3.up,
                    0.68f,
                    0.24f,
                    0.52f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (isFastener && IsArmAnchor(anchor))
            {
                Vector3 fastenerWorldDirection;
                float fastenerDistance;
                float fastenerStart;
                float fastenerEnd;

                if (isInvertedArmM25x6Screw)
                {
                    fastenerWorldDirection = Vector3.down;
                    fastenerDistance = 0.24f;
                    fastenerStart = 0.00f;
                    fastenerEnd = 0.22f;
                }
                else if (IsArmFlangeNutM3Token(token))
                {
                    fastenerWorldDirection = Vector3.down;
                    fastenerDistance = 0.12f;
                    fastenerStart = 0.00f;
                    fastenerEnd = 0.22f;
                }
                else
                {
                    fastenerWorldDirection = ResolveFastenerAuxiliaryWorldDirection(member, anchor);
                    fastenerDistance = ResolveFastenerAuxiliaryDistance(member);
                    fastenerStart = 0.00f;
                    fastenerEnd = 0.22f;
                }

                ConfigureCompositeLocalDirections(
                    member,
                    isInvertedArmM25x6Screw
                        ? (zuoCarrierWorldDirection * zuoCarrierDistance + Vector3.down * 0.52f).normalized
                        : zuoCarrierWorldDirection,
                    isInvertedArmM25x6Screw
                        ? (zuoCarrierWorldDirection * zuoCarrierDistance + Vector3.down * 0.52f).magnitude
                        : zuoCarrierDistance,
                    isInvertedArmM25x6Screw ? lowerCoverStart : zuoCarrierStart,
                    zuoCarrierEnd,
                    fastenerWorldDirection,
                    fastenerDistance,
                    fastenerStart,
                    fastenerEnd,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsRailBatteryAnchor(anchor) &&
                TryResolveExplicitRailZAxisDirection(member, anchor, normalized, out Vector3 railZDirection, out bool positiveZ))
            {
                bool dropsFirst = normalized.Contains("gai-guangliu") ||
                                  normalized.Contains("platform-plat-x500");
                bool isPlatform = normalized.Contains("platform-plat-x500");
                ResolveExplicitRailZSequence(normalized, positiveZ, out float railZStart, out float railZEnd);

                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.down,
                    dropsFirst ? isPlatform ? 0.24f : 0.22f : 0f,
                    isPlatform ? 0.48f : 0.28f,
                    isPlatform ? 0.64f : 0.48f,
                    railZDirection,
                    ResolveExplicitRailZDistance(normalized),
                    railZStart,
                    railZEnd,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsLandingGearAnchor(anchor) &&
                (normalized.Contains("mao-jiao") || normalized.Contains("jiao-eva")))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.down,
                    1.22f,
                    0.46f,
                    0.84f,
                    ResolveModelZSideWorldDirection(member, anchor),
                    1.45f,
                    0.62f,
                    0.96f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsRailBatteryAnchor(anchor) && IsRailForwardCarrierSubpiece(normalized))
            {
                bool dropsFirst = normalized.Contains("gai-guangliu");
                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.down,
                    dropsFirst ? 0.22f : 0f,
                    0.28f,
                    0.48f,
                    ResolveFrameForwardWorldDirection(member, anchor),
                    0.46f,
                    0.46f,
                    0.80f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsRailBatteryAnchor(anchor) && normalized.Contains("platform-plat-x500"))
            {
                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.down,
                    0.24f,
                    0.48f,
                    0.64f,
                    -ResolveFrameForwardWorldDirection(member, anchor),
                    0.46f,
                    0.64f,
                    0.90f,
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            if (IsRailBatteryAnchor(anchor) && IsRailTubeCompanionSubpiece(normalized))
            {
                Vector3 frontBackDirection = ResolveFrameFrontBackWorldDirection(member, anchor);
                Vector3 forwardDirection = ResolveFrameForwardWorldDirection(member, anchor);
                float sequenceStart = Vector3.Dot(frontBackDirection, forwardDirection) >= 0f ? 0.52f : 0.66f;
                ConfigureCompositeLocalDirections(
                    member,
                    Vector3.zero,
                    0f,
                    0f,
                    1f,
                    frontBackDirection,
                    0.24f,
                    sequenceStart,
                    Mathf.Min(1f, sequenceStart + 0.24f),
                    out primaryLocalDirection,
                    out primaryDistance,
                    out primaryStart,
                    out primaryEnd,
                    out secondaryLocalDirection,
                    out secondaryDistance,
                    out secondaryStart,
                    out secondaryEnd);
                return true;
            }

            return false;
        }

        private static void ConfigureCompositeLocalDirections(
            Transform member,
            Vector3 primaryWorldDirection,
            float resolvedPrimaryDistance,
            float resolvedPrimaryStart,
            float resolvedPrimaryEnd,
            Vector3 secondaryWorldDirection,
            float resolvedSecondaryDistance,
            float resolvedSecondaryStart,
            float resolvedSecondaryEnd,
            out Vector3 primaryLocalDirection,
            out float primaryDistance,
            out float primaryStart,
            out float primaryEnd,
            out Vector3 secondaryLocalDirection,
            out float secondaryDistance,
            out float secondaryStart,
            out float secondaryEnd)
        {
            primaryLocalDirection = member != null && member.parent != null
                ? member.parent.InverseTransformVector(primaryWorldDirection)
                : primaryWorldDirection;
            secondaryLocalDirection = member != null && member.parent != null
                ? member.parent.InverseTransformVector(secondaryWorldDirection)
                : secondaryWorldDirection;

            primaryDistance = resolvedPrimaryDistance;
            primaryStart = resolvedPrimaryStart;
            primaryEnd = resolvedPrimaryEnd;
            secondaryDistance = resolvedSecondaryDistance;
            secondaryStart = resolvedSecondaryStart;
            secondaryEnd = resolvedSecondaryEnd;
        }

        private static bool TryResolvePhysicalAuxiliaryProfile(
            Transform member,
            ExplodablePart anchor,
            string auxiliaryCategory,
            out Vector3 localDirection,
            out float distance,
            out float sequenceStart,
            out float sequenceEnd,
            out bool useGlobalTiming)
        {
            localDirection = Vector3.up;
            distance = 0f;
            sequenceStart = 0f;
            sequenceEnd = 1f;
            useGlobalTiming = true;

            if (member == null || anchor == null)
            {
                return false;
            }

            string normalized = SelectionHierarchy.NormalizeToken(member.name);
            bool isFastener = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ||
                              SelectionHierarchy.IsPrimitiveFastenerSource(member) ||
                              member.GetComponent<FastenerRuntimeMarker>() != null;

            Vector3 worldDirection;
            if (isFastener)
            {
                string token = BuildFastenerProfileToken(member);
                if (IsRailBatteryAnchor(anchor) && IsCountersunkM3x16Token(token))
                {
                    worldDirection = Vector3.down;
                    distance = 0.48f;
                    sequenceStart = 0.00f;
                    sequenceEnd = 0.22f;
                }
                else if (IsRailBatteryAnchor(anchor) &&
                         (IsSelfLockNutM25Token(token) || IsNylonStandoffM25x5Token(token)))
                {
                    worldDirection = Vector3.down;
                    distance = IsNylonStandoffM25x5Token(token) ? 0.34f : 0.28f;
                    sequenceStart = 0.00f;
                    sequenceEnd = 0.26f;
                }
                else if (IsRailBatteryAnchor(anchor) && IsCapScrewM25x12Token(token))
                {
                    worldDirection = Vector3.up;
                    distance = 0.58f;
                    sequenceStart = 0.00f;
                    sequenceEnd = 0.24f;
                }
                else if (IsLongArmCapScrewM3x38Token(token))
                {
                    Vector3 center = ResolveWorldCenter(member);
                    Vector3 anchorCenter = ResolveWorldCenter(anchor.transform);
                    Vector3 vertical = center.y > anchorCenter.y + 0.002f ? Vector3.up : Vector3.down;
                    worldDirection = vertical;
                    distance = 0.6f;
                    sequenceStart = 0.62f;
                    sequenceEnd = 0.92f;
                }
                else if (IsMotorCapScrewM3x6Token(token))
                {
                    worldDirection = ResolveSubpieceWorldDirection(member, anchor, Vector3.up, 0.78f);
                    distance = 0.68f;
                    sequenceStart = 0.24f;
                    sequenceEnd = 0.52f;
                }
                else if (IsArmFlangeNutM3Token(token))
                {
                    worldDirection = Vector3.down;
                    distance = 0.12f;
                    sequenceStart = 0.00f;
                    sequenceEnd = 0.22f;
                }
                else
                {
                    worldDirection = ResolveFastenerAuxiliaryWorldDirection(member, anchor);
                    distance = ResolveFastenerAuxiliaryDistance(member);
                    sequenceStart = 0.00f;
                    sequenceEnd = 0.22f;
                }
            }
            else if (IsArmAnchor(anchor) && IsPropellerSubpieceToken(normalized))
            {
                worldDirection = Vector3.up;
                distance = 2.00f;
                sequenceStart = 0.00f;
                sequenceEnd = 0.34f;
            }
            else if (IsArmAnchor(anchor) && IsMotorSubpieceToken(normalized))
            {
                worldDirection = Vector3.up;
                distance = 1.10f;
                sequenceStart = 0.18f;
                sequenceEnd = 0.52f;
            }
            else if (normalized.Contains("ban-dj-dian-f2"))
            {
                worldDirection = ResolveSubpieceWorldDirection(member, anchor, Vector3.up, 0.78f);
                distance = 0.52f;
                sequenceStart = 0.30f;
                sequenceEnd = 0.58f;
            }
            else if (normalized.Contains("hmx5v-zuo-dj-muju") ||
                     normalized.Contains("hmx5v-digai-dianjizuo-muju"))
            {
                return false;
            }
            else if (IsElectronicsStackAnchor(anchor) && !isFastener)
            {
                worldDirection = ResolveElectronicsStackWorldDirection(member, anchor);
                distance = ResolveElectronicsStackDistance(member, anchor, normalized);
                sequenceStart = ResolveElectronicsStackSequenceStart(member, anchor, normalized);
                sequenceEnd = Mathf.Min(1f, sequenceStart + 0.28f);
            }
            else if (IsRailBatteryAnchor(anchor) &&
                     TryResolveExplicitRailZAxisDirection(member, anchor, normalized, out Vector3 explicitRailZDirection, out bool explicitRailPositiveZ))
            {
                worldDirection = explicitRailZDirection;
                distance = ResolveExplicitRailZDistance(normalized);
                ResolveExplicitRailZSequence(normalized, explicitRailPositiveZ, out sequenceStart, out sequenceEnd);
            }
            else if (IsRailBatteryAnchor(anchor) && IsRailFrontBackSubpiece(normalized))
            {
                Vector3 frontBack = ResolveFrameFrontBackWorldDirection(member, anchor);
                Vector3 vertical = normalized.Contains("battery") || normalized.Contains("lipo")
                    ? Vector3.up
                    : Vector3.zero;
                worldDirection = BlendDirections(frontBack, vertical, normalized.Contains("battery") || normalized.Contains("lipo") ? 0.18f : 0f);
                distance = normalized.Contains("battery-mounting-plat") || normalized.Contains("battery") || normalized.Contains("lipo")
                    ? 0.42f
                    : normalized.Contains("jia-guan") || normalized.Contains("huan-guijiao") || normalized.Contains("rubber-grommet")
                        ? 0.24f
                        : 0.46f;
                sequenceStart = 0.44f;
                sequenceEnd = 0.78f;
            }
            else if (IsRailBatteryAnchor(anchor) && normalized.Contains("battery-pad"))
            {
                worldDirection = Vector3.down;
                distance = 0.22f;
                sequenceStart = 0.62f;
                sequenceEnd = 0.86f;
            }
            else if (IsRailBatteryAnchor(anchor) && normalized.Contains("carbon-fiber-tube300"))
            {
                worldDirection = ResolveFrameLeftRightWorldDirection(member, anchor);
                distance = 0.44f;
                sequenceStart = 0.76f;
                sequenceEnd = 1.00f;
            }
            else if (IsRailBatteryAnchor(anchor) && normalized.Contains("pylons-x500"))
            {
                worldDirection = Vector3.down;
                distance = 0.18f;
                sequenceStart = 0.82f;
                sequenceEnd = 1.00f;
            }
            else if (IsLandingGearAnchor(anchor) && normalized.Contains("jia-lianjie"))
            {
                worldDirection = Vector3.down;
                distance = 0.12f;
                sequenceStart = 0.28f;
                sequenceEnd = 0.44f;
            }
            else if (IsLandingGearAnchor(anchor) && normalized.Contains("guan-cheng"))
            {
                worldDirection = Vector3.down;
                distance = 0.48f;
                sequenceStart = 0.34f;
                sequenceEnd = 0.58f;
            }
            else if (IsLandingGearAnchor(anchor) && normalized.Contains("carbon-fiber-tube"))
            {
                worldDirection = Vector3.down;
                distance = 1.04f;
                sequenceStart = 0.44f;
                sequenceEnd = 0.82f;
            }
            else if (IsLandingGearAnchor(anchor) && normalized.Contains("jiao-lianjie"))
            {
                worldDirection = Vector3.down;
                distance = 0.76f;
                sequenceStart = 0.42f;
                sequenceEnd = 0.72f;
            }
            else if (IsLandingGearAnchor(anchor) &&
                     (normalized.Contains("mao-jiao") || normalized.Contains("jiao-eva")))
            {
                worldDirection = ResolveModelZSideWorldDirection(member, anchor);
                distance = 1.45f;
                sequenceStart = 0.58f;
                sequenceEnd = 0.96f;
            }
            else if (normalized.Contains("hmx5v-jibi-jia-muju"))
            {
                Vector3 center = ResolveWorldCenter(member);
                Vector3 anchorCenter = ResolveWorldCenter(anchor.transform);
                Vector3 vertical = center.y > anchorCenter.y + 0.002f ? Vector3.up : Vector3.down;
                worldDirection = vertical;
                distance = 0.6f;
                sequenceStart = 0.62f;
                sequenceEnd = 0.92f;
            }
            else if (normalized.Contains("hmx5v-guan-dingwei"))
            {
                worldDirection = IsArmAnchor(anchor)
                    ? ResolveArmAxisWorldDirection(member, anchor)
                    : ResolveTubeAlignedWorldDirection(member, anchor);
                distance = 0.20f;
                sequenceStart = 0.38f;
                sequenceEnd = 0.50f;
            }
            else if (normalized.Contains("jia-guan"))
            {
                worldDirection = ResolveTubeAlignedWorldDirection(member, anchor);
                distance = 0.20f;
                sequenceStart = 0.72f;
                sequenceEnd = 0.96f;
            }
            else if (normalized.Contains("guan-cheng") ||
                     normalized.Contains("battery-pad") ||
                     normalized.Contains("pylons-x500"))
            {
                worldDirection = ResolveSubpieceWorldDirection(member, anchor, Vector3.down, 0.86f);
                distance = normalized.Contains("guan-cheng") ? 0.34f : 0.22f;
                sequenceStart = 0.52f;
                sequenceEnd = 0.84f;
            }
            else if (normalized.Contains("battery-mounting-plat"))
            {
                worldDirection = ResolveSubpieceWorldDirection(member, anchor, Vector3.down, 0.68f);
                distance = 0.22f;
                sequenceStart = 0.58f;
                sequenceEnd = 0.86f;
            }
            else if (normalized.Contains("jia-lianjie") || normalized.Contains("jiao-lianjie"))
            {
                worldDirection = ResolveSubpieceWorldDirection(member, anchor, Vector3.down, 0.62f);
                distance = 0.30f;
                sequenceStart = 0.56f;
                sequenceEnd = 0.88f;
            }
            else if (normalized.Contains("jiao-eva") ||
                     normalized.Contains("mao-jiao") ||
                     normalized.Contains("huan-guijiao") ||
                     normalized.Contains("rubber-grommet"))
            {
                worldDirection = (normalized.Contains("huan-guijiao") || normalized.Contains("rubber-grommet"))
                    ? (IsArmAnchor(anchor)
                        ? ResolveArmAxisWorldDirection(member, anchor)
                        : ResolveTubeAlignedWorldDirection(member, anchor))
                    : ResolveSubpieceWorldDirection(member, anchor, Vector3.down, 0.34f);
                distance = normalized.Contains("huan-guijiao") || normalized.Contains("rubber-grommet") ? 0.12f : 0.18f;
                sequenceStart = 0.72f;
                sequenceEnd = 0.98f;
            }
            else
            {
                return false;
            }

            localDirection = member.parent != null
                ? member.parent.InverseTransformVector(worldDirection)
                : worldDirection;
            if (localDirection.sqrMagnitude < 0.0001f)
            {
                localDirection = Vector3.up;
            }

            return true;
        }

        private static bool IsArmAnchor(ExplodablePart anchor)
        {
            return anchor != null &&
                   anchor.Data != null &&
                   !string.IsNullOrWhiteSpace(anchor.Data.id) &&
                   anchor.Data.id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRailBatteryAnchor(ExplodablePart anchor)
        {
            return anchor != null &&
                   anchor.Data != null &&
                   string.Equals(anchor.Data.id, "x500v2_rails_battery", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLandingGearAnchor(ExplodablePart anchor)
        {
            return anchor != null &&
                   anchor.Data != null &&
                   string.Equals(anchor.Data.id, "x500v2_landing_gear", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsElectronicsStackAnchor(ExplodablePart anchor)
        {
            if (anchor == null || anchor.Data == null)
            {
                return false;
            }

            string id = anchor.Data.id ?? string.Empty;
            return string.Equals(id, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_gps_m10", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_power_module", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_telemetry_radio", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(id, "x500v2_rc_receiver", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRailFrontBackSubpiece(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            if (normalized.Contains("battery-pad") ||
                normalized.Contains("pylons-x500") ||
                normalized.Contains("carbon-fiber-tube300"))
            {
                return false;
            }

            return normalized.Contains("platform-plat") ||
                   normalized.Contains("zhijia-camera-intel") ||
                   normalized.Contains("camera-intel") ||
                   normalized.Contains("battery-mounting-plat") ||
                   normalized.Contains("battery-strap") ||
                   normalized.Contains("battery") ||
                   normalized.Contains("lipo") ||
                   normalized.Contains("jia-guan") ||
                   normalized.Contains("huan-guijiao") ||
                   normalized.Contains("rubber-grommet");
        }

        private static bool IsRailForwardCarrierSubpiece(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("zhijia-camera-intel") ||
                   normalized.Contains("camera-intel") ||
                   normalized.Contains("gai-guangliu");
        }

        private static bool IsRailTubeCompanionSubpiece(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("jia-guan") ||
                   normalized.Contains("huan-guijiao") ||
                   normalized.Contains("rubber-grommet");
        }

        private static bool TryResolveExplicitRailZAxisDirection(
            Transform member,
            ExplodablePart anchor,
            string normalized,
            out Vector3 direction,
            out bool positiveZ)
        {
            if (IsRailRubberGrommetSubpiece(normalized) &&
                TryResolveRailRubberGrommetSide(member, anchor, out positiveZ))
            {
                Vector3 sideModelZ = ResolveModelZWorldDirection(member, anchor);
                direction = positiveZ ? sideModelZ : -sideModelZ;
                return direction.sqrMagnitude > 0.0001f;
            }

            positiveZ = IsExplicitRailPositiveZSubpiece(normalized);
            bool negativeZ = IsExplicitRailNegativeZSubpiece(normalized);
            if (!positiveZ && !negativeZ)
            {
                direction = Vector3.zero;
                return false;
            }

            Vector3 modelZ = ResolveModelZWorldDirection(member, anchor);
            direction = positiveZ ? modelZ : -modelZ;
            return direction.sqrMagnitude > 0.0001f;
        }

        private static bool IsRailRubberGrommetSubpiece(string normalized)
        {
            return !string.IsNullOrWhiteSpace(normalized) &&
                   (normalized.Contains("rubber-grommet") || normalized.Contains("huan-guijiao"));
        }

        private static bool TryResolveRailRubberGrommetSide(Transform member, ExplodablePart anchor, out bool positiveZ)
        {
            positiveZ = false;
            if (member == null || anchor == null)
            {
                return false;
            }

            Vector3 modelZ = ResolveModelZWorldDirection(member, anchor);
            if (modelZ.sqrMagnitude < 0.0001f)
            {
                return false;
            }

            Vector3 center = ResolveDroneCenter(anchor);
            Vector3 planarOffset = Vector3.ProjectOnPlane(ResolveWorldCenter(member) - center, Vector3.up);
            if (planarOffset.sqrMagnitude < 0.0001f)
            {
                return false;
            }

            positiveZ = Vector3.Dot(planarOffset, modelZ.normalized) >= 0f;
            return true;
        }

        private static bool IsExplicitRailNegativeZSubpiece(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return HasNormalizedInstance(normalized, "zhijia-camera-intel", 1) ||
                   HasNormalizedInstance(normalized, "gai-guangliu", 1) ||
                   HasNormalizedInstance(normalized, "jia-guan", 3) ||
                   HasNormalizedInstance(normalized, "jia-guan", 4) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 3) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 4) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 3) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 4);
        }

        private static bool IsExplicitRailPositiveZSubpiece(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return HasNormalizedInstance(normalized, "platform-plat-x500", 1) ||
                   HasNormalizedInstance(normalized, "jia-guan", 1) ||
                   HasNormalizedInstance(normalized, "jia-guan", 2) ||
                   HasNormalizedInstance(normalized, "jia-guan", 5) ||
                   HasNormalizedInstance(normalized, "jia-guan", 6) ||
                   HasNormalizedInstance(normalized, "jia-guan", 7) ||
                   HasNormalizedInstance(normalized, "jia-guan", 8) ||
                   HasNormalizedInstance(normalized, "jia-guan", 63) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 5) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 6) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 7) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 8) ||
                   HasNormalizedInstance(normalized, "huan-guijiao", 63) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 5) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 6) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 7) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 8) ||
                   HasNormalizedInstance(normalized, "rubber-grommet", 63);
        }

        private static bool HasNormalizedInstance(string normalized, string baseToken, int instance)
        {
            string suffix = "-" + instance.ToString("000", System.Globalization.CultureInfo.InvariantCulture);
            return normalized.Contains(baseToken + suffix);
        }

        private static float ResolveExplicitRailZDistance(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return 0.59f;
            }

            if (IsRailRubberGrommetSubpiece(normalized))
            {
                return 1.70f;
            }

            if (IsRailTubeCompanionSubpiece(normalized))
            {
                return 2.05f;
            }

            if (normalized.Contains("platform-plat-x500") ||
                normalized.Contains("zhijia-camera-intel") ||
                normalized.Contains("gai-guangliu"))
            {
                return 3.30f;
            }

            return 2.95f;
        }

        private static void ResolveExplicitRailZSequence(string normalized, bool positiveZ, out float start, out float end)
        {
            start = positiveZ ? 0.34f : 0.30f;
            end = positiveZ ? 0.92f : 0.88f;

            if (IsRailRubberGrommetSubpiece(normalized))
            {
                start = Mathf.Min(1f, start + 0.08f);
                end = Mathf.Min(1f, end + 0.06f);
            }
        }

        private static Vector3 ResolveModelZWorldDirection(Transform member, ExplodablePart anchor)
        {
            Transform root = anchor != null && anchor.transform != null
                ? anchor.transform.root
                : member != null ? member.root : null;
            if (root != null &&
                TryResolveDroneReferenceFrame(root, out _, out Vector3 cameraFront, out _, out _) &&
                cameraFront.sqrMagnitude > 0.0001f)
            {
                // In the X500 model, the Intel camera side is the semantic -Z side.
                return -Vector3.ProjectOnPlane(cameraFront, Vector3.up).normalized;
            }

            Vector3 direction = root != null ? root.TransformDirection(Vector3.forward) : Vector3.forward;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
        }

        private static Vector3 ResolveModelZSideWorldDirection(Transform member, ExplodablePart anchor)
        {
            Vector3 modelZ = ResolveModelZWorldDirection(member, anchor);
            if (modelZ.sqrMagnitude < 0.0001f)
            {
                return ResolveFrameFrontBackWorldDirection(member, anchor);
            }

            Vector3 center = ResolveDroneCenter(anchor);
            Vector3 memberCenter = member != null ? ResolveWorldCenter(member) : center;
            Vector3 planarOffset = Vector3.ProjectOnPlane(memberCenter - center, Vector3.up);
            float signed = Vector3.Dot(planarOffset, modelZ.normalized);
            return signed >= 0f ? modelZ.normalized : -modelZ.normalized;
        }

        private static Vector3 ResolveElectronicsStackWorldDirection(Transform member, ExplodablePart anchor)
        {
            float deltaY = ResolveWorldCenter(member).y - ResolveWorldCenter(anchor != null ? anchor.transform : null).y;
            return deltaY < -0.015f ? Vector3.down : Vector3.up;
        }

        private static float ResolveElectronicsStackDistance(Transform member, ExplodablePart anchor, string normalized)
        {
            float verticalSpread = Mathf.Abs(ResolveWorldCenter(member).y - ResolveWorldCenter(anchor != null ? anchor.transform : null).y);
            float distance = 0.16f + Mathf.Clamp(verticalSpread * 0.35f, 0f, 0.18f);
            if (normalized.Contains("gps") || normalized.Contains("antenna") || normalized.Contains("gan-gps"))
            {
                distance += 0.08f;
            }
            else if (normalized.Contains("pcb") || normalized.Contains("pixhawk") || normalized.Contains("power-module"))
            {
                distance += 0.04f;
            }

            return Mathf.Clamp(distance, 0.14f, 0.42f);
        }

        private static float ResolveElectronicsStackSequenceStart(Transform member, ExplodablePart anchor, string normalized)
        {
            float deltaY = ResolveWorldCenter(member).y - ResolveWorldCenter(anchor != null ? anchor.transform : null).y;
            float layerBias = Mathf.Clamp01((deltaY + 0.12f) / 0.24f) * 0.10f;
            if (normalized.Contains("gps") || normalized.Contains("antenna"))
            {
                layerBias += 0.04f;
            }

            return Mathf.Clamp(0.56f + layerBias, 0.54f, 0.72f);
        }

        private static Vector3 ResolveFrameFrontBackWorldDirection(Transform member, ExplodablePart anchor)
        {
            return ResolveFrameSignedWorldDirection(member, anchor, true);
        }

        private static Vector3 ResolveFrameForwardWorldDirection(Transform member, ExplodablePart anchor)
        {
            Vector3 front = Vector3.forward;
            Transform root = anchor != null && anchor.transform != null
                ? anchor.transform.root
                : member != null ? member.root : null;
            if (root != null &&
                TryResolveDroneReferenceFrame(root, out _, out Vector3 resolvedFront, out _, out _))
            {
                front = resolvedFront;
            }

            front = Vector3.ProjectOnPlane(front, Vector3.up);
            return front.sqrMagnitude > 0.0001f ? front.normalized : Vector3.forward;
        }

        private static Vector3 ResolveFrameLeftRightWorldDirection(Transform member, ExplodablePart anchor)
        {
            return ResolveFrameSignedWorldDirection(member, anchor, false);
        }

        private static Vector3 ResolveFrameSignedWorldDirection(Transform member, ExplodablePart anchor, bool useFrontAxis)
        {
            Vector3 center = anchor != null ? ResolveWorldCenter(anchor.transform) : Vector3.zero;
            Vector3 front = Vector3.forward;
            Vector3 right = Vector3.right;
            float dominantSize = 1f;
            Transform root = anchor != null && anchor.transform != null ? anchor.transform.root : null;
            if (root != null)
            {
                TryResolveDroneReferenceFrame(root, out center, out front, out right, out dominantSize);
            }

            Vector3 axis = useFrontAxis ? front : right;
            if (axis.sqrMagnitude < 0.0001f)
            {
                axis = useFrontAxis ? Vector3.forward : Vector3.right;
            }

            axis = Vector3.ProjectOnPlane(axis, Vector3.up);
            if (axis.sqrMagnitude < 0.0001f)
            {
                axis = useFrontAxis ? Vector3.forward : Vector3.right;
            }

            axis.Normalize();
            Vector3 memberCenter = ResolveWorldCenter(member);
            Vector3 offset = Vector3.ProjectOnPlane(memberCenter - center, Vector3.up);
            float signed = Vector3.Dot(offset, axis);
            float deadband = Mathf.Max(0.0005f, dominantSize * 0.01f);
            if (Mathf.Abs(signed) < deadband && anchor != null)
            {
                signed = Vector3.Dot(Vector3.ProjectOnPlane(memberCenter - ResolveWorldCenter(anchor.transform), Vector3.up), axis);
            }

            return signed >= 0f ? axis : -axis;
        }

        private static Vector3 BlendDirections(Vector3 planar, Vector3 axial, float axialWeight)
        {
            Vector3 resolvedPlanar = planar.sqrMagnitude > 0.0001f ? planar.normalized : Vector3.zero;
            Vector3 resolvedAxial = axial.sqrMagnitude > 0.0001f ? axial.normalized : Vector3.zero;
            Vector3 blended = resolvedPlanar * Mathf.Clamp01(1f - axialWeight) + resolvedAxial * Mathf.Clamp01(axialWeight);
            if (blended.sqrMagnitude > 0.0001f)
            {
                return blended.normalized;
            }

            return resolvedPlanar.sqrMagnitude > 0.0001f ? resolvedPlanar : Vector3.up;
        }

        private static bool IsPropellerSubpieceToken(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("x500v2-prop") ||
                   normalized.Contains("propeller") ||
                   normalized.Contains("-prop-") ||
                   normalized.StartsWith("prop-", StringComparison.Ordinal);
        }

        private static bool IsMotorSubpieceToken(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("dj-2216") ||
                   normalized.Contains("kv880") ||
                   normalized.Contains("x500v2-motor");
        }

        private static Vector3 ResolvePropellerAlignedWorldDirection(
            Transform propeller,
            ExplodablePart anchor,
            float axialWeight)
        {
            Vector3 anchorCenter = ResolveDroneCenter(anchor);
            Vector3 hubCenter = propeller != null ? propeller.position : anchorCenter;
            Vector3 radialSource = TryResolveNearestMotorCenter(anchor, hubCenter, out Vector3 motorCenter)
                ? motorCenter
                : hubCenter;
            Vector3 radial = Vector3.ProjectOnPlane(radialSource - anchorCenter, Vector3.up);
            if (radial.sqrMagnitude < 0.0001f)
            {
                radial = Vector3.ProjectOnPlane(hubCenter - anchorCenter, Vector3.up);
            }

            Vector3 resolvedRadial = radial.sqrMagnitude > 0.0001f ? radial.normalized : Vector3.zero;
            Vector3 resolvedAxial = Vector3.up;
            Vector3 blended = resolvedRadial * Mathf.Clamp01(1f - axialWeight) + resolvedAxial * Mathf.Clamp01(axialWeight);
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : resolvedAxial;
        }

        private static Vector3 ResolveArmAxisWorldDirection(Transform member, ExplodablePart anchor)
        {
            Vector3 center = ResolveDroneCenter(anchor);
            Vector3 memberCenter = member != null ? ResolveWorldCenter(member) : center;
            Vector3 axisPoint = TryResolveNearestMotorCenter(anchor, memberCenter, out Vector3 motorCenter)
                ? motorCenter
                : memberCenter;

            Vector3 direction = Vector3.ProjectOnPlane(axisPoint - center, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                return direction.normalized;
            }

            direction = Vector3.ProjectOnPlane(ResolveExplosionDirection(anchor), Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                return direction.normalized;
            }

            return Vector3.forward;
        }

        private static Vector3 ResolveDroneCenter(ExplodablePart anchor)
        {
            Transform root = anchor != null && anchor.transform != null ? anchor.transform.root : null;
            if (root != null &&
                TryResolveDroneReferenceFrame(root, out Vector3 center, out _, out _, out _))
            {
                return center;
            }

            return anchor != null ? ResolveWorldCenter(anchor.transform) : Vector3.zero;
        }

        private static bool TryResolveNearestMotorCenter(
            ExplodablePart anchor,
            Vector3 referencePoint,
            out Vector3 motorCenter)
        {
            motorCenter = Vector3.zero;
            if (anchor == null)
            {
                return false;
            }

            if (TryResolveNearestMotorCenter(
                    anchor.GetComponentsInChildren<Renderer>(true),
                    referencePoint,
                    out motorCenter))
            {
                return true;
            }

            Transform root = anchor.transform != null ? anchor.transform.root : null;
            return root != null &&
                   TryResolveNearestMotorCenter(root.GetComponentsInChildren<Renderer>(true), referencePoint, out motorCenter);
        }

        private static bool TryResolveNearestMotorCenter(
            Renderer[] renderers,
            Vector3 referencePoint,
            out Vector3 motorCenter)
        {
            motorCenter = Vector3.zero;
            if (renderers == null)
            {
                return false;
            }

            float bestDistance = float.PositiveInfinity;
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.transform == null)
                {
                    continue;
                }

                string normalized = SelectionHierarchy.NormalizeToken(renderer.transform.name);
                if (!IsMotorSubpieceToken(normalized))
                {
                    continue;
                }

                Vector3 center = renderer.bounds.center;
                float distance = Vector3.SqrMagnitude(center - referencePoint);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    motorCenter = center;
                }
            }

            return bestDistance < float.PositiveInfinity;
        }

        private static string BuildFastenerProfileToken(Transform member)
        {
            string normalized = SelectionHierarchy.NormalizeToken(member != null ? member.name : string.Empty);
            FastenerRuntimeMarker marker = member != null ? member.GetComponent<FastenerRuntimeMarker>() : null;
            AppendNormalizedToken(ref normalized, marker != null ? marker.SceneTypeKey : string.Empty);
            AppendNormalizedToken(ref normalized, marker != null ? marker.FastenerInstanceId : string.Empty);
            AppendNormalizedToken(ref normalized, marker != null ? marker.FastenerFamilyId : string.Empty);

            ExplodablePart part = member != null ? member.GetComponent<ExplodablePart>() : null;
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

        private static bool IsInvertedArmM25x6Screw(Transform member, string token)
        {
            if (IsInvertedArmM25x6ScrewToken(token))
            {
                return true;
            }

            string normalizedName = SelectionHierarchy.NormalizeToken(member != null ? member.name : string.Empty);
            FastenerRuntimeMarker marker = member != null ? member.GetComponent<FastenerRuntimeMarker>() : null;
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

        private static Vector3 ResolveFastenerAuxiliaryWorldDirection(Transform member, ExplodablePart anchor)
        {
            string normalized = SelectionHierarchy.NormalizeToken(member != null ? member.name : string.Empty);
            FastenerRuntimeMarker marker = member != null ? member.GetComponent<FastenerRuntimeMarker>() : null;
            string typeKey = marker != null ? SelectionHierarchy.NormalizeToken(marker.SceneTypeKey) : string.Empty;
            string token = normalized + "-" + typeKey;
            Vector3 axis = ResolveLikelyFastenerAxis(member);
            Vector3 anchorCenter = anchor != null ? ResolveWorldCenter(anchor.transform) : Vector3.zero;
            Vector3 away = member != null ? Vector3.ProjectOnPlane(ResolveWorldCenter(member) - anchorCenter, Vector3.up) : Vector3.zero;
            if (away.sqrMagnitude < 0.0001f && member != null)
            {
                away = ResolveWorldCenter(member) - anchorCenter;
            }
            if (away.sqrMagnitude < 0.0001f)
            {
                away = Vector3.up;
            }
            away.Normalize();

            bool isNut = IsNutToken(token);
            if (IsRubberGrommetToken(token))
            {
                return ResolveTubeAlignedWorldDirection(member, anchor);
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
                    Vector3 memberOffset = member != null ? ResolveWorldCenter(member) - anchorCenter : Vector3.up;
                    return memberOffset.y >= 0f
                        ? (verticalDot > 0f ? axis : -axis)
                        : (verticalDot > 0f ? -axis : axis);
                }

                return verticalDot > 0f ? axis : -axis;
            }

            return Vector3.Dot(axis, away) >= 0f ? axis : -axis;
        }

        private static int ExtractTrailingInstanceIndex(string normalizedName)
        {
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return -1;
            }

            string normalized = normalizedName;
            if (normalized.EndsWith("-low", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 4);
            }

            int lastDash = normalized.LastIndexOf('-');
            if (lastDash < 0 || lastDash + 1 >= normalized.Length)
            {
                return -1;
            }

            string suffix = normalized.Substring(lastDash + 1);
            return int.TryParse(suffix, out int index) ? index : -1;
        }

        private static float ResolveFastenerAuxiliaryDistance(Transform member)
        {
            string normalized = SelectionHierarchy.NormalizeToken(member != null ? member.name : string.Empty);
            FastenerRuntimeMarker marker = member != null ? member.GetComponent<FastenerRuntimeMarker>() : null;
            string typeKey = marker != null ? SelectionHierarchy.NormalizeToken(marker.SceneTypeKey) : string.Empty;
            string token = normalized + "-" + typeKey;

            if (IsNutToken(token))
            {
                return 0.46f;
            }

            if (IsRubberGrommetToken(token))
            {
                return 0.14f;
            }

            if (token.Contains("standoff") || token.Contains("nilongzhu"))
            {
                return 0.38f;
            }

            return 0.72f;
        }

        private static Vector3 ResolveSubpieceWorldDirection(
            Transform member,
            ExplodablePart anchor,
            Vector3 axialDirection,
            float axialWeight)
        {
            Vector3 anchorCenter = anchor != null ? ResolveWorldCenter(anchor.transform) : Vector3.zero;
            Vector3 memberCenter = member != null ? ResolveWorldCenter(member) : anchorCenter;
            Vector3 radial = Vector3.ProjectOnPlane(memberCenter - anchorCenter, Vector3.up);
            if (radial.sqrMagnitude < 0.0001f && anchor != null && anchor.Data != null)
            {
                radial = ResolveExplosionDirection(anchor);
                radial = Vector3.ProjectOnPlane(radial, Vector3.up);
            }

            Vector3 resolvedAxial = axialDirection.sqrMagnitude > 0.0001f ? axialDirection.normalized : Vector3.up;
            Vector3 resolvedRadial = radial.sqrMagnitude > 0.0001f ? radial.normalized : Vector3.zero;
            Vector3 blended = resolvedRadial * Mathf.Clamp01(1f - axialWeight) + resolvedAxial * Mathf.Clamp01(axialWeight);
            return blended.sqrMagnitude > 0.0001f ? blended.normalized : resolvedAxial;
        }

        private static Vector3 ResolveTubeAlignedWorldDirection(Transform member, ExplodablePart anchor)
        {
            if (IsArmAnchor(anchor))
            {
                return ResolveArmAxisWorldDirection(member, anchor);
            }

            Vector3 direction = anchor != null ? ResolveExplosionDirection(anchor) : Vector3.zero;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                return direction.normalized;
            }

            Vector3 anchorCenter = anchor != null ? ResolveWorldCenter(anchor.transform) : Vector3.zero;
            Vector3 memberCenter = member != null ? ResolveWorldCenter(member) : anchorCenter;
            direction = Vector3.ProjectOnPlane(memberCenter - anchorCenter, Vector3.up);
            if (direction.sqrMagnitude > 0.0001f)
            {
                return direction.normalized;
            }

            return Vector3.forward;
        }

        private static Vector3 ResolveLikelyFastenerAxis(Transform fastener)
        {
            if (fastener == null)
            {
                return Vector3.up;
            }

            Renderer renderer = fastener.GetComponentInChildren<Renderer>(true);
            MeshFilter meshFilter = renderer != null ? renderer.GetComponent<MeshFilter>() : fastener.GetComponentInChildren<MeshFilter>(true);
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return fastener.up;
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

            return meshFilter.transform.TransformDirection(localAxis);
        }

        private static Vector3 ResolveWorldCenter(Transform transform)
        {
            if (transform == null)
            {
                return Vector3.zero;
            }

            Renderer renderer = transform.GetComponentInChildren<Renderer>(true);
            return renderer != null ? renderer.bounds.center : transform.position;
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

        private static void CalibrateExplosionPreset(string anchorId, ExplodablePart anchor)
        {
            if (anchor == null || anchor.Data == null)
            {
                return;
            }

            string resolvedId = !string.IsNullOrWhiteSpace(anchor.Data.id) ? anchor.Data.id : anchorId;
            anchor.Data.explosionPriority = ResolveRuntimeExplosionPriority(resolvedId, anchor.Data.partType, anchor.Data.explosionPriority);
            if (TryResolveRuntimeExplosionPreset(resolvedId, anchor.Data.partType, out Vector3 direction, out float minDistance, out float maxDistance))
            {
                anchor.Data.explosionDirection = direction;
                anchor.Data.explosionDistance = Mathf.Clamp(
                    anchor.Data.explosionDistance > 0.001f ? anchor.Data.explosionDistance : minDistance,
                    minDistance,
                    maxDistance);
                return;
            }

            // Keep authored presets for uncategorized pieces and only repair invalid values.
            if (anchor.Data.explosionDistance <= 0.001f)
            {
                anchor.Data.explosionDistance = 0.2f;
            }

            if (anchor.Data.explosionDirection.sqrMagnitude <= 0.0001f)
            {
                anchor.Data.explosionDirection = Vector3.up;
            }
            else
            {
                anchor.Data.explosionDirection = anchor.Data.explosionDirection.normalized;
            }
        }

        private static bool TryResolveRuntimeExplosionPreset(
            string partId,
            string partType,
            out Vector3 direction,
            out float minDistance,
            out float maxDistance)
        {
            string id = (partId ?? string.Empty).ToLowerInvariant();
            string type = (partType ?? string.Empty).ToLowerInvariant();

            direction = Vector3.up;
            minDistance = 0.2f;
            maxDistance = 1.0f;

            if (id.StartsWith("x500v2_prop_", StringComparison.Ordinal) || type.Contains("prop"))
            {
                direction = ResolveQuadrantDirection(id, 0.55f);
                minDistance = 1.6f;
                maxDistance = 2.2f;
                return true;
            }

            if (id.StartsWith("x500v2_motor_", StringComparison.Ordinal) || type.Contains("motor"))
            {
                direction = ResolveQuadrantDirection(id, 0.45f);
                minDistance = 1.0f;
                maxDistance = 1.6f;
                return true;
            }

            if (id.StartsWith("x500v2_arm_", StringComparison.Ordinal) || type.Contains("arm"))
            {
                direction = ResolveQuadrantDirection(id, 0.18f);
                minDistance = 0.575f;
                maxDistance = 0.8f;
                return true;
            }

            if (id == "x500v2_top_plate")
            {
                direction = Vector3.up;
                minDistance = 0.35f;
                maxDistance = 0.75f;
                return true;
            }

            if (id == "x500v2_bottom_plate")
            {
                direction = Vector3.zero;
                minDistance = 0f;
                maxDistance = 0f;
                return true;
            }

            if (id == "x500v2_platform_board")
            {
                direction = Vector3.up;
                minDistance = 0.45f;
                maxDistance = 0.85f;
                return true;
            }

            if (id == "x500v2_pixhawk6c")
            {
                direction = Vector3.up;
                minDistance = 0.55f;
                maxDistance = 1.05f;
                return true;
            }

            if (id == "x500v2_power_module" || id == "x500v2_pdb")
            {
                direction = Vector3.up;
                minDistance = 0.45f;
                maxDistance = 0.9f;
                return true;
            }

            if (id == "x500v2_rails_battery")
            {
                direction = Vector3.down;
                minDistance = 0.55f;
                maxDistance = 0.95f;
                return true;
            }

            if (id == "x500v2_landing_gear" || type.Contains("landing"))
            {
                direction = Vector3.down;
                minDistance = 0.56f;
                maxDistance = 0.65f;
                return true;
            }

            if (id == "x500v2_gps_m10" || type.Contains("gps"))
            {
                direction = Vector3.up;
                minDistance = 0.9f;
                maxDistance = 1.45f;
                return true;
            }

            if (id == "x500v2_telemetry_radio" || id == "x500v2_rc_receiver")
            {
                direction = Vector3.up;
                minDistance = 0.45f;
                maxDistance = 0.85f;
                return true;
            }

            if (id == "x500v2_battery" || type.Contains("battery"))
            {
                direction = Vector3.down;
                minDistance = 0.45f;
                maxDistance = 0.75f;
                return true;
            }

            return false;
        }

        private static int ResolveRuntimeExplosionPriority(string partId, string partType, int fallbackPriority)
        {
            string id = (partId ?? string.Empty).ToLowerInvariant();
            string type = (partType ?? string.Empty).ToLowerInvariant();

            // Lower values start earlier in ExplodedViewManager. The order follows
            // a disassembly reading: free/removable items first, structural anchors
            // later, so dependent pieces do not visually drag their supports away.
            if (id.StartsWith("x500v2_prop_", StringComparison.Ordinal) || type.Contains("prop")) return 1;
            if (id.StartsWith("x500v2_motor_", StringComparison.Ordinal) || type.Contains("motor")) return 1;
            if (id == "x500v2_battery" || type.Contains("battery")) return 2;
            if (id == "x500v2_gps_m10" || type.Contains("gps")) return 2;
            if (id == "x500v2_rc_receiver" || id == "x500v2_telemetry_radio") return 2;
            if (id == "x500v2_pixhawk6c") return 4;
            if (id == "x500v2_power_module" || id == "x500v2_pdb") return 4;
            if (id == "x500v2_platform_board") return 4;
            if (id == "x500v2_top_plate") return 5;
            if (id == "x500v2_rails_battery" || id == "x500v2_landing_gear" || type.Contains("landing")) return 6;
            if (id.StartsWith("x500v2_arm_", StringComparison.Ordinal) || type.Contains("arm")) return 7;
            if (id == "x500v2_bottom_plate") return 8;

            return fallbackPriority;
        }

        private static Vector3 ResolveExplosionDirection(ExplodablePart anchor)
        {
            if (anchor != null && anchor.Data != null && anchor.Data.explosionDirection.sqrMagnitude > 0.0001f)
            {
                return anchor.Data.explosionDirection.normalized;
            }

            return Vector3.up;
        }

        private static Vector3 ResolveQuadrantDirection(string rawId, float y)
        {
            string suffix = ResolveQuadrantSuffix((rawId ?? string.Empty).ToLowerInvariant());
            return suffix switch
            {
                "fl" => new Vector3(-1f, y, 1f).normalized,
                "fr" => new Vector3(1f, y, 1f).normalized,
                "bl" => new Vector3(-1f, y, -1f).normalized,
                "br" => new Vector3(1f, y, -1f).normalized,
                _ => new Vector3(0f, y, 1f).normalized,
            };
        }

        private static string InferDisplayCategory(string rendererName, string fallbackCategory, string thermalSourceId)
        {
            string normalized = (thermalSourceId ?? rendererName ?? string.Empty).ToLowerInvariant();
            if (normalized.Contains("motor") || normalized.Contains("prop") || normalized.Contains("esc"))
            {
                return "PropulsionSystem";
            }

            if (normalized.Contains("battery") || normalized.Contains("pdb") || normalized.Contains("power_module"))
            {
                return "PowerDistribution";
            }

            if (normalized.Contains("gps") || normalized.Contains("receiver") || normalized.Contains("telemetry") || normalized.Contains("radio"))
            {
                return "SensorsComms";
            }

            if (normalized.Contains("pixhawk") || normalized.Contains("flight_controller") || normalized.Contains("fc"))
            {
                return "Avionics";
            }

            if (normalized.Contains("arm") || normalized.Contains("plate") || normalized.Contains("landing") ||
                normalized.Contains("platform") || normalized.Contains("rail"))
            {
                return "SkeletonAirframe";
            }

            return string.IsNullOrWhiteSpace(fallbackCategory) ? "Uncategorized" : fallbackCategory;
        }

        private static string InferThermalSourcePartId(string rendererName, string anchorId)
        {
            string lowerName = (rendererName ?? string.Empty).ToLowerInvariant();
            string searchableName = lowerName.Replace('_', '-').Replace(' ', '-');
            string suffix = ResolveQuadrantSuffix(lowerName);
            if (string.IsNullOrWhiteSpace(suffix))
            {
                suffix = ResolveQuadrantSuffix((anchorId ?? string.Empty).ToLowerInvariant());
            }

            if (searchableName.Contains("prop") && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_prop_{suffix.ToUpperInvariant()}";
            if ((searchableName.Contains("motor") || searchableName.Contains("dj-2216")) && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_motor_{suffix.ToUpperInvariant()}";
            if (searchableName.Contains("esc") && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_esc_{suffix.ToUpperInvariant()}";
            if (searchableName.Contains("guan-cheng")) return "x500v2_landing_gear";
            if (searchableName.Contains("battery-mounting") || searchableName.Contains("battery-pad") ||
                searchableName.Contains("pylons") || searchableName.Contains("rail") ||
                searchableName.Contains("carbon-fiber-tube300") ||
                searchableName.Contains("camera-intel") ||
                searchableName.Contains("guangliu") ||
                searchableName.Contains("jia-guan") ||
                searchableName.Contains("huan-guijiao") ||
                searchableName.Contains("rubber-grommet"))
            {
                return "x500v2_rails_battery";
            }
            if (searchableName.Contains("battery")) return "x500v2_battery";
            if (searchableName.Contains("imu-pixhawk")) return MiscGroupId;
            if (searchableName.Contains("pixhawk")) return "x500v2_pixhawk6c";
            if (searchableName.Contains("gps") || searchableName.Contains("gan-gpsv5") || searchableName.Contains("gpsv5-zhijia")) return "x500v2_gps_m10";
            if (searchableName.Contains("pdb")) return string.Empty;
            if (searchableName.Contains("power-module") || searchableName.Contains("pm06") || searchableName.Contains("xt60") || searchableName.Contains("bm06b")) return "x500v2_power_module";
            if (searchableName.Contains("receiver")) return "x500v2_rc_receiver";
            if (searchableName.Contains("telemetry") || searchableName.Contains("radio")) return "x500v2_telemetry_radio";
            if (searchableName.Contains("landing") || searchableName.Contains("jiao-") || searchableName.Contains("mao-jiao")) return "x500v2_landing_gear";
            if (searchableName.Contains("top-plate")) return "x500v2_top_plate";
            if (searchableName.Contains("camera-intel")) return "x500v2_rails_battery";
            if (searchableName.Contains("guangliu")) return "x500v2_rails_battery";
            if (searchableName.Contains("bottom-plate")) return "x500v2_bottom_plate";
            if (searchableName.Contains("platform-plat")) return "x500v2_rails_battery";

            return anchorId;
        }

        private static string ResolveExpectedAnchorIdFromName(string rawName, string fallbackAnchorId)
        {
            string hierarchyResolved = SelectionHierarchy.ResolveCanonicalPartId(rawName, Vector3.zero, null, fallbackAnchorId);
            if (!string.IsNullOrWhiteSpace(hierarchyResolved) &&
                !string.Equals(hierarchyResolved, fallbackAnchorId, StringComparison.OrdinalIgnoreCase))
            {
                return hierarchyResolved;
            }

            if (string.IsNullOrWhiteSpace(rawName))
            {
                return fallbackAnchorId ?? string.Empty;
            }

            string lowerName = rawName.ToLowerInvariant();
            string searchableName = lowerName.Replace('_', '-').Replace(' ', '-');
            string suffix = ResolveQuadrantSuffix(lowerName);
            if (string.IsNullOrWhiteSpace(suffix) && !string.IsNullOrWhiteSpace(fallbackAnchorId))
            {
                suffix = ResolveQuadrantSuffix(fallbackAnchorId.ToLowerInvariant());
            }

            bool hasFastenerToken = IsRecognizedFastenerName(lowerName) ||
                                    lowerName.Contains("fastener") || lowerName.Contains("screw") || lowerName.Contains("cap_screw") ||
                                    lowerName.Contains("bolt") || lowerName.Contains("nut") || lowerName.Contains("washer") ||
                                    lowerName.Contains("standoff") || lowerName.Contains("spacer") ||
                                    lowerName.Contains("gb70") || lowerName.Contains("lm-") || lowerName.Contains("zslm") ||
                                    lowerName.Contains("nilongzhu") || lowerName.Contains("chen-liu") || lowerName.Contains("pan-ding");

            if (hasFastenerToken)
            {
                return string.Empty;
            }

            // Propellers are visual/functional subpieces of each arm. They remain
            // thermally independent through InferThermalSourcePartId, but selection
            // and isolation treat them as part of the arm assembly.
            if (!string.IsNullOrWhiteSpace(suffix) &&
                (searchableName.Contains("propeller") || searchableName.Contains("prop")))
            {
                return $"x500v2_arm_{suffix.ToUpperInvariant()}";
            }

            if (!string.IsNullOrWhiteSpace(suffix) &&
                (searchableName.Contains("motor") || searchableName.Contains("dj-2216")))
            {
                return $"x500v2_motor_{suffix.ToUpperInvariant()}";
            }

            if (!string.IsNullOrWhiteSpace(suffix) && searchableName.Contains("esc"))
            {
                return $"x500v2_esc_{suffix.ToUpperInvariant()}";
            }

            if (!string.IsNullOrWhiteSpace(suffix) &&
                (searchableName.Contains("arm") || searchableName.Contains("hmx5v") ||
                 searchableName.Contains("ban-dj")) &&
                !searchableName.Contains("carbon-fiber-tube300") &&
                !searchableName.Contains("jia-guan") &&
                !searchableName.Contains("huan-guijiao") &&
                !searchableName.Contains("rubber-grommet"))
            {
                return $"x500v2_arm_{suffix.ToUpperInvariant()}";
            }

            // Core electronics family
            if (searchableName.Contains("imu-pixhawk"))
            {
                return MiscGroupId;
            }

            if (searchableName.Contains("pixhawk") || searchableName.Contains("pcb-pixhawk"))
            {
                return "x500v2_pixhawk6c";
            }

            if (searchableName.Contains("gps") || searchableName.Contains("gan-gpsv5") || searchableName.Contains("gpsv5-zhijia"))
            {
                return "x500v2_gps_m10";
            }

            if (searchableName.Contains("pdb"))
            {
                return string.Empty;
            }

            if (searchableName.Contains("power-module") || searchableName.Contains("pm06") || searchableName.Contains("xt60") || searchableName.Contains("bm06b"))
            {
                return "x500v2_power_module";
            }

            if (searchableName.Contains("guan-cheng"))
            {
                return "x500v2_landing_gear";
            }

            if (searchableName.Contains("battery-mounting") || searchableName.Contains("battery-pad") ||
                searchableName.Contains("pylons") || searchableName.Contains("rails") ||
                searchableName.Contains("strap") ||
                searchableName.Contains("platform-plat") ||
                searchableName.Contains("camera-intel") ||
                searchableName.Contains("guangliu") ||
                searchableName.Contains("carbon-fiber-tube300") ||
                searchableName.Contains("jia-guan") ||
                searchableName.Contains("huan-guijiao") ||
                searchableName.Contains("rubber-grommet"))
            {
                return "x500v2_rails_battery";
            }

            if (searchableName.Contains("battery"))
            {
                return "x500v2_rails_battery";
            }

            // Frame / landing family
            if (searchableName.Contains("landing") || searchableName.Contains("jiao-") || searchableName.Contains("mao-jiao") ||
                searchableName.Contains("carbon-fiber-tube") && !searchableName.Contains("tube300"))
            {
                return "x500v2_landing_gear";
            }

            if (searchableName.Contains("top-plate"))
            {
                return "x500v2_top_plate";
            }

            if (searchableName.Contains("camera-intel"))
            {
                return "x500v2_rails_battery";
            }

            if (searchableName.Contains("guangliu"))
            {
                return "x500v2_rails_battery";
            }

            if (searchableName.Contains("bottom-plate"))
            {
                return "x500v2_bottom_plate";
            }

            if (searchableName.Contains("platform-plat"))
            {
                return "x500v2_rails_battery";
            }

            return fallbackAnchorId ?? string.Empty;
        }

        private static bool IsStableCanonicalAnchorId(string anchorId)
        {
            return !string.IsNullOrWhiteSpace(anchorId) &&
                   anchorId.StartsWith("x500v2_", StringComparison.OrdinalIgnoreCase) &&
                   !anchorId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase) &&
                   !anchorId.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(anchorId, FastenerGroupId, StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(anchorId, MiscGroupId, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsArmCanonicalAnchorId(string anchorId)
        {
            return !string.IsNullOrWhiteSpace(anchorId) &&
                   anchorId.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLandingGearMemberName(string rawName)
        {
            string token = SelectionHierarchy.NormalizeToken(rawName);
            return token.Contains("landing") ||
                   token.Contains("guan-cheng") ||
                   token.Contains("jia-lianjie") ||
                   token.Contains("jiao-lianjie") ||
                   token.Contains("jiao-eva") ||
                   token.Contains("mao-jiao") ||
                   token.Contains("carbon-fiber-tube") && !token.Contains("carbon-fiber-tube300");
        }

        private static string InferAuxiliaryCategory(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            string name = rawName.ToLowerInvariant();

            if (IsRecognizedFastenerName(name) ||
                name.Contains("fastener") || name.Contains("screw") || name.Contains("cap_screw") ||
                name.Contains("bolt") || name.Contains("nut") || name.Contains("washer") ||
                name.Contains("standoff") || name.Contains("spacer"))
            {
                return "Fasteners";
            }

            if (name.Contains("misc") || name.Contains("clip") || name.Contains("locator") || name.Contains("clamp") ||
                name.Contains("brace") || name.Contains("guide") || name.Contains("connector") || name.Contains("holder"))
            {
                return "Uncategorized";
            }

            return string.Empty;
        }

        private static string ResolveQuadrantSuffix(string lowerName)
        {
            if (lowerName.Contains("_fl")) return "fl";
            if (lowerName.Contains("_fr")) return "fr";
            if (lowerName.Contains("_bl")) return "bl";
            if (lowerName.Contains("_br")) return "br";
            return string.Empty;
        }

        private static bool IsRecognizedFastenerName(string rawName)
        {
            return SelectionHierarchy.IsPrimitiveFastenerName(rawName);
        }

        private Transform ResolveDroneRoot()
        {
            if (name.StartsWith("x500v2_Drone"))
            {
                return transform;
            }

            GameObject namedRoot = GameObject.Find("x500v2_Drone");
            return namedRoot != null ? namedRoot.transform : transform;
        }

        private static Transform[] FindPropellers(Transform droneRoot)
        {
            List<Transform> propellers = new List<Transform>();
            foreach (Transform child in droneRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child == null)
                {
                    continue;
                }

                if (child.name.StartsWith("x500v2_prop_", System.StringComparison.OrdinalIgnoreCase) ||
                    child.name.IndexOf("propeller", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    propellers.Add(child);
                }
            }

            return propellers.ToArray();
        }

        /// <summary>
        /// Post-binding diagnostic: detects duplicate FastenerRuntimeMarker instanceIds
        /// across the scene, which is the root cause of multi-instance isolation leaks.
        /// </summary>
        private static void ValidateFastenerIdentityInvariants()
        {
            FastenerRuntimeMarker[] allMarkers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);
            Dictionary<string, List<string>> idToGameObjects = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (FastenerRuntimeMarker marker in allMarkers)
            {
                if (marker == null || string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
                {
                    continue;
                }

                if (!idToGameObjects.TryGetValue(marker.FastenerInstanceId, out List<string> objectNames))
                {
                    objectNames = new List<string>();
                    idToGameObjects[marker.FastenerInstanceId] = objectNames;
                }

                objectNames.Add(marker.gameObject.name);
            }

            int violations = 0;
            foreach (KeyValuePair<string, List<string>> kvp in idToGameObjects)
            {
                if (kvp.Value.Count > 1)
                {
                    violations++;
                    Debug.LogWarning("[FastenerIdentity] Duplicate instanceId '" + kvp.Key + "' on " + kvp.Value.Count + " GOs: " + string.Join(", ", kvp.Value));
                }
            }

            if (violations == 0)
            {
                Debug.Log("[FastenerIdentity] All " + idToGameObjects.Count + " fastener instanceIds are unique. Identity invariant OK.");
            }
            else
            {
                Debug.LogError("[FastenerIdentity] " + violations + " duplicate instanceId violations detected! Isolation may show multiple instances.");
            }
        }
    }
}
