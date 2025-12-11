using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class LoadingController : PersistentSingleton<LoadingController>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement loadingPanel;
        private VisualElement progressBar;
        private Label statusLabel;
        private Label percentageLabel;

        private float currentProgress = 0f;
        private float targetProgress = 0f;
        private const float SmoothSpeed = 3f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateLoadingUI();
        }

        private void CreateLoadingUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main container
            loadingPanel = new VisualElement();
            loadingPanel.style.position = Position.Absolute;
            loadingPanel.style.left = 0;
            loadingPanel.style.right = 0;
            loadingPanel.style.top = 0;
            loadingPanel.style.bottom = 0;
            loadingPanel.style.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
            loadingPanel.style.justifyContent = Justify.Center;
            loadingPanel.style.alignItems = Align.Center;
            loadingPanel.style.display = DisplayStyle.None;

            // Title
            var title = new Label("LOADING");
            title.style.fontSize = 28;
            title.style.color = Color.white;
            title.style.marginBottom = 30;
            title.style.letterSpacing = 8;
            loadingPanel.Add(title);

            // Progress bar container
            var progressContainer = new VisualElement();
            progressContainer.style.width = Length.Percent(50);
            progressContainer.style.height = 4;
            progressContainer.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            progressContainer.style.borderTopLeftRadius = 2; progressContainer.style.borderTopRightRadius = 2; progressContainer.style.borderBottomLeftRadius = 2; progressContainer.style.borderBottomRightRadius = 2;
            loadingPanel.Add(progressContainer);

            // Progress bar fill
            progressBar = new VisualElement();
            progressBar.style.height = Length.Percent(100);
            progressBar.style.width = Length.Percent(0);
            progressBar.style.backgroundColor = new Color(0.2f, 0.8f, 1f, 1f);
            progressBar.style.borderTopLeftRadius = 2; progressBar.style.borderTopRightRadius = 2; progressBar.style.borderBottomLeftRadius = 2; progressBar.style.borderBottomRightRadius = 2;
            progressContainer.Add(progressBar);

            // Status label
            statusLabel = new Label("Initializing...");
            statusLabel.style.fontSize = 14;
            statusLabel.style.color = new Color(1, 1, 1, 0.6f);
            statusLabel.style.marginTop = 20;
            loadingPanel.Add(statusLabel);

            // Percentage
            percentageLabel = new Label("0%");
            percentageLabel.style.fontSize = 14;
            percentageLabel.style.color = new Color(1, 1, 1, 0.4f);
            percentageLabel.style.marginTop = 10;
            loadingPanel.Add(percentageLabel);

            root.Add(loadingPanel);
        }

        private void Update()
        {
            if (loadingPanel != null && loadingPanel.style.display == DisplayStyle.Flex)
            {
                currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * SmoothSpeed);
                progressBar.style.width = Length.Percent(currentProgress * 100);
                percentageLabel.text = $"{Mathf.RoundToInt(currentProgress * 100)}%";
            }
        }

        public void Show()
        {
            if (loadingPanel != null)
            {
                loadingPanel.style.display = DisplayStyle.Flex;
                currentProgress = 0f;
                targetProgress = 0f;
            }
        }

        public void Hide()
        {
            if (loadingPanel != null)
            {
                loadingPanel.style.display = DisplayStyle.None;
            }
        }

        public void SetProgress(float progress, string status = null)
        {
            targetProgress = Mathf.Clamp01(progress);
            if (!string.IsNullOrEmpty(status) && statusLabel != null)
            {
                statusLabel.text = status;
            }
        }
    }
}
