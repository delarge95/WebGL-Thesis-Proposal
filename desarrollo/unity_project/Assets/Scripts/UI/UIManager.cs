using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainDocument;

        private VisualElement root;
        private VisualElement detailsPanel;
        private VisualElement sidebar;
        private Button explodeButton;
        private Button resetButton;
        private Slider explosionSlider;
        private Label stateLabel;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            if (mainDocument == null)
                mainDocument = GetComponent<UIDocument>();

            if (mainDocument != null)
            {
                root = mainDocument.rootVisualElement;
                InitializeUI();
                SubscribeToEvents();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void InitializeUI()
        {
            // Query elements by ID
            detailsPanel = root.Q<VisualElement>("DetailsPanel");
            sidebar = root.Q<VisualElement>("Sidebar");
            explodeButton = root.Q<Button>("ExplodeBtn");
            resetButton = root.Q<Button>("ResetBtn");
            explosionSlider = root.Q<Slider>("ExplosionSlider");
            stateLabel = root.Q<Label>("StateLabel");

            // Button callbacks
            if (explodeButton != null)
            {
                explodeButton.clicked += OnExplodeClicked;
            }

            if (resetButton != null)
            {
                resetButton.clicked += OnResetClicked;
            }

            if (explosionSlider != null)
            {
                explosionSlider.RegisterValueChangedCallback(OnSliderChanged);
            }

            // Initial state
            if (detailsPanel != null)
                detailsPanel.style.display = DisplayStyle.None;
        }

        private void SubscribeToEvents()
        {
            EventBus.Subscribe<AppStateChangedEvent>(OnStateChanged);
            EventBus.Subscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void UnsubscribeFromEvents()
        {
            EventBus.Unsubscribe<AppStateChangedEvent>(OnStateChanged);
            EventBus.Unsubscribe<PartSelectedEvent>(OnPartSelected);
        }

        private void OnStateChanged(AppStateChangedEvent evt)
        {
            // Update UI based on new state
            if (stateLabel != null)
            {
                stateLabel.text = evt.NewState.ToString();
            }

            // Update button text
            if (explodeButton != null)
            {
                explodeButton.text = evt.NewState == AppState.ExplodedView ? "Collapse" : "Explode";
            }

            // Show/hide slider
            if (explosionSlider != null)
            {
                explosionSlider.style.display = evt.NewState == AppState.ExplodedView 
                    ? DisplayStyle.Flex 
                    : DisplayStyle.None;
            }
        }

        private void OnPartSelected(PartSelectedEvent evt)
        {
            if (detailsPanel == null) return;

            if (evt.PartData != null)
            {
                // Show panel with animation
                if (UIAnimator.Instance != null)
                {
                    UIAnimator.Instance.FadeIn(detailsPanel);
                }
                else
                {
                    detailsPanel.style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                // Hide panel with animation
                if (UIAnimator.Instance != null)
                {
                    UIAnimator.Instance.FadeOut(detailsPanel);
                }
                else
                {
                    detailsPanel.style.display = DisplayStyle.None;
                }
            }
        }

        private void OnExplodeClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            if (AppStateMachine.Instance != null)
            {
                var current = AppStateMachine.Instance.CurrentState;
                if (current == AppState.ExplodedView)
                {
                    AppStateMachine.Instance.SetState(AppState.Exploration);
                }
                else
                {
                    AppStateMachine.Instance.SetState(AppState.ExplodedView);
                }
            }
            else if (GameManager.Instance != null)
            {
                // Fallback
                GameManager.Instance.SetState(AppState.ExplodedView);
            }
        }

        private void OnResetClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            if (OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.ResetView();
            }

            if (SelectionManager.Instance != null)
            {
                SelectionManager.Instance.Deselect();
            }
        }

        private void OnSliderChanged(ChangeEvent<float> evt)
        {
            var manager = FindAnyObjectByType<WebGL.Core.Content.ExplodedViewManager>();
            if (manager != null)
            {
                manager.SetExplosionFactor(evt.newValue);
            }
        }
    }
}
