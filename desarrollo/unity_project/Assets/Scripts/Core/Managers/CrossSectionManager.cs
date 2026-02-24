using UnityEngine;
using WebGL.Core.Utils;
using System.Collections.Generic;

namespace WebGL.Core.Managers
{
    public enum CrossSectionAxis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// Manages cross-section clipping of all renderers.
    /// When enabled, swaps all part materials to ClippableLit with _CLIP_ENABLED keyword,
    /// and sets global shader properties for the clip plane.
    /// When disabled, restores original materials.
    /// </summary>
    public class CrossSectionManager : Singleton<CrossSectionManager>
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled = false;
        [SerializeField] private CrossSectionAxis axis = CrossSectionAxis.Y;
        [SerializeField] private float position = 0f;
        [SerializeField] private bool invertDirection = false;

        [Header("Visuals")]
        [SerializeField] private Color planeColor = new Color(1f, 0.5f, 0f, 0.3f);
        [SerializeField] private bool showPlane = true;
        [SerializeField] private Color clipEdgeColor = new Color(1f, 0.5f, 0f, 1f);
        [SerializeField] private float clipEdgeWidth = 0.005f;

        [Header("References")]
        [SerializeField] private Transform targetObject;

        // ── Cached renderers & material backup ──
        private List<Renderer> partRenderers = new List<Renderer>();
        private Dictionary<Renderer, Material[]> savedMaterials = new Dictionary<Renderer, Material[]>();

        private Vector4 clipPlane;
        private GameObject planeVisual;
        private Shader clippableShader;

        // Global property IDs (must match ClippableLit.shader globals)
        private static readonly int GlobalClipPlaneId = Shader.PropertyToID("_GlobalClipPlane");
        private static readonly int GlobalClipEnabledId = Shader.PropertyToID("_GlobalClipEnabled");

        public bool IsEnabled => isEnabled;
        public float Position => position;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            clippableShader = Shader.Find("WebGL/ClippableLit");
            if (clippableShader == null)
                Debug.LogError("[CrossSectionManager] WebGL/ClippableLit shader not found!");

