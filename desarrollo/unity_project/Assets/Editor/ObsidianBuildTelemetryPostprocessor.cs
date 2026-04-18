using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ObsidianBuildTelemetryPostprocessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        try
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
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

            // Placeholder: implementar contador real de vertices segun escena/pipeline.
            int totalVertices = -1;

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
            sb.AppendLine($"total_vertices: {totalVertices}");
            sb.AppendLine($"brotli_compression: {(brotliCompression ? "true" : "false")}");
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
            sb.AppendLine($"- Vertices (estimado): {totalVertices}");
            sb.AppendLine($"- Brotli: {(brotliCompression ? "Activo" : "Inactivo")}");

            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[ObsidianTelemetry] Build log generado en: {outputPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ObsidianTelemetry] Error generando telemetry markdown: {ex.Message}");
        }
    }
}
