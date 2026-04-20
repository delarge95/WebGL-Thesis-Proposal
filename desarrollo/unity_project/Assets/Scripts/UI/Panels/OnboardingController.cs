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
        private readonly Button _platformSwitchBtn;
        private readonly VisualElement _platformThumb;
        private readonly VisualElement _platformMobileIcon;
        private readonly VisualElement _platformPcIcon;
        private readonly Button _prevBtn;
        private readonly Button _nextBtn;
        private readonly Button _skipBtn;
        private readonly VisualElement _dotsContainer;
        private readonly Dictionary<StepVisualGlyph, VisualElement> _visualGlyphs = new Dictionary<StepVisualGlyph, VisualElement>();
        private IVisualElementScheduledItem _animatedCueLoop;
        private bool _animatedCueFlip;
        private readonly bool _isTouchDevice;
        private bool _isTouchMode;

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
            public string[] cueSequence;
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
                cueSequence = new[] { "ORBIT", "ZOOM", "PAN" },
                title = "NAVIGATE",
                description = "<b>{ORBIT}</b> to orbit the drone.\n<b>{ZOOM}</b> to control distance.\n<b>{PAN}</b> to reframe the model.\n<b>⟳</b> restores the default camera framing.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.None
            },
            new Step
            {
                icon = "👆",
                caption = "SELECTION",
                cuePrimary = "SELECT",
                cueSequence = new[] { "SELECT", "SUBPIECE", "BACK" },
                title = "SELECT / DESELECT",
                description = "<b>{SELECT}</b> on a visible component to select its parent piece.\n<b>{SELECT}</b> again on a nested element to move into the subpiece level.\n<b>{SELECT_BG}</b> to return one level in the selection hierarchy.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "🔎",
                caption = "PANEL",
                cuePrimary = "PART INFO",
                cueSequence = new[] { "SELECT PART", "PART INFO", "OPEN PANEL" },
                title = "PART INFO",
                description = "Open <b>PART INFO</b> after selecting a parent piece, a subpiece, or a hotspot group.\nThe panel always reflects the currently selected hierarchy level.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "🔍",
                caption = "MAIN MENU",
                cuePrimary = "INSPECT",
                cueSequence = new[] { "PINS", "ISOLATE", "POWER" },
                title = "INSPECT",
                description = "<b>INSPECT</b> centralizes operational inspection controls.\nManage <b>PINS</b>, <b>ISOLATE</b>, and <b>POWER</b> from a single menu.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "📍",
                caption = "INSPECT ACTION",
                cuePrimary = "PINS",
                cueSequence = new[] { "PINS ON", "HOTSPOT GROUP" },
                title = "PINS",
                description = "Toggle <b>PINS</b> to show or hide hotspot markers.\nSelecting a hotspot highlights its related group for focused inspection.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Hotspots
            },
            new Step
            {
                icon = "🎯",
                caption = "INSPECT ACTION",
                cuePrimary = "ISOLATE",
                cueSequence = new[] { "ISOLATE", "BACK ONE LEVEL" },
                title = "ISOLATE",
                description = "<b>ISOLATE</b> focuses the selected parent piece, subpiece, or hotspot group, and selecting <b>ISOLATE</b> again returns one level in the isolation stack.\nAs an alternative, <b>{PART_ISOLATE}</b> or <b>{HOTSPOT_ISOLATE}</b> performs the same flow and opens the part context.\n<b>{BG_DOUBLE}</b> also returns one level back.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Inspect
            },
            new Step
            {
                icon = "⏻",
                caption = "INSPECT ACTION",
                cuePrimary = "POWER",
                cueSequence = new[] { "POWER ON", "THERMAL RUN" },
                title = "POWER",
                description = "<b>POWER</b> toggles the drone between <b>ON/OFF</b>.\nThe <b>POWER SLIDER</b> transitions operating states between <b>IDLE</b> and <b>FLYING</b>.\nIn <b>THERMAL</b>, powering on starts dynamic temperature simulation.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Power
            },
            new Step
            {
                icon = "⚙",
                caption = "MAIN MENU",
                cuePrimary = "ANALYZE",
                cueSequence = new[] { "CUT", "EXPLODE", "FILTER" },
                title = "ANALYZE",
                description = "<b>ANALYZE</b> contains structural review tools for section cuts, exploded view, and category filtering.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Analyze
            },
            new Step
            {
                icon = "✂",
                caption = "ANALYZE ACTION",
                cuePrimary = "CUT",
                cueSequence = new[] { "ONE PLANE", "TWO PLANES", "INCLINED PLANE" },
                title = "CUT",
                description = "<b>{ACTION}</b> axis buttons (<b>X</b>, <b>Y</b>, <b>Z</b>) to activate one or up to two cut planes simultaneously, or disable them.\n<b>{ACTION}</b> <b>ANGLE</b> to replace the two-plane setup with one inclined plane across the selected axes.\nMove the cut controls to adjust plane depth and orientation.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Analyze
            },
            new Step
            {
                icon = "💥",
                caption = "ANALYZE ACTION",
                cuePrimary = "EXPLODE",
                cueSequence = new[] { "EXPLODE OFF", "EXPLODE ON" },
                title = "EXPLODE",
                description = "Move the <b>EXPLODE SLIDER</b> to control assembly separation while preserving hierarchy context.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Analyze
            },
            new Step
            {
                icon = "🧩",
                caption = "ANALYZE ACTION",
                cuePrimary = "FILTER",
                cueSequence = new[] { "FILTERS OFF", "ISOLATE FILTER" },
                title = "FILTER",
                description = "Enable or disable filters by category.\n<b>{FILTER_ISOLATE}</b> to isolate the selected category directly.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Analyze
            },
            new Step
            {
                icon = "🎨",
                caption = "MAIN MENU",
                cuePrimary = "STUDIO",
                cueSequence = new[] { "RENDER MODE", "ENVIRONMENT", "LIGHTING" },
                title = "STUDIO",
                description = "<b>STUDIO</b> manages visual presentation through render mode, environment presets, and lighting controls.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Studio
            },
            new Step
            {
                icon = "🖼",
                caption = "STUDIO ACTION",
                cuePrimary = "RENDER MODE",
                cueSequence = new[] { "REALISTIC", "X-RAY", "THERMAL" },
                title = "RENDER MODE",
                description = "The default state is <b>REALISTIC</b>.\nToggle <b>X-RAY</b>, <b>SOLID</b>, and <b>THERMAL</b> as needed.\nFor live thermal behavior, enable power from <b>INSPECT</b>.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Studio
            },
            new Step
            {
                icon = "🌄",
                caption = "STUDIO ACTION",
                cuePrimary = "ENVIRONMENT",
                cueSequence = new[] { "STUDIO LOOK", "DAY / NIGHT / SUNSET", "COLOR PRESETS" },
                title = "ENVIRONMENT",
                description = "<b>STUDIO</b> cycles <b>DARK</b>, <b>LIGHT</b>, and <b>BLUEPRINT</b>.\n<b>TIME</b> cycles <b>DAY</b>, <b>NIGHT</b>, and <b>SUNSET</b>.\n<b>COLOR</b> cycles flat color presets.",
                visualMode = StepVisualMode.Animated,
                visualGlyph = StepVisualGlyph.Studio
            },
            new Step
            {
                icon = "🎚",
                caption = "STUDIO ACTION",
                cuePrimary = "LIGHTING",
                cueSequence = new[] { "ROTATION", "INTENSITY", "BACKGROUND" },
                title = "LIGHTING CONTROLS",
                description = "Move <b>LIGHT ROTATION</b>, <b>OBJECT INTENSITY</b>, and <b>BACKGROUND TONE</b> sliders independently for controlled visual balance.",
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
            _platformSwitchBtn = _overlay.Q<Button>("OnboardPlatformSwitch");
            _platformThumb = _overlay.Q<VisualElement>("OnboardPlatformThumb");
            _platformMobileIcon = _overlay.Q<VisualElement>("OnboardPlatformMobileIcon");
            _platformPcIcon = _overlay.Q<VisualElement>("OnboardPlatformPcIcon");
            _prevBtn = _overlay.Q<Button>("OnboardPrevBtn");
            _nextBtn = _overlay.Q<Button>("OnboardNextBtn");
            _skipBtn = _overlay.Q<Button>("OnboardSkipBtn");
            _dotsContainer = _overlay.Q<VisualElement>("OnboardDots");

            _isTouchDevice = Application.isMobilePlatform || Input.touchSupported;
            _isTouchMode = _isTouchDevice;

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
            if (_platformSwitchBtn != null)
            {
                System.Action onPlatformChanged = () =>
                {
                    _isTouchMode = !_isTouchMode;
                    UpdatePlatformSwitchVisual();
                    UpdateStep();
                };

                _platformSwitchBtn.clicked += onPlatformChanged;
                _cleanupActions.Add(() => _platformSwitchBtn.clicked -= onPlatformChanged);
            }

            if (_prevBtn != null)
            {
                System.Action onPrev = OnPrevClicked;
                _prevBtn.clicked += onPrev;
                _cleanupActions.Add(() => _prevBtn.clicked -= onPrev);
            }
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

        private void OnPrevClicked()
        {
            if (_currentStep <= 0)
            {
                return;
            }

            _currentStep--;
            UpdateStep();
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
            if (_mediaCue != null) _mediaCue.text = GetStepCue(step, 0);

            if (_stepTitle != null) _stepTitle.text = step.title;
            if (_stepDescription != null) _stepDescription.text = LocalizeStepDescription(step.description);
            if (_stepCounter != null) _stepCounter.text = $"{_currentStep + 1} / {_steps.Count}";
            UpdatePlatformSwitchVisual();

            if (step.visualMode == StepVisualMode.Animated && HasCueSequence(step))
            {
                StartAnimatedCue(step);
            }

            // Next button text
            if (_nextBtn != null)
            {
                _nextBtn.text = _currentStep < _steps.Count - 1 ? "NEXT" : "GOT IT";
            }

            if (_prevBtn != null)
            {
                _prevBtn.SetEnabled(_currentStep > 0);
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

            _visualGlyphs.TryGetValue(step.visualGlyph, out var activeGlyph);

            bool hasProceduralGlyph = step.visualGlyph != StepVisualGlyph.None
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
            var cues = GetCueSequence(step);
            if (_overlay == null || _mediaCue == null || cues.Count == 0)
            {
                return;
            }

            int cueIndex = 0;
            _animatedCueLoop = _overlay.schedule.Execute(() =>
            {
                cueIndex = (cueIndex + 1) % cues.Count;
                _animatedCueFlip = cueIndex > 0;
                _mediaCue.text = cues[cueIndex];
                _stepIcon?.EnableInClassList("onboard-media--phase-alt", cueIndex % 2 == 1);
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

        private string LocalizeStepDescription(string template)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }

            if (_isTouchMode)
            {
                return template
                    .Replace("{ORBIT}", "Drag with one finger")
                    .Replace("{PAN}", "Drag with two fingers")
                    .Replace("{ZOOM}", "Pinch with two fingers")
                    .Replace("{ACTION}", "tap")
                    .Replace("{ACTION_DOUBLE}", "double tap")
                    .Replace("{SELECT}", "Tap")
                    .Replace("{SELECT_BG}", "Tap outside")
                    .Replace("{FILTER_TOGGLE}", "Tap a filter to select or clear it")
                    .Replace("{FILTER_ISOLATE}", "Double tap a filter to isolate it")
                    .Replace("{PART_ISOLATE}", "Double tap a part or subpart to isolate it")
                    .Replace("{HOTSPOT_ISOLATE}", "Double tap a hotspot to isolate it")
                    .Replace("{BG_DOUBLE}", "Double tap outside to return to the previous grouping")
                    .Replace("{INFO_CLOSE}", "Double tap to close the info panel");
            }

            return template
                .Replace("{ORBIT}", "Right mouse button + drag")
                .Replace("{PAN}", "Middle mouse button + drag")
                .Replace("{ZOOM}", "Mouse wheel")
                .Replace("{ACTION}", "click")
                .Replace("{ACTION_DOUBLE}", "double click")
                .Replace("{SELECT}", "Click")
                .Replace("{SELECT_BG}", "Click outside")
                .Replace("{FILTER_TOGGLE}", "Click a filter to select or clear it")
                .Replace("{FILTER_ISOLATE}", "Double click a filter to isolate it")
                .Replace("{PART_ISOLATE}", "Double click a part or subpart to isolate it")
                .Replace("{HOTSPOT_ISOLATE}", "Double click a hotspot to isolate it")
                .Replace("{BG_DOUBLE}", "Double click outside to return to the previous grouping")
                .Replace("{INFO_CLOSE}", "Double click to close the info panel");
        }

        private bool HasCueSequence(Step step)
        {
            return step.cueSequence != null && step.cueSequence.Length > 1;
        }

        private List<string> GetCueSequence(Step step)
        {
            var cues = new List<string>();
            if (step.cueSequence != null && step.cueSequence.Length > 0)
            {
                cues.AddRange(step.cueSequence);
            }
            else if (!string.IsNullOrWhiteSpace(step.cuePrimary))
            {
                cues.Add(step.cuePrimary);
            }

            return cues;
        }

        private string GetStepCue(Step step, int index)
        {
            if (step.cueSequence != null && step.cueSequence.Length > 0)
            {
                int safeIndex = Mathf.Clamp(index, 0, step.cueSequence.Length - 1);
                return step.cueSequence[safeIndex];
            }

            return step.cuePrimary;
        }

        private void UpdatePlatformSwitchVisual()
        {
            if (_platformSwitchBtn != null)
            {
                _platformSwitchBtn.EnableInClassList("onboard-platform-switch--touch", _isTouchMode);
                _platformSwitchBtn.EnableInClassList("onboard-platform-switch--pc", !_isTouchMode);
                _platformSwitchBtn.tooltip = _isTouchMode ? "Mobile mode" : "PC mode";
            }

            if (_platformMobileIcon != null)
            {
                _platformMobileIcon.EnableInClassList("onboard-platform-icon--active", _isTouchMode);
            }

            if (_platformPcIcon != null)
            {
                _platformPcIcon.EnableInClassList("onboard-platform-icon--active", !_isTouchMode);
            }

            if (_platformThumb != null)
            {
                _platformThumb.EnableInClassList("onboard-platform-thumb--touch", _isTouchMode);
            }
        }
    }
}
