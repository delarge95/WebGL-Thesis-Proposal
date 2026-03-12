import json
import os

import bpy
import json
import os
import re

# 1. 6-Tier Functional Assembly Mapping
cat_mapping = {
    "Avionics (Brain)": ["DIKE-PIXHAWK6C", "IMU-PIXHAWK6C", "MIANKE-PIXHAWK6C", "PCB-PIXHAWK6C"],
    "Sensors & Comms": ["GAN-GPSV5", "RC_Receiver", "Telemetry_Radio", "GAI-GUANGLIU"],
    "03_Power_Distribution": ["PCB-PM06", "BATTERY-MOUNTING", "BATTERY-PAD", "TOU-XT60", "X500-TAO-XT60", "BATTERY_STRAP", "BATTERY_LIPO", "BM06B-WO"],
    "Propulsion System": ["DJ-2216", "Propeller"],
    "Skeleton / Airframe": [
        "BOTTOM-PLATE", "TOP-PLATE", "PLATFORM-PLAT", "PYLONS",
        "CARBON-FIBER-TUBE", "HMX5V-DIGAI", "HMX5V-ZUO", "HMX5V-JIBI", "HMX5V-GUAN",
        "MAO-JIAO", "JIAO-EVA", "JIAO-LIANJIE", "GUAN-CHENG", "ZHIJIA-CAMERA",
        "GPS-ZHIJIA-ZHUANJIETOU", "GPS-ZHIJIA-ZUO", "GPSV5-ZHIJIA-LUOMAO", "GPSV5-ZHIJIA-TUOPAN",
        "BAN-DJ-DIAN"
    ],
    "Fasteners (Hardware)": ["GB70", "LM-M3", "ZSLM", "M3-", "M25-", "NILONGZHU", "HUAN-GUIJIAO", "JIA-GUAN", "JIA-LIANJIE"]
}

def determine_cat(obj_name):
    upper_name = obj_name.upper()
    for cat, prefixes in cat_mapping.items():
        for prefix in prefixes:
            if prefix.upper() in upper_name:
                return cat
    # Fallback heuristics
    if "M3" in upper_name or "M25" in upper_name or "SCREW" in upper_name:
        return "Fasteners (Hardware)"
    return "Skeleton / Airframe"

blender_inventory = {}
valid_collections = ["01_Avionics", "02_Sensors_Comms", "03_Power_Distribution", "04_Propulsion_System", "05_Skeleton_Airframe", "06_Fasteners"]

for cname in valid_collections:
    col = bpy.data.collections.get(cname)
    if not col: continue
    for obj in col.objects:
        if obj.type == 'MESH':
            if "_BACKUP" in obj.name or "LOD" in obj.name:
                continue
                
            b_name = re.sub(r'\.\d{3}$', '', obj.name) # Remove .001 trailing
            cat = determine_cat(b_name)
            
            if cat not in blender_inventory:
                blender_inventory[cat] = {}
                
            if b_name not in blender_inventory[cat]:
                blender_inventory[cat][b_name] = 0
                
            blender_inventory[cat][b_name] += 1


