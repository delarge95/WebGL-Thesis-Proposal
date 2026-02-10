using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using WebGL.Core.Data;

namespace WebGL.Editor.Antigravity
{
    public static class AssetFactory
    {
        [MenuItem("Antigravity/Create Rescue Assets")]
        public static void CreateRescueAssets()
        {
            CreatePanelSettings();
        }

        [MenuItem("Antigravity/Create Dummy Assets")]
        public static void CreateDummyAssets()
        {
            CreateDummyPartData();
        }

        public static DronePartData CreateDummyPartData()
        {
            string path = "Assets/Core/Data/DummyPart.asset";
            
            // Ensure directory
            if (!System.IO.Directory.Exists(Application.dataPath + "/Core/Data"))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Core/Data");
            }

            var data = AssetDatabase.LoadAssetAtPath<DronePartData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<DronePartData>();
                data.partName = "Test Cube Module";
                data.category = "Structure"; // Note: lowercase field, PROPERTY is capitalized
                data.description = "A standard issue testing cube. Material is unknown but highly reflective. Handle with care.";
                data.partType = "Generic";
                data.weight = 1.5f;
                data.material = "Poly-Alloy";
                
                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.SaveAssets();
                Debug.Log($"[AssetFactory] Created DummyPart at {path}");
            }
            return data;
        }

        public static void CreatePanelSettings()
        {
            string path = "Assets/UI/MainPanelSettings.asset";
            
            // Check if exists
            if (AssetDatabase.LoadAssetAtPath<PanelSettings>(path) != null)
            {
                Debug.Log($"[Antigravity] PanelSettings already exists at {path}");
                return;
            }

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            
            // Standard WebGL settings
            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0.5f; // Balance between width/height
            settings.referenceResolution = new Vector2Int(1920, 1080);
            
            // Critical for Mobile: High DPI handling
            // Unfortunately PanelSettings API for 'fallbackDpi' is hidden in some versions,
            // but ScaleWithScreenSize handles most of it.
            
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Antigravity] Created PanelSettings at {path}");
        }

        [MenuItem("Antigravity/Fix Analyzer Settings")]
        public static void FixAnalyzerSettings()
        {
            string path = "Assets/Analyzers/UnityArchitectGuard.dll";
            var importer = AssetImporter.GetAtPath(path) as PluginImporter;
            
            if (importer == null)
            {
                Debug.LogError($"[Antigravity] Analyzer DLL not found at {path}");
                return;
            }

            // Uncheck "Any Platform"
            importer.SetCompatibleWithAnyPlatform(false);
            
            // Explicitly disable for all common platforms to be safe
            importer.SetCompatibleWithEditor(false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
            importer.SetCompatibleWithPlatform(BuildTarget.WebGL, false);
            importer.SetCompatibleWithPlatform(BuildTarget.Android, false);
            importer.SetCompatibleWithPlatform(BuildTarget.iOS, false);
            importer.SetCompatibleWithPlatform(BuildTarget.NoTarget, false);

            // Force "RoslynAnalyzer" label just in case
            AssetDatabase.SetLabels(importer, new string[] { "RoslynAnalyzer" });
            
            importer.SaveAndReimport();
            Debug.Log("[Antigravity] Analyzer settings fixed. It should now only work as a Roslyn Analyzer.");
        }
    }
}
