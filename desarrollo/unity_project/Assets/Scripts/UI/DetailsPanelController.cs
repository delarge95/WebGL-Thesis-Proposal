using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Data;

namespace WebGL.UI
{
    public class DetailsPanelController : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument document;

        private VisualElement root;
        private VisualElement panel;
        private Label titleLabel;
        private Label descriptionLabel;
        private Label specsLabel; // Assuming we add this to UXML later or reuse Description
        private VisualElement iconElement;

        private void OnEnable()
        {
            if (document == null) document = GetComponent<UIDocument>();
            if (document != null)
            {
                root = document.rootVisualElement;
                panel = root.Q<VisualElement>("DetailsPanel");
                
                // Assuming these names match UXML or will be updated
                titleLabel = panel.Q<Label>(className: "title-label"); 
                descriptionLabel = panel.Q<Label>("DescriptionLabel");
            }
            
            WebGL.Core.Events.EventBus.Subscribe<WebGL.Core.Events.PartSelectedEvent>(OnPartSelected);
        }

        private void OnDisable()
        {
            WebGL.Core.Events.EventBus.Unsubscribe<WebGL.Core.Events.PartSelectedEvent>(OnPartSelected);
        }

        private void OnPartSelected(WebGL.Core.Events.PartSelectedEvent evt)
        {
            UpdateDetails(evt.PartData);
        }

        public void UpdateDetails(DronePartData data)
        {
            if (panel == null) return;

            if (data != null)
            {
                panel.style.display = DisplayStyle.Flex;
                
                if (titleLabel != null) titleLabel.text = data.partName;
                if (descriptionLabel != null) descriptionLabel.text = data.description;
                
                // Future: Update icon and other specs
            }
            else
            {
                // Hide or show default state
                panel.style.display = DisplayStyle.None;
            }
        }
    }
}
