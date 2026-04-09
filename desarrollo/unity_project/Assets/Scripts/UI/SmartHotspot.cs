using System;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Managers;

public class SmartHotspot
{
    public enum InteractionType
    {
        HoverEnter,
        HoverLeave,
        Click,
        DoubleClick
    }

    private readonly VisualElement _root;
    private readonly VisualElement _container;
    private readonly Label _nameLabel;
    private readonly ExplodablePart _targetPart;
    private readonly Transform _target;
    private readonly Camera _camera;
    private readonly HighlightSystem _highlight;
    private readonly Renderer[] _renderers;
    private readonly string _labelOverride;
    private readonly Action _clickAction;
    private readonly Action<InteractionType> _interactionAction;

    private bool _isVisible = true;
    private bool _isEnabled = true;
    private bool _isOccluded;
    private Vector2 _smoothedPos;
    private bool _hasPreviousPos;

    private readonly int _frameOffset;
    private const float SmoothSpeed = 18f;
    private const int OcclusionInterval = 8;
    private const float AnchorHeightMultiplier = 1.15f;
    private const float AnchorMinimumOffset = 0.12f;

    public SmartHotspot(
        VisualElement container,
        ExplodablePart targetPart,
        Camera camera,
        string labelOverride = "",
        Action clickAction = null,
        Action<InteractionType> interactionAction = null)
    {
        _container = container;
        _targetPart = targetPart;
        _target = targetPart != null ? targetPart.transform : null;
        _camera = camera;
        _labelOverride = labelOverride ?? string.Empty;
        _clickAction = clickAction;
        _interactionAction = interactionAction;
        _highlight = _target != null ? _target.GetComponent<HighlightSystem>() : null;
        _renderers = _target != null ? _target.GetComponentsInChildren<Renderer>(true) : null;
        _frameOffset = UnityEngine.Random.Range(0, OcclusionInterval);

        _root = new VisualElement();
        _root.AddToClassList("hotspot-dot");
        _root.pickingMode = PickingMode.Position;
        _root.name = $"Hotspot_{(_target != null ? _target.name : "Unknown")}";

        _nameLabel = new Label();
        _nameLabel.AddToClassList("hotspot-label");
        _nameLabel.text = ResolveHotspotLabel();
        _nameLabel.pickingMode = PickingMode.Ignore;
        _root.Add(_nameLabel);

        _root.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        _root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        _root.RegisterCallback<ClickEvent>(OnClick);

        _container.Add(_root);
    }

    public void Update()
    {
        if (_target == null || _camera == null || _root.panel == null || !_isEnabled)
        {
            return;
        }

        if (!TryCalculateWorldAnchor(out Vector3 worldPos, out Bounds bounds))
        {
            Hide();
            return;
        }

        Vector3 viewPos = _camera.WorldToViewportPoint(worldPos);
        if (viewPos.z <= 0f || viewPos.x < -0.05f || viewPos.x > 1.05f || viewPos.y < -0.05f || viewPos.y > 1.05f)
        {
            Hide();
            return;
        }

        if ((Time.frameCount + _frameOffset) % OcclusionInterval == 0)
        {
            _isOccluded = false;
            if (Physics.Linecast(_camera.transform.position, bounds.center, out RaycastHit hit))
            {
                Transform hitPart = hit.transform != null ? DroneRenderResolver.ResolveCanonicalPart(hit.transform)?.transform : null;
                if (hitPart != _target)
                {
                    _isOccluded = true;
                }
            }
        }

        if (_isOccluded)
        {
            Hide();
            return;
        }

        IPanel panel = _root.panel;
        if (panel == null)
        {
            return;
        }

        Vector2 rawPos = RuntimePanelUtils.CameraTransformWorldToPanel(panel, worldPos, _camera);
        if (float.IsNaN(rawPos.x) || float.IsNaN(rawPos.y))
        {
            return;
        }

        if (!_hasPreviousPos)
        {
            _smoothedPos = rawPos;
            _hasPreviousPos = true;
        }
        else
        {
            _smoothedPos = Vector2.Lerp(_smoothedPos, rawPos, Time.deltaTime * SmoothSpeed);
        }

        _root.style.left = _smoothedPos.x;
        _root.style.top = _smoothedPos.y;
        Show();
    }

