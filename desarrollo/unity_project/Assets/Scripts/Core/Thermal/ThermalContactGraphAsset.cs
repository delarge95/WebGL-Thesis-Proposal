using System;
using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Thermal
{
    [CreateAssetMenu(fileName = "ThermalContactGraph", menuName = "WebGL/Thermal Contact Graph")]
    public class ThermalContactGraphAsset : ScriptableObject
    {
        public ThermalContactGraphBuildInfo buildInfo = new ThermalContactGraphBuildInfo();
        public List<string> nodeIds = new List<string>();
        public List<ThermalContactLinkData> links = new List<ThermalContactLinkData>();
    }

    [Serializable]
    public class ThermalContactGraphBuildInfo
    {
        public string sourceSceneName;
        public string sourceRootName;
        public string generatedUtc;
        [Min(0.1f)] public float maxGapMm = 12f;
        [Min(0f)] public float minContactAreaCm2 = 0.5f;
        [Min(0.1f)] public float minPathLengthMm = 1f;
        public bool generatedFromBounds = true;
    }

    [Serializable]
    public class ThermalContactLinkData
    {
        public string fromPartId;
        public string toPartId;

        [Min(0.1f)] public float contactAreaCm2 = 1f;
        [Min(0.1f)] public float pathLengthMm = 5f;
        [Range(0f, 2f)] public float conductionScale = 1f;
        public string notes;
    }
}
