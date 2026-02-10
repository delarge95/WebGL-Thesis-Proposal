using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;

namespace WebGL.Core.Utils
{
    public class DroneAssembler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool assembleOnStart = false;
        
        private void Start()
        {
            if (assembleOnStart)
            {
                AssembleDrone();
            }
        }

        public void AssembleDrone()
        {
            // Check if drone already exists to avoid duplicates
            if (transform.Find("ProceduralDrone") != null) return;

            GameObject droneRoot = new GameObject("ProceduralDrone");
            droneRoot.transform.SetParent(transform, false);

            // 1. Chassis (Structure) - Center
            CreatePart(droneRoot, "Chassis_Main", PrimitiveType.Cube, 
                new Vector3(0, 0, 0), new Vector3(1f, 0.4f, 2f), 
                "Structure", "Carbon Fiber Body", Vector3.up, 0.5f);

            // 2. Flight Controller (Avionics) - Inside/Top
            CreatePart(droneRoot, "Flight_Controller", PrimitiveType.Cube, 
                new Vector3(0, 0.25f, 0), new Vector3(0.4f, 0.1f, 0.4f), 
                "Avionics", "Brain of the drone", Vector3.up, 1.0f);

            // 3. Battery (Power) - Back
            CreatePart(droneRoot, "Smart_Battery", PrimitiveType.Cube, 
                new Vector3(0, 0.1f, -0.5f), new Vector3(0.6f, 0.3f, 0.8f), 
                "Power", "LiPo 4S 5000mAh", Vector3.back + Vector3.up, 1.2f);

            // 4. Arms (Structure)
            CreatePart(droneRoot, "Arm_FrontLeft", PrimitiveType.Capsule, 
                new Vector3(-0.8f, 0, 0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Arm Support", new Vector3(-1, 1, 1), 1.5f, Quaternion.Euler(90, 45, 0));
                
            CreatePart(droneRoot, "Arm_FrontRight", PrimitiveType.Capsule, 
                new Vector3(0.8f, 0, 0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Arm Support", new Vector3(1, 1, 1), 1.5f, Quaternion.Euler(90, -45, 0));

            CreatePart(droneRoot, "Arm_BackLeft", PrimitiveType.Capsule, 
                new Vector3(-0.8f, 0, -0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Arm Support", new Vector3(-1, 1, -1), 1.5f, Quaternion.Euler(90, -45, 0));

            CreatePart(droneRoot, "Arm_BackRight", PrimitiveType.Capsule, 
                new Vector3(0.8f, 0, -0.8f), new Vector3(0.2f, 1f, 0.2f), 
                "Structure", "Arm Support", new Vector3(1, 1, -1), 1.5f, Quaternion.Euler(90, 45, 0));

            // 5. Motors (Propulsion)
            CreatePart(droneRoot, "Motor_FL", PrimitiveType.Cylinder, 
                new Vector3(-1.2f, 0.1f, 1.2f), new Vector3(0.4f, 0.2f, 0.4f), 
                "Propulsion", "Brushless Motor 2300KV", Vector3.up, 2.0f);
                
             CreatePart(droneRoot, "Motor_FR", PrimitiveType.Cylinder, 
                new Vector3(1.2f, 0.1f, 1.2f), new Vector3(0.4f, 0.2f, 0.4f), 
                "Propulsion", "Brushless Motor 2300KV", Vector3.up, 2.0f);

            // 6. Camera (Avionics)
            CreatePart(droneRoot, "Gimbal_Camera", PrimitiveType.Sphere, 
                new Vector3(0, -0.3f, 1.1f), new Vector3(0.4f, 0.4f, 0.4f), 
                "Avionics", "4K Stabilized Camera", Vector3.forward + Vector3.down, 1.5f);
        }

        private void CreatePart(GameObject parent, string name, PrimitiveType shape, 
            Vector3 pos, Vector3 scale, 
            string category, string desc, 
            Vector3 explodeDir, float explodeDist, 
            Quaternion rotation = default)
        {
            // 1. Create Object
            GameObject obj = GameObject.CreatePrimitive(shape);
            obj.name = name;
            obj.transform.SetParent(parent.transform, false);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            if (rotation != default) obj.transform.localRotation = rotation;

            // 2. Setup Material (Basic Color based on Category for visual debug)
            Renderer rend = obj.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Or Standard
            if (category == "Structure") mat.color = Color.gray;
            if (category == "Propulsion") mat.color = new Color(0.2f, 0.2f, 0.8f); // Blue
            if (category == "Power") mat.color = new Color(1f, 0.5f, 0f); // Orange
            if (category == "Avionics") mat.color = new Color(0f, 0.8f, 0.2f); // Green
            rend.material = mat;

            // 3. Create Data
            DronePartData data = ScriptableObject.CreateInstance<DronePartData>();
            data.partName = name.Replace("_", " ");
            data.category = category;
            data.partType = category; // Simplify for now
            data.description = desc;
            data.explosionDirection = explodeDir;
            data.explosionDistance = explodeDist;
            
            // 4. Add Components
            
            // Highlight System (Requires MaterialController)
            obj.AddComponent<MaterialController>(); 
            obj.AddComponent<HighlightSystem>();

            // Selection Logic
            // The system uses DronePart component? Let's check SelectionManager references.
            // Assuming we look for Colliders.
            
            // Explodable
            ExplodablePart explodable = obj.AddComponent<ExplodablePart>();
            explodable.SetData(data);
            explodable.Initialize();
            
            // Register with Manager? 
            // ExplodedViewManager finds them on Start(). If we run this on Start() of Assembler,
            // we need to make sure Assembler runs BEFORE ExplodedViewManager, 
            // OR we assume ExplodedViewManager also finds generic ExplodableParts.
            
            // To be safe, let's manually register if Manager exists
            if (ExplodedViewManager.Instance != null)
            {
                ExplodedViewManager.Instance.RegisterPart(explodable);
            }
            
            if (ExplodedViewManager.Instance != null)
            {
                ExplodedViewManager.Instance.RegisterPart(explodable);
            }
        }
    }
}
