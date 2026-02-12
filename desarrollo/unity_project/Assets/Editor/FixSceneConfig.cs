using UnityEngine;
using UnityEditor;

public class FixSceneConfig : MonoBehaviour
{
    [MenuItem("WebGL/Fixes/Add Hotspot Manager")]
    public static void AddHotspotManager()
    {
        // 1. Check if it already exists
        var existing = FindFirstObjectByType<HotspotManager>();
        if (existing != null)
        {
            Debug.Log($"SUCCESS: HotspotManager already exists on '{existing.gameObject.name}'.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        // 2. Find "Managers" object or create it
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = GameObject.Find("GameManager");
        }
        
        if (managers == null)
        {
            managers = new GameObject("Managers");
            Undo.RegisterCreatedObjectUndo(managers, "Create Managers");
            Debug.Log("Created 'Managers' GameObject.");
        }

        // 3. Add Component
        Undo.AddComponent<HotspotManager>(managers);
        Debug.Log($"SUCCESS: Added HotspotManager to '{managers.name}'.");
        Selection.activeGameObject = managers;
    }
}
