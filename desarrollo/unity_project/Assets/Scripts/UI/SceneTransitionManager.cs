using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
    {
        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Color fadeColor = Color.black;

        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement fadeOverlay;
        private bool isTransitioning = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            CreateFadeOverlay();
        }

        private void CreateFadeOverlay()
        {
            if (uiDocument == null) return;

            fadeOverlay = new VisualElement();
            fadeOverlay.style.position = Position.Absolute;
            fadeOverlay.style.left = 0;
            fadeOverlay.style.right = 0;
            fadeOverlay.style.top = 0;
            fadeOverlay.style.bottom = 0;
            fadeOverlay.style.backgroundColor = fadeColor;
            fadeOverlay.style.opacity = 0;
            fadeOverlay.pickingMode = PickingMode.Ignore;
            fadeOverlay.style.display = DisplayStyle.None;

            uiDocument.rootVisualElement.Add(fadeOverlay);
        }

        public void FadeOut(System.Action onComplete = null)
        {
            if (isTransitioning || fadeOverlay == null) return;
            StartCoroutine(FadeRoutine(0, 1, onComplete));
        }

        public void FadeIn(System.Action onComplete = null)
        {
            if (isTransitioning || fadeOverlay == null) return;
            StartCoroutine(FadeRoutine(1, 0, onComplete));
        }

        public void CrossFade(System.Action onMidpoint, System.Action onComplete = null)
        {
            if (isTransitioning || fadeOverlay == null) return;
            StartCoroutine(CrossFadeRoutine(onMidpoint, onComplete));
        }

        private IEnumerator FadeRoutine(float from, float to, System.Action onComplete)
        {
            isTransitioning = true;
            fadeOverlay.style.display = DisplayStyle.Flex;
            fadeOverlay.pickingMode = PickingMode.Position; // Block input during transition

            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeDuration;
                fadeOverlay.style.opacity = Mathf.Lerp(from, to, t);
                yield return null;
            }
            fadeOverlay.style.opacity = to;

            if (to == 0)
            {
                fadeOverlay.style.display = DisplayStyle.None;
            }
            fadeOverlay.pickingMode = PickingMode.Ignore;
            isTransitioning = false;

            onComplete?.Invoke();
        }

        private IEnumerator CrossFadeRoutine(System.Action onMidpoint, System.Action onComplete)
        {
            isTransitioning = true;
            fadeOverlay.style.display = DisplayStyle.Flex;
            fadeOverlay.pickingMode = PickingMode.Position;

            // Fade Out
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeOverlay.style.opacity = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
            fadeOverlay.style.opacity = 1;

            // Midpoint callback
            onMidpoint?.Invoke();
            yield return null; // Wait a frame for changes to apply

            // Fade In
            timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeOverlay.style.opacity = Mathf.Lerp(1, 0, timer / fadeDuration);
                yield return null;
            }
            fadeOverlay.style.opacity = 0;

            fadeOverlay.style.display = DisplayStyle.None;
            fadeOverlay.pickingMode = PickingMode.Ignore;
            isTransitioning = false;

            onComplete?.Invoke();
        }
    }
}
