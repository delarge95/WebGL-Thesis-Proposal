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
        // private bool isAnimating = false;

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
                // Auto-expand to default
                SetExplosionFactor(0.5f);
                AudioManager.Instance?.PlayExplosionSound();
            }
            else
            {
                // Collapse
                SetExplosionFactor(0f);
            }
        }

        private void Update()
        {
            // Get current state
            AppState currentState = AppStateMachine.Instance != null 
                ? AppStateMachine.Instance.CurrentState 
                : AppState.Exploration;

            // Calculate target
            if (currentState == AppState.ExplodedView)
            {
                targetFactor = explosionFactor;
            }
            else
            {
                targetFactor = 0f;
            }

            // Smoothly interpolate
            if (Mathf.Abs(currentFactor - targetFactor) > 0.001f)
            {
                if (useEasing)
                {
                    // Smooth easing
                    currentFactor = Mathf.Lerp(currentFactor, targetFactor, Time.deltaTime * animationSpeed);
                }
                else
                {
                    // Linear
                    currentFactor = Mathf.MoveTowards(currentFactor, targetFactor, Time.deltaTime * animationSpeed * 0.5f);
                }

                UpdateAllParts();
            }
            else if (currentFactor != targetFactor)
            {
                currentFactor = targetFactor;
                UpdateAllParts();
            }
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
                            if (part.Data.Category.Equals(cat, System.StringComparison.OrdinalIgnoreCase))
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
