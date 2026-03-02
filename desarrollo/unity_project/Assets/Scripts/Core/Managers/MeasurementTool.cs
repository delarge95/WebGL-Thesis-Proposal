using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public enum MeasurementMode
    {
        Distance,
        Angle,
        Radius
    }

    public class MeasurementTool : Singleton<MeasurementTool>
    {
        [Header("Settings")]
        [SerializeField] private bool isActive = false;
        [SerializeField] private MeasurementMode mode = MeasurementMode.Distance;
        [SerializeField] private LayerMask measureLayer;

        [Header("Visuals")]
        [SerializeField] private Color lineColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private float lineWidth = 0.01f;
        [SerializeField] private Material lineMaterial;

        private List<Vector3> measurePoints = new List<Vector3>();
        private List<LineRenderer> lineRenderers = new List<LineRenderer>();
        private List<GameObject> pointMarkers = new List<GameObject>();
        private float lastMeasurement = 0f;
        private Coroutine _inputCoroutine;

        public bool IsActive => isActive;
        public MeasurementMode Mode => mode;
        public float LastMeasurement => lastMeasurement;

        public event System.Action<float, string> OnMeasurementComplete;

        private IEnumerator InputPolling()
        {
            while (isActive)
            {
                if (Input.GetMouseButtonDown(0))
                    AddMeasurePoint();
                if (Input.GetKeyDown(KeyCode.Escape))
                    ClearMeasurement();
                yield return null;
            }
            _inputCoroutine = null;
        }

        public void Activate(MeasurementMode measureMode = MeasurementMode.Distance)
        {
            isActive = true;
            mode = measureMode;
            ClearMeasurement();

            if (_inputCoroutine != null) StopCoroutine(_inputCoroutine);
            _inputCoroutine = StartCoroutine(InputPolling());

            if (ServiceLocator.TryGet<CursorManager>(out var cursor))
            {
                cursor.SetCursor(CursorType.Pointer);
            }

            if (NotificationManager.Instance != null)
            {
                string modeText = mode == MeasurementMode.Distance ? "distance" : 
                                  mode == MeasurementMode.Angle ? "angle" : "radius";
                NotificationManager.Instance.ShowNotification($"Measurement mode: {modeText}. Click to place points.");
            }
        }

        public void Deactivate()
        {
            isActive = false;
            if (_inputCoroutine != null) { StopCoroutine(_inputCoroutine); _inputCoroutine = null; }
            ClearMeasurement();

            if (ServiceLocator.TryGet<CursorManager>(out var cursorMgr))
            {
                cursorMgr.ResetCursor();
            }
        }

        public void Toggle()
        {
            if (isActive) Deactivate();
            else Activate();
        }

        private void AddMeasurePoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, measureLayer))
            {
                measurePoints.Add(hit.point);
                CreatePointMarker(hit.point);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayClick();
                }

                // Check if we have enough points
                switch (mode)
                {
                    case MeasurementMode.Distance:
                        if (measurePoints.Count >= 2)
                        {
                            CalculateDistance();
                        }
                        break;

                    case MeasurementMode.Angle:
                        if (measurePoints.Count >= 3)
                        {
                            CalculateAngle();
                        }
                        break;

                    case MeasurementMode.Radius:
                        if (measurePoints.Count >= 2)
                        {
                            CalculateRadius();
                        }
                        break;
                }

                UpdateVisualLines();
            }
        }

        private void CalculateDistance()
        {
            if (measurePoints.Count < 2) return;

            float dist = Vector3.Distance(measurePoints[0], measurePoints[1]);
            lastMeasurement = dist;

            string result = FormatDistance(dist);
            OnMeasurementComplete?.Invoke(dist, result);

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"Distance: {result}", 3f);
            }

            Debug.Log($"[MeasurementTool] Distance: {result}");
        }

        private void CalculateAngle()
        {
            if (measurePoints.Count < 3) return;

            Vector3 a = measurePoints[0];
            Vector3 b = measurePoints[1]; // Vertex
            Vector3 c = measurePoints[2];

            Vector3 ba = (a - b).normalized;
            Vector3 bc = (c - b).normalized;

            float angle = Mathf.Acos(Vector3.Dot(ba, bc)) * Mathf.Rad2Deg;
            lastMeasurement = angle;

            string result = $"{angle:F1}°";
            OnMeasurementComplete?.Invoke(angle, result);

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"Angle: {result}", 3f);
            }

            Debug.Log($"[MeasurementTool] Angle: {result}");
        }

        private void CalculateRadius()
        {
            if (measurePoints.Count < 2) return;

            float radius = Vector3.Distance(measurePoints[0], measurePoints[1]);
            lastMeasurement = radius;

            string result = FormatDistance(radius);
            float circumference = 2 * Mathf.PI * radius;
            float area = Mathf.PI * radius * radius;

            OnMeasurementComplete?.Invoke(radius, result);

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification(
                    $"Radius: {result}\nCircumference: {FormatDistance(circumference)}\nArea: {(area * 10000):F1} cm²", 4f);
            }

            Debug.Log($"[MeasurementTool] Radius: {result}");
        }

        private string FormatDistance(float meters)
        {
            if (meters < 0.01f)
            {
                return $"{(meters * 1000):F1} mm";
            }
            else if (meters < 1f)
            {
                return $"{(meters * 100):F1} cm";
            }
            else
            {
                return $"{meters:F2} m";
            }
        }

        private void CreatePointMarker(Vector3 position)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "[MeasurePoint]";
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.02f;

            var collider = marker.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = marker.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            renderer.material.SetColor("_BaseColor", lineColor);

            pointMarkers.Add(marker);
        }

        private void UpdateVisualLines()
        {
            // Clear existing lines
            foreach (var lr in lineRenderers)
            {
                if (lr != null) Destroy(lr.gameObject);
            }
            lineRenderers.Clear();

            if (measurePoints.Count < 2) return;

            // Create line
            var lineObj = new GameObject("[MeasureLine]");
            var lr2 = lineObj.AddComponent<LineRenderer>();
            lr2.positionCount = measurePoints.Count;
            lr2.SetPositions(measurePoints.ToArray());
            lr2.startWidth = lineWidth;
            lr2.endWidth = lineWidth;
            lr2.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lr2.material.SetColor("_BaseColor", lineColor);
            lineRenderers.Add(lr2);
        }

        public void ClearMeasurement()
        {
            measurePoints.Clear();

            foreach (var marker in pointMarkers)
            {
                if (marker != null) Destroy(marker);
            }
            pointMarkers.Clear();

            foreach (var lr in lineRenderers)
            {
                if (lr != null) Destroy(lr.gameObject);
            }
            lineRenderers.Clear();

            lastMeasurement = 0f;
        }

        public void SetMode(MeasurementMode newMode)
        {
            mode = newMode;
            ClearMeasurement();
        }
    }
}
