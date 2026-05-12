using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;

internal sealed class HolybroFastenerCatalogBuildResult
{
    public FastenerFamilyCatalogJson FamiliesCatalog;
    public FastenerInstanceCatalogJson InstancesCatalog;
    public FastenerReconciliationJson ReconciliationCatalog;
    public Dictionary<string, FastenerMetadata> MetadataBySceneObjectName = new Dictionary<string, FastenerMetadata>(StringComparer.OrdinalIgnoreCase);
}

internal static class HolybroFastenerCatalogBuilder
{
    private const string FastenerGroupId = "x500v2_fastener_group";
    private const string SyncedJsonFile = "x500v2_blender_synced_parts.json";
    private const string FamiliesJsonFile = "holybro_fastener_families.json";
    private const string InstancesJsonFile = "holybro_fastener_instances.json";
    private const string ReconciliationJsonFile = "holybro_fastener_reconciliation.json";

    [Serializable]
    private class SourcePartWrapper
    {
        public SourcePart[] items;
    }

    [Serializable]
    private class SourcePart
    {
        public string partName;
        public string id;
        public string blenderName;
        public string category;
        public string description;
        public string function;
        public float weightKg;
        public string dimensions;
        public string materialType;
        public string manufacturer;
        public int quantityInScene;
    }

    private sealed class SceneFastenerInstance
    {
        public string SceneObjectName;
        public string SceneTypeKey;
        public string InstanceId;
        public string HierarchyPath;
        public string ParentCanonicalPartId;
        public Transform Transform;
    }

    private sealed class CanonicalAnchorCandidate
    {
        public string CanonicalId;
        public Transform Transform;
        public Vector3 ReferencePosition;
    }

    private sealed class CategorizedRendererCandidate
    {
        public string CanonicalId;
        public Bounds Bounds;
        public float SizeScore;
    }

