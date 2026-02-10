using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

namespace WebGL.Editor.Antigravity
{
    public static class SceneBuilder
    {
        [MenuItem("Antigravity/Build Final Scene")]
        public static void BuildScene()
        {
            string scenePath = "Assets/Scenes/MainScene_Final.unity";
            
            // 1. Create New Scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // 2. Setup Camera
            var camObj = new GameObject("Main Camera");
            var cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f); // Dark gray
            camObj.transform.position = new Vector3(0, 1, -10);
            
            // Add AudioListener
            camObj.AddComponent<AudioListener>();

            // 3. Setup Directional Light
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // 4. Setup Global Volume (Post Processing)
            var volObj = new GameObject("Global Volume");
            var vol = volObj.AddComponent<Volume>();
            vol.isGlobal = true;
            
            // Create a default profile if none exists
            // (Skipping detailed profile creation for now, just the volume)

            // 5. Setup UI Toolkit
            var uiObj = new GameObject("UI");
            var uiDoc = uiObj.AddComponent<UIDocument>();
            
            // Link Assets
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UI/Layouts/MainLayout.uxml");
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainPanelSettings.asset");

            if (visualTree == null) Debug.LogError("[SceneBuilder] MainLayout.uxml not found at Assets/Scripts/UI/Layouts/MainLayout.uxml");
            else uiDoc.visualTreeAsset = visualTree;

            if (panelSettings == null) Debug.LogError("[SceneBuilder] MainPanelSettings.asset not found at Assets/UI/MainPanelSettings.asset");
            else uiDoc.panelSettings = panelSettings;

            // 6. Setup Logic & Managers
            var managersObj = new GameObject("Managers");
            
            // AppStateMachine
            managersObj.AddComponent<WebGL.Core.Managers.AppStateMachine>();
            
            // ExplodedViewManager
            managersObj.AddComponent<WebGL.Core.Content.ExplodedViewManager>();

            // SelectionManager
            var selectionMgr = managersObj.AddComponent<WebGL.Core.Managers.SelectionManager>();
            // Use SerializedObject to set private LayerMask field "selectionLayer"
            var so = new SerializedObject(selectionMgr);
            so.FindProperty("selectionLayer").intValue = ~0; // Everything
            so.ApplyModifiedProperties();

            // 7. Setup Camera Logic
            // OrbitCameraController
            var orbit = camObj.AddComponent<WebGL.Core.Managers.OrbitCameraController>();
            
            // Create a target for the orbit camera
            var targetObj = new GameObject("OrbitTarget");
            targetObj.transform.position = Vector3.zero;
            orbit.SetTarget(targetObj.transform);

            // 8. Setup UI Logic
            // Attach UIManager to the UI object
            uiObj.AddComponent<WebGL.UI.UIManager>();

            // 8.1 Setup EventSystem (Crucial for UI Blocking)
            if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var evtObj = new GameObject("EventSystem");
                evtObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                evtObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // 9. Create Test Geometry (The "Drone")
            var testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObj.name = "TestDrone_Cube";
            testObj.transform.position = Vector3.zero;
            testObj.transform.localScale = Vector3.one * 2f;
            
            // Ensure Collider
            if (testObj.GetComponent<Collider>() == null)
            {
                testObj.AddComponent<BoxCollider>();
            }
            
            // Add Interaction Components
            var explodable = testObj.AddComponent<WebGL.Core.Content.ExplodablePart>();
            testObj.AddComponent<WebGL.Core.Content.HighlightSystem>();
            
            // Assign Dummy Data
            var dummyData = AssetFactory.CreateDummyPartData();
            // Use SerializedObject to set private field "partData"
            var soPart = new SerializedObject(explodable);
            soPart.FindProperty("partData").objectReferenceValue = dummyData;
            soPart.ApplyModifiedProperties();
            
            // Ensure it has a material that supports property blocks (Standard)
            var renderer = testObj.GetComponent<Renderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // 10. Save Scene
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SceneBuilder] Scene built with TEST CUBE at {scenePath}");
        }
    }
}
