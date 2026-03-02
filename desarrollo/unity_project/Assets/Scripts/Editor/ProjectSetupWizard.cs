using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using WebGL.Core.Content;
using WebGL.UI;

namespace WebGL.Core.Utils
{
    /// <summary>
    /// Quick setup wizard for the WebGL drone visualization project.
    /// Access via Unity menu: WebGL > Project Setup Wizard
    /// </summary>
    public static class ProjectSetupWizard
    {
#if UNITY_EDITOR
        [MenuItem("WebGL/Project Setup Wizard")]
        public static void ShowWizard()
        {
            EditorUtility.DisplayDialog(
                "WebGL Drone Viewer - Setup Wizard",
                "This wizard will help you set up the project.\n\n" +
                "Steps:\n" +
                "1. Create _GameManagers object\n" +
                "2. Create MainMenu_UI object\n" +
                "3. Configure camera\n" +
                "4. Set up lighting\n\n" +
                "Use 'WebGL > Create Scene Structure' to begin.",
                "OK"
            );
        }

        [MenuItem("WebGL/Create Scene Structure")]
        public static void CreateSceneStructure()
        {
            // Create managers container
            CreateManagersObject();
            
            // Create UI container
            CreateUIContainer();
            
            // NOTE: EventSystem not needed for UI Toolkit
            
            // Create camera rig
            CreateCameraRig();
            
            // Create lighting
            CreateLighting();
            
            // Create drone container
            CreateDroneContainer();

            Debug.Log("[ProjectSetupWizard] Scene structure created successfully!");
            EditorUtility.DisplayDialog("Setup Complete", 
                "Scene structure created!\n\nNext steps:\n" +
                "1. Import your drone 3D model\n" +
                "2. Parent it under 'Drone_Root'\n" +
                "3. Add ExplodablePart to each part\n" +
                "4. Create DronePartData assets", 
                "OK");
        }

        private static void CreateManagersObject()
        {
            if (GameObject.Find("_GameManagers") != null)
            {
                Debug.Log("[ProjectSetupWizard] _GameManagers already exists.");
                return;
            }

            var go = new GameObject("_GameManagers");
            
            // Core managers
            go.AddComponent<Managers.GameManager>();
            go.AddComponent<Managers.SelectionManager>();
            go.AddComponent<Managers.AudioManager>();
            go.AddComponent<Managers.NotificationManager>();
            // go.AddComponent<Managers.SaveSystem>(); // Static class cannot be a component
            
            // Feature managers
            go.AddComponent<ExplodedViewManager>();
            go.AddComponent<Managers.ViewModeManager>();
            go.AddComponent<Managers.PartCatalogManager>();
            go.AddComponent<Managers.PartVisibilityManager>();
            go.AddComponent<Managers.CrossSectionManager>();
            go.AddComponent<Managers.DroneStateController>();
            go.AddComponent<Managers.ModularPartsSystem>();
            
            // Engineer tools
            go.AddComponent<Managers.AssemblyGuideManager>();
            go.AddComponent<Managers.MeasurementTool>();
            go.AddComponent<Managers.ConnectionPointsViewer>();
            go.AddComponent<Managers.BillOfMaterialsManager>();
            go.AddComponent<Managers.AnnotationSystem>();
            go.AddComponent<Managers.AssemblyChecklist>();

            Debug.Log("[ProjectSetupWizard] Created _GameManagers with 18 managers.");
        }

        private static void CreateUIContainer()
        {
            if (GameObject.Find("MainMenu_UI") != null)
            {
                Debug.Log("[ProjectSetupWizard] MainMenu_UI already exists.");
                return;
            }

            var go = new GameObject("MainMenu_UI");
            var uiDocument = go.AddComponent<UnityEngine.UIElements.UIDocument>();
            
            // Add UI components
            go.AddComponent<UI.UIManager>();
            go.AddComponent<UI.EnhancedInfoPanel>();
            go.AddComponent<UI.PartCatalogUI>();
            go.AddComponent<UI.AssemblyStepUI>();

            Debug.Log("[ProjectSetupWizard] Created MainMenu_UI with UI components.");
        }

        // NOTE: EventSystem NOT needed for UI Toolkit-only scenes
        // UI Toolkit has its own native event system
        // See: https://docs.unity3d.com/6000.2/Documentation/Manual/UIE-Runtime-Event-System.html

