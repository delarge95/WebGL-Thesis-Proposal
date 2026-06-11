using System;
using UnityEngine.UIElements;
using WebGL.Core.Managers;

namespace WebGL.UI.Panels
{
    public class InspectModeHandler : BaseModeHandler
    {
        private readonly Button _hotspotBtn;
        private readonly Button _isolateBtn;
        private readonly Button _measureBtn;
        private readonly Button _powerBtn;
        private readonly VisualElement _powerLoadPanel;
        private readonly Slider _powerLoadSlider;
        private readonly Label _powerLoadValue;
        private readonly Label _powerStateLabel;

        private bool _hotspotsEnabled = true;
        private bool _isMeasuring;
        private bool _isPowered;
        private bool _suppressSliderCallback;

        public event Action OnIsolateToggleRequested;

        public InspectModeHandler(VisualElement root, VisualElement container) : base(root, container)
        {
            if (container == null) return;

            _hotspotBtn = container.Q<Button>("ToolHotspotBtn");
            _isolateBtn = container.Q<Button>("ToolIsolateBtn");
            _measureBtn = container.Q<Button>("ToolMeasureBtn");
            _powerBtn = container.Q<Button>("ToolPowerBtn");
            _powerLoadPanel = container.Q<VisualElement>("ToolPowerLoadPanel");
            _powerLoadSlider = container.Q<Slider>("ToolPowerLoadSlider");
            _powerLoadValue = container.Q<Label>("ToolPowerLoadValue");
            _powerStateLabel = container.Q<Label>("ToolPowerStateLabel");

            AppLanguageManager.LanguageChanged += OnLanguageChanged;
            AddCleanup(() => AppLanguageManager.LanguageChanged -= OnLanguageChanged);
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

            if (_powerBtn != null)
            {
                Action onPower = () => DelayAction(TogglePower);
                _powerBtn.clicked += onPower;
                AddCleanup(() => _powerBtn.clicked -= onPower);
            }

            if (_powerLoadSlider != null)
            {
                EventCallback<ChangeEvent<float>> onSliderChanged = evt =>
                {
                    if (_suppressSliderCallback) return;
                    DroneStateController.Instance?.SetLoadCommand(evt.newValue);
                    UpdatePowerLoadUI(evt.newValue);
                };
                _powerLoadSlider.RegisterValueChangedCallback(onSliderChanged);
                AddCleanup(() => _powerLoadSlider.UnregisterValueChangedCallback(onSliderChanged));

                EventCallback<PointerDownEvent> onDown = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = true;
                    }
                    evt.StopPropagation();
                };
                EventCallback<PointerUpEvent> onUp = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = false;
                    }
                };
                _powerLoadSlider.RegisterCallback(onDown);
                _powerLoadSlider.RegisterCallback(onUp);
                AddCleanup(() =>
                {
                    _powerLoadSlider.UnregisterCallback(onDown);
                    _powerLoadSlider.UnregisterCallback(onUp);
                });
            }

            DroneStateController drone = DroneStateController.Instance;
            if (drone != null)
            {
                _isPowered = drone.IsOn;
                drone.OnStateChanged += OnDroneStateChanged;
                drone.OnLoadCommandChanged += OnLoadCommandChanged;
                AddCleanup(() => drone.OnStateChanged -= OnDroneStateChanged);
                AddCleanup(() => drone.OnLoadCommandChanged -= OnLoadCommandChanged);

                SetSliderValueWithoutFeedback(drone.LoadCommandNormalized);
                UpdatePowerButtonState(drone.CurrentState);
                UpdatePowerLoadUI(drone.LoadCommandNormalized);
            }
            else
            {
                UpdatePowerButtonState(DroneState.Off);
                UpdatePowerLoadUI(0f);
            }
        }

        public override void Activate()
        {
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

        public void TogglePower()
        {
            DroneStateController.Instance?.TogglePower();
        }

        private void OnDroneStateChanged(DroneState state)
        {
            UpdatePowerButtonState(state);

            if (state == DroneState.Off)
            {
                UpdatePowerLoadUI(DroneStateController.Instance != null ? DroneStateController.Instance.LoadCommandNormalized : 0f);
            }
        }

        private void OnLoadCommandChanged(float loadNormalized)
        {
            SetSliderValueWithoutFeedback(loadNormalized);
            UpdatePowerLoadUI(loadNormalized);
        }

        private void OnLanguageChanged(string languageCode)
        {
            DroneStateController drone = DroneStateController.Instance;
            UpdatePowerButtonState(drone != null ? drone.CurrentState : DroneState.Off);
        }

        private void UpdatePowerButtonState(DroneState state)
        {
            _isPowered = state != DroneState.Off && state != DroneState.ShuttingDown;
            _powerBtn?.EnableInClassList("submenu-card--active", _isPowered);
            if (_powerLoadPanel != null)
            {
                _powerLoadPanel.style.display = _isPowered ? DisplayStyle.Flex : DisplayStyle.None;
                _powerLoadPanel.SetEnabled(_isPowered);
                _powerLoadPanel.style.opacity = 1f;
            }

            if (_powerStateLabel == null)
            {
                return;
            }

            string englishState = state switch
            {
                DroneState.StartingUp => "STARTING",
                DroneState.Idle => "IDLE",
                DroneState.Flying => "FLYING",
                DroneState.ShuttingDown => "SHUTDOWN",
                _ => "OFF",
            };

            _powerStateLabel.text = AppLanguageManager.TranslateStatic(englishState);
        }

        private void UpdatePowerLoadUI(float loadNormalized)
        {
            if (_powerLoadValue != null)
            {
                _powerLoadValue.text = $"{Math.Round(loadNormalized * 100f):F0}%";
            }
        }

        private void SetSliderValueWithoutFeedback(float value)
        {
            if (_powerLoadSlider == null) return;

            _suppressSliderCallback = true;
            _powerLoadSlider.value = value;
            _suppressSliderCallback = false;
        }

        public override void Dispose()
        {
            OnIsolateToggleRequested = null;
            base.Dispose();
        }
    }
}
