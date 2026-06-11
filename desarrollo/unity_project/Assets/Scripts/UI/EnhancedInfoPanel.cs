using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Data;
using WebGL.Core.Content;
using WebGL.Core.Utils;
using WebGL.Core.Events;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebGL.UI
{
    public class EnhancedInfoPanel : Singleton<EnhancedInfoPanel>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement infoPanel;
        private VisualElement contentContainer;
        
        // Header elements
        private Label titleLabel;
        private Label subtitleLabel;
        private VisualElement iconContainer;

        // Tabs
        private TabView tabView;
        private VisualElement overviewTab;
        private VisualElement materialsTab;
        private VisualElement specsTab;

        // Data labels
        private Label descriptionLabel;
        private Label materialTypeLabel;
        private Label materialPropsLabel;
        private Label weightLabel;
        private Label dimensionsLabel;
        private Label functionLabel;
        private Label keyNameLabel;
        private Label partNumberLabel;
        private Label manufacturerLabel;
        private Label referencesLabel;

        // Action buttons
        private Button hideButton;
        private Button isolateButton;
        private Button focusButton;
        private Button closeButton;

        private ExplodablePart currentPart;
        // private bool isVisible = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateInfoPanelUI();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void CreateInfoPanelUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main panel
            infoPanel = new VisualElement();
            infoPanel.name = "EnhancedInfoPanel";
            infoPanel.style.position = Position.Absolute;
            infoPanel.style.right = 20;
            infoPanel.style.top = 20;
            infoPanel.style.width = 350;
            infoPanel.style.maxHeight = Length.Percent(80);
            infoPanel.style.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            infoPanel.style.borderTopLeftRadius = 12; infoPanel.style.borderTopRightRadius = 12; infoPanel.style.borderBottomLeftRadius = 12; infoPanel.style.borderBottomRightRadius = 12;
            infoPanel.style.display = DisplayStyle.None;
            infoPanel.style.opacity = 0;

            // Close button (top right)
            closeButton = new Button(Hide);
            closeButton.text = "✕";
            closeButton.style.position = Position.Absolute;
            closeButton.style.right = 10;
            closeButton.style.top = 10;
            closeButton.style.width = 30;
            closeButton.style.height = 30;
            closeButton.style.backgroundColor = Color.clear;
            closeButton.style.borderTopWidth = 0; closeButton.style.borderBottomWidth = 0; closeButton.style.borderLeftWidth = 0; closeButton.style.borderRightWidth = 0;
            closeButton.style.color = new Color(1, 1, 1, 0.5f);
            closeButton.style.fontSize = 18;
            infoPanel.Add(closeButton);

            // Header
            var header = new VisualElement();
            header.style.paddingTop = 20;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 20;
            header.style.paddingBottom = 15;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(1, 1, 1, 0.1f);

            titleLabel = new Label("Part Name");
            titleLabel.style.fontSize = 22;
            titleLabel.style.color = Color.white;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.Add(titleLabel);

            subtitleLabel = new Label("Category");
            subtitleLabel.style.fontSize = 12;
            subtitleLabel.style.color = new Color(0.6f, 0.8f, 1f, 0.8f);
            subtitleLabel.style.marginTop = 5;
            header.Add(subtitleLabel);

            infoPanel.Add(header);

            // Content container with scroll
            contentContainer = new ScrollView();
            contentContainer.style.flexGrow = 1;
            contentContainer.style.paddingLeft = 20;
            contentContainer.style.paddingRight = 20;
            contentContainer.style.paddingTop = 15;
            contentContainer.style.paddingBottom = 15;

            // Overview section
            CreateSection(contentContainer, "Overview", out overviewTab);
            descriptionLabel = CreateInfoRow(overviewTab, "Description", "No description");
            weightLabel = CreateInfoRow(overviewTab, "Weight", "-");
            dimensionsLabel = CreateInfoRow(overviewTab, "Dimensions", "-");
            functionLabel = CreateInfoRow(overviewTab, "Function", "-");

            CreateSection(contentContainer, "Key & References", out specsTab);
            keyNameLabel = CreateInfoRow(specsTab, "Key name", "-");
            partNumberLabel = CreateInfoRow(specsTab, "Part / SKU", "-");
            manufacturerLabel = CreateInfoRow(specsTab, "Maker", "-");
            referencesLabel = CreateInfoRow(specsTab, "Source refs", "-");

            // Materials section
            CreateSection(contentContainer, "Materials", out materialsTab);
            materialTypeLabel = CreateInfoRow(materialsTab, "Type", "-");
            materialPropsLabel = CreateInfoRow(materialsTab, "Properties", "-");

            infoPanel.Add(contentContainer);

            // Action buttons
            var actionsContainer = new VisualElement();
            actionsContainer.style.flexDirection = FlexDirection.Row;
            actionsContainer.style.paddingLeft = 15;
            actionsContainer.style.paddingRight = 15;
            actionsContainer.style.paddingTop = 10;
            actionsContainer.style.paddingBottom = 15;
            actionsContainer.style.justifyContent = Justify.SpaceAround;
            actionsContainer.style.borderTopWidth = 1;
            actionsContainer.style.borderTopColor = new Color(1, 1, 1, 0.1f);

            hideButton = CreateActionButton("Hide", OnHideClicked);
            isolateButton = CreateActionButton("Isolate", OnIsolateClicked);
            focusButton = CreateActionButton("Focus", OnFocusClicked);

            actionsContainer.Add(hideButton);
            actionsContainer.Add(isolateButton);
            actionsContainer.Add(focusButton);

            infoPanel.Add(actionsContainer);

            root.Add(infoPanel);
        }

        private void CreateSection(VisualElement parent, string title, out VisualElement container)
        {
            var section = new VisualElement();
            section.style.marginBottom = 15;

            var header = new Label(title);
            header.style.fontSize = 14;
            header.style.color = new Color(0.6f, 0.8f, 1f);
            header.style.marginBottom = 10;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            section.Add(header);

            container = new VisualElement();
            section.Add(container);
            parent.Add(section);
        }

        private Label CreateInfoRow(VisualElement parent, string label, string value)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.marginBottom = 8;

            var labelElement = new Label(label);
            labelElement.style.color = new Color(1, 1, 1, 0.5f);
            labelElement.style.fontSize = 12;

            var valueLabel = new Label(value);
            valueLabel.style.color = Color.white;
            valueLabel.style.fontSize = 12;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            valueLabel.style.flexGrow = 1;
            valueLabel.style.marginLeft = 10;

            row.Add(labelElement);
            row.Add(valueLabel);
            parent.Add(row);

            return valueLabel;
        }

        private Button CreateActionButton(string text, System.Action onClick)
        {
            var button = new Button(onClick);
            button.text = text;
            button.style.paddingTop = 8;
            button.style.paddingBottom = 8;
            button.style.paddingLeft = 15;
            button.style.paddingRight = 15;
            button.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            button.style.borderTopWidth = 0; button.style.borderBottomWidth = 0; button.style.borderLeftWidth = 0; button.style.borderRightWidth = 0;
            button.style.borderTopLeftRadius = 6; button.style.borderTopRightRadius = 6; button.style.borderBottomLeftRadius = 6; button.style.borderBottomRightRadius = 6;
            button.style.color = Color.white;
            button.style.fontSize = 12;
            return button;
        }

        private void OnPartSelected(PartSelectedEvent evt)
        {
            if (evt.PartData != null)
            {
                var part = FindPartByData(evt.PartData);
                ShowPartInfo(part, evt.PartData, evt.SelectionLabel, evt.CanonicalPartName);
            }
            else
            {
                Hide();
            }
        }

        private ExplodablePart FindPartByData(DronePartData data)
        {
            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                if (part.Data == data) return part;
            }
            return null;
        }

        public void ShowPartInfo(ExplodablePart part, DronePartData data, string selectionLabel = "", string canonicalPartName = "")
        {
            if (data == null) return;

            currentPart = part;

            string displayName = !string.IsNullOrWhiteSpace(selectionLabel)
                ? selectionLabel
                : (!string.IsNullOrWhiteSpace(canonicalPartName) ? canonicalPartName : data.partName);

            // Update labels
            titleLabel.text = BuildReadableTitle(displayName);
            subtitleLabel.text = FormatTextOrND(data.partType);
            descriptionLabel.text = FormatTextOrND(SanitizeTechnicalText(data.description));
            weightLabel.text = FormatWeight(data.weightKg);
            dimensionsLabel.text = FormatTextOrND(SanitizeTechnicalText(data.dimensions));
            functionLabel.text = FormatTextOrND(SanitizeTechnicalText(data.function));
            materialTypeLabel.text = FormatTextOrND(SanitizeTechnicalText(data.materialType));
            materialPropsLabel.text = FormatTextOrND(SanitizeTechnicalText(data.materialProperties));
            keyNameLabel.text = FormatTextOrND(data.id);
            partNumberLabel.text = FormatTextOrND(BuildPartNumberText(data));
            manufacturerLabel.text = FormatTextOrND(data.manufacturer);
            referencesLabel.text = FormatTextOrND(BuildReferencesText(data));

            Show();
        }

        private static string FormatWeight(float weightKg)
        {
            if (weightKg <= 0f)
            {
                return "N/A";
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
                return "N/A";
            }

            string normalized = value.Trim();
            if (normalized == "-" || normalized.Equals("N/A", System.StringComparison.OrdinalIgnoreCase))
            {
                return "N/A";
            }

            return normalized;
        }

        private static string BuildPartNumberText(DronePartData data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(data.partNumber))
            {
                return data.partNumber;
            }

            if (data.fastenerMetadata != null)
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

            return string.Empty;
        }

        private static string BuildReferencesText(DronePartData data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            string refs = "Holybro X500 V2 documentation | X500 V2 assembly guide";
            if (!string.IsNullOrWhiteSpace(data.id))
            {
                refs += $" | Runtime key: {data.id}";
            }

            if (data.fastenerMetadata != null && !string.IsNullOrWhiteSpace(data.fastenerMetadata.blenderName))
            {
                refs += $" | CAD key: {data.fastenerMetadata.blenderName}";
            }

            return refs;
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
            if (lookup.Contains("BOTTOM-PLATE-X500")) return "Carbon Fiber Bottom Plate";
            if (lookup.Contains("TOP-PLATE-X500")) return "Carbon Fiber Top Plate";
            if (lookup.Contains("BATTERY-MOUNTING-PLAT")) return "Battery Mounting Board";
            if (lookup.Contains("BATTERY-PAD")) return "Battery Silicone Pad";
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
            if (lookup.Contains("M3-16-CHEN-LIU")) return "Countersunk Screw M3 x 16 mm";
            if (lookup.Contains("M25-6-CHEN-LIU")) return "Countersunk Screw M2.5 x 6 mm";
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
                .Replace("Jst", "JST")
                .Replace("Gh ", "GH ")
                .Replace("Kv", "KV")
                .Replace("Usb", "USB");
        }

        public void Show()
        {
            if (infoPanel == null) return;

            infoPanel.style.display = DisplayStyle.Flex;
            // isVisible = true;

            // Animate in
            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(0f, 1f, 0.3f, v => infoPanel.style.opacity = v);
            }
            else
            {
                infoPanel.style.opacity = 1;
            }
        }

        public void Hide()
        {
            if (infoPanel == null) return;

            // isVisible = false;

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(1f, 0f, 0.2f, 
                    v => infoPanel.style.opacity = v,
                    () => infoPanel.style.display = DisplayStyle.None);
            }
            else
            {
                infoPanel.style.opacity = 0;
                infoPanel.style.display = DisplayStyle.None;
            }

            currentPart = null;
        }

        private void OnHideClicked()
        {
            if (currentPart != null)
                PartVisibilityManager.Instance?.HidePart(currentPart);
            AudioManager.Instance?.PlayClick();
        }

        private void OnIsolateClicked()
        {
            Transform isolateTarget = SelectionManager.Instance?.CurrentSelection;
            if (isolateTarget == null && currentPart != null)
            {
                isolateTarget = currentPart.transform;
            }

            if (isolateTarget != null)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.RequestIsolationForSelection(isolateTarget);
                }
                else
                {
                    PartVisibilityManager.Instance?.IsolateTransform(isolateTarget);
                }
            }
            AudioManager.Instance?.PlayClick();
        }

        private void OnFocusClicked()
        {
            if (currentPart != null)
            {
                OrbitCameraController.Instance?.SetTarget(currentPart.transform);
                OrbitCameraController.Instance?.SetDistance(3f);
            }
            AudioManager.Instance?.PlayClick();
        }
    }
}
