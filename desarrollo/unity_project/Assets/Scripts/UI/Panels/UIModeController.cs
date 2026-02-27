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
    /// Inspect Mode:  Card grid (Info, Pins, Isolate) — all toggles, no sub-panels
    /// Analyze Mode:  Card grid (Cut, Explode, Filter) → Cross-section / Explode slider / Filter panel
    /// Studio Mode:   Shaders + Environment panel directly (single level, combined)
    ///
    /// Card click behaviour:
    ///   - "toggle" cards: toggle a feature immediately, highlight card (Info, Pins, Isolate)
    ///   - "navigate" cards: replace card grid with sub-panel (Cut, Explode, Filter)
    /// Mode button with sub-panel open → returns to card grid (back).
    /// Mode button with card grid → deactivates mode.
    /// </summary>
    public class UIModeController
    {
        // ── Mode Containers ──
        private readonly VisualElement _root;
        private readonly VisualElement _toolsModeContainer;   // Inspect
        private readonly VisualElement _analyzeModeContainer;
        private readonly VisualElement _studioModeContainer;

        // ── Inspect mode: Level 1 card grid (toggles only) ──
        private readonly VisualElement _toolsCardGrid;
        private readonly Button _toolInfoBtn;
        private readonly Button _toolHotspotBtn;
        private readonly Button _toolIsolateBtn;

        // ── Analyze mode: Level 1 card grid + Level 2 sub-panels ──
        private readonly VisualElement _analyzeCardGrid;
        private readonly Button _analyzeCrossSectionBtn;
        private readonly Button _analyzeExplodeBtn;
        private readonly Button _analyzeFilterBtn;
        private readonly VisualElement _crossSectionPanel;
        private readonly VisualElement _explodeSubPanel;
        private readonly VisualElement _filterSubPanel;
        private readonly VisualElement _sliderContainer;
        private readonly Slider _explosionSlider;

        // ── Mode Buttons (bottom bar pill) ──
        private readonly Button _modeToolsBtn;
        private readonly Button _modeAnalyzeBtn;
        private readonly Button _modeStudioBtn;

        // ── Internal enums ──
        private enum ActiveMode { None, Tools, Analyze, Studio }
        private enum SubLevel { CardGrid, SubPanel }

        // ── State ──
        private ActiveMode _activeMode = ActiveMode.None;
        private SubLevel _analyzeLevel = SubLevel.CardGrid;
        private string _analyzeActivePanel = null; // "cross-section" | "explode" | "filter" | null
        private bool _hotspotsEnabled = true;
        private bool _isSheetOpen = false;
        private bool _isExploded = false;
        private bool _isIsolated = false;
        private List<string> _activeCategories = new List<string>() { "ALL" };

        // ── Callbacks (wired by UIManager) ──
        public event System.Action OnInfoToggleRequested;
        public event System.Action OnIsolateToggleRequested;
        public event System.Action OnExplodeToggleRequested;
        public event System.Action OnAnyModeActivated; // Notifies others to close

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

            // ── Inspect mode queries ──
            if (_toolsModeContainer != null)
            {
                _toolsCardGrid = _toolsModeContainer.Q<VisualElement>("ToolsCardGrid");
                _toolInfoBtn = _toolsModeContainer.Q<Button>("ToolInfoBtn");
                _toolHotspotBtn = _toolsModeContainer.Q<Button>("ToolHotspotBtn");
                _toolIsolateBtn = _toolsModeContainer.Q<Button>("ToolIsolateBtn");
            }

            // ── Analyze mode queries ──
            if (_analyzeModeContainer != null)
            {
                _analyzeCardGrid = _analyzeModeContainer.Q<VisualElement>("AnalyzeCardGrid");
                _analyzeCrossSectionBtn = _analyzeModeContainer.Q<Button>("AnalyzeCrossSectionBtn");
                _analyzeExplodeBtn = _analyzeModeContainer.Q<Button>("AnalyzeExplodeBtn");
                _analyzeFilterBtn = _analyzeModeContainer.Q<Button>("AnalyzeFilterBtn");
                _crossSectionPanel = _analyzeModeContainer.Q<VisualElement>("CrossSectionPanel");
                _explodeSubPanel = _analyzeModeContainer.Q<VisualElement>("ExplodeSubPanel");
                _filterSubPanel = _analyzeModeContainer.Q<VisualElement>("FilterSubPanel");
                _sliderContainer = _analyzeModeContainer.Q<VisualElement>("SliderContainer");
            }

            // ── Mode buttons ──
            _modeToolsBtn = root?.Q<Button>("ModeToolsBtn");
            _modeAnalyzeBtn = root?.Q<Button>("ModeAnalyzeBtn");
            _modeStudioBtn = root?.Q<Button>("ModeStudioBtn");

            BindModeButtons();
            BindInspectCards();
            BindAnalyzeCards();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            OnInfoToggleRequested = null;
            OnIsolateToggleRequested = null;
            OnExplodeToggleRequested = null;
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Mode Button Binding (Two-Level Navigation)
        // ═══════════════════════════════════════════════════════

        private void DelayAction(System.Action action)
        {
            if (_root == null) action();
            else _root.schedule.Execute(action).StartingIn(150);
        }

        private void BindModeButtons()
        {
            if (_modeToolsBtn != null)
            {
                System.Action onTools = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Tools));
                _modeToolsBtn.clicked += onTools;
                AddCleanup(() => _modeToolsBtn.clicked -= onTools);
            }

            if (_modeAnalyzeBtn != null)
            {
                System.Action onAnalyze = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Analyze));
                _modeAnalyzeBtn.clicked += onAnalyze;
                AddCleanup(() => _modeAnalyzeBtn.clicked -= onAnalyze);
            }

            if (_modeStudioBtn != null)
            {
                System.Action onStudio = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Studio));
                _modeStudioBtn.clicked += onStudio;
                AddCleanup(() => _modeStudioBtn.clicked -= onStudio);
            }
        }

        /// <summary>
        /// Three-state toggle for mode buttons:
        /// 1. Mode inactive → activate & show card grid
        /// 2. Active + sub-panel open → return to card grid (back) [Analyze only]
        /// 3. Active + card grid shown → deactivate
        /// </summary>
        private void HandleModeBtnClick(ActiveMode mode)
        {
            if (_activeMode != mode)
            {
                ActivateMode(mode);
                return;
            }

            // Same mode — check navigation level (only Analyze has sub-panels)
            if (mode == ActiveMode.Analyze && _analyzeLevel == SubLevel.SubPanel)
            {
                NavigateToCardGrid(mode);
            }
            else
            {
                DeactivateAllModes();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Inspect Mode — Card Click Binding (all toggles)
        // ═══════════════════════════════════════════════════════

        private void BindInspectCards()
        {
            // Info card — toggle info sheet
            if (_toolInfoBtn != null)
            {
                System.Action onInfo = () =>
                {
                    DelayAction(() => OnInfoToggleRequested?.Invoke());
                };
                _toolInfoBtn.clicked += onInfo;
                AddCleanup(() => _toolInfoBtn.clicked -= onInfo);
            }

            // Pins card — immediate toggle
            if (_toolHotspotBtn != null)
            {
                _toolHotspotBtn.EnableInClassList("submenu-card--active", _hotspotsEnabled);
                System.Action onHotspot = () => DelayAction(() => ToggleHotspots());
                _toolHotspotBtn.clicked += onHotspot;
                AddCleanup(() => _toolHotspotBtn.clicked -= onHotspot);
            }

            // Isolate card — toggle isolation of selected part
            if (_toolIsolateBtn != null)
            {
                System.Action onIsolate = () =>
                {
                    DelayAction(() => OnIsolateToggleRequested?.Invoke());
                };
                _toolIsolateBtn.clicked += onIsolate;
                AddCleanup(() => _toolIsolateBtn.clicked -= onIsolate);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Mode — Card Click Binding (navigate)
        // ═══════════════════════════════════════════════════════

        private void BindAnalyzeCards()
        {
            if (_analyzeCrossSectionBtn != null)
            {
                System.Action onCross = () => DelayAction(() => NavigateToSubPanel(ActiveMode.Analyze, "cross-section"));
                _analyzeCrossSectionBtn.clicked += onCross;
                AddCleanup(() => _analyzeCrossSectionBtn.clicked -= onCross);
            }

            if (_analyzeExplodeBtn != null)
            {
                System.Action onExplode = () => DelayAction(() => NavigateToSubPanel(ActiveMode.Analyze, "explode"));
                _analyzeExplodeBtn.clicked += onExplode;
                AddCleanup(() => _analyzeExplodeBtn.clicked -= onExplode);
            }

            if (_analyzeFilterBtn != null)
            {
                System.Action onFilter = () => DelayAction(() => NavigateToSubPanel(ActiveMode.Analyze, "filter"));
                _analyzeFilterBtn.clicked += onFilter;
                AddCleanup(() => _analyzeFilterBtn.clicked -= onFilter);
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
            OnAnyModeActivated?.Invoke();

            if (mode == ActiveMode.Analyze)
            {
                _analyzeLevel = SubLevel.CardGrid;
                _analyzeActivePanel = null;
                ShowAnalyzeLevel();
            }
            // Inspect and Studio have no sub-panel navigation
        }

        public void DeactivateAllModes()
        {
            _activeMode = ActiveMode.None;
            CloseAllSubPanels();
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
        }

        /// <summary>Navigate from card grid to a sub-panel (Analyze mode only).</summary>
        private void NavigateToSubPanel(ActiveMode mode, string panelId)
        {
            if (mode == ActiveMode.Analyze)
            {
                _analyzeLevel = SubLevel.SubPanel;
                _analyzeActivePanel = panelId;
                ShowAnalyzeLevel();

                // Highlight the active card
                _analyzeCrossSectionBtn?.EnableInClassList("submenu-card--active", panelId == "cross-section");
                _analyzeExplodeBtn?.EnableInClassList("submenu-card--active", panelId == "explode");
                _analyzeFilterBtn?.EnableInClassList("submenu-card--active", panelId == "filter");

                // Enable cross-section manager when navigating to the panel
                if (panelId == "cross-section")
                    CrossSectionManager.Instance?.EnableCrossSection();
            }
        }

        /// <summary>Navigate from sub-panel back to card grid (Analyze mode).</summary>
        private void NavigateToCardGrid(ActiveMode mode)
        {
            if (mode == ActiveMode.Analyze)
            {
                // Disable cross-section when leaving the panel
                if (_analyzeActivePanel == "cross-section")
                    CrossSectionManager.Instance?.DisableCrossSection();

                _analyzeLevel = SubLevel.CardGrid;
                _analyzeActivePanel = null;
                ShowAnalyzeLevel();
                _analyzeCrossSectionBtn?.RemoveFromClassList("submenu-card--active");
                _analyzeExplodeBtn?.RemoveFromClassList("submenu-card--active");
                _analyzeFilterBtn?.RemoveFromClassList("submenu-card--active");
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Analyze Level Visibility
        // ═══════════════════════════════════════════════════════

        private void ShowAnalyzeLevel()
        {
            bool showGrid = _analyzeLevel == SubLevel.CardGrid;
            _analyzeCardGrid?.EnableInClassList("submenu--hidden", !showGrid);

            bool showCross = !showGrid && _analyzeActivePanel == "cross-section";
            bool showExplode = !showGrid && _analyzeActivePanel == "explode";
            bool showFilter = !showGrid && _analyzeActivePanel == "filter";

            _crossSectionPanel?.EnableInClassList("submenu--hidden", !showCross);
            _explodeSubPanel?.EnableInClassList("submenu--hidden", !showExplode);
            _filterSubPanel?.EnableInClassList("submenu--hidden", !showFilter);
        }

        // ═══════════════════════════════════════════════════════
        //  Legacy API compatibility
        // ═══════════════════════════════════════════════════════

        public void ToggleShaderMenu()
        {
            // Shaders are now always visible in Studio mode — just activate Studio
            if (_activeMode != ActiveMode.Studio)
                ActivateMode(ActiveMode.Studio);
            else
                DeactivateAllModes();
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
            if (_analyzeLevel == SubLevel.SubPanel && _analyzeActivePanel == "filter")
                NavigateToCardGrid(ActiveMode.Analyze);
            else
                NavigateToSubPanel(ActiveMode.Analyze, "filter");
        }

        // ═══════════════════════════════════════════════════════
        //  Sheet / Hotspot / Isolate / Slider Integration
        // ═══════════════════════════════════════════════════════

        public void SetSheetOpenState(bool isOpen)
        {
            _isSheetOpen = isOpen;
            // Update Info card highlight in Inspect mode
            _toolInfoBtn?.EnableInClassList("submenu-card--active", isOpen);

            if (isOpen && _activeMode != ActiveMode.Tools)
            {
                DeactivateAllModes();
            }
        }

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();
            _toolHotspotBtn?.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

        public void SetIsolateState(bool isolated)
        {
            _isIsolated = isolated;
            _toolIsolateBtn?.EnableInClassList("submenu-card--active", isolated);
        }

        public void SetSliderVisible(bool visible)
        {
            // Slider is now always visible inside ExplodeSubPanel — no-op
        }

        public void ToggleSliderVisibility()
        {
            if (_analyzeLevel == SubLevel.SubPanel && _analyzeActivePanel == "explode")
                NavigateToCardGrid(ActiveMode.Analyze);
            else
                NavigateToSubPanel(ActiveMode.Analyze, "explode");
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
                _analyzeExplodeBtn?.EnableInClassList("submenu-card--active", true);
                return;
            }

            if (_isExploded && newState != AppState.ExplodedView)
            {
                _isExploded = false;
                _analyzeExplodeBtn?.EnableInClassList("submenu-card--active", false);
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

            // Analyze
            _analyzeLevel = SubLevel.CardGrid;
            _analyzeActivePanel = null;
            _analyzeCardGrid?.RemoveFromClassList("submenu--hidden");
            _crossSectionPanel?.AddToClassList("submenu--hidden");
            _explodeSubPanel?.AddToClassList("submenu--hidden");
            _filterSubPanel?.AddToClassList("submenu--hidden");
            _analyzeCrossSectionBtn?.RemoveFromClassList("submenu-card--active");
            _analyzeExplodeBtn?.RemoveFromClassList("submenu-card--active");
            _analyzeFilterBtn?.RemoveFromClassList("submenu-card--active");
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
            if (_filterSubPanel == null) return;

            void UpdateBtn(string btnName, string catName)
            {
                var btn = _filterSubPanel.Q<Button>(btnName);
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
