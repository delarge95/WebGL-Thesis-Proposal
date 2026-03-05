using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    public enum ViewMode
    {
        Realistic,
        XRay,
        Blueprint,
        SolidColor,
        Wireframe,
        Ghosted,
        Thermal
    }

    [Serializable]
    public class ViewModeConfig
    {
        public ViewMode mode;
        public Material material;
        public Color primaryColor;
        public Color secondaryColor;
        public float opacity;
        public bool showEdges;
    }

    public class ViewModeManager : Singleton<ViewModeManager>
    {
        [Header("Materials")]
        [SerializeField] private Material xRayMaterial;
        [SerializeField] private Material blueprintMaterial;
        [SerializeField] private Material solidColorMaterial;
        [SerializeField] private Material wireframeMaterial;
        [SerializeField] private Material ghostedMaterial;
        [SerializeField] private Material thermalMaterial;

        [Header("Settings")]
        [SerializeField] private ViewMode currentMode = ViewMode.Realistic;
        [SerializeField] private Color solidColor = new Color(0.3f, 0.6f, 1f);


        [Header("X-Ray Settings")]
        [SerializeField] private Color xRayColor = new Color(0f, 1f, 0.8f, 0.5f);
        [SerializeField] private float xRayFresnelPower = 2f;

        [Header("Blueprint Settings")]
        [SerializeField] private Color blueprintLineColor = new Color(0.85f, 0.9f, 1f);
        [SerializeField] private Color blueprintBgColor = new Color(0.08f, 0.18f, 0.38f);
        [SerializeField] private float blueprintGridSize = 0.1f;

        private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
        private List<Renderer> allRenderers = new List<Renderer>();
        private bool isTransitioning = false;

        public ViewMode CurrentMode => currentMode;

        /// <summary>
        /// The fallback mode to return to when a temporary shader override is toggled off.
        /// Set by UIEnvironmentPanel when Blueprint env is active.
        /// </summary>
        public ViewMode BaseMode { get; set; } = ViewMode.Realistic;

        public event Action<ViewMode> OnModeChanged;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            CacheRenderers();
            CreateDefaultMaterials();
        }

        private void CacheRenderers()
        {
            // Find all part renderers
            var parts = FindObjectsByType<WebGL.Core.Content.ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                var renderer = part.GetComponent<Renderer>();
                if (renderer != null)
                {
                    allRenderers.Add(renderer);
                    originalMaterials[renderer] = renderer.sharedMaterials;
                }
            }
        }

        private void CreateDefaultMaterials()
        {
            // Create materials using custom shaders
            if (xRayMaterial == null)
            {
                var shader = Shader.Find("WebGL/XRay");
                if (shader != null)
                {
                    xRayMaterial = new Material(shader);
                    xRayMaterial.name = "XRay";
                    xRayMaterial.SetColor("_BaseColor", xRayColor);
                    xRayMaterial.SetFloat("_FresnelPower", xRayFresnelPower);
                }
                else
                {
                    xRayMaterial = CreateFallbackMaterial("XRay", xRayColor);
                }
            }

            if (solidColorMaterial == null)
            {
                var shader = Shader.Find("WebGL/SolidColor");
                if (shader != null)
                {
                    solidColorMaterial = new Material(shader);
                    solidColorMaterial.name = "SolidColor";
                    solidColorMaterial.SetColor("_BaseColor", solidColor);
                }
                else
                {
                    solidColorMaterial = CreateFallbackMaterial("SolidColor", solidColor);
                }
            }

            if (blueprintMaterial == null)
            {
                var shader = Shader.Find("WebGL/Blueprint");
                if (shader != null)
                {
                    blueprintMaterial = new Material(shader);
                    blueprintMaterial.name = "Blueprint";
                    blueprintMaterial.SetColor("_LineColor", blueprintLineColor);
                    blueprintMaterial.SetColor("_BackgroundColor", blueprintBgColor);
                    blueprintMaterial.SetFloat("_GridScale", blueprintGridSize * 100f);
                }
                else
                {
                    blueprintMaterial = CreateFallbackMaterial("Blueprint", blueprintLineColor);
                }
            }

            if (ghostedMaterial == null)
            {
                var shader = Shader.Find("WebGL/Ghosted");
                if (shader != null)
                {
                    ghostedMaterial = new Material(shader);
                    ghostedMaterial.name = "Ghosted";
                    ghostedMaterial.SetColor("_BaseColor", new Color(0.5f, 0.7f, 1f, 0.15f));
                }
                else
                {
                    ghostedMaterial = CreateFallbackMaterial("Ghosted", new Color(1, 1, 1, 0.2f));
                }
            }

            if (wireframeMaterial == null)
            {
                var shader = Shader.Find("WebGL/Wireframe");
                if (shader != null)
                {
                    wireframeMaterial = new Material(shader);
                    wireframeMaterial.name = "Wireframe";
                    wireframeMaterial.SetColor("_WireColor", new Color(0.3f, 0.8f, 1f, 1f));
                }
                else
                {
                    wireframeMaterial = CreateFallbackMaterial("Wireframe", Color.white);
                }
            }

            if (thermalMaterial == null)
            {
                var shader = Shader.Find("WebGL/Thermal");
                if (shader != null)
                {
                    thermalMaterial = new Material(shader);
                    thermalMaterial.name = "Thermal";
                }
                else
                {
                    thermalMaterial = CreateFallbackMaterial("Thermal", Color.red);
                }
            }
        }

        private Material CreateFallbackMaterial(string name, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var mat = new Material(shader);
            mat.name = name + "_Fallback";
            // Use OPAQUE rendering — transparent causes invisible objects
            Color opaqueColor = new Color(color.r, color.g, color.b, 1f);
            mat.SetColor("_BaseColor", opaqueColor);
            mat.color = opaqueColor;
            // Keep Surface=0 (Opaque), default blend, ZWrite on
            return mat;
        }

        public void SetViewMode(ViewMode mode)
        {
            if (isTransitioning) return;
            if (currentMode == mode) return;

            // Lazy re-cache if renderers were not found at Start()
            if (allRenderers.Count == 0)
            {
                CacheRenderers();
                CreateDefaultMaterials();
            }

            ViewMode previousMode = currentMode;
            currentMode = mode;

            StartCoroutine(TransitionToMode(mode));

            OnModeChanged?.Invoke(mode);
            EventBus.Publish(new ViewModeChangedEvent(mode != ViewMode.Realistic));

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[ViewModeManager] Switched to: {mode}");
        }

        private System.Collections.IEnumerator TransitionToMode(ViewMode mode)
        {
            isTransitioning = true;

            // Apply materials directly — no fade.
            // The previous FadeRenderers used MaterialPropertyBlock.GetColor("_BaseColor")
            // which returns (0,0,0,0) for blocks that haven't been explicitly set,
            // causing all renderers to turn invisible/black.
            ApplyModeToRenderers(mode);

            // Brief yield to allow visual update
            yield return null;

            isTransitioning = false;
        }

        private void ApplyModeToRenderers(ViewMode mode)
        {
            foreach (var renderer in allRenderers)
            {
                if (renderer == null) continue;

                Material[] materials;
                switch (mode)
                {
                    case ViewMode.Realistic:
                        // Restore original materials AND clear property block overrides
                        if (originalMaterials.TryGetValue(renderer, out materials))
                        {
                            renderer.materials = materials;
                        }
                        renderer.SetPropertyBlock(null); // Clear any _BaseColor overrides
                        break;

                    case ViewMode.XRay:
                        ApplyMaterialToRenderer(renderer, xRayMaterial, xRayColor);
                        break;

                    case ViewMode.Blueprint:
                        ApplyMaterialToRenderer(renderer, blueprintMaterial, blueprintLineColor);
                        break;

                    case ViewMode.SolidColor:
                        ApplyMaterialToRenderer(renderer, solidColorMaterial, solidColor);
                        break;

                    case ViewMode.Wireframe:
                        ApplyMaterialToRenderer(renderer, wireframeMaterial, Color.white);
                        break;

                    case ViewMode.Ghosted:
                        ApplyMaterialToRenderer(renderer, ghostedMaterial, new Color(1, 1, 1, 0.3f));
                        break;

                    case ViewMode.Thermal:
                        ApplyMaterialToRenderer(renderer, thermalMaterial, Color.red);
                        break;
                }
            }
        }

        private void ApplyMaterialToRenderer(Renderer renderer, Material material, Color color)
        {
            if (material == null) return;

            int count = renderer.materials.Length;
            Material[] mats = new Material[count];
            for (int i = 0; i < count; i++)
            {
                mats[i] = material;
            }
            renderer.materials = mats;

            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(block);
        }

        public void SetSolidColor(Color color)
        {
            solidColor = color;
            if (currentMode == ViewMode.SolidColor)
            {
                ApplyModeToRenderers(ViewMode.SolidColor);
            }
        }

        public void ResetToRealistic()
        {
            SetViewMode(ViewMode.Realistic);
        }

        public void CycleViewMode()
        {
            int current = (int)currentMode;
            int next = (current + 1) % Enum.GetValues(typeof(ViewMode)).Length;
            SetViewMode((ViewMode)next);
        }
    }
}
