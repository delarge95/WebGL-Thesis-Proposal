using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    public class UIAnalyzePanel
    {
        private VisualElement _shaderMenu;
        private Button _shaderBtn;
        private List<System.Action> _cleanupActions = new List<System.Action>();

        public UIAnalyzePanel(VisualElement shaderMenu, Button shaderBtn)
        {
            _shaderMenu = shaderMenu;
            _shaderBtn = shaderBtn;
            BindShaderMenuButtons();
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

        private void BindShaderMenuButtons()
        {
            if (_shaderMenu == null) return;

            var modes = new[] { "Realistic", "XRay", "Blueprint", "SolidColor", "Wireframe", "Ghosted", "Thermal" };
            var enums = new[] { ViewMode.Realistic, ViewMode.XRay, ViewMode.Blueprint, ViewMode.SolidColor, ViewMode.Wireframe, ViewMode.Ghosted, ViewMode.Thermal };

            for (int i = 0; i < modes.Length; i++)
            {
                var btn = _shaderMenu.Q<Button>($"ShaderMode_{modes[i]}");
                if (btn == null) continue;
                var mode = enums[i];
                
                System.Action onClick = () =>
                {
                    // Toggle: re-click active mode → back to Realistic
                    if (ViewModeManager.Instance != null && ViewModeManager.Instance.CurrentMode == mode)
                        ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);
                    else
                        ViewModeManager.Instance?.SetViewMode(mode);
                };
                
                btn.clicked += onClick;
                AddCleanup(() => btn.clicked -= onClick);
            }
        }

        // Exposed for UIManager to call when mode changes
        public void OnViewModeChanged(ViewMode newMode)
        {
            UpdateShaderButtonVisual(newMode);
            UpdateShaderMenuActiveState(newMode);
            ApplyBackgroundForMode(newMode);
        }

        private void UpdateShaderButtonVisual(ViewMode mode)
        {
            if (_shaderBtn == null) return;
            bool isActive = mode != ViewMode.Realistic;
            _shaderBtn.EnableInClassList("btn-tag--active", isActive);
            _shaderBtn.tooltip = isActive ? $"View: {mode}" : "Toggle Render Mode";
        }

        private void UpdateShaderMenuActiveState(ViewMode activeMode)
        {
            if (_shaderMenu == null) return;
            var modes = new[] { "Realistic", "XRay", "Blueprint", "SolidColor", "Wireframe", "Ghosted", "Thermal" };
            var enums = new[] { ViewMode.Realistic, ViewMode.XRay, ViewMode.Blueprint, ViewMode.SolidColor, ViewMode.Wireframe, ViewMode.Ghosted, ViewMode.Thermal };
            for (int i = 0; i < modes.Length; i++)
            {
                var btn = _shaderMenu.Q<Button>($"ShaderMode_{modes[i]}");
                if (btn != null) btn.EnableInClassList("submenu-card--active", enums[i] == activeMode);
            }
        }

        private void ApplyBackgroundForMode(ViewMode mode)
        {
            Color bg;
            switch (mode)
            {
                case ViewMode.XRay:       bg = new Color(0.02f, 0.03f, 0.07f); break;
                case ViewMode.Blueprint:  bg = new Color(0.04f, 0.08f, 0.18f); break;
                case ViewMode.SolidColor: bg = new Color(0.08f, 0.08f, 0.10f); break;
                case ViewMode.Wireframe:  bg = new Color(0.03f, 0.03f, 0.05f); break;
                case ViewMode.Ghosted:    bg = new Color(0.05f, 0.05f, 0.07f); break;
                case ViewMode.Thermal:    bg = new Color(0.02f, 0.02f, 0.04f); break;
                default:                  bg = new Color(0.04f, 0.055f, 0.11f); break;
            }
            if (Camera.main != null) Camera.main.backgroundColor = bg;
        }
    }
}
