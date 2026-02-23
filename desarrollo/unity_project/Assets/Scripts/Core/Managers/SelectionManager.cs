using UnityEngine;
using WebGL.Core.Data;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Manages object selection via raycasting.
    /// Handles hover highlighting and click selection.
    /// Publishes PartSelectedEvent when selection changes.
    /// </summary>
    public class SelectionManager : Singleton<SelectionManager>
    {
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
            AppState currentState = AppStateMachine.Instance != null 
                ? AppStateMachine.Instance.CurrentState 
                : (GameManager.Instance != null ? GameManager.Instance.CurrentState : AppState.Exploration);

            return currentState == AppState.Exploration || 
                   currentState == AppState.ExplodedView || 
                   currentState == AppState.FocusMode;
        }

        /// <summary>
        /// Handles hover detection and highlighting.
        /// </summary>
        private void HandleHover()
        {
            if (Camera.main == null) return;
            if (OrbitCameraController.GlobalInputBlocked) 
            {
                ClearHover();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, selectionLayer))
            {
                Transform newHover = hit.transform;

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
            if (CursorManager.Instance != null)
            {
                if (cursorType == CursorType.Default)
                    CursorManager.Instance.ResetCursor();
                else
                    CursorManager.Instance.SetCursor(cursorType);
            }
        }

        /// <summary>
        /// Handles click input for selection.
        /// </summary>
        private void HandleClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            // Respect global input block (set by UI pointer events)
            if (OrbitCameraController.GlobalInputBlocked) return;
            
            // UI Blocking Check — delegates to InputManager for centralized UI detection.
            if (InputManager.Instance != null && InputManager.Instance.IsPointerOverUI())
            {
                return;
            }
            
            if (hoveredObject == null)
            {
                Deselect();
                return;
            }

            if (hoveredObject == currentSelection)
            {
                // Clicking same object - could toggle or do nothing
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
            if (selection == null) return;

            currentSelection = selection;
            currentHighlight = selection.GetComponent<HighlightSystem>();

            // Apply highlight
            if (currentHighlight != null)
            {
                currentHighlight.OnSelect();
            }

            // Get data and publish event
            var explodable = selection.GetComponent<ExplodablePart>();
            if (explodable != null && explodable.Data != null)
            {
                EventBus.Publish(new PartSelectedEvent(explodable.Data, fromHotspot));
                
                // Track analytics
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.TrackPartSelected(explodable.Data.PartName);
                }
            }

            // Play feedback sound
            AudioManager.Instance?.PlayClick();

            Debug.Log($"[SelectionManager] Selected: {selection.name}");
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

            Debug.Log("[SelectionManager] Deselected (Background Click)");
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

            Debug.LogWarning($"[SelectionManager] Part not found: {partData.PartName}");
        }

        #endregion
    }
}
