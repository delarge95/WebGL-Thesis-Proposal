using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Thermal;

namespace WebGL.Editor.Thermal
{
    internal enum ContactAxis
    {
        X,
        Y,
        Z
    }

    internal sealed class ThermalPartBoundsRecord
    {
        public ExplodablePart Part;
        public DronePartData Data;
        public string PartId;
        public Bounds Bounds;
    }

    internal sealed class ThermalContactCandidate
    {
        public ThermalPartBoundsRecord A;
        public ThermalPartBoundsRecord B;
        public ContactAxis Axis;
        public float GapMm;
        public float ContactAreaCm2;
        public float PathLengthMm;
        public float ConductionScale;
        public float Score;
    }

    public class ThermalContactGraphBuilderWindow : EditorWindow
    {
        [SerializeField] private Transform root;
        [SerializeField] private ThermalContactGraphAsset targetAsset;
        [SerializeField] private string assetPath = "Assets/Resources/ThermalCanonicalContactGraph.asset";
        [SerializeField, Min(0.1f)] private float maxGapMm = 12f;
        [SerializeField, Min(0f)] private float minContactAreaCm2 = 0.5f;
        [SerializeField, Min(0.1f)] private float minPathLengthMm = 1f;
        [SerializeField, Range(1, 16)] private int maxLinksPerPart = 6;
        [SerializeField] private bool overwriteExistingLinks = true;
        [SerializeField] private bool logSummary = true;

        private Vector2 scroll;

        [MenuItem("Tools/Thermal/Contact Graph Builder")]
        public static void OpenWindow()
        {
            GetWindow<ThermalContactGraphBuilderWindow>("Thermal Graph Builder");
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.LabelField("Thermal Contact Graph Builder", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Genera un ThermalContactGraphAsset a partir de bounds de las piezas del dron. "
                + "Es un preprocesado offline para reducir trabajo del runtime en WebGL.",
                MessageType.Info);

            root = (Transform)EditorGUILayout.ObjectField("Drone Root", root, typeof(Transform), true);
            targetAsset = (ThermalContactGraphAsset)EditorGUILayout.ObjectField("Target Asset", targetAsset, typeof(ThermalContactGraphAsset), false);
            assetPath = EditorGUILayout.TextField("New Asset Path", assetPath);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Detection", EditorStyles.boldLabel);
            maxGapMm = EditorGUILayout.FloatField("Max Gap (mm)", maxGapMm);
            minContactAreaCm2 = EditorGUILayout.FloatField("Min Contact Area (cm2)", minContactAreaCm2);
            minPathLengthMm = EditorGUILayout.FloatField("Min Path Length (mm)", minPathLengthMm);
            maxLinksPerPart = EditorGUILayout.IntSlider("Max Links Per Part", maxLinksPerPart, 1, 16);
            overwriteExistingLinks = EditorGUILayout.Toggle("Overwrite Existing Links", overwriteExistingLinks);
            logSummary = EditorGUILayout.Toggle("Log Summary", logSummary);

            EditorGUILayout.Space(8f);
            if (GUILayout.Button("Use Selected Transform") && Selection.activeTransform != null)
            {
                root = Selection.activeTransform;
            }

            if (GUILayout.Button("Build Thermal Contact Graph"))
            {
                BuildGraph();
            }

            EditorGUILayout.EndScrollView();
        }

        private void BuildGraph()
        {
            List<ThermalPartBoundsRecord> records = CollectRecords();
            if (records.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Thermal Graph Builder",
                    "No se encontraron ExplodablePart validos en la raiz indicada o en la escena actual.",
                    "OK");
                return;
            }

            List<ThermalContactCandidate> candidates = BuildCandidates(records);
            ThermalContactGraphAsset asset = ResolveTargetAsset();
            if (asset == null)
            {
                return;
            }

            if (overwriteExistingLinks)
            {
                asset.links.Clear();
            }

            asset.nodeIds.Clear();
            foreach (ThermalPartBoundsRecord record in records)
            {
                asset.nodeIds.Add(record.PartId);
            }

            foreach (ThermalContactCandidate candidate in candidates)
            {
                asset.links.Add(new ThermalContactLinkData
                {
                    fromPartId = candidate.A.PartId,
                    toPartId = candidate.B.PartId,
                    contactAreaCm2 = candidate.ContactAreaCm2,
                    pathLengthMm = candidate.PathLengthMm,
                    conductionScale = candidate.ConductionScale,
                    notes = $"axis={candidate.Axis}; gap_mm={candidate.GapMm:0.##}; score={candidate.Score:0.###}",
                });
            }

