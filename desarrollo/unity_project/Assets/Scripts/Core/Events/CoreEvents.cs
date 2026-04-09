using WebGL.Core.Data;
using WebGL.Core.Managers;

namespace WebGL.Core.Events
{
    public struct PartSelectedEvent
    {
        public DronePartData PartData;
        public bool FromHotspot;
        public string HotspotGroupLabel;
        public string HotspotGroupSummary;
        public string HotspotGroupMembers;

        public PartSelectedEvent(
            DronePartData data,
            bool fromHotspot = false,
            string hotspotGroupLabel = "",
            string hotspotGroupSummary = "",
            string hotspotGroupMembers = "")
        {
            PartData = data;
            FromHotspot = fromHotspot;
            HotspotGroupLabel = hotspotGroupLabel ?? string.Empty;
            HotspotGroupSummary = hotspotGroupSummary ?? string.Empty;
            HotspotGroupMembers = hotspotGroupMembers ?? string.Empty;
        }
    }

    public struct PartDoubleClickedEvent
    {
        public DronePartData PartData;
        public PartDoubleClickedEvent(DronePartData data) { PartData = data; }
    }

    public struct ViewModeChangedEvent
    {
        public bool IsExploded;
        public ViewModeChangedEvent(bool isExploded) { IsExploded = isExploded; }
    }
}
