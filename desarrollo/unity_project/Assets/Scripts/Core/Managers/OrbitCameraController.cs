using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class OrbitCameraController : Singleton<OrbitCameraController>
    {
        // ── Constants ────────────────────────────────────────────
        private const float DEFAULT_VERTICAL_ANGLE  = 20f;
        private const float TOUCH_ORBIT_SCALE       = 0.07125f;
        private const float TOUCH_PINCH_RESPONSE_SCALE = 0.11f;
        private const float TOUCH_DEAD_ZONE         = 2f;   // px — ignore micro-jitter on orbit
        private const float TOUCH_TWO_FINGER_DEAD   = 4f;   // px — larger dead zone for 2-finger gestures
        private const float TOUCH_GESTURE_DOMINANCE = 1.2f;
        private const float MOUSE_SCROLL_SCALE      = 2f;
        private const float RAYCAST_MAX_DISTANCE    = 100f;
        private const float FOCUS_SNAP_DISTANCE     = 5f;
        private const float FOCUS_FALLBACK_DISTANCE = 6f;
        private const float MIN_DISTANCE_FLOOR      = 0.05f;
        private const float FASTENER_INSPECTION_MIN_DISTANCE = 0.015f;
        private const float FOCUS_DISTANCE_PADDING  = 1.35f;
        private const float FOCUS_DISTANCE_MIN      = 0.08f;
        private const float DEFAULT_MODEL_MIN_DISTANCE = 2f;
        private const float MIN_ADAPTIVE_ORBIT_FACTOR = 0.3f;
        private const float MIN_ADAPTIVE_PAN_FACTOR = 0.0035f;
        private const float ADAPTIVE_ORBIT_EXPONENT = 0.6f;
        private const float ADAPTIVE_PAN_EXPONENT = 1.7f;
        private const float ADAPTIVE_MAX_DISTANCE_FLOOR_MULTIPLIER = 5f;
        private const float DEFAULT_VIEW_DISTANCE_PADDING = 1.85f;
        private const float DEFAULT_VIEW_DOMINANT_MULTIPLIER = 2.8f;
        private const float DEFAULT_CONTEXT_MIN_DISTANCE_RATIO = 0.18f;
        private const float DEFAULT_CONTEXT_MAX_DISTANCE_RATIO = 4.5f;
        private const string RuntimeDroneRootName = "x500v2_Drone";

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
        private float defaultMinDistance;
        private float defaultMaxDistance;
        private float currentMinDistance;
        private float currentMaxDistance;
        private Transform defaultNavigationContext;
        private Transform retainedNavigationContext;
        
        private float currentX = 0f;
        private float currentY = DEFAULT_VERTICAL_ANGLE;
        private float targetX = 0f;
        private float targetY = DEFAULT_VERTICAL_ANGLE;

        private Vector3 currentFocusPoint;
        private Vector3 targetFocusPoint;

        private bool _isTouchInput;
        private bool _orbitGestureBlockedByUI;
        private bool _panGestureBlockedByUI;
        private bool _touchGestureBlockedByUI;
        private bool _touchPivotPrepared;
        private int _touchPivotFingerCount;

        // Reset Logic
        private Vector3 initialFocusPoint;
        private Vector3 initialTargetOffset;

        protected override void Awake()
        {
            base.Awake();
            currentDistance = distance;
            targetDistance = distance;
            defaultMinDistance = Mathf.Max(Mathf.Min(minDistance, DEFAULT_MODEL_MIN_DISTANCE), MIN_DISTANCE_FLOOR);
            defaultMaxDistance = Mathf.Max(maxDistance, defaultMinDistance + 0.5f);
            currentMinDistance = defaultMinDistance;
            currentMaxDistance = defaultMaxDistance;
            
            // FORCE override inspector values that might be stale
            minDistance = currentMinDistance;
            maxDistance = currentMaxDistance;
            minVerticalAngle = -89f;
            maxVerticalAngle = 89f;

            Camera cam = GetComponent<Camera>();
            if (cam != null)
            {
                cam.nearClipPlane = Mathf.Min(cam.nearClipPlane, 0.01f);
                cam.farClipPlane = Mathf.Max(cam.farClipPlane, 200f);
            }
            // Render settings are handled by EnvironmentController.Start() (Phase 5 cleanup)
        }

        private void Start()
        {
            ResolveDefaultTargetIfNeeded();

            if (target != null)
            {
                defaultNavigationContext = target;
                currentFocusPoint = target.position + targetOffset;
                targetFocusPoint = currentFocusPoint;
            }

            CalibrateDefaultNavigationContext(immediate: true);
            
            // Capture Initial State for Reset
            initialFocusPoint = targetFocusPoint;
            initialTargetOffset = targetOffset;
        }

        private void LateUpdate()
        {
            // Camera can operate without a physical target transform (using focus point).
            HandleInput();
            RefreshAdaptiveNavigationContext();
            
            // Interpolate View Shift
            currentViewShiftRatio = Mathf.Lerp(currentViewShiftRatio, targetViewShiftRatio, Time.deltaTime * dampingFactor);
            
            UpdateCamera();
        }

        private void HandleInput()
        {
            // Camera input should only yield while the UI is actively capturing
            // a primary-button gesture (sliders, sheet drag/scroll, button press).
            // Passive hover/pick checks were causing large phantom dead-zones.
            if (InputManager.InputBlocked)
            {
                if (_touchGestureBlockedByUI && Input.touchCount == 0)
                {
                    _touchGestureBlockedByUI = false;
                    _touchPivotPrepared = false;
                    _touchPivotFingerCount = 0;
                    InputManager.InputBlocked = false;
                }

                return;
            }

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
                _orbitGestureBlockedByUI = IsPointerOverCameraBlockingUI();
                if (!_orbitGestureBlockedByUI)
                {
                    PickPivot(Input.mousePosition);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                _orbitGestureBlockedByUI = false;
            }

            if (Input.GetMouseButtonDown(2))
            {
                _panGestureBlockedByUI = IsPointerOverCameraBlockingUI();
            }
            if (Input.GetMouseButtonUp(2))
            {
                _panGestureBlockedByUI = false;
            }

            // 1. Orbit (Right Click)
            if (Input.GetMouseButton(1) && !_orbitGestureBlockedByUI)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
                ApplyOrbit(mouseX, mouseY);
            }

            // 2. Pan (Middle Click)
            if (Input.GetMouseButton(2) && !_panGestureBlockedByUI)
            {
                float mouseX = Input.GetAxis("Mouse X") * panSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * panSpeed;
                ApplyPan(mouseX, mouseY);
            }

            // 3. Zoom (Scroll)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f && !IsPointerOverCameraBlockingUI())
            {
                ApplyZoom(scroll * MOUSE_SCROLL_SCALE);
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                _touchGestureBlockedByUI = false;
                _touchPivotPrepared = false;
                _touchPivotFingerCount = 0;
                InputManager.InputBlocked = false;
                return;
            }

            bool anyTouchOverBlockingUI = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    continue;
                }

                if (IsScreenPositionOverCameraBlockingUI(touch.position))
                {
                    anyTouchOverBlockingUI = true;
                    break;
                }
            }

            if (anyTouchOverBlockingUI)
            {
                _touchGestureBlockedByUI = true;
                InputManager.InputBlocked = true;
            }

            if (_touchGestureBlockedByUI)
            {
                bool allTouchesReleased = true;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        allTouchesReleased = false;
                        break;
                    }
                }

                if (allTouchesReleased)
                {
                    _touchGestureBlockedByUI = false;
                    _touchPivotPrepared = false;
                    _touchPivotFingerCount = 0;
                    InputManager.InputBlocked = false;
                }

                return;
            }

            PrepareTouchPivotIfNeeded();

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
                float panMagnitude = panDelta.magnitude;

                // ── Zoom: Pinch distance change ──
                float curDist  = Vector2.Distance(t0.position, t1.position);
                float prevDist = Vector2.Distance(
                    t0.position - t0.deltaPosition,
                    t1.position - t1.deltaPosition);
                float pinchDelta = curDist - prevDist;
                float pinchMagnitude = Mathf.Abs(pinchDelta);

                bool panActive = panMagnitude > TOUCH_TWO_FINGER_DEAD;
                bool pinchActive = pinchMagnitude > TOUCH_TWO_FINGER_DEAD;

                bool usePan = panActive && (!pinchActive || panMagnitude >= pinchMagnitude * TOUCH_GESTURE_DOMINANCE);
                bool usePinch = pinchActive && (!panActive || pinchMagnitude > panMagnitude * TOUCH_GESTURE_DOMINANCE);

                if (!usePan && !usePinch)
                {
                    if (panActive && pinchActive)
                    {
                        usePan = panMagnitude >= pinchMagnitude;
                        usePinch = !usePan;
                    }
                    else
                    {
                        usePan = panActive;
                        usePinch = pinchActive;
                    }
                }

                if (usePan)
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

                if (usePinch)
                {
                    // Normalize by screen height so the gesture feels the same
                    // on any device resolution
                    float normalizedPinch = pinchDelta / Mathf.Max(Screen.height, 1);
                    ApplyZoom(normalizedPinch
                        * zoomSpeed
                        * (currentDistance * touchZoomSpeed / 0.008f)
                        * TOUCH_PINCH_RESPONSE_SCALE);
                }
            }
        }

        private void PrepareTouchPivotIfNeeded()
        {
            int fingerCount = Input.touchCount;
            if (fingerCount == 0)
            {
                _touchPivotPrepared = false;
                _touchPivotFingerCount = 0;
                return;
            }

            bool hasNewTouch = false;
            for (int i = 0; i < fingerCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    hasNewTouch = true;
                    break;
                }
            }

            if (_touchPivotPrepared && _touchPivotFingerCount == fingerCount && !hasNewTouch)
            {
                return;
            }

            Vector2 pivotScreenPos = Input.GetTouch(0).position;
            if (fingerCount >= 2)
            {
                pivotScreenPos = (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f;
            }

            Transform activeContext = ResolveActiveInteractionContextTransform();
            if (!TryFocusNavigationOnTransform(activeContext))
            {
                PickPivot(pivotScreenPos);
            }

            _touchPivotPrepared = true;
            _touchPivotFingerCount = fingerCount;
        }

        private static bool IsPointerOverCameraBlockingUI()
        {
            return InputManager.Instance != null
                && InputManager.Instance.IsPointerOverCameraBlockingUI();
        }

        private static bool IsScreenPositionOverCameraBlockingUI(Vector2 screenPos)
        {
            return InputManager.Instance != null
                && InputManager.Instance.IsScreenPositionOverCameraBlockingUI(screenPos);
        }

        // Shared Logic
        private void ApplyOrbit(float x, float y)
        {
            float orbitFactor = GetAdaptiveOrbitFactor();
            targetX += x * orbitFactor;
            targetY -= y * orbitFactor;
            targetY = Mathf.Clamp(targetY, minVerticalAngle, maxVerticalAngle);
        }

        private void ApplyPan(float x, float y)
        {
            // Pan in camera-local space so the drag direction stays aligned with the current view.
            Vector3 right = transform.right.normalized;
            Vector3 up = transform.up.normalized;

            float panFactor = GetAdaptivePanFactor();
            Vector3 move = ((-right * x) + (-up * y)) * panFactor;

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
             float zoomMin = GetZoomMinDistance();
             float zoomMax = GetZoomMaxDistance();
             float zoomRange = Mathf.Max(zoomMax - zoomMin, 0.001f);
             float zoomStep = Mathf.Max(zoomRange * 0.12f, zoomMin * 0.25f, 0.01f);

             targetDistance -= amount * zoomStep;
             targetDistance = Mathf.Clamp(targetDistance, zoomMin, zoomMax);
        }

        private void PickPivot(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_MAX_DISTANCE))
            {
                Vector3 newFocus = hit.point;
                float newDist = Vector3.Distance(transform.position, newFocus);
                Transform hitTransform = hit.transform;
                if (hitTransform != null && TryGetObjectBounds(hitTransform, out Bounds hitBounds))
                {
                    RememberNavigationContext(hitTransform);
                    ConfigureZoomWindow(hitBounds, CalculateFocusDistance(hitBounds), hitTransform);
                }
                
                // Clamp distance to avoid being too close
                if (newDist >= GetZoomMinDistance())
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
                    targetDistance = Mathf.Clamp(newDist, GetZoomMinDistance(), GetZoomMaxDistance());
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
            if (defaultNavigationContext == null && target != null)
            {
                defaultNavigationContext = target;
            }

            if (newTarget != null && newTarget != defaultNavigationContext && TryGetObjectBounds(newTarget, out _))
            {
                RememberNavigationContext(newTarget);
            }
            else if (newTarget == defaultNavigationContext)
            {
                ClearRetainedNavigationContext();
            }

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
            float focusDistance = FOCUS_FALLBACK_DISTANCE;
            if (TryGetObjectBounds(objTransform, out Bounds objectBounds))
            {
                centerOffset = objectBounds.center - objTransform.position;
                focusDistance = CalculateFocusDistance(objectBounds);
                ConfigureZoomWindow(objectBounds, focusDistance, objTransform);
            }
            else
            {
                var rend = objTransform.GetComponent<Renderer>();
                if (rend != null)
                {
                    centerOffset = rend.bounds.center - objTransform.position;
                }

                RestoreDefaultZoomWindow();
            }

            // Keep current rotation angles
            targetX = currentX; 
            targetY = DEFAULT_VERTICAL_ANGLE;
            
            SetTarget(objTransform, false);
            targetOffset = centerOffset; 
            
            targetDistance = Mathf.Clamp(focusDistance, GetZoomMinDistance(), GetZoomMaxDistance());
        }

        public void SetViewportShift(float shiftRatio)
        {
            targetViewShiftRatio = shiftRatio;
        }

        public void ResetView()
        {
            ClearRetainedNavigationContext();

            if (defaultNavigationContext != null)
            {
                target = defaultNavigationContext;
            }

            CalibrateDefaultNavigationContext(immediate: false);

            // Reset Orbit & Zoom
            targetX = 0f;
            targetY = DEFAULT_VERTICAL_ANGLE;
            targetDistance = distance;
            RestoreDefaultZoomWindow();

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
            targetDistance = Mathf.Clamp(newDistance, GetZoomMinDistance(), GetZoomMaxDistance());
            if (immediate)
            {
                currentDistance = targetDistance;
            }
        }

        private float GetZoomMinDistance()
        {
            float floor = IsFastenerInspectionContext(ResolveAdaptiveContextTransform())
                ? FASTENER_INSPECTION_MIN_DISTANCE
                : MIN_DISTANCE_FLOOR;
            return Mathf.Max(currentMinDistance, floor);
        }

        private float GetZoomMaxDistance()
        {
            return Mathf.Max(currentMaxDistance, GetZoomMinDistance() + 0.01f);
        }

        private void RestoreDefaultZoomWindow()
        {
            currentMinDistance = defaultMinDistance;
            currentMaxDistance = defaultMaxDistance;
            minDistance = currentMinDistance;
            maxDistance = currentMaxDistance;
            targetDistance = Mathf.Clamp(targetDistance, GetZoomMinDistance(), GetZoomMaxDistance());
        }

        private void ConfigureZoomWindow(Bounds bounds, float focusDistance, Transform context)
        {
            bool fastenerInspectionContext = IsFastenerInspectionContext(context);
            float dominantExtent = Mathf.Max(bounds.extents.x, Mathf.Max(bounds.extents.y, bounds.extents.z));
            float minFloor = fastenerInspectionContext ? FASTENER_INSPECTION_MIN_DISTANCE : MIN_DISTANCE_FLOOR;
            float minRatio = fastenerInspectionContext ? 0.18f : 0.75f;
            float absoluteMin = fastenerInspectionContext ? FASTENER_INSPECTION_MIN_DISTANCE : FOCUS_DISTANCE_MIN;
            float desiredMin = Mathf.Max(dominantExtent * minRatio, absoluteMin);
            float desiredMax = Mathf.Max(focusDistance * 3.25f, desiredMin * 4f);
            float contextualMaxFloor = Mathf.Min(defaultMaxDistance, defaultMinDistance * ADAPTIVE_MAX_DISTANCE_FLOOR_MULTIPLIER);
            float relaxedMax = Mathf.Max(desiredMax, contextualMaxFloor, currentDistance * 1.15f);

            currentMinDistance = Mathf.Clamp(desiredMin, minFloor, defaultMaxDistance);
            currentMaxDistance = Mathf.Clamp(relaxedMax, currentMinDistance + 0.15f, defaultMaxDistance);
            minDistance = currentMinDistance;
            maxDistance = currentMaxDistance;
            targetDistance = Mathf.Clamp(targetDistance, GetZoomMinDistance(), GetZoomMaxDistance());
        }

        private void RefreshAdaptiveNavigationContext()
        {
            Transform context = ResolveAdaptiveContextTransform();
            if (context != null && TryGetObjectBounds(context, out Bounds bounds))
            {
                float focusDistance = CalculateFocusDistance(bounds);
                ConfigureZoomWindow(bounds, focusDistance, context);
                return;
            }

            RestoreDefaultZoomWindow();
        }

        private Transform ResolveAdaptiveContextTransform()
        {
            Transform selection = SelectionManager.Instance != null ? SelectionManager.Instance.CurrentSelection : null;
            if (selection != null)
            {
                RememberNavigationContext(selection);
                return selection;
            }

            if (PartVisibilityManager.Instance != null)
            {
                Transform isolated = PartVisibilityManager.Instance.GetIsolatedTransform();
                if (isolated != null)
                {
                    RememberNavigationContext(isolated);
                    return isolated;
                }

                ExplodablePart isolatedPart = PartVisibilityManager.Instance.GetIsolatedPart();
                if (isolatedPart != null)
                {
                    RememberNavigationContext(isolatedPart.transform);
                    return isolatedPart.transform;
                }
            }

            if (TryGetRetainedNavigationContext(out Transform retainedContext))
            {
                return retainedContext;
            }

            if (defaultNavigationContext != null)
            {
                return defaultNavigationContext;
            }

            return target;
        }

        private Transform ResolveActiveInteractionContextTransform()
        {
            Transform selection = SelectionManager.Instance != null ? SelectionManager.Instance.CurrentSelection : null;
            if (selection != null)
            {
                RememberNavigationContext(selection);
                return selection;
            }

            if (PartVisibilityManager.Instance != null)
            {
                Transform isolated = PartVisibilityManager.Instance.GetIsolatedTransform();
                if (isolated != null)
                {
                    RememberNavigationContext(isolated);
                    return isolated;
                }

                ExplodablePart isolatedPart = PartVisibilityManager.Instance.GetIsolatedPart();
                if (isolatedPart != null)
                {
                    RememberNavigationContext(isolatedPart.transform);
                    return isolatedPart.transform;
                }
            }

            if (TryGetRetainedNavigationContext(out Transform retainedContext))
            {
                return retainedContext;
            }

            return null;
        }

        private bool TryFocusNavigationOnTransform(Transform focusTransform)
        {
            if (focusTransform == null || !TryGetObjectBounds(focusTransform, out Bounds bounds))
            {
                return false;
            }

            Vector3 focusPoint = bounds.center;
            if (target != null)
            {
                targetOffset = focusPoint - target.position;
            }
            else
            {
                targetFocusPoint = focusPoint;
            }

            ConfigureZoomWindow(bounds, CalculateFocusDistance(bounds), focusTransform);
            RememberNavigationContext(focusTransform);
            targetDistance = Mathf.Clamp(targetDistance, GetZoomMinDistance(), GetZoomMaxDistance());
            return true;
        }

        private void RememberNavigationContext(Transform context)
        {
            if (context == null || context == defaultNavigationContext)
            {
                return;
            }

            retainedNavigationContext = context;
        }

        private bool TryGetRetainedNavigationContext(out Transform context)
        {
            context = retainedNavigationContext;
            if (context == null)
            {
                return false;
            }

            if (!context.gameObject.activeInHierarchy)
            {
                retainedNavigationContext = null;
                context = null;
                return false;
            }

            return true;
        }

        private void ClearRetainedNavigationContext()
        {
            retainedNavigationContext = null;
        }

        private static bool IsFastenerInspectionContext(Transform context)
        {
            if (context == null)
            {
                return false;
            }

            if (FastenerBuilder.IsFastenerDetailTransform(context))
            {
                return true;
            }

            Transform current = context;
            while (current != null)
            {
                FastenerRuntimeMarker marker = current.GetComponent<FastenerRuntimeMarker>();
                if (marker != null && marker.IsInspectable && marker.SourceIsPrimitiveFastener)
                {
                    return true;
                }

                if (FastenerBuilder.IsFastenerDetailTransform(current))
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private void ResolveDefaultTargetIfNeeded()
        {
            if (target != null && target.gameObject.activeInHierarchy && target.name == RuntimeDroneRootName)
            {
                return;
            }

            GameObject runtimeRoot = GameObject.Find(RuntimeDroneRootName);
            if (runtimeRoot != null)
            {
                target = runtimeRoot.transform;
            }
        }

        private void CalibrateDefaultNavigationContext(bool immediate)
        {
            Transform context = defaultNavigationContext != null ? defaultNavigationContext : target;
            if (context == null || !TryGetObjectBounds(context, out Bounds bounds))
            {
                return;
            }

            float dominantSize = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            if (dominantSize <= 0.001f)
            {
                return;
            }

            float calibratedMin = Mathf.Clamp(
                dominantSize * DEFAULT_CONTEXT_MIN_DISTANCE_RATIO,
                MIN_DISTANCE_FLOOR,
                DEFAULT_MODEL_MIN_DISTANCE);

            float framingDistance = CalculateFramingDistance(
                bounds,
                DEFAULT_VIEW_DISTANCE_PADDING,
                DEFAULT_VIEW_DOMINANT_MULTIPLIER);

            float calibratedDistance = Mathf.Max(framingDistance, calibratedMin + 0.5f);
            float calibratedMax = Mathf.Max(
                defaultMaxDistance,
                maxDistance,
                calibratedDistance * 2.5f,
                dominantSize * DEFAULT_CONTEXT_MAX_DISTANCE_RATIO);

            defaultMinDistance = calibratedMin;
            defaultMaxDistance = Mathf.Max(calibratedMax, defaultMinDistance + 0.5f);
            currentMinDistance = defaultMinDistance;
            currentMaxDistance = defaultMaxDistance;
            minDistance = currentMinDistance;
            maxDistance = currentMaxDistance;
            distance = Mathf.Clamp(calibratedDistance, currentMinDistance, currentMaxDistance);
            targetDistance = Mathf.Clamp(targetDistance, currentMinDistance, currentMaxDistance);

            if (immediate)
            {
                targetDistance = distance;
                currentDistance = distance;
            }
        }

        private float GetAdaptiveOrbitFactor()
        {
            float distanceFactor = currentDistance / Mathf.Max(defaultMinDistance, 0.001f);
            float normalizedFactor = Mathf.Clamp01(distanceFactor);
            float adaptiveFactor = Mathf.Pow(normalizedFactor, ADAPTIVE_ORBIT_EXPONENT);
            return Mathf.Clamp(adaptiveFactor, MIN_ADAPTIVE_ORBIT_FACTOR, 1f);
        }

        private float GetAdaptivePanFactor()
        {
            float distanceFactor = currentDistance / Mathf.Max(defaultMinDistance, 0.001f);
            float normalizedFactor = Mathf.Clamp01(distanceFactor);
            float adaptiveFactor = Mathf.Pow(normalizedFactor, ADAPTIVE_PAN_EXPONENT);
            return Mathf.Clamp(adaptiveFactor, MIN_ADAPTIVE_PAN_FACTOR, 1f);
        }

        private bool TryGetObjectBounds(Transform root, out Bounds bounds)
        {
            bounds = default;
            if (root == null)
            {
                return false;
            }

            bool hasBounds = false;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (hasBounds)
            {
                return true;
            }

            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider == null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = collider.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            return hasBounds;
        }

        private float CalculateFocusDistance(Bounds bounds)
        {
            Camera cam = GetComponent<Camera>();
            if (cam == null)
            {
                return FOCUS_FALLBACK_DISTANCE;
            }

            Vector3 extents = bounds.extents;
            float halfVerticalFov = Mathf.Max(cam.fieldOfView * 0.5f * Mathf.Deg2Rad, 0.01f);
            float halfHorizontalFov = Mathf.Max(Mathf.Atan(Mathf.Tan(halfVerticalFov) * cam.aspect), 0.01f);

            float distanceForHeight = extents.y / Mathf.Tan(halfVerticalFov);
            float distanceForWidth = extents.x / Mathf.Tan(halfHorizontalFov);
            float distanceForDepth = extents.z;
            float dominantExtent = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));

            float baseDistance = Mathf.Max(distanceForHeight, distanceForWidth) + distanceForDepth;
            float paddedDistance = Mathf.Max(baseDistance * FOCUS_DISTANCE_PADDING, dominantExtent * 2.2f);

            return Mathf.Clamp(paddedDistance, Mathf.Max(minDistance, FOCUS_DISTANCE_MIN), maxDistance);
        }

        private float CalculateFramingDistance(Bounds bounds, float padding, float dominantMultiplier)
        {
            Camera cam = GetComponent<Camera>();
            if (cam == null)
            {
                return FOCUS_FALLBACK_DISTANCE;
            }

            Vector3 extents = bounds.extents;
            float halfVerticalFov = Mathf.Max(cam.fieldOfView * 0.5f * Mathf.Deg2Rad, 0.01f);
            float halfHorizontalFov = Mathf.Max(Mathf.Atan(Mathf.Tan(halfVerticalFov) * cam.aspect), 0.01f);

            float distanceForHeight = extents.y / Mathf.Tan(halfVerticalFov);
            float distanceForWidth = extents.x / Mathf.Tan(halfHorizontalFov);
            float distanceForDepth = extents.z;
            float dominantExtent = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
            float baseDistance = Mathf.Max(distanceForHeight, distanceForWidth) + distanceForDepth;

            return Mathf.Max(baseDistance * padding, dominantExtent * dominantMultiplier);
        }
    }
}
