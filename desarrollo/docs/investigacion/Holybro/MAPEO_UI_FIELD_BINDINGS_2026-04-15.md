# MAPEO UI - CAMPOS Y BINDINGS (2026-04-15)

## Objetivo
Verificar que cada dato de DronePartData se renderiza en el lugar correcto de la UI y con formato consistente.

## 1) EnhancedInfoPanel (panel lateral superior)
Renderizado programatico (no UXML), seccion Overview y Materials.

| Campo fuente | Destino visual | Regla de formato | Evidencia |
|---|---|---|---|
| selectionLabel / canonicalPartName / partName | Titulo del panel | Prioridad: selectionLabel > canonicalPartName > partName | EnhancedInfoPanel.cs:268 |
| partType | Subtitulo (Category) | FormatTextOrND: vacio, '-', 'N/A' => N/D | EnhancedInfoPanel.cs:269, EnhancedInfoPanel.cs:295 |
| description | Overview > Description | FormatTextOrND | EnhancedInfoPanel.cs:270, EnhancedInfoPanel.cs:295 |
| weightKg | Overview > Weight | FormatWeight: <=0 => N/D, <0.01 => g, resto => kg | EnhancedInfoPanel.cs:271, EnhancedInfoPanel.cs:280 |
| dimensions | Overview > Dimensions | FormatTextOrND | EnhancedInfoPanel.cs:272, EnhancedInfoPanel.cs:295 |
| function | Overview > Function | FormatTextOrND | EnhancedInfoPanel.cs:273, EnhancedInfoPanel.cs:295 |
| materialType | Materials > Type | FormatTextOrND | EnhancedInfoPanel.cs:274, EnhancedInfoPanel.cs:295 |
| materialProperties | Materials > Properties | FormatTextOrND | EnhancedInfoPanel.cs:275, EnhancedInfoPanel.cs:295 |

## 2) UIDetailsSheet + MainLayout.uxml (sheet inferior)
El UXML define placeholders y el script enlaza y rellena valores.

### Placeholders UXML
- TopContextLabel: MainLayout.uxml:16
- SheetTitle: MainLayout.uxml:31
- PartDescription: MainLayout.uxml:44
- PartCategory: MainLayout.uxml:50
- PartFunction: MainLayout.uxml:54
- PartMaterial: MainLayout.uxml:62
- PartWeight: MainLayout.uxml:66
- PartDimensions: MainLayout.uxml:70
- PartPower: MainLayout.uxml:74
- PartTemp: MainLayout.uxml:78
- PartDifficulty: MainLayout.uxml:86
- PartTools: MainLayout.uxml:90
- PartAssemblyTime: MainLayout.uxml:94

### Enlace script -> placeholders
| Placeholder UXML | Campo script | Evidencia |
|---|---|---|
| TopContextLabel | _topContextLabel | UIDetailsSheet.cs:91 |
| SheetTitle | _sheetTitle | UIDetailsSheet.cs:78 |
| PartDescription | _sheetDesc | UIDetailsSheet.cs:82 |
| PartCategory | _sheetCategory | UIDetailsSheet.cs:79 |
| PartFunction | _sheetFunction | UIDetailsSheet.cs:80 |
| PartMaterial | _sheetMaterial | UIDetailsSheet.cs:81 |
| PartWeight | _sheetWeight | UIDetailsSheet.cs:83 |
| PartDimensions | _sheetDimensions | UIDetailsSheet.cs:84 |
| PartPower | _sheetPower | UIDetailsSheet.cs:85 |
| PartTemp | _sheetTemp | UIDetailsSheet.cs:86 |
| PartDifficulty | _sheetDifficulty | UIDetailsSheet.cs:87 |
| PartTools | _sheetTools | UIDetailsSheet.cs:88 |
| PartAssemblyTime | _sheetAssemblyTime | UIDetailsSheet.cs:89 |

### Renderizado de datos
| Campo fuente | Destino visual | Regla de formato | Evidencia |
|---|---|---|---|
| selectionLabel/canonicalPartName/partName | SheetTitle | TitleCase o hotspotGroupLabel | UIDetailsSheet.cs:254 |
| category | PartCategory | BuildCategoryText (en fasteners agrega subtipo/spec) | UIDetailsSheet.cs:258 |
| function / hotspotGroupSummary | PartFunction | FormatTextOrND + BuildFunctionText | UIDetailsSheet.cs:262 |
| materialType (+ blenderName si aplica) | PartMaterial | FormatTextOrND + BuildMaterialText | UIDetailsSheet.cs:265 |
| description | PartDescription | FormatTextOrND; para hotspot agrega Includes | UIDetailsSheet.cs:270, UIDetailsSheet.cs:276 |
| weightKg | PartWeight | FormatWeight: <=0 => N/D, <0.01 => g, resto => kg | UIDetailsSheet.cs:279, UIDetailsSheet.cs:554 |
| dimensions | PartDimensions | FormatTextOrND + BuildDimensionsText | UIDetailsSheet.cs:280, UIDetailsSheet.cs:569 |
| powerConsumption | PartPower | >0 => W, si no N/A | UIDetailsSheet.cs:281 |
| operatingTemp | PartTemp | >0 => C, si no N/A | UIDetailsSheet.cs:282 |
| difficultyLevel | PartDifficulty | Escala visual estrellas 0-5 | UIDetailsSheet.cs:287 |
| requiredTools / driveType | PartTools | BuildToolsText | UIDetailsSheet.cs:291 |
| installationTimeMinutes | PartAssemblyTime | >0 => minutos, si no N/A | UIDetailsSheet.cs:294 |

## 3) Vistas textuales auxiliares
| Componente | Regla aplicada | Evidencia |
|---|---|---|
| DronePartData.GetFullDescription | Weight usa FormatWeightForDisplay; textos usan NormalizeTextForDisplay (N/D si vacio) | DronePartData.cs:92, DronePartData.cs:163, DronePartData.cs:178 |
| BOM ExportToCSV | Pesos invalidos/no verificados salen como N/D; no se exportan negativos | BillOfMaterialsManager.cs:54, BillOfMaterialsManager.cs:95 |

## 4) Resultado de validacion
- No se detectaron cruces de campo incorrectos en los bindings revisados.
- Peso se presenta de forma consistente en panel superior, sheet inferior y descripcion textual.
- Campos vacios/placeholders se normalizan a N/D en vistas principales.
- CSV de BOM evita reportar pesos invalidos como numericos.

## 5) Nota de gobernanza de datos
- Valores de peso <= 0 se tratan como dato no disponible para visualizacion (N/D).
- En el dataset actual: 5 registros quedan como desconocidos intencionales (sentinela -1), y 0 registros quedan en cero.
