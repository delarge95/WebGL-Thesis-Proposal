using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WebGL.Editor
{
    /// <summary>
    /// One-click WebGL build fixer.
    /// Addresses ALL known issues that cause invisible models and missing UI in WebGL builds.
    /// </summary>
    public static class WebGLBuildFixer
    {
        private const string LOG_PREFIX = "<color=cyan>[WebGL Fixer]</color>";

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

            fixCount += Fix0_UpgradeURPAssets();
            fixCount += Fix1_AssignURPPipeline();
            fixCount += Fix2_SetWebGLQualityLevel();
            fixCount += Fix3_IncludeCustomShaders();
            fixCount += Fix4_SetupFont();
            fixCount += Fix5_ValidatePanelSettings();

            // Save everything
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
        //  FIX 0: Create fresh URP Asset at current version
        //  Existing assets (URP_HighFidelity, URP_WebGL) are stuck at
        //  v13 and Unity refuses to build with them.
        // ═══════════════════════════════════════════════════════════════

        private static string _fixedAssetPath = "Assets/Settings/URP_AutoFixed.asset";

        [MenuItem("WebGL/Fixes/0. Create Fresh URP Asset")]
        public static int Fix0_UpgradeURPAssets()
        {
            int fixes = 0;

            // Check if we already created a fresh asset
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(_fixedAssetPath);
            if (existing != null)
            {
                Debug.Log($"{LOG_PREFIX} FIX 0: Fresh URP asset already exists at {_fixedAssetPath} ✓");
                return 0;
            }

            // Find the renderer data from existing URP_HighFidelity
            ScriptableRendererData rendererData = null;
            string[] sourcePaths = {
                "Assets/Settings/URP_HighFidelity_Renderer.asset",
                "Assets/Settings/High_PipelineAsset_ForwardRenderer.asset",
                "Assets/Settings/Low_PipelineAsset_ForwardRenderer.asset"
            };

            foreach (var path in sourcePaths)
            {
                rendererData = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
                if (rendererData != null)
                {
                    Debug.Log($"{LOG_PREFIX} FIX 0: Found renderer data: {path}");
                    break;
                }
            }

            // Fallback: search project
            if (rendererData == null)
            {
                var guids = AssetDatabase.FindAssets("t:ScriptableRendererData");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    rendererData = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
                    Debug.Log($"{LOG_PREFIX} FIX 0: Found renderer data (fallback): {path}");
                }
            }

            if (rendererData == null)
            {
                Debug.LogError($"{LOG_PREFIX} FIX 0: No ScriptableRendererData found! Cannot create URP asset.");
                return 0;
            }

            // Create a brand new URP asset — this will be at the CURRENT code version
            var newAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            AssetDatabase.CreateAsset(newAsset, _fixedAssetPath);
            AssetDatabase.SaveAssets();

            // Now assign the renderer data via SerializedObject
            var so = new SerializedObject(newAsset);
            var rendererListProp = so.FindProperty("m_RendererDataList");
            
            // Clear and set
            rendererListProp.ClearArray();
            rendererListProp.InsertArrayElementAtIndex(0);
            rendererListProp.GetArrayElementAtIndex(0).objectReferenceValue = rendererData;

            // Set sensible WebGL defaults
            var srpBatcher = so.FindProperty("m_UseSRPBatcher");
            if (srpBatcher != null) srpBatcher.boolValue = true;

            var hdr = so.FindProperty("m_SupportsHDR");
            if (hdr != null) hdr.boolValue = false; // WebGL: disable HDR for performance

            var msaa = so.FindProperty("m_MSAA");
            if (msaa != null) msaa.intValue = 1; // No MSAA for WebGL

            var mainLightShadows = so.FindProperty("m_MainLightShadowsSupported");
            if (mainLightShadows != null) mainLightShadows.boolValue = true;

            var shadowRes = so.FindProperty("m_MainLightShadowmapResolution");
            if (shadowRes != null) shadowRes.intValue = 1024;

            var shadowDist = so.FindProperty("m_ShadowDistance");
            if (shadowDist != null) shadowDist.floatValue = 30f;

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(newAsset);
            AssetDatabase.SaveAssets();

            Debug.Log($"{LOG_PREFIX} <color=green>FIX 0:</color> Created fresh URP asset at {_fixedAssetPath} with renderer '{rendererData.name}'");
            fixes++;

            return fixes;
        }

        // ═══════════════════════════════════════════════════════════════
        //  FIX 1: Assign URP Pipeline to Graphics Settings
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/Fixes/1. Assign URP Pipeline")]
        public static int Fix1_AssignURPPipeline()
        {
            int fixes = 0;

            // Priority: URP_AutoFixed (fresh) > URP_HighFidelity > High > Low
            string[] candidates = {
                _fixedAssetPath,
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
                // Search entire project
                var guids = AssetDatabase.FindAssets("t:RenderPipelineAsset");
                if (guids.Length > 0)
                {
                    bestPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    bestAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(bestPath);
                }
            }

            if (bestAsset != null)
            {
                if (GraphicsSettings.defaultRenderPipeline != bestAsset)
                {
                    GraphicsSettings.defaultRenderPipeline = bestAsset;
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 1a:</color> Set Graphics pipeline to '{bestAsset.name}' ({bestPath})");
                    fixes++;
                }
                else
                {
                    Debug.Log($"{LOG_PREFIX} FIX 1a: Graphics pipeline already set to '{bestAsset.name}' ✓");
                }

                // Also set current quality level
                if (QualitySettings.renderPipeline != bestAsset)
                {
                    QualitySettings.renderPipeline = bestAsset;
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 1b:</color> Set Quality pipeline to '{bestAsset.name}'");
                    fixes++;
                }
                else
                {
                    Debug.Log($"{LOG_PREFIX} FIX 1b: Quality pipeline already set to '{bestAsset.name}' ✓");
                }
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX} FIX 1: FAILED — No RenderPipelineAsset found in project!");
            }

            return fixes;
        }

        // ═══════════════════════════════════════════════════════════════
        //  FIX 2: Set WebGL to use proper Quality Level
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/Fixes/2. Set WebGL Quality Level")]
        public static int Fix2_SetWebGLQualityLevel()
        {
            // WebGL is currently set to quality level 0 (Low).
            // We need to ensure it uses a level with a proper URP asset.
            // The best approach: assign the correct URP asset to ALL quality levels.

            int fixes = 0;
            string[] candidates = {
                _fixedAssetPath,
                "Assets/Settings/URP_HighFidelity.asset",
                "Assets/Settings/High_PipelineAsset.asset"
            };

            RenderPipelineAsset bestAsset = null;
            foreach (var path in candidates)
            {
                var asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
                if (asset != null) { bestAsset = asset; break; }
            }

            if (bestAsset != null)
            {
                // Set for all quality levels via SerializedObject
                var qualitySettingsPath = "ProjectSettings/QualitySettings.asset";
                var qualityAsset = AssetDatabase.LoadAllAssetsAtPath(qualitySettingsPath);

                // Use QualitySettings API for current level
                QualitySettings.renderPipeline = bestAsset;
                Debug.Log($"{LOG_PREFIX} <color=green>FIX 2:</color> Set current quality level pipeline to '{bestAsset.name}'");
                fixes++;

                // Force set for all levels via reflection/serialized property
                var allNames = QualitySettings.names;
                int currentLevel = QualitySettings.GetQualityLevel();
                for (int i = 0; i < allNames.Length; i++)
                {
                    QualitySettings.SetQualityLevel(i, false);
                    QualitySettings.renderPipeline = bestAsset;
                }
                QualitySettings.SetQualityLevel(currentLevel, false);
                Debug.Log($"{LOG_PREFIX} FIX 2: Applied URP asset to all {allNames.Length} quality levels");
            }
            else
            {
                Debug.LogWarning($"{LOG_PREFIX} FIX 2: No URP asset found for quality levels");
            }

            return fixes;
        }

        // ═══════════════════════════════════════════════════════════════
        //  FIX 3: Include Custom Shaders in "Always Included"
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/Fixes/3. Include Custom Shaders")]
        public static int Fix3_IncludeCustomShaders()
        {
            int fixes = 0;

            // Find all custom shaders in Assets/Shaders
            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shaders" });
            if (shaderGuids.Length == 0)
            {
                Debug.Log($"{LOG_PREFIX} FIX 3: No custom shaders found in Assets/Shaders ✓");
                return 0;
            }

            // Get current "Always Included Shaders" via SerializedObject
            var graphicsSettingsObj = new SerializedObject(
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("ProjectSettings/GraphicsSettings.asset")
                    ?? UnityEditor.Unsupported.GetSerializedAssetInterfaceSingleton("GraphicsSettings")
            );

            var alwaysIncludedProp = graphicsSettingsObj.FindProperty("m_AlwaysIncludedShaders");
            if (alwaysIncludedProp == null)
            {
                Debug.LogError($"{LOG_PREFIX} FIX 3: Could not find m_AlwaysIncludedShaders property!");
                return 0;
            }

            // Collect existing shader references
            var existingShaders = new HashSet<int>();
            for (int i = 0; i < alwaysIncludedProp.arraySize; i++)
            {
                var shaderRef = alwaysIncludedProp.GetArrayElementAtIndex(i).objectReferenceValue;
                if (shaderRef != null)
                    existingShaders.Add(shaderRef.GetInstanceID());
            }

            // Add missing custom shaders
            foreach (var guid in shaderGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader != null && !existingShaders.Contains(shader.GetInstanceID()))
                {
                    int newIndex = alwaysIncludedProp.arraySize;
                    alwaysIncludedProp.InsertArrayElementAtIndex(newIndex);
                    alwaysIncludedProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = shader;
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 3:</color> Added shader '{shader.name}' to Always Included");
                    fixes++;
                }
                else if (shader != null)
                {
                    Debug.Log($"{LOG_PREFIX} FIX 3: Shader '{shader.name}' already included ✓");
                }
            }

            // Also include URP Lit/Unlit/SimpleLit (critical for WebGL)
            var urpShaderNames = new[] {
                "Universal Render Pipeline/Lit",
                "Universal Render Pipeline/Unlit",
                "Universal Render Pipeline/Simple Lit",
                "Sprites/Default",
                "UI/Default"
            };

            foreach (var shaderName in urpShaderNames)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null && !existingShaders.Contains(shader.GetInstanceID()))
                {
                    int newIndex = alwaysIncludedProp.arraySize;
                    alwaysIncludedProp.InsertArrayElementAtIndex(newIndex);
                    alwaysIncludedProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = shader;
                    existingShaders.Add(shader.GetInstanceID());
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 3:</color> Added URP shader '{shaderName}' to Always Included");
                    fixes++;
                }
            }

            graphicsSettingsObj.ApplyModifiedPropertiesWithoutUndo();

            return fixes;
        }

        // ═══════════════════════════════════════════════════════════════
        //  FIX 4: Import a Font (Inter-Regular) into Assets
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/Fixes/4. Setup Font")]
        public static int Fix4_SetupFont()
        {
            int fixes = 0;

            // Check if font already exists
            var fontsDir = "Assets/UI/Fonts/Inter";
            var targetFontPath = fontsDir + "/Inter-Regular.otf";

            if (File.Exists(Path.GetFullPath(targetFontPath)))
            {
                Debug.Log($"{LOG_PREFIX} FIX 4: Font already exists at {targetFontPath} ✓");
                return 0;
            }

            // Source: Library package cache
            var packageFontPath = "Library/PackageCache/com.unity.render-pipelines.core@3b8a69964696/Samples~/Common/TextMesh Pro/Resources/Fonts & Materials/Inter-Regular.otf";
            var fullPackagePath = Path.GetFullPath(packageFontPath);

            if (File.Exists(fullPackagePath))
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetFullPath(fontsDir));

                // Copy font
                File.Copy(fullPackagePath, Path.GetFullPath(targetFontPath), true);
                AssetDatabase.Refresh();
                Debug.Log($"{LOG_PREFIX} <color=green>FIX 4:</color> Copied Inter-Regular.otf to {targetFontPath}");
                fixes++;
            }
            else
            {
                // Try to find any font in the project
                Debug.LogWarning($"{LOG_PREFIX} FIX 4: Package font not found at expected path. Searching...");

                // Search Library for any .otf or .ttf
                var searchDirs = new[] { "Library/PackageCache" };
                string foundFont = null;

                foreach (var dir in searchDirs)
                {
                    var fullDir = Path.GetFullPath(dir);
                    if (!Directory.Exists(fullDir)) continue;

                    var fonts = Directory.GetFiles(fullDir, "Inter*.otf", SearchOption.AllDirectories);
                    if (fonts.Length > 0) { foundFont = fonts[0]; break; }

                    fonts = Directory.GetFiles(fullDir, "Roboto-Regular.ttf", SearchOption.AllDirectories);
                    if (fonts.Length > 0) { foundFont = fonts[0]; break; }

                    fonts = Directory.GetFiles(fullDir, "Lato-Regular.ttf", SearchOption.AllDirectories);
                    if (fonts.Length > 0) { foundFont = fonts[0]; break; }
                }

                if (foundFont != null)
                {
                    Directory.CreateDirectory(Path.GetFullPath(fontsDir));
                    var ext = Path.GetExtension(foundFont);
                    var destPath = Path.GetFullPath(fontsDir + "/DefaultFont" + ext);
                    File.Copy(foundFont, destPath, true);
                    AssetDatabase.Refresh();
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 4:</color> Copied fallback font from {foundFont}");
                    fixes++;
                }
                else
                {
                    Debug.LogWarning($"{LOG_PREFIX} FIX 4: No font found. UI text may not render in WebGL.");
                    Debug.LogWarning($"{LOG_PREFIX} FIX 4: Please manually import a .ttf or .otf font to Assets/UI/Fonts/");
                }
            }

            return fixes;
        }

        // ═══════════════════════════════════════════════════════════════
        //  FIX 5: Validate PanelSettings (Font Asset + Text Settings)
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/Fixes/5. Validate Panel Settings")]
        public static int Fix5_ValidatePanelSettings()
        {
            int fixes = 0;

            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainPanelSettings.asset");
            if (panelSettings == null)
            {
                Debug.LogWarning($"{LOG_PREFIX} FIX 5: MainPanelSettings.asset not found!");
                return 0;
            }

            // Check if textSettings is null via SerializedObject
            var so = new SerializedObject(panelSettings);

            // Check text settings
            var textSettingsProp = so.FindProperty("textSettings");
            if (textSettingsProp != null && textSettingsProp.objectReferenceValue == null)
            {
                // Try to find an existing PanelTextSettings asset
                var textSettingsGuids = AssetDatabase.FindAssets("t:PanelTextSettings");
                UnityEngine.Object textSettings = null;

                if (textSettingsGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(textSettingsGuids[0]);
                    textSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                }

                if (textSettings == null)
                {
                    // Create via menu command (safest cross-version approach)
                    var settingsPath = "Assets/UI/Fonts/DefaultTextSettings.asset";
                    
                    // Use reflection to create PanelTextSettings
                    var panelTextSettingsType = typeof(PanelSettings).Assembly.GetType("UnityEngine.UIElements.PanelTextSettings");
                    if (panelTextSettingsType != null)
                    {
                        textSettings = ScriptableObject.CreateInstance(panelTextSettingsType);
                        AssetDatabase.CreateAsset(textSettings, settingsPath);
                        Debug.Log($"{LOG_PREFIX} <color=green>FIX 5a:</color> Created PanelTextSettings at {settingsPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"{LOG_PREFIX} FIX 5a: PanelTextSettings type not found. Trying fallback...");
                        // Fallback: search for any ScriptableObject that could be a TextSettings
                        var fallbackGuids = AssetDatabase.FindAssets("t:ScriptableObject DefaultTextSettings");
                        if (fallbackGuids.Length > 0)
                        {
                            textSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(fallbackGuids[0]));
                        }
                    }
                }

                if (textSettings != null)
                {
                    textSettingsProp.objectReferenceValue = textSettings;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log($"{LOG_PREFIX} <color=green>FIX 5b:</color> Assigned TextSettings to MainPanelSettings");
                    fixes++;

                    // Now assign font to TextSettings
                    AssignFontToTextSettings(textSettings);
                    fixes++;
                }
                else
                {
                    Debug.LogWarning($"{LOG_PREFIX} FIX 5: Could not create or find TextSettings. UI text may use default font.");
                }
            }
            else
            {
                Debug.Log($"{LOG_PREFIX} FIX 5: TextSettings already assigned ✓");

                // Still check if font is assigned
                if (textSettingsProp?.objectReferenceValue != null)
                {
                    AssignFontToTextSettings(textSettingsProp.objectReferenceValue);
                }
            }

            // Ensure ClearColor is disabled (transparent background for 3D passthrough)
            var clearColorProp = so.FindProperty("m_ClearColor");
            if (clearColorProp != null && clearColorProp.boolValue)
            {
                clearColorProp.boolValue = false;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log($"{LOG_PREFIX} <color=green>FIX 5c:</color> Disabled ClearColor on PanelSettings (transparent BG)");
                fixes++;
            }

            EditorUtility.SetDirty(panelSettings);

            return fixes;
        }

        private static void AssignFontToTextSettings(UnityEngine.Object textSettings)
        {
            if (textSettings == null) return;
            
            var textSO = new SerializedObject(textSettings);

            // Find default font asset property
            var defaultFontProp = textSO.FindProperty("m_DefaultFontAsset");
            if (defaultFontProp != null && defaultFontProp.objectReferenceValue == null)
            {
                // Search for any FontAsset in project
                var fontAssetGuids = AssetDatabase.FindAssets("t:FontAsset");
                if (fontAssetGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(fontAssetGuids[0]);
                    var fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(path);
                    if (fontAsset != null)
                    {
                        defaultFontProp.objectReferenceValue = fontAsset;
                        textSO.ApplyModifiedPropertiesWithoutUndo();
                        Debug.Log($"{LOG_PREFIX} <color=green>FIX 5d:</color> Assigned font '{fontAsset.name}' to TextSettings");
                        EditorUtility.SetDirty(textSettings);
                        return;
                    }
                }

                Debug.LogWarning($"{LOG_PREFIX} FIX 5d: No FontAsset found. Will use Unity default font.");
            }
            else
            {
                Debug.Log($"{LOG_PREFIX} FIX 5d: Font already assigned to TextSettings ✓");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  BONUS: Build WebGL directly from menu
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/🚀 Fix & Build WebGL", priority = 1)]
        public static void FixAndBuild()
        {
            // First apply ALL fixes
            FixAll();

            // Then build
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

        // ═══════════════════════════════════════════════════════════════
        //  DIAGNOSTIC: Print current state
        // ═══════════════════════════════════════════════════════════════

        [MenuItem("WebGL/📋 Diagnose Current Settings")]
        public static void DiagnoseSettings()
        {
            Debug.Log($"{LOG_PREFIX} ═══════════ DIAGNOSTIC REPORT ═══════════");

            // Graphics Pipeline
            var gfxPipeline = GraphicsSettings.defaultRenderPipeline;
            Debug.Log($"{LOG_PREFIX} Graphics Pipeline: {(gfxPipeline != null ? gfxPipeline.name : "<color=red>NONE</color>")}");

            // Quality Pipeline
            var qualityPipeline = QualitySettings.renderPipeline;
            Debug.Log($"{LOG_PREFIX} Quality Pipeline (Level {QualitySettings.GetQualityLevel()}): {(qualityPipeline != null ? qualityPipeline.name : "<color=red>NONE</color>")}");

            // WebGL Quality Level
            Debug.Log($"{LOG_PREFIX} Quality Level Names: {string.Join(", ", QualitySettings.names)}");
            Debug.Log($"{LOG_PREFIX} Current Quality Level: {QualitySettings.GetQualityLevel()} ({QualitySettings.names[QualitySettings.GetQualityLevel()]})");

            // Panel Settings
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainPanelSettings.asset");
            if (panel != null)
            {
                var so = new SerializedObject(panel);
                var textProp = so.FindProperty("textSettings");
                Debug.Log($"{LOG_PREFIX} PanelSettings TextSettings: {(textProp?.objectReferenceValue != null ? textProp.objectReferenceValue.name : "<color=red>NULL</color>")}");
            }

            // Custom Shaders
            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shaders" });
            Debug.Log($"{LOG_PREFIX} Custom Shaders in Assets/Shaders: {shaderGuids.Length}");

            // Fonts
            var fontGuids = AssetDatabase.FindAssets("t:Font", new[] { "Assets" });
            var fontAssetGuids = AssetDatabase.FindAssets("t:FontAsset", new[] { "Assets" });
            Debug.Log($"{LOG_PREFIX} Fonts in Assets: {fontGuids.Length} Font files, {fontAssetGuids.Length} FontAssets");

            // Build Scenes
            var scenes = EditorBuildSettings.scenes;
            Debug.Log($"{LOG_PREFIX} Build Scenes: {scenes.Length} ({scenes.Count(s => s.enabled)} enabled)");
            foreach (var scene in scenes)
                Debug.Log($"{LOG_PREFIX}   {(scene.enabled ? "✓" : "✗")} {scene.path}");

            Debug.Log($"{LOG_PREFIX} ═══════════════════════════════════════════");
        }
    }
}