            asset.buildInfo.sourceSceneName = SceneManager.GetActiveScene().name;
            asset.buildInfo.sourceRootName = root != null ? root.name : "SceneSearch";
            asset.buildInfo.generatedUtc = DateTime.UtcNow.ToString("o");
            asset.buildInfo.maxGapMm = maxGapMm;
            asset.buildInfo.minContactAreaCm2 = minContactAreaCm2;
            asset.buildInfo.minPathLengthMm = minPathLengthMm;
            asset.buildInfo.generatedFromBounds = true;

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (logSummary)
            {
                Debug.Log($"[ThermalContactGraphBuilder] Built graph with {asset.nodeIds.Count} nodes and {asset.links.Count} links.");
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private List<ThermalPartBoundsRecord> CollectRecords()
        {
            var records = new List<ThermalPartBoundsRecord>();
            ExplodablePart[] parts = root != null
                ? root.GetComponentsInChildren<ExplodablePart>(true)
                : FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);

            foreach (ExplodablePart part in parts)
            {
                if (part == null)
                {
                    continue;
                }

                if (!TryComputeBounds(part, out Bounds bounds))
                {
                    continue;
                }

                DronePartData data = part.Data;
                string partId = ResolvePartId(part, data);
                if (string.IsNullOrWhiteSpace(partId))
                {
                    continue;
                }

                records.Add(new ThermalPartBoundsRecord
                {
                    Part = part,
                    Data = data,
                    PartId = partId,
                    Bounds = bounds,
                });
            }

            records.Sort((left, right) => string.Compare(left.PartId, right.PartId, StringComparison.Ordinal));
            return records;
        }

        private List<ThermalContactCandidate> BuildCandidates(List<ThermalPartBoundsRecord> records)
        {
            var candidates = new List<ThermalContactCandidate>();
            var acceptedCounts = new Dictionary<string, int>();

            for (int i = 0; i < records.Count; i++)
            {
                for (int j = i + 1; j < records.Count; j++)
                {
                    ThermalContactCandidate candidate = TryBuildCandidate(records[i], records[j]);
                    if (candidate != null)
                    {
                        candidates.Add(candidate);
                    }
                }
            }

            candidates.Sort((left, right) => right.Score.CompareTo(left.Score));

            var accepted = new List<ThermalContactCandidate>();
            foreach (ThermalContactCandidate candidate in candidates)
            {
                int countA = acceptedCounts.TryGetValue(candidate.A.PartId, out int cachedA) ? cachedA : 0;
                int countB = acceptedCounts.TryGetValue(candidate.B.PartId, out int cachedB) ? cachedB : 0;
                if (countA >= maxLinksPerPart || countB >= maxLinksPerPart)
                {
                    continue;
                }

                accepted.Add(candidate);
                acceptedCounts[candidate.A.PartId] = countA + 1;
                acceptedCounts[candidate.B.PartId] = countB + 1;
            }

            accepted.Sort((left, right) => string.Compare(left.A.PartId + left.B.PartId, right.A.PartId + right.B.PartId, StringComparison.Ordinal));
            return accepted;
        }

        private ThermalContactCandidate TryBuildCandidate(ThermalPartBoundsRecord a, ThermalPartBoundsRecord b)
        {
            Vector3 separation = ComputeSeparation(a.Bounds, b.Bounds);
            float gapMm = separation.magnitude * 1000f;
            if (gapMm > maxGapMm)
            {
                return null;
            }

            Vector3 overlap = ComputeOverlap(a.Bounds, b.Bounds);
            ContactAxis axis = ResolveContactAxis(separation, overlap);
            float contactAreaCm2 = ComputeContactAreaCm2(overlap, axis);
            if (contactAreaCm2 < minContactAreaCm2)
            {
                return null;
            }

            float pathLength = Mathf.Max(minPathLengthMm, gapMm <= 0.01f ? minPathLengthMm : gapMm);
            float conductionScale = ResolveConductionScale(a.Data, b.Data);
            float score = (contactAreaCm2 * conductionScale) / Mathf.Max(pathLength, 0.1f);

            return new ThermalContactCandidate
            {
                A = a,
                B = b,
                Axis = axis,
                GapMm = gapMm,
                ContactAreaCm2 = contactAreaCm2,
                PathLengthMm = pathLength,
                ConductionScale = conductionScale,
                Score = score,
            };
        }

