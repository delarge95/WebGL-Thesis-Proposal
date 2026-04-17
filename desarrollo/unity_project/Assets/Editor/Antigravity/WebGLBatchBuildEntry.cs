using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using WebGL.Editor.Fixes;

namespace WebGL.Editor
{
    public static class WebGLBatchBuildEntry
    {
        // Metodo para ejecucion headless: Unity -executeMethod WebGL.Editor.WebGLBatchBuildEntry.BuildWebGLBatch
        public static void BuildWebGLBatch()
        {
            Debug.Log("[WebGLBatchBuild] Iniciando correcciones previas...");

            URPAssetFixer.Fix();
            URPPipelineFixer.Fix();
            QualityLevelFixer.Fix();
            ShaderInclusionFixer.Fix();
            FontSetupFixer.Fix();
            PanelSettingsFixer.Fix();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError("[WebGLBatchBuild] No hay escenas habilitadas en Build Settings.");
                EditorApplication.Exit(2);
                return;
            }

            var buildPath = Path.GetFullPath("../../docs/Build");
            Directory.CreateDirectory(buildPath);

            Debug.Log($"[WebGLBatchBuild] Construyendo WebGL en: {buildPath}");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[WebGLBatchBuild] Build exitoso.");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[WebGLBatchBuild] Build fallido: {report.summary.result}");
                EditorApplication.Exit(1);
            }
        }
    }
}
