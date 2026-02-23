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
            EventBus.Subscribe<AppStateChangedEvent>(OnStateChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<AppStateChangedEvent>(OnStateChanged);
        }

        private void OnStateChanged(AppStateChangedEvent evt)
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
                
                // Sync State immediately
                if (currentMode != VisualMode.Normal)
                {
                    Material mat = GetMaterialForMode(currentMode);
                    if (mat != null)
                    {
                        part.SetXRay(true, mat);
                    }
                }
            }
        }

        public void UnregisterPart(ExplodablePart part)
        {
            parts.Remove(part);
        }

        // Shader Modes
        public enum VisualMode
        {
            Normal = 0,
            XRay = 1,
            Blueprint = 2,
            Thermal = 3,
            Wireframe = 4,
            SolidColor = 5,
            Ghosted = 6
        }

        private VisualMode currentMode = VisualMode.Normal;
        private Dictionary<VisualMode, Material> materials = new Dictionary<VisualMode, Material>();

        public VisualMode CurrentMode => currentMode;

        public void CycleVisualMode()
        {
            int next = (int)currentMode + 1;
            if (next > 6) next = 0; // Wrap around
            SetVisualMode((VisualMode)next);
        }

        public void SetVisualMode(VisualMode mode)
        {
            currentMode = mode;
            
            // Lazy Load Material
            Material targetMat = GetMaterialForMode(mode);

            foreach (var part in parts)
            {
                if (part != null)
                {
                    // If Normal, disable override (pass null or handle in SetXRay)
                    // We reuse SetXRay but it should probably be renamed SetOverrideMaterial in future, 
                    // but for now SetXRay(false, ...) resets to original.
                    // SetXRay(true, mat) sets the override.

                    if (mode == VisualMode.Normal)
                    {
                        part.SetXRay(false, null);
                    }
                    else
                    {
                        part.SetXRay(true, targetMat);
                    }
                }
            }
            
            // Notify UI or others if needed
            Debug.Log($"[ExplodedViewManager] Visual Mode: {mode}");
        }

        private Material GetMaterialForMode(VisualMode mode)
        {
            if (mode == VisualMode.Normal) return null;
            if (materials.ContainsKey(mode)) return materials[mode];

            string shaderName = "";
            switch (mode)
            {
                case VisualMode.XRay: shaderName = "WebGL/XRay"; break;
                case VisualMode.Blueprint: shaderName = "WebGL/Blueprint"; break;
                case VisualMode.Thermal: shaderName = "WebGL/Thermal"; break;
                case VisualMode.Wireframe: shaderName = "WebGL/WireframeWebGL"; break; // Use WebGL optimized version
                case VisualMode.SolidColor: shaderName = "WebGL/SolidColor"; break;
                case VisualMode.Ghosted: shaderName = "WebGL/Ghosted"; break;
            }

            if (string.IsNullOrEmpty(shaderName)) return null;

            Shader s = Shader.Find(shaderName);
            if (s == null) 
            {
                Debug.LogWarning($"Shader {shaderName} not found!");
                return null;
            }

            Material mat = new Material(s);
            materials[mode] = mat;
            return mat;
        }

        // Backward compatibility for UIManager if needed, or update UIManager
        public bool IsXRayEnabled => currentMode != VisualMode.Normal;
        
        // This is now legacy or can be mapped to SetVisualMode(XRay)
        // usage in RegisterPart needs update
    }
}