    private static readonly Dictionary<string, string> SceneTypeToSourceId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "cap_screw_M25x10", "x500v2_blend_gb70_m25_10" },
        { "cap_screw_M25x12", "x500v2_blend_gb70_m25_12" },
        { "cap_screw_M25x6", "x500v2_blend_gb70_m25_6" },
        { "cap_screw_M3x21", "x500v2_blend_gb70_m3_21_ding" },
        { "cap_screw_M3x25", "x500v2_blend_gb70_m3_25_ding" },
        { "cap_screw_M3x38", "x500v2_blend_gb70_m3_38" },
        { "cap_screw_M3x6", "x500v2_blend_gb70_m3_6" },
        { "cap_screw_M3x8", "x500v2_blend_gb70_m3_8_ding" },
        { "countersunk_M25x6", "x500v2_blend_m25_6_chen_liu" },
        { "countersunk_M3x16", "x500v2_blend_m3_16_chen_liu" },
        { "pan_head_M3x10", "x500v2_blend_m3_10_pan_ding" },
        { "pan_head_M3x14", "x500v2_blend_m3_14_pan" },
        { "flange_nut_M3", "x500v2_blend_zslm_m3_falan" },
        { "lock_nut_M3", "x500v2_blend_lm_m3_ding" },
        { "self_lock_nut_M3", "x500v2_blend_zslm_m3_ding" },
        { "nylon_lock_nut_M3", "x500v2_blend_lm_m3_nilong" },
        { "self_lock_nut_M25", "x500v2_blend_zslm_m25" },
        { "nylon_standoff_M25x5", "x500v2_blend_nilongzhu_m25_5" },
        { "nylon_standoff_M3x5", "x500v2_blend_nilongzhu_m3_5" }
    };

    private static readonly HashSet<string> ExplicitFastenerSourceIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
    };

    private static string HolybroDocsDir => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "docs", "investigacion", "Holybro"));
    private static string SyncedJsonPath => Path.Combine(HolybroDocsDir, SyncedJsonFile);
    private static string ResourcesDir => Path.Combine(Application.dataPath, "Resources");

    public static HolybroFastenerCatalogBuildResult BuildAndWrite(Transform droneRoot)
    {
        HolybroFastenerCatalogBuildResult result = Build(droneRoot);
        WriteCatalogJson(result);
        return result;
    }

    public static FastenerMetadata GetMetadataForSceneObject(HolybroFastenerCatalogBuildResult result, string sceneObjectName)
    {
        if (result == null || string.IsNullOrWhiteSpace(sceneObjectName))
        {
            return null;
        }

        result.MetadataBySceneObjectName.TryGetValue(sceneObjectName, out FastenerMetadata metadata);
        return metadata;
    }

    private static HolybroFastenerCatalogBuildResult Build(Transform droneRoot)
    {
        List<FastenerReconciliationAlias> aliases = new List<FastenerReconciliationAlias>();
        List<FastenerReconciliationIssue> issues = new List<FastenerReconciliationIssue>();
        List<SceneFastenerInstance> sceneInstances = CollectSceneInstances(droneRoot);
        SourcePart[] sources = LoadFastenerSources();
        Dictionary<string, SourcePart> sourcesById = sources.ToDictionary(item => item.id, StringComparer.OrdinalIgnoreCase);
        HashSet<string> usedSourceIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        issues.Add(new FastenerReconciliationIssue
        {
            severity = "info",
            scope = "SceneState",
            message = "The active Unity fastener meshes are temporary proxy geometry. Family and instance identifiers are kept stable so final Blender assets can replace the visible detail later without changing the runtime contract."
        });

        List<FastenerFamilyDefinition> families = new List<FastenerFamilyDefinition>();
        foreach (IGrouping<string, SceneFastenerInstance> grouped in sceneInstances.GroupBy(item => item.SceneTypeKey, StringComparer.OrdinalIgnoreCase).OrderBy(group => group.Key))
        {
            string sceneTypeKey = grouped.Key;
            SourcePart source = null;
            if (SceneTypeToSourceId.TryGetValue(sceneTypeKey, out string sourceId) &&
                sourcesById.TryGetValue(sourceId, out source))
            {
                usedSourceIds.Add(source.id);
                aliases.Add(new FastenerReconciliationAlias
                {
                    sceneTypeKey = sceneTypeKey,
                    sourceId = source.id,
                    blenderName = source.blenderName,
                    rationale = "Explicit Unity scene type to Holybro synced source mapping."
                });
            }
            else
            {
                source = FindHeuristicSource(sceneTypeKey, sources, usedSourceIds);
                if (source != null)
                {
                    usedSourceIds.Add(source.id);
                    aliases.Add(new FastenerReconciliationAlias
                    {
                        sceneTypeKey = sceneTypeKey,
                        sourceId = source.id,
                        blenderName = source.blenderName,
                        rationale = "Heuristic fallback based on metric/length and fastener family tokens."
                    });
                }
            }

            if (source == null)
            {
                issues.Add(new FastenerReconciliationIssue
                {
                    severity = "warning",
                    scope = "SceneFamily",
                    sceneTypeKey = sceneTypeKey,
                    message = "No Holybro synced source entry matched this Unity fastener family. JSON falls back to scene-derived metadata only."
                });
            }

            FastenerFamilyDefinition family = BuildFamilyDefinition(sceneTypeKey, grouped.Count(), source, issues);
            families.Add(family);

            if (source != null && source.quantityInScene > 0 && source.quantityInScene != grouped.Count())
            {
                issues.Add(new FastenerReconciliationIssue
                {
                    severity = "warning",
                    scope = "QuantityMismatch",
                    sceneTypeKey = sceneTypeKey,
                    sourceId = source.id,
                    message = $"Scene contains {grouped.Count()} instances while synced source expects {source.quantityInScene}."
                });
            }
        }

        foreach (SourcePart source in sources)
        {
            if (!usedSourceIds.Contains(source.id))
            {
                issues.Add(new FastenerReconciliationIssue
                {
                    severity = "info",
                    scope = "SourceOnly",
                    sourceId = source.id,
                    message = $"Source fastener '{source.blenderName}' does not have a direct Unity scene family match in the current temporary proxy scene."
                });
            }
        }

        Dictionary<string, FastenerFamilyDefinition> familyByTypeKey = families.ToDictionary(item => item.sceneTypeKey, StringComparer.OrdinalIgnoreCase);
        List<FastenerInstanceDefinition> instances = new List<FastenerInstanceDefinition>(sceneInstances.Count);
        HolybroFastenerCatalogBuildResult result = new HolybroFastenerCatalogBuildResult
        {
            FamiliesCatalog = new FastenerFamilyCatalogJson
            {
                sourceScene = "Assets/Scenes/MainScene_Final.unity",
                sourceDataset = SyncedJsonFile,
                generatedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                items = families.ToArray()
            },
            InstancesCatalog = new FastenerInstanceCatalogJson
            {
                sourceScene = "Assets/Scenes/MainScene_Final.unity",
                sourceDataset = SyncedJsonFile,
                generatedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
            },
            ReconciliationCatalog = new FastenerReconciliationJson
            {
                sourceScene = "Assets/Scenes/MainScene_Final.unity",
                sourceDataset = SyncedJsonFile,
                generatedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
            }
        };

        foreach (SceneFastenerInstance sceneInstance in sceneInstances.OrderBy(item => item.SceneObjectName, StringComparer.OrdinalIgnoreCase))
        {
            familyByTypeKey.TryGetValue(sceneInstance.SceneTypeKey, out FastenerFamilyDefinition family);
            string fallbackReason = family == null
                ? "Family definition missing for this temporary Unity proxy instance."
                : (!family.modularRecipe.isInspectable ? "This family currently keeps proxy-only visualization." : string.Empty);

            FastenerInstanceDefinition instance = new FastenerInstanceDefinition
            {
                instanceId = sceneInstance.InstanceId,
                familyId = family != null ? family.familyId : string.Empty,
                sceneObjectName = sceneInstance.SceneObjectName,
                hierarchyPath = sceneInstance.HierarchyPath,
                parentCanonicalPartId = sceneInstance.ParentCanonicalPartId,
                localPosition = new SerializableVector3(sceneInstance.Transform.localPosition),
                localRotationEuler = new SerializableVector3(sceneInstance.Transform.localEulerAngles),
                localScale = new SerializableVector3(sceneInstance.Transform.localScale),
                isInspectable = family != null && family.modularRecipe != null && family.modularRecipe.isInspectable,
                fallbackReason = fallbackReason
            };

            instances.Add(instance);

            FastenerMetadata metadata = FastenerMetadata.Combine(family, instance);
            if (metadata != null)
            {
                result.MetadataBySceneObjectName[sceneInstance.SceneObjectName] = metadata;
            }
        }

        result.InstancesCatalog.items = instances.ToArray();
        result.ReconciliationCatalog.aliases = aliases.ToArray();
        result.ReconciliationCatalog.issues = issues.ToArray();

        return result;
    }

    private static void WriteCatalogJson(HolybroFastenerCatalogBuildResult result)
    {
        if (result == null)
        {
            return;
        }

        WriteJson(Path.Combine(HolybroDocsDir, FamiliesJsonFile), JsonUtility.ToJson(result.FamiliesCatalog, true));
        WriteJson(Path.Combine(HolybroDocsDir, InstancesJsonFile), JsonUtility.ToJson(result.InstancesCatalog, true));
        WriteJson(Path.Combine(HolybroDocsDir, ReconciliationJsonFile), JsonUtility.ToJson(result.ReconciliationCatalog, true));

        WriteJson(Path.Combine(ResourcesDir, FamiliesJsonFile), JsonUtility.ToJson(result.FamiliesCatalog, true));
        WriteJson(Path.Combine(ResourcesDir, InstancesJsonFile), JsonUtility.ToJson(result.InstancesCatalog, true));
        WriteJson(Path.Combine(ResourcesDir, ReconciliationJsonFile), JsonUtility.ToJson(result.ReconciliationCatalog, true));
    }

    private static void WriteJson(string path, string content)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
        File.WriteAllText(path, content ?? string.Empty, new UTF8Encoding(false));
    }

    private static List<SceneFastenerInstance> CollectSceneInstances(Transform droneRoot)
    {
        List<SceneFastenerInstance> instances = new List<SceneFastenerInstance>();
        if (droneRoot == null)
        {
            return instances;
        }

        List<CanonicalAnchorCandidate> canonicalAnchors = CollectCanonicalAnchors(droneRoot);

        foreach (Transform child in droneRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child == null || child == droneRoot)
            {
                continue;
            }

            if (!IsSceneFastenerName(child.name))
            {
                continue;
            }

            if (child.GetComponentInChildren<Renderer>(true) == null)
            {
                continue;
            }

            instances.Add(new SceneFastenerInstance
            {
                SceneObjectName = child.name,
                SceneTypeKey = FastenerNamingUtility.ExtractSceneTypeKey(child.name),
                InstanceId = FastenerNamingUtility.SanitizeId(child.name),
                HierarchyPath = BuildHierarchyPath(child, droneRoot),
                ParentCanonicalPartId = ResolveParentCanonicalPartId(child, canonicalAnchors),
                Transform = child
            });
        }

        return instances;
    }

    private static bool IsSceneFastenerName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return false;
        }

        if (SelectionHierarchy.IsKnownStructuralNonFastenerName(rawName))
        {
            return false;
        }

        string name = rawName.ToLowerInvariant().Replace('_', '-');
        return name.Contains("x500v2-fastener.") ||
               name.Contains("gb70-") ||
               name.Contains("chen-liu") ||
               name.Contains("pan-ding") ||
               name.Contains("zslm-") ||
               name.Contains("lm-m3") ||
               name.Contains("nilongzhu") ||
               name.Contains("falan");
    }

    private static string BuildHierarchyPath(Transform target, Transform root)
    {
        if (target == null)
        {
            return string.Empty;
        }

        Stack<string> segments = new Stack<string>();
        Transform current = target;
        while (current != null)
        {
            segments.Push(current.name);
            if (current == root)
            {
                break;
            }

            current = current.parent;
        }

        return string.Join("/", segments.ToArray());
    }

    private static List<CanonicalAnchorCandidate> CollectCanonicalAnchors(Transform droneRoot)
    {
        List<CanonicalAnchorCandidate> anchors = new List<CanonicalAnchorCandidate>();
        HashSet<string> seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (droneRoot == null)
        {
            return anchors;
        }

        foreach (ExplodablePart part in droneRoot.GetComponentsInChildren<ExplodablePart>(true))
        {
            if (part == null || part.transform == null)
            {
                continue;
            }

            string canonicalId = part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id)
                ? part.Data.id
                : part.transform.name;

            if (string.IsNullOrWhiteSpace(canonicalId) ||
                string.Equals(canonicalId, FastenerGroupId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(canonicalId, "x500v2_misc_group", StringComparison.OrdinalIgnoreCase) ||
                canonicalId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase) ||
                canonicalId.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase) ||
                HasRuntimeProxyVisual(part.transform) ||
                !seenIds.Add(canonicalId))
            {
                continue;
            }

            Renderer renderer = part.GetComponentInChildren<Renderer>(true);
            Vector3 referencePosition = renderer != null ? renderer.bounds.center : part.transform.position;
            anchors.Add(new CanonicalAnchorCandidate
            {
                CanonicalId = canonicalId,
                Transform = part.transform,
                ReferencePosition = referencePosition
            });
        }

        return anchors;
    }

    private static bool HasRuntimeProxyVisual(Transform candidate)
    {
        if (candidate == null)
        {
            return false;
        }

        foreach (Transform child in candidate.GetComponentsInChildren<Transform>(true))
        {
            if (child != null && child.name.EndsWith("_runtime_proxy", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string ResolveParentCanonicalPartId(Transform target, IReadOnlyList<CanonicalAnchorCandidate> canonicalAnchors)
    {
        if (target == null)
        {
            return string.Empty;
        }

        string immediateParent = target.parent != null ? target.parent.name : string.Empty;
        if (!string.IsNullOrWhiteSpace(immediateParent) &&
            !string.Equals(immediateParent, FastenerGroupId, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(immediateParent, "x500v2_misc_group", StringComparison.OrdinalIgnoreCase) &&
            !immediateParent.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase) &&
            immediateParent.StartsWith("x500v2_", StringComparison.OrdinalIgnoreCase) &&
            immediateParent.IndexOf("fastener", StringComparison.OrdinalIgnoreCase) < 0)
        {
            return immediateParent;
        }

        string rendererParent = ResolveParentFromNearestCategorizedRenderer(target);
        if (!string.IsNullOrWhiteSpace(rendererParent))
        {
            return rendererParent;
        }

        if (canonicalAnchors == null || canonicalAnchors.Count == 0)
        {
            return FastenerGroupId;
        }

        Renderer renderer = target.GetComponentInChildren<Renderer>(true);
        Vector3 referencePosition = renderer != null ? renderer.bounds.center : target.position;
        float bestDistance = float.MaxValue;
        string bestCanonicalId = FastenerGroupId;

        for (int i = 0; i < canonicalAnchors.Count; i++)
        {
            CanonicalAnchorCandidate candidate = canonicalAnchors[i];
            if (candidate == null || string.IsNullOrWhiteSpace(candidate.CanonicalId))
            {
                continue;
            }

            float distance = Vector3.SqrMagnitude(referencePosition - candidate.ReferencePosition);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCanonicalId = candidate.CanonicalId;
            }
        }

        return bestCanonicalId;
    }

    private static string ResolveParentFromNearestCategorizedRenderer(Transform target)
    {
        Renderer targetRenderer = target != null ? target.GetComponentInChildren<Renderer>(true) : null;
        if (target == null || targetRenderer == null)
        {
            return string.Empty;
        }

        Transform root = target.root;
        if (root == null)
        {
            return string.Empty;
        }

        Vector3 referencePosition = targetRenderer.bounds.center;
        List<CategorizedRendererCandidate> candidates = new List<CategorizedRendererCandidate>();
        foreach (PartRenderCategory category in root.GetComponentsInChildren<PartRenderCategory>(true))
        {
            if (category == null || category.transform == null)
            {
                continue;
            }

            string canonicalId = category.CanonicalPartId;
            if (!IsUsableCanonicalParentId(canonicalId) || IsFastenerCategory(category))
            {
                continue;
            }

            Transform categoryTransform = category.transform;
            if (categoryTransform == target ||
                categoryTransform.IsChildOf(target) ||
                target.IsChildOf(categoryTransform))
            {
                continue;
            }

            Renderer renderer = category.GetComponent<Renderer>();
            if (renderer == null)
            {
                continue;
            }

            Bounds bounds = renderer.bounds;
            if (bounds.size.sqrMagnitude <= 0.0000001f)
            {
                continue;
            }

            candidates.Add(new CategorizedRendererCandidate
            {
                CanonicalId = canonicalId,
                Bounds = bounds,
                SizeScore = bounds.size.sqrMagnitude
            });
        }

        float bestDistance = float.MaxValue;
        float bestSize = float.MaxValue;
        string bestCanonicalId = string.Empty;
        for (int i = 0; i < candidates.Count; i++)
        {
            CategorizedRendererCandidate candidate = candidates[i];
            float distance = candidate.Bounds.SqrDistance(referencePosition);
            bool closer = distance < bestDistance - 0.000001f;
            bool sameDistanceSmaller = Mathf.Abs(distance - bestDistance) <= 0.000001f &&
                                       candidate.SizeScore < bestSize;
            if (!closer && !sameDistanceSmaller)
            {
                continue;
            }

            bestDistance = distance;
            bestSize = candidate.SizeScore;
            bestCanonicalId = candidate.CanonicalId;
        }

        return bestCanonicalId;
    }

    private static bool IsUsableCanonicalParentId(string canonicalId)
    {
        return !string.IsNullOrWhiteSpace(canonicalId) &&
               !string.Equals(canonicalId, FastenerGroupId, StringComparison.OrdinalIgnoreCase) &&
               !string.Equals(canonicalId, "x500v2_misc_group", StringComparison.OrdinalIgnoreCase) &&
               !canonicalId.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase) &&
               !canonicalId.StartsWith("x500v2_fastener", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFastenerCategory(PartRenderCategory category)
    {
        if (category == null)
        {
            return false;
        }

        return string.Equals(category.PrimaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(category.AuxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase);
    }

    private static SourcePart[] LoadFastenerSources()
    {
        if (!File.Exists(SyncedJsonPath))
        {
            return Array.Empty<SourcePart>();
        }

        string raw = File.ReadAllText(SyncedJsonPath);
        SourcePartWrapper wrapper = JsonUtility.FromJson<SourcePartWrapper>("{\"items\":" + raw + "}");
        if (wrapper?.items == null)
        {
            return Array.Empty<SourcePart>();
        }

        return wrapper.items.Where(IsFastenerSource).ToArray();
    }

    private static bool IsFastenerSource(SourcePart part)
    {
        if (part == null)
        {
            return false;
        }

        if (SelectionHierarchy.IsKnownStructuralNonFastenerName(part.id) ||
            SelectionHierarchy.IsKnownStructuralNonFastenerName(part.blenderName) ||
            SelectionHierarchy.IsKnownStructuralNonFastenerName(part.partName))
        {
            return false;
        }

        if (ExplicitFastenerSourceIds.Contains(part.id))
        {
            return true;
        }

        string haystack = string.Join(" ", new[]
        {
            part.id,
            part.blenderName,
            part.partName,
            part.description,
            part.function
        }).ToLowerInvariant();

        return haystack.Contains("screw") ||
               haystack.Contains("nut") ||
               haystack.Contains("fastener") ||
               haystack.Contains("standoff") ||
               haystack.Contains("gb70") ||
               haystack.Contains("falan") ||
               haystack.Contains("nilongzhu");
    }

    private static SourcePart FindHeuristicSource(string sceneTypeKey, IEnumerable<SourcePart> sources, HashSet<string> usedSourceIds)
    {
        ParseMetricLength(sceneTypeKey, out float sceneDiameter, out float sceneLength, out _);
        string lowerType = sceneTypeKey.ToLowerInvariant();

        foreach (SourcePart source in sources)
        {
            if (source == null || usedSourceIds.Contains(source.id))
            {
                continue;
            }

            string lowerSource = string.Join(" ", new[] { source.id, source.blenderName, source.partName }).ToLowerInvariant();
            ParseMetricLength(lowerSource, out float sourceDiameter, out float sourceLength, out _);

            bool typeCompatible =
                (lowerType.Contains("cap_screw") && lowerSource.Contains("gb70")) ||
                (lowerType.Contains("pan_head") && lowerSource.Contains("pan")) ||
                (lowerType.Contains("countersunk") && lowerSource.Contains("chen")) ||
                (lowerType.Contains("flange_nut") && lowerSource.Contains("falan")) ||
                (lowerType.Contains("standoff") && lowerSource.Contains("nilongzhu")) ||
                (lowerType.Contains("lock_nut") && lowerSource.Contains("ding"));

            if (!typeCompatible)
            {
                continue;
            }

            if (sceneDiameter > 0.0001f && sourceDiameter > 0.0001f && Mathf.Abs(sceneDiameter - sourceDiameter) > 0.6f)
            {
                continue;
            }

            if (sceneLength > 0.0001f && sourceLength > 0.0001f && Mathf.Abs(sceneLength - sourceLength) > 1.5f)
            {
                continue;
            }

            return source;
        }

        return null;
    }

    private static FastenerFamilyDefinition BuildFamilyDefinition(
        string sceneTypeKey,
        int sceneCount,
        SourcePart source,
        List<FastenerReconciliationIssue> issues)
    {
        ParseMetricLength(sceneTypeKey, out float diameterMm, out float lengthMm, out string metric);

        string subtype = ResolveSubtype(sceneTypeKey);
        string driveType = ResolveDriveType(sceneTypeKey);
        string headProfile = ResolveHeadProfile(sceneTypeKey);
        float threadPitchMm = ResolveThreadPitch(metric, subtype);
        int turnCount = ResolveTurnCount(lengthMm, threadPitchMm, subtype);
        FastenerModularRecipe recipe = BuildRecipe(sceneTypeKey, subtype, turnCount);

        string notes = source != null ? source.description : "Derived from temporary Unity proxy family naming only.";
        if (source == null)
        {
            notes += " Final Blender geometry is expected to replace this proxy family later.";
        }

        if (source != null && source.quantityInScene != sceneCount)
        {
            notes += $" Synced source expects {source.quantityInScene} units while the current temporary scene contains {sceneCount}.";
        }

        if ((subtype == "Standoff" || subtype == "CapNut") && source != null && !source.partName.ToLowerInvariant().Contains("standoff"))
        {
            issues.Add(new FastenerReconciliationIssue
            {
                severity = "info",
                scope = "SemanticOverride",
                sceneTypeKey = sceneTypeKey,
                sourceId = source.id,
                message = "Unity scene naming overrides the generic synced part label so the modular family better matches the runtime placeholder behavior."
            });
        }

        return new FastenerFamilyDefinition
        {
            familyId = "holybro_" + FastenerNamingUtility.SanitizeId(sceneTypeKey).ToLowerInvariant(),
            sourceId = source != null ? source.id : string.Empty,
            blenderName = source != null ? source.blenderName : string.Empty,
            sceneTypeKey = sceneTypeKey,
            subtype = subtype,
            metric = metric,
            nominalDiameterMm = diameterMm,
            lengthMm = lengthMm,
            headProfile = headProfile,
            driveType = driveType,
            threadPitchMm = threadPitchMm,
            turnCount = turnCount,
            material = source != null ? source.materialType : "Unknown",
            finish = ResolveFinish(source != null ? source.materialType : string.Empty),
            quantityExpected = source != null && source.quantityInScene > 0 ? source.quantityInScene : sceneCount,
            notes = notes,
            modularRecipe = recipe
        };
    }

    private static FastenerModularRecipe BuildRecipe(string sceneTypeKey, string subtype, int turnCount)
    {
        string builderType = subtype switch
        {
            "PanHeadScrew" => "PanHeadScrew",
            "CountersunkScrew" => "CountersunkScrew",
            "FlangeNut" => "FlangeNut",
            "CapNut" => "CapNut",
            "NylocNut" => "NylocNut",
            "Standoff" => "Standoff",
            "RubberGrommet" => "RubberGrommet",
            "TubeStopper" => "TubeStopper",
            _ => "SocketCapScrew"
        };

        bool shortestAxis = builderType == "FlangeNut" ||
                            builderType == "CapNut" ||
                            builderType == "NylocNut" ||
                            builderType == "RubberGrommet" ||
                            builderType == "TubeStopper";

        return new FastenerModularRecipe
        {
            recipeKey = $"{sceneTypeKey}_placeholder",
            builderType = builderType,
            orientationMode = shortestAxis ? "ShortestAxis" : "LongestAxis",
            middleSegments = Mathf.Clamp(turnCount > 0 ? turnCount : 4, 2, 12),
            headLengthRatio = builderType == "SocketCapScrew" ? 0.24f : 0.20f,
            shaftLengthRatio = builderType == "SocketCapScrew" ? 0.60f : 0.64f,
            tipLengthRatio = 0.10f,
            headDiameterScale = builderType == "PanHeadScrew" ? 1.05f : 1.0f,
            shaftDiameterScale = 0.56f,
            isInspectable = true
        };
    }

    private static string ResolveSubtype(string sceneTypeKey)
    {
        string lower = sceneTypeKey.ToLowerInvariant();
        if (lower.Contains("pan_head")) return "PanHeadScrew";
        if (lower.Contains("countersunk")) return "CountersunkScrew";
        if (lower.Contains("flange_nut")) return "FlangeNut";
        if (lower.Contains("nylon_lock_nut")) return "NylocNut";
        if (lower.Contains("lock_nut")) return "CapNut";
        if (lower.Contains("standoff")) return "Standoff";
        if (lower.Contains("grommet")) return "RubberGrommet";
        if (lower.Contains("stopper")) return "TubeStopper";
        return "SocketCapScrew";
    }

    private static string ResolveHeadProfile(string sceneTypeKey)
    {
        string lower = sceneTypeKey.ToLowerInvariant();
        if (lower.Contains("pan_head")) return "Pan/Button";
        if (lower.Contains("countersunk")) return "Countersunk";
        if (lower.Contains("cap_screw")) return "Socket Cap";
        if (lower.Contains("flange_nut")) return "Flange";
        if (lower.Contains("lock_nut")) return "Cap";
        if (lower.Contains("grommet")) return "Dampener";
        if (lower.Contains("standoff")) return "Standoff";
        return "Generic";
    }

    private static string ResolveDriveType(string sceneTypeKey)
    {
        string lower = sceneTypeKey.ToLowerInvariant();
        if (lower.Contains("cap_screw")) return "Hex Socket";
        if (lower.Contains("pan_head")) return "Unknown";
        if (lower.Contains("countersunk")) return "Unknown";
        if (lower.Contains("nut") || lower.Contains("grommet") || lower.Contains("standoff")) return "N/A";
        return "Unknown";
    }

    private static string ResolveFinish(string material)
    {
        if (string.IsNullOrWhiteSpace(material))
        {
            return "Unspecified";
        }

        string lower = material.ToLowerInvariant();
        if (lower.Contains("inox")) return "Stainless";
        if (lower.Contains("lat")) return "Brass";
        if (lower.Contains("silic")) return "Silicone";
        if (lower.Contains("nylon")) return "Nylon";
        return "Standard";
    }

    private static float ResolveThreadPitch(string metric, string subtype)
    {
        if (subtype != "SocketCapScrew" && subtype != "PanHeadScrew" && subtype != "CountersunkScrew")
        {
            return 0f;
        }

        return metric switch
        {
            "M2.5" => 0.45f,
            "M3" => 0.50f,
            _ => 0f
        };
    }

    private static int ResolveTurnCount(float lengthMm, float threadPitchMm, string subtype)
    {
        if (threadPitchMm <= 0.0001f)
        {
            return subtype == "Standoff" ? 3 : 0;
        }

        return Mathf.Clamp(Mathf.RoundToInt(lengthMm / threadPitchMm), 2, 12);
    }

    private static void ParseMetricLength(string raw, out float diameterMm, out float lengthMm, out string metric)
    {
        diameterMm = 0f;
        lengthMm = 0f;
        metric = string.Empty;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        int metricIndex = raw.IndexOf('M');
        if (metricIndex < 0)
        {
            return;
        }

        string tail = raw.Substring(metricIndex + 1);
        int xIndex = tail.IndexOf('x');
        if (xIndex < 0)
        {
            xIndex = tail.IndexOf('X');
        }

        string diameterToken = xIndex >= 0 ? tail.Substring(0, xIndex) : tail;
        string lengthToken = xIndex >= 0 ? tail.Substring(xIndex + 1) : string.Empty;

        diameterToken = new string(diameterToken.TakeWhile(ch => char.IsDigit(ch) || ch == '.').ToArray());
        lengthToken = new string(lengthToken.TakeWhile(ch => char.IsDigit(ch) || ch == '.').ToArray());

        if (float.TryParse(diameterToken, NumberStyles.Float, CultureInfo.InvariantCulture, out float diameterValue))
        {
            diameterMm = diameterValue >= 10f && diameterToken.IndexOf('.') < 0
                ? diameterValue / 10f
                : diameterValue;
        }

        if (float.TryParse(lengthToken, NumberStyles.Float, CultureInfo.InvariantCulture, out float lengthValue))
        {
            lengthMm = lengthValue;
        }

        if (diameterMm > 0.0001f)
        {
            metric = $"M{diameterMm.ToString("0.##", CultureInfo.InvariantCulture)}";
        }
    }
}
