const fs = require("fs");
const path = require("path");

const root = path.resolve(__dirname, "../../../..");
const assetDir = path.join(
  root,
  "desarrollo/unity_project/Assets/Core/Data/X500V2Generated",
);

const replacements = [
  ["N/D", "N/A"],
  ["Fijación y apriete del chasis", "Chassis fastening and tightening"],
  ["Fijacion y apriete del chasis", "Chassis fastening and tightening"],
  ["Tornillería instrumento", "Instrument mounting hardware"],
  ["Tornilleria instrumento", "Instrument mounting hardware"],
  ["Tornillería rasante", "Countersunk fastening hardware"],
  ["Tornilleria rasante", "Countersunk fastening hardware"],
  ["Tuerca con brida", "Flange nut"],
  ["Tuerca ciega", "Cap nut"],
  ["Latón", "Brass"],
  ["Milimétrico", "Metric"],
  ["Milimetrico", "Metric"],
  ["Destornillador M2.5", "M2.5 screwdriver"],
  ["Destornillador M3", "M3 screwdriver"],
  [
    "Plataforma elevada para GPS y companion computer",
    "Elevated platform for GPS and companion computer",
  ],
  ["Taladros preperforados", "Pre-drilled holes"],
  [
    "Aísla magnéticamente el GPS de la PDB.",
    "Magnetically isolates the GPS from the PDB.",
  ],
  [
    "Aisla magnéticamente el GPS de la PDB.",
    "Magnetically isolates the GPS from the PDB.",
  ],
  ["Aislante anti-vibración", "Anti-vibration isolator"],
  ["Aislante anti-vibracion", "Anti-vibration isolator"],
  ["Posición", "Position"],
  ["Posicion", "Position"],
  ["Recepción", "Reception"],
  ["Recepcion", "Reception"],
  ["telemétrico", "telemetry"],
  ["telemetrico", "telemetry"],
  ["conmutación", "switching"],
  ["conmutacion", "switching"],
  ["medición", "measurement"],
  ["medicion", "measurement"],
  ["Fijación", "Fastening"],
  ["Fijacion", "Fastening"],
  ["Tornillería", "Fastener hardware"],
  ["Tornilleria", "Fastener hardware"],
  ["Métrica", "Metric"],
  ["Metrica", "Metric"],
  ["Nylon Reforzado", "Reinforced Nylon"],
  ["Acero Inox", "Stainless Steel"],
  ["Inyección plástico", "Injection-molded plastic"],
];

let filesChanged = 0;
let replacementsCount = 0;

const files = fs.readdirSync(assetDir).filter((f) => f.endsWith(".asset"));
for (const file of files) {
  const fullPath = path.join(assetDir, file);
  const original = fs.readFileSync(fullPath, "utf8");
  let updated = original;

  for (const [from, to] of replacements) {
    if (updated.includes(from)) {
      const beforeLen = updated.length;
      updated = updated.split(from).join(to);
      if (updated.length !== beforeLen || from !== to) {
        replacementsCount++;
      }
    }
  }

  if (updated !== original) {
    fs.writeFileSync(fullPath, updated, "utf8");
    filesChanged++;
  }
}

console.log(`Files changed: ${filesChanged}`);
console.log(`Replacement hits: ${replacementsCount}`);
