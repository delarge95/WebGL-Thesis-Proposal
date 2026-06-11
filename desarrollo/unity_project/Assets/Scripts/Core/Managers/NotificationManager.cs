using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class NotificationManager : PersistentSingleton<NotificationManager>
    {
        [SerializeField] private UIDocument uiDocument;
        private VisualElement root;
        private VisualElement notificationContainer;
        private Label notificationLabel;
        private Coroutine notificationRoutine;

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
            }
        }

        private void CreateNotificationElement()
        {
            notificationContainer = new VisualElement();
            notificationContainer.name = "NotificationToast";
            notificationContainer.style.position = Position.Absolute;
            notificationContainer.style.bottom = 50;
            notificationContainer.style.alignSelf = Align.Center;
            notificationContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            notificationContainer.style.paddingTop = 10;
            notificationContainer.style.paddingBottom = 10;
            notificationContainer.style.paddingLeft = 20;
            notificationContainer.style.paddingRight = 20;
            notificationContainer.style.borderTopLeftRadius = 20;
            notificationContainer.style.borderTopRightRadius = 20;
            notificationContainer.style.borderBottomLeftRadius = 20;
            notificationContainer.style.borderBottomRightRadius = 20;
            notificationContainer.style.opacity = 0;
            notificationContainer.style.display = DisplayStyle.None;
            notificationContainer.pickingMode = PickingMode.Ignore;

            notificationLabel = new Label();
            notificationLabel.name = "NotificationToastLabel";
            notificationLabel.style.color = Color.white;
            notificationLabel.style.fontSize = 16;
            notificationLabel.pickingMode = PickingMode.Ignore;
            notificationContainer.Add(notificationLabel);
        }

        public void ShowNotification(string message, float duration = 2.0f)
        {
            EnsureRoot();
            if (root == null) return;

            EnsureNotificationElement();

            if (notificationRoutine != null)
            {
                StopCoroutine(notificationRoutine);
                notificationRoutine = null;
            }

            notificationRoutine = StartCoroutine(ShowNotificationRoutine(message, duration));
        }

        private IEnumerator ShowNotificationRoutine(string message, float duration)
        {
            notificationLabel.text = message;
            
            // Fade In
            float timer = 0;
            while (timer < 0.2f)
            {
                timer += Time.deltaTime;
                notificationContainer.style.opacity = Mathf.Lerp(0, 1, timer / 0.2f);
                yield return null;
            }
            notificationContainer.style.opacity = 1;

            yield return new WaitForSeconds(duration);

            // Fade Out
            timer = 0;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                notificationContainer.style.opacity = Mathf.Lerp(1, 0, timer / 0.5f);
                yield return null;
            }
            HideNotificationElement();
            notificationRoutine = null;
        }

        private void EnsureRoot()
        {
            if (root != null)
            {
                return;
            }

            if (uiDocument == null)
            {
                uiDocument = FindAnyObjectByType<UIDocument>();
            }

            root = uiDocument != null ? uiDocument.rootVisualElement : null;
        }

        private void EnsureNotificationElement()
        {
            if (notificationContainer == null)
            {
                CreateNotificationElement();
            }

            if (notificationContainer.parent == null && root != null)
            {
                root.Add(notificationContainer);
            }

            notificationContainer.style.display = DisplayStyle.Flex;
            notificationContainer.style.opacity = 0;
        }

        private void HideNotificationElement()
        {
            if (notificationContainer == null)
            {
                return;
            }

            notificationContainer.style.opacity = 0;
            notificationContainer.style.display = DisplayStyle.None;

            if (notificationContainer.parent != null)
            {
                notificationContainer.RemoveFromHierarchy();
            }
        }
    }
}
