using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Manages the bottom details sheet: open/close and populating part data fields.
    /// Extracted from UIManager (Phase 3 Step 2: God Class Dismantling).
    /// </summary>
    public class UIDetailsSheet
    {
        // ── Singleton Instance ──
        private static UIDetailsSheet _instance;
        public static UIDetailsSheet Instance => _instance;

        // ── Elements ──
        private readonly VisualElement _root;
        private readonly VisualElement _detailsSheet;
        private readonly VisualElement _bottomBar;
        private readonly VisualElement _infoBarPeek;
        private readonly VisualElement _sheetHandle;
        private readonly VisualElement _sheetHeader;
        private readonly Button _sheetCloseBtn;
        private readonly VisualElement _contentDetails;
        private readonly Label _partNameLabel;
        private readonly Label _sheetTitle;
        private readonly Label _sheetCategory;
        private readonly Label _sheetFunction;
        private readonly Label _sheetMaterial;
        private readonly Label _sheetDesc;
        private readonly Label _sheetKeyName;
        private readonly Label _sheetPartNumber;
        private readonly Label _sheetMaker;
        private readonly Label _sheetReferences;
        private readonly Label _sheetWeight;
        private readonly Label _sheetDimensions;
        private readonly Label _sheetPower;
        private readonly Label _sheetTemp;
        private readonly Label _sheetDifficulty;
        private readonly Label _sheetTools;
        private readonly Label _sheetAssemblyTime;
        private readonly Label _sheetTorque;
        private readonly Label _sheetConnections;
        private readonly Label _sheetSafety;
        private readonly Label _sheetServiceNote;
        private readonly Foldout _foldoutIdentification;
        private readonly Foldout _foldoutSpecifications;
        private readonly Foldout _foldoutRelationship;
        private readonly Foldout _foldoutAssembly;
        private readonly Foldout _foldoutReferences;
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
        private float _swipeStartY;
        private float _swipeStartX;
        private bool _isSwipingUp;
        private bool _isSwipingDown;
        private bool _infoPeekTriggeredOpen;
        private bool _sheetOpenSwipeTriggered;
        private bool _sheetDragTriggeredClose;
        private bool _isInfoPeekPressed;

        private const float SheetSwipeThreshold = 56f;
        private const float SheetSwipeDominance = 1.25f;
        private const float SheetTransitionInputBlockSeconds = 0.45f;

        private bool HasActiveSheetGesture =>
            _isInfoPeekPressed || _isSwipingUp || _isSwipingDown;

        // ── Top context label (updates "SELECT A PART" ↔ part name) ──
        private readonly Label _topContextLabel;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();
        private DronePartData _lastData;
        private bool _lastFromHotspot;
        private string _lastHotspotGroupLabel = "";
        private string _lastHotspotGroupSummary = "";
        private string _lastHotspotGroupMembers = "";
        private string _lastSelectionLabel = "";
        private string _lastCanonicalPartName = "";

        // ── Callbacks ──
        /// <summary>Fired when sheet state changes. Param = isOpen.</summary>
        public event System.Action<bool> OnSheetStateChanged;

        public UIDetailsSheet(VisualElement root, Button infoBtn)
        {
            _instance = this;  // ✅ Set singleton instance
            _root = root;
            _infoBtn = infoBtn;

            _detailsSheet = root.Q<VisualElement>("BottomSheet");
            _bottomBar = root.Q<VisualElement>("BottomBar");
            _infoBarPeek = root.Q<VisualElement>("InfoBarPeek");
            _sheetHandle = root.Q<VisualElement>(className: "sheet-handle");
            _sheetHeader = root.Q<VisualElement>(className: "sheet-header");
            _sheetCloseBtn = root.Q<Button>("SheetCloseBtn");
            _contentDetails = root.Q<VisualElement>("SheetContent_Details");
            _partNameLabel = root.Q<Label>("SelectionIndicator");

            _sheetTitle = root.Q<Label>("SheetTitle");
            _sheetCategory = root.Q<Label>("PartCategory");
            _sheetFunction = root.Q<Label>("PartFunction");
            _sheetMaterial = root.Q<Label>("PartMaterial");
            _sheetDesc = root.Q<Label>("PartDescription");
            _sheetKeyName = root.Q<Label>("PartKeyName");
            _sheetPartNumber = root.Q<Label>("PartNumber");
            _sheetMaker = root.Q<Label>("PartMaker");
            _sheetReferences = root.Q<Label>("PartReferences");
            _sheetWeight = root.Q<Label>("PartWeight");
            _sheetDimensions = root.Q<Label>("PartDimensions");
            _sheetPower = root.Q<Label>("PartPower");
            _sheetTemp = root.Q<Label>("PartTemp");
            _sheetDifficulty = root.Q<Label>("PartDifficulty");
            _sheetTools = root.Q<Label>("PartTools");
            _sheetAssemblyTime = root.Q<Label>("PartAssemblyTime");
            _sheetTorque = root.Q<Label>("PartTorque");
            _sheetConnections = root.Q<Label>("PartConnections");
            _sheetSafety = root.Q<Label>("PartSafety");
            _sheetServiceNote = root.Q<Label>("PartServiceNote");
            _foldoutIdentification = root.Q<Foldout>("FoldoutIdentification");
            _foldoutSpecifications = root.Q<Foldout>("FoldoutSpecifications");
            _foldoutRelationship = root.Q<Foldout>("FoldoutRelationship");
            _foldoutAssembly = root.Q<Foldout>("FoldoutAssembly");
            _foldoutReferences = root.Q<Foldout>("FoldoutReferences");

            _topContextLabel = root.Q<Label>("TopContextLabel");
            _actionsRow = root.Q<VisualElement>(className: "actions-row");

            AppLanguageManager.LanguageChanged += OnLanguageChanged;
            AddCleanup(() => AppLanguageManager.LanguageChanged -= OnLanguageChanged);
            AppLanguageManager.ApplyStaticText(_root);

            if (_infoBarPeek != null)
            {
                _infoBarPeek.pickingMode = PickingMode.Ignore;
                SetDescendantsPickingMode(_infoBarPeek, PickingMode.Ignore);

                EventCallback<AttachToPanelEvent> onPeekAttached = _ => SetDescendantsPickingMode(_infoBarPeek, PickingMode.Ignore);
                _infoBarPeek.RegisterCallback(onPeekAttached);
                AddCleanup(() => _infoBarPeek.UnregisterCallback(onPeekAttached));

                EventCallback<PointerDownEvent> peekDown = evt =>
                {
                    if (!IsPrimaryPointerButton(evt.button))
                    {
                        return;
                    }

                    _isInfoPeekPressed = true;
                    _infoPeekTriggeredOpen = false;
                    _swipeStartY = evt.position.y;
                    _swipeStartX = evt.position.x;
                    InputManager.InputBlocked = true;
                    _infoBarPeek.CapturePointer(evt.pointerId);
                    evt.StopPropagation();
                };

                EventCallback<PointerMoveEvent> peekMove = evt =>
                {
                    if (!_isInfoPeekPressed)
                    {
                        return;
                    }

                    evt.StopPropagation();

                    float deltaY = _swipeStartY - evt.position.y;
                    float deltaX = Mathf.Abs(evt.position.x - _swipeStartX);
                    bool isUpSwipe = deltaY > SheetSwipeThreshold
                        && deltaY > deltaX * SheetSwipeDominance;

                    if (!_infoPeekTriggeredOpen && isUpSwipe && SelectionManager.Instance?.HasSelection == true)
                    {
                        _infoPeekTriggeredOpen = true;
                        ShowInfo();
                        InputManager.InputBlocked = true;
                    }
                };

                EventCallback<PointerUpEvent> peekUp = evt =>
                {
                    if (!IsPrimaryPointerButton(evt.button))
                    {
                        return;
                    }

                    bool shouldOpen = _isInfoPeekPressed
                        && !_infoPeekTriggeredOpen
                        && _infoBarPeek.worldBound.Contains(evt.position)
                        && SelectionManager.Instance?.HasSelection == true;

                    _isInfoPeekPressed = false;
                    _infoPeekTriggeredOpen = false;
                    InputManager.InputBlocked = false;
                    _infoBarPeek.ReleasePointer(evt.pointerId);
                    evt.StopPropagation();

                    if (shouldOpen)
                    {
                        ShowInfo();
                    }
                };

                EventCallback<PointerLeaveEvent> peekLeave = evt =>
                {
                    if (_isInfoPeekPressed)
                    {
                        evt.StopPropagation();
                        if (IsPrimaryPointerStillDown())
                        {
                            return;
                        }
                    }

                    _isInfoPeekPressed = false;
                    _infoPeekTriggeredOpen = false;
                    InputManager.InputBlocked = false;
                    _infoBarPeek.ReleasePointer(evt.pointerId);
                };

                _infoBarPeek.RegisterCallback(peekDown);
                _infoBarPeek.RegisterCallback(peekMove);
                _infoBarPeek.RegisterCallback(peekUp);
                _infoBarPeek.RegisterCallback(peekLeave);
                AddCleanup(() =>
                {
                    _infoBarPeek.UnregisterCallback(peekDown);
                    _infoBarPeek.UnregisterCallback(peekMove);
                    _infoBarPeek.UnregisterCallback(peekUp);
                    _infoBarPeek.UnregisterCallback(peekLeave);
                });
            }

            if (_sheetCloseBtn != null)
            {
                System.Action onClose = () => SetSheetState(false);
                _sheetCloseBtn.clicked += onClose;
                AddCleanup(() => _sheetCloseBtn.clicked -= onClose);
                RegisterTransientInputBlock(_sheetCloseBtn);

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
            bool stateChanged = IsSheetOpen != isOpen;
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
                    InputManager.InputBlocked = false;
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
            if (_infoBarPeek != null)
            {
                bool peekInteractive = !isOpen && SelectionManager.Instance?.HasSelection == true;
                _infoBarPeek.pickingMode = peekInteractive ? PickingMode.Position : PickingMode.Ignore;
            }
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

            if (stateChanged)
            {
                InputManager.BlockInputForSeconds(SheetTransitionInputBlockSeconds);
            }
        }

        public void OpenSheet()
        {
            if (_contentDetails == null) return;

            _contentDetails.RemoveFromClassList("sheet-content--hidden");
            _contentDetails.AddToClassList("sheet-content--active");

            string titleText = (SelectionManager.Instance?.HasSelection == true)
                ? (_sheetTitle != null ? _sheetTitle.text : "PART DETAILS")
                : AppLanguageManager.SelectPartPrompt();

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
        public void PopulatePartData(
            DronePartData data,
            bool fromHotspot,
            string hotspotGroupLabel = "",
            string hotspotGroupSummary = "",
            string hotspotGroupMembers = "",
            string selectionLabel = "",
            string canonicalPartName = "")
        {
            CacheLastSelection(data, fromHotspot, hotspotGroupLabel, hotspotGroupSummary, hotspotGroupMembers, selectionLabel, canonicalPartName);
            UpdatePartIndicator(data, selectionLabel, canonicalPartName);

            if (data != null && !string.IsNullOrEmpty(data.partName) && data.partName != "NULL")
            {
                if (_infoBtn != null) _infoBtn.SetEnabled(true);
                bool useFastenerDetails = data.HasFastenerMetadata && string.IsNullOrWhiteSpace(hotspotGroupLabel);
                if (TryRenderCatalogData(data, fromHotspot, hotspotGroupLabel, hotspotGroupSummary, hotspotGroupMembers, selectionLabel, canonicalPartName))
                {
                    ShowPeekForSelectedPart();
                    return;
                }

                RenderFallbackData(data, hotspotGroupLabel, hotspotGroupSummary, hotspotGroupMembers, selectionLabel, canonicalPartName, useFastenerDetails);
                ShowPeekForSelectedPart();
                return;
#if false

                // Title Case for editorial hierarchy (not ALL CAPS)
                if (_sheetTitle != null)
                    _sheetTitle.text = !string.IsNullOrWhiteSpace(hotspotGroupLabel)
                        ? hotspotGroupLabel
                        : BuildReadableTitle(GetDisplayName(selectionLabel, canonicalPartName, data));
                if (_sheetCategory != null)
                    _sheetCategory.text = !string.IsNullOrWhiteSpace(hotspotGroupLabel)
                        ? "System Group"
                        : BuildCategoryText(data, useFastenerDetails);
                if (_sheetFunction != null)
                    _sheetFunction.text = FormatTextOrND(!string.IsNullOrWhiteSpace(hotspotGroupSummary)
                        ? hotspotGroupSummary
                        : BuildFunctionText(data, useFastenerDetails));
                if (_sheetMaterial != null) _sheetMaterial.text = FormatTextOrND(BuildMaterialText(data, useFastenerDetails));
                if (_sheetDesc != null)
                {
                    if (!string.IsNullOrWhiteSpace(hotspotGroupLabel))
                    {
                        _sheetDesc.text = string.IsNullOrWhiteSpace(hotspotGroupMembers)
                            ? FormatTextOrND(SanitizeTechnicalText(data.description))
                            : $"{FormatTextOrND(SanitizeTechnicalText(data.description))}\n\nIncludes: {SanitizeMemberList(hotspotGroupMembers)}";
                    }
                    else
                    {
                        _sheetDesc.text = FormatTextOrND(BuildDescriptionText(data, useFastenerDetails));
                    }
                }
                if (_sheetKeyName != null) _sheetKeyName.text = FormatTextOrND(BuildKeyNameText(data, useFastenerDetails));
                if (_sheetPartNumber != null) _sheetPartNumber.text = FormatTextOrND(BuildPartNumberText(data, useFastenerDetails));
                if (_sheetMaker != null) _sheetMaker.text = FormatTextOrND(data.manufacturer);
                if (_sheetReferences != null) _sheetReferences.text = FormatTextOrND(BuildReferencesText(data, useFastenerDetails));
                if (_sheetWeight != null) _sheetWeight.text = FormatWeight(data.weightKg);
                if (_sheetDimensions != null) _sheetDimensions.text = FormatTextOrND(BuildDimensionsText(data, useFastenerDetails));
                if (_sheetPower != null) _sheetPower.text = data.powerConsumption > 0 ? $"{data.powerConsumption:F1} W" : "N/A";
                if (_sheetTemp != null) _sheetTemp.text = data.operatingTemp > 0 ? $"{data.operatingTemp:F0}°C" : "N/A";

                if (_sheetDifficulty != null)
                {
                    int d = Mathf.Clamp(data.difficultyLevel, 0, 5);
                    _sheetDifficulty.text = new string('★', d) + new string('☆', 5 - d);
                }
                if (_sheetTools != null)
                {
                    _sheetTools.text = BuildToolsText(data, useFastenerDetails);
                }
                if (_sheetAssemblyTime != null)
                    _sheetAssemblyTime.text = data.installationTimeMinutes > 0 ? $"~{data.installationTimeMinutes:F0} min" : "N/A";
                if (_sheetTorque != null) _sheetTorque.text = FormatTextOrND(data.torqueSpec);
                if (_sheetConnections != null) _sheetConnections.text = FormatTextOrND(BuildConnectionsText(data, useFastenerDetails));
                if (_sheetSafety != null) _sheetSafety.text = FormatTextOrND(BuildSafetyText(data));
                if (_sheetServiceNote != null) _sheetServiceNote.text = FormatTextOrND(BuildServiceNoteText(data));

                // Hotspot single-click selection should not auto-open the details panel.

                // PRE-PEEK INFO BAR LOGIC: Show when part is selected
                ShowPeekForSelectedPart();
#endif
            }
            else
            {
                // Deselection — clear indicators but keep sheet open
                // (sheet closes only via FAB info button or mode change)
                CacheLastSelection(null, false, "", "", "", "", "");
                if (_partNameLabel != null) _partNameLabel.AddToClassList("selection-label--hidden");
                if (_infoBtn != null) _infoBtn.SetEnabled(false);

                // PRE-PEEK INFO BAR LOGIC: Hide when nothing is selected
                if (_infoBarPeek != null)
                {
                    _infoBarPeek.RemoveFromClassList("info-bar-peek--visible");
                    _infoBarPeek.AddToClassList("info-bar-peek--hidden");
                    _infoBarPeek.pickingMode = PickingMode.Ignore;
                    
                    // Drop bottom bar back down if the sheet isn't open
                    if (_bottomBar != null && !IsSheetOpen) _bottomBar.RemoveFromClassList("ui-shifted-soft");
                }
            }
        }

        private void CacheLastSelection(
            DronePartData data,
            bool fromHotspot,
            string hotspotGroupLabel,
            string hotspotGroupSummary,
            string hotspotGroupMembers,
            string selectionLabel,
            string canonicalPartName)
        {
            _lastData = data;
            _lastFromHotspot = fromHotspot;
            _lastHotspotGroupLabel = hotspotGroupLabel ?? string.Empty;
            _lastHotspotGroupSummary = hotspotGroupSummary ?? string.Empty;
            _lastHotspotGroupMembers = hotspotGroupMembers ?? string.Empty;
            _lastSelectionLabel = selectionLabel ?? string.Empty;
            _lastCanonicalPartName = canonicalPartName ?? string.Empty;
        }

        private void OnLanguageChanged(string languageCode)
        {
            AppLanguageManager.ApplyStaticText(_root);

            if (_lastData != null)
            {
                PopulatePartData(
                    _lastData,
                    _lastFromHotspot,
                    _lastHotspotGroupLabel,
                    _lastHotspotGroupSummary,
                    _lastHotspotGroupMembers,
                    _lastSelectionLabel,
                    _lastCanonicalPartName);
                return;
            }

            UpdatePartIndicator(null);
            if (_sheetTitle != null)
            {
                _sheetTitle.text = AppLanguageManager.SelectPartPrompt();
            }
        }

        private bool TryRenderCatalogData(
            DronePartData data,
            bool fromHotspot,
            string hotspotGroupLabel,
            string hotspotGroupSummary,
            string hotspotGroupMembers,
            string selectionLabel,
            string canonicalPartName)
        {
            if (!InfoPanelCatalogService.Instance.TryResolve(
                    data,
                    fromHotspot,
                    hotspotGroupLabel,
                    selectionLabel,
                    canonicalPartName,
                    out InfoPanelCatalogEntry entry))
            {
                return false;
            }

            InfoPanelLanguageBlock block = InfoPanelCatalogService.GetLanguageBlock(entry);
            if (block == null)
            {
                return false;
            }

            RenderInfoPanelBlock(block);
            ApplySelectionTitle(block.title);
            return true;
        }

        private void RenderFallbackData(
            DronePartData data,
            string hotspotGroupLabel,
            string hotspotGroupSummary,
            string hotspotGroupMembers,
            string selectionLabel,
            string canonicalPartName,
            bool useFastenerDetails)
        {
            string title = !string.IsNullOrWhiteSpace(hotspotGroupLabel)
                ? hotspotGroupLabel
                : BuildReadableTitle(GetDisplayName(selectionLabel, canonicalPartName, data));

            string summary = !string.IsNullOrWhiteSpace(hotspotGroupSummary)
                ? hotspotGroupSummary
                : BuildDescriptionText(data, useFastenerDetails);

            string relationshipTitle = AppLanguageManager.IsSpanish ? "Piezas contenidas" : "Contained Parts";
            string[] relationshipItems = BuildRelationshipItems(data, hotspotGroupMembers);
            if (data != null && data.HasFastenerMetadata)
            {
                relationshipTitle = AppLanguageManager.IsSpanish ? "Pieza madre" : "Parent Assembly";
                relationshipItems = new[] { FormatTextOrND(data.fastenerMetadata.parentCanonicalPartId) };
            }

            var block = new InfoPanelLanguageBlock
            {
                title = title,
                summary = summary,
                identification = new[]
                {
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Categoria" : "Category", value = !string.IsNullOrWhiteSpace(hotspotGroupLabel) ? "System Group" : BuildCategoryText(data, useFastenerDetails) },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Funcion" : "Function", value = !string.IsNullOrWhiteSpace(hotspotGroupSummary) ? hotspotGroupSummary : BuildFunctionText(data, useFastenerDetails) }
                },
                specifications = new[]
                {
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Material" : "Material", value = BuildMaterialText(data, useFastenerDetails) },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Peso" : "Weight", value = FormatWeight(data != null ? data.weightKg : 0f) },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Dimensiones" : "Dimensions", value = BuildDimensionsText(data, useFastenerDetails) },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Potencia" : "Power", value = data != null && data.powerConsumption > 0 ? $"{data.powerConsumption:F1} W" : PlaceholderText() },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Temperatura" : "Temperature", value = data != null && data.operatingTemp > 0 ? $"{data.operatingTemp:F0}\u00b0C" : PlaceholderText() }
                },
                relationship = new InfoPanelRelationship
                {
                    title = relationshipTitle,
                    items = relationshipItems
                },
                assembly = new[]
                {
                    BuildToolsText(data, useFastenerDetails),
                    BuildConnectionsText(data, useFastenerDetails),
                    BuildSafetyText(data),
                    BuildServiceNoteText(data)
                },
                keyReferences = new[]
                {
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "ID de ejecucion" : "Runtime ID", value = data != null ? data.id : string.Empty },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Clave / SKU" : "Part / SKU", value = BuildPartNumberText(data, useFastenerDetails) },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Fabricante" : "Maker", value = data != null ? data.manufacturer : string.Empty },
                    new InfoPanelField { label = AppLanguageManager.IsSpanish ? "Referencias" : "Source refs", value = BuildReferencesText(data, useFastenerDetails) }
                }
            };

            RenderInfoPanelBlock(block);
            ApplySelectionTitle(title);
        }

        private static string[] BuildRelationshipItems(DronePartData data, string hotspotGroupMembers)
        {
            if (!string.IsNullOrWhiteSpace(hotspotGroupMembers))
            {
                return SanitizeMemberList(hotspotGroupMembers).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (data == null || data.subComponentNames == null || data.subComponentNames.Length == 0)
            {
                return Array.Empty<string>();
            }

            var items = new List<string>();
            foreach (string item in data.subComponentNames)
            {
                string readable = SanitizeNaturalName(item);
                if (!string.IsNullOrWhiteSpace(readable))
                {
                    items.Add(readable);
                }
            }

            return items.ToArray();
        }

        private void RenderInfoPanelBlock(InfoPanelLanguageBlock block)
        {
            if (block == null)
            {
                return;
            }

            if (_sheetTitle != null)
            {
                _sheetTitle.text = FormatTextOrND(block.title);
            }

            if (_sheetDesc != null)
            {
                _sheetDesc.text = FormatTextOrND(block.summary);
            }

            RenderFieldRows(_foldoutIdentification, AppLanguageManager.IsSpanish ? "IDENTIFICACION" : "IDENTIFICATION", block.identification);
            RenderFieldRows(_foldoutSpecifications, AppLanguageManager.IsSpanish ? "ESPECIFICACIONES" : "SPECIFICATIONS", block.specifications);
            RenderRelationship(_foldoutRelationship, block.relationship);
            RenderTextRows(_foldoutAssembly, AppLanguageManager.IsSpanish ? "ENSAMBLAJE" : "ASSEMBLY", block.assembly);
            RenderFieldRows(_foldoutReferences, AppLanguageManager.IsSpanish ? "CLAVE Y REFERENCIAS" : "KEY & REFERENCES", block.keyReferences);
        }

        private void RenderFieldRows(Foldout foldout, string title, InfoPanelField[] fields)
        {
            if (foldout == null)
            {
                return;
            }

            foldout.text = title;
            foldout.contentContainer.Clear();

            if (fields == null || fields.Length == 0)
            {
                AddDataRow(foldout, PlaceholderText(), PlaceholderText());
                return;
            }

            foreach (InfoPanelField field in fields)
            {
                if (field == null)
                {
                    continue;
                }

                AddDataRow(foldout, FormatTextOrND(field.label), FormatTextOrND(field.value));
            }
        }

        private void RenderRelationship(Foldout foldout, InfoPanelRelationship relationship)
        {
            if (foldout == null)
            {
                return;
            }

            string title = relationship != null && !string.IsNullOrWhiteSpace(relationship.title)
                ? relationship.title.ToUpperInvariant()
                : (AppLanguageManager.IsSpanish ? "PIEZAS CONTENIDAS" : "CONTAINED PARTS");

            foldout.text = title;
            foldout.contentContainer.Clear();

            string[] items = relationship != null ? relationship.items : null;
            if (items == null || items.Length == 0)
            {
                AddTextRow(foldout, PlaceholderText());
                return;
            }

            foreach (string item in items)
            {
                AddTextRow(foldout, "- " + FormatTextOrND(item));
            }
        }

        private void RenderTextRows(Foldout foldout, string title, string[] lines)
        {
            if (foldout == null)
            {
                return;
            }

            foldout.text = title;
            foldout.contentContainer.Clear();

            if (lines == null || lines.Length == 0)
            {
                AddTextRow(foldout, PlaceholderText());
                return;
            }

            foreach (string line in lines)
            {
                AddTextRow(foldout, "- " + FormatTextOrND(line));
            }
        }

        private static void AddDataRow(Foldout foldout, string label, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("data-row");

            var labelElement = new Label(label);
            labelElement.AddToClassList("data-label");
            row.Add(labelElement);

            var valueElement = new Label(value);
            valueElement.AddToClassList("data-value");
            row.Add(valueElement);

            foldout.contentContainer.Add(row);
        }

        private static void AddTextRow(Foldout foldout, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("data-row");

            var valueElement = new Label(value);
            valueElement.AddToClassList("data-value");
            valueElement.AddToClassList("data-value-list");
            row.Add(valueElement);

            foldout.contentContainer.Add(row);
        }

        private void ApplySelectionTitle(string title)
        {
            string upperName = BuildReadableTitle(title).ToUpperInvariant();

            if (_partNameLabel != null)
            {
                _partNameLabel.text = upperName;
                _partNameLabel.style.color = new StyleColor(_isLightBg ? AccentLight : AccentDark);
                _partNameLabel.RemoveFromClassList("selection-label--hidden");
            }

            if (_topContextLabel != null)
            {
                _topContextLabel.text = upperName;
                _topContextLabel.style.color = new StyleColor(_isLightBg ? AccentLight : AccentDark);
            }
        }

        private void ShowPeekForSelectedPart()
        {
            if (_infoBarPeek == null || IsSheetOpen)
            {
                return;
            }

            _infoBarPeek.RemoveFromClassList("info-bar-peek--hidden");
            _infoBarPeek.AddToClassList("info-bar-peek--visible");
            _infoBarPeek.pickingMode = PickingMode.Position;

            if (_bottomBar != null)
            {
                _bottomBar.AddToClassList("ui-shifted-soft");
            }
        }

        public void UpdatePartIndicator(DronePartData data, string selectionLabel = "", string canonicalPartName = "")
        {
            if (data != null)
            {
            string upperName = BuildReadableTitle(GetDisplayName(selectionLabel, canonicalPartName, data)).ToUpperInvariant();

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
                    _topContextLabel.text = AppLanguageManager.SelectPartPrompt();
                    _topContextLabel.style.color = new StyleColor(_isLightBg ? MutedLight : MutedDark);
                }
            }
        }

        private static string GetDisplayName(string selectionLabel, string canonicalPartName, DronePartData data)
        {
            if (!string.IsNullOrWhiteSpace(selectionLabel))
            {
                return selectionLabel;
            }

            if (!string.IsNullOrWhiteSpace(canonicalPartName))
            {
                return canonicalPartName;
            }

            return data != null ? data.partName : string.Empty;
        }

        private static string BuildReadableTitle(string rawName)
        {
            string natural = SanitizeNaturalName(rawName);
            if (string.IsNullOrWhiteSpace(natural))
            {
                return string.Empty;
            }

            bool likelyCode = rawName != null && (
                rawName.Contains("_") ||
                rawName.Contains("-") ||
                rawName.Contains(".") ||
                Regex.IsMatch(rawName, @"\bx500v2\b", RegexOptions.IgnoreCase) ||
                rawName.ToUpperInvariant() == rawName);

            string title = likelyCode
                ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(natural.ToLowerInvariant())
                : natural;

            return RestoreAcronyms(title);
        }

        private static string BuildKeyNameText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            List<string> refs = new List<string>();
            if (!string.IsNullOrWhiteSpace(data.id))
            {
                refs.Add($"Unity ID: {data.id}");
            }

            if (useFastenerDetails && data.fastenerMetadata != null)
            {
                AddIfPresent(refs, "Instance", data.fastenerMetadata.instanceId);
                AddIfPresent(refs, "Family", data.fastenerMetadata.familyId);
            }

            return refs.Count > 0 ? string.Join(" | ", refs) : data.id;
        }

        private static string BuildPartNumberText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(data.partNumber))
            {
                return data.partNumber;
            }

            if (useFastenerDetails && data.fastenerMetadata != null)
            {
                if (!string.IsNullOrWhiteSpace(data.fastenerMetadata.sourceId))
                {
                    return data.fastenerMetadata.sourceId;
                }

                if (!string.IsNullOrWhiteSpace(data.fastenerMetadata.blenderName))
                {
                    return data.fastenerMetadata.blenderName;
                }
            }

            return "N/A";
        }

        private static string BuildReferencesText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            List<string> refs = new List<string>
            {
                "Holybro X500 V2 documentation",
                "X500 V2 assembly guide",
            };

            if (!string.IsNullOrWhiteSpace(data.id))
            {
                refs.Add($"Runtime key: {data.id}");
            }

            if (useFastenerDetails && data.fastenerMetadata != null)
            {
                AddIfPresent(refs, "CAD key", data.fastenerMetadata.blenderName);
                AddIfPresent(refs, "Type key", data.fastenerMetadata.sceneTypeKey);
                AddIfPresent(refs, "Parent", data.fastenerMetadata.parentCanonicalPartId);
                AddIfPresent(refs, "Scene", data.fastenerMetadata.sceneObjectName);
            }
            else
            {
                AddIfPresent(refs, "CAD key", DeriveSourceKeyFromId(data.id));
                if (data.subComponentNames != null && data.subComponentNames.Length > 0)
                {
                    refs.Add($"Subcomponents: {data.subComponentNames.Length}");
                }
            }

            return string.Join(" | ", refs);
        }

        private static string BuildConnectionsText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            if (data.connectionTypes != null && data.connectionTypes.Length > 0)
            {
                parts.Add(string.Join(", ", data.connectionTypes));
            }

            if (data.screwCount > 0)
            {
                string screw = string.IsNullOrWhiteSpace(data.screwSize)
                    ? $"{data.screwCount} screws"
                    : $"{data.screwCount}x {data.screwSize}";
                parts.Add(screw);
            }

            if (useFastenerDetails && data.fastenerMetadata != null)
            {
                string drive = data.fastenerMetadata.driveType;
                if (!string.IsNullOrWhiteSpace(drive) && !drive.Equals("N/A", System.StringComparison.OrdinalIgnoreCase))
                {
                    parts.Add($"Drive: {drive}");
                }
            }

            return parts.Count > 0 ? string.Join(" | ", parts) : string.Empty;
        }

        private static string BuildSafetyText(DronePartData data)
        {
            if (data == null || data.safetyWarnings == null || data.safetyWarnings.Length == 0)
            {
                return string.Empty;
            }

            return string.Join("; ", data.safetyWarnings);
        }

        private static string BuildServiceNoteText(DronePartData data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            List<string> notes = new List<string>();
            if (!string.IsNullOrWhiteSpace(data.installationTips))
            {
                notes.Add(data.installationTips);
            }

            if (data.prerequisites != null && data.prerequisites.Length > 0)
            {
                notes.Add("Prerequisites: " + string.Join(", ", data.prerequisites));
            }

            return notes.Count > 0 ? string.Join(" | ", notes) : string.Empty;
        }

        private static void AddIfPresent(List<string> target, string label, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                target.Add($"{label}: {value}");
            }
        }

        private static string DeriveSourceKeyFromId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return string.Empty;
            }

            string value = id.Trim();
            value = Regex.Replace(value, @"^x500v2_blend_", string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, @"^x500v2_fastener_", string.Empty, RegexOptions.IgnoreCase);
            if (value == id)
            {
                return string.Empty;
            }

            value = value.Replace("_", "-").ToUpperInvariant();
            value = value
                .Replace("BOTTOM-PLATE-X500-V5", "BOTTOM-PLATE-X500-V5")
                .Replace("TOP-PLATE-X500-V5", "TOP-PLATE-X500-V5")
                .Replace("BATTERY-MOUNTING-PLAT", "BATTERY-MOUNTING-PLAT")
                .Replace("PLATFORM-PLAT-X500", "PLATFORM-PLAT-X500");
            return value;
        }

        private static string BuildCategoryText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return data.category.ToString();
            }

            string subtype = FastenerNamingUtility.ToTitleCase(data.fastenerMetadata.subtype);
            string spec = data.fastenerMetadata.GetTechnicalSummary();

            if (string.IsNullOrWhiteSpace(subtype))
            {
                return string.IsNullOrWhiteSpace(spec) ? data.category.ToString() : $"{data.category} | {spec}";
            }

            return string.IsNullOrWhiteSpace(spec)
                ? $"{data.category} | {subtype}"
                : $"{data.category} | {subtype} | {spec}";
        }

        private static string BuildFunctionText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return SanitizeTechnicalText(data.function);
            }

            FastenerMetadata metadata = data.fastenerMetadata;
            string detail = metadata.GetTechnicalSummary();
            return string.IsNullOrWhiteSpace(detail)
                ? "Fastens, spaces, or secures the selected assembly."
                : $"Fastens, spaces, or secures the selected assembly ({detail}).";
        }

        private static string BuildMaterialText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return SanitizeTechnicalText(data.materialType);
            }

            return SanitizeTechnicalText(data.materialType);
        }

        private static string BuildDescriptionText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return AppendSubcomponentSummary(SanitizeTechnicalText(data.description), data.subComponentNames);
            }

            FastenerMetadata metadata = data.fastenerMetadata;
            string description = SanitizeTechnicalText(data.description);

            if (!string.IsNullOrWhiteSpace(metadata.notes))
            {
                string sanitizedNotes = SanitizeTechnicalText(metadata.notes);
                description = string.IsNullOrWhiteSpace(description)
                    ? sanitizedNotes
                    : $"{description}\n\nNotes: {sanitizedNotes}";
            }

            return description;
        }

        private static string AppendSubcomponentSummary(string description, string[] subComponentNames)
        {
            if (subComponentNames == null || subComponentNames.Length == 0)
            {
                return description;
            }

            List<string> readableNames = new List<string>();
            foreach (string item in subComponentNames)
            {
                string readable = SanitizeNaturalName(item);
                if (!string.IsNullOrWhiteSpace(readable))
                {
                    readableNames.Add(readable);
                }
            }

            string assemblySummary = readableNames.Count > 0
                ? "Assembly includes:\n- " + string.Join("\n- ", readableNames)
                : string.Empty;

            return string.IsNullOrWhiteSpace(description)
                ? assemblySummary
                : $"{description}\n\n{assemblySummary}";
        }

        private static string BuildDimensionsText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return SanitizeTechnicalText(data.dimensions);
            }

            FastenerMetadata metadata = data.fastenerMetadata;
            List<string> parts = new List<string>();

            string spec = FastenerNamingUtility.FormatMetricAndLength(metadata.metric, metadata.lengthMm);
            if (!string.IsNullOrWhiteSpace(spec))
            {
                parts.Add(spec);
            }

            if (metadata.nominalDiameterMm > 0.0001f)
            {
                parts.Add($"Diameter {metadata.nominalDiameterMm:0.##} mm");
            }

            return parts.Count > 0 ? string.Join(" | ", parts) : SanitizeTechnicalText(data.dimensions);
        }

        private static string BuildToolsText(DronePartData data, bool useFastenerDetails)
        {
            if (data == null)
            {
                return "None";
            }

            if (data.requiredTools != null && data.requiredTools.Length > 0)
            {
                return string.Join(", ", data.requiredTools);
            }

            if (!useFastenerDetails || data.fastenerMetadata == null)
            {
                return "None";
            }

            return string.IsNullOrWhiteSpace(data.fastenerMetadata.driveType) || data.fastenerMetadata.driveType == "N/A"
                ? "None"
                : data.fastenerMetadata.driveType;
        }

        private static string SanitizeMemberList(string members)
        {
            if (string.IsNullOrWhiteSpace(members))
            {
                return string.Empty;
            }

            string[] tokens = members.Split(new[] { ',', ';', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> cleaned = new List<string>();
            foreach (string token in tokens)
            {
                string readable = SanitizeNaturalName(token);
                if (!string.IsNullOrWhiteSpace(readable))
                {
                    cleaned.Add(readable);
                }
            }

            return cleaned.Count > 0 ? string.Join(", ", cleaned) : members;
        }

        private static string SanitizeTechnicalText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string text = value.Trim();
            text = Regex.Replace(text, @"\s*Pieza modelada en base al CAD\s+[A-Z0-9_.\-]+\.?", ".", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*Part modeled based on CAD\s+[A-Z0-9_.\-]+\.?", ".", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*Modeled from CAD\s+[A-Z0-9_.\-]+\.?", ".", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*CAD:\s*[A-Z0-9_.\-]+\.?", ".", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*Scene instance:\s*[A-Z0-9_.\-]+\.?", ".", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*Synced source expects [^.]+\.?", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s+", " ");
            text = text.Replace("..", ".").Trim();

            return RestoreAcronyms(text);
        }

        private static string SanitizeNaturalName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string text = value.Trim();
            string lookup = text.ToUpperInvariant();
            string mapped = MapKnownSourceName(lookup);
            if (!string.IsNullOrWhiteSpace(mapped))
            {
                return mapped;
            }

            text = Regex.Replace(text, @"\.00\d\b", string.Empty);
            text = Regex.Replace(text, @"_low\b", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bx500v2\b", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bblend\b", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bmisc\b", "auxiliary", RegexOptions.IgnoreCase);
            text = text.Replace("_", " ").Replace("-", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            bool likelyCode = value.Contains("_") || value.Contains("-") || value.ToUpperInvariant() == value;
            if (likelyCode)
            {
                text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
            }

            return RestoreAcronyms(text);
        }

        private static string MapKnownSourceName(string lookup)
        {
            if (lookup.Contains("GAN-GPSV5-ZHIJIA")) return "GPS Antenna";
            if (lookup.Contains("GPS-ZHIJIA-ZHUANJIETOU")) return "GPS Folding Mast Joint";
            if (lookup.Contains("GPS-ZHIJIA-ZUO")) return "GPS Mast Base Bracket";
            if (lookup.Contains("GPSV5-ZHIJIA-LUOMAO")) return "GPS Mast Securing Nut";
            if (lookup.Contains("GPSV5-ZHIJIA-TUOPAN")) return "GPS Top Mounting Tray";
            if (lookup.Contains("PLATFORM-PLAT-X500")) return "Upper Platform Board";
            if (lookup.Contains("ZHIJIA-CAMERA-INTEL")) return "Depth Camera Bracket";
            if (lookup.Contains("GAI-GUANGLIU")) return "Optical Flow Bottom Cover";
            if (lookup.Contains("JIA-GUAN")) return "Cable Routing Clamp";
            if (lookup.Contains("GUAN-CHENG")) return "Carbon Fiber Tube";
            if (lookup.Contains("JIAO-EVA")) return "Landing Gear Foam Pad";
            if (lookup.Contains("JIAO-LIANJIE")) return "Landing Skid End Cap";
            if (lookup.Contains("JIA-LIANJIE")) return "Landing Gear Upper Connector";
            if (lookup.Contains("MAO-JIAO")) return "Landing Gear T-Connector";
            if (lookup.Contains("BAN-DJ-DIAN-F2")) return "Motor Mount Base Plate";
            if (lookup.Contains("HMX5V-DIGAI-DIANJIZUO-MUJU")) return "Lower Motor Mount Cover";
            if (lookup.Contains("HMX5V-ZUO-DJ-MUJU")) return "Upper Motor Mount Hub";
            if (lookup.Contains("HMX5V-JIBI-JIA-MUJU")) return "Arm Frame Clamp Connector";
            if (lookup.Contains("HMX5V-GUAN-DINGWEI")) return "Tube Positioning Stopper";
            if (lookup.Contains("DJ-2216")) return "Holybro 2216 KV920 Motor";
            if (lookup.Contains("PCB-PM06")) return "Power Module Board";
            if (lookup.Contains("PCB-PIXHAWK6C-F1")) return "Pixhawk 6C Main PCB";
            if (lookup.Contains("DIKE-PIXHAWK6C-LV-C1")) return "Pixhawk 6C Top Cover";
            if (lookup.Contains("MIANKE-PIXHAWK6C-LV-C1")) return "Pixhawk 6C Base Shell";
            if (lookup.Contains("BM06B-WO")) return "JST GH 6-Pin Connector";
            if (lookup.Contains("TOU-XT60H-M-14AWG")) return "XT60 Male Connector Plug";
            if (lookup.Contains("X500-TAO-XT60")) return "XT60 Panel Holder";
            if (lookup.Contains("PYLONS-X500")) return "Payload Carbon Rails";
            if (lookup.Contains("BOTTOM-PLATE-X500")) return "Carbon Fiber Bottom Plate";
            if (lookup.Contains("TOP-PLATE-X500")) return "Carbon Fiber Top Plate";
            if (lookup.Contains("BATTERY-MOUNTING-PLAT")) return "Battery Mounting Board";
            if (lookup.Contains("BATTERY-PAD")) return "Battery Silicone Pad";
            if (lookup.Contains("HUAN-GUIJIAO")) return "Silicone Vibration Dampener";
            if (lookup.Contains("NILONGZHU-M25-5")) return "Nylon Standoff M2.5 x 5 mm";
            if (lookup.Contains("NILONGZHU-M3-5")) return "Nylon Standoff M3 x 5 mm";
            if (lookup.Contains("GB70-M25-6")) return "Socket Cap Screw M2.5 x 6 mm";
            if (lookup.Contains("GB70-M25-10")) return "Socket Cap Screw M2.5 x 10 mm";
            if (lookup.Contains("GB70-M25-12")) return "Socket Cap Screw M2.5 x 12 mm";
            if (lookup.Contains("GB70-M3-6")) return "Socket Cap Screw M3 x 6 mm";
            if (lookup.Contains("GB70-M3-8")) return "Socket Cap Screw M3 x 8 mm";
            if (lookup.Contains("GB70-M3-21")) return "Socket Cap Screw M3 x 21 mm";
            if (lookup.Contains("GB70-M3-25")) return "Socket Cap Screw M3 x 25 mm";
            if (lookup.Contains("GB70-M3-38")) return "Socket Cap Screw M3 x 38 mm";
            if (lookup.Contains("M3-10-PAN")) return "Pan Head Screw M3 x 10 mm";
            if (lookup.Contains("M3-14-PAN")) return "Pan Head Screw M3 x 14 mm";
            if (lookup.Contains("M3-16-CHEN-LIU")) return "Countersunk Screw M3 x 16 mm";
            if (lookup.Contains("M25-6-CHEN-LIU")) return "Countersunk Screw M2.5 x 6 mm";
            if (lookup.Contains("LM-M3-NILONG")) return "Nyloc Nut M3";
            if (lookup.Contains("LM-M3-DING")) return "Cap Nut M3";
            if (lookup.Contains("ZSLM-M25")) return "Self-Locking Flange Nut M2.5";
            if (lookup.Contains("ZSLM-M3")) return "Self-Locking Flange Nut M3";
            if (lookup.Contains("MISC-CAMERA-MOUNT")) return "Depth Camera Mount";
            if (lookup.Contains("MISC-FRAME-CONNECTOR")) return "Auxiliary Frame Connector";
            if (lookup.Contains("MISC-LIGHT-COVER")) return "Auxiliary Light Cover";
            if (lookup.Contains("FASTENER-GROUP")) return "Fastener Set";
            return string.Empty;
        }

        private static string RestoreAcronyms(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return value
                .Replace("Lipo", "LiPo")
                .Replace("Gps", "GPS")
                .Replace("Gnss", "GNSS")
                .Replace("Pdb", "PDB")
                .Replace("Esc", "ESC")
                .Replace("Rc ", "RC ")
                .Replace("Xt60", "XT60")
                .Replace("Xt30", "XT30")
                .Replace("Pwm", "PWM")
                .Replace("Dshot", "DShot")
                .Replace("Mavlink", "MAVLink")
                .Replace("Pixhawk", "Pixhawk")
                .Replace("Jst", "JST")
                .Replace("Gh ", "GH ")
                .Replace("Kv", "KV")
                .Replace("Usb", "USB");
        }

        private static string FormatWeight(float weightKg)
        {
            if (weightKg <= 0f)
            {
                return AppLanguageManager.IsSpanish ? "No aplica" : "N/A";
            }

            if (weightKg < 0.01f)
            {
                return $"{weightKg * 1000f:F1} g";
            }

            return $"{weightKg:F2} kg";
        }

        private static string FormatTextOrND(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return AppLanguageManager.IsSpanish ? "No aplica" : "N/A";
            }

            string normalized = value.Trim();
            if (normalized == "-" || normalized.Equals("N/A", System.StringComparison.OrdinalIgnoreCase))
            {
                return AppLanguageManager.IsSpanish ? "No aplica" : "N/A";
            }

            return normalized;
        }

        // ═══════════════════════════════════════════════════════
        //  Private — interaction bindings
        // ═══════════════════════════════════════════════════════

        private static string PlaceholderText()
        {
            return AppLanguageManager.IsSpanish ? "No aplica" : "N/A";
        }

        private void BindInteractions()
        {
            RegisterTransientInputBlock(_detailsSheet);
            RegisterSheetTopZoneSwipeClose(_detailsSheet);

            // Block 3D input only while the user is actively dragging the sheet scroll.
            var sheetScroll = _root.Q<ScrollView>(className: "sheet-scroll");
            if (sheetScroll != null)
            {
                RegisterTransientInputBlock(sheetScroll);
            }

            // Close button
            if (_sheetCloseBtn != null)
            {
                EventCallback<PointerDownEvent> closePd = evt => evt.StopPropagation();
                _sheetCloseBtn.RegisterCallback(closePd);
                AddCleanup(() => _sheetCloseBtn.UnregisterCallback(closePd));
            }

            if (_contentDetails != null)
            {
                RegisterTransientInputBlock(_contentDetails);
            }

            if (_sheetHandle != null)
            {
                _sheetHandle.pickingMode = PickingMode.Position;
                RegisterSheetDragClose(_sheetHandle);
            }

            if (_sheetHeader != null)
            {
                RegisterSheetDragClose(_sheetHeader);
            }

            // Info button
            if (_infoBtn != null)
            {
                System.Action onInfoClick = ToggleInfo;
                _infoBtn.clicked += onInfoClick;
                AddCleanup(() => _infoBtn.clicked -= onInfoClick);
                RegisterTransientInputBlock(_infoBtn);
            }

            // Swipe-up on actions-row pill to open sheet (issue #7 — gesture support)
            // Note: BottomBar has picking-mode="Ignore", so we use the actions-row which IS clickable
            var actionsRow = _root.Q<VisualElement>(className: "actions-row");
            if (actionsRow != null)
            {
                actionsRow.pickingMode = PickingMode.Position;

                EventCallback<PointerDownEvent> swipeDown = evt =>
                {
                    if (!IsSheetOpen && IsPrimaryPointerButton(evt.button))
                    {
                        _swipeStartY = evt.position.y;
                        _swipeStartX = evt.position.x;
                        _isSwipingUp = true;
                        _sheetOpenSwipeTriggered = false;
                        InputManager.InputBlocked = true;
                        actionsRow.CapturePointer(evt.pointerId);
                        evt.StopPropagation();
                    }
                };
                EventCallback<PointerMoveEvent> swipeMove = evt =>
                {
                    if (!_isSwipingUp)
                    {
                        return;
                    }

                    evt.StopPropagation();

                    float deltaY = _swipeStartY - evt.position.y;
                    float deltaX = Mathf.Abs(evt.position.x - _swipeStartX);
                    bool isUpSwipe = deltaY > SheetSwipeThreshold
                        && deltaY > deltaX * SheetSwipeDominance;

                    if (_isSwipingUp && !_sheetOpenSwipeTriggered && isUpSwipe)
                    {
                        // Swipe upward > 50px threshold → open sheet
                        if (SelectionManager.Instance?.HasSelection == true)
                        {
                            _sheetOpenSwipeTriggered = true;
                            OpenSheet();
                            InputManager.InputBlocked = true;
                        }
                    }
                };
                EventCallback<PointerUpEvent> swipeUp = evt =>
                {
                    if (_isSwipingUp)
                    {
                        evt.StopPropagation();
                    }

                    _isSwipingUp = false;
                    _sheetOpenSwipeTriggered = false;
                    InputManager.InputBlocked = false;
                    actionsRow.ReleasePointer(evt.pointerId);
                };
                EventCallback<PointerLeaveEvent> swipeLeave = evt =>
                {
                    if (_isSwipingUp)
                    {
                        evt.StopPropagation();
                        if (IsPrimaryPointerStillDown())
                        {
                            return;
                        }
                    }

                    _isSwipingUp = false;
                    _sheetOpenSwipeTriggered = false;
                    InputManager.InputBlocked = false;
                    actionsRow.ReleasePointer(evt.pointerId);
                };
                actionsRow.RegisterCallback(swipeDown);
                actionsRow.RegisterCallback(swipeMove);
                actionsRow.RegisterCallback(swipeUp);
                actionsRow.RegisterCallback(swipeLeave);
                AddCleanup(() =>
                {
                    actionsRow.UnregisterCallback(swipeDown);
                    actionsRow.UnregisterCallback(swipeMove);
                    actionsRow.UnregisterCallback(swipeUp);
                    actionsRow.UnregisterCallback(swipeLeave);
                });
            }
        }

        private void RegisterSheetDragClose(VisualElement element)
        {
            if (element == null) return;

            EventCallback<PointerDownEvent> swipeDown = evt =>
            {
                if (!IsSheetOpen || !IsPrimaryPointerButton(evt.button))
                {
                    return;
                }

                _swipeStartY = evt.position.y;
                _swipeStartX = evt.position.x;
                _isSwipingDown = true;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = true;
                element.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            };

            EventCallback<PointerMoveEvent> swipeMove = evt =>
            {
                if (!_isSwipingDown)
                {
                    return;
                }

                evt.StopPropagation();

                float deltaY = evt.position.y - _swipeStartY;
                float deltaX = Mathf.Abs(evt.position.x - _swipeStartX);
                bool isDownSwipe = deltaY > SheetSwipeThreshold
                    && deltaY > deltaX * SheetSwipeDominance;

                if (!_sheetDragTriggeredClose && isDownSwipe)
                {
                    _sheetDragTriggeredClose = true;
                    SetSheetState(false);
                    InputManager.InputBlocked = true;
                }
            };

            EventCallback<PointerUpEvent> swipeUp = evt =>
            {
                if (_isSwipingDown)
                {
                    evt.StopPropagation();
                }

                _isSwipingDown = false;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = false;
                element.ReleasePointer(evt.pointerId);
            };

            EventCallback<PointerLeaveEvent> swipeLeave = evt =>
            {
                if (_isSwipingDown)
                {
                    evt.StopPropagation();
                    if (IsPrimaryPointerStillDown())
                    {
                        return;
                    }
                }

                _isSwipingDown = false;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = false;
                element.ReleasePointer(evt.pointerId);
            };

            element.RegisterCallback(swipeDown);
            element.RegisterCallback(swipeMove);
            element.RegisterCallback(swipeUp);
            element.RegisterCallback(swipeLeave);
            AddCleanup(() =>
            {
                element.UnregisterCallback(swipeDown);
                element.UnregisterCallback(swipeMove);
                element.UnregisterCallback(swipeUp);
                element.UnregisterCallback(swipeLeave);
            });
        }

        private void RegisterSheetTopZoneSwipeClose(VisualElement element)
        {
            if (element == null) return;

            EventCallback<PointerDownEvent> swipeDown = evt =>
            {
                if (!IsSheetOpen || !IsPrimaryPointerButton(evt.button) || !IsPointInSheetDragZone(evt.position))
                {
                    return;
                }

                _swipeStartY = evt.position.y;
                _swipeStartX = evt.position.x;
                _isSwipingDown = true;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = true;
                element.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            };

            EventCallback<PointerMoveEvent> swipeMove = evt =>
            {
                if (!_isSwipingDown)
                {
                    return;
                }

                evt.StopPropagation();

                float deltaY = evt.position.y - _swipeStartY;
                float deltaX = Mathf.Abs(evt.position.x - _swipeStartX);
                bool isDownSwipe = deltaY > SheetSwipeThreshold
                    && deltaY > deltaX * SheetSwipeDominance;

                if (!_sheetDragTriggeredClose && isDownSwipe)
                {
                    _sheetDragTriggeredClose = true;
                    SetSheetState(false);
                    InputManager.InputBlocked = true;
                }
            };

            EventCallback<PointerUpEvent> swipeUp = evt =>
            {
                if (_isSwipingDown)
                {
                    evt.StopPropagation();
                }

                _isSwipingDown = false;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = false;
                element.ReleasePointer(evt.pointerId);
            };

            EventCallback<PointerLeaveEvent> swipeLeave = evt =>
            {
                if (_isSwipingDown)
                {
                    evt.StopPropagation();
                    if (IsPrimaryPointerStillDown())
                    {
                        return;
                    }
                }

                _isSwipingDown = false;
                _sheetDragTriggeredClose = false;
                InputManager.InputBlocked = false;
                element.ReleasePointer(evt.pointerId);
            };

            element.RegisterCallback(swipeDown, TrickleDown.TrickleDown);
            element.RegisterCallback(swipeMove, TrickleDown.TrickleDown);
            element.RegisterCallback(swipeUp, TrickleDown.TrickleDown);
            element.RegisterCallback(swipeLeave, TrickleDown.TrickleDown);
            AddCleanup(() =>
            {
                element.UnregisterCallback(swipeDown, TrickleDown.TrickleDown);
                element.UnregisterCallback(swipeMove, TrickleDown.TrickleDown);
                element.UnregisterCallback(swipeUp, TrickleDown.TrickleDown);
                element.UnregisterCallback(swipeLeave, TrickleDown.TrickleDown);
            });
        }

        private bool IsPointInSheetDragZone(Vector2 panelPos)
        {
            if (IsPointInSheetHandleZone(panelPos))
            {
                return true;
            }

            if (_detailsSheet == null || !_detailsSheet.worldBound.Contains(panelPos))
            {
                return false;
            }

            if (_sheetCloseBtn != null && _sheetCloseBtn.worldBound.Contains(panelPos))
            {
                return false;
            }

            if (_sheetHandle != null && _sheetHandle.worldBound.Contains(panelPos))
            {
                return true;
            }

            if (_sheetHeader != null && _sheetHeader.worldBound.Contains(panelPos))
            {
                return true;
            }

            if (_contentDetails != null)
            {
                return panelPos.y < _contentDetails.worldBound.yMin;
            }

            Rect sheetBounds = _detailsSheet.worldBound;
            return panelPos.y <= sheetBounds.yMin + 140f;
        }

        private bool IsPointInSheetHandleZone(Vector2 panelPos)
        {
            if (_sheetHandle == null)
            {
                return false;
            }

            Rect handleBounds = _sheetHandle.worldBound;
            if (handleBounds.width <= 0f || handleBounds.height <= 0f)
            {
                return false;
            }

            handleBounds.xMin -= 36f;
            handleBounds.xMax += 36f;
            handleBounds.yMin -= 24f;
            handleBounds.yMax += 24f;
            return handleBounds.Contains(panelPos);
        }

        private void RegisterTransientInputBlock(VisualElement element)
        {
            if (element == null) return;

            EventCallback<PointerDownEvent> pointerDown = evt =>
            {
                if (IsPrimaryPointerButton(evt.button))
                {
                    InputManager.InputBlocked = true;
                }
            };

            EventCallback<PointerUpEvent> pointerUp = evt =>
            {
                if (IsPrimaryPointerButton(evt.button))
                {
                    InputManager.InputBlocked = false;
                }
            };

            EventCallback<PointerMoveEvent> pointerMove = evt =>
            {
                // Keep the global camera block alive during active panel/content
                // drags without stealing events from foldouts, scroll views or text.
                _ = InputManager.InputBlocked;
            };

            EventCallback<PointerLeaveEvent> pointerLeave = evt =>
            {
                if (!HasActiveSheetGesture && !IsPrimaryPointerStillDown())
                {
                    InputManager.InputBlocked = false;
                }
            };

            element.RegisterCallback(pointerDown);
            element.RegisterCallback(pointerMove);
            element.RegisterCallback(pointerUp);
            element.RegisterCallback(pointerLeave);
            AddCleanup(() =>
            {
                element.UnregisterCallback(pointerDown);
                element.UnregisterCallback(pointerMove);
                element.UnregisterCallback(pointerUp);
                element.UnregisterCallback(pointerLeave);
            });
        }

        private static bool IsPrimaryPointerButton(int button)
        {
            return button == 0 || button < 0;
        }

        private static bool IsPrimaryPointerStillDown()
        {
            return Input.GetMouseButton(0) || Input.touchCount > 0;
        }

        private static void SetDescendantsPickingMode(VisualElement root, PickingMode mode)
        {
            if (root == null) return;

            for (int i = 0; i < root.childCount; i++)
            {
                VisualElement child = root[i];
                child.pickingMode = mode;
                SetDescendantsPickingMode(child, mode);
            }
        }
    }
}
