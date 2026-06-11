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
        [SerializeField] private Light studioFillLight;
        [SerializeField] private Light studioRimLight;
        [SerializeField] private Light studioTopLight;
        [SerializeField] private Light studioBounceLight;

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

            EnsureStudioLightRig();
            EnsureSkyboxMaterial();
            ApplyPreset("Studio");
        }

        private void EnsureStudioLightRig()
        {
            if (directionalLight == null)
            {
                GameObject keyGo = new GameObject("Studio Key Light");
                directionalLight = keyGo.AddComponent<Light>();
                directionalLight.type = LightType.Directional;
            }

            ConfigureDirectionalLight(directionalLight, "Studio Key Light", LightShadows.Soft);
            studioFillLight = EnsureStudioSoftbox(studioFillLight, "Studio Fill Softbox", 82f);
            studioRimLight = EnsureStudioSoftbox(studioRimLight, "Studio Rim Softbox", 54f);
            studioTopLight = EnsureStudioSoftbox(studioTopLight, "Studio Top Softbox", 96f);
            studioBounceLight = EnsureStudioSoftbox(studioBounceLight, "Studio Bounce Softbox", 110f);
        }

        private static Light EnsureStudioSoftbox(Light current, string lightName, float spotAngle)
        {
            if (current == null)
            {
                GameObject existing = GameObject.Find(lightName);
                current = existing != null ? existing.GetComponent<Light>() : null;
            }

            if (current == null)
            {
                GameObject lightGo = new GameObject(lightName);
                current = lightGo.AddComponent<Light>();
            }

            ConfigureStudioSoftbox(current, lightName, spotAngle);
            return current;
        }

        private static void ConfigureDirectionalLight(Light light, string lightName, LightShadows shadows)
        {
            if (light == null)
            {
                return;
            }

            light.name = lightName;
            light.type = LightType.Directional;
            light.shadows = shadows;
            light.shadowStrength = shadows == LightShadows.None ? 0f : 0.52f;
            light.shadowBias = 0.06f;
            light.shadowNormalBias = 0.35f;
            light.renderMode = LightRenderMode.Auto;
            light.enabled = true;
        }

        private static void ConfigureStudioSoftbox(Light light, string lightName, float spotAngle)
        {
            if (light == null)
            {
                return;
            }

            light.name = lightName;
            light.type = LightType.Spot;
            light.shadows = LightShadows.None;
            light.renderMode = LightRenderMode.ForcePixel;
            light.spotAngle = spotAngle;
            light.innerSpotAngle = Mathf.Clamp(spotAngle * 0.62f, 12f, spotAngle);
            light.enabled = true;
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
            ApplySupplementalStudioLighting(directionalLight.color, directionalLight.intensity, euler);
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

            ApplySupplementalStudioLighting(
                directionalLight != null ? directionalLight.color : Color.white,
                intensity,
                directionalLight != null ? directionalLight.transform.eulerAngles : new Vector3(50f, 45f, 0f));
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
                        topColor       = new Color(0.16f, 0.17f, 0.19f),
                        bottomColor    = new Color(0.018f, 0.019f, 0.024f),
                        lightColor     = new Color(1f, 0.985f, 0.94f),
                        lightIntensity = 2.25f,
                        lightRotY = 36f, lightPitch = 52f,
                        pulseEnabled = true, pulseSpeed = 0.35f, gradientScale = 0.82f,
                        ditherStrength = 0.05f
                    };

                case "Studio Light":
                    return new PresetData {
                        topColor       = new Color(0.82f, 0.82f, 0.84f),  // soft light grey
                        bottomColor    = new Color(0.55f, 0.55f, 0.58f),  // mid grey edge
                        lightColor     = new Color(1f, 0.98f, 0.95f),     // warm white
                        lightIntensity = 2.35f,
                        lightRotY = 34f, lightPitch = 50f,
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

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            Color fromSkyColor = RenderSettings.ambientSkyColor;
            Color fromEquatorColor = RenderSettings.ambientEquatorColor;
            Color fromGroundColor = RenderSettings.ambientGroundColor;

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
                    
                    ResolveAmbientLighting(target, bgFactor, out Color skyLighting, out Color equatorLighting, out Color groundLighting);
                    
                    RenderSettings.ambientSkyColor = Color.Lerp(fromSkyColor, skyLighting, t);
                    RenderSettings.ambientEquatorColor = Color.Lerp(fromEquatorColor, equatorLighting, t);
                    RenderSettings.ambientGroundColor = Color.Lerp(fromGroundColor, groundLighting, t);
                }

                if (directionalLight != null)
                {
                    directionalLight.color = Color.Lerp(fromLight, target.lightColor, t);
                    directionalLight.intensity = Mathf.Lerp(fromIntensity, target.lightIntensity, t);
                    directionalLight.transform.eulerAngles = Vector3.Lerp(fromEuler, targetEuler, t);
                    ApplySupplementalStudioLighting(directionalLight.color, directionalLight.intensity, directionalLight.transform.eulerAngles);
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

                float bgFactor = Mathf.Clamp01(_backgroundIntensity / 100f);
                ResolveAmbientLighting(target, bgFactor, out Color skyLighting, out Color equatorLighting, out Color groundLighting);
                RenderSettings.ambientSkyColor = skyLighting;
                RenderSettings.ambientEquatorColor = equatorLighting;
                RenderSettings.ambientGroundColor = groundLighting;
            }

            if (directionalLight != null)
            {
                directionalLight.color = target.lightColor;
                directionalLight.intensity = target.lightIntensity;
                directionalLight.transform.eulerAngles = targetEuler;
                ApplySupplementalStudioLighting(target.lightColor, target.lightIntensity, targetEuler);
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

            ResolveAmbientLighting(GetPresetData(_currentPreset), bgFactor, out Color skyLighting, out Color equatorLighting, out Color groundLighting);
            RenderSettings.ambientSkyColor = skyLighting;
            RenderSettings.ambientEquatorColor = equatorLighting;
            RenderSettings.ambientGroundColor = groundLighting;
        }

        private void ResolveAmbientLighting(PresetData target, float bgFactor, out Color skyLighting, out Color equatorLighting, out Color groundLighting)
        {
            if (IsStudioLightingPreset(_currentPreset))
            {
                float exposure = Mathf.Clamp(target.lightIntensity, 1.25f, 2.8f);
                skyLighting = Color.Lerp(target.lightColor, new Color(0.74f, 0.80f, 0.90f, 1f), 0.34f) * (0.34f * exposure);
                equatorLighting = Color.Lerp(target.lightColor, new Color(0.55f, 0.62f, 0.72f, 1f), 0.48f) * (0.27f * exposure);
                groundLighting = new Color(0.36f, 0.40f, 0.48f, 1f) * (0.23f * exposure);
            }
            else
            {
                Color top = target.topColor * bgFactor;
                Color bottom = target.bottomColor * bgFactor;
                skyLighting = top * 0.72f;
                equatorLighting = Color.Lerp(top, bottom, 0.5f) * 0.62f;
                groundLighting = new Color(0.4f, 0.45f, 0.5f, 1f) * (target.lightIntensity * 0.42f);
            }

            skyLighting.a = 1f;
            equatorLighting.a = 1f;
            groundLighting.a = 1f;
        }

        private static bool IsStudioLightingPreset(string presetName)
        {
            return string.Equals(presetName, "Studio", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(presetName, "Studio Light", System.StringComparison.OrdinalIgnoreCase);
        }

        private void ApplySupplementalStudioLighting(Color keyColor, float keyIntensity, Vector3 keyEuler)
        {
            ResolveStudioTarget(out Vector3 targetCenter, out float targetRadius);
            float studioBoost = IsStudioLightingPreset(_currentPreset) ? 1f : 0.62f;
            Color coolSoftbox = new Color(0.68f, 0.78f, 1f, 1f);
            Color neutralSoftbox = new Color(0.92f, 0.96f, 1f, 1f);
            Color warmSoftbox = new Color(1f, 0.92f, 0.82f, 1f);

            PositionStudioSoftbox(
                studioFillLight,
                targetCenter,
                targetRadius,
                keyEuler.y + 138f,
                0.62f,
                1.55f,
                Color.Lerp(keyColor, coolSoftbox, 0.48f),
                Mathf.Clamp(keyIntensity * 0.55f * studioBoost, 0.18f, 1.35f),
                3.4f,
                88f);

            PositionStudioSoftbox(
                studioRimLight,
                targetCenter,
                targetRadius,
                keyEuler.y + 218f,
                0.88f,
                1.42f,
                Color.Lerp(keyColor, neutralSoftbox, 0.62f),
                Mathf.Clamp(keyIntensity * 0.82f * studioBoost, 0.20f, 1.85f),
                3.2f,
                58f);

            PositionStudioSoftbox(
                studioTopLight,
                targetCenter,
                targetRadius,
                keyEuler.y + 18f,
                1.85f,
                0.36f,
                Color.Lerp(keyColor, warmSoftbox, 0.35f),
                Mathf.Clamp(keyIntensity * 0.68f * studioBoost, 0.18f, 1.55f),
                2.8f,
                104f);

            PositionStudioSoftbox(
                studioBounceLight,
                targetCenter,
                targetRadius,
                keyEuler.y + 182f,
                -0.55f,
                0.82f,
                Color.Lerp(keyColor, coolSoftbox, 0.70f),
                Mathf.Clamp(keyIntensity * 0.30f * studioBoost, 0.08f, 0.72f),
                2.4f,
                116f);
        }

        private static void PositionStudioSoftbox(
            Light light,
            Vector3 targetCenter,
            float targetRadius,
            float yawDegrees,
            float heightFactor,
            float distanceFactor,
            Color color,
            float intensity,
            float rangeFactor,
            float spotAngle)
        {
            if (light == null)
            {
                return;
            }

            float yawRadians = yawDegrees * Mathf.Deg2Rad;
            Vector3 planarDirection = new Vector3(Mathf.Sin(yawRadians), 0f, Mathf.Cos(yawRadians));
            float safeRadius = Mathf.Max(targetRadius, 0.5f);
            light.transform.position = targetCenter + planarDirection * safeRadius * distanceFactor + Vector3.up * safeRadius * heightFactor;
            light.transform.LookAt(targetCenter);
            light.color = color;
            light.intensity = intensity;
            light.range = Mathf.Max(safeRadius * rangeFactor, 4f);
            light.spotAngle = spotAngle;
            light.innerSpotAngle = Mathf.Clamp(spotAngle * 0.62f, 12f, spotAngle);
            light.enabled = true;
        }

        private static void ResolveStudioTarget(out Vector3 center, out float radius)
        {
            center = Vector3.zero;
            radius = 5f;

            GameObject droneRoot = GameObject.Find("x500v2_Drone");
            if (droneRoot != null)
            {
                Renderer[] renderers = droneRoot.GetComponentsInChildren<Renderer>(true);
                bool hasBounds = false;
                Bounds bounds = new Bounds(droneRoot.transform.position, Vector3.zero);
                for (int i = 0; i < renderers.Length; i++)
                {
                    Renderer renderer = renderers[i];
                    if (renderer == null || !renderer.enabled)
                    {
                        continue;
                    }

                    if (!hasBounds)
                    {
                        bounds = renderer.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }

                if (hasBounds)
                {
                    center = bounds.center;
                    radius = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                }
            }
            else
            {
                GameObject orbitTarget = GameObject.Find("OrbitTarget");
                if (orbitTarget != null)
                {
                    center = orbitTarget.transform.position;
                }
            }

            radius = Mathf.Clamp(radius, 3.5f, 28f);
        }
    }
}
