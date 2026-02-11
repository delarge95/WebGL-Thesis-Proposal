using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core; 
using WebGL.Core.Data; // For DronePartData
using WebGL.Core.Content; // For ExplodablePart
using WebGL.Core.Managers; // For SelectionManager
using WebGL.Core.Utils; // For Singleton

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
        // 1. Find all ExplodablePart components in the scene (these are the actual GameObjects)
        ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);

        foreach (var part in parts)
        {
            if (part == null || part.Data == null) continue;
            
            // Create hotspot attached to this part's transform
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
            // Propagate selection using correct API
            if (SelectionManager.Instance != null)
            {
                SelectionManager.Instance.SelectObject(target);
            }
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
