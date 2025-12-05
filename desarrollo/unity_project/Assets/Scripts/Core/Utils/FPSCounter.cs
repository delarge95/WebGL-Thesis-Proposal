using UnityEngine;

namespace WebGL.Core.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        private float deltaTime = 0.0f;
        private GUIStyle style = new GUIStyle();

        private void Awake()
        {
            style.fontSize = 20;
            style.normal.textColor = Color.green;
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

            if (fps < 30) style.normal.textColor = Color.red;
            else if (fps < 50) style.normal.textColor = Color.yellow;
            else style.normal.textColor = Color.green;

            GUI.Label(new Rect(10, 10, 200, 50), text, style);
        }
    }
}
