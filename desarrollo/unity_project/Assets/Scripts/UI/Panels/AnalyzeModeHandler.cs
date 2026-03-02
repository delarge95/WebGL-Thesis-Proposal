using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Managers;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Handles the Analyze mode UI: Cross-section, Explode, and Filter cards
    /// with two-level navigation (card grid → sub-panel → back).
    /// </summary>
    public class AnalyzeModeHandler : BaseModeHandler
    {
        private readonly VisualElement _cardGrid;
        private readonly Button _crossSectionBtn;
        private readonly Button _explodeBtn;
        private readonly Button _filterBtn;
        private readonly VisualElement _crossSectionPanel;
        private readonly VisualElement _explodeSubPanel;
        private readonly VisualElement _filterSubPanel;

        private enum SubLevel { CardGrid, SubPanel }

        private SubLevel _level = SubLevel.CardGrid;
        private string _activePanel; // "cross-section" | "explode" | "filter" | null
        private bool _isExploded;
        private readonly List<string> _activeCategories = new() { "ALL" };

        /// <summary>True when a sub-panel is open (for back-navigation detection).</summary>
        public bool IsSubPanelOpen => _level == SubLevel.SubPanel;

        public event Action OnExplodeToggleRequested;

        public AnalyzeModeHandler(VisualElement root, VisualElement container) : base(root, container)
        {
            if (container == null) return;

            _cardGrid = container.Q<VisualElement>("AnalyzeCardGrid");
            _crossSectionBtn = container.Q<Button>("AnalyzeCrossSectionBtn");
            _explodeBtn = container.Q<Button>("AnalyzeExplodeBtn");
            _filterBtn = container.Q<Button>("AnalyzeFilterBtn");
            _crossSectionPanel = container.Q<VisualElement>("CrossSectionPanel");
            _explodeSubPanel = container.Q<VisualElement>("ExplodeSubPanel");
            _filterSubPanel = container.Q<VisualElement>("FilterSubPanel");

            BindCards();
        }

        private void BindCards()
        {
            if (_crossSectionBtn != null)
            {
                Action onCross = () => DelayAction(() => NavigateToSubPanel("cross-section"));
                _crossSectionBtn.clicked += onCross;
                AddCleanup(() => _crossSectionBtn.clicked -= onCross);
            }

            if (_explodeBtn != null)
            {
                Action onExplode = () => DelayAction(() => NavigateToSubPanel("explode"));
                _explodeBtn.clicked += onExplode;
                AddCleanup(() => _explodeBtn.clicked -= onExplode);
            }

            if (_filterBtn != null)
            {
                Action onFilter = () => DelayAction(() => NavigateToSubPanel("filter"));
                _filterBtn.clicked += onFilter;
                AddCleanup(() => _filterBtn.clicked -= onFilter);
            }
        }

        public override void Activate()
        {
            _level = SubLevel.CardGrid;
            _activePanel = null;
            ShowLevel();
        }

        public override void Deactivate()
        {
            CloseAllSubPanels();
        }

        /// <summary>Navigate from card grid to a sub-panel.</summary>
        public void NavigateToSubPanel(string panelId)
        {
            _level = SubLevel.SubPanel;
            _activePanel = panelId;
            ShowLevel();

            _crossSectionBtn?.EnableInClassList("submenu-card--active", panelId == "cross-section");
            _explodeBtn?.EnableInClassList("submenu-card--active", panelId == "explode");
            _filterBtn?.EnableInClassList("submenu-card--active", panelId == "filter");

            if (panelId == "cross-section")
                CrossSectionManager.Instance?.EnableCrossSection();
        }

        /// <summary>Navigate from sub-panel back to card grid.</summary>
        public void NavigateToCardGrid()
        {
            if (_activePanel == "cross-section")
                CrossSectionManager.Instance?.DisableCrossSection();

            _level = SubLevel.CardGrid;
            _activePanel = null;
            ShowLevel();

            _crossSectionBtn?.RemoveFromClassList("submenu-card--active");
            _explodeBtn?.RemoveFromClassList("submenu-card--active");
            _filterBtn?.RemoveFromClassList("submenu-card--active");
        }

        private void ShowLevel()
        {
            bool showGrid = _level == SubLevel.CardGrid;
            _cardGrid?.EnableInClassList("submenu--hidden", !showGrid);

            _crossSectionPanel?.EnableInClassList("submenu--hidden", !(
                !showGrid && _activePanel == "cross-section"));
            _explodeSubPanel?.EnableInClassList("submenu--hidden", !(
                !showGrid && _activePanel == "explode"));
            _filterSubPanel?.EnableInClassList("submenu--hidden", !(
                !showGrid && _activePanel == "filter"));
        }

        private void CloseAllSubPanels()
        {
            if (_activePanel == "cross-section")
                CrossSectionManager.Instance?.DisableCrossSection();

            _level = SubLevel.CardGrid;
            _activePanel = null;
            _cardGrid?.RemoveFromClassList("submenu--hidden");
            _crossSectionPanel?.AddToClassList("submenu--hidden");
            _explodeSubPanel?.AddToClassList("submenu--hidden");
            _filterSubPanel?.AddToClassList("submenu--hidden");
            _crossSectionBtn?.RemoveFromClassList("submenu-card--active");
            _explodeBtn?.RemoveFromClassList("submenu-card--active");
            _filterBtn?.RemoveFromClassList("submenu-card--active");
        }

        // ── Explode State Sync ──

        public void SetExplodeState(bool exploded)
        {
            _isExploded = exploded;
            _explodeBtn?.EnableInClassList("submenu-card--active", exploded);
        }

        public bool IsExploded => _isExploded;

        // ── Category Filters ──

        public void SetCategoryFilter(string category, Button clickedBtn)
        {
            if (category == "ALL")
            {
                _activeCategories.Clear();
                _activeCategories.Add("ALL");
            }
            else
            {
                if (_activeCategories.Contains("ALL"))
                    _activeCategories.Remove("ALL");

                if (_activeCategories.Contains(category))
                    _activeCategories.Remove(category);
                else
                    _activeCategories.Add(category);

                if (_activeCategories.Count == 0)
                    _activeCategories.Add("ALL");
            }

            ExplodedViewManager.Instance?.SetCategoryFilters(_activeCategories);
            UpdateCategoryButtonStates();
        }

        private void UpdateCategoryButtonStates()
        {
            if (_filterSubPanel == null) return;

            void UpdateBtn(string btnName, string catName)
            {
                var btn = _filterSubPanel.Q<Button>(btnName);
                btn?.EnableInClassList("submenu-card--active", _activeCategories.Contains(catName));
            }

            UpdateBtn("CatBtn_All", "ALL");
            UpdateBtn("CatBtn_Structure", "Structure");
            UpdateBtn("CatBtn_Propulsion", "Propulsion");
            UpdateBtn("CatBtn_Avionics", "Avionics");
            UpdateBtn("CatBtn_Power", "Power");
            UpdateBtn("CatBtn_Payload", "Payload");
        }

        // ── Legacy API ──

        public void ToggleCrossSectionPanel()
        {
            if (_level == SubLevel.SubPanel && _activePanel == "cross-section")
                NavigateToCardGrid();
            else
                NavigateToSubPanel("cross-section");
        }

        public void ToggleCategoryMenu()
        {
            if (_level == SubLevel.SubPanel && _activePanel == "filter")
                NavigateToCardGrid();
            else
                NavigateToSubPanel("filter");
        }

        public void ToggleExplodePanel()
        {
            if (_level == SubLevel.SubPanel && _activePanel == "explode")
                NavigateToCardGrid();
            else
                NavigateToSubPanel("explode");
        }

        public override void Dispose()
        {
            OnExplodeToggleRequested = null;
            base.Dispose();
        }
    }
}
