using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
using WebGL.UI.Panels;

namespace WebGL.UI
{
    /// <summary>
    /// Slim coordinator for all UI sub-systems (Phase 3 Step 2 + Phase 2 UX Redesign).
    /// Delegates domain logic to:
    ///   - UIDetailsSheet    → bottom sheet, part data display
    ///   - UIModeController  → 3-mode system (Explore/Analyze/Studio), sub-menus, category filters
    ///   - UIHeroController  → hero/landing screen
    ///   - UIAnalyzePanel    → shader mode menu cards
    ///   - UIEnvironmentPanel→ environment preset cards + sliders
    /// UIManager retains only: initialization, event routing, button wiring, and
    /// cross-cutting concerns (hotspots, app-state reactions, button input blockers).
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainDocument;

        // ── Root ──
        private VisualElement root;

        // ── Sub-Controllers (Phase 3 Step 2 + Phase 2 Redesign) ──
        private UIDetailsSheet _detailsSheet;
        private UIModeController _modeController;
        private UIHeroController _heroController;
        private UIAnalyzePanel _uiAnalyzePanel;
        private UIEnvironmentPanel _uiEnvironmentPanel;
        private UICrossSectionPanel _uiCrossSectionPanel;
        private OnboardingController _onboardingController;

        // ── Buttons (wired here, logic delegated) ──
        private Button resetBtn;
        // ── FAB (global info button) ──
        private VisualElement _fabContainer;
        private Button _fabInfoBtn;
        // ── Slider (explosion) ──
        private Slider explosionSlider;

