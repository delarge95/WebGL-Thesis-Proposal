using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Managers;
using WebGL.Core.Utils;

/// <summary>
/// Final import handoff for the optimized Blender runtime FBX.
/// Keeps the previous drone root as an inactive reference and prepares the new
/// model for the existing Unity setup pipeline.
/// </summary>
public static class ImportDroneModel
{
    private const string FinalFbxAssetPath = "Assets/Models/x500v2_runtime_low_final.fbx";
    private const string ExternalFinalFbxEnvVar = "TWINSIGHT_FINAL_FBX";
    private const string LocalFinalFbxFallbackPath = "blender_files/welded/x500v2_runtime_low_final.fbx";
    private const string ScenePath = "Assets/Scenes/MainScene_Final.unity";
    private const string RootName = "x500v2_Drone";
    private const string ReferenceRootName = "x500v2_ReferenceModels";
    private const string FastenerGroupId = "x500v2_fastener_group";
    private const string ReportPath = "Reports/final_runtime_import_report.md";
    private const string ImportBackupFolderName = "WebGL_tesis_import_backups";
    private const long MinimumExpectedFbxBytes = 1024 * 1024;
    private const float TargetRuntimeDominantSize = 10f;
    private const float RuntimeScaleTolerance = 0.08f;
    private const float MaxImporterScaleMultiplier = 1000f;
    private static readonly Quaternion RuntimeRootRotation = Quaternion.Euler(0f, 90f, 0f);

    [MenuItem("Tools/Import Final Runtime Drone Model")]
    public static void ImportFinalRuntimeModel()
    {
        ImportFinalRuntimeModel(showDialog: true);
    }

    public static void ImportFinalRuntimeModelBatch()
    {
        ImportFinalRuntimeModel(showDialog: false);
    }

    private static bool ImportFinalRuntimeModel(bool showDialog)
    {
        if (!EnsureFinalFbxAssetExists(showDialog, out string fbxSyncNote))
        {
            return false;
        }

        AssetDatabase.ImportAsset(FinalFbxAssetPath, ImportAssetOptions.ForceUpdate);
        ConfigureFinalModelImporter();

        GameObject fbxRoot = AssetDatabase.LoadAssetAtPath<GameObject>(FinalFbxAssetPath);
        ImportScaleReport scaleReport = CalibrateImporterScaleIfNeeded(fbxRoot);
        if (scaleReport.Reimported)
        {
            fbxRoot = AssetDatabase.LoadAssetAtPath<GameObject>(FinalFbxAssetPath);
        }

        if (!ValidateImportedFbxRoot(fbxRoot, showDialog, out int importedMeshCount))
        {
            return false;
        }

        if (showDialog && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return false;
        }

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        PreserveExistingDroneRoot();

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxRoot, scene);
        if (instance == null)
        {
            ReportError(showDialog, "No se pudo instanciar el FBX final en la escena.");
            return false;
        }

        instance.name = RootName;
        instance.transform.SetPositionAndRotation(Vector3.zero, RuntimeRootRotation);
        instance.transform.localScale = Vector3.one;
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        ImportNormalizationReport normalization = NormalizeRuntimeHierarchy(instance.transform);
        EnsureRuntimeBinder(instance);
        NormalizeSceneCameraTarget(instance.transform);
        bool setupOk = SetupImportedDroneThermalTest.PrepareImportedDroneHeadless();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        int sceneMeshCount = CountImportedMeshes(instance);
        bool hasSceneMeshes = sceneMeshCount > 0;
        if (!hasSceneMeshes)
        {
            ReportError(showDialog, "El FBX se instancio, pero la escena resultante no contiene MeshFilter/SkinnedMeshRenderer. Se aborta el setup para evitar un falso positivo.");
            UnityEngine.Object.DestroyImmediate(instance);
            return false;
        }

        WriteImportReport(instance.transform, normalization, setupOk, fbxSyncNote, importedMeshCount, sceneMeshCount, scaleReport);
        RuntimeDroneSceneAuditor.AuditResult audit = RuntimeDroneSceneAuditor.WriteAuditReport(instance.transform);

