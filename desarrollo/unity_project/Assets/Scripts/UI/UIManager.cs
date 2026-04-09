using UnityEngine;
using UnityEngine.UIElements;
using System;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
using WebGL.Core.Thermal;
using WebGL.UI.Panels;
using WebGL.UI.ProceduralIcons;

namespace WebGL.UI
{
    /// <summary>
    /// Slim coordinator for all UI sub-systems (Phase 3 Step 2 + Phase 2 UX Redesign).
    /// Delegates domain logic to:
    ///   - UIDetailsSheet    → bottom sheet, part data display
    ///   - UIModeController  → 3-mode system (Explore/Analyze/Studio), sub-menus, category filters
    ///   - UIHeroController  → hero/landing screen
    ///   - UIAnalyzePanel    → shader mode menu cards
    ///   - UIEnvironmentPanel→ environment preset cards + sliders
    /// UIManager retains only: initialization, event routing, button wiring, and
    /// cross-cutting concerns (hotspots, app-state reactions, button input blockers).
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainDocument;

        // ── Root ──
        private VisualElement root;

        // ── Sub-Controllers (Phase 3 Step 2 + Phase 2 Redesign) ──
        private UIDetailsSheet _detailsSheet;
        private UIModeController _modeController;
        private UIHeroController _heroController;
        private UIAnalyzePanel _uiAnalyzePanel;
        private UIEnvironmentPanel _uiEnvironmentPanel;
        private UICrossSectionPanel _uiCrossSectionPanel;
        private OnboardingController _onboardingController;

        // ── Buttons (wired here, logic delegated) ──
        private Button resetBtn;
        // ── FAB (global info button) ──
        private VisualElement _fabContainer;
        private Button _fabInfoBtn;
        // ── Slider (explosion) ──
        private Slider explosionSlider;

        // ── State ──
        private bool _hotspotsInitialized = false;
        private bool _isIsolated = false;
        private Transform _isolatedFullSelection;
        private Transform _isolatedSubSelection;
        private bool _hotspotIsolationActive = false;
        private readonly System.Collections.Generic.List<ExplodablePart> _hotspotIsolationMembers = new System.Collections.Generic.List<ExplodablePart>();
        private enum SubIsolationParentLayer
        {
            None,
            FullSelection,
            HotspotGroup
        }
        private SubIsolationParentLayer _subIsolationParentLayer = SubIsolationParentLayer.None;

