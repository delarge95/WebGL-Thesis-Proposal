using System;
using UnityEngine.UIElements;
using WebGL.Core.Managers;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Handles the Inspect mode UI: Info, Pins (Hotspots), Isolate, and power/load controls.
    /// All cards are immediate toggles.
    /// </summary>
    public class InspectModeHandler : BaseModeHandler
    {
        private readonly VisualElement _cardGrid;
        private readonly VisualElement _powerInlineSlider;
        private readonly Button _hotspotBtn;
        private readonly Button _isolateBtn;
        private readonly Button _measureBtn;
        private readonly Button _powerBtn;
        private readonly Slider _powerLoadSlider;
        private readonly Label _powerLoadValue;

        private bool _hotspotsEnabled = true;
        private bool _isIsolated;
        private bool _isMeasuring;
        private bool _isPowered;
        private bool _suppressPowerSliderEvent;
        private DroneStateController _drone;

        public event Action OnIsolateToggleRequested;

        public InspectModeHandler(VisualElement root, VisualElement container) : base(root, container)
        {
            if (container == null)
            {
                return;
            }

            _cardGrid = container.Q<VisualElement>("ToolsCardGrid");
            _powerInlineSlider = container.Q<VisualElement>("PowerInlineSlider");
            _hotspotBtn = container.Q<Button>("ToolHotspotBtn");
            _isolateBtn = container.Q<Button>("ToolIsolateBtn");
            _measureBtn = container.Q<Button>("ToolMeasureBtn");
            _powerBtn = container.Q<Button>("ToolPowerBtn");
            _powerLoadSlider = container.Q<Slider>("PowerLoadSlider");
            _powerLoadValue = container.Q<Label>("PowerLoadValue");

            BindCards();
            BindPowerSlider();
            AttachDroneController();
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

            if (_powerBtn != null)
            {
                Action onPower = () => DelayAction(TogglePower);
                _powerBtn.clicked += onPower;
                AddCleanup(() => _powerBtn.clicked -= onPower);
            }
        }

        private void BindPowerSlider()
        {
            if (_powerLoadSlider == null)
            {
                return;
            }

            EventCallback<ChangeEvent<float>> onLoadChanged = evt =>
            {
                if (_suppressPowerSliderEvent)
                {
                    return;
                }

                float normalizedLoad = evt.newValue / 100f;
                UpdatePowerLoadLabel(normalizedLoad);
                DroneStateController.Instance?.SetSystemLoad(normalizedLoad);
            };

            _powerLoadSlider.RegisterValueChangedCallback(onLoadChanged);
            AddCleanup(() => _powerLoadSlider.UnregisterValueChangedCallback(onLoadChanged));
        }

        public override void Activate()
        {
            AttachDroneController();
        }

        public override void Deactivate()
        {
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
            {
                MeasurementTool.Instance?.Activate();
            }
            else
            {
                MeasurementTool.Instance?.Deactivate();
            }

            _measureBtn?.EnableInClassList("submenu-card--active", _isMeasuring);
        }

        public void SetMeasureState(bool measuring)
        {
            _isMeasuring = measuring;
            _measureBtn?.EnableInClassList("submenu-card--active", measuring);
        }

        public void TogglePower()
        {
            AttachDroneController();
            DroneStateController.Instance?.TogglePower();
        }

        private void AttachDroneController()
        {
            if (_drone != null)
            {
                return;
            }

            _drone = DroneStateController.Instance;
            if (_drone == null)
            {
                return;
            }

            SyncPowerUi(_drone.IsOn, _drone.SystemLoadFactor);
            _drone.OnStateChanged += OnDroneStateChanged;
            _drone.OnSystemLoadChanged += OnSystemLoadChanged;
            AddCleanup(() =>
            {
                if (_drone != null)
                {
                    _drone.OnStateChanged -= OnDroneStateChanged;
                    _drone.OnSystemLoadChanged -= OnSystemLoadChanged;
                    _drone = null;
                }
            });
        }

        private void OnDroneStateChanged(DroneState state)
        {
            SyncPowerUi(state != DroneState.Off && state != DroneState.ShuttingDown, _drone != null ? _drone.SystemLoadFactor : 0f);
        }

        private void OnSystemLoadChanged(float normalizedLoad)
        {
            SyncPowerUi(_drone != null && _drone.IsOn, normalizedLoad);
        }

        private void SyncPowerUi(bool powered, float normalizedLoad)
        {
            _isPowered = powered;
            _powerBtn?.EnableInClassList("submenu-card--active", powered);
            _powerInlineSlider?.EnableInClassList("submenu-card--active", powered);

            _suppressPowerSliderEvent = true;
            if (_powerLoadSlider != null)
            {
                float value = normalizedLoad * 100f;
                if (Math.Abs(_powerLoadSlider.value - value) > 0.01f)
                {
                    _powerLoadSlider.SetValueWithoutNotify(value);
                }
            }
            _suppressPowerSliderEvent = false;

            UpdatePowerLoadLabel(normalizedLoad);
        }

        private void UpdatePowerLoadLabel(float normalizedLoad)
        {
            if (_powerLoadValue != null)
            {
                _powerLoadValue.text = $"{Math.Round(normalizedLoad * 100f):0}%";
            }
        }

        public override void Dispose()
        {
            OnIsolateToggleRequested = null;
            base.Dispose();
        }
    }
}