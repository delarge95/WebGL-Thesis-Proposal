using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    public class UIEnvironmentPanel
    {
        private VisualElement _envPanel;
        private Slider _envLightRotSlider;
        private Slider _envLightIntSlider;
        private Slider _envBackgroundIntSlider;
        private VisualElement _studioDots;
        private VisualElement _timeDots;
        private VisualElement _colorDots;
        private List<System.Action> _cleanupActions = new List<System.Action>();

        // ── Cycle state for TIME, COLOR, and STUDIO buttons ──
        private static readonly string[] TimeCycle = { "Day", "Night", "Sunset" };
        private static readonly string[] ColorCycle = { "White", "Grey", "Black", "Yellow", "Orange", "Green", "Blue", "Purple", "Red" };
        private static readonly string[] StudioCycle = { "Studio", "Studio Light", "Blueprint" };
        private int _timeIndex = 0;
        private int _colorIndex = 0;
        private int _studioCycleIndex = 0;
        private string _activePreset = "Studio";
        private string _displayedStudioPreset = "Studio";
        private string _displayedTimePreset = "Time";
        private string _displayedColorPreset = "Color";
        private bool _isBlueprintMode = false;

        public UIEnvironmentPanel(VisualElement envPanel)
        {
            _envPanel = envPanel;
            AppLanguageManager.LanguageChanged += OnLanguageChanged;
            AddCleanup(() => AppLanguageManager.LanguageChanged -= OnLanguageChanged);
            BindEnvPanel();
            RefreshCycleLabels();
        }

        private void AddCleanup(System.Action cleanupAction)
        {
            if (cleanupAction != null) _cleanupActions.Add(cleanupAction);
        }

        public void Dispose()
        {
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        private void BindEnvPanel()
        {
            if (_envPanel == null) return;

            _envLightRotSlider = _envPanel.Q<Slider>("EnvLightRotation");
            _envLightIntSlider = _envPanel.Q<Slider>("EnvLightIntensity");
            _envBackgroundIntSlider = _envPanel.Q<Slider>("EnvBackgroundIntensity");

            if (_envLightRotSlider != null)
            {
                EventCallback<ChangeEvent<float>> onRotChanged = evt => 
                {
                    if (ServiceLocator.TryGet<EnvironmentController>(out var env)) env.SetLightRotation(evt.newValue);
                };
                _envLightRotSlider.RegisterValueChangedCallback(onRotChanged);
                AddCleanup(() => _envLightRotSlider.UnregisterValueChangedCallback(onRotChanged));

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

                _envLightRotSlider.RegisterCallback(onDown);
                _envLightRotSlider.RegisterCallback(onUp);

                AddCleanup(() => {
                    _envLightRotSlider.UnregisterCallback(onDown);
                    _envLightRotSlider.UnregisterCallback(onUp);
                });
            }

            if (_envLightIntSlider != null)
            {
                EventCallback<ChangeEvent<float>> onIntChanged = evt => 
                {
                    if (ServiceLocator.TryGet<EnvironmentController>(out var env)) env.SetLightIntensity(evt.newValue);
                };
                _envLightIntSlider.RegisterValueChangedCallback(onIntChanged);
                AddCleanup(() => _envLightIntSlider.UnregisterValueChangedCallback(onIntChanged));

                EventCallback<PointerDownEvent> onDown2 = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = true;
                    }
                    evt.StopPropagation();
                };
                EventCallback<PointerUpEvent> onUp2 = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = false;
                    }
                };

                _envLightIntSlider.RegisterCallback(onDown2);
                _envLightIntSlider.RegisterCallback(onUp2);

                AddCleanup(() => {
                    _envLightIntSlider.UnregisterCallback(onDown2);
                    _envLightIntSlider.UnregisterCallback(onUp2);
                });
            }

            if (_envBackgroundIntSlider != null)
            {
                EventCallback<ChangeEvent<float>> onBgChanged = evt =>
                {
                    if (ServiceLocator.TryGet<EnvironmentController>(out var env)) env.SetBackgroundIntensity(evt.newValue);
                };
                _envBackgroundIntSlider.RegisterValueChangedCallback(onBgChanged);
                AddCleanup(() => _envBackgroundIntSlider.UnregisterValueChangedCallback(onBgChanged));

                EventCallback<PointerDownEvent> onDown3 = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = true;
                    }
                    evt.StopPropagation();
                };
                EventCallback<PointerUpEvent> onUp3 = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = false;
                    }
                };

                _envBackgroundIntSlider.RegisterCallback(onDown3);
                _envBackgroundIntSlider.RegisterCallback(onUp3);

                AddCleanup(() => {
                    _envBackgroundIntSlider.UnregisterCallback(onDown3);
                    _envBackgroundIntSlider.UnregisterCallback(onUp3);
                });
            }

            // ── Studio button — cycles Studio → Studio Light → Blueprint ──
            var studioBtn = _envPanel.Q<Button>("EnvPreset_Studio");
            if (studioBtn != null)
            {
                _studioDots = EnsureCycleDots(studioBtn, StudioCycle.Length);
                UpdateCycleDots(_studioDots, _studioCycleIndex);

                System.Action onStudioClick = () =>
                {
                    _studioCycleIndex = (_studioCycleIndex + 1) % StudioCycle.Length;
                    string preset = StudioCycle[_studioCycleIndex];

                    if (preset == "Blueprint")
                    {
                        _isBlueprintMode = true;
                        ApplyAndHighlight("Blueprint", "Studio");
                        if (ViewModeManager.Instance != null)
                        {
                            ViewModeManager.Instance.BaseMode = ViewMode.Blueprint;
                            ViewModeManager.Instance.SetViewMode(ViewMode.Blueprint);
                        }
                    }
                    else
                    {
                        _isBlueprintMode = false;
                        ApplyAndHighlight(preset, "Studio");
                        if (ViewModeManager.Instance != null)
                        {
                            ViewModeManager.Instance.BaseMode = ViewMode.Realistic;
                            if (ViewModeManager.Instance.CurrentMode == ViewMode.Blueprint)
                                ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
                        }
                    }

                    _displayedStudioPreset = preset;
                    SetCycleLabel("EnvPreset_Studio", _displayedStudioPreset, true);
                    UpdateCycleDots(_studioDots, _studioCycleIndex);
                };
                studioBtn.clicked += onStudioClick;
                AddCleanup(() => studioBtn.clicked -= onStudioClick);
            }

            // ── TIME button — cycles Day / Night / Sunset ──
            var nightBtn = _envPanel.Q<Button>("EnvPreset_Night");
            if (nightBtn != null)
            {
                _timeDots = EnsureCycleDots(nightBtn, TimeCycle.Length);
                UpdateCycleDots(_timeDots, _timeIndex);

                System.Action onTimeClick = () =>
                {
                    string preset = TimeCycle[_timeIndex];
                    int activeIndex = _timeIndex;
                    _timeIndex = (_timeIndex + 1) % TimeCycle.Length;
                    ApplyAndHighlight(preset, "Night");
                    DeactivateBlueprintIfNeeded();

                    _displayedTimePreset = preset;
                    SetCycleLabel("EnvPreset_Night", _displayedTimePreset, false);
                    UpdateCycleDots(_timeDots, activeIndex);
                };
                nightBtn.clicked += onTimeClick;
                AddCleanup(() => nightBtn.clicked -= onTimeClick);
            }

            // ── COLOR button — cycles 9 background colors ──
            var blueBtn = _envPanel.Q<Button>("EnvPreset_Blueprint");
            if (blueBtn != null)
            {
                _colorDots = EnsureCycleDots(blueBtn, ColorCycle.Length);
                UpdateCycleDots(_colorDots, _colorIndex);

                System.Action onColorClick = () =>
                {
                    string preset = ColorCycle[_colorIndex];
                    int activeIndex = _colorIndex;
                    _colorIndex = (_colorIndex + 1) % ColorCycle.Length;
                    ApplyAndHighlight(preset, "Blueprint");
                    DeactivateBlueprintIfNeeded();

                    _displayedColorPreset = preset;
                    SetCycleLabel("EnvPreset_Blueprint", _displayedColorPreset, false);
                    UpdateCycleDots(_colorDots, activeIndex);
                };
                blueBtn.clicked += onColorClick;
                AddCleanup(() => blueBtn.clicked -= onColorClick);
            }
        }

        private void OnLanguageChanged(string languageCode)
        {
            RefreshCycleLabels();
        }

        private static VisualElement EnsureCycleDots(Button button, int count)
        {
            if (button == null || count <= 1)
            {
                return null;
            }

            VisualElement container = button.Q<VisualElement>(className: "env-cycle-dots");
            if (container == null)
            {
                container = new VisualElement { pickingMode = PickingMode.Ignore };
                container.AddToClassList("env-cycle-dots");
                button.Add(container);
            }

            container.Clear();
            for (int i = 0; i < count; i++)
            {
                VisualElement dot = new VisualElement { pickingMode = PickingMode.Ignore };
                dot.AddToClassList("env-cycle-dot");
                container.Add(dot);
            }

            return container;
        }

        private static void UpdateCycleDots(VisualElement container, int activeIndex)
        {
            if (container == null)
            {
                return;
            }

            int index = 0;
            foreach (VisualElement dot in container.Children())
            {
                dot.EnableInClassList("env-cycle-dot--active", index == activeIndex);
                index++;
            }
        }

        private void BindSimplePreset(string presetName)
        {
            var btn = _envPanel.Q<Button>($"EnvPreset_{presetName}");
            if (btn == null) return;
            System.Action onClick = () =>
            {
                ApplyAndHighlight(presetName, presetName);
                // Reset cycle labels when returning to Studio
                ResetCycleLabels();
            };
            btn.clicked += onClick;
            AddCleanup(() => btn.clicked -= onClick);
        }

        private void ApplyAndHighlight(string presetName, string buttonKey)
        {
            if (ServiceLocator.TryGet<EnvironmentController>(out var env))
                env.ApplyPreset(presetName);
            _activePreset = buttonKey;
            UpdateEnvPresetActiveState(buttonKey);
        }

        private void ResetCycleLabels()
        {
            _timeIndex = 0;
            _colorIndex = 0;
            _studioCycleIndex = 0;
            _isBlueprintMode = false;
            _displayedStudioPreset = "Studio";
            _displayedTimePreset = "Time";
            _displayedColorPreset = "Color";

            SetCycleLabel("EnvPreset_Studio", _displayedStudioPreset, true);
            UpdateCycleDots(_studioDots, _studioCycleIndex);

            // Restore Realistic base mode and view if Blueprint was active
            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.BaseMode = ViewMode.Realistic;
                if (ViewModeManager.Instance.CurrentMode == ViewMode.Blueprint)
                    ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
            }

            SetCycleLabel("EnvPreset_Night", _displayedTimePreset, false);
            UpdateCycleDots(_timeDots, _timeIndex);

            SetCycleLabel("EnvPreset_Blueprint", _displayedColorPreset, false);
            UpdateCycleDots(_colorDots, _colorIndex);
        }

        public void UpdateEnvPresetActiveState(string activePreset)
        {
            if (_envPanel == null) return;
            var buttons = new[] { "Studio", "Night", "Blueprint" };
            foreach (var p in buttons)
            {
                var btn = _envPanel.Q<Button>($"EnvPreset_{p}");
                if (btn != null) btn.EnableInClassList("submenu-card--active", p == activePreset);
            }
        }

        private void DeactivateBlueprintIfNeeded()
        {
            if (!_isBlueprintMode) return;
            _isBlueprintMode = false;

            _displayedStudioPreset = "Studio";
            SetCycleLabel("EnvPreset_Studio", _displayedStudioPreset, true);
            _studioCycleIndex = 0;
            UpdateCycleDots(_studioDots, _studioCycleIndex);

            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.BaseMode = ViewMode.Realistic;
                if (ViewModeManager.Instance.CurrentMode == ViewMode.Blueprint)
                    ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
            }
        }

        private void RefreshCycleLabels()
        {
            SetCycleLabel("EnvPreset_Studio", _displayedStudioPreset, true);
            SetCycleLabel("EnvPreset_Night", _displayedTimePreset, false);
            SetCycleLabel("EnvPreset_Blueprint", _displayedColorPreset, false);
        }

        private void SetCycleLabel(string buttonName, string englishLabel, bool multiline)
        {
            Label label = _envPanel?.Q<Button>(buttonName)?.Q<Label>(className: "submenu-label");
            if (label == null)
            {
                return;
            }

            label.text = FormatCycleLabel(englishLabel, multiline);
        }

        private static string FormatCycleLabel(string englishLabel, bool multiline)
        {
            if (string.IsNullOrWhiteSpace(englishLabel))
            {
                return string.Empty;
            }

            string upperKey = englishLabel.Trim().ToUpperInvariant();
            string translated = AppLanguageManager.TranslateStatic(upperKey);
            if (string.Equals(translated, upperKey, System.StringComparison.Ordinal))
            {
                translated = AppLanguageManager.TranslateStatic(englishLabel.Trim());
            }

            translated = translated.ToUpperInvariant();
            return multiline ? translated.Replace(" ", "\n") : translated;
        }
    }
}
