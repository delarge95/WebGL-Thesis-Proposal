using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    /// <summary>
    /// Manages the full-screen loading overlay.
    /// Structure defined in LoadingOverlay.uxml + Overlays.uss (C05 remediation).
    /// </summary>
    public class LoadingController : PersistentSingleton<LoadingController>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset loadingTemplate;

        private VisualElement _loadingPanel;
        private VisualElement _progressFill;
        private Label _statusLabel;
        private Label _percentageLabel;

        private float _currentProgress;
        private float _targetProgress;
        private const float SmoothSpeed = 3f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            InitLoadingUI();
        }

        private void InitLoadingUI()
        {
            if (uiDocument == null || loadingTemplate == null)
            {
                Debug.LogWarning("[LoadingController] UIDocument or loadingTemplate not assigned.");
                return;
            }

            var root = uiDocument.rootVisualElement;
            var instance = loadingTemplate.Instantiate();

            // Query elements by name from the UXML template
            _loadingPanel = instance.Q<VisualElement>("LoadingPanel");
            _progressFill = instance.Q<VisualElement>("ProgressFill");
            _statusLabel  = instance.Q<Label>("StatusLabel");
            _percentageLabel = instance.Q<Label>("PercentageLabel");

            root.Add(_loadingPanel);
        }

        private void Update()
        {
            if (_loadingPanel != null && _loadingPanel.style.display == DisplayStyle.Flex)
            {
                _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * SmoothSpeed);
                _progressFill.style.width = Length.Percent(_currentProgress * 100);
                _percentageLabel.text = $"{Mathf.RoundToInt(_currentProgress * 100)}%";
            }
        }

        public void Show()
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.style.display = DisplayStyle.Flex;
                _currentProgress = 0f;
                _targetProgress = 0f;
            }
        }

        public void Hide()
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.style.display = DisplayStyle.None;
            }
        }

        public void SetProgress(float progress, string status = null)
        {
            _targetProgress = Mathf.Clamp01(progress);
            if (!string.IsNullOrEmpty(status) && _statusLabel != null)
            {
                _statusLabel.text = status;
            }
        }
    }
}
