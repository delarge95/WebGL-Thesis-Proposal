using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class PartVisibilityManager : Singleton<PartVisibilityManager>
    {
        [Header("Settings")]
        [SerializeField] private float fadeTransitionDuration = 0.3f;
#pragma warning disable CS0414 // Reserved for Inspector configuration
        [SerializeField] private float isolatedOpacity = 0.15f;
#pragma warning restore CS0414

        private Dictionary<ExplodablePart, bool> partVisibility = new Dictionary<ExplodablePart, bool>();
        private Dictionary<ExplodablePart, Material[]> originalMaterials = new Dictionary<ExplodablePart, Material[]>();
        private ExplodablePart isolatedPart = null;
        private Transform isolatedTransform = null;
        private bool isolatedGroup = false;
        private readonly List<ExplodablePart> storedGroupIsolation = new List<ExplodablePart>();
        private bool storedGroupIncludeAssociatedFasteners = false;
        private List<ExplodablePart> allParts = new List<ExplodablePart>();

        public bool HasAnyIsolationActive => isolatedPart != null || isolatedTransform != null || isolatedGroup;
        public bool IsGroupIsolationActive => isolatedGroup;
        public bool HasStoredGroupIsolation => storedGroupIsolation.Count > 0;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RebuildCache();
        }

        public void RebuildCache()
        {
            allParts.Clear();
            partVisibility.Clear();
            originalMaterials.Clear();
            isolatedPart = null;
            isolatedTransform = null;
            isolatedGroup = false;
            storedGroupIsolation.Clear();
            storedGroupIncludeAssociatedFasteners = false;

            allParts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));
            foreach (var part in allParts)
            {
                partVisibility[part] = true;
                var renderers = part.GetComponentsInChildren<Renderer>(true);
                if (renderers != null && renderers.Length > 0)
                {
                    originalMaterials[part] = renderers[0].sharedMaterials;
                }
            }
        }

        public void HidePart(ExplodablePart part)
        {
            if (part == null) return;
            partVisibility[part] = false;
            StartCoroutine(FadePartOut(part));
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public void ShowPart(ExplodablePart part)
        {
            if (part == null) return;
            partVisibility[part] = true;
            part.gameObject.SetActive(true);
            StartCoroutine(FadePartIn(part));

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public void TogglePartVisibility(ExplodablePart part)
        {
            if (partVisibility.TryGetValue(part, out bool visible))
            {
                if (visible) HidePart(part);
                else ShowPart(part);
            }
        }

        public void IsolatePart(ExplodablePart part)
        {
            if (part == null)
            {
                ClearIsolation();
                return;
            }
            IsolateTransform(part.transform);
        }

        public void IsolateTransform(Transform selection)
        {
            if (selection == null)
            {
                ClearIsolation();
                return;
            }

            Transform isolationScope = ResolveIsolationScope(selection);
            ExplodablePart parentPart = isolationScope != null
                ? isolationScope.GetComponent<ExplodablePart>()
                : null;
            if (parentPart == null && isolationScope != null)
            {
                parentPart = isolationScope.GetComponentInParent<ExplodablePart>();
            }

            if (parentPart == null)
            {
                Debug.LogWarning($"[PVM.IsolateTransform] ABORT: no parentPart found. selection={selection.name}, isolationScope={(isolationScope != null ? isolationScope.name : "NULL")}");
                ClearIsolation();
                return;
            }

            isolatedPart = parentPart;
            isolatedTransform = isolationScope != null ? isolationScope : selection;
            isolatedGroup = false;
            bool isFastenerIsolation = TryResolveFastenerInstanceId(isolatedTransform, out string isolatedFastenerInstanceId);
            bool includeAssociatedFasteners = !isFastenerIsolation
                && IsFullPartIsolationScope(isolatedTransform)
                && !string.IsNullOrWhiteSpace(ResolveCanonicalPartId(isolatedTransform));
            string selectedCanonicalPartId = includeAssociatedFasteners
                ? ResolveCanonicalPartId(isolatedTransform)
                : string.Empty;

            // ──── DIAGNOSTIC BLOCK ────
            Debug.Log($"[PVM.IsolateTransform] ──── DIAGNOSTIC ────");
            Debug.Log($"  selection.name         = {selection.name}");
            Debug.Log($"  selection.path         = {GetHierarchyPath(selection)}");
            Debug.Log($"  isolationScope.name    = {(isolationScope != null ? isolationScope.name : "NULL")}");
            Debug.Log($"  isolationScope.path    = {(isolationScope != null ? GetHierarchyPath(isolationScope) : "NULL")}");
            Debug.Log($"  parentPart.name        = {parentPart.name}");
            Debug.Log($"  isFastenerIsolation    = {isFastenerIsolation}");
            Debug.Log($"  fastenerInstanceId     = '{isolatedFastenerInstanceId}'");
            Debug.Log($"  includeAssocFasteners  = {includeAssociatedFasteners}");
            Debug.Log($"  selectedCanonicalPart  = '{selectedCanonicalPartId}'");

            // Check marker on selection directly
            FastenerRuntimeMarker selMarker = selection.GetComponent<FastenerRuntimeMarker>();
            FastenerRuntimeMarker scopeMarker = isolationScope != null ? isolationScope.GetComponent<FastenerRuntimeMarker>() : null;
            Debug.Log($"  selection has marker?   = {selMarker != null} {(selMarker != null ? $"id='{selMarker.FastenerInstanceId}'" : "")}");
            Debug.Log($"  scope has marker?       = {scopeMarker != null} {(scopeMarker != null ? $"id='{scopeMarker.FastenerInstanceId}'" : "")}");
            // ──── END DIAGNOSTIC ────

            bool anyRendererVisible = false;
            int visibleCount = 0;
            int totalRenderers = 0;

            foreach (var p in allParts)
            {
                if (p == null) continue;
                p.gameObject.SetActive(true);

                var renderers = p.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    totalRenderers++;

                    bool visible = IsRendererVisibleForIsolation(
                        renderer,
                        isolatedTransform,
                        isolatedFastenerInstanceId,
                        selectedCanonicalPartId);

                    renderer.enabled = visible;
                    anyRendererVisible |= visible;

                    if (visible)
                    {
                        visibleCount++;
                        // Log WHY this renderer is visible
                        Transform rt = renderer.transform;
                        bool byScope = rt == isolatedTransform || rt.IsChildOf(isolatedTransform);
                        bool byInstance = !string.IsNullOrWhiteSpace(isolatedFastenerInstanceId) && RendererBelongsToFastenerInstance(rt, isolatedFastenerInstanceId);
                        bool byAssoc = !string.IsNullOrWhiteSpace(selectedCanonicalPartId) && RendererBelongsToAssociatedFastener(rt, selectedCanonicalPartId);
                        FastenerRuntimeMarker rm = rt.GetComponent<FastenerRuntimeMarker>();
                        Debug.Log($"  [VISIBLE] {renderer.name} path={GetHierarchyPath(rt)} byScope={byScope} byInstance={byInstance} byAssoc={byAssoc} marker={(rm != null ? rm.FastenerInstanceId : "none")}");
                    }

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = visible;
                    }
                }
            }

            Debug.Log($"[PVM.IsolateTransform] Result: {visibleCount}/{totalRenderers} renderers visible");

            if (!anyRendererVisible)
            {
                Debug.LogWarning("[PVM.IsolateTransform] No renderers visible — falling back to IsolatePart");
                IsolatePart(parentPart);
                return;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[PartVisibility] Isolated transform: {selection.name}");
        }

        private static string GetHierarchyPath(Transform t)
        {
            if (t == null) return "null";
            string path = t.name;
            Transform p = t.parent;
            int depth = 0;
            while (p != null && depth < 6)
            {
                path = p.name + "/" + path;
                p = p.parent;
                depth++;
            }
            return path;
        }

        private static Transform ResolveIsolationScope(Transform selection)
        {
            if (selection == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);

            if (marker != null)
            {
                // Always return the marker's own transform — never escalate to the
                // parent ExplodablePart (fastener group). That group contains ALL sibling
                // fasteners and would cause IsChildOf to match them all, defeating
                // instance-level isolation.
                return marker.transform;
            }

            return selection;
        }

        private static bool IsRendererVisibleForIsolation(
            Renderer renderer,
            Transform isolationScope,
            string fastenerInstanceId,
            string selectedCanonicalPartId)
        {
            if (renderer == null || isolationScope == null)
            {
                return false;
            }

            Transform rendererTransform = renderer.transform;
            if (rendererTransform == isolationScope ||
                rendererTransform.IsChildOf(isolationScope))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(fastenerInstanceId) &&
                RendererBelongsToFastenerInstance(rendererTransform, fastenerInstanceId))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(selectedCanonicalPartId) &&
                RendererBelongsToAssociatedFastener(rendererTransform, selectedCanonicalPartId))
            {
                return true;
            }

            return false;
        }

        private static bool RendererBelongsToFastenerInstance(Transform rendererTransform, string fastenerInstanceId)
        {
            if (rendererTransform == null || string.IsNullOrWhiteSpace(fastenerInstanceId))
            {
                return false;
            }

            // ONLY use GetComponent — never GetComponentInParent.
            // After reparenting, ancestor markers belong to SIBLING fasteners
            // and cause multi-instance leaks when isolating a single fastener.
            FastenerRuntimeMarker rendererMarker = rendererTransform.GetComponent<FastenerRuntimeMarker>();

            return rendererMarker != null &&
                   string.Equals(rendererMarker.FastenerInstanceId, fastenerInstanceId, StringComparison.OrdinalIgnoreCase);
        }

        private static bool RendererBelongsToAssociatedFastener(Transform rendererTransform, string selectedCanonicalPartId)
        {
            if (rendererTransform == null || string.IsNullOrWhiteSpace(selectedCanonicalPartId))
            {
                return false;
            }

            // Direct marker only — ancestor markers cause sibling contamination.
            FastenerRuntimeMarker rendererMarker = rendererTransform.GetComponent<FastenerRuntimeMarker>();

            return rendererMarker != null &&
                   !string.IsNullOrWhiteSpace(rendererMarker.ParentCanonicalPartId) &&
                   string.Equals(rendererMarker.ParentCanonicalPartId, selectedCanonicalPartId, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryResolveFastenerInstanceId(Transform selection, out string fastenerInstanceId)
        {
            fastenerInstanceId = string.Empty;
            if (selection == null)
            {
                return false;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);

            if (marker == null || string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
            {
                return false;
            }

            fastenerInstanceId = marker.FastenerInstanceId;
            return true;
        }

        private static bool IsFullPartIsolationScope(Transform selection)
        {
            if (selection == null)
            {
                return false;
            }

            ExplodablePart directPart = selection.GetComponent<ExplodablePart>();
            return directPart != null && directPart.transform == selection;
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

            // Walk parents manually but STOP at the first ExplodablePart boundary.
            // This prevents capturing a sibling fastener's marker on a shared ancestor.
            Transform current = target.parent;
            while (current != null)
            {
                if (current.GetComponent<ExplodablePart>() != null)
                {
                    break;
                }

                marker = current.GetComponent<FastenerRuntimeMarker>();
                if (marker != null)
                {
                    return marker;
                }

                current = current.parent;
            }

            return null;
        }

        private static string ResolveCanonicalPartId(Transform selection)
        {
            if (selection == null)
            {
                return string.Empty;
            }

            PartRenderCategory directCategory = selection.GetComponent<PartRenderCategory>();
            if (directCategory != null && !string.IsNullOrWhiteSpace(directCategory.CanonicalPartId))
            {
                return directCategory.CanonicalPartId;
            }

            ExplodablePart directPart = selection.GetComponent<ExplodablePart>();
            if (directPart != null && directPart.Data != null && !string.IsNullOrWhiteSpace(directPart.Data.id))
            {
                return directPart.Data.id;
            }

            PartRenderCategory childCategory = selection.GetComponentInChildren<PartRenderCategory>(true);
            if (childCategory != null && !string.IsNullOrWhiteSpace(childCategory.CanonicalPartId))
            {
                return childCategory.CanonicalPartId;
            }

            ExplodablePart parentPart = selection.GetComponentInParent<ExplodablePart>();
            if (parentPart != null && parentPart.Data != null && !string.IsNullOrWhiteSpace(parentPart.Data.id))
            {
                return parentPart.Data.id;
            }

            return string.Empty;
        }

        public void IsolateParts(IEnumerable<ExplodablePart> parts, bool includeAssociatedFasteners = false)
        {
            if (parts == null)
            {
                ClearIsolation();
                return;
            }

            HashSet<ExplodablePart> targets = new HashSet<ExplodablePart>();
            foreach (ExplodablePart part in parts)
            {
                if (part != null)
                {
                    targets.Add(part);
                }
            }

            if (targets.Count == 0)
            {
                ClearIsolation();
                return;
            }

            storedGroupIsolation.Clear();
            storedGroupIsolation.AddRange(targets);
            storedGroupIncludeAssociatedFasteners = includeAssociatedFasteners;

            isolatedPart = null;
            isolatedTransform = null;
            isolatedGroup = true;

            HashSet<string> targetCanonicalPartIds = includeAssociatedFasteners
                ? CollectCanonicalPartIds(targets)
                : null;

            foreach (var p in allParts)
            {
                if (p == null)
                {
                    continue;
                }

                bool isTargetPart = targets.Contains(p);
                bool hasAssociatedFastenerRenderer = includeAssociatedFasteners &&
                                                    PartContainsAssociatedFastenerRenderers(p, targetCanonicalPartIds);
                bool shouldKeepPartActive = isTargetPart || hasAssociatedFastenerRenderer;

                if (shouldKeepPartActive)
                {
                    p.gameObject.SetActive(true);

                    var renderers = p.GetComponentsInChildren<Renderer>(true);
                    foreach (var renderer in renderers)
                    {
                        if (renderer == null) continue;
                        bool rendererVisible = isTargetPart ||
                                               (includeAssociatedFasteners &&
                                                RendererBelongsToAssociatedFastener(renderer.transform, targetCanonicalPartIds));

                        renderer.enabled = rendererVisible;

                        Collider collider = renderer.GetComponent<Collider>();
                        if (collider != null)
                        {
                            collider.enabled = rendererVisible;
                        }
                    }
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[PartVisibility] Isolated group with {targets.Count} parts");
        }

        public void RestoreStoredGroupIsolation()
        {
            if (storedGroupIsolation.Count == 0)
            {
                return;
            }

            IsolateParts(storedGroupIsolation, storedGroupIncludeAssociatedFasteners);
        }

        public void ClearStoredGroupIsolation()
        {
            storedGroupIsolation.Clear();
            storedGroupIncludeAssociatedFasteners = false;
        }

        public void ClearIsolation()
        {
            isolatedPart = null;
            isolatedTransform = null;
            isolatedGroup = false;
            storedGroupIncludeAssociatedFasteners = false;

            foreach (var p in allParts)
            {
                // Restore all parts
                p.gameObject.SetActive(true);
                var renderers = p.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    renderer.enabled = true;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }

                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    renderer.SetPropertyBlock(block);
                }
            }

            Debug.Log("[PartVisibility] Isolation cleared");
        }

        private void SetPartOpacity(ExplodablePart part, float opacity)
        {
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                StartCoroutine(AnimateOpacity(renderer, opacity));
            }
        }

        private System.Collections.IEnumerator AnimateOpacity(Renderer renderer, float targetOpacity)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            
            Color currentColor = block.GetColor("_BaseColor");
            if (currentColor == Color.clear) currentColor = Color.white;
            
            float startOpacity = currentColor.a;
            float timer = 0f;

            while (timer < fadeTransitionDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeTransitionDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                float opacity = Mathf.Lerp(startOpacity, targetOpacity, t);
                currentColor.a = opacity;
                block.SetColor("_BaseColor", currentColor);
                renderer.SetPropertyBlock(block);

                yield return null;
            }

            currentColor.a = targetOpacity;
            block.SetColor("_BaseColor", currentColor);
            renderer.SetPropertyBlock(block);
        }

        private System.Collections.IEnumerator FadePartOut(ExplodablePart part)
        {
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            bool animatedAny = false;
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                animatedAny = true;
                yield return AnimateOpacity(renderer, 0f);
            }
            if (!animatedAny)
            {
                yield return null;
            }
            part.gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator FadePartIn(ExplodablePart part)
        {
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                yield return AnimateOpacity(renderer, 1f);
            }
        }

        public void ShowAllParts()
        {
            foreach (var part in allParts)
            {
                ShowPart(part);
            }
            ClearIsolation();
        }

        public void HideAllParts()
        {
            foreach (var part in allParts)
            {
                HidePart(part);
            }
        }

        public bool IsPartVisible(ExplodablePart part)
        {
            return partVisibility.TryGetValue(part, out bool visible) && visible;
        }

        public ExplodablePart GetIsolatedPart() => isolatedPart;
        public Transform GetIsolatedTransform() => isolatedTransform;

        /// <summary>
        /// Re-applies the current isolation state without mutating the isolation stack,
        /// emitting diagnostics, or replaying audio. This is used when another runtime
        /// system (for example FastenerInspectionManager) temporarily toggles renderer
        /// visibility and we need PartVisibilityManager to become authoritative again.
        /// </summary>
        public void ReapplyCurrentIsolationVisuals()
        {
            if (isolatedGroup)
            {
                ReapplyStoredGroupIsolationVisuals();
                return;
            }

            if (isolatedTransform != null)
            {
                ReapplyTransformIsolationVisuals();
                return;
            }

            RestoreAllRendererVisuals();
        }

        private static HashSet<string> CollectCanonicalPartIds(IEnumerable<ExplodablePart> parts)
        {
            HashSet<string> canonicalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (parts == null)
            {
                return canonicalIds;
            }

            foreach (ExplodablePart part in parts)
            {
                if (part == null)
                {
                    continue;
                }

                string canonicalId = ResolveCanonicalPartId(part.transform);
                if (!string.IsNullOrWhiteSpace(canonicalId))
                {
                    canonicalIds.Add(canonicalId);
                }
            }

            return canonicalIds;
        }

        private static bool PartContainsAssociatedFastenerRenderers(ExplodablePart part, HashSet<string> targetCanonicalPartIds)
        {
            if (part == null || targetCanonicalPartIds == null || targetCanonicalPartIds.Count == 0)
            {
                return false;
            }

            Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer != null && RendererBelongsToAssociatedFastener(renderer.transform, targetCanonicalPartIds))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RendererBelongsToAssociatedFastener(Transform rendererTransform, HashSet<string> targetCanonicalPartIds)
        {
            if (rendererTransform == null || targetCanonicalPartIds == null || targetCanonicalPartIds.Count == 0)
            {
                return false;
            }

            // Direct marker only — ancestor markers cause sibling contamination.
            FastenerRuntimeMarker rendererMarker = rendererTransform.GetComponent<FastenerRuntimeMarker>();

            return rendererMarker != null &&
                   !string.IsNullOrWhiteSpace(rendererMarker.ParentCanonicalPartId) &&
                   targetCanonicalPartIds.Contains(rendererMarker.ParentCanonicalPartId);
        }

        private void ReapplyTransformIsolationVisuals()
        {
            if (isolatedTransform == null)
            {
                RestoreAllRendererVisuals();
                return;
            }

            bool isFastenerIsolation = TryResolveFastenerInstanceId(isolatedTransform, out string isolatedFastenerInstanceId);
            bool includeAssociatedFasteners = !isFastenerIsolation
                && IsFullPartIsolationScope(isolatedTransform)
                && !string.IsNullOrWhiteSpace(ResolveCanonicalPartId(isolatedTransform));
            string selectedCanonicalPartId = includeAssociatedFasteners
                ? ResolveCanonicalPartId(isolatedTransform)
                : string.Empty;

            foreach (ExplodablePart part in allParts)
            {
                if (part == null)
                {
                    continue;
                }

                part.gameObject.SetActive(true);

                Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    Renderer renderer = renderers[i];
                    if (renderer == null)
                    {
                        continue;
                    }

                    bool visible = IsRendererVisibleForIsolation(
                        renderer,
                        isolatedTransform,
                        isolatedFastenerInstanceId,
                        selectedCanonicalPartId);

                    renderer.enabled = visible;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = visible;
                    }
                }
            }
        }

        private void ReapplyStoredGroupIsolationVisuals()
        {
            if (storedGroupIsolation.Count == 0)
            {
                RestoreAllRendererVisuals();
                return;
            }

            HashSet<ExplodablePart> targets = new HashSet<ExplodablePart>();
            for (int i = 0; i < storedGroupIsolation.Count; i++)
            {
                ExplodablePart part = storedGroupIsolation[i];
                if (part != null)
                {
                    targets.Add(part);
                }
            }

            if (targets.Count == 0)
            {
                RestoreAllRendererVisuals();
                return;
            }

            HashSet<string> targetCanonicalPartIds = storedGroupIncludeAssociatedFasteners
                ? CollectCanonicalPartIds(targets)
                : null;

            foreach (ExplodablePart part in allParts)
            {
                if (part == null)
                {
                    continue;
                }

                bool isTargetPart = targets.Contains(part);
                bool hasAssociatedFastenerRenderer = storedGroupIncludeAssociatedFasteners &&
                                                    PartContainsAssociatedFastenerRenderers(part, targetCanonicalPartIds);
                bool shouldKeepPartActive = isTargetPart || hasAssociatedFastenerRenderer;

                if (shouldKeepPartActive)
                {
                    part.gameObject.SetActive(true);

                    Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Renderer renderer = renderers[i];
                        if (renderer == null)
                        {
                            continue;
                        }

                        bool rendererVisible = isTargetPart ||
                                               (storedGroupIncludeAssociatedFasteners &&
                                                RendererBelongsToAssociatedFastener(renderer.transform, targetCanonicalPartIds));

                        renderer.enabled = rendererVisible;

                        Collider collider = renderer.GetComponent<Collider>();
                        if (collider != null)
                        {
                            collider.enabled = rendererVisible;
                        }
                    }
                }
                else
                {
                    part.gameObject.SetActive(false);
                }
            }
        }

        private void RestoreAllRendererVisuals()
        {
            foreach (ExplodablePart part in allParts)
            {
                if (part == null)
                {
                    continue;
                }

                part.gameObject.SetActive(true);

                Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    Renderer renderer = renderers[i];
                    if (renderer == null)
                    {
                        continue;
                    }

                    renderer.enabled = true;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }
    }
}
