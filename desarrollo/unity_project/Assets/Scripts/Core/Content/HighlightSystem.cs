using System.Collections;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Content
{
    public class HighlightSystem : MonoBehaviour
    {
        public enum SelectionVisualMode
        {
            FillPulse,
            SoftTint,
            HotspotGroupTint
        }

        [Header("Highlight Settings")]
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.8f, 1f, 0.5f);
        [SerializeField] private Color parentSelectedColor = new Color(0.2f, 0.8f, 1f, 0.18f);
        [SerializeField] private Color hotspotGroupColor = new Color(1.0f, 0.78f, 0.24f, 0.24f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.2f;

        private MaterialController materialController;
        private Coroutine pulseCoroutine;
        private bool isHovered = false;
        private bool isSelected = false;
        private SelectionVisualMode currentSelectionMode = SelectionVisualMode.FillPulse;
        private Color? currentOverrideColor;

        private void Awake()
        {
            materialController = GetComponent<MaterialController>();
            if (materialController == null)
            {
                materialController = gameObject.AddComponent<MaterialController>();
            }
        }

        public void OnHoverEnter()
        {
            isHovered = true;
            if (isSelected) return;
            materialController.SetColor(hoverColor);
        }

        public void OnHoverExit()
        {
            isHovered = false;
            if (isSelected) return;
            materialController.ResetProperties();
        }

        public void OnSelect(SelectionVisualMode visualMode = SelectionVisualMode.FillPulse, Color? overrideColor = null)
        {
            isSelected = true;
            currentSelectionMode = visualMode;
            currentOverrideColor = overrideColor;
            ApplySelectionVisual();
        }

        public void OnDeselect()
        {
            isSelected = false;
            currentOverrideColor = null;
            StopPulse();
            if (isHovered)
            {
                materialController.SetColor(hoverColor);
            }
            else
            {
                materialController.ResetProperties();
            }
            
            // Optional: Disable emission if it wasn't on originally, 
            // but for safety we often leave it enabled with black color 
            // to avoid shader variant switching cost
        }

        public void RefreshVisualTargets()
        {
            if (materialController == null)
            {
                materialController = GetComponent<MaterialController>();
                if (materialController == null)
                {
                    materialController = gameObject.AddComponent<MaterialController>();
                }
            }

            materialController.RefreshRenderers();

            if (isSelected)
            {
                ApplySelectionVisual();
                return;
            }

            if (isHovered)
            {
                materialController.SetColor(hoverColor);
                return;
            }

            materialController.ResetProperties();
        }

        private void ApplySelectionVisual()
        {
            if (materialController == null)
            {
                return;
            }

            if (currentSelectionMode == SelectionVisualMode.SoftTint)
            {
                StopPulse();
                materialController.ResetProperties();
                materialController.SetColor(parentSelectedColor);
                return;
            }

            if (currentSelectionMode == SelectionVisualMode.HotspotGroupTint)
            {
                StopPulse();
                materialController.ResetProperties();
                materialController.SetColor(currentOverrideColor ?? hotspotGroupColor);
                return;
            }

            Renderer[] renderers = materialController.Renderers;
            if (renderers != null)
            {
                foreach (Renderer renderer in renderers)
                {
                    if (renderer == null) continue;
                    foreach (Material material in renderer.materials)
                    {
                        if (material != null)
                        {
                            material.EnableKeyword("_EMISSION");
                        }
                    }
                }
            }

            materialController.SetColor(selectedColor);
            StartPulse();
        }

        private void StartPulse()
        {
            StopPulse();
            if (!gameObject.activeInHierarchy) return;
            pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        private void StopPulse()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
        }

        private IEnumerator PulseRoutine()
        {
            if (currentSelectionMode == SelectionVisualMode.SoftTint
                || currentSelectionMode == SelectionVisualMode.HotspotGroupTint)
            {
                yield break;
            }

            Renderer[] renderers = materialController != null ? materialController.Renderers : null;
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            int emissionColorId = Shader.PropertyToID("_EmissionColor");

            while (isSelected)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
                Color pulsedColor = selectedColor * pulse;

                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        if (renderer == null) continue;

                        renderer.GetPropertyBlock(block);
                        block.SetColor(emissionColorId, pulsedColor * 2f);
                        block.SetColor("_BaseColor", pulsedColor);
                        block.SetColor("_Color", pulsedColor);
                        renderer.SetPropertyBlock(block);
                    }
                }

                yield return null;
            }

            materialController.ResetProperties();
        }
    }
}
