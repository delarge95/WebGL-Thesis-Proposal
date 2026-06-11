using UnityEngine;
using WebGL.Core.Data;

namespace WebGL.Core.Content
{
    public class ExplodablePart : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private DronePartData partData;

        private Vector3 initialPosition;
        private Vector3 targetPosition;
        private Vector3 compositePrimaryOffset;
        private Vector3 compositeSecondaryOffset;
        private Vector3 compositeTertiaryOffset;
        private float compositePrimaryStart;
        private float compositePrimaryEnd = 1f;
        private float compositeSecondaryStart;
        private float compositeSecondaryEnd = 1f;
        private float compositeTertiaryStart;
        private float compositeTertiaryEnd = 1f;
        private bool useSequencedCompositePrimary;
        private bool useCompositeTertiary;
        private bool useCompositeExplosion;
        private bool primarySequenceUsesLocalFactor;
        private bool tertiarySequenceUsesLocalFactor;
        private bool useCompositeTertiaryEaseOut;
        private bool initialized;

        public DronePartData Data => partData;
        public Vector3 InitialLocalPosition => initialPosition;

        public void SetData(DronePartData newData)
        {
            partData = newData;
        }

        public void Initialize()
        {
            initialPosition = transform.localPosition;
            CalculateTargetPosition();
            initialized = true;

            foreach (AuxiliaryExplodeOffset offset in GetComponentsInChildren<AuxiliaryExplodeOffset>(true))
            {
                if (offset != null)
                {
                    offset.Initialize();
                }
            }
        }

        private void Start()
        {
            // Keep initialization idempotent and independent from localPosition checks.
            if (!initialized)
            {
                Initialize();
            }
        }

        private void CalculateTargetPosition()
        {
            useCompositeExplosion = false;
            // If data is assigned, calculate target position based on direction and distance
            if (partData != null)
            {
                targetPosition = initialPosition + (partData.explosionDirection.normalized * partData.explosionDistance);
            }
            else
            {
                // Fallback if no data: explode outwards from center (or Up if at zero)
                Vector3 dir = initialPosition == Vector3.zero ? Vector3.up : initialPosition.normalized;
                targetPosition = initialPosition + (dir * 0.5f);
            }
        }

        public void ConfigureRuntimeExplosionTarget(Vector3 worldDirection, float distance)
        {
            if (!initialized)
            {
                Initialize();
            }

            Vector3 direction = worldDirection.sqrMagnitude > 0.0001f ? worldDirection.normalized : Vector3.up;
            float resolvedDistance = Mathf.Max(distance, 0f);
            Vector3 initialWorld = transform.parent != null
                ? transform.parent.TransformPoint(initialPosition)
                : initialPosition;
            Vector3 targetWorld = initialWorld + direction * resolvedDistance;
            targetPosition = transform.parent != null
                ? transform.parent.InverseTransformPoint(targetWorld)
                : targetWorld;
            useCompositeExplosion = false;
        }

        public void ConfigureRuntimeCompositeExplosionTarget(
            Vector3 primaryWorldOffset,
            Vector3 secondaryWorldOffset,
            float secondaryStart,
            float secondaryEnd)
        {
            if (!initialized)
            {
                Initialize();
            }

            compositePrimaryOffset = transform.parent != null
                ? transform.parent.InverseTransformVector(primaryWorldOffset)
                : primaryWorldOffset;
            compositeSecondaryOffset = transform.parent != null
                ? transform.parent.InverseTransformVector(secondaryWorldOffset)
                : secondaryWorldOffset;
            useSequencedCompositePrimary = false;
            compositeSecondaryStart = Mathf.Clamp01(Mathf.Min(secondaryStart, secondaryEnd));
            compositeSecondaryEnd = Mathf.Clamp01(Mathf.Max(secondaryStart, secondaryEnd));
            if (Mathf.Abs(compositeSecondaryEnd - compositeSecondaryStart) < 0.001f)
            {
                compositeSecondaryEnd = Mathf.Min(1f, compositeSecondaryStart + 0.001f);
            }

            useCompositeTertiary = false;
            useCompositeExplosion = true;
            primarySequenceUsesLocalFactor = false;
            tertiarySequenceUsesLocalFactor = false;
            useCompositeTertiaryEaseOut = false;
        }

        public void ConfigureRuntimeCompositeExplosionTarget(
            Vector3 primaryWorldOffset,
            Vector3 secondaryWorldOffset,
            float secondaryStart,
            float secondaryEnd,
            Vector3 tertiaryWorldOffset,
            float tertiaryStart,
            float tertiaryEnd)
        {
            ConfigureRuntimeCompositeExplosionTarget(
                primaryWorldOffset,
                secondaryWorldOffset,
                secondaryStart,
                secondaryEnd);

            compositeTertiaryOffset = transform.parent != null
                ? transform.parent.InverseTransformVector(tertiaryWorldOffset)
                : tertiaryWorldOffset;
            compositeTertiaryStart = Mathf.Clamp01(Mathf.Min(tertiaryStart, tertiaryEnd));
            compositeTertiaryEnd = Mathf.Clamp01(Mathf.Max(tertiaryStart, tertiaryEnd));
            if (Mathf.Abs(compositeTertiaryEnd - compositeTertiaryStart) < 0.001f)
            {
                compositeTertiaryEnd = Mathf.Min(1f, compositeTertiaryStart + 0.001f);
            }

            useCompositeTertiary = compositeTertiaryOffset.sqrMagnitude > 0.0001f;
            tertiarySequenceUsesLocalFactor = false;
            useCompositeTertiaryEaseOut = false;
        }

