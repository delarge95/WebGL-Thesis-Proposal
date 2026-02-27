using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 0: Create fresh URP Asset at current version.
    /// Existing assets (URP_HighFidelity, URP_WebGL) may be stuck at
    /// an older version and Unity refuses to build with them.
    /// </summary>
    public static class URPAssetFixer
    {
        internal static readonly string FixedAssetPath = "Assets/Settings/URP_AutoFixed.asset";

        [MenuItem("WebGL/Fixes/0. Create Fresh URP Asset")]
        public static int Fix()
        {
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(FixedAssetPath);
            if (existing != null)
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 0: Fresh URP asset already exists at {FixedAssetPath} ✓");
                return 0;
            }

            ScriptableRendererData rendererData = FindRendererData();
            if (rendererData == null)
            {
                Debug.LogError($"{WebGLBuildFixer.LOG_PREFIX} FIX 0: No ScriptableRendererData found! Cannot create URP asset.");
                return 0;
            }

            var newAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            AssetDatabase.CreateAsset(newAsset, FixedAssetPath);
            AssetDatabase.SaveAssets();

            ConfigureURPAsset(newAsset, rendererData);

            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 0:</color> Created fresh URP asset at {FixedAssetPath} with renderer '{rendererData.name}'");
            return 1;
        }

        private static ScriptableRendererData FindRendererData()
        {
            string[] sourcePaths =
            {
                "Assets/Settings/URP_HighFidelity_Renderer.asset",
                "Assets/Settings/High_PipelineAsset_ForwardRenderer.asset",
                "Assets/Settings/Low_PipelineAsset_ForwardRenderer.asset"
            };

            foreach (var path in sourcePaths)
            {
                var data = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
                if (data != null)
                {
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 0: Found renderer data: {path}");
                    return data;
                }
            }

            var guids = AssetDatabase.FindAssets("t:ScriptableRendererData");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var data = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 0: Found renderer data (fallback): {path}");
                return data;
            }

            return null;
        }

        private static void ConfigureURPAsset(UniversalRenderPipelineAsset asset, ScriptableRendererData rendererData)
        {
            var so = new SerializedObject(asset);
            var rendererListProp = so.FindProperty("m_RendererDataList");

            rendererListProp.ClearArray();
            rendererListProp.InsertArrayElementAtIndex(0);
            rendererListProp.GetArrayElementAtIndex(0).objectReferenceValue = rendererData;

            SetPropertyIfExists(so, "m_UseSRPBatcher", true);
            SetPropertyIfExists(so, "m_SupportsHDR", false);

            var msaa = so.FindProperty("m_MSAA");
            if (msaa != null) msaa.intValue = 1;

            SetPropertyIfExists(so, "m_MainLightShadowsSupported", true);

            var shadowRes = so.FindProperty("m_MainLightShadowmapResolution");
            if (shadowRes != null) shadowRes.intValue = 1024;

            var shadowDist = so.FindProperty("m_ShadowDistance");
            if (shadowDist != null) shadowDist.floatValue = 30f;

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        private static void SetPropertyIfExists(SerializedObject so, string name, bool value)
        {
            var prop = so.FindProperty(name);
            if (prop != null) prop.boolValue = value;
        }
    }
}
