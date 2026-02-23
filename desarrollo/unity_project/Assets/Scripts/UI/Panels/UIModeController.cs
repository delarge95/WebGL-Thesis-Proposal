using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the 3-mode system: Explore, Analyze, Studio.
    /// Replaces UIPopupController (Phase 2 Iteration 3: UX Redesign).
    /// 
    /// Responsibilities:
    ///   - Activate/deactivate mode containers (mutual exclusion: only 1 mode active)
    ///   - Manage sub-menus within Analyze mode (ShaderMenu, CategoryMenu, SliderContainer)
    ///   - Handle hotspot toggles and category filters
    ///   - PopupBlocker: click outside → deactivate current mode
    /// </summary>
    public class UIModeController
    {
        // ── Mode Containers ──
        private readonly VisualElement _root;
        private readonly VisualElement _analyzeModeContainer;
        private readonly VisualElement _studioModeContainer;
        private readonly VisualElement _popupBlocker;

        // ── Analyze sub-elements ──
        private readonly VisualElement _shaderMenu;
        private readonly VisualElement _categoryMenu;
        private readonly VisualElement _sliderContainer;
        private readonly Slider _explosionSlider;
        private readonly Button _hotspotBtn;

        // ── Mode Buttons ──
        private readonly Button _modeExploreBtn;
        private readonly Button _modeAnalyzeBtn;
        private readonly Button _modeStudioBtn;

        // ── State ──
        private AppState _activeMode = AppState.Exploration;
        private bool _shaderMenuShown = false;
        private bool _hotspotsEnabled = false;
        private bool _isSheetOpen = false;
        private List<string> _activeCategories = new List<string>() { "ALL" };

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        public UIModeController(
            VisualElement root,
            VisualElement analyzeModeContainer,
            VisualElement studioModeContainer,
            VisualElement popupBlocker,
            Slider explosionSlider,
            Button hotspotBtn)
        {
            _root = root;
            _analyzeModeContainer = analyzeModeContainer;
            _studioModeContainer = studioModeContainer;
            _popupBlocker = popupBlocker;
            _explosionSlider = explosionSlider;
            _hotspotBtn = hotspotBtn;

            // Query sub-elements from AnalyzeModeContainer
            if (_analyzeModeContainer != null)
            {
                _shaderMenu = _analyzeModeContainer.Q<VisualElement>("ShaderMenu");
                _categoryMenu = _analyzeModeContainer.Q<VisualElement>("CategoryMenu");
                _sliderContainer = _analyzeModeContainer.Q<VisualElement>("SliderContainer");
            }

            // Query mode buttons from root
            _modeExploreBtn = root?.Q<Button>("ModeExploreBtn");
            _modeAnalyzeBtn = root?.Q<Button>("ModeAnalyzeBtn");
            _modeStudioBtn = root?.Q<Button>("ModeStudioBtn");

            BindModeButtons();
            BindPopupBlocker();
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
        //  Mode Button Binding
        // ═══════════════════════════════════════════════════════

        private void BindModeButtons()
        {
            if (_modeExploreBtn != null)
            {
                System.Action onExplore = () => ActivateMode(AppState.Exploration);
                _modeExploreBtn.clicked += onExplore;
                AddCleanup(() => _modeExploreBtn.clicked -= onExplore);
            }

            if (_modeAnalyzeBtn != null)
            {
                System.Action onAnalyze = () =>
                {
                    if (_activeMode == AppState.Analyze)
                        ActivateMode(AppState.Exploration); // Toggle off
                    else
                        ActivateMode(AppState.Analyze);
                };
                _modeAnalyzeBtn.clicked += onAnalyze;
                AddCleanup(() => _modeAnalyzeBtn.clicked -= onAnalyze);
            }

            if (_modeStudioBtn != null)
            {
                System.Action onStudio = () =>
                {
                    if (_activeMode == AppState.Studio)
                        ActivateMode(AppState.Exploration); // Toggle off
                    else
                        ActivateMode(AppState.Studio);
                };
                _modeStudioBtn.clicked += onStudio;
                AddCleanup(() => _modeStudioBtn.clicked -= onStudio);
            }
        }

        private void BindPopupBlocker()
        {
            if (_popupBlocker == null) return;
            EventCallback<PointerDownEvent> pbDown = evt =>
            {
                // Click outside active mode → return to Explore
                if (_activeMode != AppState.Exploration)
                    ActivateMode(AppState.Exploration);
            };
            _popupBlocker.RegisterCallback(pbDown);
            AddCleanup(() => _popupBlocker.UnregisterCallback(pbDown));
        }

        // ═══════════════════════════════════════════════════════
        //  Public API — Mode Management
        // ═══════════════════════════════════════════════════════

        /// <summary>Activates the specified mode, showing its container and hiding others.</summary>
        public void ActivateMode(AppState mode)
        {
            _activeMode = mode;

            // Update AppStateMachine
            if (AppStateMachine.Instance != null)
            {
                switch (mode)
                {
                    case AppState.Exploration:
                        AppStateMachine.Instance.EnterExploration();
                        break;
                    case AppState.Analyze:
                        AppStateMachine.Instance.EnterAnalyze();
                        break;
                    case AppState.Studio:
                        AppStateMachine.Instance.EnterStudio();
                        break;
                }
            }

            // Show/hide containers
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            UpdatePopupBlocker();
        }

        /// <summary>Deactivates all modes, returning to Explore.</summary>
        public void DeactivateAllModes()
        {
            ActivateMode(AppState.Exploration);
        }

        /// <summary>Called by OnAppStateChanged — sync mode if state changed externally.</summary>
        public void SyncWithAppState(AppState newState)
        {
            if (newState == AppState.Exploration || newState == AppState.Analyze || newState == AppState.Studio)
            {
                _activeMode = newState;
                UpdateContainerVisibility();
                UpdateModeButtonStates();
                UpdatePopupBlocker();
            }
            else if (newState == AppState.ExplodedView || newState == AppState.FocusMode)
            {
                // Keep current mode UI but don't change containers
                // ExplodedView/FocusMode are sub-states within Analyze
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Sub-Menu Toggles (within AnalyzeModeContainer)
        // ═══════════════════════════════════════════════════════

        public void ToggleShaderMenu()
        {
            if (_shaderMenu == null) return;
            _shaderMenuShown = !_shaderMenuShown;

            if (_shaderMenuShown)
            {
                if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
            }

            _shaderMenu.EnableInClassList("submenu--hidden", !_shaderMenuShown);
        }

        public void ToggleCategoryMenu()
        {
            if (_categoryMenu == null) return;
            _categoryMenu.ToggleInClassList("submenu--hidden");

            if (!_categoryMenu.ClassListContains("submenu--hidden"))
            {
                if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
            }
        }

        /// <summary>Closes all sub-menus within the current mode.</summary>
        public void CloseAllMenus()
        {
            if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
            if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
        }

        // ═══════════════════════════════════════════════════════
        //  Sheet Integration
        // ═══════════════════════════════════════════════════════

        /// <summary>Notify when sheet opens/closes.</summary>
        public void SetSheetOpenState(bool isOpen)
        {
            _isSheetOpen = isOpen;
            if (isOpen) CloseAllMenus();
        }

        // ═══════════════════════════════════════════════════════
        //  Hotspots
        // ═══════════════════════════════════════════════════════

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();

            if (_hotspotBtn != null)
                _hotspotBtn.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

        // ═══════════════════════════════════════════════════════
        //  Category Filters
        // ═══════════════════════════════════════════════════════

        public void SetCategoryFilter(string category, Button clickedBtn)
        {
            if (category == "ALL")
            {
                _activeCategories.Clear();
                _activeCategories.Add("ALL");
            }
            else
            {
                if (_activeCategories.Contains("ALL"))
                    _activeCategories.Remove("ALL");

                if (_activeCategories.Contains(category))
                    _activeCategories.Remove(category);
                else
                    _activeCategories.Add(category);

                if (_activeCategories.Count == 0)
                    _activeCategories.Add("ALL");
            }

            ExplodedViewManager.Instance?.SetCategoryFilters(_activeCategories);
            UpdateCategoryButtonStates();
        }

        // ═══════════════════════════════════════════════════════
        //  Slider (Exploded View)
        // ═══════════════════════════════════════════════════════

        /// <summary>Shows/hides the explosion slider. Called by AppState changes.</summary>
        public void SetSliderVisible(bool visible)
        {
            if (_sliderContainer == null) return;

            if (visible)
            {
                _sliderContainer.RemoveFromClassList("slider-hidden");
                if (_explosionSlider != null)
                    _explosionSlider.SetValueWithoutNotify(0.5f);
            }
            else
            {
                _sliderContainer.AddToClassList("slider-hidden");
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Private — Visibility Management
        // ═══════════════════════════════════════════════════════

        private void UpdateContainerVisibility()
        {
            // Analyze container
            if (_analyzeModeContainer != null)
                _analyzeModeContainer.EnableInClassList("mode--hidden", _activeMode != AppState.Analyze);

            // Studio container
            if (_studioModeContainer != null)
                _studioModeContainer.EnableInClassList("mode--hidden", _activeMode != AppState.Studio);

            // When leaving a mode, close its sub-menus
            if (_activeMode != AppState.Analyze)
                CloseAllMenus();
        }

        private void UpdateModeButtonStates()
        {
            _modeExploreBtn?.EnableInClassList("mode-btn--active", _activeMode == AppState.Exploration);
            _modeAnalyzeBtn?.EnableInClassList("mode-btn--active", _activeMode == AppState.Analyze);
            _modeStudioBtn?.EnableInClassList("mode-btn--active", _activeMode == AppState.Studio);
        }

        private void UpdatePopupBlocker()
        {
            bool anyModeOpen = _activeMode != AppState.Exploration;
            _popupBlocker?.EnableInClassList("popup-blocker--hidden", !anyModeOpen);
        }

        private void UpdateCategoryButtonStates()
        {
            if (_categoryMenu == null) return;

            void UpdateBtn(string btnName, string catName)
            {
                var btn = _categoryMenu.Q<Button>(btnName);
                if (btn != null) btn.EnableInClassList("submenu-card--active", _activeCategories.Contains(catName));
            }

            UpdateBtn("CatBtn_All", "ALL");
            UpdateBtn("CatBtn_Structure", "Structure");
            UpdateBtn("CatBtn_Propulsion", "Propulsion");
            UpdateBtn("CatBtn_Avionics", "Avionics");
            UpdateBtn("CatBtn_Power", "Power");
        }
    }
}
