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
        private List<System.Action> _cleanupActions = new List<System.Action>();

        // ── Cycle state for TIME, COLOR, and STUDIO buttons ──
        private static readonly string[] TimeCycle = { "Day", "Night", "Sunset" };
        private static readonly string[] ColorCycle = { "White", "Grey", "Black", "Yellow", "Orange", "Green", "Blue", "Purple", "Red" };
        private static readonly string[] StudioCycle = { "Studio", "Studio Light", "Blueprint" };
        private int _timeIndex = 0;
        private int _colorIndex = 0;
        private int _studioCycleIndex = 0;
        private string _activePreset = "Studio";
        private bool _isBlueprintMode = false;

        public UIEnvironmentPanel(VisualElement envPanel)
        {
            _envPanel = envPanel;
            BindEnvPanel();
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

            if (_envLightRotSlider != null)
            {
                EventCallback<ChangeEvent<float>> onRotChanged = evt => 
                {
                    if (ServiceLocator.TryGet<EnvironmentController>(out var env)) env.SetLightRotation(evt.newValue);
                };
                _envLightRotSlider.RegisterValueChangedCallback(onRotChanged);
                AddCleanup(() => _envLightRotSlider.UnregisterValueChangedCallback(onRotChanged));

                EventCallback<PointerEnterEvent> onEnter = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> onLeave = evt => InputManager.InputBlocked = false;
                EventCallback<PointerDownEvent> onDown = evt => evt.StopPropagation();

                _envLightRotSlider.RegisterCallback(onEnter);
                _envLightRotSlider.RegisterCallback(onLeave);
                _envLightRotSlider.RegisterCallback(onDown);

                AddCleanup(() => {
                    _envLightRotSlider.UnregisterCallback(onEnter);
                    _envLightRotSlider.UnregisterCallback(onLeave);
                    _envLightRotSlider.UnregisterCallback(onDown);
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

                EventCallback<PointerEnterEvent> onEnter2 = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> onLeave2 = evt => InputManager.InputBlocked = false;
                EventCallback<PointerDownEvent> onDown2 = evt => evt.StopPropagation();

                _envLightIntSlider.RegisterCallback(onEnter2);
                _envLightIntSlider.RegisterCallback(onLeave2);
                _envLightIntSlider.RegisterCallback(onDown2);

                AddCleanup(() => {
                    _envLightIntSlider.UnregisterCallback(onEnter2);
                    _envLightIntSlider.UnregisterCallback(onLeave2);
                    _envLightIntSlider.UnregisterCallback(onDown2);
                });
            }

            // ── Studio button — cycles Studio → Studio Light → Blueprint ──
            var studioBtn = _envPanel.Q<Button>("EnvPreset_Studio");
            if (studioBtn != null)
            {
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

                    var label = studioBtn.Q<Label>(className: "submenu-label");
                    if (label != null) label.text = preset.Replace(" ", "\n").ToUpper();
                };
                studioBtn.clicked += onStudioClick;
                AddCleanup(() => studioBtn.clicked -= onStudioClick);
            }

            // ── TIME button — cycles Day / Night / Sunset ──
            var nightBtn = _envPanel.Q<Button>("EnvPreset_Night");
            if (nightBtn != null)
            {
                System.Action onTimeClick = () =>
                {
                    string preset = TimeCycle[_timeIndex];
                    _timeIndex = (_timeIndex + 1) % TimeCycle.Length;
                    ApplyAndHighlight(preset, "Night");
                    DeactivateBlueprintIfNeeded();

                    // Update label to show current preset name
                    var label = nightBtn.Q<Label>(className: "submenu-label");
                    if (label != null) label.text = preset.ToUpper();
                };
                nightBtn.clicked += onTimeClick;
                AddCleanup(() => nightBtn.clicked -= onTimeClick);
            }

            // ── COLOR button — cycles 9 background colors ──
            var blueBtn = _envPanel.Q<Button>("EnvPreset_Blueprint");
            if (blueBtn != null)
            {
                System.Action onColorClick = () =>
                {
                    string preset = ColorCycle[_colorIndex];
                    _colorIndex = (_colorIndex + 1) % ColorCycle.Length;
                    ApplyAndHighlight(preset, "Blueprint");
                    DeactivateBlueprintIfNeeded();

                    // Update label to show current color name
                    var label = blueBtn.Q<Label>(className: "submenu-label");
                    if (label != null) label.text = preset.ToUpper();
                };
                blueBtn.clicked += onColorClick;
                AddCleanup(() => blueBtn.clicked -= onColorClick);
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

            var studioLabel = _envPanel.Q<Button>("EnvPreset_Studio")?.Q<Label>(className: "submenu-label");
            if (studioLabel != null) studioLabel.text = "STUDIO";

            // Restore Realistic base mode and view if Blueprint was active
            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.BaseMode = ViewMode.Realistic;
                if (ViewModeManager.Instance.CurrentMode == ViewMode.Blueprint)
                    ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
            }

            var nightLabel = _envPanel.Q<Button>("EnvPreset_Night")?.Q<Label>(className: "submenu-label");
            if (nightLabel != null) nightLabel.text = "TIME";

            var blueLabel = _envPanel.Q<Button>("EnvPreset_Blueprint")?.Q<Label>(className: "submenu-label");
            if (blueLabel != null) blueLabel.text = "COLOR";
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

            var studioLabel = _envPanel.Q<Button>("EnvPreset_Studio")?.Q<Label>(className: "submenu-label");
            if (studioLabel != null) studioLabel.text = "STUDIO";

            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.BaseMode = ViewMode.Realistic;
                if (ViewModeManager.Instance.CurrentMode == ViewMode.Blueprint)
                    ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
            }
        }
    }
}
