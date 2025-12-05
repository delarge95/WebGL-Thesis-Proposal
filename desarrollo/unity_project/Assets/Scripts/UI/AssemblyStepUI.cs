using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class AssemblyStepUI : Singleton<AssemblyStepUI>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement guidePanel;
        private VisualElement stepContent;
        private Label stepNumberLabel;
        private Label stepTitleLabel;
        private Label instructionsLabel;
        private Label toolsLabel;
        private Label warningsLabel;
        private Label tipLabel;
        private Label timeLabel;
        private VisualElement difficultyContainer;
        private VisualElement progressBar;
        private Label progressLabel;
        private Button prevButton;
        private Button nextButton;
        private Button completeButton;
        private Button closeButton;

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateGuideUI();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (AssemblyGuideManager.Instance != null)
            {
                AssemblyGuideManager.Instance.OnStepChanged += OnStepChanged;
                AssemblyGuideManager.Instance.OnStepCompleted += OnStepCompleted;
                AssemblyGuideManager.Instance.OnGuideCompleted += OnGuideCompleted;
            }
        }

        private void CreateGuideUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Main panel (right side, larger)
            guidePanel = new VisualElement();
            guidePanel.name = "AssemblyGuidePanel";
            guidePanel.style.position = Position.Absolute;
            guidePanel.style.right = 20;
            guidePanel.style.top = 20;
            guidePanel.style.width = 380;
            guidePanel.style.maxHeight = Length.Percent(90);
            guidePanel.style.backgroundColor = new Color(0.06f, 0.08f, 0.12f, 0.98f);
            guidePanel.style.borderRadius = 16;
            guidePanel.style.display = DisplayStyle.None;

            // Header
            var header = CreateHeader();
            guidePanel.Add(header);

            // Progress bar
            var progressContainer = CreateProgressBar();
            guidePanel.Add(progressContainer);

            // Content scroll
            var scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.style.paddingLeft = 20;
            scrollView.style.paddingRight = 20;
            scrollView.style.paddingTop = 15;
            scrollView.style.paddingBottom = 15;

            stepContent = new VisualElement();

            // Step number badge
            stepNumberLabel = new Label("Step 1");
            stepNumberLabel.style.fontSize = 12;
            stepNumberLabel.style.color = new Color(0.4f, 0.8f, 1f);
            stepNumberLabel.style.backgroundColor = new Color(0.4f, 0.8f, 1f, 0.15f);
            stepNumberLabel.style.paddingLeft = 10;
            stepNumberLabel.style.paddingRight = 10;
            stepNumberLabel.style.paddingTop = 4;
            stepNumberLabel.style.paddingBottom = 4;
            stepNumberLabel.style.borderRadius = 12;
            stepNumberLabel.style.alignSelf = Align.FlexStart;
            stepContent.Add(stepNumberLabel);

            // Step title
            stepTitleLabel = new Label("Step Title");
            stepTitleLabel.style.fontSize = 20;
            stepTitleLabel.style.color = Color.white;
            stepTitleLabel.style.marginTop = 10;
            stepTitleLabel.style.marginBottom = 15;
            stepTitleLabel.style.whiteSpace = WhiteSpace.Normal;
            stepContent.Add(stepTitleLabel);

            // Instructions
            instructionsLabel = new Label("Instructions go here...");
            instructionsLabel.style.fontSize = 14;
            instructionsLabel.style.color = new Color(1, 1, 1, 0.85f);
            instructionsLabel.style.whiteSpace = WhiteSpace.Normal;
            instructionsLabel.style.marginBottom = 15;
            stepContent.Add(instructionsLabel);

            // Info cards container
            var cardsContainer = new VisualElement();
            cardsContainer.style.marginTop = 10;

            // Tools card
            var toolsCard = CreateInfoCard("🔧 Required Tools", out toolsLabel);
            cardsContainer.Add(toolsCard);

            // Time & Difficulty card
            var timeCard = CreateInfoCard("⏱️ Estimated Time", out timeLabel);
            cardsContainer.Add(timeCard);

            // Difficulty
            difficultyContainer = new VisualElement();
            difficultyContainer.style.flexDirection = FlexDirection.Row;
            difficultyContainer.style.marginTop = 10;
            difficultyContainer.style.marginBottom = 10;
            var diffLabel = new Label("Difficulty: ");
            diffLabel.style.color = new Color(1, 1, 1, 0.6f);
            diffLabel.style.fontSize = 12;
            difficultyContainer.Add(diffLabel);
            cardsContainer.Add(difficultyContainer);

            // Warnings
            warningsLabel = new Label("");
            warningsLabel.style.fontSize = 12;
            warningsLabel.style.color = new Color(1f, 0.7f, 0.3f);
            warningsLabel.style.backgroundColor = new Color(1f, 0.7f, 0.3f, 0.1f);
            warningsLabel.style.paddingLeft = 10;
            warningsLabel.style.paddingRight = 10;
            warningsLabel.style.paddingTop = 8;
            warningsLabel.style.paddingBottom = 8;
            warningsLabel.style.borderRadius = 8;
            warningsLabel.style.marginTop = 10;
            warningsLabel.style.whiteSpace = WhiteSpace.Normal;
            warningsLabel.style.display = DisplayStyle.None;
            cardsContainer.Add(warningsLabel);

            // Tip
            tipLabel = new Label("");
            tipLabel.style.fontSize = 12;
            tipLabel.style.color = new Color(0.4f, 1f, 0.6f);
            tipLabel.style.backgroundColor = new Color(0.4f, 1f, 0.6f, 0.1f);
            tipLabel.style.paddingLeft = 10;
            tipLabel.style.paddingRight = 10;
            tipLabel.style.paddingTop = 8;
            tipLabel.style.paddingBottom = 8;
            tipLabel.style.borderRadius = 8;
            tipLabel.style.marginTop = 10;
            tipLabel.style.whiteSpace = WhiteSpace.Normal;
            tipLabel.style.display = DisplayStyle.None;
            cardsContainer.Add(tipLabel);

            stepContent.Add(cardsContainer);
            scrollView.Add(stepContent);
            guidePanel.Add(scrollView);

            // Navigation buttons
            var navContainer = CreateNavigation();
            guidePanel.Add(navContainer);

            root.Add(guidePanel);
        }

        private VisualElement CreateHeader()
        {
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.paddingTop = 15;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 15;
            header.style.paddingBottom = 15;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(1, 1, 1, 0.1f);

            var title = new Label("Assembly Guide");
            title.style.fontSize = 16;
            title.style.color = Color.white;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.Add(title);

            closeButton = new Button(Hide);
            closeButton.text = "✕";
            closeButton.style.width = 30;
            closeButton.style.height = 30;
            closeButton.style.backgroundColor = Color.clear;
            closeButton.style.borderWidth = 0;
            closeButton.style.color = new Color(1, 1, 1, 0.5f);
            closeButton.style.fontSize = 16;
            header.Add(closeButton);

            return header;
        }

        private VisualElement CreateProgressBar()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 20;
            container.style.paddingRight = 20;
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;

            progressLabel = new Label("0% Complete");
            progressLabel.style.fontSize = 11;
            progressLabel.style.color = new Color(1, 1, 1, 0.5f);
            progressLabel.style.marginBottom = 5;
            container.Add(progressLabel);

            var barBg = new VisualElement();
            barBg.style.height = 4;
            barBg.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            barBg.style.borderRadius = 2;

            progressBar = new VisualElement();
            progressBar.style.height = Length.Percent(100);
            progressBar.style.width = Length.Percent(0);
            progressBar.style.backgroundColor = new Color(0.2f, 0.8f, 0.4f);
            progressBar.style.borderRadius = 2;

            barBg.Add(progressBar);
            container.Add(barBg);

            return container;
        }

        private VisualElement CreateInfoCard(string title, out Label valueLabel)
        {
            var card = new VisualElement();
            card.style.backgroundColor = new Color(1, 1, 1, 0.05f);
            card.style.borderRadius = 8;
            card.style.paddingLeft = 12;
            card.style.paddingRight = 12;
            card.style.paddingTop = 10;
            card.style.paddingBottom = 10;
            card.style.marginBottom = 8;

            var titleLabel = new Label(title);
            titleLabel.style.fontSize = 11;
            titleLabel.style.color = new Color(1, 1, 1, 0.5f);
            titleLabel.style.marginBottom = 4;
            card.Add(titleLabel);

            valueLabel = new Label("-");
            valueLabel.style.fontSize = 13;
            valueLabel.style.color = Color.white;
            valueLabel.style.whiteSpace = WhiteSpace.Normal;
            card.Add(valueLabel);

            return card;
        }

        private VisualElement CreateNavigation()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.paddingLeft = 20;
            container.style.paddingRight = 20;
            container.style.paddingTop = 15;
            container.style.paddingBottom = 15;
            container.style.borderTopWidth = 1;
            container.style.borderTopColor = new Color(1, 1, 1, 0.1f);

            prevButton = new Button(OnPrevClicked);
            prevButton.text = "← Previous";
            prevButton.style.flexGrow = 1;
            prevButton.style.height = 40;
            prevButton.style.marginRight = 5;
            prevButton.style.backgroundColor = new Color(1, 1, 1, 0.1f);
            prevButton.style.borderWidth = 0;
            prevButton.style.borderRadius = 8;
            prevButton.style.color = Color.white;
            container.Add(prevButton);

            completeButton = new Button(OnCompleteClicked);
            completeButton.text = "✓ Done";
            completeButton.style.width = 80;
            completeButton.style.height = 40;
            completeButton.style.marginRight = 5;
            completeButton.style.backgroundColor = new Color(0.2f, 0.7f, 0.4f);
            completeButton.style.borderWidth = 0;
            completeButton.style.borderRadius = 8;
            completeButton.style.color = Color.white;
            container.Add(completeButton);

            nextButton = new Button(OnNextClicked);
            nextButton.text = "Next →";
            nextButton.style.flexGrow = 1;
            nextButton.style.height = 40;
            nextButton.style.backgroundColor = new Color(0.2f, 0.6f, 1f);
            nextButton.style.borderWidth = 0;
            nextButton.style.borderRadius = 8;
            nextButton.style.color = Color.white;
            container.Add(nextButton);

            return container;
        }

        private void OnStepChanged(AssemblyStep step)
        {
            UpdateUI();
        }

        private void OnStepCompleted(AssemblyStep step)
        {
            UpdateProgress();
        }

        private void OnGuideCompleted()
        {
            // Show completion celebration
        }

        public void UpdateUI()
        {
            var manager = AssemblyGuideManager.Instance;
            if (manager == null || manager.CurrentStep == null) return;

            var step = manager.CurrentStep;

            stepNumberLabel.text = $"Step {manager.CurrentStepIndex + 1} of {manager.TotalSteps}";
            stepTitleLabel.text = step.title;
            instructionsLabel.text = step.instructions;

            // Tools
            if (step.requiredTools != null && step.requiredTools.Length > 0)
            {
                toolsLabel.text = string.Join(", ", step.requiredTools);
            }
            else
            {
                toolsLabel.text = "None required";
            }

            // Time
            timeLabel.text = $"{step.estimatedTimeMinutes:F0} minutes";

            // Difficulty stars
            UpdateDifficulty(step.difficultyLevel);

            // Warnings
            if (step.safetyWarnings != null && step.safetyWarnings.Length > 0)
            {
                warningsLabel.text = "⚠️ " + string.Join("\n⚠️ ", step.safetyWarnings);
                warningsLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                warningsLabel.style.display = DisplayStyle.None;
            }

            // Tip
            if (!string.IsNullOrEmpty(step.tipText))
            {
                tipLabel.text = "💡 " + step.tipText;
                tipLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                tipLabel.style.display = DisplayStyle.None;
            }

            // Complete button state
            completeButton.text = step.isCompleted ? "✓ Done" : "Mark Done";
            completeButton.style.backgroundColor = step.isCompleted 
                ? new Color(0.3f, 0.3f, 0.3f) 
                : new Color(0.2f, 0.7f, 0.4f);

            // Navigation buttons
            prevButton.SetEnabled(manager.CurrentStepIndex > 0);
            nextButton.SetEnabled(manager.CurrentStepIndex < manager.TotalSteps - 1);

            UpdateProgress();
        }

        private void UpdateDifficulty(int level)
        {
            // Clear existing stars
            while (difficultyContainer.childCount > 1)
            {
                difficultyContainer.RemoveAt(1);
            }

            for (int i = 0; i < 5; i++)
            {
                var star = new Label(i < level ? "★" : "☆");
                star.style.color = i < level ? new Color(1f, 0.8f, 0.2f) : new Color(0.3f, 0.3f, 0.3f);
                star.style.fontSize = 14;
                difficultyContainer.Add(star);
            }
        }

        private void UpdateProgress()
        {
            var manager = AssemblyGuideManager.Instance;
            if (manager == null) return;

            float percent = manager.ProgressPercent;
            progressBar.style.width = Length.Percent(percent);
            progressLabel.text = $"{percent:F0}% Complete ({manager.GetCompletedCount()}/{manager.TotalSteps} steps)";
        }

        private void OnPrevClicked()
        {
            AssemblyGuideManager.Instance?.PreviousStep();
        }

        private void OnNextClicked()
        {
            AssemblyGuideManager.Instance?.NextStep();
        }

        private void OnCompleteClicked()
        {
            AssemblyGuideManager.Instance?.ToggleStepComplete();
            UpdateUI();
        }

        public void Show()
        {
            if (guidePanel == null) return;
            guidePanel.style.display = DisplayStyle.Flex;
            UpdateUI();

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(0f, 1f, 0.3f, v => guidePanel.style.opacity = v);
            }
        }

        public void Hide()
        {
            if (guidePanel == null) return;
            AssemblyGuideManager.Instance?.StopGuide();

            if (TweenEngine.Instance != null)
            {
                TweenEngine.Instance.TweenFloat(1f, 0f, 0.2f,
                    v => guidePanel.style.opacity = v,
                    () => guidePanel.style.display = DisplayStyle.None);
            }
            else
            {
                guidePanel.style.display = DisplayStyle.None;
            }
        }

        public void Toggle()
        {
            if (guidePanel.style.display == DisplayStyle.None)
                Show();
            else
                Hide();
        }
    }
}
