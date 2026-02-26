using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralAnalyzeIcon : ProceduralIconBase
    {
        // 1. Data Bars (3 Vertical Bars) heights
        private float[] targetHeights = new float[3] { 0f, 0f, 0f };
        private float[] currentHeights = new float[3] { 0f, 0f, 0f };
        private float[] velHeights = new float[3] { 0f, 0f, 0f };

        // Pulse (Click interaction)
        private bool isSpiking = false;

        private float[] baseHeights = new float[3] { 0.3f, 0.6f, 0.4f }; // Proportional idle size
        private float[] hoverHeights = new float[3] { 0.5f, 0.8f, 0.6f }; // Taller

        // Init flag to avoid startup stretching
        private bool isInitialized = false;

        protected override void OnHoverEnter()
        {
            if (!isSpiking && isInitialized)
            {
                // Anticipation: Bars grow taller
                for (int i = 0; i < 3; i++) targetHeights[i] = hoverHeights[i];
            }
        }

        protected override void OnHoverExit()
        {
            if (!isSpiking && isInitialized)
            {
                // Relax
                for (int i = 0; i < 3; i++) targetHeights[i] = baseHeights[i];
            }
        }

        protected override void OnPressed()
        {
            // Extreme vertical spike on all bars (1.0 = full box height)
            if (!isSpiking && isInitialized)
            {
                isSpiking = true;
                velHeights[0] += 5f; // Fast upward impulse
                velHeights[1] += 6f; 
                velHeights[2] += 4f; 
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            // Startup setup to avoid layout stretches
            if (!isInitialized)
            {
                for (int i = 0; i < 3; i++)
                {
                    targetHeights[i] = baseHeights[i];
                    currentHeights[i] = baseHeights[i];
                }
                isInitialized = true;
                return true;
            }

            bool hasChanged = false;

            if (isSpiking)
            {
                // Revert to stable spring targets once the spike has reached the top naturally via velocity
                // Our vel goes up, then gravity (spring) pulls it down. We let the spring do all the work.
                
                // If velocity is falling and we are somewhat close to hover state, we are done spiking
                if (velHeights[1] < 0f && currentHeights[1] < hoverHeights[1] + 0.1f)
                {
                    isSpiking = false;
                    for (int i = 0; i < 3; i++) targetHeights[i] = isHovered ? hoverHeights[i] : baseHeights[i];
                }
            }

            for (int i = 0; i < 3; i++)
            {
                float oldH = currentHeights[i];
                // Smooth, bouncy spring
                currentHeights[i] = SpringFloat(currentHeights[i], targetHeights[i], ref velHeights[i], 25f, 0.5f, dt);

                if (Mathf.Abs(currentHeights[i] - oldH) > 0.005f)
                {
                    hasChanged = true;
                }
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            if (!isInitialized) return;

            float cx = width / 2f;
            float cy = height / 2f;
            
            float safeSize = Mathf.Min(width, height) * 0.45f;
            float botY = cy + safeSize * 0.8f;
            
            painter.strokeColor = currentColor;
            painter.fillColor = currentColor;

            // Box/Grid bounds (very subtle)
            float leftX = cx - safeSize * 0.8f;
            float rightX = cx + safeSize * 0.8f;
            float topMaxY = cy - safeSize * 0.8f;
            
            // Draw baseline
            painter.BeginPath();
            painter.MoveTo(new Vector2(leftX, botY));
            painter.LineTo(new Vector2(rightX, botY));
            painter.Stroke();

            // 3 Bars
            float totalAvailableWidth = rightX - leftX;
            float barWidth = totalAvailableWidth / 5f;
            float spacing = barWidth; // bar, space, bar, space, bar = 5 units
            
            float maxBarHeight = botY - topMaxY;

            for (int i = 0; i < 3; i++)
            {
                float barStartX = leftX + (i * (barWidth + spacing));
                
                // Actual animated height (clamped visually just in case)
                float h = Mathf.Max(0.05f, currentHeights[i] * maxBarHeight);
                float barTopY = botY - h;

                painter.BeginPath();
                painter.MoveTo(new Vector2(barStartX, botY));
                painter.LineTo(new Vector2(barStartX, barTopY));
                painter.LineTo(new Vector2(barStartX + barWidth, barTopY));
                painter.LineTo(new Vector2(barStartX + barWidth, botY));
                painter.ClosePath();
                
                painter.Fill();
            }
        }
    }
}
