using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the 3-mode toolbar system with two-level card navigation.
    ///
    /// Flow: Mode button → Card Grid (Level 1) → Sub-Panel (Level 2) → Back to Grid → Deactivate
    ///
    /// Tools Mode:   Card grid (Explode, Filter, Pins) → Explode sub-panel / Filter sub-panel
    /// Analyze Mode: Card grid (Shaders, Cut) → Shader menu / Cross-section panel
    /// Studio Mode:  Environment panel directly (single level)
    ///
    /// Info (i) button is a standalone floating button in the TopBar, wired by UIManager.
    ///
    /// Card click behaviour:
    ///   - "toggle" cards: toggle a feature immediately, highlight card (Pins)
    ///   - "navigate" cards: replace card grid with sub-panel (Explode, Filter, Shaders, Cut)
    /// Mode button with sub-panel open → returns to card grid (back).
    /// Mode button with card grid → deactivates mode.
    /// </summary>
    public class UIModeController
    {
        // ── Mode Containers ──
        private readonly VisualElement _root;
        private readonly VisualElement _toolsModeContainer;
        private readonly VisualElement _analyzeModeContainer;
        private readonly VisualElement _studioModeContainer;

        // ── Tools mode: Level 1 card grid + Level 2 sub-panels ──
        private readonly VisualElement _toolsCardGrid;
        private readonly VisualElement _explodeSubPanel;
        private readonly VisualElement _filterSubPanel;
        private readonly Button _toolExplodeBtn;
        private readonly Button _toolFilterBtn;
        private readonly Button _toolHotspotBtn;
        private readonly VisualElement _sliderContainer;
        private readonly Slider _explosionSlider;
        private readonly VisualElement _categoryMenu;

        // ── Analyze mode: Level 1 card grid + Level 2 sub-panels ──
        private readonly VisualElement _analyzeCardGrid;
        private readonly Button _analyzeShaderBtn;
        private readonly Button _analyzeCrossSectionBtn;
        private readonly VisualElement _shaderMenu;
        private readonly VisualElement _crossSectionPanel;

        // ── Mode Buttons (bottom bar pill) ──
        private readonly Button _modeToolsBtn;
        private readonly Button _modeAnalyzeBtn;
        private readonly Button _modeStudioBtn;

        // ── Internal enums ──
        private enum ActiveMode { None, Tools, Analyze, Studio }
        private enum SubLevel { CardGrid, SubPanel }

        // ── State ──
        private ActiveMode _activeMode = ActiveMode.None;
        private SubLevel _toolsLevel = SubLevel.CardGrid;
        private SubLevel _analyzeLevel = SubLevel.CardGrid;
        private string _toolsActivePanel = null;   // "explode" | "filter" | null
        private string _analyzeActivePanel = null; // "shaders" | "cross-section" | null
        private bool _hotspotsEnabled = false;
        private bool _isSheetOpen = false;
        private bool _isExploded = false;
        private List<string> _activeCategories = new List<string>() { "ALL" };

        // ── Callbacks (wired by UIManager) ──
        public event System.Action OnInfoToggleRequested;
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

            // ── Tools mode queries ──
            if (_toolsModeContainer != null)
            {
                _toolsCardGrid = _toolsModeContainer.Q<VisualElement>("ToolsCardGrid");
                _explodeSubPanel = _toolsModeContainer.Q<VisualElement>("ExplodeSubPanel");
                _filterSubPanel = _toolsModeContainer.Q<VisualElement>("FilterSubPanel");
                _toolExplodeBtn = _toolsModeContainer.Q<Button>("ToolExplodeBtn");
                _toolFilterBtn = _toolsModeContainer.Q<Button>("ToolFilterBtn");
                _toolHotspotBtn = _toolsModeContainer.Q<Button>("ToolHotspotBtn");
                _sliderContainer = _toolsModeContainer.Q<VisualElement>("SliderContainer");
                _categoryMenu = _filterSubPanel; // category buttons live inside FilterSubPanel now
            }

            // ── Analyze mode queries ──
            if (_analyzeModeContainer != null)
            {
                _analyzeCardGrid = _analyzeModeContainer.Q<VisualElement>("AnalyzeCardGrid");
                _analyzeShaderBtn = _analyzeModeContainer.Q<Button>("AnalyzeShaderBtn");
                _analyzeCrossSectionBtn = _analyzeModeContainer.Q<Button>("AnalyzeCrossSectionBtn");
                _shaderMenu = _analyzeModeContainer.Q<VisualElement>("ShaderMenu");
                _crossSectionPanel = _analyzeModeContainer.Q<VisualElement>("CrossSectionPanel");
            }

            // ── Mode buttons ──
            _modeToolsBtn = root?.Q<Button>("ModeToolsBtn");
            _modeAnalyzeBtn = root?.Q<Button>("ModeAnalyzeBtn");
            _modeStudioBtn = root?.Q<Button>("ModeStudioBtn");

            BindModeButtons();
            BindToolsCards();
            BindAnalyzeCards();
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
        //  Mode Button Binding (Two-Level Navigation)
        // ═══════════════════════════════════════════════════════

        private void BindModeButtons()
        {
            if (_modeToolsBtn != null)
            {
                System.Action onTools = () => HandleModeBtnClick(ActiveMode.Tools);
                _modeToolsBtn.clicked += onTools;
                AddCleanup(() => _modeToolsBtn.clicked -= onTools);
            }

            if (_modeAnalyzeBtn != null)
            {
                System.Action onAnalyze = () => HandleModeBtnClick(ActiveMode.Analyze);
                _modeAnalyzeBtn.clicked += onAnalyze;
                AddCleanup(() => _modeAnalyzeBtn.clicked -= onAnalyze);
            }

            if (_modeStudioBtn != null)
            {
                System.Action onStudio = () => HandleModeBtnClick(ActiveMode.Studio);
                _modeStudioBtn.clicked += onStudio;
                AddCleanup(() => _modeStudioBtn.clicked -= onStudio);
            }
        }

        /// <summary>
        /// Three-state toggle for mode buttons:
        /// 1. Mode inactive → activate & show card grid
        /// 2. Active + sub-panel open → return to card grid (back)
        /// 3. Active + card grid shown → deactivate
        /// </summary>
        private void HandleModeBtnClick(ActiveMode mode)
        {
            if (_activeMode != mode)
            {
                // Different mode or None → activate
                ActivateMode(mode);
                return;
            }

            // Same mode — check navigation level
            bool isSubPanelOpen = false;
            if (mode == ActiveMode.Tools)
                isSubPanelOpen = _toolsLevel == SubLevel.SubPanel;
            else if (mode == ActiveMode.Analyze)
                isSubPanelOpen = _analyzeLevel == SubLevel.SubPanel;

            if (isSubPanelOpen)
            {
                // Back to card grid
                NavigateToCardGrid(mode);
            }
            else
            {
                // Deactivate
                DeactivateAllModes();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Tools Mode — Card Click Binding
        // ═══════════════════════════════════════════════════════

        private void BindToolsCards()
        {
            // Info button is now a floating (i) button wired directly in UIManager

            // Explode card — navigate to ExplodeSubPanel (slider + categories always visible inside)
            if (_toolExplodeBtn != null)
            {
                System.Action onExplode = () => NavigateToSubPanel(ActiveMode.Tools, "explode");
                _toolExplodeBtn.clicked += onExplode;
                AddCleanup(() => _toolExplodeBtn.clicked -= onExplode);
            }

            // Filter card — navigate to FilterSubPanel (category grid)
            if (_toolFilterBtn != null)
            {
                System.Action onFilter = () => NavigateToSubPanel(ActiveMode.Tools, "filter");
                _toolFilterBtn.clicked += onFilter;
                AddCleanup(() => _toolFilterBtn.clicked -= onFilter);
            }

            // Pins card — immediate toggle (no navigation)
            if (_toolHotspotBtn != null)
            {
                System.Action onHotspot = () => ToggleHotspots();
                _toolHotspotBtn.clicked += onHotspot;
                AddCleanup(() => _toolHotspotBtn.clicked -= onHotspot);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Mode — Card Click Binding
        // ═══════════════════════════════════════════════════════

        private void BindAnalyzeCards()
        {
            if (_analyzeShaderBtn != null)
            {
                System.Action onShader = () => NavigateToSubPanel(ActiveMode.Analyze, "shaders");
                _analyzeShaderBtn.clicked += onShader;
                AddCleanup(() => _analyzeShaderBtn.clicked -= onShader);
            }

            if (_analyzeCrossSectionBtn != null)
            {
                System.Action onCross = () => NavigateToSubPanel(ActiveMode.Analyze, "cross-section");
                _analyzeCrossSectionBtn.clicked += onCross;
                AddCleanup(() => _analyzeCrossSectionBtn.clicked -= onCross);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Navigation — Activate / Navigate / Deactivate
        // ═══════════════════════════════════════════════════════

        private void ActivateMode(ActiveMode mode)
        {
            _activeMode = mode;
            CloseAllSubPanels();
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();

            // Start at card grid level
            if (mode == ActiveMode.Tools)
            {
                _toolsLevel = SubLevel.CardGrid;
                _toolsActivePanel = null;
                ShowToolsLevel();
            }
            else if (mode == ActiveMode.Analyze)
            {
                _analyzeLevel = SubLevel.CardGrid;
                _analyzeActivePanel = null;
                ShowAnalyzeLevel();
            }
        }

        public void DeactivateAllModes()
        {
            _activeMode = ActiveMode.None;
            CloseAllSubPanels();
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
        }

        /// <summary>Navigate from card grid to a sub-panel.</summary>
        private void NavigateToSubPanel(ActiveMode mode, string panelId)
        {
            if (mode == ActiveMode.Tools)
            {
                _toolsLevel = SubLevel.SubPanel;
                _toolsActivePanel = panelId;
                ShowToolsLevel();

                // Highlight the active card
                _toolExplodeBtn?.EnableInClassList("submenu-card--active", panelId == "explode");
                _toolFilterBtn?.EnableInClassList("submenu-card--active", panelId == "filter");
            }
            else if (mode == ActiveMode.Analyze)
            {
                _analyzeLevel = SubLevel.SubPanel;
                _analyzeActivePanel = panelId;
                ShowAnalyzeLevel();

                // Highlight the active card
                _analyzeShaderBtn?.EnableInClassList("submenu-card--active", panelId == "shaders");
                _analyzeCrossSectionBtn?.EnableInClassList("submenu-card--active", panelId == "cross-section");

                // Enable cross-section manager when navigating to the panel
                if (panelId == "cross-section")
                    CrossSectionManager.Instance?.EnableCrossSection();
            }
        }

        /// <summary>Navigate from sub-panel back to card grid.</summary>
        private void NavigateToCardGrid(ActiveMode mode)
        {
            if (mode == ActiveMode.Tools)
            {
                _toolsLevel = SubLevel.CardGrid;
                _toolsActivePanel = null;
                ShowToolsLevel();
                _toolExplodeBtn?.RemoveFromClassList("submenu-card--active");
                _toolFilterBtn?.RemoveFromClassList("submenu-card--active");
            }
            else if (mode == ActiveMode.Analyze)
            {
                // Disable cross-section when leaving the panel
                if (_analyzeActivePanel == "cross-section")
                    CrossSectionManager.Instance?.DisableCrossSection();

                _analyzeLevel = SubLevel.CardGrid;
                _analyzeActivePanel = null;
                ShowAnalyzeLevel();
                _analyzeShaderBtn?.RemoveFromClassList("submenu-card--active");
                _analyzeCrossSectionBtn?.RemoveFromClassList("submenu-card--active");
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Tools Level Visibility
        // ═══════════════════════════════════════════════════════

        private void ShowToolsLevel()
        {
            bool showGrid = _toolsLevel == SubLevel.CardGrid;
            _toolsCardGrid?.EnableInClassList("submenu--hidden", !showGrid);

            bool showExplode = !showGrid && _toolsActivePanel == "explode";
            bool showFilter = !showGrid && _toolsActivePanel == "filter";

            _explodeSubPanel?.EnableInClassList("submenu--hidden", !showExplode);
            _filterSubPanel?.EnableInClassList("submenu--hidden", !showFilter);
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Level Visibility
        // ═══════════════════════════════════════════════════════

        private void ShowAnalyzeLevel()
        {
            bool showGrid = _analyzeLevel == SubLevel.CardGrid;
            _analyzeCardGrid?.EnableInClassList("submenu--hidden", !showGrid);

            bool showShaders = !showGrid && _analyzeActivePanel == "shaders";
            bool showCross = !showGrid && _analyzeActivePanel == "cross-section";

            _shaderMenu?.EnableInClassList("submenu--hidden", !showShaders);
            _crossSectionPanel?.EnableInClassList("submenu--hidden", !showCross);
        }

        // ═══════════════════════════════════════════════════════
        //  Legacy API compatibility (ToggleShaderMenu, etc.)
        // ═══════════════════════════════════════════════════════

        public void ToggleShaderMenu()
        {
            if (_analyzeLevel == SubLevel.SubPanel && _analyzeActivePanel == "shaders")
                NavigateToCardGrid(ActiveMode.Analyze);
            else
                NavigateToSubPanel(ActiveMode.Analyze, "shaders");
        }

        public void ToggleCrossSectionPanel()
        {
            if (_analyzeLevel == SubLevel.SubPanel && _analyzeActivePanel == "cross-section")
                NavigateToCardGrid(ActiveMode.Analyze);
            else
                NavigateToSubPanel(ActiveMode.Analyze, "cross-section");
        }

        public void ToggleCategoryMenu()
        {
            // Navigate to/from FilterSubPanel
            if (_toolsLevel == SubLevel.SubPanel && _toolsActivePanel == "filter")
                NavigateToCardGrid(ActiveMode.Tools);
            else
                NavigateToSubPanel(ActiveMode.Tools, "filter");
        }

        // ═══════════════════════════════════════════════════════
        //  Sheet / Hotspot / Slider Integration
        // ═══════════════════════════════════════════════════════

        public void SetSheetOpenState(bool isOpen)
        {
            _isSheetOpen = isOpen;
            // Floating info button highlight managed by UIManager directly
        }

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();
            _toolHotspotBtn?.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

        public void SetSliderVisible(bool visible)
        {
            // Slider is now always visible inside ExplodeSubPanel — no-op
        }

        public void ToggleSliderVisibility()
        {
            // Navigate to/from ExplodeSubPanel
            if (_toolsLevel == SubLevel.SubPanel && _toolsActivePanel == "explode")
                NavigateToCardGrid(ActiveMode.Tools);
            else
                NavigateToSubPanel(ActiveMode.Tools, "explode");
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
        //  External State Sync
        // ═══════════════════════════════════════════════════════

        public void SyncWithAppState(AppState newState)
        {
            if (newState == AppState.ExplodedView)
            {
                _isExploded = true;
                UpdateExplodeButtonState();
                return;
            }

            if (_isExploded && newState != AppState.ExplodedView)
            {
                _isExploded = false;
                UpdateExplodeButtonState();
            }

            if (newState == AppState.FocusMode) return;

            if (newState == AppState.Analyze && _activeMode != ActiveMode.Analyze)
            {
                _activeMode = ActiveMode.Analyze;
                _analyzeLevel = SubLevel.CardGrid;
                _analyzeActivePanel = null;
                CloseAllSubPanels();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
                ShowAnalyzeLevel();
            }
            else if (newState == AppState.Studio && _activeMode != ActiveMode.Studio)
            {
                _activeMode = ActiveMode.Studio;
                CloseAllSubPanels();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  CloseAllMenus — backwards compat for UIManager calls
        // ═══════════════════════════════════════════════════════

        public void CloseAllMenus()
        {
            CloseAllSubPanels();
        }

        // ═══════════════════════════════════════════════════════
        //  Private — Visibility Management
        // ═══════════════════════════════════════════════════════

        private void CloseAllSubPanels()
        {
            // Disable cross-section if it was active
            if (_analyzeActivePanel == "cross-section")
                CrossSectionManager.Instance?.DisableCrossSection();

            // Tools
            _toolsLevel = SubLevel.CardGrid;
            _toolsActivePanel = null;
            _toolsCardGrid?.RemoveFromClassList("submenu--hidden");
            _explodeSubPanel?.AddToClassList("submenu--hidden");
            _filterSubPanel?.AddToClassList("submenu--hidden");
            _toolExplodeBtn?.RemoveFromClassList("submenu-card--active");
            _toolFilterBtn?.RemoveFromClassList("submenu-card--active");

            // Analyze
            _analyzeLevel = SubLevel.CardGrid;
            _analyzeActivePanel = null;
            _analyzeCardGrid?.RemoveFromClassList("submenu--hidden");
            _shaderMenu?.AddToClassList("submenu--hidden");
            _crossSectionPanel?.AddToClassList("submenu--hidden");
            _analyzeShaderBtn?.RemoveFromClassList("submenu-card--active");
            _analyzeCrossSectionBtn?.RemoveFromClassList("submenu-card--active");
        }

        private void UpdateContainerVisibility()
        {
            _toolsModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Tools);
            _analyzeModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Analyze);
            _studioModeContainer?.EnableInClassList("mode--hidden", _activeMode != ActiveMode.Studio);
        }

        private void UpdateModeButtonStates()
        {
            _modeToolsBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Tools);
            _modeAnalyzeBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Analyze);
            _modeStudioBtn?.EnableInClassList("mode-btn--active", _activeMode == ActiveMode.Studio);
        }

        private void UpdateExplodeButtonState()
        {
            _toolExplodeBtn?.EnableInClassList("submenu-card--active", _isExploded);
        }

        private void SyncAppState()
        {
            if (AppStateMachine.Instance == null) return;

            switch (_activeMode)
            {
                case ActiveMode.None:
                case ActiveMode.Tools:
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
