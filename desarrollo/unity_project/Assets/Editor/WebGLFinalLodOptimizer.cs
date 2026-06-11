using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;

public static class WebGLFinalLodOptimizer
{
    private const string ScenePath = "Assets/Scenes/MainScene_Final.unity";
    private const string DroneRootName = "x500v2_Drone";
    private const string GeneratedAssetFolder = "Assets/Generated/LOD";
    private const string GeneratedObjectName = "__LOD_GENERATED";
    private const string ReportPath = "Reports/final_lod_optimization_report.md";

    private const int MinimumSourceTriangles = 420;
    private const float Lod1TargetRatio = 0.68f;
    private const float Lod2TargetRatio = 0.42f;

    private static readonly string[] ExcludedNameTokens =
    {
        "screw", "bolt", "nut", "washer", "standoff", "spacer", "fastener",
        "socket cap", "cap nut", "m2", "m3", "rubber_grommet", "grommet",
        "runtime_proxy", "_lod", GeneratedObjectName.ToLowerInvariant()
    };

    private sealed class LodEntry
    {
        public string Path;
        public int SourceTriangles;
        public int Lod1Triangles;
        public int Lod2Triangles;
        public string Category;
    }

    private sealed class SkipEntry
    {
        public string Path;
        public string Reason;
    }

    [MenuItem("Tools/X500V2/Optimization/Apply Final LODs (Unity 6)")]
    public static void ApplyFinalLodsMenu()
    {
        ApplyFinalLods(showDialog: true);
    }

    public static void ApplyFinalLodsBatch()
    {
        ApplyFinalLods(showDialog: false);
    }

    public static void ApplyFinalLodsForBuild()
    {
        ApplyFinalLods(showDialog: false);
    }

    private static void ApplyFinalLods(bool showDialog)
    {
        List<LodEntry> created = new List<LodEntry>();
        List<SkipEntry> skipped = new List<SkipEntry>();

        if (Application.isBatchMode)
        {
            EditorSceneManager.SaveOpenScenes();
        }
        else if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            throw new InvalidOperationException("LOD optimization cancelled because open scene changes were not saved.");
        }

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject root = GameObject.Find(DroneRootName);
        if (root == null)
        {
            throw new InvalidOperationException($"Could not find drone root '{DroneRootName}' in {ScenePath}.");
        }

