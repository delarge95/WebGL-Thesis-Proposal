using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Events;
using WebGL.Core.Managers;
using WebGL.Core.Thermal;

namespace WebGL.Core.Utils
{
    [DisallowMultipleComponent]
    public class ImportedDroneRuntimeBinder : MonoBehaviour
    {
        [Header("Runtime Binding")]
        [SerializeField] private bool bindOnStart = true;
        [SerializeField] private bool rebuildHotspotsWhenReady = true;
        [SerializeField] private bool logBindingSummary;

        private Coroutine bindRoutine;

        private void Start()
        {
            if (!bindOnStart)
            {
                return;
            }

            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
            }

            bindRoutine = StartCoroutine(BindNextFrame());
        }

        [ContextMenu("Bind Imported Drone Runtime")]
        public void BindRuntimeNow()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
            }

            bindRoutine = StartCoroutine(BindNextFrame());
        }

        private IEnumerator BindNextFrame()
        {
            yield return null;
            BindRuntime();
            bindRoutine = null;
        }

        private void BindRuntime()
        {
            Transform droneRoot = ResolveDroneRoot();
            if (droneRoot == null)
            {
                Debug.LogWarning("[ImportedDroneRuntimeBinder] No se encontro el root del dron importado.");
                return;
            }

            RepairImportedDrone(droneRoot);
            Transform[] propellers = FindPropellers(droneRoot);

            DroneStateController.Instance?.ConfigureRuntimeBindings(droneRoot, propellers);
            ExplodedViewManager.Instance?.RebuildCache();
            PartVisibilityManager.Instance?.RebuildCache();
            PartCatalogManager.Instance?.RefreshPartsList();
            CrossSectionManager.Instance?.RefreshTargetObject();

            ViewModeManager viewMode = ViewModeManager.Instance;
            if (viewMode != null)
            {
                viewMode.RebuildCache();
                viewMode.ReapplyCurrentMode(true);
            }

            ThermalSimulationManager.Instance?.RebuildRuntime();
            ThermalViewController.Instance?.RebuildBindings();
            int partCount = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None).Length;

            if (rebuildHotspotsWhenReady)
            {
                EventBus.Publish(new ImportedDroneRuntimeBoundEvent(droneRoot.name, propellers.Length, partCount));
            }

            if (logBindingSummary)
            {
                Debug.Log($"[ImportedDroneRuntimeBinder] Bound root={droneRoot.name} propellers={propellers.Length} parts={partCount}");
            }
        }

        private void RepairImportedDrone(Transform droneRoot)
        {
            Dictionary<string, ExplodablePart> anchorsById = BuildAnchorMap(droneRoot);
            ReparentTopLevelOrphans(droneRoot, anchorsById);
            anchorsById = BuildAnchorMap(droneRoot);

            foreach (KeyValuePair<string, ExplodablePart> kvp in anchorsById)
            {
                ConfigureAnchor(kvp.Key, kvp.Value);
            }
        }

        private static Dictionary<string, ExplodablePart> BuildAnchorMap(Transform droneRoot)
        {
            Dictionary<string, ExplodablePart> anchors = new Dictionary<string, ExplodablePart>(StringComparer.OrdinalIgnoreCase);
            if (droneRoot == null)
            {
                return anchors;
            }

            foreach (ExplodablePart part in droneRoot.GetComponentsInChildren<ExplodablePart>(true))
            {
                if (part == null)
                {
                    continue;
                }

                string partId = part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id)
                    ? part.Data.id
                    : part.name;

                if (!string.IsNullOrWhiteSpace(partId) && !anchors.ContainsKey(partId))
                {
                    anchors.Add(partId, part);
                }
            }

            return anchors;
        }

        private static void ReparentTopLevelOrphans(Transform droneRoot, IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (droneRoot == null || anchorsById == null || anchorsById.Count == 0)
            {
                return;
            }

            List<Transform> directChildren = new List<Transform>();
            foreach (Transform child in droneRoot)
            {
                directChildren.Add(child);
            }

            foreach (Transform child in directChildren)
            {
                if (child == null || child.GetComponent<ExplodablePart>() != null)
                {
                    continue;
                }

                Renderer renderer = child.GetComponentInChildren<Renderer>(true);
                if (renderer == null)
                {
                    continue;
                }

                ExplodablePart bestAnchor = ResolveBestAnchor(child, renderer, anchorsById);
                if (bestAnchor == null || child.IsChildOf(bestAnchor.transform))
                {
                    continue;
                }

                child.SetParent(bestAnchor.transform, true);
            }
        }

        private static ExplodablePart ResolveBestAnchor(Transform candidate, Renderer renderer, IReadOnlyDictionary<string, ExplodablePart> anchorsById)
        {
            if (candidate == null || anchorsById == null || anchorsById.Count == 0)
            {
                return null;
            }

            string lowerName = candidate.name.ToLowerInvariant();
            string suffix = ResolveQuadrantSuffix(lowerName);
            Vector3 candidateCenter = renderer != null ? renderer.bounds.center : candidate.position;

            float bestScore = float.MaxValue;
            ExplodablePart bestAnchor = null;

            foreach (KeyValuePair<string, ExplodablePart> kvp in anchorsById)
            {
                ExplodablePart anchor = kvp.Value;
                if (anchor == null)
                {
                    continue;
                }

                float score = Vector3.Distance(candidateCenter, anchor.transform.position);
                string anchorId = kvp.Key.ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(suffix) && anchorId.EndsWith("_" + suffix, StringComparison.Ordinal))
                {
                    score -= 2f;
                }

                if (lowerName.Contains("prop") && anchorId.Contains("prop")) score -= 2.0f;
                if (lowerName.Contains("motor") && anchorId.Contains("motor")) score -= 1.8f;
                if (lowerName.Contains("esc") && anchorId.Contains("esc")) score -= 1.7f;
                if (lowerName.Contains("arm") && anchorId.Contains("arm")) score -= 1.4f;
                if (lowerName.Contains("battery") && anchorId.Contains("battery")) score -= 1.4f;
                if (lowerName.Contains("plate") && anchorId.Contains("plate")) score -= 1.0f;
                if (lowerName.Contains("landing") && anchorId.Contains("landing")) score -= 1.0f;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestAnchor = anchor;
                }
            }

            return bestAnchor;
        }

        private static void ConfigureAnchor(string anchorId, ExplodablePart anchor)
        {
            if (anchor == null)
            {
                return;
            }

            CalibrateExplosionPreset(anchorId, anchor);
            EnsureSelectableLayer(anchor.transform);

            foreach (Renderer renderer in anchor.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                {
                    continue;
                }

                EnsureSelectionCollider(renderer);

                string auxiliaryCategory = InferAuxiliaryCategory(renderer.transform.name);
                string thermalSourceId = InferThermalSourcePartId(renderer.transform.name, anchorId);
                string primaryCategory = InferDisplayCategory(renderer.transform.name, anchor.Data != null ? anchor.Data.category.ToString() : string.Empty, thermalSourceId);

                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = renderer.gameObject.AddComponent<PartRenderCategory>();
                }
                category.Configure(anchorId, primaryCategory, auxiliaryCategory, thermalSourceId);

                ConfigureAuxiliaryExplode(renderer.transform, anchor, auxiliaryCategory);
            }

            anchor.Initialize();
        }

        private static void EnsureSelectableLayer(Transform root)
        {
            int selectableLayer = LayerMask.NameToLayer("SelectablePart");
            if (root == null || selectableLayer < 0)
            {
                return;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = selectableLayer;
            }
        }

        private static void EnsureSelectionCollider(Renderer renderer)
        {
            if (renderer == null || renderer.GetComponent<Collider>() != null)
            {
                return;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return;
            }

            BoxCollider collider = renderer.gameObject.AddComponent<BoxCollider>();
            collider.center = meshFilter.sharedMesh.bounds.center;
            collider.size = meshFilter.sharedMesh.bounds.size;
        }

        private static void ConfigureAuxiliaryExplode(Transform member, ExplodablePart anchor, string auxiliaryCategory)
        {
            if (member == null || anchor == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(auxiliaryCategory))
            {
                AuxiliaryExplodeOffset existing = member.GetComponent<AuxiliaryExplodeOffset>();
                if (existing != null)
                {
                    existing.Initialize();
                }
                return;
            }

            Vector3 localDirection = member.localPosition.sqrMagnitude > 0.0001f
                ? member.localPosition.normalized
                : ResolveExplosionDirection(anchor).normalized;

            float distance = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ? 0.06f : 0.03f;
            float lead = string.Equals(auxiliaryCategory, "Fasteners", StringComparison.OrdinalIgnoreCase) ? 1.35f : 1.12f;

            AuxiliaryExplodeOffset offset = member.GetComponent<AuxiliaryExplodeOffset>();
            if (offset == null)
            {
                offset = member.gameObject.AddComponent<AuxiliaryExplodeOffset>();
            }

            offset.Configure(localDirection, distance, lead);
        }

        private static void CalibrateExplosionPreset(string anchorId, ExplodablePart anchor)
        {
            if (anchor == null || anchor.Data == null)
            {
                return;
            }

            // Keep authored explosion presets from DronePartData and only repair invalid values.
            // This avoids forcing runtime axis changes that can disorient the exploded view.
            if (anchor.Data.explosionDistance <= 0.001f)
            {
                anchor.Data.explosionDistance = 0.2f;
            }

            if (anchor.Data.explosionDirection.sqrMagnitude <= 0.0001f)
            {
                anchor.Data.explosionDirection = Vector3.up;
            }
            else
            {
                anchor.Data.explosionDirection = anchor.Data.explosionDirection.normalized;
            }
        }

        private static Vector3 ResolveExplosionDirection(ExplodablePart anchor)
        {
            if (anchor != null && anchor.Data != null && anchor.Data.explosionDirection.sqrMagnitude > 0.0001f)
            {
                return anchor.Data.explosionDirection.normalized;
            }

            return Vector3.up;
        }

        private static Vector3 ResolveQuadrantDirection(string rawId, float y)
        {
            string suffix = ResolveQuadrantSuffix((rawId ?? string.Empty).ToLowerInvariant());
            return suffix switch
            {
                "fl" => new Vector3(-1f, y, 1f).normalized,
                "fr" => new Vector3(1f, y, 1f).normalized,
                "bl" => new Vector3(-1f, y, -1f).normalized,
                "br" => new Vector3(1f, y, -1f).normalized,
                _ => new Vector3(0f, y, 1f).normalized,
            };
        }

        private static string InferDisplayCategory(string rendererName, string fallbackCategory, string thermalSourceId)
        {
            string normalized = (thermalSourceId ?? rendererName ?? string.Empty).ToLowerInvariant();
            if (normalized.Contains("motor") || normalized.Contains("prop") || normalized.Contains("esc"))
            {
                return "PropulsionSystem";
            }

            if (normalized.Contains("battery") || normalized.Contains("pdb") || normalized.Contains("power_module"))
            {
                return "PowerDistribution";
            }

            if (normalized.Contains("gps") || normalized.Contains("receiver") || normalized.Contains("telemetry") || normalized.Contains("radio"))
            {
                return "SensorsComms";
            }

            if (normalized.Contains("pixhawk") || normalized.Contains("flight_controller") || normalized.Contains("fc"))
            {
                return "Avionics";
            }

            if (normalized.Contains("arm") || normalized.Contains("plate") || normalized.Contains("landing") ||
                normalized.Contains("platform") || normalized.Contains("rail"))
            {
                return "SkeletonAirframe";
            }

            return string.IsNullOrWhiteSpace(fallbackCategory) ? "Uncategorized" : fallbackCategory;
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

        private static string InferAuxiliaryCategory(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            string name = rawName.ToLowerInvariant();

            if (name.Contains("fastener") || name.Contains("screw") || name.Contains("cap_screw") ||
                name.Contains("bolt") || name.Contains("nut") || name.Contains("washer") ||
                name.Contains("standoff") || name.Contains("spacer"))
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

        private static string ResolveQuadrantSuffix(string lowerName)
        {
            if (lowerName.Contains("_fl")) return "fl";
            if (lowerName.Contains("_fr")) return "fr";
            if (lowerName.Contains("_bl")) return "bl";
            if (lowerName.Contains("_br")) return "br";
            return string.Empty;
        }

        private Transform ResolveDroneRoot()
        {
            if (name.StartsWith("x500v2_Drone"))
            {
                return transform;
            }

            GameObject namedRoot = GameObject.Find("x500v2_Drone");
            return namedRoot != null ? namedRoot.transform : transform;
        }

        private static Transform[] FindPropellers(Transform droneRoot)
        {
            List<Transform> propellers = new List<Transform>();
            foreach (Transform child in droneRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child == null)
                {
                    continue;
                }

                if (child.name.StartsWith("x500v2_prop_", System.StringComparison.OrdinalIgnoreCase))
                {
                    propellers.Add(child);
                }
            }

            return propellers.ToArray();
        }
    }
}
