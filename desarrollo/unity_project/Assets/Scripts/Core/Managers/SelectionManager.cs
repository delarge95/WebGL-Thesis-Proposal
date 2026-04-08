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
        private const bool EnableDebugLogs = false;

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
        private Transform hoveredObject;
        private HighlightSystem currentHighlight;
        private HighlightSystem hoveredHighlight;

        // Double-click tracking
        private float _lastClickTime;
        private string _lastClickId;

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
                Transform newHover = ResolveSelectableTransform(hit.transform);

                if (newHover != hoveredObject)
                {
                    ExitCurrentHover();
                    EnterNewHover(newHover);
                }
            }
            else
            {
                ClearHover();
            }
        }

        /// <summary>
        /// Exits the current hover state.
        /// </summary>
        private void ExitCurrentHover()
        {
            if (hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverExit();
            }
        }

        /// <summary>
        /// Enters a new hover state.
        /// </summary>
        /// <param name="newHover">The new transform to hover.</param>
        private void EnterNewHover(Transform newHover)
        {
            hoveredObject = newHover;
            hoveredHighlight = hoveredObject.GetComponent<HighlightSystem>();
            if (hoveredHighlight == null)
            {
                hoveredHighlight = hoveredObject.GetComponentInParent<HighlightSystem>();
            }
            
            if (hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverEnter();
            }

            UpdateCursor(CursorType.Pointer);
        }

        /// <summary>
        /// Clears the current hover state.
        /// </summary>
        private void ClearHover()
        {
            if (hoveredHighlight != null && hoveredObject != currentSelection)
            {
                hoveredHighlight.OnHoverExit();
            }

            hoveredObject = null;
            hoveredHighlight = null;

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
            DronePartData clickedData = null;
            if (hoveredObject != null)
            {
                var exp = hoveredObject.GetComponentInParent<ExplodablePart>();
                clickedData = exp != null ? exp.Data : null;
            }

            string clickId = clickedData?.partName ?? "__bg__";
            bool isDoubleClick = clickId == _lastClickId
                && (now - _lastClickTime) < DOUBLE_CLICK_THRESHOLD;

            if (isDoubleClick)
            {
                EventBus.Publish(new PartDoubleClickedEvent(clickedData));
                _lastClickTime = 0f;
                _lastClickId = null;
                return; // Skip normal selection on double-click
            }

            // Track for next potential double-click
            _lastClickTime = now;
            _lastClickId = clickId;

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

                LogDebug("[SelectionManager.HandleClick] Deselecting...");
                // Background click → deselect current selection
                Deselect();
                return;
            }

            if (hoveredObject == currentSelection)
            {
                // Re-click on already-selected part — no-op (first click already populated UI)
                return;
            }

            // Swap Selection Logic (User Request: Don't close sheet if swapping)
            if (currentSelection != null)
            {
                // Manually clean up old selection visual
                if (currentHighlight != null) currentHighlight.OnDeselect();
                // Do NOT call Deselect() here to avoid firing PartSelectedEvent(null)
            }

            SelectObject(hoveredObject);
        }

        #endregion

        #region Selection Methods

        /// <summary>
        /// Selects the specified object.
        /// </summary>
        /// <param name="selection">The transform to select.</param>
        /// <param name="fromHotspot">True if triggered by a hotspot click.</param>
        public void SelectObject(Transform selection, bool fromHotspot = false)
        {
            selection = ResolveSelectableTransform(selection);
            if (selection == null) return;

            // Clean up previous selection highlight before applying new one
            if (currentHighlight != null && currentSelection != selection)
            {
                currentHighlight.OnDeselect();
            }

            currentSelection = selection;
            currentHighlight = selection.GetComponent<HighlightSystem>();
            if (currentHighlight == null)
            {
                currentHighlight = selection.GetComponentInParent<HighlightSystem>();
            }

            // Apply highlight
            if (currentHighlight != null)
            {
                currentHighlight.OnSelect();
            }

            // Get data and publish event
            var explodable = selection.GetComponent<ExplodablePart>();
            if (explodable == null)
            {
                explodable = selection.GetComponentInParent<ExplodablePart>();
            }
            if (explodable != null && explodable.Data != null)
            {
                EventBus.Publish(new PartSelectedEvent(explodable.Data, fromHotspot));
                
                // Track analytics
                if (ServiceLocator.TryGet<AnalyticsManager>(out var analytics))
                {
                    analytics.TrackPartSelected(explodable.Data.partName);
                }
            }

            // Play feedback sound
            AudioManager.Instance?.PlayClick();

            LogDebug($"[SelectionManager] Selected: {selection.name}");
        }

        private static Transform ResolveSelectableTransform(Transform rawTransform)
        {
            return rawTransform;
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
            currentHighlight = null;

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
                    Deselect();
                    SelectObject(part.transform);
                    return;
                }
            }

            LogWarning($"[SelectionManager] Part not found: {partData.partName}");
        }

        #endregion
    }
}
