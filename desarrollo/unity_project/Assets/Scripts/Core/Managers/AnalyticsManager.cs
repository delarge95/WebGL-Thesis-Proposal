using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class AnalyticsEvent
    {
        public string eventName;
        public string timestamp;
        public Dictionary<string, string> parameters;

        public AnalyticsEvent(string name)
        {
            eventName = name;
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            parameters = new Dictionary<string, string>();
        }
    }

    public class AnalyticsManager : PersistentSingleton<AnalyticsManager>
    {
        [Header("Settings")]
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool logToConsole = true;

        private List<AnalyticsEvent> sessionEvents = new List<AnalyticsEvent>();
        private float sessionStartTime;
        private Dictionary<string, float> partViewTimes = new Dictionary<string, float>();
        private string lastViewedPart;
        private float partViewStartTime;

        protected override void Awake()
        {
            base.Awake();
            sessionStartTime = Time.time;
        }

        public void TrackEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (!enableAnalytics) return;

            var evt = new AnalyticsEvent(eventName);
            if (parameters != null)
            {
                evt.parameters = parameters;
            }
            sessionEvents.Add(evt);

            if (logToConsole)
            {
                Debug.Log($"[Analytics] {eventName}");
            }
        }

        public void TrackPartSelected(string partName)
        {
            // End previous part view
            if (!string.IsNullOrEmpty(lastViewedPart))
            {
                float viewTime = Time.time - partViewStartTime;
                if (!partViewTimes.ContainsKey(lastViewedPart))
                {
                    partViewTimes[lastViewedPart] = 0f;
                }
                partViewTimes[lastViewedPart] += viewTime;
            }

            // Start new part view
            lastViewedPart = partName;
            partViewStartTime = Time.time;

            TrackEvent("PartSelected", new Dictionary<string, string>
            {
                { "partName", partName }
            });
        }

        public void TrackStateChange(string newState)
        {
            TrackEvent("StateChange", new Dictionary<string, string>
            {
                { "state", newState }
            });
        }

        public void TrackZoom(float distance)
        {
            TrackEvent("Zoom", new Dictionary<string, string>
            {
                { "distance", distance.ToString("F2") }
            });
        }

        public void TrackCameraPreset(string presetName)
        {
            TrackEvent("CameraPreset", new Dictionary<string, string>
            {
                { "preset", presetName }
            });
        }

        public float GetSessionDuration()
        {
            return Time.time - sessionStartTime;
        }

        public Dictionary<string, float> GetPartViewTimes()
        {
            return new Dictionary<string, float>(partViewTimes);
        }

        public string GetSessionSummary()
        {
            string summary = $"Session Duration: {GetSessionDuration():F1}s\n";
            summary += $"Total Events: {sessionEvents.Count}\n\n";
            summary += "Part View Times:\n";
            foreach (var kvp in partViewTimes)
            {
                summary += $"  {kvp.Key}: {kvp.Value:F1}s\n";
            }
            return summary;
        }

        protected override void OnApplicationQuit()
        {
            if (logToConsole)
            {
                Debug.Log("[Analytics] Session Summary:\n" + GetSessionSummary());
            }
        }
    }
}
