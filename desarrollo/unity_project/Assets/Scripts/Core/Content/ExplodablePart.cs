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

        public DronePartData Data => partData;

        public void SetData(DronePartData newData)
        {
            partData = newData;
        }

        public void Initialize()
        {
            initialPosition = transform.localPosition;
            CalculateTargetPosition();
        }

        private void Start()
        {
            // If already initialized via script, this might be redundant but safe
            if (targetPosition == Vector3.zero && transform.localPosition != Vector3.zero) 
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
            // Linear interpolation between initial and target position
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, factor);
        }
    }
}
