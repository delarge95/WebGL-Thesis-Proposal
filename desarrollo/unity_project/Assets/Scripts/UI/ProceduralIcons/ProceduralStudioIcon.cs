using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralStudioIcon : ProceduralIconBase
    {
        // 1. Cube Explosion (Faces offset)
        private float targetSpread = 0f;
        private float currentSpread = 0f;
        private float spreadVelocity = 0f;
        
        // 2. Cube overall scale squash
        private float targetScale = 1f;
        private float currentScale = 1f;
        private float scaleVelocity = 0f;

        // One-click state
        private bool isSnapping = false;

        protected override void OnHoverEnter()
        {
            if (!isSnapping)
            {
                // Anticipation: The box opens slightly (Faces move outward)
                targetSpread = 4f; 
                targetScale = 1.15f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isSnapping)
            {
                targetSpread = 0f;
                targetScale = 1f;
            }
        }

        protected override void OnPressed()
        {
            // Explosive Snap Shut
            if (!isSnapping)
            {
                isSnapping = true;
                targetSpread = -1f; // Tightly closed
                targetScale = 0.7f; // Squash entirely
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isSnapping)
            {
                if (currentScale <= 0.8f) // Near total squash
                {
                    isSnapping = false;
                    targetSpread = isHovered ? 4f : 0f;
                    targetScale = isHovered ? 1.15f : 1f;
                }
            }

            float oldScale = currentScale;
            float oldSpread = currentSpread;

            if (isSnapping)
            {
                currentScale = Mathf.Lerp(currentScale, targetScale, dt * 45f); // Fast snap
                currentSpread = Mathf.Lerp(currentSpread, targetSpread, dt * 50f);
            }
            else
            {
                currentScale = SpringFloat(currentScale, targetScale, ref scaleVelocity, 25f, 0.6f, dt); // Bounce recovery
                currentSpread = SpringFloat(currentSpread, targetSpread, ref spreadVelocity, 30f, 0.5f, dt); // Super bouncy parts
            }

            if (Mathf.Abs(currentScale - oldScale) > 0.005f ||
                Mathf.Abs(currentSpread - oldSpread) > 0.05f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            // Hexagon bounds calculation
            float size = Mathf.Min(width, height) * 0.35f * currentScale;

            painter.strokeColor = currentColor;
            painter.fillColor = currentColor;

            // Isometric 3D vectors
            Vector2 up = new Vector2(0, -size);
            Vector2 down = new Vector2(0, size);
            
            // Angles for isometric projection
            float angle30 = 30f * Mathf.Deg2Rad;
            Vector2 rightTop = new Vector2(Mathf.Cos(angle30) * size, -Mathf.Sin(angle30) * size);
            Vector2 rightBot = new Vector2(Mathf.Cos(angle30) * size, Mathf.Sin(angle30) * size);
            Vector2 leftTop = new Vector2(-Mathf.Cos(angle30) * size, -Mathf.Sin(angle30) * size);
            Vector2 leftBot = new Vector2(-Mathf.Cos(angle30) * size, Mathf.Sin(angle30) * size);

            // Origin
            Vector2 center = new Vector2(cx, cy);

            // 1. Draw Top Face (Offset by currentSpread upwards)
            Vector2 offsetTop = new Vector2(0, -currentSpread * 1.5f);
            painter.BeginPath();
            painter.MoveTo(center + offsetTop);
            painter.LineTo(center + rightTop + offsetTop);
            painter.LineTo(center + up + offsetTop);
            painter.LineTo(center + leftTop + offsetTop);
            painter.ClosePath();
            
            // Draw lines for Top Face
            painter.Stroke();

            // Fill top face with solid transparent color to sell the 3D look
            Color fillColorTop = currentColor;
            fillColorTop.a *= 0.15f;
            painter.fillColor = fillColorTop;
            painter.Fill();

            // 2. Draw Left Face (Offset diagonally left-down)
            Vector2 offsetLeft = new Vector2(-Mathf.Cos(angle30) * currentSpread, Mathf.Sin(angle30) * currentSpread);
            painter.BeginPath();
            painter.MoveTo(center + offsetLeft);
            painter.LineTo(center + leftTop + offsetLeft);
            painter.LineTo(center + leftBot + offsetLeft);
            painter.LineTo(center + down + offsetLeft);
            painter.ClosePath();
            painter.Stroke();
            
            Color fillColorLeft = currentColor;
            fillColorLeft.a *= 0.35f; // Darker
            painter.fillColor = fillColorLeft;
            painter.Fill();

            // 3. Draw Right Face (Offset diagonally right-down)
            Vector2 offsetRight = new Vector2(Mathf.Cos(angle30) * currentSpread, Mathf.Sin(angle30) * currentSpread);
            painter.BeginPath();
            painter.MoveTo(center + offsetRight);
            painter.LineTo(center + down + offsetRight);
            painter.LineTo(center + rightBot + offsetRight);
            painter.LineTo(center + rightTop + offsetRight);
            painter.ClosePath();
            painter.Stroke();
            
            Color fillColorRight = currentColor;
            fillColorRight.a *= 0.05f; // Brightest side
            painter.fillColor = fillColorRight;
            painter.Fill();
        }
    }
}
