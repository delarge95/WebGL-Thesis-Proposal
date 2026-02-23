using System;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [Serializable]
    public class ModularSlot
    {
        public string slotName;
        public Transform attachPoint;
        public string[] compatibleTypes;
        public ExplodablePart currentPart;
    }

    public class ModularPartsSystem : Singleton<ModularPartsSystem>
    {
        [Header("Modular Slots")]
        [SerializeField] private List<ModularSlot> slots = new List<ModularSlot>();

        [Header("Available Parts")]
        [SerializeField] private List<GameObject> availableParts = new List<GameObject>();

        [Header("Animation")]
        [SerializeField] private float swapDuration = 0.5f;
        [SerializeField] private float detachDistance = 0.5f;

        public List<ModularSlot> Slots => slots;

        public event Action<ModularSlot, ExplodablePart> OnPartSwapped;
        public event Action<ModularSlot> OnPartRemoved;

        public bool CanAttach(ModularSlot slot, DronePartData partData)
        {
            if (slot == null || partData == null) return false;

            foreach (var compatibleType in slot.compatibleTypes)
            {
                if (partData.PartType == compatibleType)
                {
                    return true;
                }
            }
            return false;
        }

        public void SwapPart(ModularSlot slot, GameObject newPartPrefab)
        {
            if (slot == null || newPartPrefab == null) return;

            StartCoroutine(SwapPartRoutine(slot, newPartPrefab));
        }

        private System.Collections.IEnumerator SwapPartRoutine(ModularSlot slot, GameObject newPartPrefab)
        {
            // Remove current part with animation
            if (slot.currentPart != null)
            {
                yield return DetachPartRoutine(slot.currentPart);
                Destroy(slot.currentPart.gameObject);
                slot.currentPart = null;
            }

            // Spawn new part
            var newPartGO = Instantiate(newPartPrefab, slot.attachPoint.position, slot.attachPoint.rotation);
            newPartGO.transform.SetParent(slot.attachPoint);
            
            var newPart = newPartGO.GetComponent<ExplodablePart>();
            if (newPart == null)
            {
                newPart = newPartGO.AddComponent<ExplodablePart>();
            }

            slot.currentPart = newPart;

            // Attach with animation
            yield return AttachPartRoutine(newPart, slot.attachPoint);

            OnPartSwapped?.Invoke(slot, newPart);

            AudioManager.Instance?.PlayClick();
            NotificationManager.Instance?.ShowNotification($"Installed: {newPartPrefab.name}");

            Debug.Log($"[ModularParts] Swapped part in slot: {slot.slotName}");
        }

        public void RemovePart(ModularSlot slot)
        {
            if (slot == null || slot.currentPart == null) return;

            StartCoroutine(RemovePartRoutine(slot));
        }

        private System.Collections.IEnumerator RemovePartRoutine(ModularSlot slot)
        {
            yield return DetachPartRoutine(slot.currentPart);
            Destroy(slot.currentPart.gameObject);
            slot.currentPart = null;

            OnPartRemoved?.Invoke(slot);

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification($"Removed part from: {slot.slotName}");
            }
        }

        private System.Collections.IEnumerator DetachPartRoutine(ExplodablePart part)
        {
            if (part == null) yield break;

            Vector3 startPos = part.transform.localPosition;
            Vector3 endPos = startPos + Vector3.up * detachDistance;
            float timer = 0f;

            while (timer < swapDuration * 0.5f)
            {
                timer += Time.deltaTime;
                float t = timer / (swapDuration * 0.5f);
                t = t * t; // Ease in
                part.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                
                // Fade out
                var renderer = part.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block);
                    Color color = block.GetColor("_BaseColor");
                    color.a = 1f - t;
                    block.SetColor("_BaseColor", color);
                    renderer.SetPropertyBlock(block);
                }

                yield return null;
            }
        }

        private System.Collections.IEnumerator AttachPartRoutine(ExplodablePart part, Transform attachPoint)
        {
            if (part == null) yield break;

            Vector3 startPos = attachPoint.localPosition + Vector3.up * detachDistance;
            Vector3 endPos = Vector3.zero;
            part.transform.localPosition = startPos;

            float timer = 0f;

            while (timer < swapDuration * 0.5f)
            {
                timer += Time.deltaTime;
                float t = timer / (swapDuration * 0.5f);
                t = 1f - (1f - t) * (1f - t); // Ease out
                part.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

                // Fade in
                var renderer = part.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block);
                    Color color = block.GetColor("_BaseColor");
                    color.a = t;
                    block.SetColor("_BaseColor", color);
                    renderer.SetPropertyBlock(block);
                }

                yield return null;
            }

            part.transform.localPosition = endPos;
        }

        public ModularSlot GetSlotByName(string slotName)
        {
            return slots.Find(s => s.slotName == slotName);
        }

        public List<GameObject> GetCompatibleParts(ModularSlot slot)
        {
            var compatible = new List<GameObject>();

            foreach (var partPrefab in availableParts)
            {
                var part = partPrefab.GetComponent<ExplodablePart>();
                if (part != null && part.Data != null)
                {
                    if (CanAttach(slot, part.Data))
                    {
                        compatible.Add(partPrefab);
                    }
                }
            }

            return compatible;
        }
    }
}
