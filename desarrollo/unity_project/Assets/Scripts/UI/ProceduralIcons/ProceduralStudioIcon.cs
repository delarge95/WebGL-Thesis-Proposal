using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.ProceduralIcons
{
    [UxmlElement]
    public partial class ProceduralStudioIcon : ProceduralIconBase
    {
        // Continuous rotation on Hover
        private float currentRotation = 0f;
        private float targetRotationSpeed = 0f;
        private float currentRotationSpeed = 0f;

        // Pulse scale for click
        private float targetPulseScale = 1f;
        private float currentPulseScale = 1f;
        private float pulseVelocity = 0f;
        
        // One-click state
        private bool isPulsing = false;

        protected override void OnHoverEnter()
        {
            if (!isPulsing)
            {
                // Anticipation: blades spin
                targetRotationSpeed = 180f; // degrees per sec
            }
        }

        protected override void OnHoverExit()
        {
            if (!isPulsing)
            {
                // Blades slow to a halt
                targetRotationSpeed = 0f;
            }
        }

        protected override void OnPressed()
        {
            // Explosive Pop
            if (!isPulsing)
            {
                isPulsing = true;
                targetPulseScale = 1.4f;
                // Add an immediate kick to rotation
                currentRotationSpeed += 360f;
            }
        }

        protected override void OnReleased() { }

        protected override bool UpdateCustomPhysics(float dt)
        {
            bool hasChanged = false;

            if (isPulsing)
            {
                if (currentPulseScale >= 1.35f)
                {
                    isPulsing = false;
                    targetPulseScale = 1f; // Recover
                    targetRotationSpeed = isHovered ? 180f : 0f;
                }
            }

            float oldScale = currentPulseScale;
            float oldRot = currentRotation;

            // Spin physics
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, dt * 5f);
            currentRotation += currentRotationSpeed * dt;

            // Scale physics
            if (isPulsing)
            {
                currentPulseScale = Mathf.Lerp(currentPulseScale, targetPulseScale, dt * 35f); // Fast pop
            }
            else
            {
                currentPulseScale = SpringFloat(currentPulseScale, targetPulseScale, ref pulseVelocity, 30f, 0.6f, dt); // Bouncy recovery
            }

            if (Mathf.Abs(currentPulseScale - oldScale) > 0.005f ||
                Mathf.Abs(currentRotation - oldRot) > 0.1f)
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        protected override void DrawIconPath(Painter2D painter, float width, float height)
        {
            float cx = width / 2f;
            float cy = height / 2f;
            float baseRadius = Mathf.Min(width, height) * 0.35f * currentPulseScale;
            
            painter.strokeColor = currentColor;
            
            // 1. Center Eye / Pupil
            float centerRadius = baseRadius * 0.3f;
            painter.BeginPath();
            painter.Arc(new Vector2(cx, cy), centerRadius, 0f, 360f);
            painter.Stroke();

            // 2. Three Sweeping Arcs (Blender Logo style)
            int numBlades = 3;
            float bladeRadius = baseRadius * 0.8f;
            float bladeOffset = baseRadius * 0.4f; // Dist from center

            for (int i = 0; i < numBlades; i++)
            {
                float angleOffset = (360f / numBlades) * i;
                float angleDeg = currentRotation + angleOffset;
                float angleRad = angleDeg * Mathf.Deg2Rad;

                // Center of gravity for this particular blade
                float bx = cx + Mathf.Cos(angleRad) * bladeOffset;
                float by = cy + Mathf.Sin(angleRad) * bladeOffset;

                painter.BeginPath();
                // We draw an arc that wraps partly around the center
                // To keep it Blender-esque, the sweep should point tangentially
                float startAngle = angleDeg - 45f;
                float sweepAngle = 180f; // Half circle

                painter.Arc(new Vector2(bx, by), bladeRadius, startAngle * Mathf.Deg2Rad, sweepAngle * Mathf.Deg2Rad, ArcDirection.Clockwise);
                painter.Stroke();
            }
        }
    }
}
