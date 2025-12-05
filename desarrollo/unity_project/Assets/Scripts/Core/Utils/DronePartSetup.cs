using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WebGL.Core.Utils
{
    /// <summary>
    /// Editor utility to quickly set up a drone part with all required components.
    /// </summary>
    public class DronePartSetup : MonoBehaviour
    {
        [Header("Part Configuration")]
        public Data.DronePartData partData;
        
        [Header("Auto-Setup")]
        [SerializeField] private bool autoSetupOnStart = false;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupPart();
            }
        }

        [ContextMenu("Setup Part Components")]
        public void SetupPart()
        {
            // Ensure ExplodablePart
            var explodable = GetComponent<Content.ExplodablePart>();
            if (explodable == null)
            {
                explodable = gameObject.AddComponent<Content.ExplodablePart>();
            }

            // Assign part data
            if (partData != null)
            {
                // Use reflection or serialized field access to set data
                var field = explodable.GetType().GetField("partData", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(explodable, partData);
                }
            }

            // Ensure Collider for selection
            if (GetComponent<Collider>() == null)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    var meshCollider = gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = false; // More accurate selection
                }
            }

            // Set layer for selection raycast
            gameObject.layer = LayerMask.NameToLayer("Default");

            Debug.Log($"[DronePartSetup] Setup complete for: {gameObject.name}");
        }

        [ContextMenu("Add Connection Point Markers")]
        public void AddConnectionPointMarkers()
        {
            if (partData == null)
            {
                Debug.LogWarning("No DronePartData assigned!");
                return;
            }

            // Create connection point child objects based on partData
            if (partData.screwCount > 0)
            {
                for (int i = 0; i < partData.screwCount; i++)
                {
                    var point = new GameObject($"ConnectionPoint_Screw_{i + 1}");
                    point.transform.SetParent(transform);
                    point.transform.localPosition = Vector3.zero;
                    
                    var marker = point.AddComponent<Managers.ConnectionPointMarker>();
                    // marker.ConnectionType would be set to Screw
                    // marker.Description = partData.screwSize
                }
            }

            Debug.Log($"[DronePartSetup] Added {partData.screwCount} connection point markers.");
        }

#if UNITY_EDITOR
        [ContextMenu("Create DronePartData Asset")]
        public void CreatePartDataAsset()
        {
            var data = ScriptableObject.CreateInstance<Data.DronePartData>();
            data.partName = gameObject.name;
            data.id = System.Guid.NewGuid().ToString().Substring(0, 8);

            // Calculate bounds for dimensions
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                var bounds = renderer.bounds;
                float x = bounds.size.x * 100; // Convert to cm
                float y = bounds.size.y * 100;
                float z = bounds.size.z * 100;
                data.dimensions = $"{x:F1} x {y:F1} x {z:F1} cm";
            }

            // Save asset
            string path = $"Assets/Data/Parts/{gameObject.name}_Data.asset";
            
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Parts"))
                AssetDatabase.CreateFolder("Assets/Data", "Parts");
            
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();

            partData = data;
            Debug.Log($"[DronePartSetup] Created DronePartData asset at: {path}");
        }
#endif
    }
}
