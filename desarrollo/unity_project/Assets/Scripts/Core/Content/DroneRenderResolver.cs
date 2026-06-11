using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Content
{
    public static class DroneRenderResolver
    {
        public static ExplodablePart ResolveCanonicalPart(Transform rawTransform)
        {
            if (rawTransform == null)
            {
                return null;
            }

            ExplodablePart direct = rawTransform.GetComponentInParent<ExplodablePart>();
            if (direct != null)
            {
                return direct;
            }

            PartRenderCategory category = rawTransform.GetComponent<PartRenderCategory>();
            if (category == null)
            {
                category = rawTransform.GetComponentInParent<PartRenderCategory>();
            }

            if (category == null || string.IsNullOrWhiteSpace(category.CanonicalPartId))
            {
                return null;
            }

            return FindPartById(category.CanonicalPartId);
        }

        public static ExplodablePart FindPartById(string canonicalPartId)
        {
            if (string.IsNullOrWhiteSpace(canonicalPartId))
            {
                return null;
            }

            ExplodablePart[] parts = UnityEngine.Object.FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (ExplodablePart part in parts)
            {
                if (part == null)
                {
                    continue;
                }

                string partId = part.Data != null && !string.IsNullOrWhiteSpace(part.Data.id)
                    ? part.Data.id
                    : part.name;

                if (string.Equals(partId, canonicalPartId, StringComparison.OrdinalIgnoreCase))
                {
                    return part;
                }
            }

            return null;
        }

        public static List<Renderer> CollectManagedRenderers()
        {
            List<Renderer> renderers = new List<Renderer>();
            HashSet<Renderer> seen = new HashSet<Renderer>();

            ExplodablePart[] parts = UnityEngine.Object.FindObjectsByType<ExplodablePart>(FindObjectsSortMode.None);
            foreach (ExplodablePart part in parts)
            {
                if (part == null)
                {
                    continue;
                }

                foreach (Renderer renderer in part.GetComponentsInChildren<Renderer>(true))
                {
                    if (renderer != null &&
                        renderer.transform != null &&
                        !FastenerBuilder.IsFastenerDetailTransform(renderer.transform) &&
                        seen.Add(renderer))
                    {
                        renderers.Add(renderer);
                    }
                }
            }

            PartRenderCategory[] categorized = UnityEngine.Object.FindObjectsByType<PartRenderCategory>(FindObjectsSortMode.None);
            foreach (PartRenderCategory category in categorized)
            {
                if (category == null)
                {
                    continue;
                }

                Renderer renderer = category.GetComponent<Renderer>();
                if (renderer != null &&
                    renderer.transform != null &&
                    !FastenerBuilder.IsFastenerDetailTransform(renderer.transform) &&
                    seen.Add(renderer))
                {
                    renderers.Add(renderer);
                }
            }

            return renderers;
        }

        public static string ResolveThermalSourceId(Renderer renderer, ExplodablePart fallbackPart)
        {
            if (renderer != null)
            {
                PartRenderCategory category = renderer.GetComponent<PartRenderCategory>();
                if (category == null)
                {
                    category = renderer.GetComponentInParent<PartRenderCategory>();
                }

                if (category != null && !string.IsNullOrWhiteSpace(category.ThermalSourcePartId))
                {
                    return category.ThermalSourcePartId;
                }
            }

            if (fallbackPart != null && fallbackPart.Data != null && !string.IsNullOrWhiteSpace(fallbackPart.Data.id))
            {
                return fallbackPart.Data.id;
            }

            return fallbackPart != null ? fallbackPart.name : string.Empty;
        }
    }
}
