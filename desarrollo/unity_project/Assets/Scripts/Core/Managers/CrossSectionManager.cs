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
    /// Sets global shader properties (_GlobalClipPlane / _GlobalClipEnabled) that all
    /// WebGL/* shaders read to discard fragments on the clipped side.
    /// No material swap needed — clipping is purely global-property driven.
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

        [Header("References")]
        [SerializeField] private Transform targetObject;

        private Vector4 clipPlane;
        private GameObject planeVisual;

        // Global property IDs (must match all WebGL/* shaders)
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
            FindTargetObject();
            CreatePlaneVisual();
            // Ensure clipping starts disabled
            ClearClipPlane();
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
            if (targetObject == null) FindTargetObject();

            isEnabled = true;
            UpdateClipPlane();
            ApplyClipPlane();

            if (planeVisual != null && showPlane)
                planeVisual.SetActive(true);

            AudioManager.Instance?.PlayClick();
            Debug.Log("[CrossSection] Enabled (global clip)");
        }

        public void DisableCrossSection()
        {
            isEnabled = false;
            ClearClipPlane();

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
            ClearClipPlane();
        }
    }
}
