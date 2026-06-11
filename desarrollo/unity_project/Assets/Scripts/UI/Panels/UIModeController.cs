using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using System;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Orchestrator for the 3-mode toolbar system with two-level card navigation.
    /// Delegates mode-specific logic to <see cref="InspectModeHandler"/>,
    /// <see cref="AnalyzeModeHandler"/>, and <see cref="StudioModeHandler"/>.
    ///
    /// Flow: Mode button → Handler.Activate() → Card Grid / Sub-Panel → Handler.Deactivate()
    /// </summary>
    public class UIModeController
    {
        // ── Handlers ──
        private readonly InspectModeHandler _inspect;
        private readonly AnalyzeModeHandler _analyze;
        private readonly StudioModeHandler _studio;

        // ── Root + Mode Containers ──
        private readonly VisualElement _root;
        private readonly VisualElement _toolsModeContainer;
        private readonly VisualElement _analyzeModeContainer;
        private readonly VisualElement _studioModeContainer;

        // ── Mode Buttons (bottom bar pill) ──
        private readonly Button _modeToolsBtn;
        private readonly Button _modeAnalyzeBtn;
        private readonly Button _modeStudioBtn;

        // ── Internal enum ──
        private enum ActiveMode { None, Tools, Analyze, Studio }
        private ActiveMode _activeMode = ActiveMode.None;

        // ── Events (wired by UIManager) ──
        public event Action OnIsolateToggleRequested
        {
            add => _inspect.OnIsolateToggleRequested += value;
            remove => _inspect.OnIsolateToggleRequested -= value;
        }
        public event Action OnExplodeToggleRequested
        {
            add => _analyze.OnExplodeToggleRequested += value;
            remove => _analyze.OnExplodeToggleRequested -= value;
        }
        public event Action OnAnyModeActivated;

        /// <summary>True when any mode (Inspect/Analyze/Studio) is active.</summary>
        public bool HasActiveMode => _activeMode != ActiveMode.None;

        /// <summary>
        /// Handles Escape key: closes sub-panel first, then deactivates mode.
        /// Returns true if something was closed.
        /// </summary>
        public bool HandleEscapeKey()
        {
            if (_activeMode == ActiveMode.Analyze && _analyze.IsSubPanelOpen)
            {
                _analyze.NavigateToCardGrid();
                return true;
            }

            if (_activeMode != ActiveMode.None)
            {
                DeactivateAllModes();
                return true;
            }

            return false;
        }

        public bool HandleBackNavigation() => HandleEscapeKey();

        // ── Cleanup ──
        private readonly System.Collections.Generic.List<Action> _cleanupActions = new();

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

            // ── Create handlers ──
            _inspect = new InspectModeHandler(root, toolsModeContainer);
            _analyze = new AnalyzeModeHandler(root, analyzeModeContainer);
            _studio = new StudioModeHandler(root, studioModeContainer);

            // ── Mode buttons ──
            _modeToolsBtn = root?.Q<Button>("ModeToolsBtn");
            _modeAnalyzeBtn = root?.Q<Button>("ModeAnalyzeBtn");
            _modeStudioBtn = root?.Q<Button>("ModeStudioBtn");

            BindModeButtons();
        }

        public void Dispose()
        {
            OnAnyModeActivated = null;
            _inspect.Dispose();
            _analyze.Dispose();
            _studio.Dispose();
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Mode Button Binding
        // ═══════════════════════════════════════════════════════

        private void DelayAction(Action action)
        {
            if (_root == null) action();
            else _root.schedule.Execute(action).StartingIn(250);
        }

        private void BindModeButtons()
        {
            if (_modeToolsBtn != null)
            {
                Action onTools = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Tools));
                _modeToolsBtn.clicked += onTools;
                _cleanupActions.Add(() => _modeToolsBtn.clicked -= onTools);
            }

            if (_modeAnalyzeBtn != null)
            {
                Action onAnalyze = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Analyze));
                _modeAnalyzeBtn.clicked += onAnalyze;
                _cleanupActions.Add(() => _modeAnalyzeBtn.clicked -= onAnalyze);
            }

            if (_modeStudioBtn != null)
            {
                Action onStudio = () => DelayAction(() => HandleModeBtnClick(ActiveMode.Studio));
                _modeStudioBtn.clicked += onStudio;
                _cleanupActions.Add(() => _modeStudioBtn.clicked -= onStudio);
            }
        }

        /// <summary>
        /// Three-state toggle:
        /// 1. Mode inactive → activate and show card grid
        /// 2. Active + sub-panel open → back to card grid (Analyze only)
        /// 3. Active + card grid → deactivate
        /// </summary>
        private void HandleModeBtnClick(ActiveMode mode)
        {
            if (_activeMode != mode)
            {
                ActivateMode(mode);
                return;
            }

            if (mode == ActiveMode.Analyze && _analyze.IsSubPanelOpen)
                _analyze.NavigateToCardGrid();
            else
                DeactivateAllModes();
        }

        // ═══════════════════════════════════════════════════════
        //  Navigation — Activate / Deactivate
        // ═══════════════════════════════════════════════════════

        private void ActivateMode(ActiveMode mode)
        {
            // Deactivate previous handler
            GetHandler(_activeMode)?.Deactivate();

            _activeMode = mode;
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
            OnAnyModeActivated?.Invoke();

            // Activate new handler
            GetHandler(mode)?.Activate();
        }

        public void DeactivateAllModes()
        {
            GetHandler(_activeMode)?.Deactivate();
            _activeMode = ActiveMode.None;
            UpdateContainerVisibility();
            UpdateModeButtonStates();
            SyncAppState();
        }

        private BaseModeHandler GetHandler(ActiveMode mode)
        {
            return mode switch
            {
                ActiveMode.Tools => _inspect,
                ActiveMode.Analyze => _analyze,
                ActiveMode.Studio => _studio,
                _ => null
            };
        }

        // ═══════════════════════════════════════════════════════
        //  Legacy API — delegate to handlers
        // ═══════════════════════════════════════════════════════

        public void ToggleShaderMenu()
        {
            if (_activeMode != ActiveMode.Studio)
                ActivateMode(ActiveMode.Studio);
            else
                DeactivateAllModes();
        }

        public void ToggleCrossSectionPanel() => _analyze.ToggleCrossSectionPanel();
        public void ToggleCategoryMenu() => _analyze.ToggleCategoryMenu();
        public void ToggleSliderVisibility() => _analyze.ToggleExplodePanel();

        public void SetSheetOpenState(bool isOpen)
        {
            if (isOpen) DeactivateAllModes();
        }

        public void ToggleHotspots() => _inspect.ToggleHotspots();
        public void SetIsolateState(bool isolated) => _inspect.SetIsolateState(isolated);
        public void SetSliderVisible(bool visible) { /* No-op: slider always in ExplodeSubPanel */ }
        public void SetCategoryFilter(string category, Button btn, bool exclusiveMode = false) => _analyze.SetCategoryFilter(category, btn, exclusiveMode);
        public void CloseAllMenus() => GetHandler(_activeMode)?.Deactivate();

        // ═══════════════════════════════════════════════════════
        //  External State Sync
        // ═══════════════════════════════════════════════════════

        public void SyncWithAppState(AppState newState)
        {
            if (newState == AppState.ExplodedView)
            {
                _analyze.SetExplodeState(true);
                return;
            }

            if (newState == AppState.FocusMode) return;

            if (newState == AppState.Analyze && _activeMode != ActiveMode.Analyze)
            {
                _activeMode = ActiveMode.Analyze;
                _analyze.Deactivate();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
                _analyze.Activate();
            }
            else if (newState == AppState.Studio && _activeMode != ActiveMode.Studio)
            {
                _activeMode = ActiveMode.Studio;
                _analyze.Deactivate();
                UpdateContainerVisibility();
                UpdateModeButtonStates();
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
                    if (!_analyze.IsExploded
                        && AppStateMachine.Instance.CurrentState != AppState.Exploration
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
    }
}
