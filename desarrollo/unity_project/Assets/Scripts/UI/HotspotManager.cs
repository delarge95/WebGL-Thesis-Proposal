using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

/// <summary>
/// Manages all SmartHotspot instances.
/// Self-healing: UIManager auto-creates this if missing from the scene.
/// </summary>
public class HotspotManager : Singleton<HotspotManager>
{
    private VisualElement _container;
    private readonly List<SmartHotspot> _activeHotspots = new List<SmartHotspot>();
    private Camera _mainCamera;

    /// <summary>
    /// Called by UIManager once the root VisualElement is ready.
    /// </summary>
    public void Initialize(VisualElement root)
    {
        _mainCamera = Camera.main;

        _container = root.Q<VisualElement>("WorldSpaceContainer");
        if (_container == null)
        {
            Debug.LogError("[HotspotManager] 'WorldSpaceContainer' not found in UXML.");
            return;
        }

        SpawnHotspots();
    }

    private void SpawnHotspots()
    {
        ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);

        if (parts.Length == 0) return;

        foreach (var part in parts)
        {
            if (part == null) continue;
            CreateHotspot(part.transform);
        }
    }

    private void CreateHotspot(Transform target)
    {
        // SmartHotspot now handles its own click/hover events internally
        SmartHotspot hotspot = new SmartHotspot(_container, target, _mainCamera);
        _activeHotspots.Add(hotspot);
    }

    private void LateUpdate()
    {
        if (_container == null) return;

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

    /// <summary>
    /// Show or hide all hotspots without destroying them.
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (_container == null) return;

        if (visible)
        {
            _container.style.opacity = 1f;
            _container.pickingMode = PickingMode.Ignore;
        }
        else
        {
            _container.style.opacity = 0f;
            _container.pickingMode = PickingMode.Ignore;
        }
    }
}
