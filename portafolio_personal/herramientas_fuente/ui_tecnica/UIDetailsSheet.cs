using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Data;
using System.Collections.Generic;
using System.Globalization;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the bottom details sheet: open/close and populating part data fields.
    /// Extracted from UIManager (Phase 3 Step 2: God Class Dismantling).
    /// </summary>
    public class UIDetailsSheet
    {
        // ── Elements ──
        private readonly VisualElement _root;
        private readonly VisualElement _detailsSheet;
        private readonly VisualElement _bottomBar;
        private readonly Button _infoBarPeek;
        private readonly Button _sheetCloseBtn;
        private readonly VisualElement _sheetHandle;
        private readonly VisualElement _sheetHeader;
        private readonly VisualElement _contentDetails;
        private readonly ScrollView _sheetScroll;
        private readonly Label _partNameLabel;
        private readonly Label _sheetTitle;
        private readonly Label _sheetCategory;
        private readonly Label _sheetFunction;
        private readonly Label _sheetMaterial;
        private readonly Label _sheetDesc;
        private readonly Label _sheetWeight;
        private readonly Label _sheetDimensions;
        private readonly Label _sheetPower;
        private readonly Label _sheetTemp;
        private readonly Label _sheetDifficulty;
        private readonly Label _sheetTools;
        private readonly Label _sheetAssemblyTime;
        private readonly Button _infoBtn;
        private readonly VisualElement _actionsRow;

        // ── Accent colors (adaptive) ──
        private static readonly Color AccentDark  = new Color(0.06f, 0.73f, 0.5f);   // teal on dark bg
        private static readonly Color AccentLight = new Color(0.02f, 0.48f, 0.35f);   // deeper teal on light bg
        private static readonly Color MutedDark   = new Color(0.6f, 0.6f, 0.7f);      // placeholder on dark bg
        private static readonly Color MutedLight  = new Color(0.35f, 0.35f, 0.4f);    // placeholder on light bg
        private bool _isLightBg;

        // ── State ──
        public bool IsSheetOpen { get; private set; } = false;
        private float _gestureStartY;
        private bool _isOpeningGestureActive;
        private bool _isClosingGestureActive;

        private const float OPEN_GESTURE_THRESHOLD = 44f;
        private const float CLOSE_GESTURE_THRESHOLD = 72f;

        // ── Top context label (updates "SELECT A PART" ↔ part name) ──
        private readonly Label _topContextLabel;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        // ── Callbacks ──
        /// <summary>Fired when sheet state changes. Param = isOpen.</summary>
        public event System.Action<bool> OnSheetStateChanged;

        public UIDetailsSheet(VisualElement root, Button infoBtn)
        {
            _root = root;
            _infoBtn = infoBtn;

            _detailsSheet = root.Q<VisualElement>("BottomSheet");
            _bottomBar = root.Q<VisualElement>("BottomBar");
            _infoBarPeek = root.Q<Button>("InfoBarPeek");
            _sheetCloseBtn = root.Q<Button>("SheetCloseBtn");
            _sheetHandle = root.Q<VisualElement>(className: "sheet-handle");
            _sheetHeader = root.Q<VisualElement>(className: "sheet-header");
            _contentDetails = root.Q<VisualElement>("SheetContent_Details");
            _sheetScroll = root.Q<ScrollView>(className: "sheet-scroll");
            _partNameLabel = root.Q<Label>("SelectionIndicator");

            _sheetTitle = root.Q<Label>("SheetTitle");
            _sheetCategory = root.Q<Label>("PartCategory");
            _sheetFunction = root.Q<Label>("PartFunction");
            _sheetMaterial = root.Q<Label>("PartMaterial");
            _sheetDesc = root.Q<Label>("PartDescription");
            _sheetWeight = root.Q<Label>("PartWeight");
            _sheetDimensions = root.Q<Label>("PartDimensions");
            _sheetPower = root.Q<Label>("PartPower");
            _sheetTemp = root.Q<Label>("PartTemp");
            _sheetDifficulty = root.Q<Label>("PartDifficulty");
            _sheetTools = root.Q<Label>("PartTools");
            _sheetAssemblyTime = root.Q<Label>("PartAssemblyTime");

            _topContextLabel = root.Q<Label>("TopContextLabel");
            _actionsRow = root.Q<VisualElement>(className: "actions-row");

            if (_infoBarPeek != null)
            {
                _infoBarPeek.clicked += ShowInfo;
                AddCleanup(() => _infoBarPeek.clicked -= ShowInfo);
            }

            if (_sheetCloseBtn != null)
            {
                System.Action onClose = () => SetSheetState(false);
                _sheetCloseBtn.clicked += onClose;
                AddCleanup(() => _sheetCloseBtn.clicked -= onClose);

                // Stop click from bubbling to parent containers
                EventCallback<ClickEvent> stopClick = evt => evt.StopPropagation();
                _sheetCloseBtn.RegisterCallback(stopClick);
                AddCleanup(() => _sheetCloseBtn.UnregisterCallback(stopClick));
            }

            BindInteractions();
        }

        private void AddCleanup(System.Action action)
        {
            if (action != null) _cleanupActions.Add(action);
        }

        public void Dispose()
        {
            OnSheetStateChanged = null;
            foreach (var action in _cleanupActions) action?.Invoke();
            _cleanupActions.Clear();
        }

        /// <summary>
        /// Called by UIManager when the environment changes between light/dark.
        /// Re-applies inline accent colors so they match the new background.
        /// </summary>
        public void SetLightBackground(bool isLight)
        {
            _isLightBg = isLight;

            // Re-apply accent on labels that use inline style.color
            if (_partNameLabel != null && !_partNameLabel.ClassListContains("selection-label--hidden"))
                _partNameLabel.style.color = new StyleColor(isLight ? AccentLight : AccentDark);

            if (_topContextLabel != null)
            {
                bool hasSelection = SelectionManager.Instance?.HasSelection == true;
                _topContextLabel.style.color = new StyleColor(
                    hasSelection ? (isLight ? AccentLight : AccentDark) : (isLight ? MutedLight : MutedDark));
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Public API
        // ═══════════════════════════════════════════════════════

        public void SetSheetState(bool isOpen)
        {
            IsSheetOpen = isOpen;

            if (_detailsSheet != null)
            {
                if (isOpen)
                {
                    _detailsSheet.RemoveFromClassList("details-sheet--hidden");
                    _detailsSheet.pickingMode = PickingMode.Position;
                }
                else
                {
                    _detailsSheet.AddToClassList("details-sheet--hidden");
                    _detailsSheet.pickingMode = PickingMode.Ignore;
                }
            }

            if (_bottomBar != null) _bottomBar.EnableInClassList("ui-shifted", isOpen);
            if (_bottomBar != null)
            {
                _bottomBar.EnableInClassList("ui-shifted", isOpen);
                
                // If the sheet is open, we don't need soft shift. 
                // We re-enable soft shift only if a part is selected AND the sheet is closed.
                if (isOpen) 
                {
                    _bottomBar.RemoveFromClassList("ui-shifted-soft");
                }
                else
                {
                    bool hasSelection = (SelectionManager.Instance?.HasSelection == true);
                    _bottomBar.EnableInClassList("ui-shifted-soft", hasSelection);
                }
            }

            if (_infoBarPeek != null) _infoBarPeek.EnableInClassList("info-bar-peek--sheet-open", isOpen);
            if (_actionsRow != null) _actionsRow.EnableInClassList("actions-row--sheet-open", isOpen);
            if (_partNameLabel != null) _partNameLabel.EnableInClassList("selection-label--hidden", isOpen);

            OrbitCameraController.Instance?.SetViewportShift(isOpen ? 0.15f : 0f);

            // Focus camera on the selected part when opening the sheet
            if (isOpen)
            {
                var sel = SelectionManager.Instance?.CurrentSelection;
                if (sel != null)
                    OrbitCameraController.Instance?.FocusOnObject(sel.transform);
            }

            OnSheetStateChanged?.Invoke(isOpen);
        }

        public void OpenSheet()
        {
            if (_contentDetails == null) return;

            _contentDetails.RemoveFromClassList("sheet-content--hidden");
            _contentDetails.AddToClassList("sheet-content--active");

            string titleText = (SelectionManager.Instance?.HasSelection == true)
                ? (_sheetTitle != null ? _sheetTitle.text : "PART DETAILS")
                : "SELECT A PART";

            if (_sheetTitle != null) _sheetTitle.text = titleText;
            SetSheetState(true);
        }

        public void ShowInfo()
        {
            if (!IsSheetOpen)
                OpenSheet();
        }

        public void ToggleInfo()
        {
            if (IsSheetOpen) SetSheetState(false);
            else OpenSheet();
        }

        /// <summary>Populate sheet fields from part data. Pass null to show deselected state.</summary>
        public void PopulatePartData(DronePartData data, bool fromHotspot)
        {
            UpdatePartIndicator(data);

            if (data != null && !string.IsNullOrEmpty(data.partName) && data.partName != "NULL")
            {
                if (_infoBtn != null) _infoBtn.SetEnabled(true);

                // Title Case for editorial hierarchy (not ALL CAPS)
                if (_sheetTitle != null)
                    _sheetTitle.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data.partName.ToLower());
                if (_sheetCategory != null) _sheetCategory.text = data.category;
                if (_sheetFunction != null) _sheetFunction.text = data.function;
                if (_sheetMaterial != null) _sheetMaterial.text = data.materialType;
                if (_sheetDesc != null) _sheetDesc.text = data.description;
                if (_sheetWeight != null) _sheetWeight.text = $"{data.weightKg:F2} kg";
                if (_sheetDimensions != null) _sheetDimensions.text = data.dimensions;
                if (_sheetPower != null) _sheetPower.text = data.powerConsumption > 0 ? $"{data.powerConsumption:F1} W" : "N/A";
                if (_sheetTemp != null) _sheetTemp.text = data.operatingTemp > 0 ? $"{data.operatingTemp:F0}°C" : "N/A";

                if (_sheetDifficulty != null)
                {
                    int d = Mathf.Clamp(data.difficultyLevel, 0, 5);
                    _sheetDifficulty.text = new string('★', d) + new string('☆', 5 - d);
                }
                if (_sheetTools != null)
                {
                    _sheetTools.text = (data.requiredTools != null && data.requiredTools.Length > 0)
                        ? string.Join(", ", data.requiredTools)
                        : "None";
                }
                if (_sheetAssemblyTime != null)
                    _sheetAssemblyTime.text = data.installationTimeMinutes > 0 ? $"~{data.installationTimeMinutes:F0} min" : "N/A";

                if (fromHotspot && !IsSheetOpen) OpenSheet();

                // PRE-PEEK INFO BAR LOGIC: Show when part is selected
                if (_infoBarPeek != null && !IsSheetOpen)
                {
                    _infoBarPeek.RemoveFromClassList("info-bar-peek--hidden");
                    _infoBarPeek.AddToClassList("info-bar-peek--visible");
                    
                    // Nudge bottom bar soft up
                    if (_bottomBar != null) _bottomBar.AddToClassList("ui-shifted-soft");
                }
            }
            else
            {
                // Deselection — clear indicators but keep sheet open
                // (sheet closes only via FAB info button or mode change)
                if (_partNameLabel != null) _partNameLabel.AddToClassList("selection-label--hidden");
                if (_infoBtn != null) _infoBtn.SetEnabled(false);

                // PRE-PEEK INFO BAR LOGIC: Hide when nothing is selected
                if (_infoBarPeek != null)
                {
                    _infoBarPeek.RemoveFromClassList("info-bar-peek--visible");
                    _infoBarPeek.AddToClassList("info-bar-peek--hidden");
                    
                    // Drop bottom bar back down if the sheet isn't open
                    if (_bottomBar != null && !IsSheetOpen) _bottomBar.RemoveFromClassList("ui-shifted-soft");
                }
            }
        }

        public void UpdatePartIndicator(DronePartData data)
        {
            if (data != null)
            {
                string upperName = data.partName.ToUpper();

                // Bottom selection indicator — show part name
                if (_partNameLabel != null)
                {
                    _partNameLabel.text = upperName;
                    _partNameLabel.style.color = new StyleColor(_isLightBg ? AccentLight : AccentDark);
                    _partNameLabel.RemoveFromClassList("selection-label--hidden");
                }

                // Top context label — show part name too
                if (_topContextLabel != null)
                {
                    _topContextLabel.text = upperName;
                    _topContextLabel.style.color = new StyleColor(_isLightBg ? AccentLight : AccentDark);
                }
            }
            else
            {
                // No selection — hide bottom indicator, top shows "SELECT A PART"
                if (_partNameLabel != null)
                    _partNameLabel.AddToClassList("selection-label--hidden");

                if (_topContextLabel != null)
                {
                    _topContextLabel.text = "SELECT A PART";
                    _topContextLabel.style.color = new StyleColor(_isLightBg ? MutedLight : MutedDark);
                }
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Private — interaction bindings
        // ═══════════════════════════════════════════════════════

        private void BindInteractions()
        {
            // Details sheet input blocking
            if (_detailsSheet != null)
            {
                EventCallback<PointerDownEvent> pd = evt => evt.StopPropagation();
                EventCallback<PointerUpEvent> pu = evt => evt.StopPropagation();
                _detailsSheet.RegisterCallback(pd);
                _detailsSheet.RegisterCallback(pu);
                AddCleanup(() => { _detailsSheet.UnregisterCallback(pd); _detailsSheet.UnregisterCallback(pu); });

                EventCallback<PointerEnterEvent> pe = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> pl = evt => InputManager.InputBlocked = false;
                _detailsSheet.RegisterCallback(pe);
                _detailsSheet.RegisterCallback(pl);
                AddCleanup(() => { _detailsSheet.UnregisterCallback(pe); _detailsSheet.UnregisterCallback(pl); });
            }

            // ScrollView blocks camera zoom
            if (_sheetScroll != null)
            {
                EventCallback<PointerEnterEvent> scrollEnter = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> scrollLeave = evt => InputManager.InputBlocked = false;
                _sheetScroll.RegisterCallback(scrollEnter);
                _sheetScroll.RegisterCallback(scrollLeave);
                AddCleanup(() => { _sheetScroll.UnregisterCallback(scrollEnter); _sheetScroll.UnregisterCallback(scrollLeave); });
            }

            // Close button
            if (_sheetCloseBtn != null)
            {
                EventCallback<PointerDownEvent> closePd = evt => evt.StopPropagation();
                _sheetCloseBtn.RegisterCallback(closePd);
                AddCleanup(() => _sheetCloseBtn.UnregisterCallback(closePd));
            }

            // Info button
            if (_infoBtn != null)
            {
                System.Action onInfoClick = ToggleInfo;
                _infoBtn.clicked += onInfoClick;
                AddCleanup(() => _infoBtn.clicked -= onInfoClick);
            }

            BindOpenGesture();
            BindCloseGesture(_sheetHandle);
            BindCloseGesture(_sheetHeader);
        }

        private void BindOpenGesture()
        {
            if (_infoBarPeek == null)
                return;

            EventCallback<PointerDownEvent> onDown = evt =>
            {
                if (IsSheetOpen || SelectionManager.Instance?.HasSelection != true)
                    return;

                _gestureStartY = evt.position.y;
                _isOpeningGestureActive = true;
                _infoBarPeek.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            };

            EventCallback<PointerMoveEvent> onMove = evt =>
            {
                if (!_isOpeningGestureActive)
                    return;

                float deltaY = _gestureStartY - evt.position.y;
                if (deltaY >= OPEN_GESTURE_THRESHOLD)
                {
                    OpenSheet();
                    CancelOpeningGesture(evt.pointerId);
                }
            };

            EventCallback<PointerUpEvent> onUp = evt => CancelOpeningGesture(evt.pointerId);
            EventCallback<PointerCancelEvent> onCancel = evt => CancelOpeningGesture(evt.pointerId);
            EventCallback<PointerCaptureOutEvent> onCaptureOut = evt => _isOpeningGestureActive = false;

            _infoBarPeek.RegisterCallback(onDown);
            _infoBarPeek.RegisterCallback(onMove);
            _infoBarPeek.RegisterCallback(onUp);
            _infoBarPeek.RegisterCallback(onCancel);
            _infoBarPeek.RegisterCallback(onCaptureOut);
            AddCleanup(() =>
            {
                _infoBarPeek.UnregisterCallback(onDown);
                _infoBarPeek.UnregisterCallback(onMove);
                _infoBarPeek.UnregisterCallback(onUp);
                _infoBarPeek.UnregisterCallback(onCancel);
                _infoBarPeek.UnregisterCallback(onCaptureOut);
            });
        }

        private void BindCloseGesture(VisualElement gestureSource)
        {
            if (gestureSource == null)
                return;

            EventCallback<PointerDownEvent> onDown = evt =>
            {
                if (!IsSheetOpen)
                    return;

                _gestureStartY = evt.position.y;
                _isClosingGestureActive = true;
                gestureSource.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            };

            EventCallback<PointerMoveEvent> onMove = evt =>
            {
                if (!_isClosingGestureActive)
                    return;

                float deltaY = evt.position.y - _gestureStartY;
                if (deltaY >= CLOSE_GESTURE_THRESHOLD)
                {
                    SetSheetState(false);
                    CancelClosingGesture(gestureSource, evt.pointerId);
                }
            };

            EventCallback<PointerUpEvent> onUp = evt => CancelClosingGesture(gestureSource, evt.pointerId);
            EventCallback<PointerCancelEvent> onCancel = evt => CancelClosingGesture(gestureSource, evt.pointerId);
            EventCallback<PointerCaptureOutEvent> onCaptureOut = evt => _isClosingGestureActive = false;

            gestureSource.RegisterCallback(onDown);
            gestureSource.RegisterCallback(onMove);
            gestureSource.RegisterCallback(onUp);
            gestureSource.RegisterCallback(onCancel);
            gestureSource.RegisterCallback(onCaptureOut);
            AddCleanup(() =>
            {
                gestureSource.UnregisterCallback(onDown);
                gestureSource.UnregisterCallback(onMove);
                gestureSource.UnregisterCallback(onUp);
                gestureSource.UnregisterCallback(onCancel);
                gestureSource.UnregisterCallback(onCaptureOut);
            });
        }

        private void CancelOpeningGesture(int pointerId)
        {
            if (_infoBarPeek != null && _infoBarPeek.HasPointerCapture(pointerId))
                _infoBarPeek.ReleasePointer(pointerId);

            _isOpeningGestureActive = false;
        }

        private void CancelClosingGesture(VisualElement gestureSource, int pointerId)
        {
            if (gestureSource != null && gestureSource.HasPointerCapture(pointerId))
                gestureSource.ReleasePointer(pointerId);

            _isClosingGestureActive = false;
        }
    }
}
