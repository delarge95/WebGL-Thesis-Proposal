using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using System;

namespace WebGL.Core.Content
{
    public class ExplodedViewManager : Singleton<ExplodedViewManager>
    {
        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float explosionFactor = 0f;
        [SerializeField] private float animationSpeed = 2f;
        [SerializeField] private bool useEasing = true;

        private List<ExplodablePart> parts = new List<ExplodablePart>();
        private float currentFactor = 0f;
        private float targetFactor = 0f;
        private Coroutine _animCoroutine;

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
                if (part != null)
                {
                    part.UpdateExplosion(currentFactor);
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
        }
    }
}
