using UnityEngine;
using WebGL.Core.Data;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;
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

        // Double-click tracking
        private float _lastClickTime;
        private string _lastClickId;
        private bool _lastClickHadSelection;
        private Transform _lastClickSelection;
        private Transform _lastClickFullSelection;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the currently selected transform.
        /// </summary>
        public Transform CurrentSelection => currentSelection;

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

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
            float now = Time.time;
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
            bool isDoubleClick = clickId == _lastClickId
                && (now - _lastClickTime) < DOUBLE_CLICK_THRESHOLD;

            if (isDoubleClick)
            {
                EventBus.Publish(new PartDoubleClickedEvent(
                    clickedData,
                    clickedSelection,
                    clickedFull,
                    clickedSelection == null,
                    _lastClickHadSelection,
                    _lastClickSelection,
                    _lastClickFullSelection));
                _lastClickTime = 0f;
                _lastClickId = null;
                _lastClickHadSelection = false;
                _lastClickSelection = null;
                _lastClickFullSelection = null;
                return; // Skip normal selection on double-click
            }

            // Track for next potential double-click
            _lastClickTime = now;
            _lastClickId = clickId;
            _lastClickHadSelection = hadSelectionBeforeClick;
            _lastClickSelection = selectionBeforeClick;
            _lastClickFullSelection = fullSelectionBeforeClick;

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

            if (hotspotGroupSelectionActive)
            {
                // First mesh click after hotspot-group selection should immediately enter
                // normal piece flow by selecting the clicked mother part.
                hotspotGroupSelectionActive = false;
                EventBus.Publish(new HotspotGroupVisualsClearRequestedEvent());
                SelectObject(clickedFullSelection != null ? clickedFullSelection : clickedRaw);
                return;
            }

            if (currentFullSelection == null)
            {
                // First click always selects full parent piece.
                SelectObject(clickedFullSelection != null ? clickedFullSelection : clickedRaw);
                return;
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
            string hotspotGroupMembers = "")
        {
            if (selection == null) return;

            if (hoveredEffectApplied && hoveredHighlight != null)
            {
                hoveredHighlight.OnHoverExit();
                hoveredEffectApplied = false;
            }

            // Clean up previous selection highlight before applying new one
            if (currentHighlight != null && currentSelection != selection)
            {
                currentHighlight.OnDeselect();
            }

            Transform fullSelection = ResolvePrimarySelection(selection);
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
            if (marker != null && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
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
            if (marker != null)
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

            if (marker != null && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId))
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

            // Walk parents manually but STOP at the first ExplodablePart boundary.
            // This prevents capturing a sibling fastener's marker on a shared ancestor
            // after ImportedDroneRuntimeBinder reparents renderers under mother parts.
            Transform current = target.parent;
            while (current != null)
            {
                if (current.GetComponent<ExplodablePart>() != null)
                {
                    break;
                }

                marker = current.GetComponent<FastenerRuntimeMarker>();
                if (marker != null)
                {
                    return marker;
                }

                current = current.parent;
            }

            return null;
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
