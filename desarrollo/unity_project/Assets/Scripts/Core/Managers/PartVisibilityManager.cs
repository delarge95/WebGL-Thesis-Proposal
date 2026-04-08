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
        private Transform isolatedTransform = null;
        private List<ExplodablePart> allParts = new List<ExplodablePart>();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RebuildCache();
        }

        public void RebuildCache()
        {
            allParts.Clear();
            partVisibility.Clear();
            originalMaterials.Clear();
            isolatedPart = null;
            isolatedTransform = null;

            allParts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));
            foreach (var part in allParts)
            {
                partVisibility[part] = true;
                var renderers = part.GetComponentsInChildren<Renderer>(true);
                if (renderers != null && renderers.Length > 0)
                {
                    originalMaterials[part] = renderers[0].sharedMaterials;
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
            isolatedTransform = null;

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

        public void IsolateTransform(Transform selection)
        {
            if (selection == null)
            {
                ClearIsolation();
                return;
            }

            ExplodablePart parentPart = selection.GetComponentInParent<ExplodablePart>();
            if (parentPart == null)
            {
                ClearIsolation();
                return;
            }

            isolatedPart = parentPart;
            isolatedTransform = selection;

            bool anyRendererVisible = false;

            foreach (var p in allParts)
            {
                if (p == null) continue;
                p.gameObject.SetActive(true);

                var renderers = p.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;

                    bool visible =
                        renderer.transform == selection ||
                        renderer.transform.IsChildOf(selection) ||
                        selection.IsChildOf(renderer.transform);

                    renderer.enabled = visible;
                    anyRendererVisible |= visible;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = visible;
                    }
                }
            }

            if (!anyRendererVisible)
            {
                // Fallback to canonical isolation when selection has no render surface.
                IsolatePart(parentPart);
                return;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }

            Debug.Log($"[PartVisibility] Isolated transform: {selection.name}");
        }

        public void ClearIsolation()
        {
            isolatedPart = null;
            isolatedTransform = null;

            foreach (var p in allParts)
            {
                // Restore all parts
                p.gameObject.SetActive(true);
                var renderers = p.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    renderer.enabled = true;

                    Collider collider = renderer.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }

                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    renderer.SetPropertyBlock(block);
                }
            }

            Debug.Log("[PartVisibility] Isolation cleared");
        }

        private void SetPartOpacity(ExplodablePart part, float opacity)
        {
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                StartCoroutine(AnimateOpacity(renderer, opacity));
            }
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
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            bool animatedAny = false;
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                animatedAny = true;
                yield return AnimateOpacity(renderer, 0f);
            }
            if (!animatedAny)
            {
                yield return null;
            }
            part.gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator FadePartIn(ExplodablePart part)
        {
            var renderers = part.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
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
