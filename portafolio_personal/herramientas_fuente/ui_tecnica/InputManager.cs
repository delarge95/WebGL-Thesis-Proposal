using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Centralized input manager. Provides unified input state and UI-awareness.
    /// Other managers (SelectionManager, OrbitCameraController) query IsPointerOverUI()
    /// to avoid processing 3D input when the user is interacting with UI Toolkit elements.
    /// </summary>
    public class InputManager : PersistentSingleton<InputManager>
    {
        [Header("Settings")]
        [SerializeField] private float mouseSensitivity = 1.0f;
        [SerializeField] private float touchSensitivity = 0.5f;

        // ── Cached UI references ──
        private UIDocument _mainUIDocument;
        private IPanel _uiPanel;

        // ── Public input state ──
        public Vector2 LookInput { get; private set; }
        public float ZoomInput { get; private set; }
        public bool IsInteracting { get; private set; }

        /// <summary>
        /// True while the user is actively dragging (orbiting/panning) the 3D viewport.
        /// Used by OrbitCameraController to distinguish "started drag on 3D" from "pointer is over UI".
        /// </summary>
        public bool IsDragging3D { get; private set; }

        /// <summary>
        /// Global flag set by UI pointer-enter/leave callbacks to block 3D input
        /// (orbit, pan, zoom, selection hover) while the user is interacting with
        /// UI Toolkit elements like sliders, sheets, or scroll views.
        /// Replaces the old OrbitCameraController.GlobalInputBlocked static field
        /// (Phase 4: Hardening — single source of truth in InputManager).
        /// </summary>
        public static bool InputBlocked { get; set; }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            CacheUIDocumentIfNeeded();
            HandleInput();
        }

        // ═══════════════════════════════════════════════════════
        //  UI Detection — Uses RuntimePanelUtils + Panel.Pick()
        //  Safe with ScaleWithScreenSize PanelSettings.
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Returns true if the mouse/touch pointer is currently over an interactive
        /// UI Toolkit element (one with picking-mode: Position).
        /// </summary>
        public bool IsPointerOverUI()
        {
            CacheUIDocumentIfNeeded();
            if (_uiPanel == null) return false;

            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (screenPos.x < 0f || screenPos.y < 0f) return false;

            // RuntimePanelUtils.ScreenToPanel handles:
            //  - Y-axis inversion (Screen bottom-left → Panel top-left)
            //  - ScaleWithScreenSize scaling factor
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, screenPos);
            VisualElement picked = _uiPanel.Pick(panelPos);

            // Pick() returns null if nothing with picking-mode: Position is hit
            if (picked == null) return false;

            return IsInteractivePick(picked);
        }

        // ═══════════════════════════════════════════════════════
        //  Core Input Polling
        // ═══════════════════════════════════════════════════════

        private void HandleInput()
        {
            LookInput = Vector2.zero;
            ZoomInput = 0f;
            IsInteracting = false;

            // Track drag state: starts on mouse-down in 3D, ends on mouse-up
            if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
            {
                IsDragging3D = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                IsDragging3D = false;
            }

            // Mouse Input
            if (Input.GetMouseButton(1)) // Right click to orbit
            {
                LookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
                IsInteracting = true;
            }

            ZoomInput = Input.GetAxis("Mouse ScrollWheel");

            // Touch Input (Simple implementation)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    LookInput = touch.deltaPosition * touchSensitivity * Time.deltaTime;
                    IsInteracting = true;
                }

                // Pinch to zoom logic would go here
            }
        }

        /// <summary>
        /// Lazily caches the UIDocument reference. This avoids requiring a serialized field
        /// and handles scenes where UIDocument may not exist yet at Awake time.
        /// </summary>
        private void CacheUIDocumentIfNeeded()
        {
            if (_mainUIDocument == null || _mainUIDocument.rootVisualElement == null)
            {
                _mainUIDocument = Object.FindFirstObjectByType<UIDocument>();
            }

            if (_mainUIDocument != null && _mainUIDocument.rootVisualElement != null)
            {
                _uiPanel = _mainUIDocument.rootVisualElement.panel;
            }
            else
            {
                _uiPanel = null;
            }
        }

        private Vector2 GetCurrentPointerScreenPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }

            return Input.mousePosition;
        }

        private bool IsInteractivePick(VisualElement picked)
        {
            if (picked == null || _mainUIDocument?.rootVisualElement == null) return false;

            VisualElement root = _mainUIDocument.rootVisualElement;
            VisualElement templateRoot = root.parent;

            for (VisualElement current = picked; current != null; current = current.parent)
            {
                if (current == root || current == templateRoot) return false;

                if (current.pickingMode != PickingMode.Position) continue;
                if (!current.enabledInHierarchy) continue;
                if (current.resolvedStyle.display == DisplayStyle.None) continue;
                if (current.resolvedStyle.visibility == Visibility.Hidden) continue;

                return true;
            }

            return false;
        }
    }
}
