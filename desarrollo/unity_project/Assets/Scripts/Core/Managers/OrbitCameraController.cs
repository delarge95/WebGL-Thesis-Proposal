using UnityEngine;
using WebGL.Core.Utils;
using System.Collections;

namespace WebGL.Core.Managers
{
    public class OrbitCameraController : Singleton<OrbitCameraController>
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = Vector3.zero;

        [Header("Orbit Settings")]
        [SerializeField] private float distance = 10f;
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float maxDistance = 25f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 5f; 
        [SerializeField] private float minVerticalAngle = -89f; // Allow looking from below
        [SerializeField] private float maxVerticalAngle = 89f;

        [Header("Panning Settings")]
        [SerializeField] private float panSpeed = 0.25f; // Slower for precision
        [SerializeField] private Vector2 panLimit = new Vector2(10f, 10f); // Limit X/Z panning

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float zoomDamping = 10f;

        [Header("Damping")]
        [SerializeField] private float dampingFactor = 5f; // Smoother transitions

        // View Shift for UI
        private float currentViewShiftRatio = 0f; // 0 to 1 (percentage of screen height)
        private float targetViewShiftRatio = 0f;

        private float currentDistance;
        private float targetDistance;
        
        private float currentX = 0f;
        private float currentY = 20f;
        private float targetX = 0f;
        private float targetY = 20f;

        private Vector3 currentFocusPoint;
        private Vector3 targetFocusPoint;

        // Reset Logic
        private Vector3 initialFocusPoint;
        private Vector3 initialTargetOffset;

        protected override void Awake()
        {
            base.Awake();
            currentDistance = distance;
            targetDistance = distance;
            
            // FORCE override inspector values that might be stale
            minVerticalAngle = -89f; 
            maxVerticalAngle = 89f;
        }

        private void Start()
        {
            if (target != null)
            {
                currentFocusPoint = target.position + targetOffset;
                targetFocusPoint = currentFocusPoint;
            }
            
            // Capture Initial State for Reset
            initialFocusPoint = targetFocusPoint;
            initialTargetOffset = targetOffset;
        }

        private void LateUpdate()
        {
            // Camera can operate without a physical target transform (using focus point).
            HandleInput();
            
            // Interpolate View Shift
            currentViewShiftRatio = Mathf.Lerp(currentViewShiftRatio, targetViewShiftRatio, Time.deltaTime * dampingFactor);
            
            UpdateCamera();
        }

        private void HandleInput()
        {
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
        }

        private void HandleMouseInput()
        {
            // 0. Smart Pivot: Re-center on Interaction Start
            if (Input.GetMouseButtonDown(1))
            {
                PickPivot(Input.mousePosition);
            }

            // 1. Orbit (Right Click)
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
                ApplyOrbit(mouseX, mouseY);
            }

            // 2. Pan (Middle Click)
            if (Input.GetMouseButton(2))
            {
                float mouseX = Input.GetAxis("Mouse X") * panSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * panSpeed;
                ApplyPan(mouseX, mouseY);
            }

