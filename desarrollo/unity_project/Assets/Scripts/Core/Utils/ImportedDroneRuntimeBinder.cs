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
            anchorsById = BuildAnchorMap(droneRoot);

            foreach (KeyValuePair<string, ExplodablePart> kvp in anchorsById)
            {
                ConfigureAnchor(kvp.Key, kvp.Value, registry);
            }

            SealLooseFastenerMetadata(droneRoot, registry);
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

                string parentId = ResolveDingweiParentCanonicalId(candidate.name);
                if (string.IsNullOrWhiteSpace(parentId) ||
                    !anchorsById.TryGetValue(parentId, out ExplodablePart parentPart) ||
                    parentPart == null)
                {
                    Debug.LogWarning($"[ImportedDroneRuntimeBinder] HMX5V-GUAN-DINGWEI sin padre canonico confiable: {candidate.name}");
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
                category.Configure(parentId, primaryCategory, string.Empty, parentId, "hmx5v-guan-dingwei");
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

        private static string ResolveDingweiParentCanonicalId(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            string typeKey = FastenerNamingUtility.ExtractSceneTypeKey(rawName).ToLowerInvariant();
            if (typeKey.Contains(".001"))
            {
                return "x500v2_arm_BR";
            }

            if (typeKey.Contains(".002"))
            {
                return "x500v2_arm_FR";
            }

            if (typeKey.Contains(".003"))
            {
                return "x500v2_arm_FL";
            }

            if (typeKey.Contains(".004"))
            {
                return "x500v2_arm_BL";
            }

            return string.Empty;
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

                if (IsStableCanonicalAnchorId(currentAnchorId) &&
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

            foreach (Renderer renderer in anchor.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
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

                ConfigureAuxiliaryExplode(renderer.transform, anchor, auxiliaryCategory);
            }

            SealFastenerMetadata(anchor, registry);
            anchor.Initialize();
        }

        private static void SealFastenerMetadata(ExplodablePart anchor, FastenerRegistry registry)
        {
            if (anchor == null)
            {
                return;
            }

            DronePartData data = anchor.Data;
            bool isFastener = (data != null && data.category == PartCategory.Fasteners) ||
                              anchor.name.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase);
            if (!isFastener)
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

        private static void CalibrateExplosionPreset(string anchorId, ExplodablePart anchor)
        {
            if (anchor == null || anchor.Data == null)
            {
                return;
            }

            // Keep authored explosion presets from DronePartData and only repair invalid values.
            // This avoids forcing runtime axis changes that can disorient the exploded view.
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
            if (searchableName.Contains("battery-mounting") || searchableName.Contains("battery-pad") || searchableName.Contains("pylons") || searchableName.Contains("rail") || searchableName.Contains("guan-cheng")) return "x500v2_rails_battery";
            if (searchableName.Contains("battery")) return "x500v2_battery";
            if (searchableName.Contains("pixhawk") || searchableName.Contains("imu-pixhawk") || searchableName.Contains("bm06b")) return "x500v2_pixhawk6c";
            if (searchableName.Contains("gps") || searchableName.Contains("gan-gpsv5") || searchableName.Contains("gpsv5-zhijia")) return "x500v2_gps_m10";
            if (searchableName.Contains("pdb")) return "x500v2_pdb";
            if (searchableName.Contains("power-module") || searchableName.Contains("pm06") || searchableName.Contains("xt60")) return "x500v2_power_module";
            if (searchableName.Contains("receiver")) return "x500v2_rc_receiver";
            if (searchableName.Contains("telemetry") || searchableName.Contains("radio")) return "x500v2_telemetry_radio";
            if (searchableName.Contains("landing") || searchableName.Contains("jiao-") || searchableName.Contains("mao-jiao")) return "x500v2_landing_gear";
            if (searchableName.Contains("top-plate")) return "x500v2_top_plate";
            if (searchableName.Contains("bottom-plate") || searchableName.Contains("camera-intel") || searchableName.Contains("guangliu")) return "x500v2_bottom_plate";
            if (searchableName.Contains("platform-plat")) return "x500v2_platform_board";

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

            // Propulsion anchors remain selectable and thermally independent. The arm
            // assembly keeps only structural arm meshes; fasteners are resolved by
            // catalog metadata instead of broad name heuristics.
            if (!string.IsNullOrWhiteSpace(suffix) &&
                (searchableName.Contains("propeller") || searchableName.Contains("prop")))
            {
                return $"x500v2_prop_{suffix.ToUpperInvariant()}";
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
                (searchableName.Contains("arm") || searchableName.Contains("carbon-fiber-tube300") || searchableName.Contains("hmx5v") ||
                 searchableName.Contains("ban-dj") || searchableName.Contains("jia-guan")))
            {
                return $"x500v2_arm_{suffix.ToUpperInvariant()}";
            }

            // Core electronics family
            if (searchableName.Contains("pixhawk") || searchableName.Contains("imu-pixhawk") || searchableName.Contains("pcb-pixhawk") || searchableName.Contains("bm06b"))
            {
                return "x500v2_pixhawk6c";
            }

            if (searchableName.Contains("gps") || searchableName.Contains("gan-gpsv5") || searchableName.Contains("gpsv5-zhijia"))
            {
                return "x500v2_gps_m10";
            }

            if (searchableName.Contains("pdb"))
            {
                return "x500v2_pdb";
            }

            if (searchableName.Contains("power-module") || searchableName.Contains("pm06") || searchableName.Contains("xt60"))
            {
                return "x500v2_power_module";
            }

            if (searchableName.Contains("battery-mounting") || searchableName.Contains("battery-pad") ||
                searchableName.Contains("pylons") || searchableName.Contains("rails") ||
                searchableName.Contains("guan-cheng") || searchableName.Contains("strap"))
            {
                return "x500v2_rails_battery";
            }

            if (searchableName.Contains("battery"))
            {
                return "x500v2_battery";
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

            if (searchableName.Contains("bottom-plate") || searchableName.Contains("camera-intel") || searchableName.Contains("guangliu"))
            {
                return "x500v2_bottom_plate";
            }

            if (searchableName.Contains("platform-plat"))
            {
                return "x500v2_platform_board";
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
