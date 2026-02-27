using UnityEngine;
using UnityEditor;
using System.IO;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 4: Ensure a usable font file exists in the project.
    /// </summary>
    public static class FontSetupFixer
    {
        [MenuItem("WebGL/Fixes/4. Setup Font")]
        public static int Fix()
        {
            var fontsDir = "Assets/UI/Fonts/Inter";
            var targetFontPath = fontsDir + "/Inter-Regular.otf";

            if (File.Exists(Path.GetFullPath(targetFontPath)))
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 4: Font already exists at {targetFontPath} ✓");
                return 0;
            }

            var packageFontPath = "Library/PackageCache/com.unity.render-pipelines.core@3b8a69964696/Samples~/Common/TextMesh Pro/Resources/Fonts & Materials/Inter-Regular.otf";
            var fullPackagePath = Path.GetFullPath(packageFontPath);

            if (File.Exists(fullPackagePath))
            {
                Directory.CreateDirectory(Path.GetFullPath(fontsDir));
                File.Copy(fullPackagePath, Path.GetFullPath(targetFontPath), true);
                AssetDatabase.Refresh();
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 4:</color> Copied Inter-Regular.otf to {targetFontPath}");
                return 1;
            }

            Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 4: Package font not found. Searching fallback...");
            return TryFallbackFontCopy(fontsDir);
        }

        private static int TryFallbackFontCopy(string fontsDir)
        {
            var searchDirs = new[] { "Library/PackageCache" };

            foreach (var dir in searchDirs)
            {
                var fullDir = Path.GetFullPath(dir);
                if (!Directory.Exists(fullDir)) continue;

                foreach (var pattern in new[] { "Inter*.otf", "Roboto-Regular.ttf", "Lato-Regular.ttf" })
                {
                    var fonts = Directory.GetFiles(fullDir, pattern, SearchOption.AllDirectories);
                    if (fonts.Length > 0)
                    {
                        Directory.CreateDirectory(Path.GetFullPath(fontsDir));
                        var ext = Path.GetExtension(fonts[0]);
                        var destPath = Path.GetFullPath(fontsDir + "/DefaultFont" + ext);
                        File.Copy(fonts[0], destPath, true);
                        AssetDatabase.Refresh();
                        Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 4:</color> Copied fallback font from {fonts[0]}");
                        return 1;
                    }
                }
            }

            Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 4: No font found. UI text may not render in WebGL.");
            Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 4: Please manually import a .ttf or .otf font to Assets/UI/Fonts/");
            return 0;
        }
    }
}
