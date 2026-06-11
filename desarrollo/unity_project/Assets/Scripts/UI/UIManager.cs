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
        private bool _isolatedSubSelectionHasAssociatedFasteners = false;
        private Transform _nestedReturnSelection;
        private Transform _nestedReturnFullSelection;
        private bool _nestedReturnHasAssociatedFasteners = false;
        private SubIsolationParentLayer _nestedReturnParentLayer = SubIsolationParentLayer.None;
        private bool _hotspotIsolationActive = false;
        private bool _hotspotIsolationIncludesAssociatedFasteners = false;
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
            HandleLayeredBackNavigation(allowExitFromHeroMain: false);
        }

        public bool HandleBrowserBackNavigation()
        {
            return HandleLayeredBackNavigation(allowExitFromHeroMain: true);
        }

        private bool HandleLayeredBackNavigation(bool allowExitFromHeroMain)
        {
            if (_onboardingController != null && _onboardingController.IsOpen)
            {
                _onboardingController.Dismiss();
                return true;
            }

            if (_detailsSheet != null && _detailsSheet.IsSheetOpen)
            {
                _detailsSheet.SetSheetState(false);
                return true;
            }

            if (_heroController != null && !_heroController.HeroDismissed)
            {
                if (_heroController.HandleBackNavigation())
                {
                    return true;
                }

                if (allowExitFromHeroMain)
                {
                    return _heroController.RequestExitConfirmation();
                }

                return true;
            }

            if (_modeController != null && _modeController.HandleBackNavigation())
            {
                return true;
            }

            if (_heroController != null && _heroController.HeroDismissed)
            {
                _heroController.ReturnToHero();
                return true;
            }

            return false;
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
            return ResolveFullSelection(selection, null);
        }

        private static Transform ResolveFullSelection(Transform selection, Transform preferredContext)
        {
            if (selection == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);
            if (IsValidFastenerMarker(marker))
            {
                if (preferredContext != null && IsFastenerAssociatedWithSelection(selection, preferredContext))
                {
                    return preferredContext;
                }

                if (!string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId))
                {
                    Transform canonicalParent = ResolveCanonicalPartTransform(marker.ParentCanonicalPartId);
                    if (canonicalParent != null)
                    {
                        return canonicalParent;
                    }
                }
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

        private static Transform ResolveCanonicalPartTransform(string canonicalPartId)
        {
            if (string.IsNullOrWhiteSpace(canonicalPartId))
            {
                return null;
            }

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            for (int i = 0; i < parts.Length; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null || part.Data == null)
                {
                    continue;
                }

                if (string.Equals(part.Data.id, canonicalPartId, StringComparison.OrdinalIgnoreCase))
                {
                    return part.transform;
                }
            }

            return null;
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

        private static FastenerRuntimeMarker ResolveFastenerMarker(Transform selection)
        {
            if (selection == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = selection.GetComponent<FastenerRuntimeMarker>();
            if (marker != null)
            {
                return marker;
            }

            // Walk parents manually but STOP after checking the first ExplodablePart
            // boundary. This lets child meshes resolve their own fastener root without
            // capturing sibling markers on shared mother-part ancestors.
            Transform current = selection.parent;
            while (current != null)
            {
                marker = current.GetComponent<FastenerRuntimeMarker>();
                if (marker != null)
                {
                    return marker;
                }

                if (current.GetComponent<ExplodablePart>() != null)
                {
                    break;
                }

                current = current.parent;
            }

            return null;
        }

        private static bool IsFastenerAssociatedWithSelection(Transform selection, Transform fullSelection)
        {
            if (selection == null || fullSelection == null)
            {
                return false;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);
            if (!IsValidFastenerMarker(marker) ||
                string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId))
            {
                return false;
            }

            string fullSelectionPartId = ResolveCanonicalPartId(fullSelection);
            if (!string.IsNullOrWhiteSpace(fullSelectionPartId)
                && string.Equals(marker.ParentCanonicalPartId, fullSelectionPartId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsFastenerGeometricallyAssociatedWithScope(marker.transform, fullSelection);
        }

        private static bool IsFastenerGeometricallyAssociatedWithScope(Transform fastenerTransform, Transform scopeTransform)
        {
            if (fastenerTransform == null || scopeTransform == null)
            {
                return false;
            }

            if (!TryGetWorldBounds(scopeTransform, out Bounds scopeBounds) ||
                !TryGetWorldBounds(fastenerTransform, out Bounds fastenerBounds))
            {
                return false;
            }

            float threshold = Mathf.Max(
                GetDominantSize(fastenerBounds) * 0.9f,
                GetDominantSize(scopeBounds) * 0.008f,
                0.025f);
            Bounds expanded = scopeBounds;
            expanded.Expand(threshold * 2f);
            return expanded.Intersects(fastenerBounds) ||
                   expanded.Contains(fastenerBounds.center) ||
                   CalculateBoundsDistance(scopeBounds, fastenerBounds) <= threshold;
        }

        private static float CalculateBoundsDistance(Bounds a, Bounds b)
        {
            Vector3 aMin = a.min;
            Vector3 aMax = a.max;
            Vector3 bMin = b.min;
            Vector3 bMax = b.max;

            float dx = Mathf.Max(0f, Mathf.Max(aMin.x - bMax.x, bMin.x - aMax.x));
            float dy = Mathf.Max(0f, Mathf.Max(aMin.y - bMax.y, bMin.y - aMax.y));
            float dz = Mathf.Max(0f, Mathf.Max(aMin.z - bMax.z, bMin.z - aMax.z));
            return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private static float GetDominantSize(Bounds bounds)
        {
            return Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        }

        private static bool TryGetWorldBounds(Transform root, out Bounds bounds)
        {
            bounds = default;
            if (root == null)
            {
                return false;
            }

            bool hasBounds = false;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds;
        }

        private static bool IsValidFastenerMarker(FastenerRuntimeMarker marker)
        {
            return marker != null &&
                   marker.SourceIsPrimitiveFastener &&
                   SelectionHierarchy.IsPrimitiveFastenerSource(marker.transform);
        }

        private static bool IsFastenerSelection(Transform selection)
        {
            return IsValidFastenerMarker(ResolveFastenerMarker(selection));
        }

        private bool HotspotContainsFullSelection(Transform fullSelection)
        {
            if (fullSelection == null)
            {
                return false;
            }

            for (int i = 0; i < _hotspotIsolationMembers.Count; i++)
            {
                ExplodablePart member = _hotspotIsolationMembers[i];
                if (member != null && member.transform == fullSelection)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldEnterFullLayerBeforeNestedSelection(Transform nestedSelection, Transform fullSelection)
        {
            if (nestedSelection == null || fullSelection == null || nestedSelection == fullSelection)
            {
                return false;
            }

            if (!IsNestedSelectionLayer(nestedSelection, fullSelection))
            {
                return false;
            }

            if (HasHotspotIsolationLayer && !HotspotContainsFullSelection(fullSelection))
            {
                return false;
            }

            return true;
        }

        private static bool IsNestedSelectionLayer(Transform selection, Transform fullSelection)
        {
            return IsSubSelection(selection, fullSelection)
                || IsFastenerAssociatedWithSelection(selection, fullSelection);
        }

        private static bool CanNestSelectionWithinIsolation(Transform selection, Transform parentIsolation)
        {
            if (selection == null || parentIsolation == null || selection == parentIsolation)
            {
                return false;
            }

            return IsNestedSelectionLayer(selection, parentIsolation);
        }

        private void ClearNestedReturnLayer()
        {
            _nestedReturnSelection = null;
            _nestedReturnFullSelection = null;
            _nestedReturnHasAssociatedFasteners = false;
            _nestedReturnParentLayer = SubIsolationParentLayer.None;
        }

        private void CaptureNestedReturnLayerIfNeeded(bool nextHasAssociatedFasteners)
        {
            if (_isolatedSubSelection != null
                && _isolatedSubSelectionHasAssociatedFasteners
                && !nextHasAssociatedFasteners)
            {
                _nestedReturnSelection = _isolatedSubSelection;
                _nestedReturnFullSelection = _isolatedFullSelection;
                _nestedReturnHasAssociatedFasteners = true;
                _nestedReturnParentLayer = _subIsolationParentLayer;
                return;
            }

            if (nextHasAssociatedFasteners)
            {
                ClearNestedReturnLayer();
            }
        }

        private bool RestoreNestedReturnLayer()
        {
            if (_nestedReturnSelection == null)
            {
                return false;
            }

            Transform returnSelection = _nestedReturnSelection;
            Transform returnFullSelection = _nestedReturnFullSelection;
            bool returnHasAssociatedFasteners = _nestedReturnHasAssociatedFasteners;
            SubIsolationParentLayer returnParentLayer = _nestedReturnParentLayer;
            ClearNestedReturnLayer();

            IsolateSubSelection(
                returnSelection,
                returnFullSelection,
                returnHasAssociatedFasteners,
                returnParentLayer);
            return true;
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
            _isolatedSubSelectionHasAssociatedFasteners = false;
            ClearNestedReturnLayer();
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _modeController.SetIsolateState(true);

            // Center camera on the isolated part
            OrbitCameraController.Instance?.FocusOnObject(fullSelection);
        }

        private void IsolateSubSelection(
            Transform subSelection,
            Transform fullSelection,
            bool includeAssociatedFasteners = true,
            SubIsolationParentLayer parentLayerOverride = SubIsolationParentLayer.None)
        {
            if (subSelection == null)
            {
                return;
            }

            if (fullSelection == null)
            {
                fullSelection = ResolveFullSelection(
                    subSelection,
                    _isolatedFullSelection ?? SelectionManager.Instance?.CurrentFullSelection);
            }

            bool hadIsolatedFullLayer = _isIsolated && _isolatedSubSelection == null && _isolatedFullSelection != null;

            bool isFastener = IsFastenerSelection(subSelection);
            bool shouldIncludeAssociatedFasteners = includeAssociatedFasteners && !isFastener;
            CaptureNestedReturnLayerIfNeeded(shouldIncludeAssociatedFasteners);
            if (shouldIncludeAssociatedFasteners)
            {
                PartVisibilityManager.Instance?.IsolateTransformWithAssociatedFasteners(subSelection);
            }
            else
            {
                PartVisibilityManager.Instance?.IsolateTransformOnly(subSelection);
            }

            _isIsolated = true;
            _isolatedFullSelection = fullSelection;
            _isolatedSubSelection = subSelection;
            _isolatedSubSelectionHasAssociatedFasteners = shouldIncludeAssociatedFasteners;
            if (parentLayerOverride != SubIsolationParentLayer.None)
            {
                _subIsolationParentLayer = parentLayerOverride;
            }
            else if (hadIsolatedFullLayer)
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

            Transform fullSelection = ResolveFullSelection(
                sel,
                _isolatedFullSelection ?? SelectionManager.Instance?.CurrentFullSelection);
            if (fullSelection == null)
            {
                return;
            }

            if (IsNestedSelectionLayer(sel, fullSelection))
            {
                IsolateSubSelection(sel, fullSelection, !IsFastenerSelection(sel));
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

        private void RestoreParentIsolationFromSubSelection()
        {
            if (RestoreNestedReturnLayer())
            {
                return;
            }

            if (_subIsolationParentLayer == SubIsolationParentLayer.HotspotGroup)
            {
                RestoreHotspotIsolation();
                return;
            }

            RestoreFullIsolationFromStack();
        }

        private bool TryPromoteSelectionToNestedIsolation(Transform selection)
        {
            if (!_isIsolated || _isolatedFullSelection == null || selection == null)
            {
                return false;
            }

            if (!CanNestSelectionWithinIsolation(selection, _isolatedFullSelection))
            {
                return false;
            }

            IsolateSubSelection(selection, _isolatedFullSelection, !IsFastenerSelection(selection));
            return true;
        }

        private bool IsCurrentIsolationTarget(Transform selection)
        {
            if (selection == null)
            {
                return false;
            }

            if (_isolatedSubSelection != null)
            {
                return selection == _isolatedSubSelection;
            }

            if (_isolatedFullSelection != null)
            {
                return selection == _isolatedFullSelection;
            }

            return false;
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
                visibility?.IsolateParts(_hotspotIsolationMembers, _hotspotIsolationIncludesAssociatedFasteners);
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
            _isolatedSubSelectionHasAssociatedFasteners = false;
            ClearNestedReturnLayer();
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
            _isolatedSubSelectionHasAssociatedFasteners = false;
            ClearNestedReturnLayer();
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _hotspotIsolationActive = false;
            _hotspotIsolationIncludesAssociatedFasteners = false;
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
            Transform currentSelection = SelectionManager.Instance?.CurrentSelection;

            if (_isIsolated)
            {
                if (currentSelection != null && !IsCurrentIsolationTarget(currentSelection))
                {
                    RequestIsolationForSelection(currentSelection);
                    return;
                }

                if (_isolatedSubSelection != null && _isolatedFullSelection != null)
                {
                    if (currentSelection == _isolatedSubSelection
                        && _isolatedSubSelectionHasAssociatedFasteners
                        && !IsFastenerSelection(_isolatedSubSelection))
                    {
                        IsolateSubSelection(
                            _isolatedSubSelection,
                            _isolatedFullSelection,
                            includeAssociatedFasteners: false);
                    }
                    else
                    {
                        RestoreParentIsolationFromSubSelection();
                    }
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

        public void RequestIsolationForSelection(Transform selection)
        {
            if (selection == null)
            {
                return;
            }

            if (_isIsolated)
            {
                if (_isolatedSubSelection != null && selection == _isolatedSubSelection)
                {
                    if (_isolatedSubSelectionHasAssociatedFasteners && !IsFastenerSelection(selection))
                    {
                        IsolateSubSelection(
                            selection,
                            _isolatedFullSelection,
                            includeAssociatedFasteners: false);
                    }
                    else
                    {
                        RestoreParentIsolationFromSubSelection();
                    }
                    return;
                }

                if (_isolatedFullSelection != null && selection == _isolatedFullSelection)
                {
                    ClearIsolation();
                    return;
                }

                if (TryPromoteSelectionToNestedIsolation(selection))
                {
                    return;
                }
            }

            Transform fullSelection = ResolveFullSelection(
                selection,
                _isolatedFullSelection ?? SelectionManager.Instance?.CurrentFullSelection);
            if (fullSelection == null)
            {
                return;
            }

            if (CanNestSelectionWithinIsolation(selection, fullSelection))
            {
                IsolateSubSelection(selection, fullSelection, !IsFastenerSelection(selection));
            }
            else
            {
                IsolateFullSelection(fullSelection);
            }
        }

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
                // Step back one navigation layer at a time; do not skip the
                // intermediate "subpiece + associated fasteners" state.
                if (_isolatedSubSelection != null && _isolatedFullSelection != null)
                {
                    RestoreParentIsolationFromSubSelection();
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

            Transform selectionBeforeDoubleClick = evt.SelectionBeforeFirstClick;
            Transform fullBeforeDoubleClick = ResolveFullSelection(
                selectionBeforeDoubleClick,
                _isolatedFullSelection ?? SelectionManager.Instance?.CurrentFullSelection);
            if (fullBeforeDoubleClick == null)
            {
                fullBeforeDoubleClick = evt.FullSelectionBeforeFirstClick;
            }

            Transform preferredFullContext = _isolatedFullSelection
                ?? fullBeforeDoubleClick
                ?? SelectionManager.Instance?.CurrentFullSelection;
            Transform clickedFull = ResolveFullSelection(clickedSelection, preferredFullContext);
            if (clickedFull == null)
            {
                clickedFull = evt.FullPartTransform;
            }

            bool clickedIsFastener = IsFastenerSelection(clickedSelection);
            bool clickedIsSubSelection = IsNestedSelectionLayer(clickedSelection, clickedFull);

            if (_isIsolated
                && _isolatedSubSelection == null
                && _isolatedFullSelection != null
                && clickedFull == _isolatedFullSelection
                && !clickedIsSubSelection
                && !clickedIsFastener)
            {
                ClearIsolation();
                return;
            }

            bool hadSelectionBeforeDoubleClick = evt.HadSelectionBeforeFirstClick;
            bool wasSubSelectionBeforeDoubleClick = IsNestedSelectionLayer(selectionBeforeDoubleClick, fullBeforeDoubleClick);
            bool sameFullPartAsBefore = fullBeforeDoubleClick != null
                && clickedFull != null
                && fullBeforeDoubleClick == clickedFull;
            bool hotspotAllowsDirectNestedIsolation = HasHotspotIsolationLayer
                && !HotspotContainsFullSelection(clickedFull);

            bool canIsolateSubSelection = clickedIsSubSelection && (
                (hadSelectionBeforeDoubleClick && !wasSubSelectionBeforeDoubleClick && sameFullPartAsBefore) ||
                (hadSelectionBeforeDoubleClick && wasSubSelectionBeforeDoubleClick && selectionBeforeDoubleClick == clickedSelection) ||
                hotspotAllowsDirectNestedIsolation);

            bool canNestWithinCurrentIsolation = _isIsolated
                && _isolatedSubSelection == null
                && clickedSelection != null
                && clickedSelection != _isolatedFullSelection
                && CanNestSelectionWithinIsolation(clickedSelection, _isolatedFullSelection);

            if (!_isIsolated
                && clickedIsSubSelection
                && !canIsolateSubSelection
                && ShouldEnterFullLayerBeforeNestedSelection(clickedSelection, clickedFull))
            {
                IsolateFullSelection(clickedFull);
                return;
            }

            if (_isolatedSubSelection != null)
            {
                bool sameSubSelection = clickedSelection != null && clickedSelection == _isolatedSubSelection;
                if (sameSubSelection)
                {
                    if (_isolatedSubSelectionHasAssociatedFasteners && !clickedIsFastener)
                    {
                        IsolateSubSelection(
                            _isolatedSubSelection,
                            _isolatedFullSelection,
                            includeAssociatedFasteners: false);
                    }
                    else
                    {
                        RestoreParentIsolationFromSubSelection();
                    }
                    return;
                }

                if (clickedSelection != null
                    && clickedSelection != _isolatedSubSelection
                    && TryPromoteSelectionToNestedIsolation(clickedSelection))
                {
                    return;
                }

                if (canIsolateSubSelection)
                {
                    IsolateSubSelection(clickedSelection, clickedFull, !clickedIsFastener);
                    return;
                }

                IsolateFullSelection(clickedFull ?? clickedSelection);
                return;
            }

            if (_isIsolated)
            {
                if (canNestWithinCurrentIsolation && TryPromoteSelectionToNestedIsolation(clickedSelection))
                {
                    return;
                }

                if (canIsolateSubSelection)
                {
                    IsolateSubSelection(clickedSelection, clickedFull, !clickedIsFastener);
                    return;
                }

                IsolateFullSelection(clickedFull ?? clickedSelection);
                return;
            }

            if (canIsolateSubSelection)
            {
                IsolateSubSelection(clickedSelection, clickedFull, !clickedIsFastener);
            }
            else
            {
                IsolateFullSelection(clickedFull ?? clickedSelection);
            }
        }

        private void OnHotspotGroupIsolated(HotspotGroupIsolatedEvent evt)
        {
            _hotspotIsolationActive = evt.Members != null && evt.Members.Count > 0;
            _hotspotIsolationIncludesAssociatedFasteners = evt.IncludeAssociatedFasteners;
            _hotspotIsolationMembers.Clear();
            if (evt.Members != null)
            {
                _hotspotIsolationMembers.AddRange(evt.Members);
            }

            _isIsolated = false;
            _isolatedFullSelection = null;
            _isolatedSubSelection = null;
            _isolatedSubSelectionHasAssociatedFasteners = false;
            ClearNestedReturnLayer();
            _subIsolationParentLayer = SubIsolationParentLayer.None;
            _modeController.SetIsolateState(_hotspotIsolationActive);

            // Hotspot double-tap isolates the system only. The info panel opens
            // from its dedicated tab/gesture so isolation never feels aggressive.
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
