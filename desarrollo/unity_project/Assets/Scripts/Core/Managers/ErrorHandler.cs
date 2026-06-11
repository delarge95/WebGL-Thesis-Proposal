using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    /// <summary>
    /// Manages the full-screen error overlay.
    /// Structure defined in ErrorOverlay.uxml + Overlays.uss (C05 remediation).
    /// </summary>
    public class ErrorHandler : PersistentSingleton<ErrorHandler>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset errorTemplate;

        private VisualElement _errorPanel;
        private Label _errorMessage;
        private Button _retryButton;
        private Button _dismissButton;

        private System.Action _onRetry;

        protected override void Awake()
        {
            base.Awake();
            Application.logMessageReceived += HandleLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            InitErrorUI();
        }

        private void InitErrorUI()
        {
            if (uiDocument == null || errorTemplate == null)
            {
                Debug.LogWarning("[ErrorHandler] UIDocument or errorTemplate not assigned.");
                return;
            }

            var root = uiDocument.rootVisualElement;
            var instance = errorTemplate.Instantiate();

            // Query elements by name from the UXML template
            _errorPanel   = instance.Q<VisualElement>("ErrorPanel");
            _errorMessage = instance.Q<Label>("ErrorMessage");
            _retryButton  = instance.Q<Button>("RetryButton");
            _dismissButton = instance.Q<Button>("DismissButton");

            // Wire up button callbacks
            _retryButton.clicked += () => { _onRetry?.Invoke(); Hide(); };
            _dismissButton.clicked += Hide;

            root.Add(_errorPanel);
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                ShowError(logString);
            }
        }

        public void ShowError(string message, System.Action retryAction = null)
        {
            if (_errorPanel == null) return;

            _errorMessage.text = message;
            _onRetry = retryAction;
            _retryButton.style.display = retryAction != null ? DisplayStyle.Flex : DisplayStyle.None;
            _errorPanel.style.display = DisplayStyle.Flex;

            Debug.LogError($"[ErrorHandler] {message}");
        }

        public void Hide()
        {
            if (_errorPanel != null)
            {
                _errorPanel.style.display = DisplayStyle.None;
            }
        }
    }
}
