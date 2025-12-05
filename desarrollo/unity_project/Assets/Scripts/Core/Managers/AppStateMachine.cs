using UnityEngine;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Central application state machine.
    /// Controls the overall state of the application and fires events on state changes.
    /// </summary>
    public class AppStateMachine : Singleton<AppStateMachine>
    {
        #region Serialized Fields

        [Header("State Configuration")]
        [SerializeField]
        [Tooltip("The initial state when the application starts")]
        private AppState initialState = AppState.Intro;

        [SerializeField]
        [Tooltip("Enable debug logging for state transitions")]
        private bool debugLogging = true;

        #endregion

        #region Private Fields

        private AppState currentState;
        private AppState previousState;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current application state.
        /// </summary>
        public AppState CurrentState => currentState;

        /// <summary>
        /// Gets the previous application state.
        /// </summary>
        public AppState PreviousState => previousState;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the application state changes.
        /// </summary>
        public event System.Action<AppState, AppState> OnStateChanged;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            currentState = initialState;
        }

        private void Start()
        {
            // Publish initial state
            EventBus.Publish(new StateChangedEvent(currentState, currentState));
            
            if (debugLogging)
            {
                Debug.Log($"[AppStateMachine] Initialized with state: {currentState}");
            }
        }

        #endregion

        #region State Management

        /// <summary>
        /// Transitions to the specified state.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        /// <returns>True if transition was successful, false if blocked.</returns>
        public bool SetState(AppState newState)
        {
            if (currentState == newState) return false;

            if (!CanTransitionTo(newState))
            {
                if (debugLogging)
                {
                    Debug.LogWarning($"[AppStateMachine] Blocked transition: {currentState} -> {newState}");
                }
                return false;
            }

            previousState = currentState;
            currentState = newState;

            // Fire events
            OnStateChanged?.Invoke(previousState, currentState);
            EventBus.Publish(new StateChangedEvent(previousState, currentState));

            if (debugLogging)
            {
                Debug.Log($"[AppStateMachine] State changed: {previousState} -> {currentState}");
            }

            return true;
        }

        /// <summary>
        /// Returns to the previous state.
        /// </summary>
        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        /// <summary>
        /// Checks if transition to the specified state is allowed.
        /// Override this method to implement custom transition rules.
        /// </summary>
        /// <param name="targetState">The target state.</param>
        /// <returns>True if transition is allowed.</returns>
        protected virtual bool CanTransitionTo(AppState targetState)
        {
            // Default: allow all transitions
            // Custom rules can be added here
            switch (currentState)
            {
                case AppState.Loading:
                    // Can only exit loading to intro or exploration
                    return targetState == AppState.Intro || targetState == AppState.Exploration;
                    
                case AppState.Intro:
                    // Can exit intro to exploration
                    return targetState == AppState.Exploration;
                    
                default:
                    return true;
            }
        }

        #endregion

        #region Convenience Methods

        /// <summary>
        /// Enters exploration mode.
        /// </summary>
        public void EnterExploration() => SetState(AppState.Exploration);

        /// <summary>
        /// Enters exploded view mode.
        /// </summary>
        public void EnterExplodedView() => SetState(AppState.ExplodedView);

        /// <summary>
        /// Enters focus mode for detailed part inspection.
        /// </summary>
        public void EnterFocusMode() => SetState(AppState.FocusMode);

        /// <summary>
        /// Opens the settings menu.
        /// </summary>
        public void OpenSettings() => SetState(AppState.Settings);

        /// <summary>
        /// Opens the main menu.
        /// </summary>
        public void OpenMenu() => SetState(AppState.Menu);

        /// <summary>
        /// Checks if the application is in an interactive state.
        /// </summary>
        public bool IsInteractive()
        {
            return currentState == AppState.Exploration || 
                   currentState == AppState.ExplodedView || 
                   currentState == AppState.FocusMode;
        }

        #endregion
    }

    /// <summary>
    /// Defines the possible application states.
    /// </summary>
    public enum AppState
    {
        /// <summary>Initial loading state.</summary>
        Loading,
        
        /// <summary>Intro/splash screen state.</summary>
        Intro,
        
        /// <summary>Main exploration state - user can navigate and select parts.</summary>
        Exploration,
        
        /// <summary>Exploded view state - parts are separated for inspection.</summary>
        ExplodedView,
        
        /// <summary>Focus mode - camera focuses on selected part.</summary>
        FocusMode,
        
        /// <summary>Settings menu state.</summary>
        Settings,
        
        /// <summary>Main menu state.</summary>
        Menu
    }
}
