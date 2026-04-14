using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Content;
using WebGL.Core.Data;
using WebGL.Core.Events;
using WebGL.Core.Utils;

namespace WebGL.Core.Managers
{
    [DisallowMultipleComponent]
    public class FastenerInspectionManager : Singleton<FastenerInspectionManager>
    {
        private readonly List<Renderer> hiddenProxyRenderers = new List<Renderer>();

        private Transform currentProxyRoot;
        private GameObject currentDetailRoot;

        private void OnEnable()
        {
            EventBus.Subscribe<PartSelectedEvent>(HandlePartSelected);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PartSelectedEvent>(HandlePartSelected);
            ClearCurrentInspection();
        }

        private void HandlePartSelected(PartSelectedEvent evt)
        {
            if (evt.PartData == null || evt.PartData.category != PartCategory.Fasteners)
            {
                ClearCurrentInspection();
                return;
            }

            Transform selection = SelectionManager.Instance != null ? SelectionManager.Instance.CurrentSelection : null;
            if (selection == null)
            {
                ClearCurrentInspection();
                return;
            }

            FastenerRegistry registry = FastenerRegistry.Instance;
            FastenerMetadata metadata = registry != null
                ? registry.ResolveMetadata(selection, evt.PartData)
                : evt.PartData.fastenerMetadata;

            if (metadata == null || !metadata.isInspectable)
            {
                ClearCurrentInspection();
                return;
            }

            ShowInspection(selection, metadata);
        }

        private void ShowInspection(Transform proxyRoot, FastenerMetadata metadata)
        {
            if (proxyRoot == null || metadata == null)
            {
                ClearCurrentInspection();
                return;
            }

            if (currentProxyRoot == proxyRoot && currentDetailRoot != null)
            {
                return;
            }

            ClearCurrentInspection();

            Renderer[] proxyRenderers = proxyRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < proxyRenderers.Length; i++)
            {
                Renderer renderer = proxyRenderers[i];
                if (renderer == null || renderer.transform == null || renderer.transform.name == "__fastener_detail")
                {
                    continue;
                }

                renderer.enabled = false;
                hiddenProxyRenderers.Add(renderer);
            }

            currentProxyRoot = proxyRoot;
            currentDetailRoot = FastenerBuilder.BuildDetailVisual(proxyRoot, metadata);
            RefreshHighlightState(currentProxyRoot);
        }

        private void ClearCurrentInspection()
        {
            Transform previousProxyRoot = currentProxyRoot;

            for (int i = 0; i < hiddenProxyRenderers.Count; i++)
            {
                Renderer renderer = hiddenProxyRenderers[i];
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }

            hiddenProxyRenderers.Clear();
            currentProxyRoot = null;

            if (currentDetailRoot != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(currentDetailRoot);
                }
                else
                {
                    DestroyImmediate(currentDetailRoot);
                }
            }

            currentDetailRoot = null;
            RefreshHighlightState(previousProxyRoot);
        }

        private static void RefreshHighlightState(Transform proxyRoot)
        {
            if (proxyRoot == null)
            {
                return;
            }

            HighlightSystem highlight = proxyRoot.GetComponent<HighlightSystem>();
            if (highlight != null)
            {
                highlight.RefreshVisualTargets();
                return;
            }

            MaterialController controller = proxyRoot.GetComponent<MaterialController>();
            controller?.RefreshRenderers();
        }
    }
}
