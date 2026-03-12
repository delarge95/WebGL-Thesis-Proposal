using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;

namespace WebGL.Core.Thermal
{
    public enum ThermalSurfacePattern
    {
        Automatic,
        Uniform,
        Radial,
        Axial
    }

    [DisallowMultipleComponent]
    public class ThermalSurfaceProfile : MonoBehaviour
    {
        [Header("Binding")]
        [SerializeField] private string partIdOverride;

        [Header("Visualization")]
        [SerializeField] private ThermalSurfacePattern pattern = ThermalSurfacePattern.Automatic;
        [SerializeField] private Vector3 hotspotLocal = Vector3.zero;
        [SerializeField] private Vector3 directionLocal = Vector3.zero;
        [SerializeField, Min(0f)] private float spread;
        [SerializeField, Range(0f, 1f)] private float edgeCooling = 0.22f;
        [SerializeField, Range(0f, 1f)] private float baseVariation = 0.18f;
        [SerializeField, Range(0.25f, 2f)] private float propagation = 1f;

        public string PartIdOverride => partIdOverride;
        public ThermalSurfacePattern Pattern => pattern;
        public Vector3 HotspotLocal => hotspotLocal;
        public Vector3 DirectionLocal => directionLocal;
        public float Spread => spread;
        public float EdgeCooling => edgeCooling;
        public float BaseVariation => baseVariation;
        public float Propagation => propagation;

        public string ResolvePartId(ExplodablePart part)
        {
            if (!string.IsNullOrWhiteSpace(partIdOverride))
            {
                return partIdOverride;
            }

            DronePartData data = part != null ? part.Data : null;
            if (data != null && !string.IsNullOrWhiteSpace(data.id))
            {
                return data.id;
            }

            return part != null ? part.gameObject.name : gameObject.name;
        }
    }
}