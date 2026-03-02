using System;
using UnityEngine.UIElements;
using WebGL.Core.Managers;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Handles the Inspect mode UI: Info, Pins (Hotspots), and Isolate toggle cards.
    /// All cards are immediate toggles — no sub-panel navigation.
    /// </summary>
    public class InspectModeHandler : BaseModeHandler
    {
        private readonly VisualElement _cardGrid;
        private readonly Button _hotspotBtn;
        private readonly Button _isolateBtn;

        private bool _hotspotsEnabled = true;
        private bool _isIsolated;

        // ── Events forwarded to orchestrator ──
        public event Action OnIsolateToggleRequested;

        public InspectModeHandler(VisualElement root, VisualElement container) : base(root, container)
        {
            if (container == null) return;

            _cardGrid = container.Q<VisualElement>("ToolsCardGrid");
            _hotspotBtn = container.Q<Button>("ToolHotspotBtn");
            _isolateBtn = container.Q<Button>("ToolIsolateBtn");

            BindCards();
        }

        private void BindCards()
        {
            if (_hotspotBtn != null)
            {
                _hotspotBtn.EnableInClassList("submenu-card--active", _hotspotsEnabled);
                Action onHotspot = () => DelayAction(ToggleHotspots);
                _hotspotBtn.clicked += onHotspot;
                AddCleanup(() => _hotspotBtn.clicked -= onHotspot);
            }

            if (_isolateBtn != null)
            {
                Action onIsolate = () => DelayAction(() => OnIsolateToggleRequested?.Invoke());
                _isolateBtn.clicked += onIsolate;
                AddCleanup(() => _isolateBtn.clicked -= onIsolate);
            }
        }

        public override void Activate() { /* Inspect cards are always visible when container shows */ }

        public override void Deactivate() { /* No sub-panel state to reset */ }

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();
            _hotspotBtn?.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

        public void SetIsolateState(bool isolated)
        {
            _isIsolated = isolated;
            _isolateBtn?.EnableInClassList("submenu-card--active", isolated);
        }

        public override void Dispose()
        {
            OnIsolateToggleRequested = null;
            base.Dispose();
        }
    }
}
