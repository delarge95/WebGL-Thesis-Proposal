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
        private bool _shaderPointerDown = false;
        private float _shaderDownTime = 0f;
        private const float LONG_PRESS_THRESHOLD = 0.45f;
        private bool _shaderMenuShown = false;
        private bool _longPressTriggered = false;

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

        private void Update()
        {
            // Long-press detection for shader button
            if (_shaderPointerDown && !_longPressTriggered)
            {
                _shaderDownTime += Time.unscaledDeltaTime;
                if (_shaderDownTime >= LONG_PRESS_THRESHOLD)
                {
                    _longPressTriggered = true;
                    ShowShaderMenu();
                }
            }
        }

        private void InitializeUI()
        {
            if (root == null) return;

            // Bind Elements
            detailsSheet = root.Q<VisualElement>("DetailsSheet");
            bottomBar = root.Q<VisualElement>("BottomBar");
            sliderContainer = root.Q<VisualElement>("SliderContainer");
            
            partNameLabel = root.Q<Label>("SelectionIndicator");
            
            sheetTitle = root.Q<Label>("PartName");
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

            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // ── Shader Button: Tap + Long-Press ──
            if (shaderBtn != null)
            {
                // Use TrickleDown to capture events BEFORE the button's internal click handler
                shaderBtn.RegisterCallback<PointerDownEvent>(evt =>
                {
                    _shaderPointerDown = true;
                    _shaderDownTime = 0f;
                    _longPressTriggered = false;
                }, TrickleDown.TrickleDown);
                shaderBtn.RegisterCallback<PointerUpEvent>(evt =>
                {
                    _shaderPointerDown = false;
                    if (_longPressTriggered)
                    {
                        // Long-press already handled — prevent the click
                        evt.StopImmediatePropagation();
                    }
                    else
                    {
                        OnShaderTap();
                    }
                }, TrickleDown.TrickleDown);
                shaderBtn.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    _shaderPointerDown = false;
                });
            }

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
            if (header != null) header.RegisterCallback<ClickEvent>(evt => SetSheetState(!isSheetOpen));

            // ── Hero Menu ──
            var heroExploreBtn = root.Q<Button>("HeroExploreBtn");
            var heroAssemblyBtn = root.Q<Button>("HeroAssemblyBtn");
            var heroAboutBtn = root.Q<Button>("HeroAboutBtn");
            _heroContainer = root.Q<VisualElement>("HeroContainer");

            if (heroExploreBtn != null) heroExploreBtn.clicked += () =>
            {
                DismissHero();
                if (AppStateMachine.Instance != null) AppStateMachine.Instance.EnterExploration();
            };
            if (heroAssemblyBtn != null) heroAssemblyBtn.clicked += () =>
            {
                DismissHero();
                if (AppStateMachine.Instance != null) AppStateMachine.Instance.EnterExplodedView();
            };
            if (heroAboutBtn != null) heroAboutBtn.clicked += () =>
            {
                var aboutPanel = root.Q<VisualElement>("AboutPanel");
                if (aboutPanel != null) aboutPanel.RemoveFromClassList("about-panel--hidden");
            };

            // ── About Close Button ──
            var aboutCloseBtn = root.Q<Button>("AboutCloseBtn");
            if (aboutCloseBtn != null) aboutCloseBtn.clicked += () =>
            {
                var aboutPanel = root.Q<VisualElement>("AboutPanel");
                if (aboutPanel != null) aboutPanel.AddToClassList("about-panel--hidden");
            };

            // Initial State
            UpdatePartIndicator(null);
            if (infoBtn != null) infoBtn.SetEnabled(false);
            
            // Slider ALWAYS starts hidden — only shown when ExplodeBtn is clicked
            if (sliderContainer != null)
                sliderContainer.AddToClassList("slider-hidden");

            // ── Auto-create managers if missing ──
            EnsureManagers();

            // Hotspots are NOT initialized here — gated by _heroDismissed.
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

        /// <summary> Single tap: Toggle Realistic ↔ XRay </summary>
        private void OnShaderTap()
        {
            if (ViewModeManager.Instance == null) return;

            var current = ViewModeManager.Instance.CurrentMode;
            if (current == ViewMode.Realistic)
                ViewModeManager.Instance.SetViewMode(ViewMode.XRay);
            else
                ViewModeManager.Instance.SetViewMode(ViewMode.Realistic);

            UpdateShaderButtonVisual();
        }

        /// <summary> Long press: Show shader selection menu </summary>
        private void ShowShaderMenu()
        {
            if (shaderMenu == null) return;
            _shaderMenuShown = !_shaderMenuShown;
            shaderMenu.EnableInClassList("shader-menu--hidden", !_shaderMenuShown);
            // Explicitly set picking mode to ensure buttons are clickable when menu is visible
            shaderMenu.pickingMode = _shaderMenuShown ? PickingMode.Position : PickingMode.Ignore;
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
                    _shaderMenuShown = false;
                    if (shaderMenu != null) shaderMenu.AddToClassList("shader-menu--hidden");
                    UpdateShaderButtonVisual();
                    UpdateShaderMenuActiveState(mode);
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
            SetSheetState(!isSheetOpen);
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
                    detailsSheet.RemoveFromClassList("details-sheet--hidden");
                else
                    detailsSheet.AddToClassList("details-sheet--hidden");
            }

            if (bottomBar != null) bottomBar.EnableInClassList("ui-shifted", isOpen);
            
            if (sliderContainer != null) 
                sliderContainer.EnableInClassList("ui-shifted", isOpen);

            if (partNameLabel != null)
                partNameLabel.EnableInClassList("selection-label--hidden", isOpen);

            if (OrbitCameraController.Instance != null)
                OrbitCameraController.Instance.SetViewportShift(isOpen ? 0.15f : 0f);
        }

        private void ToggleCategoryMenu()
        {
            if (categoryMenu != null)
            {
                bool isHidden = categoryMenu.ClassListContains("category-menu--hidden");
                categoryMenu.ToggleInClassList("category-menu--hidden");
                
                if (sliderContainer != null)
                    sliderContainer.EnableInClassList("slider-container--shifted-up", isHidden);
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
                    
                    if (categoryMenu != null && !categoryMenu.ClassListContains("category-menu--hidden"))
                        sliderContainer.AddToClassList("slider-container--shifted-up");
                    else
                        sliderContainer.RemoveFromClassList("slider-container--shifted-up");

                    if (explosionSlider != null)
                        explosionSlider.SetValueWithoutNotify(0.5f);
                }
                else
                {
                    sliderContainer.AddToClassList("slider-hidden");
                }
            }
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

                // Data populated — user opens sheet manually via InfoBtn
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

        #endregion
    }
}
