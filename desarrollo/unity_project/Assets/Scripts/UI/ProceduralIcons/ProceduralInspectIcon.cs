using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralInspectIcon : ProceduralIconBase
    {
        // 1. Rotation (Searching)
        private float targetRotation = 45f;
        private float currentRotation = 45f;
        private float rotationVelocity = 0f;

        // 2. Glint Line (Scanning)
        private float currentGlintPos = -1f; // -1 to 1 across the lens
        private bool isInspecting = false;

        protected override void OnHoverEnter()
        {
            if (!isInspecting)
            {
                // Anticipation: The glass rotates to "inspect" closer
                targetRotation = 15f; 
            }
        }

        protected override void OnHoverExit()
        {
            if (!isInspecting)
            {
                targetRotation = 45f;
            }
        }

        protected override void OnPressed()
        {
            // Trigger a high-speed scanner glint across the glass
            if (!isInspecting)
            {
                isInspecting = true;
                currentGlintPos = -1.2f; // Start outside left
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isInspecting)
            {
                // Fast sweep
                currentGlintPos += dt * 8f; 

                if (currentGlintPos >= 1.2f)
                {
                    isInspecting = false;
                    currentGlintPos = -1f; // Reset
                }
            }

            float oldRot = currentRotation;
            currentRotation = SpringFloat(currentRotation, targetRotation, ref rotationVelocity, 25f, 0.6f, dt);

            if (Mathf.Abs(currentRotation - oldRot) > 0.05f || isInspecting)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.35f; // lens size

            // We must rotate the entire context. UI Toolkit Painter2D uses Matrix modifications?
            // Since we only draw primitives, we can just do math rotation manually for points.
            // But center is always center.
            float lenHandle = baseSize * 1.5f;

            painter.strokeColor = currentColor;

            // 1. Calculate glass offset (We'll pivot around the lens center for simplicity)
            float rotRad = currentRotation * Mathf.Deg2Rad;
            Vector2 lensCenter = new Vector2(cx - baseSize * 0.2f, cy - baseSize * 0.2f);
            
            // Handle end
            float hx = lensCenter.x + Mathf.Cos(rotRad) * lenHandle;
            float hy = lensCenter.y + Mathf.Sin(rotRad) * lenHandle;

            // Handle
            painter.BeginPath();
            painter.MoveTo(lensCenter);
            painter.LineTo(new Vector2(hx, hy));
            painter.Stroke();

            // Lens (Draw it after handle so it overlays)
            painter.BeginPath();
            painter.Arc(lensCenter, baseSize, 0f, 360f);

            // Fill the inner lens very slightly to make the glint pop
            Color lensFill = currentColor;
            lensFill.a *= 0.1f;
            painter.fillColor = lensFill;
            painter.Fill();
            painter.Stroke();

            // 2. Glint (Scanner line inside the lens)
            if (isInspecting && currentGlintPos > -1f && currentGlintPos < 1f)
            {
                // We draw a slanted line across the circle.
                float currentLineXLocal = baseSize * currentGlintPos;
                
                // Using circle equation: x^2 + y^2 = r^2  => y = sqrt(r^2 - x^2)
                float absX = Mathf.Abs(currentLineXLocal);
                if (absX < baseSize)
                {
                    float yExt = Mathf.Sqrt((baseSize * baseSize) - (currentLineXLocal * currentLineXLocal)) * 0.9f;
                    
                    Vector2 glintTop = new Vector2(lensCenter.x + currentLineXLocal, lensCenter.y - yExt);
                    Vector2 glintBot = new Vector2(lensCenter.x + currentLineXLocal, lensCenter.y + yExt);

                    painter.strokeColor = currentColor;
                    painter.lineWidth = HoveredStrokeWidth * 1.5f; // thick glint
                    painter.BeginPath();
                    painter.MoveTo(glintTop);
                    painter.LineTo(glintBot);
                    painter.Stroke();
                    painter.lineWidth = currentStrokeWidth; // revert
                }
            }
        }
    }
}
