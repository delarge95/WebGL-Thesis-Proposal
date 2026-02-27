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
        // Settings
        protected float UnhoveredStrokeWidth = 1.5f;
        protected float HoveredStrokeWidth = 2.0f;
        protected Color NormalColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        protected Color HoverColor = new Color(0.4f, 0.85f, 0.9f, 1.0f);   // Cyan/Sci-fi accent
        protected Color PressedColor = new Color(0.2f, 0.6f, 1.0f, 1.0f);

        // State Machine
        protected bool isHovered = false;
        protected bool isPressed = false;

        // Current Animated Values (Driven by math)
        protected float currentStrokeWidth;
        protected Color currentColor;
        
        // Custom Scheduled Update Task for animation smoothing
        private IVisualElementScheduledItem animationTask;

        // Base constructor
        public ProceduralIconBase()
        {
            // Register callback to hook events when attached to the visual tree
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);

            // Subscribe to the generateVisualContent event (Where the 2D magic happens)
            generateVisualContent += OnGenerateVisualContent;

            // Initialize visual states
            currentStrokeWidth = UnhoveredStrokeWidth;
            currentColor = NormalColor;

            // Schedule a continuous update loop to perform math interpolations
            // Updating every 16ms roughly equals 60fps
            animationTask = schedule.Execute(UpdatePhysics).Every(16);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            var parentBtn = this.GetFirstAncestorOfType<Button>();
            if (parentBtn != null)
            {
                parentBtn.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
                parentBtn.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
                parentBtn.RegisterCallback<PointerDownEvent>(OnPointerDown);
                parentBtn.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
            else
            {
                RegisterCallback<PointerEnterEvent>(OnPointerEnter);
                RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
                RegisterCallback<PointerDownEvent>(OnPointerDown);
                RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            var parentBtn = this.GetFirstAncestorOfType<Button>();
            if (parentBtn != null)
            {
                parentBtn.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
                parentBtn.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
                parentBtn.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                parentBtn.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }
            else
            {
                UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
                UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
                UnregisterCallback<PointerDownEvent>(OnPointerDown);
                UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
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
                OnPressed();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button == 0)
            {
                isPressed = false;
                OnReleased();
            }
        }

        /// <summary>
        /// Update loop for Spring physics and standard Lerping. 
        /// Calls MarkDirtyRepaint() if values changed to trigger a new draw call.
        /// </summary>
        private void UpdatePhysics()
        {
            bool needsRepaint = false;

            // Standard Lerps for generic properties (Color and Stroke)
            float targetStroke = isHovered ? HoveredStrokeWidth : UnhoveredStrokeWidth;
            Color targetColor = isPressed ? PressedColor : (isHovered ? HoverColor : NormalColor);

            if (Mathf.Abs(currentStrokeWidth - targetStroke) > 0.01f)
            {
                currentStrokeWidth = Mathf.Lerp(currentStrokeWidth, targetStroke, Time.unscaledDeltaTime * 15f);
                needsRepaint = true;
            }

            if (currentColor != targetColor)
            {
                currentColor = Color.Lerp(currentColor, targetColor, Time.unscaledDeltaTime * 15f);
                needsRepaint = true;
            }

            // Let the child class do its complex math
            if (UpdateCustomPhysics(Time.unscaledDeltaTime))
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
            var painter = mgc.painter2D;
            
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.strokeColor = currentColor;
            painter.lineWidth = currentStrokeWidth;

            // Delegate drawing to the specific icon implementation
            DrawIconPath(painter, resolvedStyle.width, resolvedStyle.height);
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
            float displacement = current - target;
            float springForce = -(springScale * springScale) * displacement;
            float dampingForce = -2f * springScale * damp * velocity;
            float acceleration = springForce + dampingForce;

            velocity += acceleration * dt;
            return current + velocity * dt;
        }
    }
}
