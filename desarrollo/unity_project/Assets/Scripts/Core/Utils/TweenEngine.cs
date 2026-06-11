using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public enum EaseType
    {
        Linear,
        EaseInOut,
        EaseIn,
        EaseOut,
        EaseInBack,
        EaseOutBack,
        EaseInElastic,
        EaseOutElastic
    }

    public class TweenEngine : PersistentSingleton<TweenEngine>
    {
        private List<Coroutine> activeTweens = new List<Coroutine>();

        #region Float Tweens
        public Coroutine TweenFloat(float from, float to, float duration, Action<float> onUpdate, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            var coroutine = StartCoroutine(TweenFloatRoutine(from, to, duration, onUpdate, onComplete, ease));
            activeTweens.Add(coroutine);
            return coroutine;
        }

        private IEnumerator TweenFloatRoutine(float from, float to, float duration, Action<float> onUpdate, Action onComplete, EaseType ease)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float easedT = ApplyEase(t, ease);
                float value = Mathf.LerpUnclamped(from, to, easedT);
                onUpdate?.Invoke(value);
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
        #endregion

        #region Vector3 Tweens
        public Coroutine TweenPosition(Transform target, Vector3 to, float duration, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            return TweenVector3(target.position, to, duration, v => target.position = v, onComplete, ease);
        }

        public Coroutine TweenLocalPosition(Transform target, Vector3 to, float duration, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            return TweenVector3(target.localPosition, to, duration, v => target.localPosition = v, onComplete, ease);
        }

        public Coroutine TweenScale(Transform target, Vector3 to, float duration, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            return TweenVector3(target.localScale, to, duration, v => target.localScale = v, onComplete, ease);
        }

        public Coroutine TweenVector3(Vector3 from, Vector3 to, float duration, Action<Vector3> onUpdate, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            var coroutine = StartCoroutine(TweenVector3Routine(from, to, duration, onUpdate, onComplete, ease));
            activeTweens.Add(coroutine);
            return coroutine;
        }

        private IEnumerator TweenVector3Routine(Vector3 from, Vector3 to, float duration, Action<Vector3> onUpdate, Action onComplete, EaseType ease)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float easedT = ApplyEase(t, ease);
                Vector3 value = Vector3.LerpUnclamped(from, to, easedT);
                onUpdate?.Invoke(value);
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
        #endregion

        #region Color Tweens
        public Coroutine TweenColor(Color from, Color to, float duration, Action<Color> onUpdate, Action onComplete = null, EaseType ease = EaseType.EaseInOut)
        {
            var coroutine = StartCoroutine(TweenColorRoutine(from, to, duration, onUpdate, onComplete, ease));
            activeTweens.Add(coroutine);
            return coroutine;
        }

        private IEnumerator TweenColorRoutine(Color from, Color to, float duration, Action<Color> onUpdate, Action onComplete, EaseType ease)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float easedT = ApplyEase(t, ease);
                Color value = Color.LerpUnclamped(from, to, easedT);
                onUpdate?.Invoke(value);
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
        #endregion

        #region Easing Functions
        private float ApplyEase(float t, EaseType ease)
        {
            switch (ease)
            {
                case EaseType.Linear: return t;
                case EaseType.EaseIn: return t * t;
                case EaseType.EaseOut: return 1f - (1f - t) * (1f - t);
                case EaseType.EaseInOut: return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
                case EaseType.EaseInBack: return 2.70158f * t * t * t - 1.70158f * t * t;
                case EaseType.EaseOutBack: return 1f + 2.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f);
                case EaseType.EaseInElastic:
                    return t == 0f ? 0f : t == 1f ? 1f : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * (2f * Mathf.PI) / 3f);
                case EaseType.EaseOutElastic:
                    return t == 0f ? 0f : t == 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI) / 3f) + 1f;
                default: return t;
            }
        }
        #endregion

        public void StopTween(Coroutine tween)
        {
            if (tween != null)
            {
                StopCoroutine(tween);
                activeTweens.Remove(tween);
            }
        }

        public void StopAllTweens()
        {
            foreach (var tween in activeTweens)
            {
                if (tween != null) StopCoroutine(tween);
            }
            activeTweens.Clear();
        }
    }
}
