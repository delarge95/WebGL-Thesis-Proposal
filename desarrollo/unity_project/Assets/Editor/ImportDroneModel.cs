using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// One-click importer: places x500v2_drone.fbx into MainScene_Final at world origin.
/// Run via menu: Tools > Import Drone Model
/// The FBX is already at Assets/Models/x500v2_drone.fbx — Unity auto-imports it on
/// domain reload. This script just instantiates a prefab/asset from it in the scene.
/// </summary>
public static class ImportDroneModel
{
    private const string FbxAssetPath = "Assets/Models/x500v2_drone.fbx";
    private const string ScenePath    = "Assets/Scenes/MainScene_Final.unity";
    private const string RootName     = "x500v2_Drone";

    [MenuItem("Tools/Import Drone Model Into Scene")]
    public static void ImportIntoScene()
    {
        // ------------------------------------------------------------------
        // 1. Load the FBX as a root GameObject asset
        // ------------------------------------------------------------------
        GameObject fbxRoot = AssetDatabase.LoadAssetAtPath<GameObject>(FbxAssetPath);
        if (fbxRoot == null)
        {
            Debug.LogError($"[ImportDroneModel] FBX not found at: {FbxAssetPath}\n" +
                           "Make sure Unity has finished importing before running this tool.");
            EditorUtility.DisplayDialog("Error",
                "FBX not found. Wait for Unity to finish importing, then retry.", "OK");
            return;
        }

        // ------------------------------------------------------------------
        // 2. Open MainScene_Final (save current first)
        // ------------------------------------------------------------------
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        // ------------------------------------------------------------------
        // 3. Remove any previous drone root in the scene
        // ------------------------------------------------------------------
        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Debug.Log($"[ImportDroneModel] Removing existing '{RootName}' from scene.");
            Object.DestroyImmediate(existing);
        }

        // ------------------------------------------------------------------
        // 4. Instantiate and place at origin
        // ------------------------------------------------------------------
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxRoot);
        instance.name = RootName;
        instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        instance.transform.localScale = Vector3.one;

        // ------------------------------------------------------------------
        // 5. Save scene
        // ------------------------------------------------------------------
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log($"[ImportDroneModel] '{RootName}' placed in {ScenePath} at world origin.\n" +
                  $"  FBX: {FbxAssetPath}\n" +
                  $"  Child count: {instance.transform.childCount}");

        EditorUtility.DisplayDialog("Done",
            $"Drone model placed in MainScene_Final.\n" +
            $"Children: {instance.transform.childCount}", "OK");

        // Focus scene view on the drone
        Selection.activeGameObject = instance;
        SceneView.FrameLastActiveSceneView();
    }
}
