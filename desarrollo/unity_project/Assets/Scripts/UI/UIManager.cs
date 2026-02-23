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
    /// Slim coordinator for all UI sub-systems (Phase 3 Step 2: God Class Dismantling).
    /// Delegates domain logic to:
    ///   - UIDetailsSheet   → bottom sheet, part data display
    ///   - UIPopupController → popup menus, stacking, category filters
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

        // ── Sub-Controllers (Phase 3 Step 2) ──
        private UIDetailsSheet _detailsSheet;
        private UIPopupController _popupController;
        private UIHeroController _heroController;
        private UIAnalyzePanel _uiAnalyzePanel;
        private UIEnvironmentPanel _uiEnvironmentPanel;

        // ── Buttons (wired here, logic delegated) ──
        private Button shaderBtn;
        private Button explodeBtn;
        private Button resetBtn;
        private Button envBtn;

        // ── Slider (explosion) ──
        private Slider explosionSlider;
        private VisualElement sliderContainer;

        // ── State ──
        private bool _hotspotsInitialized = false;

        // ── Memory Leak Prevention (Phase 3 Step 1) ──
        private System.Collections.Generic.List<System.Action> _uiCleanupActions = new System.Collections.Generic.List<System.Action>();

        // ── Button input blocker callbacks (shared instances) ──
        private EventCallback<PointerEnterEvent> _onBtnEnter = evt => OrbitCameraController.GlobalInputBlocked = true;
        private EventCallback<PointerLeaveEvent> _onBtnLeave = evt => OrbitCameraController.GlobalInputBlocked = false;
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
            shaderBtn = root.Q<Button>("ShaderBtn");
            explodeBtn = root.Q<Button>("ExplodeBtn");
            resetBtn = root.Q<Button>("ResetViewBtn");
            envBtn = root.Q<Button>("EnvBtn");
            var infoBtn = root.Q<Button>("InfoBtn");
            var hotspotBtn = root.Q<Button>("HotspotBtn");
            sliderContainer = root.Q<VisualElement>("SliderContainer");
            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // ── Details Sheet ──
            _detailsSheet = new UIDetailsSheet(root, infoBtn);
            AddCleanup(() => _detailsSheet.Dispose());

            // ── Popup Controller ──
            _popupController = new UIPopupController(
                root,
                root.Q<VisualElement>("ShaderMenu"),
                root.Q<VisualElement>("CategoryMenu"),
                root.Q<VisualElement>("EnvPanel"),
                sliderContainer,
                root.Q<VisualElement>("PopupBlocker"),
                explosionSlider,
                hotspotBtn
            );
            AddCleanup(() => _popupController.Dispose());

            // Notify popup controller when sheet state changes
            _detailsSheet.OnSheetStateChanged += (isOpen) =>
            {
                _popupController.SetSheetOpenState(isOpen);
            };

            // ── Analyze Panel (shader cards) ──
            _uiAnalyzePanel = new UIAnalyzePanel(root.Q<VisualElement>("ShaderMenu"), shaderBtn);
            AddCleanup(() => _uiAnalyzePanel.Dispose());

            // ── Environment Panel (presets + sliders) ──
            _uiEnvironmentPanel = new UIEnvironmentPanel(root.Q<VisualElement>("EnvPanel"));
            AddCleanup(() => _uiEnvironmentPanel.Dispose());

            // ── Hero Controller ──
            _heroController = new UIHeroController(root);
            _heroController.OnHeroDismissed += OnHeroDismissed;
            _heroController.OnHeroReturned += OnHeroReturned;
            AddCleanup(() => _heroController.Dispose());

            // ── Wire toolbar buttons ──
            if (shaderBtn != null) { shaderBtn.clicked += _popupController.ToggleShaderMenu; AddCleanup(() => shaderBtn.clicked -= _popupController.ToggleShaderMenu); }
            if (explodeBtn != null) { explodeBtn.clicked += OnExplodeToggle; AddCleanup(() => explodeBtn.clicked -= OnExplodeToggle); }
            if (resetBtn != null) { resetBtn.clicked += OnResetClicked; AddCleanup(() => resetBtn.clicked -= OnResetClicked); }
            if (envBtn != null) { envBtn.clicked += _popupController.ToggleEnvPanel; AddCleanup(() => envBtn.clicked -= _popupController.ToggleEnvPanel); }

            // ── Layer button → category menu ──
            var layerBtn = root.Q<Button>("LayerBtn");
            if (layerBtn != null) { layerBtn.clicked += _popupController.ToggleCategoryMenu; AddCleanup(() => layerBtn.clicked -= _popupController.ToggleCategoryMenu); }

            // ── Category filter buttons ──
            BindCat("CatBtn_All", "ALL");
            BindCat("CatBtn_Structure", "Structure");
            BindCat("CatBtn_Propulsion", "Propulsion");
            BindCat("CatBtn_Avionics", "Avionics");
            BindCat("CatBtn_Power", "Power");

            // ── Hotspot button ──
            if (hotspotBtn != null) { hotspotBtn.clicked += _popupController.ToggleHotspots; AddCleanup(() => hotspotBtn.clicked -= _popupController.ToggleHotspots); }

            // ── Explosion slider ──
            if (explosionSlider != null)
            {
                explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);
                AddCleanup(() => explosionSlider.UnregisterValueChangedCallback(OnExplosionSliderChanged));

                EventCallback<PointerEnterEvent> esEn = evt => OrbitCameraController.GlobalInputBlocked = true;
                EventCallback<PointerLeaveEvent> esLe = evt => OrbitCameraController.GlobalInputBlocked = false;
                EventCallback<PointerDownEvent> esDo = evt => evt.StopPropagation();
                explosionSlider.RegisterCallback(esEn);
                explosionSlider.RegisterCallback(esLe);
                explosionSlider.RegisterCallback(esDo);
                AddCleanup(() => { explosionSlider.UnregisterCallback(esEn); explosionSlider.UnregisterCallback(esLe); explosionSlider.UnregisterCallback(esDo); });
            }

            // ── Initial state ──
            _detailsSheet.UpdatePartIndicator(null);
            if (infoBtn != null) infoBtn.SetEnabled(false);
            if (sliderContainer != null) sliderContainer.AddToClassList("slider-hidden");

            // ── All buttons block 3D input ──
            RegisterButtonInputBlockers();

            // ── Initial layout ──
            _popupController.RepositionPopups();
        }

        // ═══════════════════════════════════════════════════════
        //  Category filter helper
        // ═══════════════════════════════════════════════════════

        private void BindCat(string btnName, string category)
        {
            var btn = root.Q<Button>(btnName);
            if (btn == null) return;
            System.Action a = () => _popupController.SetCategoryFilter(category, btn);
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
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.SetVisible(true);
        }

        private void OnHeroReturned()
        {
            // Close everything
            _detailsSheet.SetSheetState(false);
            _popupController.CloseAllMenus();

            // Hide hotspots
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.SetVisible(false);

            // Reset camera
            if (OrbitCameraController.Instance != null)
                OrbitCameraController.Instance.ResetView();

            // Reset view mode
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);

            // Reset app state
            if (AppStateMachine.Instance != null)
                AppStateMachine.Instance.SetState(AppState.Exploration);
        }

        // ═══════════════════════════════════════════════════════
        //  Toolbar button handlers (simple, remain here)
        // ═══════════════════════════════════════════════════════

        private void OnExplodeToggle()
        {
            if (AppStateMachine.Instance != null)
            {
                var current = AppStateMachine.Instance.CurrentState;
                if (current == AppState.ExplodedView)
                    AppStateMachine.Instance.EnterExploration();
                else
                    AppStateMachine.Instance.EnterExplodedView();
            }
        }

        private void OnResetClicked()
        {
            if (AppStateMachine.Instance != null && AppStateMachine.Instance.CurrentState != AppState.Exploration)
                AppStateMachine.Instance.EnterExploration();

            if (OrbitCameraController.Instance != null)
                OrbitCameraController.Instance.ResetView();

            if (ViewModeManager.Instance != null && ViewModeManager.Instance.CurrentMode != ViewMode.Realistic)
                ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);

            EventBus.Publish(new PartSelectedEvent(null));
        }

        private void OnExplosionSliderChanged(ChangeEvent<float> evt)
        {
            if (ExplodedViewManager.Instance != null)
                ExplodedViewManager.Instance.SetExplosionFactor(evt.newValue);
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
            // Close all popup menus when a part is selected/deselected
            _popupController.CloseAllMenus();

            // Delegate data display to details sheet
            _detailsSheet.PopulatePartData(evt.PartData, evt.FromHotspot);
        }

        private void OnAppStateChanged(AppStateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            bool isInteractive = evt.NewState == AppState.Exploration || isExploded;

            // Hotspots: only after hero is dismissed
            bool heroDismissed = _heroController != null && _heroController.HeroDismissed;
            if (heroDismissed && isInteractive && !_hotspotsInitialized)
            {
                _hotspotsInitialized = true;
                if (HotspotManager.Instance != null)
                    HotspotManager.Instance.Initialize(root);
            }
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.SetVisible(heroDismissed && isInteractive);

            // Slider visibility
            _popupController.SetSliderVisible(isExploded);
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
