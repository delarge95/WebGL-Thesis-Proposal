using WebGL.Core.Data;
using WebGL.Core.Managers;
using UnityEngine;
using System.Collections.Generic;
using WebGL.Core.Content;

namespace WebGL.Core.Events
{
    public struct PartSelectedEvent
    {
        public DronePartData PartData;
        public bool FromHotspot;
        public string HotspotGroupLabel;
        public string HotspotGroupSummary;
        public string HotspotGroupMembers;
        public string SelectionLabel;
        public string CanonicalPartName;

        public PartSelectedEvent(
            DronePartData data,
            bool fromHotspot = false,
            string hotspotGroupLabel = "",
            string hotspotGroupSummary = "",
            string hotspotGroupMembers = "",
            string selectionLabel = "",
            string canonicalPartName = "")
        {
            PartData = data;
            FromHotspot = fromHotspot;
            HotspotGroupLabel = hotspotGroupLabel ?? string.Empty;
            HotspotGroupSummary = hotspotGroupSummary ?? string.Empty;
            HotspotGroupMembers = hotspotGroupMembers ?? string.Empty;
            SelectionLabel = selectionLabel ?? string.Empty;
            CanonicalPartName = canonicalPartName ?? string.Empty;
        }
    }

    public struct PartDoubleClickedEvent
    {
        public DronePartData PartData;
        public Transform ClickedTransform;
        public Transform FullPartTransform;
        public bool IsBackground;
        public bool HadSelectionBeforeFirstClick;
        public Transform SelectionBeforeFirstClick;
        public Transform FullSelectionBeforeFirstClick;

        public PartDoubleClickedEvent(
            DronePartData data,
            Transform clickedTransform,
            Transform fullPartTransform,
            bool isBackground,
            bool hadSelectionBeforeFirstClick,
            Transform selectionBeforeFirstClick,
            Transform fullSelectionBeforeFirstClick)
        {
            PartData = data;
            ClickedTransform = clickedTransform;
            FullPartTransform = fullPartTransform;
            IsBackground = isBackground;
            HadSelectionBeforeFirstClick = hadSelectionBeforeFirstClick;
            SelectionBeforeFirstClick = selectionBeforeFirstClick;
            FullSelectionBeforeFirstClick = fullSelectionBeforeFirstClick;
        }
    }

    public struct ViewModeChangedEvent
    {
        public bool IsExploded;
        public ViewModeChangedEvent(bool isExploded) { IsExploded = isExploded; }
    }

    public struct HotspotGroupVisualsClearRequestedEvent
    {
    }

    public struct HotspotGroupIsolatedEvent
    {
        public string GroupId;
        public List<ExplodablePart> Members;

        public HotspotGroupIsolatedEvent(string groupId, List<ExplodablePart> members)
        {
            GroupId = groupId ?? string.Empty;
            Members = members ?? new List<ExplodablePart>();
        }
    }
}
