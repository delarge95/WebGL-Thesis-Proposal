using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 2: Set WebGL to use proper Quality Level.
    /// Ensures all quality levels use the correct URP asset.
    /// </summary>
    public static class QualityLevelFixer
    {
        [MenuItem("WebGL/Fixes/2. Set WebGL Quality Level")]
        public static int Fix()
        {
            string[] candidates =
            {
                URPAssetFixer.FixedAssetPath,
                "Assets/Settings/URP_HighFidelity.asset",
                "Assets/Settings/High_PipelineAsset.asset"
            };

            RenderPipelineAsset bestAsset = null;
            foreach (var path in candidates)
            {
                var asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
                if (asset != null) { bestAsset = asset; break; }
            }

            if (bestAsset == null)
            {
                Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 2: No URP asset found for quality levels");
                return 0;
            }

            QualitySettings.renderPipeline = bestAsset;
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 2:</color> Set current quality level pipeline to '{bestAsset.name}'");

            var allNames = QualitySettings.names;
            int currentLevel = QualitySettings.GetQualityLevel();
            for (int i = 0; i < allNames.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = bestAsset;
            }
            QualitySettings.SetQualityLevel(currentLevel, false);
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 2: Applied URP asset to all {allNames.Length} quality levels");

            return 1;
        }
    }
}
