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

        // ── Buttons (wired here, logic delegated) ──
        private Button resetBtn;

        // ── Slider (explosion) ──
        private Slider explosionSlider;
        private VisualElement sliderContainer;

        // ── State ──
        private bool _hotspotsInitialized = false;

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
            var hotspotBtn = root.Q<Button>("HotspotBtn");
            sliderContainer = root.Q<VisualElement>("SliderContainer");
            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // ── Details Sheet (no InfoBtn in new layout — parts clicked directly) ──
            _detailsSheet = new UIDetailsSheet(root, null);
            AddCleanup(() => _detailsSheet.Dispose());

            // ── Mode Controller (replaces UIPopupController) ──
            _modeController = new UIModeController(
                root,
                root.Q<VisualElement>("AnalyzeModeContainer"),
                root.Q<VisualElement>("StudioModeContainer"),
                root.Q<VisualElement>("PopupBlocker"),
                explosionSlider,
                hotspotBtn
            );
            AddCleanup(() => _modeController.Dispose());

            // Notify mode controller when sheet state changes
            _detailsSheet.OnSheetStateChanged += (isOpen) =>
            {
                _modeController.SetSheetOpenState(isOpen);
            };

            // ── Analyze Panel (shader cards — ShaderMenu is now inside AnalyzeModeContainer) ──
            var shaderMenu = root.Q<VisualElement>("ShaderMenu");
            _uiAnalyzePanel = new UIAnalyzePanel(shaderMenu, null);
            AddCleanup(() => _uiAnalyzePanel.Dispose());

            // ── Environment Panel (presets + sliders — EnvPanel is now inside StudioModeContainer) ──
            _uiEnvironmentPanel = new UIEnvironmentPanel(root.Q<VisualElement>("EnvPanel"));
            AddCleanup(() => _uiEnvironmentPanel.Dispose());

            // ── Cross-Section Panel (inside AnalyzeModeContainer) ──
            _uiCrossSectionPanel = new UICrossSectionPanel(root.Q<VisualElement>("CrossSectionPanel"));
            AddCleanup(() => _uiCrossSectionPanel.Dispose());

            // ── Hero Controller ──
            _heroController = new UIHeroController(root);
            _heroController.OnHeroDismissed += OnHeroDismissed;
            _heroController.OnHeroReturned += OnHeroReturned;
            AddCleanup(() => _heroController.Dispose());

            // ── Wire remaining toolbar buttons (mode buttons are handled by UIModeController) ──
            if (resetBtn != null) { resetBtn.clicked += OnResetClicked; AddCleanup(() => resetBtn.clicked -= OnResetClicked); }

            // ── Category filter buttons (inside AnalyzeModeContainer) ──
            BindCat("CatBtn_All", "ALL");
            BindCat("CatBtn_Structure", "Structure");
            BindCat("CatBtn_Propulsion", "Propulsion");
            BindCat("CatBtn_Avionics", "Avionics");
            BindCat("CatBtn_Power", "Power");

            // ── Hotspot button (inside CategoryMenu in AnalyzeModeContainer) ──
            if (hotspotBtn != null) { hotspotBtn.clicked += _modeController.ToggleHotspots; AddCleanup(() => hotspotBtn.clicked -= _modeController.ToggleHotspots); }

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

            // ── Initial state ──
            _detailsSheet.UpdatePartIndicator(null);
            if (sliderContainer != null) sliderContainer.AddToClassList("slider-hidden");

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

            EventBus.Publish(new PartSelectedEvent(null));
        }

        private void OnExplosionSliderChanged(ChangeEvent<float> evt)
        {
            ExplodedViewManager.Instance?.SetExplosionFactor(evt.newValue);
        }

        // ═══════════════════════════════════════════════════════
        //  EventBus subscriptions
        // ═══════════════════════════════════════════════════════

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Subscribe<AppStateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged += OnViewModeChanged;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Unsubscribe<AppStateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged -= OnViewModeChanged;
        }

        // ═══════════════════════════════════════════════════════
        //  Event callbacks — route to sub-controllers
        // ═══════════════════════════════════════════════════════

        private void OnPartSelected(PartSelectedEvent evt)
        {
            // Close sub-menus within current mode when a part is selected
            _modeController.CloseAllMenus();

            // Delegate data display to details sheet
            _detailsSheet.PopulatePartData(evt.PartData, evt.FromHotspot);
        }

        private void OnAppStateChanged(AppStateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            bool isInteractive = AppStateMachine.Instance?.IsInteractive() ?? false;

            // Sync mode controller with external state changes
            _modeController.SyncWithAppState(evt.NewState);

            // Hotspots: only after hero is dismissed
            bool heroDismissed = _heroController != null && _heroController.HeroDismissed;
            if (heroDismissed && isInteractive && !_hotspotsInitialized)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance?.Initialize(root);
            }
            HotspotManager.Instance?.SetVisible(heroDismissed && isInteractive);

            // Slider visibility (only in ExplodedView)
            _modeController.SetSliderVisible(isExploded);
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
            if (EnvironmentController.Instance == null) managers.AddComponent<EnvironmentController>();
        }
    }
}
