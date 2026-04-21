using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement] // Expose to UI Builder
    public partial class ProceduralResetIcon : ProceduralIconBase
    {
        // Animated Rotation variables
        private float targetRotationAngle = 0f;
        private float currentRotationAngle = 0f;
        private float rotationVelocity = 0f;

        // Visual Tension variables
        // Makes the circle smaller or tighter when hovered
        private float targetRadiusSquash = 1f;
        private float currentRadiusSquash = 1f;

        protected override void OnHoverEnter()
        {
            // Rotate 45 degrees, squeeze the radius slightly to show "tension"
            targetRotationAngle = 45f;
            targetRadiusSquash = 0.85f;
        }

        protected override void OnHoverExit()
        {
            // Return to resting state
            targetRotationAngle = 0f;
            targetRadiusSquash = 1f;
        }

        protected override void OnPressed()
        {
            // Full spin 360 degrees forward 
            targetRotationAngle += 360f; 
            targetRadiusSquash = 0.7f; // Clench tightest on click
        }

        protected override void OnReleased()
        {
            if (isHovered)
            {
                // Go to exactly where it was hovering, relative to the 360 spin
                // (Using modulo so it doesn't spin backwards 360 degrees)
                targetRotationAngle = Mathf.Floor(targetRotationAngle / 360f) * 360f + 45f;
                targetRadiusSquash = 0.85f;
            }
            else
            {
                targetRotationAngle = Mathf.Floor(targetRotationAngle / 360f) * 360f;
                targetRadiusSquash = 1f;
            }
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            float oldRot = currentRotationAngle;
            float oldSqs = currentRadiusSquash;

            // Apply spring to rotation (bouncy but fairly fast)
            currentRotationAngle = SpringFloat(currentRotationAngle, targetRotationAngle, ref rotationVelocity, 18f, 0.7f, dt);
            
            // Standard lerp for squash
            currentRadiusSquash = Mathf.Lerp(currentRadiusSquash, targetRadiusSquash, dt * 15f);

            if (Mathf.Abs(currentRotationAngle - oldRot) > 0.05f ||
                Mathf.Abs(currentRadiusSquash - oldSqs) > 0.01f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            float baseRadius = Mathf.Min(width, height) * 0.35f;
            float radius = baseRadius * currentRadiusSquash;

            painter.BeginPath();

            // Painter2D doesn't have an easy "RotateCanvas" yet, so we have to do the math to rotate the start/end points of the arcs
            // Arrow 1 (Top right curving down)
            float startAngle1 = (15f + currentRotationAngle) * Mathf.Deg2Rad;
            float endAngle1 = (165f + currentRotationAngle) * Mathf.Deg2Rad;
            
            // Note: UI Toolkit's Arc method draws slightly differently than standard Canvas API, 
            // usually taking Center, Radius, StartAngle, EndAngle. However, Painter2D's Arc method 
            // is missing in some older 2021/2022 versions, so we use multiple segments or approximations
            // if needed. For Unity 2023+, Arc is usually available.
            // Since Unity 6 has Painter2D.Arc(): 
            painter.Arc(new Vector2(cx, cy), radius, startAngle1, endAngle1 - startAngle1); // Start and Sweep amount
            painter.Stroke();
            
            // Draw Arrow Head 1 manually at the end of the arc
            DrawArrowHead(painter, cx, cy, radius, endAngle1);

            painter.BeginPath();
            // Arrow 2 (Bottom left curving up)
            float startAngle2 = (195f + currentRotationAngle) * Mathf.Deg2Rad;
            float endAngle2 = (345f + currentRotationAngle) * Mathf.Deg2Rad;
            
            painter.Arc(new Vector2(cx, cy), radius, startAngle2, endAngle2 - startAngle2);
            painter.Stroke();

            // Draw Arrow Head 2 
            DrawArrowHead(painter, cx, cy, radius, endAngle2);
        }

        /// <summary>
        /// Draws a small minimal arrow head at the specified point on the circle
        /// </summary>
        private void DrawArrowHead(Painter2D painter, float cx, float cy, float radius, float angleRad)
        {
            // Position on loop
            float x = cx + Mathf.Cos(angleRad) * radius;
            float y = cy + Mathf.Sin(angleRad) * radius;

            // Direction tangent to the circle
            float tangentDir = angleRad + Mathf.PI / 2f;

            // Size of the arrowhead
            float headSize = 3.2f * currentRadiusSquash;

            // Left prong
            float leftProngX = x + Mathf.Cos(tangentDir + 2.5f) * headSize;
            float leftProngY = y + Mathf.Sin(tangentDir + 2.5f) * headSize;

            // Right prong
            float rightProngX = x + Mathf.Cos(tangentDir - 2.5f) * headSize;
            float rightProngY = y + Mathf.Sin(tangentDir - 2.5f) * headSize;

            painter.BeginPath();
            painter.MoveTo(new Vector2(leftProngX, leftProngY));
            painter.LineTo(new Vector2(x, y));
            painter.LineTo(new Vector2(rightProngX, rightProngY));
            painter.Stroke();
        }
    }
}
