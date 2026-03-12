using UnityEngine;

namespace WebGL.Core.Data
{
    [CreateAssetMenu(fileName = "NewDronePart", menuName = "WebGL/Drone Part Data")]
    public class DronePartData : ScriptableObject
    {
        [Header("General Info")]
        public string partName;
        public string id;
        public string partType; // Frame, Motor, Propeller, ESC, etc.
        [TextArea(3, 5)] public string description;

        [Header("Technical Specs")]
        public float weightKg;
        public string dimensions; // "10x5x3 cm"
        public string function; // What does this part do
        
        [Header("Materials")]
        public string materialType; // Carbon Fiber, Aluminum, Plastic, etc.
        public string materialProperties; // "Lightweight, High Strength"
        public string manufacturer;
        public string partNumber;

        [Header("Performance")]
        public float powerConsumption; // Watts
        public float maxLoad; // kg or N
        public float operatingTemp; // Celsius

        [Header("Thermal")]
        public float operatingTempMin = 20f;
        public float operatingTempMax = 80f;
        public float thermalHover = 25f;
        public float thermalPeak = 40f;
        public float thermalWarmupSeconds = 8f;
        public string thermalProfile;
        public string thermalMaterialKey;
        public bool isThermallyCritical = false;
        public bool inheritParentThermal = false;
        [Range(0.1f, 2f)] public float thermalMassScale = 1f;
        [Range(0f, 1f)] public float thermalExposure = 0.5f;
        [Range(0f, 2f)] public float thermalSourceWeight = 1f;
        [Range(0f, 2f)] public float thermalConductionScale = 1f;

        [Header("Visuals")]
        public Sprite icon;
        public Color highlightColor = Color.cyan;
        public Texture2D thumbnail;

        [Header("Exploded View")]
        public Vector3 explosionDirection;
        public float explosionDistance = 1.0f;
        public int explosionPriority = 0; // Order of explosion

        [Header("Modular")]
        public bool isModular = false;
        public string[] compatibleSlots;
        public DronePartData[] alternativeParts;

        [Header("Assembly Info")]
        public string[] requiredTools; // What tools needed to install this part
        public string[] safetyWarnings; // Safety precautions
        public string installationTips; // Pro tips for installation
        public float installationTimeMinutes; // Estimated time
        public int difficultyLevel; // 1-5
        public string torqueSpec; // e.g., "2.5 Nm"
        public int assemblyOrder; // Sequence in assembly
        public string[] prerequisites; // Parts that must be installed first

        [Header("Connections")]
        public string[] connectionTypes; // Screw, Snap, Solder, Wire
        public int screwCount;
        public string screwSize; // M2, M3, etc.

        // UI category field
        public string category;

        public string GetFullDescription()
        {
            return $"{partName}\n\n" +
                   $"Type: {partType}\n" +
                   $"Material: {materialType}\n" +
                   $"Weight: {weightKg:F2} kg\n" +
                   $"Dimensions: {dimensions}\n\n" +
                   $"Function: {function}\n\n" +
                   $"{description}";
        }

        public string GetAssemblyInfo()
        {
            var info = $"Assembly Information\n{new string('-', 18)}\n";
            info += $"Difficulty: {new string('\u2605', difficultyLevel)}{new string('\u2606', 5 - difficultyLevel)}\n";
            info += $"Time: ~{installationTimeMinutes:F0} minutes\n";
            
            if (requiredTools != null && requiredTools.Length > 0)
            {
                info += $"Tools: {string.Join(", ", requiredTools)}\n";
            }
            
            if (!string.IsNullOrEmpty(torqueSpec))
            {
                info += $"Torque: {torqueSpec}\n";
            }
            
            if (screwCount > 0)
            {
                info += $"Screws: {screwCount}x {screwSize}\n";
            }

            if (!string.IsNullOrEmpty(installationTips))
            {
                info += $"\nTip: {installationTips}";
            }

            return info;
        }
    }
}