        ClearGeneratedLods(root);
        EnsureGeneratedAssetFolder();

        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (!TryCreateLodGroup(renderer, created, skipped))
            {
                continue;
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        WriteReport(created, skipped);

        string summary = $"LOD optimization complete. Created {created.Count} LODGroups. Skipped {skipped.Count} renderers.";
        Debug.Log($"[WebGLFinalLodOptimizer] {summary}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Final LOD Optimizer", summary, "OK");
        }
    }

    private static bool TryCreateLodGroup(MeshRenderer sourceRenderer, List<LodEntry> created, List<SkipEntry> skipped)
    {
        string path = GetHierarchyPath(sourceRenderer.transform);

        if (ShouldSkipRenderer(sourceRenderer, out string skipReason))
        {
            skipped.Add(new SkipEntry { Path = path, Reason = skipReason });
            return false;
        }

        MeshFilter sourceFilter = sourceRenderer.GetComponent<MeshFilter>();
        Mesh sourceMesh = sourceFilter != null ? sourceFilter.sharedMesh : null;
        if (sourceMesh == null)
        {
            skipped.Add(new SkipEntry { Path = path, Reason = "No MeshFilter/sharedMesh." });
            return false;
        }

        int sourceTriangles = CountTriangles(sourceMesh);
        if (sourceTriangles < MinimumSourceTriangles)
        {
            skipped.Add(new SkipEntry { Path = path, Reason = $"Below triangle threshold ({sourceTriangles})." });
            return false;
        }

        if (!sourceMesh.isReadable)
        {
            skipped.Add(new SkipEntry { Path = path, Reason = "Mesh is not readable; importer was not modified." });
            return false;
        }

        Mesh lod1Mesh = CreateSimplifiedMesh(sourceMesh, Lod1TargetRatio, $"{sourceMesh.name}_LOD1");
        Mesh lod2Mesh = CreateSimplifiedMesh(sourceMesh, Lod2TargetRatio, $"{sourceMesh.name}_LOD2");

        int lod1Triangles = CountTriangles(lod1Mesh);
        int lod2Triangles = CountTriangles(lod2Mesh);
        if (lod1Triangles >= sourceTriangles || lod2Triangles >= lod1Triangles)
        {
            UnityEngine.Object.DestroyImmediate(lod1Mesh);
            UnityEngine.Object.DestroyImmediate(lod2Mesh);
            skipped.Add(new SkipEntry { Path = path, Reason = "Simplification did not reduce geometry safely." });
            return false;
        }

        string hash = StableHash(path);
        string assetPath1 = $"{GeneratedAssetFolder}/{SanitizeAssetName(sourceRenderer.name)}_{hash}_LOD1.asset";
        string assetPath2 = $"{GeneratedAssetFolder}/{SanitizeAssetName(sourceRenderer.name)}_{hash}_LOD2.asset";
        AssetDatabase.CreateAsset(lod1Mesh, assetPath1);
        AssetDatabase.CreateAsset(lod2Mesh, assetPath2);

        Transform container = sourceRenderer.transform.Find(GeneratedObjectName);
        if (container == null)
        {
            GameObject containerObject = new GameObject(GeneratedObjectName);
            container = containerObject.transform;
            container.SetParent(sourceRenderer.transform, false);
            container.localPosition = Vector3.zero;
            container.localRotation = Quaternion.identity;
            container.localScale = Vector3.one;
        }

        MeshRenderer lod1Renderer = CreateLodChild(container, sourceRenderer, lod1Mesh, 1, sourceTriangles, lod1Triangles, path);
        MeshRenderer lod2Renderer = CreateLodChild(container, sourceRenderer, lod2Mesh, 2, sourceTriangles, lod2Triangles, path);

        LODGroup group = sourceRenderer.GetComponent<LODGroup>();
        if (group == null)
        {
            group = sourceRenderer.gameObject.AddComponent<LODGroup>();
        }

        group.fadeMode = LODFadeMode.CrossFade;
        group.animateCrossFading = false;

        LOD[] lods =
        {
            new LOD(0.55f, new Renderer[] { sourceRenderer }) { fadeTransitionWidth = 0.08f },
            new LOD(0.22f, new Renderer[] { lod1Renderer }) { fadeTransitionWidth = 0.08f },
            new LOD(0.025f, new Renderer[] { lod2Renderer }) { fadeTransitionWidth = 0.04f }
        };

        group.SetLODs(lods);
        group.RecalculateBounds();

        PartRenderCategory category = sourceRenderer.GetComponent<PartRenderCategory>();
        if (category == null)
        {
            category = sourceRenderer.GetComponentInParent<PartRenderCategory>();
        }

        created.Add(new LodEntry
        {
            Path = path,
            SourceTriangles = sourceTriangles,
            Lod1Triangles = lod1Triangles,
            Lod2Triangles = lod2Triangles,
            Category = category != null ? category.PrimaryCategory : "Uncategorized"
        });

        return true;
    }

    private static bool ShouldSkipRenderer(MeshRenderer renderer, out string reason)
    {
        reason = string.Empty;
        if (renderer.GetComponent<LodGeneratedRenderer>() != null ||
            renderer.GetComponentInParent<LodGeneratedRenderer>() != null)
        {
            reason = "Generated LOD renderer.";
            return true;
        }

        if (FastenerBuilder.IsFastenerDetailTransform(renderer.transform))
        {
            reason = "Runtime fastener detail renderer.";
            return true;
        }

        LODGroup existingGroup = renderer.GetComponent<LODGroup>();
        if (existingGroup != null && existingGroup.GetComponentInChildren<LodGeneratedRenderer>(true) == null)
        {
            reason = "Existing non-generated LODGroup preserved.";
            return true;
        }

        string normalizedPath = GetHierarchyPath(renderer.transform).ToLowerInvariant();
        for (int i = 0; i < ExcludedNameTokens.Length; i++)
        {
            if (normalizedPath.Contains(ExcludedNameTokens[i]))
            {
                reason = $"Excluded token '{ExcludedNameTokens[i]}'.";
                return true;
            }
        }

        return false;
    }

    private static MeshRenderer CreateLodChild(
        Transform container,
        MeshRenderer sourceRenderer,
        Mesh mesh,
        int level,
        int sourceTriangles,
        int generatedTriangles,
        string sourcePath)
    {
        GameObject child = new GameObject($"{sourceRenderer.name}_LOD{level}");
        Transform childTransform = child.transform;
        childTransform.SetParent(container, false);
        childTransform.localPosition = Vector3.zero;
        childTransform.localRotation = Quaternion.identity;
        childTransform.localScale = Vector3.one;
        child.layer = sourceRenderer.gameObject.layer;
        child.tag = sourceRenderer.gameObject.tag;

        MeshFilter filter = child.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        MeshRenderer renderer = child.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = sourceRenderer.sharedMaterials;
        renderer.shadowCastingMode = sourceRenderer.shadowCastingMode;
        renderer.receiveShadows = sourceRenderer.receiveShadows;
        renderer.lightProbeUsage = sourceRenderer.lightProbeUsage;
        renderer.reflectionProbeUsage = sourceRenderer.reflectionProbeUsage;
        renderer.motionVectorGenerationMode = sourceRenderer.motionVectorGenerationMode;

        PartRenderCategory sourceCategory = sourceRenderer.GetComponent<PartRenderCategory>();
        if (sourceCategory == null)
        {
            sourceCategory = sourceRenderer.GetComponentInParent<PartRenderCategory>();
        }

        if (sourceCategory != null)
        {
            PartRenderCategory childCategory = child.AddComponent<PartRenderCategory>();
            childCategory.Configure(
                sourceCategory.CanonicalPartId,
                sourceCategory.PrimaryCategory,
                sourceCategory.AuxiliaryCategory,
                sourceCategory.ThermalSourcePartId,
                sourceCategory.SubpieceId);
        }

        LodGeneratedRenderer marker = child.AddComponent<LodGeneratedRenderer>();
        marker.Configure(sourcePath, level, sourceTriangles, generatedTriangles);

        return renderer;
    }

    private static Mesh CreateSimplifiedMesh(Mesh source, float targetRatio, string meshName)
    {
        int sourceTriangles = CountTriangles(source);
        int targetTriangles = Mathf.Max(24, Mathf.RoundToInt(sourceTriangles * Mathf.Clamp01(targetRatio)));
        int minimumTriangles = Mathf.Max(12, Mathf.RoundToInt(sourceTriangles * 0.18f));

        Mesh best = null;
        int bestTriangles = int.MaxValue;
        int[] resolutions = targetRatio > 0.5f
            ? new[] { 80, 64, 48, 36, 28, 22, 18, 14, 10 }
            : new[] { 48, 36, 28, 22, 18, 14, 10, 8, 6 };

        for (int i = 0; i < resolutions.Length; i++)
        {
            Mesh candidate = BuildClusteredMesh(source, resolutions[i], meshName);
            int candidateTriangles = CountTriangles(candidate);
            bool candidateIsBetter = candidateTriangles < bestTriangles && candidateTriangles >= minimumTriangles;
            if (candidateIsBetter)
            {
                if (best != null)
                {
                    UnityEngine.Object.DestroyImmediate(best);
                }

                best = candidate;
                bestTriangles = candidateTriangles;
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(candidate);
            }

            if (bestTriangles <= targetTriangles && bestTriangles >= minimumTriangles)
            {
                break;
            }
        }

        if (best == null)
        {
            best = UnityEngine.Object.Instantiate(source);
            best.name = meshName;
        }

        best.name = meshName;
        return best;
    }

    private static Mesh BuildClusteredMesh(Mesh source, int resolution, string meshName)
    {
        Vector3[] sourceVertices = source.vertices;
        Vector3[] sourceNormals = source.normals;
        Vector2[] sourceUv = source.uv;
        Color[] sourceColors = source.colors;

        Bounds bounds = source.bounds;
        Vector3 min = bounds.min;
        Vector3 size = bounds.size;
        size.x = Mathf.Max(size.x, 0.0001f);
        size.y = Mathf.Max(size.y, 0.0001f);
        size.z = Mathf.Max(size.z, 0.0001f);

        Dictionary<Vector3Int, int> clusterToIndex = new Dictionary<Vector3Int, int>();
        int[] oldToNew = new int[sourceVertices.Length];
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < sourceVertices.Length; i++)
        {
            Vector3 normalized = new Vector3(
                (sourceVertices[i].x - min.x) / size.x,
                (sourceVertices[i].y - min.y) / size.y,
                (sourceVertices[i].z - min.z) / size.z);

            Vector3Int key = new Vector3Int(
                Mathf.Clamp(Mathf.FloorToInt(normalized.x * resolution), 0, resolution - 1),
                Mathf.Clamp(Mathf.FloorToInt(normalized.y * resolution), 0, resolution - 1),
                Mathf.Clamp(Mathf.FloorToInt(normalized.z * resolution), 0, resolution - 1));

            if (!clusterToIndex.TryGetValue(key, out int newIndex))
            {
                newIndex = vertices.Count;
                clusterToIndex.Add(key, newIndex);
                vertices.Add(sourceVertices[i]);
                if (sourceNormals != null && sourceNormals.Length == sourceVertices.Length)
                {
                    normals.Add(sourceNormals[i]);
                }
                if (sourceUv != null && sourceUv.Length == sourceVertices.Length)
                {
                    uv.Add(sourceUv[i]);
                }
                if (sourceColors != null && sourceColors.Length == sourceVertices.Length)
                {
                    colors.Add(sourceColors[i]);
                }
            }

            oldToNew[i] = newIndex;
        }

        Mesh result = new Mesh
        {
            name = meshName,
            indexFormat = vertices.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
        };

        result.SetVertices(vertices);
        if (normals.Count == vertices.Count)
        {
            result.SetNormals(normals);
        }
        if (uv.Count == vertices.Count)
        {
            result.SetUVs(0, uv);
        }
        if (colors.Count == vertices.Count)
        {
            result.SetColors(colors);
        }

        int subMeshCount = source.subMeshCount;
        result.subMeshCount = subMeshCount;
        for (int subMesh = 0; subMesh < subMeshCount; subMesh++)
        {
            if (source.GetTopology(subMesh) != MeshTopology.Triangles)
            {
                result.SetTriangles(Array.Empty<int>(), subMesh);
                continue;
            }

            int[] sourceTriangles = source.GetTriangles(subMesh);
            List<int> simplifiedTriangles = new List<int>(sourceTriangles.Length);
            for (int i = 0; i + 2 < sourceTriangles.Length; i += 3)
            {
                int a = oldToNew[sourceTriangles[i]];
                int b = oldToNew[sourceTriangles[i + 1]];
                int c = oldToNew[sourceTriangles[i + 2]];
                if (a == b || b == c || a == c)
                {
                    continue;
                }

                simplifiedTriangles.Add(a);
                simplifiedTriangles.Add(b);
                simplifiedTriangles.Add(c);
            }

            result.SetTriangles(simplifiedTriangles, subMesh);
        }

        result.RecalculateBounds();
        if (normals.Count != vertices.Count)
        {
            result.RecalculateNormals();
        }

        return result;
    }

    private static int CountTriangles(Mesh mesh)
    {
        if (mesh == null)
        {
            return 0;
        }

        int triangles = 0;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            if (mesh.GetTopology(i) == MeshTopology.Triangles)
            {
                ulong indexCount = mesh.GetIndexCount(i);
                triangles += indexCount > int.MaxValue ? 0 : (int)(indexCount / 3);
            }
        }

        return triangles;
    }

    private static void ClearGeneratedLods(GameObject root)
    {
        LODGroup[] groups = root.GetComponentsInChildren<LODGroup>(true);
        for (int i = groups.Length - 1; i >= 0; i--)
        {
            LODGroup group = groups[i];
            if (group != null && group.GetComponentInChildren<LodGeneratedRenderer>(true) != null)
            {
                UnityEngine.Object.DestroyImmediate(group);
            }
        }

        LodGeneratedRenderer[] generated = root.GetComponentsInChildren<LodGeneratedRenderer>(true);
        for (int i = generated.Length - 1; i >= 0; i--)
        {
            if (generated[i] != null)
            {
                UnityEngine.Object.DestroyImmediate(generated[i].gameObject);
            }
        }

        if (AssetDatabase.IsValidFolder(GeneratedAssetFolder))
        {
            AssetDatabase.DeleteAsset(GeneratedAssetFolder);
        }
    }

    private static void EnsureGeneratedAssetFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Generated"))
        {
            AssetDatabase.CreateFolder("Assets", "Generated");
        }

        if (!AssetDatabase.IsValidFolder(GeneratedAssetFolder))
        {
            AssetDatabase.CreateFolder("Assets/Generated", "LOD");
        }
    }

    private static void WriteReport(IReadOnlyList<LodEntry> created, IReadOnlyList<SkipEntry> skipped)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string reportPath = Path.Combine(projectRoot, ReportPath);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath));

        int sourceTriangles = 0;
        int lod1Triangles = 0;
        int lod2Triangles = 0;
        for (int i = 0; i < created.Count; i++)
        {
            sourceTriangles += created[i].SourceTriangles;
            lod1Triangles += created[i].Lod1Triangles;
            lod2Triangles += created[i].Lod2Triangles;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Final Unity 6 LOD Optimization Report");
        builder.AppendLine();
        builder.AppendLine($"- Generated at UTC: `{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}`");
        builder.AppendLine($"- Scene: `{ScenePath}`");
        builder.AppendLine($"- Drone root: `{DroneRootName}`");
        builder.AppendLine($"- LODGroups created: `{created.Count}`");
        builder.AppendLine($"- Renderers skipped: `{skipped.Count}`");
        builder.AppendLine($"- LOD0 source triangles in optimized renderers: `{sourceTriangles}`");
        builder.AppendLine($"- LOD1 generated triangles: `{lod1Triangles}`");
        builder.AppendLine($"- LOD2 generated triangles: `{lod2Triangles}`");
        builder.AppendLine($"- LOD thresholds: `0.55 / 0.22 / 0.025` with cross-fade.");
        builder.AppendLine($"- Exclusions: modular fasteners, small meshes, generated LOD children and existing manual LODGroups.");
        builder.AppendLine();
        builder.AppendLine("## Optimized Renderers");
        builder.AppendLine();
        builder.AppendLine("| Renderer | Category | LOD0 tris | LOD1 tris | LOD2 tris |");
        builder.AppendLine("|---|---:|---:|---:|---:|");

        for (int i = 0; i < created.Count; i++)
        {
            LodEntry entry = created[i];
            builder.AppendLine(
                $"| `{entry.Path}` | {entry.Category} | {entry.SourceTriangles.ToString(CultureInfo.InvariantCulture)} | {entry.Lod1Triangles.ToString(CultureInfo.InvariantCulture)} | {entry.Lod2Triangles.ToString(CultureInfo.InvariantCulture)} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Skipped Renderers");
        builder.AppendLine();
        builder.AppendLine("| Renderer | Reason |");
        builder.AppendLine("|---|---|");

        int skippedLimit = Mathf.Min(skipped.Count, 250);
        for (int i = 0; i < skippedLimit; i++)
        {
            SkipEntry entry = skipped[i];
            builder.AppendLine($"| `{entry.Path}` | {entry.Reason} |");
        }

        if (skipped.Count > skippedLimit)
        {
            builder.AppendLine($"| ... | {skipped.Count - skippedLimit} additional skipped renderers omitted from this table. |");
        }

        File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);
    }

    private static string GetHierarchyPath(Transform transform)
    {
        if (transform == null)
        {
            return string.Empty;
        }

        Stack<string> names = new Stack<string>();
        Transform current = transform;
        while (current != null)
        {
            names.Push(current.name);
            current = current.parent;
        }

        return string.Join("/", names.ToArray());
    }

    private static string SanitizeAssetName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "mesh";
        }

        StringBuilder builder = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            builder.Append(char.IsLetterOrDigit(c) ? c : '_');
        }

        return builder.ToString();
    }

    private static string StableHash(string value)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
            StringBuilder builder = new StringBuilder(10);
            for (int i = 0; i < 5 && i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}

public sealed class WebGLFinalLodBuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => -500;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
        {
            return;
        }

        WebGLFinalLodOptimizer.ApplyFinalLodsForBuild();
    }
}
