using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;

namespace Editor
{
    public class ObsidianBuildTelemetryPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            try
            {
                var dataParent = Directory.GetParent(Application.dataPath);
                string projectRoot = dataParent != null ? dataParent.FullName : Application.dataPath;
                string vaultRoot = Path.GetFullPath(Path.Combine(projectRoot, "..", ".."));
                string outputDir = Path.Combine(vaultRoot, "Telemetria", "Unity_Builds");
                Directory.CreateDirectory(outputDir);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"BuildLog_{timestamp}.md";
                string outputPath = Path.Combine(outputDir, fileName);

                bool success = report.summary.result == BuildResult.Succeeded;
                string status = success ? "SUCCESS" : "FAILED";
                double buildTimeMs = report.summary.totalTime.TotalMilliseconds;
                ulong totalSizeBytes = report.summary.totalSize;

                SceneGeometryStats geometry = CollectSceneGeometryStats();

                // Heuristica simple: si la compilacion es WebGL, asumimos Brotli activo por default de pipeline.
                bool brotliCompression = report.summary.platform == BuildTarget.WebGL;

                var sb = new StringBuilder();
                sb.AppendLine("---");
                sb.AppendLine("tipo: telemetry_build");
                sb.AppendLine("estado: activo");
                sb.AppendLine($"status: {status}");
                sb.AppendLine($"platform: {report.summary.platform}");
                sb.AppendLine($"build_time_ms: {Math.Round(buildTimeMs, 0)}");
                sb.AppendLine($"total_size_bytes: {totalSizeBytes}");
                sb.AppendLine($"total_vertices: {geometry.TotalVertices}");
                sb.AppendLine($"total_triangles: {geometry.TotalTriangles}");
                sb.AppendLine($"mesh_count: {geometry.MeshCount}");
                sb.AppendLine($"renderer_count: {geometry.RendererCount}");
                sb.AppendLine($"lod_group_count: {geometry.LodGroupCount}");
                sb.AppendLine($"generated_lod_renderer_count: {geometry.GeneratedLodRendererCount}");
                sb.AppendLine($"brotli_compression: {(brotliCompression ? "true" : "false")} ");
                sb.AppendLine($"output_path: \"{report.summary.outputPath.Replace("\\", "/")}\"");
                sb.AppendLine("---");
                sb.AppendLine();
                sb.AppendLine("# Build Telemetry");
                sb.AppendLine();
                sb.AppendLine($"- Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"- Resultado: {status}");
                sb.AppendLine($"- Plataforma: {report.summary.platform}");
                sb.AppendLine($"- Tiempo (ms): {Math.Round(buildTimeMs, 0)}");
                sb.AppendLine($"- Tamano (bytes): {totalSizeBytes}");
                sb.AppendLine($"- Vertices (escena activa): {geometry.TotalVertices}");
                sb.AppendLine($"- Triangulos (escena activa): {geometry.TotalTriangles}");
                sb.AppendLine($"- Mallas unicas: {geometry.MeshCount}");
                sb.AppendLine($"- Renderers: {geometry.RendererCount}");
                sb.AppendLine($"- LODGroups: {geometry.LodGroupCount}");
                sb.AppendLine($"- Renderers LOD generados: {geometry.GeneratedLodRendererCount}");
                sb.AppendLine($"- Brotli: {(brotliCompression ? "Activo" : "Inactivo")} ");

                File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
                Debug.Log($"[ObsidianTelemetry] Build log generado en: {outputPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ObsidianTelemetry] Error generando telemetry markdown: {ex.Message}");
            }
        }

        private static SceneGeometryStats CollectSceneGeometryStats()
        {
            SceneGeometryStats stats = new SceneGeometryStats();
            HashSet<Mesh> meshes = new HashSet<Mesh>();

            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] roots = activeScene.IsValid() ? activeScene.GetRootGameObjects() : Array.Empty<GameObject>();
            foreach (GameObject root in roots)
            {
                if (root == null)
                {
                    continue;
                }

                Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
                stats.RendererCount += renderers.Length;
                stats.LodGroupCount += root.GetComponentsInChildren<LODGroup>(true).Length;
                stats.GeneratedLodRendererCount += root.GetComponentsInChildren<LodGeneratedRenderer>(true).Length;

                MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : null;
                    if (mesh != null && meshes.Add(mesh))
                    {
                        stats.MeshCount++;
                        stats.TotalVertices += mesh.vertexCount;
                        stats.TotalTriangles += CountTriangles(mesh);
                    }
                }
            }

            return stats;
        }

        private static int CountTriangles(Mesh mesh)
        {
            if (mesh == null)
            {
                return 0;
            }

#if UNITY_2019_1_OR_NEWER
            int triangles = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                // GetIndexCount devuelve uint en algunas versiones; convertir de forma segura a int
                uint indexCountU = mesh.GetIndexCount(i);
                int indexCount = indexCountU > int.MaxValue ? int.MaxValue : (int)indexCountU;
                // No asumimos topología específica; aproximamos como indexCount/3
                triangles += indexCount / 3;
            }
            return triangles;
#else
            // Fallback para versiones antiguas sin GetIndexCount
            try
            {
                return mesh.triangles != null ? mesh.triangles.Length / 3 : 0;
            }
            catch
            {
                return 0;
            }
#endif
        }

        private struct SceneGeometryStats
        {
            public int MeshCount;
            public int RendererCount;
            public int LodGroupCount;
            public int GeneratedLodRendererCount;
            public int TotalVertices;
            public int TotalTriangles;
        }
    }
}
