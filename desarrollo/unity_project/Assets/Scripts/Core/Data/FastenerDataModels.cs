using System;
using System.Globalization;
using UnityEngine;

namespace WebGL.Core.Data
{
    [Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3()
        {
        }

        public SerializableVector3(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class FastenerModularRecipe
    {
        public string recipeKey;
        public string builderType;
        public string orientationMode;
        public int middleSegments;
        public float headLengthRatio;
        public float shaftLengthRatio;
        public float tipLengthRatio;
        public float headDiameterScale;
        public float shaftDiameterScale;
        public bool isInspectable = true;
    }

    [Serializable]
    public class FastenerFamilyDefinition
    {
        public string familyId;
        public string sourceId;
        public string blenderName;
        public string sceneTypeKey;
        public string subtype;
        public string metric;
        public float nominalDiameterMm;
        public float lengthMm;
        public string headProfile;
        public string driveType;
        public float threadPitchMm;
        public int turnCount;
        public string material;
        public string finish;
        public int quantityExpected;
        public string notes;
        public FastenerModularRecipe modularRecipe;
    }

    [Serializable]
    public class FastenerInstanceDefinition
    {
        public string instanceId;
        public string familyId;
        public string sceneObjectName;
        public string hierarchyPath;
        public string parentCanonicalPartId;
        public SerializableVector3 localPosition;
        public SerializableVector3 localRotationEuler;
        public SerializableVector3 localScale;
        public bool isInspectable;
        public string fallbackReason;
    }

    [Serializable]
    public class FastenerReconciliationAlias
    {
        public string sceneTypeKey;
        public string sourceId;
        public string blenderName;
        public string rationale;
    }

    [Serializable]
    public class FastenerReconciliationIssue
    {
        public string severity;
        public string scope;
        public string sceneTypeKey;
        public string sourceId;
        public string instanceId;
        public string message;
    }

    [Serializable]
    public class FastenerMetadata
    {
        public string instanceId;
        public string familyId;
        public string sourceId;
        public string sceneObjectName;
        public string hierarchyPath;
        public string parentCanonicalPartId;
        public string blenderName;
        public string sceneTypeKey;
        public string subtype;
        public string metric;
        public float nominalDiameterMm;
        public float lengthMm;
        public string headProfile;
        public string driveType;
        public float threadPitchMm;
        public int turnCount;
        public string material;
        public string finish;
        public int quantityExpected;
        public bool isInspectable;
        public string fallbackReason;
        public string notes;
        public string recipeKey;
        public string builderType;
        public string orientationMode;

        public string GetDisplayName()
        {
            string baseName = FastenerNamingUtility.ToTitleCase(subtype);
            string dimension = FastenerNamingUtility.FormatMetricAndLength(metric, lengthMm);

            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "Fastener";
            }

            if (string.IsNullOrWhiteSpace(dimension))
            {
                return baseName;
            }

            return $"{baseName} {dimension}";
        }

        public string GetTechnicalSummary()
        {
            string dimension = FastenerNamingUtility.FormatMetricAndLength(metric, lengthMm);
            string head = string.IsNullOrWhiteSpace(headProfile) ? string.Empty : headProfile;
            string drive = string.IsNullOrWhiteSpace(driveType) || string.Equals(driveType, "N/A", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : driveType;

            string summary = dimension;
            if (!string.IsNullOrWhiteSpace(head))
            {
                summary = string.IsNullOrWhiteSpace(summary) ? head : $"{summary} | {head}";
            }

            if (!string.IsNullOrWhiteSpace(drive))
            {
                summary = string.IsNullOrWhiteSpace(summary) ? drive : $"{summary} | {drive}";
            }

            return summary;
        }

        public bool HasIdentifiers()
        {
            return !string.IsNullOrWhiteSpace(instanceId) || !string.IsNullOrWhiteSpace(familyId);
        }

        public static FastenerMetadata Combine(FastenerFamilyDefinition family, FastenerInstanceDefinition instance)
        {
            if (family == null && instance == null)
            {
                return null;
            }

            return new FastenerMetadata
            {
                instanceId = instance != null ? instance.instanceId : string.Empty,
                familyId = family != null ? family.familyId : (instance != null ? instance.familyId : string.Empty),
                sourceId = family != null ? family.sourceId : string.Empty,
                sceneObjectName = instance != null ? instance.sceneObjectName : string.Empty,
                hierarchyPath = instance != null ? instance.hierarchyPath : string.Empty,
                parentCanonicalPartId = instance != null ? instance.parentCanonicalPartId : string.Empty,
                blenderName = family != null ? family.blenderName : string.Empty,
                sceneTypeKey = family != null ? family.sceneTypeKey : string.Empty,
                subtype = family != null ? family.subtype : string.Empty,
                metric = family != null ? family.metric : string.Empty,
                nominalDiameterMm = family != null ? family.nominalDiameterMm : 0f,
                lengthMm = family != null ? family.lengthMm : 0f,
                headProfile = family != null ? family.headProfile : string.Empty,
                driveType = family != null ? family.driveType : string.Empty,
                threadPitchMm = family != null ? family.threadPitchMm : 0f,
                turnCount = family != null ? family.turnCount : 0,
                material = family != null ? family.material : string.Empty,
                finish = family != null ? family.finish : string.Empty,
                quantityExpected = family != null ? family.quantityExpected : 0,
                isInspectable = instance != null ? instance.isInspectable : (family == null || family.modularRecipe == null || family.modularRecipe.isInspectable),
                fallbackReason = instance != null ? instance.fallbackReason : string.Empty,
                notes = family != null ? family.notes : string.Empty,
                recipeKey = family != null && family.modularRecipe != null ? family.modularRecipe.recipeKey : string.Empty,
                builderType = family != null && family.modularRecipe != null ? family.modularRecipe.builderType : string.Empty,
                orientationMode = family != null && family.modularRecipe != null ? family.modularRecipe.orientationMode : string.Empty
            };
        }
    }

    [Serializable]
    public class FastenerFamilyCatalogJson
    {
        public string sourceScene;
        public string sourceDataset;
        public string generatedAtUtc;
        public FastenerFamilyDefinition[] items;
    }

    [Serializable]
    public class FastenerInstanceCatalogJson
    {
        public string sourceScene;
        public string sourceDataset;
        public string generatedAtUtc;
        public FastenerInstanceDefinition[] items;
    }

    [Serializable]
    public class FastenerReconciliationJson
    {
        public string sourceScene;
        public string sourceDataset;
        public string generatedAtUtc;
        public FastenerReconciliationAlias[] aliases;
        public FastenerReconciliationIssue[] issues;
    }

    public static class FastenerNamingUtility
    {
        public static string SanitizeId(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return "fastener_instance";
            }

            char[] buffer = new char[rawName.Length];
            int length = 0;
            foreach (char character in rawName)
            {
                buffer[length++] = char.IsLetterOrDigit(character) || character == '_' || character == '-'
                    ? character
                    : '_';
            }

            return new string(buffer, 0, length);
        }

        public static string ExtractSceneTypeKey(string sceneObjectName)
        {
            if (string.IsNullOrWhiteSpace(sceneObjectName))
            {
                return string.Empty;
            }

            string normalized = sceneObjectName;
            const string prefix = "x500v2_fastener.";
            if (normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(prefix.Length);
            }

            int lastUnderscore = normalized.LastIndexOf('_');
            if (lastUnderscore > 0 && lastUnderscore + 1 < normalized.Length)
            {
                string suffix = normalized.Substring(lastUnderscore + 1);
                if (int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    return normalized.Substring(0, lastUnderscore);
                }
            }

            return normalized;
        }

        public static string FormatMetricAndLength(string metric, float lengthMm)
        {
            string cleanMetric = string.IsNullOrWhiteSpace(metric) ? string.Empty : metric.Trim();
            bool hasLength = lengthMm > 0.0001f;

            if (string.IsNullOrWhiteSpace(cleanMetric))
            {
                return hasLength ? $"{lengthMm.ToString("0.##", CultureInfo.InvariantCulture)} mm" : string.Empty;
            }

            return hasLength
                ? $"{cleanMetric} x {lengthMm.ToString("0.##", CultureInfo.InvariantCulture)} mm"
                : cleanMetric;
        }

        public static string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string raw = value.Trim();
            char[] expanded = new char[raw.Length * 2];
            int length = 0;

            for (int i = 0; i < raw.Length; i++)
            {
                char current = raw[i];
                if (i > 0 &&
                    char.IsUpper(current) &&
                    (char.IsLower(raw[i - 1]) || char.IsDigit(raw[i - 1])))
                {
                    expanded[length++] = ' ';
                }

                expanded[length++] = current;
            }

            string normalized = new string(expanded, 0, length)
                .Replace('_', ' ')
                .Replace('-', ' ')
                .Trim();

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized.ToLowerInvariant());
        }
    }
}
