using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;

namespace WebGL.Core.Utils
{
    public class DroneAssembler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool assembleOnStart = false;
        [SerializeField] private bool applyRealisticMaterials = true;
        
        private void Start()
        {
            if (assembleOnStart)
            {
                AssembleDrone();
            }

            if (applyRealisticMaterials)
            {
                ApplyRealisticMaterialsToExisting();
            }
        }

        /// <summary>
        /// Re-applies realistic PBR materials to already-serialized parts.
        /// </summary>
        private void ApplyRealisticMaterialsToExisting()
        {
            var parts = GetComponentsInChildren<ExplodablePart>();
            foreach (var part in parts)
            {
                var rend = part.GetComponent<Renderer>();
                if (rend == null) continue;

                string category = part.Data != null ? part.Data.category : "";
                string partName = part.gameObject.name;

                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                ConfigureRealisticMaterial(mat, category, partName);
                rend.material = mat;
            }
        }

        public void AssembleDrone()
        {
            if (transform.Find("ProceduralDrone") != null) return;

            GameObject droneRoot = new GameObject("ProceduralDrone");
            droneRoot.transform.SetParent(transform, false);

            // ── 1. Chassis (Structure) ──
            CreatePart(droneRoot, "Chassis_Main", PrimitiveType.Cube, 
                new Vector3(0, 0, 0), new Vector3(1f, 0.4f, 2f), 
                "Structure", 
                "Main structural frame providing rigidity and mounting points for all subsystems. Made from aerospace-grade carbon fiber composite.",
                "Carbon Fiber", Vector3.up, 0.5f,
                weightKg: 0.32f, dimensions: "25 x 10 x 50 cm",
                powerConsumption: 0f, operatingTemp: 85f,
                difficulty: 2, tools: new[] { "M3 Hex Key", "Thread Locker" },
                assemblyTime: 15f, function: "Primary structural frame");

            // ── 2. Flight Controller (Avionics) ──
            CreatePart(droneRoot, "Flight_Controller", PrimitiveType.Cube, 
                new Vector3(0, 0.25f, 0), new Vector3(0.4f, 0.1f, 0.4f), 
                "Avionics", 
                "32-bit ARM Cortex-M7 flight controller with dual IMU, barometer, and GPS. Runs ArduPilot firmware with PID stabilization.",
                "FR-4 PCB", Vector3.up, 1.0f,
                weightKg: 0.035f, dimensions: "4.5 x 4.5 x 1.2 cm",
                powerConsumption: 2.5f, operatingTemp: 70f,
                difficulty: 3, tools: new[] { "M2 Screwdriver", "Anti-static Strap" },
                assemblyTime: 10f, function: "Flight stabilization & autopilot");

            // ── 3. Battery (Power) ──
            CreatePart(droneRoot, "Smart_Battery", PrimitiveType.Cube, 
                new Vector3(0, 0.1f, -0.5f), new Vector3(0.6f, 0.3f, 0.8f), 
                "Power", 
                "4S LiPo 5000mAh smart battery with integrated BMS, cell balancing, and voltage monitoring. Provides 18-25 minutes flight time.",
                "Lithium Polymer", Vector3.back + Vector3.up, 1.2f,
                weightKg: 0.52f, dimensions: "14 x 5 x 4.5 cm",
                powerConsumption: 0f, operatingTemp: 45f,
                difficulty: 1, tools: new[] { "Velcro Strap" },
                assemblyTime: 2f, function: "Primary power source (14.8V)");

            // ── 4. Arms (Structure) ──
            CreatePart(droneRoot, "Arm_FrontLeft", PrimitiveType.Capsule, 
                new Vector3(-0.8f, 0, 0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Carbon fiber tubular arm providing motor mount and wiring conduit.",
                "Carbon Fiber Tube", new Vector3(-1, 1, 1), 1.5f, Quaternion.Euler(90, 45, 0),
                weightKg: 0.045f, dimensions: "22 x 2 x 2 cm",
                powerConsumption: 0f, operatingTemp: 80f,
                difficulty: 2, tools: new[] { "M3 Hex Key" },
                assemblyTime: 5f, function: "Motor mount & wiring conduit");
                
            CreatePart(droneRoot, "Arm_FrontRight", PrimitiveType.Capsule, 
                new Vector3(0.8f, 0, 0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Carbon fiber tubular arm providing motor mount and wiring conduit.",
                "Carbon Fiber Tube", new Vector3(1, 1, 1), 1.5f, Quaternion.Euler(90, -45, 0),
                weightKg: 0.045f, dimensions: "22 x 2 x 2 cm",
                powerConsumption: 0f, operatingTemp: 80f,
                difficulty: 2, tools: new[] { "M3 Hex Key" },
                assemblyTime: 5f, function: "Motor mount & wiring conduit");

            CreatePart(droneRoot, "Arm_BackLeft", PrimitiveType.Capsule, 
                new Vector3(-0.8f, 0, -0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Carbon fiber tubular arm providing motor mount and wiring conduit.",
                "Carbon Fiber Tube", new Vector3(-1, 1, -1), 1.5f, Quaternion.Euler(90, -45, 0),
                weightKg: 0.045f, dimensions: "22 x 2 x 2 cm",
                powerConsumption: 0f, operatingTemp: 80f,
                difficulty: 2, tools: new[] { "M3 Hex Key" },
                assemblyTime: 5f, function: "Motor mount & wiring conduit");

            CreatePart(droneRoot, "Arm_BackRight", PrimitiveType.Capsule, 
                new Vector3(0.8f, 0, -0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Carbon fiber tubular arm providing motor mount and wiring conduit.",
                "Carbon Fiber Tube", new Vector3(1, 1, -1), 1.5f, Quaternion.Euler(90, 45, 0),
                weightKg: 0.045f, dimensions: "22 x 2 x 2 cm",
                powerConsumption: 0f, operatingTemp: 80f,
                difficulty: 2, tools: new[] { "M3 Hex Key" },
                assemblyTime: 5f, function: "Motor mount & wiring conduit");

            // ── 5. Motors (Propulsion) ──
            CreatePart(droneRoot, "Motor_FL", PrimitiveType.Cylinder, 
                new Vector3(-1.2f, 0.1f, 1.2f), new Vector3(0.4f, 0.2f, 0.4f), 
                "Propulsion", 
                "Brushless outrunner motor 2300KV. N52 magnets, 12N14P configuration. Max thrust: 1.2kg per motor.",
                "Aluminum Alloy", Vector3.up, 2.0f,
                weightKg: 0.065f, dimensions: "2.8 x 2.8 x 3.2 cm",
                powerConsumption: 180f, operatingTemp: 65f,
                difficulty: 2, tools: new[] { "M3 Socket Wrench", "Loctite" },
                assemblyTime: 8f, function: "Thrust generation (CW)");
                
            CreatePart(droneRoot, "Motor_FR", PrimitiveType.Cylinder, 
                new Vector3(1.2f, 0.1f, 1.2f), new Vector3(0.4f, 0.2f, 0.4f), 
                "Propulsion", 
                "Brushless outrunner motor 2300KV. N52 magnets, 12N14P configuration. Max thrust: 1.2kg per motor.",
                "Aluminum Alloy", Vector3.up, 2.0f,
                weightKg: 0.065f, dimensions: "2.8 x 2.8 x 3.2 cm",
                powerConsumption: 180f, operatingTemp: 65f,
                difficulty: 2, tools: new[] { "M3 Socket Wrench", "Loctite" },
                assemblyTime: 8f, function: "Thrust generation (CCW)");

            // ── 6. Camera (Avionics) ──
            CreatePart(droneRoot, "Gimbal_Camera", PrimitiveType.Sphere, 
                new Vector3(0, -0.3f, 1.1f), new Vector3(0.4f, 0.4f, 0.4f), 
                "Avionics", 
                "3-axis stabilized gimbal with 4K/60fps camera. 1/2.3\" CMOS sensor with 12MP stills. ±0.01° stabilization accuracy.",
                "Magnesium Alloy", Vector3.forward + Vector3.down, 1.5f,
                weightKg: 0.12f, dimensions: "4.5 x 4.5 x 5.0 cm",
                powerConsumption: 8f, operatingTemp: 50f,
                difficulty: 4, tools: new[] { "M2 Hex Key", "Ribbon Cable Tool", "Calibration Software" },
                assemblyTime: 20f, function: "Aerial imaging & FPV feed");
        }

        private void CreatePart(GameObject parent, string name, PrimitiveType shape, 
            Vector3 pos, Vector3 scale, 
            string category, string desc, string materialType,
            Vector3 explodeDir, float explodeDist, 
            Quaternion rotation = default,
            float weightKg = 0f, string dimensions = "",
            float powerConsumption = 0f, float operatingTemp = 0f,
            int difficulty = 1, string[] tools = null,
            float assemblyTime = 5f, string function = "")
        {
            // 1. Create Object
            GameObject obj = GameObject.CreatePrimitive(shape);
            obj.name = name;
            obj.transform.SetParent(parent.transform, false);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            if (rotation != default) obj.transform.localRotation = rotation;

            // 2. Setup Material — realistic PBR per category
            Renderer rend = obj.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            ConfigureRealisticMaterial(mat, category, name);
            rend.material = mat;

            // 3. Create Rich Data
            DronePartData data = ScriptableObject.CreateInstance<DronePartData>();
            data.partName = name.Replace("_", " ");
            data.category = category;
            data.partType = category;
            data.description = desc;
            data.materialType = materialType;
            data.function = function;
            data.weightKg = weightKg;
            data.dimensions = dimensions;
            data.powerConsumption = powerConsumption;
            data.operatingTemp = operatingTemp;
            data.difficultyLevel = difficulty;
            data.requiredTools = tools ?? new string[0];
            data.installationTimeMinutes = assemblyTime;
            data.explosionDirection = explodeDir;
            data.explosionDistance = explodeDist;
            
            // 4. Add Components
            obj.AddComponent<MaterialController>(); 
            obj.AddComponent<HighlightSystem>();

            ExplodablePart explodable = obj.AddComponent<ExplodablePart>();
            explodable.SetData(data);
            explodable.Initialize();
            
            ExplodedViewManager.Instance?.RegisterPart(explodable);
        }

        /// <summary>
        /// Configures realistic PBR material properties per part category.
        /// These are temporary test materials to showcase different surface types.
        /// </summary>
        private void ConfigureRealisticMaterial(Material mat, string category, string partName)
        {
            // Enable smoothness from albedo alpha (SurfaceType Opaque is default)
            mat.SetFloat("_WorkflowMode", 1f); // Metallic workflow

            switch (category)
            {
                case "Structure":
                    // Carbon fiber composite — lighter charcoal for better visibility
                    mat.color = new Color(0.32f, 0.32f, 0.35f, 1f);
                    mat.SetFloat("_Metallic", 0.15f);
                    mat.SetFloat("_Smoothness", 0.75f);
                    break;

                case "Propulsion":
                    // Brushed aluminum alloy — metallic
                    mat.color = new Color(0.7f, 0.72f, 0.74f, 1f);
                    mat.SetFloat("_Metallic", 0.9f);
                    mat.SetFloat("_Smoothness", 0.55f);
                    break;

                case "Power":
                    // Battery — dark grey plastic (lighter than before)
                    mat.color = new Color(0.22f, 0.22f, 0.25f, 1f);
                    mat.SetFloat("_Metallic", 0f);
                    mat.SetFloat("_Smoothness", 0.35f);
                    break;

                case "Avionics":
                    if (partName.Contains("Gimbal") || partName.Contains("Camera"))
                    {
                        // Camera lens housing — polycarbonate (glossy, lighter)
                        mat.color = new Color(0.35f, 0.37f, 0.40f, 1f);
                        mat.SetFloat("_Metallic", 0.05f);
                        mat.SetFloat("_Smoothness", 0.92f);
                        mat.SetColor("_SpecColor", new Color(0.6f, 0.6f, 0.65f, 1f));
                    }
                    else
                    {
                        // Flight controller PCB — green FR-4 fiberglass
                        mat.color = new Color(0.05f, 0.28f, 0.08f, 1f);
                        mat.SetFloat("_Metallic", 0.1f);
                        mat.SetFloat("_Smoothness", 0.6f);
                    }
                    break;

                default:
                    mat.color = new Color(0.6f, 0.6f, 0.62f, 1f);
                    mat.SetFloat("_Metallic", 0f);
                    mat.SetFloat("_Smoothness", 0.5f);
                    break;
            }
        }
    }
}
