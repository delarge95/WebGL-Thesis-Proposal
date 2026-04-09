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
        private bool initialized;

        public DronePartData Data => partData;

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

        public void UpdateExplosion(float factor)
        {
            // ExplodedViewManager can call UpdateExplosion before Start; capture base pose first.
            if (!initialized)
            {
                Initialize();
            }

            // Linear interpolation between initial and target position
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, factor);

            foreach (AuxiliaryExplodeOffset offset in GetComponentsInChildren<AuxiliaryExplodeOffset>(true))
            {
                if (offset != null)
                {
                    offset.Apply(factor);
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
