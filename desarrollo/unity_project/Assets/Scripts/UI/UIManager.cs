using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
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
        private Label partNameLabel; // In Bottom Bar
        private Label sheetTitle;
        private Label sheetCategory;
        private Label sheetDesc;

        // Buttons
        private Button shaderBtn;
        private Button explodeBtn;
        private Button infoBtn;

        // State
        private bool isSheetOpen = false;

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
            sheetDesc = root.Q<Label>("PartDescription");

            shaderBtn = root.Q<Button>("ShaderBtn");
            explodeBtn = root.Q<Button>("ExplodeBtn");
            infoBtn = root.Q<Button>("InfoBtn");

            // Event Listeners
            if (shaderBtn != null) shaderBtn.clicked += OnShaderToggle;
            if (explodeBtn != null) explodeBtn.clicked += OnExplodeToggle;
            if (infoBtn != null) infoBtn.clicked += OnInfoToggle;

            // Step 2 Fix: Allow closing by clicking header since buttons hide
            var header = root.Q(className: "sheet-header");
            if (header != null) header.RegisterCallback<ClickEvent>(evt => SetSheetState(!isSheetOpen));

            // Initial State
            UpdatePartIndicator(null);
            if (infoBtn != null) infoBtn.SetEnabled(false);
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
        }

        #region Interaction Handlers

        private void OnShaderToggle()
        {
            Debug.Log("[UIManager] Shader Toggle Clicked (Feature Pending)");
            // Future implementation: Cycle RenderPipelineAsset or Material Properties
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
            if (sliderContainer != null) sliderContainer.EnableInClassList("ui-shifted", isOpen);

            // Step 2.5: Shift Visual Center of 3D World
            // If sheet is open (~40% height), we want to shift center UP by half of that (~20%)?
            // User request: "resolution equivalent to previous size minus sheet height".
            // Effectively centering in the top 60%.
            // Center of top 60% is at y=0.7 (relative to full 0.5). Shift is +0.2?
            // Let's try 0.15f (15%) as a safe "Professional" offset.
            if (OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.SetViewportShift(isOpen ? 0.15f : 0f);
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
                if (sheetCategory != null) sheetCategory.text = $"CATEGORY: {evt.PartData.Category}"; 
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
