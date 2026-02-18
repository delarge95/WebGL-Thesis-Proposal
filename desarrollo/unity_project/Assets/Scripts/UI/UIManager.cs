using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
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
        private Label partNameLabel;
        private Label sheetTitle;
        private TextField sheetCategory;
        private TextField sheetFunction;
        private TextField sheetMaterial;
        private TextField sheetDesc;
        private TextField sheetWeight;
        private TextField sheetDimensions;
        private TextField sheetPower;
        private TextField sheetTemp;
        private TextField sheetDifficulty;
        private TextField sheetTools;
        private TextField sheetAssemblyTime;

        // Buttons
        private Button shaderBtn;
        private Button explodeBtn;
        private Button infoBtn;
        private Button resetBtn;
        private Button envBtn;

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

        // Sheet Content Containers
        private VisualElement contentDetails;
        private VisualElement contentDevices;
        private VisualElement contentAbout;
        private VisualElement contentExit;

        public enum SheetMode { Details, Devices, About, Exit }
        private SheetMode currentSheetMode = SheetMode.Details;

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
        }



        private void InitializeUI()
        {
            if (root == null) return;

            // Bind Elements
            detailsSheet = root.Q<VisualElement>("BottomSheet");
            bottomBar = root.Q<VisualElement>("BottomBar");
            
            // Bind Content Containers
            contentDetails = root.Q<VisualElement>("SheetContent_Details");
            contentDevices = root.Q<VisualElement>("SheetContent_Devices");
            contentAbout = root.Q<VisualElement>("SheetContent_About");
            contentExit = root.Q<VisualElement>("SheetContent_Exit");

            sliderContainer = root.Q<VisualElement>("SliderContainer");
            
            if (detailsSheet != null)
            {
                // Prevent clicks from passing through to 3D scene or other UI
                detailsSheet.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
                detailsSheet.RegisterCallback<PointerUpEvent>(evt => evt.StopPropagation());
                
                // Block 3D input when hovering ANY part of the sheet
                detailsSheet.RegisterCallback<PointerEnterEvent>(evt => OrbitCameraController.GlobalInputBlocked = true);
                detailsSheet.RegisterCallback<PointerLeaveEvent>(evt => OrbitCameraController.GlobalInputBlocked = false);
            }
            
            partNameLabel = root.Q<Label>("SelectionIndicator");
            
            sheetTitle = root.Q<Label>("SheetTitle");
            sheetCategory = root.Q<TextField>("PartCategory");
            sheetFunction = root.Q<TextField>("PartFunction");
            sheetMaterial = root.Q<TextField>("PartMaterial");
            sheetDesc = root.Q<TextField>("PartDescription");
            sheetWeight = root.Q<TextField>("PartWeight");
            sheetDimensions = root.Q<TextField>("PartDimensions");
            sheetPower = root.Q<TextField>("PartPower");
            sheetTemp = root.Q<TextField>("PartTemp");
            sheetDifficulty = root.Q<TextField>("PartDifficulty");
            sheetTools = root.Q<TextField>("PartTools");
            sheetAssemblyTime = root.Q<TextField>("PartAssemblyTime");

            shaderBtn = root.Q<Button>("ShaderBtn");
            explodeBtn = root.Q<Button>("ExplodeBtn");
            infoBtn = root.Q<Button>("InfoBtn");
            resetBtn = root.Q<Button>("ResetViewBtn");
            envBtn = root.Q<Button>("EnvBtn");

            explosionSlider = root.Q<Slider>("ExplosionSlider");

            var hotspotBtn = root.Q<Button>("HotspotBtn");
            if (hotspotBtn != null) hotspotBtn.clicked += ToggleHotspots;

            // ── Shader Button: Single Click Toggle ──
            if (shaderBtn != null) shaderBtn.clicked += ToggleShaderMenu;

            if (explodeBtn != null) explodeBtn.clicked += OnExplodeToggle;
            if (infoBtn != null) infoBtn.clicked += OnInfoToggle;
            if (resetBtn != null) resetBtn.clicked += OnResetClicked;

            // ── Shader Menu ──
            shaderMenu = root.Q<VisualElement>("ShaderMenu");
            BindShaderMenuButtons();

            // ── Environment Panel ──
            envPanel = root.Q<VisualElement>("EnvPanel");
            if (envBtn != null) envBtn.clicked += ToggleEnvPanel;
            BindEnvPanel();

            // ── Category Menu ──
            categoryMenu = root.Q<VisualElement>("CategoryMenu");
            var btnAll = root.Q<Button>("CatBtn_All");
            var btnStructure = root.Q<Button>("CatBtn_Structure");
            var btnPropulsion = root.Q<Button>("CatBtn_Propulsion");
            var btnAvionics = root.Q<Button>("CatBtn_Avionics");
            var btnPower = root.Q<Button>("CatBtn_Power");

            var layerBtn = root.Q<Button>("LayerBtn");
            if (layerBtn != null) layerBtn.clicked += ToggleCategoryMenu;

            if (btnAll != null) btnAll.clicked += () => SetCategoryFilter("ALL", btnAll);
            if (btnStructure != null) btnStructure.clicked += () => SetCategoryFilter("Structure", btnStructure);
            if (btnPropulsion != null) btnPropulsion.clicked += () => SetCategoryFilter("Propulsion", btnPropulsion);
            if (btnAvionics != null) btnAvionics.clicked += () => SetCategoryFilter("Avionics", btnAvionics);
            if (btnPower != null) btnPower.clicked += () => SetCategoryFilter("Power", btnPower);

            if (explosionSlider != null) explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);

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
            var heroExploreBtn = root.Q<Button>("HeroExploreBtn");
            var heroDeviceBtn = root.Q<Button>("HeroDeviceBtn");
            var heroAboutBtn = root.Q<Button>("HeroAboutBtn");
            var heroExitBtn = root.Q<Button>("HeroExitBtn");
            var sheetCloseBtn = root.Q<Button>("SheetCloseBtn");
            
            _heroContainer = root.Q<VisualElement>("HeroContainer");

            if (heroExploreBtn != null) heroExploreBtn.clicked += () =>
            {
                DismissHero();
                if (AppStateMachine.Instance != null) AppStateMachine.Instance.EnterExploration();
            };
            
            // Unified Bottom Sheet Navigation
            if (heroDeviceBtn != null) heroDeviceBtn.clicked += () => OpenSheet(SheetMode.Devices);
            if (heroAboutBtn != null) heroAboutBtn.clicked += () => OpenSheet(SheetMode.About);
            if (heroExitBtn != null) heroExitBtn.clicked += () => OpenSheet(SheetMode.Exit);
            
            // Sheet Close
            if (sheetCloseBtn != null) sheetCloseBtn.clicked += () => SetSheetState(false);

            // Exit Confirmation
            var exitConfirmBtn = root.Q<Button>("ExitConfirmBtn");
            var exitCancelBtn = root.Q<Button>("ExitCancelBtn");
            
            if (exitCancelBtn != null) exitCancelBtn.clicked += () => SetSheetState(false);
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
            if (_heroContainer != null)
            {
                _heroContainer.RemoveFromClassList("hero--hidden");
                _heroContainer.pickingMode = PickingMode.Position;
                _heroContainer.style.display = DisplayStyle.Flex;
                
                // Hide device selector when returning
                var deviceSelector = _heroContainer.Q<VisualElement>("DeviceSelector");
                if (deviceSelector != null) deviceSelector.AddToClassList("device-selector--hidden");
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
                // Close env panel to prevent 4-deep stacking overlap
                if (envPanel != null) envPanel.AddToClassList("env-panel--hidden");
            }

            shaderMenu.EnableInClassList("shader-menu--hidden", !_shaderMenuShown);
            RepositionPopups();
        }

        private void BindShaderMenuButtons()
        {
            if (shaderMenu == null) return;

            var modes = new[] { "Realistic", "XRay", "Blueprint", "SolidColor", "Wireframe", "Ghosted", "Thermal" };
            var enums = new[] { ViewMode.Realistic, ViewMode.XRay, ViewMode.Blueprint, ViewMode.SolidColor, ViewMode.Wireframe, ViewMode.Ghosted, ViewMode.Thermal };

            for (int i = 0; i < modes.Length; i++)
            {
                var btn = shaderMenu.Q<Button>($"ShaderMode_{modes[i]}");
                if (btn == null) continue;
                var mode = enums[i]; // capture for closure
                btn.clicked += () =>
                {
                    if (ViewModeManager.Instance != null)
                        ViewModeManager.Instance.SetViewMode(mode);
                    UpdateShaderButtonVisual();
                    UpdateShaderMenuActiveState(mode);
                    RepositionPopups(); // Ensure layout stays correct (though menu didn't move)
                };
            }
        }

        private void UpdateShaderButtonVisual()
        {
            if (shaderBtn == null || ViewModeManager.Instance == null) return;
            var mode = ViewModeManager.Instance.CurrentMode;
            bool isActive = mode != ViewMode.Realistic;
            shaderBtn.EnableInClassList("btn-tag--active", isActive);
            shaderBtn.tooltip = isActive ? $"View: {mode}" : "Toggle Render Mode";
        }

        private void UpdateShaderMenuActiveState(ViewMode activeMode)
        {
            if (shaderMenu == null) return;
            var modes = new[] { "Realistic", "XRay", "Blueprint", "SolidColor", "Wireframe", "Ghosted", "Thermal" };
            var enums = new[] { ViewMode.Realistic, ViewMode.XRay, ViewMode.Blueprint, ViewMode.SolidColor, ViewMode.Wireframe, ViewMode.Ghosted, ViewMode.Thermal };
            for (int i = 0; i < modes.Length; i++)
            {
                var btn = shaderMenu.Q<Button>($"ShaderMode_{modes[i]}");
                if (btn != null) btn.EnableInClassList("shader-mode-btn--active", enums[i] == activeMode);
            }
        }

        /// <summary> Reacts to ViewModeManager changes to update background </summary>
        private void OnViewModeChanged(ViewMode newMode)
        {
            UpdateShaderButtonVisual();
            UpdateShaderMenuActiveState(newMode);
            ApplyBackgroundForMode(newMode);
        }

        private void ApplyBackgroundForMode(ViewMode mode)
        {
            Color bg;
            switch (mode)
            {
                case ViewMode.XRay:       bg = new Color(0.02f, 0.03f, 0.07f); break;
                case ViewMode.Blueprint:  bg = new Color(0.04f, 0.08f, 0.18f); break;
                case ViewMode.SolidColor: bg = new Color(0.08f, 0.08f, 0.10f); break;
                case ViewMode.Wireframe:  bg = new Color(0.03f, 0.03f, 0.05f); break;
                case ViewMode.Ghosted:    bg = new Color(0.05f, 0.05f, 0.07f); break;
                case ViewMode.Thermal:    bg = new Color(0.02f, 0.02f, 0.04f); break;
                default:                  bg = new Color(0.04f, 0.055f, 0.11f); break;
            }

            // ONLY set camera background — DO NOT set root.style.backgroundColor!
            // Setting root background paints an opaque layer that covers the 3D viewport.
            if (Camera.main != null) Camera.main.backgroundColor = bg;
        }

        #endregion

        #region Environment Panel

        private void ToggleEnvPanel()
        {
            if (envPanel == null) return;
            envPanel.ToggleInClassList("env-panel--hidden");
            if (!envPanel.ClassListContains("env-panel--hidden"))
            {
                envPanel.BringToFront();
                // Close shader menu to prevent 4-deep stacking overlap
                if (shaderMenu != null)
                {
                    shaderMenu.AddToClassList("shader-menu--hidden");
                    _shaderMenuShown = false;
                }
            }
            RepositionPopups();
        }

        private void BindEnvPanel()
        {
            if (envPanel == null) return;

            envLightRotSlider = envPanel.Q<Slider>("EnvLightRotation");
            envLightIntSlider = envPanel.Q<Slider>("EnvLightIntensity");

            if (envLightRotSlider != null)
            {
                envLightRotSlider.RegisterValueChangedCallback(evt =>
                {
                    if (EnvironmentController.Instance != null)
                        EnvironmentController.Instance.SetLightRotation(evt.newValue);
                });
            }
            if (envLightIntSlider != null)
            {
                envLightIntSlider.RegisterValueChangedCallback(evt =>
                {
                    if (EnvironmentController.Instance != null)
                        EnvironmentController.Instance.SetLightIntensity(evt.newValue);
                });
            }

            // Preset buttons
            var presets = new[] { "Studio", "Sunset", "Night", "Blueprint", "Neutral" };
            foreach (var preset in presets)
            {
                var btn = envPanel.Q<Button>($"EnvPreset_{preset}");
                if (btn == null) continue;
                var p = preset; // capture
                btn.clicked += () =>
                {
                    if (EnvironmentController.Instance != null)
                        EnvironmentController.Instance.ApplyPreset(p);
                    UpdateEnvPresetActiveState(p);
                };
            }
        }

        private void UpdateEnvPresetActiveState(string activePreset)
        {
            if (envPanel == null) return;
            var presets = new[] { "Studio", "Sunset", "Night", "Blueprint", "Neutral" };
            foreach (var p in presets)
            {
                var btn = envPanel.Q<Button>($"EnvPreset_{p}");
                if (btn != null) btn.EnableInClassList("env-preset-btn--active", p == activePreset);
            }
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
            if (isOpen)
            {
                // Hide exploded view slider
                if (sliderContainer != null)
                    sliderContainer.AddToClassList("slider-hidden");

                // Close shader menu
                if (shaderMenu != null)
                {
                    shaderMenu.AddToClassList("shader-menu--hidden");
                    _shaderMenuShown = false;
                }

                // Close category menu
                if (categoryMenu != null)
                    categoryMenu.AddToClassList("category-menu--hidden");

                // Close environment panel
                if (envPanel != null)
                    envPanel.AddToClassList("env-panel--hidden");
            }

            // Reposition all popups (shifted = +200px when sheet is open)
            RepositionPopups();
        }

        private void ToggleCategoryMenu()
        {
            if (categoryMenu != null)
            {
                categoryMenu.ToggleInClassList("category-menu--hidden");
                if (!categoryMenu.ClassListContains("category-menu--hidden")) categoryMenu.BringToFront();
                RepositionPopups();
            }
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
            if (HotspotManager.Instance != null)
                HotspotManager.Instance.ToggleVisibility();
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
            // Heights are measured from CSS: padding + content.
            var popups = new (UnityEngine.UIElements.VisualElement el, string hiddenClass, float height)[]
            {
                (sliderContainer, "slider-hidden",        56f),   // label + slider
                (categoryMenu,    "category-menu--hidden", 64f),   // pill bar row
                (shaderMenu,      "shader-menu--hidden",   100f),  // Reduced height estimate (fixes Env panel jumping too high)
                (envPanel,        "env-panel--hidden",     220f),  // presets + 2 sliders
            };

            foreach (var (el, hiddenClass, height) in popups)
            {
                if (el == null) continue;

                bool isVisible = !el.ClassListContains(hiddenClass);
                if (isVisible)
                {
                    el.style.bottom = new UnityEngine.UIElements.StyleLength(currentBottom);
                    currentBottom += height + POPUP_GAP;
                }
            }
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
                        btn.EnableInClassList("btn-tag--active", activeCategories.Contains(catName));
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
            UpdatePartIndicator(evt.PartData);

            if (evt.PartData != null)
            {
                if (infoBtn != null) infoBtn.SetEnabled(true);

                // Populate all fields
                if (sheetTitle != null) sheetTitle.text = evt.PartData.PartName.ToUpper();
                if (sheetCategory != null) sheetCategory.value = evt.PartData.Category;
                if (sheetFunction != null) sheetFunction.value = evt.PartData.Function;
                if (sheetMaterial != null) sheetMaterial.value = evt.PartData.MaterialType;
                if (sheetDesc != null) sheetDesc.value = evt.PartData.Description;
                if (sheetWeight != null) sheetWeight.value = $"{evt.PartData.Weight:F2} kg";
                if (sheetDimensions != null) sheetDimensions.value = evt.PartData.Dimensions;
                if (sheetPower != null) sheetPower.value = evt.PartData.powerConsumption > 0 ? $"{evt.PartData.powerConsumption:F1} W" : "N/A";
                if (sheetTemp != null) sheetTemp.value = evt.PartData.operatingTemp > 0 ? $"{evt.PartData.operatingTemp:F0}°C" : "N/A";

                // Assembly info
                if (sheetDifficulty != null)
                {
                    int d = Mathf.Clamp(evt.PartData.difficultyLevel, 0, 5);
                    sheetDifficulty.value = new string('★', d) + new string('☆', 5 - d);
                }
                if (sheetTools != null)
                {
                    sheetTools.value = (evt.PartData.requiredTools != null && evt.PartData.requiredTools.Length > 0)
                        ? string.Join(", ", evt.PartData.requiredTools)
                        : "None";
                }
                if (sheetAssemblyTime != null)
                    sheetAssemblyTime.value = evt.PartData.installationTimeMinutes > 0 ? $"~{evt.PartData.installationTimeMinutes:F0} min" : "N/A";

                // Auto-open info sheet ONLY when triggered by a hotspot click
                if (evt.FromHotspot)
                    OpenSheet(SheetMode.Details);
            }
            else
            {
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

        private void RegisterButtonInputBlockers()
        {
            // Find all buttons in the UI
            root.Query<Button>().ForEach(btn => 
            {
                // When hovering ANY button, block 3D input
                btn.RegisterCallback<PointerEnterEvent>(evt => OrbitCameraController.GlobalInputBlocked = true);
                btn.RegisterCallback<PointerLeaveEvent>(evt => OrbitCameraController.GlobalInputBlocked = false);
                
                // Stop click propagation to prevent 3D selection/deselection
                btn.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
                btn.RegisterCallback<PointerUpEvent>(evt => evt.StopPropagation());
            });
        }

        public void OpenSheet(SheetMode mode)
        {
            DismissHero(); // Ensure hero is dismissed (keeps top bar)

            // Verify containers exist
            if (contentDetails == null) return;

            // Helper to hide
            void Hide(VisualElement el) 
            {
                if (el == null) return;
                el.AddToClassList("sheet-content--hidden");
                el.RemoveFromClassList("sheet-content--active");
            }
            // Helper to show
            void Show(VisualElement el)
            {
                if (el == null) return;
                el.RemoveFromClassList("sheet-content--hidden");
                el.AddToClassList("sheet-content--active");
            }

            // Reset all
            Hide(contentDetails);
            Hide(contentDevices);
            Hide(contentAbout);
            Hide(contentExit);

            // Set active
            string titleText = "";
            switch (mode)
            {
                case SheetMode.Details:
                    Show(contentDetails);
                    // Title checks
                    titleText = (SelectionManager.Instance != null && SelectionManager.Instance.HasSelection) 
                        ? (sheetTitle != null ? sheetTitle.text : "PART DETAILS")
                        : "SELECT A PART"; 
                    break;
                case SheetMode.Devices:
                    Show(contentDevices);
                    titleText = "SELECT DEVICE";
                    break;
                case SheetMode.About:
                    Show(contentAbout);
                    titleText = "ABOUT";
                    break;
                case SheetMode.Exit:
                    Show(contentExit);
                    titleText = "EXIT APPLICATION";
                    break;
            }

            if (sheetTitle != null) sheetTitle.text = titleText;
            
            currentSheetMode = mode;
            SetSheetState(true);
        }

        #endregion
    }
}
