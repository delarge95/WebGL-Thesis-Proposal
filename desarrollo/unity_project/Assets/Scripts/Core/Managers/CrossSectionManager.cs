using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Utils;

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
    /// Supports up to 2 simultaneous clip planes for diagonal cuts.
    /// Sets global shader properties (_GlobalClipPlane / _GlobalClipEnabled and
    /// _GlobalClipPlane2 / _GlobalClipEnabled2) that all WebGL/* shaders read.
    /// When in Realistic mode, swaps URP/Lit originals to ClippableLit copies
    /// so that clipping works even with the standard materials.
    /// </summary>
    public class CrossSectionManager : Singleton<CrossSectionManager>
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled = false;

        [Header("Plane 1")]
        [SerializeField] private bool plane1Active = true;
        [SerializeField] private CrossSectionAxis axis1 = CrossSectionAxis.Y;
        [SerializeField] private float position1 = 0f;
        [SerializeField] private bool invertDirection1 = false;

        [Header("Plane 2")]
        [SerializeField] private bool combinePlanes = false;
        [SerializeField] private bool plane2Active = false;
        [SerializeField] private CrossSectionAxis axis2 = CrossSectionAxis.X;
        [SerializeField] private float position2 = 0f;
        [SerializeField] private bool invertDirection2 = false;

        [Header("Visuals")]
        [SerializeField] private Color planeColor = new Color(0.2745f, 0.6863f, 1f, 0.006f);
        [SerializeField] private bool showPlane = true;

        [Header("References")]
        [SerializeField] private Transform targetObject;

        private Vector4 clipPlane1;
        private Vector4 clipPlane2;
        private GameObject planeVisual1;
        private GameObject planeVisual2;

        // Material swap for Realistic mode
        private Shader clippableLitShader;
        private Dictionary<Renderer, Material[]> clippableCopies = new Dictionary<Renderer, Material[]>();
        private bool materialsSwapped = false;

        // Cached property IDs
        private static readonly int GlobalClipPlaneId = Shader.PropertyToID("_GlobalClipPlane");
        private static readonly int GlobalClipEnabledId = Shader.PropertyToID("_GlobalClipEnabled");
        private static readonly int GlobalClipPlane2Id = Shader.PropertyToID("_GlobalClipPlane2");
        private static readonly int GlobalClipEnabled2Id = Shader.PropertyToID("_GlobalClipEnabled2");

        public bool IsEnabled => isEnabled;
        public float Position1 => position1;
        public float Position2 => position2;
        public bool Plane2Active => plane2Active;
        public CrossSectionAxis Axis1 => axis1;
        public CrossSectionAxis Axis2 => axis2;
        public bool Inverted1 => invertDirection1;
        public bool Inverted2 => invertDirection2;

        // Event fired when cross-section state changes (UI can react)
        public event Action<bool> OnCrossSectionToggled;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            clippableLitShader = Shader.Find("WebGL/ClippableLit");
            if (clippableLitShader == null)
            {
                // Fallback: load from Resources for WebGL builds where Shader.Find may fail
                var mat = Resources.Load<Material>("ClippableLitRef");
                if (mat != null) clippableLitShader = mat.shader;
                if (clippableLitShader == null)
                    Debug.LogWarning("[CrossSection] ClippableLit shader not found! Cross-section in Realistic mode will not work.");
            }
            FindTargetObject();
            CreatePlaneVisual(ref planeVisual1, "[CrossSectionPlane1]");
            CreatePlaneVisual(ref planeVisual2, "[CrossSectionPlane2]");
            ClearAllClipPlanes();

            // Listen for view mode changes
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged += OnViewModeChanged;
        }

        private void OnDestroy()
        {
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged -= OnViewModeChanged;
        }

        private void FindTargetObject()
        {
            if (targetObject == null)
            {
                var parts = FindObjectsByType<WebGL.Core.Content.ExplodablePart>(FindObjectsSortMode.None);
                if (parts.Length > 0)
                    targetObject = parts[0].transform.root;
            }
        }

        public void RefreshTargetObject()
        {
            targetObject = null;
            FindTargetObject();

            if (isEnabled)
            {
                UpdateAndApply();
            }
        }

        private void CreatePlaneVisual(ref GameObject visual, string name)
        {
            visual = GameObject.CreatePrimitive(PrimitiveType.Quad);
            visual.name = name;
            visual.transform.SetParent(transform);
            visual.transform.localScale = new Vector3(10f, 10f, 1f);

            var collider = visual.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = visual.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetColor("_BaseColor", planeColor);
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            renderer.material = mat;

            visual.SetActive(false);
        }

        // ═══════════════════════════════════════════════════════
        //  Public API
        // ═══════════════════════════════════════════════════════

        public void EnableCrossSection()
        {
            if (targetObject == null) FindTargetObject();

            isEnabled = true;
            UpdateAndApply();

            if (planeVisual1 != null && showPlane && plane1Active)
                planeVisual1.SetActive(true);
            if (planeVisual2 != null && showPlane && plane2Active && !combinePlanes)
                planeVisual2.SetActive(true);

            // Swap materials for Realistic mode
            TrySwapMaterialsForRealistic();

            OnCrossSectionToggled?.Invoke(true);
            AudioManager.Instance?.PlayClick();
            Debug.Log("[CrossSection] Enabled (dual-plane global clip)");
        }

        public void DisableCrossSection()
        {
            isEnabled = false;
            ClearAllClipPlanes();

            if (planeVisual1 != null) planeVisual1.SetActive(false);
            if (planeVisual2 != null) planeVisual2.SetActive(false);

            // Restore original materials if they were swapped
            RestoreOriginalMaterials();

            OnCrossSectionToggled?.Invoke(false);
            AudioManager.Instance?.PlayClick();
            Debug.Log("[CrossSection] Disabled");
        }

        public void ToggleCrossSection()
        {
            if (isEnabled) DisableCrossSection();
            else EnableCrossSection();
        }

        // ── Plane 1 controls ──

        public void SetPlane1Active(bool active)
        {
            plane1Active = active;
            if (planeVisual1 != null)
                planeVisual1.SetActive(active && isEnabled && showPlane);

            if (!active)
            {
                Shader.SetGlobalFloat(GlobalClipEnabledId, 0f);
            }
            else if (isEnabled)
            {
                UpdateAndApply();
            }
        }

        public void SetCombinePlanes(bool combine)
        {
            combinePlanes = combine;
            UpdateAndApply();
        }

        public void SetAxis1(CrossSectionAxis newAxis)
        {
            axis1 = newAxis;
            UpdateAndApply();
        }

        public void SetPosition1(float newPosition)
        {
            position1 = newPosition;
            UpdateAndApply();
        }

        public void SetInverted1(bool inverted)
        {
            invertDirection1 = inverted;
            UpdateAndApply();
        }

        // ── Plane 2 controls ──

        public void SetPlane2Active(bool active)
        {
            plane2Active = active;
            if (planeVisual2 != null)
                planeVisual2.SetActive(active && isEnabled && showPlane);

            if (!active)
            {
                // Disable plane 2 in shader
                Shader.SetGlobalFloat(GlobalClipEnabled2Id, 0f);
            }
            else if (isEnabled)
            {
                UpdateAndApply();
            }
        }

        public void SetAxis2(CrossSectionAxis newAxis)
        {
            axis2 = newAxis;
            UpdateAndApply();
        }

        public void SetPosition2(float newPosition)
        {
            position2 = newPosition;
            UpdateAndApply();
        }

        public void SetInverted2(bool inverted)
        {
            invertDirection2 = inverted;
            UpdateAndApply();
        }

        // ── Legacy API (maps to Plane 1) ──

        public void SetAxis(CrossSectionAxis newAxis) => SetAxis1(newAxis);
        public void SetPosition(float newPosition) => SetPosition1(newPosition);
        public void SetInverted(bool inverted) => SetInverted1(inverted);

        // ═══════════════════════════════════════════════════════
        //  Clip Plane Calculation
        // ═══════════════════════════════════════════════════════

        private void UpdateAndApply()
        {
            Vector3 worldCenter = targetObject != null ? targetObject.position : Vector3.zero;

            if (combinePlanes && plane1Active && plane2Active)
            {
                // COMBINED DIAGONAL PLANE
                Vector3 n1 = GetNormal(axis1, invertDirection1);
                Vector3 n2 = GetNormal(axis2, invertDirection2);
                Vector3 combinedNormal = (n1 + n2).normalized;

                Vector3 v1 = GetAxisVector(axis1);
                Vector3 v2 = GetAxisVector(axis2);
                Vector3 combinedAxisVector = (v1 + v2).normalized;

                Vector3 point = worldCenter + combinedAxisVector * position1;
                
                clipPlane1 = new Vector4(combinedNormal.x, combinedNormal.y, combinedNormal.z, -Vector3.Dot(combinedNormal, point));
                Shader.SetGlobalVector(GlobalClipPlaneId, clipPlane1);
                Shader.SetGlobalFloat(GlobalClipEnabledId, 1f);

                if (planeVisual1 != null)
                {
                    planeVisual1.transform.position = point;
                    planeVisual1.transform.rotation = Quaternion.LookRotation(combinedNormal);
                    planeVisual1.SetActive(showPlane);
                }

                // Disable Plane 2 representation
                Shader.SetGlobalFloat(GlobalClipEnabled2Id, 0f);
                if (planeVisual2 != null) planeVisual2.SetActive(false);
            }
            else
            {
                // NORMAL SEPARATE PLANES
                if (plane1Active)
                {
                    clipPlane1 = CalculateClipPlane(axis1, position1, invertDirection1, worldCenter);
                    Shader.SetGlobalVector(GlobalClipPlaneId, clipPlane1);
                    Shader.SetGlobalFloat(GlobalClipEnabledId, 1f);

                    if (planeVisual1 != null)
                    {
                        Vector3 pos1 = worldCenter + GetAxisVector(axis1) * position1;
                        planeVisual1.transform.position = pos1;
                        planeVisual1.transform.rotation = Quaternion.LookRotation(GetNormal(axis1, invertDirection1));
                        planeVisual1.SetActive(showPlane);
                    }
                }
                else
                {
                    Shader.SetGlobalFloat(GlobalClipEnabledId, 0f);
                    if (planeVisual1 != null) planeVisual1.SetActive(false);
                }

                // Plane 2
                if (plane2Active)
                {
                    clipPlane2 = CalculateClipPlane(axis2, position2, invertDirection2, worldCenter);
                    Shader.SetGlobalVector(GlobalClipPlane2Id, clipPlane2);
                    Shader.SetGlobalFloat(GlobalClipEnabled2Id, 1f);

                    if (planeVisual2 != null)
                    {
                        Vector3 pos2 = worldCenter + GetAxisVector(axis2) * position2;
                        planeVisual2.transform.position = pos2;
                        planeVisual2.transform.rotation = Quaternion.LookRotation(GetNormal(axis2, invertDirection2));
                        planeVisual2.SetActive(showPlane);
                    }
                }
                else
                {
                    Shader.SetGlobalFloat(GlobalClipEnabled2Id, 0f);
                    if (planeVisual2 != null) planeVisual2.SetActive(false);
                }
            }
        }

        private Vector4 CalculateClipPlane(CrossSectionAxis axis, float pos, bool inverted, Vector3 worldCenter)
        {
            Vector3 normal = GetNormal(axis, inverted);
            Vector3 point = worldCenter + GetAxisVector(axis) * pos;
            return new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, point));
        }

        private Vector3 GetNormal(CrossSectionAxis axis, bool inverted)
        {
            switch (axis)
            {
                case CrossSectionAxis.X: return inverted ? Vector3.left : Vector3.right;
                case CrossSectionAxis.Y: return inverted ? Vector3.down : Vector3.up;
                case CrossSectionAxis.Z: return inverted ? Vector3.back : Vector3.forward;
                default: return Vector3.up;
            }
        }

        private Vector3 GetAxisVector(CrossSectionAxis axis)
        {
            switch (axis)
            {
                case CrossSectionAxis.X: return Vector3.right;
                case CrossSectionAxis.Y: return Vector3.up;
                case CrossSectionAxis.Z: return Vector3.forward;
                default: return Vector3.up;
            }
        }

        private void ClearAllClipPlanes()
        {
            Shader.SetGlobalFloat(GlobalClipEnabledId, 0f);
            Shader.SetGlobalFloat(GlobalClipEnabled2Id, 0f);
        }

        // ═══════════════════════════════════════════════════════
        //  Material Swap for Realistic Mode
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// When cross-section is active and ViewMode is Realistic,
        /// swap URP/Lit originals to ClippableLit copies that
        /// respond to _GlobalClipPlane globals.
        /// </summary>
        private void TrySwapMaterialsForRealistic()
        {
            if (materialsSwapped) return;
            if (ViewModeManager.Instance == null) return;
            if (ViewModeManager.Instance.CurrentMode != ViewMode.Realistic) return;
            if (clippableLitShader == null) return;

            var renderers = GetAllPartRenderers();
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                var origMats = renderer.sharedMaterials;
                var newMats = new Material[origMats.Length];

                for (int i = 0; i < origMats.Length; i++)
                {
                    newMats[i] = CreateClippableCopy(origMats[i]);
                }

                clippableCopies[renderer] = origMats; // store originals for restore
                renderer.materials = newMats;
            }

            materialsSwapped = true;
            Debug.Log("[CrossSection] Swapped to ClippableLit for Realistic mode");
        }

        private void RestoreOriginalMaterials()
        {
            if (!materialsSwapped) return;

            foreach (var kvp in clippableCopies)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.materials = kvp.Value;
                    kvp.Key.SetPropertyBlock(null);
                }
            }

            clippableCopies.Clear();
            materialsSwapped = false;
            Debug.Log("[CrossSection] Restored original materials");
        }

        private void ReleaseSwappedStateWithoutRestoring()
        {
            if (!materialsSwapped)
            {
                return;
            }

            // ViewModeManager already applied the destination mode material.
            // We only need to drop our Realistic swap bookkeeping to avoid
            // restoring stale URP/Lit materials over Thermal/Blueprint/etc.
            clippableCopies.Clear();
            materialsSwapped = false;
        }

        private Material CreateClippableCopy(Material original)
        {
            if (original == null)
                return new Material(clippableLitShader);

            var copy = new Material(clippableLitShader);
            copy.name = original.name + "_Clippable";

            // Copy standard PBR properties
            if (original.HasProperty("_BaseMap"))
                copy.SetTexture("_BaseMap", original.GetTexture("_BaseMap"));
            if (original.HasProperty("_BaseColor"))
                copy.SetColor("_BaseColor", original.GetColor("_BaseColor"));
            else if (original.HasProperty("_Color"))
                copy.SetColor("_BaseColor", original.GetColor("_Color"));
            if (original.HasProperty("_Metallic"))
                copy.SetFloat("_Metallic", original.GetFloat("_Metallic"));
            if (original.HasProperty("_Smoothness"))
                copy.SetFloat("_Smoothness", original.GetFloat("_Smoothness"));
            if (original.HasProperty("_BumpMap"))
                copy.SetTexture("_BumpMap", original.GetTexture("_BumpMap"));
            if (original.HasProperty("_BumpScale"))
                copy.SetFloat("_BumpScale", original.GetFloat("_BumpScale"));

            return copy;
        }

        private List<Renderer> GetAllPartRenderers()
        {
            return WebGL.Core.Content.DroneRenderResolver.CollectManagedRenderers();
        }

        /// <summary>
        /// When view mode changes while cross-section is active:
        /// - Switching TO Realistic → swap to ClippableLit
        /// - Switching FROM Realistic → restore originals first (ViewModeManager will apply new shader)
        /// </summary>
        private void OnViewModeChanged(ViewMode newMode)
        {
            if (!isEnabled) return;

            if (newMode == ViewMode.Realistic)
            {
                // ViewModeManager already restored originals, now swap them to clippable
                // Use a small delay to ensure ViewModeManager has finished its transition
                StartCoroutine(SwapAfterFrame());
            }
            else
            {
                // Going to a custom shader that already supports clipping —
                // do NOT restore URP/Lit materials here; ViewModeManager already
                // applied the destination shader (Thermal/Blueprint/etc).
                if (materialsSwapped)
                {
                    ReleaseSwappedStateWithoutRestoring();
                }
            }
        }

        private System.Collections.IEnumerator SwapAfterFrame()
        {
            yield return null;
            yield return null; // Wait 2 frames for ViewModeManager coroutine
            TrySwapMaterialsForRealistic();
        }

        private void OnDisable()
        {
            ClearAllClipPlanes();
            if (materialsSwapped)
                RestoreOriginalMaterials();
        }
    }
}
