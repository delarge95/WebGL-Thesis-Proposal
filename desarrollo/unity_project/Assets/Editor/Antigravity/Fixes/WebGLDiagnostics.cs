using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.Linq;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// Diagnostic utility — prints current pipeline, font, and build state.
    /// </summary>
    public static class WebGLDiagnostics
    {
        [MenuItem("WebGL/📋 Diagnose Current Settings")]
        public static void Run()
        {
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} ═══════════ DIAGNOSTIC REPORT ═══════════");

            var gfxPipeline = GraphicsSettings.defaultRenderPipeline;
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Graphics Pipeline: {(gfxPipeline != null ? gfxPipeline.name : "<color=red>NONE</color>")}");

            var qualityPipeline = QualitySettings.renderPipeline;
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Quality Pipeline (Level {QualitySettings.GetQualityLevel()}): {(qualityPipeline != null ? qualityPipeline.name : "<color=red>NONE</color>")}");

            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Quality Level Names: {string.Join(", ", QualitySettings.names)}");
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Current Quality Level: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");

            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainPanelSettings.asset");
            if (panel != null)
            {
                var so = new SerializedObject(panel);
                var textProp = so.FindProperty("textSettings");
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} PanelSettings TextSettings: {(textProp?.objectReferenceValue != null ? textProp.objectReferenceValue.name : "<color=red>NULL</color>")}");
            }

            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shaders" });
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Custom Shaders in Assets/Shaders: {shaderGuids.Length}");

            var fontGuids = AssetDatabase.FindAssets("t:Font", new[] { "Assets" });
            var fontAssetGuids = AssetDatabase.FindAssets("t:FontAsset", new[] { "Assets" });
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Fonts in Assets: {fontGuids.Length} Font files, {fontAssetGuids.Length} FontAssets");

            var scenes = EditorBuildSettings.scenes;
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Build Scenes: {scenes.Length} ({scenes.Count(s => s.enabled)} enabled)");
            foreach (var scene in scenes)
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX}   {(scene.enabled ? "✓" : "✗")} {scene.path}");

            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} ═══════════════════════════════════════════");
        }
    }
}
