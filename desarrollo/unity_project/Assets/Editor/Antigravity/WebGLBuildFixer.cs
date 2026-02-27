using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using WebGL.Editor.Fixes;

namespace WebGL.Editor
{
    /// <summary>
    /// Lightweight orchestrator for WebGL build fixes.
    /// Individual fixes live in <see cref="WebGL.Editor.Fixes"/> namespace.
    /// </summary>
    public static class WebGLBuildFixer
    {
        internal const string LOG_PREFIX = "<color=cyan>[WebGL Fixer]</color>";

        // ═══════════════════════════════════════════════════════════════
        //  MAIN ENTRY POINT — Run ALL fixes
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/🔧 Fix ALL WebGL Issues", priority = 0)]
        public static void FixAll()
        {
            Debug.Log($"{LOG_PREFIX} ═══════════════════════════════════════════");
            Debug.Log($"{LOG_PREFIX} Starting comprehensive WebGL fix...");
            Debug.Log($"{LOG_PREFIX} ═══════════════════════════════════════════");

            int fixCount = 0;

            fixCount += URPAssetFixer.Fix();
            fixCount += URPPipelineFixer.Fix();
            fixCount += QualityLevelFixer.Fix();
            fixCount += ShaderInclusionFixer.Fix();
            fixCount += FontSetupFixer.Fix();
            fixCount += PanelSettingsFixer.Fix();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"{LOG_PREFIX} ═══════════════════════════════════════════");
            Debug.Log($"{LOG_PREFIX} <color=green>DONE!</color> Applied {fixCount} fixes.");
            Debug.Log($"{LOG_PREFIX} ═══════════════════════════════════════════");

            string message = fixCount > 0
                ? $"Applied {fixCount} fix(es).\n\nYou can now Build the project.\n\nGo to: File > Build Profiles > Build"
                : "All settings are already correct!\n\nYou can Build the project.";

            EditorUtility.DisplayDialog("WebGL Build Fixer", message, "OK");
        }

        // ═══════════════════════════════════════════════════════════════
        //  BONUS: Build WebGL directly from menu
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/🚀 Fix & Build WebGL", priority = 1)]
        public static void FixAndBuild()
        {
            FixAll();

            var buildPath = Path.GetFullPath("../../docs/Build");
            Directory.CreateDirectory(buildPath);

            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError($"{LOG_PREFIX} No scenes in Build Settings! Add your scene first.");
                EditorUtility.DisplayDialog("Build Error", "No scenes in Build Settings!\nAdd your main scene via File > Build Settings.", "OK");
                return;
            }

            Debug.Log($"{LOG_PREFIX} Building WebGL to: {buildPath}");
            Debug.Log($"{LOG_PREFIX} Scenes: {string.Join(", ", scenes)}");

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"{LOG_PREFIX} <color=green>BUILD SUCCEEDED!</color> Output: {buildPath}");
                EditorUtility.DisplayDialog("Build Complete", $"WebGL build succeeded!\n\nOutput: {buildPath}\n\nNow commit & push to deploy.", "OK");
                EditorUtility.RevealInFinder(buildPath);
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX} BUILD FAILED: {report.summary.result}");
                EditorUtility.DisplayDialog("Build Failed", $"Build failed: {report.summary.result}\n\nCheck the Console for details.", "OK");
            }
        }
    }
}
