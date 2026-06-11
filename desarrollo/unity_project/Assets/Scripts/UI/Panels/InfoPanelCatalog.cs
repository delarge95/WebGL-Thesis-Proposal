using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using WebGL.Core.Data;

namespace WebGL.UI.Panels
{
    [Serializable]
    public sealed class InfoPanelCatalogRoot
    {
        public string version;
        public string status;
        public InfoPanelCatalogEntry[] entries;
    }

    [Serializable]
    public sealed class InfoPanelCatalogEntry
    {
        public string entryId;
        public string entryType;
        public string[] selectionIds;
        public string canonicalPartId;
        public string[] parentIds;
        public string[] childIds;
        public string[] aliases;
        public string sourceConfidence;
        public string[] sourceRefs;
        public InfoPanelLanguageBlock en;
        public InfoPanelLanguageBlock es;
        public string[] notes;
    }

    [Serializable]
    public sealed class InfoPanelLanguageBlock
    {
        public string title;
        public string summary;
        public InfoPanelField[] identification;
        public InfoPanelField[] specifications;
        public InfoPanelRelationship relationship;
        public string[] assembly;
        public InfoPanelField[] keyReferences;
    }

    [Serializable]
    public sealed class InfoPanelField
    {
        public string label;
        public string value;
    }

    [Serializable]
    public sealed class InfoPanelRelationship
    {
        public string title;
        public string[] items;
    }

    public sealed class InfoPanelCatalogService
    {
        private const string ResourceName = "x500v2_info_panel_bilingual_catalog";
        private static InfoPanelCatalogService instance;

        private readonly Dictionary<string, InfoPanelCatalogEntry> lookup = new Dictionary<string, InfoPanelCatalogEntry>();
        private readonly List<KeyValuePair<string, InfoPanelCatalogEntry>> looseLookup = new List<KeyValuePair<string, InfoPanelCatalogEntry>>();
        private InfoPanelCatalogRoot catalog;
        private bool loaded;

        public static InfoPanelCatalogService Instance => instance ?? (instance = new InfoPanelCatalogService());

        public string Version => catalog != null ? catalog.version : string.Empty;

        public bool TryResolve(
            DronePartData data,
            bool fromHotspot,
            string hotspotGroupLabel,
            string selectionLabel,
            string canonicalPartName,
            out InfoPanelCatalogEntry entry)
        {
            EnsureLoaded();
            entry = null;

            if (!loaded || data == null)
            {
                return false;
            }

            if (data.HasFastenerMetadata && data.fastenerMetadata != null)
            {
                if (TryFindExact(data.fastenerMetadata.familyId, out entry) ||
                    TryFindExact(data.fastenerMetadata.sourceId, out entry) ||
                    TryFindExact(data.fastenerMetadata.blenderName, out entry) ||
                    TryFindLoose(data.fastenerMetadata.sceneTypeKey, out entry) ||
                    TryFindLoose(data.fastenerMetadata.instanceId, out entry))
                {
                    return true;
                }
            }

            if (fromHotspot && !string.IsNullOrWhiteSpace(hotspotGroupLabel))
            {
                if (TryFindExact(hotspotGroupLabel, out entry) || TryFindLoose(hotspotGroupLabel, out entry))
                {
                    return true;
                }
            }

            string[] candidates =
            {
                selectionLabel,
                canonicalPartName,
                data.id,
                data.partName,
                data.partNumber,
                data.hotspotLabel
            };

            foreach (string candidate in candidates)
            {
                if (TryFindExact(candidate, out entry))
                {
                    return true;
                }
            }

            foreach (string candidate in candidates)
            {
                if (TryFindLoose(candidate, out entry))
                {
                    return true;
                }
            }

            return false;
        }

        public static InfoPanelLanguageBlock GetLanguageBlock(InfoPanelCatalogEntry entry)
        {
            if (entry == null)
            {
                return null;
            }

            return AppLanguageManager.IsSpanish ? (entry.es ?? entry.en) : (entry.en ?? entry.es);
        }

        private void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            TextAsset asset = Resources.Load<TextAsset>(ResourceName);
            if (asset == null)
            {
                Debug.LogWarning($"[InfoPanelCatalog] Resource '{ResourceName}' was not found. Falling back to ScriptableObject part data.");
                loaded = true;
                return;
            }

            catalog = JsonUtility.FromJson<InfoPanelCatalogRoot>(asset.text);
            if (catalog == null || catalog.entries == null || catalog.entries.Length == 0)
            {
                Debug.LogWarning("[InfoPanelCatalog] Catalog JSON could not be parsed or contains no entries.");
                loaded = true;
                return;
            }

            foreach (InfoPanelCatalogEntry entry in catalog.entries)
            {
                RegisterEntry(entry);
            }

            loaded = true;
            Debug.Log($"[InfoPanelCatalog] Loaded {catalog.entries.Length} entries ({catalog.version}).");
        }

        private void RegisterEntry(InfoPanelCatalogEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            AddLookup(entry.entryId, entry);
            AddLookup(entry.canonicalPartId, entry);
            AddLookups(entry.selectionIds, entry);
            AddLookups(entry.aliases, entry);
            AddLookup(entry.en != null ? entry.en.title : string.Empty, entry);
            AddLookup(entry.es != null ? entry.es.title : string.Empty, entry);
        }

        private void AddLookups(string[] values, InfoPanelCatalogEntry entry)
        {
            if (values == null)
            {
                return;
            }

            foreach (string value in values)
            {
                AddLookup(value, entry);
            }
        }

        private void AddLookup(string value, InfoPanelCatalogEntry entry)
        {
            string key = NormalizeKey(value);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (!lookup.ContainsKey(key))
            {
                lookup.Add(key, entry);
                looseLookup.Add(new KeyValuePair<string, InfoPanelCatalogEntry>(key, entry));
            }
        }

        private bool TryFindExact(string value, out InfoPanelCatalogEntry entry)
        {
            entry = null;
            string key = NormalizeKey(value);
            return !string.IsNullOrWhiteSpace(key) && lookup.TryGetValue(key, out entry);
        }

        private bool TryFindLoose(string value, out InfoPanelCatalogEntry entry)
        {
            entry = null;
            string key = NormalizeKey(value);
            if (string.IsNullOrWhiteSpace(key) || key.Length < 8)
            {
                return false;
            }

            if (lookup.TryGetValue(key, out entry))
            {
                return true;
            }

            foreach (KeyValuePair<string, InfoPanelCatalogEntry> pair in looseLookup)
            {
                if (pair.Key.Length < 8)
                {
                    continue;
                }

                if (key.Contains(pair.Key) || pair.Key.Contains(key))
                {
                    entry = pair.Value;
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"\.\d{3,}", string.Empty);
            normalized = Regex.Replace(normalized, @"(_low|_mesh|_renderer|_proxy)$", string.Empty);
            normalized = normalized.Replace("shared fasteners:", string.Empty);
            normalized = normalized.Replace("fasteners:", string.Empty);
            normalized = Regex.Replace(normalized, @"\bx\s*\d+\b", string.Empty);
            normalized = Regex.Replace(normalized, @"[^a-z0-9]+", string.Empty);
            return normalized;
        }
    }
}
