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
        private static readonly bool EnableDebugLogs = false;

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
        private static float _inputBlockedSetAt = -999f;
        private static float _inputBlockedUntil = -999f;
        private const float InputBlockFailSafeSeconds = 0.75f;

        public static bool InputBlocked
        {
            get
            {
                if (Time.unscaledTime < _inputBlockedUntil)
                {
                    return true;
                }

                // Fail-safe: if no mouse buttons or touches are active, clear any stuck input blocks
                // after a short grace period. UI Toolkit pointer events can lead Unity's legacy
                // Input state by a frame or two, especially in WebGL/mobile simulation paths.
                // This prevents UI Toolkit bugs (missed PointerLeave/PointerUp) from permanently locking 3D interaction.
                if (_inputBlocked
                    && !Input.GetMouseButton(0)
                    && !Input.GetMouseButton(1)
                    && !Input.GetMouseButton(2)
                    && Input.touchCount == 0
                    && Time.unscaledTime - _inputBlockedSetAt > InputBlockFailSafeSeconds)
                {
                    _inputBlocked = false;
                }
                return _inputBlocked;
            }
            set
            {
                _inputBlocked = value;
                _inputBlockedSetAt = value ? Time.unscaledTime : -999f;
            }
        }

        public static void BlockInputForSeconds(float seconds)
        {
            if (seconds <= 0f)
            {
                return;
            }

            _inputBlockedUntil = Mathf.Max(_inputBlockedUntil, Time.unscaledTime + seconds);
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
            return IsPointerOverSelectionBlockingUI();
        }

        /// <summary>
        /// Returns true when the pointer is over UI that should block camera orbit/pan/zoom.
        /// When the info panel is open, the full sheet rectangle owns the gesture so
        /// swipes cannot leak through and orbit the drone underneath.
        /// </summary>
        public bool IsPointerOverCameraBlockingUI()
        {
            if (InputBlocked)
            {
                return true;
            }

            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (!IsFinite(screenPos) || screenPos.x < 0f || screenPos.y < 0f)
            {
                return false;
            }

            return IsScreenPositionOverCameraBlockingUIRaw(screenPos);
        }

        /// <summary>
        /// Returns true when the given screen coordinate is over UI that should
        /// own camera gestures. Touch handlers must pass the specific finger
        /// position instead of relying on Input.GetTouch(0), otherwise a panel
        /// touch can leak through when another pointer path is active.
        /// </summary>
        public bool IsScreenPositionOverCameraBlockingUI(Vector2 screenPos)
        {
            return IsScreenPositionOverCameraBlockingUIRaw(screenPos);
        }

        private bool IsScreenPositionOverCameraBlockingUIRaw(Vector2 screenPos)
        {
            if (!TryGetPanelPositionCandidates(screenPos, out Vector2 rawPanelPos, out bool hasRaw, out Vector2 mirroredPanelPos, out bool hasMirrored))
            {
                return false;
            }

            if (IsDetailsSheetVisible())
            {
                return hasMirrored && IsPanelPositionOverDetailsSheet(mirroredPanelPos);
            }

            // Block on explicit controls only when sheet is closed
            return (hasRaw && IsPointerOverBlockingElement(rawPanelPos))
                || (hasMirrored && IsPointerOverBlockingElement(mirroredPanelPos))
                || IsScreenPositionOverInteractiveElement(screenPos)
                || IsScreenPositionOverInteractiveElement(MirrorScreenY(screenPos));
        }

        /// <summary>
        /// Returns true when the pointer is over any visible UI Toolkit element.
        /// Used by selection/hover systems so UI clicks are never interpreted as
        /// background clicks that would deselect the current part.
        /// 
        /// IMPORTANT DISTINCTION:
        /// - When sheet is CLOSED: blocks only explicit controls (buttons, sliders, etc)
        /// - When sheet is OPEN: blocks the panel background area AND controls, 
        ///   but ALLOWS 3D interaction outside the panel (on the canvas)
        /// </summary>
        public bool IsPointerOverSelectionBlockingUI()
        {
            if (InputBlocked)
            {
                return true;
            }

            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (!IsFinite(screenPos) || screenPos.x < 0f || screenPos.y < 0f)
            {
                return false;
            }

            return IsScreenPositionOverSelectionBlockingUIRaw(screenPos);
        }

        /// <summary>
        /// Raw probe that ignores InputBlocked and checks only actual UI hit-testing
        /// at the current pointer position.
        /// </summary>
        public bool IsPointerOverBlockingUIRaw()
        {
            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (!IsFinite(screenPos) || screenPos.x < 0f || screenPos.y < 0f)
            {
                return false;
            }

            return IsScreenPositionOverSelectionBlockingUIRaw(screenPos);
        }

        /// <summary>
        /// Diagnostic helper for click-block analysis.
        /// </summary>
        public string GetSelectionBlockDebugReason()
        {
            if (InputBlocked)
            {
                return "InputBlocked=true";
            }

            Vector2 screenPos = GetCurrentPointerScreenPosition();
            if (!TryGetPanelPositionCandidates(screenPos, out Vector2 rawPanelPos, out bool hasRaw, out Vector2 mirroredPanelPos, out bool hasMirrored))
            {
                return "TryGetPanelPositionCandidates=false";
            }

            bool sheetVisible = IsDetailsSheetVisible();
            if (sheetVisible)
            {
                if (hasMirrored && IsPointerOverNamedElement("SheetCloseBtn", mirroredPanelPos)) return "SheetCloseBtn";
                if (hasMirrored && IsPointerOverNamedElement("BottomSheet", mirroredPanelPos)) return "BottomSheet";
                if (hasMirrored && IsPointerOverSheetScrollRegion(mirroredPanelPos)) return "SheetScrollRegion";
                if (hasMirrored && IsPointerOverGlobalSelectionControl(mirroredPanelPos)) return "InteractiveUI";
                if (IsScreenPositionOverInteractiveElement(screenPos)
                    || IsScreenPositionOverInteractiveElement(MirrorScreenY(screenPos))) return "ScreenInteractiveUI";

                return $"NoBlock sheetOpen screen=({screenPos.x:F1},{screenPos.y:F1}) raw=({rawPanelPos.x:F1},{rawPanelPos.y:F1}) mirrored=({mirroredPanelPos.x:F1},{mirroredPanelPos.y:F1})";
            }

            Vector2 panelPos = hasMirrored ? mirroredPanelPos : rawPanelPos;
            if (IsPointerOverNamedElement("InfoBarPeek", panelPos, requireVisibleClass: "info-bar-peek--hidden")) return "InfoBarPeek";
            if (IsPointerOverInteractiveUI(panelPos)) return "InteractiveUI";

            return $"NoBlock screen=({screenPos.x:F1},{screenPos.y:F1}) panel=({panelPos.x:F1},{panelPos.y:F1})";
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
            if (_mainUIDocument != null && _mainUIDocument.rootVisualElement != null && ContainsSelectionBlockingLayout(_mainUIDocument.rootVisualElement))
            {
                _uiPanel = _mainUIDocument.rootVisualElement.panel;
                return;
            }

            UIDocument bestDocument = null;
            int bestScore = -1;

            UIDocument[] documents = UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
            foreach (UIDocument document in documents)
            {
                if (document == null || document.rootVisualElement == null)
                {
                    continue;
                }

                int score = ScoreDocumentForSelectionBlockingUI(document.rootVisualElement);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDocument = document;
                }
            }

            _mainUIDocument = bestDocument;
            if (_mainUIDocument != null && _mainUIDocument.rootVisualElement != null)
            {
                _uiPanel = _mainUIDocument.rootVisualElement.panel;
            }
            else
            {
                _uiPanel = null;
            }
        }

        private static bool ContainsSelectionBlockingLayout(VisualElement root)
        {
            if (root == null)
            {
                return false;
            }

            return root.Q<VisualElement>("BottomSheet") != null
                || root.Q<VisualElement>("InfoBarPeek") != null
                || root.Q<Button>("SheetCloseBtn") != null
                || root.Q<VisualElement>("SheetContent_Details") != null;
        }

        private static int ScoreDocumentForSelectionBlockingUI(VisualElement root)
        {
            if (root == null)
            {
                return -1;
            }

            int score = 0;

            if (root.Q<VisualElement>("BottomSheet") != null) score += 4;
            if (root.Q<VisualElement>("SheetContent_Details") != null) score += 3;
            if (root.Q<Button>("SheetCloseBtn") != null) score += 3;
            if (root.Q<VisualElement>("InfoBarPeek") != null) score += 2;
            if (root.Q<VisualElement>("BottomBar") != null) score += 1;
            if (root.Q<VisualElement>("TopBar") != null) score += 1;

            return score;
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

            screenPos = GetCurrentPointerScreenPosition();
            if (!TryGetPanelPositionCandidates(screenPos, out Vector2 rawPanelPos, out bool hasRaw, out Vector2 mirroredPanelPos, out bool hasMirrored))
            {
                return false;
            }

            bool rawBlocks = hasRaw && IsPointerOverBlockingElement(rawPanelPos);
            bool mirroredBlocks = hasMirrored && IsPointerOverBlockingElement(mirroredPanelPos);

            panelPos = hasMirrored
                ? (rawBlocks && !mirroredBlocks ? rawPanelPos : mirroredPanelPos)
                : rawPanelPos;
            if (!IsFinite(panelPos))
            {
                panelPos = rawPanelPos;
            }

            return true;
        }

        private bool IsScreenPositionOverSelectionBlockingUIRaw(Vector2 screenPos)
        {
            if (!TryGetPanelPositionCandidates(screenPos, out Vector2 rawPanelPos, out bool hasRaw, out Vector2 mirroredPanelPos, out bool hasMirrored))
            {
                return false;
            }

            if (IsDetailsSheetVisible())
            {
                // For an open sheet, use the same panel-space coordinate path as
                // camera blocking. The raw candidate can mirror lower-screen UI
                // onto upper canvas clicks, which blocks selection outside the
                // actual visible panel.
                bool overVisibleSheet = hasMirrored && IsPanelPositionOverDetailsSheet(mirroredPanelPos);
                if (overVisibleSheet)
                {
                    return true;
                }

                // Outside the open sheet, the 3D canvas must remain fully selectable:
                // single tap/click, deselect, and double tap/click isolation all pass through.
                return (hasMirrored && IsPointerOverGlobalSelectionControl(mirroredPanelPos))
                    || IsScreenPositionOverInteractiveElement(screenPos)
                    || IsScreenPositionOverInteractiveElement(MirrorScreenY(screenPos));
            }

            return (hasRaw && IsPointerOverBlockingElement(rawPanelPos))
                || (hasMirrored && IsPointerOverBlockingElement(mirroredPanelPos))
                || IsScreenPositionOverInteractiveElement(screenPos)
                || IsScreenPositionOverInteractiveElement(MirrorScreenY(screenPos));
        }

        private bool TryGetPanelPositionCandidates(
            Vector2 screenPos,
            out Vector2 rawPanelPos,
            out bool hasRaw,
            out Vector2 mirroredPanelPos,
            out bool hasMirrored)
        {
            rawPanelPos = Vector2.zero;
            mirroredPanelPos = Vector2.zero;
            hasRaw = false;
            hasMirrored = false;

            CacheUIDocumentIfNeeded();
            if (_uiPanel == null || !IsFinite(screenPos) || screenPos.x < 0f || screenPos.y < 0f)
            {
                return false;
            }

            rawPanelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, screenPos);
            hasRaw = IsFinite(rawPanelPos);

            Vector2 mirroredScreenPos = MirrorScreenY(screenPos);
            mirroredPanelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, mirroredScreenPos);
            hasMirrored = IsFinite(mirroredPanelPos);

            return hasRaw || hasMirrored;
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
                    LogWarning($"[InputManager] Panel.Pick failed. Falling back to InputBlocked flag. {ex.Message}");
                }

                pickFailed = true;
                return false;
            }

            // Pick() returns null if nothing with picking-mode: Position is hit
            return picked != null;
        }

        private bool IsExplicitBlockingSurfaceAtPoint(Vector2 screenPos)
        {
            CacheUIDocumentIfNeeded();
            if (_mainUIDocument?.rootVisualElement == null || _uiPanel == null) return false;

            // Convert screen position to panel position using the same method as TryGetPointerPositions
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_uiPanel, screenPos);

            // Treat the visible BottomSheet as a blocking surface so drags on the
            // sheet/handle do not leak through to the 3D camera.
            VisualElement bottomSheet = _mainUIDocument.rootVisualElement.Q<VisualElement>("BottomSheet");
            if (bottomSheet != null && IsElementVisible(bottomSheet) && ElementContainsPoint(bottomSheet, panelPos))
            {
                // Additional check: only block if sheet is actually visible/not-hidden
                if (!bottomSheet.ClassListContains("details-sheet--hidden"))
                {
                    return true;
                }
            }

            // Check InfoBarPeek button explicitly
            VisualElement infoBarPeek = _mainUIDocument.rootVisualElement.Q<VisualElement>("InfoBarPeek");
            if (infoBarPeek != null && IsElementVisible(infoBarPeek) && ElementContainsPoint(infoBarPeek, panelPos))
            {
                return true;
            }

            return false;
        }

        private bool IsPointerOverBlockingElement(Vector2 panelPos)
        {
            if (_mainUIDocument?.rootVisualElement == null) return false;

            bool sheetVisible = IsDetailsSheetVisible();

            // Sheet-only blockers must not apply when the sheet is hidden.
            if (sheetVisible)
            {
                if (IsPointerOverNamedElement("SheetCloseBtn", panelPos)
                    || IsPointerOverNamedElement("BottomSheet", panelPos)
                    || IsPointerOverSheetScrollRegion(panelPos))
                {
                    return true;
                }
            }

            // Global blockers (toolbar, mode buttons, sliders, etc.)
            return IsPointerOverNamedElement("InfoBarPeek", panelPos, requireVisibleClass: "info-bar-peek--hidden")
                || IsPointerOverInteractiveUI(panelPos);
        }

        private bool IsPointerOverGlobalSelectionControl(Vector2 panelPos)
        {
            return IsPointerOverNamedElement("InfoBarPeek", panelPos, requireVisibleClass: "info-bar-peek--hidden")
                || IsPointerOverInteractiveUI(panelPos);
        }

        private bool IsPanelPositionOverDetailsSheet(Vector2 panelPos)
        {
            return IsPointerOverNamedElement("SheetCloseBtn", panelPos)
                || IsPointerOverNamedElement("BottomSheet", panelPos)
                || IsPointerOverNamedElement("SheetContent_Details", panelPos)
                || IsPointerOverSheetScrollRegion(panelPos);
        }

        private bool IsScreenPositionOverNamedElement(string elementName, Vector2 screenPos, string requireVisibleClass = null)
        {
            VisualElement element = _mainUIDocument?.rootVisualElement?.Q<VisualElement>(elementName);
            if (!IsElementVisible(element)) return false;

            if (!string.IsNullOrEmpty(requireVisibleClass) && element.ClassListContains(requireVisibleClass))
            {
                return false;
            }

            return ElementContainsPoint(element, screenPos);
        }

        private bool IsScreenPositionOverInteractiveElement(Vector2 screenPos)
        {
            return IsScreenPositionOverNamedElement("InfoBarPeek", screenPos, requireVisibleClass: "info-bar-peek--hidden")
                || IsScreenPositionOverNamedElement("HomeBtn", screenPos)
                || IsScreenPositionOverNamedElement("ResetViewBtn", screenPos)
                || IsScreenPositionOverNamedElement("ToolHotspotBtn", screenPos)
                || IsScreenPositionOverNamedElement("ToolIsolateBtn", screenPos)
                || IsScreenPositionOverNamedElement("ToolPowerBtn", screenPos)
                || IsScreenPositionOverNamedElement("AnalyzeCrossSectionBtn", screenPos)
                || IsScreenPositionOverNamedElement("AnalyzeExplodeBtn", screenPos)
                || IsScreenPositionOverNamedElement("AnalyzeFilterBtn", screenPos)
                || IsScreenPositionOverNamedElement("ExplosionSlider", screenPos)
                || IsScreenPositionOverNamedElement("CrossSectionPosition", screenPos)
                || IsScreenPositionOverNamedElement("CrossSectionPosition2", screenPos)
                || IsScreenPositionOverNamedElement("ToolPowerLoadSlider", screenPos);
        }

        private bool IsDetailsSheetVisible()
        {
            VisualElement bottomSheet = _mainUIDocument?.rootVisualElement?.Q<VisualElement>("BottomSheet");
            return IsElementVisible(bottomSheet) && !bottomSheet.ClassListContains("details-sheet--hidden");
        }

        private bool IsPointerOverSheetScrollRegion(Vector2 panelPos)
        {
            if (_mainUIDocument?.rootVisualElement == null) return false;

            VisualElement root = _mainUIDocument.rootVisualElement;
            ScrollView sheetScroll = root.Q<ScrollView>(className: "sheet-scroll");
            if (!IsElementVisible(sheetScroll)) return false;

            VisualElement[] candidates =
            {
                sheetScroll.Q<VisualElement>(className: "unity-content-and-vertical-scroll-container"),
                sheetScroll.Q<VisualElement>(className: "unity-content-container"),
                sheetScroll.contentViewport,
                sheetScroll.contentContainer
            };

            foreach (VisualElement candidate in candidates)
            {
                if (IsElementVisible(candidate) && ElementContainsPoint(candidate, panelPos))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPointerOverInteractiveUI(Vector2 panelPos)
        {
            if (_mainUIDocument?.rootVisualElement == null) return false;

            VisualElement root = _mainUIDocument.rootVisualElement;

            if (IsPointerOverHotspotDot(root, panelPos))
            {
                return true;
            }

            string[] namedControls =
            {
                "HomeBtn",
                "ResetViewBtn",
                "ToolHotspotBtn",
                "ToolIsolateBtn",
                "ToolPowerBtn",
                "AnalyzeCrossSectionBtn",
                "AnalyzeExplodeBtn",
                "AnalyzeFilterBtn",
                "ShaderMode_Realistic",
                "ShaderMode_XRay",
                "ShaderMode_Blueprint",
                "ShaderMode_SolidColor",
                "CrossSectionAxisX",
                "CrossSectionAxisY",
                "CrossSectionAxisZ",
                "CrossSectionInvertBtn",
                "CrossSectionCombineBtn",
                "CrossSectionInvertBtn2",
                "CatBtn_Structure",
                "CatBtn_Propulsion",
                "CatBtn_Avionics",
                "CatBtn_Sensors",
                "CatBtn_Power",
                "CatBtn_Payload",
                "ToolPowerLoadSlider",
                "ExplosionSlider",
                "CrossSectionPosition",
                "CrossSectionPosition2"
            };

            foreach (string name in namedControls)
            {
                VisualElement element = root.Q<VisualElement>(name);
                if (IsElementVisible(element) && ElementContainsPoint(element, panelPos))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPointerOverHotspotDot(VisualElement root, Vector2 panelPos)
        {
            if (root == null)
            {
                return false;
            }

            bool hit = false;
            root.Query<VisualElement>(className: "hotspot-dot").ForEach(dot =>
            {
                if (hit || dot == null)
                {
                    return;
                }

                if (!IsElementVisible(dot) || dot.ClassListContains("hotspot-dot--hidden"))
                {
                    return;
                }

                if (ElementContainsPoint(dot, panelPos))
                {
                    hit = true;
                }
            });

            return hit;
        }

        private bool IsPointerOverNamedElement(string elementName, Vector2 panelPos, string requireVisibleClass = null)
        {
            VisualElement element = _mainUIDocument?.rootVisualElement?.Q<VisualElement>(elementName);
            if (!IsElementVisible(element)) return false;

            if (!string.IsNullOrEmpty(requireVisibleClass) && element.ClassListContains(requireVisibleClass))
            {
                return false;
            }

            return ElementContainsPoint(element, panelPos);
        }

        private bool IsNamedSurfaceAtPoint(string elementName, Vector2 panelPos)
        {
            VisualElement element = _mainUIDocument?.rootVisualElement?.Q<VisualElement>(elementName);
            return IsElementVisible(element) && ElementContainsPoint(element, panelPos);
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

        private static bool IsFinite(Vector2 value)
        {
            return IsFinite(value.x) && IsFinite(value.y);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static Vector2 MirrorScreenY(Vector2 screenPos)
        {
            return new Vector2(screenPos.x, Screen.height - screenPos.y);
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
            if (element == null) return false;
            
            // Special case: BottomSheet toggles pickingMode with visibility, but
            // the full visible sheet must block 3D input even when passive child
            // labels are hit.
            if (element.name == "BottomSheet")
            {
                return IsElementVisible(element) && !element.ClassListContains("details-sheet--hidden");
            }

            return element.pickingMode == PickingMode.Position && IsElementVisible(element);
        }

        private static bool IsElementVisible(VisualElement element)
        {
            if (element == null)
            {
                return false;
            }

            for (VisualElement current = element; current != null; current = current.parent)
            {
                if (!current.enabledInHierarchy
                    || current.resolvedStyle.display == DisplayStyle.None
                    || current.resolvedStyle.visibility == Visibility.Hidden)
                {
                    return false;
                }

                if (current.ClassListContains("info-bar-peek--hidden")
                    || current.ClassListContains("info-bar-peek--sheet-open")
                    || current.ClassListContains("selection-label--hidden")
                    || current.ClassListContains("details-sheet--hidden"))
                {
                    return false;
                }
            }

            return true;
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

            // Special cases for named containers that should block selection
            if (element.name == "SheetCloseBtn" || element.name == "InfoBarPeek")
            {
                return true;
            }

            if (element.ClassListContains("mode-submenu") ||
                element.ClassListContains("hero-container") ||
                element.ClassListContains("hero-submenu") ||
                element.ClassListContains("onboard-overlay") ||
                element.ClassListContains("sheet-foldout"))
            {
                return true;
            }

            string name = element.name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                string lower = name.ToLowerInvariant();
                if (lower.Contains("btn") || lower.Contains("button") ||
                    lower.Contains("slider") || lower.Contains("scroll") ||
                    lower.Contains("foldout"))
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

            // IMPORTANT: BottomSheet and SheetContent_Details should NOT be pass-through.
            // They should block 3D interaction. Only specific internal elements allow pass-through.
            // Removed: element.name == "BottomSheet" || element.name == "SheetContent_Details"

            // Sheet header allows dragging through to select/deselect
            if (element.ClassListContains("sheet-header"))
            {
                return true;
            }

            // Specific content containers - these also should NOT pass through
            // The children (labels, values) are passive and don't block, but the container itself doesn't either
            // Removed: element.ClassListContains("details-sheet") || element.ClassListContains("sheet-content")

            return false;
        }
    }
}
