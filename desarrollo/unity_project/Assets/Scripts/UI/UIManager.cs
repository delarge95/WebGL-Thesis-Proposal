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
        private VisualElement categoryMenu; // Added for Step 5.2
        private Label partNameLabel; // In Bottom Bar
        private Label sheetTitle;
        private Label sheetCategory;
        private Label sheetFunction;
        private Label sheetMaterial;
        private Label sheetDesc;

        // Buttons
        private Button shaderBtn;
        private Button explodeBtn;
        private Button infoBtn;
        private Button resetBtn;
        
        // Input Controls
        // Input Controls
        private Slider explosionSlider;

        // State
        private bool isSheetOpen = false;
        private System.Collections.Generic.List<string> activeCategories = new System.Collections.Generic.List<string>() { "ALL" };

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
            detailsSheet = root.Q<VisualElement>("DetailsSheet");
            bottomBar = root.Q<VisualElement>("BottomBar");
            sliderContainer = root.Q<VisualElement>("SliderContainer");
            
            partNameLabel = root.Q<Label>("SelectionIndicator");
            
            sheetTitle = root.Q<Label>("PartName");
            sheetCategory = root.Q<Label>("PartCategory");
            sheetFunction = root.Q<Label>("PartFunction");
            sheetMaterial = root.Q<Label>("PartMaterial");
            sheetDesc = root.Q<Label>("PartDescription");

            shaderBtn = root.Q<Button>("ShaderBtn");
            explodeBtn = root.Q<Button>("ExplodeBtn");
            infoBtn = root.Q<Button>("InfoBtn");
            resetBtn = root.Q<Button>("ResetViewBtn"); // Bound Reset Button
            
            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // Event Listeners
            if (shaderBtn != null) shaderBtn.clicked += OnShaderToggle;
            if (explodeBtn != null) explodeBtn.clicked += OnExplodeToggle;
            if (infoBtn != null) infoBtn.clicked += OnInfoToggle;
            if (resetBtn != null) resetBtn.clicked += OnResetClicked;
            
            // Category Menu Bindings
            categoryMenu = root.Q<VisualElement>("CategoryMenu");
            var btnAll = root.Q<Button>("CatBtn_All");
            var btnStructure = root.Q<Button>("CatBtn_Structure");
            var btnPropulsion = root.Q<Button>("CatBtn_Propulsion");
            var btnAvionics = root.Q<Button>("CatBtn_Avionics");
            var btnPower = root.Q<Button>("CatBtn_Power");

            // Layer Button Toggles Menu
            var layerBtn = root.Q<Button>("LayerBtn");
            if (layerBtn != null) layerBtn.clicked += ToggleCategoryMenu;

            // Category Clicks
            if (btnAll != null) btnAll.clicked += () => SetCategoryFilter("ALL", btnAll);
            if (btnStructure != null) btnStructure.clicked += () => SetCategoryFilter("Structure", btnStructure);
            if (btnPropulsion != null) btnPropulsion.clicked += () => SetCategoryFilter("Propulsion", btnPropulsion);
            if (btnAvionics != null) btnAvionics.clicked += () => SetCategoryFilter("Avionics", btnAvionics);
            if (btnPower != null) btnPower.clicked += () => SetCategoryFilter("Power", btnPower);

            if (explosionSlider != null) explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);

            // Step 2 Fix: Allow closing by clicking header since buttons hide
            var header = root.Q(className: "sheet-header");
            if (header != null) header.RegisterCallback<ClickEvent>(evt => SetSheetState(!isSheetOpen));

            // Initial State
            UpdatePartIndicator(null);
            if (infoBtn != null) infoBtn.SetEnabled(false);
            
            // Check Initial App State for Slider
            if (AppStateMachine.Instance != null && sliderContainer != null)
            {
                 bool isExploded = AppStateMachine.Instance.CurrentState == AppState.ExplodedView;
                 sliderContainer.EnableInClassList("slider-hidden", !isExploded);
            }
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Subscribe<AppStateChangedEvent>(OnAppStateChanged);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Unsubscribe<AppStateChangedEvent>(OnAppStateChanged);
        }

        #region Interaction Handlers

        private void OnShaderToggle()
        {
            if (ExplodedViewManager.Instance != null)
            {
                ExplodedViewManager.Instance.CycleVisualMode();
                var currentMode = ExplodedViewManager.Instance.CurrentMode;

                // Visual Feedback
                if (shaderBtn != null)
                {
                    bool isActive = currentMode != ExplodedViewManager.VisualMode.Normal;
                    shaderBtn.EnableInClassList("btn-tag--active", isActive); 

                    // Update Tooltip or Icon Color if possible
                    var label = shaderBtn.Q<Label>();
                    // Unfortunately shaderBtn usually has an icon, but we can tint it
                    
                    if (isActive)
                    {
                        // Tint based on mode?
                        Color tint = Color.white;
                        switch(currentMode)
                        {
                            case ExplodedViewManager.VisualMode.XRay: tint = Color.cyan; break;
                            case ExplodedViewManager.VisualMode.Blueprint: tint = new Color(0.2f, 0.6f, 1.0f); break; // Blue
                            case ExplodedViewManager.VisualMode.Thermal: tint = new Color(1.0f, 0.4f, 0.0f); break; // Orange
                            case ExplodedViewManager.VisualMode.Wireframe: tint = Color.green; break;
                            case ExplodedViewManager.VisualMode.Ghosted: tint = new Color(0.8f, 0.8f, 1.0f); break;
                            default: tint = Color.white; break;
                        }
                        shaderBtn.style.color = new StyleColor(tint);
                        shaderBtn.tooltip = $"View: {currentMode}";
                    }
                    else
                    {
                        shaderBtn.style.color = new StyleColor(Color.white);
                        shaderBtn.tooltip = "Toggle Visual Mode";
                    }
                }
            }
        }

        private void OnExplodeToggle()
        {
            // Toggle between Exploration and ExplodedView
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
            // 1. Reset Application State (Collapse Explosion)
            if (AppStateMachine.Instance != null && AppStateMachine.Instance.CurrentState != AppState.Exploration)
            {
                AppStateMachine.Instance.EnterExploration();
            }

            // 2. Reset Camera
            if (OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.ResetView();
            }

            // 3. Clear Selection (Close Sheet)
            EventBus.Publish(new PartSelectedEvent(null)); // Selects nothing
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

            // Step 2 Refinement: Shift UI up
            if (bottomBar != null) bottomBar.EnableInClassList("ui-shifted", isOpen);
            
            // Note: SliderContainer visibility is now managed by AppState, not Sheet State entirely.
            // But if Sheet is open, we might want to shift it if it WAS visible.
            if (sliderContainer != null) 
            {
                sliderContainer.EnableInClassList("ui-shifted", isOpen);
            }

            // Step 2.5: Shift Visual Center of 3D World
            if (OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.SetViewportShift(isOpen ? 0.15f : 0f);
            }
        }

        private void ToggleCategoryMenu()
        {
            if (categoryMenu != null)
            {
                bool isHidden = categoryMenu.ClassListContains("category-menu--hidden");
                categoryMenu.ToggleInClassList("category-menu--hidden");
                
                // If we are OPENING the menu (isHidden was true), shift slider up
                if (sliderContainer != null)
                {
                    sliderContainer.EnableInClassList("slider-container--shifted-up", isHidden);
                }
            }
        }

        private void SetCategoryFilter(string category, Button clickedBtn)
        {
            // Logic:
            // - If "ALL" is clicked -> Clear others, select ALL.
            // - If specific category clicked -> 
            //      - If "ALL" was selected, remove "ALL".
            //      - Toggle specific category.
            //      - If list becomes empty, re-select "ALL".

            if (category == "ALL")
            {
                activeCategories.Clear();
                activeCategories.Add("ALL");
            }
            else
            {
                if (activeCategories.Contains("ALL"))
                {
                    activeCategories.Remove("ALL");
                }

                if (activeCategories.Contains(category))
                {
                    activeCategories.Remove(category);
                }
                else
                {
                    activeCategories.Add(category);
                }

                // Safety: If nothing selected, revert to ALL
                if (activeCategories.Count == 0)
                {
                    activeCategories.Add("ALL");
                }
            }

            // 1. Apply Filter
            if (ExplodedViewManager.Instance != null)
            {
                ExplodedViewManager.Instance.SetCategoryFilters(activeCategories);
            }

            // 2. Update UI Visuals
            if (categoryMenu != null)
            {
                // Helper to update button state
                void UpdateButtonState(string btnName, string catName)
                {
                    var btn = categoryMenu.Q<Button>(btnName);
                    if (btn != null)
                    {
                        bool isActive = activeCategories.Contains(catName);
                        btn.EnableInClassList("btn-tag--active", isActive);
                    }
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
            {
                ExplodedViewManager.Instance.SetExplosionFactor(evt.newValue);
            }
        }
        
        private void OnAppStateChanged(AppStateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            
            if (sliderContainer != null)
            {
                if (isExploded)
                {
                    sliderContainer.RemoveFromClassList("slider-hidden");
                    
                    // Check if menu is open, if so, ensure shifted up
                    if (categoryMenu != null && !categoryMenu.ClassListContains("category-menu--hidden"))
                    {
                        sliderContainer.AddToClassList("slider-container--shifted-up");
                    }
                    else
                    {
                        sliderContainer.RemoveFromClassList("slider-container--shifted-up");
                    }

                    // Sync slider value
                    if (explosionSlider != null)
                    {
                        // Default explosion factor is 0.5f in ExplodedViewManager
                        // We set this explicitly to ensure UI matches state
                        explosionSlider.SetValueWithoutNotify(0.5f); 
                    }
                }
                else
                {
                    sliderContainer.AddToClassList("slider-hidden");
                }
            }

            // Update Explode Button Visuals (Optional - e.g. toggled state)
            if (explodeBtn != null)
            {
                // Could change icon or style
            }
        }

        #endregion

        #region Event Callbacks

        private void OnPartSelected(PartSelectedEvent evt)
        {
            UpdatePartIndicator(evt.PartData);

            if (evt.PartData != null)
            {
                // Part Selected: Enable Info Button
                if (infoBtn != null) infoBtn.SetEnabled(true);

                if (sheetTitle != null) sheetTitle.text = evt.PartData.PartName.ToUpper();
                if (sheetCategory != null) sheetCategory.text = evt.PartData.Category; 
                if (sheetFunction != null) sheetFunction.text = evt.PartData.Function;
                if (sheetMaterial != null) sheetMaterial.text = evt.PartData.MaterialType;
                if (sheetDesc != null) sheetDesc.text = evt.PartData.Description;
            }
            else
            {
                // Deselected: Disable Info Button & Auto-Close Sheet
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
                partNameLabel.style.color = new StyleColor(new Color(0.06f, 0.73f, 0.5f)); // Cyber Green
            }
            else
            {
                partNameLabel.text = "SELECT A PART";
                partNameLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.7f)); // Silver
            }
        }

        #endregion
    }
}
