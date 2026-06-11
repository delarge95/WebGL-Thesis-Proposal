using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralInspectIcon : ProceduralIconBase
    {
        // 1. Bracket Spread (Focus in/out)
        private float targetSpread = 1f;
        private float currentSpread = 1f;
        private float spreadVelocity = 0f;

        // 2. Scanner Line
        private float currentScannerY = -1.2f; // -1 to 1 local
        private bool isInspecting = false;

        protected override void OnHoverEnter()
        {
            if (!isInspecting)
            {
                // Anticipation: Focus brackets expand slightly
                targetSpread = 1.25f; 
            }
        }

        protected override void OnHoverExit()
        {
            if (!isInspecting)
            {
                targetSpread = 1f;
            }
        }

        protected override void OnPressed()
        {
            // Rapid Focus Snap and Scanner Activation
            if (!isInspecting)
            {
                isInspecting = true;
                targetSpread = 0.8f; // Snap tighter
                currentScannerY = -1.2f; // Start scanner above
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isInspecting)
            {
                // Fast scanner sweep
                currentScannerY += dt * 8f; 

                if (currentScannerY >= 1.2f)
                {
                    isInspecting = false;
                    currentScannerY = -1f; // Reset
                    targetSpread = isHovered ? 1.25f : 1f;
                }
            }

            float oldSpread = currentSpread;
            
            if (isInspecting)
            {
                currentSpread = Mathf.Lerp(currentSpread, targetSpread, dt * 40f); // Quick snap
            }
            else
            {
                currentSpread = SpringFloat(currentSpread, targetSpread, ref spreadVelocity, 30f, 0.6f, dt); // Bouncy breath
            }

            if (Mathf.Abs(currentSpread - oldSpread) > 0.005f || isInspecting)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            
            // Base bounding box for the focus brackets
            float baseSize = Mathf.Min(width, height) * 0.40f * currentSpread;
            float cornerLen = baseSize * 0.4f; // Length of the L shape arms
            
            painter.strokeColor = currentColor;
            painter.lineWidth = currentStrokeWidth;

            // Define four corners
            Vector2 tl = new Vector2(cx - baseSize, cy - baseSize);
            Vector2 tr = new Vector2(cx + baseSize, cy - baseSize);
            Vector2 bl = new Vector2(cx - baseSize, cy + baseSize);
            Vector2 br = new Vector2(cx + baseSize, cy + baseSize);

            // 1. Draw Top-Left Bracket
            painter.BeginPath();
            painter.MoveTo(tl + new Vector2(0, cornerLen));
            painter.LineTo(tl);
            painter.LineTo(tl + new Vector2(cornerLen, 0));
            painter.Stroke();

            // 2. Draw Top-Right Bracket
            painter.BeginPath();
            painter.MoveTo(tr + new Vector2(-cornerLen, 0));
            painter.LineTo(tr);
            painter.LineTo(tr + new Vector2(0, cornerLen));
            painter.Stroke();

            // 3. Draw Bottom-Left Bracket
            painter.BeginPath();
            painter.MoveTo(bl + new Vector2(0, -cornerLen));
            painter.LineTo(bl);
            painter.LineTo(bl + new Vector2(cornerLen, 0));
            painter.Stroke();

            // 4. Draw Bottom-Right Bracket
            painter.BeginPath();
            painter.MoveTo(br + new Vector2(-cornerLen, 0));
            painter.LineTo(br);
            painter.LineTo(br + new Vector2(0, -cornerLen));
            painter.Stroke();

            // 5. Draw the Central Dot (Focus Point)
            painter.BeginPath();
            painter.Arc(new Vector2(cx, cy), currentStrokeWidth * 1.5f, 0f, 360f);
            painter.fillColor = currentColor;
            painter.Fill();

            // 6. Draw Scanner Line
            if (isInspecting && currentScannerY > -1f && currentScannerY < 1f)
            {
                float scanY = cy + (baseSize * currentScannerY);
                
                // Draw horizontal line
                painter.strokeColor = currentColor;
                painter.lineWidth = HoveredStrokeWidth * 1.5f; // Thicker scanner
                painter.BeginPath();
                painter.MoveTo(new Vector2(tl.x - cornerLen*0.2f, scanY));
                painter.LineTo(new Vector2(tr.x + cornerLen*0.2f, scanY));
                painter.Stroke();
                
                painter.lineWidth = currentStrokeWidth; // Revert
            }
        }
    }
}
