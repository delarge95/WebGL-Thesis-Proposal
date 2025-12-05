using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class ErrorHandler : PersistentSingleton<ErrorHandler>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement errorPanel;
        private Label errorMessage;
        private Button retryButton;
        private Button dismissButton;

        private System.Action onRetry;

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
            CreateErrorUI();
        }

        private void CreateErrorUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Error panel
            errorPanel = new VisualElement();
            errorPanel.style.position = Position.Absolute;
            errorPanel.style.left = 0;
            errorPanel.style.right = 0;
            errorPanel.style.top = 0;
            errorPanel.style.bottom = 0;
            errorPanel.style.backgroundColor = new Color(0.1f, 0f, 0f, 0.95f);
            errorPanel.style.justifyContent = Justify.Center;
            errorPanel.style.alignItems = Align.Center;
            errorPanel.style.display = DisplayStyle.None;

            // Error icon
            var icon = new Label("⚠");
            icon.style.fontSize = 64;
            icon.style.color = new Color(1f, 0.3f, 0.3f);
            icon.style.marginBottom = 20;
            errorPanel.Add(icon);

            // Title
            var title = new Label("An error occurred");
            title.style.fontSize = 24;
            title.style.color = Color.white;
            title.style.marginBottom = 10;
            errorPanel.Add(title);

            // Message
            errorMessage = new Label("Unknown error");
            errorMessage.style.fontSize = 14;
            errorMessage.style.color = new Color(1, 1, 1, 0.7f);
            errorMessage.style.marginBottom = 30;
            errorMessage.style.maxWidth = Length.Percent(80);
            errorMessage.style.whiteSpace = WhiteSpace.Normal;
            errorMessage.style.unityTextAlign = TextAnchor.MiddleCenter;
            errorPanel.Add(errorMessage);

            // Buttons container
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;

            // Retry button
            retryButton = new Button(() => { onRetry?.Invoke(); Hide(); });
            retryButton.text = "Retry";
            retryButton.style.marginRight = 10;
            retryButton.style.paddingLeft = 30;
            retryButton.style.paddingRight = 30;
            retryButton.style.paddingTop = 10;
            retryButton.style.paddingBottom = 10;
            buttonContainer.Add(retryButton);

            // Dismiss button
            dismissButton = new Button(Hide);
            dismissButton.text = "Dismiss";
            dismissButton.style.paddingLeft = 30;
            dismissButton.style.paddingRight = 30;
            dismissButton.style.paddingTop = 10;
            dismissButton.style.paddingBottom = 10;
            buttonContainer.Add(dismissButton);

            errorPanel.Add(buttonContainer);
            root.Add(errorPanel);
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
            if (errorPanel == null) return;

            errorMessage.text = message;
            onRetry = retryAction;
            retryButton.style.display = retryAction != null ? DisplayStyle.Flex : DisplayStyle.None;
            errorPanel.style.display = DisplayStyle.Flex;

            Debug.LogError($"[ErrorHandler] {message}");
        }

        public void Hide()
        {
            if (errorPanel != null)
            {
                errorPanel.style.display = DisplayStyle.None;
            }
        }
    }
}
