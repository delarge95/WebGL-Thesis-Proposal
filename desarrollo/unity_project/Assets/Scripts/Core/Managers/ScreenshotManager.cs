using System;
using System.IO;
using UnityEngine;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    public class ScreenshotManager : Singleton<ScreenshotManager>
    {
        [Header("Settings")]
        [SerializeField] private int superSize = 2; // 2x resolution
        [SerializeField] private string screenshotFolder = "Screenshots";
        [SerializeField] private KeyCode screenshotKey = KeyCode.F12;

        private bool isCapturing = false;

        private void Update()
        {
            if (Input.GetKeyDown(screenshotKey))
            {
                CaptureScreenshot();
            }
        }

        public void CaptureScreenshot(Action<string> onComplete = null)
        {
            if (isCapturing) return;
            StartCoroutine(CaptureRoutine(onComplete));
        }

        private System.Collections.IEnumerator CaptureRoutine(Action<string> onComplete)
        {
            isCapturing = true;
            
            // Wait for end of frame to capture rendered content
            yield return new WaitForEndOfFrame();

            // Create texture
            int width = Screen.width * superSize;
            int height = Screen.height * superSize;
            RenderTexture rt = new RenderTexture(width, height, 24);
            
            Camera cam = Camera.main;
            RenderTexture prevRT = cam.targetTexture;
            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = prevRT;

            RenderTexture.active = rt;
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();
            RenderTexture.active = null;

            // Generate filename
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"Screenshot_{timestamp}.png";

            // In WebGL, we can't save to disk directly, so we use base64 download
            #if UNITY_WEBGL && !UNITY_EDITOR
            byte[] bytes = screenshot.EncodeToPNG();
            string base64 = Convert.ToBase64String(bytes);
            // This would call a JavaScript function to trigger download
            Debug.Log($"[ScreenshotManager] WebGL screenshot captured. Size: {bytes.Length} bytes");
            onComplete?.Invoke("WebGL_Screenshot");
            #else
            // In Editor/Standalone, save to disk
            string folderPath = Path.Combine(Application.persistentDataPath, screenshotFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fullPath = Path.Combine(folderPath, filename);
            
            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(fullPath, bytes);
            
            Debug.Log($"[ScreenshotManager] Screenshot saved: {fullPath}");
            onComplete?.Invoke(fullPath);
            #endif

            // Cleanup
            Destroy(screenshot);
            Destroy(rt);
            
            // Notify user
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Screenshot captured!");
            }

            isCapturing = false;
        }
    }
}
