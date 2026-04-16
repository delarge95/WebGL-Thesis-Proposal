using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Step-by-step onboarding overlay shown on first visit.
    /// Uses PlayerPrefs("Onboarding_Seen") to persist dismissal across sessions.
    /// Can be re-triggered from the hero menu HELP button.
    /// </summary>
    public class OnboardingController
    {
        private const string PREF_KEY = "Onboarding_Seen";

        // ── Elements ──
        private readonly VisualElement _overlay;
        private readonly Label _stepCounter;
        private readonly Label _stepTitle;
        private readonly Label _stepDescription;
        private readonly VisualElement _stepIcon;
        private readonly Button _nextBtn;
        private readonly Button _skipBtn;
        private readonly VisualElement _dotsContainer;

        // ── Steps definition ──
        private struct Step
        {
            public string icon;
            public string title;
            public string description;
        }

        private readonly List<Step> _steps = new List<Step>
        {
            new Step
            {
                icon = "🖱",
                title = "ORBIT & ZOOM",
                description = "Right-drag to orbit the drone.\nMiddle-drag to pan.\nUse the mouse wheel to zoom."
            },
            new Step
            {
                icon = "👆",
                title = "SELECT PARTS",
                description = "Click any component to select it.\nDouble-click a selected part to open its details."
            },
            new Step
            {
                icon = "🔍",
                title = "INSPECT",
                description = "Use INSPECT to read the part record,\nshow hotspot markers, or isolate a selection."
            },
            new Step
            {
                icon = "⚙",
                title = "ANALYZE",
                description = "Use ANALYZE to reveal cross-sections,\nexploded views, and category filters."
            },
            new Step
            {
                icon = "🎨",
                title = "STUDIO",
                description = "Use STUDIO to switch render modes,\npick an environment, and tune lighting."
            }
        };

        private int _currentStep = 0;

        // ── Cleanup ──
        private readonly List<System.Action> _cleanupActions = new List<System.Action>();

        // ── Events ──
        public event System.Action OnDismissed;

        public OnboardingController(VisualElement root)
        {
            _overlay = root.Q<VisualElement>("OnboardingOverlay");
            if (_overlay == null) return;

            _stepCounter = _overlay.Q<Label>("OnboardStepCounter");
            _stepTitle = _overlay.Q<Label>("OnboardStepTitle");
            _stepDescription = _overlay.Q<Label>("OnboardStepDesc");
            _stepIcon = _overlay.Q<VisualElement>("OnboardStepIcon");
            _nextBtn = _overlay.Q<Button>("OnboardNextBtn");
            _skipBtn = _overlay.Q<Button>("OnboardSkipBtn");
            _dotsContainer = _overlay.Q<VisualElement>("OnboardDots");

            BindButtons();
            BuildDots();
        }

        public void Dispose()
        {
            OnDismissed = null;
            foreach (var a in _cleanupActions) a?.Invoke();
            _cleanupActions.Clear();
        }

        // ═══════════════════════════════════════════════
        //  Public API
        // ═══════════════════════════════════════════════

        /// <summary>Show overlay only if user hasn't seen it before.</summary>
        public void ShowIfFirstTime()
        {
            if (PlayerPrefs.GetInt(PREF_KEY, 0) == 1) return;
            Show();
        }

        /// <summary>Force-show the overlay (e.g. from HELP button).</summary>
        public void Show()
        {
            if (_overlay == null) return;
            _currentStep = 0;
            UpdateStep();
            _overlay.style.display = DisplayStyle.Flex;
            _overlay.RemoveFromClassList("onboard--hidden");
        }

        /// <summary>Dismiss and mark as seen.</summary>
        public void Dismiss()
        {
            if (_overlay == null) return;
            _overlay.AddToClassList("onboard--hidden");
            // Let transition play then hide
            _overlay.schedule.Execute(() =>
            {
                _overlay.style.display = DisplayStyle.None;
            }).ExecuteLater(400);

            PlayerPrefs.SetInt(PREF_KEY, 1);
            PlayerPrefs.Save();
            OnDismissed?.Invoke();
        }

        // ═══════════════════════════════════════════════
        //  Internal
        // ═══════════════════════════════════════════════

        private void BindButtons()
        {
            if (_nextBtn != null)
            {
                System.Action onNext = OnNextClicked;
                _nextBtn.clicked += onNext;
                _cleanupActions.Add(() => _nextBtn.clicked -= onNext);
            }
            if (_skipBtn != null)
            {
                System.Action onSkip = Dismiss;
                _skipBtn.clicked += onSkip;
                _cleanupActions.Add(() => _skipBtn.clicked -= onSkip);
            }
        }

        private void OnNextClicked()
        {
            if (_currentStep < _steps.Count - 1)
            {
                _currentStep++;
                UpdateStep();
            }
            else
            {
                Dismiss();
            }
        }

        private void UpdateStep()
        {
            var step = _steps[_currentStep];

            if (_stepIcon != null)
            {
                // Clear previous icon label and set new one
                var iconLabel = _stepIcon.Q<Label>("OnboardIconLabel");
                if (iconLabel != null) iconLabel.text = step.icon;
            }

            if (_stepTitle != null) _stepTitle.text = step.title;
            if (_stepDescription != null) _stepDescription.text = step.description;
            if (_stepCounter != null) _stepCounter.text = $"{_currentStep + 1} / {_steps.Count}";

            // Next button text
            if (_nextBtn != null)
            {
                _nextBtn.text = _currentStep < _steps.Count - 1 ? "NEXT" : "GOT IT";
            }

            // Update dots
            UpdateDots();
        }

        private void BuildDots()
        {
            if (_dotsContainer == null) return;
            _dotsContainer.Clear();
            for (int i = 0; i < _steps.Count; i++)
            {
                var dot = new VisualElement();
                dot.AddToClassList("onboard-dot");
                if (i == 0) dot.AddToClassList("onboard-dot--active");
                _dotsContainer.Add(dot);
            }
        }

        private void UpdateDots()
        {
            if (_dotsContainer == null) return;
            int i = 0;
            foreach (var child in _dotsContainer.Children())
            {
                child.EnableInClassList("onboard-dot--active", i == _currentStep);
                i++;
            }
        }
    }
}
