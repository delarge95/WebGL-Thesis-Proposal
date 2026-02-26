using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralInspectIcon : ProceduralIconBase
    {
        // 1. Rotation (Searching)
        private float targetRotation = 0f; // Base rotation offset
        private float currentRotation = 0f;
        private float rotationVelocity = 0f;

        // 2. Glint Line (Scanning)
        private float currentGlintPos = -1f; // -1 to 1 across the lens
        private bool isInspecting = false;

        protected override void OnHoverEnter()
        {
            if (!isInspecting)
            {
                // Anticipation: The glass rotates to "inspect" closer
                targetRotation = -20f; // Tilt up into action
            }
        }

        protected override void OnHoverExit()
        {
            if (!isInspecting)
            {
                targetRotation = 0f;
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
                currentGlintPos += dt * 10f; 

                if (currentGlintPos >= 1.2f)
                {
                    isInspecting = false;
                    currentGlintPos = -1f; // Reset
                }
            }

            float oldRot = currentRotation;
            currentRotation = SpringFloat(currentRotation, targetRotation, ref rotationVelocity, 30f, 0.6f, dt);

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
            
            // Highly recognizable magnifying glass proportions
            float baseSize = Mathf.Min(width, height) * 0.45f;
            float lensRadius = baseSize * 0.5f; 
            
            painter.strokeColor = currentColor;

            // Offset the center so the whole icon rests symmetrically in the box
            // The handle goes down-right, so move the lens up-left
            Vector2 pivot = new Vector2(cx, cy); 
            Vector2 lensOrigin = new Vector2(cx - baseSize * 0.15f, cy - baseSize * 0.15f);

            // Apply hover rotation to points mathematically (since it's a fixed shape)
            float rotRad = currentRotation * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rotRad);
            float sin = Mathf.Sin(rotRad);

            // Function to rotate a point around pivot
            Vector2 Rotate(Vector2 p)
            {
                float dx = p.x - pivot.x;
                float dy = p.y - pivot.y;
                return new Vector2(
                    pivot.x + (dx * cos - dy * sin),
                    pivot.y + (dx * sin + dy * cos)
                );
            }

            Vector2 rotatedLensCenter = Rotate(lensOrigin);

            // The handle extends at exactly 45 degrees relative to the lens
            Vector2 handleStart = rotatedLensCenter + Rotate(new Vector2(lensRadius, lensRadius)) - pivot; // Approximated start at edge
            Vector2 handleEnd = Rotate(new Vector2(cx + baseSize * 0.45f, cy + baseSize * 0.45f));

            // 1. Draw thick handle
            painter.BeginPath();
            painter.lineWidth = currentStrokeWidth * 1.5f;
            painter.MoveTo(handleStart);
            painter.LineTo(handleEnd);
            painter.Stroke();

            // 2. Draw Lens
            painter.lineWidth = currentStrokeWidth;
            painter.BeginPath();
            painter.Arc(rotatedLensCenter, lensRadius, 0f, 360f);

            Color lensFill = currentColor;
            lensFill.a *= 0.1f;
            painter.fillColor = lensFill;
            painter.Fill();
            painter.Stroke();

            // 3. Draw scanning glint
            if (isInspecting && currentGlintPos > -1f && currentGlintPos < 1f)
            {
                float currentLineXLocal = lensRadius * currentGlintPos;
                float absX = Mathf.Abs(currentLineXLocal);
                
                if (absX < lensRadius)
                {
                    float yExt = Mathf.Sqrt((lensRadius * lensRadius) - (absX * absX)) * 0.95f;
                    
                    // Draw a scanner line (straight vertical relative to lens)
                    Vector2 glintTop = rotatedLensCenter + Rotate(new Vector2(currentLineXLocal, -yExt)) - pivot;
                    Vector2 glintBot = rotatedLensCenter + Rotate(new Vector2(currentLineXLocal, yExt)) - pivot;

                    painter.strokeColor = currentColor;
                    painter.lineWidth = HoveredStrokeWidth * 1.5f;
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
