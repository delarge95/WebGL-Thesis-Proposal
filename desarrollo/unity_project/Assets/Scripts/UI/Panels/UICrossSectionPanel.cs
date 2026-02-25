using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// UI controller for the Cross-Section panel inside Analyze Mode.
    /// Supports up to 2 simultaneous clip planes for diagonal cuts.
    /// Axis buttons are multi-selectable (FIFO deselection on 3rd click).
    /// Phase 2 Iteration 10: Dual-plane cross-section.
    /// </summary>
    public class UICrossSectionPanel
    {
        // ── Elements ──
        private readonly VisualElement _panel;
        private readonly Button _toggleBtn;
        private readonly Button _axisXBtn;
        private readonly Button _axisYBtn;
        private readonly Button _axisZBtn;

        // Plane 1 controls
        private readonly Slider _positionSlider1;
        private readonly Button _invertBtn1;

        // Plane 2 controls
        private readonly VisualElement _controls2Container;
        private readonly Slider _positionSlider2;
        private readonly Button _invertBtn2;

        // ── State ──
        private bool _isEnabled = false;
        private List<CrossSectionAxis> _activeAxes = new List<CrossSectionAxis>();
        private bool _isInverted1 = false;
        private bool _isInverted2 = false;

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

            _positionSlider1 = _panel.Q<Slider>("CrossSectionPosition");
            _invertBtn1 = _panel.Q<Button>("CrossSectionInvertBtn");

            _controls2Container = _panel.Q<VisualElement>("CrossSectionControls2");
            _positionSlider2 = _panel.Q<Slider>("CrossSectionPosition2");
            _invertBtn2 = _panel.Q<Button>("CrossSectionInvertBtn2");

            // Initialize with default axis
            _activeAxes.Add(CrossSectionAxis.Y);

            BindButtons();
            BindSliders();
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
            if (_toggleBtn != null)
            {
                System.Action onToggle = OnToggleClicked;
                _toggleBtn.clicked += onToggle;
                AddCleanup(() => _toggleBtn.clicked -= onToggle);
            }

            if (_axisXBtn != null)
            {
                System.Action onX = () => OnAxisToggled(CrossSectionAxis.X);
                _axisXBtn.clicked += onX;
                AddCleanup(() => _axisXBtn.clicked -= onX);
            }
            if (_axisYBtn != null)
            {
                System.Action onY = () => OnAxisToggled(CrossSectionAxis.Y);
                _axisYBtn.clicked += onY;
                AddCleanup(() => _axisYBtn.clicked -= onY);
            }
            if (_axisZBtn != null)
            {
                System.Action onZ = () => OnAxisToggled(CrossSectionAxis.Z);
                _axisZBtn.clicked += onZ;
                AddCleanup(() => _axisZBtn.clicked -= onZ);
            }

            if (_invertBtn1 != null)
            {
                System.Action onInvert1 = OnInvert1Clicked;
                _invertBtn1.clicked += onInvert1;
                AddCleanup(() => _invertBtn1.clicked -= onInvert1);
            }

            if (_invertBtn2 != null)
            {
                System.Action onInvert2 = OnInvert2Clicked;
                _invertBtn2.clicked += onInvert2;
                AddCleanup(() => _invertBtn2.clicked -= onInvert2);
            }
        }

        private void BindSliders()
        {
            BindSlider(_positionSlider1, val => CrossSectionManager.Instance?.SetPosition1(val));
            BindSlider(_positionSlider2, val => CrossSectionManager.Instance?.SetPosition2(val));
        }

        private void BindSlider(Slider slider, System.Action<float> onChanged)
        {
            if (slider == null) return;

            EventCallback<ChangeEvent<float>> onChange = evt => onChanged?.Invoke(evt.newValue);
            slider.RegisterValueChangedCallback(onChange);
            AddCleanup(() => slider.UnregisterValueChangedCallback(onChange));

            EventCallback<PointerEnterEvent> onEnter = evt => InputManager.InputBlocked = true;
            EventCallback<PointerLeaveEvent> onLeave = evt => InputManager.InputBlocked = false;
            EventCallback<PointerDownEvent> onDown = evt => evt.StopPropagation();
            slider.RegisterCallback(onEnter);
            slider.RegisterCallback(onLeave);
            slider.RegisterCallback(onDown);
            AddCleanup(() =>
            {
                slider.UnregisterCallback(onEnter);
                slider.UnregisterCallback(onLeave);
                slider.UnregisterCallback(onDown);
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

        /// <summary>
        /// Toggle an axis on/off. Up to 2 can be active simultaneously.
        /// 3rd toggle uses FIFO deselection (oldest axis removed).
        /// Clicking an already-active axis deselects it (must keep at least 1).
        /// </summary>
        private void OnAxisToggled(CrossSectionAxis axis)
        {
            if (_activeAxes.Contains(axis))
            {
                // Deselect — but keep at least 1 axis
                if (_activeAxes.Count > 1)
                {
                    _activeAxes.Remove(axis);
                }
            }
            else
            {
                if (_activeAxes.Count >= 2)
                {
                    // FIFO: remove the oldest
                    _activeAxes.RemoveAt(0);
                }
                _activeAxes.Add(axis);
            }

            ApplyAxesToManager();
            UpdateAxisButtons();
            UpdatePlane2Visibility();
        }

        private void ApplyAxesToManager()
        {
            var mgr = CrossSectionManager.Instance;
            if (mgr == null) return;

            if (_activeAxes.Count >= 1)
            {
                mgr.SetAxis1(_activeAxes[0]);
            }

            if (_activeAxes.Count >= 2)
            {
                mgr.SetPlane2Active(true);
                mgr.SetAxis2(_activeAxes[1]);
            }
            else
            {
                mgr.SetPlane2Active(false);
            }
        }

        private void OnInvert1Clicked()
        {
            _isInverted1 = !_isInverted1;
            CrossSectionManager.Instance?.SetInverted1(_isInverted1);
            _invertBtn1?.EnableInClassList("submenu-card--active", _isInverted1);
        }

        private void OnInvert2Clicked()
        {
            _isInverted2 = !_isInverted2;
            CrossSectionManager.Instance?.SetInverted2(_isInverted2);
            _invertBtn2?.EnableInClassList("submenu-card--active", _isInverted2);
        }

        // ═══════════════════════════════════════════════════════
        //  Visual State
        // ═══════════════════════════════════════════════════════

        private void UpdateVisualState()
        {
            _toggleBtn?.EnableInClassList("submenu-card--active", _isEnabled);

            var controlsContainer = _panel?.Q<VisualElement>("CrossSectionControls");
            if (controlsContainer != null)
            {
                controlsContainer.style.display = _isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
                controlsContainer.style.opacity = _isEnabled ? 1f : 0f;
            }

            UpdateAxisButtons();
            UpdatePlane2Visibility();
        }

        private void UpdateAxisButtons()
        {
            _axisXBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.X));
            _axisYBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.Y));
            _axisZBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.Z));
        }

        private void UpdatePlane2Visibility()
        {
            bool show2 = _isEnabled && _activeAxes.Count >= 2;
            if (_controls2Container != null)
            {
                _controls2Container.style.display = show2 ? DisplayStyle.Flex : DisplayStyle.None;
                _controls2Container.style.opacity = show2 ? 1f : 0f;
            }
        }
    }
}
