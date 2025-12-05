using UnityEngine;

namespace WebGL.Core.Content
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialController : MonoBehaviour
    {
        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private static readonly int ColorPropertyId = Shader.PropertyToID("_BaseColor"); // URP Lit standard property

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propBlock = new MaterialPropertyBlock();
        }

        public void SetColor(Color color)
        {
            // Get current properties
            _renderer.GetPropertyBlock(_propBlock);
            
            // Set new value
            _propBlock.SetColor(ColorPropertyId, color);
            
            // Apply back to renderer
            _renderer.SetPropertyBlock(_propBlock);
        }

        public void ResetProperties()
        {
             // Clearing the block removes overrides, reverting to material defaults
            _renderer.SetPropertyBlock(null);
        }
    }
}
