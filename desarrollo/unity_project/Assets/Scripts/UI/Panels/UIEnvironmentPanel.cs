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

            var presets = new[] { "Studio", "Night", "Blueprint", "Neutral" };
            foreach (var preset in presets)
            {
                var btn = _envPanel.Q<Button>($"EnvPreset_{preset}");
                if (btn == null) continue;
                var p = preset; 
                System.Action onClick = () =>
                {
                    if (ServiceLocator.TryGet<EnvironmentController>(out var env)) env.ApplyPreset(p);
                    UpdateEnvPresetActiveState(p);
                };
                btn.clicked += onClick;
                AddCleanup(() => btn.clicked -= onClick);
            }
        }

        public void UpdateEnvPresetActiveState(string activePreset)
        {
            if (_envPanel == null) return;
            var presets = new[] { "Studio", "Night", "Blueprint", "Neutral" };
            foreach (var p in presets)
            {
                var btn = _envPanel.Q<Button>($"EnvPreset_{p}");
                if (btn != null) btn.EnableInClassList("submenu-card--active", p == activePreset);
            }
        }
    }
}
