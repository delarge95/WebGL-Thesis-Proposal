using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class OrbitCameraController : Singleton<OrbitCameraController>
    {
        // ── Constants ────────────────────────────────────────────
        private const float DEFAULT_VERTICAL_ANGLE  = 20f;
        private const float TOUCH_ORBIT_SCALE       = 0.12f;
        private const float TOUCH_DEAD_ZONE         = 2f;   // px — ignore micro-jitter on orbit
        private const float TOUCH_TWO_FINGER_DEAD   = 4f;   // px — larger dead zone for 2-finger gestures
        private const float MOUSE_SCROLL_SCALE      = 2f;
        private const float RAYCAST_MAX_DISTANCE    = 100f;
        private const float FOCUS_SNAP_DISTANCE     = 5f;
        private const float FOCUS_ORBIT_DISTANCE    = 6f;

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
        [SerializeField] private float panSpeed = 0.25f; // Slower for precision (mouse only)
        [SerializeField] private Vector2 panLimit = new Vector2(10f, 10f); // Limit X/Z panning

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float zoomDamping = 10f;
        [SerializeField] private float touchZoomSpeed = 0.008f; // Pinch zoom — much gentler than mouse

        [Header("Damping")]
        [SerializeField] private float dampingFactor = 5f; // Smoother transitions
        [SerializeField] private float touchDampingFactor = 8f; // Extra smoothing for touch

        // View Shift for UI
        private float currentViewShiftRatio = 0f; // 0 to 1 (percentage of screen height)
        private float targetViewShiftRatio = 0f;

        private float currentDistance;
        private float targetDistance;
        
        private float currentX = 0f;
        private float currentY = DEFAULT_VERTICAL_ANGLE;
        private float targetX = 0f;
        private float targetY = DEFAULT_VERTICAL_ANGLE;

        private Vector3 currentFocusPoint;
        private Vector3 targetFocusPoint;

        private bool _isTouchInput;

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
            // Render settings are handled by EnvironmentController.Start() (Phase 5 cleanup)
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
            // Phase 4: Centralized input blocking via InputManager
            if (InputManager.InputBlocked) return;

            // Phase 4: Skip camera input when pointer is over UI Toolkit elements
            if (InputManager.Instance != null && InputManager.Instance.IsPointerOverUI()) return;

            if (Input.touchCount > 0)
            {
                _isTouchInput = true;
                HandleTouchInput();
            }
            else
            {
                _isTouchInput = false;
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
                ApplyZoom(scroll * MOUSE_SCROLL_SCALE);
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
                    if (touch.deltaPosition.magnitude < TOUCH_DEAD_ZONE) return;
                    float touchX = touch.deltaPosition.x * rotationSpeed * TOUCH_ORBIT_SCALE;
                    float touchY = touch.deltaPosition.y * rotationSpeed * TOUCH_ORBIT_SCALE;
                    ApplyOrbit(touchX, touchY);
                }
            }

            // 2 Fingers: Pan & Zoom — "Paper Drag" projection
            if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                // ── Pan: Project screen-pixel delta into world units ──
                // This gives a 1:1 "drag the paper" feel at any zoom level.
                Vector2 curCenter  = (t0.position + t1.position) * 0.5f;
                Vector2 prevCenter = ((t0.position - t0.deltaPosition) + (t1.position - t1.deltaPosition)) * 0.5f;
                Vector2 panDelta   = curCenter - prevCenter;

                if (panDelta.magnitude > TOUCH_TWO_FINGER_DEAD)
                {
                    Camera cam = GetComponent<Camera>();
                    if (cam != null)
                    {
                        // Frustum dimensions at the current orbit distance
                        float frustumHeight = 2f * currentDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                        float frustumWidth  = frustumHeight * cam.aspect;

                        // Convert pixel delta → world delta (proportional)
                        float worldX = (panDelta.x / Screen.width)  * frustumWidth;
                        float worldY = (panDelta.y / Screen.height) * frustumHeight;

                        ApplyPan(worldX, worldY);
                    }
                }

                // ── Zoom: Pinch distance change ──
                float curDist  = Vector2.Distance(t0.position, t1.position);
                float prevDist = Vector2.Distance(
                    t0.position - t0.deltaPosition,
                    t1.position - t1.deltaPosition);
                float pinchDelta = curDist - prevDist;

                if (Mathf.Abs(pinchDelta) > TOUCH_TWO_FINGER_DEAD)
                {
                    // Normalize by screen height so the gesture feels the same
                    // on any device resolution
                    float normalizedPinch = pinchDelta / Screen.height;
                    ApplyZoom(normalizedPinch * zoomSpeed * (currentDistance * touchZoomSpeed / 0.008f));
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
            // Pan in camera-local space so the drag direction stays aligned with the current view.
            Vector3 right = transform.right.normalized;
            Vector3 up = transform.up.normalized;

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
            
            if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_MAX_DISTANCE))
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
            float damp = _isTouchInput ? touchDampingFactor : dampingFactor;

            // Smooth Orbit Angles
            currentX = Mathf.Lerp(currentX, targetX, dt * damp);
            currentY = Mathf.Lerp(currentY, targetY, dt * damp);

            // Smooth Distance
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, dt * zoomDamping);

            // Smooth Focus Point (Panning/Target Switch)
            if (target != null)
                targetFocusPoint = target.position + targetOffset;
            
            currentFocusPoint = Vector3.Lerp(currentFocusPoint, targetFocusPoint, dt * damp);

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
                    targetDistance = FOCUS_SNAP_DISTANCE;
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
            targetY = DEFAULT_VERTICAL_ANGLE;
            
            SetTarget(objTransform, false);
            targetOffset = centerOffset; 
            
            targetDistance = FOCUS_ORBIT_DISTANCE;
        }

        public void SetViewportShift(float shiftRatio)
        {
            targetViewShiftRatio = shiftRatio;
        }

        public void ResetView()
        {
            // Reset Orbit & Zoom
            targetX = 0f;
            targetY = DEFAULT_VERTICAL_ANGLE;
            targetDistance = distance;

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
