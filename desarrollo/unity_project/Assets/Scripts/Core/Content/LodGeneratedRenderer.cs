using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public sealed class LodGeneratedRenderer : MonoBehaviour
    {
        [SerializeField] private string sourceRendererPath = string.Empty;
        [SerializeField] private int lodLevel;
        [SerializeField] private int sourceTriangleCount;
        [SerializeField] private int generatedTriangleCount;

        public string SourceRendererPath => sourceRendererPath;
        public int LodLevel => lodLevel;
        public int SourceTriangleCount => sourceTriangleCount;
        public int GeneratedTriangleCount => generatedTriangleCount;

        public void Configure(string sourcePath, int level, int sourceTriangles, int generatedTriangles)
        {
            sourceRendererPath = sourcePath ?? string.Empty;
            lodLevel = Mathf.Max(1, level);
            sourceTriangleCount = Mathf.Max(0, sourceTriangles);
            generatedTriangleCount = Mathf.Max(0, generatedTriangles);
        }
    }
}
