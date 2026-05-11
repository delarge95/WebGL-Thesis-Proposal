using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public class PartRenderCategory : MonoBehaviour
    {
        [SerializeField] private string canonicalPartId;
        [SerializeField] private string subpieceId = string.Empty;
        [SerializeField] private string primaryCategory = "Uncategorized";
        [SerializeField] private string auxiliaryCategory = string.Empty;
        [SerializeField] private string thermalSourcePartId = string.Empty;

        private static readonly Dictionary<string, string> CategoryAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ALL", "ALL" },
            { "Structure", "SkeletonAirframe" },
            { "Propulsion", "PropulsionSystem" },
            { "Electronics", "Avionics" },
            { "Power", "PowerDistribution" },
            { "Misc", "Uncategorized" },
            { "Skeleton & Airframe", "SkeletonAirframe" },
            { "Sensors & Comms", "SensorsComms" },
            { "Power Distribution", "PowerDistribution" },
            { "Propulsion System", "PropulsionSystem" }
        };

        public string CanonicalPartId => canonicalPartId;
        public string SubpieceId => subpieceId;
        public string PrimaryCategory => primaryCategory;
        public string AuxiliaryCategory => auxiliaryCategory;
        public string ThermalSourcePartId => string.IsNullOrWhiteSpace(thermalSourcePartId) ? canonicalPartId : thermalSourcePartId;

        public bool MatchesAny(IReadOnlyList<string> categories)
        {
            if (categories == null || categories.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                if (MatchesCategory(categories[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MatchesCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || string.Equals(category, "ALL", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string normalizedFilter = NormalizeCategory(category);
            string normalizedPrimary = NormalizeCategory(primaryCategory);
            string normalizedAux = NormalizeCategory(auxiliaryCategory);

            if (string.Equals(normalizedPrimary, normalizedFilter, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(auxiliaryCategory)
                && string.Equals(normalizedAux, normalizedFilter, StringComparison.OrdinalIgnoreCase);
        }

        public void Configure(string canonicalId, string primary, string auxiliary, string thermalSourceId, string subpiece = "")
        {
            canonicalPartId = canonicalId ?? string.Empty;
            subpieceId = subpiece ?? string.Empty;
            primaryCategory = NormalizeCategory(string.IsNullOrWhiteSpace(primary) ? "Uncategorized" : primary);
            auxiliaryCategory = auxiliary ?? string.Empty;
            thermalSourcePartId = string.IsNullOrWhiteSpace(thermalSourceId) ? canonicalPartId : thermalSourceId;
        }

        private static string NormalizeCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return "Uncategorized";
            }

            string trimmed = category.Trim();
            return CategoryAliases.TryGetValue(trimmed, out string canonical)
                ? canonical
                : trimmed;
        }
    }

    [Serializable]
    public sealed class SelectionHierarchyCatalog
    {
        public string version;
        public HotspotGroupDefinition[] hotspotGroups;
        public CanonicalSelectionGroupDefinition[] canonicalGroups;
    }

    [Serializable]
    public sealed class HotspotGroupDefinition
    {
        public string groupId;
        public string label;
        public string summary;
        public bool includeAssociatedFasteners;
        public string[] canonicalPartIds;
    }

    [Serializable]
    public sealed class CanonicalSelectionGroupDefinition
    {
        public string canonicalPartId;
        public string displayName;
        public string[] subpieceNames;
    }

    public static class SelectionHierarchy
    {
        public const string FastenerGroupId = "x500v2_fastener_group";
        public const string MiscGroupId = "x500v2_misc_group";

        private const string ResourcePath = "holybro_selection_hierarchy";

        private static SelectionHierarchyCatalog cachedCatalog;
        private static Dictionary<string, CanonicalSelectionGroupDefinition> canonicalById;
        private static Dictionary<string, string> subpieceToCanonical;

        public static IReadOnlyList<HotspotGroupDefinition> HotspotGroups => GetCatalog().hotspotGroups;

        public static bool TryGetSubpieceParent(string rawSubpieceName, out string canonicalPartId)
        {
            EnsureLookups();
            canonicalPartId = string.Empty;

            string normalized = NormalizeSubpieceLookupToken(rawSubpieceName);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            if (subpieceToCanonical.TryGetValue(normalized, out canonicalPartId))
            {
                return true;
            }

            foreach (KeyValuePair<string, string> kvp in subpieceToCanonical)
            {
                if (normalized.StartsWith(kvp.Key + "-", StringComparison.OrdinalIgnoreCase))
                {
                    canonicalPartId = kvp.Value;
                    return true;
                }
            }

            return false;
        }

        public static bool IsCanonicalPartId(string id)
        {
            EnsureLookups();
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return canonicalById.ContainsKey(id) ||
                   id.StartsWith("x500v2_arm_", StringComparison.OrdinalIgnoreCase) ||
                   id.StartsWith("x500v2_motor_", StringComparison.OrdinalIgnoreCase) ||
                   id.StartsWith("x500v2_esc_", StringComparison.OrdinalIgnoreCase) ||
                   id.StartsWith("x500v2_prop_", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPrimitiveFastenerSource(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            if (IsKnownStructuralNonFastenerName(target.name))
            {
                return false;
            }

            if (IsPrimitiveFastenerName(target.name))
            {
                return true;
            }

            Transform current = target.parent;
            int depth = 0;
            while (current != null && depth < 16)
            {
                string normalized = NormalizeToken(current.name);
                if (string.Equals(current.name, FastenerGroupId, StringComparison.OrdinalIgnoreCase) ||
                    normalized.Contains("primitive-fastener") ||
                    normalized.Contains("fastener-primitive"))
                {
                    return true;
                }

                current = current.parent;
                depth++;
            }

            return false;
        }

        public static bool IsPrimitiveFastenerName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return false;
            }

            if (IsKnownStructuralNonFastenerName(rawName))
            {
                return false;
            }

            string name = NormalizeToken(rawName);
            return name.Contains("x500v2-fastener") ||
                   name.Contains("cap-screw") ||
                   name.Contains("socket-screw") ||
                   name.Contains("gb70") ||
                   name.Contains("m25-6-chen-liu") ||
                   name.Contains("m3-16-chen-liu") ||
                   name.Contains("m3-10-pan-ding") ||
                   name.Contains("m3-14-pan") ||
                   name.Contains("zslm-m25") ||
                   name.Contains("zslm-m3") ||
                   name.Contains("lm-m3") ||
                   name.Contains("gpsv5-zhijia-luomao") ||
                   name.Contains("luomao") ||
                   name.Contains("huan-guijiao") ||
                   name.Contains("nilongzhu-m25") ||
                   name.Contains("nilongzhu-m3");
        }

        public static bool IsKnownStructuralNonFastenerName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return false;
            }

            string normalized = NormalizeToken(rawName);
            return normalized.Contains("hmx5v-guan-dingwei");
        }

        public static string ResolveCanonicalPartId(Transform target, Transform root, string fallbackCanonicalId = "")
        {
            if (target == null)
            {
                return fallbackCanonicalId ?? string.Empty;
            }

            Renderer renderer = target.GetComponentInChildren<Renderer>(true);
            Vector3 position = renderer != null ? renderer.bounds.center : target.position;
            return ResolveCanonicalPartId(target.name, position, root, fallbackCanonicalId);
        }

        public static string ResolveCanonicalPartId(string rawName, Vector3 worldPosition, Transform root, string fallbackCanonicalId = "")
        {
            if (string.IsNullOrWhiteSpace(rawName) || IsPrimitiveFastenerName(rawName))
            {
                return string.Empty;
            }

            string lowered = rawName.ToLowerInvariant();
            if (lowered.StartsWith("x500v2_", StringComparison.Ordinal) &&
                !lowered.StartsWith("x500v2_blend_", StringComparison.Ordinal) &&
                !lowered.StartsWith("x500v2_fastener", StringComparison.Ordinal))
            {
                int dot = rawName.IndexOf('.');
                string directId = dot > 0 ? rawName.Substring(0, dot) : rawName;
                return IsCanonicalPartId(directId) ? directId : fallbackCanonicalId ?? string.Empty;
            }

            if (TryGetSubpieceParent(rawName, out string explicitParent))
            {
                if (RequiresQuadrant(explicitParent))
                {
                    string suffix = ResolveQuadrantSuffix(lowered);
                    if (string.IsNullOrWhiteSpace(suffix))
                    {
                        suffix = ResolveQuadrantSuffixFromWorld(worldPosition, root);
                    }

                    return string.IsNullOrWhiteSpace(suffix)
                        ? string.Empty
                        : explicitParent + "_" + suffix.ToUpperInvariant();
                }

                return explicitParent;
            }

            string token = NormalizeToken(rawName);
            string quadrant = ResolveQuadrantSuffix(lowered);
            if (string.IsNullOrWhiteSpace(quadrant))
            {
                quadrant = ResolveQuadrantSuffixFromWorld(worldPosition, root);
            }

            if (token.Contains("dj-2216") || token.Contains("motor"))
            {
                return !string.IsNullOrWhiteSpace(quadrant) ? "x500v2_motor_" + quadrant.ToUpperInvariant() : string.Empty;
            }

            if (token.Contains("propeller") || token.Contains("prop"))
            {
                return !string.IsNullOrWhiteSpace(quadrant) ? "x500v2_prop_" + quadrant.ToUpperInvariant() : string.Empty;
            }

            if (token.Contains("esc"))
            {
                return !string.IsNullOrWhiteSpace(quadrant) ? "x500v2_esc_" + quadrant.ToUpperInvariant() : string.Empty;
            }

            if (token.Contains("battery-mounting") || token.Contains("battery-pad") ||
                token.Contains("pylons-x500") || token.Contains("guan-cheng") ||
                token.Contains("payload-rail"))
            {
                return "x500v2_rails_battery";
            }

            if (token.Contains("battery")) return "x500v2_battery";
            if (token.Contains("pdb")) return "x500v2_pdb";
            if (token.Contains("pm06") || token.Contains("xt60") || token.Contains("power-module")) return "x500v2_power_module";
            if (token.Contains("pixhawk") || token.Contains("imu-pixhawk") || token.Contains("bm06b")) return "x500v2_pixhawk6c";
            if (token.Contains("gps") || token.Contains("gpsv5")) return "x500v2_gps_m10";
            if (token.Contains("receiver")) return "x500v2_rc_receiver";
            if (token.Contains("telemetry") || token.Contains("radio")) return "x500v2_telemetry_radio";

            return fallbackCanonicalId ?? string.Empty;
        }

        public static string ResolveSubpieceId(Transform target, string canonicalPartId)
        {
            if (target == null || string.IsNullOrWhiteSpace(canonicalPartId) || IsPrimitiveFastenerName(target.name))
            {
                return string.Empty;
            }

            if (!TryGetSubpieceParent(target.name, out string parentId))
            {
                return string.Empty;
            }

            bool directParent = string.Equals(parentId, canonicalPartId, StringComparison.OrdinalIgnoreCase);
            bool quadrantParent = RequiresQuadrant(parentId) &&
                                  canonicalPartId.StartsWith(parentId + "_", StringComparison.OrdinalIgnoreCase);
            return directParent || quadrantParent ? NormalizeSubpieceLookupToken(target.name) : string.Empty;
        }

        public static bool HotspotContains(HotspotGroupDefinition group, string canonicalPartId)
        {
            if (group == null || group.canonicalPartIds == null || string.IsNullOrWhiteSpace(canonicalPartId))
            {
                return false;
            }

            for (int i = 0; i < group.canonicalPartIds.Length; i++)
            {
                if (string.Equals(group.canonicalPartIds[i], canonicalPartId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static string NormalizeToken(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            string value = rawValue.Trim().ToLowerInvariant();
            const string blenderPrefix = "x500v2_blend_";
            if (value.StartsWith(blenderPrefix, StringComparison.Ordinal))
            {
                value = value.Substring(blenderPrefix.Length);
            }

            char[] buffer = new char[value.Length];
            int length = 0;
            bool previousSeparator = false;
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (char.IsLetterOrDigit(ch))
                {
                    buffer[length++] = ch;
                    previousSeparator = false;
                }
                else if (!previousSeparator)
                {
                    buffer[length++] = '-';
                    previousSeparator = true;
                }
            }

            while (length > 0 && buffer[length - 1] == '-')
            {
                length--;
            }

            return new string(buffer, 0, length);
        }

        private static string NormalizeSubpieceLookupToken(string rawValue)
        {
            string normalized = NormalizeToken(rawValue);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return string.Empty;
            }

            if (normalized.EndsWith("-low", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 4);
            }

            int lastDash = normalized.LastIndexOf('-');
            if (lastDash > 0 && lastDash + 1 < normalized.Length)
            {
                string suffix = normalized.Substring(lastDash + 1);
                bool numericSuffix = true;
                for (int i = 0; i < suffix.Length; i++)
                {
                    if (!char.IsDigit(suffix[i]))
                    {
                        numericSuffix = false;
                        break;
                    }
                }

                if (numericSuffix)
                {
                    normalized = normalized.Substring(0, lastDash);
                }
            }

            return normalized;
        }

        private static SelectionHierarchyCatalog GetCatalog()
        {
            if (cachedCatalog != null)
            {
                return cachedCatalog;
            }

            TextAsset resource = Resources.Load<TextAsset>(ResourcePath);
            if (resource != null)
            {
                try
                {
                    cachedCatalog = JsonUtility.FromJson<SelectionHierarchyCatalog>(resource.text);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("[SelectionHierarchy] No se pudo parsear holybro_selection_hierarchy.json: " + ex.Message);
                }
            }

            if (cachedCatalog == null || cachedCatalog.hotspotGroups == null || cachedCatalog.canonicalGroups == null)
            {
                cachedCatalog = BuildFallbackCatalog();
            }

            return cachedCatalog;
        }

        private static void EnsureLookups()
        {
            if (canonicalById != null && subpieceToCanonical != null)
            {
                return;
            }

            canonicalById = new Dictionary<string, CanonicalSelectionGroupDefinition>(StringComparer.OrdinalIgnoreCase);
            subpieceToCanonical = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            SelectionHierarchyCatalog catalog = GetCatalog();
            for (int i = 0; i < catalog.canonicalGroups.Length; i++)
            {
                CanonicalSelectionGroupDefinition group = catalog.canonicalGroups[i];
                if (group == null || string.IsNullOrWhiteSpace(group.canonicalPartId))
                {
                    continue;
                }

                canonicalById[group.canonicalPartId] = group;
                if (group.subpieceNames == null)
                {
                    continue;
                }

                for (int j = 0; j < group.subpieceNames.Length; j++)
                {
                    string normalized = NormalizeToken(group.subpieceNames[j]);
                    if (!string.IsNullOrWhiteSpace(normalized) && !subpieceToCanonical.ContainsKey(normalized))
                    {
                        subpieceToCanonical[normalized] = group.canonicalPartId;
                    }
                }
            }
        }

        private static bool RequiresQuadrant(string canonicalPartId)
        {
            return string.Equals(canonicalPartId, "x500v2_arm", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(canonicalPartId, "x500v2_motor", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(canonicalPartId, "x500v2_esc", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(canonicalPartId, "x500v2_prop", StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveQuadrantSuffix(string lowerName)
        {
            if (string.IsNullOrWhiteSpace(lowerName)) return string.Empty;
            if (lowerName.Contains("_fl") || lowerName.Contains("-fl") || lowerName.Contains("front-left")) return "FL";
            if (lowerName.Contains("_fr") || lowerName.Contains("-fr") || lowerName.Contains("front-right")) return "FR";
            if (lowerName.Contains("_bl") || lowerName.Contains("-bl") || lowerName.Contains("back-left")) return "BL";
            if (lowerName.Contains("_br") || lowerName.Contains("-br") || lowerName.Contains("back-right")) return "BR";
            string blenderInstanceSuffix = ResolveBlenderInstanceQuadrantSuffix(lowerName);
            if (!string.IsNullOrWhiteSpace(blenderInstanceSuffix)) return blenderInstanceSuffix;
            return string.Empty;
        }

        private static string ResolveBlenderInstanceQuadrantSuffix(string lowerName)
        {
            if (string.IsNullOrWhiteSpace(lowerName))
            {
                return string.Empty;
            }

            string normalized = NormalizeToken(lowerName);
            // Observed in the final Blender export / imported scene:
            // .001=BR, .002=FR, .003=FL, .004=BL for fourfold quadrant instances.
            if (normalized.Contains("-001-low")) return "BR";
            if (normalized.Contains("-002-low")) return "FR";
            if (normalized.Contains("-003-low")) return "FL";
            if (normalized.Contains("-004-low")) return "BL";
            if (normalized.EndsWith("-001", StringComparison.Ordinal)) return "BR";
            if (normalized.EndsWith("-002", StringComparison.Ordinal)) return "FR";
            if (normalized.EndsWith("-003", StringComparison.Ordinal)) return "FL";
            if (normalized.EndsWith("-004", StringComparison.Ordinal)) return "BL";
            return string.Empty;
        }

        private static string ResolveQuadrantSuffixFromWorld(Vector3 worldPosition, Transform root)
        {
            Vector3 center = root != null && TryComputeWorldCenter(root, out Vector3 rootCenter) ? rootCenter : Vector3.zero;
            float dx = worldPosition.x - center.x;
            float dz = worldPosition.z - center.z;
            if (Mathf.Abs(dx) < 0.0001f && Mathf.Abs(dz) < 0.0001f)
            {
                return string.Empty;
            }

            return (dz >= 0f ? "F" : "B") + (dx < 0f ? "L" : "R");
        }

        private static bool TryComputeWorldCenter(Transform root, out Vector3 center)
        {
            center = Vector3.zero;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            Bounds bounds = default;
            bool hasBounds = false;
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || renderer.transform == root)
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

            if (!hasBounds)
            {
                return false;
            }

            center = bounds.center;
            return true;
        }

        private static SelectionHierarchyCatalog BuildFallbackCatalog()
        {
            return new SelectionHierarchyCatalog
            {
                version = "fallback-2026-05-08",
                hotspotGroups = new[]
                {
                    Hotspot("hotspot_power_distribution", "Power Distribution", "PM06/XT60 power routing. PDB is documented but not synthesized when absent from the FBX.", true, "x500v2_power_module"),
                    Hotspot("hotspot_flight_controller", "Flight Controller", "Pixhawk 6C autopilot stack.", false, "x500v2_pixhawk6c"),
                    Hotspot("hotspot_gps_compass", "GPS & Compass", "GPS mast, antenna and compass module.", false, "x500v2_gps_m10"),
                    Hotspot("hotspot_propulsion", "Propulsion System", "Four arms, motors, propellers and associated primitive fasteners. ESCs are documented but not synthesized when absent from the FBX.", true,
                        "x500v2_arm_FL", "x500v2_arm_FR", "x500v2_arm_BL", "x500v2_arm_BR",
                        "x500v2_motor_FL", "x500v2_motor_FR", "x500v2_motor_BL", "x500v2_motor_BR",
                        "x500v2_prop_FL", "x500v2_prop_FR", "x500v2_prop_BL", "x500v2_prop_BR"),
                    Hotspot("hotspot_battery", "Battery", "Battery and physical rail/mount system.", true, "x500v2_battery", "x500v2_rails_battery")
                },
                canonicalGroups = new[]
                {
                    Group("x500v2_bottom_plate", "Carbon Fiber Bottom Plate", "BOTTOM-PLATE-X500-V5", "GAI-GUANGLIU", "ZHIJIA-CAMERA-INTEL"),
                    Group("x500v2_top_plate", "Carbon Fiber Top Plate", "TOP-PLATE-X500-V5"),
                    Group("x500v2_platform_board", "Platform Board", "PLATFORM-PLAT-X500"),
                    Group("x500v2_rails_battery", "Rail System & Battery Mount", "BATTERY-MOUNTING-PLAT", "BATTERY-PAD", "PYLONS-X500", "GUAN-CHENG"),
                    Group("x500v2_pdb", "Power Distribution Board", "PDB"),
                    Group("x500v2_power_module", "Power Module PM06/XT60", "PCB-PM06", "TOU-XT60H-M-14AWG", "X500-TAO-XT60"),
                    Group("x500v2_pixhawk6c", "Pixhawk 6C Autopilot", "DIKE-PIXHAWK6C-LV-C1", "IMU-PIXHAWK6C", "MIANKE-PIXHAWK6C-LV-C1", "PCB-PIXHAWK6C-F1", "BM06B-WO"),
                    Group("x500v2_gps_m10", "Holybro M10 GPS Module", "GAN-GPSV5-ZHIJIA", "GPS-ZHIJIA-ZHUANJIETOU", "GPS-ZHIJIA-ZUO", "GPSV5-ZHIJIA-TUOPAN"),
                    Group("x500v2_landing_gear", "Landing Gear", "CARBON-FIBER-TUBE", "JIAO-EVA", "JIAO-LIANJIE", "MAO-JIAO"),
                    Group("x500v2_arm", "Arm Assembly Quadrant", "CARBON-FIBER-TUBE300", "HMX5V-DIGAI-DIANJIZUO-MUJU", "HMX5V-GUAN-DINGWEI", "HMX5V-JIBI-JIA-MUJU", "HMX5V-ZUO-DJ-MUJU", "BAN-DJ-DIAN-F2", "JIA-GUAN"),
                    Group("x500v2_motor", "Motor 2216 Quadrant", "DJ-2216-KV880"),
                    Group("x500v2_esc", "ESC Quadrant", "ESC"),
                    Group("x500v2_prop", "Propeller Quadrant", "PROPELLER", "PROP"),
                    Group("x500v2_battery", "LiPo Battery", "BATTERY", "BATTERY-STRAP"),
                    Group("x500v2_rc_receiver", "RC Receiver", "RECEIVER"),
                    Group("x500v2_telemetry_radio", "Telemetry Radio", "TELEMETRY", "RADIO")
                }
            };
        }

        private static HotspotGroupDefinition Hotspot(string id, string label, string summary, bool includeFasteners, params string[] canonicalPartIds)
        {
            return new HotspotGroupDefinition
            {
                groupId = id,
                label = label,
                summary = summary,
                includeAssociatedFasteners = includeFasteners,
                canonicalPartIds = canonicalPartIds
            };
        }

        private static CanonicalSelectionGroupDefinition Group(string id, string displayName, params string[] subpieceNames)
        {
            return new CanonicalSelectionGroupDefinition
            {
                canonicalPartId = id,
                displayName = displayName,
                subpieceNames = subpieceNames
            };
        }
    }
}
