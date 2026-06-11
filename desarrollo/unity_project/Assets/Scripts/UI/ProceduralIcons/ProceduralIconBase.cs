using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    /// <summary>
    /// Base class for all procedural, math-driven animated icons in UI Toolkit.
    /// Handles pointer events, layout, and a custom spring-physics update loop.
    /// </summary>
    public abstract class ProceduralIconBase : VisualElement
    {
        private const float MinDrawableSize = 2f;
        private const float MaxDrawableSize = 256f;
        private const float MaxAnimationDeltaTime = 1f / 30f;
        private const float MaxSpringVelocity = 5000f;

        // Settings — dark theme (default)
        private static readonly Color DarkNormal  = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private static readonly Color DarkHover   = new Color(0.4f, 0.85f, 0.9f, 1.0f);
        private static readonly Color DarkPressed = new Color(0.2f, 0.6f, 1.0f, 1.0f);

        // Settings — light theme
        private static readonly Color LightNormal  = new Color(0.12f, 0.12f, 0.14f, 0.85f);
        private static readonly Color LightHover   = new Color(0.02f, 0.48f, 0.55f, 1.0f);
        private static readonly Color LightPressed = new Color(0.0f, 0.35f, 0.65f, 1.0f);

        protected float UnhoveredStrokeWidth = 1.5f;
        protected float HoveredStrokeWidth = 2.0f;
        protected Color NormalColor  = DarkNormal;
        protected Color HoverColor   = DarkHover;
        protected Color PressedColor = DarkPressed;

        // State Machine
        protected bool isHovered = false;
        protected bool isPressed = false;
        private bool _isLightBg = false;

        // Current Animated Values (Driven by math)
        protected float currentStrokeWidth;
        protected Color currentColor;
        
        // Custom Scheduled Update Task for animation smoothing
        private IVisualElementScheduledItem animationTask;
        private VisualElement _eventTarget;
        private int _pressedPointerId = -1;

        // Base constructor
        public ProceduralIconBase()
        {
            pickingMode = PickingMode.Ignore;
            style.overflow = Overflow.Hidden;

            // Register callback to hook events when attached to the visual tree
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);

            // Subscribe to the generateVisualContent event (Where the 2D magic happens)
            generateVisualContent += OnGenerateVisualContent;

            // Initialize visual states
            currentStrokeWidth = UnhoveredStrokeWidth;
            currentColor = NormalColor;

            // Schedule a continuous update loop to perform math interpolations
            // Updating every 16ms roughly equals 60fps
            animationTask = schedule.Execute(UpdatePhysics).Every(16);
        }

        /// <summary>
        /// Called by external code when the background theme changes.
        /// </summary>
        public virtual void SetLightBackground(bool isLight)
        {
            if (_isLightBg == isLight) return;
            _isLightBg = isLight;
            NormalColor  = isLight ? LightNormal  : DarkNormal;
            HoverColor   = isLight ? LightHover   : DarkHover;
            PressedColor = isLight ? LightPressed  : DarkPressed;
            MarkDirtyRepaint();
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            UnregisterPointerCallbacks();

            var parentBtn = this.GetFirstAncestorOfType<Button>();
            _eventTarget = parentBtn != null ? parentBtn : this;

            // UI Toolkit Buttons consume pointer events by default.
            // Use capture phase on the stable button parent, but register exactly once.
            _eventTarget.RegisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
            _eventTarget.RegisterCallback<PointerLeaveEvent>(OnPointerLeave, TrickleDown.TrickleDown);
            _eventTarget.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            _eventTarget.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            _eventTarget.RegisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);
            _eventTarget.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut, TrickleDown.TrickleDown);
            ResetInteractionState();
        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterPointerCallbacks();
            ResetInteractionState();
        }

        private void UnregisterPointerCallbacks()
        {
            if (_eventTarget == null)
            {
                return;
            }

            _eventTarget.UnregisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
            _eventTarget.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave, TrickleDown.TrickleDown);
            _eventTarget.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            _eventTarget.UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            _eventTarget.UnregisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);
            _eventTarget.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut, TrickleDown.TrickleDown);
            _eventTarget = null;
        }

        private void ResetInteractionState()
        {
            isHovered = false;
            isPressed = false;
            _pressedPointerId = -1;
            OnHoverExit();
            OnReleased();
            MarkDirtyRepaint();
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            if (!IsMousePointer(evt.pointerType))
            {
                return;
            }

            isHovered = true;
            OnHoverEnter();
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            isHovered = false;
            isPressed = false;
            OnHoverExit();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button == 0) // Left click only
            {
                isPressed = true;
                _pressedPointerId = evt.pointerId;
                OnPressed();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button == 0 && (_pressedPointerId < 0 || evt.pointerId == _pressedPointerId))
            {
                isPressed = false;
                _pressedPointerId = -1;
                OnReleased();
            }
        }

        private void OnPointerCancel(PointerCancelEvent evt)
        {
            if (_pressedPointerId < 0 || evt.pointerId == _pressedPointerId)
            {
                ResetInteractionState();
            }
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            ResetInteractionState();
        }

        private static bool IsMousePointer(string pointerType)
        {
            return string.IsNullOrEmpty(pointerType)
                || string.Equals(pointerType, "mouse", System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Update loop for Spring physics and standard Lerping. 
        /// Calls MarkDirtyRepaint() if values changed to trigger a new draw call.
        /// </summary>
        private void UpdatePhysics()
        {
            bool needsRepaint = false;
            float dt = SafeDeltaTime(Time.unscaledDeltaTime);

            // Standard Lerps for generic properties (Color and Stroke)
            float targetStroke = isHovered ? HoveredStrokeWidth : UnhoveredStrokeWidth;
            Color targetColor = isPressed ? PressedColor : (isHovered ? HoverColor : NormalColor);

            if (Mathf.Abs(currentStrokeWidth - targetStroke) > 0.01f)
            {
                currentStrokeWidth = Mathf.Lerp(currentStrokeWidth, targetStroke, dt * 8f);
                needsRepaint = true;
            }

            if (currentColor != targetColor)
            {
                currentColor = Color.Lerp(currentColor, targetColor, dt * 8f);
                needsRepaint = true;
            }

            // Let the child class do its complex math
            if (UpdateCustomPhysics(dt))
            {
                needsRepaint = true;
            }

            if (needsRepaint)
            {
                MarkDirtyRepaint(); // Forces UI Toolkit to call OnGenerateVisualContent
            }
        }

        /// <summary>
        /// Draws the actual geometry using the Painter2D API.
        /// </summary>
        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (!TryGetSafeDrawSize(out float width, out float height))
            {
                return;
            }

            var painter = mgc.painter2D;
            
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.strokeColor = currentColor;
            painter.lineWidth = Mathf.Clamp(currentStrokeWidth, 0.25f, 8f);

            // Delegate drawing to the specific icon implementation
            DrawIconPath(painter, width, height);
        }

        // --- ABSTRACT METHODS FOR CHILDREN TO IMPLEMENT ---

        protected abstract void OnHoverEnter();
        protected abstract void OnHoverExit();
        protected abstract void OnPressed();
        protected abstract void OnReleased();

        /// <summary>
        /// Perform custom math (springs/lerps) here. Return true if visual changed.
        /// </summary>
        protected abstract bool UpdateCustomPhysics(float deltaTime);

        /// <summary>
        /// Draw the vector math using the Painter2D reference.
        /// </summary>
        protected abstract void DrawIconPath(Painter2D painter, float width, float height);

        // --- HELPER MATH ---
        
        /// <summary>
        /// Simple critically damped spring implementation for bouncy, organic movement
        /// </summary>
        protected float SpringFloat(float current, float target, ref float velocity, float springScale = 15f, float damp = 0.8f, float dt = 0.016f)
        {
            dt = SafeDeltaTime(dt);

            if (!IsFinite(current) || !IsFinite(target) || !IsFinite(velocity))
            {
                velocity = 0f;
                return IsFinite(target) ? target : 0f;
            }

            float displacement = current - target;
            float springForce = -(springScale * springScale) * displacement;
            float dampingForce = -2f * springScale * damp * velocity;
            float acceleration = springForce + dampingForce;

            velocity += acceleration * dt;
            velocity = Mathf.Clamp(velocity, -MaxSpringVelocity, MaxSpringVelocity);

            float result = current + velocity * dt;
            if (!IsFinite(result))
            {
                velocity = 0f;
                return target;
            }

            return result;
        }

        private bool TryGetSafeDrawSize(out float width, out float height)
        {
            width = resolvedStyle.width;
            height = resolvedStyle.height;

            if (!IsFinite(width) || !IsFinite(height) || width < MinDrawableSize || height < MinDrawableSize)
            {
                width = layout.width;
                height = layout.height;
            }

            if (!IsFinite(width) || !IsFinite(height) || width < MinDrawableSize || height < MinDrawableSize)
            {
                width = contentRect.width;
                height = contentRect.height;
            }

            if (!IsFinite(width) || !IsFinite(height) || width < MinDrawableSize || height < MinDrawableSize)
            {
                return false;
            }

            width = Mathf.Clamp(width, MinDrawableSize, MaxDrawableSize);
            height = Mathf.Clamp(height, MinDrawableSize, MaxDrawableSize);
            return true;
        }

        private static float SafeDeltaTime(float dt)
        {
            if (!IsFinite(dt) || dt <= 0f)
            {
                return 1f / 60f;
            }

            return Mathf.Min(dt, MaxAnimationDeltaTime);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
