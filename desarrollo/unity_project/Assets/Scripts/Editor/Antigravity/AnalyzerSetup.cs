using UnityEngine;
using UnityEditor;

namespace WebGL.Editor.Antigravity
{
    public class AnalyzerSetup : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.Contains("UnityArchitectGuard.dll"))
                {
                    PluginImporter plugin = AssetImporter.GetAtPath(str) as PluginImporter;
                    if (plugin != null)
                    {
                        // Analyzers should NOT be included in build or editor runtime
                        plugin.SetCompatibleWithAnyPlatform(false);
                        plugin.SetCompatibleWithEditor(false); 
                        // It seems we can't easily set "Roslyn Analyzer" category via API in all versions
                        // But disabling it from platforms prevents the runtime error.
                        // Unity might fully support "Analyzer" label via label setting?
                        
                        // Let's try to set the label "RoslynAnalyzer" which Unity uses to identify them
                        AssetDatabase.SetLabels(plugin, new string[] { "RoslynAnalyzer" });
                        
                        Debug.Log("[Antigravity] Configured UnityArchitectGuard.dll as Roslyn Analyzer candidate.");
                    }
                }
            }
        }
    }
}
