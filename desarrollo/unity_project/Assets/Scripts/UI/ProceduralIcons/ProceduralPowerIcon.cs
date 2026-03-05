using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralPowerIcon : ProceduralIconBase
    {
        // Stem length animation (grows on hover, pulses on click)
        private float targetStemScale = 1f;
        private float currentStemScale = 1f;
        private float stemVelocity = 0f;

        // Click punch
        private bool isPunching = false;

        protected override void OnHoverEnter()
        {
            if (!isPunching)
                targetStemScale = 1.25f;
        }

        protected override void OnHoverExit()
        {
            if (!isPunching)
                targetStemScale = 1f;
        }

        protected override void OnPressed()
        {
            isPunching = true;
            targetStemScale = 0.5f; // shrink fast
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            if (isPunching && currentStemScale <= 0.55f)
            {
                isPunching = false;
                targetStemScale = isHovered ? 1.25f : 1f;
            }

            float oldStem = currentStemScale;
            currentStemScale = SpringFloat(currentStemScale, targetStemScale,
                ref stemVelocity, 22f, 0.6f, dt);

            return Mathf.Abs(currentStemScale - oldStem) > 0.003f;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.38f;

            painter.strokeColor = currentColor;
            painter.lineWidth = currentStrokeWidth;
            painter.lineCap = LineCap.Round;

            // 1. Static open arc (power symbol circle with gap at top)
            float radius = baseSize * 0.85f;
            float gapHalf = 30f;
            float startAngle = -90f + gapHalf;
            float endAngle = -90f - gapHalf + 360f;

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

            // 2. Animated vertical stem
            float stemLen = baseSize * 0.9f * currentStemScale;
            float stemTop = cy - stemLen;
            float stemBottom = cy + baseSize * 0.1f;
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx, stemTop));
            painter.LineTo(new Vector2(cx, stemBottom));
            painter.Stroke();
        }
    }
}
