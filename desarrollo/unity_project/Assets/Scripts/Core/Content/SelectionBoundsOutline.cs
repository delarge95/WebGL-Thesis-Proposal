using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Content
{
    /// <summary>
    /// Draws a lightweight world-space bounding-box outline using line renderers.
    /// Intended for friendly full-part selection feedback without solid fill tinting.
    /// </summary>
    public class SelectionBoundsOutline : MonoBehaviour
    {
        [SerializeField] private Color outlineColor = new Color(0.2f, 0.8f, 1f, 0.95f);
        [SerializeField] private float lineWidth = 0.0035f;

        private readonly List<LineRenderer> _edges = new List<LineRenderer>(12);
        private Renderer[] _sourceRenderers;
        private Material _lineMaterial;
        private bool _isVisible;

        public void Configure(Renderer[] sourceRenderers, Color color, float width)
        {
            _sourceRenderers = sourceRenderers;
            outlineColor = color;
            lineWidth = Mathf.Max(0.0005f, width);

            EnsureEdges();
            ApplyStyle();
            UpdateGeometry();
        }

        public void SetVisible(bool visible)
        {
            _isVisible = visible;
            EnsureEdges();

            foreach (LineRenderer edge in _edges)
            {
                if (edge != null)
                {
                    edge.enabled = visible;
                }
            }

            if (visible)
            {
                UpdateGeometry();
            }
        }

        private void LateUpdate()
        {
            if (_isVisible)
            {
                UpdateGeometry();
            }
        }

        private void EnsureEdges()
        {
            EnsureMaterial();

            while (_edges.Count < 12)
            {
                GameObject edgeObject = new GameObject($"OutlineEdge_{_edges.Count}");
                edgeObject.transform.SetParent(transform, false);

                LineRenderer line = edgeObject.AddComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.positionCount = 2;
                line.sharedMaterial = _lineMaterial;
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.receiveShadows = false;
                line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                line.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                line.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                line.textureMode = LineTextureMode.Stretch;
                line.numCornerVertices = 1;
                line.numCapVertices = 0;
                line.alignment = LineAlignment.View;

                _edges.Add(line);
            }
        }

        private void EnsureMaterial()
        {
            if (_lineMaterial != null)
            {
                return;
            }

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            _lineMaterial = new Material(shader);
            _lineMaterial.name = "SelectionOutlineRuntimeMat";
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        private void ApplyStyle()
        {
            if (_lineMaterial != null)
            {
                _lineMaterial.color = outlineColor;
            }

            foreach (LineRenderer edge in _edges)
            {
                if (edge == null)
                {
                    continue;
                }

                edge.startColor = outlineColor;
                edge.endColor = outlineColor;
                edge.startWidth = lineWidth;
                edge.endWidth = lineWidth;
            }
        }

        private void UpdateGeometry()
        {
            if (!TryGetCombinedBounds(out Bounds bounds))
            {
                foreach (LineRenderer edge in _edges)
                {
                    if (edge != null)
                    {
                        edge.enabled = false;
                    }
                }
                return;
            }

            ApplyStyle();

            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            Vector3[] corners = new Vector3[8];
            corners[0] = c + new Vector3(-e.x, -e.y, -e.z);
            corners[1] = c + new Vector3(e.x, -e.y, -e.z);
            corners[2] = c + new Vector3(e.x, -e.y, e.z);
            corners[3] = c + new Vector3(-e.x, -e.y, e.z);
            corners[4] = c + new Vector3(-e.x, e.y, -e.z);
            corners[5] = c + new Vector3(e.x, e.y, -e.z);
            corners[6] = c + new Vector3(e.x, e.y, e.z);
            corners[7] = c + new Vector3(-e.x, e.y, e.z);

            int[,] edgeIndices =
            {
                {0, 1}, {1, 2}, {2, 3}, {3, 0},
                {4, 5}, {5, 6}, {6, 7}, {7, 4},
                {0, 4}, {1, 5}, {2, 6}, {3, 7}
            };

            for (int i = 0; i < _edges.Count; i++)
            {
                LineRenderer edge = _edges[i];
                if (edge == null)
                {
                    continue;
                }

                int a = edgeIndices[i, 0];
                int b = edgeIndices[i, 1];

                edge.enabled = _isVisible;
                edge.SetPosition(0, corners[a]);
                edge.SetPosition(1, corners[b]);
            }
        }

        private bool TryGetCombinedBounds(out Bounds bounds)
        {
            bounds = default;

            if (_sourceRenderers == null || _sourceRenderers.Length == 0)
            {
                _sourceRenderers = GetComponentsInChildren<Renderer>(true);
            }

            bool hasBounds = false;
            foreach (Renderer renderer in _sourceRenderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds;
        }

        private void OnDestroy()
        {
            if (_lineMaterial != null)
            {
                Destroy(_lineMaterial);
                _lineMaterial = null;
            }
        }
    }
}
