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
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

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

        // X-Ray Support
        private Material originalMaterial;

        public void ToggleXRay(bool enable, Material xrayMat)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();

            if (enable)
            {
                if (originalMaterial == null) originalMaterial = _renderer.sharedMaterial;
                _renderer.material = xrayMat; // Use instance for now to avoid leaking
            }
            else
            {
                if (originalMaterial != null)
                {
                    _renderer.material = originalMaterial;
                }
            }
        }
    }
}
