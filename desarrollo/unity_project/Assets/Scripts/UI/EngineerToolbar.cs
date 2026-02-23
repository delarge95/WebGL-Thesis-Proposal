using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    /// <summary>
    /// [DEPRECATED — Phase 2 UX Redesign] Engineering tools dropdown.
    /// Future integration: tools could move into a dedicated Analyze/Studio sub-panel.
    /// Kept for reference; not instantiated at runtime.
    /// </summary>
    [System.Obsolete("Replaced by 3-mode system (Phase 2 UX Redesign). Not instantiated at runtime.")]
    public class EngineerToolbar : Singleton<EngineerToolbar>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement toolbar;
        private VisualElement dropdownPanel;
        private bool isDropdownOpen = false;

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateToolbarUI();
        }

        private void CreateToolbarUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main toolbar (top right)
            toolbar = new VisualElement();
            toolbar.name = "EngineerToolbar";
            toolbar.style.position = Position.Absolute;
            toolbar.style.right = 20;
            toolbar.style.top = 80;
            toolbar.style.flexDirection = FlexDirection.Column;
            toolbar.style.alignItems = Align.FlexEnd;

            // Toggle button
            var toggleBtn = CreateToolbarButton("🔧", "Engineer Tools", () => ToggleDropdown());
            toolbar.Add(toggleBtn);

            // Dropdown panel
            dropdownPanel = new VisualElement();
            dropdownPanel.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            dropdownPanel.style.borderTopLeftRadius = 12; dropdownPanel.style.borderTopRightRadius = 12; dropdownPanel.style.borderBottomLeftRadius = 12; dropdownPanel.style.borderBottomRightRadius = 12;
            dropdownPanel.style.marginTop = 10;
            dropdownPanel.style.paddingLeft = 10;
            dropdownPanel.style.paddingRight = 10;
            dropdownPanel.style.paddingTop = 10;
            dropdownPanel.style.paddingBottom = 10;
            dropdownPanel.style.display = DisplayStyle.None;

            // Tool buttons
            AddToolButton(dropdownPanel, "📏", "Measure Distance", () => {
                MeasurementTool.Instance?.Activate(MeasurementMode.Distance);
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "📐", "Measure Angle", () => {
                MeasurementTool.Instance?.Activate(MeasurementMode.Angle);
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "🔌", "Connection Points", () => {
                ConnectionPointsViewer.Instance?.Toggle();
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "📝", "Add Annotation", () => {
                StartAnnotationMode();
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "📋", "Assembly Guide", () => {
                AssemblyStepUI.Instance?.Toggle();
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "✅", "Checklist", () => {
                ShowChecklist();
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "📄", "Bill of Materials", () => {
                ShowBOM();
                CloseDropdown();
            });

            AddToolButton(dropdownPanel, "🔍", "Part Catalog", () => {
                PartCatalogUI.Instance?.Toggle();
                CloseDropdown();
            });

            toolbar.Add(dropdownPanel);
            root.Add(toolbar);
        }

        private Button CreateToolbarButton(string icon, string tooltip, System.Action onClick)
        {
            var btn = new Button(onClick);
            btn.text = icon;
            btn.style.width = 50;
            btn.style.height = 50;
            btn.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
            btn.style.borderTopLeftRadius = 25; btn.style.borderTopRightRadius = 25; btn.style.borderBottomLeftRadius = 25; btn.style.borderBottomRightRadius = 25;
            btn.style.color = Color.white;
            btn.style.fontSize = 22;
            btn.tooltip = tooltip;
            return btn;
        }

        private void AddToolButton(VisualElement parent, string icon, string label, System.Action onClick)
        {
            var btn = new Button(onClick);
            btn.style.flexDirection = FlexDirection.Row;
            btn.style.alignItems = Align.Center;
            btn.style.width = 180;
            btn.style.height = 40;
            btn.style.marginBottom = 5;
            btn.style.paddingLeft = 12;
            btn.style.backgroundColor = new Color(1, 1, 1, 0.05f);
            btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
            btn.style.borderTopLeftRadius = 8; btn.style.borderTopRightRadius = 8; btn.style.borderBottomLeftRadius = 8; btn.style.borderBottomRightRadius = 8;

            var iconLabel = new Label(icon);
            iconLabel.style.fontSize = 16;
            iconLabel.style.marginRight = 10;
            btn.Add(iconLabel);

            var textLabel = new Label(label);
            textLabel.style.fontSize = 12;
            textLabel.style.color = Color.white;
            btn.Add(textLabel);

            parent.Add(btn);
        }

        private void ToggleDropdown()
        {
            isDropdownOpen = !isDropdownOpen;
            dropdownPanel.style.display = isDropdownOpen ? DisplayStyle.Flex : DisplayStyle.None;
            AudioManager.Instance?.PlayClick();
        }

        private void CloseDropdown()
        {
            isDropdownOpen = false;
            dropdownPanel.style.display = DisplayStyle.None;
        }

        private void StartAnnotationMode()
        {
            // Enter annotation mode
            NotificationManager.Instance?.ShowNotification("Click on a part to add an annotation");
            // Would set a flag for SelectionManager to handle annotation creation
        }

        private void ShowChecklist()
        {
            if (AssemblyChecklist.Instance == null || NotificationManager.Instance == null) return;

            var summary = AssemblyChecklist.Instance.GetSummary();
            NotificationManager.Instance.ShowNotification(summary, 5f);
        }

        private void ShowBOM()
        {
            if (BillOfMaterialsManager.Instance == null || NotificationManager.Instance == null) return;

            var summary = BillOfMaterialsManager.Instance.GetSummary();
            NotificationManager.Instance.ShowNotification(summary, 5f);
        }
    }
}
