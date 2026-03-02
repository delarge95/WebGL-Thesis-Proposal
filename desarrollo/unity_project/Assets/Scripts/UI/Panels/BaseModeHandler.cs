using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Base class for mode-specific UI handlers.
    /// Each handler owns the UI elements and logic for a single mode tab.
    /// </summary>
    public abstract class BaseModeHandler
    {
        protected readonly VisualElement Container;
        protected readonly VisualElement Root;
        protected readonly List<Action> CleanupActions = new();

        protected BaseModeHandler(VisualElement root, VisualElement container)
        {
            Root = root;
            Container = container;
        }

        /// <summary>Called when this mode becomes active.</summary>
        public abstract void Activate();

        /// <summary>Called when this mode is deactivated.</summary>
        public abstract void Deactivate();

        /// <summary>Schedule an action with a small delay to avoid UI conflicts.</summary>
        protected void DelayAction(Action action)
        {
            if (Root == null) action();
            else Root.schedule.Execute(action).StartingIn(250);
        }

        protected void AddCleanup(Action action)
        {
            if (action != null) CleanupActions.Add(action);
        }

        public virtual void Dispose()
        {
            foreach (var action in CleanupActions) action?.Invoke();
            CleanupActions.Clear();
        }
    }
}
