using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralPinsIcon : ProceduralIconBase
    {
        // 1. Pin Elevation
        private float targetPinY = 0f;
        private float currentPinY = 0f;
        private float pinVelocity = 0f;

        // 2. Ground Line width
        private float targetGroundW = 0.5f;
        private float currentGroundW = 0.5f;

        // One-click state
        private bool isPlanting = false;

        protected override void OnHoverEnter()
        {
            if (!isPlanting)
            {
                // Anticipation: Pin floats up preparing to plant
                targetPinY = -8f; // Up is negative Y
                targetGroundW = 1.2f; // Ground line extends
            }
        }

        protected override void OnHoverExit()
        {
            if (!isPlanting)
            {
                targetPinY = 0f;
                targetGroundW = 0.5f;
            }
        }

        protected override void OnPressed()
        {
            if (!isPlanting)
            {
                // Slam into the ground
                isPlanting = true;
                targetPinY = 6f; // Overshoot into the ground
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isPlanting)
            {
                if (currentPinY >= 5.5f)
                {
                    isPlanting = false;
                    targetPinY = isHovered ? -8f : 0f;
                }
            }

            float oldY = currentPinY;
            float oldGW = currentGroundW;

            if (isPlanting)
            {
                currentPinY = Mathf.Lerp(currentPinY, targetPinY, dt * 50f); // Fast slam
            }
            else
            {
                currentPinY = SpringFloat(currentPinY, targetPinY, ref pinVelocity, 25f, 0.6f, dt); // Bouncy float
            }

            currentGroundW = Mathf.Lerp(currentGroundW, targetGroundW, dt * 15f);

            if (Mathf.Abs(currentPinY - oldY) > 0.05f || 
                Mathf.Abs(currentGroundW - oldGW) > 0.01f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.4f;

            painter.strokeColor = currentColor;

            // 1. Draw the Ground Line
            float groundY = cy + baseSize * 0.8f;
            float gw = baseSize * currentGroundW;
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - gw, groundY));
            painter.LineTo(new Vector2(cx + gw, groundY));
            painter.Stroke();

            // 2. Draw the Pin (Circle with a spike)
            float pinCX = cx;
            float pinCY = cy + currentPinY; // Offset by animation

            float pinRadius = baseSize * 0.45f;
            
            // Instead of just a line, let's draw a nice loc-pin shape
            // Main circle top
            painter.BeginPath();
            // Start at bottom right of the circle
            painter.Arc(new Vector2(pinCX, pinCY - pinRadius * 0.5f), pinRadius, 45f * Mathf.Deg2Rad, 135f * Mathf.Deg2Rad, ArcDirection.CounterClockwise);
            // Connect to spike tip
            painter.LineTo(new Vector2(pinCX, pinCY + baseRadius * 0.7f));
            painter.ClosePath();
            painter.Stroke();

            // Inner dot
            painter.BeginPath();
            painter.Arc(new Vector2(pinCX, pinCY - pinRadius * 0.5f), pinRadius * 0.3f, 0f, 360f);
            painter.Stroke();
        }
    }
}
