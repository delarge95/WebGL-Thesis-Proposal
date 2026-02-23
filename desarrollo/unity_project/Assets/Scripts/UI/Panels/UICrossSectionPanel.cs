using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// UI controller for the Cross-Section panel inside Analyze Mode.
    /// Binds to CrossSectionManager's public API:
    ///   - Toggle on/off
    ///   - Axis selection (X, Y, Z)
    ///   - Position slider
    ///   - Invert direction toggle
    /// Phase 2 Iteration 5: Cross-Section UI.
    /// </summary>
    public class UICrossSectionPanel
    {
        // ── Elements ──
        private readonly VisualElement _panel;
        private readonly Button _toggleBtn;
        private readonly Button _axisXBtn;
        private readonly Button _axisYBtn;
        private readonly Button _axisZBtn;
        private readonly Slider _positionSlider;
        private readonly Button _invertBtn;

        // ── State ──
        private bool _isEnabled = false;
        private CrossSectionAxis _currentAxis = CrossSectionAxis.Y;
        private bool _isInverted = false;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        public UICrossSectionPanel(VisualElement crossSectionPanel)
        {
            _panel = crossSectionPanel;
            if (_panel == null)
            {
                Debug.LogWarning("[UICrossSectionPanel] CrossSectionPanel not found in UXML.");
                return;
            }

            // Query elements
            _toggleBtn = _panel.Q<Button>("CrossSectionToggleBtn");
            _axisXBtn = _panel.Q<Button>("CrossSectionAxisX");
            _axisYBtn = _panel.Q<Button>("CrossSectionAxisY");
            _axisZBtn = _panel.Q<Button>("CrossSectionAxisZ");
            _positionSlider = _panel.Q<Slider>("CrossSectionPosition");
            _invertBtn = _panel.Q<Button>("CrossSectionInvertBtn");

            BindButtons();
            BindSlider();
            UpdateVisualState();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Binding
        // ═══════════════════════════════════════════════════════

        private void BindButtons()
        {
            // Toggle
            if (_toggleBtn != null)
            {
                System.Action onToggle = OnToggleClicked;
                _toggleBtn.clicked += onToggle;
                AddCleanup(() => _toggleBtn.clicked -= onToggle);
            }

            // Axis buttons
            if (_axisXBtn != null)
            {
                System.Action onX = () => OnAxisSelected(CrossSectionAxis.X);
                _axisXBtn.clicked += onX;
                AddCleanup(() => _axisXBtn.clicked -= onX);
            }
            if (_axisYBtn != null)
            {
                System.Action onY = () => OnAxisSelected(CrossSectionAxis.Y);
                _axisYBtn.clicked += onY;
                AddCleanup(() => _axisYBtn.clicked -= onY);
            }
            if (_axisZBtn != null)
            {
                System.Action onZ = () => OnAxisSelected(CrossSectionAxis.Z);
                _axisZBtn.clicked += onZ;
                AddCleanup(() => _axisZBtn.clicked -= onZ);
            }

            // Invert
            if (_invertBtn != null)
            {
                System.Action onInvert = OnInvertClicked;
                _invertBtn.clicked += onInvert;
                AddCleanup(() => _invertBtn.clicked -= onInvert);
            }
        }

        private void BindSlider()
        {
            if (_positionSlider == null) return;

            EventCallback<ChangeEvent<float>> onChange = evt =>
            {
                CrossSectionManager.Instance?.SetPosition(evt.newValue);
            };
            _positionSlider.RegisterValueChangedCallback(onChange);
            AddCleanup(() => _positionSlider.UnregisterValueChangedCallback(onChange));

            // Block 3D input while dragging slider
            EventCallback<PointerEnterEvent> onEnter = evt => InputManager.InputBlocked = true;
            EventCallback<PointerLeaveEvent> onLeave = evt => InputManager.InputBlocked = false;
            EventCallback<PointerDownEvent> onDown = evt => evt.StopPropagation();
            _positionSlider.RegisterCallback(onEnter);
            _positionSlider.RegisterCallback(onLeave);
            _positionSlider.RegisterCallback(onDown);
            AddCleanup(() =>
            {
                _positionSlider.UnregisterCallback(onEnter);
                _positionSlider.UnregisterCallback(onLeave);
                _positionSlider.UnregisterCallback(onDown);
            });
        }

        // ═══════════════════════════════════════════════════════
        //  Event Handlers
        // ═══════════════════════════════════════════════════════

        private void OnToggleClicked()
        {
            CrossSectionManager.Instance?.ToggleCrossSection();
            _isEnabled = CrossSectionManager.Instance?.IsEnabled ?? false;
            UpdateVisualState();
        }

        private void OnAxisSelected(CrossSectionAxis axis)
        {
            _currentAxis = axis;
            CrossSectionManager.Instance?.SetAxis(axis);
            UpdateAxisButtons();
        }

        private void OnInvertClicked()
        {
            _isInverted = !_isInverted;
            CrossSectionManager.Instance?.SetInverted(_isInverted);
            _invertBtn?.EnableInClassList("submenu-card--active", _isInverted);
        }

        // ═══════════════════════════════════════════════════════
        //  Visual State
        // ═══════════════════════════════════════════════════════

        private void UpdateVisualState()
        {
            _toggleBtn?.EnableInClassList("submenu-card--active", _isEnabled);

            // Show/hide axis controls based on enabled state
            var controlsContainer = _panel?.Q<VisualElement>("CrossSectionControls");
            if (controlsContainer != null)
            {
                controlsContainer.style.display = _isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
                controlsContainer.style.opacity = _isEnabled ? 1f : 0f;
            }

            UpdateAxisButtons();
        }

        private void UpdateAxisButtons()
        {
            _axisXBtn?.EnableInClassList("submenu-card--active", _currentAxis == CrossSectionAxis.X);
            _axisYBtn?.EnableInClassList("submenu-card--active", _currentAxis == CrossSectionAxis.Y);
            _axisZBtn?.EnableInClassList("submenu-card--active", _currentAxis == CrossSectionAxis.Z);
        }
    }
}
