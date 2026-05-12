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
        private float compositeSecondaryStart;
        private float compositeSecondaryEnd = 1f;
        private bool useCompositeExplosion;
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
            compositeSecondaryStart = Mathf.Clamp01(Mathf.Min(secondaryStart, secondaryEnd));
            compositeSecondaryEnd = Mathf.Clamp01(Mathf.Max(secondaryStart, secondaryEnd));
            if (Mathf.Abs(compositeSecondaryEnd - compositeSecondaryStart) < 0.001f)
            {
                compositeSecondaryEnd = Mathf.Min(1f, compositeSecondaryStart + 0.001f);
            }

            useCompositeExplosion = true;
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
                float secondaryFactor = Mathf.InverseLerp(
                    compositeSecondaryStart,
                    compositeSecondaryEnd,
                    Mathf.Clamp01(globalFactor));
                secondaryFactor = secondaryFactor * secondaryFactor * (3f - 2f * secondaryFactor);
                transform.localPosition =
                    initialPosition +
                    compositePrimaryOffset * Mathf.Clamp01(factor) +
                    compositeSecondaryOffset * secondaryFactor;
            }
            else
            {
                // Linear interpolation between initial and target position
                transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, factor);
            }

            foreach (AuxiliaryExplodeOffset offset in GetComponentsInChildren<AuxiliaryExplodeOffset>(true))
            {
                if (offset != null)
                {
                    offset.Apply(factor, globalFactor);
                }
            }
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
