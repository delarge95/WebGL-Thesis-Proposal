using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class BOMItem
    {
        public string partId;
        public string partName;
        public string partNumber;
        public string category;
        public int quantity;
        public float unitWeight;
        public float totalWeight;
        public string material;
        public string supplier;
        public bool isAvailable;
    }

    public class BillOfMaterialsManager : Singleton<BillOfMaterialsManager>
    {
        private List<BOMItem> bomItems = new List<BOMItem>();

        public List<BOMItem> Items => bomItems;
        public float TotalWeight => bomItems.Sum(i => i.totalWeight);
        public int TotalParts => bomItems.Sum(i => i.quantity);
        public int UniquePartsCount => bomItems.Count;

        private void Start()
        {
            GenerateBOM();
        }

        public void GenerateBOM()
        {
            bomItems.Clear();

            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            var partGroups = parts
                .Where(p => p.Data != null)
                .GroupBy(p => p.Data.id);

            foreach (var group in partGroups)
            {
                var first = group.First();
                var data = first.Data;

                var item = new BOMItem
                {
                    partId = data.id,
                    partName = data.partName,
                    partNumber = data.partNumber,
                    category = data.partType,
                    quantity = group.Count(),
                    unitWeight = data.weightKg,
                    totalWeight = data.weightKg * group.Count(),
                    material = data.materialType,
                    supplier = data.manufacturer,
                    isAvailable = true
                };

                bomItems.Add(item);
            }

            // Sort by category then name
            bomItems = bomItems.OrderBy(i => i.category).ThenBy(i => i.partName).ToList();

            Debug.Log($"[BOM] Generated {bomItems.Count} unique items, {TotalParts} total parts");
        }

        public List<BOMItem> GetByCategory(string category)
        {
            return bomItems.Where(i => i.category == category).ToList();
        }

        public List<BOMItem> GetByMaterial(string material)
        {
            return bomItems.Where(i => i.material == material).ToList();
        }

        public string ExportToCSV()
        {
            var csv = "Part ID,Part Name,Part Number,Category,Quantity,Unit Weight (kg),Total Weight (kg),Material,Supplier\n";
            
            foreach (var item in bomItems)
            {
                csv += $"{item.partId},{item.partName},{item.partNumber},{item.category},{item.quantity},{item.unitWeight:F3},{item.totalWeight:F3},{item.material},{item.supplier}\n";
            }

            csv += $"\nTotal Parts:,{TotalParts}\n";
            csv += $"Total Weight:,{TotalWeight:F3} kg\n";

            return csv;
        }

        public string GetSummary()
        {
            return $"Bill of Materials\n" +
                   $"─────────────────\n" +
                   $"Unique Parts: {UniquePartsCount}\n" +
                   $"Total Parts: {TotalParts}\n" +
                   $"Total Weight: {TotalWeight:F2} kg\n";
        }
    }
}
