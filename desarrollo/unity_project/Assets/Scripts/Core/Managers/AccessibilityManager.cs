using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class AccessibilityManager : PersistentSingleton<AccessibilityManager>
    {
        [Header("Settings")]
        [SerializeField] private float defaultUIScale = 1f;
        [SerializeField] private float minUIScale = 0.8f;
        [SerializeField] private float maxUIScale = 1.5f;

        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private float currentUIScale = 1f;
        private bool highContrastMode = false;
        private bool reducedMotion = false;

        public float UIScale => currentUIScale;
        public bool HighContrastMode => highContrastMode;
        public bool ReducedMotion => reducedMotion;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            ApplySettings();
        }

        private void LoadSettings()
        {
            currentUIScale = PlayerPrefs.GetFloat("A11y_UIScale", defaultUIScale);
            highContrastMode = PlayerPrefs.GetInt("A11y_HighContrast", 0) == 1;
            reducedMotion = PlayerPrefs.GetInt("A11y_ReducedMotion", 0) == 1;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("A11y_UIScale", currentUIScale);
            PlayerPrefs.SetInt("A11y_HighContrast", highContrastMode ? 1 : 0);
            PlayerPrefs.SetInt("A11y_ReducedMotion", reducedMotion ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetUIScale(float scale)
        {
            currentUIScale = Mathf.Clamp(scale, minUIScale, maxUIScale);
            ApplySettings();
            SaveSettings();
        }

        public void SetHighContrastMode(bool enabled)
        {
            highContrastMode = enabled;
            ApplySettings();
            SaveSettings();
        }

        public void SetReducedMotion(bool enabled)
        {
            reducedMotion = enabled;
            ApplySettings();
            SaveSettings();
        }

        private void ApplySettings()
        {
            if (uiDocument == null) return;

            var root = uiDocument.rootVisualElement;

            // Apply UI Scale
            root.style.scale = new StyleScale(new Scale(new Vector3(currentUIScale, currentUIScale, 1f)));

            // Apply High Contrast Mode
            if (highContrastMode)
            {
                root.AddToClassList("high-contrast");
            }
            else
            {
                root.RemoveFromClassList("high-contrast");
            }

            // Apply Reduced Motion
            if (reducedMotion)
            {
                root.AddToClassList("reduced-motion");
            }
            else
            {
                root.RemoveFromClassList("reduced-motion");
            }

            Debug.Log($"[Accessibility] Scale: {currentUIScale}, HighContrast: {highContrastMode}, ReducedMotion: {reducedMotion}");
        }
    }
}
