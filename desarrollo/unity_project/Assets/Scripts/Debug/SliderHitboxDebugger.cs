using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace WebGL.Debug
{
    /// <summary>
    /// Runtime debugger: press F9 to log worldBound of every .glass-slider dragger,
    /// its drag-container, and the input element. Helps verify that the hitbox
    /// (layout rect / worldBound) matches the visual position of the dragger circle.
    ///
    /// Attach to any GameObject that has a UIDocument, or it will find one automatically.
    /// Remove this script before shipping.
    /// </summary>
    public class SliderHitboxDebugger : MonoBehaviour
    {
        private UIDocument _doc;

        private void Start()
        {
            _doc = GetComponent<UIDocument>();
            if (_doc == null)
                _doc = FindAnyObjectByType<UIDocument>();

            if (_doc == null)
                UnityEngine.Debug.LogWarning("[SliderDebug] No UIDocument found.");
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F9) || _doc == null) return;

            var root = _doc.rootVisualElement;
            if (root == null) return;

            var sliders = root.Query(className: "glass-slider").ToList();
            UnityEngine.Debug.Log($"[SliderDebug] ═══ Found {sliders.Count} .glass-slider elements ═══");

            foreach (var slider in sliders)
            {
                var input    = slider.Q(className: "unity-base-slider__input");
                var dragCont = slider.Q("unity-drag-container");
                var tracker  = slider.Q(className: "unity-base-slider__tracker");
                var dragger  = slider.Q(className: "unity-base-slider__dragger");

                UnityEngine.Debug.Log($"[SliderDebug] ── Slider: {slider.name} ──");
                LogElement("  Slider    ", slider);
                LogElement("  Input     ", input);
                LogElement("  DragCont  ", dragCont);
                LogElement("  Tracker   ", tracker);
                LogElement("  Dragger   ", dragger);

                if (dragger != null)
                {
                    var pos = dragger.resolvedStyle;
                    UnityEngine.Debug.Log(
                        $"[SliderDebug]   Dragger resolved → " +
                        $"position:{pos.position}, " +
                        $"left:{pos.left:F1}, top:{pos.top:F1}, " +
                        $"width:{pos.width:F1}, height:{pos.height:F1}, " +
                        $"marginTop:{pos.marginTop:F1}, marginLeft:{pos.marginLeft:F1}, " +
                        $"translate:{dragger.style.translate.value}"
                    );
                }
            }

            UnityEngine.Debug.Log("[SliderDebug] ═══════════════════════════════════════════");
        }

        private static void LogElement(string label, VisualElement el)
        {
            if (el == null)
            {
                UnityEngine.Debug.Log($"[SliderDebug] {label}: NULL");
                return;
            }

            var wb = el.worldBound;
            var lb = el.layout;
            UnityEngine.Debug.Log(
                $"[SliderDebug] {label}: " +
                $"worldBound=({wb.x:F1},{wb.y:F1},{wb.width:F1},{wb.height:F1})  " +
                $"layout=({lb.x:F1},{lb.y:F1},{lb.width:F1},{lb.height:F1})  " +
                $"pickingMode={el.pickingMode}"
            );
        }
    }
}