        // ── State ──
        private bool _hotspotsInitialized = false;
        private bool _isIsolated = false;



        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                HandleEscapeKey();
        }

        /// <summary>
        /// Escape key priority: sheet → sub-panel / active mode → nothing.
        /// </summary>
        private void HandleEscapeKey()
        {
            // 1. Close bottom sheet if open
            if (_detailsSheet != null && _detailsSheet.IsSheetOpen)
            {
                _detailsSheet.SetSheetState(false);
                return;
            }

            // 2. Close sub-panel or deactivate mode
            if (_modeController != null && _modeController.HandleEscapeKey())
                return;
        }

        // ── Memory Leak Prevention (Phase 3 Step 1) ──
        private System.Collections.Generic.List<System.Action> _uiCleanupActions = new System.Collections.Generic.List<System.Action>();

        // ── Button input blocker callbacks (shared instances) ──
        private EventCallback<PointerEnterEvent> _onBtnEnter = evt => InputManager.InputBlocked = true;
        private EventCallback<PointerLeaveEvent> _onBtnLeave = evt => InputManager.InputBlocked = false;
        private EventCallback<PointerDownEvent> _onBtnDown = evt => evt.StopPropagation();
        private EventCallback<PointerUpEvent> _onBtnUp = evt => evt.StopPropagation();

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            if (mainDocument == null)
                mainDocument = GetComponent<UIDocument>();

            if (mainDocument != null)
            {
                root = mainDocument.rootVisualElement;
                InitializeUI();
                SubscribeToEvents();
            }
        }

        private void OnDisable()
        {
            // Pre-clear dynamic VisualElements before UIDocument teardown
            // to prevent "Cannot modify hierarchy during layout" errors (Unity 6 known issue).
            HotspotManager.Instance?.ClearHotspots();

            UnsubscribeFromEvents();
            UnsubscribeFromUIEvents();
        }

        // ═══════════════════════════════════════════════════════
        //  Cleanup helpers
        // ═══════════════════════════════════════════════════════

        private void AddCleanup(System.Action cleanupAction)
        {
            if (cleanupAction != null) _uiCleanupActions.Add(cleanupAction);
        }

        private void UnsubscribeFromUIEvents()
        {
            foreach (var action in _uiCleanupActions) action?.Invoke();
            _uiCleanupActions.Clear();
            Debug.Log("[UIManager] UI Events Unsubscribed to prevent memory leaks.");
        }

        // ═══════════════════════════════════════════════════════
        //  Initialization
        // ═══════════════════════════════════════════════════════

        private void InitializeUI()
        {
            if (root == null) return;

            // ── Auto-create managers if missing ──
            EnsureManagers();

            // ── Query shared elements ──
            resetBtn = root.Q<Button>("ResetViewBtn");
            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // ── Details Sheet (info toggle handled via UIModeController event, not direct button binding) ──
            _detailsSheet = new UIDetailsSheet(root, null);
            AddCleanup(() => _detailsSheet.Dispose());

            // ── Mode Controller (3-mode system: Tools / Analyze / Studio) ──
            _modeController = new UIModeController(
                root,
                root.Q<VisualElement>("ToolsModeContainer"),
                root.Q<VisualElement>("AnalyzeModeContainer"),
                root.Q<VisualElement>("StudioModeContainer"),
                explosionSlider
            );
            AddCleanup(() => _modeController.Dispose());

            // Wire Inspect-mode isolate toggle event from UIModeController
            _modeController.OnIsolateToggleRequested += () => ToggleIsolation();

            // Notify mode controller when sheet state changes + sync FAB active state
            _detailsSheet.OnSheetStateChanged += (isOpen) =>
            {
                _modeController.SetSheetOpenState(isOpen);
                _fabInfoBtn?.EnableInClassList("fab-button--active", isOpen);
            };

            // Notify sheet to close if any mode activates
            _modeController.OnAnyModeActivated += () =>
            {
                if (_detailsSheet.IsSheetOpen)
                {
                    _detailsSheet.SetSheetState(false);
                }
            };

            // ── Analyze Panel (shader cards — ShaderMenu inside StudioModeContainer) ──
            var shaderMenu = root.Q<VisualElement>("ShaderMenu");
            _uiAnalyzePanel = new UIAnalyzePanel(shaderMenu, null);
            AddCleanup(() => _uiAnalyzePanel.Dispose());

            // ── Environment Panel (presets + sliders — StudioPanel inside StudioModeContainer) ──
            _uiEnvironmentPanel = new UIEnvironmentPanel(root.Q<VisualElement>("StudioPanel"));
            AddCleanup(() => _uiEnvironmentPanel.Dispose());

            // ── Cross-Section Panel (inside AnalyzeModeContainer) ──
            _uiCrossSectionPanel = new UICrossSectionPanel(root.Q<VisualElement>("CrossSectionPanel"));
            AddCleanup(() => _uiCrossSectionPanel.Dispose());

            // ── Hero Controller ──
            _heroController = new UIHeroController(root);
            _heroController.OnHeroDismissed += OnHeroDismissed;
            _heroController.OnHeroReturned += OnHeroReturned;
            AddCleanup(() => _heroController.Dispose());

            // ── Onboarding Controller ──
            _onboardingController = new OnboardingController(root);
            _heroController.OnHelpRequested += () => _onboardingController.Show();
            AddCleanup(() => _onboardingController.Dispose());

            // ── Wire remaining toolbar buttons (mode buttons handled by UIModeController) ──
            if (resetBtn != null) { resetBtn.clicked += OnResetClicked; AddCleanup(() => resetBtn.clicked -= OnResetClicked); }

            // ── Category filter buttons (inside AnalyzeModeContainer/FilterSubPanel) ──
            BindCat("CatBtn_All", "ALL");
            BindCat("CatBtn_Structure", "Structure");
            BindCat("CatBtn_Propulsion", "Propulsion");
            BindCat("CatBtn_Avionics", "Avionics");
            BindCat("CatBtn_Power", "Power");
            BindCat("CatBtn_Payload", "Payload");

            // ── FAB (global info button) ──
            _fabContainer = root.Q<VisualElement>("GlobalActionContainer");
            _fabInfoBtn = root.Q<Button>("ToolInfoBtn");
            if (_fabInfoBtn != null)
            {
                System.Action fabClick = () => _detailsSheet.ToggleInfo();
                _fabInfoBtn.clicked += fabClick;
                AddCleanup(() => _fabInfoBtn.clicked -= fabClick);
            }

            // ── Explosion slider ──
            if (explosionSlider != null)
            {
                explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);
                AddCleanup(() => explosionSlider.UnregisterValueChangedCallback(OnExplosionSliderChanged));

                EventCallback<PointerEnterEvent> esEn = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> esLe = evt => InputManager.InputBlocked = false;
                EventCallback<PointerDownEvent> esDo = evt => evt.StopPropagation();
                explosionSlider.RegisterCallback(esEn);
                explosionSlider.RegisterCallback(esLe);
                explosionSlider.RegisterCallback(esDo);
                AddCleanup(() => { explosionSlider.UnregisterCallback(esEn); explosionSlider.UnregisterCallback(esLe); explosionSlider.UnregisterCallback(esDo); });
            }

            // ── ExplodeInlineSlider container — block pointer events from bubbling
            //    past the slider so clicks don't reach parent handlers. ──
            var explodeInline = root.Q<VisualElement>("ExplodeInlineSlider");
            if (explodeInline != null)
            {
                EventCallback<PointerEnterEvent> epEn = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> epLe = evt => InputManager.InputBlocked = false;
                EventCallback<PointerDownEvent> epDo = evt => evt.StopPropagation();
                explodeInline.RegisterCallback(epEn);
                explodeInline.RegisterCallback(epLe);
                explodeInline.RegisterCallback(epDo);
                AddCleanup(() => { explodeInline.UnregisterCallback(epEn); explodeInline.UnregisterCallback(epLe); explodeInline.UnregisterCallback(epDo); });
            }

            // ── CrossSectionPanel & FilterSubPanel — same protection ──
            foreach (var panelName in new[] { "CrossSectionPanel", "FilterSubPanel" })
            {
                var panel = root.Q<VisualElement>(panelName);
                if (panel == null) continue;
                EventCallback<PointerEnterEvent> pEn = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> pLe = evt => InputManager.InputBlocked = false;
                EventCallback<PointerDownEvent> pDo = evt => evt.StopPropagation();
                panel.RegisterCallback(pEn);
                panel.RegisterCallback(pLe);
                panel.RegisterCallback(pDo);
                AddCleanup(() => { panel.UnregisterCallback(pEn); panel.UnregisterCallback(pLe); panel.UnregisterCallback(pDo); });
            }

            // ── Initial state ──
            _detailsSheet.UpdatePartIndicator(null);
            // Slider is inside ExplodeSubPanel (starts hidden via submenu--hidden)

            // ── All buttons block 3D input ──
            RegisterButtonInputBlockers();
        }

        // ═══════════════════════════════════════════════════════
        //  Category filter helper
        // ═══════════════════════════════════════════════════════

        private void BindCat(string btnName, string category)
        {
            var btn = root.Q<Button>(btnName);
            if (btn == null) return;
            System.Action a = () => _modeController.SetCategoryFilter(category, btn);
            btn.clicked += a;
            AddCleanup(() => btn.clicked -= a);
        }

        private void SetFabVisible(bool visible)
        {
            _fabContainer?.EnableInClassList("fab-button--hidden", !visible);
        }

        // ═══════════════════════════════════════════════════════
        //  Hero lifecycle callbacks
        // ═══════════════════════════════════════════════════════

        private void OnHeroDismissed()
        {
            // Initialize hotspots now
            if (!_hotspotsInitialized && HotspotManager.Instance != null)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance.Initialize(root);
            }
            HotspotManager.Instance?.SetVisible(true);

            // Show onboarding overlay the first time the user enters the 3D view
            _onboardingController?.ShowIfFirstTime();
        }

        private void OnHeroReturned()
        {
            // Close everything
            _detailsSheet.SetSheetState(false);
            _modeController.DeactivateAllModes();

            // Hide hotspots
            HotspotManager.Instance?.SetVisible(false);

            // Reset camera
            OrbitCameraController.Instance?.ResetView();

            // Reset view mode
            ViewModeManager.Instance?.SetViewMode(ViewMode.Realistic);

            // Reset app state — handled by DeactivateAllModes → Exploration
        }

        // ═══════════════════════════════════════════════════════
        //  Toolbar button handlers (simple, remain here)
        // ═══════════════════════════════════════════════════════

        private void OnResetClicked()
        {
            if (AppStateMachine.Instance?.CurrentState != null && AppStateMachine.Instance.CurrentState != AppState.Exploration)
                AppStateMachine.Instance.EnterExploration();

            OrbitCameraController.Instance?.ResetView();

            if (ViewModeManager.Instance?.CurrentMode != null && ViewModeManager.Instance.CurrentMode != ViewMode.Realistic)
                ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);

            // Explicitly disable cross-section on reset (one of only two valid triggers)
            CrossSectionManager.Instance?.DisableCrossSection();

            EventBus.Publish(new PartSelectedEvent(null));
        }

        private void OnExplosionSliderChanged(ChangeEvent<float> evt)
        {
            ExplodedViewManager.Instance?.SetExplosionFactor(evt.newValue);

            // Update dynamic percentage label
            var label = root.Q<Label>("ExplosionValue");
            if (label != null)
                label.text = $"{(evt.newValue * 100):F0}%";

            if (AppStateMachine.Instance == null) return;

            // Moving slider > 0 enters ExplodedView, moving to 0 exits
            if (evt.newValue > 0.001f && AppStateMachine.Instance.CurrentState != AppState.ExplodedView)
            {
                AppStateMachine.Instance.EnterExplodedView();
            }
            else if (evt.newValue <= 0.001f && AppStateMachine.Instance.CurrentState == AppState.ExplodedView)
            {
                AppStateMachine.Instance.EnterExploration();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Isolation helpers
        // ═══════════════════════════════════════════════════════

        private void IsolateSelectedPart()
        {
            var sel = SelectionManager.Instance?.CurrentSelection;
            if (sel == null) return;
            var part = sel.GetComponent<ExplodablePart>();
            if (part == null) return;
            PartVisibilityManager.Instance?.IsolatePart(part);
            _isIsolated = true;
            _modeController.SetIsolateState(true);
        }

        private void ClearIsolation()
        {
            PartVisibilityManager.Instance?.ClearIsolation();
            _isIsolated = false;
            _modeController.SetIsolateState(false);
        }

        private void ToggleIsolation()
        {
            if (_isIsolated)
                ClearIsolation();
            else
                IsolateSelectedPart();
        }

        // ═══════════════════════════════════════════════════════
        //  EventBus subscriptions
        // ═══════════════════════════════════════════════════════

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Subscribe<PartDoubleClickedEvent>(OnPartDoubleClicked);
            EventBus.Subscribe<StateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged += OnViewModeChanged;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Unsubscribe<PartDoubleClickedEvent>(OnPartDoubleClicked);
            EventBus.Unsubscribe<StateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged -= OnViewModeChanged;
        }

        // ═══════════════════════════════════════════════════════
        //  Event callbacks — route to sub-controllers
        // ═══════════════════════════════════════════════════════

        private void OnPartSelected(PartSelectedEvent evt)
        {
            // Close sub-menus within current mode when a part is selected
            // Skip when selection comes from a hotspot to avoid flashing/re-opening the sheet
            if (!evt.FromHotspot)
                _modeController.CloseAllMenus();

            // Delegate data display to details sheet
            _detailsSheet.PopulatePartData(evt.PartData, evt.FromHotspot);

            // Show/hide FAB: visible when a part is selected OR the sheet is still open
            bool fabVisible = evt.PartData != null
                || (_detailsSheet != null && _detailsSheet.IsSheetOpen);
            SetFabVisible(fabVisible);
        }

        /// <summary>
        /// Handles double-click events published by SelectionManager.
        /// </summary>
        private void OnPartDoubleClicked(PartDoubleClickedEvent evt)
        {
            if (evt.PartData == null)
            {
                // Double-click on background → exit isolate if active
                if (_isIsolated) ClearIsolation();
            }
            else if (_isIsolated)
            {
                // Already isolated + double-click on part → de-isolate
                ClearIsolation();
            }
            else
            {
                // First double-click on a part → isolate + open info
                _detailsSheet.OpenSheet();
                IsolateSelectedPart();
            }
        }

        private void OnAppStateChanged(StateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            bool isInteractive = AppStateMachine.Instance?.IsInteractive() ?? false;

            // Sync mode controller with external state changes
            _modeController.SyncWithAppState(evt.NewState);

            // Hotspots: initialize once after hero is dismissed, but do NOT
            // auto-set visibility — that is controlled only by the Pins toggle button.
            bool heroDismissed = _heroController != null && _heroController.HeroDismissed;
            if (heroDismissed && isInteractive && !_hotspotsInitialized)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance?.Initialize(root);
                // Show hotspots on first init (default state)
                HotspotManager.Instance?.SetVisible(true);
            }

            // Slider visibility now controlled by Explode button toggle, not by state changes
        }

        private void OnViewModeChanged(ViewMode newMode)
        {
            if (_uiAnalyzePanel != null) _uiAnalyzePanel.OnViewModeChanged(newMode);
        }

        // ═══════════════════════════════════════════════════════
        //  Button input blockers (all buttons block 3D raycast)
        // ═══════════════════════════════════════════════════════

        private void RegisterButtonInputBlockers()
        {
            root.Query<Button>().ForEach(btn =>
            {
                btn.RegisterCallback(_onBtnEnter);
                btn.RegisterCallback(_onBtnLeave);
                btn.RegisterCallback(_onBtnDown);
                btn.RegisterCallback(_onBtnUp);

                AddCleanup(() =>
                {
                    btn.UnregisterCallback(_onBtnEnter);
                    btn.UnregisterCallback(_onBtnLeave);
                    btn.UnregisterCallback(_onBtnDown);
                    btn.UnregisterCallback(_onBtnUp);
                });
            });
        }

        // ═══════════════════════════════════════════════════════
        //  Manager auto-creation
        // ═══════════════════════════════════════════════════════

        private void EnsureManagers()
        {
            GameObject managers = GameObject.Find("Managers");
            if (managers == null) managers = new GameObject("Managers");

            if (HotspotManager.Instance == null) managers.AddComponent<HotspotManager>();
            if (ViewModeManager.Instance == null) managers.AddComponent<ViewModeManager>();
            if (!ServiceLocator.Has<EnvironmentController>()) managers.AddComponent<EnvironmentController>();
            if (CrossSectionManager.Instance == null) managers.AddComponent<CrossSectionManager>();
            if (PartVisibilityManager.Instance == null) managers.AddComponent<PartVisibilityManager>();
        }
    }
}
