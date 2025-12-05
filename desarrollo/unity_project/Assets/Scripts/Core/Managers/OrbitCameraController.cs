using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class OrbitCameraController : Singleton<OrbitCameraController>
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = Vector3.zero;

        [Header("Orbit Settings")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 15f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float minVerticalAngle = -80f;
        [SerializeField] private float maxVerticalAngle = 80f;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float zoomSmoothness = 10f;

        [Header("Damping")]
        [SerializeField] private float rotationDamping = 5f;
        [SerializeField] private float movementDamping = 8f;

        [Header("Auto Rotation")]
        [SerializeField] private bool autoRotate = false;
        [SerializeField] private float autoRotateSpeed = 10f;
        [SerializeField] private float autoRotateDelay = 5f;

        private float currentHorizontalAngle = 0f;
        private float currentVerticalAngle = 30f;
        private float targetHorizontalAngle = 0f;
        private float targetVerticalAngle = 30f;
        private float targetDistance;
        private float lastInputTime;
        private Vector3 currentTargetPosition;

        protected override void Awake()
        {
            base.Awake();
            targetDistance = distance;
            lastInputTime = Time.time;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleInput();
            HandleAutoRotation();
            UpdateCamera();
        }

        private void HandleInput()
        {
            // Use InputManager if available
            Vector2 lookInput = Vector2.zero;
            float zoomInput = 0f;

            if (InputManager.Instance != null)
            {
                lookInput = InputManager.Instance.LookInput;
                zoomInput = InputManager.Instance.ZoomInput;
            }
            else
            {
                // Fallback to direct input
                if (Input.GetMouseButton(1))
                {
                    lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }
                zoomInput = Input.GetAxis("Mouse ScrollWheel");
            }

            // Apply rotation
            if (lookInput.sqrMagnitude > 0.001f)
            {
                targetHorizontalAngle += lookInput.x * rotationSpeed;
                targetVerticalAngle -= lookInput.y * rotationSpeed;
                targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
                lastInputTime = Time.time;
            }

            // Apply zoom
            if (Mathf.Abs(zoomInput) > 0.01f)
            {
                targetDistance -= zoomInput * zoomSpeed;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
                lastInputTime = Time.time;
            }
        }

        private void HandleAutoRotation()
        {
            if (!autoRotate) return;

            if (Time.time - lastInputTime > autoRotateDelay)
            {
                targetHorizontalAngle += autoRotateSpeed * Time.deltaTime;
            }
        }

        private void UpdateCamera()
        {
            // Smooth angles
            currentHorizontalAngle = Mathf.LerpAngle(currentHorizontalAngle, targetHorizontalAngle, rotationDamping * Time.deltaTime);
            currentVerticalAngle = Mathf.Lerp(currentVerticalAngle, targetVerticalAngle, rotationDamping * Time.deltaTime);
            distance = Mathf.Lerp(distance, targetDistance, zoomSmoothness * Time.deltaTime);

            // Smooth target position
            Vector3 targetPos = target.position + targetOffset;
            currentTargetPosition = Vector3.Lerp(currentTargetPosition, targetPos, movementDamping * Time.deltaTime);

            // Calculate camera position
            Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
            Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
            Vector3 desiredPosition = currentTargetPosition + offset;

            transform.position = desiredPosition;
            transform.LookAt(currentTargetPosition);
        }

        public void SetTarget(Transform newTarget, Vector3 offset = default)
        {
            target = newTarget;
            targetOffset = offset;
            if (target != null)
            {
                currentTargetPosition = target.position + targetOffset;
            }
        }

        public void SetAngles(float horizontal, float vertical, bool immediate = false)
        {
            targetHorizontalAngle = horizontal;
            targetVerticalAngle = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);

            if (immediate)
            {
                currentHorizontalAngle = targetHorizontalAngle;
                currentVerticalAngle = targetVerticalAngle;
            }
        }

        public void SetDistance(float newDistance, bool immediate = false)
        {
            targetDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
            if (immediate)
            {
                distance = targetDistance;
            }
        }

        public void ResetView()
        {
            SetAngles(0f, 30f, false);
            SetDistance(5f, false);
        }
    }
}
