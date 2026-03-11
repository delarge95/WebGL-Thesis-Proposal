using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralCloseIcon : ProceduralIconBase
    {
        private const float BaseRotation = 45f;

        private float targetScale = 1f;
        private float currentScale = 1f;
        private float scaleVelocity = 0f;

        private float targetRotation = BaseRotation;
        private float currentRotation = BaseRotation;
        private float rotationVelocity = 0f;

        protected override void OnHoverEnter()
        {
            targetScale = 1.12f;
            targetRotation = BaseRotation + 8f;
        }

        protected override void OnHoverExit()
        {
            targetScale = 1f;
            targetRotation = BaseRotation;
        }

        protected override void OnPressed()
        {
            targetScale = 0.82f;
            targetRotation = BaseRotation;
        }

        protected override void OnReleased()
        {
            if (isHovered)
                OnHoverEnter();
            else
                OnHoverExit();
        }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool changed = false;

            float oldScale = currentScale;
            float oldRotation = currentRotation;

            currentScale = SpringFloat(currentScale, targetScale, ref scaleVelocity, 24f, 0.72f, dt);
            currentRotation = SpringFloat(currentRotation, targetRotation, ref rotationVelocity, 20f, 0.75f, dt);

            if (Mathf.Abs(currentScale - oldScale) > 0.005f || Mathf.Abs(currentRotation - oldRotation) > 0.05f)
                changed = true;

            return changed;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width * 0.5f;
            float cy = height * 0.5f;
            float half = Mathf.Min(width, height) * 0.22f * currentScale;
            float angle = currentRotation * Mathf.Deg2Rad;

            Vector2 axisA = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * half;
            Vector2 axisB = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * half;
            Vector2 center = new Vector2(cx, cy);

            painter.BeginPath();
            painter.MoveTo(center - axisA);
            painter.LineTo(center + axisA);
            painter.MoveTo(center - axisB);
            painter.LineTo(center + axisB);
            painter.Stroke();
        }
    }
}