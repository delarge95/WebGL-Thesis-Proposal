using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Utils
{
    public class RuntimeConsole : PersistentSingleton<RuntimeConsole>
    {
        [Header("Settings")]
        [SerializeField] private bool showConsole = true;
        [SerializeField] private int maxLogCount = 20;
        
        private Queue<string> logs = new Queue<string>();
        private Vector2 scrollPosition;
        private bool isVisible = false;

        protected override void Awake()
        {
            base.Awake();
            Application.logMessageReceived += HandleLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            string color = "white";
            switch (type)
            {
                case LogType.Error: color = "red"; break;
                case LogType.Warning: color = "yellow"; break;
                case LogType.Exception: color = "magenta"; break;
            }

            string formattedLog = $"<color={color}>[{type}] {logString}</color>";
            logs.Enqueue(formattedLog);

            if (logs.Count > maxLogCount)
            {
                logs.Dequeue();
            }
        }

        private void Update()
        {
            // Toggle with Tilde key or 3-finger tap
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.touchCount == 3)
            {
                isVisible = !isVisible;
            }
        }

        private void OnGUI()
        {
            if (!showConsole || !isVisible) return;

            float height = Screen.height * 0.3f;
            GUI.Box(new Rect(0, 0, Screen.width, height), "Debug Console");

            GUILayout.BeginArea(new Rect(10, 20, Screen.width - 20, height - 20));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (string log in logs)
            {
                GUILayout.Label(log);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
