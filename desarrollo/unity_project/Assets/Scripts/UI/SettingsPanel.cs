using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Managers;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.UI
{
    public class SettingsPanel : Singleton<SettingsPanel>
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement settingsContainer;
        private Slider masterVolumeSlider;
        private Slider sfxVolumeSlider;
        private Slider musicVolumeSlider;
        private Slider uiScaleSlider;
        private Toggle highContrastToggle;
        private Toggle reducedMotionToggle;
        private Button closeButton;

        private bool isVisible = false;

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateSettingsUI();
        }

        private void CreateSettingsUI()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Container
            settingsContainer = new VisualElement();
            settingsContainer.style.position = Position.Absolute;
            settingsContainer.style.left = 0;
            settingsContainer.style.right = 0;
            settingsContainer.style.top = 0;
            settingsContainer.style.bottom = 0;
            settingsContainer.style.backgroundColor = new Color(0, 0, 0, 0.8f);
            settingsContainer.style.justifyContent = Justify.Center;
            settingsContainer.style.alignItems = Align.Center;
            settingsContainer.style.display = DisplayStyle.None;

            // Panel
            var panel = new VisualElement();
            panel.style.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            panel.style.width = 400;
            panel.style.paddingTop = 20;
            panel.style.paddingBottom = 20;
            panel.style.paddingLeft = 30;
            panel.style.paddingRight = 30;
            panel.style.borderTopLeftRadius = 12; panel.style.borderTopRightRadius = 12; panel.style.borderBottomLeftRadius = 12; panel.style.borderBottomRightRadius = 12;

            // Title
            var title = new Label("Settings");
            title.style.fontSize = 24;
            title.style.color = Color.white;
            title.style.marginBottom = 20;
            title.style.unityTextAlign = TextAnchor.MiddleCenter;
            panel.Add(title);

            // Audio Section
            panel.Add(CreateSectionLabel("Audio"));
            masterVolumeSlider = CreateSlider("Master Volume", 0, 1, SaveSystem.LoadVolume());
            masterVolumeSlider.RegisterValueChangedCallback(e => AudioManager.Instance?.SetMasterVolume(e.newValue));
            panel.Add(masterVolumeSlider);

            sfxVolumeSlider = CreateSlider("SFX Volume", 0, 1, PlayerPrefs.GetFloat("SFXVolume", 1f));
            sfxVolumeSlider.RegisterValueChangedCallback(e => AudioManager.Instance?.SetSFXVolume(e.newValue));
            panel.Add(sfxVolumeSlider);

            musicVolumeSlider = CreateSlider("Music Volume", 0, 1, PlayerPrefs.GetFloat("MusicVolume", 0.5f));
            musicVolumeSlider.RegisterValueChangedCallback(e => AudioManager.Instance?.SetMusicVolume(e.newValue));
            panel.Add(musicVolumeSlider);

            // Accessibility Section
            panel.Add(CreateSectionLabel("Accessibility"));
            
            uiScaleSlider = CreateSlider("UI Scale", 0.8f, 1.5f, AccessibilityManager.Instance?.UIScale ?? 1f);
            uiScaleSlider.RegisterValueChangedCallback(e => AccessibilityManager.Instance?.SetUIScale(e.newValue));
            panel.Add(uiScaleSlider);

            highContrastToggle = new Toggle("High Contrast Mode");
            highContrastToggle.value = AccessibilityManager.Instance?.HighContrastMode ?? false;
            highContrastToggle.RegisterValueChangedCallback(e => AccessibilityManager.Instance?.SetHighContrastMode(e.newValue));
            highContrastToggle.style.marginTop = 10;
            panel.Add(highContrastToggle);

            reducedMotionToggle = new Toggle("Reduced Motion");
            reducedMotionToggle.value = AccessibilityManager.Instance?.ReducedMotion ?? false;
            reducedMotionToggle.RegisterValueChangedCallback(e => AccessibilityManager.Instance?.SetReducedMotion(e.newValue));
            reducedMotionToggle.style.marginTop = 5;
            panel.Add(reducedMotionToggle);

            // Close Button
            closeButton = new Button(Hide);
            closeButton.text = "Close";
            closeButton.style.marginTop = 30;
            closeButton.style.paddingTop = 10;
            closeButton.style.paddingBottom = 10;
            panel.Add(closeButton);

            settingsContainer.Add(panel);
            root.Add(settingsContainer);
        }

        private Label CreateSectionLabel(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 16;
            label.style.color = new Color(0.6f, 0.8f, 1f);
            label.style.marginTop = 15;
            label.style.marginBottom = 5;
            return label;
        }

        private Slider CreateSlider(string label, float min, float max, float value)
        {
            var slider = new Slider(label, min, max);
            slider.value = value;
            slider.style.marginTop = 5;
            return slider;
        }

        public void Show()
        {
            if (settingsContainer == null) return;
            settingsContainer.style.display = DisplayStyle.Flex;
            isVisible = true;

            if (AppStateMachine.Instance != null)
            {
                AppStateMachine.Instance.SetState(AppState.Settings);
            }
        }

        public void Hide()
        {
            if (settingsContainer == null) return;
            settingsContainer.style.display = DisplayStyle.None;
            isVisible = false;

            if (AppStateMachine.Instance != null)
            {
                AppStateMachine.Instance.SetState(AppState.Exploration);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public void Toggle()
        {
            if (isVisible) Hide();
            else Show();
        }
    }
}
