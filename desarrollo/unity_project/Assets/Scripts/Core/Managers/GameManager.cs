using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Main game manager that persists across scenes.
    /// Provides backward compatibility and basic state management.
    /// For advanced state management, use AppStateMachine.
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        #region Serialized Fields

        [Header("State")]
        [SerializeField] 
        [Tooltip("Current application state")]
        private AppState currentState;

        [Header("Settings")]
        [SerializeField]
        [Tooltip("Enable debug logging")]
        private bool debugMode = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the current application state.
        /// </summary>
        public AppState CurrentState => currentState;

        /// <summary>
        /// Gets the debug mode status.
        /// </summary>
        public bool DebugMode => debugMode;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the game state changes.
        /// </summary>
        public event System.Action<AppState> OnStateChanged;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            SetState(AppState.Intro);
            
            if (debugMode)
            {
                Debug.Log("[GameManager] Initialized");
            }
        }

        #endregion

        #region State Management

        /// <summary>
        /// Sets the application state and notifies listeners.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        public void SetState(AppState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            
            OnStateChanged?.Invoke(newState);

            if (debugMode)
            {
                Debug.Log($"[GameManager] State changed to: {newState}");
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks if the game is in debug mode.
        /// </summary>
        public static bool IsDebugMode()
        {
            return Instance != null && Instance.debugMode;
        }

        /// <summary>
        /// Logs a message only if debug mode is enabled.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void DebugLog(string message)
        {
            if (IsDebugMode())
            {
                Debug.Log(message);
            }
        }

        #endregion
    }
}
