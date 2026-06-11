using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class AssignURPConfig : MonoBehaviour
{
    [MenuItem("WebGL/Tools/Assign URP Asset")]
    public static void AssignAsset()
    {
        // 1. Load the URP Asset (Adjust path if needed)
        // We know the user created it, probably in Assets/Settings or Assets/Rendering
        // Let's try to find it by type
        string[] guids = AssetDatabase.FindAssets("t:RenderPipelineAsset URP_WebGL");
        
        if (guids.Length == 0)
        {
            Debug.LogError("URP_WebGL asset not found! Please create it via Create > Rendering > URP Asset.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        RenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
        
        if (urpAsset != null)
        {
            GraphicsSettings.defaultRenderPipeline = urpAsset;
            // QualitySettings.renderPipeline is deprecated; relying on GraphicsSettings.
            
            Debug.Log($"SUCCESS: Assigned '{urpAsset.name}' as the active Render Pipeline Asset in Graphics Settings.");
        }
        else
        {
            Debug.LogError($"Failed to load asset at {path}");
        }
    }
}
