using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralIsolateIcon : ProceduralIconBase
    {
        // The central isolate object (Square)
        private float targetSquareAlpha = 1f;
        private float currentSquareAlpha = 1f;

        // The outer brackets
        private float targetBracketScale = 1f;
        private float currentBracketScale = 1f;
        private float bracketVelocity = 0f;

        // One-Click state
        private bool isIsolating = false;

        protected override void OnHoverEnter()
        {
            if (!isIsolating)
            {
                // Focus effect: Brackets snap inward quickly, Square becomes highlighted (pulsing alpha slightly)
                targetBracketScale = 0.7f; 
                targetSquareAlpha = 1f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isIsolating)
            {
                // Relaxed state
                targetBracketScale = 1f;
                targetSquareAlpha = 0.5f; // Dim the center object when not isolated
            }
        }

        protected override void OnPressed()
        {
            // Extremely aggressive ONE-CLICK shutter snap effect
            isIsolating = true;
            targetBracketScale = 0.15f; // Closes almost entirely
            targetSquareAlpha = 1.2f; 
        }

        protected override void OnReleased()
        {
            // Handled dynamically in physics
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isIsolating)
            {
                if (currentBracketScale <= 0.2f)
                {
                    isIsolating = false;
                    targetBracketScale = isHovered ? 0.7f : 1f;
                    targetSquareAlpha = isHovered ? 1f : 0.5f;
                }
            }

            float oldScale = currentBracketScale;
            float oldAlpha = currentSquareAlpha;

            if (isIsolating)
            {
                currentBracketScale = Mathf.Lerp(currentBracketScale, targetBracketScale, dt * 50f); // Violent snap
            }
            else
            {
                currentBracketScale = SpringFloat(currentBracketScale, targetBracketScale, ref bracketVelocity, 35f, 0.75f, dt); // Crisper recovery
            }
            
            // Fast lerp for alpha
            currentSquareAlpha = Mathf.Lerp(currentSquareAlpha, targetSquareAlpha, dt * 25f);

            if (Mathf.Abs(currentBracketScale - oldScale) > 0.01f ||
                Mathf.Abs(currentSquareAlpha - oldAlpha) > 0.01f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.45f;

            // 1. Draw Central Solid Target
            Color squareColor = currentColor;
            squareColor.a *= currentSquareAlpha;
            painter.fillColor = squareColor;

            float squareSize = baseSize * 0.25f; // Always same physical size, just alpha changes
            
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - squareSize, cy - squareSize));
            painter.LineTo(new Vector2(cx + squareSize, cy - squareSize));
            painter.LineTo(new Vector2(cx + squareSize, cy + squareSize));
            painter.LineTo(new Vector2(cx - squareSize, cy + squareSize));
            painter.ClosePath();
            painter.Fill();

            // 2. Draw Outer Targeting Brackets
            painter.strokeColor = currentColor; 
            
            // Brackets scale inward/outward
            float bHeight = baseSize * 0.8f * currentBracketScale;
            float bWidth = baseSize * 0.8f * currentBracketScale;
            float bracketArm = baseSize * 0.25f; // The corner length

            // Top Left Corner
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - bWidth, cy - bHeight + bracketArm));
            painter.LineTo(new Vector2(cx - bWidth, cy - bHeight));
            painter.LineTo(new Vector2(cx - bWidth + bracketArm, cy - bHeight));
            painter.Stroke();

            // Top Right Corner
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx + bWidth - bracketArm, cy - bHeight));
            painter.LineTo(new Vector2(cx + bWidth, cy - bHeight));
            painter.LineTo(new Vector2(cx + bWidth, cy - bHeight + bracketArm));
            painter.Stroke();

            // Bottom Left Corner
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - bWidth, cy + bHeight - bracketArm));
            painter.LineTo(new Vector2(cx - bWidth, cy + bHeight));
            painter.LineTo(new Vector2(cx - bWidth + bracketArm, cy + bHeight));
            painter.Stroke();

            // Bottom Right Corner
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx + bWidth - bracketArm, cy + bHeight));
            painter.LineTo(new Vector2(cx + bWidth, cy + bHeight));
            painter.LineTo(new Vector2(cx + bWidth, cy + bHeight - bracketArm));
            painter.Stroke();
        }
    }
}
