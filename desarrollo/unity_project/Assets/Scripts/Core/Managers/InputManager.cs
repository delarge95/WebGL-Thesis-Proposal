using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;
using System;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Centralized input manager. Provides unified input state and UI-awareness.
    /// Selection queries UI hit-testing to avoid click-through, while camera motion
    /// relies on explicit gesture capture through InputBlocked.
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
        /// Manual blocker for active UI gestures (slider drags, sheet scrolling, etc.).
        /// This should only be toggled on press/drag lifecycles, never on hover.
        /// </summary>
        private static bool _inputBlocked;

        public static bool InputBlocked
        {
            get
            {
                // Fail-safe: if no mouse buttons or touches are active, clear any stuck input blocks.
                // This prevents UI Toolkit bugs (missed PointerLeave/PointerUp) from permanently locking 3D interaction.
                if (_inputBlocked && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) && Input.touchCount == 0)
                {
                    _inputBlocked = false;
                }
                return _inputBlocked;
            }
            set
            {
                _inputBlocked = value;
            }
        }

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
            // Fast path: explicit blocker set only while the UI is actively
            // capturing a gesture and should keep 3D input suspended.
            if (InputBlocked)
            {
                return true;
            }

            if (!TryGetPointerPositions(out Vector2 screenPos, out Vector2 panelPos))
            {
                return false;
            }

            if (IsExplicitBlockingSurfaceAtPoint(screenPos))
            {
                return true;
            }

            if (!TryPickElementAtPanelPosition(panelPos, out VisualElement picked, out bool pickFailed))
            {
                return pickFailed ? InputBlocked : false;
            }

            return IsInteractivePick(picked, panelPos);
        }

        /// <summary>
        /// Returns true when the pointer is over any visible UI Toolkit element.
        /// Used by selection/hover systems so UI clicks are never interpreted as
        /// background clicks that would deselect the current part.
        /// </summary>
        public bool IsPointerOverSelectionBlockingUI()
        {
            if (InputBlocked)
            {
                return true;
            }

            if (!TryGetPointerPositions(out Vector2 screenPos, out Vector2 panelPos))
            {
                return false;
            }

            if (IsExplicitBlockingSurfaceAtPoint(screenPos))
            {
                return true;
            }

            if (!TryPickElementAtPanelPosition(panelPos, out VisualElement picked, out bool pickFailed))
            {
                return pickFailed ? InputBlocked : false;
            }

            return IsVisiblePick(picked, panelPos);
        }

        // ═══════════════════════════════════════════════════════
        //  Core Input Polling
        // ═══════════════════════════════════════════════════════

        private void HandleInput()
        {
            LookInput = Vector2.zero;
            ZoomInput = 0f;
            IsInteracting = false;

            // Track drag state: starts when the user begins a 3D drag outside an
            // actively captured UI gesture, ends on mouse-up.
            if (Input.GetMouseButtonDown(1) && !InputBlocked)
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

        private bool TryGetPointerPositions(out Vector2 screenPos, out Vector2 panelPos)
        {
            screenPos = Vector2.zero;
            panelPos = Vector2.zero;

            CacheUIDocumentIfNeeded();
            if (_uiPanel == null) return false;

            screenPos = GetCurrentPointerScreenPosition();
            if (screenPos.x < 0f || screenPos.y < 0f) return false;

            // RuntimePanelUtils.ScreenToPanel handles:
            //  - Y-axis inversion (Screen bottom-left → Panel top-left)
            //  - ScaleWithScreenSize scaling factor
            panelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, screenPos);
            return true;
        }

        private bool TryPickElementAtPanelPosition(Vector2 panelPos, out VisualElement picked, out bool pickFailed)
        {
            picked = null;
            pickFailed = false;
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

                pickFailed = true;
                return false;
            }

            // Pick() returns null if nothing with picking-mode: Position is hit
            return picked != null;
        }

        private bool IsExplicitBlockingSurfaceAtPoint(Vector2 screenPos)
        {
            if (_mainUIDocument?.rootVisualElement == null) return false;

            return IsNamedSurfaceAtPoint("BottomSheet", screenPos)
                || IsNamedSurfaceAtPoint("InfoBarPeek", screenPos);
        }

        private bool IsNamedSurfaceAtPoint(string elementName, Vector2 screenPos)
        {
            VisualElement element = _mainUIDocument?.rootVisualElement?.Q<VisualElement>(elementName);
            return IsElementVisible(element) && ElementContainsScreenPoint(element, screenPos);
        }

        private bool IsVisiblePick(VisualElement picked, Vector2 panelPos)
        {
            if (picked == null || _mainUIDocument?.rootVisualElement == null) return false;

            VisualElement root = _mainUIDocument.rootVisualElement;
            VisualElement templateRoot = root.parent;

            if (picked == root || picked == templateRoot) return false;

            return IsElementChainAtPoint(picked, panelPos, root, templateRoot);
        }

        /// <summary>
        /// Determines whether a picked VisualElement should block 3D interaction.
        /// Walks up the ancestor chain looking for interactive UI controls, but
        /// verifies that each candidate's worldBound actually contains the pointer
        /// position. This prevents false positives from overflow-clipped elements
        /// whose bounding boxes extend beyond their visible parent.
        /// </summary>
        private bool IsInteractivePick(VisualElement picked, Vector2 panelPos)
        {
            if (picked == null || _mainUIDocument?.rootVisualElement == null) return false;

            VisualElement root = _mainUIDocument.rootVisualElement;
            VisualElement templateRoot = root.parent;

            // Quick exit: if picked IS the document root or panel root, it's empty space.
            if (picked == root || picked == templateRoot) return false;

            // Walk up the chain and validate each candidate against the full visible
            // containment path. This prevents overflow-clipped sheet descendants from
            // blocking 3D input outside the visible UI surface.
            const int MAX_ANCESTOR_DEPTH = 16;
            int depth = 0;
            for (VisualElement current = picked; current != null && depth < MAX_ANCESTOR_DEPTH; current = current.parent, depth++)
            {
                if (current == root || current == templateRoot) break;

                if (!CanElementBlock3D(current)) continue;

                if (IsPassThroughSurface(current)) continue;
                if (!IsInteractiveElement(current)) continue;

                if (IsElementChainAtPoint(current, panelPos, root, templateRoot))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsElementChainAtPoint(VisualElement element, Vector2 panelPos, VisualElement root, VisualElement templateRoot)
        {
            const int MAX_ANCESTOR_DEPTH = 24;
            int depth = 0;

            for (VisualElement current = element; current != null && depth < MAX_ANCESTOR_DEPTH; current = current.parent, depth++)
            {
                if (current == root || current == templateRoot)
                {
                    return true;
                }

                if (!IsElementVisible(current)) return false;
                if (!ElementContainsPoint(current, panelPos)) return false;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the element's world bounding rect contains the given panel-space point.
        /// Used to verify that an interactive ancestor is actually visually at the pointer position,
        /// filtering out false positives from overflow-clipped or off-screen elements.
        /// </summary>
        private static bool ElementContainsPoint(VisualElement element, Vector2 panelPos)
        {
            if (element == null) return false;
            Rect wb = element.worldBound;
            // worldBound can be zero-sized for hidden or collapsed elements
            if (wb.width <= 0f || wb.height <= 0f) return false;
            return wb.Contains(panelPos);
        }

        private static bool ElementContainsScreenPoint(VisualElement element, Vector2 screenPos)
        {
            if (element == null) return false;

            Rect wb = element.worldBound;
            if (wb.width <= 0f || wb.height <= 0f) return false;

            // worldBound is expressed in panel/screen space with origin at the
            // lower-left in this runtime setup, so the raw screen pointer aligns
            // with it more reliably than ScreenToPanel-converted coordinates.
            return wb.Contains(screenPos);
        }

        private static bool CanElementBlock3D(VisualElement element)
        {
            return element != null
                && element.pickingMode == PickingMode.Position
                && IsElementVisible(element);
        }

        private static bool IsElementVisible(VisualElement element)
        {
            return element != null
                && element.enabledInHierarchy
                && element.resolvedStyle.display != DisplayStyle.None
                && element.resolvedStyle.visibility != Visibility.Hidden;
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

            if (element.ClassListContains("mode-submenu") ||
                element.ClassListContains("hero-container") ||
                element.ClassListContains("hero-submenu") ||
                element.ClassListContains("onboard-overlay"))
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
