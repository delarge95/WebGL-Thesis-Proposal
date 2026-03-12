using UnityEngine;
using UnityEngine.SceneManagement;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using WebGL.Core.Thermal;

namespace WebGL.Core.Utils
{
    /// <summary>
    /// Scene bootstrapper that ensures all managers are properly initialized.
    /// Attach to a GameObject in the main scene.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform droneRoot;
        [SerializeField] private Transform cameraRig;
        [SerializeField] private Light mainLight;
        
        [Header("Configuration")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool showDebugInfo = true;

        private void Awake()
        {
            if (autoInitialize)
            {
                ValidateManagers();
            }
        }

        private void Start()
        {
            if (showDebugInfo)
            {
                LogSystemStatus();
            }
        }

        private void ValidateManagers()
        {
            // Core managers that should exist
            EnsureManager<GameManager>("GameManager");
            EnsureManager<SelectionManager>("SelectionManager");
            EnsureManager<AudioManager>("AudioManager");
            EnsureManager<NotificationManager>("NotificationManager");
            
            // Feature managers
            EnsureManager<ExplodedViewManager>("ExplodedViewManager");
            EnsureManager<ViewModeManager>("ViewModeManager");
            EnsureManager<PartCatalogManager>("PartCatalogManager");
            EnsureManager<PartVisibilityManager>("PartVisibilityManager");
            EnsureManager<CrossSectionManager>("CrossSectionManager");
            EnsureManager<DroneStateController>("DroneStateController");
            EnsureManager<ModularPartsSystem>("ModularPartsSystem");
            
            // Engineer tools
            EnsureManager<AssemblyGuideManager>("AssemblyGuideManager");
            EnsureManager<MeasurementTool>("MeasurementTool");
            EnsureManager<ConnectionPointsViewer>("ConnectionPointsViewer");
            EnsureManager<BillOfMaterialsManager>("BillOfMaterialsManager");
            EnsureManager<AnnotationSystem>("AnnotationSystem");
            EnsureManager<AssemblyChecklist>("AssemblyChecklist");

            // Performance
            EnsureManager<QualityManager>("QualityManager");

            // Thermal simulation
            EnsureManager<ThermalSimulationManager>("ThermalSimulationManager");
            EnsureManager<ThermalViewController>("ThermalViewController");
        }

        private void EnsureManager<T>(string name) where T : Component
        {
            if (FindAnyObjectByType<T>() == null)
            {
                Debug.LogWarning($"[SceneBootstrapper] Missing manager: {name}. Creating default.");
                var go = new GameObject($"_{name}");
                go.AddComponent<T>();
            }
        }

        private void LogSystemStatus()
        {
            Debug.Log("=== WebGL Drone Viewer - System Status ===");
            Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
            
            // Count parts
            var parts = FindObjectsByType<Content.ExplodablePart>(FindObjectsSortMode.None);
            Debug.Log($"Explodable Parts: {parts.Length}");
            
            // Check managers
            LogManagerStatus<GameManager>("GameManager");
            LogManagerStatus<SelectionManager>("SelectionManager");
            LogManagerStatus<ViewModeManager>("ViewModeManager");
            LogManagerStatus<ExplodedViewManager>("ExplodedViewManager");
            
            Debug.Log("==========================================");
        }

        private void LogManagerStatus<T>(string name) where T : Component
        {
            var manager = FindAnyObjectByType<T>();
            string status = manager != null ? "✓" : "✗";
            Debug.Log($"  {status} {name}");
        }

        [ContextMenu("Validate Scene Setup")]
        public void ValidateSceneSetup()
        {
            bool valid = true;
            
            // Check camera
            if (Camera.main == null)
            {
                Debug.LogError("[SceneBootstrapper] No main camera found!");
                valid = false;
            }
            
            // Check lighting
            if (mainLight == null)
            {
                mainLight = FindAnyObjectByType<Light>();
                if (mainLight == null)
                {
                    Debug.LogWarning("[SceneBootstrapper] No directional light found!");
                }
            }
            
            // Check drone
            if (droneRoot == null)
            {
                var parts = FindObjectsByType<Content.ExplodablePart>(FindObjectsSortMode.None);
                if (parts.Length == 0)
                {
                    Debug.LogWarning("[SceneBootstrapper] No ExplodablePart components found!");
                }
            }
            
            // Check UI
            var uiDocument = FindAnyObjectByType<UnityEngine.UIElements.UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogWarning("[SceneBootstrapper] No UIDocument found for UI Toolkit!");
            }
            
            if (valid)
            {
                Debug.Log("[SceneBootstrapper] Scene validation passed!");
            }
        }

        [ContextMenu("Create Default Managers GameObject")]
        public void CreateDefaultManagersObject()
        {
            var managersGO = new GameObject("_GameManagers");
            
            managersGO.AddComponent<GameManager>();
            managersGO.AddComponent<SelectionManager>();
            managersGO.AddComponent<AudioManager>();
            managersGO.AddComponent<NotificationManager>();
            managersGO.AddComponent<ExplodedViewManager>();
            managersGO.AddComponent<ViewModeManager>();
            managersGO.AddComponent<PartCatalogManager>();
            managersGO.AddComponent<PartVisibilityManager>();
            managersGO.AddComponent<CrossSectionManager>();
            managersGO.AddComponent<DroneStateController>();
            managersGO.AddComponent<ModularPartsSystem>();
            managersGO.AddComponent<AssemblyGuideManager>();
            managersGO.AddComponent<MeasurementTool>();
            managersGO.AddComponent<ConnectionPointsViewer>();
            managersGO.AddComponent<BillOfMaterialsManager>();
            managersGO.AddComponent<AnnotationSystem>();
            managersGO.AddComponent<AssemblyChecklist>();
            
            Debug.Log("[SceneBootstrapper] Created _GameManagers with all manager components.");
        }
    }
}