# Mapping exact real specs including thermal (Hover T, Peak T), heating rate (warmup index), and Hotspots
specs_mapping = {
    # Electronics
    "PCB-PM06": {"name": "Power Module PM02/PM06 Board", "func": "Distribuye 5V al Pixhawk y sensa V/I", "mat": "PCB FR4", "wt": "0.020", "dim": "55×17 mm", "tBase": 35, "tMax": 55, "hRate": 3.0, "hotspot": False, "hLabel": ""},
    "DIKE-PIXHAWK6C": {"name": "Pixhawk 6C Top Cover", "func": "Carcasa superior del controlador", "mat": "Aluminio CNC", "wt": "0.015", "dim": "85×44 mm", "tBase": 42, "tMax": 60, "hRate": 6.0, "hotspot": True, "hLabel": "Flight Controller"},
    "IMU-PIXHAWK6C": {"name": "Pixhawk 6C IMU Module", "func": "Sensores inerciales ICM-42688", "mat": "PCB montada en silicona", "wt": "0.005", "dim": "Interno", "tBase": 42, "tMax": 60, "hRate": 6.0, "hotspot": False, "hLabel": ""},
    "MIANKE-PIXHAWK6C": {"name": "Pixhawk 6C Base Shell", "func": "Carcasa inferior del controlador", "mat": "Aluminio / Plástico", "wt": "0.020", "dim": "85×44 mm", "tBase": 42, "tMax": 60, "hRate": 6.0, "hotspot": False, "hLabel": ""},
    "PCB-PIXHAWK6C": {"name": "Pixhawk 6C Main PCB", "func": "Circuito integrado STM32H743", "mat": "PCB FR4", "wt": "0.010", "dim": "Interno", "tBase": 45, "tMax": 65, "hRate": 5.0, "hotspot": False, "hLabel": ""},
    "GAN-GPSV5": {"name": "Holybro M10 GPS Antenna", "func": "Posicionamiento GNSS Multiconstelación", "mat": "Cerámica y PCB", "wt": "0.032", "dim": "⌀50×14 mm", "tBase": 28, "tMax": 40, "hRate": 2.0, "hotspot": True, "hLabel": "GNSS & Compass"},
    "BM06B-WO": {"name": "JST GH 6-Pin Connector", "func": "Puerto telemétrico interno/externo", "mat": "Plástico/Cobre", "wt": "0.001", "dim": "Miniatura", "tBase": 25, "tMax": 30, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    
    # Propulsion
    "DJ-2216-KV880": {"name": "Holybro 2216 KV880 Motor", "func": "Generación del empuje aerodinámico", "mat": "Aleación de Aluminio / Acero", "wt": "0.063", "dim": "⌀28×32 mm", "tBase": 55, "tMax": 95, "hRate": 10.0, "hotspot": True, "hLabel": "Propulsion System"},
    "BAN-DJ-DIAN-F2": {"name": "Motor Mount Base Plate", "func": "Montura disipadora inferior p/Motor", "mat": "Aluminio/Fibra", "wt": "0.005", "dim": "16×19 mm base", "tBase": 40, "tMax": 70, "hRate": 8.0, "hotspot": False, "hLabel": ""},
    
    # Structure (Battery specifically gets hot)
    "BATTERY-MOUNTING-PLAT": {"name": "Battery Mounting Board", "func": "Placa ajustable p/correas de batería", "mat": "Fibra / Plástico", "wt": "0.025", "dim": "Variable", "tBase": 35, "tMax": 55, "hRate": 15.0, "hotspot": True, "hLabel": "Battery (LiPo 4S)"},
    "BATTERY-PAD": {"name": "Battery Silicone Pad", "func": "Superficie antideslizante para batería 4S", "mat": "Silicona adherente", "wt": "0.008", "dim": "Universal", "tBase": 30, "tMax": 45, "hRate": 10.0, "hotspot": False, "hLabel": ""},
    "TOU-XT60": {"name": "XT60 Male Connector Plug", "func": "Unión eléctrica de potencia", "mat": "Nylon / Oro", "wt": "0.005", "dim": "Estándar", "tBase": 40, "tMax": 60, "hRate": 5.0, "hotspot": False, "hLabel": ""},
    "X500-TAO-XT60": {"name": "XT60 Panel / Holder", "func": "Soporte cautivo del conector XT60", "mat": "Inyección plástico", "wt": "0.003", "dim": "Estándar", "tBase": 35, "tMax": 45, "hRate": 3.0, "hotspot": False, "hLabel": ""},
    
# Generic Structure (Cold)
    "ZHIJIA-CAMERA": {"name": "Intel Realsense Camera Bracket", "func": "Montura frontal para sistema estéreo", "mat": "Nylon Reforzado", "wt": "0.015", "dim": "A medida", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GPS-ZHIJIA-ZHUANJIETOU": {"name": "GPS Folding Mast Joint", "func": "Articulación del mástil abatible del GPS", "mat": "Nylon Reforzado", "wt": "0.005", "dim": "Mástil M3", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GPS-ZHIJIA-ZUO": {"name": "GPS Mast Base Bracket", "func": "Soporte atornillable del mástil del GPS", "mat": "Nylon Reforzado", "wt": "0.005", "dim": "Mástil M3", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GPSV5-ZHIJIA-LUOMAO": {"name": "GPS Mast Securing Nut", "func": "Tuerca o pieza enroscable del mástil", "mat": "Nylon Reforzado", "wt": "0.002", "dim": "Manual", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GPSV5-ZHIJIA-TUOPAN": {"name": "GPS Top Mounting Tray", "func": "Bandeja circular superior del GPS", "mat": "Plástico ABS", "wt": "0.005", "dim": "⌀50 mm", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    
    "BOTTOM-PLATE": {"name": "Carbon Fiber Bottom Plate", "func": "Interfase de tren, brazos y PDB", "mat": "Fibra Carbono Twill 2mm", "wt": "0.075", "dim": "144×144 mm", "tBase": 23, "tMax": 28, "hRate": 1.0, "hotspot": False, "hLabel": ""},
    "TOP-PLATE": {"name": "Carbon Fiber Top Plate", "func": "Interfase de vuelo, Pixhawk, Radio", "mat": "Fibra Carbono Twill 2mm", "wt": "0.075", "dim": "144×144 mm", "tBase": 23, "tMax": 28, "hRate": 1.0, "hotspot": False, "hLabel": ""},
    "PLATFORM-PLAT": {"name": "Upper Platform Board", "func": "Montura elevada del GPS M10", "mat": "Fibra de Carbono", "wt": "0.015", "dim": "80×80 mm", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "PYLONS": {"name": "Payload 10mm Carbon Rails", "func": "Riel inferior porta-elementos 10mm", "mat": "Fibra de carbono", "wt": "0.015", "dim": "⌀10×250 mm", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "CARBON-FIBER-TUBE300": {"name": "Arm 16mm Carbon Tube (Long)", "func": "Brazo extendido diagonal de empuje", "mat": "Fibra de carbono", "wt": "0.030", "dim": "⌀16×300 mm", "tBase": 22, "tMax": 27, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "CARBON-FIBER-TUBE": {"name": "Landing Gear 16mm Carbon Tube", "func": "Patín o pata vertical principal", "mat": "Fibra de carbono", "wt": "0.020", "dim": "⌀16 mm", "tBase": 22, "tMax": 24, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    
    "HMX5V-DIGAI-DIAN": {"name": "Arm Tube Motor Mount (Bottom)", "func": "Soporte plástico del motor a tubo", "mat": "Nylon", "wt": "0.012", "dim": "P/ Tubo 16mm", "tBase": 25, "tMax": 35, "hRate": 2.0, "hotspot": False, "hLabel": ""},
    "HMX5V-ZUO-DJ": {"name": "Arm Tube Motor Mount (Top Hub)", "func": "Abrazadera superior del motor", "mat": "Nylon", "wt": "0.008", "dim": "P/ Tubo 16mm", "tBase": 25, "tMax": 35, "hRate": 2.0, "hotspot": False, "hLabel": ""},
    "HMX5V-JIBI-JIA": {"name": "Tube Frame Clamp Connector", "func": "Abrazadera brazo a chasis", "mat": "Nylon", "wt": "0.015", "dim": "P/ Tubo 16mm", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "HMX5V-GUAN-DINGWEI": {"name": "Tube Positioning Stopper", "func": "Tope interior para la abrazadera central", "mat": "Nylon", "wt": "0.002", "dim": "Interno 16mm", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    
    "MAO-JIAO": {"name": "Landing Gear T-Connector", "func": "Unión T del tren", "mat": "Plástico Reforzado", "wt": "0.008", "dim": "16 a 10 mm", "tBase": 22, "tMax": 24, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "JIAO-EVA": {"name": "Landing Gear Foam Pad", "func": "Almohadilla amortiguadora inferior", "mat": "Espuma EVA", "wt": "0.002", "dim": "⌀10 Ext", "tBase": 22, "tMax": 24, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "JIAO-LIANJIE": {"name": "Landing Skid End Cap", "func": "Tapón de la cruz inferior", "mat": "Nylon Protector", "wt": "0.005", "dim": "⌀10 mm", "tBase": 22, "tMax": 24, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GUAN-CHENG": {"name": "Payload Rail Clip Holder", "func": "Soporte colgante de tubos 10mm", "mat": "Plástico", "wt": "0.005", "dim": "P/ Tubo 10mm", "tBase": 22, "tMax": 24, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "HUAN-GUIJIAO": {"name": "Silicone Mount Dampener", "func": "Aislante anti-vibración para hardware", "mat": "Silicona", "wt": "0.001", "dim": "M3-M4", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "JIA-GUAN": {"name": "Cable Routing Clamp", "func": "Sujeción de cables agrupados", "mat": "Abrazadera", "wt": "0.001", "dim": "Miniatura", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "JIA-LIANJIE": {"name": "Miscellaneous Frame Clip", "func": "Clip conectivo estructural", "mat": "Plástico", "wt": "0.002", "dim": "Miniatura", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "GAI-GUANGLIU": {"name": "Optical Flow Bottom Cover", "func": "Cubierta protectora de sensor", "mat": "ABS", "wt": "0.003", "dim": "Pequeño", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    
    # New Proxies (Propellers, Receiver, SiK Radio, Straps)
    "PROPELLER_1045": {"name": "10-inch Propeller", "func": "Sustentación aerodinámica", "mat": "Nylon / Carbon Mix", "wt": "0.015", "dim": "10x4.5 inch", "tBase": 25, "tMax": 30, "hRate": 1.0, "hotspot": False, "hLabel": ""},
    "RC_RECEIVER": {"name": "RC Receiver Crossfire/FrSky", "func": "Recepción de mandos RC 2.4/900MHz", "mat": "PCB / Cinta Retráctil", "wt": "0.005", "dim": "30x15 mm", "tBase": 30, "tMax": 45, "hRate": 2.0, "hotspot": False, "hLabel": ""},
    "TELEMETRY_RADIO": {"name": "Telemetry Radio SiK", "func": "Enlace de tierra MAVLink", "mat": "PCB / Plástico", "wt": "0.015", "dim": "45x20 mm", "tBase": 38, "tMax": 55, "hRate": 4.0, "hotspot": False, "hLabel": ""},
    "BATTERY_STRAP": {"name": "Battery Velcro Strap", "func": "Sujeción de Batería Principal", "mat": "Nylon Velcro / Kevlar", "wt": "0.005", "dim": "20x200 mm", "tBase": 25, "tMax": 35, "hRate": 0.0, "hotspot": False, "hLabel": ""},
    "BATTERY_LIPO": {"name": "LiPo 4S 5000mAh Battery", "func": "Principal fuente de energía", "mat": "Litio / Polímero", "wt": "0.450", "dim": "135x45x35 mm", "tBase": 35, "tMax": 65, "hRate": 25.0, "hotspot": True, "hLabel": "Main Power Supply"},
    
    # Fasteners Mapping
    "GB70": {"name": "Socket Head Cap Screw", "func": "Fijación y apriete del chasis", "mat": "Acero Inox", "wt": "0.001", "dim": "Milimétrico", "tBase": 22, "tMax": 26, "hRate": 1.0, "hotspot": False, "hLabel": ""},
    "LM-M3-DING": {"name": "M3 Dome/Cap Nut", "func": "Tuerca ciega", "mat": "Latón", "wt": "0.001", "dim": "M3", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "LM-M3-NILONG": {"name": "M3 Nyloc Nut", "func": "Tuerca autoblocante", "mat": "Acero/Nylon", "wt": "0.001", "dim": "M3", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "ZSLM": {"name": "Self-Locking Flange Nut", "func": "Tuerca con brida", "mat": "Acero", "wt": "0.001", "dim": "M3/M2.5", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "M3-": {"name": "M3 Pan/Countersunk Screw", "func": "Tornillería rasante", "mat": "Acero Inox", "wt": "0.001", "dim": "M3", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "M25-": {"name": "M2.5 Micro Screw", "func": "Tornillería instrumento", "mat": "Acero", "wt": "0.001", "dim": "M2.5", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""},
    "NILONGZHU": {"name": "Nylon Hex Spacer", "func": "Espaciador aislante", "mat": "Nylon", "wt": "0.001", "dim": "M3/M2.5", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""}
}

def get_specs(ble_name):
    ble_up = ble_name.upper()
    for key, data in specs_mapping.items():
        if key.upper() in ble_up:
            return data
    # Fallback to smart parsing
    if "M3" in ble_up or "M25" in ble_up:
        return {"name": f"Fastener Hardware ({ble_name})", "func": "Sujeción milimétrica", "mat": "Acero", "wt": "0.001", "dim": "Milimetro var", "tBase": 22, "tMax": 25, "hRate": 0.5, "hotspot": False, "hLabel": ""}
    return {"name": ble_name, "func": "Componente estructural general", "mat": "Mixto", "wt": "0.010", "dim": "A medida", "tBase": 22, "tMax": 25, "hRate": 0.0, "hotspot": False, "hLabel": ""}

json_array = []
md_lines = []
md_lines.append("# Inventario Físico y Térmico del Gemelo Digital Holybro X500 V2")
md_lines.append("")
md_lines.append("Esta tabla representa el listado exacto de las **247 piezas de malla** presentes en Blender, categorizadas bajo el esquema ingenieril e incluyendo **física térmica para la simulación WebGL** (Temp Base, Temp Máx, e Índice de Calentamiento) y la definición de **UI Hotspots** interactivos.")
md_lines.append("")
md_lines.append("| Categoría | Nombre en Blender | Nombre Real Exacto | Material | Peso (Kg) | T Base (°C) | T Máx (°C) | Ind. Calor | UI Hotspot | Cant. |")
md_lines.append("|-----------|-------------------|--------------------|----------|-----------|-------------|------------|------------|------------|-------|")

total_pieces = 0

for cat in sorted(list(blender_inventory.keys())):
    items = blender_inventory[cat]
    for b_name in sorted(list(items.keys())):
        count = items[b_name]
        total_pieces += count
        specs = get_specs(b_name)
        
        # Determine hotspot string for MD
        hotspot_str = f"Sí ({specs['hLabel']})" if specs['hotspot'] else "No"
        
        # MD Table row
        md_lines.append(f"| **{cat}** | `{b_name}` | {specs['name']} | {specs['mat']} | {specs['wt']} | {specs['tBase']} | {specs['tMax']} | {specs['hRate']} | {hotspot_str} | **{count}** |")
        
        # JSON Output Element
        new_item = {
            "partName": specs["name"],
            "id": f"x500v2_blend_{b_name.replace('-', '_').lower()}",
            "blenderName": b_name,
            "category": cat,
            "description": specs["func"] + ". Pieza modelada en base al CAD " + b_name + ".",
            "function": specs["func"],
            "weightKg": float(specs["wt"]) if specs["wt"] != "N/D" else 0.0,
            "dimensions": specs["dim"],
            "materialType": specs["mat"],
            "manufacturer": "Holybro",
            "quantityInScene": count,
            "physics": {
                "thermalHover": specs["tBase"],
                "thermalPeak": specs["tMax"],
                "heatingIndex": specs["hRate"]
            },
            "ui": {
                "isHotspotTarget": specs["hotspot"],
                "hotspotLabel": specs["hLabel"]
            }
        }
        json_array.append(new_item)

synced_json_path = r"E:\WebGL_tesis\desarrollo\docs\investigacion\Holybro\x500v2_blender_synced_parts.json"
with open(synced_json_path, 'w', encoding='utf-8') as f:
    json.dump(json_array, f, indent=2, ensure_ascii=False)

md_path = r"E:\WebGL_tesis\desarrollo\docs\investigacion\Holybro\x500v2_blender_inventory.md"
with open(md_path, "w", encoding="utf-8") as f:
    f.write("\n".join(md_lines))

print(f"Generated successfully with thermal physics data. Found {total_pieces} total pieces.")
