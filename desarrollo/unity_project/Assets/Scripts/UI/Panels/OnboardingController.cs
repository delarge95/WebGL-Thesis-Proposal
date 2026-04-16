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
        private readonly Label _mediaCaption;
        private readonly Label _mediaCue;
        private readonly Label _stepEmojiIcon;
        private readonly Label _stepTitle;
        private readonly Label _stepDescription;
        private readonly VisualElement _stepIcon;
        private readonly Button _nextBtn;
        private readonly Button _skipBtn;
        private readonly VisualElement _dotsContainer;
        private readonly Dictionary<StepVisualGlyph, VisualElement> _visualGlyphs = new Dictionary<StepVisualGlyph, VisualElement>();
        private IVisualElementScheduledItem _animatedCueLoop;
        private bool _animatedCueFlip;

        private enum StepVisualMode
        {
            Static,
            Animated
        }

        private enum StepVisualGlyph
        {
            None,
            Hotspots,
            Inspect,
            Power,
            Analyze,
            Thermal,
            Studio
        }

        // ── Steps definition ──
        private struct Step
        {
            public string icon;
            public string caption;
            public string cuePrimary;
            public string cueSecondary;
            public string title;
            public string description;
            public StepVisualMode visualMode;
            public StepVisualGlyph visualGlyph;
        }

        private readonly List<Step> _steps = new List<Step>
        {
            new Step
            {
                icon = "🖱",
                caption = "GESTURE",
                cuePrimary = "ORBIT",
                cueSecondary = "ZOOM",
                title = "ORBIT & ZOOM",
                description = "Right-drag to orbit the drone.\nMiddle-drag to pan.\nUse the mouse wheel to zoom.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.None
            },
            new Step
            {
                icon = "👆",
                caption = "SELECTION",
                cuePrimary = "MOTHER PART",
                cueSecondary = "SUBPIECE",
                title = "SELECT PARTS",
                description = "Click a visible mesh to select its mother part.\nClick a child mesh to drill into a subpiece.\nDouble-click the current selection to open details.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "📍",
                caption = "PINS",
                cuePrimary = "DISCOVER",
                cueSecondary = "",
                title = "HOTSPOTS",
                description = "Use PINS to show hotspot markers on hotspot-enabled parts.\nHotspot selections can resolve to grouped assemblies.\nSelecting a hotspot updates the inspected part record.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Hotspots
            },
            new Step
            {
                icon = "🔍",
                caption = "DETAILS",
                cuePrimary = "INSPECT",
                cueSecondary = "",
                title = "INSPECT",
                description = "Use INSPECT to read the selected part record.\nUse ISOLATE to focus on the current selection.\nUse POWER to open the drone power controls.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "⏻",
                caption = "LOAD",
                cuePrimary = "0% TO 100%",
                cueSecondary = "",
                title = "POWER",
                description = "The power control sets the drone load from 0% to 100%.\nThe state label switches between OFF and ON.\nUse it to simulate the powered state shown in the panel.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Power
            },
            new Step
            {
                icon = "⚙",
                caption = "CUT / EXPLODE",
                cuePrimary = "ANALYZE",
                cueSecondary = "",
                title = "ANALYZE",
                description = "Use ANALYZE for cut planes, exploded views, and category filters.\nThe cut tool supports axis selection and inversion.\nThe explode slider controls separation strength.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Analyze
            },
            new Step
            {
                icon = "🌡",
                caption = "HEAT MAP",
                cuePrimary = "THERMAL",
                cueSecondary = "",
                title = "THERMAL",
                description = "THERMAL switches the scene to thermal shading.\nThe legend shows the active temperature range.\nThis mode uses the thermal data bound to each part.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Thermal
            },
            new Step
            {
                icon = "🎨",
                caption = "RENDER",
                cuePrimary = "STUDIO",
                cueSecondary = "",
                title = "STUDIO",
                description = "Use STUDIO to switch render modes,\npick an environment preset, and tune lighting.\nBlueprint is available from the Studio environment cycle.",
                visualMode = StepVisualMode.Static,
                visualGlyph = StepVisualGlyph.Studio
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
            _mediaCaption = _overlay.Q<Label>("OnboardMediaCaption");
            _mediaCue = _overlay.Q<Label>("OnboardMediaCue");
            _stepEmojiIcon = _overlay.Q<Label>("OnboardIconLabel");
            _stepTitle = _overlay.Q<Label>("OnboardStepTitle");
            _stepDescription = _overlay.Q<Label>("OnboardStepDesc");
            _stepIcon = _overlay.Q<VisualElement>("OnboardStepIcon");
            _nextBtn = _overlay.Q<Button>("OnboardNextBtn");
            _skipBtn = _overlay.Q<Button>("OnboardSkipBtn");
            _dotsContainer = _overlay.Q<VisualElement>("OnboardDots");

            _visualGlyphs[StepVisualGlyph.Hotspots] = _overlay.Q<VisualElement>("OnboardVisualHotspots");
            _visualGlyphs[StepVisualGlyph.Inspect] = _overlay.Q<VisualElement>("OnboardVisualInspect");
            _visualGlyphs[StepVisualGlyph.Power] = _overlay.Q<VisualElement>("OnboardVisualPower");
            _visualGlyphs[StepVisualGlyph.Analyze] = _overlay.Q<VisualElement>("OnboardVisualAnalyze");
            _visualGlyphs[StepVisualGlyph.Thermal] = _overlay.Q<VisualElement>("OnboardVisualThermal");
            _visualGlyphs[StepVisualGlyph.Studio] = _overlay.Q<VisualElement>("OnboardVisualStudio");

            BindButtons();
            BuildDots();
        }

        public void Dispose()
        {
            StopAnimatedCue();
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
            StopAnimatedCue();
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

            StopAnimatedCue();
            SetActiveGlyph(step);

            if (_stepIcon != null)
            {
                _stepIcon.EnableInClassList("onboard-media--animated", step.visualMode == StepVisualMode.Animated);
                _stepIcon.EnableInClassList("onboard-media--static", step.visualMode == StepVisualMode.Static);
            }

            if (_mediaCaption != null) _mediaCaption.text = step.caption;
            if (_mediaCue != null) _mediaCue.text = step.cuePrimary;

            if (_stepTitle != null) _stepTitle.text = step.title;
            if (_stepDescription != null) _stepDescription.text = step.description;
            if (_stepCounter != null) _stepCounter.text = $"{_currentStep + 1} / {_steps.Count}";

            if (step.visualMode == StepVisualMode.Animated && !string.IsNullOrWhiteSpace(step.cueSecondary))
            {
                StartAnimatedCue(step);
            }

            // Next button text
            if (_nextBtn != null)
            {
                _nextBtn.text = _currentStep < _steps.Count - 1 ? "NEXT" : "GOT IT";
            }

            // Update dots
            UpdateDots();
        }

        private void SetActiveGlyph(Step step)
        {
            foreach (var glyph in _visualGlyphs.Values)
            {
                if (glyph != null)
                {
                    glyph.style.display = DisplayStyle.None;
                }
            }

            bool hasProceduralGlyph = step.visualGlyph != StepVisualGlyph.None
                && _visualGlyphs.TryGetValue(step.visualGlyph, out var activeGlyph)
                && activeGlyph != null;

            if (hasProceduralGlyph)
            {
                activeGlyph.style.display = DisplayStyle.Flex;
            }

            if (_stepEmojiIcon != null)
            {
                _stepEmojiIcon.text = step.icon;
                _stepEmojiIcon.style.display = hasProceduralGlyph ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void StartAnimatedCue(Step step)
        {
            if (_overlay == null || _mediaCue == null)
            {
                return;
            }

            _animatedCueFlip = false;
            _animatedCueLoop = _overlay.schedule.Execute(() =>
            {
                _animatedCueFlip = !_animatedCueFlip;
                _mediaCue.text = _animatedCueFlip ? step.cueSecondary : step.cuePrimary;
                _stepIcon?.EnableInClassList("onboard-media--phase-alt", _animatedCueFlip);
            }).Every(720);
        }

        private void StopAnimatedCue()
        {
            _animatedCueLoop?.Pause();
            _animatedCueLoop = null;
            _stepIcon?.RemoveFromClassList("onboard-media--phase-alt");
            _animatedCueFlip = false;
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
