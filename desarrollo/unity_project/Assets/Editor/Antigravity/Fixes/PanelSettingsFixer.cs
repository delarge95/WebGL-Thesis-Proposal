using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 5: Validate PanelSettings (Font Asset + Text Settings).
    /// </summary>
    public static class PanelSettingsFixer
    {
        [MenuItem("WebGL/Fixes/5. Validate Panel Settings")]
        public static int Fix()
        {
            int fixes = 0;

            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainPanelSettings.asset");
            if (panelSettings == null)
            {
                Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 5: MainPanelSettings.asset not found!");
                return 0;
            }

            var so = new SerializedObject(panelSettings);

            fixes += EnsureTextSettings(so);
            fixes += EnsureClearColorDisabled(so);

            EditorUtility.SetDirty(panelSettings);
            return fixes;
        }

        private static int EnsureTextSettings(SerializedObject so)
        {
            var textSettingsProp = so.FindProperty("textSettings");
            if (textSettingsProp == null || textSettingsProp.objectReferenceValue != null)
            {
                if (textSettingsProp?.objectReferenceValue != null)
                {
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 5: TextSettings already assigned ✓");
                    AssignFontToTextSettings(textSettingsProp.objectReferenceValue);
                }
                return 0;
            }

            Object textSettings = FindOrCreateTextSettings();
            if (textSettings == null)
            {
                Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 5: Could not create or find TextSettings. UI text may use default font.");
                return 0;
            }

            textSettingsProp.objectReferenceValue = textSettings;
            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 5b:</color> Assigned TextSettings to MainPanelSettings");

            AssignFontToTextSettings(textSettings);
            return 2; // textSettings assignment + font assignment
        }

        private static Object FindOrCreateTextSettings()
        {
            var guids = AssetDatabase.FindAssets("t:PanelTextSettings");
            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guids[0]));

            var settingsPath = "Assets/UI/Fonts/DefaultTextSettings.asset";
            var panelTextSettingsType = typeof(PanelSettings).Assembly.GetType("UnityEngine.UIElements.PanelTextSettings");
            if (panelTextSettingsType != null)
            {
                var instance = ScriptableObject.CreateInstance(panelTextSettingsType);
                AssetDatabase.CreateAsset(instance, settingsPath);
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 5a:</color> Created PanelTextSettings at {settingsPath}");
                return instance;
            }

            Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 5a: PanelTextSettings type not found. Trying fallback...");
            var fallbackGuids = AssetDatabase.FindAssets("t:ScriptableObject DefaultTextSettings");
            if (fallbackGuids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(fallbackGuids[0]));

            return null;
        }

        private static void AssignFontToTextSettings(Object textSettings)
        {
            if (textSettings == null) return;

            var textSO = new SerializedObject(textSettings);
            var defaultFontProp = textSO.FindProperty("m_DefaultFontAsset");
            if (defaultFontProp == null || defaultFontProp.objectReferenceValue != null)
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 5d: Font already assigned to TextSettings ✓");
                return;
            }

            var fontAssetGuids = AssetDatabase.FindAssets("t:FontAsset");
            if (fontAssetGuids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(fontAssetGuids[0]);
                var fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(path);
                if (fontAsset != null)
                {
                    defaultFontProp.objectReferenceValue = fontAsset;
                    textSO.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 5d:</color> Assigned font '{fontAsset.name}' to TextSettings");
                    EditorUtility.SetDirty(textSettings);
                    return;
                }
            }

            Debug.LogWarning($"{WebGLBuildFixer.LOG_PREFIX} FIX 5d: No FontAsset found. Will use Unity default font.");
        }

        private static int EnsureClearColorDisabled(SerializedObject so)
        {
            var clearColorProp = so.FindProperty("m_ClearColor");
            if (clearColorProp != null && clearColorProp.boolValue)
            {
                clearColorProp.boolValue = false;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 5c:</color> Disabled ClearColor on PanelSettings (transparent BG)");
                return 1;
            }
            return 0;
        }
    }
}
