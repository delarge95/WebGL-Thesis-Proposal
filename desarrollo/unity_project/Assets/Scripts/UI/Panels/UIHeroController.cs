using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void X500V2ExitToLanding();
#endif

        public enum SubmenuType { Devices, About, Exit }

        // ── Elements ──
        private readonly VisualElement _heroContainer;
        private readonly VisualElement _heroMain;
        private readonly VisualElement _submenuDevices;
        private readonly VisualElement _submenuAbout;
        private readonly VisualElement _submenuExit;
        private readonly VisualElement _root;
        private readonly Button _languageBtn;
        private readonly Label _languageEnLabel;
        private readonly Label _languageEsLabel;

        // ── State ──
        public bool HeroDismissed { get; private set; } = false;
        public bool HasOpenSubmenu =>
            (_submenuDevices != null && _submenuDevices.ClassListContains("hero-submenu--active")) ||
            (_submenuAbout != null && _submenuAbout.ClassListContains("hero-submenu--active")) ||
            (_submenuExit != null && _submenuExit.ClassListContains("hero-submenu--active"));

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
            _languageBtn = root.Q<Button>("HeroLanguageBtn");
            _languageEnLabel = root.Q<Label>("HeroLangEnLabel");
            _languageEsLabel = root.Q<Label>("HeroLangEsLabel");

            AppLanguageManager.LanguageChanged += OnLanguageChanged;
            AddCleanup(() => AppLanguageManager.LanguageChanged -= OnLanguageChanged);
            BindButtons();
            UpdateLanguageVisuals();
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

        public bool HandleBackNavigation()
        {
            if (HeroDismissed)
            {
                return false;
            }

            if (HasOpenSubmenu)
            {
                CloseHeroSubmenu();
                return true;
            }

            return false;
        }

        public bool RequestExitConfirmation()
        {
            if (HeroDismissed)
            {
                return false;
            }

            OpenHeroSubmenu(SubmenuType.Exit);
            return true;
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
            var heroLanguageBtn = _root.Q<Button>("HeroLanguageBtn");

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
            if (heroLanguageBtn != null)
            {
                System.Action onLanguage = AppLanguageManager.ToggleLanguage;
                heroLanguageBtn.clicked += onLanguage;
                AddCleanup(() => heroLanguageBtn.clicked -= onLanguage);
            }

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

            var heroPerfCaptureBtn = _root.Q<Button>("HeroPerfCaptureBtn");
            if (heroPerfCaptureBtn != null)
            {
                System.Action onPerfCapture = TogglePerformanceCapture;
                heroPerfCaptureBtn.clicked += onPerfCapture;
                AddCleanup(() => heroPerfCaptureBtn.clicked -= onPerfCapture);
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
                System.Action onExitConfirm = RequestExitToLanding;
                EventCallback<PointerUpEvent> onExitConfirmPointer = evt =>
                {
                    if (evt.button == 0)
                    {
                        evt.StopPropagation();
                        RequestExitToLanding();
                    }
                };
                exitConfirmBtn.clicked += onExitConfirm;
                AddCleanup(() => exitConfirmBtn.clicked -= onExitConfirm);
                exitConfirmBtn.RegisterCallback(onExitConfirmPointer);
                AddCleanup(() => exitConfirmBtn.UnregisterCallback(onExitConfirmPointer));
            }

            // Home button (return to hero)
            var homeBtn = _root.Q<Button>("HomeBtn");
            if (homeBtn != null) { homeBtn.clicked += ReturnToHero; AddCleanup(() => homeBtn.clicked -= ReturnToHero); }
        }

        private static void TogglePerformanceCapture()
        {
            WebGLProfiler profiler = UnityEngine.Object.FindAnyObjectByType<WebGLProfiler>();
            if (profiler == null)
            {
                var host = new GameObject("_WebGLProfiler");
                profiler = host.AddComponent<WebGLProfiler>();
                UnityEngine.Object.DontDestroyOnLoad(host);
            }

            profiler.ToggleCapturePanel();
        }

        private void RequestExitToLanding()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            X500V2ExitToLanding();
#else
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
        }

        private void OnLanguageChanged(string languageCode)
        {
            UpdateLanguageVisuals();
        }

        private void UpdateLanguageVisuals()
        {
            AppLanguageManager.ApplyStaticText(_root);
            if (_languageBtn != null)
            {
                bool spanish = AppLanguageManager.IsSpanish;
                _languageBtn.text = string.Empty;
                _languageBtn.EnableInClassList("hero-language-switch--es", spanish);
                _languageBtn.EnableInClassList("hero-language-switch--en", !spanish);
                _languageBtn.tooltip = AppLanguageManager.IsSpanish ? "Switch to English" : "Cambiar a espa\u00f1ol";
            }

            _languageEnLabel?.EnableInClassList("hero-language-option--active", !AppLanguageManager.IsSpanish);
            _languageEsLabel?.EnableInClassList("hero-language-option--active", AppLanguageManager.IsSpanish);
        }
    }
}
