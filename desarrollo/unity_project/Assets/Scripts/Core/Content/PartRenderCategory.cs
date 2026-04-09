using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public class PartRenderCategory : MonoBehaviour
    {
        [SerializeField] private string canonicalPartId;
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

        public void Configure(string canonicalId, string primary, string auxiliary, string thermalSourceId)
        {
            canonicalPartId = canonicalId ?? string.Empty;
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
}
