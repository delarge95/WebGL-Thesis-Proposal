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

        private void Start()
        {
            initialPosition = transform.localPosition;
            
            // If data is assigned, calculate target position based on direction and distance
            if (partData != null)
            {
                targetPosition = initialPosition + (partData.explosionDirection.normalized * partData.explosionDistance);
            }
            else
            {
                // Fallback if no data: explode outwards from center
                targetPosition = initialPosition + (initialPosition.normalized * 0.5f);
            }
        }

        public void UpdateExplosion(float factor)
        {
            // Linear interpolation between initial and target position
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, factor);
        }
    }
}
