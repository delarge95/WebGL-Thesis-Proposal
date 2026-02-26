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
                targetPinY = -6f; // Up is negative Y
                targetGroundW = 1.0f; // Ground line extends
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
                targetPinY = 5f; // Overshoot down into the ground
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isPlanting)
            {
                if (currentPinY >= 4.5f)
                {
                    isPlanting = false;
                    targetPinY = isHovered ? -6f : 0f;
                }
            }

            float oldY = currentPinY;
            float oldGW = currentGroundW;

            if (isPlanting)
            {
                currentPinY = Mathf.Lerp(currentPinY, targetPinY, dt * 60f); // Violent slam
            }
            else
            {
                currentPinY = SpringFloat(currentPinY, targetPinY, ref pinVelocity, 25f, 0.55f, dt); // Bouncy float
            }

            currentGroundW = Mathf.Lerp(currentGroundW, targetGroundW, dt * 20f);

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
            float baseSize = Mathf.Min(width, height) * 0.45f;

            painter.strokeColor = currentColor;
            painter.fillColor = currentColor;

            // 1. Draw the Ground Line (Bottom Base)
            float groundY = cy + baseSize * 0.7f;
            float gw = baseSize * currentGroundW;
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - gw, groundY));
            painter.LineTo(new Vector2(cx + gw, groundY));
            painter.Stroke();

            // 2. Draw the Classic Teardrop Pin
            // An arc from approx -30 deg to 210 deg, then straight lines down to a tip
            float pinCX = cx;
            float pinCY = cy - baseSize * 0.1f + currentPinY; 
            
            float pinRadius = baseSize * 0.4f;
            Vector2 pinCenter = new Vector2(pinCX, pinCY);
            Vector2 pinTip = new Vector2(pinCX, pinCY + baseSize * 0.65f); // Pointy tip touching the ground

            painter.BeginPath();
            
            // The top curve of the teardrop goes sweeping from the right down to the tip, to the left, and back over the top.
            // Wait, UI Toolkit Painter2D Arc draws counter-clockwise from startAngle to endAngle? Or clockwise? Default matches mathematical.
            // 30 degrees (lower right), over the top 90 (top), to 150 degrees (lower left).
            float startAngleRad = 210f * Mathf.Deg2Rad;
            float endAngleRad = -30f * Mathf.Deg2Rad;
            Vector2 arcStartLeft = new Vector2(pinCX + Mathf.Cos(startAngleRad)*pinRadius, pinCY - Mathf.Sin(startAngleRad)*pinRadius);
            Vector2 arcEndRight = new Vector2(pinCX + Mathf.Cos(endAngleRad)*pinRadius, pinCY - Mathf.Sin(endAngleRad)*pinRadius);

            // Start at bottom-left of the circle
            painter.MoveTo(arcStartLeft);
            // Draw left diagonal line to tip
            painter.LineTo(pinTip);
            // Draw right diagonal line from tip
            painter.LineTo(arcEndRight);
            
            // Draw top arc to connect right to left smoothly
            painter.Arc(pinCenter, pinRadius, -30f * Mathf.Deg2Rad, 210f * Mathf.Deg2Rad, ArcDirection.CounterClockwise);
            
            // We will fill the pin body with background color to hide any overlapping lines 
            // (e.g., ground line showing through), and stroke it.
            painter.fillColor = new Color(0.12f, 0.12f, 0.12f); // Same as common UI_icons_test bg
            painter.Fill(FillRule.NonZero); 
            painter.Stroke();

            // Inner hole (Dot)
            painter.fillColor = currentColor; // Reset for potential use later
            painter.BeginPath();
            painter.Arc(pinCenter, pinRadius * 0.35f, 0f, 360f);
            painter.Stroke();
        }
    }
}
