namespace WebGL.Core.Events
{
    public struct ThermalLoadChangedEvent
    {
        public float NormalizedLoad;

        public ThermalLoadChangedEvent(float normalizedLoad)
        {
            NormalizedLoad = normalizedLoad;
        }
    }

    public struct ThermalSimulationReadyEvent
    {
        public bool IsReady;
        public int NodeCount;

        public ThermalSimulationReadyEvent(bool isReady, int nodeCount)
        {
            IsReady = isReady;
            NodeCount = nodeCount;
        }
    }

    public struct ThermalSimulationResetEvent
    {
        public float AmbientTemperatureC;

        public ThermalSimulationResetEvent(float ambientTemperatureC)
        {
            AmbientTemperatureC = ambientTemperatureC;
        }
    }

    public struct ImportedDroneRuntimeBoundEvent
    {
        public string RootName;
        public int PropellerCount;
        public int PartCount;

        public ImportedDroneRuntimeBoundEvent(string rootName, int propellerCount, int partCount)
        {
            RootName = rootName;
            PropellerCount = propellerCount;
            PartCount = partCount;
        }
    }
}
