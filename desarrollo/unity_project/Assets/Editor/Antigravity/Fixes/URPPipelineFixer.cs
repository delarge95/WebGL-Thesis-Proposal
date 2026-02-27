using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 1: Assign URP Pipeline to Graphics Settings.
    /// </summary>
    public static class URPPipelineFixer
    {
        [MenuItem("WebGL/Fixes/1. Assign URP Pipeline")]
        public static int Fix()
        {
            int fixes = 0;

            string[] candidates =
            {
                URPAssetFixer.FixedAssetPath,
                "Assets/Settings/URP_HighFidelity.asset",
                "Assets/Settings/High_PipelineAsset.asset",
                "Assets/Settings/Low_PipelineAsset.asset"
            };

            RenderPipelineAsset bestAsset = null;
            string bestPath = "";

            foreach (var path in candidates)
            {
                var asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
                if (asset != null)
                {
                    bestAsset = asset;
                    bestPath = path;
                    break;
                }
            }

            if (bestAsset == null)
            {
                var guids = AssetDatabase.FindAssets("t:RenderPipelineAsset");
                if (guids.Length > 0)
                {
                    bestPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    bestAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(bestPath);
                }
            }

            if (bestAsset == null)
            {
                Debug.LogError($"{WebGLBuildFixer.LOG_PREFIX} FIX 1: FAILED — No RenderPipelineAsset found in project!");
                return 0;
            }

            if (GraphicsSettings.defaultRenderPipeline != bestAsset)
            {
                GraphicsSettings.defaultRenderPipeline = bestAsset;
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 1a:</color> Set Graphics pipeline to '{bestAsset.name}' ({bestPath})");
                fixes++;
            }
            else
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 1a: Graphics pipeline already set to '{bestAsset.name}' ✓");
            }

            if (QualitySettings.renderPipeline != bestAsset)
            {
                QualitySettings.renderPipeline = bestAsset;
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 1b:</color> Set Quality pipeline to '{bestAsset.name}'");
                fixes++;
            }
            else
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 1b: Quality pipeline already set to '{bestAsset.name}' ✓");
            }

            return fixes;
        }
    }
}
