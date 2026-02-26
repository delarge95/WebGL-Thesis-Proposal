using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralFilterIcon : ProceduralIconBase
    {
        // Funnel Widths
        private float targetTopWidth = 0.8f;
        private float currentTopWidth = 0.8f;
        
        private float targetTubeWidth = 0.2f;
        private float currentTubeWidth = 0.2f;

        private float funnelVelocityTop = 0f;
        private float funnelVelocityTube = 0f;

        // Container Squash
        private float targetScaleY = 1f;
        private float currentScaleY = 1f;
        private float scaleYVelocity = 0f;

        // Drop animation (One-click)
        private bool isFiltering = false;
        private float currentDropY = 0f; // offset from tube bottom

        protected override void OnHoverEnter()
        {
            if (!isFiltering)
            {
                // Anticipation: Funnel opens wider to accept data
                targetTopWidth = 1.0f;
                targetTubeWidth = 0.3f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isFiltering)
            {
                targetTopWidth = 0.8f;
                targetTubeWidth = 0.2f;
            }
        }

        protected override void OnPressed()
        {
            // Squash and filter drop (One-Click)
            if (!isFiltering)
            {
                isFiltering = true;
                currentDropY = 0.1f; // start the drop
                
                // Squash the funnel down
                targetScaleY = 0.7f;
            }
        }

        protected override void OnReleased()
        {
            // Animation fully handled by physics loop
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isFiltering)
            {
                // Drop accelerates downwards fast!
                currentDropY += dt * 80f; // Velocity
                
                // When drop reaches far bottom, trigger recovery
                if (currentDropY >= 25f)
                {
                    isFiltering = false;
                    currentDropY = 0f;
                    targetScaleY = 1f;
                    targetTopWidth = isHovered ? 1.0f : 0.8f;
                    targetTubeWidth = isHovered ? 0.3f : 0.2f;
                }
            }

            float oldTop = currentTopWidth;
            float oldTube = currentTubeWidth;
            float oldScale = currentScaleY;

            // Spring interpolations
            currentTopWidth = SpringFloat(currentTopWidth, targetTopWidth, ref funnelVelocityTop, 25f, 0.7f, dt);
            currentTubeWidth = SpringFloat(currentTubeWidth, targetTubeWidth, ref funnelVelocityTube, 25f, 0.7f, dt);
            
            // Rebound squash using an elastic spring
            currentScaleY = SpringFloat(currentScaleY, targetScaleY, ref scaleYVelocity, 35f, 0.5f, dt);

            if (Mathf.Abs(currentTopWidth - oldTop) > 0.005f ||
                Mathf.Abs(currentTubeWidth - oldTube) > 0.005f ||
                Mathf.Abs(currentScaleY - oldScale) > 0.005f || 
                isFiltering)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseRadius = Mathf.Min(width, height) * 0.45f;
            
            float topY = cy - baseRadius * 0.6f;
            float midY = cy + baseRadius * 0.2f;
            float tubeY = cy + baseRadius * 0.7f;
            
            // Squash scaling relative to the bottom of the tube
            float totalHeight = tubeY - topY;
            float centerSquashY = tubeY; // Squash from bottom up
            
            topY = Mathf.Lerp(centerSquashY, topY, currentScaleY);
            midY = Mathf.Lerp(centerSquashY, midY, currentScaleY);
            
            // 1. Draw Funnel Outline
            painter.strokeColor = currentColor;
            painter.BeginPath();
            
            float scaledTopW = baseRadius * currentTopWidth;
            float scaledTubeW = baseRadius * currentTubeWidth;

            painter.MoveTo(new Vector2(cx - scaledTopW, topY));
            painter.LineTo(new Vector2(cx + scaledTopW, topY));
            // Diagonal right
            painter.LineTo(new Vector2(cx + scaledTubeW, midY));
            // Tube right
            painter.LineTo(new Vector2(cx + scaledTubeW, tubeY));
            // Bottom opening
            painter.LineTo(new Vector2(cx - scaledTubeW, tubeY));
            // Tube left
            painter.LineTo(new Vector2(cx - scaledTubeW, midY));
            // Diagonal left back up
            painter.ClosePath();
            
            painter.Stroke();

            // 2. Draw Filtering Drop (if active)
            if (isFiltering && currentDropY > 1f)
            {
                // The drop stretches vertically as it falls (animation principle)
                float dropStretchY = Mathf.Clamp(currentDropY * 0.2f, 1f, 3f);
                float dropRadius = baseRadius * 0.15f; 
                float dropX = cx;
                float dropY = tubeY + currentDropY;
                
                // Fade out as it falls
                float alpha = Mathf.Clamp01(1f - (currentDropY / 25f));
                Color dropColor = currentColor;
                dropColor.a *= alpha;
                
                painter.fillColor = dropColor;
                painter.strokeColor = dropColor;

                // Draw drop (ellipse approximation using line hack for UI Toolkit Painter2D without Ellipse support)
                painter.lineWidth = dropRadius * 2f;
                painter.BeginPath();
                painter.MoveTo(new Vector2(dropX, dropY - dropStretchY * 2f));
                painter.LineTo(new Vector2(dropX, dropY + dropStretchY));
                painter.Stroke();
                
                painter.lineWidth = currentStrokeWidth; // revert
            }
        }
    }
}
