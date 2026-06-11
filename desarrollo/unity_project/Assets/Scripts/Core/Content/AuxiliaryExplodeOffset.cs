using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public class AuxiliaryExplodeOffset : MonoBehaviour
    {
        [SerializeField] private Vector3 baseLocalPosition;
        [SerializeField] private Vector3 offsetDirection = Vector3.up;
        [SerializeField] private float offsetDistance = 0.04f;
        [SerializeField] private Vector3 radialCorrectionLocal = Vector3.zero;
        [SerializeField] private float leadFactor = 1.1f;
        [SerializeField] private bool useGlobalFactor;
        [SerializeField] private float sequenceStart;
        [SerializeField] private float sequenceEnd = 1f;
        [SerializeField] private bool useSecondaryOffset;
        [SerializeField] private Vector3 secondaryOffsetDirection = Vector3.zero;
        [SerializeField] private float secondaryOffsetDistance;
        [SerializeField] private float secondarySequenceStart;
        [SerializeField] private float secondarySequenceEnd = 1f;
        [SerializeField] private bool initialized;

        public void Configure(Vector3 direction, float distance, float lead)
        {
            baseLocalPosition = transform.localPosition;
            offsetDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.up;
            offsetDistance = Mathf.Max(0f, distance);
            radialCorrectionLocal = Vector3.zero;
            leadFactor = Mathf.Max(0.01f, lead);
            useGlobalFactor = false;
            sequenceStart = 0f;
            sequenceEnd = 1f;
            useSecondaryOffset = false;
            initialized = true;
        }

        public void ConfigureSequenced(Vector3 direction, float distance, float start, float end, bool globalTiming)
        {
            baseLocalPosition = transform.localPosition;
            offsetDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.up;
            offsetDistance = Mathf.Max(0f, distance);
            radialCorrectionLocal = Vector3.zero;
            leadFactor = 1f;
            useGlobalFactor = globalTiming;
            sequenceStart = Mathf.Clamp01(Mathf.Min(start, end));
            sequenceEnd = Mathf.Clamp01(Mathf.Max(start, end));
            if (Mathf.Abs(sequenceEnd - sequenceStart) < 0.001f)
            {
                sequenceEnd = Mathf.Min(1f, sequenceStart + 0.001f);
            }
            useSecondaryOffset = false;
            initialized = true;
        }

        public void ConfigureSequencedComposite(
            Vector3 primaryDirection,
            float primaryDistance,
            float primaryStart,
            float primaryEnd,
            Vector3 secondaryDirection,
            float secondaryDistance,
            float secondaryStart,
            float secondaryEnd,
            bool globalTiming)
        {
            ConfigureSequenced(primaryDirection, primaryDistance, primaryStart, primaryEnd, globalTiming);
            secondaryOffsetDirection = secondaryDirection.sqrMagnitude > 0.0001f ? secondaryDirection.normalized : Vector3.zero;
            secondaryOffsetDistance = Mathf.Max(0f, secondaryDistance);
            secondarySequenceStart = Mathf.Clamp01(Mathf.Min(secondaryStart, secondaryEnd));
            secondarySequenceEnd = Mathf.Clamp01(Mathf.Max(secondaryStart, secondaryEnd));
            if (Mathf.Abs(secondarySequenceEnd - secondarySequenceStart) < 0.001f)
            {
                secondarySequenceEnd = Mathf.Min(1f, secondarySequenceStart + 0.001f);
            }
            useSecondaryOffset = secondaryOffsetDirection.sqrMagnitude > 0.0001f && secondaryOffsetDistance > 0f;
            initialized = true;
        }

        public void ConfigureRadialCorrection(Vector3 localOffset)
        {
            radialCorrectionLocal = localOffset;
        }

        public void Initialize()
        {
            baseLocalPosition = transform.localPosition;
            initialized = true;
        }

        public void Apply(float parentFactor)
        {
            Apply(parentFactor, parentFactor);
        }

        public void Apply(float parentFactor, float globalFactor)
        {
            if (!initialized)
            {
                Initialize();
            }

            float sourceFactor = useGlobalFactor ? globalFactor : parentFactor;
            float localFactor = useGlobalFactor
                ? Mathf.InverseLerp(sequenceStart, sequenceEnd, Mathf.Clamp01(sourceFactor))
                : Mathf.Clamp01(sourceFactor * leadFactor);
            localFactor = localFactor * localFactor * (3f - 2f * localFactor);
            Vector3 resolvedOffset = offsetDirection * offsetDistance + radialCorrectionLocal;

            Vector3 finalOffset = resolvedOffset * localFactor;
            if (useSecondaryOffset)
            {
                float secondarySourceFactor = useGlobalFactor ? globalFactor : parentFactor;
                float secondaryFactor = Mathf.InverseLerp(
                    secondarySequenceStart,
                    secondarySequenceEnd,
                    Mathf.Clamp01(secondarySourceFactor));
                secondaryFactor = secondaryFactor * secondaryFactor * (3f - 2f * secondaryFactor);
                finalOffset += secondaryOffsetDirection * secondaryOffsetDistance * secondaryFactor;
            }

            transform.localPosition = baseLocalPosition + finalOffset;
        }
    }
}
