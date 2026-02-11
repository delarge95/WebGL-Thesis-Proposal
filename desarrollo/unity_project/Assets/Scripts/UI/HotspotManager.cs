using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core; // For DronePartData
using WebGL.Core.Utils; // For Singleton if it's there

public class HotspotManager : Singleton<HotspotManager>
{
    //[SerializeField] private UIDocument uiDocument; // Optional if we want direct reference, but UIManager has it
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0); // Global offset

    private VisualElement _container;
    private List<SmartHotspot> _activeHotspots = new List<SmartHotspot>();
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        // Wait for UIManager to be ready or grab container directly if possible
        // Better to subscribe to an event or wait for Start
    }

    public void Initialize(VisualElement root)
    {
        _container = root.Q<VisualElement>("WorldSpaceContainer");
        if (_container == null)
        {
            Debug.LogError("HotspotManager: 'WorldSpaceContainer' not found in UXML.");
            return;
        }

        SpawnHotspots();
    }

    private void SpawnHotspots()
    {
        // 1. Find all DronePartData components in the scene
        DronePartData[] parts = FindObjectsByType<DronePartData>(FindObjectsSortMode.None);

        foreach (var part in parts)
        {
            // Only add hotspots for significant parts or all parts?
            // Let's add for all for now.
            CreateHotspot(part.transform);
        }
        
        Debug.Log($"HotspotManager: Created {_activeHotspots.Count} hotspots.");
    }

    private void CreateHotspot(Transform target)
    {
        SmartHotspot hotspot = new SmartHotspot(_container, target, _mainCamera);
        
        // Register Click Event
        hotspot.RegisterCallback<ClickEvent>(evt => 
        {
            // Propagate selection
            SelectionManager.Instance.SelectPart(target.gameObject);
        });

        _activeHotspots.Add(hotspot);
    }

    private void LateUpdate()
    {
        if (_container == null || _activeHotspots.Count == 0) return;

        // Batch update positions
        foreach (var hotspot in _activeHotspots)
        {
            hotspot.Update();
        }
    }

    public void ClearHotspots()
    {
        foreach (var hotspot in _activeHotspots)
        {
            hotspot.Destroy();
        }
        _activeHotspots.Clear();
    }
}
