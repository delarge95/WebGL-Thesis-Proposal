using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [DisallowMultipleComponent]
    public class FastenerRegistry : Singleton<FastenerRegistry>
    {
        [Header("Resource Paths")]
        [SerializeField] private string familiesResourcePath = "holybro_fastener_families";
        [SerializeField] private string instancesResourcePath = "holybro_fastener_instances";
        [SerializeField] private string reconciliationResourcePath = "holybro_fastener_reconciliation";
        [SerializeField] private bool logWarnings = true;

        private readonly Dictionary<string, FastenerFamilyDefinition> familiesById = new Dictionary<string, FastenerFamilyDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FastenerFamilyDefinition> familiesByTypeKey = new Dictionary<string, FastenerFamilyDefinition>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FastenerInstanceDefinition> instancesById = new Dictionary<string, FastenerInstanceDefinition>(StringComparer.OrdinalIgnoreCase);

        private FastenerFamilyCatalogJson familiesCatalog;
        private FastenerInstanceCatalogJson instancesCatalog;
        private FastenerReconciliationJson reconciliationCatalog;

        public IReadOnlyDictionary<string, FastenerFamilyDefinition> FamiliesById => familiesById;
        public IReadOnlyDictionary<string, FastenerInstanceDefinition> InstancesById => instancesById;
        public FastenerReconciliationJson ReconciliationCatalog => reconciliationCatalog;

        protected override void Awake()
        {
            base.Awake();
            RebuildRegistry();
        }

        public void RebuildRegistry()
        {
            familiesById.Clear();
            familiesByTypeKey.Clear();
            instancesById.Clear();

            familiesCatalog = LoadCatalog<FastenerFamilyCatalogJson>(familiesResourcePath);
            instancesCatalog = LoadCatalog<FastenerInstanceCatalogJson>(instancesResourcePath);
            reconciliationCatalog = LoadCatalog<FastenerReconciliationJson>(reconciliationResourcePath);

            if (familiesCatalog != null && familiesCatalog.items != null)
            {
                for (int i = 0; i < familiesCatalog.items.Length; i++)
                {
                    FastenerFamilyDefinition family = familiesCatalog.items[i];
                    if (family == null || string.IsNullOrWhiteSpace(family.familyId))
                    {
                        continue;
                    }

                    familiesById[family.familyId] = family;
                    if (!string.IsNullOrWhiteSpace(family.sceneTypeKey))
                    {
                        familiesByTypeKey[family.sceneTypeKey] = family;
                    }
                }
            }

            if (instancesCatalog != null && instancesCatalog.items != null)
            {
                for (int i = 0; i < instancesCatalog.items.Length; i++)
                {
                    FastenerInstanceDefinition instance = instancesCatalog.items[i];
                    if (instance == null || string.IsNullOrWhiteSpace(instance.instanceId))
                    {
                        continue;
                    }

                    instancesById[instance.instanceId] = instance;
                }
            }
        }

        public bool TryGetFamilyDefinition(string familyId, out FastenerFamilyDefinition family)
        {
            family = null;
            return !string.IsNullOrWhiteSpace(familyId) && familiesById.TryGetValue(familyId, out family);
        }

        public bool TryGetInstanceDefinition(string instanceId, out FastenerInstanceDefinition instance)
        {
            instance = null;
            return !string.IsNullOrWhiteSpace(instanceId) && instancesById.TryGetValue(instanceId, out instance);
        }

        public bool TryResolveMetadata(string instanceId, out FastenerMetadata metadata)
        {
            metadata = null;
            if (!TryGetInstanceDefinition(instanceId, out FastenerInstanceDefinition instance))
            {
                return false;
            }

            FastenerFamilyDefinition family = null;
            if (!string.IsNullOrWhiteSpace(instance.familyId))
            {
                TryGetFamilyDefinition(instance.familyId, out family);
            }

            metadata = FastenerMetadata.Combine(family, instance);
            return metadata != null;
        }

        public FastenerMetadata ResolveMetadata(Transform target, DronePartData fallbackData = null)
        {
            if (fallbackData != null && fallbackData.HasFastenerMetadata)
            {
                return fallbackData.fastenerMetadata;
            }

            if (target == null)
            {
                return null;
            }

            FastenerRuntimeMarker marker = target.GetComponent<FastenerRuntimeMarker>();
            if (marker == null)
            {
                marker = target.GetComponentInParent<FastenerRuntimeMarker>();
            }

            if (marker != null && TryResolveMetadata(marker.FastenerInstanceId, out FastenerMetadata metadata))
            {
                return metadata;
            }

            string instanceId = FastenerNamingUtility.SanitizeId(target.name);
            if (TryResolveMetadata(instanceId, out FastenerMetadata byName))
            {
                return byName;
            }

            string sceneTypeKey = FastenerNamingUtility.ExtractSceneTypeKey(target.name);
            if (!string.IsNullOrWhiteSpace(sceneTypeKey) &&
                familiesByTypeKey.TryGetValue(sceneTypeKey, out FastenerFamilyDefinition family))
            {
                return FastenerMetadata.Combine(family, null);
            }

            return null;
        }

        public void SealMarker(Transform target, FastenerMetadata metadata)
        {
            if (target == null || metadata == null)
            {
                return;
            }

            FastenerRuntimeMarker marker = target.GetComponent<FastenerRuntimeMarker>();

            // NEVER overwrite an existing marker that already has a valid instanceId
            // with a DIFFERENT instanceId. This prevents the ancestor anchor's metadata
            // from clobbering a child renderer's own fastener identity after reparenting.
            if (marker != null
                && !string.IsNullOrWhiteSpace(marker.FastenerInstanceId)
                && !string.IsNullOrWhiteSpace(metadata.instanceId)
                && !string.Equals(marker.FastenerInstanceId, metadata.instanceId, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (marker == null)
            {
                marker = target.gameObject.AddComponent<FastenerRuntimeMarker>();
            }

            marker.Configure(
                metadata.familyId,
                metadata.instanceId,
                metadata.sceneTypeKey,
                metadata.parentCanonicalPartId,
                metadata.isInspectable,
                metadata.fallbackReason);
        }

        private T LoadCatalog<T>(string resourcePath) where T : class
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return null;
            }

            TextAsset asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
            {
                if (logWarnings)
                {
                    Debug.LogWarning($"[FastenerRegistry] No se encontro TextAsset en Resources/{resourcePath}.json");
                }

                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(asset.text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FastenerRegistry] No se pudo parsear {resourcePath}: {ex.Message}");
                return null;
            }
        }
    }
}
