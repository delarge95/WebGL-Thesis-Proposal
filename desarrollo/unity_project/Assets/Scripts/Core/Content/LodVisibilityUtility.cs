using UnityEngine;

namespace WebGL.Core.Content
{
    public static class LodVisibilityUtility
    {
        public static void ApplyRendererEnabled(Renderer renderer, bool visible)
        {
            if (renderer == null)
            {
                return;
            }

            LODGroup managedGroup = FindManagedGroup(renderer);
            if (managedGroup != null)
            {
                managedGroup.enabled = visible;
            }

            renderer.enabled = visible;
        }

        public static bool IsGeneratedLodRenderer(Renderer renderer)
        {
            return renderer != null && renderer.GetComponent<LodGeneratedRenderer>() != null;
        }

        public static bool BelongsToManagedLodGroup(Renderer renderer)
        {
            return FindManagedGroup(renderer) != null;
        }

        private static LODGroup FindManagedGroup(Renderer renderer)
        {
            if (renderer == null)
            {
                return null;
            }

            LODGroup group = renderer.GetComponentInParent<LODGroup>();
            if (group == null)
            {
                return null;
            }

            return group.GetComponentInChildren<LodGeneratedRenderer>(true) != null ? group : null;
        }
    }
}
