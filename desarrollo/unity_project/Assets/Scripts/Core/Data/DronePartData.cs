using UnityEngine;

namespace WebGL.Core.Data
{
    public enum PartCategory
    {
        Avionics,
        SensorsComms,
        PowerDistribution,
        PropulsionSystem,
        SkeletonAirframe,
        Fasteners,
        Uncategorized
    }
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

        [Header("Thermal Simulation")]
        public float operatingTempMin;        // °C — lower bound of normal operating range
        public float operatingTempMax;        // °C — upper bound of normal operating range
        public float thermalPeak;             // °C — absolute peak temperature under max load
        public float thermalHover;            // °C — steady-state temperature at hover load
        public float thermalWarmupSeconds;    // seconds to reach equilibrium from ambient
        public float thermalExposure;         // 0-1 — how exposed the part is to convective cooling
        public float thermalConductionScale;  // multiplier for conduction through contact links
        public float thermalSourceWeight;     // 0-1 — how strongly this part drives its own heat
        public bool  isThermallyCritical;     // true if this part should trigger thermal warnings

        [Header("Visuals")]
        public Sprite icon;
        public Color highlightColor = Color.cyan;
        public Texture2D thumbnail;
        public bool isHotspotTarget;
        public string hotspotLabel;

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
        public string[] subComponentNames;

        [Header("Fastener Metadata")]
        public FastenerMetadata fastenerMetadata;

        // UI category field
        public PartCategory category = PartCategory.Uncategorized;

        public bool HasFastenerMetadata => fastenerMetadata != null && fastenerMetadata.HasIdentifiers();

        public string GetFullDescription()
        {
             string displayWeight = FormatWeightForDisplay(weightKg);
             string displayDimensions = NormalizeTextForDisplay(dimensions);
             string displayMaterial = NormalizeTextForDisplay(materialType);
             string displayFunction = NormalizeTextForDisplay(function);
             string displayDescription = NormalizeTextForDisplay(description);

            string desc = $"{partName}\n\n" +
                   $"Type: {partType}\n" +
                 $"Material: {displayMaterial}\n" +
                 $"Weight: {displayWeight}\n" +
                 $"Dimensions: {displayDimensions}\n\n" +
                 $"Function: {displayFunction}\n\n" +
                 $"{displayDescription}";
            if (subComponentNames != null && subComponentNames.Length > 0)
            {
                desc += "\n\nAssembly includes:\n- " + string.Join("\n- ", subComponentNames);
            }

            if (HasFastenerMetadata)
            {
                string summary = fastenerMetadata.GetTechnicalSummary();
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    desc += $"\n\nFastener Spec: {summary}";
                }

                if (!string.IsNullOrWhiteSpace(fastenerMetadata.blenderName))
                {
                    desc += $"\nSource CAD: {fastenerMetadata.blenderName}";
                }
            }

            return desc;
        }

        public string GetAssemblyInfo()
        {
            var info = $"Assembly Information\n──────────────────\n";
            info += $"Difficulty: {new string('★', difficultyLevel)}{new string('☆', 5 - difficultyLevel)}\n";
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

            if (HasFastenerMetadata && !string.IsNullOrWhiteSpace(fastenerMetadata.sceneObjectName))
            {
                info += $"Instance: {fastenerMetadata.sceneObjectName}\n";
            }

            if (!string.IsNullOrEmpty(installationTips))
            {
                info += $"\n💡 Tip: {installationTips}";
            }

            return info;
        }

        private static string FormatWeightForDisplay(float valueKg)
        {
            if (valueKg <= 0f)
            {
                return "N/A";
            }

            if (valueKg < 0.01f)
            {
                return $"{valueKg * 1000f:F1} g";
            }

            return $"{valueKg:F2} kg";
        }

        private static string NormalizeTextForDisplay(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "N/A";
            }

            string normalized = value.Trim();
            return normalized == "-" ? "N/A" : normalized;
        }
    }
}
