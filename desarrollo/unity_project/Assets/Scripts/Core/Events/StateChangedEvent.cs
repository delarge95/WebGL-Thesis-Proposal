using WebGL.Core.Managers;

namespace WebGL.Core.Events
{
    public struct StateChangedEvent
    {
        public AppState PreviousState { get; }
        public AppState NewState { get; }

        public StateChangedEvent(AppState previousState, AppState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}
