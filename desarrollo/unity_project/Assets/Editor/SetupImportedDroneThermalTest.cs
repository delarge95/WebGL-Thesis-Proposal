using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;

public static class SetupImportedDroneThermalTest
{
    private const string RootName = "x500v2_Drone";
    private const string GeneratedDataFolder = "Assets/Core/Data/X500V2Generated";
    private const string CanonicalJsonFile = "x500v2_parts_data.json";
    private const string SyncedJsonFile = "x500v2_blender_synced_parts.json";
    private const string FastenerGroupId = "x500v2_fastener_group";
    private const string MiscGroupId = "x500v2_misc_group";

    private static string HolybroDocsDir => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "docs", "investigacion", "Holybro"));
    private static string CanonicalJsonPath => Path.Combine(HolybroDocsDir, CanonicalJsonFile);
    private static string SyncedJsonPath => Path.Combine(HolybroDocsDir, SyncedJsonFile);

    [Serializable]
    private class DronePartJsonWrapper
    {
        public DronePartJson[] items;
    }

    [Serializable]
    private class DronePartJson
    {
        public string partName;
        public string id;
        public string blenderName;
        public string partType;
        public string category;
        public string description;
        public string function;
        public float weightKg;
        public string dimensions;
        public string materialType;
        public string materialProperties;
        public string manufacturer;
        public string partNumber;
        public float powerConsumption;
        public float maxLoad;
        public float operatingTemp;
        public float operatingTempMin;
        public float operatingTempMax;
        public float thermalHover;
        public float thermalPeak;
        public float thermalWarmupSeconds;
        public int difficultyLevel;
        public string[] requiredTools;
        public string[] safetyWarnings;
        public string installationTips;
        public float installationTimeMinutes;
        public string torqueSpec;
        public int assemblyOrder;
        public string[] prerequisites;
        public string[] connectionTypes;
        public int screwCount;
        public string screwSize;
        public float[] explosionDirection;
        public float explosionDistance;
        public int explosionPriority;
        public float[] highlightColor;
        public bool isHotspotTarget;
        public string hotspotLabel;
    }

    [Serializable]
    private class SyncedPartJsonWrapper
    {
        public SyncedPartJson[] items;
    }

    [Serializable]
    private class SyncedPhysicsJson
    {
        public float thermalHover;
        public float thermalPeak;
        public float heatingIndex;
    }

    [Serializable]
    private class SyncedUiJson
    {
        public bool isHotspotTarget;
        public string hotspotLabel;
    }

    [Serializable]
    private class SyncedPartJson
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
        public SyncedPhysicsJson physics;
        public SyncedUiJson ui;
    }

    [MenuItem("Tools/Thermal/Prepare Imported Drone For Thermal Test")]
    public static void PrepareImportedDrone()
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null)
        {
            EditorUtility.DisplayDialog("Thermal Test Setup", $"No se encontro '{RootName}' en la escena activa.", "OK");
            return;
        }

        EnsureEditableHierarchy(root);

        DronePartJson[] jsonParts = LoadJsonParts(root.transform, out string selectedSource, out int matchedPartIds);
        if (jsonParts == null || jsonParts.Length == 0)
        {
            EditorUtility.DisplayDialog("Thermal Test Setup", "No se pudo leer el dataset de piezas (synced/canonical).", "OK");
            return;
        }

        EnsureFolder("Assets/Core");
        EnsureFolder("Assets/Core/Data");
        EnsureFolder(GeneratedDataFolder);
        EnsureRuntimeBinder(root);

        Dictionary<string, DronePartData> assetsById = GenerateOrUpdatePartAssets(jsonParts);
        EnsureSyntheticGroupAsset(assetsById, FastenerGroupId, "Fasteners Group", PartCategory.Fasteners);
        EnsureSyntheticGroupAsset(assetsById, MiscGroupId, "Misc Group", PartCategory.Uncategorized);
        Dictionary<string, Transform> anchorsById = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

        int prepared = 0;
        int groupedChildren = 0;
        int auxiliaryReparented = 0;
        int syntheticGroupReparented = 0;
        int prefixReparented = 0;
        int heuristicReparented = 0;
        int warnings = 0;

        foreach (DronePartJson jsonPart in jsonParts)
        {
            if (string.IsNullOrWhiteSpace(jsonPart.id))
            {
                continue;
            }

            List<Transform> matches = FindMatches(root.transform, jsonPart);
            if (matches.Count == 0)
            {
                Debug.LogWarning($"[SetupImportedDroneThermalTest] No se encontraron nodos para {jsonPart.id}");
                warnings++;
                continue;
            }

            Transform anchor = ResolveOrCreateAnchor(root.transform, jsonPart.id, matches);
            if (anchor == null)
            {
                warnings++;
                continue;
            }

            foreach (Transform match in matches)
            {
                if (match == null || match == anchor || match.IsChildOf(anchor))
                {
                    continue;
                }

                Undo.SetTransformParent(match, anchor, "Group imported drone part");
                groupedChildren++;
            }

            if (!assetsById.TryGetValue(jsonPart.id, out DronePartData dataAsset) || dataAsset == null)
            {
                Debug.LogWarning($"[SetupImportedDroneThermalTest] No existe DronePartData generado para {jsonPart.id}");
                warnings++;
                continue;
            }

            ExplodablePart explodable = anchor.GetComponent<ExplodablePart>();
            if (explodable == null)
            {
                explodable = Undo.AddComponent<ExplodablePart>(anchor.gameObject);
            }

            if (anchor.GetComponent<MaterialController>() == null)
            {
                Undo.AddComponent<MaterialController>(anchor.gameObject);
            }

            if (anchor.GetComponent<HighlightSystem>() == null)
            {
                Undo.AddComponent<HighlightSystem>(anchor.gameObject);
            }

            explodable.SetData(dataAsset);
            explodable.Initialize();
            EditorUtility.SetDirty(explodable);

            anchorsById[jsonPart.id] = anchor;
            prepared++;
        }

        auxiliaryReparented = ReparentAuxiliaryChildren(
            root.transform,
            anchorsById,
            assetsById,
            out syntheticGroupReparented,
            out prefixReparented,
            out heuristicReparented);

        foreach (DronePartJson jsonPart in jsonParts)
        {
            if (!anchorsById.TryGetValue(jsonPart.id, out Transform anchor) || anchor == null)
            {
                continue;
            }

            AnnotateRenderHierarchy(anchor, jsonPart);
            EnsureSelectionColliders(anchor);
            AssignSelectableLayer(anchor);

            ExplodablePart explodable = anchor.GetComponent<ExplodablePart>();
            if (explodable != null)
            {
                explodable.Initialize();
            }
        }

        ProcessExistingSyntheticGroupMembers(root.transform, anchorsById, assetsById, FastenerGroupId);
        ProcessExistingSyntheticGroupMembers(root.transform, anchorsById, assetsById, MiscGroupId);
        NormalizeAnchorPivots(anchorsById);

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string message =
            $"Fuente usada: {selectedSource} (matches: {matchedPartIds}/{jsonParts.Length})\n" +
            $"Preparadas {prepared} piezas canonicas para la prueba.\n" +
            $"Hijos agrupados: {groupedChildren}\n" +
            $"Auxiliares reasignados: {auxiliaryReparented}\n" +
            $"  - Por grupo sintetico: {syntheticGroupReparented}\n" +
            $"  - Por prefijo canonico: {prefixReparented}\n" +
            $"  - Por heuristica: {heuristicReparented}\n" +
            $"Warnings: {warnings}";

        Debug.Log($"[SetupImportedDroneThermalTest] {message}");
        EditorUtility.DisplayDialog("Thermal Test Setup", message, "OK");
    }

    private static DronePartJson[] LoadJsonParts(Transform root, out string selectedSource, out int matchedPartIds)
    {
        selectedSource = "N/A";
        matchedPartIds = 0;

        bool hasSynced = TryLoadParts(SyncedJsonPath, out DronePartJson[] syncedParts, useSyncedSchema: true);
        bool hasCanonical = TryLoadParts(CanonicalJsonPath, out DronePartJson[] canonicalParts, useSyncedSchema: false);

        int syncedMatches = hasSynced ? EstimateMatchedParts(root, syncedParts) : -1;
        int canonicalMatches = hasCanonical ? EstimateMatchedParts(root, canonicalParts) : -1;

        if (hasSynced && hasCanonical)
        {
            if (syncedMatches >= canonicalMatches)
            {
                selectedSource = SyncedJsonFile;
                matchedPartIds = syncedMatches;
                Debug.Log($"[SetupImportedDroneThermalTest] Fuente de piezas: {SyncedJsonFile} ({syncedParts.Length} entradas, matches={syncedMatches})");
                return syncedParts;
            }

            selectedSource = CanonicalJsonFile;
            matchedPartIds = canonicalMatches;
            Debug.LogWarning($"[SetupImportedDroneThermalTest] Seleccion automatica de fuente: {CanonicalJsonFile} ({canonicalParts.Length} entradas, matches={canonicalMatches}) por mayor cobertura de escena frente a {SyncedJsonFile} (matches={syncedMatches}).");
            return canonicalParts;
        }

        if (hasSynced)
        {
            selectedSource = SyncedJsonFile;
            matchedPartIds = syncedMatches;
            Debug.Log($"[SetupImportedDroneThermalTest] Fuente de piezas: {SyncedJsonFile} ({syncedParts.Length} entradas, matches={syncedMatches})");
            return syncedParts;
        }

        if (hasCanonical)
        {
            selectedSource = CanonicalJsonFile;
            matchedPartIds = canonicalMatches;
            Debug.LogWarning($"[SetupImportedDroneThermalTest] Fallback a {CanonicalJsonFile} ({canonicalParts.Length} entradas, matches={canonicalMatches})");
            return canonicalParts;
        }

        Debug.LogError($"[SetupImportedDroneThermalTest] No se pudo cargar ni {SyncedJsonFile} ni {CanonicalJsonFile} en {HolybroDocsDir}");
        return null;
    }

    private static int EstimateMatchedParts(Transform root, IReadOnlyList<DronePartJson> parts)
    {
        if (root == null || parts == null || parts.Count == 0)
        {
            return 0;
        }

        int matched = 0;
        foreach (DronePartJson part in parts)
        {
            if (part == null || string.IsNullOrWhiteSpace(part.id))
            {
                continue;
            }

            if (FindMatches(root, part).Count > 0)
            {
                matched++;
            }
        }

        return matched;
    }

    private static bool TryLoadParts(string path, out DronePartJson[] parts, bool useSyncedSchema)
    {
        parts = null;
        if (!File.Exists(path))
        {
            return false;
        }

        string raw = File.ReadAllText(path);
        string wrapped = "{\"items\":" + raw + "}";

        if (useSyncedSchema)
        {
            SyncedPartJsonWrapper synced = JsonUtility.FromJson<SyncedPartJsonWrapper>(wrapped);
            if (synced?.items == null || synced.items.Length == 0)
            {
                return false;
            }

            parts = ConvertSyncedParts(synced.items);
            return parts.Length > 0;
        }

        DronePartJsonWrapper canonical = JsonUtility.FromJson<DronePartJsonWrapper>(wrapped);
        parts = canonical?.items;
        return parts != null && parts.Length > 0;
    }

    private static DronePartJson[] ConvertSyncedParts(SyncedPartJson[] synced)
    {
        List<DronePartJson> output = new List<DronePartJson>(synced.Length);
        foreach (SyncedPartJson item in synced)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.id))
            {
                continue;
            }

            float thermalHover = item.physics != null ? item.physics.thermalHover : 0f;
            float thermalPeak = item.physics != null ? item.physics.thermalPeak : 0f;
            float heatingIndex = item.physics != null ? item.physics.heatingIndex : 0f;

            DronePartJson part = new DronePartJson
            {
                partName = item.partName,
                id = item.id,
                blenderName = item.blenderName,
                partType = !string.IsNullOrWhiteSpace(item.category) ? item.category : item.blenderName,
                category = item.category,
                description = item.description,
                function = item.function,
                weightKg = item.weightKg,
                dimensions = item.dimensions,
                materialType = item.materialType,
                manufacturer = item.manufacturer,
                thermalHover = thermalHover,
                thermalPeak = thermalPeak,
                thermalWarmupSeconds = GuessWarmupSeconds(heatingIndex),
                operatingTemp = thermalPeak > 0f ? thermalPeak : thermalHover,
                operatingTempMin = Math.Max(0f, thermalHover - 15f),
                operatingTempMax = thermalPeak > 0f ? thermalPeak : thermalHover + 20f,
                isHotspotTarget = item.ui != null && item.ui.isHotspotTarget,
                hotspotLabel = item.ui != null ? item.ui.hotspotLabel : string.Empty,
                requiredTools = Array.Empty<string>(),
                safetyWarnings = Array.Empty<string>(),
                prerequisites = Array.Empty<string>(),
                connectionTypes = Array.Empty<string>(),
                explosionDirection = new[] { GuessExplosionDirection(item.id).x, GuessExplosionDirection(item.id).y, GuessExplosionDirection(item.id).z },
                explosionDistance = GuessExplosionDistance(item.id, item.category),
                highlightColor = new[] { GuessHighlightColor(item.id, item.category).r, GuessHighlightColor(item.id, item.category).g, GuessHighlightColor(item.id, item.category).b, GuessHighlightColor(item.id, item.category).a },
                explosionPriority = 0
            };

            output.Add(part);
        }

        return output.ToArray();
    }

    private static Dictionary<string, DronePartData> GenerateOrUpdatePartAssets(IEnumerable<DronePartJson> jsonParts)
    {
        Dictionary<string, DronePartData> assetsById = new Dictionary<string, DronePartData>(StringComparer.OrdinalIgnoreCase);

        foreach (DronePartJson jsonPart in jsonParts)
        {
            if (jsonPart == null || string.IsNullOrWhiteSpace(jsonPart.id))
            {
                continue;
            }

            string assetPath = $"{GeneratedDataFolder}/{jsonPart.id}.asset";
            DronePartData asset = AssetDatabase.LoadAssetAtPath<DronePartData>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<DronePartData>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            ApplyJsonToAsset(asset, jsonPart);
            EditorUtility.SetDirty(asset);
            assetsById[jsonPart.id] = asset;
        }

        return assetsById;
    }

    private static void ApplyJsonToAsset(DronePartData asset, DronePartJson jsonPart)
    {
        asset.partName = jsonPart.partName;
        asset.id = jsonPart.id;
        asset.partType = jsonPart.partType;
        asset.category = NormalizePartCategory(jsonPart.category, jsonPart.id, jsonPart.partType);
        asset.description = jsonPart.description;
        asset.function = jsonPart.function;
        asset.weightKg = jsonPart.weightKg;
        asset.dimensions = jsonPart.dimensions;
        asset.materialType = jsonPart.materialType;
        asset.materialProperties = jsonPart.materialProperties;
        asset.manufacturer = jsonPart.manufacturer;
        asset.partNumber = jsonPart.partNumber;
        asset.powerConsumption = jsonPart.powerConsumption;
        asset.maxLoad = jsonPart.maxLoad;
        asset.operatingTemp = jsonPart.operatingTemp;
        asset.operatingTempMin = jsonPart.operatingTempMin;
        asset.operatingTempMax = jsonPart.operatingTempMax;
        asset.thermalHover = jsonPart.thermalHover;
        asset.thermalPeak = jsonPart.thermalPeak;
        asset.thermalWarmupSeconds = jsonPart.thermalWarmupSeconds;
        asset.requiredTools = jsonPart.requiredTools ?? Array.Empty<string>();
        asset.safetyWarnings = jsonPart.safetyWarnings ?? Array.Empty<string>();
        asset.installationTips = jsonPart.installationTips;
        asset.installationTimeMinutes = jsonPart.installationTimeMinutes;
        asset.difficultyLevel = jsonPart.difficultyLevel;
        asset.torqueSpec = jsonPart.torqueSpec;
        asset.assemblyOrder = jsonPart.assemblyOrder;
        asset.prerequisites = jsonPart.prerequisites ?? Array.Empty<string>();
        asset.connectionTypes = jsonPart.connectionTypes ?? Array.Empty<string>();
        asset.screwCount = jsonPart.screwCount;
        asset.screwSize = jsonPart.screwSize;
        asset.explosionDirection = ToVector3(jsonPart.explosionDirection, GuessExplosionDirection(jsonPart.id));
        asset.explosionDistance = jsonPart.explosionDistance > 0f ? jsonPart.explosionDistance : GuessExplosionDistance(jsonPart.id, jsonPart.partType);
        asset.explosionPriority = jsonPart.explosionPriority;
        asset.highlightColor = ToColor(jsonPart.highlightColor, GuessHighlightColor(jsonPart.id, jsonPart.partType));
        asset.isThermallyCritical = IsThermallyCritical(jsonPart.id, jsonPart.partType);
        asset.thermalExposure = GuessThermalExposure(jsonPart.id, jsonPart.partType);
        asset.thermalSourceWeight = GuessSourceWeight(jsonPart.id, jsonPart.partType);
        asset.thermalConductionScale = 1f;
        asset.isHotspotTarget = jsonPart.isHotspotTarget;
        asset.hotspotLabel = jsonPart.hotspotLabel;
    }

    private static List<Transform> FindMatches(Transform root, string canonicalId)
    {
        List<Transform> matches = new List<Transform>();
        string dottedPrefix = canonicalId + ".";

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child == null || child == root)
            {
                continue;
            }

            string name = child.name;
            if (string.Equals(name, canonicalId, StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith(dottedPrefix, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(child);
            }
        }

        return matches;
    }

    private static List<Transform> FindMatches(Transform root, DronePartJson jsonPart)
    {
        List<Transform> byCanonicalId = FindMatches(root, jsonPart.id);
        if (byCanonicalId.Count > 0)
        {
            return byCanonicalId;
        }

        if (string.IsNullOrWhiteSpace(jsonPart.blenderName))
        {
            return byCanonicalId;
        }

        List<Transform> matches = new List<Transform>();
        string dottedPrefix = jsonPart.blenderName + ".";

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child == null || child == root)
            {
                continue;
            }

            string name = child.name;
            if (string.Equals(name, jsonPart.blenderName, StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith(dottedPrefix, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(child);
            }
        }

        return matches;
    }

    private static Transform ResolveOrCreateAnchor(Transform root, string canonicalId, List<Transform> matches)
    {
        foreach (Transform match in matches)
        {
            if (string.Equals(match.name, canonicalId, StringComparison.OrdinalIgnoreCase))
            {
                return match;
            }
        }

        Bounds bounds = default;
        bool hasBounds = false;
        foreach (Transform match in matches)
        {
            Renderer[] renderers = match.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                continue;
            }

            foreach (Renderer renderer in renderers)
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
        }

        GameObject anchorObject = new GameObject(canonicalId);
        Undo.RegisterCreatedObjectUndo(anchorObject, "Create canonical drone anchor");
        anchorObject.transform.SetParent(root, true);
        anchorObject.transform.localScale = Vector3.one;
        anchorObject.transform.rotation = root.rotation;
        Vector3 fallbackCenter;
        bool hasFallbackCenter = TryComputeWorldCenterFromDescendants(root, out fallbackCenter);
        anchorObject.transform.position = hasBounds
            ? bounds.center
            : (hasFallbackCenter ? fallbackCenter : root.position);
        return anchorObject.transform;
    }

    private static bool TryComputeWorldCenterFromDescendants(Transform root, out Vector3 center)
    {
        center = Vector3.zero;
        if (root == null)
        {
            return false;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        bool hasRendererBounds = false;
        Bounds aggregate = default;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Transform rendererTransform = renderer.transform;
            if (rendererTransform == root)
            {
                continue;
            }

            if (!hasRendererBounds)
            {
                aggregate = renderer.bounds;
                hasRendererBounds = true;
            }
            else
            {
                aggregate.Encapsulate(renderer.bounds);
            }
        }

        if (hasRendererBounds)
        {
            center = aggregate.center;
            return true;
        }

        int childCount = root.childCount;
        if (childCount == 0)
        {
            return false;
        }

        Vector3 accumulator = Vector3.zero;
        int counted = 0;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child == null)
            {
                continue;
            }

            accumulator += child.position;
            counted++;
        }

        if (counted == 0)
        {
            return false;
        }

        center = accumulator / counted;
        return true;
    }

    private static void NormalizeAnchorPivots(IReadOnlyDictionary<string, Transform> anchorsById)
    {
        if (anchorsById == null || anchorsById.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, Transform> kvp in anchorsById)
        {
            Transform anchor = kvp.Value;
            if (anchor == null)
            {
                continue;
            }

            RecenterAnchorPivotWithoutMovingGeometry(anchor);
        }
    }

    private static void RecenterAnchorPivotWithoutMovingGeometry(Transform anchor)
    {
        if (anchor == null || anchor.childCount == 0)
        {
            return;
        }

        Renderer[] renderers = anchor.GetComponentsInChildren<Renderer>(true);
        Bounds aggregate = default;
        bool hasBounds = false;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (renderer.transform == anchor)
            {
                continue;
            }

            if (!hasBounds)
            {
                aggregate = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                aggregate.Encapsulate(renderer.bounds);
            }
        }

        if (!hasBounds)
        {
            return;
        }

        Vector3 previous = anchor.position;
        Vector3 target = aggregate.center;
        Vector3 delta = target - previous;

        if (delta.sqrMagnitude < 0.0000001f)
        {
            return;
        }

        List<Transform> directChildren = new List<Transform>();
        foreach (Transform child in anchor)
        {
            directChildren.Add(child);
        }

        Undo.RecordObject(anchor, "Recenter anchor pivot");
        Undo.RecordObjects(directChildren.ConvertAll(child => (UnityEngine.Object)child).ToArray(), "Keep child world positions");

        anchor.position = target;
        foreach (Transform child in directChildren)
        {
            if (child == null)
            {
                continue;
            }

            child.position -= delta;
        }
    }

    private static int ReparentAuxiliaryChildren(
        Transform root,
        Dictionary<string, Transform> anchorsById,
        IReadOnlyDictionary<string, DronePartData> assetsById,
        out int syntheticMoved,
        out int prefixedMoved,
        out int heuristicMoved)
    {
        syntheticMoved = 0;
        prefixedMoved = 0;
        heuristicMoved = 0;
        int moved = 0;
        List<Transform> directChildren = new List<Transform>();
        foreach (Transform child in root)
        {
            directChildren.Add(child);
        }

        foreach (Transform child in directChildren)
        {
            if (child == null || child.GetComponent<ExplodablePart>() != null)
            {
                continue;
            }

            string syntheticGroupId = ResolveSyntheticGroupIdFromName(child.name);
            if (!string.IsNullOrWhiteSpace(syntheticGroupId))
            {
                Transform syntheticAnchor = EnsureSyntheticGroupAnchor(root, anchorsById, assetsById, syntheticGroupId);
                if (syntheticAnchor != null && !child.IsChildOf(syntheticAnchor))
                {
                    Undo.SetTransformParent(child, syntheticAnchor, "Attach auxiliary imported part (synthetic-group)");
                    moved++;
                    syntheticMoved++;
                }

                EnsureSyntheticInstanceSelectable(child, syntheticGroupId);
                continue;
            }

            Transform prefixedAnchor = ResolveAnchorFromNamePrefix(child.name, anchorsById);
            if (prefixedAnchor != null && !child.IsChildOf(prefixedAnchor))
            {
                Undo.SetTransformParent(child, prefixedAnchor, "Attach auxiliary imported part (prefix)");
                moved++;
                prefixedMoved++;
                continue;
            }

            Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                continue;
            }

            Transform bestAnchor = ResolveBestAnchor(child, renderers[0], anchorsById);
            if (bestAnchor == null || child.IsChildOf(bestAnchor))
            {
                continue;
            }

            Undo.SetTransformParent(child, bestAnchor, "Attach auxiliary imported part");
            moved++;
            heuristicMoved++;
        }

        return moved;
    }

    private static string ResolveSyntheticGroupIdFromName(string candidateName)
    {
        if (string.IsNullOrWhiteSpace(candidateName))
        {
            return string.Empty;
        }

        if (candidateName.StartsWith("x500v2_fastener.", StringComparison.OrdinalIgnoreCase))
        {
            return FastenerGroupId;
        }

        if (candidateName.IndexOf("x500v2_fastener.", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return FastenerGroupId;
        }

        if (candidateName.StartsWith("x500v2_misc.", StringComparison.OrdinalIgnoreCase))
        {
            return MiscGroupId;
        }

        if (candidateName.IndexOf("x500v2_misc.", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return MiscGroupId;
        }

        return string.Empty;
    }

    private static void EnsureEditableHierarchy(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        if (!PrefabUtility.IsPartOfPrefabInstance(root))
        {
            return;
        }

        PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        Debug.Log("[SetupImportedDroneThermalTest] Prefab instance unpacked to allow hierarchy reparent operations.");
    }

    private static Transform EnsureSyntheticGroupAnchor(
        Transform root,
        Dictionary<string, Transform> anchorsById,
        IReadOnlyDictionary<string, DronePartData> assetsById,
        string groupId)
    {
        if (root == null || string.IsNullOrWhiteSpace(groupId))
        {
            return null;
        }

        if (anchorsById.TryGetValue(groupId, out Transform existing) && existing != null)
        {
            return existing;
        }

        Transform anchor = root.Find(groupId);
        if (anchor == null)
        {
            GameObject anchorObject = new GameObject(groupId);
            Undo.RegisterCreatedObjectUndo(anchorObject, "Create synthetic group anchor");
            anchor = anchorObject.transform;
            anchor.SetParent(root, true);
            anchor.localScale = Vector3.one;
            Vector3 fallbackCenter;
            bool hasFallbackCenter = TryComputeWorldCenterFromDescendants(root, out fallbackCenter);
            anchor.position = hasFallbackCenter ? fallbackCenter : root.position;
        }

        if (anchor.GetComponent<MaterialController>() == null)
        {
            Undo.AddComponent<MaterialController>(anchor.gameObject);
        }

        if (anchor.GetComponent<HighlightSystem>() == null)
        {
            Undo.AddComponent<HighlightSystem>(anchor.gameObject);
        }

        ExplodablePart explodable = anchor.GetComponent<ExplodablePart>();
        if (explodable == null)
        {
            explodable = Undo.AddComponent<ExplodablePart>(anchor.gameObject);
        }

        if (assetsById.TryGetValue(groupId, out DronePartData dataAsset) && dataAsset != null)
        {
            explodable.SetData(dataAsset);
            explodable.Initialize();
            EditorUtility.SetDirty(explodable);
        }

        anchorsById[groupId] = anchor;
        return anchor;
    }

    private static void EnsureSyntheticGroupAsset(
        Dictionary<string, DronePartData> assetsById,
        string id,
        string partName,
        PartCategory category)
    {
        if (assetsById.TryGetValue(id, out DronePartData existing) && existing != null)
        {
            return;
        }

        string assetPath = $"{GeneratedDataFolder}/{id}.asset";
        DronePartData asset = AssetDatabase.LoadAssetAtPath<DronePartData>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<DronePartData>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        asset.partName = partName;
        asset.id = id;
        asset.partType = partName;
        asset.category = category;
        asset.description = "Synthetic grouping anchor for imported geometry.";
        asset.function = "Grouping anchor";
        asset.requiredTools = Array.Empty<string>();
        asset.safetyWarnings = Array.Empty<string>();
        asset.prerequisites = Array.Empty<string>();
        asset.connectionTypes = Array.Empty<string>();
        asset.explosionDirection = Vector3.up;
        asset.explosionDistance = 0.1f;
        asset.highlightColor = new Color(0.3f, 0.8f, 1f, 1f);
        asset.isThermallyCritical = false;
        asset.isHotspotTarget = false;

        EditorUtility.SetDirty(asset);
        assetsById[id] = asset;
    }

    private static void ProcessSyntheticGroupAnchor(Transform anchor)
    {
        if (anchor == null)
        {
            return;
        }

        // Keep the group anchor as an empty organizational container.
        // Individual selectable parts are attached to the child instance nodes.
        ExplodablePart existingExplodable = anchor.GetComponent<ExplodablePart>();
        if (existingExplodable != null)
        {
            Undo.DestroyObjectImmediate(existingExplodable);
        }

        MaterialController existingMaterial = anchor.GetComponent<MaterialController>();
        if (existingMaterial != null)
        {
            Undo.DestroyObjectImmediate(existingMaterial);
        }

        HighlightSystem existingHighlight = anchor.GetComponent<HighlightSystem>();
        if (existingHighlight != null)
        {
            Undo.DestroyObjectImmediate(existingHighlight);
        }
    }

    private static void ProcessExistingSyntheticGroupMembers(
        Transform root,
        Dictionary<string, Transform> anchorsById,
        IReadOnlyDictionary<string, DronePartData> assetsById,
        string groupId)
    {
        Transform groupAnchor = EnsureSyntheticGroupAnchor(root, anchorsById, assetsById, groupId);
        if (groupAnchor == null)
        {
            return;
        }

        ProcessSyntheticGroupAnchor(groupAnchor);

        List<Transform> directChildren = new List<Transform>();
        foreach (Transform child in groupAnchor)
        {
            directChildren.Add(child);
        }

        foreach (Transform child in directChildren)
        {
            if (child == null)
            {
                continue;
            }

            Renderer renderer = child.GetComponentInChildren<Renderer>(true);
            if (renderer == null)
            {
                continue;
            }

            EnsureSyntheticInstanceSelectable(child, groupId);
        }
    }

    private static void EnsureSyntheticInstanceSelectable(Transform child, string syntheticGroupId)
    {
        if (child == null || string.IsNullOrWhiteSpace(syntheticGroupId))
        {
            return;
        }

        string assetId = SanitizeAssetId(child.name);
        string assetPath = $"{GeneratedDataFolder}/{assetId}.asset";
        DronePartData asset = AssetDatabase.LoadAssetAtPath<DronePartData>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<DronePartData>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        string readableName = child.name.Replace('_', ' ').Replace('.', ' ');
        bool isFastenerGroup = string.Equals(syntheticGroupId, FastenerGroupId, StringComparison.OrdinalIgnoreCase);

        asset.partName = readableName;
        asset.id = assetId;
        asset.partType = isFastenerGroup ? "Fastener" : "Misc Part";
        asset.category = isFastenerGroup ? PartCategory.Fasteners : PartCategory.Uncategorized;
        asset.description = isFastenerGroup
            ? "Individual fastener instance imported from the model hierarchy."
            : "Individual auxiliary instance imported from the model hierarchy.";
        asset.function = isFastenerGroup ? "Fastening / mounting" : "Auxiliary support";
        asset.weightKg = 0f;
        asset.dimensions = string.Empty;
        asset.materialType = isFastenerGroup ? "Metal" : "Mixed";
        asset.requiredTools = Array.Empty<string>();
        asset.safetyWarnings = Array.Empty<string>();
        asset.prerequisites = Array.Empty<string>();
        asset.connectionTypes = Array.Empty<string>();
        asset.explosionDirection = Vector3.up;
        asset.explosionDistance = 0.03f;
        asset.highlightColor = isFastenerGroup
            ? new Color(1f, 0.72f, 0.2f, 1f)
            : new Color(0.3f, 0.8f, 1f, 1f);
        asset.isThermallyCritical = false;
        asset.isHotspotTarget = false;

        DronePartJson syntheticPart = new DronePartJson
        {
            id = assetId,
            partName = readableName,
            category = isFastenerGroup ? "Fasteners" : "Misc",
            partType = asset.partType,
            description = asset.description,
            function = asset.function,
            materialType = asset.materialType,
            weightKg = asset.weightKg,
            dimensions = asset.dimensions
        };

        ExplodablePart explodable = child.GetComponent<ExplodablePart>();
        if (explodable == null)
        {
            explodable = Undo.AddComponent<ExplodablePart>(child.gameObject);
        }

        if (child.GetComponent<MaterialController>() == null)
        {
            Undo.AddComponent<MaterialController>(child.gameObject);
        }

        if (child.GetComponent<HighlightSystem>() == null)
        {
            Undo.AddComponent<HighlightSystem>(child.gameObject);
        }

        explodable.SetData(asset);
        explodable.Initialize();

        AnnotateRenderHierarchy(child, syntheticPart);
        EnsureSelectionColliders(child);
        AssignSelectableLayer(child);

        EditorUtility.SetDirty(asset);
        EditorUtility.SetDirty(explodable);
    }

    private static string SanitizeAssetId(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return "synthetic_instance";
        }

        char[] buffer = new char[rawName.Length];
        int length = 0;
        foreach (char character in rawName)
        {
            buffer[length++] = char.IsLetterOrDigit(character) || character == '_' || character == '-' ? character : '_';
        }

        return new string(buffer, 0, length);
    }

    private static Transform ResolveAnchorFromNamePrefix(string candidateName, IReadOnlyDictionary<string, Transform> anchorsById)
    {
        if (string.IsNullOrWhiteSpace(candidateName) || anchorsById == null || anchorsById.Count == 0)
        {
            return null;
        }

        if (anchorsById.TryGetValue(candidateName, out Transform exact))
        {
            return exact;
        }

        int dot = candidateName.IndexOf('.');
        if (dot > 0)
        {
            string prefix = candidateName.Substring(0, dot);
            if (anchorsById.TryGetValue(prefix, out Transform byPrefix))
            {
                return byPrefix;
            }
        }

        foreach (KeyValuePair<string, Transform> kvp in anchorsById)
        {
            if (candidateName.StartsWith(kvp.Key + ".", StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    private static Transform ResolveBestAnchor(Transform candidate, Renderer renderer, IReadOnlyDictionary<string, Transform> anchorsById)
    {
        if (candidate == null || anchorsById == null || anchorsById.Count == 0)
        {
            return null;
        }

        string lowerName = candidate.name.ToLowerInvariant();
        string suffix = ResolveQuadrantSuffix(lowerName);
        Vector3 candidateCenter = renderer != null ? renderer.bounds.center : candidate.position;

        float bestScore = float.MaxValue;
        Transform bestAnchor = null;

        foreach (KeyValuePair<string, Transform> kvp in anchorsById)
        {
            Transform anchor = kvp.Value;
            if (anchor == null)
            {
                continue;
            }

            float score = Vector3.Distance(candidateCenter, anchor.position);
            string anchorId = kvp.Key.ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(suffix) && anchorId.EndsWith("_" + suffix, StringComparison.Ordinal))
            {
                score -= 2f;
            }

            if (lowerName.Contains("motor") && anchorId.Contains("motor")) score -= 1.5f;
            if (lowerName.Contains("esc") && anchorId.Contains("esc")) score -= 1.5f;
            if (lowerName.Contains("prop") && anchorId.Contains("prop")) score -= 1.5f;
            if (lowerName.Contains("arm") && anchorId.Contains("arm")) score -= 1.2f;
            if (lowerName.Contains("battery") && anchorId.Contains("battery")) score -= 1.2f;
            if (lowerName.Contains("plate") && anchorId.Contains("plate")) score -= 0.9f;

            if (score < bestScore)
            {
                bestScore = score;
                bestAnchor = anchor;
            }
        }

        return bestAnchor;
    }

    private static void AnnotateRenderHierarchy(Transform anchor, DronePartJson jsonPart)
    {
        Renderer[] renderers = anchor.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
            if (category == null)
            {
                category = Undo.AddComponent<PartRenderCategory>(renderer.gameObject);
            }

            string auxiliaryCategory = InferAuxiliaryCategory(renderer.transform.name);
            string thermalSourceId = InferThermalSourcePartId(renderer.transform.name, jsonPart.id);
            string primaryCategory = InferDisplayCategory(renderer.transform.name, jsonPart.category, thermalSourceId);
            category.Configure(jsonPart.id, primaryCategory, auxiliaryCategory, thermalSourceId);
            EditorUtility.SetDirty(category);
        }
    }

    private static string InferAuxiliaryCategory(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        string name = rawName.ToLowerInvariant();

        if (name.Contains("fastener") || name.Contains("screw") || name.Contains("cap_screw") || name.Contains("bolt") ||
            name.Contains("nut") || name.Contains("washer") || name.Contains("standoff") || name.Contains("spacer"))
        {
            return "Fasteners";
        }

        if (name.Contains("misc") || name.Contains("clip") || name.Contains("locator") || name.Contains("clamp") ||
            name.Contains("brace") || name.Contains("guide") || name.Contains("connector") || name.Contains("holder"))
        {
            return "Uncategorized";
        }

        return string.Empty;
    }

    private static string InferDisplayCategory(string rendererName, string fallbackCategory, string thermalSourceId)
    {
        string normalized = (thermalSourceId ?? rendererName ?? string.Empty).ToLowerInvariant();
        if (normalized.Contains("motor") || normalized.Contains("prop") || normalized.Contains("esc"))
        {
            return "PropulsionSystem";
        }

        if (normalized.Contains("battery") || normalized.Contains("gps") || normalized.Contains("pixhawk") ||
            normalized.Contains("pdb") || normalized.Contains("power_module") || normalized.Contains("receiver") ||
            normalized.Contains("telemetry") || normalized.Contains("radio"))
        {
            if (normalized.Contains("battery") || normalized.Contains("pdb") || normalized.Contains("power_module"))
            {
                return "PowerDistribution";
            }

            if (normalized.Contains("pixhawk"))
            {
                return "Avionics";
            }

            return "SensorsComms";
        }

        if (normalized.Contains("arm") || normalized.Contains("plate") || normalized.Contains("landing") ||
            normalized.Contains("platform") || normalized.Contains("rail"))
        {
            return "SkeletonAirframe";
        }

        string fallback = NormalizeDisplayCategory(fallbackCategory);
        return string.IsNullOrWhiteSpace(fallback) ? "Uncategorized" : fallback;
    }

    private static PartCategory NormalizePartCategory(string rawCategory, string canonicalId, string partType)
    {
        string normalized = NormalizeDisplayCategory(rawCategory);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = InferDisplayCategory(canonicalId, partType, canonicalId);
        }

        if (Enum.TryParse(normalized, true, out PartCategory parsed))
        {
            return parsed;
        }

        return PartCategory.Uncategorized;
    }

    private static string NormalizeDisplayCategory(string rawCategory)
    {
        if (string.IsNullOrWhiteSpace(rawCategory))
        {
            return string.Empty;
        }

        string compact = rawCategory.Replace(" ", string.Empty).Replace("&", string.Empty).Replace("/", string.Empty).ToLowerInvariant();
        return compact switch
        {
            "structure" => "SkeletonAirframe",
            "skeletonairframe" => "SkeletonAirframe",
            "propulsion" => "PropulsionSystem",
            "propulsionsystem" => "PropulsionSystem",
            "electronics" => "SensorsComms",
            "sensorscomms" => "SensorsComms",
            "avionics" => "Avionics",
            "power" => "PowerDistribution",
            "powerdistribution" => "PowerDistribution",
            "fasteners" => "Fasteners",
            "misc" => "Uncategorized",
            "uncategorized" => "Uncategorized",
            _ => rawCategory.Trim()
        };
    }

    private static float GuessWarmupSeconds(float heatingIndex)
    {
        if (heatingIndex <= 0f)
        {
            return 25f;
        }

        return Mathf.Clamp(65f - (heatingIndex * 4f), 10f, 60f);
    }

    private static string InferThermalSourcePartId(string rendererName, string anchorId)
    {
        string lowerName = (rendererName ?? string.Empty).ToLowerInvariant();
        string suffix = ResolveQuadrantSuffix(lowerName);
        if (string.IsNullOrWhiteSpace(suffix))
        {
            suffix = ResolveQuadrantSuffix((anchorId ?? string.Empty).ToLowerInvariant());
        }

        if (lowerName.Contains("prop") && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_prop_{suffix.ToUpperInvariant()}";
        if (lowerName.Contains("motor") && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_motor_{suffix.ToUpperInvariant()}";
        if (lowerName.Contains("esc") && !string.IsNullOrWhiteSpace(suffix)) return $"x500v2_esc_{suffix.ToUpperInvariant()}";
        if (lowerName.Contains("battery")) return "x500v2_battery";
        if (lowerName.Contains("pixhawk")) return "x500v2_pixhawk6c";
        if (lowerName.Contains("gps")) return "x500v2_gps_m10";
        if (lowerName.Contains("pdb")) return "x500v2_pdb";
        if (lowerName.Contains("power_module")) return "x500v2_power_module";
        if (lowerName.Contains("receiver")) return "x500v2_rc_receiver";
        if (lowerName.Contains("telemetry") || lowerName.Contains("radio")) return "x500v2_telemetry_radio";
        if (lowerName.Contains("rail")) return "x500v2_rails_battery";
        if (lowerName.Contains("landing")) return "x500v2_landing_gear";
        if (lowerName.Contains("top_plate")) return "x500v2_top_plate";
        if (lowerName.Contains("bottom_plate")) return "x500v2_bottom_plate";

        return anchorId;
    }

    private static string ResolveQuadrantSuffix(string lowerName)
    {
        if (lowerName.Contains("_fl")) return "fl";
        if (lowerName.Contains("_fr")) return "fr";
        if (lowerName.Contains("_bl")) return "bl";
        if (lowerName.Contains("_br")) return "br";
        return string.Empty;
    }

    private static void EnsureSelectionColliders(Transform anchor)
    {
        Renderer[] renderers = anchor.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                continue;
            }

            if (renderer.GetComponent<Collider>() != null)
            {
                continue;
            }

            BoxCollider collider = Undo.AddComponent<BoxCollider>(renderer.gameObject);
            collider.center = meshFilter.sharedMesh.bounds.center;
            collider.size = meshFilter.sharedMesh.bounds.size;
        }
    }

    private static void AssignSelectableLayer(Transform anchor)
    {
        int selectableLayer = LayerMask.NameToLayer("SelectablePart");
        if (anchor == null || selectableLayer < 0)
        {
            return;
        }

        foreach (Transform child in anchor.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = selectableLayer;
        }
    }

    private static void EnsureRuntimeBinder(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        if (root.GetComponent<ImportedDroneRuntimeBinder>() == null)
        {
            Undo.AddComponent<ImportedDroneRuntimeBinder>(root);
        }
    }

    private static Vector3 ToVector3(float[] values, Vector3 fallback)
    {
        if (values != null && values.Length >= 3)
        {
            return new Vector3(values[0], values[1], values[2]);
        }

        return fallback;
    }

    private static Color ToColor(float[] values, Color fallback)
    {
        if (values != null && values.Length >= 4)
        {
            return new Color(values[0], values[1], values[2], values[3]);
        }

        return fallback;
    }

    private static bool IsThermallyCritical(string canonicalId, string partType)
    {
        string normalizedType = (partType ?? string.Empty).ToLowerInvariant();
        return normalizedType.Contains("motor")
            || normalizedType.Contains("esc")
            || normalizedType.Contains("battery")
            || normalizedType.Contains("arm")
            || string.Equals(canonicalId, "x500v2_pdb", StringComparison.OrdinalIgnoreCase)
            || string.Equals(canonicalId, "x500v2_power_module", StringComparison.OrdinalIgnoreCase)
            || string.Equals(canonicalId, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase)
            || string.Equals(canonicalId, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase)
            || string.Equals(canonicalId, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase);
    }

    private static float GuessThermalExposure(string canonicalId, string partType)
    {
        string normalizedType = (partType ?? string.Empty).ToLowerInvariant();
        if (normalizedType.Contains("propeller") || normalizedType.Contains("landing")) return 0.95f;
        if (normalizedType.Contains("arm") || normalizedType.Contains("motor") || normalizedType.Contains("esc")) return 0.8f;
        if (string.Equals(canonicalId, "x500v2_battery", StringComparison.OrdinalIgnoreCase)) return 0.35f;
        if (string.Equals(canonicalId, "x500v2_pdb", StringComparison.OrdinalIgnoreCase) || string.Equals(canonicalId, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase)) return 0.45f;
        return 0.55f;
    }

    private static float GuessSourceWeight(string canonicalId, string partType)
    {
        string normalizedType = (partType ?? string.Empty).ToLowerInvariant();
        if (normalizedType.Contains("motor")) return 1f;
        if (normalizedType.Contains("esc")) return 0.92f;
        if (normalizedType.Contains("battery")) return 0.8f;
        if (string.Equals(canonicalId, "x500v2_pdb", StringComparison.OrdinalIgnoreCase)) return 0.45f;
        if (string.Equals(canonicalId, "x500v2_power_module", StringComparison.OrdinalIgnoreCase) || string.Equals(canonicalId, "x500v2_pixhawk6c", StringComparison.OrdinalIgnoreCase)) return 0.5f;
        return 0.2f;
    }

    private static Vector3 GuessExplosionDirection(string canonicalId)
    {
        if (canonicalId.IndexOf("_prop_", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (canonicalId.EndsWith("_FL", StringComparison.OrdinalIgnoreCase)) return new Vector3(-1f, 0f, 1f);
            if (canonicalId.EndsWith("_FR", StringComparison.OrdinalIgnoreCase)) return new Vector3(1f, 0f, 1f);
            if (canonicalId.EndsWith("_BL", StringComparison.OrdinalIgnoreCase)) return new Vector3(-1f, 0f, -1f);
            if (canonicalId.EndsWith("_BR", StringComparison.OrdinalIgnoreCase)) return new Vector3(1f, 0f, -1f);
        }

        if (canonicalId.EndsWith("_FL", StringComparison.OrdinalIgnoreCase)) return new Vector3(-1f, 0f, 1f);
        if (canonicalId.EndsWith("_FR", StringComparison.OrdinalIgnoreCase)) return new Vector3(1f, 0f, 1f);
        if (canonicalId.EndsWith("_BL", StringComparison.OrdinalIgnoreCase)) return new Vector3(-1f, 0f, -1f);
        if (canonicalId.EndsWith("_BR", StringComparison.OrdinalIgnoreCase)) return new Vector3(1f, 0f, -1f);
        if (string.Equals(canonicalId, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase)) return Vector3.up;
        if (string.Equals(canonicalId, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase)) return Vector3.down;
        if (string.Equals(canonicalId, "x500v2_battery", StringComparison.OrdinalIgnoreCase)) return new Vector3(0f, 0f, -1f);
        return Vector3.up;
    }

    private static float GuessExplosionDistance(string canonicalId, string partType)
    {
        string normalizedType = (partType ?? string.Empty).ToLowerInvariant();
        if (normalizedType.Contains("propeller")) return 0.16f;
        if (normalizedType.Contains("motor")) return 0.22f;
        if (normalizedType.Contains("esc")) return 0.18f;
        if (normalizedType.Contains("arm")) return 0.42f;
        if (normalizedType.Contains("battery")) return 0.18f;
        if (normalizedType.Contains("landing")) return 0.18f;
        if (string.Equals(canonicalId, "x500v2_top_plate", StringComparison.OrdinalIgnoreCase) || string.Equals(canonicalId, "x500v2_bottom_plate", StringComparison.OrdinalIgnoreCase)) return 0.16f;
        return 0.2f;
    }

    private static Color GuessHighlightColor(string canonicalId, string partType)
    {
        string normalizedType = (partType ?? string.Empty).ToLowerInvariant();
        if (normalizedType.Contains("motor") || normalizedType.Contains("esc")) return new Color(1f, 0.45f, 0.15f, 1f);
        if (normalizedType.Contains("battery")) return new Color(0.95f, 0.8f, 0.2f, 1f);
        if (normalizedType.Contains("arm") || canonicalId.IndexOf("plate", StringComparison.OrdinalIgnoreCase) >= 0) return new Color(0.35f, 0.75f, 1f, 1f);
        return new Color(0.3f, 0.8f, 1f, 1f);
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
        string name = Path.GetFileName(folderPath);
        if (!string.IsNullOrWhiteSpace(parent) && !string.IsNullOrWhiteSpace(name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
