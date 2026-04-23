using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralInvertIcon : ProceduralIconBase
    {
        private float targetSpread = 7f;
        private float currentSpread = 7f;
        private float spreadVelocity = 0f;

        private float targetSwap = 0f;
        private float currentSwap = 0f;
        private float swapVelocity = 0f;

        private bool isSwapping;

        public ProceduralInvertIcon()
        {
            UnhoveredStrokeWidth = 1.95f;
            HoveredStrokeWidth = 2.5f;
        }

        protected override void OnHoverEnter()
        {
            if (!isSwapping)
            {
                targetSpread = 9.5f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isSwapping)
            {
                targetSpread = 7f;
            }
        }

        protected override void OnPressed()
        {
            isSwapping = true;
            targetSpread = 12f;
            targetSwap = 1f;
        }

        protected override void OnReleased()
        {
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool changed = false;
            float oldSpread = currentSpread;
            float oldSwap = currentSwap;

            currentSpread = SpringFloat(currentSpread, targetSpread, ref spreadVelocity, 26f, 0.72f, dt);

            if (isSwapping)
            {
                currentSwap = Mathf.Lerp(currentSwap, targetSwap, dt * 26f);
                if (currentSwap >= 0.95f)
                {
                    isSwapping = false;
                    targetSwap = 0f;
                    targetSpread = isHovered ? 9.5f : 7f;
                }
            }
            else
            {
                currentSwap = SpringFloat(currentSwap, targetSwap, ref swapVelocity, 24f, 0.78f, dt);
            }

            if (Mathf.Abs(currentSpread - oldSpread) > 0.01f || Mathf.Abs(currentSwap - oldSwap) > 0.01f)
            {
                changed = true;
            }

            return changed;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width * 0.5f;
            float cy = height * 0.5f;
            float lane = currentSpread;
            float horizontalTravel = Mathf.Lerp(0f, width * 0.13f, currentSwap);
            float lineLength = width * 0.43f;
            float arrowSize = width * 0.17f;

            DrawArrow(painter, cx - lineLength * 0.5f + horizontalTravel, cy - lane, cx + lineLength * 0.5f + horizontalTravel, cy - lane, arrowSize);
            DrawArrow(painter, cx + lineLength * 0.5f - horizontalTravel, cy + lane, cx - lineLength * 0.5f - horizontalTravel, cy + lane, arrowSize);
        }

        private void DrawArrow(Painter2D painter, float x1, float y1, float x2, float y2, float arrowSize)
        {
            painter.BeginPath();
            painter.MoveTo(new Vector2(x1, y1));
            painter.LineTo(new Vector2(x2, y2));
            painter.Stroke();

            Vector2 dir = new Vector2(x2 - x1, y2 - y1).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x);
            Vector2 tip = new Vector2(x2, y2);
            Vector2 back = tip - dir * arrowSize;

            painter.BeginPath();
            painter.MoveTo(tip);
            painter.LineTo(back + perp * arrowSize * 0.62f);
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(tip);
            painter.LineTo(back - perp * arrowSize * 0.62f);
            painter.Stroke();
        }
    }
}
