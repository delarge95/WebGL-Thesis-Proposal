using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;

/// <summary>
/// Editor-only audit for the imported runtime drone scene.
/// It checks the canonical interactive layer, not Blender source authoring.
/// </summary>
public static class RuntimeDroneSceneAuditor
{
    private const string RootName = "x500v2_Drone";
    private const string CanonicalJsonPath = @"E:\WebGL_tesis\desarrollo\docs\investigacion\Holybro\x500v2_parts_data.json";
    private const string FastenerInstancesJsonPath = @"E:\WebGL_tesis\desarrollo\docs\investigacion\Holybro\holybro_fastener_instances.json";
    private const string ReportPath = "Reports/runtime_drone_scene_audit.md";

    [MenuItem("Tools/Audit/Runtime Drone Scene Audit")]
    public static void RunActiveSceneAudit()
    {
        GameObject root = GameObject.Find(RootName);
        AuditResult result = WriteAuditReport(root != null ? root.transform : null);
        EditorUtility.DisplayDialog(
            "Runtime Drone Scene Audit",
            $"Errores: {result.Errors}\nWarnings: {result.Warnings}\nReporte: {ReportPath}",
            "OK");
    }

    public static AuditResult WriteAuditReport(Transform root)
    {
        AuditResult result = BuildAudit(root);
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string reportPath = Path.Combine(projectRoot, ReportPath);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath) ?? projectRoot);
        File.WriteAllText(reportPath, result.Markdown, new UTF8Encoding(false));

        if (result.Errors > 0)
        {
            Debug.LogError($"[RuntimeDroneSceneAuditor] Audit completed with {result.Errors} errors and {result.Warnings} warnings. See {ReportPath}");
        }
        else if (result.Warnings > 0)
        {
            Debug.LogWarning($"[RuntimeDroneSceneAuditor] Audit completed with {result.Warnings} warnings. See {ReportPath}");
        }
        else
        {
            Debug.Log($"[RuntimeDroneSceneAuditor] Audit OK. See {ReportPath}");
        }

        return result;
    }

    private static AuditResult BuildAudit(Transform root)
    {
        List<string> errors = new List<string>();
        List<string> warnings = new List<string>();
        List<string> info = new List<string>();

        if (root == null)
        {
            errors.Add("No active x500v2_Drone root was found in the scene.");
            return ComposeResult(errors, warnings, info, Array.Empty<string>());
        }

        string[] canonicalIds = LoadCanonicalIds();
        string[] requiredCanonicalIds = canonicalIds
            .Where(id => !IsSuppressedSyntheticCanonical(id))
            .ToArray();
        ExplodablePart[] parts = root.GetComponentsInChildren<ExplodablePart>(true);
        HashSet<string> explodableIds = new HashSet<string>(
            parts.Select(ResolvePartId).Where(id => !string.IsNullOrWhiteSpace(id)),
            StringComparer.OrdinalIgnoreCase);

        string[] missingCanonical = requiredCanonicalIds
            .Where(id => !explodableIds.Contains(id))
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (missingCanonical.Length > 0)
        {
            errors.Add("Missing canonical ExplodablePart anchors: " + string.Join(", ", missingCanonical));
        }

        int blendAnchors = parts.Count(part =>
            ResolvePartId(part).StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase));
        if (blendAnchors > 0)
        {
            errors.Add($"Granular Blender anchors are still selectable: {blendAnchors}");
        }

        int runtimeProxies = root.GetComponentsInChildren<Transform>(true)
            .Count(t => t != null && t.name.EndsWith("_runtime_proxy", StringComparison.OrdinalIgnoreCase));
        if (runtimeProxies > 0)
        {
            warnings.Add($"Runtime canonical proxies present: {runtimeProxies}. This is acceptable only while Blender lacks final geometry for those canonical parts.");
        }

        int meshFilters = root.GetComponentsInChildren<MeshFilter>(true)
            .Count(filter => filter != null && filter.sharedMesh != null);
        if (meshFilters <= 0)
        {
            errors.Add("No MeshFilter with mesh was found under the active drone root.");
        }

        int propellers = explodableIds.Count(id => id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase));
        if (propellers != 4)
        {
            warnings.Add($"Expected 4 propeller anchors; found {propellers}.");
        }

        FastenerRuntimeMarker[] runtimeMarkers = root.GetComponentsInChildren<FastenerRuntimeMarker>(true);
        int fastenerMarkers = runtimeMarkers
            .Count(marker => marker != null && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId));
        if (fastenerMarkers <= 0)
        {
            errors.Add("No sealed fastener runtime markers were found.");
        }

        int invalidFastenerMarkers = runtimeMarkers.Count(marker =>
            marker != null &&
            !SelectionHierarchy.IsPrimitiveFastenerSource(marker.transform));
        if (invalidFastenerMarkers > 0)
        {
            errors.Add($"FastenerRuntimeMarker found on non-primitive fastener objects: {invalidFastenerMarkers}.");
        }

        foreach (HotspotGroupDefinition hotspot in SelectionHierarchy.HotspotGroups)
        {
            if (hotspot == null || hotspot.canonicalPartIds == null)
            {
                continue;
            }

            string[] missingMembers = hotspot.canonicalPartIds
                .Where(id => !IsSuppressedSyntheticCanonical(id) && !explodableIds.Contains(id))
                .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (missingMembers.Length > 0)
            {
                warnings.Add($"Hotspot '{hotspot.label}' references missing canonical members: {string.Join(", ", missingMembers)}");
            }
        }

        FastenerInstanceAudit fasteners = AuditFastenerCatalog();
        if (fasteners.TotalInstances <= 0)
        {
            errors.Add("holybro_fastener_instances.json has no fastener instances.");
        }
        if (fasteners.BlendParents > 0)
        {
            errors.Add($"Fastener instances still assigned to x500v2_blend_* parents: {fasteners.BlendParents}");
        }
        if (fasteners.GroupParents > 0)
        {
            warnings.Add($"Fastener instances still assigned to x500v2_fastener_group: {fasteners.GroupParents}");
        }

        if (fasteners.TotalInstances > 0 && fastenerMarkers != fasteners.TotalInstances)
        {
            warnings.Add($"Fastener marker/catalog mismatch: {fastenerMarkers} runtime markers vs {fasteners.TotalInstances} catalog instances.");
        }

        Bounds bounds;
        if (TryBuildRendererBounds(root, out bounds))
        {
            float dominant = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            info.Add($"Dominant renderer size: {dominant.ToString("0.###", CultureInfo.InvariantCulture)} Unity units.");
            if (dominant < 9f || dominant > 11f)
            {
                warnings.Add($"Dominant size is outside expected 10u +/-10% range: {dominant:0.###}u.");
            }
        }
        else
        {
            errors.Add("Could not compute renderer bounds for active drone root.");
        }

        info.Add($"ExplodablePart anchors: {parts.Length}.");
        info.Add($"Canonical ids documented: {canonicalIds.Length}.");
        info.Add($"Canonical ids required in current FBX: {requiredCanonicalIds.Length}.");
        info.Add("Suppressed synthetic canonical ids: x500v2_pdb, x500v2_platform_board, x500v2_battery, x500v2_rc_receiver, x500v2_prop_FL/FR/BL/BR, x500v2_esc_FL/FR/BL/BR.");
        info.Add($"MeshFilters with mesh: {meshFilters}.");
        info.Add($"Fastener markers: {fastenerMarkers}.");
        info.Add($"Fastener catalog instances: {fasteners.TotalInstances}.");

        return ComposeResult(errors, warnings, info, missingCanonical);
    }

    private static AuditResult ComposeResult(
        IReadOnlyList<string> errors,
        IReadOnlyList<string> warnings,
        IReadOnlyList<string> info,
        IReadOnlyList<string> missingCanonical)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Runtime Drone Scene Audit");
        builder.AppendLine();
        builder.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"Errores: {errors.Count}");
        builder.AppendLine($"Warnings: {warnings.Count}");
        builder.AppendLine();

        AppendSection(builder, "Errors", errors);
        AppendSection(builder, "Warnings", warnings);
        AppendSection(builder, "Info", info);

        if (missingCanonical != null && missingCanonical.Count > 0)
        {
            AppendSection(builder, "Missing Canonical Anchors", missingCanonical);
        }

        return new AuditResult
        {
            Errors = errors.Count,
            Warnings = warnings.Count,
            Markdown = builder.ToString()
        };
    }

    private static void AppendSection(StringBuilder builder, string title, IReadOnlyList<string> rows)
    {
        builder.AppendLine("## " + title);
        builder.AppendLine();
        if (rows == null || rows.Count == 0)
        {
            builder.AppendLine("- None");
            builder.AppendLine();
            return;
        }

        foreach (string row in rows)
        {
            builder.AppendLine("- " + row);
        }
        builder.AppendLine();
    }

    private static string ResolvePartId(ExplodablePart part)
    {
        if (part == null)
        {
            return string.Empty;
        }

        DronePartData data = part.Data;
        return data != null && !string.IsNullOrWhiteSpace(data.id) ? data.id : part.name;
    }

    private static string[] LoadCanonicalIds()
    {
        if (!File.Exists(CanonicalJsonPath))
        {
            return Array.Empty<string>();
        }

        string raw = File.ReadAllText(CanonicalJsonPath);
        CanonicalPartWrapper wrapper = JsonUtility.FromJson<CanonicalPartWrapper>("{\"items\":" + raw + "}");
        return wrapper?.items == null
            ? Array.Empty<string>()
            : wrapper.items.Where(item => item != null && !string.IsNullOrWhiteSpace(item.id)).Select(item => item.id).ToArray();
    }

    private static bool IsSuppressedSyntheticCanonical(string canonicalId)
    {
        if (string.IsNullOrWhiteSpace(canonicalId))
        {
            return false;
        }

        return canonicalId.StartsWith("x500v2_esc_", StringComparison.OrdinalIgnoreCase) ||
               canonicalId.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(canonicalId, "x500v2_pdb", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(canonicalId, "x500v2_platform_board", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(canonicalId, "x500v2_battery", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(canonicalId, "x500v2_rc_receiver", StringComparison.OrdinalIgnoreCase);
    }

    private static FastenerInstanceAudit AuditFastenerCatalog()
    {
        if (!File.Exists(FastenerInstancesJsonPath))
        {
            return new FastenerInstanceAudit();
        }

        string raw = File.ReadAllText(FastenerInstancesJsonPath);
        FastenerInstanceWrapper wrapper = JsonUtility.FromJson<FastenerInstanceWrapper>(raw);
        if (wrapper?.items == null)
        {
            return new FastenerInstanceAudit();
        }

        return new FastenerInstanceAudit
        {
            TotalInstances = wrapper.items.Length,
            BlendParents = wrapper.items.Count(item =>
                item != null &&
                !string.IsNullOrWhiteSpace(item.parentCanonicalPartId) &&
                item.parentCanonicalPartId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase)),
            GroupParents = wrapper.items.Count(item =>
                item != null &&
                string.Equals(item.parentCanonicalPartId, "x500v2_fastener_group", StringComparison.OrdinalIgnoreCase))
        };
    }

    private static bool TryBuildRendererBounds(Transform root, out Bounds bounds)
    {
        bounds = default;
        if (root == null)
        {
            return false;
        }

        bool hasBounds = false;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return hasBounds;
    }

    public sealed class AuditResult
    {
        public int Errors;
        public int Warnings;
        public string Markdown;
    }

    private sealed class FastenerInstanceAudit
    {
        public int TotalInstances;
        public int BlendParents;
        public int GroupParents;
    }

    [Serializable]
    private sealed class CanonicalPartWrapper
    {
        public CanonicalPartItem[] items;
    }

    [Serializable]
    private sealed class CanonicalPartItem
    {
        public string id;
    }

    [Serializable]
    private sealed class FastenerInstanceWrapper
    {
        public FastenerInstanceItem[] items;
    }

    [Serializable]
    private sealed class FastenerInstanceItem
    {
        public string parentCanonicalPartId;
    }
}
