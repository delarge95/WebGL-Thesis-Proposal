using System.Collections;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Content
{
    [RequireComponent(typeof(Renderer))]
    public class HighlightSystem : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.8f, 1f, 0.5f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.2f;

        private MaterialController materialController;
        private Color originalColor = Color.white;
        private Coroutine pulseCoroutine;
        private bool isSelected = false;
        private bool isHovered = false;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

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
            isHovered = true;
            materialController.SetColor(hoverColor);
        }

        public void OnHoverExit()
        {
            if (isSelected) return;
            isHovered = false;
            materialController.ResetProperties();
        }

        public void OnSelect()
        {
            isSelected = true;
            isHovered = false;
            materialController.SetColor(selectedColor);
            StartPulse();
        }

        public void OnDeselect()
        {
            isSelected = false;
            StopPulse();
            materialController.ResetProperties();
        }

        private void StartPulse()
        {
            StopPulse();
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
            Renderer rend = GetComponent<Renderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            while (isSelected)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
                Color pulsedColor = selectedColor * pulse;

                rend.GetPropertyBlock(block);
                block.SetColor("_BaseColor", pulsedColor);
                rend.SetPropertyBlock(block);

                yield return null;
            }

            // Reset when done
            materialController.ResetProperties();
        }
    }
}
