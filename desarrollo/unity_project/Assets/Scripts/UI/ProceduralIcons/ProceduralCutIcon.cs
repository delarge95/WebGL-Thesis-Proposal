using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralCutIcon : ProceduralIconBase
    {
        // 1. Circle Scale
        private float targetCircleScale = 1f;
        private float currentCircleScale = 1f;
        private float circleVelocity = 0f;

        // 2. Scalpel position
        private float targetScalpelPos = 0f;
        private float currentScalpelPos = 0f;
        private float scalpelVelocity = 0f;

        // One-click animation states
        private bool isPerformingCut = false;
        private bool isCutTraceVisible = false;

        protected override void OnHoverEnter()
        {
            if (!isPerformingCut)
            {
                targetCircleScale = 1.1f;
                targetScalpelPos = 0.6f; 
                isCutTraceVisible = false; 
            }
        }

        protected override void OnHoverExit()
        {
            if (!isPerformingCut)
            {
                targetCircleScale = 1f;
                targetScalpelPos = 0f;
                isCutTraceVisible = false;
            }
        }

        protected override void OnPressed()
        {
            // One-click slash action triggers the animation
            isPerformingCut = true;
            targetScalpelPos = -1.1f; // Shortened from -1.5f to stay in bounds
            isCutTraceVisible = true; 
        }

        protected override void OnReleased()
        {
            // Animation is fully handled by UpdateCustomPhysics, no release reset required.
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isPerformingCut)
            {
                // Wait until the scalpel reaches just past the circle bound (-1.0f)
                if (currentScalpelPos <= -1.0f)
                {
                    isPerformingCut = false;
                    isCutTraceVisible = false;
                    
                    // Automatically heal and recover back to mouse state
                    targetCircleScale = isHovered ? 1.1f : 1f;
                    targetScalpelPos = isHovered ? 0.6f : 0f;
                }
            }

            float oldScale = currentCircleScale;
            float oldPos = currentScalpelPos;

            currentCircleScale = SpringFloat(currentCircleScale, targetCircleScale, ref circleVelocity, 25f, 0.7f, dt);
            
            // The Attack is a blazing fast linear slice. The Recovery is a beautiful, slow organic spring glide.
            if (isPerformingCut) 
            {
                currentScalpelPos = Mathf.Lerp(currentScalpelPos, targetScalpelPos, dt * 35f);
            } 
            else 
            {
                currentScalpelPos = SpringFloat(currentScalpelPos, targetScalpelPos, ref scalpelVelocity, 14f, 0.85f, dt);
            }

            if (Mathf.Abs(currentCircleScale - oldScale) > 0.005f ||
                Mathf.Abs(currentScalpelPos - oldPos) > 0.01f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseRadius = Mathf.Min(width, height) * 0.40f;
            float radius = baseRadius * currentCircleScale;

            // ============================================
            // 2. DRAW THE INTACT CIRCLE
            // ============================================

            // Circle stays perfectly still, does not split
            Color circleColor = currentColor;
            circleColor.a *= 0.4f; 
            painter.strokeColor = circleColor;

            painter.BeginPath();
            painter.Arc(new Vector2(cx, cy), radius, 0f, 360f);
            painter.Stroke();

            // ============================================
            // 3. DRAW THE EXACT SLASH TRACE
            // ============================================
            if (isCutTraceVisible)
            {
                painter.strokeColor = currentColor; // Highlight slash
                
                // The Slash traces EXACTLY where the tip of the blade traveled!
                // Cut line starts from the top-right preparing edge (0.6f) and follows the scalpel down
                float slashDistStart = radius * 1.5f * 0.6f; 
                float slashDistEnd = radius * 1.5f * currentScalpelPos; // The moving tip!

                float startX = cx + Mathf.Cos(45f * Mathf.Deg2Rad) * slashDistStart;
                float startY = cy - Mathf.Sin(45f * Mathf.Deg2Rad) * slashDistStart;
                
                float endX = cx + Mathf.Cos(45f * Mathf.Deg2Rad) * slashDistEnd;
                float endY = cy - Mathf.Sin(45f * Mathf.Deg2Rad) * slashDistEnd;

                painter.BeginPath();
                painter.MoveTo(new Vector2(startX, startY));
                painter.LineTo(new Vector2(endX, endY));
                painter.Stroke();
            }

            // Revert stroke color for the scalpel
            painter.strokeColor = currentColor;
            
            // ============================================
            // 1. DRAW SCALPEL
            // ============================================
            float r = baseRadius * 0.9f; 
            
            // The diagonal cut line goes from Top-Right (+radius, -radius) to Bottom-Left (-radius, +radius)
            // We use currentScalpelPos (1 = top right, -1.2 = bottom left) to interpolate its physical center
            
            float cutDistance = radius * 1.5f; // How far the scalpel rides along the diagonal track
            float scalpelCx = cx + Mathf.Cos(45f * Mathf.Deg2Rad) * cutDistance * currentScalpelPos;
            float scalpelCy = cy - Mathf.Sin(45f * Mathf.Deg2Rad) * cutDistance * currentScalpelPos;

            // The Scalpel geometry is itself tilted at 45 degrees
            float angleRad = -45f * Mathf.Deg2Rad;
            float cosA = Mathf.Cos(angleRad);
            float sinA = Mathf.Sin(angleRad);

            // Shape definition. To ensure the TIP tracks EXACTLY on the perfect diameter (Y=0 in local space),
            // we configure p3 (tip) to have y=0.
            Vector2 p1 = RotatePoint(-r * 0.7f, -r * 0.12f, cosA, sinA, scalpelCx, scalpelCy); // Top-left of handle
            Vector2 p2 = RotatePoint(r * 0.2f, -r * 0.12f, cosA, sinA, scalpelCx, scalpelCy);  // Top-right of handle
            Vector2 p3 = RotatePoint(r * 0.8f, 0f, cosA, sinA, scalpelCx, scalpelCy);          // Tip of blade EXACTLY on axis
            Vector2 p4 = RotatePoint(r * 0.3f,  r * 0.25f, cosA, sinA, scalpelCx, scalpelCy);  // Bottom belly of blade
            Vector2 p5 = RotatePoint(r * 0.2f,  r * 0.12f, cosA, sinA, scalpelCx, scalpelCy);  // Bottom-right of handle
            Vector2 p6 = RotatePoint(-r * 0.7f, r * 0.12f, cosA, sinA, scalpelCx, scalpelCy);  // Bottom-left of handle
            
            // Scalpel interior grip line detail
            Vector2 g1 = RotatePoint(-r * 0.6f, 0f, cosA, sinA, scalpelCx, scalpelCy);
            Vector2 g2 = RotatePoint(r * 0.1f, 0f, cosA, sinA, scalpelCx, scalpelCy);

            // Draw Scalpel Outline
            painter.BeginPath();
            painter.MoveTo(p1);
            painter.LineTo(p2);
            painter.LineTo(p3);
            painter.LineTo(p4);
            painter.LineTo(p5);
            painter.LineTo(p6);
            painter.ClosePath();
            painter.Stroke();
            
            // Draw Grip Line
            painter.BeginPath();
            painter.MoveTo(g1);
            painter.LineTo(g2);
            painter.Stroke();
        }

        private Vector2 RotatePoint(float x, float y, float cosA, float sinA, float cx, float cy)
        {
            return new Vector2(
                cx + (x * cosA - y * sinA),
                cy + (x * sinA + y * cosA)
            );
        }
    }
}