        private ThermalContactGraphAsset ResolveTargetAsset()
        {
            if (targetAsset != null)
            {
                return targetAsset;
            }

            string sanitizedPath = string.IsNullOrWhiteSpace(assetPath)
                ? "Assets/Data/Thermal/ThermalContactGraph.asset"
                : assetPath.Trim();

            if (!sanitizedPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                EditorUtility.DisplayDialog(
                    "Thermal Graph Builder",
                    "La ruta del asset debe comenzar con 'Assets/'.",
                    "OK");
                return null;
            }

            string folder = Path.GetDirectoryName(sanitizedPath);
            if (!string.IsNullOrWhiteSpace(folder) && !AssetDatabase.IsValidFolder(folder))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folder));
                AssetDatabase.Refresh();
            }

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(sanitizedPath);
            var asset = CreateInstance<ThermalContactGraphAsset>();
            AssetDatabase.CreateAsset(asset, uniquePath);
            targetAsset = asset;
            return asset;
        }

        private static bool TryComputeBounds(ExplodablePart part, out Bounds bounds)
        {
            Renderer[] renderers = part.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bool hasBounds = false;
            bounds = default;
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

        private static string ResolvePartId(ExplodablePart part, DronePartData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.id))
            {
                return data.id;
            }

            return part != null ? part.gameObject.name : string.Empty;
        }

        private static Vector3 ComputeSeparation(Bounds a, Bounds b)
        {
            return new Vector3(
                ComputeAxisGap(a.min.x, a.max.x, b.min.x, b.max.x),
                ComputeAxisGap(a.min.y, a.max.y, b.min.y, b.max.y),
                ComputeAxisGap(a.min.z, a.max.z, b.min.z, b.max.z));
        }

        private static Vector3 ComputeOverlap(Bounds a, Bounds b)
        {
            return new Vector3(
                ComputeAxisOverlap(a.min.x, a.max.x, b.min.x, b.max.x),
                ComputeAxisOverlap(a.min.y, a.max.y, b.min.y, b.max.y),
                ComputeAxisOverlap(a.min.z, a.max.z, b.min.z, b.max.z));
        }

        private static float ComputeAxisGap(float aMin, float aMax, float bMin, float bMax)
        {
            if (aMax < bMin)
            {
                return bMin - aMax;
            }

            if (bMax < aMin)
            {
                return aMin - bMax;
            }

            return 0f;
        }

        private static float ComputeAxisOverlap(float aMin, float aMax, float bMin, float bMax)
        {
            return Mathf.Max(0f, Mathf.Min(aMax, bMax) - Mathf.Max(aMin, bMin));
        }

        private static ContactAxis ResolveContactAxis(Vector3 separation, Vector3 overlap)
        {
            if (separation.sqrMagnitude > 0.0000001f)
            {
                if (separation.x >= separation.y && separation.x >= separation.z)
                {
                    return ContactAxis.X;
                }

                if (separation.y >= separation.z)
                {
                    return ContactAxis.Y;
                }

                return ContactAxis.Z;
            }

            if (overlap.x <= overlap.y && overlap.x <= overlap.z)
            {
                return ContactAxis.X;
            }

            if (overlap.y <= overlap.z)
            {
                return ContactAxis.Y;
            }

            return ContactAxis.Z;
        }

        private static float ComputeContactAreaCm2(Vector3 overlap, ContactAxis axis)
        {
            float areaM2 = axis switch
            {
                ContactAxis.X => overlap.y * overlap.z,
                ContactAxis.Y => overlap.x * overlap.z,
                _ => overlap.x * overlap.y,
            };

            return Mathf.Max(0f, areaM2 * 10000f);
        }

        private static float ResolveConductionScale(DronePartData a, DronePartData b)
        {
            float scaleA = a != null && a.thermalConductionScale > 0f ? a.thermalConductionScale : 1f;
            float scaleB = b != null && b.thermalConductionScale > 0f ? b.thermalConductionScale : 1f;
            return 0.5f * (scaleA + scaleB);
        }
    }
}
