using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public enum CursorType
    {
        Default,
        Pointer,
        Grab,
        Grabbing,
        ZoomIn,
        ZoomOut,
        Loading
    }

    public class CursorManager : Singleton<CursorManager>
    {
        [Header("Cursor Textures")]
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D pointerCursor;
        [SerializeField] private Texture2D grabCursor;
        [SerializeField] private Texture2D grabbingCursor;
        [SerializeField] private Texture2D zoomInCursor;
        [SerializeField] private Texture2D zoomOutCursor;
        [SerializeField] private Texture2D loadingCursor;

        [Header("Hotspots")]
        [SerializeField] private Vector2 defaultHotspot = Vector2.zero;
        [SerializeField] private Vector2 pointerHotspot = new Vector2(7, 0);
        [SerializeField] private Vector2 grabHotspot = new Vector2(8, 8);

        private CursorType currentType = CursorType.Default;

        public void SetCursor(CursorType type)
        {
            if (currentType == type) return;
            currentType = type;

            switch (type)
            {
                case CursorType.Default:
                    Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
                    break;
                case CursorType.Pointer:
                    Cursor.SetCursor(pointerCursor, pointerHotspot, CursorMode.Auto);
                    break;
                case CursorType.Grab:
                    Cursor.SetCursor(grabCursor, grabHotspot, CursorMode.Auto);
                    break;
                case CursorType.Grabbing:
                    Cursor.SetCursor(grabbingCursor, grabHotspot, CursorMode.Auto);
                    break;
                case CursorType.ZoomIn:
                    Cursor.SetCursor(zoomInCursor, grabHotspot, CursorMode.Auto);
                    break;
                case CursorType.ZoomOut:
                    Cursor.SetCursor(zoomOutCursor, grabHotspot, CursorMode.Auto);
                    break;
                case CursorType.Loading:
                    Cursor.SetCursor(loadingCursor, grabHotspot, CursorMode.Auto);
                    break;
            }
        }

        public void ResetCursor()
        {
            SetCursor(CursorType.Default);
        }
    }
}
