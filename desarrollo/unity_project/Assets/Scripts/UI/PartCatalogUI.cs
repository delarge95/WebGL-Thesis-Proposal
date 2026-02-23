using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class PartCatalogUI : Singleton<PartCatalogUI>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement catalogPanel;
        private TextField searchField;
        private VisualElement categoryContainer;
        private ScrollView partsList;
        private VisualElement filterDropdown;
        private Label resultsCount;

        private List<Button> categoryButtons = new List<Button>();
        private string currentCategory = "";

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateCatalogUI();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (PartCatalogManager.Instance != null)
            {
                PartCatalogManager.Instance.OnFilterChanged += OnFilterChanged;
            }
        }

        private void CreateCatalogUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main panel (left side)
            catalogPanel = new VisualElement();
            catalogPanel.name = "PartCatalogPanel";
            catalogPanel.style.position = Position.Absolute;
            catalogPanel.style.left = 20;
            catalogPanel.style.top = 20;
            catalogPanel.style.bottom = 20;
            catalogPanel.style.width = 280;
            catalogPanel.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            catalogPanel.style.borderTopLeftRadius = 20; catalogPanel.style.borderTopRightRadius = 20; catalogPanel.style.borderBottomLeftRadius = 20; catalogPanel.style.borderBottomRightRadius = 20;
            catalogPanel.style.display = DisplayStyle.None;

            // Header
            var header = new VisualElement();
            header.style.paddingTop = 20;
            header.style.paddingLeft = 15;
            header.style.paddingRight = 15;
            header.style.paddingBottom = 15;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(1, 1, 1, 0.1f);

            var title = new Label("Part Catalog");
            title.style.fontSize = 20;
            title.style.color = Color.white;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.Add(title);

            // Search field
            searchField = new TextField();
            // searchField.placeholder = "Search parts...";
            searchField.value = "Search parts..."; // Fallback
            searchField.style.marginTop = 15;
            searchField.style.height = 36;
            searchField.style.backgroundColor = new Color(0, 0, 0, 0.3f);
            searchField.style.borderTopLeftRadius = 8; searchField.style.borderTopRightRadius = 8; searchField.style.borderBottomLeftRadius = 8; searchField.style.borderBottomRightRadius = 8;
            searchField.style.color = Color.white;
            searchField.RegisterValueChangedCallback(OnSearchChanged);
            header.Add(searchField);

            catalogPanel.Add(header);

            // Category filters
            categoryContainer = new VisualElement();
            categoryContainer.style.flexDirection = FlexDirection.Row;
            categoryContainer.style.flexWrap = Wrap.Wrap;
            categoryContainer.style.paddingLeft = 10;
            categoryContainer.style.paddingRight = 10;
            categoryContainer.style.paddingTop = 10;
            categoryContainer.style.paddingBottom = 10;

            // Add "All" button
            var allBtn = CreateCategoryButton("All", "");
            categoryContainer.Add(allBtn);

            if (PartCatalogManager.Instance != null)
            {
                foreach (var category in PartCatalogManager.Instance.Categories)
                {
                    var btn = CreateCategoryButton(category.name, category.name);
                    categoryContainer.Add(btn);
                }
            }

            catalogPanel.Add(categoryContainer);

            // Results count
            resultsCount = new Label("0 parts found");
            resultsCount.style.fontSize = 12;
            resultsCount.style.color = new Color(1, 1, 1, 0.5f);
            resultsCount.style.paddingLeft = 15;
            resultsCount.style.paddingBottom = 10;
            catalogPanel.Add(resultsCount);

            // Parts list
            partsList = new ScrollView();
            partsList.style.flexGrow = 1;
            partsList.style.paddingLeft = 10;
            partsList.style.paddingRight = 10;
            catalogPanel.Add(partsList);

            root.Add(catalogPanel);

            // Initial population
            RefreshPartsList();
        }

        private Button CreateCategoryButton(string label, string category)
        {
            var btn = new Button(() => OnCategoryClicked(category));
            btn.text = label;
            btn.style.height = 28;
            btn.style.paddingLeft = 12;
            btn.style.paddingRight = 12;
            btn.style.marginRight = 5;
            btn.style.marginBottom = 5;
            btn.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
            btn.style.borderTopLeftRadius = 14; btn.style.borderTopRightRadius = 14; btn.style.borderBottomLeftRadius = 14; btn.style.borderBottomRightRadius = 14;
            btn.style.color = Color.white;
            btn.style.fontSize = 11;
            categoryButtons.Add(btn);
            return btn;
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            PartCatalogManager.Instance?.Search(evt.newValue);
        }

        private void OnCategoryClicked(string category)
        {
            currentCategory = category;
            
            // Update button styles
            foreach (var btn in categoryButtons)
            {
                bool isActive = (string.IsNullOrEmpty(category) && btn.text == "All") || 
                               btn.text == category;
                btn.style.backgroundColor = isActive 
                    ? new Color(0.2f, 0.6f, 1f, 0.8f) 
                    : new Color(1, 1, 1, 0.1f);
            }

            PartCatalogManager.Instance?.FilterByCategory(category);
            AudioManager.Instance?.PlayClick();
        }

        private void OnFilterChanged(List<ExplodablePart> parts)
        {
            RefreshPartsList(parts);
        }

        private void RefreshPartsList(List<ExplodablePart> parts = null)
        {
            if (parts == null && PartCatalogManager.Instance != null)
            {
                parts = PartCatalogManager.Instance.FilteredParts;
            }

            partsList.Clear();

            if (parts == null) return;

            resultsCount.text = $"{parts.Count} parts found";

            foreach (var part in parts)
            {
                if (part == null || part.Data == null) continue;
                var item = CreatePartListItem(part);
                partsList.Add(item);
            }
        }

        private VisualElement CreatePartListItem(ExplodablePart part)
        {
            var item = new Button(() => OnPartClicked(part));
            item.style.flexDirection = FlexDirection.Row;
            item.style.alignItems = Align.Center;
            item.style.height = 50;
            item.style.paddingLeft = 10;
            item.style.paddingRight = 10;
            item.style.marginBottom = 5;
            item.style.backgroundColor = new Color(1, 1, 1, 0.05f);
            item.style.borderTopLeftRadius = 8; item.style.borderTopRightRadius = 8; item.style.borderBottomLeftRadius = 8; item.style.borderBottomRightRadius = 8;
            item.style.borderTopWidth = 0; item.style.borderBottomWidth = 0; item.style.borderLeftWidth = 0; item.style.borderRightWidth = 0;

            // Icon placeholder
            var icon = new VisualElement();
            icon.style.width = 32;
            icon.style.height = 32;
            icon.style.backgroundColor = new Color(0.2f, 0.6f, 1f, 0.3f);
            icon.style.borderTopLeftRadius = 6; icon.style.borderTopRightRadius = 6; icon.style.borderBottomLeftRadius = 6; icon.style.borderBottomRightRadius = 6;
            icon.style.marginRight = 10;
            item.Add(icon);

            // Text container
            var textContainer = new VisualElement();
            textContainer.style.flexGrow = 1;

            var name = new Label(part.Data.PartName);
            name.style.fontSize = 13;
            name.style.color = Color.white;
            textContainer.Add(name);

            var type = new Label(part.Data.PartType);
            type.style.fontSize = 10;
            type.style.color = new Color(1, 1, 1, 0.5f);
            textContainer.Add(type);

            item.Add(textContainer);

            // Visibility toggle
            var visBtn = new Button(() => OnVisibilityToggle(part));
            visBtn.text = PartVisibilityManager.Instance?.IsPartVisible(part) == true ? "👁" : "👁‍🗨";
            visBtn.style.width = 30;
            visBtn.style.height = 30;
            visBtn.style.backgroundColor = Color.clear;
            visBtn.style.borderTopWidth = 0; visBtn.style.borderBottomWidth = 0; visBtn.style.borderLeftWidth = 0; visBtn.style.borderRightWidth = 0;
            visBtn.style.fontSize = 14;
            item.Add(visBtn);

            return item;
        }

        private void OnPartClicked(ExplodablePart part)
        {
            PartCatalogManager.Instance?.FocusPart(part);
        }

        private void OnVisibilityToggle(ExplodablePart part)
        {
            PartVisibilityManager.Instance?.TogglePartVisibility(part);
            RefreshPartsList();
        }

        public void Show()
        {
            if (catalogPanel == null) return;
            catalogPanel.style.display = DisplayStyle.Flex;
            RefreshPartsList();

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(0f, 1f, 0.3f, v => catalogPanel.style.opacity = v);
            }
        }

        public void Hide()
        {
            if (catalogPanel == null) return;

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(1f, 0f, 0.2f,
                    v => catalogPanel.style.opacity = v,
                    () => catalogPanel.style.display = DisplayStyle.None);
            }
            else
            {
                catalogPanel.style.display = DisplayStyle.None;
            }
        }

        public void Toggle()
        {
            if (catalogPanel.style.display == DisplayStyle.None)
                Show();
            else
                Hide();
        }
    }
}
