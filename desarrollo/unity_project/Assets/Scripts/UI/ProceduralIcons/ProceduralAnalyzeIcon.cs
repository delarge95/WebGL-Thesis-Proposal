using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralAnalyzeIcon : ProceduralIconBase
    {
        // 1. Data Points Animation
        private float timeAccumulator;
        
        // Target vertical scales for each of the 3 nodes
        private float[] targetY = new float[3] { 0f, 0f, 0f };
        private float[] currentY = new float[3] { 0f, 0f, 0f };
        private float[] velY = new float[3] { 0f, 0f, 0f };

        // 2. Pulse (Click interaction)
        private bool isSpiking = false;
        private float currentSpikeScale = 1f;

        protected override void OnHoverEnter()
        {
            if (!isSpiking)
            {
                // Anticipation: Nodes spread vertically more intensely
                targetY[0] = -5f;
                targetY[1] = 8f;
                targetY[2] = -6f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isSpiking)
            {
                // Relax
                targetY[0] = 0f;
                targetY[1] = 0f;
                targetY[2] = 0f;
            }
        }

        protected override void OnPressed()
        {
            // Extreme vertical spike on all nodes
            if (!isSpiking)
            {
                isSpiking = true;
                currentSpikeScale = 2.5f; 
                velY[0] += 100f; // Positive (Down)
                velY[1] -= 150f; // Negative (Up)
                velY[2] += 120f; // Positive (Down)
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            timeAccumulator += dt;
            bool hasChanged = true; // Always true if we have a breathing idle

            if (isSpiking)
            {
                currentSpikeScale = Mathf.Lerp(currentSpikeScale, 1f, dt * 10f); // Fast decay
                if (currentSpikeScale <= 1.05f)
                {
                    isSpiking = false;
                    currentSpikeScale = 1f;
                    targetY[0] = isHovered ? -5f : 0f;
                    targetY[1] = isHovered ? 8f : 0f;
                    targetY[2] = isHovered ? -6f : 0f;
                }
            }
            else if (!isHovered)
            {
                // Breathing idle
                float breathe = Mathf.Sin(timeAccumulator * 2f) * 2f;
                targetY[0] = breathe;
                targetY[1] = -breathe;
                targetY[2] = breathe * 0.5f;
            }

            // Springs for each node
            for (int i = 0; i < 3; i++)
            {
                currentY[i] = SpringFloat(currentY[i], targetY[i] * currentSpikeScale, ref velY[i], 15f, 0.6f, dt);
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            float spanX = Mathf.Min(width, height) * 0.7f;
            float stepX = spanX / 2f; // 3 points, 2 segments
            float startX = cx - (spanX / 2f);

            painter.strokeColor = currentColor;
            painter.fillColor = currentColor; // Fill the dots

            // Nodes
            Vector2[] pts = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                pts[i] = new Vector2(startX + (stepX * i), cy + currentY[i]);
            }

            // Draw Lines
            painter.BeginPath();
            painter.MoveTo(pts[0]);
            painter.LineTo(pts[1]);
            painter.LineTo(pts[2]);
            painter.Stroke();

            // Draw Dots (Nodes) on top of lines
            float dotRadius = currentStrokeWidth * 1.5f;
            for (int i = 0; i < 3; i++)
            {
                painter.BeginPath();
                painter.Arc(pts[i], dotRadius, 0f, 360f);
                painter.Fill();
                painter.Stroke();
            }
        }
    }
}
