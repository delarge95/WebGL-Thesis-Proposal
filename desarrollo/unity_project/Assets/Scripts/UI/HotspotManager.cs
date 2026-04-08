using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Events;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

/// <summary>
/// Manages all SmartHotspot instances.
/// Self-healing: UIManager auto-creates this if missing from the scene.
/// </summary>
public class HotspotManager : Singleton<HotspotManager>
{
    private VisualElement _root;
    private VisualElement _container;
    private readonly List<SmartHotspot> _activeHotspots = new List<SmartHotspot>();
    private Camera _mainCamera;

    /// <summary>
    /// Called by UIManager once the root VisualElement is ready.
    /// </summary>
    public void Initialize(VisualElement root)
    {
        _root = root;
        _mainCamera = Camera.main;

        _container = root.Q<VisualElement>("WorldSpaceContainer");
        if (_container == null)
        {
            Debug.LogError("[HotspotManager] 'WorldSpaceContainer' not found in UXML.");
            return;
        }

        ClearHotspots();
        SpawnHotspots();
    }

    public void RebuildHotspots()
    {
        if (_root == null)
        {
            return;
        }

        Initialize(_root);
        SetVisible(IsVisible);
    }

    private void SpawnHotspots()
    {
        ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);

        if (parts.Length == 0) return;

        foreach (var part in parts)
        {
            if (part == null || part.Data == null || !part.Data.isHotspotTarget) continue;
            CreateHotspot(part);
        }
    }

    private void CreateHotspot(ExplodablePart targetPart)
    {
        SmartHotspot hotspot = new SmartHotspot(_container, targetPart, _mainCamera);
        _activeHotspots.Add(hotspot);
    }

    /// <summary>
    /// Pre-clear dynamic VisualElements before UIDocument teardown
    /// to prevent "Cannot modify hierarchy during layout" errors (Unity 6 known issue).
    /// </summary>
    private void OnDisable()
    {
        EventBus.Unsubscribe<ImportedDroneRuntimeBoundEvent>(HandleImportedDroneRuntimeBound);
        ClearHotspots();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ImportedDroneRuntimeBoundEvent>(HandleImportedDroneRuntimeBound);
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
    public bool IsVisible { get; private set; } = true;

    /// <summary>
    /// Show or hide all hotspots without destroying them.
    /// </summary>
    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        if (_container == null) return;

        _container.style.opacity = visible ? 1f : 0f;
        _container.pickingMode = PickingMode.Ignore; // Always ignore container, children handle clicks

        foreach (var hotspot in _activeHotspots)
        {
            hotspot.SetEnabled(visible);
        }
    }

    public void ToggleVisibility()
    {
        SetVisible(!IsVisible);
    }

    private void HandleImportedDroneRuntimeBound(ImportedDroneRuntimeBoundEvent _)
    {
        RebuildHotspots();
    }
}
