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
        private bool isSelected = false;
        private SelectionVisualMode currentSelectionMode = SelectionVisualMode.FillPulse;

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
            if (isSelected) return;
            // isHovered = true;
            materialController.SetColor(hoverColor);
        }

        public void OnHoverExit()
        {
            if (isSelected) return;
            // isHovered = false;
            materialController.ResetProperties();
        }

        public void OnSelect(SelectionVisualMode visualMode = SelectionVisualMode.FillPulse, Color? overrideColor = null)
        {
            isSelected = true;
            currentSelectionMode = visualMode;

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
                materialController.SetColor(overrideColor ?? hotspotGroupColor);
                return;
            }

            Renderer[] renderers = materialController != null ? materialController.Renderers : null;
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

        public void OnDeselect()
        {
            isSelected = false;
            StopPulse();
            materialController.ResetProperties();
            
            // Optional: Disable emission if it wasn't on originally, 
            // but for safety we often leave it enabled with black color 
            // to avoid shader variant switching cost
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
