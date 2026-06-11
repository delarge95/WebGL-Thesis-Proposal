using System.Runtime.InteropServices;
using UnityEngine;

namespace WebGL.UI
{
    public class WebGLBrowserBackBridge : MonoBehaviour
    {
        private const string BridgeObjectName = "X500V2BrowserBackBridge";

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void X500V2ReportBrowserBackResult(int handled);
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBridge()
        {
            if (GameObject.Find(BridgeObjectName) != null)
            {
                return;
            }

            GameObject bridge = new GameObject(BridgeObjectName);
            DontDestroyOnLoad(bridge);
            bridge.AddComponent<WebGLBrowserBackBridge>();
        }

        public void HandleBrowserBackFromPage()
        {
            bool handled = UIManager.Instance != null && UIManager.Instance.HandleBrowserBackNavigation();

#if UNITY_WEBGL && !UNITY_EDITOR
            X500V2ReportBrowserBackResult(handled ? 1 : 0);
#endif
        }
    }
}
