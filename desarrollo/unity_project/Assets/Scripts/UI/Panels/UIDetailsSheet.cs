using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Data;
using System.Collections.Generic;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the bottom details sheet: open/close, drag-to-dismiss,
    /// and populating part data fields.
    /// Extracted from UIManager (Phase 3 Step 2: God Class Dismantling).
    /// </summary>
    public class UIDetailsSheet
    {
        // ── Elements ──
        private readonly VisualElement _root;
        private readonly VisualElement _detailsSheet;
        private readonly VisualElement _bottomBar;
        private readonly VisualElement _contentDetails;
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

        // ── State ──
        public bool IsSheetOpen { get; private set; } = false;
        private float _dragStartY;
        private bool _isDraggingSheet;

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
            _contentDetails = root.Q<VisualElement>("SheetContent_Details");
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
            if (_partNameLabel != null) _partNameLabel.EnableInClassList("selection-label--hidden", isOpen);

            if (OrbitCameraController.Instance != null)
                OrbitCameraController.Instance.SetViewportShift(isOpen ? 0.15f : 0f);

            OnSheetStateChanged?.Invoke(isOpen);
        }

        public void OpenSheet()
        {
            if (_contentDetails == null) return;

            _contentDetails.RemoveFromClassList("sheet-content--hidden");
            _contentDetails.AddToClassList("sheet-content--active");

            string titleText = (SelectionManager.Instance != null && SelectionManager.Instance.HasSelection)
                ? (_sheetTitle != null ? _sheetTitle.text : "PART DETAILS")
                : "SELECT A PART";

            if (_sheetTitle != null) _sheetTitle.text = titleText;
            SetSheetState(true);
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

            if (data != null && !string.IsNullOrEmpty(data.PartName) && data.PartName != "NULL")
            {
                if (_infoBtn != null) _infoBtn.SetEnabled(true);

                if (_sheetTitle != null) _sheetTitle.text = data.PartName.ToUpper();
                if (_sheetCategory != null) _sheetCategory.text = data.Category;
                if (_sheetFunction != null) _sheetFunction.text = data.Function;
                if (_sheetMaterial != null) _sheetMaterial.text = data.MaterialType;
                if (_sheetDesc != null) _sheetDesc.text = data.Description;
                if (_sheetWeight != null) _sheetWeight.text = $"{data.Weight:F2} kg";
                if (_sheetDimensions != null) _sheetDimensions.text = data.Dimensions;
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

                if (fromHotspot) OpenSheet();
            }
            else
            {
                // Deselection
                if (_partNameLabel != null) _partNameLabel.AddToClassList("selection-label--hidden");
                if (_infoBtn != null) _infoBtn.SetEnabled(false);
                SetSheetState(false);
            }
        }

        public void UpdatePartIndicator(DronePartData data)
        {
            if (_partNameLabel == null) return;

            if (data != null)
            {
                _partNameLabel.text = data.PartName.ToUpper();
                _partNameLabel.style.color = new StyleColor(new Color(0.06f, 0.73f, 0.5f));
            }
            else
            {
                _partNameLabel.text = "SELECT A PART";
                _partNameLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.7f));
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

            // Header click to toggle
            var header = _root.Q(className: "sheet-header");
            if (header != null)
            {
                EventCallback<ClickEvent> headerClick = evt => SetSheetState(!IsSheetOpen);
                header.RegisterCallback(headerClick);
                AddCleanup(() => header.UnregisterCallback(headerClick));
            }

            // Drag-to-dismiss handle
            var handle = _root.Q(className: "sheet-handle");
            if (handle != null)
            {
                EventCallback<PointerDownEvent> handleDown = evt => { _dragStartY = evt.position.y; _isDraggingSheet = true; };
                EventCallback<PointerUpEvent> handleUp = evt => _isDraggingSheet = false;
                EventCallback<PointerLeaveEvent> handleLeave = evt => _isDraggingSheet = false;
                EventCallback<PointerMoveEvent> handleMove = evt =>
                {
                    if (_isDraggingSheet && (evt.position.y - _dragStartY > 50))
                    {
                        SetSheetState(false);
                        _isDraggingSheet = false;
                    }
                };
                handle.RegisterCallback(handleDown);
                handle.RegisterCallback(handleUp);
                handle.RegisterCallback(handleLeave);
                handle.RegisterCallback(handleMove);
                AddCleanup(() =>
                {
                    handle.UnregisterCallback(handleDown);
                    handle.UnregisterCallback(handleUp);
                    handle.UnregisterCallback(handleLeave);
                    handle.UnregisterCallback(handleMove);
                });
            }

            // ScrollView blocks camera zoom
            var sheetScroll = _root.Q<ScrollView>(className: "sheet-scroll");
            if (sheetScroll != null)
            {
                EventCallback<PointerEnterEvent> scrollEnter = evt => InputManager.InputBlocked = true;
                EventCallback<PointerLeaveEvent> scrollLeave = evt => InputManager.InputBlocked = false;
                sheetScroll.RegisterCallback(scrollEnter);
                sheetScroll.RegisterCallback(scrollLeave);
                AddCleanup(() => { sheetScroll.UnregisterCallback(scrollEnter); sheetScroll.UnregisterCallback(scrollLeave); });
            }

            // Sheet close button
            var sheetCloseBtn = _root.Q<Button>("SheetCloseBtn");
            if (sheetCloseBtn != null)
            {
                EventCallback<ClickEvent> onSheetClose = evt =>
                {
                    evt.StopPropagation();
                    SetSheetState(false);
                };
                sheetCloseBtn.RegisterCallback(onSheetClose);
                AddCleanup(() => sheetCloseBtn.UnregisterCallback(onSheetClose));
            }

            // Info button
            if (_infoBtn != null)
            {
                System.Action onInfoClick = ToggleInfo;
                _infoBtn.clicked += onInfoClick;
                AddCleanup(() => _infoBtn.clicked -= onInfoClick);
            }
        }
    }
}
