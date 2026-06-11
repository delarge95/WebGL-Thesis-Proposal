using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace WebGL.Editor.Fixes
{
    /// <summary>
    /// FIX 3: Include Custom Shaders in "Always Included".
    /// </summary>
    public static class ShaderInclusionFixer
    {
        [MenuItem("WebGL/Fixes/3. Include Custom Shaders")]
        public static int Fix()
        {
            int fixes = 0;

            var shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Shaders" });
            if (shaderGuids.Length == 0)
            {
                Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 3: No custom shaders found in Assets/Shaders ✓");
                return 0;
            }

            var graphicsSettingsObj = new SerializedObject(
                AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/GraphicsSettings.asset")
                    ?? UnityEditor.Unsupported.GetSerializedAssetInterfaceSingleton("GraphicsSettings")
            );

            var alwaysIncludedProp = graphicsSettingsObj.FindProperty("m_AlwaysIncludedShaders");
            if (alwaysIncludedProp == null)
            {
                Debug.LogError($"{WebGLBuildFixer.LOG_PREFIX} FIX 3: Could not find m_AlwaysIncludedShaders property!");
                return 0;
            }

            var existingShaders = CollectExistingShaders(alwaysIncludedProp);

            fixes += AddCustomShaders(alwaysIncludedProp, existingShaders, shaderGuids);
            fixes += AddURPShaders(alwaysIncludedProp, existingShaders);

            graphicsSettingsObj.ApplyModifiedPropertiesWithoutUndo();
            return fixes;
        }

        private static HashSet<int> CollectExistingShaders(SerializedProperty prop)
        {
            var set = new HashSet<int>();
            for (int i = 0; i < prop.arraySize; i++)
            {
                var shaderRef = prop.GetArrayElementAtIndex(i).objectReferenceValue;
                if (shaderRef != null)
                    set.Add(shaderRef.GetInstanceID());
            }
            return set;
        }

        private static int AddCustomShaders(SerializedProperty prop, HashSet<int> existing, string[] guids)
        {
            int fixes = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader == null) continue;

                if (!existing.Contains(shader.GetInstanceID()))
                {
                    int idx = prop.arraySize;
                    prop.InsertArrayElementAtIndex(idx);
                    prop.GetArrayElementAtIndex(idx).objectReferenceValue = shader;
                    existing.Add(shader.GetInstanceID());
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 3:</color> Added shader '{shader.name}' to Always Included");
                    fixes++;
                }
                else
                {
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} FIX 3: Shader '{shader.name}' already included ✓");
                }
            }
            return fixes;
        }

        private static int AddURPShaders(SerializedProperty prop, HashSet<int> existing)
        {
            int fixes = 0;
            var urpShaderNames = new[]
            {
                "Universal Render Pipeline/Lit",
                "Universal Render Pipeline/Unlit",
                "Universal Render Pipeline/Simple Lit",
                "Sprites/Default",
                "UI/Default"
            };

            foreach (var shaderName in urpShaderNames)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null && !existing.Contains(shader.GetInstanceID()))
                {
                    int idx = prop.arraySize;
                    prop.InsertArrayElementAtIndex(idx);
                    prop.GetArrayElementAtIndex(idx).objectReferenceValue = shader;
                    existing.Add(shader.GetInstanceID());
                    Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} <color=green>FIX 3:</color> Added URP shader '{shaderName}' to Always Included");
                    fixes++;
                }
            }
            return fixes;
        }
    }
}
