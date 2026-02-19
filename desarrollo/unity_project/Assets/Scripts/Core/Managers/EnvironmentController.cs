using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Controls scene environment: directional light rotation/intensity and camera background.
    /// Uses procedural gradients (zero asset cost) instead of heavy HDRI cubemaps.
    /// </summary>
    public class EnvironmentController : Singleton<EnvironmentController>
    {
        [Header("References")]
        [SerializeField] private Light directionalLight;

        [Header("Defaults")]

        private string _currentPreset = "Studio";
        private Material _gradientSkybox;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (directionalLight == null)
            {
                // Auto-find
                directionalLight = FindFirstObjectByType<Light>();
                if (directionalLight != null && directionalLight.type != LightType.Directional)
                    directionalLight = null;
            }

            ApplyPreset("Studio");
        }

        /// <summary> Rotate directional light around Y axis (0-360) </summary>
        public void SetLightRotation(float angleY)
        {
            if (directionalLight == null) return;
            var euler = directionalLight.transform.eulerAngles;
            euler.y = angleY;
            directionalLight.transform.eulerAngles = euler;
        }

        /// <summary> Set directional light intensity (0.1 - 3.0) </summary>
        public void SetLightIntensity(float intensity)
        {
            if (directionalLight == null) return;
            directionalLight.intensity = Mathf.Clamp(intensity, 0.1f, 3f);
        }

        /// <summary> Apply a named environment preset </summary>
        public void ApplyPreset(string presetName)
        {
            _currentPreset = presetName;

            Color bgColor;
            Color lightColor;
            float lightIntensity;
            float lightRotY;
            float lightPitch;

            switch (presetName)
            {
                case "Studio":
                    bgColor = new Color(5f / 255f, 5f / 255f, 5f / 255f); // #050505 — matches web
                    lightColor = new Color(1f, 0.98f, 0.95f);
                    lightIntensity = 1.2f;
                    lightRotY = 45f;
                    lightPitch = 50f;
                    break;

                case "Sunset":
                    bgColor = new Color(0.12f, 0.05f, 0.02f);
                    lightColor = new Color(1f, 0.65f, 0.3f);
                    lightIntensity = 1.5f;
                    lightRotY = 220f;
                    lightPitch = 15f;
                    break;

                case "Night":
                    bgColor = new Color(0.01f, 0.015f, 0.04f);
                    lightColor = new Color(0.5f, 0.6f, 1f);
                    lightIntensity = 0.4f;
                    lightRotY = 180f;
                    lightPitch = 60f;
                    break;

                case "Blueprint":
                    bgColor = new Color(0.04f, 0.08f, 0.18f);
                    lightColor = new Color(0.7f, 0.85f, 1f);
                    lightIntensity = 0.8f;
                    lightRotY = 90f;
                    lightPitch = 45f;
                    break;

                case "Neutral":
                default:
                    bgColor = new Color(0.15f, 0.15f, 0.16f);
                    lightColor = Color.white;
                    lightIntensity = 1f;
                    lightRotY = 0f;
                    lightPitch = 50f;
                    break;
            }

            // Apply camera background
            if (Camera.main != null)
            {
                if (presetName == "Studio")
                {
                    if (_gradientSkybox == null)
                    {
                        var shader = Shader.Find("Skybox/AnimatedGradientSkybox");
                        if (shader != null) _gradientSkybox = new Material(shader);
                    }
                    
                    if (_gradientSkybox != null)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                        RenderSettings.skybox = _gradientSkybox;
                    }
                    else
                    {
                        // Fallback if shader not found
                        Camera.main.clearFlags = CameraClearFlags.SolidColor;
                        Camera.main.backgroundColor = bgColor;
                    }
                }
                else
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = bgColor;
                    RenderSettings.skybox = null;
                }
            }

            // Apply light
            if (directionalLight != null)
            {
                directionalLight.color = lightColor;
                directionalLight.intensity = lightIntensity;
                directionalLight.transform.eulerAngles = new Vector3(lightPitch, lightRotY, 0f);
            }
        }

        public string CurrentPreset => _currentPreset;
    }
}
