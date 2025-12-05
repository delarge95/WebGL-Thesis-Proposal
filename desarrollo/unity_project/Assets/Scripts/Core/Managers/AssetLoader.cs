using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WebGL.Core.Managers
{
    public class AssetLoader : MonoBehaviour
    {
        // In the future, replace this with Addressables or AssetBundles
        
        public void LoadAssetAsync(string assetName, UnityAction<GameObject> onComplete)
        {
            StartCoroutine(LoadRoutine(assetName, onComplete));
        }

        private IEnumerator LoadRoutine(string assetName, UnityAction<GameObject> onComplete)
        {
            Debug.Log($"[AssetLoader] Starting load for: {assetName}");
            
            // Simulate network/disk delay
            yield return new WaitForSeconds(1.0f);

            // For now, we just look for it in Resources as a fallback/stub
            // In production, this would be: Addressables.LoadAssetAsync<GameObject>(assetName);
            ResourceRequest request = Resources.LoadAsync<GameObject>(assetName);
            
            yield return request;

            if (request.asset != null)
            {
                Debug.Log($"[AssetLoader] Loaded: {assetName}");
                onComplete?.Invoke(request.asset as GameObject);
            }
            else
            {
                Debug.LogError($"[AssetLoader] Failed to load: {assetName}");
                onComplete?.Invoke(null);
            }
        }
    }
}
