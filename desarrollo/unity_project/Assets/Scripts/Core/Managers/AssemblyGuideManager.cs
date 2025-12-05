using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class AssemblyStep
    {
        public string stepId;
        public int stepNumber;
        public string title;
        [TextArea(2, 4)] public string instructions;
        public ExplodablePart[] involvedParts;
        public string[] requiredTools;
        public string[] safetyWarnings;
        public float estimatedTimeMinutes;
        public int difficultyLevel; // 1-5
        public string tipText;
        public Vector3 cameraPosition;
        public Vector3 cameraLookAt;
        public bool isCompleted;
    }

    [CreateAssetMenu(fileName = "AssemblyGuide", menuName = "WebGL/Assembly Guide")]
    public class AssemblyGuideData : ScriptableObject
    {
        public string guideName;
        public string productName;
        public string version;
        public List<AssemblyStep> steps = new List<AssemblyStep>();
        public float totalEstimatedTime;
    }

    public class AssemblyGuideManager : Singleton<AssemblyGuideManager>
    {
        [Header("Guide Data")]
        [SerializeField] private AssemblyGuideData currentGuide;

        [Header("Settings")]
        [SerializeField] private bool highlightCurrentParts = true;
        [SerializeField] private bool autoAdvance = false;
        [SerializeField] private Color currentStepHighlight = new Color(0.2f, 1f, 0.4f, 0.8f);
        [SerializeField] private Color completedStepColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private int currentStepIndex = 0;
        private bool isGuideActive = false;

        public AssemblyGuideData CurrentGuide => currentGuide;
        public AssemblyStep CurrentStep => currentGuide?.steps[currentStepIndex];
        public int CurrentStepIndex => currentStepIndex;
        public int TotalSteps => currentGuide?.steps.Count ?? 0;
        public bool IsActive => isGuideActive;
        public float ProgressPercent => TotalSteps > 0 ? (float)GetCompletedCount() / TotalSteps * 100f : 0f;

        public event Action<AssemblyStep> OnStepChanged;
        public event Action<AssemblyStep> OnStepCompleted;
        public event Action OnGuideCompleted;

        public void StartGuide(AssemblyGuideData guide = null)
        {
            if (guide != null) currentGuide = guide;
            if (currentGuide == null)
            {
                Debug.LogError("[AssemblyGuide] No guide data assigned!");
                return;
            }

            currentStepIndex = 0;
            isGuideActive = true;
            ResetAllSteps();
            ShowCurrentStep();

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"Starting: {currentGuide.guideName}");
            }

            Debug.Log($"[AssemblyGuide] Started guide: {currentGuide.guideName} with {TotalSteps} steps");
        }

        public void StopGuide()
        {
            isGuideActive = false;
            ClearHighlights();

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Assembly guide stopped");
            }
        }

        public void NextStep()
        {
            if (!isGuideActive || currentGuide == null) return;

            if (currentStepIndex < TotalSteps - 1)
            {
                currentStepIndex++;
                ShowCurrentStep();
                OnStepChanged?.Invoke(CurrentStep);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayClick();
                }
            }
        }

        public void PreviousStep()
        {
            if (!isGuideActive || currentGuide == null) return;

            if (currentStepIndex > 0)
            {
                currentStepIndex--;
                ShowCurrentStep();
                OnStepChanged?.Invoke(CurrentStep);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayClick();
                }
            }
        }

        public void GoToStep(int index)
        {
            if (!isGuideActive || currentGuide == null) return;
            if (index < 0 || index >= TotalSteps) return;

            currentStepIndex = index;
            ShowCurrentStep();
            OnStepChanged?.Invoke(CurrentStep);
        }

        public void CompleteCurrentStep()
        {
            if (CurrentStep == null) return;

            CurrentStep.isCompleted = true;
            OnStepCompleted?.Invoke(CurrentStep);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccess();
            }

            // Check if guide is complete
            if (GetCompletedCount() == TotalSteps)
            {
                OnGuideCompleted?.Invoke();
                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification("🎉 Assembly Complete!");
                }
            }
            else if (autoAdvance)
            {
                NextStep();
            }

            Debug.Log($"[AssemblyGuide] Step {currentStepIndex + 1} completed");
        }

        public void ToggleStepComplete()
        {
            if (CurrentStep == null) return;
            
            if (CurrentStep.isCompleted)
            {
                CurrentStep.isCompleted = false;
            }
            else
            {
                CompleteCurrentStep();
            }
        }

        private void ShowCurrentStep()
        {
            if (CurrentStep == null) return;

            ClearHighlights();

            // Highlight involved parts
            if (highlightCurrentParts && CurrentStep.involvedParts != null)
            {
                foreach (var part in CurrentStep.involvedParts)
                {
                    if (part == null) continue;

                    var highlight = part.GetComponent<HighlightSystem>();
                    if (highlight != null)
                    {
                        highlight.OnSelect();
                    }

                    // Make sure part is visible
                    PartVisibilityManager.Instance?.ShowPart(part);
                }

                // Isolate to show only relevant parts
                if (CurrentStep.involvedParts.Length > 0)
                {
                    // Could implement multi-part isolation here
                }
            }

            // Move camera to step position
            if (CurrentStep.cameraPosition != Vector3.zero && OrbitCameraController.Instance != null)
            {
                // Set camera to predefined position for this step
                if (CurrentStep.involvedParts != null && CurrentStep.involvedParts.Length > 0)
                {
                    OrbitCameraController.Instance.SetTarget(CurrentStep.involvedParts[0].transform);
                }
            }

            Debug.Log($"[AssemblyGuide] Showing step {currentStepIndex + 1}: {CurrentStep.title}");
        }

        private void ClearHighlights()
        {
            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                var highlight = part.GetComponent<HighlightSystem>();
                if (highlight != null)
                {
                    highlight.OnDeselect();
                }
            }
        }

        private void ResetAllSteps()
        {
            if (currentGuide == null) return;
            foreach (var step in currentGuide.steps)
            {
                step.isCompleted = false;
            }
        }

        public int GetCompletedCount()
        {
            if (currentGuide == null) return 0;
            int count = 0;
            foreach (var step in currentGuide.steps)
            {
                if (step.isCompleted) count++;
            }
            return count;
        }

        public List<string> GetAllRequiredTools()
        {
            var tools = new HashSet<string>();
            if (currentGuide == null) return new List<string>();

            foreach (var step in currentGuide.steps)
            {
                if (step.requiredTools != null)
                {
                    foreach (var tool in step.requiredTools)
                    {
                        tools.Add(tool);
                    }
                }
            }
            return new List<string>(tools);
        }

        public float GetTotalEstimatedTime()
        {
            if (currentGuide == null) return 0f;
            float total = 0f;
            foreach (var step in currentGuide.steps)
            {
                total += step.estimatedTimeMinutes;
            }
            return total;
        }
    }
}