        private static void CreateCameraRig()
        {
            if (GameObject.Find("CameraRig") != null)
            {
                Debug.Log("[ProjectSetupWizard] CameraRig already exists.");
                return;
            }

            var rig = new GameObject("CameraRig");
            
            // Create camera
            var cameraGO = new GameObject("Main Camera");
            cameraGO.transform.SetParent(rig.transform);
            cameraGO.transform.localPosition = new Vector3(0, 2, -5);
            cameraGO.transform.LookAt(Vector3.zero);
            cameraGO.tag = "MainCamera";
            
            var camera = cameraGO.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 60;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            
            cameraGO.AddComponent<AudioListener>();
            cameraGO.AddComponent<Managers.OrbitCameraController>();
            cameraGO.AddComponent<Managers.CameraPresets>();

            Debug.Log("[ProjectSetupWizard] Created CameraRig with OrbitCamera.");
        }

        private static void CreateLighting()
        {
            if (GameObject.Find("Lighting") != null)
            {
                Debug.Log("[ProjectSetupWizard] Lighting already exists.");
                return;
            }

            var lighting = new GameObject("Lighting");
            
            // Main directional light
            var sunGO = new GameObject("Directional Light");
            sunGO.transform.SetParent(lighting.transform);
            sunGO.transform.rotation = Quaternion.Euler(50, -30, 0);
            
            var light = sunGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.9f);
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;

            // Fill light
            var fillGO = new GameObject("Fill Light");
            fillGO.transform.SetParent(lighting.transform);
            fillGO.transform.rotation = Quaternion.Euler(30, 150, 0);
            
            var fill = fillGO.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.7f, 0.8f, 1f);
            fill.intensity = 0.3f;
            fill.shadows = LightShadows.None;

            Debug.Log("[ProjectSetupWizard] Created Lighting setup.");
        }

        private static void CreateDroneContainer()
        {
            if (GameObject.Find("Drone_Root") != null)
            {
                Debug.Log("[ProjectSetupWizard] Drone_Root already exists.");
                return;
            }

            var droneRoot = new GameObject("Drone_Root");
            droneRoot.transform.position = Vector3.zero;
            
            // Add sample structure
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body_Sample";
            body.transform.SetParent(droneRoot.transform);
            body.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
            body.AddComponent<Content.ExplodablePart>();
            
            // Add arms
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f;
                var arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arm.name = $"Arm_{i + 1}_Sample";
                arm.transform.SetParent(droneRoot.transform);
                arm.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);
                arm.transform.localRotation = Quaternion.Euler(0, 0, 90);
                arm.transform.localPosition = Quaternion.Euler(0, angle, 0) * new Vector3(0.3f, 0, 0);
                arm.AddComponent<Content.ExplodablePart>();
            }

            Debug.Log("[ProjectSetupWizard] Created Drone_Root with sample primitives. Replace with actual model.");
        }

        [MenuItem("WebGL/Validate Scene")]
        public static void ValidateScene()
        {
            int issues = 0;
            
            // Check managers
            if (GameObject.Find("_GameManagers") == null)
            {
                Debug.LogWarning("Missing: _GameManagers");
                issues++;
            }
            
            // Check camera
            if (Camera.main == null)
            {
                Debug.LogWarning("Missing: Main Camera");
                issues++;
            }
            
            // Check UI
            if (Object.FindAnyObjectByType<UnityEngine.UIElements.UIDocument>() == null)
            {
                Debug.LogWarning("Missing: UIDocument");
                issues++;
            }
            
            // Check parts
            var parts = Object.FindObjectsByType<Content.ExplodablePart>(FindObjectsSortMode.None);
            if (parts.Length == 0)
            {
                Debug.LogWarning("No ExplodablePart components found");
                issues++;
            }
            else
            {
                Debug.Log($"Found {parts.Length} ExplodablePart components");
            }
            
            if (issues == 0)
            {
                Debug.Log("[Validation] Scene is properly configured!");
                EditorUtility.DisplayDialog("Validation Passed", 
                    $"Scene is properly configured!\n\nFound {parts.Length} drone parts.", 
                    "OK");
            }
            else
            {
                Debug.LogError($"[Validation] Found {issues} issues. Use 'WebGL > Create Scene Structure' to fix.");
            }
        }
#endif
    }
}


