using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebGL.Core.Data;
using WebGL.Core.Content;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class UICategory
    {
        public PartCategory categoryEnum;
        public string displayName;
        public Sprite icon;
    }

    public class PartCatalogManager : Singleton<PartCatalogManager>
    {
        [Header("Categories")]
        [SerializeField] private UICategory[] uiCategories = new UICategory[]
        {
            new UICategory { categoryEnum = PartCategory.Avionics, displayName = "Avionics" },
            new UICategory { categoryEnum = PartCategory.SensorsComms, displayName = "Sensors & Comms" },
            new UICategory { categoryEnum = PartCategory.PowerDistribution, displayName = "Power Distribution" },
            new UICategory { categoryEnum = PartCategory.PropulsionSystem, displayName = "Propulsion System" },
            new UICategory { categoryEnum = PartCategory.SkeletonAirframe, displayName = "Skeleton & Airframe" },
            new UICategory { categoryEnum = PartCategory.Fasteners, displayName = "Fasteners" }
        };

        private List<ExplodablePart> allParts = new List<ExplodablePart>();
        private List<ExplodablePart> filteredParts = new List<ExplodablePart>();
        private string currentSearchQuery = "";
        private string currentCategoryFilter = "";
        private string currentMaterialFilter = "";

        public List<ExplodablePart> AllParts => allParts;
        public List<ExplodablePart> FilteredParts => filteredParts;
        public UICategory[] Categories => uiCategories;

        public event Action<List<ExplodablePart>> OnFilterChanged;
        public event Action<ExplodablePart> OnPartFocused;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RefreshPartsList();
        }

        public void RefreshPartsList()
        {
            allParts.Clear();
            allParts.AddRange(FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None));
            filteredParts = new List<ExplodablePart>(allParts);
            Debug.Log($"[PartCatalog] Found {allParts.Count} parts");
        }

        public void Search(string query)
        {
            currentSearchQuery = query.ToLower().Trim();
            ApplyFilters();
        }

        public void FilterByCategory(string category)
        {
            currentCategoryFilter = category;
            ApplyFilters();
        }

        public void FilterByMaterial(string material)
        {
            currentMaterialFilter = material;
            ApplyFilters();
        }

        public void ClearFilters()
        {
            currentSearchQuery = "";
            currentCategoryFilter = "";
            currentMaterialFilter = "";
            filteredParts = new List<ExplodablePart>(allParts);
            OnFilterChanged?.Invoke(filteredParts);
        }

        private void ApplyFilters()
        {
            filteredParts = allParts.Where(part =>
            {
                if (part == null || part.Data == null) return false;

                // Search filter
                if (!string.IsNullOrEmpty(currentSearchQuery))
                {
                    bool matchesSearch = 
                        part.Data.partName.ToLower().Contains(currentSearchQuery) ||
                        part.Data.description.ToLower().Contains(currentSearchQuery) ||
                        part.Data.partType.ToLower().Contains(currentSearchQuery);
                    
                    if (!matchesSearch) return false;
                }

                // Category filter
                if (!string.IsNullOrEmpty(currentCategoryFilter))
                {
                    var uiCat = uiCategories.FirstOrDefault(c => c.displayName == currentCategoryFilter);
                    if (uiCat != null && part.Data.category != uiCat.categoryEnum)
                    {
                        return false;
                    }
                }

                // Material filter
                if (!string.IsNullOrEmpty(currentMaterialFilter))
                {
                    if (part.Data.materialType != currentMaterialFilter)
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            Debug.Log($"[PartCatalog] Filtered to {filteredParts.Count} parts");
            OnFilterChanged?.Invoke(filteredParts);
        }

        public void FocusPart(ExplodablePart part)
        {
            if (part == null) return;

            // Publish selection event
            if (part.Data != null)
            {
                EventBus.Publish(new PartSelectedEvent(part.Data));
            }

            // Focus camera on part
            if (OrbitCameraController.Instance != null)
            {
                OrbitCameraController.Instance.SetTarget(part.transform);
            }

            // Make part visible
            PartVisibilityManager.Instance?.ShowPart(part);
            PartVisibilityManager.Instance?.IsolatePart(part);

            OnPartFocused?.Invoke(part);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClick();
            }
        }

        public List<string> GetAllMaterials()
        {
            return allParts
                .Where(p => p != null && p.Data != null)
                .Select(p => p.Data.materialType)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
        }

        public List<string> GetAllPartTypes()
        {
            return allParts
                .Where(p => p != null && p.Data != null)
                .Select(p => p.Data.partType)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        public ExplodablePart GetPartByName(string name)
        {
            return allParts.FirstOrDefault(p => p.Data != null && p.Data.partName == name);
        }
    }
}
