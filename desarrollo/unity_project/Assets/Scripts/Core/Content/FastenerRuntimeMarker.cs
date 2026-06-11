using UnityEngine;

namespace WebGL.Core.Content
{
    [DisallowMultipleComponent]
    public class FastenerRuntimeMarker : MonoBehaviour
    {
        [SerializeField] private string fastenerFamilyId;
        [SerializeField] private string fastenerInstanceId;
        [SerializeField] private string sceneTypeKey;
        [SerializeField] private string parentCanonicalPartId;
        [SerializeField] private bool isInspectable = true;
        [SerializeField] private bool sourceIsPrimitiveFastener = true;
        [SerializeField] private string fallbackReason = string.Empty;

        public string FastenerFamilyId => fastenerFamilyId;
        public string FastenerInstanceId => fastenerInstanceId;
        public string SceneTypeKey => sceneTypeKey;
        public string ParentCanonicalPartId => parentCanonicalPartId;
        public bool IsInspectable => isInspectable;
        public bool SourceIsPrimitiveFastener => sourceIsPrimitiveFastener;
        public string FallbackReason => fallbackReason;

        public void Configure(
            string familyId,
            string instanceId,
            string typeKey,
            string parentId,
            bool inspectable,
            string fallback,
            bool primitiveSource = true)
        {
            fastenerFamilyId = familyId ?? string.Empty;
            fastenerInstanceId = instanceId ?? string.Empty;
            sceneTypeKey = typeKey ?? string.Empty;
            parentCanonicalPartId = parentId ?? string.Empty;
            isInspectable = inspectable;
            sourceIsPrimitiveFastener = primitiveSource;
            fallbackReason = fallback ?? string.Empty;
        }
    }
}
