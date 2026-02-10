using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WebGL.Editor.Antigravity
{
    /// <summary>
    /// The "Eyes" of the Antigravity Agent.
    /// Captures editor state, compilation errors, and runtime logs to JSON files.
    /// </summary>
    [InitializeOnLoad]
    public static class AntigravityBridge
    {
        private static readonly string LOG_DIR = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");
        private static readonly string STATE_FILE = Path.Combine(LOG_DIR, "agent_state.json");
        private static readonly string ERROR_FILE = Path.Combine(LOG_DIR, "agent_errors.json");

        static AntigravityBridge()
        {
            // Ensure log directory exists
            if (!Directory.Exists(LOG_DIR)) Directory.CreateDirectory(LOG_DIR);

            // Subscribe to events
            Application.logMessageReceived += OnLogMessage;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            
            // Initial state dump
            DumpState();
        }

        private static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                var errorEntry = new ErrorEntry
                {
                    timestamp = System.DateTime.Now.ToString("o"),
                    type = type.ToString(),
                    message = condition,
                    stackTrace = stackTrace.Split('\n').Take(5).ToArray() // First 5 lines only
                };
                
                // Append to error log (JSONL format for easy appending)
                File.AppendAllText(ERROR_FILE, JsonUtility.ToJson(errorEntry) + "\n");
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            DumpState();
        }

        private static void OnCompilationFinished(object obj)
        {
            DumpState();
        }

        private static void DumpState()
        {
            var state = new EditorState
            {
                timestamp = System.DateTime.Now.ToString("o"),
                isPlaying = EditorApplication.isPlaying,
                isCompiling = EditorApplication.isCompiling,
                isPaused = EditorApplication.isPaused,
                activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                platform = EditorUserBuildSettings.activeBuildTarget.ToString()
            };

            File.WriteAllText(STATE_FILE, JsonUtility.ToJson(state, true));
        }

        [System.Serializable]
        private class EditorState
        {
            public string timestamp;
            public bool isPlaying;
            public bool isCompiling;
            public bool isPaused;
            public string activeScene;
            public string platform;
        }

        [System.Serializable]
        private class ErrorEntry
        {
            public string timestamp;
            public string type;
            public string message;
            public string[] stackTrace;
        }
    }
}
