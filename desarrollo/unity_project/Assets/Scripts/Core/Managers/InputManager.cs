using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        [Header("Settings")]
        [SerializeField] private float mouseSensitivity = 1.0f;
        [SerializeField] private float touchSensitivity = 0.5f;

        public Vector2 LookInput { get; private set; }
        public float ZoomInput { get; private set; }
        public bool IsInteracting { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            LookInput = Vector2.zero;
            ZoomInput = 0f;
            IsInteracting = false;

            // Mouse Input
            if (Input.GetMouseButton(1)) // Right click to orbit
            {
                LookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
                IsInteracting = true;
            }

            ZoomInput = Input.GetAxis("Mouse ScrollWheel");

            // Touch Input (Simple implementation)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    LookInput = touch.deltaPosition * touchSensitivity * Time.deltaTime;
                    IsInteracting = true;
                }
                
                // Pinch to zoom logic would go here
            }
        }
    }
}
