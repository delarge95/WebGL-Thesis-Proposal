using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class PartVisibilityManager : Singleton<PartVisibilityManager>
    {
        [Header("Settings")]
        [SerializeField] private float fadeTransitionDuration = 0.3f;
#pragma warning disable CS0414 // Reserved for Inspector configuration
        [SerializeField] private float isolatedOpacity = 0.15f;
#pragma warning restore CS0414

        private Dictionary<ExplodablePart, bool> partVisibility = new Dictionary<ExplodablePart, bool>();
        private Dictionary<ExplodablePart, Material[]> originalMaterials = new Dictionary<ExplodablePart, Material[]>();
        private ExplodablePart isolatedPart = null;
        private List<ExplodablePart> allParts = new List<ExplodablePart>();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            CacheParts();
        }

        private void CacheParts()
        {
            allParts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));
            foreach (var part in allParts)
            {
                partVisibility[part] = true;
                var renderer = part.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalMaterials[part] = renderer.sharedMaterials;
                }
            }
        }

        public void HidePart(ExplodablePart part)
        {
            if (part == null) return;
            partVisibility[part] = false;
            StartCoroutine(FadePartOut(part));
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public void ShowPart(ExplodablePart part)
        {
            if (part == null) return;
            partVisibility[part] = true;
            part.gameObject.SetActive(true);
            StartCoroutine(FadePartIn(part));

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public void TogglePartVisibility(ExplodablePart part)
        {
            if (partVisibility.TryGetValue(part, out bool visible))
            {
                if (visible) HidePart(part);
                else ShowPart(part);
            }
        }

        public void IsolatePart(ExplodablePart part)
        {
            if (part == null)
            {
                ClearIsolation();
                return;
            }

            isolatedPart = part;

            foreach (var p in allParts)
            {
                if (p == part)
                {
                    // Ensure isolated part is visible
                    p.gameObject.SetActive(true);
                }
                else
                {
                    // Hide all other parts
                    p.gameObject.SetActive(false);
                }
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[PartVisibility] Isolated: {part.name}");
        }

        public void ClearIsolation()
        {
            isolatedPart = null;

            foreach (var p in allParts)
            {
                // Restore all parts
                p.gameObject.SetActive(true);
                // Reset any material property block overrides
                var renderer = p.GetComponent<Renderer>();
                if (renderer != null)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    renderer.SetPropertyBlock(block);
                }
            }

            Debug.Log("[PartVisibility] Isolation cleared");
        }

        private void SetPartOpacity(ExplodablePart part, float opacity)
        {
            var renderer = part.GetComponent<Renderer>();
            if (renderer == null) return;

            StartCoroutine(AnimateOpacity(renderer, opacity));
        }

        private System.Collections.IEnumerator AnimateOpacity(Renderer renderer, float targetOpacity)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            
            Color currentColor = block.GetColor("_BaseColor");
            if (currentColor == Color.clear) currentColor = Color.white;
            
            float startOpacity = currentColor.a;
            float timer = 0f;

            while (timer < fadeTransitionDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeTransitionDuration;
                t = t * t * (3f - 2f * t); // Smoothstep

                float opacity = Mathf.Lerp(startOpacity, targetOpacity, t);
                currentColor.a = opacity;
                block.SetColor("_BaseColor", currentColor);
                renderer.SetPropertyBlock(block);

                yield return null;
            }

            currentColor.a = targetOpacity;
            block.SetColor("_BaseColor", currentColor);
            renderer.SetPropertyBlock(block);
        }

        private System.Collections.IEnumerator FadePartOut(ExplodablePart part)
        {
            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                yield return AnimateOpacity(renderer, 0f);
            }
            part.gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator FadePartIn(ExplodablePart part)
        {
            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                yield return AnimateOpacity(renderer, 1f);
            }
        }

        public void ShowAllParts()
        {
            foreach (var part in allParts)
            {
                ShowPart(part);
            }
            ClearIsolation();
        }

        public void HideAllParts()
        {
            foreach (var part in allParts)
            {
                HidePart(part);
            }
        }

        public bool IsPartVisible(ExplodablePart part)
        {
            return partVisibility.TryGetValue(part, out bool visible) && visible;
        }

        public ExplodablePart GetIsolatedPart() => isolatedPart;
    }
}
