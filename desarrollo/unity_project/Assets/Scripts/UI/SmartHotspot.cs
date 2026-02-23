using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Managers;

/// <summary>
/// A screen-space UI dot that tracks a 3D target.
/// Professional features:
///   - Position smoothing (Lerp) to eliminate jitter/flickering.
///   - Y offset above the part's bounds for easier selection.
///   - Hover → HighlightSystem preview + part name tooltip.
///   - Click → SelectionManager.SelectObject.
///   - Staggered occlusion raycasts for performance.
/// </summary>
public class SmartHotspot
{
    // ─── Core References ───
    private readonly VisualElement _root;
    private readonly VisualElement _container;
    private readonly Label _nameLabel;
    private readonly Transform _target;
    private readonly Camera _camera;
    private readonly HighlightSystem _highlight;

    // ─── State ───
    private bool _isVisible = true;
    private bool _isOccluded = false;
    private Vector2 _smoothedPos;
    private bool _hasPreviousPos = false;

    // ─── Configuration ───
    private readonly int _frameOffset;
    private const float SMOOTH_SPEED = 18f;
    private const float Y_OFFSET_WORLD = 0.35f;
    private const int OCCLUSION_INTERVAL = 8;

    public SmartHotspot(VisualElement container, Transform target, Camera cam)
    {
        _container = container;
        _target = target;
        _camera = cam;
        _highlight = target.GetComponent<HighlightSystem>();
        _frameOffset = Random.Range(0, OCCLUSION_INTERVAL);

        // ─── Build UI ───
        _root = new VisualElement();
        _root.AddToClassList("hotspot-dot");
        _root.pickingMode = PickingMode.Position;
        _root.name = $"Hotspot_{target.name}";

        // ─── Name Label (Tooltip) ───
        _nameLabel = new Label();
        _nameLabel.AddToClassList("hotspot-label");
        _nameLabel.text = FormatPartName(target.name);
        _nameLabel.pickingMode = PickingMode.Ignore;
        _root.Add(_nameLabel);

        // ─── Events ───
        _root.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        _root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        _root.RegisterCallback<ClickEvent>(OnClick);

        _container.Add(_root);
    }

    // ═══════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════
    public void Update()
    {
        if (_target == null || _camera == null || _root.panel == null) return;

        Vector3 worldPos = _target.position + Vector3.up * Y_OFFSET_WORLD;

        // 1. Frustum Check
        Vector3 viewPos = _camera.WorldToViewportPoint(worldPos);
        if (viewPos.z <= 0 || viewPos.x < -0.05f || viewPos.x > 1.05f || viewPos.y < -0.05f || viewPos.y > 1.05f)
        {
            Hide();
            return;
        }

        // 2. Occlusion Check (Staggered)
        if ((Time.frameCount + _frameOffset) % OCCLUSION_INTERVAL == 0)
        {
            _isOccluded = false;
            if (Physics.Linecast(_camera.transform.position, _target.position, out RaycastHit hit))
            {
                if (hit.transform != _target && !hit.transform.IsChildOf(_target))
                {
                    _isOccluded = true;
                }
            }
        }

        if (_isOccluded) { Hide(); return; }

        // 3. Panel Coordinate Conversion
        IPanel panel = _root.panel;
        if (panel == null) return;

        Vector2 rawPos = RuntimePanelUtils.CameraTransformWorldToPanel(panel, worldPos, _camera);
        if (float.IsNaN(rawPos.x) || float.IsNaN(rawPos.y)) return;

        // 4. Smooth Position
        if (!_hasPreviousPos)
        {
            _smoothedPos = rawPos;
            _hasPreviousPos = true;
        }
        else
        {
            _smoothedPos = Vector2.Lerp(_smoothedPos, rawPos, Time.deltaTime * SMOOTH_SPEED);
        }

        // 5. Apply
        _root.style.left = _smoothedPos.x;
        _root.style.top = _smoothedPos.y;

        Show();
    }

    // ═══════════════════════════════════════════════
    // INTERACTIONS
    // ═══════════════════════════════════════════════

    private void OnPointerEnter(PointerEnterEvent evt)
    {
        _root.AddToClassList("hotspot-dot--hover");
        _nameLabel.AddToClassList("hotspot-label--visible");
        _highlight?.OnHoverEnter();
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        _root.RemoveFromClassList("hotspot-dot--hover");
        _nameLabel.RemoveFromClassList("hotspot-label--visible");
        _highlight?.OnHoverExit();
    }

    private void OnClick(ClickEvent evt)
    {
        evt.StopPropagation();
        SelectionManager.Instance?.SelectObject(_target, fromHotspot: true);
    }

    // ═══════════════════════════════════════════════
    // VISIBILITY
    // ═══════════════════════════════════════════════

    private void Hide()
    {
        if (!_isVisible) return;
        _root.AddToClassList("hotspot-dot--hidden");
        _isVisible = false;
    }

    private void Show()
    {
        if (_isVisible) return;
        _root.RemoveFromClassList("hotspot-dot--hidden");
        _isVisible = true;
    }

    // ═══════════════════════════════════════════════
    // UTILITIES
    // ═══════════════════════════════════════════════

    /// <summary>
    /// Converts "Chassis_Main" → "Chassis Main"
    /// </summary>
    private static string FormatPartName(string rawName)
    {
        return rawName.Replace("_", " ");
    }

    public void RegisterCallback<T>(EventCallback<T> callback) where T : EventBase<T>, new()
    {
        _root.RegisterCallback(callback);
    }

    public void Destroy()
    {
        _root.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        _root.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
        _root.UnregisterCallback<ClickEvent>(OnClick);
        _root.parent?.Remove(_root);
    }
}
