using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
using WebGL.UI.Panels;
using System.Collections;

namespace WebGL.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainDocument;

        // Visual Elements
        private VisualElement root;
        private VisualElement detailsSheet;
        private VisualElement bottomBar;
        private VisualElement sliderContainer;
        private VisualElement categoryMenu;
        private VisualElement shaderMenu;
        private VisualElement envPanel;
        private VisualElement popupBlocker;
        private Label partNameLabel;
        private Label sheetTitle;
        private Label sheetCategory;
        private Label sheetFunction;
        private Label sheetMaterial;
        private Label sheetDesc;
        private Label sheetWeight;
        private Label sheetDimensions;
        private Label sheetPower;
        private Label sheetTemp;
        private Label sheetDifficulty;
        private Label sheetTools;
        private Label sheetAssemblyTime;

        // Buttons
        private Button shaderBtn;
        private Button explodeBtn;
        private Button infoBtn;
        private Button resetBtn;
        private Button envBtn;
        private Button hotspotBtn;

        // Input Controls
        private Slider explosionSlider;
        private Slider envLightRotSlider;
        private Slider envLightIntSlider;

        // State
        private bool isSheetOpen = false;
        private bool _hotspotsInitialized = false;
        private bool _heroDismissed = false;
        private VisualElement _heroContainer;
        private System.Collections.Generic.List<string> activeCategories = new System.Collections.Generic.List<string>() { "ALL" };

        // Shader long-press
        private bool _shaderMenuShown = false;
        private bool _hotspotsEnabled = false;

        // Sheet Content Containers
        private VisualElement contentDetails; 
        
        // Hero Submenus
        private VisualElement heroMain;
        private VisualElement submenuDevices;
        private VisualElement submenuAbout;
        private VisualElement submenuExit;

        public enum SheetMode { Details } // Only Details uses the sheet now
        public enum SubmenuType { Devices, About, Exit }
        
        private SheetMode currentSheetMode = SheetMode.Details;

        // ── Extracted Panels (Phase 3: Step 2) ──
        private UIEnvironmentPanel _uiEnvironmentPanel;
        private UIAnalyzePanel _uiAnalyzePanel;

        // ── Layout Math Constants (8pt grid) ──
        // padding-bottom(24) + info-row(24) + margin(16) + button(104) + EXTRA_GAP(24) = 192px
        private const float POPUP_BASE_BOTTOM = 192f;
        private const float POPUP_GAP = 24f;        // 8pt × 3 (Increased from 16f)


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

        // ── Memory Leak Prevention (Phase 3: Step 1) ──
        private System.Collections.Generic.List<System.Action> _uiCleanupActions = new System.Collections.Generic.List<System.Action>();

        private void AddCleanup(System.Action cleanupAction)
        {
            if (cleanupAction != null) _uiCleanupActions.Add(cleanupAction);
        }

        private void UnsubscribeFromUIEvents()
        {
            foreach (var action in _uiCleanupActions)
            {
                action?.Invoke();
            }
            _uiCleanupActions.Clear();
            Debug.Log("[UIManager] UI Events Unsubscribed to prevent memory leaks.");
        }



        private void InitializeUI()
        {
            if (root == null) return;

            // Bind Elements
            detailsSheet = root.Q<VisualElement>("BottomSheet");
            bottomBar = root.Q<VisualElement>("BottomBar");
            
            // Bind Content Containers
            contentDetails = root.Q<VisualElement>("SheetContent_Details");
            // Removed other sheet contents as they are now Hero Submenus

            sliderContainer = root.Q<VisualElement>("SliderContainer");
            
            if (detailsSheet != null)
            {
                EventCallback<PointerDownEvent> pd = evt => evt.StopPropagation();
                EventCallback<PointerUpEvent> pu = evt => evt.StopPropagation();
                detailsSheet.RegisterCallback(pd);
                detailsSheet.RegisterCallback(pu);
                AddCleanup(() => { detailsSheet.UnregisterCallback(pd); detailsSheet.UnregisterCallback(pu); });
                
                EventCallback<PointerEnterEvent> pe = evt => OrbitCameraController.GlobalInputBlocked = true;
                EventCallback<PointerLeaveEvent> pl = evt => OrbitCameraController.GlobalInputBlocked = false;
                detailsSheet.RegisterCallback(pe);
                detailsSheet.RegisterCallback(pl);
                AddCleanup(() => { detailsSheet.UnregisterCallback(pe); detailsSheet.UnregisterCallback(pl); });
            }
            
            partNameLabel = root.Q<Label>("SelectionIndicator");
            
            sheetTitle = root.Q<Label>("SheetTitle");
            sheetCategory = root.Q<Label>("PartCategory");
            sheetFunction = root.Q<Label>("PartFunction");
            sheetMaterial = root.Q<Label>("PartMaterial");
            sheetDesc = root.Q<Label>("PartDescription");
            sheetWeight = root.Q<Label>("PartWeight");
            sheetDimensions = root.Q<Label>("PartDimensions");
            sheetPower = root.Q<Label>("PartPower");
            sheetTemp = root.Q<Label>("PartTemp");
            sheetDifficulty = root.Q<Label>("PartDifficulty");
            sheetTools = root.Q<Label>("PartTools");
            sheetAssemblyTime = root.Q<Label>("PartAssemblyTime");

            shaderBtn = root.Q<Button>("ShaderBtn");
            explodeBtn = root.Q<Button>("ExplodeBtn");
            infoBtn = root.Q<Button>("InfoBtn");
            resetBtn = root.Q<Button>("ResetViewBtn");
            envBtn = root.Q<Button>("EnvBtn");
            hotspotBtn = root.Q<Button>("HotspotBtn");

            // ── Popup Blocker ──
            popupBlocker = root.Q<VisualElement>("PopupBlocker");
            if (popupBlocker != null)
            {
                EventCallback<PointerDownEvent> pbDown = evt => CloseAllMenus();
                popupBlocker.RegisterCallback(pbDown);
                AddCleanup(() => popupBlocker.UnregisterCallback(pbDown));
            }

            explosionSlider = root.Q<Slider>("ExplosionSlider");
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
            if (hotspotBtn != null) { hotspotBtn.clicked += ToggleHotspots; AddCleanup(() => hotspotBtn.clicked -= ToggleHotspots); }

            // ── Shader Button: Single Click Toggle ──
            if (shaderBtn != null) { shaderBtn.clicked += ToggleShaderMenu; AddCleanup(() => shaderBtn.clicked -= ToggleShaderMenu); }

            if (explodeBtn != null) { explodeBtn.clicked += OnExplodeToggle; AddCleanup(() => explodeBtn.clicked -= OnExplodeToggle); }
            if (infoBtn != null) { infoBtn.clicked += OnInfoToggle; AddCleanup(() => infoBtn.clicked -= OnInfoToggle); }
            if (resetBtn != null) { resetBtn.clicked += OnResetClicked; AddCleanup(() => resetBtn.clicked -= OnResetClicked); }

            // ── Shader Menu ──
            shaderMenu = root.Q<VisualElement>("ShaderMenu");
            _uiAnalyzePanel = new UIAnalyzePanel(shaderMenu, shaderBtn);
            AddCleanup(() => _uiAnalyzePanel.Dispose());

            // ── Environment Panel ──
            envPanel = root.Q<VisualElement>("EnvPanel");
            if (envBtn != null) { envBtn.clicked += ToggleEnvPanel; AddCleanup(() => envBtn.clicked -= ToggleEnvPanel); }
            _uiEnvironmentPanel = new UIEnvironmentPanel(envPanel);
            AddCleanup(() => _uiEnvironmentPanel.Dispose());

            // ── Category Menu ──
            categoryMenu = root.Q<VisualElement>("CategoryMenu");
            var btnAll = root.Q<Button>("CatBtn_All");
            var btnStructure = root.Q<Button>("CatBtn_Structure");
            var btnPropulsion = root.Q<Button>("CatBtn_Propulsion");
            var btnAvionics = root.Q<Button>("CatBtn_Avionics");
            var btnPower = root.Q<Button>("CatBtn_Power");

            var layerBtn = root.Q<Button>("LayerBtn");
            if (layerBtn != null) { layerBtn.clicked += ToggleCategoryMenu; AddCleanup(() => layerBtn.clicked -= ToggleCategoryMenu); }

            void BindCat(Button b, string cat) { 
                if (b == null) return; 
                System.Action a = () => SetCategoryFilter(cat, b); 
                b.clicked += a; 
                AddCleanup(() => b.clicked -= a); 
            }

            BindCat(btnAll, "ALL");
            BindCat(btnStructure, "Structure");
            BindCat(btnPropulsion, "Propulsion");
            BindCat(btnAvionics, "Avionics");
            BindCat(btnPower, "Power");
    
            // Moved slider logic up to registration block above
            // if (explosionSlider != null) explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);

            var header = root.Q(className: "sheet-header");
            var handle = root.Q(className: "sheet-handle");
            if (header != null) header.RegisterCallback<ClickEvent>(evt => SetSheetState(!isSheetOpen));
            
            // Drag to dismiss
            if (handle != null)
            {
                handle.RegisterCallback<PointerDownEvent>(evt => { _dragStartY = evt.position.y; _isDraggingSheet = true; });
                handle.RegisterCallback<PointerUpEvent>(evt => _isDraggingSheet = false);
                handle.RegisterCallback<PointerLeaveEvent>(evt => _isDraggingSheet = false);
                handle.RegisterCallback<PointerMoveEvent>(evt => 
                {
                    if (_isDraggingSheet && (evt.position.y - _dragStartY > 50)) 
                    {
                        SetSheetState(false);
                        _isDraggingSheet = false;
                    }
                });
            }

            // Block Camera Zoom on Scroll
            var sheetScroll = root.Q<ScrollView>(className: "sheet-scroll");
            if (sheetScroll != null)
            {
                sheetScroll.RegisterCallback<PointerEnterEvent>(evt => OrbitCameraController.GlobalInputBlocked = true);
                sheetScroll.RegisterCallback<PointerLeaveEvent>(evt => OrbitCameraController.GlobalInputBlocked = false);
            }

            // ── Hero Menu ──
            // ── Hero Menu ──
            var heroExploreBtn = root.Q<Button>("HeroExploreBtn");
            var heroDeviceBtn = root.Q<Button>("HeroDeviceBtn");
            var heroAboutBtn = root.Q<Button>("HeroAboutBtn");
            var heroExitBtn = root.Q<Button>("HeroExitBtn");
            var sheetCloseBtn = root.Q<Button>("SheetCloseBtn");
            
            _heroContainer = root.Q<VisualElement>("HeroContainer");
            heroMain = root.Q<VisualElement>("HeroMain");
            submenuDevices = root.Q<VisualElement>("HeroSubmenu_Devices");
            submenuAbout = root.Q<VisualElement>("HeroSubmenu_About");
            submenuExit = root.Q<VisualElement>("HeroSubmenu_Exit");

            if (heroExploreBtn != null) heroExploreBtn.clicked += () =>
            {
                DismissHero();
                if (AppStateMachine.Instance != null) AppStateMachine.Instance.EnterExploration();
            };
            
            // Hero Submenu Navigation
            if (heroDeviceBtn != null) heroDeviceBtn.clicked += () => OpenHeroSubmenu(SubmenuType.Devices);
            if (heroAboutBtn != null) heroAboutBtn.clicked += () => OpenHeroSubmenu(SubmenuType.About);
            if (heroExitBtn != null) heroExitBtn.clicked += () => OpenHeroSubmenu(SubmenuType.Exit);
            
            // Submenu Back Buttons
            var backDevices = root.Q<Button>("SubmenuBackBtn_Devices");
            var backAbout = root.Q<Button>("SubmenuBackBtn_About");
            var backExit = root.Q<Button>("SubmenuBackBtn_Exit");
            
            if (backDevices != null) backDevices.clicked += CloseHeroSubmenu;
            if (backAbout != null) backAbout.clicked += CloseHeroSubmenu;
            if (backExit != null) backExit.clicked += CloseHeroSubmenu;

            // Sheet Close (For Details Sheet) — StopPropagation prevents header toggle re-opening
            if (sheetCloseBtn != null)
            {
                sheetCloseBtn.RegisterCallback<ClickEvent>(evt =>
                {
                    evt.StopPropagation();
                    SetSheetState(false);
                });
            }

            // Exit Confirmation Actions (Inside Submenu)
            var exitConfirmBtn = root.Q<Button>("ExitConfirmBtn");
            var exitCancelBtn = root.Q<Button>("ExitCancelBtn");
            
            if (exitCancelBtn != null) exitCancelBtn.clicked += CloseHeroSubmenu; // Cancel returns to Hero Main
            if (exitConfirmBtn != null) exitConfirmBtn.clicked += () => 
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                Application.ExternalEval("window.history.back()");
#else
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
            };
 
            // ── Home Button (Return to Hero) ──
            var homeBtn = root.Q<Button>("HomeBtn");
            if (homeBtn != null) homeBtn.clicked += ReturnToHero;

            // Old About logic removed

            // Initial State
            UpdatePartIndicator(null);
            if (infoBtn != null) infoBtn.SetEnabled(false);
            
            // Slider ALWAYS starts hidden — only shown when ExplodeBtn is clicked
            if (sliderContainer != null)
                sliderContainer.AddToClassList("slider-hidden");

            // ── Auto-create managers if missing ──
            EnsureManagers();

            // Hotspots are NOT initialized here — gated by _heroDismissed.

            // Ensure all buttons block 3D input (Fixes Info button deselecting piece)
            RegisterButtonInputBlockers();

            // Ensure layout is correct from frame 0
            RepositionPopups();
        }

        /// <summary>Hide the hero screen and enable hotspots</summary>
        private void DismissHero()
        {
            _heroDismissed = true;
            if (_heroContainer != null)
            {
                _heroContainer.AddToClassList("hero--hidden");
                // CRITICAL: Set picking mode to Ignore AND display to None
                // so the hero container doesn't block clicks to the 3D scene
                _heroContainer.pickingMode = PickingMode.Ignore;
                _heroContainer.style.display = DisplayStyle.None;
            }

            // Initialize hotspots now
            if (!_hotspotsInitialized && HotspotManager.Instance != null)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance.Initialize(root);
            }
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.SetVisible(true);
        }

        /// <summary>Return to the hero screen from 3D viewer</summary>
        private void ReturnToHero()
        {
            _heroDismissed = false;

            // Close any open panels/menus first
            SetSheetState(false);
            if (shaderMenu != null) { shaderMenu.AddToClassList("shader-menu--hidden"); _shaderMenuShown = false; }
            if (categoryMenu != null) categoryMenu.AddToClassList("category-menu--hidden");
            if (envPanel != null) envPanel.AddToClassList("env-panel--hidden");
            if (sliderContainer != null) sliderContainer.AddToClassList("slider-hidden");

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

            // Re-show hero
            // Re-show hero
            if (_heroContainer != null)
            {
                _heroContainer.RemoveFromClassList("hero--hidden");
                _heroContainer.pickingMode = PickingMode.Position;
                _heroContainer.style.display = DisplayStyle.Flex;
                
                // Ensure we start at main menu, not a submenu
                CloseHeroSubmenu();
            }
        }

        /// <summary>Ensure ViewModeManager and EnvironmentController exist</summary>
        private void EnsureManagers()
        {
            GameObject managers = GameObject.Find("Managers");
            if (managers == null) managers = new GameObject("Managers");

            if (HotspotManager.Instance == null)
                managers.AddComponent<HotspotManager>();

            if (ViewModeManager.Instance == null)
                managers.AddComponent<ViewModeManager>();

            if (EnvironmentController.Instance == null)
                managers.AddComponent<EnvironmentController>();
        }

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

        #region Shader System

        /// <summary> Toggle shader selection menu </summary>
        private void ToggleShaderMenu()
        {
            if (shaderMenu == null) return;
            _shaderMenuShown = !_shaderMenuShown;
            
            if (_shaderMenuShown)
            {
                shaderMenu.BringToFront();
                // Close other menus
                if (envPanel != null) envPanel.AddToClassList("submenu--hidden");
                if (categoryMenu != null) categoryMenu.AddToClassList("submenu--hidden");
            }

            shaderMenu.EnableInClassList("submenu--hidden", !_shaderMenuShown);
            RepositionPopups();
        }

        /// <summary> Reacts to ViewModeManager changes to update background </summary>
        private void OnViewModeChanged(ViewMode newMode)
        {
            if (_uiAnalyzePanel != null) _uiAnalyzePanel.OnViewModeChanged(newMode);
        }

        #endregion

        #region Environment Panel

        private void ToggleEnvPanel()
        {
            if (envPanel == null) return;
            envPanel.ToggleInClassList("submenu--hidden");
            
            if (!envPanel.ClassListContains("submenu--hidden"))
            {
                envPanel.BringToFront();
                // Close other menus
                if (shaderMenu != null)
                {
                    shaderMenu.AddToClassList("submenu--hidden");
                    _shaderMenuShown = false;
                }
                if (categoryMenu != null) categoryMenu.AddToClassList("submenu--hidden");
            }
            RepositionPopups();
        }

        #endregion

        #region Interaction Handlers

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

        private void OnInfoToggle()
        {
            if (isSheetOpen) SetSheetState(false);
            else OpenSheet(SheetMode.Details);
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

        private void SetSheetState(bool isOpen)
        {
            isSheetOpen = isOpen;
            if (detailsSheet != null)
            {
                if (isOpen)
                {
                    detailsSheet.RemoveFromClassList("details-sheet--hidden");
                    detailsSheet.pickingMode = PickingMode.Position;
                }
                else
                {
                    detailsSheet.AddToClassList("details-sheet--hidden");
                    detailsSheet.pickingMode = PickingMode.Ignore;
                }
            }

            // Move buttons up/down with the sheet
            if (bottomBar != null) bottomBar.EnableInClassList("ui-shifted", isOpen);

            if (partNameLabel != null)
                partNameLabel.EnableInClassList("selection-label--hidden", isOpen);

            if (OrbitCameraController.Instance != null)
                OrbitCameraController.Instance.SetViewportShift(isOpen ? 0.15f : 0f);

            // When info sheet OPENS: hide slider and close ALL submenus
            // When info sheet OPENS: hide slider and close ALL submenus
            if (isOpen)
            {
                // Hide exploded view slider
                if (sliderContainer != null)
                    sliderContainer.AddToClassList("slider-hidden");

                // Close shader menu
                if (shaderMenu != null)
                {
                    shaderMenu.AddToClassList("submenu--hidden");
                    _shaderMenuShown = false;
                }

                // Close category menu
                if (categoryMenu != null)
                    categoryMenu.AddToClassList("submenu--hidden");

                // Close environment panel
                if (envPanel != null)
                    envPanel.AddToClassList("submenu--hidden");
            }

            // Reposition all popups (shifted = +200px when sheet is open)
            RepositionPopups();
        }

        private void ToggleCategoryMenu()
        {
            if (categoryMenu != null)
            {
                categoryMenu.ToggleInClassList("submenu--hidden");
                
                if (!categoryMenu.ClassListContains("submenu--hidden")) 
                {
                    categoryMenu.BringToFront();
                    // Close other menus
                    if (shaderMenu != null) { shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
                    if (envPanel != null) envPanel.AddToClassList("submenu--hidden");
                }
                RepositionPopups();
            }
        }

        private void CloseAllMenus()
        {
            if (shaderMenu != null) { shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
            if (categoryMenu != null) categoryMenu.AddToClassList("submenu--hidden");
            if (envPanel != null) envPanel.AddToClassList("submenu--hidden");
            if (sliderContainer != null) sliderContainer.AddToClassList("slider-hidden");
            RepositionPopups();
        }

        // ══════════════════════════════════════════════════════
        //  POPUP STACKING — Dynamic repositioning for multi-menu
        //  
        //  MATH:
        //    Base = POPUP_BASE_BOTTOM (184px) from screen bottom
        //    Each visible popup stacks above the previous one:
        //      popup[0].bottom = base + sheetOffset
        //      popup[1].bottom = popup[0].bottom + popup[0].height + POPUP_GAP
        //      etc.
        //    When info sheet is open, sheetOffset = SHEET_SHIFT (200px)
        // ══════════════════════════════════════════════════════
        private float _dragStartY;
        private bool _isDraggingSheet;

        private void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.ToggleVisibility();
            
            if (hotspotBtn != null) 
                hotspotBtn.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

        private void RepositionPopups()
        {
            // Dynamic shift based on screen height (matches CSS .ui-shifted bottom: 56%)
            float rootHeight = root != null ? root.layout.height : 0f;
            if (float.IsNaN(rootHeight) || rootHeight < 1f) rootHeight = Screen.height;
            
            // Base calculation:
            // Closed: Start at 192px.
            // Open: Start at (SheetHeight 56%) + 192px (to clear the buttons which are now at 56%).
            float currentBottom = isSheetOpen ? (rootHeight * 0.56f) + POPUP_BASE_BOTTOM : POPUP_BASE_BOTTOM;

            // Ordered list of popups — bottom to top stacking priority.
            // Each entry: (element, hiddenClass, estimatedHeight).
            var popups = new (UnityEngine.UIElements.VisualElement el, string hiddenClass, float height)[]
            {
                (sliderContainer, "slider-hidden",        56f),   // label + slider
                (categoryMenu,    "submenu--hidden",      192f),  // increased height for grid
                (shaderMenu,      "submenu--hidden",      192f),  // increased height for grid
                (envPanel,        "submenu--hidden",      220f),  // presets + 2 sliders
            };


            bool anyMenuVisible = false;

            foreach (var (el, hiddenClass, height) in popups)
            {
                if (el == null) continue;

                bool isVisible = !el.ClassListContains(hiddenClass);
                if (isVisible)
                {
                    el.style.bottom = new UnityEngine.UIElements.StyleLength(currentBottom);
                    currentBottom += height + POPUP_GAP;
                    
                    // Track visibility for PopupBlocker (EXCLUDE slider per User Req 2: "I cant click any of the parts")
                    // PRO: Allows partial interaction. CON: "Click outside" only works if we handle it elsewhere (e.g. OnPartSelected).
                    if (el != sliderContainer)
                        anyMenuVisible = true;
                }
            }

            if (popupBlocker != null)
                popupBlocker.EnableInClassList("popup-blocker--hidden", !anyMenuVisible);
        }

        private void SetCategoryFilter(string category, Button clickedBtn)
        {
            if (category == "ALL")
            {
                activeCategories.Clear();
                activeCategories.Add("ALL");
            }
            else
            {
                if (activeCategories.Contains("ALL"))
                    activeCategories.Remove("ALL");

                if (activeCategories.Contains(category))
                    activeCategories.Remove(category);
                else
                    activeCategories.Add(category);

                if (activeCategories.Count == 0)
                    activeCategories.Add("ALL");
            }

            if (ExplodedViewManager.Instance != null)
                ExplodedViewManager.Instance.SetCategoryFilters(activeCategories);

            if (categoryMenu != null)
            {
                void UpdateButtonState(string btnName, string catName)
                {
                    var btn = categoryMenu.Q<Button>(btnName);
                    if (btn != null)
                        btn.EnableInClassList("submenu-card--active", activeCategories.Contains(catName));
                }

                UpdateButtonState("CatBtn_All", "ALL");
                UpdateButtonState("CatBtn_Structure", "Structure");
                UpdateButtonState("CatBtn_Propulsion", "Propulsion");
                UpdateButtonState("CatBtn_Avionics", "Avionics");
                UpdateButtonState("CatBtn_Power", "Power");
            }
        }

        private void OnExplosionSliderChanged(ChangeEvent<float> evt)
        {
            if (ExplodedViewManager.Instance != null)
                ExplodedViewManager.Instance.SetExplosionFactor(evt.newValue);
        }
        
        private void OnAppStateChanged(AppStateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            bool isInteractive = evt.NewState == AppState.Exploration || isExploded;

            // ── Hotspots: only after hero is dismissed ──
            if (_heroDismissed && isInteractive && !_hotspotsInitialized)
            {
                _hotspotsInitialized = true;
                if (HotspotManager.Instance != null)
                    HotspotManager.Instance.Initialize(root);
            }
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.SetVisible(_heroDismissed && isInteractive);

            // ── Slider ──
            if (sliderContainer != null)
            {
                if (isExploded)
                {
                    sliderContainer.RemoveFromClassList("slider-hidden");
                    sliderContainer.BringToFront();

                    if (explosionSlider != null)
                        explosionSlider.SetValueWithoutNotify(0.5f);
                }
                else
                {
                    sliderContainer.AddToClassList("slider-hidden");
                }
            }

            // Reposition all visible popups after slider visibility change
            RepositionPopups();
        }

        #endregion

        #region Event Callbacks

        private void OnPartSelected(PartSelectedEvent evt)
    {
        if (partNameLabel == null) return;
        
        // User Interaction: Selecting a part should close menus/sliders (User Req 2)
        CloseAllMenus();

        UpdatePartIndicator(evt.PartData);

        // Check PartData first as it contains the source of truth
        if (evt.PartData != null && !string.IsNullOrEmpty(evt.PartData.PartName) && evt.PartData.PartName != "NULL")
        {
            if (infoBtn != null) infoBtn.SetEnabled(true);
            
            // Populate all fields
            if (sheetTitle != null) sheetTitle.text = evt.PartData.PartName.ToUpper();
            if (sheetCategory != null) sheetCategory.text = evt.PartData.Category;
            if (sheetFunction != null) sheetFunction.text = evt.PartData.Function;
            if (sheetMaterial != null) sheetMaterial.text = evt.PartData.MaterialType;
            if (sheetDesc != null) sheetDesc.text = evt.PartData.Description;
            if (sheetWeight != null) sheetWeight.text = $"{evt.PartData.Weight:F2} kg";
            if (sheetDimensions != null) sheetDimensions.text = evt.PartData.Dimensions;
            if (sheetPower != null) sheetPower.text = evt.PartData.powerConsumption > 0 ? $"{evt.PartData.powerConsumption:F1} W" : "N/A";
            if (sheetTemp != null) sheetTemp.text = evt.PartData.operatingTemp > 0 ? $"{evt.PartData.operatingTemp:F0}°C" : "N/A";

            // Assembly info
            if (sheetDifficulty != null)
            {
                int d = Mathf.Clamp(evt.PartData.difficultyLevel, 0, 5);
                sheetDifficulty.text = new string('★', d) + new string('☆', 5 - d);
            }
            if (sheetTools != null)
            {
                sheetTools.text = (evt.PartData.requiredTools != null && evt.PartData.requiredTools.Length > 0)
                    ? string.Join(", ", evt.PartData.requiredTools)
                    : "None";
            }
            if (sheetAssemblyTime != null)
                sheetAssemblyTime.text = evt.PartData.installationTimeMinutes > 0 ? $"~{evt.PartData.installationTimeMinutes:F0} min" : "N/A";

            // Auto-open info sheet ONLY when triggered by a hotspot click
            if (evt.FromHotspot)
                OpenSheet(SheetMode.Details);
        }
        else
        {
            // Deselection
            partNameLabel.AddToClassList("selection-label--hidden");

            if (infoBtn != null) infoBtn.SetEnabled(false);
            SetSheetState(false);
        }
    }

        private void UpdatePartIndicator(WebGL.Core.Data.DronePartData data)
        {
            if (partNameLabel == null) return;

            if (data != null)
            {
                partNameLabel.text = data.PartName.ToUpper();
                partNameLabel.style.color = new StyleColor(new Color(0.06f, 0.73f, 0.5f));
            }
            else
            {
                partNameLabel.text = "SELECT A PART";
                partNameLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.7f));
            }
        }

        private EventCallback<PointerEnterEvent> _onBtnEnter = evt => OrbitCameraController.GlobalInputBlocked = true;
        private EventCallback<PointerLeaveEvent> _onBtnLeave = evt => OrbitCameraController.GlobalInputBlocked = false;
        private EventCallback<PointerDownEvent> _onBtnDown = evt => evt.StopPropagation();
        private EventCallback<PointerUpEvent> _onBtnUp = evt => evt.StopPropagation();

        private void RegisterButtonInputBlockers()
        {
            root.Query<Button>().ForEach(btn => 
            {
                btn.RegisterCallback(_onBtnEnter);
                btn.RegisterCallback(_onBtnLeave);
                btn.RegisterCallback(_onBtnDown);
                btn.RegisterCallback(_onBtnUp);
                
                AddCleanup(() => {
                    btn.UnregisterCallback(_onBtnEnter);
                    btn.UnregisterCallback(_onBtnLeave);
                    btn.UnregisterCallback(_onBtnDown);
                    btn.UnregisterCallback(_onBtnUp);
                });
            });
        }

        public void OpenHeroSubmenu(SubmenuType type)
        {
            // Toggle Hero Main Content OFF
            if (heroMain != null) heroMain.style.display = DisplayStyle.None;

            // Reset Submenus
            if (submenuDevices != null) submenuDevices.RemoveFromClassList("hero-submenu--active");
            if (submenuAbout != null) submenuAbout.RemoveFromClassList("hero-submenu--active");
            if (submenuExit != null) submenuExit.RemoveFromClassList("hero-submenu--active");

            // Activate Target
            switch (type)
            {
                case SubmenuType.Devices:
                    if (submenuDevices != null) submenuDevices.AddToClassList("hero-submenu--active");
                    break;
                case SubmenuType.About:
                    if (submenuAbout != null) submenuAbout.AddToClassList("hero-submenu--active");
                    break;
                case SubmenuType.Exit:
                    if (submenuExit != null) submenuExit.AddToClassList("hero-submenu--active");
                    break;
            }
        }

        public void CloseHeroSubmenu()
        {
            // Reset Submenus
            if (submenuDevices != null) submenuDevices.RemoveFromClassList("hero-submenu--active");
            if (submenuAbout != null) submenuAbout.RemoveFromClassList("hero-submenu--active");
            if (submenuExit != null) submenuExit.RemoveFromClassList("hero-submenu--active");

            // Restore Hero Main
            if (heroMain != null) heroMain.style.display = DisplayStyle.Flex;
        }

        public void OpenSheet(SheetMode mode)
        {
            DismissHero(); // Ensure hero is dismissed (keeps top bar)

            // Verify containers exist
            if (contentDetails == null) return;

            // Only Details currently supported in Sheet
            contentDetails.RemoveFromClassList("sheet-content--hidden");
            contentDetails.AddToClassList("sheet-content--active");

             // Title checks
            string titleText = (SelectionManager.Instance != null && SelectionManager.Instance.HasSelection) 
                ? (sheetTitle != null ? sheetTitle.text : "PART DETAILS")
                : "SELECT A PART"; 

            if (sheetTitle != null) sheetTitle.text = titleText;
            
            currentSheetMode = mode;
            SetSheetState(true);
        }

        #endregion
    }
}
