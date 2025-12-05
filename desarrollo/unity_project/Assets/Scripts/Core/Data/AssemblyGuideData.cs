using UnityEngine;

namespace WebGL.Core.Data
{
    [CreateAssetMenu(fileName = "NewAssemblyGuide", menuName = "WebGL/Assembly Guide Data")]
    public class AssemblyGuideData : ScriptableObject
    {
        [Header("Guide Info")]
        public string guideName;
        public string guideId;
        [TextArea(2, 4)] public string description;
        public Sprite thumbnail;

        [Header("Steps")]
        public AssemblyStep[] steps;

        [Header("Summary")]
        public float totalEstimatedMinutes;
        public int overallDifficulty; // 1-5
        public string[] allRequiredTools;

        public int StepCount => steps?.Length ?? 0;

        private void OnValidate()
        {
            // Auto-calculate totals
            if (steps != null && steps.Length > 0)
            {
                totalEstimatedMinutes = 0;
                int maxDifficulty = 1;
                var tools = new System.Collections.Generic.HashSet<string>();

                foreach (var step in steps)
                {
                    totalEstimatedMinutes += step.estimatedTimeMinutes;
                    if (step.difficultyLevel > maxDifficulty)
                        maxDifficulty = step.difficultyLevel;
                    
                    if (step.requiredTools != null)
                    {
                        foreach (var tool in step.requiredTools)
                        {
                            tools.Add(tool);
                        }
                    }
                }

                overallDifficulty = maxDifficulty;
                allRequiredTools = new string[tools.Count];
                tools.CopyTo(allRequiredTools);
            }
        }
    }

    [System.Serializable]
    public class AssemblyStep
    {
        [Header("Step Info")]
        public string stepId;
        public int stepNumber;
        public string title;
        [TextArea(3, 6)] public string instructions;

        [Header("Parts")]
        public string[] involvedPartIds; // IDs from DronePartData
        
        [Header("Requirements")]
        public string[] requiredTools;
        public string[] safetyWarnings;
        
        [Header("Time & Difficulty")]
        public float estimatedTimeMinutes;
        [Range(1, 5)] public int difficultyLevel = 1;

        [Header("Tips")]
        [TextArea(2, 4)] public string tipText;

        [Header("Camera")]
        public Vector3 cameraPosition;
        public Vector3 cameraLookAt;

        [Header("State")]
        public bool isCompleted;
    }
}
