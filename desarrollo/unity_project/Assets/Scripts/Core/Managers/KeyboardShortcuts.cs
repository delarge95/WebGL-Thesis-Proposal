using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class KeyboardShortcuts : Singleton<KeyboardShortcuts>
    {
        [Header("Settings")]
        [SerializeField] private bool enableShortcuts = true;

        private void Update()
        {
            if (!enableShortcuts) return;

            // Phase 4: Skip keyboard shortcuts when pointer is interacting with UI
            // (prevents accidental state changes while typing or interacting with UI elements)
            if (InputManager.InputBlocked) return;

            // Camera Presets
            if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyCameraPreset(0); // Front
            if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyCameraPreset(1); // Back
            if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyCameraPreset(2); // Left
            if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyCameraPreset(3); // Right
            if (Input.GetKeyDown(KeyCode.Alpha5)) ApplyCameraPreset(4); // Top
            if (Input.GetKeyDown(KeyCode.Alpha6)) ApplyCameraPreset(5); // Isometric

            // State Transitions
            if (Input.GetKeyDown(KeyCode.E)) ToggleExplodedView();
            if (Input.GetKeyDown(KeyCode.Escape)) GoBack();
            if (Input.GetKeyDown(KeyCode.R)) ResetView();

            // Mode shortcuts (Phase 2 UX Redesign)
            if (Input.GetKeyDown(KeyCode.Alpha7)) SwitchToExplore();
            if (Input.GetKeyDown(KeyCode.Alpha8)) SwitchToAnalyze();
            if (Input.GetKeyDown(KeyCode.Alpha9)) SwitchToStudio();

            // Accessibility
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Plus))
            {
                IncreaseUIScale();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Minus))
            {
                DecreaseUIScale();
            }

            // Debug
            if (Input.GetKeyDown(KeyCode.F1)) ToggleDebugConsole();
            if (Input.GetKeyDown(KeyCode.F2)) ToggleFPSCounter();
        }

        private void ApplyCameraPreset(int index)
        {
            CameraPresets.Instance?.ApplyPreset(index);
            AudioManager.Instance?.PlayClick();
        }

        private void ToggleExplodedView()
        {
            if (AppStateMachine.Instance == null) return;

            var currentState = AppStateMachine.Instance.CurrentState;

            // Phase 2: Only toggle exploded view from interactive modes
            if (!AppStateMachine.Instance.IsInteractive()) return;

            if (currentState == AppState.ExplodedView)
            {
                AppStateMachine.Instance.SetState(AppState.Exploration);
            }
            else if (currentState == AppState.Exploration || currentState == AppState.Analyze)
            {
                AppStateMachine.Instance.SetState(AppState.ExplodedView);
            }
            
            AudioManager.Instance?.PlayClick();
        }

        private void GoBack()
        {
            if (AppStateMachine.Instance == null) return;

            var currentState = AppStateMachine.Instance.CurrentState;
            
            // Deselect first
            SelectionManager.Instance?.Deselect();

            // Then transition
            if (currentState != AppState.Exploration)
            {
                AppStateMachine.Instance.SetState(AppState.Exploration);
            }
        }

        private void ResetView()
        {
            OrbitCameraController.Instance?.ResetView();
            SelectionManager.Instance?.Deselect();
            NotificationManager.Instance?.ShowNotification("View reset");
        }

        private void IncreaseUIScale()
        {
            if (AccessibilityManager.Instance != null)
            {
                float current = AccessibilityManager.Instance.UIScale;
                AccessibilityManager.Instance.SetUIScale(current + 0.1f);
            }
        }

        private void DecreaseUIScale()
        {
            if (AccessibilityManager.Instance != null)
            {
                float current = AccessibilityManager.Instance.UIScale;
                AccessibilityManager.Instance.SetUIScale(current - 0.1f);
            }
        }

        private void ToggleDebugConsole()
        {
            // RuntimeConsole handles its own toggle via BackQuote key
            // This is an alternative for F1
            Debug.Log("[KeyboardShortcuts] Debug console toggle requested");
        }

        private void ToggleFPSCounter()
        {
            var fpsCounter = FindAnyObjectByType<FPSCounter>();
            if (fpsCounter != null)
            {
                fpsCounter.enabled = !fpsCounter.enabled;
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Mode shortcuts (Phase 2 UX Redesign)
        // ═══════════════════════════════════════════════════════

        private void SwitchToExplore()
        {
            if (AppStateMachine.Instance == null || !AppStateMachine.Instance.IsInteractive()) return;
            AppStateMachine.Instance.EnterExploration();
            AudioManager.Instance?.PlayClick();
        }

        private void SwitchToAnalyze()
        {
            if (AppStateMachine.Instance == null || !AppStateMachine.Instance.IsInteractive()) return;
            AppStateMachine.Instance.EnterAnalyze();
            AudioManager.Instance?.PlayClick();
        }

        private void SwitchToStudio()
        {
            if (AppStateMachine.Instance == null || !AppStateMachine.Instance.IsInteractive()) return;
            AppStateMachine.Instance.EnterStudio();
            AudioManager.Instance?.PlayClick();
        }
    }
}
