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
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.22f);
        [SerializeField] private Color selectedColor = new Color(0.42f, 0.82f, 1f, 0.16f);
        [SerializeField] private Color parentSelectedColor = new Color(0.36f, 0.76f, 1f, 0.12f);
        [SerializeField] private Color hotspotGroupColor = new Color(1.0f, 0.78f, 0.24f, 0.2f);

        private MaterialController materialController;
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
            if (isHovered)
            {
                materialController.SetColor(hoverColor);
            }
            else
            {
                materialController.ResetProperties();
            }
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
                materialController.ResetProperties();
                materialController.SetColor(parentSelectedColor);
                return;
            }

            if (currentSelectionMode == SelectionVisualMode.HotspotGroupTint)
            {
                materialController.ResetProperties();
                materialController.SetColor(currentOverrideColor ?? hotspotGroupColor);
                return;
            }

            materialController.ResetProperties();
            materialController.SetColor(selectedColor);
        }
    }
}
