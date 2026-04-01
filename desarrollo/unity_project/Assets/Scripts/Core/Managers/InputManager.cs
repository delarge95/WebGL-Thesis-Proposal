using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;
using System;

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
        private bool _pickErrorLogged;

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
            // Fast path: explicit blocker set by UI handlers.
            if (InputBlocked)
            {
                return true;
            }

            CacheUIDocumentIfNeeded();
            if (_uiPanel == null) return false;

            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (screenPos.x < 0f || screenPos.y < 0f) return false;

            // RuntimePanelUtils.ScreenToPanel handles:
            //  - Y-axis inversion (Screen bottom-left → Panel top-left)
            //  - ScaleWithScreenSize scaling factor
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, screenPos);
            VisualElement picked;
            try
            {
                picked = _uiPanel.Pick(panelPos);
            }
            catch (Exception ex)
            {
                if (!_pickErrorLogged)
                {
                    _pickErrorLogged = true;
                    Debug.LogWarning($"[InputManager] Panel.Pick failed. Falling back to InputBlocked flag. {ex.Message}");
                }

                return InputBlocked;
            }

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
                _mainUIDocument = UnityEngine.Object.FindFirstObjectByType<UIDocument>();
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

                if (IsPassThroughSurface(current))
                {
                    continue;
                }

                if (IsInteractiveElement(current))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInteractiveElement(VisualElement element)
        {
            if (element == null)
            {
                return false;
            }

            if (element is Button ||
                element is Slider ||
                element is SliderInt ||
                element is ScrollView ||
                element is Scroller ||
                element is Toggle ||
                element is Foldout ||
                element is TextField ||
                element is DropdownField)
            {
                return true;
            }

            string name = element.name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                string lower = name.ToLowerInvariant();
                if (lower.Contains("btn") || lower.Contains("button") ||
                    lower.Contains("slider") || lower.Contains("scroll"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPassThroughSurface(VisualElement element)
        {
            if (element == null)
            {
                return false;
            }

            if (element.name == "BottomSheet" || element.name == "SheetContent_Details")
            {
                return true;
            }

            if (element.ClassListContains("sheet-header"))
            {
                return true;
            }

            if (element.ClassListContains("details-sheet") || element.ClassListContains("sheet-content"))
            {
                return true;
            }

            return false;
        }
    }
}
