using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Data;
using WebGL.Core.Content;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.UI
{
    public class EnhancedInfoPanel : Singleton<EnhancedInfoPanel>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement infoPanel;
        private VisualElement contentContainer;
        
        // Header elements
        private Label titleLabel;
        private Label subtitleLabel;
        private VisualElement iconContainer;

        // Tabs
        private TabView tabView;
        private VisualElement overviewTab;
        private VisualElement materialsTab;
        private VisualElement specsTab;

        // Data labels
        private Label descriptionLabel;
        private Label materialTypeLabel;
        private Label materialPropsLabel;
        private Label weightLabel;
        private Label dimensionsLabel;
        private Label functionLabel;

        // Action buttons
        private Button hideButton;
        private Button isolateButton;
        private Button focusButton;
        private Button closeButton;

        private ExplodablePart currentPart;
        private bool isVisible = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateInfoPanelUI();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void CreateInfoPanelUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main panel
            infoPanel = new VisualElement();
            infoPanel.name = "EnhancedInfoPanel";
            infoPanel.style.position = Position.Absolute;
            infoPanel.style.right = 20;
            infoPanel.style.top = 20;
            infoPanel.style.width = 350;
            infoPanel.style.maxHeight = Length.Percent(80);
            infoPanel.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            infoPanel.style.borderRadius = 12;
            infoPanel.style.display = DisplayStyle.None;
            infoPanel.style.opacity = 0;

            // Close button (top right)
            closeButton = new Button(Hide);
            closeButton.text = "✕";
            closeButton.style.position = Position.Absolute;
            closeButton.style.right = 10;
            closeButton.style.top = 10;
            closeButton.style.width = 30;
            closeButton.style.height = 30;
            closeButton.style.backgroundColor = Color.clear;
            closeButton.style.borderWidth = 0;
            closeButton.style.color = new Color(1, 1, 1, 0.5f);
            closeButton.style.fontSize = 18;
            infoPanel.Add(closeButton);

            // Header
            var header = new VisualElement();
            header.style.paddingTop = 20;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 20;
            header.style.paddingBottom = 15;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(1, 1, 1, 0.1f);

            titleLabel = new Label("Part Name");
            titleLabel.style.fontSize = 22;
            titleLabel.style.color = Color.white;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.Add(titleLabel);

            subtitleLabel = new Label("Category");
            subtitleLabel.style.fontSize = 12;
            subtitleLabel.style.color = new Color(0.6f, 0.8f, 1f, 0.8f);
            subtitleLabel.style.marginTop = 5;
            header.Add(subtitleLabel);

            infoPanel.Add(header);

            // Content container with scroll
            contentContainer = new ScrollView();
            contentContainer.style.flexGrow = 1;
            contentContainer.style.paddingLeft = 20;
            contentContainer.style.paddingRight = 20;
            contentContainer.style.paddingTop = 15;
            contentContainer.style.paddingBottom = 15;

            // Overview section
            CreateSection(contentContainer, "Overview", out overviewTab);
            descriptionLabel = CreateInfoRow(overviewTab, "Description", "No description");
            weightLabel = CreateInfoRow(overviewTab, "Weight", "-");
            dimensionsLabel = CreateInfoRow(overviewTab, "Dimensions", "-");
            functionLabel = CreateInfoRow(overviewTab, "Function", "-");

            // Materials section
            CreateSection(contentContainer, "Materials", out materialsTab);
            materialTypeLabel = CreateInfoRow(materialsTab, "Type", "-");
            materialPropsLabel = CreateInfoRow(materialsTab, "Properties", "-");

            infoPanel.Add(contentContainer);

            // Action buttons
            var actionsContainer = new VisualElement();
            actionsContainer.style.flexDirection = FlexDirection.Row;
            actionsContainer.style.paddingLeft = 15;
            actionsContainer.style.paddingRight = 15;
            actionsContainer.style.paddingTop = 10;
            actionsContainer.style.paddingBottom = 15;
            actionsContainer.style.justifyContent = Justify.SpaceAround;
            actionsContainer.style.borderTopWidth = 1;
            actionsContainer.style.borderTopColor = new Color(1, 1, 1, 0.1f);

            hideButton = CreateActionButton("Hide", OnHideClicked);
            isolateButton = CreateActionButton("Isolate", OnIsolateClicked);
            focusButton = CreateActionButton("Focus", OnFocusClicked);

            actionsContainer.Add(hideButton);
            actionsContainer.Add(isolateButton);
            actionsContainer.Add(focusButton);

            infoPanel.Add(actionsContainer);

            root.Add(infoPanel);
        }

        private void CreateSection(VisualElement parent, string title, out VisualElement container)
        {
            var section = new VisualElement();
            section.style.marginBottom = 15;

            var header = new Label(title);
            header.style.fontSize = 14;
            header.style.color = new Color(0.6f, 0.8f, 1f);
            header.style.marginBottom = 10;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            section.Add(header);

            container = new VisualElement();
            section.Add(container);
            parent.Add(section);
        }

        private Label CreateInfoRow(VisualElement parent, string label, string value)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.marginBottom = 8;

            var labelElement = new Label(label);
            labelElement.style.color = new Color(1, 1, 1, 0.5f);
            labelElement.style.fontSize = 12;

            var valueLabel = new Label(value);
            valueLabel.style.color = Color.white;
            valueLabel.style.fontSize = 12;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            valueLabel.style.flexGrow = 1;
            valueLabel.style.marginLeft = 10;

            row.Add(labelElement);
            row.Add(valueLabel);
            parent.Add(row);

            return valueLabel;
        }

        private Button CreateActionButton(string text, System.Action onClick)
        {
            var button = new Button(onClick);
            button.text = text;
            button.style.paddingTop = 8;
            button.style.paddingBottom = 8;
            button.style.paddingLeft = 15;
            button.style.paddingRight = 15;
            button.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            button.style.borderWidth = 0;
            button.style.borderRadius = 6;
            button.style.color = Color.white;
            button.style.fontSize = 12;
            return button;
        }

        private void OnPartSelected(PartSelectedEvent evt)
        {
            if (evt.PartData != null)
            {
                var part = FindPartByData(evt.PartData);
                ShowPartInfo(part, evt.PartData);
            }
            else
            {
                Hide();
            }
        }

        private ExplodablePart FindPartByData(DronePartData data)
        {
            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                if (part.Data == data) return part;
            }
            return null;
        }

        public void ShowPartInfo(ExplodablePart part, DronePartData data)
        {
            if (data == null) return;

            currentPart = part;

            // Update labels
            titleLabel.text = data.PartName;
            subtitleLabel.text = data.PartType;
            descriptionLabel.text = data.Description;
            weightLabel.text = $"{data.Weight:F2} kg";
            dimensionsLabel.text = data.Dimensions;
            functionLabel.text = data.Function;
            materialTypeLabel.text = data.MaterialType;
            materialPropsLabel.text = data.MaterialProperties;

            Show();
        }

        public void Show()
        {
            if (infoPanel == null) return;

            infoPanel.style.display = DisplayStyle.Flex;
            isVisible = true;

            // Animate in
            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(0f, 1f, 0.3f, v => infoPanel.style.opacity = v);
            }
            else
            {
                infoPanel.style.opacity = 1;
            }
        }

        public void Hide()
        {
            if (infoPanel == null) return;

            isVisible = false;

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(1f, 0f, 0.2f, 
                    v => infoPanel.style.opacity = v,
                    () => infoPanel.style.display = DisplayStyle.None);
            }
            else
            {
                infoPanel.style.opacity = 0;
                infoPanel.style.display = DisplayStyle.None;
            }

            currentPart = null;
        }

        private void OnHideClicked()
        {
            if (currentPart != null && PartVisibilityManager.Instance != null)
            {
                PartVisibilityManager.Instance.HidePart(currentPart);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        }

        private void OnIsolateClicked()
        {
            if (currentPart != null && PartVisibilityManager.Instance != null)
            {
                PartVisibilityManager.Instance.IsolatePart(currentPart);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        }

        private void OnFocusClicked()
        {
            if (currentPart != null && OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.SetTarget(currentPart.transform);
                OrbitCameraController.Instance.SetDistance(3f);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        }
    }
}
