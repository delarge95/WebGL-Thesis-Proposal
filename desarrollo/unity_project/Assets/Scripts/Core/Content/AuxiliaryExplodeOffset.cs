using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public class AuxiliaryExplodeOffset : MonoBehaviour
    {
        [SerializeField] private Vector3 baseLocalPosition;
        [SerializeField] private Vector3 offsetDirection = Vector3.up;
        [SerializeField] private float offsetDistance = 0.04f;
        [SerializeField] private float leadFactor = 1.1f;
        [SerializeField] private bool initialized;

        public void Configure(Vector3 direction, float distance, float lead)
        {
            baseLocalPosition = transform.localPosition;
            offsetDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.up;
            offsetDistance = Mathf.Max(0f, distance);
            leadFactor = Mathf.Max(0.01f, lead);
            initialized = true;
        }

        public void Initialize()
        {
            baseLocalPosition = transform.localPosition;
            initialized = true;
        }

        public void Apply(float parentFactor)
        {
            if (!initialized)
            {
                Initialize();
            }

            float localFactor = Mathf.Clamp01(parentFactor * leadFactor);
            transform.localPosition = baseLocalPosition + offsetDirection * offsetDistance * localFactor;
        }
    }
}
