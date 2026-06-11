using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.Panels
{
    public enum OnboardingSceneId
    {
        None,
        Navigate,
        Select,
        PartInfo,
        Inspect,
        Pins,
        Isolate,
        Power,
        Analyze,
        Cut,
        Explode,
        Filter,
        Studio,
        RenderMode,
        Environment,
        Lighting
    }

    internal enum OnboardingMiniIcon
    {
        Inspect,
        Pin,
        Isolate,
        Power,
        Analyze,
        Cut,
        Explode,
        Filter,
        Studio,
        Render,
        Environment,
        Lighting,
        Info,
        Reset
    }

    /// <summary>
    /// Painter2D-driven preview canvas for onboarding cards.
    /// It renders three things for every step:
    /// 1. the interaction actor (cursor or touch point),
    /// 2. the UI/model target that receives the action,
    /// 3. the system response that explains the result.
    /// This keeps the onboarding lightweight while still showing the same gestures
    /// and UI intent that the real app uses.
    /// </summary>
    public sealed class OnboardingAnimationView : VisualElement
    {
        private enum MouseHintButton
        {
            Right,
            Middle
        }

        private enum CutButtonKind
        {
            X,
            Y,
            Z,
            Invert,
            Angle
        }

        private enum RenderOptionKind
        {
            XRay,
            Solid,
            Thermal
        }

        private enum FilterGlyphKind
        {
            Airframe,
            Propulsion,
            Avionics,
            Sensors,
            Power,
            Fasteners
        }

        private static readonly Color FrameFill = new Color(0.06f, 0.08f, 0.11f, 0.62f);
        private static readonly Color FrameStroke = new Color(0.58f, 0.73f, 0.88f, 0.14f);
        private static readonly Color Accent = new Color(0.40f, 0.77f, 1.00f, 1.00f);
        private static readonly Color AccentSoft = new Color(0.40f, 0.77f, 1.00f, 0.48f);
        private static readonly Color AccentGlow = new Color(0.40f, 0.77f, 1.00f, 0.18f);
        private static readonly Color Surface = new Color(0.15f, 0.18f, 0.24f, 0.90f);
        private static readonly Color SurfaceAlt = new Color(0.19f, 0.23f, 0.31f, 0.96f);
        private static readonly Color SurfaceMuted = new Color(0.18f, 0.20f, 0.24f, 0.52f);
        private static readonly Color Foreground = new Color(0.93f, 0.96f, 1.00f, 0.96f);
        private static readonly Color ForegroundMuted = new Color(0.78f, 0.83f, 0.91f, 0.54f);
        private static readonly Color Warning = new Color(1.00f, 0.83f, 0.24f, 1.00f);
        private static readonly Color Success = new Color(0.40f, 0.96f, 0.65f, 1.00f);
        private static readonly Color Danger = new Color(1.00f, 0.42f, 0.42f, 1.00f);
        private static readonly Color ThermalHot = new Color(1.00f, 0.52f, 0.16f, 0.92f);
        private static readonly Color ThermalWarm = new Color(1.00f, 0.76f, 0.20f, 0.82f);
        private static readonly Color ThermalCool = new Color(0.38f, 0.72f, 1.00f, 0.68f);
        private static readonly Color Blueprint = new Color(0.43f, 0.78f, 1.00f, 0.26f);
        private static readonly Color AmbientNight = new Color(0.10f, 0.16f, 0.28f, 0.96f);
        private static readonly Color AmbientDay = new Color(0.73f, 0.83f, 0.95f, 0.96f);
        private static readonly Color AmbientSunset = new Color(0.95f, 0.47f, 0.27f, 0.90f);
        private static readonly Color SelectedFill = new Color(0.2f, 0.8f, 1f, 0.5f);
        private static readonly Color ParentSelectedFill = new Color(0.2f, 0.8f, 1f, 0.18f);
        private static readonly Color HoverFill = new Color(1f, 1f, 1f, 0.3f);
        private static readonly Color HotspotGroupFill = new Color(1f, 0.78f, 0.24f, 0.24f);
        private static readonly Color GestureTrail = new Color(0.40f, 0.77f, 1.00f, 0.18f);
        private static readonly Color GestureTrailStrong = new Color(0.40f, 0.77f, 1.00f, 0.28f);

        private OnboardingSceneId _scene = OnboardingSceneId.None;
        private bool _isTouchMode;
        private bool _isPlaying;
        private float _animationStartTime;
        private int[] _phaseDurationsMs = Array.Empty<int>();
        private int _currentPhaseIndex = -1;
        private readonly Label _partInfoTabLabel;
        private readonly Label _studioRenderLabel;
        private readonly Label _studioEnvironmentLabel;

        public event Action<int> OnPhaseChanged;

        public int CurrentPhaseIndex => Mathf.Max(_currentPhaseIndex, 0);

        public OnboardingAnimationView()
        {
            pickingMode = PickingMode.Ignore;
            generateVisualContent += OnGenerateVisualContent;
            _partInfoTabLabel = new Label("INFO");
            _partInfoTabLabel.pickingMode = PickingMode.Ignore;
            _partInfoTabLabel.AddToClassList("onboard-demo-tab-label");
            _partInfoTabLabel.style.display = DisplayStyle.None;
            hierarchy.Add(_partInfoTabLabel);

            _studioRenderLabel = new Label("RENDER");
            _studioRenderLabel.pickingMode = PickingMode.Ignore;
            _studioRenderLabel.AddToClassList("onboard-demo-panel-label");
            _studioRenderLabel.style.display = DisplayStyle.None;
            hierarchy.Add(_studioRenderLabel);

            _studioEnvironmentLabel = new Label("ENVIRONMENT");
            _studioEnvironmentLabel.pickingMode = PickingMode.Ignore;
            _studioEnvironmentLabel.AddToClassList("onboard-demo-panel-label");
            _studioEnvironmentLabel.style.display = DisplayStyle.None;
            hierarchy.Add(_studioEnvironmentLabel);

            RegisterCallback<GeometryChangedEvent>(_ => RefreshOverlayElements());
            schedule.Execute(UpdateFrame).Every(16);
        }

        public void SetScene(OnboardingSceneId scene, bool isTouchMode)
        {
            _scene = scene;
            _isTouchMode = isTouchMode;
            _phaseDurationsMs = GetPhaseDurations(scene);
            _animationStartTime = Time.realtimeSinceStartup;
            _currentPhaseIndex = -1;
            UpdateFrame();
            RefreshOverlayElements();
            MarkDirtyRepaint();
        }

        public void SetPlaying(bool isPlaying)
        {
            _isPlaying = isPlaying;
            if (isPlaying)
            {
                _animationStartTime = Time.realtimeSinceStartup;
                _currentPhaseIndex = -1;
                UpdateFrame();
            }

            RefreshOverlayElements();
        }

        private void UpdateFrame()
        {
            if (!_isPlaying || _scene == OnboardingSceneId.None || _phaseDurationsMs.Length == 0)
            {
                return;
            }

            ComputeTimeline(out int phaseIndex, out _, out _);
            if (phaseIndex != _currentPhaseIndex)
            {
                _currentPhaseIndex = phaseIndex;
                OnPhaseChanged?.Invoke(_currentPhaseIndex);
            }

            RefreshOverlayElements();
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            float width = resolvedStyle.width;
            float height = resolvedStyle.height;
            if (width <= 1f || height <= 1f || _scene == OnboardingSceneId.None)
            {
                return;
            }

            ComputeTimeline(out int phaseIndex, out float phaseT, out float loopT);

            var painter = mgc.painter2D;
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;

            DrawStageChrome(painter, width, height, loopT);

            switch (_scene)
            {
                case OnboardingSceneId.Navigate:
                    DrawNavigateScene(painter, width, height, phaseIndex, phaseT, loopT);
                    break;
                case OnboardingSceneId.Select:
                    DrawSelectScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.PartInfo:
                    DrawPartInfoScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Inspect:
                    DrawModeMenuScene(painter, width, height, phaseIndex, phaseT, OnboardingMiniIcon.Inspect,
                        OnboardingMiniIcon.Pin, OnboardingMiniIcon.Isolate, OnboardingMiniIcon.Power);
                    break;
                case OnboardingSceneId.Pins:
                    DrawPinsScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Isolate:
                    DrawIsolateScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Power:
                    DrawPowerScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Analyze:
                    DrawModeMenuScene(painter, width, height, phaseIndex, phaseT, OnboardingMiniIcon.Analyze,
                        OnboardingMiniIcon.Cut, OnboardingMiniIcon.Explode, OnboardingMiniIcon.Filter);
                    break;
                case OnboardingSceneId.Cut:
                    DrawCutScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Explode:
                    DrawExplodeScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Filter:
                    DrawFilterScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Studio:
                    DrawStudioMenuScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.RenderMode:
                    DrawRenderModeScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Environment:
                    DrawEnvironmentScene(painter, width, height, phaseIndex, phaseT);
                    break;
                case OnboardingSceneId.Lighting:
                    DrawLightingScene(painter, width, height, phaseIndex, phaseT);
                    break;
            }
        }

        private void DrawStageChrome(Painter2D painter, float width, float height, float loopT)
        {
            var frame = new Rect(12f, 12f, width - 24f, height - 24f);
            FillRect(painter, frame, FrameFill);
            StrokeRect(painter, frame, FrameStroke, 1.25f);
        }

        // Navigate is staged as:
        // phase 0 = orbit,
        // phase 1 = zoom in then zoom out,
        // phase 2 = pan then reset.
        // For desktop, the mouse hint stays fixed and only its active control changes.
        private void DrawNavigateScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT, float loopT)
        {
            float stageScale = Mathf.Clamp(Mathf.Min(width / 390f, height / 640f), 0.98f, 1.30f);
            Vector2 baseCenter = new Vector2(width * 0.50f, height * 0.34f);
            Rect resetRect = new Rect(width * 0.50f + 120f * stageScale, 44f * stageScale, 64f * stageScale, 64f * stageScale);
            Vector2 orbitLineStart = baseCenter + new Vector2(-90f * stageScale, 188f * stageScale);
            Vector2 orbitLineEnd = baseCenter + new Vector2(82f * stageScale, 188f * stageScale);
            Vector2 wheelCenter = baseCenter + new Vector2(0f, 230f * stageScale);
            Vector2 wheelContact = wheelCenter + new Vector2(-86f * stageScale, -10f * stageScale);
            Vector2 panLineStart = baseCenter + new Vector2(-94f * stageScale, 244f * stageScale);
            Vector2 panLineEnd = panLineStart + new Vector2(188f * stageScale, -16f * stageScale);
            Vector2 resetContact = resetRect.center + new Vector2(resetRect.width * 0.24f, 0f);
            Vector2 mouseHintCenter = baseCenter + new Vector2(-140f * stageScale, 226f * stageScale);

            float orbitAction = phaseIndex == 0 ? GetActionProgress(phaseT, true) : 1f;
            float zoomIn = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.06f, 0.46f), true) : 0f;
            float zoomOut = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.54f, 0.94f), true) : 0f;
            float panSegmentT = phaseIndex == 2 ? Phase01(phaseT, 0.08f, 0.58f) : 0f;
            float panAction = phaseIndex == 2 ? GetActionProgress(panSegmentT, true) : 0f;
            float resetSegmentT = phaseIndex == 2 ? Phase01(phaseT, 0.64f, 1f) : 0f;
            float resetAction = phaseIndex == 2 ? GetActionProgress(resetSegmentT, false) : 0f;

            float droneRotation = phaseIndex == 0 ? Mathf.Lerp(0f, -40f, orbitAction) : Mathf.Lerp(-40f, 0f, resetAction);
            float droneScale = 1.04f;
            if (phaseIndex == 1)
            {
                droneScale = zoomOut > 0f
                    ? Mathf.Lerp(3.15f, 1.02f, zoomOut)
                    : Mathf.Lerp(1.04f, 3.15f, zoomIn);
            }
            else if (phaseIndex == 2)
            {
                droneScale = Mathf.Lerp(1.02f, 1.04f, resetAction);
            }

            Vector2 panOffset = phaseIndex == 2
                ? Vector2.Lerp(new Vector2(0f, 0f), new Vector2(74f * stageScale, -26f * stageScale), panAction)
                : Vector2.zero;
            if (phaseIndex == 2 && resetAction > 0f)
            {
                panOffset = Vector2.Lerp(panOffset, Vector2.zero, resetAction);
            }

            Vector2 droneCenter = baseCenter + panOffset;
            float resetButtonActive = phaseIndex == 2
                ? Mathf.Lerp(0.18f, 1f, resetAction)
                : 0.16f;
            float resetButtonPress = phaseIndex == 2 ? GetPressProgress(resetSegmentT, false) : 0f;
            DrawIconButton(painter, resetRect, OnboardingMiniIcon.Reset, resetButtonActive, resetButtonPress);
            DrawDroneSilhouette(painter, droneCenter, droneScale, droneRotation, 0f, Foreground, SurfaceAlt);

            if (!_isTouchMode)
            {
                if (phaseIndex == 0)
                {
                    float approach = GetApproachProgress(phaseT, true);
                    float press = GetPressProgress(phaseT, true);
                    Vector2 actorStart = resetContact;
                    Vector2 cursor = approach < 0.999f ? Vector2.Lerp(actorStart, orbitLineStart, approach) : Vector2.Lerp(orbitLineStart, orbitLineEnd, orbitAction);
                    if (orbitAction > 0f)
                    {
                        DrawDragTrail(painter, orbitLineStart, cursor, WithAlpha(Accent, 0.12f), 1.6f);
                    }

                    DrawCursorActor(painter, cursor, press, false);
                    DrawMouseHint(painter, mouseHintCenter, MouseHintButton.Right);
                    DrawTargetClickPulse(painter, orbitLineStart, phaseT, false, true, Accent);
                }
                else if (phaseIndex == 1)
                {
                    float moveToWheel = EaseInOut(Phase01(phaseT, 0.06f, 0.26f));
                    float wheelUpPress = GetPressProgress(Phase01(phaseT, 0.06f, 0.46f), true);
                    float wheelDownPress = GetPressProgress(Phase01(phaseT, 0.54f, 0.94f), true);
                    Vector2 cursor = Vector2.Lerp(orbitLineEnd, wheelContact, moveToWheel);
                    DrawCursorActor(painter, cursor, Mathf.Max(wheelUpPress, wheelDownPress), false);
                    // The arrow is the on-screen wheel travel cue for desktop onboarding.
                    // Per the current UX review, the first visible cue points downward and the
                    // second points upward so the hint matches the reviewed mockup exactly.
                    bool isFirstWheelGesture = phaseT < 0.54f;
                    float wheelAmount = isFirstWheelGesture ? zoomIn : zoomOut;
                    bool wheelArrowPointsUp = !isFirstWheelGesture;
                    DrawWheelMouseHint(painter, mouseHintCenter, wheelAmount, wheelArrowPointsUp);
                }
                else
                {
                    if (phaseT < 0.64f)
                    {
                        float moveToPan = EaseInOut(Phase01(phaseT, 0.04f, 0.18f));
                        float press = GetPressProgress(panSegmentT, true);
                        Vector2 cursor = phaseT < 0.18f
                            ? Vector2.Lerp(wheelContact, panLineStart, moveToPan)
                            : Vector2.Lerp(panLineStart, panLineEnd, panAction);
                        if (phaseT >= 0.18f && panAction > 0f)
                        {
                            DrawDragTrail(painter, panLineStart, cursor, WithAlpha(Accent, 0.14f), 1.7f);
                        }

                        DrawCursorActor(painter, cursor, press, true);
                        DrawMouseHint(painter, mouseHintCenter, MouseHintButton.Middle);
                        if (phaseT >= 0.18f)
                        {
                            DrawTargetClickPulse(painter, panLineStart, panSegmentT, false, true, Accent);
                        }
                    }

                    if (resetSegmentT > 0.01f)
                    {
                        DrawActorToTarget(painter, width, height, resetRect.center, resetSegmentT, false, false,
                            new Vector2(resetRect.width * 0.24f, 0f), panLineEnd);
                        DrawTargetClickPulse(painter, resetContact, resetSegmentT, false, false, Accent);
                    }
                }
            }
            else
            {
                Vector2 orbitTouchStart = baseCenter + new Vector2(-76f * stageScale, 188f * stageScale);
                Vector2 orbitTouchEnd = baseCenter + new Vector2(76f * stageScale, 188f * stageScale);
                Vector2 pinchCenter = baseCenter + new Vector2(0f, 228f * stageScale);
                Vector2 pinchStartA = pinchCenter + new Vector2(-24f * stageScale, 0f);
                Vector2 pinchStartB = pinchCenter + new Vector2(24f * stageScale, 0f);
                Vector2 pinchWideA = pinchCenter + new Vector2(-84f * stageScale, 0f);
                Vector2 pinchWideB = pinchCenter + new Vector2(84f * stageScale, 0f);
                Vector2 pinchCloseA = pinchCenter + new Vector2(-34f * stageScale, 0f);
                Vector2 pinchCloseB = pinchCenter + new Vector2(34f * stageScale, 0f);

                if (phaseIndex == 0)
                {
                    float approach = GetApproachProgress(phaseT, true);
                    float press = GetPressProgress(phaseT, true);
                    Vector2 actorStart = resetContact;
                    Vector2 touch = approach < 0.999f ? Vector2.Lerp(actorStart, orbitTouchStart, approach) : Vector2.Lerp(orbitTouchStart, orbitTouchEnd, orbitAction);
                    if (orbitAction > 0f)
                    {
                        DrawDragTrail(painter, orbitTouchStart, touch, WithAlpha(Accent, 0.14f), 1.6f);
                    }

                    DrawTouchActor(painter, touch, press, false);
                    DrawTargetClickPulse(painter, orbitTouchStart, phaseT, false, true, Accent);
                }
                else if (phaseIndex == 1)
                {
                    Vector2 a = zoomOut > 0f ? Vector2.Lerp(pinchWideA, pinchCloseA, zoomOut) : Vector2.Lerp(pinchStartA, pinchWideA, zoomIn);
                    Vector2 b = zoomOut > 0f ? Vector2.Lerp(pinchWideB, pinchCloseB, zoomOut) : Vector2.Lerp(pinchStartB, pinchWideB, zoomIn);
                    if (zoomIn > 0f)
                    {
                        DrawDragTrail(painter, pinchStartA, a, WithAlpha(Accent, 0.12f), 1.6f);
                        DrawDragTrail(painter, pinchStartB, b, WithAlpha(Accent, 0.12f), 1.6f);
                    }
                    else if (zoomOut > 0f)
                    {
                        DrawDragTrail(painter, pinchWideA, a, WithAlpha(Accent, 0.12f), 1.6f);
                        DrawDragTrail(painter, pinchWideB, b, WithAlpha(Accent, 0.12f), 1.6f);
                    }

                    DrawPinchActor(painter, a, b, Mathf.Max(zoomIn, zoomOut));
                }
                else
                {
                    Vector2 lineStartA = baseCenter + new Vector2(-64f * stageScale, 246f * stageScale);
                    Vector2 lineStartB = baseCenter + new Vector2(18f * stageScale, 246f * stageScale);
                    Vector2 lineEndA = lineStartA + new Vector2(152f * stageScale, -16f * stageScale);
                    Vector2 lineEndB = lineStartB + new Vector2(152f * stageScale, -16f * stageScale);
                    if (phaseT < 0.64f)
                    {
                        float press = GetPressProgress(panSegmentT, true);
                        Vector2 touchA = phaseT < 0.18f ? Vector2.Lerp(pinchCloseA, lineStartA, EaseInOut(Phase01(phaseT, 0.04f, 0.16f))) : Vector2.Lerp(lineStartA, lineEndA, panAction);
                        Vector2 touchB = phaseT < 0.18f ? Vector2.Lerp(pinchCloseB, lineStartB, EaseInOut(Phase01(phaseT, 0.04f, 0.16f))) : Vector2.Lerp(lineStartB, lineEndB, panAction);
                        if (phaseT >= 0.18f && panAction > 0f)
                        {
                            DrawDragTrail(painter, lineStartA, touchA, WithAlpha(Accent, 0.14f), 1.6f);
                            DrawDragTrail(painter, lineStartB, touchB, WithAlpha(Accent, 0.14f), 1.6f);
                        }

                        DrawTouchActor(painter, touchA, press, false);
                        DrawTouchActor(painter, touchB, press, false);
                        if (phaseT >= 0.18f)
                        {
                            DrawTargetClickPulse(painter, lineStartA, panSegmentT, false, true, Accent);
                            DrawTargetClickPulse(painter, lineStartB, panSegmentT, false, true, Accent);
                        }
                    }

                    if (resetSegmentT > 0.01f)
                    {
                        DrawActorToTarget(painter, width, height, resetRect.center, resetSegmentT, false, false,
                            new Vector2(resetRect.width * 0.24f, 0f), lineEndB);
                        DrawTargetClickPulse(painter, resetContact, resetSegmentT, false, false, Accent);
                    }
                }
            }
        }

        private void DrawSelectScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 center = new Vector2(width * 0.52f, height * 0.38f);
            Rect neighbor = new Rect(center.x - 132f, center.y - 58f, 66f, 112f);
            Rect[] parentParts =
            {
                new Rect(center.x - 20f, center.y - 54f, 40f, 30f),
                new Rect(center.x + 24f, center.y - 50f, 58f, 34f),
                new Rect(center.x - 12f, center.y - 10f, 54f, 40f),
                new Rect(center.x + 46f, center.y + 4f, 34f, 34f)
            };
            Rect parentGroup = new Rect(center.x - 24f, center.y - 58f, 110f, 104f);
            int activeChildIndex = 2;
            Vector2 contactOffset = new Vector2(14f, 0f);
            Vector2 parentContact = parentParts[1].center + contactOffset;
            Vector2 childContact = parentParts[activeChildIndex].center + contactOffset;
            Vector2 loopContact = GetDefaultActorStart(parentContact);
            Vector2 outsideTarget = loopContact - contactOffset;
            float parentPress = phaseIndex == 0 ? GetPressProgress(phaseT, false) : 0f;
            float childPress = phaseIndex == 1 ? GetPressProgress(phaseT, false) : 0f;

            float childClearSeq = phaseIndex == 2 ? Phase01(phaseT, 0.52f, 0.74f) : 0f;
            float parentClearSeq = phaseIndex == 2 ? Phase01(phaseT, 0.74f, 0.98f) : 0f;
            float backgroundClickSeq = phaseIndex == 2 ? Phase01(phaseT, 0.06f, 0.92f) : 0f;
            float parentMode = phaseIndex == 0
                ? GetActionProgress(phaseT, false)
                : (phaseIndex == 1 ? 1f : 1f - GetActionProgress(parentClearSeq, false));
            float childMode = phaseIndex == 1
                ? GetActionProgress(phaseT, false)
                : (phaseIndex == 2 ? 1f - GetActionProgress(childClearSeq, false) : 0f);
            float pulse = 0.92f + Mathf.Sin(Time.realtimeSinceStartup * 2.2f) * 0.08f;

            DrawPartNode(painter, neighbor, SurfaceAlt, FrameStroke, 1.2f);
            DrawLine(painter, new Vector2(neighbor.xMax, neighbor.center.y), new Vector2(parentGroup.x - 8f, parentGroup.center.y - 6f), WithAlpha(ForegroundMuted, 0.24f), 1.4f);

            StrokeRect(painter, parentGroup, Color.Lerp(WithAlpha(FrameStroke, 0.18f), Accent, parentMode * 0.72f), 1.2f + parentMode * 0.5f);
            for (int i = 0; i < parentParts.Length; i++)
            {
                bool isActiveChild = i == activeChildIndex;
                Rect visualPart = parentParts[i];
                if (phaseIndex == 0 && i == 1)
                {
                    visualPart = ScaleRect(visualPart, 1f + parentPress * 0.06f);
                }
                else if (phaseIndex == 1 && isActiveChild)
                {
                    visualPart = ScaleRect(visualPart, 1f + childPress * 0.06f);
                }

                float childHighlight = isActiveChild ? childMode : 0f;
                float parentHighlight = Mathf.Max(parentMode * (1f - childMode), 0f);
                Color fill = Color.Lerp(Surface, WithAlpha(SelectedFill, SelectedFill.a * 0.88f * pulse), childHighlight);
                fill = Color.Lerp(fill, WithAlpha(SelectedFill, SelectedFill.a * 0.62f), parentHighlight);
                Color stroke = Color.Lerp(FrameStroke, Accent, Mathf.Max(parentHighlight * 0.72f, childHighlight));
                float strokeWidth = 1.15f + Mathf.Max(parentHighlight * 0.35f, childHighlight * 0.55f);
                DrawPartNode(painter, visualPart, fill, stroke, strokeWidth);
            }

            if (parentMode > 0.02f && phaseIndex != 2)
            {
                DrawSelectionHalo(painter, parentGroup.center, 52f, parentMode);
            }

            if (childMode > 0.02f && phaseIndex != 2)
            {
                DrawSelectionHalo(painter, parentParts[activeChildIndex].center, 28f, childMode);
            }

            if (phaseIndex == 0)
            {
                Vector2 target = parentParts[1].center;
                DrawActorToTarget(painter, width, height, target, phaseT, false, false, contactOffset, loopContact);
                DrawTargetClickPulse(painter, parentContact, phaseT, false, false, Accent);
            }
            else if (phaseIndex == 1)
            {
                Vector2 target = parentParts[activeChildIndex].center;
                DrawActorToTarget(painter, width, height, target, phaseT, false, false, contactOffset, parentContact);
                DrawTargetClickPulse(painter, childContact, phaseT, false, false, Accent);
            }
            else
            {
                DrawActorToTarget(painter, width, height, outsideTarget, backgroundClickSeq, false, false, contactOffset, childContact);
                DrawTargetClickPulse(painter, loopContact, backgroundClickSeq, false, false, Accent);
                DrawClickRing(painter, loopContact, EaseOut(Phase01(phaseT, 0.80f, 0.98f)), Accent);
            }
        }

        private void DrawPartInfoScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 center = new Vector2(width * 0.52f, height * 0.34f);
            Rect[] parentParts =
            {
                new Rect(center.x - 20f, center.y - 50f, 40f, 30f),
                new Rect(center.x + 24f, center.y - 46f, 58f, 34f),
                new Rect(center.x - 12f, center.y - 8f, 54f, 40f),
                new Rect(center.x + 46f, center.y + 6f, 34f, 34f)
            };
            Rect parentGroup = new Rect(center.x - 24f, center.y - 54f, 110f, 104f);
            Vector2 loopContact = GetDefaultActorStart(parentParts[1].center);
            float closeSeq = phaseIndex == 2 ? Phase01(phaseT, 0.10f, 0.48f) : 0f;
            float backgroundSeq = phaseIndex == 2 ? Phase01(phaseT, 0.60f, 0.94f) : 0f;
            float partSelected = phaseIndex == 0
                ? GetActionProgress(phaseT, false)
                : (phaseIndex == 1 ? 1f : 1f - GetActionProgress(backgroundSeq, false));
            float tabReveal = phaseIndex == 0
                ? EaseOut(Phase01(phaseT, 0.72f, 0.98f))
                : (phaseIndex == 2 ? 1f - GetActionProgress(Phase01(phaseT, 0.78f, 0.98f), false) : 1f);
            float tabFocus = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.06f, 0.46f), false) : 0f;
            float panelOpen = phaseIndex == 1
                ? EaseOut(Phase01(phaseT, 0.44f, 0.96f))
                : (phaseIndex == 2 ? 1f - GetActionProgress(closeSeq, false) : 0f);
            float peekVisible = Mathf.Max(tabReveal - panelOpen * 1.25f, 0f);
            float pulse = 0.92f + Mathf.Sin(Time.realtimeSinceStartup * 2f) * 0.08f;
            float partPress = phaseIndex == 0 ? GetPressProgress(phaseT, false) : 0f;
            float tabPress = phaseIndex == 1 ? GetPressProgress(Phase01(phaseT, 0.06f, 0.46f), false) : 0f;
            float closePress = phaseIndex == 2 ? GetPressProgress(closeSeq, false) : 0f;

            GetPartInfoRects(width, height, tabReveal, panelOpen, out _, out Rect tab, out Rect panel);
            Rect animatedTab = phaseIndex == 1 ? ScaleRect(tab, 1f + tabPress * 0.04f) : tab;

            StrokeRect(painter, parentGroup, Color.Lerp(WithAlpha(FrameStroke, 0.18f), Accent, partSelected * 0.72f), 1.2f + partSelected * 0.5f);
            for (int i = 0; i < parentParts.Length; i++)
            {
                Rect visualPart = i == 1 && phaseIndex == 0 ? ScaleRect(parentParts[i], 1f + partPress * 0.06f) : parentParts[i];
                Color fill = Color.Lerp(Surface, WithAlpha(SelectedFill, SelectedFill.a * 0.62f * pulse), partSelected);
                Color stroke = Color.Lerp(FrameStroke, Accent, partSelected * 0.74f);
                DrawPartNode(painter, visualPart, fill, stroke, 1.15f + partSelected * 0.4f);
            }

            if (partSelected > 0.02f)
            {
                DrawSelectionHalo(painter, parentGroup.center, 52f, partSelected);
            }

            DrawPartInfoPeek(painter, animatedTab, peekVisible, tabFocus);
            DrawBottomSheetPanel(painter, panel, panelOpen, out Rect closeButton, phaseIndex == 2 ? 0.44f : 0f, closePress);
            if (closeButton.width <= 0f)
            {
                closeButton = new Rect(panel.xMax - 38f, panel.y + 14f, 24f, 24f);
            }

            GetPartInfoRects(width, height, 1f, 1f, out _, out _, out Rect fullyOpenPanel);
            Vector2 fixedCloseContact = new Rect(fullyOpenPanel.xMax - 38f, fullyOpenPanel.y + 14f, 24f, 24f).center;

            if (phaseIndex == 0)
            {
                Vector2 target = parentParts[1].center;
                DrawActorToTarget(painter, width, height, target, phaseT, false, false);
                DrawTargetClickPulse(painter, target, phaseT, false, false, Accent);
            }
            else if (phaseIndex == 1)
            {
                Vector2 target = tab.center + new Vector2(tab.width * 0.20f, 0f);
                DrawActorToTarget(painter, width, height, tab.center, Phase01(phaseT, 0.06f, 0.46f), false, false, new Vector2(tab.width * 0.20f, 0f), parentParts[1].center + new Vector2(14f, 0f));
                DrawTargetClickPulse(painter, target, Phase01(phaseT, 0.06f, 0.46f), false, false, Accent);
            }
            else
            {
                Vector2 tabContact = tab.center + new Vector2(tab.width * 0.20f, 0f);
                if (phaseT < 0.56f)
                {
                    DrawActorToTarget(painter, width, height, fixedCloseContact, closeSeq, false, false, default, tabContact);
                    DrawTargetClickPulse(painter, fixedCloseContact, closeSeq, false, false, Accent);
                }
                else
                {
                    DrawActorToTarget(painter, width, height, loopContact, backgroundSeq, false, false, default, fixedCloseContact);
                    DrawTargetClickPulse(painter, loopContact, backgroundSeq, false, false, Accent);
                }
            }
        }

        private void DrawModeMenuScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT,
            OnboardingMiniIcon mainIcon, OnboardingMiniIcon itemA, OnboardingMiniIcon itemB, OnboardingMiniIcon itemC)
        {
            Rect modeBar = new Rect(width * 0.08f, height * 0.84f, width * 0.84f, 88f);
            Rect leftMode = new Rect(modeBar.x + 12f, modeBar.y + 8f, 96f, 72f);
            Rect centerMode = new Rect(modeBar.center.x - 48f, modeBar.y + 8f, 96f, 72f);
            Rect rightMode = new Rect(modeBar.xMax - 108f, modeBar.y + 8f, 96f, 72f);
            Rect a = new Rect(width * 0.50f - 184f, height * 0.49f - 68f, 118f, 136f);
            Rect b = new Rect(width * 0.50f - 58f, height * 0.49f - 74f, 118f, 148f);
            Rect c = new Rect(width * 0.50f + 68f, height * 0.49f - 68f, 118f, 136f);

            float mainSeq = phaseIndex == 0 ? Phase01(phaseT, 0.04f, 0.44f) : 1f;
            float closeSeq = phaseIndex == 2 ? Phase01(phaseT, 0.58f, 0.98f) : 0f;
            float closeCollapse = phaseIndex == 2 ? EaseInOut(Phase01(closeSeq, 0.50f, 1f)) : 0f;
            float closeAction = phaseIndex == 2 ? GetActionProgress(closeSeq, false) : 0f;
            float mainPress = phaseIndex == 0
                ? GetPressReleaseProgress(mainSeq, false)
                : (phaseIndex == 2 ? GetPressReleaseProgress(closeSeq, false) : 0f);
            float mainAction = phaseIndex == 0 ? GetActionProgress(mainSeq, false) : 1f;
            float mainHover = phaseIndex == 0
                ? EaseOut(Phase01(mainSeq, 0.20f, 0.54f))
                : (phaseIndex == 2 ? EaseOut(Phase01(closeSeq, 0.18f, 0.54f)) : 0f);
            float submenuReveal = phaseIndex == 0
                ? EaseOut(Phase01(phaseT, 0.42f, 0.76f))
                : (phaseIndex == 2 ? 1f - closeCollapse : 1f);
            float activeModeStrength = phaseIndex == 0
                ? Mathf.Lerp(mainAction, 0.72f, submenuReveal)
                : (phaseIndex == 2 ? Mathf.Lerp(0.72f, 0f, closeCollapse) : 0.72f);

            float aSeq = phaseIndex == 0 ? Phase01(phaseT, 0.56f, 1f) : 0f;
            float bSeq = phaseIndex == 1 ? Phase01(phaseT, 0.12f, 0.96f) : 0f;
            float cSeq = phaseIndex == 2 ? Phase01(phaseT, 0.10f, 0.52f) : 0f;
            float aRelease = phaseIndex == 1 ? EaseInOut(Phase01(bSeq, 0.44f, 0.94f)) : (phaseIndex > 1 ? 1f : 0f);
            float bRelease = phaseIndex == 2 ? EaseInOut(Phase01(cSeq, 0.44f, 0.94f)) : 0f;

            float aHover = phaseIndex == 0
                ? EaseOut(Phase01(aSeq, 0.18f, 0.54f))
                : (phaseIndex == 1 ? 0.24f * (1f - aRelease) : 0f);
            float aPress = phaseIndex == 0 ? GetPressReleaseProgress(aSeq, false) : 0f;
            float bHover = phaseIndex == 1
                ? EaseOut(Phase01(bSeq, 0.18f, 0.54f))
                : (phaseIndex == 2 ? 0.24f * (1f - bRelease) : 0f);
            float bPress = phaseIndex == 1 ? GetPressReleaseProgress(bSeq, false) : 0f;
            float cHover = phaseIndex == 2
                ? Mathf.Max(EaseOut(Phase01(cSeq, 0.18f, 0.54f)), 0.18f * (1f - closeCollapse))
                : 0f;
            float cPress = phaseIndex == 2 ? GetPressReleaseProgress(cSeq, false) : 0f;
            float aClicked = phaseIndex == 0 ? GetActionProgress(aSeq, false) : 1f;
            float bClicked = phaseIndex >= 1 ? (phaseIndex == 1 ? GetActionProgress(bSeq, false) : 1f) : 0f;
            float cClicked = phaseIndex == 2 ? GetActionProgress(cSeq, false) : 0f;
            float aActive = aClicked * (1f - aRelease);
            float bActive = bClicked * (1f - bRelease);
            float cActive = cClicked * (1f - closeCollapse);

            DrawModeBar(painter, modeBar);
            DrawModeBarButton(painter, leftMode, OnboardingMiniIcon.Inspect, mainIcon == OnboardingMiniIcon.Inspect ? activeModeStrength : 0.08f, mainIcon == OnboardingMiniIcon.Inspect ? mainHover : 0f, mainIcon == OnboardingMiniIcon.Inspect ? mainPress : 0f);
            DrawModeBarButton(painter, centerMode, OnboardingMiniIcon.Analyze, mainIcon == OnboardingMiniIcon.Analyze ? activeModeStrength : 0.08f, mainIcon == OnboardingMiniIcon.Analyze ? mainHover : 0f, mainIcon == OnboardingMiniIcon.Analyze ? mainPress : 0f);
            DrawModeBarButton(painter, rightMode, OnboardingMiniIcon.Studio, mainIcon == OnboardingMiniIcon.Studio ? activeModeStrength : 0.08f, mainIcon == OnboardingMiniIcon.Studio ? mainHover : 0f, mainIcon == OnboardingMiniIcon.Studio ? mainPress : 0f);

            Rect activeModeRect = mainIcon == OnboardingMiniIcon.Inspect
                ? leftMode
                : (mainIcon == OnboardingMiniIcon.Analyze ? centerMode : rightMode);
            Vector2 activeContactOffset = new Vector2(18f, 0f);
            Vector2 activeContact = activeModeRect.center + activeContactOffset;
            Vector2 aContact = a.center + new Vector2(18f, 0f);
            Vector2 bContact = b.center + new Vector2(18f, 0f);

            DrawRadialConnector(painter, activeModeRect.center, a.center, submenuReveal);
            DrawRadialConnector(painter, activeModeRect.center, b.center, submenuReveal);
            DrawRadialConnector(painter, activeModeRect.center, c.center, submenuReveal);
            DrawMenuOption(painter, a, itemA, submenuReveal, aHover, aPress, aActive);
            DrawMenuOption(painter, b, itemB, submenuReveal, bHover, bPress, bActive);
            DrawMenuOption(painter, c, itemC, submenuReveal, cHover, cPress, cActive);

            if (phaseIndex == 0 && phaseT < 0.60f)
            {
                DrawActorToTarget(painter, width, height, activeModeRect.center, mainSeq, false, false, activeContactOffset);
                DrawTargetClickPulse(painter, activeContact, mainSeq, false, false, Accent);
            }
            else if (phaseIndex == 2 && phaseT >= 0.58f)
            {
                DrawActorToTarget(painter, width, height, activeModeRect.center, closeSeq, false, false, activeContactOffset, c.center + new Vector2(18f, 0f));
                DrawTargetClickPulse(painter, activeContact, closeSeq, false, false, Accent);
            }
            else
            {
                Rect currentRect = phaseIndex == 0 ? a : (phaseIndex == 1 ? b : c);
                float localT = phaseIndex == 0 ? aSeq : (phaseIndex == 1 ? bSeq : cSeq);
                Vector2 contactOffset = new Vector2(18f, 0f);
                Vector2 pulseTarget = currentRect.center + contactOffset;
                Vector2 startContact = phaseIndex == 0 ? activeContact : (phaseIndex == 1 ? aContact : bContact);
                DrawActorToTarget(painter, width, height, currentRect.center, localT, false, false, contactOffset, startContact);
                DrawTargetClickPulse(painter, pulseTarget, localT, false, false, Accent);
            }
        }

        private void DrawStudioMenuScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect renderCard = new Rect(width * 0.50f - 118f, height * 0.20f, 236f, 90f);
            Rect environmentCard = new Rect(width * 0.50f - 118f, height * 0.39f, 236f, 90f);
            Rect sliderA = new Rect(width * 0.50f - 122f, height * 0.61f, 244f, 24f);
            Rect sliderB = new Rect(width * 0.50f - 122f, height * 0.72f, 244f, 24f);
            Rect sliderC = new Rect(width * 0.50f - 122f, height * 0.83f, 244f, 24f);
            Rect modeBar = new Rect(width * 0.08f, height * 0.88f, width * 0.84f, 72f);
            Rect leftMode = new Rect(modeBar.x + 14f, modeBar.y + 8f, 90f, 56f);
            Rect centerMode = new Rect(modeBar.center.x - 45f, modeBar.y + 8f, 90f, 56f);
            Rect rightMode = new Rect(modeBar.xMax - 104f, modeBar.y + 8f, 90f, 56f);

            float menuSeq = phaseIndex == 0 ? Phase01(phaseT, 0.04f, 0.34f) : 1f;
            float renderSeq = phaseIndex == 0 ? Phase01(phaseT, 0.42f, 0.96f) : 0f;
            float renderHover = phaseIndex == 0 ? EaseOut(Phase01(renderSeq, 0.18f, 0.54f)) : 0.12f;
            float renderPress = phaseIndex == 0 ? GetPressProgress(renderSeq, false) : 0f;
            float renderActive = phaseIndex == 0 ? GetActionProgress(renderSeq, false) : 0.18f;
            float environmentHover = phaseIndex == 1 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0.12f;
            float environmentPress = phaseIndex == 1 ? GetPressProgress(phaseT, false) : 0f;
            float environmentActive = phaseIndex == 1 ? GetActionProgress(phaseT, false) : 0.18f;
            float sliderFocus = phaseIndex == 2 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0f;
            float sliderPress = phaseIndex == 2 ? GetPressProgress(phaseT, true) : 0f;
            float studioMenuActive = phaseIndex == 0
                ? Mathf.Lerp(GetActionProgress(menuSeq, false), 0.78f, renderActive)
                : 0.78f;
            float studioMenuPress = phaseIndex == 0 ? GetPressProgress(menuSeq, false) : 0f;
            float panelReveal = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.28f, 0.42f)) : 1f;

            DrawModeBar(painter, modeBar);
            DrawModeBarButton(painter, leftMode, OnboardingMiniIcon.Inspect, 0.10f, 0f, 0f);
            DrawModeBarButton(painter, centerMode, OnboardingMiniIcon.Analyze, 0.10f, 0f, 0f);
            DrawModeBarButton(painter, rightMode, OnboardingMiniIcon.Studio, studioMenuActive, 0f, studioMenuPress);

            if (panelReveal > 0.01f)
            {
                Rect renderVisual = ScaleRect(renderCard, Mathf.Lerp(0.86f, 1f, panelReveal));
                Rect environmentVisual = ScaleRect(environmentCard, Mathf.Lerp(0.86f, 1f, panelReveal));
                Rect sliderVisualA = ScaleRect(sliderA, Mathf.Lerp(0.90f, 1f, panelReveal));
                Rect sliderVisualB = ScaleRect(sliderB, Mathf.Lerp(0.90f, 1f, panelReveal));
                Rect sliderVisualC = ScaleRect(sliderC, Mathf.Lerp(0.90f, 1f, panelReveal));
                DrawStudioPanelCard(painter, renderVisual, OnboardingMiniIcon.Render, renderActive * panelReveal, renderHover * panelReveal, renderPress * panelReveal, true);
                DrawStudioPanelCard(painter, environmentVisual, OnboardingMiniIcon.Environment, environmentActive * panelReveal, environmentHover * panelReveal, environmentPress * panelReveal, false);
                DrawSlider(painter, sliderVisualA, phaseIndex == 2 ? Mathf.Lerp(0.24f, 0.78f, GetActionProgress(phaseT, true)) : 0.24f, Accent, true, sliderFocus * panelReveal, sliderPress * panelReveal);
                DrawSlider(painter, sliderVisualB, phaseIndex == 2 ? Mathf.Lerp(0.56f, 0.30f, GetActionProgress(phaseT, true)) : 0.56f, Accent, true, sliderFocus * 0.74f * panelReveal, sliderPress * 0.82f * panelReveal);
                DrawSlider(painter, sliderVisualC, phaseIndex == 2 ? Mathf.Lerp(0.38f, 0.66f, GetActionProgress(phaseT, true)) : 0.38f, Accent, true, sliderFocus * 0.60f * panelReveal, sliderPress * 0.66f * panelReveal);
            }

            Vector2 studioContact = rightMode.center + new Vector2(rightMode.width * 0.16f, 0f);
            Vector2 renderContact = renderCard.center + new Vector2(renderCard.width * 0.30f, 0f);
            Vector2 environmentContact = environmentCard.center + new Vector2(environmentCard.width * 0.30f, 0f);
            if (phaseIndex == 0 && phaseT < 0.40f)
            {
                DrawActorToTarget(painter, width, height, rightMode.center, menuSeq, false, false, new Vector2(rightMode.width * 0.16f, 0f));
                DrawTargetClickPulse(painter, studioContact, menuSeq, false, false, Accent);
            }
            else if (phaseIndex == 0)
            {
                DrawActorToTarget(painter, width, height, renderCard.center, renderSeq, false, false, new Vector2(renderCard.width * 0.30f, 0f), studioContact);
                DrawTargetClickPulse(painter, renderContact, renderSeq, false, false, Accent);
            }
            else if (phaseIndex == 1)
            {
                DrawActorToTarget(painter, width, height, environmentCard.center, phaseT, false, false, new Vector2(environmentCard.width * 0.30f, 0f), renderContact);
                DrawTargetClickPulse(painter, environmentContact, phaseT, false, false, Accent);
            }
            else
            {
                Vector2 sliderStart = GetSliderThumb(sliderA, 0.24f);
                Vector2 sliderEnd = GetSliderThumb(sliderA, 0.78f);
                DrawActorDragBetween(painter, sliderStart, sliderEnd, phaseT, false, environmentContact);
                DrawTargetClickPulse(painter, sliderStart, phaseT, false, true, Accent);
            }
        }

        private void DrawPinsScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 droneCenter = new Vector2(width * 0.50f, height * 0.30f);
            float droneScale = 1.48f;
            Vector2[] rotors = GetDroneRotorCenters(droneCenter, droneScale, 0f);
            Vector2[] pinPoints =
            {
                rotors[0] + new Vector2(0f, -18f),
                rotors[1] + new Vector2(0f, -18f),
                droneCenter + new Vector2(0f, 28f)
            };
            Rect button = new Rect(width * 0.50f - 42f, height * 0.76f, 84f, 84f);

            float buttonHover = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.30f, 0.58f)) : 0.42f;
            float buttonPress = phaseIndex == 0 ? GetPressProgress(phaseT, false) : 0f;
            float pinsOn = phaseIndex == 0 ? GetActionProgress(phaseT, false) : 1f;
            float hotspotSelectSeq = phaseIndex == 1 ? Phase01(phaseT, 0.08f, 0.56f) : 0f;
            float hotspotClearSeq = phaseIndex == 1 ? Phase01(phaseT, 0.66f, 1f) : 0f;
            float hotspotSelect = phaseIndex == 1
                ? GetActionProgress(hotspotSelectSeq, false) * (1f - GetActionProgress(hotspotClearSeq, false))
                : 0f;

            DrawDroneSilhouette(painter, droneCenter, droneScale, 0f, 0f, Foreground, Color.Lerp(SurfaceAlt, AccentGlow, pinsOn * 0.12f));

            for (int i = 0; i < rotors.Length; i++)
            {
                float highlight = hotspotSelect;
                if (highlight > 0.01f)
                {
                    FillCircle(painter, rotors[i], 10f * droneScale, WithAlpha(Warning, 0.10f + highlight * 0.22f));
                    StrokeCircle(painter, rotors[i], 10f * droneScale, WithAlpha(Warning, 0.48f + highlight * 0.20f), 1.8f);
                }
            }

            DrawPinMarker(painter, pinPoints[0], pinsOn, Accent);
            DrawPinMarker(painter, pinPoints[2], pinsOn * 0.90f, Accent);
            DrawPinMarker(painter, pinPoints[1], pinsOn, Color.Lerp(Accent, Warning, hotspotSelect));
            if (hotspotSelect > 0.01f)
            {
                StrokeCircle(painter, pinPoints[1], 16f + hotspotSelect * 6f, WithAlpha(Warning, 0.34f + hotspotSelect * 0.26f), 1.6f);
            }

            DrawStandaloneActionButton(painter, button, OnboardingMiniIcon.Pin, pinsOn, buttonHover, buttonPress);

            if (phaseIndex == 0)
            {
                Vector2 contactOffset = new Vector2(button.width * 0.34f, 0f);
                Vector2 pulseTarget = button.center + contactOffset;
                DrawActorToTarget(painter, width, height, button.center, phaseT, false, false, contactOffset);
                DrawTargetClickPulse(painter, pulseTarget, phaseT, false, false, Accent);
            }
            else if (phaseT < 0.62f)
            {
                Vector2 contactOffset = new Vector2(16f, 0f);
                Vector2 pulseTarget = pinPoints[1] + contactOffset;
                DrawActorToTarget(painter, width, height, pinPoints[1], hotspotSelectSeq, false, false, contactOffset, button.center + new Vector2(button.width * 0.34f, 0f));
                DrawTargetClickPulse(painter, pulseTarget, hotspotSelectSeq, false, false, Accent);
            }
            else
            {
                Vector2 contactOffset = new Vector2(16f, 0f);
                Vector2 buttonContact = button.center + new Vector2(button.width * 0.34f, 0f);
                Vector2 loopContact = GetDefaultActorStart(buttonContact);
                Vector2 resetTarget = loopContact - contactOffset;
                DrawActorToTarget(painter, width, height, resetTarget, hotspotClearSeq, false, false, contactOffset, pinPoints[1] + contactOffset);
                DrawTargetClickPulse(painter, loopContact, hotspotClearSeq, false, false, Accent);
                DrawClickRing(painter, loopContact, EaseOut(Phase01(hotspotClearSeq, 0.78f, 0.98f)), Accent);
            }
        }

        private void DrawIsolateScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 center = new Vector2(width * 0.52f, height * 0.33f);
            GetSelectionAssemblyRects(center, out Rect neighbor, out Rect[] parentParts);
            Rect button = new Rect(width * 0.50f - 42f, height * 0.78f, 84f, 84f);
            int activeChild = 2;

            float isolateOn = phaseIndex == 0 ? GetActionProgress(phaseT, false) : 1f;
            float childSeq = phaseIndex == 1 ? Phase01(phaseT, 0.08f, 0.48f) : 0f;
            float backSeq = phaseIndex == 1 ? Phase01(phaseT, 0.54f, 0.74f) : 0f;
            float offSeq = phaseIndex == 1 ? Phase01(phaseT, 0.74f, 0.94f) : 0f;
            float childIsolate = phaseIndex == 1 ? GetDoubleClickActionProgress(childSeq) : 0f;
            float backAction = phaseIndex == 1 ? GetDoubleClickActionProgress(backSeq) : 0f;
            float offAction = phaseIndex == 1 ? GetActionProgress(offSeq, false) : 0f;

            float neighborVisible = phaseIndex == 0 ? 1f - isolateOn : Mathf.Lerp(0f, 1f, offAction);
            float parentHighlight = phaseIndex == 0
                ? 1f
                : Mathf.Lerp(Mathf.Lerp(1f, 0f, childIsolate), 1f, backAction) * (1f - offAction);
            float childHighlight = phaseIndex == 1 ? childIsolate * (1f - backAction) * (1f - offAction) : 0f;
            float nonSelectedVisible = phaseIndex == 1 ? Mathf.Lerp(1f - childIsolate, 1f, backAction) : 1f;
            float buttonActive = phaseIndex == 0 ? isolateOn : 1f - offAction;
            float buttonHover = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.30f, 0.58f)) : 0.40f * (1f - offAction);
            float buttonPress = phaseIndex == 0 ? GetPressProgress(phaseT, false) : (phaseIndex == 1 ? GetPressProgress(offSeq, false) : 0f);

            DrawSelectionAssembly(painter, neighbor, parentParts, neighborVisible, nonSelectedVisible, parentHighlight, activeChild, childHighlight, phaseIndex == 0 || phaseT < 0.52f);
            DrawStandaloneActionButton(painter, button, OnboardingMiniIcon.Isolate, buttonActive, buttonHover, buttonPress);

            if (phaseIndex == 0)
            {
                Vector2 contactOffset = new Vector2(button.width * 0.34f, 0f);
                Vector2 pulseTarget = button.center + contactOffset;
                DrawActorToTarget(painter, width, height, button.center, phaseT, false, false, contactOffset);
                DrawTargetClickPulse(painter, pulseTarget, phaseT, false, false, Accent);
            }
            else if (phaseT < 0.52f)
            {
                Vector2 target = parentParts[activeChild].center;
                Vector2 contactOffset = new Vector2(16f, 0f);
                Vector2 pulseTarget = target + contactOffset;
                DrawActorToTarget(painter, width, height, target, childSeq, true, false, contactOffset, button.center + new Vector2(button.width * 0.34f, 0f));
                DrawTargetClickPulse(painter, pulseTarget, childSeq, true, false, Accent);
                float secondClickAccent = EaseOut(Phase01(childSeq, 0.82f, 0.98f));
                DrawClickRing(painter, pulseTarget, secondClickAccent, Warning);
            }
            else if (phaseT < 0.74f)
            {
                Vector2 target = new Vector2(width * 0.79f, height * 0.16f);
                Vector2 contactOffset = new Vector2(14f, 0f);
                Vector2 pulseTarget = target + contactOffset;
                DrawActorToTarget(painter, width, height, target, backSeq, true, false, contactOffset, parentParts[activeChild].center + new Vector2(16f, 0f));
                DrawTargetClickPulse(painter, pulseTarget, backSeq, true, false, Accent);
                float secondClickAccent = EaseOut(Phase01(backSeq, 0.82f, 0.98f));
                DrawClickRing(painter, pulseTarget, secondClickAccent, Warning);
            }
            else
            {
                Vector2 contactOffset = new Vector2(button.width * 0.34f, 0f);
                Vector2 pulseTarget = button.center + contactOffset;
                DrawActorToTarget(painter, width, height, button.center, offSeq, false, false, contactOffset, new Vector2(width * 0.79f, height * 0.16f) + new Vector2(14f, 0f));
                DrawTargetClickPulse(painter, pulseTarget, offSeq, false, false, Accent);
            }
        }

        private void DrawPowerScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 droneCenter = new Vector2(width * 0.50f, height * 0.30f);
            float droneScale = 1.48f;
            Rect button = new Rect(width * 0.50f - 42f, height * 0.72f, 84f, 84f);
            Rect slider = new Rect(width * 0.50f - 120f, height * 0.89f, 240f, 28f);
            Vector2 dragStart = GetSliderThumb(slider, 0f);
            float dragSeq = phaseIndex == 1 ? Phase01(phaseT, 0.20f, 0.72f) : 0f;
            float offSeq = phaseIndex == 1 ? Phase01(phaseT, 0.78f, 1f) : 0f;
            float sliderAmount = phaseIndex == 1 ? GetActionProgress(dragSeq, true) : 0f;
            Vector2 dragEnd = GetSliderThumb(slider, sliderAmount);

            float buttonHover = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.30f, 0.58f)) : 0.36f;
            float buttonPress = phaseIndex == 0
                ? GetPressProgress(phaseT, false)
                : GetPressProgress(offSeq, false);
            float powerOn = phaseIndex == 0
                ? GetActionProgress(phaseT, false)
                : 1f - GetActionProgress(offSeq, false);
            float sliderHover = phaseIndex == 1 ? EaseOut(Phase01(phaseT, 0.22f, 0.42f)) : 0f;
            float sliderPress = phaseIndex == 1 ? GetPressProgress(dragSeq, true) : 0f;
            float thermalLayout = phaseIndex == 1
                ? EaseOut(Phase01(phaseT, 0.54f, 0.82f)) * powerOn
                : 0f;
            float rotorSpin = powerOn > 0.06f ? Time.realtimeSinceStartup * (140f + 760f * Mathf.Max(sliderAmount, powerOn * 0.24f)) : 0f;
            Color poweredFill = Color.Lerp(SurfaceAlt, new Color(0.20f, 0.42f, 0.86f, 0.56f), powerOn);

            DrawDroneSilhouette(painter, droneCenter, droneScale, 0f, rotorSpin, Foreground, poweredFill);
            if (powerOn > 0.01f)
            {
                FillCircle(painter, droneCenter, 34f * droneScale, WithAlpha(AccentGlow, 0.08f + powerOn * 0.10f));
            }

            DrawPowerHeatLayout(painter, droneCenter, droneScale, thermalLayout);
            DrawStandaloneActionButton(painter, button, OnboardingMiniIcon.Power, powerOn, buttonHover, buttonPress);
            DrawSlider(painter, slider, sliderAmount, Accent, powerOn > 0.01f, sliderHover, sliderPress);

            if (phaseIndex == 0)
            {
                Vector2 contactOffset = new Vector2(button.width * 0.34f, 0f);
                Vector2 pulseTarget = button.center + contactOffset;
                DrawActorToTarget(painter, width, height, button.center, phaseT, false, false, contactOffset);
                DrawTargetClickPulse(painter, pulseTarget, phaseT, false, false, Accent);
            }
            else if (phaseT < 0.78f)
            {
                Vector2 buttonContact = button.center + new Vector2(button.width * 0.34f, 0f);
                float moveToSlider = EaseInOut(Phase01(phaseT, 0.06f, 0.20f));
                float press = GetPressProgress(dragSeq, true);
                if (_isTouchMode)
                {
                    Vector2 touch = phaseT < 0.20f ? Vector2.Lerp(buttonContact, dragStart, moveToSlider) : Vector2.Lerp(dragStart, dragEnd, sliderAmount);
                    if (phaseT >= 0.20f && sliderAmount > 0f)
                    {
                        DrawDragTrail(painter, dragStart, touch, WithAlpha(Accent, 0.14f), 1.6f);
                    }

                    DrawTouchActor(painter, touch, press, false);
                }
                else
                {
                    Vector2 cursor = phaseT < 0.20f ? Vector2.Lerp(buttonContact, dragStart, moveToSlider) : Vector2.Lerp(dragStart, dragEnd, sliderAmount);
                    if (phaseT >= 0.20f && sliderAmount > 0f)
                    {
                        DrawDragTrail(painter, dragStart, cursor, WithAlpha(Accent, 0.14f), 1.8f);
                    }

                    DrawCursorActor(painter, cursor, press, false);
                }

                DrawTargetClickPulse(painter, dragStart, dragSeq, false, true, Accent);
            }
            else
            {
                Vector2 contactOffset = new Vector2(button.width * 0.34f, 0f);
                Vector2 buttonContact = button.center + contactOffset;
                DrawActorToTarget(painter, width, height, button.center, offSeq, false, false, contactOffset, dragEnd);
                DrawTargetClickPulse(painter, buttonContact, offSeq, false, false, Accent);
            }
        }

        private void DrawCutScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect outer = new Rect(width * 0.50f - 86f, height * 0.23f, 172f, 228f);
            Rect inner = new Rect(width * 0.50f - 48f, outer.center.y - 64f, 96f, 128f);
            float buttonWidth = 66f;
            float buttonGap = 8f;
            float rowWidth = buttonWidth * 5f + buttonGap * 4f;
            float buttonsY = height * 0.71f;
            float buttonsX = width * 0.50f - rowWidth * 0.5f;
            Rect btnX = new Rect(buttonsX, buttonsY, buttonWidth, 54f);
            Rect btnY = new Rect(buttonsX + (buttonWidth + buttonGap) * 1f, buttonsY, buttonWidth, 54f);
            Rect btnZ = new Rect(buttonsX + (buttonWidth + buttonGap) * 2f, buttonsY, buttonWidth, 54f);
            Rect btnInvert = new Rect(buttonsX + (buttonWidth + buttonGap) * 3f, buttonsY, buttonWidth, 54f);
            Rect btnAngle = new Rect(buttonsX + (buttonWidth + buttonGap) * 4f, buttonsY, buttonWidth, 54f);
            Rect slider = new Rect(width * 0.50f - 132f, height * 0.88f, 264f, 28f);

            Vector2 outerCenter = outer.center;
            float planeX = outerCenter.x;
            float basePlaneY = outer.y + 94f;

            float xSeq = phaseIndex == 0 ? Phase01(phaseT, 0.04f, 0.34f) : 1f;
            float xPress = phaseIndex == 0 ? GetPressProgress(xSeq, false) : 0f;
            float xActive = phaseIndex == 0 ? GetActionProgress(xSeq, false) : 1f;
            float xTrace = phaseIndex == 0 ? EaseOut(Phase01(xSeq, 0.58f, 0.88f)) : 1f;
            float xFill = phaseIndex == 0 ? EaseOut(Phase01(xSeq, 0.74f, 1f)) : 1f;

            float sliderSeq = phaseIndex == 0 ? Phase01(phaseT, 0.36f, 1f) : 0f;
            float sliderOut = 0f;
            float sliderBack = 0f;
            float sliderValue = phaseIndex == 0
                ? EvaluateRoundTripDrag(sliderSeq, 0.50f, 0.88f, 0.50f, out sliderOut, out sliderBack)
                : 0.50f;
            float lineLift = phaseIndex == 0
                ? (sliderSeq < 0.56f ? sliderOut : 1f - sliderBack)
                : 0f;
            float planeY = Mathf.Lerp(basePlaneY, outer.y + 28f, lineLift);

            float ySeq = phaseIndex == 1 ? Phase01(phaseT, 0.12f, 0.96f) : 0f;
            float yPress = phaseIndex == 1 ? GetPressProgress(ySeq, false) : 0f;
            float yActive = phaseIndex == 1 ? GetActionProgress(ySeq, false) : (phaseIndex > 1 ? 1f : 0f);
            float yTrace = phaseIndex == 1 ? EaseOut(Phase01(ySeq, 0.58f, 0.88f)) : (phaseIndex > 1 ? 1f : 0f);
            float yFill = phaseIndex == 1 ? EaseOut(Phase01(ySeq, 0.74f, 1f)) : (phaseIndex > 1 ? 1f : 0f);

            float angleSeq = phaseIndex == 2 ? Phase01(phaseT, 0.08f, 0.62f) : 0f;
            float invertSeq = phaseIndex == 2 ? Phase01(phaseT, 0.70f, 1f) : 0f;
            float anglePress = phaseIndex == 2 ? GetPressProgress(angleSeq, false) : 0f;
            float angleActive = phaseIndex == 2 ? GetActionProgress(angleSeq, false) : 0f;
            float invertPress = phaseIndex == 2 ? GetPressProgress(invertSeq, false) : 0f;
            float invertActive = phaseIndex == 2 ? GetActionProgress(invertSeq, false) : 0f;
            bool diagonalInverted = invertActive > 0.54f;

            DrawSlider(painter, slider, sliderValue, Accent, true, phaseIndex == 0 ? 0.42f : 0f, phaseIndex == 0 ? Mathf.Max(GetPressProgress(Phase01(sliderSeq, 0.06f, 0.56f), true), GetPressProgress(Phase01(sliderSeq, 0.56f, 1f), true)) : 0f);

            if (phaseIndex == 0)
            {
                float xState = Mathf.Max(xActive, lineLift);
                if (xState <= 0.01f)
                {
                    FillRect(painter, outer, WithAlpha(AccentGlow, 0.10f));
                    StrokeRect(painter, outer, WithAlpha(Accent, 0.42f), 1.2f);
                    FillRect(painter, inner, WithAlpha(AccentGlow, 0.06f));
                    StrokeRect(painter, inner, WithAlpha(Accent, 0.34f), 1.1f);
                }
                else
                {
                    DrawNestedHorizontalCutPair(
                        painter,
                        outer,
                        inner,
                        planeY,
                        Mathf.Max(xTrace, lineLift),
                        Mathf.Max(xFill, lineLift),
                        WithAlpha(AccentGlow, 0.18f + Mathf.Max(xFill, lineLift) * 0.16f),
                        WithAlpha(Accent, 0.46f + Mathf.Max(xFill, lineLift) * 0.28f));
                }
            }
            else if (phaseIndex == 1)
            {
                DrawNestedHorizontalCutPair(
                    painter,
                    outer,
                    inner,
                    basePlaneY,
                    1f,
                    1f,
                    WithAlpha(AccentGlow, 0.28f),
                    WithAlpha(Accent, 0.74f));

                DrawNestedQuarterCutPair(
                    painter,
                    outer,
                    inner,
                    planeX,
                    basePlaneY,
                    true,
                    true,
                    0f,
                    yTrace,
                    yFill,
                    WithAlpha(AccentGlow, 0.24f + yFill * 0.14f),
                    WithAlpha(Accent, 0.48f + yFill * 0.22f));
            }
            else
            {
                float oldPlanesFade = 1f - EaseOut(Phase01(angleActive, 0f, 0.28f));
                DrawNestedQuarterCutPair(
                    painter,
                    outer,
                    inner,
                    planeX,
                    basePlaneY,
                    true,
                    true,
                    oldPlanesFade,
                    oldPlanesFade,
                    oldPlanesFade,
                    WithAlpha(AccentGlow, 0.24f * oldPlanesFade),
                    WithAlpha(Accent, 0.50f * oldPlanesFade));

                DrawSharedDiagonalCutPair(
                    painter,
                    outer,
                    inner,
                    angleActive,
                    WithAlpha(Blueprint, 0.16f + angleActive * 0.20f),
                    Color.Lerp(Accent, Warning, angleActive * 0.55f),
                    diagonalInverted);
            }

            DrawAxisButton(painter, btnX, CutButtonKind.X, phaseIndex == 0 ? xActive : 0.92f, phaseIndex == 0 ? EaseOut(Phase01(xSeq, 0.18f, 0.54f)) : 0f, xPress);
            DrawAxisButton(painter, btnY, CutButtonKind.Y, phaseIndex > 1 ? 0.92f : (phaseIndex == 1 ? Mathf.Lerp(0.14f, 0.92f, yActive) : 0.14f), phaseIndex == 1 ? EaseOut(Phase01(ySeq, 0.18f, 0.54f)) : 0f, yPress);
            DrawAxisButton(painter, btnZ, CutButtonKind.Z, 0.10f, 0f, 0f);
            DrawAxisButton(painter, btnInvert, CutButtonKind.Invert, phaseIndex == 2 ? Mathf.Max(0.18f, invertActive) : 0.12f, phaseIndex == 2 ? EaseOut(Phase01(invertSeq, 0.18f, 0.54f)) : 0f, invertPress);
            DrawAxisButton(painter, btnAngle, CutButtonKind.Angle, phaseIndex == 2 ? Mathf.Max(0.22f, angleActive) : 0.12f, phaseIndex == 2 ? EaseOut(Phase01(angleSeq, 0.18f, 0.54f)) : 0f, anglePress);

            Vector2 buttonOffset = new Vector2(12f, 0f);
            if (phaseIndex == 0 && phaseT < 0.36f)
            {
                Vector2 pulseTarget = btnX.center + buttonOffset;
                DrawActorToTarget(painter, width, height, btnX.center, xSeq, false, false, buttonOffset);
                DrawTargetClickPulse(painter, pulseTarget, xSeq, false, false, Accent);
            }
            else if (phaseIndex == 0)
            {
                Vector2 sliderStart = GetSliderThumb(slider, 0.50f);
                Vector2 sliderPeak = GetSliderThumb(slider, 0.88f);
                Vector2 sliderEnd = GetSliderThumb(slider, 0.50f);
                DrawActorTwoSegmentDrag(painter, sliderStart, sliderPeak, sliderEnd, sliderSeq, btnX.center + buttonOffset);
                DrawTargetClickPulse(painter, sliderStart, Phase01(sliderSeq, 0.06f, 0.24f), false, false, Accent);
            }
            else if (phaseIndex == 1)
            {
                Vector2 startContact = GetSliderThumb(slider, 0.50f);
                DrawActorToTarget(painter, width, height, btnY.center, ySeq, false, false, buttonOffset, startContact);
                DrawTargetClickPulse(painter, btnY.center + buttonOffset, ySeq, false, false, Accent);
            }
            else if (phaseT < 0.68f)
            {
                DrawActorToTarget(painter, width, height, btnAngle.center, angleSeq, false, false, buttonOffset, btnY.center + buttonOffset);
                DrawTargetClickPulse(painter, btnAngle.center + buttonOffset, angleSeq, false, false, Accent);
            }
            else
            {
                DrawActorToTarget(painter, width, height, btnInvert.center, invertSeq, false, false, buttonOffset, btnAngle.center + buttonOffset);
                DrawTargetClickPulse(painter, btnInvert.center + buttonOffset, invertSeq, false, false, Accent);
            }
        }

        private void DrawExplodeScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Vector2 center = new Vector2(width * 0.50f, height * 0.30f);
            float scale = 1.34f;
            Rect button = new Rect(width * 0.50f - 46f, height * 0.70f, 92f, 92f);
            Rect slider = new Rect(width * 0.50f - 124f, height * 0.89f, 248f, 30f);
            float baseExplode = phaseIndex == 0 ? GetActionProgress(phaseT, false) * 0.46f : 0.46f;
            float sliderOut;
            float sliderBack;
            float sliderReveal = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.44f, 0.74f)) : 1f;
            float sliderMotionSeq = phaseIndex == 1 ? Phase01(phaseT, 0f, 0.86f) : 0f;
            float sliderValue = phaseIndex == 1
                ? EvaluateRoundTripDrag(sliderMotionSeq, 0.46f, 1f, 0f, out sliderOut, out sliderBack)
                : 0.46f;
            float explodeBoost = phaseIndex == 1
                ? (sliderValue <= 0.46f
                    ? EaseInOut(Mathf.InverseLerp(0f, 0.46f, sliderValue)) * 0.46f
                    : Mathf.Lerp(0.46f, 1.18f, EaseOut(Mathf.InverseLerp(0.46f, 1f, sliderValue))))
                : baseExplode;
            float explode = phaseIndex == 1 ? explodeBoost : baseExplode;
            Vector2 sliderStart = GetSliderThumb(slider, 0.46f);
            Vector2 sliderPeak = GetSliderThumb(slider, 1f);
            Vector2 sliderEnd = GetSliderThumb(slider, 0f);
            Vector2 buttonContact = button.center + new Vector2(button.width * 0.34f, 0f);
            Vector2 loopStart = GetDefaultActorStart(buttonContact);

            DrawExplodedDrone(painter, center, scale, explode);
            DrawStandaloneActionButton(painter, button, OnboardingMiniIcon.Explode, phaseIndex == 0 ? GetActionProgress(phaseT, false) : 1f, phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.30f, 0.60f)) : 0.36f, phaseIndex == 0 ? GetPressProgress(phaseT, false) : 0f);
            DrawSlider(painter, slider, sliderValue, Accent, sliderReveal > 0.01f, phaseIndex == 1 ? 0.46f : sliderReveal * 0.22f, phaseIndex == 1 ? Mathf.Max(GetPressProgress(Phase01(sliderMotionSeq, 0.06f, 0.56f), true), GetPressProgress(Phase01(sliderMotionSeq, 0.56f, 1f), true)) : 0f);

            if (phaseIndex == 0)
            {
                Vector2 offset = new Vector2(button.width * 0.34f, 0f);
                DrawActorToTarget(painter, width, height, button.center, phaseT, false, false, offset);
                DrawTargetClickPulse(painter, button.center + offset, phaseT, false, false, Accent);
            }
            else if (phaseT < 0.86f)
            {
                DrawActorTwoSegmentDrag(painter, sliderStart, sliderPeak, sliderEnd, sliderMotionSeq, buttonContact);
                DrawTargetClickPulse(painter, sliderStart, Phase01(sliderMotionSeq, 0.06f, 0.24f), false, false, Accent);
            }
            else
            {
                float returnT = EaseInOut(Phase01(phaseT, 0.86f, 1f));
                Vector2 actorPos = Vector2.Lerp(sliderEnd, loopStart, returnT);
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, actorPos, 0f, false);
                }
                else
                {
                    DrawCursorActor(painter, actorPos, 0f, false);
                }
            }
        }

        private void DrawFilterScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect[] buttons =
            {
                new Rect(width * 0.50f - 132f, height * 0.66f, 74f, 74f),
                new Rect(width * 0.50f - 38f, height * 0.66f, 74f, 74f),
                new Rect(width * 0.50f + 56f, height * 0.66f, 74f, 74f),
                new Rect(width * 0.50f - 132f, height * 0.80f, 74f, 74f),
                new Rect(width * 0.50f - 38f, height * 0.80f, 74f, 74f),
                new Rect(width * 0.50f + 56f, height * 0.80f, 74f, 74f)
            };
            FilterGlyphKind[] kinds =
            {
                FilterGlyphKind.Airframe, FilterGlyphKind.Propulsion, FilterGlyphKind.Avionics,
                FilterGlyphKind.Sensors, FilterGlyphKind.Power, FilterGlyphKind.Fasteners
            };

            int focusIndex = 1;
            float disableSeq = phaseIndex == 0 ? Phase01(phaseT, 0.06f, 0.46f) : 0f;
            float enableSeq = phaseIndex == 0 ? Phase01(phaseT, 0.66f, 0.96f) : 0f;
            float moveOutSeq = phaseIndex == 1 ? Phase01(phaseT, 0.02f, 0.12f) : 0f;
            float moveBackSeq = phaseIndex == 1 ? Phase01(phaseT, 0.12f, 0.22f) : 0f;
            float isolateSeq = phaseIndex == 1 ? Phase01(phaseT, 0.22f, 0.58f) : 0f;
            float restoreSeq = phaseIndex == 1 ? Phase01(phaseT, 0.64f, 0.82f) : 0f;
            float returnSeq = phaseIndex == 1 ? Phase01(phaseT, 0.82f, 1f) : 0f;
            float disabled = phaseIndex == 0 ? GetActionProgress(disableSeq, false) : 0f;
            float enabledAgain = phaseIndex == 0 ? GetActionProgress(enableSeq, false) : 0f;
            float restored = phaseIndex == 1 ? GetActionProgress(restoreSeq, false) : 0f;
            float isolated = phaseIndex == 1 ? GetDoubleClickActionProgress(isolateSeq) * (1f - restored) : 0f;
            float focusVisible = phaseIndex == 0
                ? Mathf.Lerp(1f - disabled, 1f, enabledAgain)
                : 1f;

            Rect[] droneParts =
            {
                new Rect(width * 0.50f - 72f, height * 0.22f, 34f, 92f),
                new Rect(width * 0.50f - 24f, height * 0.24f, 48f, 30f),
                new Rect(width * 0.50f + 34f, height * 0.22f, 34f, 92f),
                new Rect(width * 0.50f - 16f, height * 0.30f, 32f, 28f),
                new Rect(width * 0.50f - 22f, height * 0.35f, 44f, 24f),
                new Rect(width * 0.50f - 8f, height * 0.18f, 16f, 20f)
            };

            for (int i = 0; i < droneParts.Length; i++)
            {
                float visible = 1f;
                if (phaseIndex == 0 && i == focusIndex)
                {
                    visible = focusVisible;
                }
                else if (phaseIndex == 1)
                {
                    visible = i == focusIndex ? 1f : Mathf.Lerp(Mathf.Lerp(1f, 0f, isolated), 1f, restored);
                }

                if (visible <= 0.02f) continue;
                Color fill = i == focusIndex ? WithAlpha(AccentGlow, 0.26f + isolated * 0.18f) : WithAlpha(AccentGlow, 0.16f);
                Color stroke = i == focusIndex ? Color.Lerp(Accent, Warning, isolated * 0.28f) : WithAlpha(Accent, 0.64f);
                fill.a *= visible;
                stroke.a *= visible;
                DrawPartNode(painter, droneParts[i], fill, stroke, i == focusIndex ? 1.5f : 1.2f);
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                float active = 1f;
                float hover = 0f;
                float press = 0f;
                if (i == focusIndex)
                {
                    if (phaseIndex == 0)
                    {
                        active = focusVisible;
                        if (phaseT < 0.52f)
                        {
                            hover = EaseOut(Phase01(disableSeq, 0.18f, 0.52f));
                            press = GetPressProgress(disableSeq, false);
                        }
                        else
                        {
                            hover = EaseOut(Phase01(enableSeq, 0.18f, 0.52f));
                            press = GetPressProgress(enableSeq, false);
                        }
                    }
                    else
                    {
                        active = 1f;
                        hover = Mathf.Max(
                            EaseOut(Phase01(moveBackSeq, 0.42f, 0.98f)),
                            Mathf.Max(
                                EaseOut(Phase01(isolateSeq, 0.18f, 0.56f)),
                                EaseOut(Phase01(restoreSeq, 0.18f, 0.56f))));
                        press = Mathf.Max(
                            Mathf.Max(GetPressProgress(isolateSeq, false), GetSecondClickProgress(isolateSeq, false)),
                            GetPressProgress(restoreSeq, false));
                    }
                }

                if (phaseIndex == 1)
                {
                    active = i == focusIndex ? 1f : Mathf.Lerp(Mathf.Lerp(1f, 0.12f, isolated), 1f, restored);
                }

                DrawFilterButton(painter, buttons[i], kinds[i], active, hover, press);
            }

            Vector2 offset = new Vector2(12f, 0f);
            Vector2 focusContact = buttons[focusIndex].center + offset;
            Vector2 leaveContact = focusContact + new Vector2(38f, -26f);
            Vector2 loopStart = GetDefaultActorStart(focusContact);

            if (phaseIndex == 0 && phaseT < 0.52f)
            {
                DrawActorToTarget(painter, width, height, buttons[focusIndex].center, disableSeq, false, false, offset);
                DrawTargetClickPulse(painter, focusContact, disableSeq, false, false, Accent);
            }
            else if (phaseIndex == 0 && phaseT < 0.66f)
            {
                Vector2 pos = Vector2.Lerp(focusContact, leaveContact, EaseInOut(Phase01(phaseT, 0.52f, 0.66f)));
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, pos, 0f, false);
                }
                else
                {
                    DrawCursorActor(painter, pos, 0f, false);
                }
            }
            else if (phaseIndex == 0)
            {
                DrawActorToTarget(painter, width, height, buttons[focusIndex].center, enableSeq, false, false, offset, focusContact + new Vector2(38f, -26f));
                DrawTargetClickPulse(painter, focusContact, enableSeq, false, false, Accent);
            }
            else if (phaseT < 0.12f)
            {
                Vector2 pos = Vector2.Lerp(focusContact, leaveContact, EaseInOut(moveOutSeq));
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, pos, 0f, false);
                }
                else
                {
                    DrawCursorActor(painter, pos, 0f, false);
                }
            }
            else if (phaseT < 0.22f)
            {
                Vector2 pos = Vector2.Lerp(leaveContact, focusContact, EaseInOut(moveBackSeq));
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, pos, 0f, false);
                }
                else
                {
                    DrawCursorActor(painter, pos, 0f, false);
                }
            }
            else if (phaseT < 0.64f)
            {
                float visualPress = Mathf.Max(GetPressProgress(isolateSeq, false), GetSecondClickProgress(isolateSeq, false));
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, focusContact, visualPress, true);
                }
                else
                {
                    DrawCursorActor(painter, focusContact, visualPress, false);
                }

                DrawTargetClickPulse(painter, focusContact, isolateSeq, true, false, Accent);
                DrawClickRing(painter, focusContact, EaseOut(Phase01(isolateSeq, 0.82f, 0.98f)), Warning);
            }
            else if (phaseT < 0.82f)
            {
                DrawActorToTarget(painter, width, height, buttons[focusIndex].center, restoreSeq, false, false, offset, focusContact);
                DrawTargetClickPulse(painter, focusContact, restoreSeq, false, false, Accent);
            }
            else
            {
                Vector2 pos = Vector2.Lerp(focusContact, loopStart, EaseInOut(returnSeq));
                if (_isTouchMode)
                {
                    DrawTouchActor(painter, pos, 0f, false);
                }
                else
                {
                    DrawCursorActor(painter, pos, 0f, false);
                }
            }
        }

        private void DrawRenderModeScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect viewport = new Rect(width * 0.50f - 96f, height * 0.17f, 192f, 206f);
            Rect btnA = new Rect(width * 0.50f - 144f, height * 0.72f, 88f, 98f);
            Rect btnB = new Rect(width * 0.50f - 44f, height * 0.72f, 88f, 98f);
            Rect btnC = new Rect(width * 0.50f + 56f, height * 0.72f, 88f, 98f);
            Vector2 offset = new Vector2(14f, 0f);
            Vector2 contactA = btnA.center + offset;
            Vector2 contactB = btnB.center + offset;
            Vector2 contactC = btnC.center + offset;

            FillRect(painter, viewport, WithAlpha(Surface, 0.58f));
            StrokeRect(painter, viewport, FrameStroke, 1.25f);

            Vector2 droneCenter = viewport.center + new Vector2(0f, 18f);
            float activeA = 0.18f;
            float activeB = 0.18f;
            float activeC = 0.18f;
            if (phaseIndex == 0)
            {
                float xrayBlend = GetActionProgress(phaseT, false);
                DrawWireDrone(painter, droneCenter, 1.08f, Color.Lerp(Foreground, Accent, xrayBlend));
                activeA = Mathf.Lerp(0.18f, 1f, xrayBlend);
            }
            else if (phaseIndex == 1)
            {
                float solidBlend = GetActionProgress(phaseT, false);
                DrawWireDrone(painter, droneCenter, 1.08f, WithAlpha(Accent, 1f - solidBlend));
                DrawDroneSilhouette(painter, droneCenter, 1.08f, 0f, 0f, WithAlpha(Foreground, 0.24f + solidBlend * 0.08f), Color.Lerp(WithAlpha(Surface, 0.12f), Color.black, solidBlend));
                activeA = Mathf.Lerp(1f, 0.18f, solidBlend);
                activeB = Mathf.Lerp(0.18f, 1f, solidBlend);
            }
            else
            {
                float thermalOn = GetActionProgress(Phase01(phaseT, 0.08f, 0.46f), false);
                float thermalOff = GetActionProgress(Phase01(phaseT, 0.72f, 0.98f), false);
                float thermalVisible = thermalOn * (1f - thermalOff);
                float restoreWhite = thermalOff;
                Color baseFill = Color.Lerp(Color.black, WithAlpha(Surface, 0.18f), restoreWhite);
                Color baseStroke = Color.Lerp(WithAlpha(Foreground, 0.24f), Foreground, restoreWhite);
                DrawDroneSilhouette(painter, droneCenter, 1.08f, 0f, 0f, baseStroke, baseFill);
                if (thermalVisible > 0.01f)
                {
                    DrawPowerHeatLayout(painter, droneCenter, 1.08f, thermalVisible);
                }

                activeB = Mathf.Lerp(1f, 0.18f, thermalOn);
                activeC = thermalVisible;
            }

            DrawRenderOptionButton(painter, btnA, RenderOptionKind.XRay, activeA, phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0f, phaseIndex == 0 ? GetPressProgress(phaseT, false) : 0f);
            DrawRenderOptionButton(painter, btnB, RenderOptionKind.Solid, activeB, phaseIndex == 1 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0f, phaseIndex == 1 ? GetPressProgress(phaseT, false) : 0f);
            DrawRenderOptionButton(painter, btnC, RenderOptionKind.Thermal, activeC, phaseIndex == 2 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0f, phaseIndex == 2 ? Mathf.Max(GetPressProgress(Phase01(phaseT, 0.08f, 0.46f), false), GetPressProgress(Phase01(phaseT, 0.72f, 0.98f), false)) : 0f);

            if (phaseIndex == 0)
            {
                DrawActorToTarget(painter, width, height, btnA.center, phaseT, false, false, offset);
                DrawTargetClickPulse(painter, contactA, phaseT, false, false, Accent);
            }
            else if (phaseIndex == 1)
            {
                DrawActorToTarget(painter, width, height, btnB.center, phaseT, false, false, offset, contactA);
                DrawTargetClickPulse(painter, contactB, phaseT, false, false, Accent);
            }
            else
            {
                float thermalSeq = Phase01(phaseT, 0.08f, 0.46f);
                float restoreSeq = Phase01(phaseT, 0.72f, 0.98f);
                Vector2 bounce = contactC + new Vector2(28f, -24f);
                if (phaseT < 0.52f)
                {
                    DrawActorToTarget(painter, width, height, btnC.center, thermalSeq, false, false, offset, contactB);
                    DrawTargetClickPulse(painter, contactC, thermalSeq, false, false, Accent);
                }
                else if (phaseT < 0.72f)
                {
                    Vector2 pos = Vector2.Lerp(contactC, bounce, EaseInOut(Phase01(phaseT, 0.52f, 0.72f)));
                    if (_isTouchMode)
                    {
                        DrawTouchActor(painter, pos, 0f, false);
                    }
                    else
                    {
                        DrawCursorActor(painter, pos, 0f, false);
                    }
                }
                else
                {
                    DrawActorToTarget(painter, width, height, btnC.center, restoreSeq, false, false, offset, bounce);
                    DrawTargetClickPulse(painter, contactC, restoreSeq, false, false, Accent);
                }
            }
        }

        private void DrawEnvironmentScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect viewport = new Rect(width * 0.50f - 106f, height * 0.15f, 212f, 232f);
            Rect btnStudio = new Rect(width * 0.50f - 150f, height * 0.72f, 90f, 98f);
            Rect btnTime = new Rect(width * 0.50f - 45f, height * 0.72f, 90f, 98f);
            Rect btnColor = new Rect(width * 0.50f + 60f, height * 0.72f, 90f, 98f);
            Vector2 droneCenter = viewport.center + new Vector2(0f, 24f);
            Vector2 offset = new Vector2(14f, 0f);
            Vector2 studioContact = btnStudio.center + offset;
            Vector2 timeContact = btnTime.center + offset;
            Vector2 colorContact = btnColor.center + offset;

            if (phaseIndex == 0)
            {
                float lightSeq = Phase01(phaseT, 0.06f, 0.42f);
                float blueprintSeq = Phase01(phaseT, 0.58f, 0.94f);
                float lightT = GetActionProgress(lightSeq, false);
                float blueT = GetActionProgress(blueprintSeq, false);
                Color bg = SurfaceAlt;
                bg = Color.Lerp(bg, AmbientDay, lightT);
                bg = Color.Lerp(bg, new Color(0.14f, 0.30f, 0.56f, 0.96f), blueT);

                FillRect(painter, viewport, bg);
                StrokeRect(painter, viewport, WithAlpha(Foreground, 0.22f), 1.2f);
                DrawDroneSilhouette(
                    painter,
                    droneCenter,
                    1.04f,
                    0f,
                    0f,
                    blueT > 0.50f ? WithAlpha(Foreground, 0.82f) : Foreground,
                    lightT > 0.08f && blueT < 0.44f ? WithAlpha(Color.black, 0.92f) : WithAlpha(Surface, 0.56f));

                if (blueT > 0.01f)
                {
                    DrawBlueprintLines(painter, viewport);
                }
            }
            else if (phaseIndex == 1)
            {
                float dayClickSeq = Phase01(phaseT, 0.04f, 0.12f);
                float dayT = phaseT < 0.12f ? 0f : EaseInOut(Phase01(phaseT, 0.12f, 0.30f));
                float sunsetClickSeq = Phase01(phaseT, 0.36f, 0.44f);
                float sunsetT = phaseT < 0.44f ? 0f : EaseInOut(Phase01(phaseT, 0.44f, 0.62f));
                float nightClickSeq = Phase01(phaseT, 0.68f, 0.76f);
                float nightT = phaseT < 0.76f ? 0f : EaseInOut(Phase01(phaseT, 0.76f, 0.90f));
                Color bg = new Color(0.14f, 0.30f, 0.56f, 0.96f);
                bg = Color.Lerp(bg, AmbientDay, dayT);
                bg = Color.Lerp(bg, new Color(0.74f, 0.42f, 0.32f, 0.98f), sunsetT);
                bg = Color.Lerp(bg, AmbientNight, nightT);

                FillRect(painter, viewport, bg);
                StrokeRect(painter, viewport, WithAlpha(Foreground, 0.22f), 1.2f);
                DrawDroneSilhouette(painter, droneCenter, 1.04f, 0f, 0f, Foreground, WithAlpha(Surface, 0.56f));
                float horizonY = viewport.yMax - 44f;
                DrawLine(painter, new Vector2(viewport.x + 12f, horizonY), new Vector2(viewport.xMax - 12f, horizonY), WithAlpha(Foreground, 0.12f), 1.2f);
                Vector2 orbitCenter = new Vector2(viewport.center.x, horizonY);
                float orbitRadiusX = viewport.width * 0.44f;
                float orbitRadiusY = viewport.height * 0.36f;
                float sunAngle = sunsetT > 0.001f
                    ? Mathf.Lerp(90f, 8f, sunsetT)
                    : Mathf.Lerp(180f, 90f, dayT);
                if (nightT > 0.001f)
                {
                    sunAngle = Mathf.Lerp(8f, 0f, nightT);
                }

                Vector2 sunPos = EllipsePoint(orbitCenter, orbitRadiusX, orbitRadiusY, sunAngle);
                float sunVisible = Mathf.Clamp01(Mathf.Max(dayT, sunsetT) * (1f - nightT));
                if (sunVisible > 0.01f)
                {
                    Color sunColor = Color.Lerp(Warning, new Color(0.98f, 0.42f, 0.18f, 0.98f), sunsetT);
                    FillCircle(painter, sunPos, 10f, WithAlpha(sunColor, sunVisible));
                }

                float moonVisible = nightT;
                if (moonVisible > 0.01f)
                {
                    Vector2 moonPos = EllipsePoint(orbitCenter, orbitRadiusX, orbitRadiusY, Mathf.Lerp(180f, 90f, nightT));
                    FillCircle(painter, moonPos, 8f, WithAlpha(Foreground, 0.88f * moonVisible));
                    FillCircle(painter, moonPos + new Vector2(3f, -1f), 7f, WithAlpha(bg, 0.92f * moonVisible));
                }
            }
            else
            {
                float activateColorSeq = Phase01(phaseT, 0.06f, 0.18f);
                float colorTransition = GetActionProgress(activateColorSeq, false);
                Color bg = AmbientNight;
                Color[] palette =
                {
                    new Color(0.92f, 0.26f, 0.22f, 0.98f),
                    new Color(0.16f, 0.40f, 0.92f, 0.98f),
                    new Color(0.92f, 0.82f, 0.20f, 0.98f)
                };

                if (colorTransition > 0.01f)
                {
                    if (phaseT < 0.34f)
                    {
                        bg = Color.Lerp(AmbientNight, palette[0], EaseInOut(Phase01(phaseT, 0.18f, 0.34f)));
                    }
                    else if (phaseT < 0.52f)
                    {
                        bg = palette[0];
                    }
                    else if (phaseT < 0.66f)
                    {
                        bg = Color.Lerp(palette[0], palette[1], EaseInOut(Phase01(phaseT, 0.52f, 0.66f)));
                    }
                    else if (phaseT < 0.82f)
                    {
                        bg = palette[1];
                    }
                    else if (phaseT < 0.94f)
                    {
                        bg = Color.Lerp(palette[1], palette[2], EaseInOut(Phase01(phaseT, 0.82f, 0.94f)));
                    }
                    else
                    {
                        bg = palette[2];
                    }
                }

                FillRect(painter, viewport, bg);
                StrokeRect(painter, viewport, WithAlpha(Foreground, 0.22f), 1.2f);
                DrawDroneSilhouette(painter, droneCenter, 1.04f, 0f, 0f, Foreground, WithAlpha(Surface, 0.36f));
                float moonFade = 1f - EaseOut(Phase01(phaseT, 0.18f, 0.34f));
                if (moonFade > 0.01f)
                {
                    float horizonY = viewport.yMax - 44f;
                    Vector2 orbitCenter = new Vector2(viewport.center.x, horizonY);
                    float orbitRadiusX = viewport.width * 0.44f;
                    float orbitRadiusY = viewport.height * 0.36f;
                    Vector2 moonPos = EllipsePoint(orbitCenter, orbitRadiusX, orbitRadiusY, 90f);
                    FillCircle(painter, moonPos, 8f, WithAlpha(Foreground, 0.88f * moonFade));
                    FillCircle(painter, moonPos + new Vector2(3f, -1f), 7f, WithAlpha(bg, 0.92f * moonFade));
                }
            }

            float studioLightAction = phaseIndex == 0 ? GetActionProgress(Phase01(phaseT, 0.06f, 0.42f), false) : 0f;
            float studioBlueprintAction = phaseIndex == 0 ? GetActionProgress(Phase01(phaseT, 0.58f, 0.94f), false) : 0f;
            float studioHover = phaseIndex == 0
                ? Mathf.Max(EaseOut(Phase01(Phase01(phaseT, 0.06f, 0.42f), 0.24f, 0.54f)), EaseOut(Phase01(Phase01(phaseT, 0.58f, 0.94f), 0.24f, 0.54f)))
                : 0f;
            float studioPress = phaseIndex == 0
                ? Mathf.Max(GetPressProgress(Phase01(phaseT, 0.06f, 0.42f), false), GetPressProgress(Phase01(phaseT, 0.58f, 0.94f), false))
                : 0f;
            float studioActive = phaseIndex == 0 ? Mathf.Max(studioLightAction, studioBlueprintAction) : 0.20f;
            float timeDayAction = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.04f, 0.12f), false) : 0f;
            float timeSunsetAction = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.36f, 0.44f), false) : 0f;
            float timeNightAction = phaseIndex == 1 ? GetActionProgress(Phase01(phaseT, 0.68f, 0.76f), false) : 0f;
            float timeHover = phaseIndex == 1
                ? Mathf.Max(
                    EaseOut(Phase01(Phase01(phaseT, 0.04f, 0.12f), 0.24f, 0.54f)),
                    Mathf.Max(EaseOut(Phase01(Phase01(phaseT, 0.36f, 0.44f), 0.24f, 0.54f)),
                        EaseOut(Phase01(Phase01(phaseT, 0.68f, 0.76f), 0.24f, 0.54f))))
                : 0f;
            float timePress = phaseIndex == 1
                ? Mathf.Max(
                    GetPressProgress(Phase01(phaseT, 0.04f, 0.12f), false),
                    Mathf.Max(GetPressProgress(Phase01(phaseT, 0.36f, 0.44f), false), GetPressProgress(Phase01(phaseT, 0.68f, 0.76f), false)))
                : 0f;
            float colorAction = phaseIndex == 2 ? GetActionProgress(Phase01(phaseT, 0.06f, 0.18f), false) : 0f;
            float timeActive = phaseIndex == 1
                ? Mathf.Max(timeDayAction, Mathf.Max(timeSunsetAction, timeNightAction))
                : (phaseIndex == 2 ? Mathf.Lerp(1f, 0.20f, colorAction) : 0.20f);
            float colorHover = phaseIndex == 2 ? EaseOut(Phase01(Phase01(phaseT, 0.06f, 0.18f), 0.24f, 0.54f)) : 0f;
            float colorPress = phaseIndex == 2 ? GetPressProgress(Phase01(phaseT, 0.06f, 0.18f), false) : 0f;

            DrawSubmenuCardButton(painter, btnStudio, studioActive, studioHover, studioPress,
                (visual, color) => DrawMiniIcon(painter, OnboardingMiniIcon.Studio, visual.center + new Vector2(0f, -6f), 24f, color));
            DrawSubmenuCardButton(painter, btnTime, timeActive, timeHover, timePress,
                (visual, color) =>
                {
                    DrawLine(painter, visual.center + new Vector2(-15f, 2f), visual.center + new Vector2(15f, 2f), color, 1.5f);
                    DrawArcSegment(painter, visual.center + new Vector2(0f, 2f), 13f, 210f, 330f, WithAlpha(color, 0.72f), 1.2f);
                    FillCircle(painter, visual.center + new Vector2(7f, -8f), 5f, color);
                });
            DrawSubmenuCardButton(painter, btnColor, phaseIndex == 2 ? colorAction : 0.20f, colorHover, colorPress,
                (visual, color) =>
                {
                    FillCircle(painter, visual.center + new Vector2(-11f, -6f), 4.5f, WithAlpha(color, 0.90f));
                    FillCircle(painter, visual.center + new Vector2(0f, -10f), 4.5f, WithAlpha(color, 0.72f));
                    FillCircle(painter, visual.center + new Vector2(11f, -6f), 4.5f, WithAlpha(color, 0.56f));
                });

            if (phaseIndex == 0)
            {
                float lightSeq = Phase01(phaseT, 0.06f, 0.42f);
                float blueprintSeq = Phase01(phaseT, 0.58f, 0.94f);
                Vector2 bounce = studioContact + new Vector2(26f, -22f);
                if (phaseT < 0.50f)
                {
                    DrawActorToTarget(painter, width, height, btnStudio.center, lightSeq, false, false, offset);
                    DrawTargetClickPulse(painter, studioContact, lightSeq, false, false, Accent);
                }
                else if (phaseT < 0.60f)
                {
                    Vector2 pos = Vector2.Lerp(studioContact, bounce, EaseInOut(Phase01(phaseT, 0.50f, 0.60f)));
                    if (_isTouchMode)
                    {
                        DrawTouchActor(painter, pos, 0f, false);
                    }
                    else
                    {
                        DrawCursorActor(painter, pos, 0f, false);
                    }
                }
                else
                {
                    DrawActorToTarget(painter, width, height, btnStudio.center, blueprintSeq, false, false, offset, bounce);
                    DrawTargetClickPulse(painter, studioContact, blueprintSeq, false, false, Accent);
                }
            }
            else if (phaseIndex == 1)
            {
                float daySeq = Phase01(phaseT, 0.04f, 0.12f);
                float sunsetSeq = Phase01(phaseT, 0.36f, 0.44f);
                float nightSeq = Phase01(phaseT, 0.68f, 0.76f);
                Vector2 bounceA = timeContact + new Vector2(32f, -26f);
                Vector2 bounceB = timeContact + new Vector2(-26f, -28f);
                if (phaseT < 0.34f)
                {
                    DrawActorToTarget(painter, width, height, btnTime.center, daySeq, false, false, offset, studioContact);
                    DrawTargetClickPulse(painter, timeContact, daySeq, false, false, Accent);
                }
                else if (phaseT < 0.44f)
                {
                    Vector2 pos = Vector2.Lerp(timeContact, bounceA, EaseInOut(Phase01(phaseT, 0.34f, 0.44f)));
                    if (_isTouchMode)
                    {
                        DrawTouchActor(painter, pos, 0f, false);
                    }
                    else
                    {
                        DrawCursorActor(painter, pos, 0f, false);
                    }
                }
                else if (phaseT < 0.66f)
                {
                    DrawActorToTarget(painter, width, height, btnTime.center, sunsetSeq, false, false, offset, bounceA);
                    DrawTargetClickPulse(painter, timeContact, sunsetSeq, false, false, Accent);
                }
                else if (phaseT < 0.76f)
                {
                    Vector2 pos = Vector2.Lerp(timeContact, bounceB, EaseInOut(Phase01(phaseT, 0.66f, 0.76f)));
                    if (_isTouchMode)
                    {
                        DrawTouchActor(painter, pos, 0f, false);
                    }
                    else
                    {
                        DrawCursorActor(painter, pos, 0f, false);
                    }
                }
                else
                {
                    DrawActorToTarget(painter, width, height, btnTime.center, nightSeq, false, false, offset, bounceB);
                    DrawTargetClickPulse(painter, timeContact, nightSeq, false, false, Accent);
                }
            }
            else
            {
                float activateColorSeq = Phase01(phaseT, 0.06f, 0.18f);
                DrawActorToTarget(painter, width, height, btnColor.center, activateColorSeq, false, false, offset, timeContact);
                DrawTargetClickPulse(painter, colorContact, activateColorSeq, false, false, Accent);
            }
        }

        private void DrawLightingScene(Painter2D painter, float width, float height, int phaseIndex, float phaseT)
        {
            Rect viewport = new Rect(width * 0.50f - 84f, height * 0.18f, 168f, 168f);
            float rotBase = 0.20f;
            float intensityBase = 0.28f;
            float backgroundBase = 0.24f;
            float rotOut;
            float rotBack;
            float intensityOut;
            float intensityBack;
            float backgroundOut;
            float backgroundBack;
            float rot = phaseIndex == 0 ? EvaluateRoundTripDrag(phaseT, rotBase, 0.82f, rotBase, out rotOut, out rotBack) : rotBase;
            float intensity = phaseIndex == 1 ? EvaluateRoundTripDrag(phaseT, intensityBase, 0.92f, intensityBase, out intensityOut, out intensityBack) : intensityBase;
            float background = phaseIndex == 2 ? EvaluateRoundTripDrag(phaseT, backgroundBase, 0.82f, backgroundBase, out backgroundOut, out backgroundBack) : backgroundBase;

            FillRect(painter, viewport, Color.Lerp(Surface, AmbientDay, background * 0.75f));
            StrokeRect(painter, viewport, FrameStroke, 1.2f);
            DrawLightBall(painter, viewport.center, rot, intensity);

            Rect sliderA = new Rect(width * 0.50f - 126f, height * 0.62f, 252f, 26f);
            Rect sliderB = new Rect(width * 0.50f - 126f, height * 0.75f, 252f, 26f);
            Rect sliderC = new Rect(width * 0.50f - 126f, height * 0.88f, 252f, 26f);
            DrawSlider(painter, sliderA, rot, Accent, true, phaseIndex == 0 ? 0.46f : 0f, phaseIndex == 0 ? GetPressProgress(phaseT, true) : 0f);
            DrawSlider(painter, sliderB, intensity, Accent, true, phaseIndex == 1 ? 0.46f : 0f, phaseIndex == 1 ? GetPressProgress(phaseT, true) : 0f);
            DrawSlider(painter, sliderC, background, Accent, true, phaseIndex == 2 ? 0.46f : 0f, phaseIndex == 2 ? GetPressProgress(phaseT, true) : 0f);

            Rect currentSlider = phaseIndex == 0 ? sliderA : (phaseIndex == 1 ? sliderB : sliderC);
            float activeValue = phaseIndex == 0 ? rot : (phaseIndex == 1 ? intensity : background);
            Vector2 start = GetSliderThumb(currentSlider, phaseIndex == 0 ? rotBase : (phaseIndex == 1 ? intensityBase : backgroundBase));
            Vector2 peak = GetSliderThumb(currentSlider, phaseIndex == 0 ? 0.82f : (phaseIndex == 1 ? 0.92f : 0.82f));
            Vector2 end = GetSliderThumb(currentSlider, phaseIndex == 0 ? rotBase : (phaseIndex == 1 ? intensityBase : backgroundBase));
            Vector2? previousContact = phaseIndex == 0
                ? null
                : (phaseIndex == 1
                    ? GetSliderThumb(sliderA, rotBase)
                    : GetSliderThumb(sliderB, intensityBase));
            DrawActorTwoSegmentDrag(painter, start, peak, end, phaseT, previousContact);
            DrawTargetClickPulse(painter, start, phaseT, false, true, Accent);
        }

        private static float GetApproachProgress(float phaseT, bool drag)
        {
            return drag
                ? EaseInOut(Phase01(phaseT, 0.08f, 0.34f))
                : EaseInOut(Phase01(phaseT, 0.08f, 0.46f));
        }

        private static float GetPressProgress(float phaseT, bool drag)
        {
            return drag
                ? EaseOut(Phase01(phaseT, 0.38f, 0.54f))
                : EaseOut(Phase01(phaseT, 0.50f, 0.66f));
        }

        private static float GetPressReleaseProgress(float phaseT, bool drag)
        {
            float pressIn = drag
                ? EaseOut(Phase01(phaseT, 0.38f, 0.52f))
                : EaseOut(Phase01(phaseT, 0.50f, 0.62f));
            float releaseOut = drag
                ? EaseInOut(Phase01(phaseT, 0.52f, 0.82f))
                : EaseInOut(Phase01(phaseT, 0.62f, 0.86f));
            return pressIn * (1f - releaseOut);
        }

        private static float GetActionProgress(float phaseT, bool drag)
        {
            return drag
                ? EaseInOut(Phase01(phaseT, 0.58f, 0.80f))
                : EaseOut(Phase01(phaseT, 0.68f, 0.82f));
        }

        private static float GetClickPulseProgress(float phaseT, bool drag)
        {
            return drag
                ? EaseOut(Phase01(phaseT, 0.48f, 0.74f))
                : EaseOut(Phase01(phaseT, 0.56f, 0.82f));
        }

        private static float GetSecondClickProgress(float phaseT, bool drag)
        {
            return drag
                ? EaseOut(Phase01(phaseT, 0.74f, 0.88f))
                : EaseOut(Phase01(phaseT, 0.78f, 0.92f));
        }

        private static float GetDoubleClickActionProgress(float phaseT)
        {
            return EaseOut(Phase01(phaseT, 0.90f, 0.98f));
        }

        private float EvaluateRoundTripDrag(float phaseT, float startValue, float peakValue, float endValue, out float outward, out float inward)
        {
            float outwardSeq = Phase01(phaseT, 0.06f, 0.56f);
            float inwardSeq = Phase01(phaseT, 0.56f, 1f);
            outward = phaseT < 0.56f ? GetActionProgress(outwardSeq, true) : 1f;
            inward = phaseT >= 0.56f ? GetActionProgress(inwardSeq, true) : 0f;
            return phaseT < 0.56f
                ? Mathf.Lerp(startValue, peakValue, outward)
                : Mathf.Lerp(peakValue, endValue, inward);
        }

        private Vector2 GetDefaultActorStart(Vector2 contact)
        {
            return _isTouchMode
                ? contact + new Vector2(58f, -48f)
                : contact + new Vector2(62f, -50f);
        }

        private void DrawActorToTarget(Painter2D painter, float width, float height, Vector2 target, float phaseT, bool doubleTap, bool drag,
            Vector2 contactOffset = default, Vector2? startContact = null, bool alternateMouse = false)
        {
            Vector2 contact = target + contactOffset;
            float approach = GetApproachProgress(phaseT, drag);
            float press = GetPressProgress(phaseT, drag);
            float secondPress = doubleTap ? GetSecondClickProgress(phaseT, drag) : 0f;
            float action = GetActionProgress(phaseT, drag);
            float visualPress = Mathf.Max(press, secondPress);
            Vector2 actorStart = startContact ?? GetDefaultActorStart(contact);

            if (_isTouchMode)
            {
                Vector2 pos = Vector2.Lerp(actorStart, contact, approach);
                if (drag)
                {
                    Vector2 dragEnd = contact + new Vector2(24f, 0f);
                    if (approach >= 0.999f)
                    {
                        pos = Vector2.Lerp(contact, dragEnd, action);
                    }

                    if (action > 0f)
                    {
                        DrawDragTrail(painter, contact, pos, WithAlpha(Accent, 0.14f), 1.7f);
                    }
                }

                DrawTouchActor(painter, pos, visualPress, doubleTap);
            }
            else
            {
                Vector2 pos = Vector2.Lerp(actorStart, contact, approach);
                if (drag)
                {
                    Vector2 dragEnd = contact + new Vector2(30f, 0f);
                    if (approach >= 0.999f)
                    {
                        pos = Vector2.Lerp(contact, dragEnd, action);
                    }

                    if (action > 0f)
                    {
                        DrawDragTrail(painter, contact, pos, WithAlpha(Accent, 0.14f), 1.8f);
                    }
                }

                DrawCursorActor(painter, pos, visualPress, alternateMouse);
            }
        }

        private void DrawActorDragBetween(Painter2D painter, Vector2 startContact, Vector2 endContact, float phaseT, bool doubleTap = false,
            Vector2? previousContact = null, bool alternateMouse = false)
        {
            float approach = GetApproachProgress(phaseT, true);
            float press = GetPressProgress(phaseT, true);
            float secondPress = doubleTap ? GetSecondClickProgress(phaseT, true) : 0f;
            float action = GetActionProgress(phaseT, true);
            float visualPress = Mathf.Max(press, secondPress);

            if (_isTouchMode)
            {
                Vector2 start = previousContact ?? GetDefaultActorStart(startContact);
                Vector2 pos = Vector2.Lerp(start, startContact, approach);
                if (approach >= 0.999f)
                {
                    pos = Vector2.Lerp(startContact, endContact, action);
                }

                if (action > 0f)
                {
                    DrawDragTrail(painter, startContact, pos, WithAlpha(Accent, 0.14f), 1.7f);
                }

                DrawTouchActor(painter, pos, visualPress, doubleTap);
            }
            else
            {
                Vector2 start = previousContact ?? GetDefaultActorStart(startContact);
                Vector2 pos = Vector2.Lerp(start, startContact, approach);
                if (approach >= 0.999f)
                {
                    pos = Vector2.Lerp(startContact, endContact, action);
                }

                if (action > 0f)
                {
                    DrawDragTrail(painter, startContact, pos, WithAlpha(Accent, 0.14f), 1.8f);
                }

                DrawCursorActor(painter, pos, visualPress, alternateMouse);
            }
        }

        private void DrawTargetClickPulse(Painter2D painter, Vector2 center, float phaseT, bool doubleTap, bool drag, Color color)
        {
            if (!doubleTap)
            {
                DrawClickRing(painter, center, GetClickPulseProgress(phaseT, drag), color);
                return;
            }

            float first = EaseOut(Phase01(phaseT, 0.42f, 0.62f));
            float second = EaseOut(Phase01(phaseT, 0.82f, 0.98f));
            DrawClickRing(painter, center, first, color);
            DrawClickRing(painter, center, second, WithAlpha(color, 0.92f));
            if (first > 0f)
            {
                FillCircle(painter, center, Mathf.Lerp(2f, 5f, first), WithAlpha(color, 0.08f));
            }

            if (second > 0f)
            {
                FillCircle(painter, center, Mathf.Lerp(2f, 5f, second), WithAlpha(color, 0.14f));
            }
        }

        private void GetPartInfoRects(float width, float height, float tabReveal, float panelOpen, out Rect part, out Rect tab, out Rect panel)
        {
            part = new Rect(width * 0.50f - 34f, height * 0.28f, 82f, 60f);

            float finalPanelWidth = width - 30f;
            float finalPanelHeight = 176f;
            float closedTabY = height + 14f;
            float openTabY = height - 44f + panelOpen * 16f;
            float tabY = Mathf.Lerp(closedTabY, openTabY, tabReveal);

            tab = new Rect((width - 122f) * 0.5f, tabY, 122f, 34f);

            float panelYClosed = height + 22f;
            float panelYOpen = height - finalPanelHeight;
            panel = new Rect(15f, Mathf.Lerp(panelYClosed, panelYOpen, panelOpen), finalPanelWidth, finalPanelHeight);
        }

        private void DrawPartInfoPeek(Painter2D painter, Rect tab, float visible, float focus)
        {
            if (visible <= 0f)
            {
                return;
            }

            FillRect(painter, tab, WithAlpha(Color.black, 0.96f * visible));
            StrokeRect(painter, tab, Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, focus * 0.80f), 1.15f);
            FillRect(painter, new Rect(tab.center.x - 16f, tab.y + 6f, 32f, 4f), WithAlpha(Foreground, 0.20f + focus * 0.14f));
        }

        private void DrawBottomSheetPanel(Painter2D painter, Rect panel, float open, out Rect closeButton, float closeHover = 0f, float closePress = 0f)
        {
            closeButton = default;
            if (open <= 0f)
            {
                return;
            }

            FillRect(painter, panel, new Color(0f, 0f, 0f, 0.82f));
            StrokeRect(painter, panel, WithAlpha(Foreground, 0.10f + open * 0.06f), 1.15f);
            closeButton = new Rect(panel.xMax - 38f, panel.y + 14f, 24f, 24f);
            Rect visualClose = GetInteractiveVisualRect(closeButton, closeHover, closePress, open, 0.03f, 0.05f, 0.04f);
            FillCircle(painter, visualClose.center, visualClose.width * 0.50f, WithAlpha(SurfaceAlt, 0.42f + open * 0.08f));
            StrokeCircle(painter, visualClose.center, visualClose.width * 0.50f, WithAlpha(Foreground, 0.22f + open * 0.12f), 1.2f);
            DrawLine(painter, visualClose.center + new Vector2(-4f, -4f), visualClose.center + new Vector2(4f, 4f), WithAlpha(Foreground, 0.86f), 1.7f);
            DrawLine(painter, visualClose.center + new Vector2(4f, -4f), visualClose.center + new Vector2(-4f, 4f), WithAlpha(Foreground, 0.86f), 1.7f);
            DrawContentLines(painter, new Rect(panel.x + 14f, panel.y + 18f, panel.width - 54f, panel.height - 26f), open);
        }

        private void DrawPinMarker(Painter2D painter, Vector2 point, float amount, Color color)
        {
            if (amount <= 0f) return;
            Color stroke = WithAlpha(color, Mathf.Lerp(0f, 0.88f, amount));
            DrawLine(painter, point + new Vector2(0f, 4f), point + new Vector2(0f, 14f), stroke, 2f);
            FillCircle(painter, point, 4f + amount * 1.4f, stroke);
            StrokeCircle(painter, point, 9f + amount * 4f, WithAlpha(color, 0.16f + amount * 0.12f), 1.2f);
        }

        private void DrawMenuButton(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active, float pressed, float alpha)
        {
            Rect visual = rect;
            visual.y += Mathf.Lerp(0f, 3f, pressed * 0.75f);
            FillCircle(painter, visual.center, visual.width * 0.34f, WithAlpha(AccentGlow, alpha * (0.12f + active * 0.16f)));
            DrawMiniIcon(painter, icon, visual.center, 14f, WithAlpha(Color.Lerp(ForegroundMuted, Accent, active), alpha));
            FillRect(painter, new Rect(visual.center.x - 12f, visual.yMax - 2f, 24f, 2f), WithAlpha(Accent, alpha * active));
        }

        private static Rect ScaleRect(Rect rect, float scale)
        {
            return new Rect(
                rect.center.x - rect.width * scale * 0.5f,
                rect.center.y - rect.height * scale * 0.5f,
                rect.width * scale,
                rect.height * scale);
        }

        private static void GetSelectionAssemblyRects(Vector2 center, out Rect neighbor, out Rect[] parentParts)
        {
            neighbor = new Rect(center.x - 132f, center.y - 58f, 66f, 112f);
            parentParts = new[]
            {
                new Rect(center.x - 20f, center.y - 54f, 40f, 30f),
                new Rect(center.x + 24f, center.y - 50f, 58f, 34f),
                new Rect(center.x - 12f, center.y - 10f, 54f, 40f),
                new Rect(center.x + 46f, center.y + 8f, 34f, 34f)
            };
        }

        private void DrawSelectionAssembly(Painter2D painter, Rect neighbor, Rect[] parentParts, float neighborVisible, float siblingVisible,
            float parentHighlight, int activeChild, float childHighlight, bool showHalos = true)
        {
            if (neighborVisible > 0.02f)
            {
                Color neighborFill = Color.Lerp(Surface, SurfaceAlt, 0.12f);
                neighborFill.a *= Mathf.Lerp(0.12f, 1f, neighborVisible);
                Color neighborStroke = WithAlpha(FrameStroke, Mathf.Lerp(0.20f, 1f, neighborVisible));
                DrawPartNode(painter, neighbor, neighborFill, neighborStroke, 1.2f);
            }

            Vector2 groupCenter = Vector2.zero;
            for (int i = 0; i < parentParts.Length; i++)
            {
                groupCenter += parentParts[i].center;
                bool isActiveChild = i == activeChild;
                float visible = isActiveChild ? 1f : siblingVisible;
                if (visible <= 0.02f)
                {
                    continue;
                }

                Color fill = SurfaceAlt;
                Color stroke = FrameStroke;
                float strokeWidth = 1.25f;
                if (parentHighlight > 0.01f)
                {
                    fill = Color.Lerp(fill, ParentSelectedFill, parentHighlight);
                    stroke = Color.Lerp(stroke, Accent, parentHighlight * 0.75f);
                    strokeWidth = 1.35f + parentHighlight * 0.35f;
                }

                if (isActiveChild && childHighlight > 0.01f)
                {
                    fill = Color.Lerp(fill, SelectedFill, childHighlight);
                    stroke = Color.Lerp(stroke, Accent, childHighlight);
                    strokeWidth = 1.5f + childHighlight * 0.45f;
                }

                fill.a *= Mathf.Lerp(0.10f, 1f, visible);
                stroke.a *= Mathf.Lerp(0.18f, 1f, visible);
                DrawPartNode(painter, parentParts[i], fill, stroke, strokeWidth);
            }

            groupCenter /= Mathf.Max(1, parentParts.Length);
            if (showHalos && parentHighlight > 0.01f)
            {
                DrawSelectionHalo(painter, groupCenter + new Vector2(14f, -2f), 40f, parentHighlight * (1f - childHighlight * 0.35f));
            }

            if (showHalos && childHighlight > 0.01f)
            {
                DrawSelectionHalo(painter, parentParts[activeChild].center, 24f, childHighlight);
            }
        }

        private static Vector2[] GetDroneRotorCenters(Vector2 center, float scale, float rotationDeg)
        {
            float armX = 30f * scale;
            float armY = 18f * scale;
            return new[]
            {
                center + Rotate(new Vector2(-armX, -armY), rotationDeg),
                center + Rotate(new Vector2(armX, -armY), rotationDeg),
                center + Rotate(new Vector2(-armX, armY), rotationDeg),
                center + Rotate(new Vector2(armX, armY), rotationDeg)
            };
        }

        private void DrawPowerHeatLayout(Painter2D painter, Vector2 center, float scale, float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            Vector2[] rotors = GetDroneRotorCenters(center, scale, 0f);
            float rotorSize = 11f * scale;
            for (int i = 0; i < rotors.Length; i++)
            {
                Rect rotorRect = new Rect(rotors[i].x - rotorSize * 0.5f, rotors[i].y - rotorSize * 0.5f, rotorSize, rotorSize);
                FillRect(painter, rotorRect, WithAlpha(ThermalHot, 0.22f + amount * 0.56f));
            }

            FillRect(painter, new Rect(center.x - 26f * scale, center.y - 15f * scale, 52f * scale, 8f * scale), WithAlpha(ThermalCool, 0.18f + amount * 0.42f));
            FillRect(painter, new Rect(center.x - 26f * scale, center.y + 7f * scale, 52f * scale, 8f * scale), WithAlpha(ThermalCool, 0.18f + amount * 0.42f));
            FillRect(painter, new Rect(center.x - 12f * scale, center.y - 4f * scale, 24f * scale, 10f * scale), WithAlpha(ThermalWarm, 0.24f + amount * 0.48f));
        }

        private void DrawStandaloneActionButton(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active, float hover, float press)
        {
            float active01 = Mathf.Clamp01(active);
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            float scale = 1f + hover01 * 0.06f + active01 * 0.14f + press01 * 0.04f;
            Rect visual = ScaleRect(rect, scale);
            visual.y += press01 * 1.4f;

            FillCircle(painter, visual.center, visual.width * 0.46f, WithAlpha(AccentGlow, 0.04f + active01 * 0.14f + hover01 * 0.08f));
            DrawAnimatedMiniIcon(painter, icon, visual.center + new Vector2(0f, -2f), visual.width * 0.30f, Color.Lerp(ForegroundMuted, Accent, Mathf.Clamp01(active01 * 0.9f + hover01 * 0.25f)), hover01, press01, active01);
            FillCapsule(painter, new Rect(visual.center.x - 16f, visual.yMax - 6f, 32f, 4f), WithAlpha(Accent, 0.12f + active01 * 0.72f));
        }

        private Rect GetInteractiveVisualRect(Rect rect, float hover, float press, float active, float activeScale = 0.06f, float hoverScale = 0.04f, float pressScale = 0.05f)
        {
            float scale = 1f
                + Mathf.Clamp01(active) * activeScale
                + Mathf.Clamp01(hover) * hoverScale
                + Mathf.Clamp01(press) * pressScale;
            Rect visual = ScaleRect(rect, scale);
            visual.y += Mathf.Clamp01(press) * 1.4f;
            return visual;
        }

        private Vector2 GetSliderThumb(Rect rect, float amount)
        {
            float margin = 16f;
            return new Vector2(Mathf.Lerp(rect.x + margin, rect.xMax - margin, Mathf.Clamp01(amount)), rect.center.y);
        }

        private void DrawActorTwoSegmentDrag(Painter2D painter, Vector2 startContact, Vector2 midContact, Vector2 endContact, float phaseT,
            Vector2? previousContact = null, bool alternateMouse = false)
        {
            if (phaseT < 0.56f)
            {
                DrawActorDragBetween(painter, startContact, midContact, Phase01(phaseT, 0.06f, 0.56f), false, previousContact, alternateMouse);
            }
            else
            {
                DrawActorDragBetween(painter, midContact, endContact, Phase01(phaseT, 0.56f, 1f), false, midContact, alternateMouse);
            }
        }

        private void DrawActorDoubleClickBounce(Painter2D painter, Vector2 target, float phaseT, Vector2 contactOffset = default,
            Vector2? startContact = null, Vector2? bounceContact = null, bool alternateMouse = false)
        {
            Vector2 contact = target + contactOffset;
            Vector2 start = startContact ?? GetDefaultActorStart(contact);
            Vector2 bounce = bounceContact ?? (contact + new Vector2(_isTouchMode ? 20f : 30f, -22f));
            float firstApproach = EaseInOut(Phase01(phaseT, 0.08f, 0.26f));
            float firstPress = EaseOut(Phase01(phaseT, 0.34f, 0.46f));
            float moveOut = EaseInOut(Phase01(phaseT, 0.48f, 0.62f));
            float moveBack = EaseInOut(Phase01(phaseT, 0.66f, 0.80f));
            float secondPress = EaseOut(Phase01(phaseT, 0.84f, 0.96f));
            float visualPress = Mathf.Max(firstPress, secondPress);
            Vector2 pos = phaseT < 0.48f
                ? Vector2.Lerp(start, contact, firstApproach)
                : (phaseT < 0.66f ? Vector2.Lerp(contact, bounce, moveOut) : Vector2.Lerp(bounce, contact, moveBack));

            if (_isTouchMode)
            {
                DrawTouchActor(painter, pos, visualPress, true);
            }
            else
            {
                DrawCursorActor(painter, pos, visualPress, alternateMouse);
            }
        }

        private static float RiseAndFall(float t, float riseStart = 0.12f, float riseEnd = 0.52f, float fallEnd = 0.94f)
        {
            if (t <= riseEnd)
            {
                return EaseOut(Phase01(t, riseStart, riseEnd));
            }

            float down = EaseInOut(Phase01(t, riseEnd, fallEnd));
            return Mathf.Lerp(1f, 0f, down);
        }

        private void DrawSubmenuCardButton(Painter2D painter, Rect rect, float active, float hover, float press, Action<Rect, Color> drawIcon)
        {
            float active01 = Mathf.Clamp01(active);
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            Rect visual = GetInteractiveVisualRect(rect, hover01, press01, active01, 0.16f, 0.08f, 0.05f);
            FillRect(painter, visual, WithAlpha(Surface, 0.10f + active01 * 0.10f + hover01 * 0.04f));
            StrokeRect(painter, visual, Color.Lerp(WithAlpha(Foreground, 0.14f), Accent, Mathf.Clamp01(active01 * 0.92f + hover01 * 0.30f)), 1.2f);
            Rect iconRect = new Rect(visual.x + 16f, visual.y + 12f, visual.width - 32f, visual.height - 38f);
            drawIcon?.Invoke(iconRect, Color.Lerp(ForegroundMuted, Accent, Mathf.Clamp01(active01 * 0.9f + hover01 * 0.25f)));
            FillCapsule(painter, new Rect(visual.center.x - 16f, visual.yMax - 8f, 32f, 4f), WithAlpha(Accent, 0.12f + active01 * 0.72f));
        }

        private void DrawAxisButton(Painter2D painter, Rect rect, CutButtonKind kind, float active, float hover, float press)
        {
            Rect visual = GetInteractiveVisualRect(rect, hover, press, active, 0.12f, 0.06f, 0.04f);
            FillRect(painter, visual, WithAlpha(Surface, 0.10f + active * 0.10f));
            StrokeRect(painter, visual, Color.Lerp(WithAlpha(Foreground, 0.12f), Accent, active), 1.2f);
            DrawCutGlyph(painter, visual.center, kind, visual.height * 0.32f, Color.Lerp(ForegroundMuted, Accent, Mathf.Clamp01(active * 0.9f + hover * 0.25f)));
        }

        private void DrawCutGlyph(Painter2D painter, Vector2 center, CutButtonKind kind, float size, Color color)
        {
            switch (kind)
            {
                case CutButtonKind.X:
                    DrawLine(painter, center + new Vector2(-size, -size), center + new Vector2(size, size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(size, -size), center + new Vector2(-size, size), color, 1.8f);
                    break;
                case CutButtonKind.Y:
                    DrawLine(painter, center + new Vector2(0f, 0f), center + new Vector2(0f, size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, 0f), center + new Vector2(-size, -size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, 0f), center + new Vector2(size, -size), color, 1.8f);
                    break;
                case CutButtonKind.Z:
                    DrawLine(painter, center + new Vector2(-size, -size), center + new Vector2(size, -size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(size, -size), center + new Vector2(-size, size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(-size, size), center + new Vector2(size, size), color, 1.8f);
                    break;
                case CutButtonKind.Invert:
                    DrawLine(painter, center + new Vector2(0f, -size), center + new Vector2(0f, size), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, -size), center + new Vector2(-4f, -size + 5f), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, -size), center + new Vector2(4f, -size + 5f), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, size), center + new Vector2(-4f, size - 5f), color, 1.8f);
                    DrawLine(painter, center + new Vector2(0f, size), center + new Vector2(4f, size - 5f), color, 1.8f);
                    break;
                case CutButtonKind.Angle:
                    DrawLine(painter, center + new Vector2(-size * 0.82f, size * 0.46f), center + new Vector2(-size * 0.12f, -size * 0.44f), color, 1.8f);
                    DrawLine(painter, center + new Vector2(-size * 0.12f, -size * 0.44f), center + new Vector2(size * 0.78f, size * 0.10f), color, 1.8f);
                    DrawArcSegment(painter, center + new Vector2(-size * 0.12f, 0f), size * 0.42f, 232f, 314f, WithAlpha(color, 0.58f), 1.15f);
                    break;
            }
        }

        private void DrawRenderOptionButton(Painter2D painter, Rect rect, RenderOptionKind kind, float active, float hover, float press)
        {
            DrawSubmenuCardButton(painter, rect, active, hover, press, (visual, color) =>
            {
                switch (kind)
                {
                    case RenderOptionKind.XRay:
                        DrawWireDrone(painter, visual.center + new Vector2(0f, -6f), 0.42f, color);
                        break;
                    case RenderOptionKind.Solid:
                        DrawDroneSilhouette(painter, visual.center + new Vector2(0f, -6f), 0.42f, 0f, 0f, WithAlpha(Foreground, 0.28f), Color.Lerp(Color.black, color, 0.10f));
                        break;
                    case RenderOptionKind.Thermal:
                        DrawDroneSilhouette(painter, visual.center + new Vector2(0f, -6f), 0.42f, 0f, 0f, color, WithAlpha(Surface, 0.18f));
                        DrawPowerHeatLayout(painter, visual.center + new Vector2(0f, -6f), 0.42f, 1f);
                        break;
                }
            });
        }

        private void DrawFilterButton(Painter2D painter, Rect rect, FilterGlyphKind kind, float active, float hover, float press)
        {
            float active01 = Mathf.Clamp01(active);
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            Rect visual = GetInteractiveVisualRect(rect, hover01, press01, active01, 0.12f, 0.06f, 0.04f);
            Vector2 c = visual.center + new Vector2(0f, -6f);
            float radius = Mathf.Min(visual.width, visual.height) * 0.32f;
            Color iconColor = Color.Lerp(ForegroundMuted, Accent, Mathf.Clamp01(active01 * 0.9f + hover01 * 0.25f));
            StrokeCircle(painter, c, radius, Color.Lerp(WithAlpha(Foreground, 0.18f), Accent, Mathf.Clamp01(active01 * 0.9f + hover01 * 0.25f)), 1.3f);
            if (active01 > 0.01f || hover01 > 0.01f)
            {
                FillCircle(painter, c, radius - 1f, WithAlpha(AccentGlow, 0.04f + active01 * 0.10f + hover01 * 0.06f));
            }

            float size = radius * 0.70f;
            switch (kind)
            {
                case FilterGlyphKind.Airframe:
                    StrokeRect(painter, new Rect(c.x - size, c.y - size, size * 2f, size * 2f), iconColor, 1.5f);
                    break;
                case FilterGlyphKind.Propulsion:
                    StrokeCircle(painter, c, size * 0.84f, iconColor, 1.4f);
                    DrawRotorBlades(painter, c, size * 0.75f, 0f, iconColor);
                    break;
                case FilterGlyphKind.Avionics:
                    FillRect(painter, new Rect(c.x - size * 0.8f, c.y - size * 0.8f, size * 1.6f, size * 1.6f), WithAlpha(iconColor, 0.18f));
                    StrokeRect(painter, new Rect(c.x - size * 0.8f, c.y - size * 0.8f, size * 1.6f, size * 1.6f), iconColor, 1.4f);
                    break;
                case FilterGlyphKind.Sensors:
                    StrokeCircle(painter, c, size * 0.82f, iconColor, 1.4f);
                    DrawLine(painter, c, c + new Vector2(size * 0.84f, 0f), iconColor, 1.4f);
                    break;
                case FilterGlyphKind.Power:
                    DrawPowerIconAnimated(painter, c, size * 1.8f, iconColor, hover01, press01);
                    break;
                case FilterGlyphKind.Fasteners:
                    DrawPinIcon(painter, c, size * 1.8f, iconColor);
                    break;
            }
        }

        private void DrawSharedCutPair(Painter2D painter, Rect rectA, Rect rectB, bool verticalCut, float cut, bool invert, Color fill, Color stroke)
        {
            Rect pair = Rect.MinMaxRect(
                Mathf.Min(rectA.x, rectB.x),
                Mathf.Min(rectA.y, rectB.y),
                Mathf.Max(rectA.xMax, rectB.xMax),
                Mathf.Max(rectA.yMax, rectB.yMax));

            if (verticalCut)
            {
                float x = Mathf.Lerp(pair.x + 12f, pair.xMax - 12f, cut);
                DrawCutRectAgainstPlane(painter, rectA, true, x, invert, fill, stroke);
                DrawCutRectAgainstPlane(painter, rectB, true, x, invert, fill, stroke);
                DrawLine(painter, new Vector2(x, pair.y - 8f), new Vector2(x, pair.yMax + 8f), stroke, 2.2f);
            }
            else
            {
                float y = Mathf.Lerp(pair.y + 12f, pair.yMax - 12f, cut);
                DrawCutRectAgainstPlane(painter, rectA, false, y, invert, fill, stroke);
                DrawCutRectAgainstPlane(painter, rectB, false, y, invert, fill, stroke);
                DrawLine(painter, new Vector2(pair.x - 8f, y), new Vector2(pair.xMax + 8f, y), stroke, 2.2f);
            }
        }

        private void DrawCutRectAgainstPlane(Painter2D painter, Rect rect, bool verticalCut, float plane, bool invert, Color fill, Color stroke)
        {
            if (verticalCut)
            {
                float x = Mathf.Clamp(plane, rect.x, rect.xMax);
                Rect visible = invert
                    ? new Rect(x, rect.y, rect.xMax - x, rect.height)
                    : new Rect(rect.x, rect.y, x - rect.x, rect.height);
                if (visible.width > 2f)
                {
                    FillRect(painter, visible, fill);
                    if (invert)
                    {
                        DrawRectBorderSegments(painter, visible, WithAlpha(stroke, 0.55f), 1.1f, false, true, true, true);
                    }
                    else
                    {
                        DrawRectBorderSegments(painter, visible, WithAlpha(stroke, 0.55f), 1.1f, true, true, false, true);
                    }
                }
            }
            else
            {
                float y = Mathf.Clamp(plane, rect.y, rect.yMax);
                Rect visible = invert
                    ? new Rect(rect.x, y, rect.width, rect.yMax - y)
                    : new Rect(rect.x, rect.y, rect.width, y - rect.y);
                if (visible.height > 2f)
                {
                    FillRect(painter, visible, fill);
                    if (invert)
                    {
                        DrawRectBorderSegments(painter, visible, WithAlpha(stroke, 0.55f), 1.1f, true, false, true, true);
                    }
                    else
                    {
                        DrawRectBorderSegments(painter, visible, WithAlpha(stroke, 0.55f), 1.1f, true, true, true, false);
                    }
                }
            }
        }

        private void DrawPartialLine(Painter2D painter, Vector2 from, Vector2 to, float reveal, Color color, float width)
        {
            if (reveal <= 0f)
            {
                return;
            }

            DrawLine(painter, from, Vector2.Lerp(from, to, EaseOut(reveal)), color, width);
        }

        private void DrawRectBorderSegments(Painter2D painter, Rect rect, Color color, float width, bool left, bool top, bool right, bool bottom)
        {
            if (left)
            {
                DrawLine(painter, new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.yMax), color, width);
            }

            if (top)
            {
                DrawLine(painter, new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.y), color, width);
            }

            if (right)
            {
                DrawLine(painter, new Vector2(rect.xMax, rect.y), new Vector2(rect.xMax, rect.yMax), color, width);
            }

            if (bottom)
            {
                DrawLine(painter, new Vector2(rect.x, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, width);
            }
        }

        private void DrawQuarterCutRect(Painter2D painter, Rect rect, float planeX, float planeY, bool keepLeft, bool keepTop, Color fill, Color stroke)
        {
            float x = Mathf.Clamp(planeX, rect.x, rect.xMax);
            float y = Mathf.Clamp(planeY, rect.y, rect.yMax);
            float visibleX = keepLeft ? rect.x : x;
            float visibleY = keepTop ? rect.y : y;
            float visibleWidth = keepLeft ? x - rect.x : rect.xMax - x;
            float visibleHeight = keepTop ? y - rect.y : rect.yMax - y;

            if (visibleWidth > 2f && visibleHeight > 2f)
            {
                Rect visible = new Rect(visibleX, visibleY, visibleWidth, visibleHeight);
                FillRect(painter, visible, fill);
                DrawRectBorderSegments(
                    painter,
                    visible,
                    WithAlpha(stroke, 0.55f),
                    1.1f,
                    keepLeft,
                    keepTop,
                    !keepLeft,
                    !keepTop);
            }
        }

        private void DrawNestedHorizontalCutPair(Painter2D painter, Rect outer, Rect inner, float planeY, float lineReveal, float fillReveal, Color fill, Color stroke)
        {
            Color visibleFill = WithAlpha(fill, fill.a * Mathf.Clamp01(fillReveal));
            DrawCutRectAgainstPlane(painter, outer, false, planeY, false, visibleFill, stroke);
            DrawCutRectAgainstPlane(painter, inner, false, planeY, false, visibleFill, stroke);
            DrawPartialLine(painter, new Vector2(outer.x - 8f, planeY), new Vector2(outer.xMax + 8f, planeY), lineReveal, stroke, 2.2f);
        }

        private void DrawNestedQuarterCutPair(Painter2D painter, Rect outer, Rect inner, float planeX, float planeY,
            bool keepLeft, bool keepTop, float horizontalReveal, float verticalReveal, float fillReveal, Color fill, Color stroke)
        {
            Color visibleFill = WithAlpha(fill, fill.a * Mathf.Clamp01(fillReveal));
            DrawQuarterCutRect(painter, outer, planeX, planeY, keepLeft, keepTop, visibleFill, stroke);
            DrawQuarterCutRect(painter, inner, planeX, planeY, keepLeft, keepTop, visibleFill, stroke);

            DrawPartialLine(painter, new Vector2(outer.x - 8f, planeY), new Vector2(outer.xMax + 8f, planeY), horizontalReveal, stroke, 2.0f);
            DrawPartialLine(painter, new Vector2(planeX, outer.y - 8f), new Vector2(planeX, outer.yMax + 8f), verticalReveal, stroke, 2.0f);
        }

        private void DrawSharedDiagonalCutPair(Painter2D painter, Rect rectA, Rect rectB, float amount, Color fill, Color stroke, bool invert = false)
        {
            Rect pair = Rect.MinMaxRect(
                Mathf.Min(rectA.x, rectB.x),
                Mathf.Min(rectA.y, rectB.y),
                Mathf.Max(rectA.xMax, rectB.xMax),
                Mathf.Max(rectA.yMax, rectB.yMax));

            if (amount <= 0.001f)
            {
                return;
            }

            DrawDiagonalCutBox(painter, rectA, amount, fill, stroke, invert, false);
            DrawDiagonalCutBox(painter, rectB, amount, fill, stroke, invert, false);

            Vector2 verticalA = new Vector2(pair.center.x, pair.y - 8f);
            Vector2 verticalB = new Vector2(pair.center.x, pair.yMax + 8f);
            Vector2 diagA = new Vector2(pair.x, pair.y);
            Vector2 diagB = new Vector2(pair.xMax, pair.yMax);
            DrawLine(
                painter,
                Vector2.Lerp(verticalA, diagA, amount),
                Vector2.Lerp(verticalB, diagB, amount),
                WithAlpha(stroke, 0.38f + amount * 0.30f),
                2.1f);
        }

        private void DrawDiagonalCutBox(Painter2D painter, Rect rect, float amount, Color fill, Color stroke, bool invert = false, bool drawCutLine = true)
        {
            Vector2 a = Vector2.Lerp(
                new Vector2(rect.x + 8f, rect.center.y),
                new Vector2(rect.x, rect.y),
                amount);
            Vector2 b = Vector2.Lerp(
                new Vector2(rect.xMax - 8f, rect.center.y),
                new Vector2(rect.xMax, rect.yMax),
                amount);
            if (drawCutLine)
            {
                DrawLine(painter, a, b, stroke, 2.3f);
            }

            Vector2[] kept = invert
                ? new[]
                {
                    a,
                    b,
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.x, rect.yMax)
                }
                : new[]
                {
                    new Vector2(rect.x, rect.y),
                    new Vector2(rect.xMax, rect.y),
                    b,
                    a
                };
            FillPolygon(painter, kept, fill);

            if (invert)
            {
                DrawLine(painter, b, new Vector2(rect.xMax, rect.yMax), WithAlpha(stroke, 0.55f), 1.1f);
                DrawLine(painter, new Vector2(rect.xMax, rect.yMax), new Vector2(rect.x, rect.yMax), WithAlpha(stroke, 0.55f), 1.1f);
                DrawLine(painter, new Vector2(rect.x, rect.yMax), a, WithAlpha(stroke, 0.55f), 1.1f);
            }
            else
            {
                DrawLine(painter, new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.y), WithAlpha(stroke, 0.55f), 1.1f);
                DrawLine(painter, new Vector2(rect.xMax, rect.y), b, WithAlpha(stroke, 0.55f), 1.1f);
                DrawLine(painter, a, new Vector2(rect.x, rect.y), WithAlpha(stroke, 0.55f), 1.1f);
            }
        }

        private void DrawExplodedDrone(Painter2D painter, Vector2 center, float scale, float amount)
        {
            float eased = amount <= 0f ? 0f : EaseOutBack(Mathf.Min(amount, 1f), 1.14f);
            float extraSeparation = amount <= 0.46f
                ? 0f
                : EaseOutBack(Mathf.InverseLerp(0.46f, 1.18f, Mathf.Min(amount, 1.18f)), 1.04f);
            float rotorSpread = (54f * eased + 38f * extraSeparation) * scale;
            float plateLiftA = (36f * eased + 20f * extraSeparation) * scale;
            float plateLiftB = (68f * eased + 28f * extraSeparation) * scale;
            Vector2 bodyCenter = center + new Vector2(0f, 8f * eased * scale);
            Vector2[] baseRotors = GetDroneRotorCenters(bodyCenter, scale, 0f);
            Vector2[] rotorOffsets =
            {
                new Vector2(-rotorSpread, -rotorSpread * 0.68f),
                new Vector2(rotorSpread, -rotorSpread * 0.68f),
                new Vector2(-rotorSpread, rotorSpread * 0.68f),
                new Vector2(rotorSpread, rotorSpread * 0.68f)
            };

            Rect body = new Rect(bodyCenter.x - 22f * scale, bodyCenter.y - 14f * scale, 44f * scale, 28f * scale);
            Rect upperPlate = new Rect(bodyCenter.x - 18f * scale, bodyCenter.y - 8f * scale - plateLiftA, 36f * scale, 12f * scale);
            Rect topPlate = new Rect(bodyCenter.x - 12f * scale, bodyCenter.y - 6f * scale - plateLiftB, 24f * scale, 10f * scale);
            DrawPartNode(painter, body, SurfaceAlt, Foreground, 1.4f);
            DrawPartNode(painter, upperPlate, WithAlpha(SurfaceAlt, 0.78f), WithAlpha(Foreground, 0.86f), 1.2f);
            DrawPartNode(painter, topPlate, WithAlpha(SurfaceAlt, 0.70f), WithAlpha(Foreground, 0.92f), 1.15f);

            Vector2[] bodyAnchors =
            {
                bodyCenter + new Vector2(-18f * scale, -10f * scale),
                bodyCenter + new Vector2(18f * scale, -10f * scale),
                bodyCenter + new Vector2(-18f * scale, 10f * scale),
                bodyCenter + new Vector2(18f * scale, 10f * scale)
            };

            for (int i = 0; i < baseRotors.Length; i++)
            {
                Vector2 rotor = baseRotors[i] + rotorOffsets[i];
                DrawLine(painter, bodyAnchors[i], rotor, WithAlpha(Foreground, 0.54f), 1.6f);
                StrokeCircle(painter, rotor, 11f * scale, Foreground, 1.6f);
                DrawRotorBlades(painter, rotor, 8.4f * scale, 0f, WithAlpha(Foreground, 0.40f));
            }

            Vector2 nose = bodyCenter + new Vector2(30f * scale, 0f);
            Vector2 noseLeft = bodyCenter + new Vector2(16f * scale, -6f * scale);
            Vector2 noseRight = bodyCenter + new Vector2(16f * scale, 6f * scale);
            FillPolygon(painter, new[] { noseLeft, nose, noseRight }, WithAlpha(Accent, 0.92f));
        }

        private void DrawMenuOption(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float reveal, float hover, float press, float active)
        {
            if (reveal <= 0f) return;

            float active01 = Mathf.Clamp01(active);
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            float scale = 1f + hover01 * 0.08f + active01 * 0.20f + press01 * 0.05f;
            Vector2 center = rect.center + new Vector2(0f, Mathf.Lerp(18f, 0f, reveal) + press01 * 1.6f);
            float iconSize = (rect.width * 0.32f + active01 * 8f) * scale;
            Color baseColor = Color.Lerp(ForegroundMuted, Foreground, hover01 * 0.38f);
            Color iconColor = WithAlpha(Color.Lerp(baseColor, Accent, active01), reveal);
            DrawAnimatedMiniIcon(painter, icon, center + new Vector2(0f, -4f), iconSize, iconColor, hover01, press01, active01);
            Color underline = Color.Lerp(WithAlpha(Foreground, 0.12f + hover01 * 0.10f), Accent, active01);
            FillCapsule(painter, new Rect(center.x - 26f, center.y + iconSize * 0.76f, 52f, 3f), WithAlpha(underline, reveal));
        }

        private void DrawRadialConnector(Painter2D painter, Vector2 from, Vector2 to, float reveal)
        {
            if (reveal <= 0f) return;
            Vector2 end = Vector2.Lerp(from, to, EaseOut(reveal));
            DrawLine(painter, from, end, WithAlpha(AccentSoft, 0.12f * reveal), 1.0f);
        }

        private void DrawModeBar(Painter2D painter, Rect rect)
        {
            DrawLine(painter, new Vector2(rect.x + 8f, rect.y + 10f), new Vector2(rect.xMax - 8f, rect.y + 10f), WithAlpha(Foreground, 0.20f), 1.15f);
        }

        private void DrawModeBarButton(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active, float hover, float press)
        {
            float emphasis = Mathf.Clamp01(active);
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            Rect visual = ScaleRect(rect, 1f + hover01 * 0.10f + emphasis * 0.22f + press01 * 0.05f);
            visual.y += press01 * 1.2f;
            float iconSize = 24f + hover01 * 2f + emphasis * 10f;
            Color baseColor = Color.Lerp(ForegroundMuted, Foreground, hover01 * 0.40f);
            Color iconColor = Color.Lerp(baseColor, Accent, emphasis);
            DrawAnimatedMiniIcon(painter, icon, visual.center + new Vector2(0f, -6f), iconSize, iconColor, hover01, press01, emphasis);
            Color lineColor = Color.Lerp(WithAlpha(Foreground, 0.16f + hover01 * 0.10f), Accent, emphasis);
            FillRect(painter, new Rect(visual.center.x - 18f, rect.y + 2f, 36f, 3f), lineColor);
        }

        private void DrawAnimatedMiniIcon(Painter2D painter, OnboardingMiniIcon icon, Vector2 center, float size, Color color, float hover, float press, float active)
        {
            float hover01 = Mathf.Clamp01(hover);
            float press01 = Mathf.Clamp01(press);
            float active01 = Mathf.Clamp01(active);

            switch (icon)
            {
                case OnboardingMiniIcon.Pin:
                {
                    float lift = -hover01 * size * 0.14f + press01 * size * 0.12f;
                    DrawLine(painter, center + new Vector2(-size * 0.24f, size * 0.52f), center + new Vector2(size * 0.24f, size * 0.52f), WithAlpha(color, 0.56f + hover01 * 0.24f), 1.4f);
                    DrawPinIcon(painter, center + new Vector2(0f, lift), size, color);
                    break;
                }
                case OnboardingMiniIcon.Isolate:
                {
                    float scale = Mathf.Lerp(1f, 0.82f, hover01);
                    scale = Mathf.Lerp(scale, 0.52f, press01);
                    DrawBracketIcon(painter, center, size * scale, color);
                    break;
                }
                case OnboardingMiniIcon.Power:
                {
                    DrawPowerIconAnimated(painter, center, size, color, hover01, press01);
                    break;
                }
                case OnboardingMiniIcon.Cut:
                {
                    DrawCutIconAnimated(painter, center, size, color, hover01, press01);
                    break;
                }
                case OnboardingMiniIcon.Inspect:
                {
                    DrawInspectIconAnimated(painter, center, size, color, hover01, press01);
                    break;
                }
                case OnboardingMiniIcon.Explode:
                {
                    DrawExplodeIconAnimated(painter, center, size, color, hover01, press01, active01);
                    break;
                }
                case OnboardingMiniIcon.Filter:
                {
                    DrawFilterIconAnimated(painter, center, size, color, hover01, press01);
                    break;
                }
                default:
                {
                    float scale = (1f + hover01 * 0.05f + active01 * 0.04f) * (1f - press01 * 0.05f);
                    DrawMiniIcon(painter, icon, center, size * scale, color);
                    break;
                }
            }
        }

        private void DrawPowerIconAnimated(Painter2D painter, Vector2 center, float size, Color color, float hover, float press)
        {
            float radius = size * 0.52f;
            DrawArcSegment(painter, center, radius, -58f, 238f, color, 1.7f);
            float stemScale = Mathf.Lerp(1f, 1.25f, hover);
            stemScale = Mathf.Lerp(stemScale, 0.56f, press);
            DrawLine(painter, center + new Vector2(0f, -size * 0.64f * stemScale), center + new Vector2(0f, -size * 0.06f), color, 1.8f);
        }

        private void DrawCutIconAnimated(Painter2D painter, Vector2 center, float size, Color color, float hover, float press)
        {
            float circleScale = 1f + hover * 0.10f;
            float radius = size * 0.50f * circleScale;
            StrokeCircle(painter, center, radius, WithAlpha(color, 0.42f), 1.35f);

            float track = Mathf.Lerp(0.56f, -0.96f, press);
            float trace = Mathf.Clamp01(press * 1.15f);
            Vector2 start = center + new Vector2(radius * 0.82f, -radius * 0.82f);
            Vector2 end = center + new Vector2(radius * track, -radius * track);
            if (trace > 0.01f)
            {
                DrawLine(painter, start, Vector2.Lerp(start, end, trace), color, 1.55f);
            }

            Vector2 knifeCenter = center + new Vector2(radius * track * 0.78f, -radius * track * 0.78f);
            float knifeAngle = -45f;
            Vector2 handleA = knifeCenter + Rotate(new Vector2(-size * 0.30f, -size * 0.06f), knifeAngle);
            Vector2 handleB = knifeCenter + Rotate(new Vector2(size * 0.04f, -size * 0.06f), knifeAngle);
            Vector2 bladeTip = knifeCenter + Rotate(new Vector2(size * 0.34f, 0f), knifeAngle);
            Vector2 bladeBelly = knifeCenter + Rotate(new Vector2(size * 0.08f, size * 0.12f), knifeAngle);
            Vector2 handleC = knifeCenter + Rotate(new Vector2(-size * 0.30f, size * 0.06f), knifeAngle);
            StrokePolygon(painter, new[] { handleA, handleB, bladeTip, bladeBelly, handleC }, color, 1.4f);
            DrawLine(painter, knifeCenter + Rotate(new Vector2(-size * 0.20f, 0f), knifeAngle), knifeCenter + Rotate(new Vector2(size * 0.02f, 0f), knifeAngle), WithAlpha(color, 0.74f), 1.1f);
        }

        private void DrawInspectIconAnimated(Painter2D painter, Vector2 center, float size, Color color, float hover, float press)
        {
            float spread = Mathf.Lerp(1f, 1.20f, hover);
            spread = Mathf.Lerp(spread, 0.84f, press);
            float half = size * 0.54f * spread;
            float arm = size * 0.28f;
            DrawLine(painter, center + new Vector2(-half, -half + arm), center + new Vector2(-half, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, -half), center + new Vector2(-half + arm, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half - arm, -half), center + new Vector2(half, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half, -half), center + new Vector2(half, -half + arm), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, half - arm), center + new Vector2(-half, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, half), center + new Vector2(-half + arm, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half - arm, half), center + new Vector2(half, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half, half), center + new Vector2(half, half - arm), color, 1.6f);
            FillCircle(painter, center, size * 0.10f, color);
            if (press > 0.01f)
            {
                DrawLine(painter, center + new Vector2(-half * 0.92f, 0f), center + new Vector2(half * 0.92f, 0f), WithAlpha(color, 0.88f), 1.6f);
            }
        }

        private void DrawExplodeIconAnimated(Painter2D painter, Vector2 center, float size, Color color, float hover, float press, float active)
        {
            float spread = Mathf.Lerp(0.20f, 0.34f, active);
            spread = Mathf.Lerp(spread, 0.12f, hover);
            spread = Mathf.Lerp(spread, 0.44f, press);
            float layerScale = Mathf.Lerp(1f, 1.08f, press);
            float halfW = size * 0.42f * layerScale;
            float halfH = halfW * 0.52f;
            DrawExplodeDiamondLayer(painter, center + new Vector2(0f, size * spread), halfW, halfH, WithAlpha(color, 0.30f), WithAlpha(color, 0.14f));
            DrawExplodeDiamondLayer(painter, center, halfW, halfH, WithAlpha(color, 0.66f), WithAlpha(color, 0.18f));
            DrawExplodeDiamondLayer(painter, center - new Vector2(0f, size * spread), halfW, halfH, color, WithAlpha(color, 0.24f));
        }

        private void DrawExplodeDiamondLayer(Painter2D painter, Vector2 center, float halfW, float halfH, Color stroke, Color fill)
        {
            Vector2[] points =
            {
                new Vector2(center.x, center.y - halfH),
                new Vector2(center.x + halfW, center.y),
                new Vector2(center.x, center.y + halfH),
                new Vector2(center.x - halfW, center.y)
            };
            FillPolygon(painter, points, fill);
            StrokePolygon(painter, points, stroke, 1.4f);
        }

        private void DrawFilterIconAnimated(Painter2D painter, Vector2 center, float size, Color color, float hover, float press)
        {
            float topWidth = Mathf.Lerp(0.78f, 1.0f, hover) * size * 0.42f;
            float tubeWidth = Mathf.Lerp(0.18f, 0.28f, hover) * size;
            float scaleY = Mathf.Lerp(1f, 0.74f, press);
            float topY = center.y - size * 0.34f * scaleY;
            float midY = center.y + size * 0.08f * scaleY;
            float bottomY = center.y + size * 0.38f;

            Vector2[] funnel =
            {
                new Vector2(center.x - topWidth, topY),
                new Vector2(center.x + topWidth, topY),
                new Vector2(center.x + tubeWidth, midY),
                new Vector2(center.x + tubeWidth, bottomY),
                new Vector2(center.x - tubeWidth, bottomY),
                new Vector2(center.x - tubeWidth, midY)
            };
            StrokePolygon(painter, funnel, color, 1.5f);

            if (press > 0.06f)
            {
                float dropY = Mathf.Lerp(bottomY + 2f, bottomY + size * 0.28f, press);
                FillCircle(painter, new Vector2(center.x, dropY), size * 0.10f, WithAlpha(color, 0.92f * (1f - press * 0.26f)));
            }
        }

        private void DrawStudioPanelCard(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active, float hover, float press, bool topCard)
        {
            Rect visual = GetInteractiveVisualRect(rect, hover, press, active, 0.12f, 0.08f, 0.05f);
            FillRect(painter, visual, WithAlpha(Surface, 0.14f + active * 0.10f));
            StrokeRect(painter, visual, Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, Mathf.Clamp01(active * 0.9f + hover * 0.25f)), 1.25f);

            Vector2 iconCenter = new Vector2(visual.x + 38f, visual.center.y - 4f);
            DrawAnimatedMiniIcon(painter, icon, iconCenter, 18f + active * 4f, Color.Lerp(ForegroundMuted, Accent, Mathf.Clamp01(active * 0.9f + hover * 0.30f)), hover, press, active);

            if (topCard)
            {
                Rect sampleA = new Rect(visual.xMax - 40f, visual.y + 20f, 12f, 18f);
                Rect sampleB = new Rect(visual.xMax - 22f, visual.y + 20f, 12f, 18f);
                StrokeRect(painter, sampleA, WithAlpha(Accent, 0.72f), 1.15f);
                StrokeRect(painter, sampleB, WithAlpha(Foreground, 0.56f), 1.15f);
            }
            else
            {
                FillCircle(painter, visual.xMax - 28f > 0f ? new Vector2(visual.xMax - 30f, visual.center.y - 6f) : visual.center, 6f, WithAlpha(Warning, 0.90f));
                FillCircle(painter, new Vector2(visual.xMax - 16f, visual.center.y + 8f), 4.5f, WithAlpha(Foreground, 0.74f));
            }
        }

        private void FillCapsule(Painter2D painter, Rect rect, Color color)
        {
            float radius = rect.height * 0.5f;
            if (rect.width <= rect.height)
            {
                FillCircle(painter, rect.center, radius, color);
                return;
            }

            FillRect(painter, new Rect(rect.x + radius, rect.y, rect.width - radius * 2f, rect.height), color);
            FillCircle(painter, new Vector2(rect.x + radius, rect.center.y), radius, color);
            FillCircle(painter, new Vector2(rect.xMax - radius, rect.center.y), radius, color);
        }

        private void StrokeCapsule(Painter2D painter, Rect rect, Color color, float width)
        {
            float radius = rect.height * 0.5f;
            Vector2 left = new Vector2(rect.x + radius, rect.center.y);
            Vector2 right = new Vector2(rect.xMax - radius, rect.center.y);
            StrokeCircle(painter, left, radius, color, width);
            StrokeCircle(painter, right, radius, color, width);
            DrawLine(painter, new Vector2(left.x, rect.y), new Vector2(right.x, rect.y), color, width);
            DrawLine(painter, new Vector2(left.x, rect.yMax), new Vector2(right.x, rect.yMax), color, width);
        }

        private void DrawPartNode(Painter2D painter, Rect rect, Color fill, Color stroke, float strokeWidth)
        {
            FillRect(painter, rect, fill);
            StrokeRect(painter, rect, stroke, strokeWidth);
        }

        private void DrawSelectionHalo(Painter2D painter, Vector2 center, float radius, float amount)
        {
            if (amount <= 0f) return;
            Color main = WithAlpha(SelectedFill, 0.24f + amount * 0.20f);
            Color outer = WithAlpha(SelectedFill, 0.08f + amount * 0.10f);
            StrokeCircle(painter, center, radius + amount * 4f, main, 2f);
            StrokeCircle(painter, center, radius + 10f + amount * 5f, outer, 1.25f);
        }

        private void DrawChipWithIcon(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active)
        {
            FillRect(painter, rect, WithAlpha(Color.clear, 0f));
            StrokeRect(painter, rect, Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, active), 1.25f);
            DrawMiniIcon(painter, icon, rect.center, rect.width * 0.22f, Color.Lerp(ForegroundMuted, Foreground, active));
        }

        private void DrawAxisChip(Painter2D painter, Rect rect, float active)
        {
            FillRect(painter, rect, Color.Lerp(Surface, SurfaceAlt, active * 0.30f));
            StrokeRect(painter, rect, Color.Lerp(FrameStroke, Accent, active), 1.2f);
            DrawLine(painter, rect.center + new Vector2(-7f, 7f), rect.center + new Vector2(7f, -7f), Color.Lerp(ForegroundMuted, Foreground, active), 2f);
        }

        private void DrawFilterChip(Painter2D painter, Rect rect, float active)
        {
            FillRect(painter, rect, WithAlpha(Color.clear, 0f));
            StrokeRect(painter, rect, Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, active), 1.25f);
            DrawMiniIcon(painter, OnboardingMiniIcon.Filter, rect.center, rect.width * 0.22f, Color.Lerp(ForegroundMuted, Foreground, active));
        }

        private void DrawModeChip(Painter2D painter, Rect rect, float active)
        {
            FillRect(painter, rect, WithAlpha(Surface, 0.32f + active * 0.12f));
            StrokeRect(painter, rect, Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, active), 1.2f);
        }

        private void DrawSlider(Painter2D painter, Rect rect, float amount, Color color, bool active = false, float hover = 0f, float pressed = 0f)
        {
            float y = rect.center.y;
            float amount01 = Mathf.Clamp01(amount);
            float hover01 = Mathf.Clamp01(hover);
            float pressed01 = Mathf.Clamp01(pressed);
            float margin = 16f;
            float trackWidth = rect.width - margin * 2f;
            float thumbRadius = Mathf.Lerp(10f, 14f, hover01);
            thumbRadius = Mathf.Lerp(thumbRadius, 11f, pressed01);
            Vector2 thumb = new Vector2(rect.x + margin + trackWidth * amount01, y);
            Color thumbFill = Color.black;
            Color thumbStroke = WithAlpha(Foreground, 0.60f);

            DrawLine(painter, new Vector2(rect.x + margin, y), new Vector2(rect.xMax - margin, y), WithAlpha(Foreground, 0.12f), 4f);

            if (active && hover01 > 0.01f)
            {
                thumbFill = Color.Lerp(Color.black, Foreground, hover01);
                thumbStroke = Color.Lerp(thumbStroke, WithAlpha(Foreground, 0f), hover01);
            }

            if (pressed01 > 0.01f)
            {
                thumbFill = Color.Lerp(thumbFill, color, pressed01);
                thumbStroke = Color.Lerp(thumbStroke, WithAlpha(color, 0f), pressed01);
            }

            FillCircle(painter, thumb, thumbRadius + 10f, WithAlpha(AccentGlow, active ? 0.10f + hover01 * 0.10f + pressed01 * 0.08f : 0.08f));
            FillCircle(painter, thumb, thumbRadius, thumbFill);
            if (thumbStroke.a > 0.02f)
            {
                StrokeCircle(painter, thumb, thumbRadius, thumbStroke, 2f);
            }
        }

        private void DrawAssemblyLayer(Painter2D painter, Rect rect, Color fill, Color stroke)
        {
            FillRect(painter, rect, fill);
            StrokeRect(painter, rect, stroke, 1.2f);
        }

        private void DrawBoxShell(Painter2D painter, Rect rect, Color stroke)
        {
            StrokeRect(painter, rect, stroke, 1.2f);
            StrokeRect(painter, new Rect(rect.x + 10f, rect.y + 10f, rect.width - 20f, rect.height - 20f), WithAlpha(stroke, 0.55f), 1f);
        }

        private void DrawCutPlane(Painter2D painter, Rect box, float normalizedX, bool horizontal, Color color)
        {
            if (horizontal)
            {
                float y = Mathf.Lerp(box.y + 16f, box.yMax - 16f, normalizedX);
                DrawLine(painter, new Vector2(box.x + 6f, y), new Vector2(box.xMax - 6f, y), color, 2.4f);
            }
            else
            {
                float x = Mathf.Lerp(box.x + 16f, box.xMax - 16f, normalizedX);
                DrawLine(painter, new Vector2(x, box.y + 6f), new Vector2(x, box.yMax - 6f), color, 2.4f);
            }
        }

        private void DrawInclinedPlane(Painter2D painter, Rect box, float amount)
        {
            Vector2 a = new Vector2(box.x + 14f, Mathf.Lerp(box.y + 18f, box.y + 4f, amount));
            Vector2 b = new Vector2(box.xMax - 14f, Mathf.Lerp(box.yMax - 18f, box.yMax - 4f, amount));
            DrawLine(painter, a, b, WithAlpha(Warning, 0.88f), 2.6f);
        }

        private void DrawThermalBands(Painter2D painter, Rect rect, float amount)
        {
            if (amount <= 0f) return;
            FillRect(painter, new Rect(rect.x + 10f, rect.y + 14f, rect.width - 20f, 12f), WithAlpha(ThermalHot, 0.18f + amount * 0.26f));
            FillRect(painter, new Rect(rect.x + 6f, rect.y + rect.height * 0.42f, rect.width - 12f, 14f), WithAlpha(ThermalWarm, 0.14f + amount * 0.22f));
            FillRect(painter, new Rect(rect.x + 12f, rect.yMax - 30f, rect.width - 24f, 12f), WithAlpha(ThermalCool, 0.14f + amount * 0.18f));
        }

        private void DrawDroneSilhouette(Painter2D painter, Vector2 center, float scale, float rotationDeg, float rotorSpinDeg, Color stroke, Color fill)
        {
            float bodyHalfW = 22f * scale;
            float bodyHalfH = 14f * scale;
            Vector2[] body =
            {
                center + Rotate(new Vector2(-bodyHalfW, -bodyHalfH), rotationDeg),
                center + Rotate(new Vector2(bodyHalfW, -bodyHalfH), rotationDeg),
                center + Rotate(new Vector2(bodyHalfW, bodyHalfH), rotationDeg),
                center + Rotate(new Vector2(-bodyHalfW, bodyHalfH), rotationDeg)
            };

            FillPolygon(painter, body, fill);
            StrokePolygon(painter, body, stroke, 1.6f);

            float armX = 30f * scale;
            float armY = 18f * scale;
            float rotorRadius = 10f * scale;
            Vector2[] rotors =
            {
                center + Rotate(new Vector2(-armX, -armY), rotationDeg),
                center + Rotate(new Vector2(armX, -armY), rotationDeg),
                center + Rotate(new Vector2(-armX, armY), rotationDeg),
                center + Rotate(new Vector2(armX, armY), rotationDeg)
            };

            foreach (Vector2 rotor in rotors)
            {
                DrawLine(painter, center, rotor, WithAlpha(stroke, 0.78f), 1.6f);
                StrokeCircle(painter, rotor, rotorRadius, stroke, 1.6f);
                DrawRotorBlades(painter, rotor, rotorRadius * 0.88f, rotorSpinDeg, WithAlpha(stroke, 0.40f));
            }

            Vector2 nose = center + Rotate(new Vector2(bodyHalfW + 8f * scale, 0f), rotationDeg);
            Vector2 noseLeft = center + Rotate(new Vector2(bodyHalfW - 2f * scale, -4f * scale), rotationDeg);
            Vector2 noseRight = center + Rotate(new Vector2(bodyHalfW - 2f * scale, 4f * scale), rotationDeg);
            FillPolygon(painter, new[] { noseLeft, nose, noseRight }, WithAlpha(Accent, 0.92f));
        }

        private void DrawWireDrone(Painter2D painter, Vector2 center, float scale, Color color)
        {
            float bodyW = 40f * scale;
            float bodyH = 28f * scale;
            StrokeRect(painter, new Rect(center.x - bodyW * 0.5f, center.y - bodyH * 0.5f, bodyW, bodyH), color, 1.4f);
            DrawLine(painter, center + new Vector2(-bodyW * 0.5f, -bodyH * 0.5f), center + new Vector2(bodyW * 0.5f, bodyH * 0.5f), WithAlpha(color, 0.70f), 1f);
            DrawLine(painter, center + new Vector2(bodyW * 0.5f, -bodyH * 0.5f), center + new Vector2(-bodyW * 0.5f, bodyH * 0.5f), WithAlpha(color, 0.70f), 1f);
            DrawDroneSilhouette(painter, center, scale, 0f, 0f, color, WithAlpha(Color.clear, 0f));
        }

        private void DrawRotorBlades(Painter2D painter, Vector2 center, float radius, float angleDeg, Color color)
        {
            Vector2 a = center + Rotate(new Vector2(radius, 0f), angleDeg);
            Vector2 b = center + Rotate(new Vector2(-radius, 0f), angleDeg);
            Vector2 c = center + Rotate(new Vector2(0f, radius), angleDeg + 25f);
            Vector2 d = center + Rotate(new Vector2(0f, -radius), angleDeg + 25f);
            DrawLine(painter, a, b, color, 1f);
            DrawLine(painter, c, d, color, 1f);
        }

        private void DrawFramingBox(Painter2D painter, Vector2 center, float droneScale, Color color, float width)
        {
            Rect frame = new Rect(center.x - 38f * droneScale, center.y - 30f * droneScale, 76f * droneScale, 60f * droneScale);
            StrokeRect(painter, frame, color, width);
        }

        private void DrawIconButton(Painter2D painter, Rect rect, OnboardingMiniIcon icon, float active, float pressed)
        {
            Vector2 center = rect.center + new Vector2(0f, pressed * 1.0f);
            float radius = rect.width * (0.5f + pressed * 0.03f);
            FillCircle(painter, center, radius, WithAlpha(Color.clear, 0f));
            StrokeCircle(painter, center, radius + 2.2f, WithAlpha(Color.Lerp(WithAlpha(Foreground, 0.10f), Accent, active), 0.96f), 1.2f);
            FillCircle(painter, center, radius * 0.92f, WithAlpha(AccentGlow, active * 0.16f));
            DrawMiniIcon(painter, icon, center, radius * 0.55f, Color.Lerp(ForegroundMuted, Foreground, active));
        }

        private void DrawMouseHint(Painter2D painter, Vector2 center, MouseHintButton button)
        {
            Rect rect = new Rect(center.x - 24f, center.y - 34f, 48f, 68f);
            Vector2[] outline =
            {
                new Vector2(rect.center.x, rect.y),
                new Vector2(rect.xMax - 4f, rect.y + 6f),
                new Vector2(rect.xMax, rect.y + 18f),
                new Vector2(rect.xMax - 1f, rect.yMax - 8f),
                new Vector2(rect.center.x, rect.yMax),
                new Vector2(rect.x + 1f, rect.yMax - 8f),
                new Vector2(rect.x, rect.y + 18f),
                new Vector2(rect.x + 4f, rect.y + 6f)
            };
            StrokePolygon(painter, outline, WithAlpha(Foreground, 0.68f), 1.25f);

            DrawLine(painter, new Vector2(rect.center.x, rect.y + 8f), new Vector2(rect.center.x, rect.yMax - 10f), WithAlpha(ForegroundMuted, 0.18f), 1.1f);
            if (button == MouseHintButton.Right)
            {
                FillRect(painter, new Rect(rect.center.x, rect.y + 6f, rect.width * 0.5f - 2f, 17f), WithAlpha(Accent, 0.82f));
            }
            else
            {
                FillRect(painter, new Rect(rect.center.x - 6f, rect.y + 7f, 12f, 20f), WithAlpha(Accent, 0.82f));
            }
        }

        // The arrow represents wheel rotation direction, not the drone movement:
        // scrollUp = wheel forward/up, scrollDown = wheel backward/down.
        private void DrawWheelMouseHint(Painter2D painter, Vector2 center, float amount, bool scrollUp)
        {
            DrawMouseHint(painter, center, MouseHintButton.Middle);

            Rect wheel = new Rect(center.x - 7f, center.y - 16f, 14f, 24f);
            FillRect(painter, wheel, WithAlpha(Accent, 0.92f));

            float arrowTravel = Mathf.Lerp(0f, 18f, amount);
            Vector2 baseArrow = center + new Vector2(44f, 0f);
            Vector2 dir = scrollUp ? Vector2.up : Vector2.down;
            Vector2 tip = baseArrow + dir * (14f + arrowTravel);
            DrawLine(painter, baseArrow - dir * 15f, tip, WithAlpha(Accent, 0.86f), 2.4f);
            DrawLine(painter, tip, tip - dir * 8f + new Vector2(-6f, scrollUp ? 6f : -6f), WithAlpha(Accent, 0.86f), 2.4f);
            DrawLine(painter, tip, tip - dir * 8f + new Vector2(6f, scrollUp ? 6f : -6f), WithAlpha(Accent, 0.86f), 2.4f);
        }

        private void DrawLightBall(Painter2D painter, Vector2 center, float rotation, float intensity)
        {
            float bgRadius = 18f + intensity * 20f;
            FillCircle(painter, center, bgRadius + 14f, WithAlpha(AccentGlow, 0.10f + intensity * 0.14f));
            FillCircle(painter, center, bgRadius, WithAlpha(Foreground, 0.18f + intensity * 0.26f));
            float lightDistance = 30f;
            Vector2 light = center + new Vector2(Mathf.Cos(Mathf.Lerp(-1.8f, 0.7f, rotation)), Mathf.Sin(Mathf.Lerp(-1.8f, 0.7f, rotation))) * lightDistance;
            DrawLine(painter, center, light, WithAlpha(Warning, 0.72f), 2.2f);
            FillCircle(painter, light, 6.8f + intensity * 2.8f, Warning);
        }

        private void DrawBlueprintLines(Painter2D painter, Rect rect)
        {
            DrawLine(painter, new Vector2(rect.x + 12f, rect.y + 18f), new Vector2(rect.xMax - 12f, rect.y + 18f), WithAlpha(Foreground, 0.20f), 1f);
            DrawLine(painter, new Vector2(rect.x + 12f, rect.y + 42f), new Vector2(rect.xMax - 12f, rect.y + 42f), WithAlpha(Foreground, 0.20f), 1f);
            DrawLine(painter, new Vector2(rect.x + 16f, rect.yMax - 24f), new Vector2(rect.xMax - 16f, rect.yMax - 24f), WithAlpha(Foreground, 0.20f), 1f);
        }

        private void DrawSunMoonCue(Painter2D painter, Rect rect, float phaseT)
        {
            float blend = EaseInOut(Phase01(phaseT, 0.22f, 0.82f));
            Vector2 sun = new Vector2(Mathf.Lerp(rect.x + 22f, rect.center.x, blend), Mathf.Lerp(rect.y + 22f, rect.y + 16f, blend));
            Vector2 moon = new Vector2(Mathf.Lerp(rect.center.x, rect.xMax - 22f, blend), Mathf.Lerp(rect.y + 18f, rect.y + 24f, blend));
            FillCircle(painter, sun, 6f, Warning);
            StrokeCircle(painter, moon, 6f, WithAlpha(Foreground, 0.88f), 1.6f);
        }

        private void DrawColorPresetSwatches(Painter2D painter, Rect rect, float phaseT)
        {
            float select = EaseOut(Phase01(phaseT, 0.18f, 0.84f));
            Rect a = new Rect(rect.x, rect.y, 38f, rect.height);
            Rect b = new Rect(rect.x + 50f, rect.y, 38f, rect.height);
            Rect c = new Rect(rect.x + 100f, rect.y, 38f, rect.height);
            FillRect(painter, a, new Color(0.21f, 0.53f, 0.96f, 0.78f));
            FillRect(painter, b, new Color(0.90f, 0.34f, 0.30f, 0.78f));
            FillRect(painter, c, new Color(0.23f, 0.84f, 0.62f, 0.78f));
            StrokeRect(painter, b, WithAlpha(Foreground, 0.18f + select * 0.34f), 1.2f);
        }

        private void DrawContentLines(Painter2D painter, Rect rect, float alpha)
        {
            if (alpha <= 0f) return;
            Color stroke = WithAlpha(Foreground, alpha * 0.42f);
            DrawLine(painter, new Vector2(rect.x, rect.y + 8f), new Vector2(rect.x + rect.width * 0.68f, rect.y + 8f), stroke, 2f);
            DrawLine(painter, new Vector2(rect.x, rect.y + 28f), new Vector2(rect.x + rect.width * 0.88f, rect.y + 28f), WithAlpha(ForegroundMuted, alpha * 0.44f), 1.6f);
            DrawLine(painter, new Vector2(rect.x, rect.y + 44f), new Vector2(rect.x + rect.width * 0.82f, rect.y + 44f), WithAlpha(ForegroundMuted, alpha * 0.44f), 1.6f);
            DrawLine(painter, new Vector2(rect.x, rect.y + 60f), new Vector2(rect.x + rect.width * 0.76f, rect.y + 60f), WithAlpha(ForegroundMuted, alpha * 0.44f), 1.6f);
        }

        private void DrawCursorActor(Painter2D painter, Vector2 position, float pressAmount, bool alternate)
        {
            float press = Mathf.Clamp01(pressAmount);
            float scale = Mathf.Lerp(1f, 0.88f, press);
            Vector2 tip = position + new Vector2(press * 1.5f, press * 2f);
            Vector2[] points =
            {
                tip,
                tip + new Vector2(0f, 24f * scale),
                tip + new Vector2(7f * scale, 19f * scale),
                tip + new Vector2(11f * scale, 29f * scale),
                tip + new Vector2(16f * scale, 27f * scale),
                tip + new Vector2(11f * scale, 16f * scale),
                tip + new Vector2(19f * scale, 16f * scale)
            };

            FillPolygon(painter, points, Foreground);
            StrokePolygon(painter, points, WithAlpha(Surface, 0.88f), 1.1f);

            if (alternate)
            {
                DrawLine(painter, tip + new Vector2(10f, 12f), tip + new Vector2(16f, 12f), WithAlpha(Accent, 0.72f), 1.6f);
            }
        }

        private void DrawTouchActor(Painter2D painter, Vector2 position, float pressAmount, bool doubleTap)
        {
            float press = Mathf.Clamp01(pressAmount);
            float outerRadius = Mathf.Lerp(8f, 6.6f, press);
            float innerRadius = Mathf.Lerp(3.2f, 2.6f, press);
            FillCircle(painter, position, outerRadius, WithAlpha(Foreground, 0.92f));
            FillCircle(painter, position, innerRadius, Accent);
            if (press > 0.01f)
            {
                DrawRipple(painter, position, 0.38f + press * 0.18f, Accent);
                if (doubleTap)
                {
                    DrawRipple(painter, position, 0.62f + press * 0.18f, WithAlpha(Accent, 0.72f));
                }
            }
        }

        private void DrawPinchActor(Painter2D painter, Vector2 a, Vector2 b, float amount)
        {
            DrawTouchActor(painter, a, 1f, false);
            DrawTouchActor(painter, b, 1f, false);
            DrawLine(painter, a, b, WithAlpha(Accent, 0.22f + amount * 0.12f), 1.4f);
        }

        private void DrawClickRing(Painter2D painter, Vector2 center, float amount, Color color)
        {
            if (amount <= 0f) return;
            StrokeCircle(painter, center, Mathf.Lerp(6f, 18f, amount), WithAlpha(color, 1f - amount * 0.72f), 2f - amount * 0.6f);
        }

        private void DrawRipple(Painter2D painter, Vector2 center, float amount, Color color)
        {
            if (amount <= 0f) return;
            StrokeCircle(painter, center, Mathf.Lerp(7f, 20f, amount), WithAlpha(color, 0.88f - amount * 0.64f), 2f - amount * 0.5f);
        }

        private void DrawDragTrail(Painter2D painter, Vector2 from, Vector2 to, Color color, float width)
        {
            DrawLine(painter, from, to, color, width);
        }

        private void DrawOrbitArrow(Painter2D painter, Vector2 center, float radius, float angleDeg, Color color)
        {
            float angle = angleDeg * Mathf.Deg2Rad;
            Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 tangent = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
            DrawLine(painter, pos - tangent * 6f, pos, color, 1.8f);
            DrawLine(painter, pos, pos - tangent * 4f + new Vector2(2f, -2f), color, 1.8f);
        }

        private void DrawPanGuides(Painter2D painter, Vector2 center, float amount)
        {
            if (amount <= 0f) return;
            DrawLine(painter, center + new Vector2(-26f, -34f), center + new Vector2(-26f + amount * 18f, -34f + amount * 6f), WithAlpha(Accent, 0.34f), 1.4f);
            DrawLine(painter, center + new Vector2(26f, 34f), center + new Vector2(26f + amount * 18f, 34f + amount * 6f), WithAlpha(Accent, 0.24f), 1.4f);
        }

        private void DrawArcSegment(Painter2D painter, Vector2 center, float radius, float startDeg, float endDeg, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.Arc(center, radius, startDeg, endDeg);
            painter.Stroke();
        }

        private static void FillRect(Painter2D painter, Rect rect, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Fill();
        }

        private static void StrokeRect(Painter2D painter, Rect rect, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Stroke();
        }

        private static void FillCircle(Painter2D painter, Vector2 center, float radius, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.Arc(center, radius, 0f, 360f);
            painter.Fill();
        }

        private static void StrokeCircle(Painter2D painter, Vector2 center, float radius, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.Arc(center, radius, 0f, 360f);
            painter.Stroke();
        }

        private static void DrawLine(Painter2D painter, Vector2 from, Vector2 to, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }

        private static void FillPolygon(Painter2D painter, Vector2[] points, Color color)
        {
            if (points == null || points.Length < 3) return;
            painter.fillColor = color;
            painter.BeginPath();
            painter.MoveTo(points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                painter.LineTo(points[i]);
            }

            painter.ClosePath();
            painter.Fill();
        }

        private static void StrokePolygon(Painter2D painter, Vector2[] points, Color color, float width)
        {
            if (points == null || points.Length < 3) return;
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                painter.LineTo(points[i]);
            }

            painter.ClosePath();
            painter.Stroke();
        }

        private void DrawMiniIcon(Painter2D painter, OnboardingMiniIcon icon, Vector2 center, float size, Color color)
        {
            switch (icon)
            {
                case OnboardingMiniIcon.Pin:
                    DrawPinIcon(painter, center, size, color);
                    break;
                case OnboardingMiniIcon.Isolate:
                    DrawBracketIcon(painter, center, size, color);
                    break;
                case OnboardingMiniIcon.Power:
                    DrawPowerIcon(painter, center, size, color);
                    break;
                case OnboardingMiniIcon.Analyze:
                    DrawBarsIcon(painter, center, size, color);
                    break;
                case OnboardingMiniIcon.Cut:
                    StrokeCircle(painter, center, size * 0.52f, WithAlpha(color, 0.42f), 1.3f);
                    DrawLine(painter, center + new Vector2(-size * 0.54f, size * 0.54f), center + new Vector2(size * 0.54f, -size * 0.54f), color, 1.8f);
                    break;
                case OnboardingMiniIcon.Explode:
                    StrokePolygon(painter, new[]
                    {
                        center + new Vector2(0f, -size * 0.54f),
                        center + new Vector2(size * 0.42f, -size * 0.30f),
                        center + new Vector2(0f, -size * 0.06f),
                        center + new Vector2(-size * 0.42f, -size * 0.30f)
                    }, WithAlpha(color, 0.48f), 1.3f);
                    StrokePolygon(painter, new[]
                    {
                        center + new Vector2(0f, -size * 0.20f),
                        center + new Vector2(size * 0.52f, 0f),
                        center + new Vector2(0f, size * 0.20f),
                        center + new Vector2(-size * 0.52f, 0f)
                    }, color, 1.3f);
                    StrokePolygon(painter, new[]
                    {
                        center + new Vector2(0f, size * 0.14f),
                        center + new Vector2(size * 0.42f, size * 0.38f),
                        center + new Vector2(0f, size * 0.62f),
                        center + new Vector2(-size * 0.42f, size * 0.38f)
                    }, WithAlpha(color, 0.76f), 1.3f);
                    break;
                case OnboardingMiniIcon.Filter:
                    StrokePolygon(painter, new[]
                    {
                        center + new Vector2(-size * 0.54f, -size * 0.40f),
                        center + new Vector2(size * 0.54f, -size * 0.40f),
                        center + new Vector2(size * 0.20f, -size * 0.02f),
                        center + new Vector2(size * 0.20f, size * 0.52f),
                        center + new Vector2(-size * 0.20f, size * 0.52f),
                        center + new Vector2(-size * 0.20f, -size * 0.02f)
                    }, color, 1.55f);
                    break;
                case OnboardingMiniIcon.Studio:
                    DrawStudioIcon(painter, center, size, color);
                    break;
                case OnboardingMiniIcon.Render:
                {
                    Rect box = new Rect(center.x - size * 0.48f, center.y - size * 0.48f, size * 0.96f, size * 0.96f);
                    StrokeRect(painter, box, color, 1.35f);
                    float splitX = center.x + size * 0.08f;
                    DrawLine(painter, new Vector2(splitX, box.y - 2f), new Vector2(splitX, box.yMax + 2f), color, 1.45f);
                    DrawLine(painter, new Vector2(box.x + size * 0.12f, center.y - size * 0.18f), new Vector2(splitX, center.y - size * 0.18f), WithAlpha(color, 0.70f), 1.05f);
                    DrawLine(painter, new Vector2(box.x + size * 0.12f, center.y + size * 0.18f), new Vector2(splitX, center.y + size * 0.18f), WithAlpha(color, 0.70f), 1.05f);
                    DrawLine(painter, new Vector2(center.x + size * 0.22f, box.y + size * 0.18f), new Vector2(center.x + size * 0.22f, box.yMax - size * 0.18f), WithAlpha(color, 0.56f), 1.0f);
                    break;
                }
                case OnboardingMiniIcon.Environment:
                    DrawLine(painter, center + new Vector2(-size * 0.52f, size * 0.18f), center + new Vector2(size * 0.52f, size * 0.18f), color, 1.6f);
                    FillCircle(painter, center + new Vector2(size * 0.18f, -size * 0.16f), size * 0.16f, color);
                    break;
                case OnboardingMiniIcon.Lighting:
                    DrawLine(painter, center + new Vector2(0f, -size * 0.54f), center + new Vector2(0f, size * 0.54f), color, 1.5f);
                    DrawLine(painter, center + new Vector2(-size * 0.54f, 0f), center + new Vector2(size * 0.54f, 0f), color, 1.5f);
                    FillCircle(painter, center, size * 0.14f, color);
                    break;
                case OnboardingMiniIcon.Info:
                    StrokeCircle(painter, center, size * 0.46f, color, 1.4f);
                    DrawLine(painter, center + new Vector2(0f, -size * 0.12f), center + new Vector2(0f, size * 0.20f), color, 1.6f);
                    FillCircle(painter, center + new Vector2(0f, -size * 0.30f), size * 0.06f, color);
                    break;
                case OnboardingMiniIcon.Inspect:
                    DrawBracketIcon(painter, center, size, color);
                    FillCircle(painter, center, size * 0.10f, color);
                    break;
                case OnboardingMiniIcon.Reset:
                    DrawResetIcon(painter, center, size, color);
                    break;
            }
        }

        private static void DrawBracketIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            float half = size * 0.54f;
            float arm = size * 0.28f;
            FillRect(painter, new Rect(center.x - size * 0.14f, center.y - size * 0.14f, size * 0.28f, size * 0.28f), WithAlpha(color, 0.72f));
            DrawLine(painter, center + new Vector2(-half, -half + arm), center + new Vector2(-half, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, -half), center + new Vector2(-half + arm, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half - arm, -half), center + new Vector2(half, -half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half, -half), center + new Vector2(half, -half + arm), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, half - arm), center + new Vector2(-half, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(-half, half), center + new Vector2(-half + arm, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half - arm, half), center + new Vector2(half, half), color, 1.6f);
            DrawLine(painter, center + new Vector2(half, half), center + new Vector2(half, half - arm), color, 1.6f);
        }

        private static void DrawPowerIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            painter.strokeColor = color;
            painter.lineWidth = 1.6f;
            painter.BeginPath();
            painter.Arc(center, size * 0.44f, -50f, 230f);
            painter.Stroke();
            DrawLine(painter, center + new Vector2(0f, -size * 0.52f), center + new Vector2(0f, -size * 0.04f), color, 1.6f);
        }

        private static void DrawBarsIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            DrawLine(painter, center + new Vector2(-size * 0.64f, size * 0.44f), center + new Vector2(size * 0.64f, size * 0.44f), WithAlpha(color, 0.78f), 1.2f);
            FillRect(painter, new Rect(center.x - size * 0.52f, center.y - size * 0.04f, size * 0.18f, size * 0.48f), color);
            FillRect(painter, new Rect(center.x - size * 0.10f, center.y - size * 0.34f, size * 0.18f, size * 0.78f), color);
            FillRect(painter, new Rect(center.x + size * 0.32f, center.y - size * 0.18f, size * 0.18f, size * 0.62f), color);
        }

        private static void DrawPinIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            Vector2 pinCenter = center + new Vector2(0f, -size * 0.08f);
            Vector2 tip = center + new Vector2(0f, size * 0.56f);
            painter.strokeColor = color;
            painter.lineWidth = 1.35f;
            painter.BeginPath();
            painter.MoveTo(pinCenter + new Vector2(-size * 0.28f, size * 0.02f));
            painter.LineTo(tip);
            painter.LineTo(pinCenter + new Vector2(size * 0.28f, size * 0.02f));
            painter.Arc(pinCenter, size * 0.30f, -30f * Mathf.Deg2Rad, 210f * Mathf.Deg2Rad, ArcDirection.CounterClockwise);
            painter.Stroke();
            StrokeCircle(painter, pinCenter, size * 0.10f, color, 1.2f);
        }

        private static void DrawStudioIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            float edge = size * 0.54f;
            float angle30 = 30f * Mathf.Deg2Rad;
            Vector2 up = new Vector2(0f, -edge);
            Vector2 down = new Vector2(0f, edge);
            Vector2 rightTop = new Vector2(Mathf.Cos(angle30) * edge, -Mathf.Sin(angle30) * edge);
            Vector2 rightBot = new Vector2(Mathf.Cos(angle30) * edge, Mathf.Sin(angle30) * edge);
            Vector2 leftTop = new Vector2(-Mathf.Cos(angle30) * edge, -Mathf.Sin(angle30) * edge);
            Vector2 leftBot = new Vector2(-Mathf.Cos(angle30) * edge, Mathf.Sin(angle30) * edge);
            StrokePolygon(painter, new[] { center, center + rightTop, center + up, center + leftTop }, color, 1.35f);
            StrokePolygon(painter, new[] { center, center + leftTop, center + leftBot, center + down }, WithAlpha(color, 0.86f), 1.35f);
            StrokePolygon(painter, new[] { center, center + down, center + rightBot, center + rightTop }, WithAlpha(color, 0.72f), 1.35f);
        }

        private void DrawResetIcon(Painter2D painter, Vector2 center, float size, Color color)
        {
            float radius = size * 0.42f;
            DrawArcSegment(painter, center, radius, 18f, 168f, color, 1.45f);
            DrawArcSegment(painter, center, radius, 198f, 348f, color, 1.45f);

            Vector2 headA = center + AngleToVector(168f) * radius;
            Vector2 headB = center + AngleToVector(348f) * radius;
            DrawLine(painter, headA, headA + new Vector2(-3f, 4f), color, 1.4f);
            DrawLine(painter, headA, headA + new Vector2(2f, 5f), color, 1.4f);
            DrawLine(painter, headB, headB + new Vector2(3f, -4f), color, 1.4f);
            DrawLine(painter, headB, headB + new Vector2(-2f, -5f), color, 1.4f);
        }

        private void RefreshOverlayElements()
        {
            if (_partInfoTabLabel == null || _studioRenderLabel == null || _studioEnvironmentLabel == null)
            {
                return;
            }

            float width = resolvedStyle.width;
            float height = resolvedStyle.height;
            if (width <= 1f || height <= 1f)
            {
                _partInfoTabLabel.style.display = DisplayStyle.None;
                _studioRenderLabel.style.display = DisplayStyle.None;
                _studioEnvironmentLabel.style.display = DisplayStyle.None;
                return;
            }

            ComputeTimeline(out int phaseIndex, out float phaseT, out _);

            if (_scene == OnboardingSceneId.PartInfo)
            {
                float closeSeq = phaseIndex == 2 ? Phase01(phaseT, 0.10f, 0.48f) : 0f;
                float tabReveal = phaseIndex == 0
                    ? EaseOut(Phase01(phaseT, 0.72f, 0.98f))
                    : (phaseIndex == 2 ? 1f - GetActionProgress(Phase01(phaseT, 0.78f, 0.98f), false) : 1f);
                float panelOpen = phaseIndex == 1
                    ? EaseOut(Phase01(phaseT, 0.44f, 0.96f))
                    : (phaseIndex == 2 ? 1f - GetActionProgress(closeSeq, false) : 0f);
                float peekVisible = Mathf.Max(tabReveal - panelOpen * 1.25f, 0f);
                GetPartInfoRects(width, height, tabReveal, panelOpen, out _, out Rect tab, out _);
                _partInfoTabLabel.style.display = peekVisible > 0.06f ? DisplayStyle.Flex : DisplayStyle.None;
                SetAbsoluteRect(_partInfoTabLabel, tab);
            }
            else
            {
                _partInfoTabLabel.style.display = DisplayStyle.None;
            }

            if (_scene == OnboardingSceneId.Studio)
            {
                Rect renderCard = new Rect(width * 0.50f - 118f, height * 0.20f, 236f, 90f);
                Rect environmentCard = new Rect(width * 0.50f - 118f, height * 0.39f, 236f, 90f);
                float panelReveal = phaseIndex == 0 ? EaseOut(Phase01(phaseT, 0.28f, 0.42f)) : 1f;
                float renderSeq = phaseIndex == 0 ? Phase01(phaseT, 0.42f, 0.96f) : 0f;
                float renderHover = phaseIndex == 0 ? EaseOut(Phase01(renderSeq, 0.18f, 0.54f)) : 0.12f;
                float renderPress = phaseIndex == 0 ? GetPressProgress(renderSeq, false) : 0f;
                float renderActive = phaseIndex == 0 ? GetActionProgress(renderSeq, false) : 0.18f;
                float environmentHover = phaseIndex == 1 ? EaseOut(Phase01(phaseT, 0.18f, 0.52f)) : 0.12f;
                float environmentPress = phaseIndex == 1 ? GetPressProgress(phaseT, false) : 0f;
                float environmentActive = phaseIndex == 1 ? GetActionProgress(phaseT, false) : 0.18f;
                DisplayStyle labelDisplay = panelReveal > 0.06f ? DisplayStyle.Flex : DisplayStyle.None;
                _studioRenderLabel.style.display = labelDisplay;
                _studioEnvironmentLabel.style.display = labelDisplay;
                SetStudioOverlayLabelVisual(_studioRenderLabel, renderCard, renderActive, renderHover, renderPress, panelReveal);
                SetStudioOverlayLabelVisual(_studioEnvironmentLabel, environmentCard, environmentActive, environmentHover, environmentPress, panelReveal);
            }
            else
            {
                _studioRenderLabel.style.display = DisplayStyle.None;
                _studioEnvironmentLabel.style.display = DisplayStyle.None;
            }
        }

        private void SetStudioOverlayLabelVisual(Label label, Rect rect, float active, float hover, float press, float panelReveal)
        {
            Rect panelBase = ScaleRect(rect, Mathf.Lerp(0.86f, 1f, panelReveal));
            Rect visual = GetInteractiveVisualRect(panelBase, hover * panelReveal, press * panelReveal, active * panelReveal, 0.12f, 0.08f, 0.05f);
            Rect labelRect = new Rect(visual.x + 66f, visual.center.y - 12f, visual.width - 86f, 24f + active * 4f);
            SetAbsoluteRect(label, labelRect);
            label.style.fontSize = 14f + active * 2.4f + hover * 1.2f;
        }

        private static void SetAbsoluteRect(VisualElement element, Rect rect)
        {
            element.style.left = rect.x;
            element.style.top = rect.y;
            element.style.width = rect.width;
            element.style.height = rect.height;
        }

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        private static Vector2 EllipsePoint(Vector2 center, float radiusX, float radiusY, float angleDeg)
        {
            float radians = angleDeg * Mathf.Deg2Rad;
            return new Vector2(
                center.x + Mathf.Cos(radians) * radiusX,
                center.y - Mathf.Sin(radians) * radiusY);
        }

        private static Vector2 AngleToVector(float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private void ComputeTimeline(out int phaseIndex, out float phaseT, out float loopT)
        {
            phaseIndex = 0;
            phaseT = 0f;
            loopT = 0f;

            if (_phaseDurationsMs.Length == 0)
            {
                return;
            }

            float totalMs = 0f;
            for (int i = 0; i < _phaseDurationsMs.Length; i++)
            {
                totalMs += _phaseDurationsMs[i];
            }

            float elapsedMs = Mathf.Max(0f, (Time.realtimeSinceStartup - _animationStartTime) * 1000f);
            float loopMs = totalMs <= 0f ? 1f : elapsedMs % totalMs;
            loopT = loopMs / totalMs;

            float cursor = 0f;
            for (int i = 0; i < _phaseDurationsMs.Length; i++)
            {
                float next = cursor + _phaseDurationsMs[i];
                if (loopMs <= next || i == _phaseDurationsMs.Length - 1)
                {
                    phaseIndex = i;
                    phaseT = _phaseDurationsMs[i] <= 0 ? 0f : (loopMs - cursor) / _phaseDurationsMs[i];
                    phaseT = Mathf.Clamp01(phaseT);
                    return;
                }

                cursor = next;
            }
        }

        private static int[] GetPhaseDurations(OnboardingSceneId scene)
        {
            switch (scene)
            {
                case OnboardingSceneId.Navigate:
                    return new[] { 3400, 5000, 5400 };
                case OnboardingSceneId.Select:
                    return new[] { 2600, 2800, 3000 };
                case OnboardingSceneId.PartInfo:
                    return new[] { 2600, 2800, 3400 };
                case OnboardingSceneId.Inspect:
                case OnboardingSceneId.Analyze:
                    return new[] { 4000, 3400, 3400 };
                case OnboardingSceneId.Studio:
                    return new[] { 4200, 3400, 3400 };
                case OnboardingSceneId.Pins:
                    return new[] { 2800, 3800 };
                case OnboardingSceneId.Isolate:
                    return new[] { 2800, 8400 };
                case OnboardingSceneId.Power:
                    return new[] { 3000, 5200 };
                case OnboardingSceneId.Cut:
                    return new[] { 6200, 4400, 6400 };
                case OnboardingSceneId.Explode:
                    return new[] { 3400, 5600 };
                case OnboardingSceneId.Filter:
                    return new[] { 4600, 5000 };
                case OnboardingSceneId.RenderMode:
                    return new[] { 3200, 3400, 3800 };
                case OnboardingSceneId.Environment:
                    return new[] { 4600, 12000, 5600 };
                case OnboardingSceneId.Lighting:
                    return new[] { 4200, 4200, 4200 };
                default:
                    return new[] { 1600 };
            }
        }

        private static float Phase01(float t, float start, float end)
        {
            if (Mathf.Approximately(start, end))
            {
                return 1f;
            }

            return Mathf.Clamp01((t - start) / (end - start));
        }

        private static float EaseOut(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private static float EaseInOut(float t)
        {
            t = Mathf.Clamp01(t);
            return t * t * (3f - 2f * t);
        }

        private static float EaseOutBack(float t, float overshoot = 1.12f)
        {
            t = Mathf.Clamp01(t);
            float inv = t - 1f;
            return 1f + (overshoot + 1f) * inv * inv * inv + overshoot * inv * inv;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
