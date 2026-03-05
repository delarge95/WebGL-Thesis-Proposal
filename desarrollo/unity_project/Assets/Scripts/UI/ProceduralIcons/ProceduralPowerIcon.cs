using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralPowerIcon : ProceduralIconBase
    {
        // Arc gap rotation on hover
        private float targetGapRotation = 0f;
        private float currentGapRotation = 0f;
        private float gapVelocity = 0f;

        // Stem pulse scale
        private float targetStemScale = 1f;
        private float currentStemScale = 1f;

        // Click flash
        private bool isFlashing = false;

        protected override void OnHoverEnter()
        {
            if (!isFlashing)
            {
                targetGapRotation = 15f;
                targetStemScale = 1.15f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isFlashing)
            {
                targetGapRotation = 0f;
                targetStemScale = 1f;
            }
        }

        protected override void OnPressed()
        {
            isFlashing = true;
            targetStemScale = 0.6f;
            targetGapRotation = -10f;
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isFlashing && currentStemScale <= 0.65f)
            {
                isFlashing = false;
                targetStemScale = isHovered ? 1.15f : 1f;
                targetGapRotation = isHovered ? 15f : 0f;
            }

            float oldRot = currentGapRotation;
            float oldStem = currentStemScale;

            currentGapRotation = SpringFloat(currentGapRotation, targetGapRotation,
                ref gapVelocity, 20f, 0.65f, dt);
            currentStemScale = Mathf.Lerp(currentStemScale, targetStemScale, dt * 18f);

            if (Mathf.Abs(currentGapRotation - oldRot) > 0.05f ||
                Mathf.Abs(currentStemScale - oldStem) > 0.005f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.38f;

            painter.strokeColor = currentColor;
            painter.lineWidth = currentStrokeWidth;
            painter.lineCap = LineCap.Round;

            // 1. Draw the open arc (power symbol circle with gap at top)
            float radius = baseSize * 0.85f;
            float gapHalf = 30f; // degrees of gap on each side of top
            float startAngle = -90f + gapHalf + currentGapRotation;
            float endAngle = -90f - gapHalf + 360f + currentGapRotation;

            // Draw arc as series of line segments
            int segments = 32;
            float angleStep = (endAngle - startAngle) / segments;
            painter.BeginPath();
            for (int i = 0; i <= segments; i++)
            {
                float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
                float px = cx + Mathf.Cos(angle) * radius;
                float py = cy + Mathf.Sin(angle) * radius;
                if (i == 0) painter.MoveTo(new Vector2(px, py));
                else painter.LineTo(new Vector2(px, py));
            }
            painter.Stroke();

            // 2. Draw the vertical stem (power line)
            float stemTop = cy - baseSize * 0.9f * currentStemScale;
            float stemBottom = cy + baseSize * 0.1f;
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx, stemTop));
            painter.LineTo(new Vector2(cx, stemBottom));
            painter.Stroke();
        }
    }
}
