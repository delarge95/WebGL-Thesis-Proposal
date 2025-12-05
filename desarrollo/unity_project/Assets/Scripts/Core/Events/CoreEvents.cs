using WebGL.Core.Data;
using WebGL.Core.Managers;

namespace WebGL.Core.Events
{
    public struct PartSelectedEvent
    {
        public DronePartData PartData;
        public PartSelectedEvent(DronePartData data) { PartData = data; }
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
