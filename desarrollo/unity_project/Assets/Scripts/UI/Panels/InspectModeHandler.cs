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
        private readonly Button _measureBtn;

        private bool _hotspotsEnabled = true;
        private bool _isIsolated;
        private bool _isMeasuring;

        // ── Events forwarded to orchestrator ──
        public event Action OnIsolateToggleRequested;

        public InspectModeHandler(VisualElement root, VisualElement container) : base(root, container)
        {
            if (container == null) return;

            _cardGrid = container.Q<VisualElement>("ToolsCardGrid");
            _hotspotBtn = container.Q<Button>("ToolHotspotBtn");
            _isolateBtn = container.Q<Button>("ToolIsolateBtn");
            _measureBtn = container.Q<Button>("ToolMeasureBtn");

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

            if (_measureBtn != null)
            {
                Action onMeasure = () => DelayAction(ToggleMeasure);
                _measureBtn.clicked += onMeasure;
                AddCleanup(() => _measureBtn.clicked -= onMeasure);
            }
        }

        public override void Activate() { /* Inspect cards are always visible when container shows */ }

        public override void Deactivate()
        {
            // Ensure measurement tool is turned off when leaving Inspect mode
            if (_isMeasuring)
            {
                _isMeasuring = false;
                MeasurementTool.Instance?.Deactivate();
                _measureBtn?.EnableInClassList("submenu-card--active", false);
            }
        }

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

        public void ToggleMeasure()
        {
            _isMeasuring = !_isMeasuring;
            if (_isMeasuring)
                MeasurementTool.Instance?.Activate();
            else
                MeasurementTool.Instance?.Deactivate();

            _measureBtn?.EnableInClassList("submenu-card--active", _isMeasuring);
        }

        public void SetMeasureState(bool measuring)
        {
            _isMeasuring = measuring;
            _measureBtn?.EnableInClassList("submenu-card--active", measuring);
        }

        public override void Dispose()
        {
            OnIsolateToggleRequested = null;
            base.Dispose();
        }
    }
}
