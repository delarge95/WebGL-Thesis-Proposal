using UnityEngine;
using UnityEngine.UIElements;

public class SmartHotspot
{
    private VisualElement _root;
    private Transform _target;
    private Camera _camera;
    private VisualElement _container;

    private bool _isVisible = true;

    // Fixed offset if needed (e.g., above the center)
    private Vector3 _offset = Vector3.zero;

    public SmartHotspot(VisualElement container, Transform target, Camera cam)
    {
        _container = container;
        _target = target;
        _camera = cam;

        // Create the UI element
        _root = new VisualElement();
        _root.AddToClassList("hotspot-dot");
        _root.pickingMode = PickingMode.Position; // Enable clicks
        _root.name = $"Hotspot_{target.name}";

        // Store reference to target logic if needed (e.g. userData)
        _root.userData = target;

        _container.Add(_root);
    }

    public void Update()
    {
        if (_target == null || _camera == null) return;

        // 1. Check if behind camera
        Vector3 screenPos = _camera.WorldToScreenPoint(_target.position + _offset);

        if (screenPos.z < 0)
        {
            Hide();
            return; 
        }

        // 2. Convert to Panel Coordinates (Y is flipped in UI Toolkit vs Screen)
        // Note: RuntimePanelUtils.ScreenToPanel is the robust way, but simple flip often works for overlay.
        // screenPos.y = Screen.height - screenPos.y; 
        // CAUTION: VisualElement uses layout coordinates. If proper match is needed:
        
        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_root.panel, new Vector2(screenPos.x, Screen.height - screenPos.y));

        // 3. Apply position
        _root.style.left = panelPos.x;
        _root.style.top = panelPos.y;

        Show();
    }

    public void RegisterCallback<T>(EventCallback<T> callback) where T : EventBase
    {
        _root.RegisterCallback(callback);
    }

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

    public void Destroy()
    {
        if (_root != null && _root.parent != null)
        {
            _root.parent.Remove(_root);
        }
    }
}
