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
        private bool _isCombined = false;
        private readonly Button _combineBtn;

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
            _axisXBtn = _panel.Q<Button>("CrossSectionAxisX");
            _axisYBtn = _panel.Q<Button>("CrossSectionAxisY");
            _axisZBtn = _panel.Q<Button>("CrossSectionAxisZ");

            _positionSlider1 = _panel.Q<Slider>("CrossSectionPosition");
            _invertBtn1 = _panel.Q<Button>("CrossSectionInvertBtnInline") ?? _panel.Q<Button>("CrossSectionInvertBtn");
            _combineBtn = _panel.Q<Button>("CrossSectionCombineBtn");

            _controls2Container = _panel.Q<VisualElement>("CrossSectionControls2");
            _positionSlider2 = _panel.Q<Slider>("CrossSectionPosition2");
            _invertBtn2 = _panel.Q<Button>("CrossSectionInvertBtn2Inline") ?? _panel.Q<Button>("CrossSectionInvertBtn2");

            // Initialize with default axis
            _activeAxes.Add(CrossSectionAxis.Y);

            // Cross-section is always enabled when panel is visible (no toggle needed)
            _isEnabled = true;

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

            if (_combineBtn != null)
            {
                System.Action onCombine = OnCombineClicked;
                _combineBtn.clicked += onCombine;
                AddCleanup(() => _combineBtn.clicked -= onCombine);
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
            slider.RegisterCallback(onDown);
            slider.RegisterCallback(onUp);
            AddCleanup(() =>
            {
                slider.UnregisterCallback(onDown);
                slider.UnregisterCallback(onUp);
            });
        }

        // ═══════════════════════════════════════════════════════
        //  Event Handlers
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Toggle an axis on/off. Up to 2 can be active simultaneously.
        /// 3rd toggle uses FIFO deselection (oldest axis removed).
        /// Clicking an already-active axis deselects it (must keep at least 1).
        /// </summary>
        private void OnAxisToggled(CrossSectionAxis axis)
        {
            if (_activeAxes.Contains(axis))
            {
                _activeAxes.Remove(axis);
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

            // If all axes deselected → disable cross-section entirely
            if (_activeAxes.Count == 0)
            {
                CrossSectionManager.Instance?.DisableCrossSection();
            }

            ApplyAxesToManager();
            UpdateVisualState();
        }

        private void ApplyAxesToManager()
        {
            var mgr = CrossSectionManager.Instance;
            if (mgr == null) return;

            // If user re-selects an axis after turning all of them off,
            // we must re-enable cross-section immediately from this panel.
            if (_activeAxes.Count > 0 && !mgr.IsEnabled)
            {
                mgr.EnableCrossSection();
            }

            if (_activeAxes.Count >= 1)
            {
                mgr.SetPlane1Active(true);
                mgr.SetAxis1(_activeAxes[0]);
            }
            else
            {
                mgr.SetPlane1Active(false);
            }

            if (_activeAxes.Count >= 2)
            {
                mgr.SetPlane2Active(true);
                mgr.SetAxis2(_activeAxes[1]);
            }
            else
            {
                mgr.SetPlane2Active(false);
                
                // If dropping back to 1 axis or 0, disable combination mode
                if (_isCombined)
                {
                    _isCombined = false;
                    mgr.SetCombinePlanes(false);
                    _combineBtn?.RemoveFromClassList("submenu-card--active");
                }
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

        private void OnCombineClicked()
        {
            _isCombined = !_isCombined;
            CrossSectionManager.Instance?.SetCombinePlanes(_isCombined);
            _combineBtn?.EnableInClassList("submenu-card--active", _isCombined);
            UpdateVisualState();
        }

        // ═══════════════════════════════════════════════════════
        //  Visual State
        // ═══════════════════════════════════════════════════════

        private void UpdateVisualState()
        {
            // Controls are always visible when the panel is shown (no wrapper to toggle)
            UpdateAxisButtons();
            UpdateCombineButtonVisibility();
            UpdatePlane2Visibility();
        }

        private void UpdateAxisButtons()
        {
            _axisXBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.X));
            _axisYBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.Y));
            _axisZBtn?.EnableInClassList("submenu-card--active", _activeAxes.Contains(CrossSectionAxis.Z));
        }

        private void UpdateCombineButtonVisibility()
        {
            if (_combineBtn == null)
            {
                return;
            }

            bool showCombine = _activeAxes.Count >= 2;
            _combineBtn.EnableInClassList("cross-section-config-btn--visible", showCombine);
            _combineBtn.EnableInClassList("cross-section-config-btn--hidden", !showCombine);
            _combineBtn.pickingMode = showCombine ? PickingMode.Position : PickingMode.Ignore;
        }

        private void UpdatePlane2Visibility()
        {
            bool show2 = _isEnabled && _activeAxes.Count >= 2 && !_isCombined;
            if (_controls2Container != null)
            {
                _controls2Container.EnableInClassList("cross-section-controls--visible", show2);
                _controls2Container.EnableInClassList("cross-section-controls--hidden", !show2);
                _controls2Container.pickingMode = show2 ? PickingMode.Position : PickingMode.Ignore;
            }
        }
    }
}
