using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    /// <summary>
    /// Procedural ruler/dimension icon for the MEASURE card in Inspect mode.
    /// Draws a horizontal dimension line with end-caps, dots, and tick marks.
    /// </summary>
    [UxmlElement]
    public partial class ProceduralMeasureIcon : ProceduralIconBase
    {
        public ProceduralMeasureIcon() { }

        // Endpoint spread
        private float targetSpread = 1f;
        private float currentSpread = 1f;
        private float spreadVelocity = 0f;

        // Tick flash alpha
        private float targetTickAlpha = 0.4f;
        private float currentTickAlpha = 0.4f;

        // One-click pulse
        private bool isMeasuring = false;

        protected override void OnHoverEnter()
        {
            if (!isMeasuring)
            {
                targetSpread = 1.15f;
                targetTickAlpha = 1f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isMeasuring)
            {
                targetSpread = 1f;
                targetTickAlpha = 0.4f;
            }
        }

        protected override void OnPressed()
        {
            isMeasuring = true;
            targetSpread = 0.6f;
            targetTickAlpha = 1.2f;
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isMeasuring)
            {
                if (currentSpread <= 0.65f)
                {
                    isMeasuring = false;
                    targetSpread = isHovered ? 1.15f : 1f;
                    targetTickAlpha = isHovered ? 1f : 0.4f;
                }
            }

            float oldSpread = currentSpread;
            float oldAlpha  = currentTickAlpha;

            if (isMeasuring)
                currentSpread = Mathf.Lerp(currentSpread, targetSpread, dt * 40f);
            else
                currentSpread = SpringFloat(currentSpread, targetSpread, ref spreadVelocity, 30f, 0.7f, dt);

            currentTickAlpha = Mathf.Lerp(currentTickAlpha, targetTickAlpha, dt * 20f);

            if (Mathf.Abs(currentSpread - oldSpread) > 0.005f ||
                Mathf.Abs(currentTickAlpha - oldAlpha) > 0.005f)
                hasChanged = true;

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width  / 2f;
            float cy = height / 2f;
            float baseSize = Mathf.Min(width, height) * 0.40f;

            float halfLen  = baseSize * currentSpread;
            float capH     = baseSize * 0.30f;   // height of the end-cap ticks
            float dotR     = baseSize * 0.08f;

            // ── Main horizontal dimension line ──
            painter.strokeColor = currentColor;
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - halfLen, cy));
            painter.LineTo(new Vector2(cx + halfLen, cy));
            painter.Stroke();

            // ── End-cap verticals ──
            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - halfLen, cy - capH));
            painter.LineTo(new Vector2(cx - halfLen, cy + capH));
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(new Vector2(cx + halfLen, cy - capH));
            painter.LineTo(new Vector2(cx + halfLen, cy + capH));
            painter.Stroke();

            // ── Endpoint dots ──
            painter.fillColor = currentColor;
            painter.BeginPath();
            painter.Arc(new Vector2(cx - halfLen, cy), dotR, 0f, 360f);
            painter.Fill();

            painter.BeginPath();
            painter.Arc(new Vector2(cx + halfLen, cy), dotR, 0f, 360f);
            painter.Fill();

            // ── Center tick marks (subtle ruler vibe) ──
            Color tickCol = currentColor;
            tickCol.a *= Mathf.Clamp01(currentTickAlpha);
            painter.strokeColor = tickCol;

            int ticks = 3;
            for (int i = 1; i <= ticks; i++)
            {
                float t  = (float)i / (ticks + 1);
                float tx = Mathf.Lerp(cx - halfLen, cx + halfLen, t);
                float th = capH * 0.45f;
                painter.BeginPath();
                painter.MoveTo(new Vector2(tx, cy - th));
                painter.LineTo(new Vector2(tx, cy + th));
                painter.Stroke();
            }
        }
    }
}
