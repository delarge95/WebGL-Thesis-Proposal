using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class ConnectionPoint
    {
        public string connectionId;
        public string connectionType; // Screw, Snap, Solder, Wire, etc.
        public Transform point;
        public ExplodablePart connectedTo;
        public string description;
        public string torqueSpec; // "2.5 Nm"
        public bool isConnected;
    }

    public class ConnectionPointsViewer : Singleton<ConnectionPointsViewer>
    {
        [Header("Settings")]
        [SerializeField] private bool showConnections = false;
        [SerializeField] private float markerSize = 0.02f;

        [Header("Colors by Type")]
        [SerializeField] private Color screwColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color snapColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color solderColor = new Color(0.8f, 0.4f, 0.1f);
        [SerializeField] private Color wireColor = new Color(0.2f, 0.6f, 1f);

        private List<ConnectionPoint> allConnections = new List<ConnectionPoint>();
        private List<GameObject> connectionMarkers = new List<GameObject>();
        private List<LineRenderer> connectionLines = new List<LineRenderer>();

        public bool IsVisible => showConnections;
        public List<ConnectionPoint> AllConnections => allConnections;

        public event Action<ConnectionPoint> OnConnectionSelected;

        private void Start()
        {
            FindAllConnections();
        }

        public void FindAllConnections()
        {
            allConnections.Clear();

            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (var part in parts)
            {
                var connPoints = part.GetComponentsInChildren<ConnectionPointMarker>();
                foreach (var cp in connPoints)
                {
                    var connection = new ConnectionPoint
                    {
                        connectionId = cp.ConnectionId,
                        connectionType = cp.ConnectionType,
                        point = cp.transform,
                        description = cp.Description,
                        torqueSpec = cp.TorqueSpec,
                        isConnected = cp.IsConnected
                    };
                    allConnections.Add(connection);
                }
            }

            Debug.Log($"[ConnectionPoints] Found {allConnections.Count} connection points");
        }

        public void ShowConnections()
        {
            showConnections = true;
            CreateMarkers();
            CreateConnectionLines();

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Showing connection points");
            }
        }

        public void HideConnections()
        {
            showConnections = false;
            ClearMarkers();
            ClearLines();
        }

        public void Toggle()
        {
            if (showConnections) HideConnections();
            else ShowConnections();
        }

        private void CreateMarkers()
        {
            ClearMarkers();

            foreach (var conn in allConnections)
            {
                if (conn.point == null) continue;

                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = $"[ConnPoint_{conn.connectionId}]";
                marker.transform.position = conn.point.position;
                marker.transform.localScale = Vector3.one * markerSize;

                var collider = marker.GetComponent<Collider>();
                if (collider != null) Destroy(collider);

                var renderer = marker.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                renderer.material.SetColor("_BaseColor", GetColorForType(conn.connectionType));

                connectionMarkers.Add(marker);
            }
        }

        private void CreateConnectionLines()
        {
            ClearLines();

            foreach (var conn in allConnections)
            {
                if (conn.point == null || conn.connectedTo == null) continue;

                var lineObj = new GameObject($"[ConnLine_{conn.connectionId}]");
                var lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, conn.point.position);
                lr.SetPosition(1, conn.connectedTo.transform.position);
                lr.startWidth = 0.005f;
                lr.endWidth = 0.005f;
                lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                lr.material.SetColor("_BaseColor", GetColorForType(conn.connectionType));

                connectionLines.Add(lr);
            }
        }

        private Color GetColorForType(string type)
        {
            return type?.ToLower() switch
            {
                "screw" => screwColor,
                "snap" => snapColor,
                "solder" => solderColor,
                "wire" => wireColor,
                _ => Color.white
            };
        }

        private void ClearMarkers()
        {
            foreach (var marker in connectionMarkers)
            {
                if (marker != null) Destroy(marker);
            }
            connectionMarkers.Clear();
        }

        private void ClearLines()
        {
            foreach (var line in connectionLines)
            {
                if (line != null) Destroy(line.gameObject);
            }
            connectionLines.Clear();
        }

        public List<ConnectionPoint> GetConnectionsByType(string type)
        {
            return allConnections.FindAll(c => c.connectionType == type);
        }

        public List<ConnectionPoint> GetConnectionsForPart(ExplodablePart part)
        {
            return allConnections.FindAll(c => 
                c.point != null && c.point.IsChildOf(part.transform));
        }
    }

    // Helper component to mark connection points on parts
    public class ConnectionPointMarker : MonoBehaviour
    {
        public string ConnectionId;
        public string ConnectionType = "Screw";
        public string Description;
        public string TorqueSpec;
        public bool IsConnected;
        public ExplodablePart ConnectedPart;
    }
}
