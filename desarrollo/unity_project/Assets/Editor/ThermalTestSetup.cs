using UnityEditor;
using UnityEngine;

// Legacy compatibility entry-point. The official workflow lives in
// SetupImportedDroneThermalTest.PrepareImportedDrone.
public class ThermalTestSetup : EditorWindow
{
    [MenuItem("Tools/Thermal/Legacy/Setup MVP Test (Deprecated)")]
    public static void SetupMVPTest()
    {
        Debug.LogWarning("[ThermalTestSetup] Deprecated path. Redirecting to official setup: Tools/Thermal/Prepare Imported Drone For Thermal Test.");
        SetupImportedDroneThermalTest.PrepareImportedDrone();
    }
}