        private bool HasHotspotIsolationLayer
        {
            get
            {
                if (_hotspotIsolationMembers.Count > 0)
                {
                    return true;
                }

                return PartVisibilityManager.Instance != null
                    && PartVisibilityManager.Instance.HasStoredGroupIsolation;
            }
        }



        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                HandleEscapeKey();
        }

        /// <summary>
        /// Escape key priority: sheet → sub-panel / active mode → nothing.
        /// </summary>
        private void HandleEscapeKey()
        {
            // 1. Close bottom sheet if open
            if (_detailsSheet != null && _detailsSheet.IsSheetOpen)
            {
                _detailsSheet.SetSheetState(false);
                return;
            }

            // 2. Close sub-panel or deactivate mode
            if (_modeController != null && _modeController.HandleEscapeKey())
                return;
        }

        // ── Memory Leak Prevention (Phase 3 Step 1) ──
        private System.Collections.Generic.List<System.Action> _uiCleanupActions = new System.Collections.Generic.List<System.Action>();

        // ── Button event guards (shared instances) ──
        private EventCallback<PointerDownEvent> _onBtnDown = evt =>
        {
            if (evt.button == 0)
            {
                InputManager.InputBlocked = true;
            }
            evt.StopPropagation();
        };
        private EventCallback<PointerUpEvent> _onBtnUp = evt =>
        {
            if (evt.button == 0)
            {
                InputManager.InputBlocked = false;
            }
            evt.StopPropagation();
        };

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
            // Pre-clear dynamic VisualElements before UIDocument teardown
            // to prevent "Cannot modify hierarchy during layout" errors (Unity 6 known issue).
            HotspotManager.Instance?.ClearHotspots();

            UnsubscribeFromEvents();
            UnsubscribeFromUIEvents();
        }

        // ═══════════════════════════════════════════════════════
        //  Cleanup helpers
        // ═══════════════════════════════════════════════════════

        private void AddCleanup(System.Action cleanupAction)
        {
            if (cleanupAction != null) _uiCleanupActions.Add(cleanupAction);
        }

        private void UnsubscribeFromUIEvents()
        {
            foreach (var action in _uiCleanupActions) action?.Invoke();
            _uiCleanupActions.Clear();
            Debug.Log("[UIManager] UI Events Unsubscribed to prevent memory leaks.");
        }

        // ═══════════════════════════════════════════════════════
        //  Initialization
        // ═══════════════════════════════════════════════════════

        private void InitializeUI()
        {
            if (root == null) return;

            // ── Auto-create managers if missing ──
            EnsureManagers();

            // ── Query shared elements ──
            resetBtn = root.Q<Button>("ResetViewBtn");
            explosionSlider = root.Q<Slider>("ExplosionSlider");

            // ── Details Sheet (info toggle handled via UIModeController event, not direct button binding) ──
            _detailsSheet = new UIDetailsSheet(root, null);
            AddCleanup(() => _detailsSheet.Dispose());

            // ── Mode Controller (3-mode system: Tools / Analyze / Studio) ──
            _modeController = new UIModeController(
                root,
                root.Q<VisualElement>("ToolsModeContainer"),
                root.Q<VisualElement>("AnalyzeModeContainer"),
                root.Q<VisualElement>("StudioModeContainer"),
                explosionSlider
            );
            AddCleanup(() => _modeController.Dispose());

            // Wire Inspect-mode isolate toggle event from UIModeController
            _modeController.OnIsolateToggleRequested += () => ToggleIsolation();

            // Notify mode controller when sheet state changes + sync FAB active state
            _detailsSheet.OnSheetStateChanged += (isOpen) =>
            {
                _modeController.SetSheetOpenState(isOpen);
                _fabInfoBtn?.EnableInClassList("fab-button--active", isOpen);
            };

            // Notify sheet to close if any mode activates
            _modeController.OnAnyModeActivated += () =>
            {
                if (_detailsSheet.IsSheetOpen)
                {
                    _detailsSheet.SetSheetState(false);
                }
            };

            // ── Analyze Panel (shader cards — ShaderMenu inside StudioModeContainer) ──
            var shaderMenu = root.Q<VisualElement>("ShaderMenu");
            _uiAnalyzePanel = new UIAnalyzePanel(shaderMenu, null);
            AddCleanup(() => _uiAnalyzePanel.Dispose());

            // ── Environment Panel (presets + sliders — StudioPanel inside StudioModeContainer) ──
            _uiEnvironmentPanel = new UIEnvironmentPanel(root.Q<VisualElement>("StudioPanel"));
            AddCleanup(() => _uiEnvironmentPanel.Dispose());

            // ── Cross-Section Panel (inside AnalyzeModeContainer) ──
            _uiCrossSectionPanel = new UICrossSectionPanel(root.Q<VisualElement>("CrossSectionPanel"));
            AddCleanup(() => _uiCrossSectionPanel.Dispose());

            // ── Hero Controller ──
            _heroController = new UIHeroController(root);
            _heroController.OnHeroDismissed += OnHeroDismissed;
            _heroController.OnHeroReturned += OnHeroReturned;
            AddCleanup(() => _heroController.Dispose());

            // ── Onboarding Controller ──
            _onboardingController = new OnboardingController(root);
            _heroController.OnHelpRequested += () => _onboardingController.Show();
            AddCleanup(() => _onboardingController.Dispose());

            // ── Wire remaining toolbar buttons (mode buttons handled by UIModeController) ──
            if (resetBtn != null) { resetBtn.clicked += OnResetClicked; AddCleanup(() => resetBtn.clicked -= OnResetClicked); }

            // ── Category filter buttons (inside AnalyzeModeContainer/FilterSubPanel) ──
            BindCat("CatBtn_Structure", "SkeletonAirframe");
            BindCat("CatBtn_Propulsion", "PropulsionSystem");
            BindCat("CatBtn_Avionics", "Avionics");
            BindCat("CatBtn_Sensors", "SensorsComms");
            BindCat("CatBtn_Power", "PowerDistribution");
            BindCat("CatBtn_Payload", "Fasteners");

            // ── FAB (global info button) ──
            _fabContainer = root.Q<VisualElement>("GlobalActionContainer");
            _fabInfoBtn = root.Q<Button>("ToolInfoBtn");
            if (_fabInfoBtn != null)
            {
                System.Action fabClick = () => _detailsSheet.ShowInfo();
                _fabInfoBtn.clicked += fabClick;
                AddCleanup(() => _fabInfoBtn.clicked -= fabClick);
            }

            // ── Explosion slider ──
            if (explosionSlider != null)
            {
                explosionSlider.RegisterValueChangedCallback(OnExplosionSliderChanged);
                AddCleanup(() => explosionSlider.UnregisterValueChangedCallback(OnExplosionSliderChanged));

                EventCallback<PointerDownEvent> esDo = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = true;
                    }
                    evt.StopPropagation();
                };
                EventCallback<PointerUpEvent> esUp = evt =>
                {
                    if (evt.button == 0)
                    {
                        InputManager.InputBlocked = false;
                    }
                };
                explosionSlider.RegisterCallback(esDo);
                explosionSlider.RegisterCallback(esUp);
                AddCleanup(() => { explosionSlider.UnregisterCallback(esDo); explosionSlider.UnregisterCallback(esUp); });
            }

            // ── ExplodeInlineSlider container — stop pointer events from bubbling
            //    past the slider so clicks don't reach parent handlers. ──
            var explodeInline = root.Q<VisualElement>("ExplodeInlineSlider");
            if (explodeInline != null)
            {
                EventCallback<PointerDownEvent> epDo = evt => evt.StopPropagation();
                explodeInline.RegisterCallback(epDo);
                AddCleanup(() => explodeInline.UnregisterCallback(epDo));
            }

            // ── CrossSectionPanel & FilterSubPanel — same protection ──
            foreach (var panelName in new[] { "CrossSectionPanel", "FilterSubPanel" })
            {
                var panel = root.Q<VisualElement>(panelName);
                if (panel == null) continue;
                EventCallback<PointerDownEvent> pDo = evt => evt.StopPropagation();
                panel.RegisterCallback(pDo);
                AddCleanup(() => panel.UnregisterCallback(pDo));
            }

            // ── Initial state ──
            _detailsSheet.UpdatePartIndicator(null);
            // Slider is inside ExplodeSubPanel (starts hidden via submenu--hidden)

            // ── Adaptive UI contrast based on environment luminance ──
            var envCtrl = EnvironmentController.Instance;
            if (envCtrl != null)
            {
                envCtrl.OnLightBackgroundChanged += OnLightBgChanged;
                AddCleanup(() => envCtrl.OnLightBackgroundChanged -= OnLightBgChanged);
                root.EnableInClassList("ui-light-bg", envCtrl.IsLightBackground);
                root.Query<ProceduralIconBase>().ForEach(icon => icon.SetLightBackground(envCtrl.IsLightBackground));
            }

            // ── All buttons stop propagation inside UITK to avoid click-through ──
            RegisterButtonEventGuards();
        }

        // ═══════════════════════════════════════════════════════
        //  Category filter helper
        // ═══════════════════════════════════════════════════════

        private void BindCat(string btnName, string category)
        {
            var btn = root.Q<Button>(btnName);
            if (btn == null) return;

            EventCallback<ClickEvent> onClick = evt =>
            {
                bool exclusive = evt.clickCount >= 2;
                _modeController.SetCategoryFilter(category, btn, exclusive);
                evt.StopPropagation();
            };

            btn.RegisterCallback(onClick);
            AddCleanup(() => btn.UnregisterCallback(onClick));
        }

        private void SetFabVisible(bool visible)
        {
            _fabContainer?.EnableInClassList("fab-button--hidden", !visible);
        }

        // ═══════════════════════════════════════════════════════
        //  Hero lifecycle callbacks
        // ═══════════════════════════════════════════════════════

        private void OnHeroDismissed()
        {
            // Initialize hotspots now
            if (!_hotspotsInitialized && HotspotManager.Instance != null)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance.Initialize(root);
            }
            HotspotManager.Instance?.SetVisible(true);

            // Show onboarding overlay the first time the user enters the 3D view
            _onboardingController?.ShowIfFirstTime();
        }

        private void OnHeroReturned()
        {
            // Close everything
            _detailsSheet.SetSheetState(false);
            _modeController.DeactivateAllModes();

            // Hide hotspots
            HotspotManager.Instance?.SetVisible(false);

            // Reset camera
            OrbitCameraController.Instance?.ResetView();

            // Reset view mode
            ViewModeManager.Instance?.SetViewMode(ViewMode.Realistic);

            // Reset app state — handled by DeactivateAllModes → Exploration
        }

        // ═══════════════════════════════════════════════════════
        //  Toolbar button handlers (simple, remain here)
        // ═══════════════════════════════════════════════════════

        private void OnResetClicked()
        {
            if (AppStateMachine.Instance?.CurrentState != null && AppStateMachine.Instance.CurrentState != AppState.Exploration)
                AppStateMachine.Instance.EnterExploration();

            OrbitCameraController.Instance?.ResetView();

            // Preserve current view mode (Blueprint, Thermal, etc.) — only reset geometry state

            // Explicitly disable cross-section on reset (one of only two valid triggers)
            CrossSectionManager.Instance?.DisableCrossSection();

            EventBus.Publish(new PartSelectedEvent(null));
        }

        private void OnExplosionSliderChanged(ChangeEvent<float> evt)
        {
            ExplodedViewManager.Instance?.SetExplosionFactor(evt.newValue);

            // Update dynamic percentage label
            var label = root.Q<Label>("ExplosionValue");
            if (label != null)
                label.text = $"{(evt.newValue * 100):F0}%";

            if (AppStateMachine.Instance == null) return;

            // Moving slider > 0 enters ExplodedView, moving to 0 exits
            if (evt.newValue > 0.001f && AppStateMachine.Instance.CurrentState != AppState.ExplodedView)
            {
                AppStateMachine.Instance.EnterExplodedView();
            }
            else if (evt.newValue <= 0.001f && AppStateMachine.Instance.CurrentState == AppState.ExplodedView)
            {
                AppStateMachine.Instance.EnterExploration();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Isolation helpers
        // ═══════════════════════════════════════════════════════

        private static Transform ResolveFullSelection(Transform selection)
        {
            if (selection == null)
            {
                return null;
            }

            var direct = selection.GetComponent<ExplodablePart>();
            if (direct != null)
            {
                return direct.transform;
            }

            var parent = selection.GetComponentInParent<ExplodablePart>();
            if (parent != null)
            {
                return parent.transform;
            }

            return selection;
        }

        private static bool IsSubSelection(Transform selection, Transform fullSelection)
        {
            if (selection == null || fullSelection == null || selection == fullSelection)
            {
                return false;
            }

            if (selection.IsChildOf(fullSelection))
            {
                return true;
            }

            string selectionPartId = ResolveCanonicalPartId(selection);
            string fullSelectionPartId = ResolveCanonicalPartId(fullSelection);

            return !string.IsNullOrWhiteSpace(selectionPartId)
                && !string.IsNullOrWhiteSpace(fullSelectionPartId)
                && string.Equals(selectionPartId, fullSelectionPartId, StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveCanonicalPartId(Transform selection)
        {
            if (selection == null)
            {
                return string.Empty;
            }

            PartRenderCategory directCategory = selection.GetComponent<PartRenderCategory>();
            if (directCategory != null && !string.IsNullOrWhiteSpace(directCategory.CanonicalPartId))
            {
                return directCategory.CanonicalPartId;
            }

            PartRenderCategory childCategory = selection.GetComponentInChildren<PartRenderCategory>(true);
            if (childCategory != null && !string.IsNullOrWhiteSpace(childCategory.CanonicalPartId))
            {
                return childCategory.CanonicalPartId;
            }

            PartRenderCategory parentCategory = selection.GetComponentInParent<PartRenderCategory>();
            if (parentCategory != null && !string.IsNullOrWhiteSpace(parentCategory.CanonicalPartId))
            {
                return parentCategory.CanonicalPartId;
            }

            ExplodablePart explodable = selection.GetComponent<ExplodablePart>();
            if (explodable == null)
            {
                explodable = selection.GetComponentInParent<ExplodablePart>();
            }

            if (explodable != null && explodable.Data != null && !string.IsNullOrWhiteSpace(explodable.Data.id))
            {
                return explodable.Data.id;
            }

            return string.Empty;
        }

        private void IsolateFullSelection(Transform fullSelection)
        {
            if (fullSelection == null)
            {
                return;
            }

            PartVisibilityManager.Instance?.IsolateTransform(fullSelection);
            _isIsolated = true;
            _isolatedFullSelection = fullSelection;
            _isolatedSubSelection = null;
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _modeController.SetIsolateState(true);

            // Center camera on the isolated part
            OrbitCameraController.Instance?.FocusOnObject(fullSelection);
        }

        private void IsolateSubSelection(Transform subSelection, Transform fullSelection)
        {
            if (subSelection == null)
            {
                return;
            }

            if (fullSelection == null)
            {
                fullSelection = ResolveFullSelection(subSelection);
            }

            bool hadIsolatedFullLayer = _isIsolated && _isolatedSubSelection == null && _isolatedFullSelection != null;

            PartVisibilityManager.Instance?.IsolateTransform(subSelection);
            _isIsolated = true;
            _isolatedFullSelection = fullSelection;
            _isolatedSubSelection = subSelection;
            if (hadIsolatedFullLayer)
            {
                _subIsolationParentLayer = SubIsolationParentLayer.FullSelection;
            }
            else if (HasHotspotIsolationLayer)
            {
                _subIsolationParentLayer = SubIsolationParentLayer.HotspotGroup;
            }
            else
            {
                _subIsolationParentLayer = SubIsolationParentLayer.FullSelection;
            }
            _modeController.SetIsolateState(true);

            OrbitCameraController.Instance?.FocusOnObject(subSelection);
        }

        private void IsolateSelectedPart()
        {
            var sel = SelectionManager.Instance?.CurrentSelection;
            if (sel == null)
            {
                return;
            }

            Transform fullSelection = ResolveFullSelection(sel);
            if (fullSelection == null)
            {
                return;
            }

            if (IsSubSelection(sel, fullSelection))
            {
                IsolateSubSelection(sel, fullSelection);
            }
            else
            {
                IsolateFullSelection(fullSelection);
            }
        }

        private void RestoreFullIsolationFromStack()
        {
            if (_isolatedFullSelection == null)
            {
                if (_hotspotIsolationActive)
                {
                    RestoreHotspotIsolation();
                }
                else
                {
                    ClearIsolation();
                }
                return;
            }

            IsolateFullSelection(_isolatedFullSelection);
        }

        private void RestoreHotspotIsolation()
        {
            PartVisibilityManager visibility = PartVisibilityManager.Instance;

            bool hasLocalMembers = false;
            if (_hotspotIsolationMembers.Count > 0)
            {
                for (int i = 0; i < _hotspotIsolationMembers.Count; i++)
                {
                    if (_hotspotIsolationMembers[i] != null)
                    {
                        hasLocalMembers = true;
                        break;
                    }
                }
            }

            if (hasLocalMembers)
            {
                visibility?.IsolateParts(_hotspotIsolationMembers);
            }

            // Always attempt fallback from persisted group isolation to avoid losing
            // hotspot context if local member list is stale.
            if (visibility != null && !visibility.IsGroupIsolationActive && visibility.HasStoredGroupIsolation)
            {
                visibility.RestoreStoredGroupIsolation();
            }

            if (visibility != null && !visibility.IsGroupIsolationActive)
            {
                // Keep state untouched if restore failed; avoid collapsing the stack to full clear.
                return;
            }

            _isIsolated = false;
            _isolatedFullSelection = null;
            _isolatedSubSelection = null;
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _hotspotIsolationActive = true;
            _modeController.SetIsolateState(true);
        }

        private void ClearIsolation()
        {
            PartVisibilityManager.Instance?.ClearIsolation();
            _isIsolated = false;
            _isolatedFullSelection = null;
            _isolatedSubSelection = null;
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _hotspotIsolationActive = false;
            _hotspotIsolationMembers.Clear();
            PartVisibilityManager.Instance?.ClearStoredGroupIsolation();
            _modeController.SetIsolateState(false);

            // Smoothly return camera to full drone view
            var cam = OrbitCameraController.Instance;
            if (cam != null)
            {
                cam.ResetView();
            }
        }

        private void ToggleIsolation()
        {
            if (_isIsolated)
            {
                if (_isolatedSubSelection != null && _isolatedFullSelection != null)
                {
                    RestoreFullIsolationFromStack();
                }
                else
                {
                    ClearIsolation();
                }
            }
            else
            {
                IsolateSelectedPart();
            }
        }

        // ═══════════════════════════════════════════════════════
        //  EventBus subscriptions
        // ═══════════════════════════════════════════════════════

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Subscribe<PartDoubleClickedEvent>(OnPartDoubleClicked);
            EventBus.Subscribe<HotspotGroupIsolatedEvent>(OnHotspotGroupIsolated);
            EventBus.Subscribe<StateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged += OnViewModeChanged;
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
            EventBus.Unsubscribe<PartDoubleClickedEvent>(OnPartDoubleClicked);
            EventBus.Unsubscribe<HotspotGroupIsolatedEvent>(OnHotspotGroupIsolated);
            EventBus.Unsubscribe<StateChangedEvent>(OnAppStateChanged);
            if (ViewModeManager.Instance != null)
                ViewModeManager.Instance.OnModeChanged -= OnViewModeChanged;
        }

        // ═══════════════════════════════════════════════════════
        //  Event callbacks — route to sub-controllers
        // ═══════════════════════════════════════════════════════

        private void OnPartSelected(PartSelectedEvent evt)
        {
            // Delegate data display to details sheet
            _detailsSheet.PopulatePartData(
                evt.PartData,
                evt.FromHotspot,
                evt.HotspotGroupLabel,
                evt.HotspotGroupSummary,
                evt.HotspotGroupMembers,
                evt.SelectionLabel,
                evt.CanonicalPartName);

            // Show/hide FAB: visible when a part is selected OR the sheet is still open
            bool fabVisible = evt.PartData != null
                || (_detailsSheet != null && _detailsSheet.IsSheetOpen);
            SetFabVisible(fabVisible);
        }

        /// <summary>
        /// Handles double-click events published by SelectionManager.
        /// </summary>
        private void OnPartDoubleClicked(PartDoubleClickedEvent evt)
        {
            if (evt.IsBackground)
            {
                // When a sub-piece is isolated, background double-click steps back
                // to full-part isolation. Otherwise it clears isolation.
                if (_isolatedSubSelection != null && _isolatedFullSelection != null)
                {
                    switch (_subIsolationParentLayer)
                    {
                        case SubIsolationParentLayer.FullSelection:
                            RestoreFullIsolationFromStack();
                            break;

                        case SubIsolationParentLayer.HotspotGroup:
                            RestoreHotspotIsolation();
                            break;

                        default:
                            if (HasHotspotIsolationLayer)
                            {
                                RestoreHotspotIsolation();
                            }
                            else
                            {
                                ClearIsolation();
                            }
                            break;
                    }
                }
                else if (_isIsolated)
                {
                    if (HasHotspotIsolationLayer)
                    {
                        RestoreHotspotIsolation();
                    }
                    else
                    {
                        ClearIsolation();
                    }
                }
                else if (HasHotspotIsolationLayer)
                {
                    if (PartVisibilityManager.Instance != null && PartVisibilityManager.Instance.IsGroupIsolationActive)
                    {
                        ClearIsolation();
                    }
                    else
                    {
                        RestoreHotspotIsolation();
                    }
                }
                else if (PartVisibilityManager.Instance != null && PartVisibilityManager.Instance.HasAnyIsolationActive)
                {
                    // Covers hotspot-system isolation that does not use _isIsolated stack.
                    ClearIsolation();
                }

                _detailsSheet?.SetSheetState(false);
                EnhancedInfoPanel.Instance?.Hide();

                return;
            }

            Transform clickedSelection = evt.ClickedTransform != null
                ? evt.ClickedTransform
                : SelectionManager.Instance?.CurrentSelection;

            Transform clickedFull = evt.FullPartTransform != null
                ? evt.FullPartTransform
                : ResolveFullSelection(clickedSelection);

            bool clickedIsSubSelection = IsSubSelection(clickedSelection, clickedFull);

            Transform selectionBeforeDoubleClick = evt.SelectionBeforeFirstClick;
            Transform fullBeforeDoubleClick = evt.FullSelectionBeforeFirstClick != null
                ? evt.FullSelectionBeforeFirstClick
                : ResolveFullSelection(selectionBeforeDoubleClick);

            bool hadSelectionBeforeDoubleClick = evt.HadSelectionBeforeFirstClick;
            bool wasSubSelectionBeforeDoubleClick = IsSubSelection(selectionBeforeDoubleClick, fullBeforeDoubleClick);
            bool sameFullPartAsBefore = fullBeforeDoubleClick != null
                && clickedFull != null
                && fullBeforeDoubleClick == clickedFull;

            bool canIsolateSubSelection = clickedIsSubSelection && (
                (hadSelectionBeforeDoubleClick && !wasSubSelectionBeforeDoubleClick && sameFullPartAsBefore) ||
                (hadSelectionBeforeDoubleClick && wasSubSelectionBeforeDoubleClick && selectionBeforeDoubleClick == clickedSelection));

            if (_isolatedSubSelection != null)
            {
                bool sameSubSelection = clickedSelection != null && clickedSelection == _isolatedSubSelection;
                if (sameSubSelection)
                {
                    // If a sub-piece is isolated, double-clicking it returns
                    // to the previous full-part isolation state.
                    RestoreFullIsolationFromStack();
                    return;
                }

                if (canIsolateSubSelection)
                {
                    IsolateSubSelection(clickedSelection, clickedFull);
                    _detailsSheet?.OpenSheet();
                    return;
                }

                IsolateFullSelection(clickedFull ?? clickedSelection);
                _detailsSheet?.OpenSheet();
                return;
            }

            if (_isIsolated)
            {
                if (canIsolateSubSelection)
                {
                    IsolateSubSelection(clickedSelection, clickedFull);
                    _detailsSheet?.OpenSheet();
                    return;
                }

                IsolateFullSelection(clickedFull ?? clickedSelection);
                _detailsSheet?.OpenSheet();
                return;
            }

            if (canIsolateSubSelection)
            {
                IsolateSubSelection(clickedSelection, clickedFull);
            }
            else
            {
                IsolateFullSelection(clickedFull ?? clickedSelection);
            }

            // Double-click isolation should always reveal the details panel.
            _detailsSheet?.OpenSheet();
        }

        private void OnHotspotGroupIsolated(HotspotGroupIsolatedEvent evt)
        {
            _hotspotIsolationActive = evt.Members != null && evt.Members.Count > 0;
            _hotspotIsolationMembers.Clear();
            if (evt.Members != null)
            {
                _hotspotIsolationMembers.AddRange(evt.Members);
            }

            _isIsolated = false;
            _isolatedFullSelection = null;
            _isolatedSubSelection = null;
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _modeController.SetIsolateState(_hotspotIsolationActive);

            if (_hotspotIsolationActive)
            {
                _detailsSheet?.OpenSheet();
            }
        }

        private void OnAppStateChanged(StateChangedEvent evt)
        {
            bool isExploded = evt.NewState == AppState.ExplodedView;
            bool isInteractive = AppStateMachine.Instance?.IsInteractive() ?? false;

            // Sync mode controller with external state changes
            _modeController.SyncWithAppState(evt.NewState);

            // Hotspots: initialize once after hero is dismissed, but do NOT
            // auto-set visibility — that is controlled only by the Pins toggle button.
            bool heroDismissed = _heroController != null && _heroController.HeroDismissed;
            if (heroDismissed && isInteractive && !_hotspotsInitialized)
            {
                _hotspotsInitialized = true;
                HotspotManager.Instance?.Initialize(root);
                // Show hotspots on first init (default state)
                HotspotManager.Instance?.SetVisible(true);
            }

            // Slider visibility now controlled by Explode button toggle, not by state changes
        }

        private void OnViewModeChanged(ViewMode newMode)
        {
            if (_uiAnalyzePanel != null) _uiAnalyzePanel.OnViewModeChanged(newMode);
        }

        private void OnLightBgChanged(bool isLight)
        {
            root?.EnableInClassList("ui-light-bg", isLight);
            _detailsSheet?.SetLightBackground(isLight);
            root?.Query<ProceduralIconBase>().ForEach(icon => icon.SetLightBackground(isLight));
        }

        // ═══════════════════════════════════════════════════════
        //  Button event guards
        // ═══════════════════════════════════════════════════════

        private void RegisterButtonEventGuards()
        {
            root.Query<Button>().ForEach(btn =>
            {
                btn.RegisterCallback(_onBtnDown);
                btn.RegisterCallback(_onBtnUp);

                AddCleanup(() =>
                {
                    btn.UnregisterCallback(_onBtnDown);
                    btn.UnregisterCallback(_onBtnUp);
                });
            });
        }

        // ═══════════════════════════════════════════════════════
        //  Manager auto-creation
        // ═══════════════════════════════════════════════════════

        private void EnsureManagers()
        {
            GameObject managers = GameObject.Find("Managers");
            if (managers == null) managers = new GameObject("Managers");

            if (InputManager.Instance == null) managers.AddComponent<InputManager>();
            if (SelectionManager.Instance == null) managers.AddComponent<SelectionManager>();
            if (ExplodedViewManager.Instance == null) managers.AddComponent<ExplodedViewManager>();
            if (PartCatalogManager.Instance == null) managers.AddComponent<PartCatalogManager>();
            if (NotificationManager.Instance == null) managers.AddComponent<NotificationManager>();
            if (HotspotManager.Instance == null) managers.AddComponent<HotspotManager>();
            if (ViewModeManager.Instance == null) managers.AddComponent<ViewModeManager>();
            if (!ServiceLocator.Has<EnvironmentController>()) managers.AddComponent<EnvironmentController>();
            if (CrossSectionManager.Instance == null) managers.AddComponent<CrossSectionManager>();
            if (PartVisibilityManager.Instance == null) managers.AddComponent<PartVisibilityManager>();
            if (DroneStateController.Instance == null) managers.AddComponent<DroneStateController>();
            if (ThermalSimulationManager.Instance == null) managers.AddComponent<ThermalSimulationManager>();
            if (ThermalViewController.Instance == null) managers.AddComponent<ThermalViewController>();

            GameObject droneRoot = GameObject.Find("x500v2_Drone");
            if (droneRoot != null && droneRoot.GetComponent<ImportedDroneRuntimeBinder>() == null)
            {
                droneRoot.AddComponent<ImportedDroneRuntimeBinder>();
            }
        }
    }
}
