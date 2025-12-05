using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class Annotation
    {
        public string annotationId;
        public string text;
        public Vector3 worldPosition;
        public ExplodablePart attachedPart;
        public Color color;
        public DateTime createdAt;
        public string author;
        public bool isVisible;
    }

    public class AnnotationSystem : Singleton<AnnotationSystem>
    {
        [Header("Settings")]
        [SerializeField] private bool showAnnotations = true;
        [SerializeField] private Color defaultAnnotationColor = new Color(1f, 0.9f, 0.3f);
        [SerializeField] private float labelScale = 1f;

        private List<Annotation> annotations = new List<Annotation>();
        private Dictionary<string, GameObject> annotationObjects = new Dictionary<string, GameObject>();

        public List<Annotation> Annotations => annotations;
        public bool IsVisible => showAnnotations;

        public event Action<Annotation> OnAnnotationAdded;
        public event Action<Annotation> OnAnnotationRemoved;

        private void Start()
        {
            LoadAnnotations();
        }

        public Annotation AddAnnotation(Vector3 position, string text, ExplodablePart part = null)
        {
            var annotation = new Annotation
            {
                annotationId = Guid.NewGuid().ToString().Substring(0, 8),
                text = text,
                worldPosition = position,
                attachedPart = part,
                color = defaultAnnotationColor,
                createdAt = DateTime.Now,
                author = "Engineer",
                isVisible = true
            };

            annotations.Add(annotation);
            CreateAnnotationVisual(annotation);
            OnAnnotationAdded?.Invoke(annotation);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[Annotation] Added: {text}");
            return annotation;
        }

        public void RemoveAnnotation(string annotationId)
        {
            var annotation = annotations.Find(a => a.annotationId == annotationId);
            if (annotation == null) return;

            annotations.Remove(annotation);

            if (annotationObjects.TryGetValue(annotationId, out var obj))
            {
                Destroy(obj);
                annotationObjects.Remove(annotationId);
            }

            OnAnnotationRemoved?.Invoke(annotation);
            Debug.Log($"[Annotation] Removed: {annotationId}");
        }

        public void ClearAllAnnotations()
        {
            foreach (var obj in annotationObjects.Values)
            {
                if (obj != null) Destroy(obj);
            }
            annotationObjects.Clear();
            annotations.Clear();

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("All annotations cleared");
            }
        }

        private void CreateAnnotationVisual(Annotation annotation)
        {
            // Create container
            var container = new GameObject($"[Annotation_{annotation.annotationId}]");
            container.transform.position = annotation.worldPosition;

            // Create line from part to annotation
            if (annotation.attachedPart != null)
            {
                var lineObj = new GameObject("Line");
                lineObj.transform.SetParent(container.transform);
                var lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, annotation.attachedPart.transform.position);
                lr.SetPosition(1, annotation.worldPosition);
                lr.startWidth = 0.002f;
                lr.endWidth = 0.002f;
                lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                lr.material.SetColor("_BaseColor", annotation.color);
            }

            // Create marker sphere
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Marker";
            marker.transform.SetParent(container.transform);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = Vector3.one * 0.015f;
            
            var collider = marker.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            var renderer = marker.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            renderer.material.SetColor("_BaseColor", annotation.color);

            // Create text (using 3D text for now, would use TextMeshPro in production)
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(container.transform);
            textObj.transform.localPosition = Vector3.up * 0.03f;
            
            var textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = annotation.text;
            textMesh.fontSize = 12;
            textMesh.characterSize = 0.01f * labelScale;
            textMesh.anchor = TextAnchor.LowerCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = annotation.color;

            // Billboard behavior (face camera)
            var billboard = textObj.AddComponent<BillboardBehavior>();

            annotationObjects[annotation.annotationId] = container;
        }

        public void ShowAnnotations()
        {
            showAnnotations = true;
            foreach (var obj in annotationObjects.Values)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        public void HideAnnotations()
        {
            showAnnotations = false;
            foreach (var obj in annotationObjects.Values)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (showAnnotations) HideAnnotations();
            else ShowAnnotations();
        }

        public List<Annotation> GetAnnotationsForPart(ExplodablePart part)
        {
            return annotations.FindAll(a => a.attachedPart == part);
        }

        private void LoadAnnotations()
        {
            // Can load from PlayerPrefs or file in production
        }

        public void SaveAnnotations()
        {
            // Can save to PlayerPrefs or file in production
        }
    }

    // Helper component for billboard text
    public class BillboardBehavior : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
