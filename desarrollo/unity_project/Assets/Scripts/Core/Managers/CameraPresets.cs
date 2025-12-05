using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [System.Serializable]
    public struct CameraPreset
    {
        public string name;
        public float horizontalAngle;
        public float verticalAngle;
        public float distance;
    }

    public class CameraPresets : Singleton<CameraPresets>
    {
        [Header("Presets")]
        [SerializeField] private CameraPreset[] presets = new CameraPreset[]
        {
            new CameraPreset { name = "Front", horizontalAngle = 0f, verticalAngle = 0f, distance = 5f },
            new CameraPreset { name = "Back", horizontalAngle = 180f, verticalAngle = 0f, distance = 5f },
            new CameraPreset { name = "Left", horizontalAngle = -90f, verticalAngle = 0f, distance = 5f },
            new CameraPreset { name = "Right", horizontalAngle = 90f, verticalAngle = 0f, distance = 5f },
            new CameraPreset { name = "Top", horizontalAngle = 0f, verticalAngle = 89f, distance = 6f },
            new CameraPreset { name = "Isometric", horizontalAngle = 45f, verticalAngle = 35f, distance = 7f }
        };

        public CameraPreset[] Presets => presets;

        public void ApplyPreset(int index)
        {
            if (index < 0 || index >= presets.Length) return;
            ApplyPreset(presets[index]);
        }

        public void ApplyPreset(string name)
        {
            foreach (var preset in presets)
            {
                if (preset.name == name)
                {
                    ApplyPreset(preset);
                    return;
                }
            }
            Debug.LogWarning($"[CameraPresets] Preset '{name}' not found.");
        }

        private void ApplyPreset(CameraPreset preset)
        {
            if (OrbitCameraController.Instance == null)
            {
                Debug.LogWarning("[CameraPresets] OrbitCameraController not found.");
                return;
            }

            OrbitCameraController.Instance.SetAngles(preset.horizontalAngle, preset.verticalAngle);
            OrbitCameraController.Instance.SetDistance(preset.distance);

            Debug.Log($"[CameraPresets] Applied: {preset.name}");
        }
    }
}