        public void ConfigureRuntimeCompositeExplosionTarget(
            Vector3 primaryWorldOffset,
            float primaryStart,
            float primaryEnd,
            Vector3 secondaryWorldOffset,
            float secondaryStart,
            float secondaryEnd)
        {
            ConfigureRuntimeCompositeExplosionTarget(primaryWorldOffset, secondaryWorldOffset, secondaryStart, secondaryEnd);
            compositePrimaryStart = Mathf.Clamp01(Mathf.Min(primaryStart, primaryEnd));
            compositePrimaryEnd = Mathf.Clamp01(Mathf.Max(primaryStart, primaryEnd));
            if (Mathf.Abs(compositePrimaryEnd - compositePrimaryStart) < 0.001f)
            {
                compositePrimaryEnd = Mathf.Min(1f, compositePrimaryStart + 0.001f);
            }
            useSequencedCompositePrimary = true;
        }

        public void ConfigureRuntimeCompositeExplosionTarget(
            Vector3 primaryWorldOffset,
            float primaryStart,
            float primaryEnd,
            Vector3 secondaryWorldOffset,
            float secondaryStart,
            float secondaryEnd,
            Vector3 tertiaryWorldOffset,
            float tertiaryStart,
            float tertiaryEnd)
        {
            ConfigureRuntimeCompositeExplosionTarget(
                primaryWorldOffset,
                primaryStart,
                primaryEnd,
                secondaryWorldOffset,
                secondaryStart,
                secondaryEnd);

            compositeTertiaryOffset = transform.parent != null
                ? transform.parent.InverseTransformVector(tertiaryWorldOffset)
                : tertiaryWorldOffset;
            compositeTertiaryStart = Mathf.Clamp01(Mathf.Min(tertiaryStart, tertiaryEnd));
            compositeTertiaryEnd = Mathf.Clamp01(Mathf.Max(tertiaryStart, tertiaryEnd));
            if (Mathf.Abs(compositeTertiaryEnd - compositeTertiaryStart) < 0.001f)
            {
                compositeTertiaryEnd = Mathf.Min(1f, compositeTertiaryStart + 0.001f);
            }

            useCompositeTertiary = compositeTertiaryOffset.sqrMagnitude > 0.0001f;
            tertiarySequenceUsesLocalFactor = false;
            useCompositeTertiaryEaseOut = false;
        }

        public void ConfigureCompositeTimelineSource(bool primaryUsesLocalFactor, bool tertiaryUsesLocalFactor)
        {
            primarySequenceUsesLocalFactor = primaryUsesLocalFactor;
            tertiarySequenceUsesLocalFactor = tertiaryUsesLocalFactor;
        }

        public void ConfigureCompositeTertiaryEaseOut(bool enabled)
        {
            useCompositeTertiaryEaseOut = enabled;
        }

        public void UpdateExplosion(float factor)
        {
            UpdateExplosion(factor, factor);
        }

        public void UpdateExplosion(float factor, float globalFactor)
        {
            // ExplodedViewManager can call UpdateExplosion before Start; capture base pose first.
            if (!initialized)
            {
                Initialize();
            }

            if (useCompositeExplosion)
            {
                float primarySourceFactor = primarySequenceUsesLocalFactor ? factor : globalFactor;
                float primaryFactor = useSequencedCompositePrimary
                    ? Mathf.InverseLerp(compositePrimaryStart, compositePrimaryEnd, Mathf.Clamp01(primarySourceFactor))
                    : Mathf.Clamp01(factor);
                if (useSequencedCompositePrimary)
                {
                    primaryFactor = SmoothStep01(primaryFactor);
                }
                float secondaryFactor = Mathf.InverseLerp(
                    compositeSecondaryStart,
                    compositeSecondaryEnd,
                    Mathf.Clamp01(globalFactor));
                secondaryFactor = SmoothStep01(secondaryFactor);
                Vector3 resolvedPosition =
                    initialPosition +
                    compositePrimaryOffset * primaryFactor +
                    compositeSecondaryOffset * secondaryFactor;
                if (useCompositeTertiary)
                {
                    float tertiarySourceFactor = tertiarySequenceUsesLocalFactor ? factor : globalFactor;
                    float tertiaryFactor = Mathf.InverseLerp(
                        compositeTertiaryStart,
                        compositeTertiaryEnd,
                        Mathf.Clamp01(tertiarySourceFactor));
                    tertiaryFactor = useCompositeTertiaryEaseOut
                        ? EaseOutCubic(tertiaryFactor)
                        : SmoothStep01(tertiaryFactor);
                    resolvedPosition += compositeTertiaryOffset * tertiaryFactor;
                }

                transform.localPosition = resolvedPosition;
            }
            else
            {
                // Linear interpolation between initial and target position
                transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, factor);
            }

            foreach (AuxiliaryExplodeOffset offset in GetComponentsInChildren<AuxiliaryExplodeOffset>(true))
            {
                // Auxiliary offsets belong to child renderers only. If one is attached to
                // this same transform it would overwrite the composite ExplodablePart pose.
                if (offset != null && offset.transform != transform)
                {
                    offset.Apply(factor, globalFactor);
                }
            }
        }

        private static float SmoothStep01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }

        private static float EaseOutCubic(float value)
        {
            value = Mathf.Clamp01(value);
            float inverse = 1f - value;
            return 1f - inverse * inverse * inverse;
        }

        public void SetXRay(bool enable, Material xrayMat)
        {
            MaterialController controller = GetComponent<MaterialController>();
            if (controller != null)
            {
                controller.ToggleXRay(enable, xrayMat);
            }
        }
    }
}
