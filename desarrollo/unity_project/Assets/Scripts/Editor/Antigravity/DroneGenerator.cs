using UnityEngine;
using UnityEditor;
using WebGL.Core.Utils;

namespace WebGL.Editor.Antigravity
{
    public class DroneGenerator : EditorWindow
    {
        [MenuItem("Antigravity/Generate Procedural Drone")]
        public static void GenerateDrone()
        {
            // 1. Find or Create Assembler Object
            GameObject assemblerObj = GameObject.Find("DroneAssembler");
            if (assemblerObj == null)
            {
                assemblerObj = new GameObject("DroneAssembler");
                Undo.RegisterCreatedObjectUndo(assemblerObj, "Create Drone Assembler");
            }

            // 2. Add Component if missing
            DroneAssembler assembler = assemblerObj.GetComponent<DroneAssembler>();
            if (assembler == null)
            {
                assembler = Undo.AddComponent<DroneAssembler>(assemblerObj);
            }

            // 3. Trigger Assembly
            // We need to make sure AssembleDrone supports being called in Edit Mode.
            // Since it instantiates primitives and components, it should be fine.
            // But we should register Undo for the created children.
            
            // Note: DroneAssembler.AssembleDrone() needs to be slightly modified to support Undo if possible,
            // or we just let it run. For better Editor integration, using Undo.RegisterCreatedObjectUndo 
            // inside AssembleDrone would be best, but modifying the runtime script with #if UNITY_EDITOR is cleaner.
            
            assembler.AssembleDrone();
            
            Selection.activeGameObject = assemblerObj;
            SceneView.FrameLastActiveSceneView();
            
            Debug.Log("[Antigravity] Procedural Drone Generated Successfully! 🚁");
        }
    }
}
