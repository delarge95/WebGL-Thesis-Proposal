using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement] // This exposes it to UI Builder and UXML
    public partial class ProceduralHomeIcon : ProceduralIconBase
    {
        // Professional, cohesive scale animation instead of detaching roof
        private float targetScale = 1f;
        private float currentScale = 1f;
        private float scaleVelocity = 0f;

        protected override void OnHoverEnter()
        {
            // Subtle, professional 'pop' up (Anticipation)
            targetScale = 1.15f; 
        }

        protected override void OnHoverExit()
        {
            targetScale = 1f;
        }

        protected override void OnPressed()
        {
            // Fast, snappy squash down
            targetScale = 0.8f;
        }

        protected override void OnReleased()
        {
            // If released over the button, return to hover state
            if (isHovered)
            {
                OnHoverEnter(); 
            }
            else
            {
                OnHoverExit();
            }
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;
            float oldScale = currentScale;

            // Fast, snappy spring for professional UI feel (high stiffness, high damping)
            currentScale = SpringFloat(currentScale, targetScale, ref scaleVelocity, 25f, 0.7f, dt);

            if (Mathf.Abs(currentScale - oldScale) > 0.005f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            // Apply scale animated
            float iconSize = Mathf.Min(width, height) * 0.45f * currentScale; 
            
            // Draw a cohesive, continuous line-art house (Material Design style)
            // Make it slightly taller than wide to look "less wide"
            float w = iconSize * 0.85f;
            float h = iconSize;

            float bot = cy + h * 0.8f;
            float top = cy - h * 0.8f;
            float left = cx - w;
            float right = cx + w;

            // The roof intersection points
            float roofY = cy - h * 0.2f;

            painter.BeginPath();
            
            // Start bottom left
            painter.MoveTo(new Vector2(left, bot));
            // Go up left wall to roof eaves
            painter.LineTo(new Vector2(left, roofY));
            // Slight overhang (optional, keeping it minimal: just go straight to peak)
            painter.LineTo(new Vector2(cx, top));
            // Down right roof
            painter.LineTo(new Vector2(right, roofY));
            // Down right wall
            painter.LineTo(new Vector2(right, bot));
            // Across bottom
            painter.LineTo(new Vector2(left, bot));

            painter.Stroke();
        }
    }
}
