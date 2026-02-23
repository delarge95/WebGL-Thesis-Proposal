using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Content;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the popup/submenu system: ShaderMenu, CategoryMenu, EnvPanel, SliderContainer.
    /// Handles toggling, mutual exclusion (only one menu open at a time), and dynamic stacking.
    /// Extracted from UIManager (Phase 3 Step 2: God Class Dismantling).
    /// </summary>
    public class UIPopupController
    {
        // ── Layout Math Constants (8pt grid) ──
        private const float POPUP_BASE_BOTTOM = 192f;
        private const float POPUP_GAP = 24f;

        // ── Elements ──
        private readonly VisualElement _root;
        private readonly VisualElement _shaderMenu;
        private readonly VisualElement _categoryMenu;
        private readonly VisualElement _envPanel;
        private readonly VisualElement _sliderContainer;
        private readonly VisualElement _popupBlocker;
        private readonly Slider _explosionSlider;
        private readonly Button _hotspotBtn;

        // ── State ──
        private bool _shaderMenuShown = false;
        private bool _hotspotsEnabled = false;
        private bool _isSheetOpen = false;
        private List<string> _activeCategories = new List<string>() { "ALL" };

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        public UIPopupController(
            VisualElement root,
            VisualElement shaderMenu,
            VisualElement categoryMenu,
            VisualElement envPanel,
            VisualElement sliderContainer,
            VisualElement popupBlocker,
            Slider explosionSlider,
            Button hotspotBtn)
        {
            _root = root;
            _shaderMenu = shaderMenu;
            _categoryMenu = categoryMenu;
            _envPanel = envPanel;
            _sliderContainer = sliderContainer;
            _popupBlocker = popupBlocker;
            _explosionSlider = explosionSlider;
            _hotspotBtn = hotspotBtn;

            BindPopupBlocker();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        private void BindPopupBlocker()
        {
            if (_popupBlocker == null) return;
            EventCallback<PointerDownEvent> pbDown = evt => CloseAllMenus();
            _popupBlocker.RegisterCallback(pbDown);
            AddCleanup(() => _popupBlocker.UnregisterCallback(pbDown));
        }

        // ═══════════════════════════════════════════════════════
        //  Public API — called by UIManager
        // ═══════════════════════════════════════════════════════

        /// <summary>Notify the popup controller when the sheet opens/closes so stacking adjusts.</summary>
        public void SetSheetOpenState(bool isOpen)
        {
            _isSheetOpen = isOpen;

            // When sheet opens, close all popups
            if (isOpen)
            {
                CloseAllMenus();
            }
            else
            {
                RepositionPopups();
            }
        }

        public void ToggleShaderMenu()
        {
            if (_shaderMenu == null) return;
            _shaderMenuShown = !_shaderMenuShown;

            if (_shaderMenuShown)
            {
                _shaderMenu.BringToFront();
                if (_envPanel != null) _envPanel.AddToClassList("submenu--hidden");
                if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
            }

            _shaderMenu.EnableInClassList("submenu--hidden", !_shaderMenuShown);
            RepositionPopups();
        }

        public void ToggleEnvPanel()
        {
            if (_envPanel == null) return;
            _envPanel.ToggleInClassList("submenu--hidden");

            if (!_envPanel.ClassListContains("submenu--hidden"))
            {
                _envPanel.BringToFront();
                if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
                if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
            }
            RepositionPopups();
        }

        public void ToggleCategoryMenu()
        {
            if (_categoryMenu == null) return;
            _categoryMenu.ToggleInClassList("submenu--hidden");

            if (!_categoryMenu.ClassListContains("submenu--hidden"))
            {
                _categoryMenu.BringToFront();
                if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
                if (_envPanel != null) _envPanel.AddToClassList("submenu--hidden");
            }
            RepositionPopups();
        }

        public void CloseAllMenus()
        {
            if (_shaderMenu != null) { _shaderMenu.AddToClassList("submenu--hidden"); _shaderMenuShown = false; }
            if (_categoryMenu != null) _categoryMenu.AddToClassList("submenu--hidden");
            if (_envPanel != null) _envPanel.AddToClassList("submenu--hidden");
            if (_sliderContainer != null) _sliderContainer.AddToClassList("slider-hidden");
            RepositionPopups();
        }

        public void ToggleHotspots()
        {
            _hotspotsEnabled = !_hotspotsEnabled;
            HotspotManager.Instance?.ToggleVisibility();

            if (_hotspotBtn != null)
                _hotspotBtn.EnableInClassList("submenu-card--active", _hotspotsEnabled);
        }

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

        /// <summary>Shows/hides the slider and repositions popups. Called by AppState changes.</summary>
        public void SetSliderVisible(bool visible)
        {
            if (_sliderContainer == null) return;

            if (visible)
            {
                _sliderContainer.RemoveFromClassList("slider-hidden");
                _sliderContainer.BringToFront();

                if (_explosionSlider != null)
                    _explosionSlider.SetValueWithoutNotify(0.5f);
            }
            else
            {
                _sliderContainer.AddToClassList("slider-hidden");
            }

            RepositionPopups();
        }

        // ═══════════════════════════════════════════════════════
        //  Popup Stacking
        // ═══════════════════════════════════════════════════════

        public void RepositionPopups()
        {
            float rootHeight = _root != null ? _root.layout.height : 0f;
            if (float.IsNaN(rootHeight) || rootHeight < 1f) rootHeight = Screen.height;

            float currentBottom = _isSheetOpen ? (rootHeight * 0.56f) + POPUP_BASE_BOTTOM : POPUP_BASE_BOTTOM;

            var popups = new (VisualElement el, string hiddenClass, float height)[]
            {
                (_sliderContainer, "slider-hidden",   56f),
                (_categoryMenu,    "submenu--hidden", 192f),
                (_shaderMenu,      "submenu--hidden", 192f),
                (_envPanel,        "submenu--hidden", 220f),
            };

            bool anyMenuVisible = false;

            foreach (var (el, hiddenClass, height) in popups)
            {
                if (el == null) continue;

                bool isVisible = !el.ClassListContains(hiddenClass);
                if (isVisible)
                {
                    el.style.bottom = new StyleLength(currentBottom);
                    currentBottom += height + POPUP_GAP;

                    if (el != _sliderContainer)
                        anyMenuVisible = true;
                }
            }

            if (_popupBlocker != null)
                _popupBlocker.EnableInClassList("popup-blocker--hidden", !anyMenuVisible);
        }

        // ═══════════════════════════════════════════════════════
        //  Private helpers
        // ═══════════════════════════════════════════════════════

        private void UpdateCategoryButtonStates()
        {
            if (_categoryMenu == null) return;

            void UpdateBtn(string btnName, string catName)
            {
                var btn = _categoryMenu.Q<Button>(btnName);
                if (btn != null) btn.EnableInClassList("submenu-card--active", _activeCategories.Contains(catName));
            }

            UpdateBtn("CatBtn_All", "ALL");
            UpdateBtn("CatBtn_Structure", "Structure");
            UpdateBtn("CatBtn_Propulsion", "Propulsion");
            UpdateBtn("CatBtn_Avionics", "Avionics");
            UpdateBtn("CatBtn_Power", "Power");
        }
    }
}