    private void OnPointerEnter(PointerEnterEvent evt)
    {
        _root.AddToClassList("hotspot-dot--hover");
        _nameLabel.AddToClassList("hotspot-label--visible");
        _interactionAction?.Invoke(InteractionType.HoverEnter);
        _highlight?.OnHoverEnter();
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        _root.RemoveFromClassList("hotspot-dot--hover");
        _nameLabel.RemoveFromClassList("hotspot-label--visible");
        _interactionAction?.Invoke(InteractionType.HoverLeave);
        _highlight?.OnHoverExit();
    }

    private void OnClick(ClickEvent evt)
    {
        evt.StopPropagation();

        if (evt.clickCount >= 2)
        {
            _interactionAction?.Invoke(InteractionType.DoubleClick);
            return;
        }
        _interactionAction?.Invoke(InteractionType.Click);

        if (_clickAction != null)
        {
            _clickAction.Invoke();
            return;
        }

        SelectionManager.Instance?.SelectObject(_target, fromHotspot: true);
    }

    private void Hide()
    {
        if (!_isVisible)
        {
            return;
        }

        _root.AddToClassList("hotspot-dot--hidden");
        _root.pickingMode = PickingMode.Ignore;
        _isVisible = false;
    }

    private void Show()
    {
        if (_isVisible || !_isEnabled)
        {
            return;
        }

        _root.RemoveFromClassList("hotspot-dot--hidden");
        _root.pickingMode = PickingMode.Position;
        _isVisible = true;
    }

    public void SetEnabled(bool enabled)
    {
        if (_isEnabled == enabled)
        {
            return;
        }

        _isEnabled = enabled;
        if (!enabled)
        {
            _root.AddToClassList("hotspot-dot--hidden");
            _root.pickingMode = PickingMode.Ignore;
            _nameLabel.RemoveFromClassList("hotspot-label--visible");
            _root.RemoveFromClassList("hotspot-dot--hover");
            _highlight?.OnHoverExit();
            _isVisible = false;
            return;
        }

        _hasPreviousPos = false;
        if (!_isOccluded)
        {
            _root.RemoveFromClassList("hotspot-dot--hidden");
            _root.pickingMode = PickingMode.Position;
            _isVisible = true;
        }
    }

    private string ResolveHotspotLabel()
    {
        if (!string.IsNullOrWhiteSpace(_labelOverride))
        {
            return _labelOverride;
        }

        if (_targetPart != null && _targetPart.Data != null)
        {
            if (!string.IsNullOrWhiteSpace(_targetPart.Data.hotspotLabel))
            {
                return _targetPart.Data.hotspotLabel;
            }

            if (!string.IsNullOrWhiteSpace(_targetPart.Data.partName))
            {
                return _targetPart.Data.partName;
            }
        }

        return (_target != null ? _target.name : "Unknown").Replace("_", " ");
    }

    private bool TryCalculateWorldAnchor(out Vector3 worldPos, out Bounds bounds)
    {
        worldPos = _target != null ? _target.position : Vector3.zero;
        bounds = new Bounds(worldPos, Vector3.zero);

        if (_renderers == null || _renderers.Length == 0)
        {
            return _target != null;
        }

        bool hasBounds = false;
        foreach (Renderer renderer in _renderers)
        {
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
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

        if (!hasBounds)
        {
            return false;
        }

        worldPos = bounds.center + Vector3.up * Mathf.Max(bounds.extents.y * AnchorHeightMultiplier, AnchorMinimumOffset);
        return true;
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

        try
        {
            _root.parent?.Remove(_root);
        }
        catch (System.InvalidOperationException)
        {
        }
    }
}
