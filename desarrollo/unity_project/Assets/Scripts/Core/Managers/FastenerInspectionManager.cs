using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Events;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [DisallowMultipleComponent]
    public class FastenerInspectionManager : Singleton<FastenerInspectionManager>
    {
        private sealed class ActiveInspection
        {
            public Transform ProxyRoot;
            public GameObject DetailRoot;
            public string FastenerInstanceId;
            public string NormalizedFastenerInstanceId;
            public string NormalizedSceneObjectName;
            public readonly List<Renderer> HiddenProxyRenderers = new List<Renderer>();
        }

        private readonly Dictionary<Transform, ActiveInspection> activeInspections = new Dictionary<Transform, ActiveInspection>();
        private readonly List<Transform> cachedFastenerRoots = new List<Transform>();
        private readonly Dictionary<string, List<Transform>> fastenersByParentCanonical = new Dictionary<string, List<Transform>>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<Transform> desiredProxyRoots = new HashSet<Transform>();

        private bool cacheInitialized;

        private void OnEnable()
        {
            EventBus.Subscribe<PartSelectedEvent>(HandlePartSelected);
            cacheInitialized = false;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(HandlePartSelected);
            ClearAllInspections();
        }

        private void LateUpdate()
        {
            RefreshFastenerCacheIfNeeded();
            ReconcileInspectionSet();
        }

        private void HandlePartSelected(PartSelectedEvent evt)
        {
            // Selection events are still useful to invalidate the desired set
            // immediately, but the effective replacement policy is now resolved
            // every frame from selection + isolation state.
        }

        private void ReconcileInspectionSet()
        {
            desiredProxyRoots.Clear();
            CollectSelectedFastener(desiredProxyRoots);
            CollectIsolatedFasteners(desiredProxyRoots);

            if (desiredProxyRoots.Count == 0)
            {
                bool hadActiveInspections = activeInspections.Count > 0;
                ClearAllInspections();
                if (hadActiveInspections)
                {
                    PartVisibilityManager.Instance?.ReapplyCurrentIsolationVisuals();
                }
                return;
            }

            List<Transform> rootsToRemove = null;
            foreach (KeyValuePair<Transform, ActiveInspection> pair in activeInspections)
            {
                if (pair.Key != null && desiredProxyRoots.Contains(pair.Key))
                {
                    continue;
                }

                rootsToRemove ??= new List<Transform>();
                rootsToRemove.Add(pair.Key);
            }

            if (rootsToRemove != null)
            {
                for (int i = 0; i < rootsToRemove.Count; i++)
                {
                    ClearInspection(rootsToRemove[i]);
                }

                PartVisibilityManager.Instance?.ReapplyCurrentIsolationVisuals();
            }

            foreach (Transform proxyRoot in desiredProxyRoots)
            {
                if (proxyRoot == null || activeInspections.ContainsKey(proxyRoot))
                {
                    continue;
                }

                FastenerMetadata metadata = ResolveInspectableMetadata(proxyRoot);
                if (metadata == null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(metadata.instanceId) &&
                    HasActiveInspectionForInstanceId(metadata.instanceId))
                {
                    continue;
                }

                ShowInspection(proxyRoot, metadata);
            }
        }

        private void CollectSelectedFastener(HashSet<Transform> targets)
        {
            Transform selection = SelectionManager.Instance != null ? SelectionManager.Instance.CurrentSelection : null;
            if (selection == null)
            {
                return;
            }

            if (TryResolveFastenerRoot(selection, out Transform fastenerRoot))
            {
                targets.Add(fastenerRoot);
            }
        }

        private void CollectIsolatedFasteners(HashSet<Transform> targets)
        {
            PartVisibilityManager visibilityManager = PartVisibilityManager.Instance;
            if (visibilityManager == null)
            {
                return;
            }

            Transform isolatedTransform = visibilityManager.GetIsolatedTransform();
            if (isolatedTransform == null)
            {
                return;
            }

            if (TryResolveFastenerRoot(isolatedTransform, out Transform isolatedFastenerRoot))
            {
                targets.Add(isolatedFastenerRoot);
                return;
            }

            string parentCanonicalPartId = ResolveCanonicalPartId(isolatedTransform);
            if (string.IsNullOrWhiteSpace(parentCanonicalPartId) ||
                !fastenersByParentCanonical.TryGetValue(parentCanonicalPartId, out List<Transform> associatedFasteners))
            {
                return;
            }

            for (int i = 0; i < associatedFasteners.Count; i++)
            {
                Transform fastenerRoot = associatedFasteners[i];
                if (fastenerRoot == null)
                {
                    continue;
                }

                targets.Add(fastenerRoot);
            }
        }

        private FastenerMetadata ResolveInspectableMetadata(Transform proxyRoot)
        {
            if (proxyRoot == null)
            {
                return null;
            }

            FastenerRegistry registry = FastenerRegistry.Instance;
            if (registry == null)
            {
                return null;
            }

            FastenerMetadata metadata = registry.ResolveMetadata(proxyRoot);
            return metadata != null && metadata.isInspectable && FastenerBuilder.CanBuildModular(metadata) ? metadata : null;
        }

        private static bool TryResolveFastenerRoot(Transform candidate, out Transform fastenerRoot)
        {
            fastenerRoot = null;
            if (candidate == null)
            {
                return false;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(candidate);

            if (marker == null || !IsPrimitiveFastenerMarker(marker))
            {
                return false;
            }

            Transform resolvedRoot = ResolveProxyRoot(marker);
            if (resolvedRoot == null)
            {
                return false;
            }

            fastenerRoot = resolvedRoot;
            return true;
        }

        private static FastenerRuntimeMarker ResolveFastenerMarker(Transform target)
        {
            if (target == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = target.GetComponent<FastenerRuntimeMarker>();
            if (marker != null)
            {
                return marker;
            }

            // Walk parents manually but STOP after checking the first ExplodablePart
            // boundary. This lets child meshes resolve their own fastener root without
            // capturing sibling markers on shared mother-part ancestors.
            Transform current = target.parent;
            while (current != null)
            {
                marker = current.GetComponent<FastenerRuntimeMarker>();
                if (marker != null)
                {
                    return marker;
                }

                if (current.GetComponent<ExplodablePart>() != null)
                {
                    break;
                }

                current = current.parent;
            }

            return null;
        }

        private static Transform ResolveProxyRoot(FastenerRuntimeMarker marker)
        {
            if (marker == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
            {
                Transform root = marker.transform;
                Transform current = root.parent;
                while (current != null)
                {
                    FastenerRuntimeMarker parentMarker = current.GetComponent<FastenerRuntimeMarker>();
                    if (parentMarker != null &&
                        string.Equals(parentMarker.FastenerInstanceId, marker.FastenerInstanceId, StringComparison.OrdinalIgnoreCase))
                    {
                        root = current;
                        current = current.parent;
                        continue;
                    }

                    if (current.GetComponent<ExplodablePart>() != null)
                    {
                        break;
                    }

                    current = current.parent;
                }

                return root;
            }

            ExplodablePart fastenerPart = marker.GetComponent<ExplodablePart>();
            if (fastenerPart == null)
            {
                fastenerPart = marker.GetComponentInParent<ExplodablePart>();
            }

            if (fastenerPart != null &&
                fastenerPart.Data != null &&
                fastenerPart.Data.category == PartCategory.Fasteners)
            {
                return fastenerPart.transform;
            }

            return marker.transform;
        }

        private static string ResolveCanonicalPartId(Transform target)
        {
            if (target == null)
            {
                return string.Empty;
            }

            ExplodablePart part = target.GetComponent<ExplodablePart>();
            if (part == null)
            {
                part = target.GetComponentInParent<ExplodablePart>();
            }

            return part != null && part.Data != null
                ? part.Data.id ?? string.Empty
                : string.Empty;
        }

        private void ShowInspection(Transform proxyRoot, FastenerMetadata metadata)
        {
            if (proxyRoot == null || metadata == null)
            {
                return;
            }

            ActiveInspection inspection = new ActiveInspection
            {
                ProxyRoot = proxyRoot,
                FastenerInstanceId = metadata.instanceId ?? string.Empty,
                NormalizedFastenerInstanceId = SelectionHierarchy.NormalizeToken(metadata.instanceId ?? string.Empty),
                NormalizedSceneObjectName = SelectionHierarchy.NormalizeToken(metadata.sceneObjectName ?? string.Empty)
            };

            DestroyExistingDetailRootsForInspection(proxyRoot, inspection.FastenerInstanceId);

            Renderer[] proxyRenderers = CollectProxyRenderersForInspection(
                proxyRoot,
                inspection.FastenerInstanceId,
                inspection.NormalizedFastenerInstanceId,
                inspection.NormalizedSceneObjectName);
            for (int i = 0; i < proxyRenderers.Length; i++)
            {
                Renderer renderer = proxyRenderers[i];
                if (renderer == null || renderer.transform == null || FastenerBuilder.IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                LodVisibilityUtility.ApplyRendererEnabled(renderer, false);
                inspection.HiddenProxyRenderers.Add(renderer);
            }

            inspection.DetailRoot = FastenerBuilder.BuildDetailVisual(proxyRoot, metadata);
            if (inspection.DetailRoot == null)
            {
                RestoreProxyRenderers(inspection);
                return;
            }

            ViewModeManager.Instance?.RegisterDynamicRendererRoot(inspection.DetailRoot);
            activeInspections[proxyRoot] = inspection;
            RefreshHighlightState(proxyRoot);
        }

        private bool HasActiveInspectionForInstanceId(string fastenerInstanceId)
        {
            if (string.IsNullOrWhiteSpace(fastenerInstanceId))
            {
                return false;
            }

            foreach (ActiveInspection inspection in activeInspections.Values)
            {
                if (inspection == null)
                {
                    continue;
                }

                if (string.Equals(inspection.FastenerInstanceId, fastenerInstanceId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void ClearInspection(Transform proxyRoot)
        {
            if (proxyRoot == null || !activeInspections.TryGetValue(proxyRoot, out ActiveInspection inspection))
            {
                return;
            }

            RestoreProxyRenderers(inspection);
            DestroyDetailRoot(inspection.DetailRoot);
            activeInspections.Remove(proxyRoot);
            RefreshHighlightState(proxyRoot);
        }

        private void ClearAllInspections()
        {
            if (activeInspections.Count == 0)
            {
                return;
            }

            List<Transform> proxyRoots = new List<Transform>(activeInspections.Keys);
            for (int i = 0; i < proxyRoots.Count; i++)
            {
                ClearInspection(proxyRoots[i]);
            }
        }

        private static void RestoreProxyRenderers(ActiveInspection inspection)
        {
            if (inspection == null)
            {
                return;
            }

            for (int i = 0; i < inspection.HiddenProxyRenderers.Count; i++)
            {
                Renderer renderer = inspection.HiddenProxyRenderers[i];
                if (renderer != null)
                {
                    LodVisibilityUtility.ApplyRendererEnabled(renderer, true);
                }
            }

            inspection.HiddenProxyRenderers.Clear();
        }

        public bool ShouldSuppressProxyRenderer(Renderer renderer)
        {
            if (renderer == null || activeInspections.Count == 0)
            {
                return false;
            }

            if (renderer.transform != null && FastenerBuilder.IsFastenerDetailTransform(renderer.transform))
            {
                return false;
            }

            FastenerRuntimeMarker rendererMarker = ResolveFastenerMarker(renderer.transform);
            foreach (ActiveInspection inspection in activeInspections.Values)
            {
                if (inspection == null)
                {
                    continue;
                }

                for (int i = 0; i < inspection.HiddenProxyRenderers.Count; i++)
                {
                    if (inspection.HiddenProxyRenderers[i] == renderer)
                    {
                        return true;
                    }
                }

                if (rendererMarker != null &&
                    !string.IsNullOrWhiteSpace(inspection.FastenerInstanceId) &&
                    string.Equals(rendererMarker.FastenerInstanceId, inspection.FastenerInstanceId, StringComparison.OrdinalIgnoreCase) &&
                    IsPrimitiveFastenerMarker(rendererMarker))
                {
                    return true;
                }

                if (RendererNameMatchesInspection(renderer.transform, inspection))
                {
                    return true;
                }
            }

            return false;
        }

        private static Renderer[] CollectProxyRenderersForInspection(
            Transform proxyRoot,
            string fastenerInstanceId,
            string normalizedFastenerInstanceId,
            string normalizedSceneObjectName)
        {
            HashSet<Renderer> collected = new HashSet<Renderer>();
            AddProxyRenderers(proxyRoot, collected);

            if (!string.IsNullOrWhiteSpace(fastenerInstanceId))
            {
                FastenerRuntimeMarker[] markers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);
                for (int i = 0; i < markers.Length; i++)
                {
                    FastenerRuntimeMarker marker = markers[i];
                    if (marker == null ||
                        !IsPrimitiveFastenerMarker(marker) ||
                        !string.Equals(marker.FastenerInstanceId, fastenerInstanceId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    AddProxyRenderers(ResolveProxyRoot(marker), collected);
                    AddProxyRenderers(marker.transform, collected);
                }
            }

            if (!string.IsNullOrWhiteSpace(normalizedFastenerInstanceId) ||
                !string.IsNullOrWhiteSpace(normalizedSceneObjectName))
            {
                Renderer[] sceneRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
                for (int i = 0; i < sceneRenderers.Length; i++)
                {
                    Renderer renderer = sceneRenderers[i];
                    if (renderer == null ||
                        renderer.transform == null ||
                        FastenerBuilder.IsFastenerDetailTransform(renderer.transform) ||
                        !RendererNameMatchesInspection(renderer.transform, normalizedFastenerInstanceId, normalizedSceneObjectName))
                    {
                        continue;
                    }

                    collected.Add(renderer);
                }
            }

            Renderer[] renderers = new Renderer[collected.Count];
            collected.CopyTo(renderers);
            return renderers;
        }

        private static bool RendererNameMatchesInspection(Transform transform, ActiveInspection inspection)
        {
            return inspection != null &&
                   RendererNameMatchesInspection(
                       transform,
                       inspection.NormalizedFastenerInstanceId,
                       inspection.NormalizedSceneObjectName);
        }

        private static bool RendererNameMatchesInspection(
            Transform transform,
            string normalizedFastenerInstanceId,
            string normalizedSceneObjectName)
        {
            Transform current = transform;
            int depth = 0;
            while (current != null && depth < 8)
            {
                string normalizedName = SelectionHierarchy.NormalizeToken(current.name);
                if (!string.IsNullOrWhiteSpace(normalizedName) &&
                    ((!string.IsNullOrWhiteSpace(normalizedFastenerInstanceId) &&
                      normalizedName.Contains(normalizedFastenerInstanceId)) ||
                     (!string.IsNullOrWhiteSpace(normalizedSceneObjectName) &&
                      normalizedName.Contains(normalizedSceneObjectName))))
                {
                    return true;
                }

                if (current.GetComponent<ExplodablePart>() != null)
                {
                    break;
                }

                current = current.parent;
                depth++;
            }

            return false;
        }

        private static void AddProxyRenderers(Transform root, HashSet<Renderer> collected)
        {
            if (root == null || collected == null)
            {
                return;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null ||
                    renderer.transform == null ||
                    FastenerBuilder.IsFastenerDetailTransform(renderer.transform))
                {
                    continue;
                }

                collected.Add(renderer);
            }
        }

        private static void DestroyDetailRoot(GameObject detailRoot)
        {
            if (detailRoot == null)
            {
                return;
            }

            ViewModeManager.Instance?.UnregisterDynamicRendererRoot(detailRoot);

            if (Application.isPlaying)
            {
                Destroy(detailRoot);
            }
            else
            {
                DestroyImmediate(detailRoot);
            }
        }

        private static void DestroyExistingDetailRoots(Transform proxyRoot)
        {
            if (proxyRoot == null)
            {
                return;
            }

            Transform[] children = proxyRoot.GetComponentsInChildren<Transform>(true);
            List<GameObject> staleDetails = new List<GameObject>();
            for (int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];
                if (child == null || child == proxyRoot)
                {
                    continue;
                }

                if (string.Equals(child.name, FastenerBuilder.DetailRootName, StringComparison.Ordinal))
                {
                    staleDetails.Add(child.gameObject);
                }
            }

            for (int i = 0; i < staleDetails.Count; i++)
            {
                DestroyDetailRoot(staleDetails[i]);
            }
        }

        private static void DestroyExistingDetailRootsForInspection(Transform proxyRoot, string fastenerInstanceId)
        {
            DestroyExistingDetailRoots(proxyRoot);

            if (string.IsNullOrWhiteSpace(fastenerInstanceId))
            {
                return;
            }

            FastenerRuntimeMarker[] markers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);
            for (int i = 0; i < markers.Length; i++)
            {
                FastenerRuntimeMarker marker = markers[i];
                if (marker == null ||
                    !IsPrimitiveFastenerMarker(marker) ||
                    !string.Equals(marker.FastenerInstanceId, fastenerInstanceId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DestroyExistingDetailRoots(ResolveProxyRoot(marker));
                DestroyExistingDetailRoots(marker.transform);
            }
        }

        private void RefreshFastenerCacheIfNeeded()
        {
            if (cacheInitialized)
            {
                if (cachedFastenerRoots.Count == 0)
                {
                    cacheInitialized = false;
                }

                bool hasNullRoot = false;
                for (int i = 0; i < cachedFastenerRoots.Count; i++)
                {
                    if (cachedFastenerRoots[i] == null)
                    {
                        hasNullRoot = true;
                        break;
                    }
                }

                if (!hasNullRoot)
                {
                    return;
                }

                cacheInitialized = false;
            }

            cachedFastenerRoots.Clear();
            fastenersByParentCanonical.Clear();

            FastenerRuntimeMarker[] markers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);
            HashSet<string> uniqueFastenerKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < markers.Length; i++)
            {
                FastenerRuntimeMarker marker = markers[i];
                if (marker == null || !IsPrimitiveFastenerMarker(marker))
                {
                    continue;
                }

                Transform proxyRoot = ResolveProxyRoot(marker);
                if (proxyRoot == null)
                {
                    continue;
                }

                string uniqueKey = !string.IsNullOrWhiteSpace(marker.FastenerInstanceId)
                    ? marker.FastenerInstanceId
                    : proxyRoot.GetInstanceID().ToString();

                if (!uniqueFastenerKeys.Add(uniqueKey))
                {
                    continue;
                }

                cachedFastenerRoots.Add(proxyRoot);

                if (!string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId))
                {
                    if (!fastenersByParentCanonical.TryGetValue(marker.ParentCanonicalPartId, out List<Transform> roots))
                    {
                        roots = new List<Transform>();
                        fastenersByParentCanonical[marker.ParentCanonicalPartId] = roots;
                    }

                    roots.Add(proxyRoot);
                }
            }

            cacheInitialized = true;
        }

        private static void RefreshHighlightState(Transform proxyRoot)
        {
            if (proxyRoot == null)
            {
                return;
            }

            HighlightSystem highlight = proxyRoot.GetComponent<HighlightSystem>();
            if (highlight != null)
            {
                highlight.RefreshVisualTargets();
                return;
            }

            MaterialController controller = proxyRoot.GetComponent<MaterialController>();
            controller?.RefreshRenderers();
        }

        private static bool IsPrimitiveFastenerMarker(FastenerRuntimeMarker marker)
        {
            return marker != null &&
                   marker.SourceIsPrimitiveFastener &&
                   SelectionHierarchy.IsPrimitiveFastenerSource(marker.transform);
        }
    }
}
