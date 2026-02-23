using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the 3-mode toolbar system: Tools, Analyze, Studio.
    /// Each mode has an action bar with sub-action buttons that toggle panels/features.
    ///
    /// Tools Mode  → Info (BottomSheet), Explode (slider + categories), Hotspots (toggle)
    /// Analyze Mode → Shaders (menu), Cross-Section (panel)
    /// Studio Mode  → Environment presets + lighting sliders (always visible)
    ///
    /// Responsibilities:
    ///   - Activate/deactivate mode containers (mutual exclusion: only 1 mode active)
    ///   - Manage sub-menus within each mode via action bar buttons
    ///   - Handle hotspot toggles and category filters
    ///   - Track active action button states (CSS class toggle)
    /// </summary>
    public class UIModeController
    {
        // ── Mode Containers ──
        private readonly VisualElement _root;
        private readonly VisualElement _toolsModeContainer;
        private readonly VisualElement _analyzeModeContainer;
        private readonly VisualElement _studioModeContainer;

        // ── Tools mode sub-elements ──
        private readonly Button _toolInfoBtn;
        private readonly Button _toolExplodeBtn;
        private readonly Button _toolHotspotBtn;
        private readonly VisualElement _sliderContainer;
        private readonly Slider _explosionSlider;
        private readonly VisualElement _categoryMenu;

        // ── Analyze mode sub-elements ──
        private readonly Button _analyzeShaderBtn;
        private readonly Button _analyzeCrossSectionBtn;
        private readonly VisualElement _shaderMenu;
        private readonly VisualElement _crossSectionPanel;

        // ── Mode Buttons (bottom bar) ──
        private readonly Button _modeToolsBtn;
        private readonly Button _modeAnalyzeBtn;
        private readonly Button _modeStudioBtn;

        // ── Internal mode enum ──
        private enum ActiveMode { None, Tools, Analyze, Studio }

        // ── State ──
        private ActiveMode _activeMode = ActiveMode.None;
        private bool _shaderMenuShown = false;
        private bool _crossSectionPanelShown = false;
        private bool _hotspotsEnabled = false;
        private bool _isSheetOpen = false;
        private bool _isExploded = false;
        private List<string> _activeCategories = new List<string>() { "ALL" };

        // ── Callbacks (wired by UIManager) ──
        /// <summary>Fired when Info action button is clicked.</summary>
        public event System.Action OnInfoToggleRequested;
        /// <summary>Fired when Explode action button is clicked.</summary>
        public event System.Action OnExplodeToggleRequested;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        public UIModeController(
            VisualElement root,
            VisualElement toolsModeContainer,
            VisualElement analyzeModeContainer,
            VisualElement studioModeContainer,
            Slider explosionSlider)
        {
            _root = root;
            _toolsModeContainer = toolsModeContainer;
            _analyzeModeContainer = analyzeModeContainer;
            _studioModeContainer = studioModeContainer;
            _explosionSlider = explosionSlider;

            // Query Tools mode sub-elements
            if (_toolsModeContainer != null)
            {
                _toolInfoBtn = _toolsModeContainer.Q<Button>("ToolInfoBtn");
                _toolExplodeBtn = _toolsModeContainer.Q<Button>("ToolExplodeBtn");
                _toolHotspotBtn = _toolsModeContainer.Q<Button>("ToolHotspotBtn");
                _sliderContainer = _toolsModeContainer.Q<VisualElement>("SliderContainer");
                _categoryMenu = _toolsModeContainer.Q<VisualElement>("CategoryMenu");
            }

            // Query Analyze mode sub-elements
            if (_analyzeModeContainer != null)
            {
                _analyzeShaderBtn = _analyzeModeContainer.Q<Button>("AnalyzeShaderBtn");
                _analyzeCrossSectionBtn = _analyzeModeContainer.Q<Button>("AnalyzeCrossSectionBtn");
                _shaderMenu = _analyzeModeContainer.Q<VisualElement>("ShaderMenu");
                _crossSectionPanel = _analyzeModeContainer.Q<VisualElement>("CrossSectionPanel");
            }

            // Query mode buttons from bottom bar
            _modeToolsBtn = root?.Q<Button>("ModeToolsBtn");
            _modeAnalyzeBtn = root?.Q<Button>("ModeAnalyzeBtn");
            _modeStudioBtn = root?.Q<Button>("ModeStudioBtn");

            BindModeButtons();
            BindToolsActionButtons();
            BindAnalyzeActionButtons();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            OnInfoToggleRequested = null;
            OnExplodeToggleRequested = null;
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Mode Button Binding (bottom bar: Tools / Analyze / Studio)
        // ═══════════════════════════════════════════════════════

        private void BindModeButtons()
        {
            if (_modeToolsBtn != null)
            {
                System.Action onTools = () =>
                {
                    if (_activeMode == ActiveMode.Tools)
                        DeactivateAllModes(); // Toggle off
                    else
                        ActivateMode(ActiveMode.Tools);
                };
                _modeToolsBtn.clicked += onTools;
                AddCleanup(() => _modeToolsBtn.clicked -= onTools);
            }

            if (_modeAnalyzeBtn != null)
            {
                System.Action onAnalyze = () =>
                {
                    if (_activeMode == ActiveMode.Analyze)
                        DeactivateAllModes();
                    else
                        ActivateMode(ActiveMode.Analyze);
                };
                _modeAnalyzeBtn.clicked += onAnalyze;
                AddCleanup(() => _modeAnalyzeBtn.clicked -= onAnalyze);
            }

            if (_modeStudioBtn != null)
            {
                System.Action onStudio = () =>
                {
                    if (_activeMode == ActiveMode.Studio)
                        DeactivateAllModes();
                    else
                        ActivateMode(ActiveMode.Studio);
                };
                _modeStudioBtn.clicked += onStudio;
                AddCleanup(() => _modeStudioBtn.clicked -= onStudio);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Tools Mode — Action Button Binding
        // ═══════════════════════════════════════════════════════

        private void BindToolsActionButtons()
        {
            if (_toolInfoBtn != null)
            {
                System.Action onInfo = () => OnInfoToggleRequested?.Invoke();
                _toolInfoBtn.clicked += onInfo;
                AddCleanup(() => _toolInfoBtn.clicked -= onInfo);
            }

            if (_toolExplodeBtn != null)
            {
                System.Action onExplode = () => OnExplodeToggleRequested?.Invoke();
                _toolExplodeBtn.clicked += onExplode;
                AddCleanup(() => _toolExplodeBtn.clicked -= onExplode);
            }

            if (_toolHotspotBtn != null)
            {
                System.Action onHotspot = () => ToggleHotspots();
                _toolHotspotBtn.clicked += onHotspot;
                AddCleanup(() => _toolHotspotBtn.clicked -= onHotspot);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Mode — Action Button Binding
        // ═══════════════════════════════════════════════════════

        private void BindAnalyzeActionButtons()
        {
            if (_analyzeShaderBtn != null)
            {
                System.Action onShader = () => ToggleShaderMenu();
                _analyzeShaderBtn.clicked += onShader;
                AddCleanup(() => _analyzeShaderBtn.clicked -= onShader);
            }

            if (_analyzeCrossSectionBtn != null)
            {
                System.Action onCross = () => ToggleCrossSectionPanel();
                _analyzeCrossSectionBtn.clicked += onCross;
                AddCleanup(() => _analyzeCrossSectionBtn.clicked -= onCross);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Public API — Mode Management
        // ═══════════════════════════════════════════════════════

        /// <summary>Activates the specified mode, showing its container and hiding others.</summary>
        private void ActivateMode(ActiveMode mode)
        {
            _activeMode = mode;
            CloseAllMenus();
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
        }

        /// <summary>Deactivates all modes — returns to base Exploration state.</summary>
        public void DeactivateAllModes()
        {
            _activeMode = ActiveMode.None;
            CloseAllMenus();
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
        }

        /// <summary>Called by OnAppStateChanged — syncs UI mode to reflect external state changes.</summary>
        public void SyncWithAppState(AppState newState)
        {
            // ExplodedView: show slider + category menu, mark explode button active
            if (newState == AppState.ExplodedView)
            {
                _isExploded = true;
                SetSliderVisible(true);
                ShowCategoryMenu(true);
                UpdateExplodeButtonState();
                return;
            }

            // Leaving ExplodedView
            if (_isExploded && newState != AppState.ExplodedView)
            {
                _isExploded = false;
                SetSliderVisible(false);
                ShowCategoryMenu(false);
                UpdateExplodeButtonState();
            }

            // Don't force mode switch for FocusMode — keep current mode
            if (newState == AppState.FocusMode) return;

            // Sync mode from external state changes (e.g. keyboard shortcut)
            if (newState == AppState.Analyze && _activeMode != ActiveMode.Analyze)
            {
                _activeMode = ActiveMode.Analyze;
                CloseAllMenus();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
            }
            else if (newState == AppState.Studio && _activeMode != ActiveMode.Studio)
            {
                _activeMode = ActiveMode.Studio;
                CloseAllMenus();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
            }
            else if (newState == AppState.Exploration)
            {
                // External exploration state — don't force close, let user control
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Sub-Menu Toggles
        // ═══════════════════════════════════════════════════════

        public void ToggleShaderMenu()
        {
            if (_shaderMenu == null) return;
            _shaderMenuShown = !_shaderMenuShown;

            // Mutual exclusion: close cross-section if opening shaders
            if (_shaderMenuShown && _crossSectionPanelShown)
            {
                _crossSectionPanelShown = false;
                _crossSectionPanel?.AddToClassList("submenu--hidden");
                _analyzeCrossSectionBtn?.RemoveFromClassList("mode-action-btn--active");
            }

            _shaderMenu.EnableInClassList("submenu--hidden", !_shaderMenuShown);
            _analyzeShaderBtn?.EnableInClassList("mode-action-btn--active", _shaderMenuShown);
        }

        public void ToggleCrossSectionPanel()
        {
            if (_crossSectionPanel == null) return;
            _crossSectionPanelShown = !_crossSectionPanelShown;

            // Mutual exclusion: close shaders if opening cross-section
            if (_crossSectionPanelShown && _shaderMenuShown)
            {
                _shaderMenuShown = false;
                _shaderMenu?.AddToClassList("submenu--hidden");
                _analyzeShaderBtn?.RemoveFromClassList("mode-action-btn--active");
            }

            _crossSectionPanel.EnableInClassList("submenu--hidden", !_crossSectionPanelShown);
            _analyzeCrossSectionBtn?.EnableInClassList("mode-action-btn--active", _crossSectionPanelShown);
        }

        public void ToggleCategoryMenu()
        {
            if (_categoryMenu == null) return;
            _categoryMenu.ToggleInClassList("submenu--hidden");
        }

        /// <summary>Shows or hides the category filter menu. Called when entering/leaving ExplodedView.</summary>
        private void ShowCategoryMenu(bool visible)
        {
            if (_categoryMenu == null) return;
            _categoryMenu.EnableInClassList("submenu--hidden", !visible);
        }

        /// <summary>Closes all sub-menus across all modes and resets action button states.</summary>
        public void CloseAllMenus()
        {
            // Analyze sub-menus
            if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
            if (_crossSectionPanel != null) { _crossSectionPanel.AddToClassList("submenu--hidden"); _crossSectionPanelShown = false; }
            _analyzeShaderBtn?.RemoveFromClassList("mode-action-btn--active");
            _analyzeCrossSectionBtn?.RemoveFromClassList("mode-action-btn--active");

            // Tools sub-menus (CategoryMenu starts hidden, slider controlled separately)
            if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
        }

        // ═══════════════════════════════════════════════════════
        //  Sheet Integration
        // ═══════════════════════════════════════════════════════

        /// <summary>Notify when sheet opens/closes — updates Info button active state.</summary>
        public void SetSheetOpenState(bool isOpen)
        {
            _isSheetOpen = isOpen;
            _toolInfoBtn?.EnableInClassList("mode-action-btn--active", isOpen);
        }

        // ═══════════════════════════════════════════════════════
        //  Hotspots
        // ═══════════════════════════════════════════════════════

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();
            _toolHotspotBtn?.EnableInClassList("mode-action-btn--active", _hotspotsEnabled);
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
            _toolsModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Tools);
            _analyzeModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Analyze);
            _studioModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Studio);

            // When leaving a mode, close its sub-menus
            if (_activeMode != ActiveMode.Analyze && _activeMode != ActiveMode.Tools)
                CloseAllMenus();
        }

        private void UpdateModeButtonStates()
        {
            _modeToolsBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Tools);
            _modeAnalyzeBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Analyze);
            _modeStudioBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Studio);
        }

        private void UpdateExplodeButtonState()
        {
            _toolExplodeBtn?.EnableInClassList("mode-action-btn--active", _isExploded);
        }

        private void SyncAppState()
        {
            if (AppStateMachine.Instance == null) return;

            switch (_activeMode)
            {
                case ActiveMode.None:
                case ActiveMode.Tools:
                    // Tools maps to Exploration (or keeps ExplodedView)
                    if (!_isExploded && AppStateMachine.Instance.CurrentState != AppState.Exploration
                        && AppStateMachine.Instance.CurrentState != AppState.FocusMode)
                        AppStateMachine.Instance.EnterExploration();
                    break;
                case ActiveMode.Analyze:
                    if (AppStateMachine.Instance.CurrentState != AppState.Analyze)
                        AppStateMachine.Instance.EnterAnalyze();
                    break;
                case ActiveMode.Studio:
                    if (AppStateMachine.Instance.CurrentState != AppState.Studio)
                        AppStateMachine.Instance.EnterStudio();
                    break;
            }
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
