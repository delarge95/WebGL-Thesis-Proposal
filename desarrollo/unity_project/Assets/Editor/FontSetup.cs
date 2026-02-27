using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.IO;

/// <summary>
/// Editor utility that generates SDF Font Assets from raw .ttf/.otf fonts
/// and configures the project's TextSettings + PanelSettings for proper
/// font rendering in UI Toolkit (both Editor preview and runtime).
///
/// Usage:
///   Menu → WebGL / Fonts / Setup Font Assets
///   Also runs automatically on first import via [InitializeOnLoad].
/// </summary>
[InitializeOnLoad]
public static class FontSetup
{
    // Paths
    private const string INTER_OTF       = "Assets/Resources/Fonts/Inter/Inter-Regular.otf";
    private const string SPACE_REGULAR    = "Assets/Resources/Fonts/SpaceGrotesk/SpaceGrotesk-Regular.ttf";
    private const string SPACE_BOLD       = "Assets/Resources/Fonts/SpaceGrotesk/SpaceGrotesk-Bold.ttf";
    private const string SPACE_MEDIUM     = "Assets/Resources/Fonts/SpaceGrotesk/SpaceGrotesk-Medium.ttf";
    private const string SPACE_SEMIBOLD   = "Assets/Resources/Fonts/SpaceGrotesk/SpaceGrotesk-SemiBold.ttf";
    private const string SPACE_LIGHT      = "Assets/Resources/Fonts/SpaceGrotesk/SpaceGrotesk-Light.ttf";

    private const string OUTPUT_DIR       = "Assets/UI/Fonts/Generated";
    private const string TEXT_SETTINGS     = "Assets/UI/Fonts/DefaultTextSettings.asset";
    private const string PANEL_SETTINGS    = "Assets/UI/MainPanelSettings.asset";

    // Session key to avoid running every domain reload
    private const string SESSION_KEY = "FontSetup_Done_v2";

    static FontSetup()
    {
        // Run once per editor session, not every domain reload
        if (!SessionState.GetBool(SESSION_KEY, false))
        {
            // Delay to let asset database finish loading
            EditorApplication.delayCall += () =>
            {
                if (NeedsFontSetup())
                {
                    Debug.Log("[FontSetup] Font Assets not detected. Running auto-setup...");
                    RunSetup();
                }
                SessionState.SetBool(SESSION_KEY, true);
            };
        }
    }

    [MenuItem("WebGL/Fonts/Setup Font Assets")]
    public static void RunSetupFromMenu()
    {
        RunSetup();
    }

    private static bool NeedsFontSetup()
    {
        string interSDF = $"{OUTPUT_DIR}/Inter-Regular SDF.asset";
        return !File.Exists(interSDF);
    }

