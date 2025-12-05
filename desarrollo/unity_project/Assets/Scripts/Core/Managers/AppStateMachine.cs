using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Events;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public enum AppState
    {
        Loading,
        Intro,
        Exploration,
        FocusMode,
        ExplodedView,
        Settings
    }

    public class AppStateMachine : PersistentSingleton<AppStateMachine>
    {
        [Header("Debug")]
        [SerializeField] private AppState currentState = AppState.Loading;
        
        private Dictionary<AppState, Action> onEnterState = new Dictionary<AppState, Action>();
        private Dictionary<AppState, Action> onExitState = new Dictionary<AppState, Action>();
        private Dictionary<AppState, Action> onUpdateState = new Dictionary<AppState, Action>();

        public AppState CurrentState => currentState;

        protected override void Awake()
        {
            base.Awake();
            InitializeCallbacks();
        }

        private void InitializeCallbacks()
        {
            // Initialize empty dictionaries
            foreach (AppState state in Enum.GetValues(typeof(AppState)))
            {
                onEnterState[state] = null;
                onExitState[state] = null;
                onUpdateState[state] = null;
            }
        }

        private void Update()
        {
            // Run update callback for current state
            onUpdateState[currentState]?.Invoke();
        }

        public void TransitionTo(AppState newState)
        {
            if (currentState == newState) return;

            Debug.Log($"[StateMachine] {currentState} -> {newState}");

            // Exit current state
            onExitState[currentState]?.Invoke();

            AppState previousState = currentState;
            currentState = newState;

            // Enter new state
            onEnterState[currentState]?.Invoke();

            // Publish event for other systems
            EventBus.Publish(new AppStateChangedEvent(currentState));
        }

        public void RegisterEnterCallback(AppState state, Action callback)
        {
            onEnterState[state] += callback;
        }

        public void RegisterExitCallback(AppState state, Action callback)
        {
            onExitState[state] += callback;
        }

        public void RegisterUpdateCallback(AppState state, Action callback)
        {
            onUpdateState[state] += callback;
        }

        public void UnregisterEnterCallback(AppState state, Action callback)
        {
            onEnterState[state] -= callback;
        }

        public void UnregisterExitCallback(AppState state, Action callback)
        {
            onExitState[state] -= callback;
        }

        public void UnregisterUpdateCallback(AppState state, Action callback)
        {
            onUpdateState[state] -= callback;
        }
    }
}
