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
    public class ChecklistItem
    {
        public string itemId;
        public string partName;
        public string partType;
        public int requiredQuantity;
        public int verifiedQuantity;
        public bool isVerified;
        public string notes;
        public DateTime verifiedAt;
    }

    public class AssemblyChecklist : Singleton<AssemblyChecklist>
    {
        private List<ChecklistItem> checklistItems = new List<ChecklistItem>();
        
        public List<ChecklistItem> Items => checklistItems;
        public int TotalItems => checklistItems.Count;
        public int VerifiedCount => checklistItems.Count(i => i.isVerified);
        public float ProgressPercent => TotalItems > 0 ? (float)VerifiedCount / TotalItems * 100f : 0f;
        public bool IsComplete => VerifiedCount == TotalItems;

        public event Action<ChecklistItem> OnItemVerified;
        public event Action OnChecklistComplete;

        private void Start()
        {
            GenerateChecklist();
        }

        public void GenerateChecklist()
        {
            checklistItems.Clear();

            var parts = FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            var partGroups = parts
                .Where(p => p.Data != null)
                .GroupBy(p => p.Data.id);

            foreach (var group in partGroups)
            {
                var first = group.First();
                var data = first.Data;

                var item = new ChecklistItem
                {
                    itemId = data.id,
                    partName = data.partName,
                    partType = data.partType,
                    requiredQuantity = group.Count(),
                    verifiedQuantity = 0,
                    isVerified = false,
                    notes = ""
                };

                checklistItems.Add(item);
            }

            checklistItems = checklistItems.OrderBy(i => i.partType).ThenBy(i => i.partName).ToList();

            Debug.Log($"[Checklist] Generated {checklistItems.Count} items");
        }

        public void VerifyItem(string itemId, int quantity = -1)
        {
            var item = checklistItems.Find(i => i.itemId == itemId);
            if (item == null) return;

            if (quantity < 0)
            {
                // Full verification
                item.verifiedQuantity = item.requiredQuantity;
            }
            else
            {
                item.verifiedQuantity = Mathf.Min(quantity, item.requiredQuantity);
            }

            item.isVerified = item.verifiedQuantity >= item.requiredQuantity;
            item.verifiedAt = DateTime.Now;

            OnItemVerified?.Invoke(item);

            AudioManager.Instance?.PlaySuccess();

            // Check if all complete
            if (IsComplete)
            {
                OnChecklistComplete?.Invoke();
                NotificationManager.Instance?.ShowNotification("✅ All parts verified!");
            }

            Debug.Log($"[Checklist] Verified: {item.partName} ({item.verifiedQuantity}/{item.requiredQuantity})");
        }

        public void UnverifyItem(string itemId)
        {
            var item = checklistItems.Find(i => i.itemId == itemId);
            if (item == null) return;

            item.verifiedQuantity = 0;
            item.isVerified = false;
        }

        public void ToggleItem(string itemId)
        {
            var item = checklistItems.Find(i => i.itemId == itemId);
            if (item == null) return;

            if (item.isVerified)
            {
                UnverifyItem(itemId);
            }
            else
            {
                VerifyItem(itemId);
            }
        }

        public void AddNote(string itemId, string note)
        {
            var item = checklistItems.Find(i => i.itemId == itemId);
            if (item != null)
            {
                item.notes = note;
            }
        }

        public void ResetChecklist()
        {
            foreach (var item in checklistItems)
            {
                item.verifiedQuantity = 0;
                item.isVerified = false;
                item.notes = "";
            }

            NotificationManager.Instance?.ShowNotification("Checklist reset");
        }

        public List<ChecklistItem> GetPendingItems()
        {
            return checklistItems.Where(i => !i.isVerified).ToList();
        }

        public List<ChecklistItem> GetVerifiedItems()
        {
            return checklistItems.Where(i => i.isVerified).ToList();
        }

        public string GetSummary()
        {
            return $"Assembly Checklist\n" +
                   $"──────────────────\n" +
                   $"Progress: {ProgressPercent:F0}%\n" +
                   $"Verified: {VerifiedCount}/{TotalItems}\n" +
                   $"Pending: {TotalItems - VerifiedCount}\n";
        }
    }
}
