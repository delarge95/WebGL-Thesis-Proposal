using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public enum AppState
    {
        Intro,
        Exploration,
        ExplodedView,
        FocusMode
    }

    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("State")]
        [SerializeField] private AppState currentState;
        
        public AppState CurrentState => currentState;

        // Awake is handled by base class

        private void Start()
        {
            SetState(AppState.Intro);
        }

        public void SetState(AppState newState)
        {
            currentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
            
            // Event dispatching could go here
        }
    }
}
