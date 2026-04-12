using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the Hero/Landing screen: main menu, submenus (Devices, About, Exit),
    /// and transitions into/out of the 3D viewer.
    /// Extracted from UIManager (Phase 3 Step 2: God Class Dismantling).
    /// </summary>
    public class UIHeroController
    {
        private const string RepositoryUrl = "https://github.com/delarge95/WebGL-Thesis-Proposal";

        public enum SubmenuType { Devices, About, Exit }

        // ── Elements ──
        private readonly VisualElement _heroContainer;
        private readonly VisualElement _heroMain;
        private readonly VisualElement _submenuDevices;
        private readonly VisualElement _submenuAbout;
        private readonly VisualElement _submenuExit;
        private readonly VisualElement _root;

        // ── State ──
        public bool HeroDismissed { get; private set; } = false;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        // ── Callbacks that UIManager can hook into ──
        public event System.Action OnHeroDismissed;
        public event System.Action OnHeroReturned;
        public event System.Action OnHelpRequested;

        public UIHeroController(VisualElement root)
        {
            _root = root;
            _heroContainer = root.Q<VisualElement>("HeroContainer");
            _heroMain = root.Q<VisualElement>("HeroMain");
            _submenuDevices = root.Q<VisualElement>("HeroSubmenu_Devices");
            _submenuAbout = root.Q<VisualElement>("HeroSubmenu_About");
            _submenuExit = root.Q<VisualElement>("HeroSubmenu_Exit");

            BindButtons();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            OnHeroDismissed = null;
            OnHeroReturned = null;
            OnHelpRequested = null;
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════════════
        //  Public API
        // ═══════════════════════════════════════════════════════

        /// <summary>Hide the hero screen and notify listeners.</summary>
        public void DismissHero()
        {
            HeroDismissed = true;
            if (_heroContainer != null)
            {
                _heroContainer.AddToClassList("hero--hidden");
                _heroContainer.pickingMode = PickingMode.Ignore;
                _heroContainer.style.display = DisplayStyle.None;
            }
            OnHeroDismissed?.Invoke();
        }

        /// <summary>Return to the hero screen from 3D viewer.</summary>
        public void ReturnToHero()
        {
            HeroDismissed = false;

            if (_heroContainer != null)
            {
                _heroContainer.RemoveFromClassList("hero--hidden");
                _heroContainer.pickingMode = PickingMode.Position;
                _heroContainer.style.display = DisplayStyle.Flex;
                CloseHeroSubmenu();
            }
            OnHeroReturned?.Invoke();
        }

        public void OpenHeroSubmenu(SubmenuType type)
        {
            if (_heroMain != null) _heroMain.style.display = DisplayStyle.None;

            if (_submenuDevices != null) _submenuDevices.RemoveFromClassList("hero-submenu--active");
            if (_submenuAbout != null) _submenuAbout.RemoveFromClassList("hero-submenu--active");
            if (_submenuExit != null) _submenuExit.RemoveFromClassList("hero-submenu--active");

            switch (type)
            {
                case SubmenuType.Devices:
                    if (_submenuDevices != null) _submenuDevices.AddToClassList("hero-submenu--active");
                    break;
                case SubmenuType.About:
                    if (_submenuAbout != null) _submenuAbout.AddToClassList("hero-submenu--active");
                    break;
                case SubmenuType.Exit:
                    if (_submenuExit != null) _submenuExit.AddToClassList("hero-submenu--active");
                    break;
            }
        }

        public void CloseHeroSubmenu()
        {
            if (_submenuDevices != null) _submenuDevices.RemoveFromClassList("hero-submenu--active");
            if (_submenuAbout != null) _submenuAbout.RemoveFromClassList("hero-submenu--active");
            if (_submenuExit != null) _submenuExit.RemoveFromClassList("hero-submenu--active");
            if (_heroMain != null) _heroMain.style.display = DisplayStyle.Flex;
        }

        // ═══════════════════════════════════════════════════════
        //  Button Bindings (with cleanup)
        // ═══════════════════════════════════════════════════════

        private void BindButtons()
        {
            var heroExploreBtn = _root.Q<Button>("HeroExploreBtn");
            var heroDeviceBtn = _root.Q<Button>("HeroDeviceBtn");
            var heroAboutBtn = _root.Q<Button>("HeroAboutBtn");
            var heroExitBtn = _root.Q<Button>("HeroExitBtn");

            // Explore
            if (heroExploreBtn != null)
            {
                System.Action onExplore = () =>
                {
                    DismissHero();
                    if (AppStateMachine.Instance != null) AppStateMachine.Instance.EnterExploration();
                };
                heroExploreBtn.clicked += onExplore;
                AddCleanup(() => heroExploreBtn.clicked -= onExplore);
            }

            // Submenu navigation
            System.Action onDevices = () => OpenHeroSubmenu(SubmenuType.Devices);
            System.Action onAbout = () => OpenHeroSubmenu(SubmenuType.About);
            System.Action onExit = () => OpenHeroSubmenu(SubmenuType.Exit);
            if (heroDeviceBtn != null) { heroDeviceBtn.clicked += onDevices; AddCleanup(() => heroDeviceBtn.clicked -= onDevices); }
            if (heroAboutBtn != null) { heroAboutBtn.clicked += onAbout; AddCleanup(() => heroAboutBtn.clicked -= onAbout); }
            if (heroExitBtn != null) { heroExitBtn.clicked += onExit; AddCleanup(() => heroExitBtn.clicked -= onExit); }

            // Help
            var heroHelpBtn = _root.Q<Button>("HeroHelpBtn");
            if (heroHelpBtn != null)
            {
                System.Action onHelp = () => OnHelpRequested?.Invoke();
                heroHelpBtn.clicked += onHelp;
                AddCleanup(() => heroHelpBtn.clicked -= onHelp);
            }

            var heroGithubBtn = _root.Q<Button>("HeroGithubBtn");
            if (heroGithubBtn != null)
            {
                System.Action onGithub = () => Application.OpenURL(RepositoryUrl);
                heroGithubBtn.clicked += onGithub;
                AddCleanup(() => heroGithubBtn.clicked -= onGithub);
            }

            // Back buttons
            var backDevices = _root.Q<Button>("SubmenuBackBtn_Devices");
            var backAbout = _root.Q<Button>("SubmenuBackBtn_About");
            var backExit = _root.Q<Button>("SubmenuBackBtn_Exit");
            if (backDevices != null) { backDevices.clicked += CloseHeroSubmenu; AddCleanup(() => backDevices.clicked -= CloseHeroSubmenu); }
            if (backAbout != null) { backAbout.clicked += CloseHeroSubmenu; AddCleanup(() => backAbout.clicked -= CloseHeroSubmenu); }
            if (backExit != null) { backExit.clicked += CloseHeroSubmenu; AddCleanup(() => backExit.clicked -= CloseHeroSubmenu); }

            // Exit confirmation
            var exitConfirmBtn = _root.Q<Button>("ExitConfirmBtn");
            var exitCancelBtn = _root.Q<Button>("ExitCancelBtn");
            if (exitCancelBtn != null) { exitCancelBtn.clicked += CloseHeroSubmenu; AddCleanup(() => exitCancelBtn.clicked -= CloseHeroSubmenu); }
            if (exitConfirmBtn != null)
            {
                System.Action onExitConfirm = () =>
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
                exitConfirmBtn.clicked += onExitConfirm;
                AddCleanup(() => exitConfirmBtn.clicked -= onExitConfirm);
            }

            // Home button (return to hero)
            var homeBtn = _root.Q<Button>("HomeBtn");
            if (homeBtn != null) { homeBtn.clicked += ReturnToHero; AddCleanup(() => homeBtn.clicked -= ReturnToHero); }
        }
    }
}
