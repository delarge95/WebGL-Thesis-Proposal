using UnityEngine;
using WebGL.Core.Data;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using WebGL.Core.Content;

namespace WebGL.Core.Managers
{
    public class SelectionManager : Singleton<SelectionManager>
    {
        [Header("Settings")]
        [SerializeField] private LayerMask selectionLayer;
        [SerializeField] private float maxRayDistance = 100f;

        private Transform currentSelection;
        private Transform hoveredObject;
        private HighlightSystem currentHighlight;
        private HighlightSystem hoveredHighlight;

        private void Update()
        {
            // Use AppStateMachine if available, fallback to GameManager
            AppState currentState = AppStateMachine.Instance != null 
                ? AppStateMachine.Instance.CurrentState 
                : (GameManager.Instance != null ? GameManager.Instance.CurrentState : AppState.Exploration);

            if (currentState != AppState.Exploration && 
                currentState != AppState.ExplodedView && 
                currentState != AppState.FocusMode) return;

            HandleHover();
            HandleClick();
        }

        private void HandleHover()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance, selectionLayer))
            {
                Transform newHover = hit.transform;

                if (newHover != hoveredObject)
                {
                    // Exit previous hover
                    if (hoveredHighlight != null && hoveredObject != currentSelection)
                    {
                        hoveredHighlight.OnHoverExit();
                    }

                    // Enter new hover
                    hoveredObject = newHover;
                    hoveredHighlight = hoveredObject.GetComponent<HighlightSystem>();
                    
                    if (hoveredHighlight != null && hoveredObject != currentSelection)
                    {
                        hoveredHighlight.OnHoverEnter();
                    }

                    // Update cursor
                    if (CursorManager.Instance != null)
                    {
                        CursorManager.Instance.SetCursor(CursorType.Pointer);
                    }
                }
            }
            else
            {
                // Clear hover
                if (hoveredHighlight != null && hoveredObject != currentSelection)
                {
                    hoveredHighlight.OnHoverExit();
                }
                hoveredObject = null;
                hoveredHighlight = null;

                if (CursorManager.Instance != null)
                {
                    CursorManager.Instance.ResetCursor();
                }
            }
        }

        private void HandleClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (hoveredObject == null)
            {
                Deselect();
                return;
            }

            if (hoveredObject == currentSelection)
            {
                // Clicking same object - toggle or do nothing
                return;
            }

            // Deselect previous
            Deselect();

            // Select new
            SelectObject(hoveredObject);
        }

        private void SelectObject(Transform selection)
        {
            currentSelection = selection;
            currentHighlight = selection.GetComponent<HighlightSystem>();

            if (currentHighlight != null)
            {
                currentHighlight.OnSelect();
            }

            // Get data and publish event
            var explodable = selection.GetComponent<ExplodablePart>();
            if (explodable != null && explodable.Data != null)
            {
                EventBus.Publish(new PartSelectedEvent(explodable.Data));
                
                // Track analytics
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.TrackPartSelected(explodable.Data.PartName);
                }
            }

            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[SelectionManager] Selected: {selection.name}");
        }

        public void Deselect()
        {
            if (currentSelection == null) return;

            if (currentHighlight != null)
            {
                currentHighlight.OnDeselect();
            }

            EventBus.Publish(new PartSelectedEvent(null));

            currentSelection = null;
            currentHighlight = null;
        }

        public Transform GetCurrentSelection() => currentSelection;
    }
}
