using System;
using System.Collections.Generic;
using System.Linq;
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
    private sealed class HotspotSystemGroup
    {
        public string Id;
        public string Label;
        public string Summary;
        public string[] Keywords;
        public bool IncludeFasteners;
    }

    private sealed class HotspotGroupRuntime
    {
        public HotspotSystemGroup Definition;
        public ExplodablePart Anchor;
        public List<ExplodablePart> Members;
    }

    private static readonly HotspotSystemGroup[] GroupDefinitions =
    {
        new HotspotSystemGroup
        {
            Id = "propulsion",
            Label = "Propulsion System",
            Summary = "Motors, ESCs, propellers and related mounting hardware.",
            Keywords = new[] { "motor", "esc", "prop", "propeller", "rotor" },
            IncludeFasteners = true
        },
        new HotspotSystemGroup
        {
            Id = "power",
            Label = "Power Distribution",
            Summary = "Battery, power module, PDB and primary power routing.",
            Keywords = new[] { "battery", "power", "pdb", "xt60", "cable", "harness" },
            IncludeFasteners = false
        },
        new HotspotSystemGroup
        {
            Id = "flight_control",
            Label = "Flight Control & Comms",
            Summary = "Autopilot, GPS, receiver, telemetry and radio links.",
            Keywords = new[] { "pixhawk", "gps", "receiver", "telemetry", "radio", "antenna", "fc" },
            IncludeFasteners = false
        },
        new HotspotSystemGroup
        {
            Id = "airframe",
            Label = "Airframe & Landing",
            Summary = "Arms, plates, rails and landing structure.",
            Keywords = new[] { "arm", "plate", "frame", "rail", "landing", "gear", "chassis" },
            IncludeFasteners = true
        },
        new HotspotSystemGroup
        {
            Id = "payload",
            Label = "Payload & Mounts",
            Summary = "Payload interfaces, holders and support fixtures.",
            Keywords = new[] { "payload", "mount", "gimbal", "camera", "holder", "bracket" },
            IncludeFasteners = true
        }
    };

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

        List<HotspotGroupRuntime> groupedHotspots = BuildGroupedHotspots(parts);
        if (groupedHotspots.Count > 0)
        {
            foreach (HotspotGroupRuntime grouped in groupedHotspots)
            {
                CreateGroupHotspot(grouped);
            }
            return;
        }

        foreach (ExplodablePart part in parts)
        {
            if (part == null || part.Data == null || !part.Data.isHotspotTarget) continue;
            CreateHotspot(part);
        }
    }

    private List<HotspotGroupRuntime> BuildGroupedHotspots(IEnumerable<ExplodablePart> parts)
    {
        List<ExplodablePart> validParts = parts
            .Where(p => p != null && p.Data != null)
            .ToList();

        List<HotspotGroupRuntime> groups = new List<HotspotGroupRuntime>();
        foreach (HotspotSystemGroup definition in GroupDefinitions)
        {
            List<ExplodablePart> members = validParts
                .Where(part => MatchesGroup(part, definition))
                .Distinct()
                .ToList();

            if (members.Count == 0)
            {
                continue;
            }

            ExplodablePart anchor = SelectBestAnchor(members);
            if (anchor == null)
            {
                continue;
            }

            groups.Add(new HotspotGroupRuntime
            {
                Definition = definition,
                Anchor = anchor,
                Members = members
            });
        }

        return groups;
    }

    private static bool MatchesGroup(ExplodablePart part, HotspotSystemGroup definition)
    {
        if (part == null || part.Data == null || definition == null)
        {
            return false;
        }

        string haystack = string.Join(" ", new[]
        {
            part.name,
            part.transform != null ? part.transform.name : string.Empty,
            part.Data.id,
            part.Data.partName,
            part.Data.partType,
            part.Data.category.ToString()
        }).ToLowerInvariant();

        for (int i = 0; i < definition.Keywords.Length; i++)
        {
            if (haystack.Contains(definition.Keywords[i]))
            {
                return true;
            }
        }

        return definition.IncludeFasteners && IsFastener(part);
    }

    private static bool IsFastener(ExplodablePart part)
    {
        if (part == null || part.Data == null)
        {
            return false;
        }

        if (part.Data.category == WebGL.Core.Data.PartCategory.Fasteners)
        {
            return true;
        }

        string fastenerTokens = string.Join(" ", new[]
        {
            part.name,
            part.Data.id,
            part.Data.partName,
            part.Data.partType
        }).ToLowerInvariant();

        return fastenerTokens.Contains("screw") ||
               fastenerTokens.Contains("bolt") ||
               fastenerTokens.Contains("nut") ||
               fastenerTokens.Contains("washer") ||
               fastenerTokens.Contains("standoff") ||
               fastenerTokens.Contains("spacer") ||
               fastenerTokens.Contains("m2") ||
               fastenerTokens.Contains("m3");
    }

    private static ExplodablePart SelectBestAnchor(List<ExplodablePart> members)
    {
        if (members == null || members.Count == 0)
        {
            return null;
        }

        List<ExplodablePart> preferred = members.Where(p => p.Data != null && !IsFastener(p)).ToList();
        if (preferred.Count == 0)
        {
            preferred = members;
        }

        ExplodablePart flagged = preferred.FirstOrDefault(p => p.Data != null && p.Data.isHotspotTarget);
        if (flagged != null)
        {
            return flagged;
        }

        return preferred
            .OrderByDescending(GetApproximateRenderVolume)
            .FirstOrDefault();
    }

    private static float GetApproximateRenderVolume(ExplodablePart part)
    {
        if (part == null)
        {
            return 0f;
        }

        Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            return 0f;
        }

        Bounds bounds = default;
        bool hasBounds = false;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!hasBounds)
        {
            return 0f;
        }

        Vector3 size = bounds.size;
        return Mathf.Abs(size.x * size.y * size.z);
    }

    private void CreateGroupHotspot(HotspotGroupRuntime grouped)
    {
        if (grouped == null || grouped.Anchor == null || grouped.Definition == null)
        {
            return;
        }

        string memberNames = BuildGroupMemberSummary(grouped.Members);
        string summary = $"{grouped.Definition.Summary} Includes {grouped.Members.Count} parts.";

        Action clickAction = () =>
        {
            SelectionManager.Instance?.SelectObject(
                grouped.Anchor.transform,
                fromHotspot: true,
                hotspotGroupLabel: grouped.Definition.Label,
                hotspotGroupSummary: summary,
                hotspotGroupMembers: memberNames);
        };

        SmartHotspot hotspot = new SmartHotspot(
            _container,
            grouped.Anchor,
            _mainCamera,
            grouped.Definition.Label,
            clickAction);

        _activeHotspots.Add(hotspot);
    }

    private static string BuildGroupMemberSummary(List<ExplodablePart> members)
    {
        if (members == null || members.Count == 0)
        {
            return string.Empty;
        }

        string[] topNames = members
            .Where(m => m != null && m.Data != null)
            .Select(m => string.IsNullOrWhiteSpace(m.Data.partName) ? m.name : m.Data.partName)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToArray();

        return string.Join(", ", topNames);
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