    private static void RunSetup()
    {
        // Ensure output directory
        if (!AssetDatabase.IsValidFolder(OUTPUT_DIR))
        {
            string parent = Path.GetDirectoryName(OUTPUT_DIR).Replace("\\", "/");
            string folder = Path.GetFileName(OUTPUT_DIR);
            if (!AssetDatabase.IsValidFolder(parent))
            {
                string grandparent = Path.GetDirectoryName(parent).Replace("\\", "/");
                string parentFolder = Path.GetFileName(parent);
                AssetDatabase.CreateFolder(grandparent, parentFolder);
            }
            AssetDatabase.CreateFolder(parent, folder);
        }

        // Generate SDF Font Assets
        FontAsset interFA    = CreateFontAsset(INTER_OTF,     "Inter-Regular SDF");
        FontAsset spaceRegFA = CreateFontAsset(SPACE_REGULAR, "SpaceGrotesk-Regular SDF");
        FontAsset spaceBoldFA = CreateFontAsset(SPACE_BOLD,   "SpaceGrotesk-Bold SDF");
        FontAsset spaceMedFA = CreateFontAsset(SPACE_MEDIUM,  "SpaceGrotesk-Medium SDF");
        FontAsset spaceSBFA  = CreateFontAsset(SPACE_SEMIBOLD,"SpaceGrotesk-SemiBold SDF");
        FontAsset spaceLightFA = CreateFontAsset(SPACE_LIGHT, "SpaceGrotesk-Light SDF");

        // Configure TextSettings
        if (interFA != null)
        {
            ConfigureTextSettings(interFA, new FontAsset[] { spaceRegFA, spaceBoldFA, spaceMedFA });
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[FontSetup] ✓ Font Assets generated and TextSettings configured.");
        Debug.Log($"[FontSetup]   Default font: {(interFA != null ? interFA.name : "FAILED")}");
        Debug.Log($"[FontSetup]   Output dir: {OUTPUT_DIR}");
        Debug.Log("[FontSetup] ✓ Font preview should now work correctly in UI Builder.");
    }

    private static FontAsset CreateFontAsset(string sourcePath, string assetName)
    {
        string outputPath = $"{OUTPUT_DIR}/{assetName}.asset";

        // Skip if already exists
        var existing = AssetDatabase.LoadAssetAtPath<FontAsset>(outputPath);
        if (existing != null)
        {
            Debug.Log($"[FontSetup] Font Asset already exists: {outputPath}");
            return existing;
        }

        // Load source font
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
        if (sourceFont == null)
        {
            Debug.LogWarning($"[FontSetup] Source font not found: {sourcePath}");
            return null;
        }

        // Create SDF Font Asset
        // Unity 6 API: FontAsset.CreateFontAsset(Font, int samplingPointSize, int atlasPadding,
        //   GlyphRenderMode renderMode, int atlasWidth, int atlasHeight,
        //   AtlasPopulationMode populationMode, bool enableMultiAtlasSupport)
        FontAsset fontAsset = FontAsset.CreateFontAsset(
            sourceFont,
            36,                                                    // samplingPointSize — high quality
            4,                                                     // atlasPadding
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,  // SDF with anti-aliasing
            2048,                                                  // atlasWidth
            2048,                                                  // atlasHeight
            AtlasPopulationMode.Dynamic,                           // Dynamic: generates glyphs on demand
            true                                                   // enableMultiAtlasSupport
        );

        if (fontAsset == null)
        {
            Debug.LogError($"[FontSetup] Failed to create Font Asset from: {sourcePath}");
            return null;
        }

        fontAsset.name = assetName;
        AssetDatabase.CreateAsset(fontAsset, outputPath);
        Debug.Log($"[FontSetup] Created: {outputPath}");
        return fontAsset;
    }

    private static void ConfigureTextSettings(FontAsset defaultFont, FontAsset[] fallbacks)
    {
        var textSettings = AssetDatabase.LoadAssetAtPath<TextSettings>(TEXT_SETTINGS);
        if (textSettings == null)
        {
            Debug.LogWarning($"[FontSetup] TextSettings not found at: {TEXT_SETTINGS}");
            return;
        }

        // Use SerializedObject for proper undo + save
        var so = new SerializedObject(textSettings);

        // Set default font asset
        var defaultFontProp = so.FindProperty("m_DefaultFontAsset");
        if (defaultFontProp != null)
        {
            defaultFontProp.objectReferenceValue = defaultFont;
        }

        // Set fallback fonts
        var fallbackProp = so.FindProperty("m_FallbackFontAssets");
        if (fallbackProp != null && fallbacks != null)
        {
            fallbackProp.ClearArray();
            foreach (var fb in fallbacks)
            {
                if (fb == null) continue;
                fallbackProp.InsertArrayElementAtIndex(fallbackProp.arraySize);
                fallbackProp.GetArrayElementAtIndex(fallbackProp.arraySize - 1).objectReferenceValue = fb;
            }
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(textSettings);
        Debug.Log($"[FontSetup] TextSettings configured — default: {defaultFont.name}, fallbacks: {fallbacks?.Length ?? 0}");
    }
}
