using WebGL.Core.Data;
using WebGL.Core.Managers;

namespace WebGL.Core.Events
{
    public struct PartSelectedEvent
    {
        public DronePartData PartData;
        public bool FromHotspot;
        public PartSelectedEvent(DronePartData data, bool fromHotspot = false)
        {
            PartData = data;
            FromHotspot = fromHotspot;
        }
    }

    public struct AppStateChangedEvent
    {
        public AppState NewState;
        public AppStateChangedEvent(AppState state) { NewState = state; }
    }

    public struct ViewModeChangedEvent
    {
        public bool IsExploded;
        public ViewModeChangedEvent(bool isExploded) { IsExploded = isExploded; }
    }
}
