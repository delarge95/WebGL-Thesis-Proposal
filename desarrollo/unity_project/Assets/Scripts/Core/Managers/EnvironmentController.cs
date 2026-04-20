using System.Collections;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Controls scene environment: directional light and procedural gradient skybox.
    /// Every preset uses the AnimatedGradientSkybox shader — no more flat SolidColor.
    /// </summary>
    public class EnvironmentController : Singleton<EnvironmentController>
    {
        // ── Preset definition ────────────────────────────────
        private struct PresetData
        {
            public Color topColor;      // Gradient center
            public Color bottomColor;   // Gradient edge
            public Color lightColor;
            public float lightIntensity;
            public float lightRotY;
            public float lightPitch;
            public bool  pulseEnabled;
            public float pulseSpeed;
            public float gradientScale;
            public float ditherStrength;  // 0 = subtle anti-banding, 1 = visible grain (Blueprint)
            public bool  gridEnabled;     // true = show screen-space blueprint grid on skybox
        }

        [Header("References")]
        [SerializeField] private Light directionalLight;

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 0.5f;

        private string _currentPreset = "Studio";
        private Material _gradientSkybox;
        private Coroutine _transitionRoutine;

        // Adaptive UI — light/dark background notification
        public event System.Action<bool> OnLightBackgroundChanged;
        public bool IsLightBackground { get; private set; }

        // Shader property IDs (cached)
        private static readonly int TopColorId    = Shader.PropertyToID("_TopColor");
        private static readonly int BottomColorId = Shader.PropertyToID("_BottomColor");
        private static readonly int SpeedId       = Shader.PropertyToID("_Speed");
        private static readonly int ScaleId       = Shader.PropertyToID("_Scale");
        private static readonly int PulseEnabledId = Shader.PropertyToID("_PulseEnabled");
        private static readonly int DitherStrengthId = Shader.PropertyToID("_DitherStrength");
        private static readonly int GridEnabledId = Shader.PropertyToID("_GridEnabled");

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (directionalLight == null)
            {
                directionalLight = FindFirstObjectByType<Light>();
                if (directionalLight != null && directionalLight.type != LightType.Directional)
                    directionalLight = null;
            }

            EnsureSkyboxMaterial();
            ApplyPreset("Studio");
        }

        private void EnsureSkyboxMaterial()
        {
            if (_gradientSkybox != null) return;
            var shader = Shader.Find("Skybox/AnimatedGradientSkybox");
            if (shader != null) _gradientSkybox = new Material(shader);
        }

        // ── Public API ───────────────────────────────────────

        public void SetLightRotation(float angleY)
        {
            if (directionalLight == null) return;
            var euler = directionalLight.transform.eulerAngles;
            euler.y = angleY;
            directionalLight.transform.eulerAngles = euler;
        }

        // Base skybox colors captured when a preset is applied (for intensity scaling)
        private Color _baseTopColor;
        private Color _baseBottomColor;
        private float _backgroundIntensity = 100f;

        public void SetLightIntensity(float intensity)
        {
            intensity = Mathf.Clamp(intensity, 0.1f, 3f);

            if (directionalLight != null)
                directionalLight.intensity = intensity;
        }

        public void SetBackgroundIntensity(float intensityPercent)
        {
            _backgroundIntensity = Mathf.Clamp(intensityPercent, 0f, 100f);
            ApplyBackgroundColors();
        }

        public void ApplyPreset(string presetName)
        {
            _currentPreset = presetName;
            var data = GetPresetData(presetName);

            // Adaptive UI contrast — use center (topColor) luminance only,
            // because UI elements overlap the center region of the gradient.
            float bgFactor = Mathf.Clamp01(_backgroundIntensity / 100f);
            Color effectiveTop = data.topColor * bgFactor;
            float topLum = 0.299f * effectiveTop.r + 0.587f * effectiveTop.g + 0.114f * effectiveTop.b;
            bool newIsLight = topLum > 0.35f;
            if (newIsLight != IsLightBackground)
            {
                IsLightBackground = newIsLight;
                OnLightBackgroundChanged?.Invoke(newIsLight);
            }

            if (_transitionRoutine != null)
                StopCoroutine(_transitionRoutine);

            _transitionRoutine = StartCoroutine(TransitionToPreset(data));
        }

        public string CurrentPreset => _currentPreset;

        // ── Preset table ─────────────────────────────────────

        private PresetData GetPresetData(string name)
        {
            switch (name)
            {
                // ── Atmosphere presets (TIME cycle) ──
                case "Studio":
                    return new PresetData {
                        topColor       = new Color(0.10f, 0.10f, 0.12f),
                        bottomColor    = Color.black,
                        lightColor     = new Color(1f, 0.98f, 0.95f),
                        lightIntensity = 1.2f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = true, pulseSpeed = 0.5f, gradientScale = 0.8f,
                        ditherStrength = 0.05f
                    };

                case "Studio Light":
                    return new PresetData {
                        topColor       = new Color(0.82f, 0.82f, 0.84f),  // soft light grey
                        bottomColor    = new Color(0.55f, 0.55f, 0.58f),  // mid grey edge
                        lightColor     = new Color(1f, 0.98f, 0.95f),     // warm white
                        lightIntensity = 1.8f,
                        lightRotY = 30f, lightPitch = 45f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.4f,
                        ditherStrength = 0.03f
                    };

                case "Day":
                    return new PresetData {
                        topColor       = new Color(0.92f, 0.94f, 0.98f),    // warm white center
                        bottomColor    = new Color(0.45f, 0.62f, 0.82f),    // sky blue edge
                        lightColor     = new Color(1f, 0.98f, 0.92f),
                        lightIntensity = 1.6f,
                        lightRotY = 120f, lightPitch = 55f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.35f,
                        ditherStrength = 0.05f
                    };

                case "Sunset":
                    return new PresetData {
                        topColor       = new Color(0.95f, 0.72f, 0.35f),    // warm gold center
                        bottomColor    = new Color(0.18f, 0.06f, 0.22f),    // deep purple edge
                        lightColor     = new Color(1f, 0.65f, 0.3f),
                        lightIntensity = 1.5f,
                        lightRotY = 220f, lightPitch = 15f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.25f,
                        ditherStrength = 0.05f
                    };

                case "Night":
                    return new PresetData {
                        topColor       = new Color(0.01f, 0.015f, 0.04f),   // original dark indigo
                        bottomColor    = new Color(0.002f, 0.002f, 0.008f), // near-black edge
                        lightColor     = new Color(0.5f, 0.6f, 1f),
                        lightIntensity = 0.4f,
                        lightRotY = 180f, lightPitch = 60f,
                        pulseEnabled = true, pulseSpeed = 0.3f, gradientScale = 1.1f,
                        ditherStrength = 0.05f
                    };

                case "Blueprint":
                    return new PresetData {
                        topColor       = new Color(0.12f, 0.22f, 0.45f),   // blueprint blue center
                        bottomColor    = new Color(0.04f, 0.08f, 0.18f),   // dark navy edge
                        lightColor     = new Color(0.7f, 0.85f, 1f),
                        lightIntensity = 0.8f,
                        lightRotY = 90f, lightPitch = 45f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.0f,
                        ditherStrength = 0.12f,
                        gridEnabled = true
                    };

                // ── Solid color presets (COLOR cycle) — now with gradients ──
                case "White":
                    return new PresetData {
                        topColor       = new Color(0.95f, 0.95f, 0.95f),
                        bottomColor    = new Color(0.72f, 0.72f, 0.74f),
                        lightColor     = new Color(1f, 0.98f, 0.95f),
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.45f,
                        ditherStrength = 0.05f
                    };

                case "Grey":
                    return new PresetData {
                        topColor       = new Color(0.30f, 0.30f, 0.32f),    // darkened for WCAG AA contrast with light icons
                        bottomColor    = new Color(0.10f, 0.10f, 0.11f),
                        lightColor     = Color.white,
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.35f,
                        ditherStrength = 0.05f
                    };

                case "Black":
                    return new PresetData {
                        topColor       = new Color(0.06f, 0.06f, 0.06f),
                        bottomColor    = new Color(0.01f, 0.01f, 0.01f),
                        lightColor     = Color.white,
                        lightIntensity = 1.2f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.25f,
                        ditherStrength = 0.05f
                    };

                case "Yellow":
                    return new PresetData {
                        topColor       = new Color(0.95f, 0.88f, 0.55f),    // soft warm yellow
                        bottomColor    = new Color(0.35f, 0.25f, 0.05f),    // dark amber
                        lightColor     = new Color(1f, 0.95f, 0.80f),       // warm tint on model
                        lightIntensity = 1.1f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Orange":
                    return new PresetData {
                        topColor       = new Color(0.95f, 0.62f, 0.35f),    // soft peach
                        bottomColor    = new Color(0.30f, 0.10f, 0.02f),    // dark rust
                        lightColor     = new Color(1f, 0.82f, 0.65f),       // warm orange tint
                        lightIntensity = 1.1f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Green":
                    return new PresetData {
                        topColor       = new Color(0.45f, 0.75f, 0.52f),    // soft sage
                        bottomColor    = new Color(0.05f, 0.18f, 0.08f),    // deep forest
                        lightColor     = new Color(0.85f, 1f, 0.88f),       // green tint on model
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Blue":
                    return new PresetData {
                        topColor       = new Color(0.35f, 0.55f, 0.88f),    // soft cerulean
                        bottomColor    = new Color(0.04f, 0.08f, 0.25f),    // deep navy
                        lightColor     = new Color(0.80f, 0.88f, 1f),       // cool blue tint
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Purple":
                    return new PresetData {
                        topColor       = new Color(0.65f, 0.48f, 0.82f),    // brighter lavender (WCAG AA with dark icons)
                        bottomColor    = new Color(0.12f, 0.05f, 0.22f),    // deep plum
                        lightColor     = new Color(0.90f, 0.82f, 1f),       // purple tint
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Red":
                    return new PresetData {
                        topColor       = new Color(0.88f, 0.42f, 0.38f),    // brighter terracotta (WCAG AA with dark icons)
                        bottomColor    = new Color(0.22f, 0.04f, 0.04f),    // deep maroon
                        lightColor     = new Color(1f, 0.85f, 0.82f),       // warm red tint
                        lightIntensity = 1.0f,
                        lightRotY = 45f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.3f,
                        ditherStrength = 0.05f
                    };

                case "Neutral":
                default:
                    return new PresetData {
                        topColor       = new Color(0.18f, 0.18f, 0.20f),
                        bottomColor    = new Color(0.05f, 0.05f, 0.06f),
                        lightColor     = Color.white,
                        lightIntensity = 1f,
                        lightRotY = 0f, lightPitch = 50f,
                        pulseEnabled = false, pulseSpeed = 0f, gradientScale = 1.25f,
                        ditherStrength = 0.05f
                    };
            }
        }

        // ── Smooth transition ────────────────────────────────

        private IEnumerator TransitionToPreset(PresetData target)
        {
            EnsureSkyboxMaterial();

            // Ensure skybox is active (no more SolidColor)
            if (Camera.main != null && _gradientSkybox != null)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = _gradientSkybox;
            }

            // Snapshot current values for lerp
            Color fromTop    = _gradientSkybox != null ? _gradientSkybox.GetColor(TopColorId)    : Color.black;
            Color fromBottom = _gradientSkybox != null ? _gradientSkybox.GetColor(BottomColorId) : Color.black;
            Color fromLight  = directionalLight != null ? directionalLight.color : Color.white;
            float fromIntensity = directionalLight != null ? directionalLight.intensity : 1f;
            Vector3 fromEuler = directionalLight != null ? directionalLight.transform.eulerAngles : Vector3.zero;
            float fromScale  = _gradientSkybox != null ? _gradientSkybox.GetFloat(ScaleId) : 0.8f;
            float fromDither = _gradientSkybox != null ? _gradientSkybox.GetFloat(DitherStrengthId) : 0f;
            float fromGrid = _gradientSkybox != null ? _gradientSkybox.GetFloat(GridEnabledId) : 0f;

            Vector3 targetEuler = new Vector3(target.lightPitch, target.lightRotY, 0f);

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

                if (_gradientSkybox != null)
                {
                    float bgFactor = Mathf.Clamp01(_backgroundIntensity / 100f);
                    Color lerpedTop = Color.Lerp(fromTop, target.topColor, t) * bgFactor;
                    Color lerpedBottom = Color.Lerp(fromBottom, target.bottomColor, t) * bgFactor;
                    lerpedTop.a = 1f;
                    lerpedBottom.a = 1f;

                    _gradientSkybox.SetColor(TopColorId, lerpedTop);
                    _gradientSkybox.SetColor(BottomColorId, lerpedBottom);
                    _gradientSkybox.SetFloat(ScaleId, Mathf.Lerp(fromScale, target.gradientScale, t));
                    _gradientSkybox.SetFloat(DitherStrengthId, Mathf.Lerp(fromDither, target.ditherStrength, t));
                    _gradientSkybox.SetFloat(GridEnabledId, Mathf.Lerp(fromGrid, target.gridEnabled ? 1f : 0f, t));
                }

                if (directionalLight != null)
                {
                    directionalLight.color = Color.Lerp(fromLight, target.lightColor, t);
                    directionalLight.intensity = Mathf.Lerp(fromIntensity, target.lightIntensity, t);
                    directionalLight.transform.eulerAngles = Vector3.Lerp(fromEuler, targetEuler, t);
                }

                yield return null;
            }

            // Snap final values
            if (_gradientSkybox != null)
            {
                _gradientSkybox.SetFloat(ScaleId,       target.gradientScale);
                _gradientSkybox.SetFloat(DitherStrengthId, target.ditherStrength);
                _gradientSkybox.SetFloat(GridEnabledId, target.gridEnabled ? 1f : 0f);
                _gradientSkybox.SetFloat(PulseEnabledId, target.pulseEnabled ? 1f : 0f);
                _gradientSkybox.SetFloat(SpeedId,       target.pulseSpeed);
            }

            if (directionalLight != null)
            {
                directionalLight.color = target.lightColor;
                directionalLight.intensity = target.lightIntensity;
                directionalLight.transform.eulerAngles = targetEuler;
            }

            // Capture base values so SetLightIntensity can scale proportionally
            _baseTopColor    = target.topColor;
            _baseBottomColor = target.bottomColor;
            ApplyBackgroundColors();

            float topLum = 0.299f * (_baseTopColor.r * Mathf.Clamp01(_backgroundIntensity / 100f))
                + 0.587f * (_baseTopColor.g * Mathf.Clamp01(_backgroundIntensity / 100f))
                + 0.114f * (_baseTopColor.b * Mathf.Clamp01(_backgroundIntensity / 100f));
            bool newIsLight = topLum > 0.35f;
            if (newIsLight != IsLightBackground)
            {
                IsLightBackground = newIsLight;
                OnLightBackgroundChanged?.Invoke(newIsLight);
            }

            _transitionRoutine = null;
        }

        private void ApplyBackgroundColors()
        {
            if (_gradientSkybox == null)
            {
                return;
            }

            float bgFactor = Mathf.Clamp01(_backgroundIntensity / 100f);
            Color top = _baseTopColor * bgFactor;
            Color bottom = _baseBottomColor * bgFactor;
            top.a = 1f;
            bottom.a = 1f;

            _gradientSkybox.SetColor(TopColorId, top);
            _gradientSkybox.SetColor(BottomColorId, bottom);
        }
    }
}
