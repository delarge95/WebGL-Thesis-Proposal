using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WebGL.Editor.Fixes
{
    [Serializable]
    public sealed class MigrationBuildArtifactInfo
    {
        public string artifactType;
        public string relativePath;
        public long bytes;
        public float sizeMb;
        public string modifiedAtUtc;
    }

    [Serializable]
    public sealed class MigrationBuildSnapshot
    {
        public string generatedAtUtc;
        public string unityVersion;
        public string buildRoot;
        public MigrationBuildArtifactInfo dataArtifact;
        public MigrationBuildArtifactInfo wasmArtifact;
        public MigrationBuildArtifactInfo frameworkArtifact;
        public long totalBytes;
        public float totalSizeMb;
    }

    public static class MigrationMetricsReporter
    {
        [MenuItem("WebGL/Migration/Write Build and Config Snapshots", priority = 30)]
        public static void WriteBuildAndConfigSnapshots()
        {
            WebGLDiagnostics.Run();

            string snapshotsRoot = GetSnapshotsRoot();
            Directory.CreateDirectory(snapshotsRoot);

            MigrationBuildSnapshot buildSnapshot = BuildSnapshot();
            string buildSnapshotPath = Path.Combine(snapshotsRoot, "latest_build_snapshot.json");
            File.WriteAllText(buildSnapshotPath, JsonUtility.ToJson(buildSnapshot, true));

            string configSnapshotPath = Path.Combine(snapshotsRoot, "latest_config_snapshot.md");
            File.WriteAllText(configSnapshotPath, BuildConfigSnapshotMarkdown());

            AssetDatabase.Refresh();
            Debug.Log($"{WebGLBuildFixer.LOG_PREFIX} Wrote migration snapshots to: {snapshotsRoot}");
        }

        [MenuItem("WebGL/Migration/Open Metrics Folder", priority = 31)]
        public static void OpenMetricsFolder()
        {
            string reportsRoot = GetReportsRoot();
            Directory.CreateDirectory(reportsRoot);
            EditorUtility.RevealInFinder(reportsRoot);
        }

        private static MigrationBuildSnapshot BuildSnapshot()
        {
            string workspaceRoot = GetWorkspaceRoot();
            string buildRoot = Path.Combine(workspaceRoot, "docs", "Build");

            MigrationBuildArtifactInfo dataArtifact = FindNewestArtifact(buildRoot, workspaceRoot, ".data.br", ".data");
            MigrationBuildArtifactInfo wasmArtifact = FindNewestArtifact(buildRoot, workspaceRoot, ".wasm.br", ".wasm");
            MigrationBuildArtifactInfo frameworkArtifact = FindNewestArtifact(buildRoot, workspaceRoot, ".framework.js.br", ".framework.js");

            long totalBytes = 0L;
            if (dataArtifact != null)
            {
                totalBytes += dataArtifact.bytes;
            }

            if (wasmArtifact != null)
            {
                totalBytes += wasmArtifact.bytes;
            }

            if (frameworkArtifact != null)
            {
                totalBytes += frameworkArtifact.bytes;
            }

            return new MigrationBuildSnapshot
            {
                generatedAtUtc = DateTime.UtcNow.ToString("o"),
                unityVersion = Application.unityVersion,
                buildRoot = MakeRelativePath(workspaceRoot, buildRoot),
                dataArtifact = dataArtifact,
                wasmArtifact = wasmArtifact,
                frameworkArtifact = frameworkArtifact,
                totalBytes = totalBytes,
                totalSizeMb = BytesToMb(totalBytes)
            };
        }

        private static MigrationBuildArtifactInfo FindNewestArtifact(string buildRoot, string workspaceRoot, params string[] suffixes)
        {
            if (!Directory.Exists(buildRoot))
            {
                return null;
            }

            FileInfo newestFile = null;
            foreach (string filePath in Directory.EnumerateFiles(buildRoot, "*", SearchOption.AllDirectories))
            {
                bool matchesSuffix = false;
                for (int i = 0; i < suffixes.Length; i++)
                {
                    if (filePath.EndsWith(suffixes[i], StringComparison.OrdinalIgnoreCase))
                    {
                        matchesSuffix = true;
                        break;
                    }
                }

                if (!matchesSuffix)
                {
                    continue;
                }

                FileInfo candidate = new FileInfo(filePath);
                if (newestFile == null || candidate.LastWriteTimeUtc > newestFile.LastWriteTimeUtc)
                {
                    newestFile = candidate;
                }
            }

            if (newestFile == null)
            {
                return null;
            }

            return new MigrationBuildArtifactInfo
            {
                artifactType = newestFile.Extension,
                relativePath = MakeRelativePath(workspaceRoot, newestFile.FullName),
                bytes = newestFile.Length,
                sizeMb = BytesToMb(newestFile.Length),
                modifiedAtUtc = newestFile.LastWriteTimeUtc.ToString("o")
            };
        }

        private static string BuildConfigSnapshotMarkdown()
        {
            string workspaceRoot = GetWorkspaceRoot();
            string projectRoot = GetProjectRoot();
            string projectVersionPath = Path.Combine(projectRoot, "ProjectSettings", "ProjectVersion.txt");
            string projectSettingsPath = Path.Combine(projectRoot, "ProjectSettings", "ProjectSettings.asset");

            string projectVersionText = File.Exists(projectVersionPath)
                ? File.ReadAllText(projectVersionPath).Trim()
                : "ProjectVersion.txt not found";

            string projectSettingsText = File.Exists(projectSettingsPath)
                ? File.ReadAllText(projectSettingsPath)
                : string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Migration Config Snapshot");
            builder.AppendLine();
            builder.AppendLine($"Generated at: {DateTime.UtcNow:o}");
            builder.AppendLine($"Unity version: {Application.unityVersion}");
            builder.AppendLine($"Project root: `{MakeRelativePath(workspaceRoot, projectRoot)}`");
            builder.AppendLine();
            builder.AppendLine("## Project Version");
            builder.AppendLine("```text");
            builder.AppendLine(projectVersionText);
            builder.AppendLine("```");
            builder.AppendLine();
            builder.AppendLine("## WebGL Settings");
            AppendSettingLine(builder, projectSettingsText, "webGLLinkerTarget");
            AppendSettingLine(builder, projectSettingsText, "webGLThreadsSupport");
            AppendSettingLine(builder, projectSettingsText, "webGLEnableWebGPU");
            AppendSettingLine(builder, projectSettingsText, "webGLDataCaching");
            AppendSettingLine(builder, projectSettingsText, "webGLCompressionFormat");
            AppendSettingLine(builder, projectSettingsText, "webGLMaximumMemorySize");
            builder.AppendLine();
            builder.AppendLine("## Render Pipelines");
            builder.AppendLine($"- GraphicsSettings.defaultRenderPipeline: `{GetAssetPathOrNone(GraphicsSettings.defaultRenderPipeline)}`");
            builder.AppendLine($"- QualitySettings.renderPipeline: `{GetAssetPathOrNone(QualitySettings.renderPipeline)}`");
            builder.AppendLine($"- Quality level: `{QualitySettings.names[QualitySettings.GetQualityLevel()]}`");
            builder.AppendLine();
            builder.AppendLine("## Enabled Build Scenes");
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    builder.AppendLine($"- `{scene.path}`");
                }
            }

            return builder.ToString();
        }

        private static void AppendSettingLine(StringBuilder builder, string projectSettingsText, string key)
        {
            string value = FindProjectSettingValue(projectSettingsText, key);
            builder.AppendLine($"- {key}: `{value}`");
        }

        private static string FindProjectSettingValue(string projectSettingsText, string key)
        {
            if (string.IsNullOrEmpty(projectSettingsText))
            {
                return "<missing>";
            }

            string[] lines = projectSettingsText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            string prefix = $"{key}:";
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (trimmed.StartsWith(prefix, StringComparison.Ordinal))
                {
                    return trimmed.Substring(prefix.Length).Trim();
                }
            }

            return "<missing>";
        }

        private static string GetAssetPathOrNone(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            return string.IsNullOrWhiteSpace(assetPath) ? "<none>" : assetPath;
        }

        private static string GetProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        private static string GetWorkspaceRoot()
        {
            return Path.GetFullPath(Path.Combine(GetProjectRoot(), "..", ".."));
        }

        private static string GetReportsRoot()
        {
            return Path.Combine(GetProjectRoot(), "Reports", "MigrationMetrics");
        }

        private static string GetSnapshotsRoot()
        {
            return Path.Combine(GetReportsRoot(), "_snapshots");
        }

        private static float BytesToMb(long bytes)
        {
            return bytes / (1024f * 1024f);
        }

        private static string MakeRelativePath(string root, string path)
        {
            try
            {
                return Path.GetRelativePath(root, path).Replace('\\', '/');
            }
            catch
            {
                return path.Replace('\\', '/');
            }
        }
    }
}
