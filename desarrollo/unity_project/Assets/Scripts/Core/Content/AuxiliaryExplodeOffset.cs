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
            transform.localPosition = baseLocalPosition + resolvedOffset * localFactor;
        }
    }
}
