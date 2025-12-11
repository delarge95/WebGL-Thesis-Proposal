using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class UIAnimator : Singleton<UIAnimator>
    {
        // Constants for animation classes defined in USS
        public const string ClassFadeOut = "opacity-0";
        public const string ClassSlideOut = "slide-out";

        public void FadeIn(VisualElement element, float duration = 0.3f)
        {
            if (element == null) return;
            
            // Ensure element is visible before fading in
            element.style.display = DisplayStyle.Flex;
            
            // Wait a frame to ensure layout is calculated if it was hidden
            StartCoroutine(FadeInRoutine(element, duration));
        }

        private IEnumerator FadeInRoutine(VisualElement element, float duration)
        {
            yield return null; // Wait for layout
            
            // Use UI Toolkit's experimental animation API if available, or manual lerp
            // Here we use a simple transition class approach which is cleaner
            element.RemoveFromClassList(ClassFadeOut);
            
            // Fallback manual lerp if CSS transitions aren't set up
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                element.style.opacity = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
            element.style.opacity = 1;
        }

        public void FadeOut(VisualElement element, float duration = 0.3f)
        {
            if (element == null) return;
            StartCoroutine(FadeOutRoutine(element, duration));
        }

        private IEnumerator FadeOutRoutine(VisualElement element, float duration)
        {
            element.AddToClassList(ClassFadeOut);

            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                element.style.opacity = Mathf.Lerp(1, 0, timer / duration);
                yield return null;
            }
            element.style.opacity = 0;
            element.style.display = DisplayStyle.None;
        }
    }
}
