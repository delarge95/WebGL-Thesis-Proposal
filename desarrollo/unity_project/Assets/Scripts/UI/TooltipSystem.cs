using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.UI
{
    public class TooltipSystem : PersistentSingleton<TooltipSystem>
    {
        [SerializeField] private UIDocument uiDocument;
        private VisualElement root;
        private Label tooltipLabel;
        private VisualElement tooltipContainer;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
            if (uiDocument != null)
            {
                root = uiDocument.rootVisualElement;
                CreateTooltipElement();
            }
        }

        private void CreateTooltipElement()
        {
            // Create tooltip UI programmatically if it doesn't exist
            tooltipContainer = new VisualElement();
            tooltipContainer.style.position = Position.Absolute;
            tooltipContainer.style.backgroundColor = new Color(0, 0, 0, 0.8f);
            tooltipContainer.style.paddingTop = 5;
            tooltipContainer.style.paddingBottom = 5;
            tooltipContainer.style.paddingLeft = 10;
            tooltipContainer.style.paddingRight = 10;
            tooltipContainer.style.borderTopLeftRadius = 5; tooltipContainer.style.borderTopRightRadius = 5; tooltipContainer.style.borderBottomLeftRadius = 5; tooltipContainer.style.borderBottomRightRadius = 5;
            tooltipContainer.style.display = DisplayStyle.None;

            tooltipLabel = new Label();
            tooltipLabel.style.color = Color.white;
            tooltipContainer.Add(tooltipLabel);

            root.Add(tooltipContainer);
        }

        private void Update()
        {
            if (tooltipContainer != null && tooltipContainer.style.display == DisplayStyle.Flex)
            {
                Vector2 mousePos = Input.mousePosition;
                // Convert screen space to panel space
                // Note: This is a simplified conversion. In a real project, use RuntimePanelUtils.ScreenToPanel
                float panelHeight = root.layout.height;
                tooltipContainer.style.left = mousePos.x + 15;
                tooltipContainer.style.top = (panelHeight - mousePos.y) + 15;
            }
        }

        public void Show(string text)
        {
            if (tooltipContainer != null)
            {
                tooltipLabel.text = text;
                tooltipContainer.style.display = DisplayStyle.Flex;
            }
        }

        public void Hide()
        {
            if (tooltipContainer != null)
            {
                tooltipContainer.style.display = DisplayStyle.None;
            }
        }
    }
}
