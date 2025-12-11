using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class ViewModeToolbar : Singleton<ViewModeToolbar>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement toolbar;
        private VisualElement modeButtonsContainer;
        private Label currentModeLabel;

        private readonly (string name, ViewMode mode)[] modes = new[]
        {
            ("Realistic", ViewMode.Realistic),
            ("X-Ray", ViewMode.XRay),
            ("Blueprint", ViewMode.Blueprint),
            ("Solid", ViewMode.SolidColor),
            ("Wireframe", ViewMode.Wireframe),
            ("Ghosted", ViewMode.Ghosted),
            ("Thermal", ViewMode.Thermal)
        };

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateToolbarUI();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.OnModeChanged += OnModeChanged;
            }
        }

        private void CreateToolbarUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Toolbar container (bottom center)
            toolbar = new VisualElement();
            toolbar.name = "ViewModeToolbar";
            toolbar.style.position = Position.Absolute;
            toolbar.style.bottom = 20;
            toolbar.style.left = Length.Percent(50);
            toolbar.style.translate = new Translate(Length.Percent(-50), 0);
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.alignItems = Align.Center;
            toolbar.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            toolbar.style.borderTopLeftRadius = 25; toolbar.style.borderTopRightRadius = 25; toolbar.style.borderBottomLeftRadius = 25; toolbar.style.borderBottomRightRadius = 25;
            toolbar.style.paddingLeft = 10;
            toolbar.style.paddingRight = 10;
            toolbar.style.paddingTop = 8;
            toolbar.style.paddingBottom = 8;

            // Mode buttons container
            modeButtonsContainer = new VisualElement();
            modeButtonsContainer.style.flexDirection = FlexDirection.Row;

            foreach (var (name, mode) in modes)
            {
                var btn = CreateModeButton(name, mode);
                modeButtonsContainer.Add(btn);
            }

            toolbar.Add(modeButtonsContainer);

            // Divider
            var divider = new VisualElement();
            divider.style.width = 1;
            divider.style.height = 30;
            divider.style.backgroundColor = new Color(1, 1, 1, 0.2f);
            divider.style.marginLeft = 10;
            divider.style.marginRight = 10;
            toolbar.Add(divider);

            // Additional controls
            var crossSectionBtn = CreateToolButton("✂", "Cross Section", OnCrossSectionClicked);
            var droneStateBtn = CreateToolButton("⚡", "Power", OnDroneStateClicked);
            var catalogBtn = CreateToolButton("📋", "Catalog", OnCatalogClicked);
            var resetBtn = CreateToolButton("↺", "Reset", OnResetClicked);

            toolbar.Add(crossSectionBtn);
            toolbar.Add(droneStateBtn);
            toolbar.Add(catalogBtn);
            toolbar.Add(resetBtn);

            root.Add(toolbar);
        }

        private Button CreateModeButton(string label, ViewMode mode)
        {
            var btn = new Button(() => OnModeButtonClicked(mode));
            btn.text = label;
            btn.style.height = 34;
            btn.style.paddingLeft = 12;
            btn.style.paddingRight = 12;
            btn.style.marginRight = 5;
            btn.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
            btn.style.borderTopLeftRadius = 17; btn.style.borderTopRightRadius = 17; btn.style.borderBottomLeftRadius = 17; btn.style.borderBottomRightRadius = 17;
            btn.style.color = Color.white;
            btn.style.fontSize = 11;
            btn.name = $"ModeBtn_{mode}";

            // Highlight current mode
            if (ViewModeManager.Instance != null && ViewModeManager.Instance.CurrentMode == mode)
            {
                btn.style.backgroundColor = new Color(0.2f, 0.6f, 1f, 0.8f);
            }

            return btn;
        }

        private Button CreateToolButton(string icon, string tooltip, System.Action onClick)
        {
            var btn = new Button(onClick);
            btn.text = icon;
            btn.style.width = 40;
            btn.style.height = 34;
            btn.style.marginRight = 5;
            btn.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
            btn.style.borderTopLeftRadius = 17; btn.style.borderTopRightRadius = 17; btn.style.borderBottomLeftRadius = 17; btn.style.borderBottomRightRadius = 17;
            btn.style.color = Color.white;
            btn.style.fontSize = 16;
            btn.tooltip = tooltip;
            return btn;
        }

        private void OnModeButtonClicked(ViewMode mode)
        {
            if (ViewModeManager.Instance != null)
            {
                ViewModeManager.Instance.SetViewMode(mode);
            }
        }

        private void OnModeChanged(ViewMode newMode)
        {
            // Update button styles
            foreach (var child in modeButtonsContainer.Children())
            {
                if (child is Button btn)
                {
                    bool isActive = btn.name == $"ModeBtn_{newMode}";
                    btn.style.backgroundColor = isActive
                        ? new Color(0.2f, 0.6f, 1f, 0.8f)
                        : new Color(1, 1, 1, 0.1f);
                }
            }
        }

        private void OnCrossSectionClicked()
        {
            if (CrossSectionManager.Instance != null)
            {
                CrossSectionManager.Instance.ToggleCrossSection();
            }
        }

        private void OnDroneStateClicked()
        {
            if (DroneStateController.Instance != null)
            {
                DroneStateController.Instance.TogglePower();
            }
        }

        private void OnCatalogClicked()
        {
            if (PartCatalogUI.Instance != null)
            {
                PartCatalogUI.Instance.Toggle();
            }
        }

        private void OnResetClicked()
        {
            // Reset everything
            ViewModeManager.Instance?.SetViewMode(ViewMode.Realistic);
            CrossSectionManager.Instance?.DisableCrossSection();
            PartVisibilityManager.Instance?.ShowAllParts();
            OrbitCameraController.Instance?.ResetView();
            SelectionManager.Instance?.Deselect();
            
            if (AppStateMachine.Instance != null)
            {
                AppStateMachine.Instance.SetState(AppState.Exploration);
            }

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("View reset");
            }
        }

        public void Show()
        {
            if (toolbar != null) toolbar.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (toolbar != null) toolbar.style.display = DisplayStyle.None;
        }
    }
}
