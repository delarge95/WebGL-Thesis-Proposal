using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;

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
            parts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));
            
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
                SetExplosionFactor(0.5f);
                AudioManager.Instance?.PlayExplosionSound();
            }
            else
            {
                SetExplosionFactor(0f);
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

        public void SetCategoryFilters(List<string> activeCategories)
        {
            foreach (var part in parts)
            {
                if (part == null) continue;

                if (activeCategories == null || activeCategories.Count == 0 || activeCategories.Contains("ALL"))
                {
                    part.gameObject.SetActive(true);
                }
                else
                {
                    // Case-insensitive check against multiple categories
                    bool match = false;
                    if (part.Data != null)
                    {
                        foreach (var cat in activeCategories)
                        {
                            if (part.Data.category.Equals(cat, System.StringComparison.OrdinalIgnoreCase))
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    part.gameObject.SetActive(match);
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