            // 3. Zoom (Scroll)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                ApplyZoom(scroll * 2f);
            }
        }

        private void HandleTouchInput()
        {
            // 1 Finger: Orbit
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    float touchX = touch.deltaPosition.x * rotationSpeed * 0.2f; // Scale down for touch
                    float touchY = touch.deltaPosition.y * rotationSpeed * 0.2f;
                    ApplyOrbit(touchX, touchY);
                }
            }

            // 2 Fingers: Pan & Zoom
            if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                // Pan Logic (Movement of center point)
                Vector2 curCenter = (t0.position + t1.position) / 2f;
                Vector2 prevCenter = (t0.position - t0.deltaPosition + t1.position - t1.deltaPosition) / 2f;
                Vector2 panDelta = curCenter - prevCenter;

                // Zoom Logic (Pinch distance change)
                float curDist = Vector2.Distance(t0.position, t1.position);
                float prevDist = Vector2.Distance(t0.position - t0.deltaPosition, t1.position - t1.deltaPosition);
                float zoomDelta = (curDist - prevDist) * 0.01f; // Scale factor

                // Apply
                if (panDelta.magnitude > 0.1f)
                {
                     ApplyPan(panDelta.x * panSpeed * 0.5f, panDelta.y * panSpeed * 0.5f);
                }
                
                if (Mathf.Abs(zoomDelta) > 0.001f)
                {
                    ApplyZoom(zoomDelta * 5f);
                }
            }
        }

        // Shared Logic
        private void ApplyOrbit(float x, float y)
        {
            targetX += x;
            targetY -= y;
            targetY = Mathf.Clamp(targetY, minVerticalAngle, maxVerticalAngle);
        }

        private void ApplyPan(float x, float y)
        {
            Vector3 right = transform.right;
            // Use World Up to prevent "pulling back" sensation ("Elevator Panning")
            Vector3 up = Vector3.up; 
            
            // Flatten right vector to keep it horizontal
            right.y = 0f; 
            right.Normalize();

            Vector3 move = (-right * x) + (-up * y);

            if (target != null)
            {
                targetOffset += move;
                targetOffset.x = Mathf.Clamp(targetOffset.x, -panLimit.x, panLimit.x);
                targetOffset.y = Mathf.Clamp(targetOffset.y, -panLimit.y, panLimit.y); // Negative allowed
                targetOffset.z = Mathf.Clamp(targetOffset.z, -panLimit.y, panLimit.y);
            }
            else
            {
                targetFocusPoint += move;
                targetFocusPoint.x = Mathf.Clamp(targetFocusPoint.x, -panLimit.x, panLimit.x);
                targetFocusPoint.y = Mathf.Clamp(targetFocusPoint.y, -panLimit.y, panLimit.y); // Negative allowed
                targetFocusPoint.z = Mathf.Clamp(targetFocusPoint.z, -panLimit.y, panLimit.y);
            }
        }

        private void ApplyZoom(float amount)
        {
             targetDistance -= amount * zoomSpeed; 
             targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        private void PickPivot(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) // Max distance 100
            {
                Vector3 newFocus = hit.point;
                float newDist = Vector3.Distance(transform.position, newFocus);
                
                // Clamp distance to avoid being too close
                if (newDist >= minDistance)
                {
                    if (target != null)
                    {
                        targetOffset = newFocus - target.position;
                    }
                    else
                    {
                        targetFocusPoint = newFocus;
                    }

                    // Smooth Transition
                    targetFocusPoint = newFocus;
                    targetDistance = newDist;
                }
            }
        }

        private void UpdateCamera()
        {
            float dt = Time.deltaTime;

            // Smooth Orbit Angles
            currentX = Mathf.Lerp(currentX, targetX, dt * dampingFactor);
            currentY = Mathf.Lerp(currentY, targetY, dt * dampingFactor);

            // Smooth Distance
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, dt * zoomDamping);

            // Smooth Focus Point (Panning/Target Switch)
            if (target != null)
                targetFocusPoint = target.position + targetOffset;
            
            currentFocusPoint = Vector3.Lerp(currentFocusPoint, targetFocusPoint, dt * dampingFactor);

            // Calculate Position & Rotation
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            
            // Base Position
            Vector3 direction = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = rotation * direction + currentFocusPoint;

            // --- VIEW SHIFT LOGIC ---
            // Calculate world-space vertical offset based on FOV and Distance
            if (Mathf.Abs(currentViewShiftRatio) > 0.001f)
            {
                Camera cam = GetComponent<Camera>();
                if (cam != null)
                {
                    // Frustum height at target distance
                    float frustumHeight = 2.0f * currentDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float worldOffset = frustumHeight * currentViewShiftRatio;

                    // Shift position DOWN to make object appear UP
                    // We effectively look at a point below the actual target
                    Vector3 shiftVector = -Vector3.up * worldOffset; 
                    
                    position += shiftVector;
                    
                    // We must also shift the LookAt target so rotation pivot remains correct relative to the screen
                    transform.position = position;
                    transform.LookAt(currentFocusPoint + shiftVector);
                    return;
                }
            }

            transform.rotation = rotation;
            transform.position = position;
        }

        public void SetTarget(Transform newTarget, bool snap = false)
        {
            target = newTarget;
            targetOffset = Vector3.zero;
            
            if (target != null)
            {
                targetFocusPoint = target.position + targetOffset;
                if (snap)
                {
                    currentFocusPoint = targetFocusPoint;
                    targetDistance = 5f; // Reset zoom on focus
                }
            }
        }

        public void FocusOnObject(Transform objTransform)
        {
            if (objTransform == null) return;
            
            Vector3 centerOffset = Vector3.zero;
            var rend = objTransform.GetComponent<Renderer>();
            if (rend != null)
            {
                centerOffset = rend.bounds.center - objTransform.position;
            }

            // Keep current rotation angles
            targetX = currentX; 
            targetY = 20f;      
            
            SetTarget(objTransform, false);
            targetOffset = centerOffset; 
            
            targetDistance = 6f; // Standard zoom
        }

        public void SetViewportShift(float shiftRatio)
        {
            targetViewShiftRatio = shiftRatio;
        }

        public void ResetView()
        {
            // Reset Orbit & Zoom
            targetX = 0f;
            targetY = 20f;
            targetDistance = 10f; // Could also capture initial distance if desired

            // Reset Pan / Focus
            targetFocusPoint = initialFocusPoint;
            targetOffset = initialTargetOffset;
            
            // Optional: Reset View Shift
            targetViewShiftRatio = 0f;
        }

        public void SetAngles(float horizontal, float vertical, bool immediate = false)
        {
            targetX = horizontal;
            targetY = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);

            if (immediate)
            {
                currentX = targetX;
                currentY = targetY;
            }
        }

        public void SetDistance(float newDistance, bool immediate = false)
        {
            targetDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
            if (immediate)
            {
                currentDistance = targetDistance;
            }
        }
    }
}
