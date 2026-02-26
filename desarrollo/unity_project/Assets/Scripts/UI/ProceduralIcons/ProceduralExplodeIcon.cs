using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralExplodeIcon : ProceduralIconBase
    {
        // Spread between layers
        private float targetSpreadY = 0f;
        private float currentSpreadY = 0f;
        private float spreadVelocity = 0f;

        // Overall scale squash on click
        private float targetGroupScale = 1f;
        private float currentGroupScale = 1f;
        private float groupVelocity = 0f;

        // One-click state
        private bool isExploding = false;

        protected override void OnHoverEnter()
        {
            if (!isExploding)
            {
                // Exaggerated separation on hover
                targetSpreadY = 10f; 
            }
        }

        protected override void OnHoverExit()
        {
            if (!isExploding)
            {
                // Noticeable base separation
                targetSpreadY = 4f; 
            }
        }

        protected override void OnPressed()
        {
            // Aggressive outward stretch (One-Click)
            isExploding = true;
            targetSpreadY = 18f; // Huge stretch
            targetGroupScale = 1.1f; // Slight pop 
        }

        protected override void OnReleased()
        {
            // Handled in UpdateCustomPhysics
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isExploding)
            {
                // Reach the massive stretch
                if (currentSpreadY >= 17f)
                {
                    isExploding = false;
                    targetSpreadY = isHovered ? 10f : 4f;
                    targetGroupScale = 1f;
                }
            }

            float oldSpread = currentSpreadY;
            float oldScale = currentGroupScale;

            // Attack is extremely fast lerp, recovery is a spring
            if (isExploding)
            {
                currentSpreadY = Mathf.Lerp(currentSpreadY, targetSpreadY, dt * 45f); // Very aggressive
                currentGroupScale = Mathf.Lerp(currentGroupScale, targetGroupScale, dt * 45f);
            }
            else
            {
                currentSpreadY = SpringFloat(currentSpreadY, targetSpreadY, ref spreadVelocity, 20f, 0.7f, dt);
                currentGroupScale = SpringFloat(currentGroupScale, targetGroupScale, ref groupVelocity, 25f, 0.6f, dt);
            }

            if (Mathf.Abs(currentSpreadY - oldSpread) > 0.01f ||
                Mathf.Abs(currentGroupScale - oldScale) > 0.01f)
            {
                hasChanged = true;
            }

            // Initialization state logic
            if (targetSpreadY == 0 && currentSpreadY == 0 && !isHovered) {
                targetSpreadY = 4f; // New wider resting state
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            // Base size of a single diamond layer
            float diamondWidth = Mathf.Min(width, height) * 0.45f * currentGroupScale;
            float diamondHeight = diamondWidth * 0.5f; // Squashed to fake isometric perspective

            // Draw 3 layers, from bottom to top (rendering order)
            // Bottom Layer
            DrawDiamondLayer(painter, cx, cy + currentSpreadY, diamondWidth, diamondHeight, 0.3f);
            
            // Middle Layer
            DrawDiamondLayer(painter, cx, cy, diamondWidth, diamondHeight, 0.7f);
            
            // Top Layer
            DrawDiamondLayer(painter, cx, cy - currentSpreadY, diamondWidth, diamondHeight, 1.0f);
        }

        private void DrawDiamondLayer(Painter2D painter, float cx, float cy, float halfW, float halfH, float alphaMultiplier)
        {
            Color layerColor = currentColor;
            layerColor.a *= alphaMultiplier;

            painter.strokeColor = layerColor;
            
            // We use Solid fill for the top layer only, or slightly transparent for all to show depth
            Color fillColor = layerColor;
            fillColor.a *= 0.2f; // Much more transparent for the fill
            painter.fillColor = fillColor;

            painter.BeginPath();
            // Top point
            painter.MoveTo(new Vector2(cx, cy - halfH));
            // Right point
            painter.LineTo(new Vector2(cx + halfW, cy));
            // Bottom point
            painter.LineTo(new Vector2(cx, cy + halfH));
            // Left point
            painter.LineTo(new Vector2(cx - halfW, cy));
            // Back to top
            painter.LineTo(new Vector2(cx, cy - halfH));

            painter.Fill(); // Fill interior
            painter.Stroke(); // Draw border
        }
    }
}