            CacheRenderers();
            CreatePlaneVisual();
            UpdateClipPlane();
        }

        private void CacheRenderers()
        {
            partRenderers.Clear();

            if (targetObject == null)
            {
                var parts = FindObjectsByType<WebGL.Core.Content.ExplodablePart>(FindObjectsSortMode.None);
                if (parts.Length > 0)
                    targetObject = parts[0].transform.root;
            }

            // Cache only part renderers (not the plane visual, skybox, etc.)
            var parts2 = FindObjectsByType<WebGL.Core.Content.ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts2)
            {
                var r = part.GetComponent<Renderer>();
                if (r != null) partRenderers.Add(r);
            }

            Debug.Log($"[CrossSectionManager] Cached {partRenderers.Count} part renderers.");
        }

        private void CreatePlaneVisual()
        {
            planeVisual = GameObject.CreatePrimitive(PrimitiveType.Quad);
            planeVisual.name = "[CrossSectionPlane]";
            planeVisual.transform.SetParent(transform);
            planeVisual.transform.localScale = new Vector3(10f, 10f, 1f);

            var collider = planeVisual.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = planeVisual.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetColor("_BaseColor", planeColor);
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            renderer.material = mat;

            planeVisual.SetActive(false);
        }

        private void Update()
        {
            if (isEnabled)
            {
                UpdateClipPlane();
                ApplyClipPlane();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Public API
        // ═══════════════════════════════════════════════════════

        public void EnableCrossSection()
        {
            if (clippableShader == null)
            {
                clippableShader = Shader.Find("WebGL/ClippableLit");
                if (clippableShader == null) { Debug.LogError("[CrossSection] Shader missing!"); return; }
            }

            // Re-cache if needed
            if (partRenderers.Count == 0) CacheRenderers();

            isEnabled = true;
            SwapToClippableMaterials();

            if (planeVisual != null && showPlane)
                planeVisual.SetActive(true);

            ApplyClipPlane();
            AudioManager.Instance?.PlayClick();
            Debug.Log("[CrossSection] Enabled");
        }

        public void DisableCrossSection()
        {
            isEnabled = false;
            ClearClipPlane();
            RestoreOriginalMaterials();

            if (planeVisual != null)
                planeVisual.SetActive(false);

            AudioManager.Instance?.PlayClick();
            Debug.Log("[CrossSection] Disabled");
        }

        public void ToggleCrossSection()
        {
            if (isEnabled) DisableCrossSection();
            else EnableCrossSection();
        }

        public void SetAxis(CrossSectionAxis newAxis)
        {
            axis = newAxis;
            UpdateClipPlane();
        }

        public void SetPosition(float newPosition)
        {
            position = newPosition;
            UpdateClipPlane();
        }

        public void SetInverted(bool inverted)
        {
            invertDirection = inverted;
            UpdateClipPlane();
        }

        // ═══════════════════════════════════════════════════════
        //  Material Swap — makes all parts clippable
        // ═══════════════════════════════════════════════════════

        private void SwapToClippableMaterials()
        {
            savedMaterials.Clear();

            foreach (var renderer in partRenderers)
            {
                if (renderer == null) continue;

                // Save current materials
                savedMaterials[renderer] = renderer.sharedMaterials;

                // Create clippable copies
                var origMats = renderer.sharedMaterials;
                var newMats = new Material[origMats.Length];

                for (int i = 0; i < origMats.Length; i++)
                {
                    var orig = origMats[i];
                    var clipMat = new Material(clippableShader);
                    clipMat.name = (orig != null ? orig.name : "Unknown") + "_Clippable";

                    // Copy base properties from original
                    if (orig != null)
                    {
                        if (orig.HasProperty("_BaseMap"))
                            clipMat.SetTexture("_BaseMap", orig.GetTexture("_BaseMap"));
                        if (orig.HasProperty("_BaseColor"))
                            clipMat.SetColor("_BaseColor", orig.GetColor("_BaseColor"));
                        else if (orig.HasProperty("_Color"))
                            clipMat.SetColor("_BaseColor", orig.GetColor("_Color"));
                        if (orig.HasProperty("_Metallic"))
                            clipMat.SetFloat("_Metallic", orig.GetFloat("_Metallic"));
                        if (orig.HasProperty("_Smoothness"))
                            clipMat.SetFloat("_Smoothness", orig.GetFloat("_Smoothness"));
                        if (orig.HasProperty("_BumpMap"))
                            clipMat.SetTexture("_BumpMap", orig.GetTexture("_BumpMap"));
                    }

                    // Enable clipping
                    clipMat.EnableKeyword("_CLIP_ENABLED");
                    clipMat.SetFloat("_ClipEnabled", 1f);
                    clipMat.SetColor("_ClipColor", clipEdgeColor);
                    clipMat.SetFloat("_ClipEdgeWidth", clipEdgeWidth);

                    newMats[i] = clipMat;
                }

                renderer.materials = newMats;
            }
        }

        private void RestoreOriginalMaterials()
        {
            foreach (var kvp in savedMaterials)
            {
                if (kvp.Key != null)
                    kvp.Key.materials = kvp.Value;
            }
            savedMaterials.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Clip Plane Calculation
        // ═══════════════════════════════════════════════════════

        private void UpdateClipPlane()
        {
            Vector3 normal = Vector3.zero;
            Vector3 worldPos = targetObject != null ? targetObject.position : Vector3.zero;
            worldPos += GetAxisVector() * position;

            switch (axis)
            {
                case CrossSectionAxis.X:
                    normal = invertDirection ? Vector3.left : Vector3.right;
                    break;
                case CrossSectionAxis.Y:
                    normal = invertDirection ? Vector3.down : Vector3.up;
                    break;
                case CrossSectionAxis.Z:
                    normal = invertDirection ? Vector3.back : Vector3.forward;
                    break;
            }

            // Plane equation: dot(pos, normal) + w = 0
            // w = -dot(normal, pointOnPlane)
            clipPlane = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, worldPos));

            // Update visual plane
            if (planeVisual != null)
            {
                planeVisual.transform.position = worldPos;
                planeVisual.transform.rotation = Quaternion.LookRotation(normal);
            }
        }

        private Vector3 GetAxisVector()
        {
            switch (axis)
            {
                case CrossSectionAxis.X: return Vector3.right;
                case CrossSectionAxis.Y: return Vector3.up;
                case CrossSectionAxis.Z: return Vector3.forward;
                default: return Vector3.up;
            }
        }

        private void ApplyClipPlane()
        {
            Shader.SetGlobalVector(GlobalClipPlaneId, clipPlane);
            Shader.SetGlobalFloat(GlobalClipEnabledId, 1f);
        }

        private void ClearClipPlane()
        {
            Shader.SetGlobalFloat(GlobalClipEnabledId, 0f);
        }

        private void OnDisable()
        {
            if (isEnabled)
            {
                ClearClipPlane();
                RestoreOriginalMaterials();
            }
        }
    }
}