        string message =
            $"FBX final importado: {FinalFbxAssetPath}\n" +
            $"Root activo: {RootName}\n" +
            $"Modelo anterior: preservado como referencia inactiva\n" +
            $"Meshes importados: {sceneMeshCount}\n" +
            $"Escala dominante: {scaleReport.DominantSizeBefore.ToString("0.###", CultureInfo.InvariantCulture)}u -> {scaleReport.DominantSizeAfter.ToString("0.###", CultureInfo.InvariantCulture)}u\n" +
            $"Helices normalizadas: {normalization.PropellersRenamed}\n" +
            $"Helices proxy creadas: {normalization.PropellerProxiesCreated}\n" +
            $"Fasteners normalizados: {normalization.FastenersRenamed}\n" +
            $"Audit: {audit.Errors} errores, {audit.Warnings} warnings\n" +
            $"Setup thermal/runtime: {(setupOk ? "OK" : "revisar logs")}";

        Debug.Log($"[ImportDroneModel] {message}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Final Drone Import", message, "OK");
            Selection.activeGameObject = instance;
            SceneView.FrameLastActiveSceneView();
        }

        return setupOk;
    }

    private static void NormalizeSceneCameraTarget(Transform runtimeRoot)
    {
        if (runtimeRoot == null)
        {
            return;
        }

        OrbitCameraController[] controllers = UnityEngine.Object.FindObjectsByType<OrbitCameraController>(FindObjectsInactive.Include);
        foreach (OrbitCameraController controller in controllers)
        {
            if (controller == null)
            {
                continue;
            }

            SerializedObject serializedController = new SerializedObject(controller);
            SerializedProperty targetProperty = serializedController.FindProperty("target");
            if (targetProperty == null)
            {
                continue;
            }

            targetProperty.objectReferenceValue = runtimeRoot;
            serializedController.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(controller);
        }
    }

    private static bool EnsureFinalFbxAssetExists(bool showDialog, out string syncNote)
    {
        syncNote = string.Empty;
        string unityProjectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string targetPath = Path.GetFullPath(Path.Combine(unityProjectRoot, FinalFbxAssetPath));
        string externalFinalFbxPath = ResolveExternalFinalFbxPath(unityProjectRoot);
        string externalSourceLabel = string.IsNullOrWhiteSpace(externalFinalFbxPath)
            ? $"variable de entorno {ExternalFinalFbxEnvVar}"
            : externalFinalFbxPath;

        bool hasExternal = !string.IsNullOrWhiteSpace(externalFinalFbxPath) && File.Exists(externalFinalFbxPath);
        bool hasTarget = File.Exists(targetPath);

        if (hasExternal && !ValidateFbxFile(externalFinalFbxPath, out string externalReason))
        {
            ReportError(showDialog, $"El FBX fuente existe, pero no parece valido:\n{externalFinalFbxPath}\n{externalReason}");
            return false;
        }

        if (!hasTarget && !hasExternal)
        {
            ReportError(showDialog, $"No se encontro el FBX final en Assets ni en la fuente externa configurada ({externalSourceLabel}).");
            return false;
        }

        if (hasExternal)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? unityProjectRoot);
            if (!hasTarget)
            {
                File.Copy(externalFinalFbxPath, targetPath, overwrite: true);
                syncNote = "FBX copiado desde fuente externa porque no existia en Assets.";
            }
            else if (!FilesAreIdentical(externalFinalFbxPath, targetPath))
            {
                string backupFolder = Path.Combine(Path.GetTempPath(), ImportBackupFolderName);
                Directory.CreateDirectory(backupFolder);
                string backupName = $"x500v2_runtime_low_final_previous_{DateTime.Now:yyyyMMdd_HHmmss}.fbx";
                string backupPath = Path.Combine(backupFolder, backupName);
                File.Copy(targetPath, backupPath, overwrite: true);
                File.Copy(externalFinalFbxPath, targetPath, overwrite: true);
                syncNote = $"FBX de Assets reemplazado por fuente externa; copia anterior preservada en {backupPath}.";
            }
            else
            {
                syncNote = "FBX de Assets coincide con la fuente externa.";
            }
        }
        else
        {
            syncNote = "FBX fuente externa no disponible; se usa el archivo existente en Assets.";
        }

        if (!ValidateFbxFile(targetPath, out string targetReason))
        {
            ReportError(showDialog, $"El FBX en Assets no parece valido:\n{targetPath}\n{targetReason}");
            return false;
        }

        AssetDatabase.Refresh();
        return true;
    }

    private static string ResolveExternalFinalFbxPath(string unityProjectRoot)
    {
        string configuredPath = Environment.GetEnvironmentVariable(ExternalFinalFbxEnvVar);
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(configuredPath));
        }

        string repositoryRoot = Path.GetFullPath(Path.Combine(unityProjectRoot, "..", ".."));
        return Path.GetFullPath(Path.Combine(repositoryRoot, LocalFinalFbxFallbackPath));
    }

    private static void ConfigureFinalModelImporter()
    {
        ModelImporter importer = AssetImporter.GetAtPath(FinalFbxAssetPath) as ModelImporter;
        if (importer == null)
        {
            return;
        }

        importer.importAnimation = false;
        importer.importCameras = false;
        importer.importLights = false;
        importer.importBlendShapes = false;
        importer.importVisibility = true;
        importer.preserveHierarchy = true;
        importer.sortHierarchyByName = false;
        importer.useFileUnits = true;
        importer.bakeAxisConversion = false;
        importer.weldVertices = true;
        importer.isReadable = true;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.SaveAndReimport();
    }

    private static ImportScaleReport CalibrateImporterScaleIfNeeded(GameObject importedRoot)
    {
        ImportScaleReport report = new ImportScaleReport();
        if (importedRoot == null || !TryMeasurePrefabBounds(importedRoot, out Bounds bounds))
        {
            report.Note = "No se pudieron medir bounds del FBX para calibrar escala.";
            return report;
        }

        report.DominantSizeBefore = GetDominantSize(bounds);
        if (report.DominantSizeBefore <= 0.00001f)
        {
            report.Note = "Bounds del FBX sin tamano util.";
            return report;
        }

        float requiredMultiplier = TargetRuntimeDominantSize / report.DominantSizeBefore;
        report.RequiredMultiplier = requiredMultiplier;
        if (Mathf.Abs(1f - requiredMultiplier) <= RuntimeScaleTolerance)
        {
            report.DominantSizeAfter = report.DominantSizeBefore;
            report.Note = "Escala FBX dentro de tolerancia runtime.";
            return report;
        }

        ModelImporter importer = AssetImporter.GetAtPath(FinalFbxAssetPath) as ModelImporter;
        if (importer == null)
        {
            report.Note = "No se pudo acceder a ModelImporter para calibrar escala.";
            return report;
        }

        float currentGlobalScale = Mathf.Max(importer.globalScale, 0.000001f);
        float newGlobalScale = Mathf.Clamp(currentGlobalScale * requiredMultiplier, 0.000001f, MaxImporterScaleMultiplier);
        report.GlobalScaleBefore = currentGlobalScale;
        report.GlobalScaleAfter = newGlobalScale;

        if (Mathf.Abs(newGlobalScale - currentGlobalScale) <= 0.000001f)
        {
            report.DominantSizeAfter = report.DominantSizeBefore;
            report.Note = "La escala requerida fue clamped y no produjo cambio efectivo.";
            return report;
        }

        importer.globalScale = newGlobalScale;
        importer.SaveAndReimport();
        AssetDatabase.Refresh();

        GameObject reimportedRoot = AssetDatabase.LoadAssetAtPath<GameObject>(FinalFbxAssetPath);
        if (reimportedRoot != null && TryMeasurePrefabBounds(reimportedRoot, out Bounds calibratedBounds))
        {
            report.DominantSizeAfter = GetDominantSize(calibratedBounds);
        }

        report.Reimported = true;
        report.Note = $"Escala calibrada a tamano dominante objetivo {TargetRuntimeDominantSize:0.###}u.";
        return report;
    }

    private static void PreserveExistingDroneRoot()
    {
        GameObject existing = GameObject.Find(RootName);
        if (existing == null)
        {
            return;
        }

        GameObject referenceRoot = GameObject.Find(ReferenceRootName);
        if (referenceRoot == null)
        {
            referenceRoot = new GameObject(ReferenceRootName);
        }

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        existing.name = $"x500v2_Drone_REFERENCE_{timestamp}";
        existing.transform.SetParent(referenceRoot.transform, true);
        existing.SetActive(false);
        referenceRoot.SetActive(false);
    }

    private static ImportNormalizationReport NormalizeRuntimeHierarchy(Transform root)
    {
        ImportNormalizationReport report = new ImportNormalizationReport();
        if (root == null)
        {
            return report;
        }

        report.PropellersRenamed = NormalizePropellers(root, out report.PropellerProxiesCreated);
        report.FastenersRenamed = NormalizeFasteners(root);
        return report;
    }

    private static int NormalizePropellers(Transform root, out int proxiesCreated)
    {
        proxiesCreated = 0;
        List<Transform> candidates = CollectRootMostCandidates(root, IsPropellerName);
        if (candidates.Count == 0)
        {
            proxiesCreated = CreatePropellerProxiesFromMotors(root);
            candidates = CollectRootMostCandidates(root, IsPropellerName);
            if (candidates.Count == 0)
            {
                return 0;
            }
        }

        Bounds droneBounds = BuildRendererBounds(root);
        Vector3 center = droneBounds.center;
        Dictionary<string, int> suffixCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int renamed = 0;

        foreach (Transform candidate in candidates.OrderByDescending(t => GetWorldCenter(t).z).ThenBy(t => GetWorldCenter(t).x))
        {
            Vector3 position = GetWorldCenter(candidate);
            string suffix = ResolvePropellerImportSuffix(ResolveQuadrantSuffix(position, center));
            suffixCounts.TryGetValue(suffix, out int count);
            suffixCounts[suffix] = count + 1;

            candidate.name = count == 0
                ? $"x500v2_prop_{suffix}"
                : $"x500v2_prop_{suffix}_{count + 1:00}";
            renamed++;
        }

        return renamed;
    }

    private static int CreatePropellerProxiesFromMotors(Transform root)
    {
        if (root == null)
        {
            return 0;
        }

        List<Transform> motors = CollectRootMostCandidates(root, IsMotorName);
        if (motors.Count == 0)
        {
            return 0;
        }

        Bounds droneBounds = BuildRendererBounds(root);
        Vector3 center = droneBounds.center;
        float footprint = Mathf.Max(droneBounds.size.x, droneBounds.size.z);
        float radius = Mathf.Clamp(footprint * 0.115f, 0.035f, 0.18f);
        float bladeWidth = Mathf.Max(radius * 0.14f, 0.004f);
        float bladeThickness = Mathf.Max(radius * 0.018f, 0.0015f);
        float verticalOffset = Mathf.Max(radius * 0.09f, droneBounds.size.y * 0.015f);

        HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int created = 0;
        foreach (Transform motor in motors
                     .OrderByDescending(t => GetWorldCenter(t).z)
                     .ThenBy(t => GetWorldCenter(t).x)
                     .Take(4))
        {
            Vector3 position = GetWorldCenter(motor);
            string suffix = ResolvePropellerImportSuffix(ResolveQuadrantSuffix(position, center));
            string propellerName = $"x500v2_prop_{suffix}";
            if (!usedNames.Add(propellerName) || root.Find(propellerName) != null)
            {
                continue;
            }

            GameObject propeller = new GameObject(propellerName);
            propeller.transform.SetParent(root, worldPositionStays: true);
            propeller.transform.position = position + Vector3.up * verticalOffset;
            propeller.transform.rotation = Quaternion.identity;
            propeller.transform.localScale = Vector3.one;

            CreatePropellerBlade(propeller.transform, "blade_A", radius, bladeWidth, bladeThickness, 0f);
            CreatePropellerBlade(propeller.transform, "blade_B", radius, bladeWidth, bladeThickness, 90f);
            created++;
        }

        return created;
    }

    private static void CreatePropellerBlade(Transform parent, string name, float radius, float width, float thickness, float yawDegrees)
    {
        GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blade.name = name;
        blade.transform.SetParent(parent, worldPositionStays: false);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localRotation = Quaternion.Euler(0f, yawDegrees, 0f);
        blade.transform.localScale = new Vector3(radius * 2f, thickness, width);

        Collider collider = blade.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }
    }

    private static int NormalizeFasteners(Transform root)
    {
        List<Transform> candidates = CollectRootMostCandidates(root, SelectionHierarchy.IsPrimitiveFastenerSource);
        if (candidates.Count == 0)
        {
            return 0;
        }

        Transform group = EnsureChildGroup(root, FastenerGroupId);
        Dictionary<string, int> familyCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int renamed = 0;

        foreach (Transform candidate in candidates.OrderBy(t => t.name, StringComparer.OrdinalIgnoreCase))
        {
            if (candidate == group || candidate.IsChildOf(group))
            {
                continue;
            }

            string typeKey = FastenerNamingUtility.ExtractSceneTypeKey(candidate.name);
            if (string.IsNullOrWhiteSpace(typeKey))
            {
                typeKey = FastenerNamingUtility.SanitizeId(candidate.name);
            }

            familyCounts.TryGetValue(typeKey, out int count);
            count++;
            familyCounts[typeKey] = count;

            candidate.name = $"x500v2_fastener.{typeKey}_{count:000}";
            candidate.SetParent(group, worldPositionStays: true);
            renamed++;
        }

        return renamed;
    }

    private static List<Transform> CollectRootMostCandidates(Transform root, Func<string, bool> namePredicate)
    {
        return CollectRootMostCandidates(root, candidate => candidate != null && namePredicate(candidate.name));
    }

    private static List<Transform> CollectRootMostCandidates(Transform root, Func<Transform, bool> transformPredicate)
    {
        List<Transform> output = new List<Transform>();
        foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
        {
            if (candidate == null || candidate == root || !transformPredicate(candidate))
            {
                continue;
            }

            if (candidate.GetComponentInChildren<Renderer>(true) == null)
            {
                continue;
            }

            bool hasNamedAncestor = false;
            Transform ancestor = candidate.parent;
            while (ancestor != null && ancestor != root)
            {
                if (transformPredicate(ancestor) && ancestor.GetComponentInChildren<Renderer>(true) != null)
                {
                    hasNamedAncestor = true;
                    break;
                }

                ancestor = ancestor.parent;
            }

            if (!hasNamedAncestor)
            {
                output.Add(candidate);
            }
        }

        return output;
    }

    private static Transform EnsureChildGroup(Transform root, string groupName)
    {
        Transform existing = root.Find(groupName);
        if (existing != null)
        {
            return existing;
        }

        GameObject group = new GameObject(groupName);
        group.transform.SetParent(root, worldPositionStays: false);
        group.transform.localPosition = Vector3.zero;
        group.transform.localRotation = Quaternion.identity;
        group.transform.localScale = Vector3.one;
        return group.transform;
    }

    private static bool IsPropellerName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return false;
        }

        string name = rawName.ToLowerInvariant();
        return name.StartsWith("x500v2_prop_", StringComparison.Ordinal) ||
               name.Contains("propeller");
    }

    private static bool IsMotorName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return false;
        }

        string name = rawName.ToLowerInvariant().Replace('_', '-');
        return name.Contains("dj-2216-kv880") ||
               name.Contains("motor");
    }

    private static bool IsFastenerName(string rawName)
    {
        return SelectionHierarchy.IsPrimitiveFastenerName(rawName);
    }

    private static string ResolveQuadrantSuffix(Vector3 position, Vector3 center)
    {
        string side = position.x < center.x ? "L" : "R";
        string frontBack = position.z >= center.z ? "F" : "B";
        return frontBack + side;
    }

    private static string ResolvePropellerImportSuffix(string resolvedPhysicalSuffix)
    {
        return resolvedPhysicalSuffix switch
        {
            "FL" => "BL",
            "BL" => "FL",
            "FR" => "BR",
            "BR" => "FR",
            _ => resolvedPhysicalSuffix
        };
    }

    private static Bounds BuildRendererBounds(Transform root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        Bounds bounds = new Bounds(root.position, Vector3.one);
        bool hasBounds = false;
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

        return bounds;
    }

    private static bool TryMeasurePrefabBounds(GameObject root, out Bounds bounds)
    {
        bounds = default;
        if (root == null)
        {
            return false;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
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

        return hasBounds;
    }

    private static float GetDominantSize(Bounds bounds)
    {
        return Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
    }

    private static Vector3 GetWorldCenter(Transform target)
    {
        Renderer renderer = target != null ? target.GetComponentInChildren<Renderer>(true) : null;
        return renderer != null ? renderer.bounds.center : (target != null ? target.position : Vector3.zero);
    }

    private static void EnsureRuntimeBinder(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        ImportedDroneRuntimeBinder binder = root.GetComponent<ImportedDroneRuntimeBinder>();
        if (binder == null)
        {
            root.AddComponent<ImportedDroneRuntimeBinder>();
        }
    }

    private static bool ValidateFbxFile(string path, out string reason)
    {
        reason = string.Empty;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            reason = "Archivo no encontrado.";
            return false;
        }

        FileInfo file = new FileInfo(path);
        if (file.Length < MinimumExpectedFbxBytes)
        {
            reason = $"Tamano inesperadamente bajo ({file.Length} bytes).";
            return false;
        }

        byte[] header = new byte[Math.Min(64, (int)file.Length)];
        using (FileStream stream = File.OpenRead(path))
        {
            stream.Read(header, 0, header.Length);
        }

        string asciiHeader = Encoding.ASCII.GetString(header);
        if (asciiHeader.StartsWith("version https://git-lfs", StringComparison.OrdinalIgnoreCase))
        {
            reason = "El archivo es un puntero Git LFS, no un FBX real.";
            return false;
        }

        if (!asciiHeader.StartsWith("Kaydara FBX Binary", StringComparison.Ordinal))
        {
            reason = $"Cabecera FBX binaria no reconocida: {asciiHeader.Trim()}";
            return false;
        }

        return true;
    }

    private static bool FilesAreIdentical(string leftPath, string rightPath)
    {
        FileInfo left = new FileInfo(leftPath);
        FileInfo right = new FileInfo(rightPath);
        if (left.Length != right.Length)
        {
            return false;
        }

        byte[] leftHash = ComputeSha256(leftPath);
        byte[] rightHash = ComputeSha256(rightPath);
        return leftHash.SequenceEqual(rightHash);
    }

    private static byte[] ComputeSha256(string path)
    {
        using (SHA256 sha = SHA256.Create())
        using (FileStream stream = File.OpenRead(path))
        {
            return sha.ComputeHash(stream);
        }
    }

    private static string ComputeSha256String(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return string.Empty;
        }

        return BitConverter.ToString(ComputeSha256(path)).Replace("-", string.Empty);
    }

    private static bool ValidateImportedFbxRoot(GameObject fbxRoot, bool showDialog, out int importedMeshCount)
    {
        importedMeshCount = CountImportedMeshes(fbxRoot);
        if (fbxRoot == null)
        {
            ReportError(showDialog, $"Unity no pudo cargar el FBX final en {FinalFbxAssetPath}.");
            return false;
        }

        if (importedMeshCount <= 0)
        {
            ReportError(showDialog, $"Unity cargo el asset {FinalFbxAssetPath}, pero no encontro mallas importadas. Reexportar FBX o revisar corrupcion/cache antes de continuar.");
            return false;
        }

        return true;
    }

    private static int CountImportedMeshes(GameObject root)
    {
        if (root == null)
        {
            return 0;
        }

        int meshFilters = root.GetComponentsInChildren<MeshFilter>(true)
            .Count(meshFilter => meshFilter != null && meshFilter.sharedMesh != null);
        int skinned = root.GetComponentsInChildren<SkinnedMeshRenderer>(true)
            .Count(renderer => renderer != null && renderer.sharedMesh != null);
        return meshFilters + skinned;
    }

    private static void WriteImportReport(
        Transform root,
        ImportNormalizationReport normalization,
        bool setupOk,
        string fbxSyncNote,
        int importedMeshCount,
        int sceneMeshCount,
        ImportScaleReport scaleReport)
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string reportPath = Path.Combine(projectRoot, ReportPath);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath) ?? projectRoot);

        MeshFilter[] meshFilters = root != null ? root.GetComponentsInChildren<MeshFilter>(true) : Array.Empty<MeshFilter>();
        Dictionary<Mesh, int> meshUseCounts = new Dictionary<Mesh, int>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                continue;
            }

            meshUseCounts.TryGetValue(meshFilter.sharedMesh, out int count);
            meshUseCounts[meshFilter.sharedMesh] = count + 1;
        }

        int sharedMeshGroups = meshUseCounts.Count(kvp => kvp.Value > 1);
        int sharedMeshUsers = meshUseCounts.Where(kvp => kvp.Value > 1).Sum(kvp => kvp.Value);
        ExplodablePart[] explodableParts = root != null ? root.GetComponentsInChildren<ExplodablePart>(true) : Array.Empty<ExplodablePart>();
        int blendSelectableAnchors = explodableParts.Count(part =>
            part != null &&
            part.Data != null &&
            !string.IsNullOrWhiteSpace(part.Data.id) &&
            part.Data.id.StartsWith("x500v2_blend_", StringComparison.OrdinalIgnoreCase));
        int runtimeProxyVisuals = root != null
            ? root.GetComponentsInChildren<Transform>(true).Count(t => t != null && t.name.EndsWith("_runtime_proxy", StringComparison.OrdinalIgnoreCase))
            : 0;

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Final Runtime Import Report");
        builder.AppendLine();
        builder.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"FBX: {FinalFbxAssetPath}");
        builder.AppendLine($"FBX sync: {fbxSyncNote}");
        builder.AppendLine($"FBX bytes: {new FileInfo(Path.Combine(projectRoot, FinalFbxAssetPath)).Length}");
        builder.AppendLine($"FBX sha256: {ComputeSha256String(Path.Combine(projectRoot, FinalFbxAssetPath))}");
        builder.AppendLine($"Root activo: {RootName}");
        builder.AppendLine($"Setup runtime: {(setupOk ? "OK" : "REVISAR")}");
        builder.AppendLine();
        builder.AppendLine("## Escala");
        builder.AppendLine();
        builder.AppendLine($"- Tamano dominante objetivo: {TargetRuntimeDominantSize.ToString("0.###", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Tamano dominante antes: {scaleReport.DominantSizeBefore.ToString("0.######", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Tamano dominante despues: {scaleReport.DominantSizeAfter.ToString("0.######", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Global scale antes: {scaleReport.GlobalScaleBefore.ToString("0.######", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Global scale despues: {scaleReport.GlobalScaleAfter.ToString("0.######", CultureInfo.InvariantCulture)}");
        builder.AppendLine($"- Reimportado por escala: {(scaleReport.Reimported ? "si" : "no")}");
        builder.AppendLine($"- Nota escala: {scaleReport.Note}");
        builder.AppendLine();
        builder.AppendLine("## Normalizacion");
        builder.AppendLine();
        builder.AppendLine($"- Meshes importados en asset: {importedMeshCount}");
        builder.AppendLine($"- Meshes instanciados en escena: {sceneMeshCount}");
        builder.AppendLine($"- Helices renombradas: {normalization.PropellersRenamed}");
        builder.AppendLine($"- Helices proxy creadas: {normalization.PropellerProxiesCreated}");
        builder.AppendLine($"- Fasteners renombrados/agrupados: {normalization.FastenersRenamed}");
        builder.AppendLine($"- ExplodablePart en root activo: {explodableParts.Length}");
        builder.AppendLine($"- Anchors granulares x500v2_blend_* seleccionables: {blendSelectableAnchors}");
        builder.AppendLine($"- Proxies canonicos temporales: {runtimeProxyVisuals}");
        builder.AppendLine();
        builder.AppendLine("## Instancing Unity");
        builder.AppendLine();
        builder.AppendLine($"- MeshFilters totales: {meshFilters.Length}");
        builder.AppendLine($"- Mesh assets unicos: {meshUseCounts.Count}");
        builder.AppendLine($"- Grupos de sharedMesh con mas de un usuario: {sharedMeshGroups}");
        builder.AppendLine($"- MeshFilters usando sharedMesh repetido: {sharedMeshUsers}");
        builder.AppendLine();
        builder.AppendLine("Nota: si `sharedMesh` se repite, Unity preservo instanciacion logica de malla. Si no se repite, el FBX entro como meshes duplicados y debe revisarse el export desde Blender.");
        File.WriteAllText(reportPath, builder.ToString(), new UTF8Encoding(false));
    }

    private static void ReportError(bool showDialog, string message)
    {
        Debug.LogError($"[ImportDroneModel] {message}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Final Drone Import", message, "OK");
        }
    }

    private sealed class ImportNormalizationReport
    {
        public int PropellersRenamed;
        public int PropellerProxiesCreated;
        public int FastenersRenamed;
    }

    private sealed class ImportScaleReport
    {
        public float DominantSizeBefore;
        public float DominantSizeAfter;
        public float RequiredMultiplier = 1f;
        public float GlobalScaleBefore = 1f;
        public float GlobalScaleAfter = 1f;
        public bool Reimported;
        public string Note = string.Empty;
    }
}
