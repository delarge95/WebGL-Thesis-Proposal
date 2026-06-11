using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralAngleIcon : ProceduralIconBase
    {
        private float targetAngleDeg = 48f;
        private float currentAngleDeg = 48f;
        private float angleVelocity = 0f;

        private float targetArcScale = 1f;
        private float currentArcScale = 1f;
        private float arcVelocity = 0f;

        private bool isSnapping;

        public ProceduralAngleIcon()
        {
            UnhoveredStrokeWidth = 1.8f;
            HoveredStrokeWidth = 2.35f;
        }

        protected override void OnHoverEnter()
        {
            if (!isSnapping)
            {
                targetAngleDeg = 66f;
                targetArcScale = 1.08f;
            }
        }

        protected override void OnHoverExit()
        {
            if (!isSnapping)
            {
                targetAngleDeg = 48f;
                targetArcScale = 1f;
            }
        }

        protected override void OnPressed()
        {
            isSnapping = true;
            targetAngleDeg = 92f;
            targetArcScale = 1.2f;
        }

        protected override void OnReleased()
        {
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool changed = false;
            float oldAngle = currentAngleDeg;
            float oldArc = currentArcScale;

            if (isSnapping)
            {
                currentAngleDeg = Mathf.Lerp(currentAngleDeg, targetAngleDeg, dt * 22f);
                currentArcScale = Mathf.Lerp(currentArcScale, targetArcScale, dt * 20f);
                if (currentAngleDeg >= 88f)
                {
                    isSnapping = false;
                    targetAngleDeg = isHovered ? 66f : 48f;
                    targetArcScale = isHovered ? 1.08f : 1f;
                }
            }
            else
            {
                currentAngleDeg = SpringFloat(currentAngleDeg, targetAngleDeg, ref angleVelocity, 24f, 0.78f, dt);
                currentArcScale = SpringFloat(currentArcScale, targetArcScale, ref arcVelocity, 24f, 0.78f, dt);
            }

            if (Mathf.Abs(currentAngleDeg - oldAngle) > 0.01f || Mathf.Abs(currentArcScale - oldArc) > 0.01f)
            {
                changed = true;
            }

            return changed;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width * 0.3f;
            float cy = height * 0.72f;
            float rayLength = Mathf.Min(width, height) * 0.54f;

            Vector2 baseDir = Vector2.right;
            Vector2 upperDir = new Vector2(Mathf.Cos(-currentAngleDeg * Mathf.Deg2Rad), Mathf.Sin(-currentAngleDeg * Mathf.Deg2Rad));

            painter.BeginPath();
            painter.MoveTo(new Vector2(cx, cy));
            painter.LineTo(new Vector2(cx, cy) + baseDir * rayLength);
            painter.Stroke();

            painter.BeginPath();
            painter.MoveTo(new Vector2(cx, cy));
            painter.LineTo(new Vector2(cx, cy) + upperDir * rayLength);
            painter.Stroke();

            float arcRadius = rayLength * 0.42f * currentArcScale;
            painter.BeginPath();
            painter.Arc(new Vector2(cx, cy), arcRadius, -currentAngleDeg, 0f);
            painter.Stroke();
        }
    }
}
