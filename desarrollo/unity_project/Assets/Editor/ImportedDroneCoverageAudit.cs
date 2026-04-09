using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using WebGL.Core.Content;

public static class ImportedDroneCoverageAudit
{
    private const string RootName = "x500v2_Drone";
    private const string CanonicalJsonFile = "x500v2_parts_data.json";
    private const string SyncedJsonFile = "x500v2_blender_synced_parts.json";
    private const string ReportRelativePath = "Assets/../Reports/imported_drone_coverage_report.md";

    private static string HolybroDocsDir => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "docs", "investigacion", "Holybro"));
    private static string CanonicalJsonPath => Path.Combine(HolybroDocsDir, CanonicalJsonFile);
    private static string SyncedJsonPath => Path.Combine(HolybroDocsDir, SyncedJsonFile);

    [Serializable]
    private class CanonicalPart
    {
        public string id;
    }

    [Serializable]
    private class CanonicalWrapper
    {
        public CanonicalPart[] items;
    }

    [Serializable]
    private class SyncedPart
    {
        public string id;
    }

    [Serializable]
    private class SyncedWrapper
    {
        public SyncedPart[] items;
    }

    [MenuItem("Tools/Thermal/Audit Imported Drone Coverage")]
    public static void RunAudit()
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null)
        {
            EditorUtility.DisplayDialog("Coverage Audit", "No se encontro x500v2_Drone en la escena activa.", "OK");
            return;
        }

        HashSet<string> expectedIds = LoadExpectedIds(out string sourceName);
        if (expectedIds.Count == 0)
        {
            EditorUtility.DisplayDialog("Coverage Audit", "No se pudieron leer IDs esperados desde synced/canonical JSON.", "OK");
            return;
        }

        List<ExplodablePart> parts = root.GetComponentsInChildren<ExplodablePart>(true).ToList();
        Dictionary<string, ExplodablePart> partById = new Dictionary<string, ExplodablePart>(StringComparer.OrdinalIgnoreCase);
        List<ExplodablePart> partsWithoutDataId = new List<ExplodablePart>();

        foreach (ExplodablePart part in parts)
        {
            if (part == null)
            {
                continue;
            }

            string id = ResolvePartId(part);
            if (string.IsNullOrWhiteSpace(id))
            {
                partsWithoutDataId.Add(part);
                continue;
            }

            if (!partById.ContainsKey(id))
            {
                partById.Add(id, part);
            }
        }

        List<string> missingExpected = expectedIds.Where(id => !partById.ContainsKey(id)).OrderBy(id => id).ToList();
        List<string> extraAnchors = partById.Keys.Where(id => !expectedIds.Contains(id)).OrderBy(id => id).ToList();

        HashSet<string> canonicalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (TryLoadCanonical(out HashSet<string> canonicalLoaded))
        {
            canonicalIds = canonicalLoaded;
        }

        List<string> missingCanonical = canonicalIds.Where(id => !partById.ContainsKey(id)).OrderBy(id => id).ToList();
        List<string> extraVsCanonical = partById.Keys.Where(id => !canonicalIds.Contains(id)).OrderBy(id => id).ToList();

        int selectableLayer = LayerMask.NameToLayer("SelectablePart");
        int totalRenderers = 0;
        int renderersWithCollider = 0;
        int renderersWithoutCollider = 0;
        int renderersOnSelectableLayer = 0;
        int renderersWrongLayer = 0;

        List<string> anchorsWithoutAnyRenderer = new List<string>();
        List<string> anchorsWithRendererWithoutCollider = new List<string>();
        List<string> anchorsNearOrigin = new List<string>();
        List<string> anchorsNearRoot = new List<string>();

        List<string> orphanTopLevelRenderers = new List<string>();
        List<string> orphanTopLevelResolvableByPrefix = new List<string>();
        List<string> orphanTopLevelUnresolved = new List<string>();

        foreach (KeyValuePair<string, ExplodablePart> kvp in partById)
        {
            string id = kvp.Key;
            ExplodablePart part = kvp.Value;
            Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);

            if (renderers.Length == 0)
            {
                anchorsWithoutAnyRenderer.Add(id);
            }

            bool hasRendererWithoutCollider = false;
            Vector3 representative = part.transform.position;

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                totalRenderers++;
                representative = renderer.bounds.center;

                Collider col = renderer.GetComponent<Collider>();
                if (col != null)
                {
                    renderersWithCollider++;
                }
                else
                {
                    renderersWithoutCollider++;
                    hasRendererWithoutCollider = true;
                }

                if (selectableLayer >= 0)
                {
                    if (renderer.gameObject.layer == selectableLayer)
                    {
                        renderersOnSelectableLayer++;
                    }
                    else
                    {
                        renderersWrongLayer++;
                    }
                }
            }

            if (hasRendererWithoutCollider)
            {
                anchorsWithRendererWithoutCollider.Add(id);
            }

            if (representative.magnitude <= 0.05f)
            {
                anchorsNearOrigin.Add($"{id} ({FormatVector(representative)})");
            }

            if (Vector3.Distance(part.transform.position, root.transform.position) <= 0.02f)
            {
                anchorsNearRoot.Add($"{id} ({FormatVector(part.transform.position)})");
            }
        }

        foreach (Transform child in root.transform)
        {
            if (child == null)
            {
                continue;
            }

            if (child.GetComponent<ExplodablePart>() != null)
            {
                continue;
            }

            Renderer renderer = child.GetComponentInChildren<Renderer>(true);
            if (renderer == null)
            {
                continue;
            }

            orphanTopLevelRenderers.Add(child.name);

            if (ResolveAnchorIdFromNamePrefix(child.name, partById.Keys) != null)
            {
                orphanTopLevelResolvableByPrefix.Add(child.name);
            }
            else
            {
                orphanTopLevelUnresolved.Add(child.name);
            }
        }

        StringBuilder report = new StringBuilder();
        report.AppendLine("# Imported Drone Coverage Audit");
        report.AppendLine();
        report.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Escena activa: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        report.AppendLine($"Root auditado: {RootName}");
        report.AppendLine($"Fuente de IDs esperados: {sourceName}");
        report.AppendLine();
        report.AppendLine("## Resumen");
        report.AppendLine();
        report.AppendLine($"- IDs esperados: {expectedIds.Count}");
        report.AppendLine($"- Anchors con ExplodablePart: {partById.Count}");
        report.AppendLine($"- IDs faltantes en escena: {missingExpected.Count}");
        report.AppendLine($"- Anchors extra no esperados: {extraAnchors.Count}");
        if (canonicalIds.Count > 0)
        {
            report.AppendLine($"- IDs canónicos faltantes en escena: {missingCanonical.Count}");
            report.AppendLine($"- Anchors extra vs canónico: {extraVsCanonical.Count}");
        }
        report.AppendLine($"- Anchors sin renderer: {anchorsWithoutAnyRenderer.Count}");
        report.AppendLine($"- Renderers totales: {totalRenderers}");
        report.AppendLine($"- Renderers con collider: {renderersWithCollider}");
        report.AppendLine($"- Renderers sin collider: {renderersWithoutCollider}");
        if (selectableLayer >= 0)
        {
            report.AppendLine($"- Renderers en layer SelectablePart: {renderersOnSelectableLayer}");
            report.AppendLine($"- Renderers fuera de SelectablePart: {renderersWrongLayer}");
        }
        else
        {
            report.AppendLine("- Layer SelectablePart no existe en este proyecto.");
        }
        report.AppendLine($"- Anchors cerca del origen (|pos| <= 0.05): {anchorsNearOrigin.Count}");
        report.AppendLine($"- Anchors colapsados cerca de root (dist <= 0.02): {anchorsNearRoot.Count}");
        report.AppendLine($"- Renderers huérfanos en primer nivel (sin ExplodablePart): {orphanTopLevelRenderers.Count}");
        report.AppendLine($"- Huérfanos reparables por prefijo: {orphanTopLevelResolvableByPrefix.Count}");
        report.AppendLine($"- Huérfanos no resueltos por prefijo: {orphanTopLevelUnresolved.Count}");
        report.AppendLine();

        AppendListSection(report, "IDs faltantes", missingExpected);
        AppendListSection(report, "Anchors extra", extraAnchors);
        if (canonicalIds.Count > 0)
        {
            AppendListSection(report, "IDs canónicos faltantes", missingCanonical);
            AppendListSection(report, "Anchors extra vs canónico", extraVsCanonical);
        }
        AppendListSection(report, "Anchors sin renderer", anchorsWithoutAnyRenderer);
        AppendListSection(report, "Anchors con al menos un renderer sin collider", anchorsWithRendererWithoutCollider.OrderBy(x => x).ToList());
        AppendListSection(report, "Anchors cerca del origen", anchorsNearOrigin.OrderBy(x => x).ToList());
        AppendListSection(report, "Anchors colapsados cerca del root", anchorsNearRoot.OrderBy(x => x).ToList());
        AppendListSection(report, "Objetos top-level huérfanos con renderer", orphanTopLevelRenderers.OrderBy(x => x).ToList());
        AppendListSection(report, "Objetos top-level huérfanos reparables por prefijo", orphanTopLevelResolvableByPrefix.OrderBy(x => x).ToList());
        AppendListSection(report, "Objetos top-level huérfanos no resueltos por prefijo", orphanTopLevelUnresolved.OrderBy(x => x).ToList());
        AppendListSection(report, "ExplodablePart sin Data.id", partsWithoutDataId.Select(p => p.name).OrderBy(x => x).ToList());

        string reportPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Reports", "imported_drone_coverage_report.md"));
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath) ?? Path.GetFullPath(Path.Combine(Application.dataPath, "..")));
        File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();
        Debug.Log($"[ImportedDroneCoverageAudit] Reporte generado: {reportPath}");
        EditorUtility.DisplayDialog("Coverage Audit", "Reporte generado en Reports/imported_drone_coverage_report.md", "OK");
    }

    private static void AppendListSection(StringBuilder report, string title, List<string> values)
    {
        report.AppendLine($"## {title}");
        report.AppendLine();

        if (values == null || values.Count == 0)
        {
            report.AppendLine("- Ninguno");
            report.AppendLine();
            return;
        }

        foreach (string value in values)
        {
            report.AppendLine($"- {value}");
        }

        report.AppendLine();
    }

    private static string ResolvePartId(ExplodablePart part)
    {
        if (part == null)
        {
            return string.Empty;
        }

        if (part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id))
        {
            return part.Data.id;
        }

        return part.name;
    }

    private static string FormatVector(Vector3 value)
    {
        return $"({value.x:0.###}, {value.y:0.###}, {value.z:0.###})";
    }

    private static string ResolveAnchorIdFromNamePrefix(string candidateName, IEnumerable<string> anchorIds)
    {
        if (string.IsNullOrWhiteSpace(candidateName) || anchorIds == null)
        {
            return null;
        }

        HashSet<string> ids = new HashSet<string>(anchorIds, StringComparer.OrdinalIgnoreCase);
        if (ids.Contains(candidateName))
        {
            return candidateName;
        }

        int dot = candidateName.IndexOf('.');
        if (dot > 0)
        {
            string prefix = candidateName.Substring(0, dot);
            if (ids.Contains(prefix))
            {
                return prefix;
            }
        }

        foreach (string id in ids)
        {
            if (candidateName.StartsWith(id + ".", StringComparison.OrdinalIgnoreCase))
            {
                return id;
            }
        }

        return null;
    }

    private static HashSet<string> LoadExpectedIds(out string source)
    {
        if (TryLoadSynced(out HashSet<string> syncedIds))
        {
            source = SyncedJsonFile;
            return syncedIds;
        }

        if (TryLoadCanonical(out HashSet<string> canonicalIds))
        {
            source = CanonicalJsonFile;
            return canonicalIds;
        }

        source = "N/A";
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryLoadSynced(out HashSet<string> ids)
    {
        ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(SyncedJsonPath))
        {
            return false;
        }

        string raw = File.ReadAllText(SyncedJsonPath);
        SyncedWrapper wrapper = JsonUtility.FromJson<SyncedWrapper>("{\"items\":" + raw + "}");
        if (wrapper?.items == null || wrapper.items.Length == 0)
        {
            return false;
        }

        foreach (SyncedPart item in wrapper.items)
        {
            if (item != null && !string.IsNullOrWhiteSpace(item.id))
            {
                ids.Add(item.id);
            }
        }

        return ids.Count > 0;
    }

    private static bool TryLoadCanonical(out HashSet<string> ids)
    {
        ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(CanonicalJsonPath))
        {
            return false;
        }

        string raw = File.ReadAllText(CanonicalJsonPath);
        CanonicalWrapper wrapper = JsonUtility.FromJson<CanonicalWrapper>("{\"items\":" + raw + "}");
        if (wrapper?.items == null || wrapper.items.Length == 0)
        {
            return false;
        }

        foreach (CanonicalPart item in wrapper.items)
        {
            if (item != null && !string.IsNullOrWhiteSpace(item.id))
            {
                ids.Add(item.id);
            }
        }

        return ids.Count > 0;
    }
}
