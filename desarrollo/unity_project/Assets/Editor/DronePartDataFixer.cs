using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WebGL.Core.Data;
using System.IO;

namespace WebGL.Editor
{
    public class DronePartDataFixer : EditorWindow
    {
        [MenuItem("WebGL Tesis/Legacy/Fix DronePartData Categories")]
        public static void FixCategories()
        {
            string[] guids = AssetDatabase.FindAssets("t:DronePartData");
            int fixedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DronePartData data = AssetDatabase.LoadAssetAtPath<DronePartData>(path);
                
                if (data != null)
                {
                    PartCategory oldCat = data.category;
                    data.category = InferCategoryFromTypeOrName(data);

                    if (oldCat != data.category)
                    {
                        EditorUtility.SetDirty(data);
                        fixedCount++;
                    }
                    
                    // Ensure only canonical parent nodes have HotspotTarget enabled for macro UI.
                    bool shouldBeHotspot = IsMacroAssembly(data.id);
                    if (data.isHotspotTarget != shouldBeHotspot)
                    {
                        data.isHotspotTarget = shouldBeHotspot;
                        EditorUtility.SetDirty(data);
                    }

                    // Dummy subcomponents for specific nodes to test the new logic
                    if (shouldBeHotspot && string.IsNullOrEmpty(data.id) == false)
                    {
                        string[] subs = GetKnownSubcomponents(data.id);
                        if (subs != null && (data.subComponentNames == null || data.subComponentNames.Length != subs.Length))
                        {
                            data.subComponentNames = subs;
                            EditorUtility.SetDirty(data);
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[DronePartDataFixer] Success! {fixedCount} assets updated according to the 6-tier taxonomy.");
        }

        [MenuItem("WebGL Tesis/Legacy/Create Missing Mock Parts")]
        public static void CreateMissingMockPartsMenu()
        {
            CreateMissingMocks();
            AssetDatabase.SaveAssets();
            Debug.Log("[DronePartDataFixer] Mock parts creados/actualizados en Assets/Data/Parts.");
        }

        private static PartCategory InferCategoryFromTypeOrName(DronePartData data)
        {
            string type = data.partType != null ? data.partType.ToLower() : "";
            string name = data.partName != null ? data.partName.ToLower() : "";
            string id = data.id != null ? data.id.ToLower() : "";

            if (type.Contains("flight") || type.Contains("imu") || type.Contains("avionics") || id.Contains("pixhawk")) return PartCategory.Avionics;
            if (type.Contains("gps") || type.Contains("receiver") || type.Contains("radio") || id.Contains("telemetry")) return PartCategory.SensorsComms;
            if (type.Contains("batter") || type.Contains("power") || type.Contains("pdb") || type.Contains("pm06")) return PartCategory.PowerDistribution;
            if (type.Contains("motor") || type.Contains("prop") || type.Contains("esc") || type.Contains("propulsion")) return PartCategory.PropulsionSystem;
            if (type.Contains("frame") || type.Contains("body") || type.Contains("arm") || type.Contains("landing") || type.Contains("carbon") || type.Contains("plate")) return PartCategory.SkeletonAirframe;
            if (type.Contains("screw") || type.Contains("nut") || type.Contains("fastener") || type.Contains("clip") || type.Contains("spacer") || name.StartsWith("m3") || name.StartsWith("gb70")) return PartCategory.Fasteners;

            return PartCategory.Uncategorized;
        }

        // Determina si una pieza representa un conjunto mayor para aislar visualmente el Hotspot
        private static bool IsMacroAssembly(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            
            string lowerCmd = id.ToLower();
            string[] canonicalMacroParents = new string[] 
            {
                "x500v2_pixhawk6c",
                "x500v2_gps_m10",
                "x500v2_telemetry_radio",
                "x500v2_battery",
                "x500v2_power_module",
                "x500v2_motor_fl", "x500v2_motor_fr", "x500v2_motor_bl", "x500v2_motor_br",
                "x500v2_esc_fl", "x500v2_esc_fr", "x500v2_esc_bl", "x500v2_esc_br"
            };

            foreach (var macro in canonicalMacroParents)
            {
                if (lowerCmd == macro) return true;
            }
            return false;
        }

        private static void CreateMissingMocks()
        {
            string dir = "Assets/Data/Parts";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string[] toCreate = new string[] {
                "x500v2_pdb", 
                "x500v2_esc_FL", "x500v2_esc_FR", "x500v2_esc_BL", "x500v2_esc_BR"
            };

            foreach (var partId in toCreate)
            {
                string path = $"{dir}/{partId}.asset";
                if (!File.Exists(path))
                {
                    DronePartData newPart = ScriptableObject.CreateInstance<DronePartData>();
                    newPart.id = partId;
                    newPart.partName = partId.Replace("x500v2_", "").Replace("_", " ").ToUpper();
                    
                    if (partId == "x500v2_pdb")
                    {
                        newPart.partType = "PDB";
                        newPart.category = PartCategory.PowerDistribution;
                        newPart.description = "Power Distribution Board for x500 v2";
                    }
                    else
                    {
                        newPart.partType = "ESC";
                        newPart.category = PartCategory.PropulsionSystem;
                        newPart.description = "20A Electronic Speed Controller";
                        newPart.isHotspotTarget = true;
                    }

                    AssetDatabase.CreateAsset(newPart, path);
                    Debug.Log($"Created missing part data: {partId}");
                }
            }
        }

        private static string[] GetKnownSubcomponents(string id)
        {
            id = id.ToLower();
            if (id == "x500v2_pixhawk6c") return new[] { "Pixhawk 6C Top Cover", "Pixels 6C Base Shell", "IMU Sensor Board", "Main PCB", "JST GH 6-Pin Connector" };
            if (id == "x500v2_gps_m10") return new[] { "Holybro M10 GPS Antenna", "Folding Mast Joint", "Mast Base Bracket", "Securing Nut", "Top Mounting Tray" };
            if (id == "x500v2_power_module") return new[] { "Power Module PM02/PM06 Board", "XT60 Male Connector Plug", "XT60 Holder Panel" };
            if (id == "x500v2_battery") return new[] { "Lithium Polymer 4S Battery", "Battery Strap 1", "Battery Strap 2" };
            if (id.Contains("x500v2_motor_")) return new[] { "Holybro 2216 KV920 Motor Unit", "Propeller Mount Set", "Mount Base Plate" };
            if (id == "x500v2_landing_gear") return new[] { "16mm Carbon Tubes (x2)", "Landing Skid End Caps (x4)", "EVA Foam Pads (x4)", "T-Connectors (x4)" };
            if (id == "x500v2_rails_battery") return new[] { "Battery Mounting Board", "Battery Silicone Pad", "Payload 10mm Carbon Rails", "Payload Rail Clip Holders" };
            return null;
        }
    }
}
