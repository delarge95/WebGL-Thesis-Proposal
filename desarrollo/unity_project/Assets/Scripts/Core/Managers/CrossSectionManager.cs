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
        [SerializeField] private Material crossSectionMaterial;

        private Renderer[] allRenderers;
        private Material[] originalMaterials;
        private Vector4 clipPlane;
        private GameObject planeVisual;

        private static readonly int ClipPlaneId = Shader.PropertyToID("_ClipPlane");
        private static readonly int ClipEnabledId = Shader.PropertyToID("_ClipEnabled");

        public bool IsEnabled => isEnabled;
        public float Position => position;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            CacheRenderers();
            CreatePlaneVisual();
            UpdateClipPlane();
        }

        private void CacheRenderers()
        {
            if (targetObject == null)
            {
                var parts = FindObjectsByType<WebGL.Core.Content.ExplodablePart>(FindObjectsSortMode.None);
                if (parts.Length > 0)
                {
                    targetObject = parts[0].transform.root;
                }
            }

            allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
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
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            renderer.material.SetColor("_BaseColor", planeColor);
            renderer.material.SetFloat("_Surface", 1); // Transparent
            renderer.material.renderQueue = 3000;

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

        public void EnableCrossSection()
        {
            isEnabled = true;
            if (planeVisual != null && showPlane)
            {
                planeVisual.SetActive(true);
            }
            ApplyClipPlane();
            AudioManager.Instance?.PlayClick();

            Debug.Log("[CrossSection] Enabled");
        }

        public void DisableCrossSection()
        {
            isEnabled = false;
            if (planeVisual != null)
            {
                planeVisual.SetActive(false);
            }
            ClearClipPlane();
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
            Shader.SetGlobalVector(ClipPlaneId, clipPlane);
            Shader.SetGlobalFloat(ClipEnabledId, 1f);
        }

        private void ClearClipPlane()
        {
            Shader.SetGlobalFloat(ClipEnabledId, 0f);
        }

        private void OnDisable()
        {
            ClearClipPlane();
        }
    }
}
