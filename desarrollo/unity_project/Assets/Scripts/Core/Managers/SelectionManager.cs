using UnityEngine;
using WebGL.Core.Data;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
using System.Collections.Generic;
using System.Reflection;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Manages object selection via raycasting.
    /// Handles hover highlighting and click selection.
    /// Publishes PartSelectedEvent on selection changes and
    /// PartDoubleClickedEvent on double-click / double-tap.
    /// </summary>
    public class SelectionManager : Singleton<SelectionManager>
    {
        private static readonly bool EnableDebugLogs = false;

        #region Constants

        private const float DOUBLE_CLICK_THRESHOLD = 0.35f;

        #endregion

        #region Serialized Fields

        [Header("Raycast Settings")]
        [SerializeField] 
        [Tooltip("Layer mask for selectable objects")]
        private LayerMask selectionLayer;
        
        [SerializeField] 
        [Tooltip("Maximum distance for selection raycast")]
        private float maxRayDistance = 100f;

        #endregion

        #region Private Fields

        private Transform currentSelection;
        private Transform currentFullSelection;
        private Transform currentSubSelection;
        private bool hotspotGroupSelectionActive;
        private Transform hoveredObject;
        private Transform hoveredRawTransform;
        private HighlightSystem currentHighlight;
        private HighlightSystem hoveredHighlight;
        private bool hoveredEffectApplied;
        private readonly List<HighlightSystem> currentAssociatedHighlights = new List<HighlightSystem>();

        // Double-click tracking
        private const float DOUBLE_CLICK_MAX_SCREEN_DISTANCE = 72f;
        private float _lastClickTime;
        private string _lastClickId;
        private Vector2 _lastClickScreenPosition;
        private bool _lastClickHadSelection;
        private Transform _lastClickSelection;
        private Transform _lastClickFullSelection;
        private Transform _lastClickedTransform;
        private Transform _lastClickedFullTransform;
        private DronePartData _lastClickedData;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the currently selected transform.
        /// </summary>
        public Transform CurrentSelection => currentSelection;
        public Transform CurrentFullSelection => currentFullSelection;
        public Transform CurrentSubSelection => currentSubSelection;

        /// <summary>
        /// Gets the currently hovered transform.
        /// </summary>
        public Transform HoveredObject => hoveredObject;

        /// <summary>
        /// Returns true if any object is currently selected.
        /// </summary>
        public bool HasSelection => currentSelection != null;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (selectionLayer.value == 0)
            {
                int selectableLayer = LayerMask.NameToLayer("SelectablePart");
                selectionLayer = selectableLayer >= 0 ? (1 << selectableLayer) : ~0;
            }
        }

        private void Update()
        {
            if (!ShouldProcessInput()) return;

            HandleHover();
            HandleClick();
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Determines if selection input should be processed based on app state.
        /// </summary>
        private bool ShouldProcessInput()
        {
            return AppStateMachine.Instance != null 
                ? AppStateMachine.Instance.IsInteractive() 
                : true; // Default to allowing input if state machine unavailable
        }

        /// <summary>
        /// Handles hover detection and highlighting.
        /// </summary>
        private void HandleHover()
        {
            if (Camera.main == null) return;

            // Selection should yield to any visible UI so hovering a button/panel never
            // highlights or clicks through to the 3D model underneath.
            if (InputManager.InputBlocked) 
            {
                ClearHover();
                return;
            }
            if (InputManager.Instance != null && InputManager.Instance.IsPointerOverSelectionBlockingUI())
            {
                ClearHover();
                return;
            }

            Camera mainCamera = Camera.main;
            if (!TryGetValidPointerScreenPosition(mainCamera, out Vector3 pointerPosition))
            {
                ClearHover();
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, selectionLayer))
            {
                hoveredRawTransform = hit.transform;
                Transform newHover = ResolveSelectableTransform(hit.transform);

                if (newHover != hoveredObject)
                {
                    ExitCurrentHover();
                    EnterNewHover(newHover);
                }
            }
            else
            {
                hoveredRawTransform = null;
                ClearHover();
            }
        }

        /// <summary>
        /// Exits the current hover state.
        /// </summary>
        private void ExitCurrentHover()
        {
            if (hoveredEffectApplied && hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverExit();
            }

            hoveredEffectApplied = false;
        }

        /// <summary>
        /// Enters a new hover state.
        /// </summary>
        /// <param name="newHover">The new transform to hover.</param>
        private void EnterNewHover(Transform newHover)
        {
            hoveredObject = newHover;
            Transform hoverHighlightTarget = ResolveHighlightTarget(hoveredObject);
            hoveredHighlight = hoverHighlightTarget != null ? hoverHighlightTarget.GetComponent<HighlightSystem>() : null;
            if (hoveredHighlight == null && hoverHighlightTarget != null)
            {
                hoveredHighlight = hoverHighlightTarget.GetComponentInParent<HighlightSystem>();
            }
            
            bool suppressHoverEffect = ShouldSuppressHoverEffect(hoveredObject);
            if (!suppressHoverEffect && hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverEnter();
                hoveredEffectApplied = true;
            }
            else
            {
                hoveredEffectApplied = false;
            }

            UpdateCursor(CursorType.Pointer);
        }

        /// <summary>
        /// Clears the current hover state.
        /// </summary>
        private void ClearHover()
        {
            if (hoveredEffectApplied && hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverExit();
            }

            hoveredObject = null;
            hoveredRawTransform = null;
            hoveredHighlight = null;
            hoveredEffectApplied = false;

            UpdateCursor(CursorType.Default);
        }

        /// <summary>
        /// Updates the cursor appearance.
        /// </summary>
        /// <param name="cursorType">The cursor type to set.</param>
        private void UpdateCursor(CursorType cursorType)
        {
            if (ServiceLocator.TryGet<CursorManager>(out var cursor))
            {
                if (cursorType == CursorType.Default)
                    cursor.ResetCursor();
                else
                    cursor.SetCursor(cursorType);
            }
        }

        /// <summary>
        /// Handles click input for selection.
        /// </summary>
        private void HandleClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            LogDebug("[SelectionManager.HandleClick] LMB clicked");
            
            // Phase 4: Respect centralized input block (set by UI pointer events)
            if (InputManager.InputBlocked)
            {
                bool stillOverBlockingUI = InputManager.Instance != null && InputManager.Instance.IsPointerOverBlockingUIRaw();
                if (!stillOverBlockingUI)
                {
                    // Recover from stale block states (missed pointer-up / focus transitions).
                    InputManager.InputBlocked = false;
                    LogWarning("[SelectionManager.HandleClick] Recovered stale InputBlocked=true");
                }
                else
                {
                    string reason = InputManager.Instance != null
                        ? InputManager.Instance.GetSelectionBlockDebugReason()
                        : "InputManager null";
                    LogWarning($"[SelectionManager.HandleClick] BLOCKED by InputBlocked. reason={reason}");
                    return;
                }
            }
            
            // UI clicks should never be interpreted as background deselection clicks.
            bool isOverUI = InputManager.Instance != null && InputManager.Instance.IsPointerOverSelectionBlockingUI();
            LogDebug($"[SelectionManager.HandleClick] IsPointerOverSelectionBlockingUI = {isOverUI}");
            
            if (isOverUI)
            {
                string reason = InputManager.Instance != null
                    ? InputManager.Instance.GetSelectionBlockDebugReason()
                    : "InputManager null";
                LogWarning($"[SelectionManager.HandleClick] BLOCKED by IsPointerOverSelectionBlockingUI. reason={reason}");
                return;
            }

            // ── Double-click / double-tap detection ──
            if (!TryGetValidPointerScreenPosition(Camera.main, out Vector3 pointerPosition))
            {
                LogDebug("[SelectionManager.HandleClick] Ignored click with invalid pointer position");
                return;
            }

            float now = Time.time;
            Vector2 clickScreenPosition = new Vector2(pointerPosition.x, pointerPosition.y);
            Transform selectionBeforeClick = currentSelection;
            Transform fullSelectionBeforeClick = ResolvePrimarySelection(selectionBeforeClick);
            bool hadSelectionBeforeClick = selectionBeforeClick != null;

            DronePartData clickedData = null;
            Transform clickedSelection = hoveredObject;
            Transform clickedFull = ResolvePrimarySelection(hoveredRawTransform != null ? hoveredRawTransform : hoveredObject);
            if (clickedFull != null)
            {
                var exp = clickedFull.GetComponent<ExplodablePart>();
                if (exp == null)
                {
                    exp = clickedFull.GetComponentInParent<ExplodablePart>();
                }

                clickedData = exp != null ? exp.Data : null;
            }

            string clickId = clickedSelection != null
                ? clickedSelection.GetInstanceID().ToString()
                : "__bg__";
            bool withinDoubleClickWindow = (now - _lastClickTime) < DOUBLE_CLICK_THRESHOLD;
            bool sameClickedObject = clickId == _lastClickId;
            bool sameScreenIntent = _lastClickedTransform != null
                && (clickScreenPosition - _lastClickScreenPosition).sqrMagnitude <=
                   DOUBLE_CLICK_MAX_SCREEN_DISTANCE * DOUBLE_CLICK_MAX_SCREEN_DISTANCE;
            bool useFirstClickTarget = withinDoubleClickWindow && !sameClickedObject && sameScreenIntent;
            bool isDoubleClick = withinDoubleClickWindow && (sameClickedObject || useFirstClickTarget);

            if (isDoubleClick)
            {
                Transform eventClickedSelection = useFirstClickTarget ? _lastClickedTransform : clickedSelection;
                Transform eventClickedFull = useFirstClickTarget ? _lastClickedFullTransform : clickedFull;
                DronePartData eventClickedData = useFirstClickTarget ? _lastClickedData : clickedData;

                EventBus.Publish(new PartDoubleClickedEvent(
                    eventClickedData,
                    eventClickedSelection,
                    eventClickedFull,
                    eventClickedSelection == null,
                    _lastClickHadSelection,
                    _lastClickSelection,
                    _lastClickFullSelection));
                _lastClickTime = 0f;
                _lastClickId = null;
                _lastClickHadSelection = false;
                _lastClickSelection = null;
                _lastClickFullSelection = null;
                _lastClickedTransform = null;
                _lastClickedFullTransform = null;
                _lastClickedData = null;
                return; // Skip normal selection on double-click
            }

            // Track for next potential double-click
            _lastClickTime = now;
            _lastClickId = clickId;
            _lastClickScreenPosition = clickScreenPosition;
            _lastClickHadSelection = hadSelectionBeforeClick;
            _lastClickSelection = selectionBeforeClick;
            _lastClickFullSelection = fullSelectionBeforeClick;
            _lastClickedTransform = clickedSelection;
            _lastClickedFullTransform = clickedFull;
            _lastClickedData = clickedData;

            // ── Normal selection logic ──
            if (hoveredObject == null)
            {
                LogDebug("[SelectionManager.HandleClick] hoveredObject == null (background click)");
                
                // Background click → block only when the pointer is over actual UI controls.
                bool blockDeselection = false;

                if (InputManager.Instance != null && InputManager.Instance.IsPointerOverSelectionBlockingUI())
                {
                    LogDebug("[SelectionManager.HandleClick] Block: Pointer over blocking UI");
                    blockDeselection = true;
                }
                
                if (blockDeselection)
                {
                    LogDebug("[SelectionManager.HandleClick] Return: Deselection blocked");
                    return;  // Don't deselect when clicking UI
                }

                // Background click behavior:
                // 1) If a subpiece is active, go back to full parent selection.
                // 2) If only full parent is active, fully deselect.
                if (currentSubSelection != null && currentFullSelection != null)
                {
                    LogDebug("[SelectionManager.HandleClick] Background → step back to full selection");
                    SelectObject(currentFullSelection);
                }
                else
                {
                    LogDebug("[SelectionManager.HandleClick] Deselecting full selection...");
                    Deselect();
                }
                return;
            }

            Transform clickedRaw = hoveredObject;
            Transform clickedFullSelection = ResolvePrimarySelection(hoveredRawTransform != null ? hoveredRawTransform : hoveredObject);
            bool clickedIsSubSelection = clickedRaw != null
                && clickedFullSelection != null
                && clickedRaw != clickedFullSelection
                && clickedRaw.IsChildOf(clickedFullSelection);
            FastenerRuntimeMarker clickedFastenerMarker = ResolveFastenerMarker(clickedRaw);
            bool clickedIsFastener = IsValidFastenerMarker(clickedFastenerMarker);
            Transform clickedFastenerRoot = clickedIsFastener ? ResolveFastenerSelectionRoot(clickedRaw) : null;
            Transform clickedFastenerParent = clickedIsFastener
                ? ResolveCanonicalPartTransform(clickedFastenerMarker.ParentCanonicalPartId)
                : null;

            if (hotspotGroupSelectionActive)
            {
                // First mesh click after hotspot-group selection enters the clicked layer:
                // subpiece -> subpiece, fastener -> fastener, parent -> parent.
                hotspotGroupSelectionActive = false;
                EventBus.Publish(new HotspotGroupVisualsClearRequestedEvent());
                if (clickedIsFastener && clickedFastenerRoot != null)
                {
                    SelectObject(clickedFastenerRoot);
                }
                else if (clickedIsSubSelection)
                {
                    SelectObject(clickedRaw);
                }
                else
                {
                    SelectObject(clickedFullSelection != null ? clickedFullSelection : clickedRaw);
                }
                return;
            }

            if (currentFullSelection == null)
            {
                // First click on a fastener selects its canonical mother part.
                // A second click inside that context can then drill into the fastener itself.
                if (clickedIsFastener && clickedFastenerParent != null)
                {
                    SelectObject(clickedFastenerParent);
                    return;
                }

                // First click on regular geometry always selects full parent piece.
                SelectObject(clickedFullSelection != null ? clickedFullSelection : clickedRaw);
                return;
            }

            if (clickedIsFastener)
            {
                string currentCanonicalId = ResolveCanonicalPartId(currentFullSelection);
                bool sameCanonicalContext = !string.IsNullOrWhiteSpace(currentCanonicalId) &&
                                            string.Equals(
                                                currentCanonicalId,
                                                clickedFastenerMarker.ParentCanonicalPartId,
                                                System.StringComparison.OrdinalIgnoreCase);
                bool compatibleWithCurrentContext = sameCanonicalContext ||
                                                    IsFastenerCompatibleWithSelectionContext(
                                                        clickedFastenerRoot != null ? clickedFastenerRoot : clickedRaw,
                                                        currentSubSelection,
                                                        currentFullSelection);

                if (compatibleWithCurrentContext && clickedFastenerRoot != null)
                {
                    SelectObject(clickedFastenerRoot, contextFullSelection: currentFullSelection);
                    return;
                }

                if (clickedFastenerParent != null)
                {
                    SelectObject(clickedFastenerParent);
                    return;
                }
            }

            if (clickedFullSelection == currentFullSelection)
            {
                if (currentSubSelection == null)
                {
                    // Parent is selected; clicking a subpiece switches to that subpiece.
                    if (clickedIsSubSelection)
                    {
                        SelectObject(clickedRaw);
                    }
                    // Clicking parent again is a no-op.
                    return;
                }

                // Subpiece is selected under same parent.
                if (clickedIsSubSelection)
                {
                    if (clickedRaw == currentSubSelection)
                    {
                        return;
                    }

                    SelectObject(clickedRaw);
                    return;
                }

                // Clicking parent while subpiece selected returns to full parent selection.
                SelectObject(currentFullSelection);
                return;
            }

            // Clicked another parent branch: start over with that full parent selection.
            SelectObject(clickedFullSelection != null ? clickedFullSelection : clickedRaw);
        }

        #endregion

        #region Selection Methods

        /// <summary>
        /// Selects the specified object.
        /// </summary>
        /// <param name="selection">The transform to select.</param>
        /// <param name="fromHotspot">True if triggered by a hotspot click.</param>
        public void SelectObject(
            Transform selection,
            bool fromHotspot = false,
            string hotspotGroupLabel = "",
            string hotspotGroupSummary = "",
            string hotspotGroupMembers = "",
            Transform contextFullSelection = null)
        {
            if (selection == null) return;

            if (hoveredEffectApplied && hoveredHighlight != null)
            {
                hoveredHighlight.OnHoverExit();
                hoveredEffectApplied = false;
            }

            // Clean up previous selection highlight before applying new one
            ClearAssociatedSelectionHighlights();
            if (currentHighlight != null && currentSelection != selection)
            {
                currentHighlight.OnDeselect();
            }

            Transform fullSelection = contextFullSelection != null
                ? contextFullSelection
                : ResolvePrimarySelection(selection);
            bool isFullPartSelection = fullSelection != null && selection == fullSelection;

            currentFullSelection = fullSelection != null ? fullSelection : selection;
            currentSubSelection = isFullPartSelection ? null : selection;

            currentSelection = selection;
            Transform highlightTarget = isFullPartSelection ? fullSelection : ResolveHighlightTarget(selection);
            if (highlightTarget == null)
            {
                highlightTarget = selection;
            }

            currentHighlight = highlightTarget != null ? highlightTarget.GetComponent<HighlightSystem>() : null;

            // For full-part selection we can reuse parent-level highlight components.
            // For subpiece selection we intentionally avoid parent fallback so only the
            // clicked subpiece receives the hard highlight.
            if (currentHighlight == null && highlightTarget != null && isFullPartSelection)
            {
                currentHighlight = highlightTarget.GetComponentInParent<HighlightSystem>();
            }

            if (currentHighlight == null && highlightTarget != null)
            {
                currentHighlight = highlightTarget.gameObject.AddComponent<HighlightSystem>();
            }

            // Apply highlight
            if (currentHighlight != null)
            {
                HighlightSystem.SelectionVisualMode visualMode =
                    isFullPartSelection && !IsFastenerSelection(selection)
                        ? HighlightSystem.SelectionVisualMode.SoftTint
                        : HighlightSystem.SelectionVisualMode.FillPulse;

                currentHighlight.OnSelect(visualMode);
            }

            ApplyAssociatedFastenerHighlights(selection, isFullPartSelection);

            // Get data and publish event
            var explodable = selection.GetComponent<ExplodablePart>();
            if (explodable == null)
            {
                explodable = selection.GetComponentInParent<ExplodablePart>();
            }
            if (explodable != null && explodable.Data != null)
            {
                hotspotGroupSelectionActive = fromHotspot;

                string selectionLabel = BuildSelectionLabel(selection, explodable.transform, explodable.Data.partName);
                if (fromHotspot && !string.IsNullOrWhiteSpace(hotspotGroupLabel))
                {
                    selectionLabel = hotspotGroupLabel;
                }

                EventBus.Publish(new PartSelectedEvent(
                    explodable.Data,
                    fromHotspot,
                    hotspotGroupLabel,
                    hotspotGroupSummary,
                    hotspotGroupMembers,
                    selectionLabel,
                    explodable.Data.partName));
                
                // Track analytics
                if (ServiceLocator.TryGet<AnalyticsManager>(out var analytics))
                {
                    analytics.TrackPartSelected(explodable.Data.partName);
                }
            }

            // Play feedback sound
            AudioManager.Instance?.PlayClick();

            // A hover can be active on the clicked subpiece before this selection is promoted
            // to full parent. If we keep the hover flag, the next hover-exit can clear the
            // property block and accidentally remove the parent soft tint.
            hoveredEffectApplied = false;

            LogDebug($"[SelectionManager] Selected: {selection.name}");
        }

        private static string BuildSelectionLabel(Transform selection, Transform partRoot, string canonicalPartName)
        {
            if (selection == null)
            {
                return string.Empty;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);
            if (IsValidFastenerMarker(marker) && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
            {
                FastenerMetadata metadata = FastenerRegistry.Instance != null
                    ? FastenerRegistry.Instance.ResolveMetadata(selection)
                    : null;

                if (metadata != null)
                {
                    string fastenerLabel = metadata.GetDisplayName();
                    if (!string.IsNullOrWhiteSpace(fastenerLabel))
                    {
                        return fastenerLabel;
                    }
                }
            }

            if (partRoot == null || selection == partRoot)
            {
                return !string.IsNullOrWhiteSpace(canonicalPartName) ? canonicalPartName : selection.name;
            }

            // For subpiece selection, show only the clicked subpiece label.
            return selection.name;
        }

        private Transform ResolveSelectableTransform(Transform rawTransform)
        {
            if (rawTransform == null)
            {
                return null;
            }

            Transform fastenerRoot = ResolveFastenerSelectionRoot(rawTransform);
            if (fastenerRoot != null)
            {
                return fastenerRoot;
            }

            // Preserve the exact clicked transform for non-fastener parts so parent and
            // subpiece clicks remain distinct.
            return rawTransform;
        }

        private static Transform ResolveFastenerSelectionRoot(Transform rawTransform)
        {
            if (rawTransform == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(rawTransform);
            if (IsValidFastenerMarker(marker))
            {
                return marker.transform;
            }

            ExplodablePart direct = rawTransform.GetComponent<ExplodablePart>();
            if (IsFastenerPart(direct))
            {
                return direct.transform;
            }

            // If the raw transform is a child of a Fastener ExplodablePart (group),
            // return the rawTransform itself - NOT the group parent.
            // Returning the group would collapse all siblings into one selection,
            // defeating instance-level isolation.
            ExplodablePart parent = rawTransform.GetComponentInParent<ExplodablePart>();
            return IsFastenerPart(parent) ? rawTransform : null;
        }

        private static bool IsFastenerPart(ExplodablePart part)
        {
            return part != null && part.Data != null && part.Data.category == PartCategory.Fasteners;
        }

        private static bool IsFastenerSelection(Transform selection)
        {
            if (selection == null)
            {
                return false;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(selection);

            if (IsValidFastenerMarker(marker) && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
            {
                return true;
            }

            ExplodablePart part = selection.GetComponent<ExplodablePart>();
            if (part == null)
            {
                part = selection.GetComponentInParent<ExplodablePart>();
            }

            return IsFastenerPart(part);
        }

        private static FastenerRuntimeMarker ResolveFastenerMarker(Transform target)
        {
            if (target == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = target.GetComponent<FastenerRuntimeMarker>();
            if (marker != null)
            {
                return marker;
            }

            // Walk parents manually but STOP after checking the first ExplodablePart
            // boundary. This lets child meshes resolve their own fastener root without
            // capturing sibling markers on shared mother-part ancestors.
            Transform current = target.parent;
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

        private static bool IsValidFastenerMarker(FastenerRuntimeMarker marker)
        {
            return marker != null &&
                   marker.SourceIsPrimitiveFastener &&
                   SelectionHierarchy.IsPrimitiveFastenerSource(marker.transform);
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

                if (string.Equals(part.Data.id, canonicalPartId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return part.transform;
                }
            }

            return null;
        }

        private static bool IsFastenerCompatibleWithSelectionContext(
            Transform fastenerTransform,
            Transform currentSubSelection,
            Transform currentFullSelection)
        {
            if (fastenerTransform == null || currentFullSelection == null)
            {
                return false;
            }

            FastenerRuntimeMarker marker = ResolveFastenerMarker(fastenerTransform);
            if (!IsValidFastenerMarker(marker) || string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId))
            {
                return false;
            }

            string contextCanonicalPartId = ResolveCanonicalPartId(currentFullSelection);
            HashSet<string> canonicalScopeIds = BuildCanonicalPartScopeIds(contextCanonicalPartId);
            return SelectionHierarchy.FastenerBelongsToCanonicalScope(marker, canonicalScopeIds);
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

            float threshold = ResolveFastenerContextThreshold(GetDominantSize(scopeBounds), GetDominantSize(fastenerBounds));
            Bounds expanded = scopeBounds;
            expanded.Expand(threshold * 2f);
            return expanded.Intersects(fastenerBounds) ||
                   expanded.Contains(fastenerBounds.center) ||
                   CalculateBoundsDistance(scopeBounds, fastenerBounds) <= threshold;
        }

        private static float ResolveFastenerContextThreshold(float scopeDominantSize, float fastenerDominantSize)
        {
            return Mathf.Max(fastenerDominantSize * 0.9f, scopeDominantSize * 0.008f, 0.025f);
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

        private void ApplyAssociatedFastenerHighlights(Transform selection, bool isFullPartSelection)
        {
            if (selection == null || !isFullPartSelection || IsFastenerSelection(selection))
            {
                return;
            }

            string canonicalPartId = ResolveCanonicalPartId(selection);
            if (string.IsNullOrWhiteSpace(canonicalPartId))
            {
                return;
            }

            HashSet<string> canonicalScopeIds = BuildCanonicalPartScopeIds(canonicalPartId);
            ApplyAssociatedAssemblyPartHighlights(canonicalPartId);

            FastenerRuntimeMarker[] markers = FindObjectsByType<FastenerRuntimeMarker>(FindObjectsSortMode.None);
            for (int i = 0; i < markers.Length; i++)
            {
                FastenerRuntimeMarker marker = markers[i];
                if (!IsValidFastenerMarker(marker) ||
                    string.IsNullOrWhiteSpace(marker.ParentCanonicalPartId) ||
                    !SelectionHierarchy.FastenerBelongsToCanonicalScope(marker, canonicalScopeIds))
                {
                    continue;
                }

                Transform highlightTarget = ResolveHighlightTarget(marker.transform);
                if (highlightTarget == null)
                {
                    continue;
                }

                HighlightSystem highlight = highlightTarget.GetComponent<HighlightSystem>();
                if (highlight == null)
                {
                    highlight = highlightTarget.gameObject.AddComponent<HighlightSystem>();
                }

                if (highlight == null || highlight == currentHighlight || currentAssociatedHighlights.Contains(highlight))
                {
                    continue;
                }

                highlight.OnSelect(HighlightSystem.SelectionVisualMode.SoftTint);
                currentAssociatedHighlights.Add(highlight);
            }
        }

        private void ApplyAssociatedAssemblyPartHighlights(string canonicalPartId)
        {
            HashSet<string> companionIds = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            SelectionHierarchy.AddAssemblyCompanionCanonicalIds(canonicalPartId, companionIds);
            if (companionIds.Count == 0)
            {
                return;
            }

            ExplodablePart[] parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            for (int i = 0; i < parts.Length; i++)
            {
                ExplodablePart part = parts[i];
                if (part == null ||
                    part.Data == null ||
                    string.IsNullOrWhiteSpace(part.Data.id) ||
                    !companionIds.Contains(part.Data.id))
                {
                    continue;
                }

                Transform highlightTarget = ResolveHighlightTarget(part.transform);
                if (highlightTarget == null)
                {
                    continue;
                }

                HighlightSystem highlight = highlightTarget.GetComponent<HighlightSystem>();
                if (highlight == null)
                {
                    highlight = highlightTarget.gameObject.AddComponent<HighlightSystem>();
                }

                if (highlight == null || highlight == currentHighlight || currentAssociatedHighlights.Contains(highlight))
                {
                    continue;
                }

                highlight.OnSelect(HighlightSystem.SelectionVisualMode.SoftTint);
                currentAssociatedHighlights.Add(highlight);
            }
        }

        private static HashSet<string> BuildCanonicalPartScopeIds(string canonicalPartId)
        {
            HashSet<string> canonicalScopeIds = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(canonicalPartId))
            {
                return canonicalScopeIds;
            }

            canonicalScopeIds.Add(canonicalPartId);
            SelectionHierarchy.AddAssemblyCompanionCanonicalIds(canonicalPartId, canonicalScopeIds);
            return canonicalScopeIds;
        }

        private void ClearAssociatedSelectionHighlights()
        {
            for (int i = 0; i < currentAssociatedHighlights.Count; i++)
            {
                HighlightSystem highlight = currentAssociatedHighlights[i];
                if (highlight != null && highlight != currentHighlight)
                {
                    highlight.OnDeselect();
                }
            }

            currentAssociatedHighlights.Clear();
        }

        private static string ResolveCanonicalPartId(Transform selection)
        {
            if (selection == null)
            {
                return string.Empty;
            }

            PartRenderCategory renderCategory = selection.GetComponent<PartRenderCategory>();
            if (renderCategory != null && !string.IsNullOrWhiteSpace(renderCategory.CanonicalPartId))
            {
                return renderCategory.CanonicalPartId;
            }

            ExplodablePart part = selection.GetComponent<ExplodablePart>();
            if (part == null)
            {
                part = selection.GetComponentInParent<ExplodablePart>();
            }

            return part != null && part.Data != null ? part.Data.id ?? string.Empty : string.Empty;
        }

        private static Transform ResolveHighlightTarget(Transform selection)
        {
            if (selection == null)
            {
                return null;
            }

            if (selection.GetComponent<Renderer>() != null)
            {
                return selection;
            }

            if (selection.GetComponent<HighlightSystem>() != null)
            {
                return selection;
            }

            Renderer[] renderers = selection.GetComponentsInChildren<Renderer>(true);
            if (renderers != null)
            {
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null)
                    {
                        return renderer.transform;
                    }
                }
            }

            return selection;
        }

        private static Transform ResolvePrimarySelection(Transform rawTransform)
        {
            if (rawTransform == null)
            {
                return null;
            }

            ExplodablePart direct = rawTransform.GetComponent<ExplodablePart>();
            if (direct != null)
            {
                return direct.transform;
            }

            ExplodablePart parent = rawTransform.GetComponentInParent<ExplodablePart>();
            if (parent != null)
            {
                return parent.transform;
            }

            return rawTransform;
        }

        private bool ShouldSuppressHoverEffect(Transform hoverTarget)
        {
            if (hoverTarget == null)
            {
                return false;
            }

            // If full parent is selected (no active subpiece), hovering descendants should not
            // temporarily override/reset property blocks, otherwise the parent soft highlight is lost.
            if (currentFullSelection != null
                && currentSubSelection == null
                && currentSelection == currentFullSelection
                && hoverTarget != currentFullSelection
                && hoverTarget.IsChildOf(currentFullSelection))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deselects the currently selected object.
        /// </summary>
        public void Deselect()
        {
            // if (currentSelection == null) return; // User Fix: Allow event firing even if null to trigger UI close

            // Remove highlight
            ClearAssociatedSelectionHighlights();
            if (currentHighlight != null)
            {
                currentHighlight.OnDeselect();
            }

            // Publish null selection event (ALWAYS, even if already null, to signal "Background Clicked")
            EventBus.Publish(new PartSelectedEvent(null));

            currentSelection = null;
            currentFullSelection = null;
            currentSubSelection = null;
            hotspotGroupSelectionActive = false;
            currentHighlight = null;

            if (hoveredObject != null && hoveredHighlight != null)
            {
                hoveredHighlight.OnHoverEnter();
                hoveredEffectApplied = true;
            }
            else
            {
                hoveredEffectApplied = false;
            }

            LogDebug("[SelectionManager] Deselected (Background Click)");
        }

        /// <summary>
        /// Checks if the details sheet is open using reflection to avoid assembly dependency.
        /// Looks for WebGL.UI.Panels.UIDetailsSheet.Instance.IsSheetOpen without direct import.
        /// </summary>
        private static bool IsDetailsSheetOpen()
        {
            try
            {
                // Get the UIDetailsSheet type via reflection across loaded assemblies.
                System.Type sheetType = null;
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    sheetType = assembly.GetType("WebGL.UI.Panels.UIDetailsSheet");
                    if (sheetType != null)
                    {
                        break;
                    }
                }

                if (sheetType == null)
                {
                    return false;
                }

                // Get the Instance property
                PropertyInfo instanceProp = sheetType.GetProperty("Instance", 
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                if (instanceProp == null)
                {
                    return false;
                }

                // Get the instance
                object instance = instanceProp.GetValue(null);
                if (instance == null)
                {
                    return false;
                }

                // Get the IsSheetOpen property
                PropertyInfo isOpenProp = sheetType.GetProperty("IsSheetOpen",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (isOpenProp == null)
                {
                    return false;
                }

                // Get the value
                object isOpen = isOpenProp.GetValue(instance);
                return (bool)isOpen;
            }
            catch
            {
                return false;
            }
        }

        private static void LogDebug(string message)
        {
            if (EnableDebugLogs) Debug.Log(message);
        }

        private static void LogWarning(string message)
        {
            if (EnableDebugLogs) Debug.LogWarning(message);
        }

        private static void LogError(string message)
        {
            if (EnableDebugLogs) Debug.LogError(message);
        }

        private static bool TryGetValidPointerScreenPosition(Camera camera, out Vector3 pointerPosition)
        {
            pointerPosition = Input.mousePosition;

            if (camera == null)
            {
                return false;
            }

            if (!IsFinite(pointerPosition.x) || !IsFinite(pointerPosition.y) || !IsFinite(pointerPosition.z))
            {
                return false;
            }

            Rect pixelRect = camera.pixelRect;
            if (pixelRect.width <= 0f || pixelRect.height <= 0f
                || !IsFinite(pixelRect.xMin) || !IsFinite(pixelRect.yMin)
                || !IsFinite(pixelRect.xMax) || !IsFinite(pixelRect.yMax))
            {
                return false;
            }

            if (pointerPosition.x < pixelRect.xMin || pointerPosition.x > pixelRect.xMax
                || pointerPosition.y < pixelRect.yMin || pointerPosition.y > pixelRect.yMax)
            {
                return false;
            }

            pointerPosition.z = 0f;
            return true;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// Forces selection of a specific part by its data.
        /// Useful for programmatic selection (e.g., from catalog).
        /// </summary>
        /// <param name="partData">The part data to select.</param>
        public void SelectByData(DronePartData partData)
        {
            if (partData == null) return;

            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                if (part.Data == partData)
                {
                    ClearAssociatedSelectionHighlights();
                    if (currentHighlight != null)
                    {
                        currentHighlight.OnDeselect();
                    }

                    currentSelection = null;
                    currentFullSelection = null;
                    currentSubSelection = null;
                    currentHighlight = null;

                    SelectObject(part.transform);
                    return;
                }
            }

            LogWarning($"[SelectionManager] Part not found: {partData.partName}");
        }

        #endregion
    }
}
