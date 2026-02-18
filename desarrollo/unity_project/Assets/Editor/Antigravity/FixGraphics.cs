using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WebGL.Editor
{
    public class FixGraphics : MonoBehaviour
    {
        [MenuItem("WebGL/Fixes/Assign URP Asset (Fix Invisible Model)")]
        public static void Fix()
        {
            // 1. Find the URP Asset
            string assetPath = "Assets/Settings/URP_HighFidelity.asset";
            var asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(assetPath);
            
            if (asset == null)
            {
                // Fallback to Balanced if HighFidelity missing
                assetPath = "Assets/Settings/URP_Balanced.asset";
                asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(assetPath);
            }

            if (asset != null)
            {
                // 2. Assign to Graphics Settings (Project-wide)
                GraphicsSettings.defaultRenderPipeline = asset;
                
                // 3. Assign to Quality Settings (Current Level)
                QualitySettings.renderPipeline = asset;

                Debug.Log($"<color=green>SUCCESS:</color> Assigned '{asset.name}' to Graphics & Quality Settings.");
                EditorUtility.DisplayDialog("Graphics Fix", $"Fixed! Assigned '{asset.name}' to your Graphics Settings.\n\nYou can now Build the project.", "OK");
            }
            else
            {
                Debug.LogError("COULD NOT FIND URP ASSET! Checked: URP_HighFidelity and URP_Balanced in Assets/Settings/.");
                EditorUtility.DisplayDialog("Graphics Fix", "Error: Could not find 'URP_HighFidelity.asset'. Please check the Console.", "OK");
            }
        }
    }
}
