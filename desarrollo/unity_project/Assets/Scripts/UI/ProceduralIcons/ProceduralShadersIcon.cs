using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralShadersIcon : ProceduralIconBase
    {
        // Position of the "Scanner" lines separating solid from wireframe
        // 0.0 is far left, 1.0 is far right
        private float targetRevealLineX = 0.5f;
        private float currentRevealLineX = 0.5f;

        private float targetEraseLineX = 0.0f;
        private float currentEraseLineX = 0.0f;

        private bool isChangingShader = false;

        protected override void OnHoverEnter()
        {
            if (!isChangingShader)
            {
                // Scanner sweeps to the far right, revealing full wireframe
                targetRevealLineX = 1.0f; 
                targetEraseLineX = 0.0f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isChangingShader)
            {
                // Return to equilibrium (split half and half)
                targetRevealLineX = 0.5f;
                targetEraseLineX = 0.0f;
            }
        }

        protected override void OnPressed()
        {
            // Trigger the one-click erasing sweep
            if (!isChangingShader)
            {
                isChangingShader = true;
                // Reveal line is already at 1.0 (since we must be hovering to click)
                targetEraseLineX = 1.05f; // Sweep all the way past the right edge
            }
        }

        protected override void OnReleased()
        {
            // Handled by physics
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isChangingShader)
            {
                // When the erase line finishes its sweep (reaches the right edge)
                if (currentEraseLineX >= 1.0f)
                {
                    isChangingShader = false;
                    // Reset positions instantly behind the scenes
                    currentRevealLineX = 0.0f;
                    targetRevealLineX = isHovered ? 1.0f : 0.5f;
                    
                    currentEraseLineX = 0.0f;
                    targetEraseLineX = 0.0f;
                }
            }

            float oldReveal = currentRevealLineX;
            float oldErase = currentEraseLineX;

            // Very fast lerp (was 10f, now 25f for snappiness)
            currentRevealLineX = Mathf.Lerp(currentRevealLineX, targetRevealLineX, dt * 25f);
            
            // Erase line moves slightly slower to show the sweeping effect clearly
            if (isChangingShader)
            {
                currentEraseLineX = Mathf.Lerp(currentEraseLineX, targetEraseLineX, dt * 18f);
            }
            else
            {
                currentEraseLineX = 0f; // Keep it collapsed on left
            }

            if (Mathf.Abs(currentRevealLineX - oldReveal) > 0.005f ||
                Mathf.Abs(currentEraseLineX - oldErase) > 0.005f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float bSize = Mathf.Min(width, height) * 0.4f;

            float leftX = cx - bSize;
            float rightX = cx + bSize;
            float topY = cy - bSize;
            float botY = cy + bSize;

            float revealAbsoluteX = Mathf.Lerp(leftX, rightX, currentRevealLineX);
            float eraseAbsoluteX = Mathf.Lerp(leftX, rightX, currentEraseLineX);

            // 1. Draw the Solid Outline Box
            painter.BeginPath();
            painter.MoveTo(new Vector2(leftX, topY));
            painter.LineTo(new Vector2(rightX, topY));
            painter.LineTo(new Vector2(rightX, botY));
            painter.LineTo(new Vector2(leftX, botY));
            painter.LineTo(new Vector2(leftX, topY)); // Close loop
            painter.Stroke();

            // 2. Draw the vertical scanner lines (thicker)
            painter.lineWidth = HoveredStrokeWidth;
            painter.strokeColor = currentColor;
            
            // Draw Reveal Line (only if it hasn't reached the end, or if we want it resting)
            if (currentRevealLineX < 1.0f || !isHovered)
            {
                painter.BeginPath();
                painter.MoveTo(new Vector2(revealAbsoluteX, topY - 2f));
                painter.LineTo(new Vector2(revealAbsoluteX, botY + 2f));
                painter.Stroke();
            }

            // Draw Erase Line (only if it's actively sweeping)
            if (currentEraseLineX > 0.01f && currentEraseLineX < 1.0f)
            {
                painter.BeginPath();
                painter.MoveTo(new Vector2(eraseAbsoluteX, topY - 2f));
                painter.LineTo(new Vector2(eraseAbsoluteX, botY + 2f));
                painter.Stroke();
            }
            
            // Revert strict width for grid
            painter.lineWidth = UnhoveredStrokeWidth * 0.6f; // Grid lines are thinner

            // 3. Draw the Wireframe Grid BETWEEN eraseLine and revealLine
            int gridStep = 4; // 3 columns/rows inside the box
            float stepSizeX = (bSize * 2f) / gridStep;
            float stepSizeY = (bSize * 2f) / gridStep;

            painter.BeginPath();
            // Vertical grid lines
            for (int i = 1; i < gridStep; i++)
            {
                float gridX = leftX + (i * stepSizeX);
                // Only draw if it's between the two scanner lines
                if (gridX > eraseAbsoluteX && gridX < revealAbsoluteX)
                {
                    painter.MoveTo(new Vector2(gridX, topY));
                    painter.LineTo(new Vector2(gridX, botY));
                }
            }

            // Horizontal grid lines
            for (int j = 1; j < gridStep; j++)
            {
                float gridY = topY + (j * stepSizeY);
                // Draw line between erase line and reveal line bounds
                float startX = Mathf.Max(leftX, eraseAbsoluteX);
                float endX = Mathf.Min(rightX, revealAbsoluteX);
                
                if (startX < endX) // Ensuring we actually have space to draw
                {
                    painter.MoveTo(new Vector2(startX, gridY));
                    painter.LineTo(new Vector2(endX, gridY));
                }
            }
            
            painter.Stroke();

            painter.lineWidth = currentStrokeWidth; // Restore default
        }
    }
}
