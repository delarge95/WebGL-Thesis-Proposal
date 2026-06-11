using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Main game manager that persists across scenes.
    /// Provides global debug utilities.
    /// For application state, use AppStateMachine (single source of truth).
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        #region Serialized Fields

        [Header("Settings")]
        [SerializeField]
        [Tooltip("Enable debug logging")]
        private bool debugMode = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug mode status.
        /// </summary>
        public bool DebugMode => debugMode;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (debugMode)
            {
                Debug.Log("[GameManager] Initialized");
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
