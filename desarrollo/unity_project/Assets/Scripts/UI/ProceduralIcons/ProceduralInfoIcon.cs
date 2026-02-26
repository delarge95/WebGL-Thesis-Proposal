using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralInfoIcon : ProceduralIconBase
    {
        // 1. Group Scale (Hover)
        private float targetGroupScale = 1f;
        private float currentGroupScale = 1f;
        private float groupVelocity = 0f;

        // 2. Physics for the dot (Squash and Stretch bounce)
        // Equilibrium is 0. 
        private float targetDotElevation = 0f;
        private float currentDotElevation = 0f;
        private float dotVelocity = 0f;

        protected override void OnHoverEnter()
        {
            // Just scale the entire icon uniformly and elegantly
            targetGroupScale = 1.12f; 
        }

        protected override void OnHoverExit()
        {
            targetGroupScale = 1f;
        }

        protected override void OnPressed()
        {
            // Apply a massive INSTANT UPWARD IMPULSE to the dot's velocity!
            // The underdamped spring will naturally pull it down and make it bounce.
            // This achieves pure fluid animation principles procedurally.
            dotVelocity += 180f; 
        }

        protected override void OnReleased()
        {
            // Fully driven by the spring
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            float oldScale = currentGroupScale;
            float oldElev = currentDotElevation;

            currentGroupScale = SpringFloat(currentGroupScale, targetGroupScale, ref groupVelocity, 25f, 0.7f, dt);
            
            // Underdamped spring for the Dot (Damping = 0.45f causes bounces!)
            currentDotElevation = SpringFloat(currentDotElevation, targetDotElevation, ref dotVelocity, 40f, 0.4f, dt);
            
            // Prevent the dot from penetrating too deep into the "i" stem when bouncing down
            if (currentDotElevation < -4f)
            {
                currentDotElevation = -4f;
                dotVelocity = -dotVelocity * 0.5f; // Bounce energy loss against the ground
            }

            if (Mathf.Abs(currentGroupScale - oldScale) > 0.005f ||
                Mathf.Abs(currentDotElevation - oldElev) > 0.05f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            // Base structural math
            float ringRadius = Mathf.Min(width, height) * 0.45f * currentGroupScale;
            
            // 1. Draw Outer Ring
            painter.BeginPath();
            painter.Arc(new Vector2(cx, cy), ringRadius, 0, 360f); 
            painter.Stroke();

            // Calculate adaptive Squash & Stretch based on velocity and position!
            // If moving fast -> stretch vertically. If hitting bottom -> squash horizontally.
            float velocityStretch = dotVelocity * 0.005f; // Positive when going up
            float squashFactor = Mathf.Clamp(currentDotElevation, -4f, 0f) / -4f; // 1.0 when fully squashed at -4
            
            float dotScaleY = 1f + velocityStretch - (squashFactor * 0.4f);
            float dotScaleX = 1f / Mathf.Max(0.5f, dotScaleY); // Conserve volume
            
            // Clamp extremes
            dotScaleY = Mathf.Clamp(dotScaleY, 0.4f, 1.8f);
            dotScaleX = Mathf.Clamp(dotScaleX, 0.4f, 1.8f);

            // 2. Draw the vertical line of the "i"
            float stemHeight = ringRadius * 0.4f;
            float lineBotY = cy + ringRadius * 0.5f;
            float lineTopY = lineBotY - stemHeight;
            
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx, lineTopY));
            painter.LineTo(new Vector2(cx, lineBotY));
            painter.Stroke();

            // 3. Draw the bouncing dot "i" (Squash / Stretch animated)
            float baseDotY = lineTopY - (ringRadius * 0.35f);
            float dotRadius = HoveredStrokeWidth * 1.5f * currentGroupScale;

            float drawWidth = dotRadius * dotScaleX;
            float drawHeight = dotRadius * dotScaleY;
            
            Vector2 dotCenter = new Vector2(cx, baseDotY - currentDotElevation);

            painter.BeginPath();
            // A thick line closely mimics a perfect circle/ellipse
            painter.lineWidth = drawWidth * 2f; 
            painter.MoveTo(new Vector2(cx, dotCenter.y - drawHeight * 0.5f));
            painter.LineTo(new Vector2(cx, dotCenter.y + drawHeight * 0.5f));
            painter.Stroke();
            
            painter.lineWidth = currentStrokeWidth; // Revert
        }
    }
}
