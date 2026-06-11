# Estructura de Carpetas - Proyecto WebGL

## 📁 Estructura Recomendada

```
WebGL_tesis/
├── desarrollo/                          # 🆕 Carpeta principal de desarrollo
│   ├── HOJA_DE_RUTA.md                 # Hoja de ruta detallada del proyecto
│   ├── ESTRUCTURA_CARPETAS.md          # Este archivo
│   │
│   ├── unity_project/                   # Proyecto Unity
│   │   ├── Assets/
│   │   │   ├── Scenes/
│   │   │   │   └── MainScene.unity
│   │   │   ├── Scripts/
│   │   │   │   ├── Camera/
│   │   │   │   │   └── OrbitCamera.cs
│   │   │   │   ├── Interaction/
│   │   │   │   │   ├── ObjectSelector.cs
│   │   │   │   │   └── ExplodedView.cs
│   │   │   │   ├── UI/
│   │   │   │   │   └── UIController.cs
│   │   │   │   └── Loading/
│   │   │   │       └── AssetBundleLoader.cs
│   │   │   ├── Materials/
│   │   │   │   └── PBR/
│   │   │   ├── Textures/
│   │   │   │   ├── Albedo/
│   │   │   │   ├── Normal/
│   │   │   │   ├── Metallic/
│   │   │   │   └── Roughness/
│   │   │   ├── Models/
│   │   │   │   └── Hardware/
│   │   │   ├── Prefabs/
│   │   │   ├── UI/
│   │   │   └── StreamingAssets/
│   │   ├── ProjectSettings/
│   │   └── Packages/
│   │
│   ├── blender_assets/                  # Archivos fuente Blender
│   │   ├── high_poly/
│   │   │   └── hardware_highpoly.blend
│   │   ├── low_poly/
│   │   │   └── hardware_lowpoly.blend
│   │   ├── exports/
│   │   │   └── hardware_final.fbx
│   │   └── references/
│   │       └── reference_images/
│   │
│   ├── textures/                        # Texturas PBR (fuente)
│   │   ├── baked/
│   │   │   ├── normal_map.png
│   │   │   └── ao_map.png
│   │   ├── pbr/
│   │   │   ├── albedo.png
│   │   │   ├── metallic.png
│   │   │   ├── roughness.png
│   │   │   └── normal.png
│   │   └── hdri/
│   │       └── environment.hdr
│   │
│   ├── builds/                          # Builds WebGL
│   │   ├── development/
│   │   └── production/
│   │
│   ├── testing/                         # Resultados de pruebas
│   │   ├── profiling/
│   │   │   ├── profiler_data/
│   │   │   └── screenshots/
│   │   ├── usability/
│   │   │   ├── sus_results.xlsx
│   │   │   ├── nasa_tlx_results.xlsx
│   │   │   └── session_notes/
│   │   └── performance/
│   │       └── metrics.xlsx
│   │
│   └── docs/                            # Documentación técnica
│       ├── architecture/
│       │   ├── component_diagram.png
│       │   └── data_flow.png
│       ├── pipeline/
│       │   └── optimization_pipeline.md
│       └── api/
│           └── script_documentation.md
│
├── Propuesta/                           # Propuesta de tesis (existente)
│   ├── final_proposal.tex
│   ├── final_proposal.pdf
│   ├── sections/
│   └── references.bib
│
├── Informe_final/                       # Informe final (existente)
│   ├── informe_final.tex
│   ├── informe_final.pdf
│   ├── Manual_tecnico/
│   └── Manual_de_usuario/
│
├── External_docs/                       # Documentación externa (existente)
│
└── README.md                            # README principal del repositorio
```

## 📝 Descripción de Carpetas Principales

### `/desarrollo/unity_project/`
Proyecto Unity completo. Contiene todos los assets, scripts, escenas y configuraciones del proyecto WebGL.

**Subcarpetas clave:**
- `Assets/Scripts/` - Todo el código C#
- `Assets/Models/` - Modelos 3D importados (.fbx)
- `Assets/Textures/` - Texturas optimizadas para Unity
- `Assets/Materials/` - Materiales PBR
- `ProjectSettings/` - Configuración del proyecto Unity

### `/desarrollo/blender_assets/`
Archivos fuente de Blender para modelado 3D.

**Subcarpetas:**
- `high_poly/` - Modelos high-poly para baking
- `low_poly/` - Modelos low-poly optimizados
- `exports/` - Archivos FBX exportados a Unity
- `references/` - Imágenes de referencia

### `/desarrollo/textures/`
Texturas PBR en formato fuente (alta resolución).

**Subcarpetas:**
- `baked/` - Mapas baked (normal, AO)
- `pbr/` - Texturas PBR (albedo, metallic, roughness, normal)
- `hdri/` - Imágenes HDRI para iluminación IBL

### `/desarrollo/builds/`
Builds WebGL generados por Unity.

**Subcarpetas:**
- `development/` - Builds de desarrollo (sin optimizaciones agresivas)
- `production/` - Builds de producción (optimizados, comprimidos)

### `/desarrollo/testing/`
Resultados de pruebas y validación.

**Subcarpetas:**
- `profiling/` - Datos de Unity Profiler
- `usability/` - Resultados de pruebas SUS y NASA-TLX
- `performance/` - Métricas de rendimiento

### `/Informe_final/Desarrollo_App/Documentacion_Tecnica/`
Documentación técnica seleccionada para el cierre académico público.

**Subcarpetas:**
- `01_Arquitectura_del_Sistema.md` - Arquitectura final documentada
- `04_Guia_Despliegue.md` - Ruta de despliegue y publicación
- `05_Configuracion_WebGL.md` - Configuración WebGL y mediciones

## 🔧 Configuración de Git

### `.gitignore` Recomendado

```gitignore
# Unity
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Unity3D generated meta files
*.pidb.meta
*.pdb.meta
*.mdb.meta

# Unity3D generated file on crash reports
sysinfo.txt

# Builds
*.apk
*.aab
*.unitypackage
*.app

# Blender
*.blend1
*.blend2

# OS
.DS_Store
Thumbs.db

# IDEs
.vscode/
.idea/
*.suo
*.user
*.sln.docstates
```

### Git LFS Configuration

```bash
# Configurar Git LFS para archivos grandes
git lfs track "*.fbx"
git lfs track "*.blend"
git lfs track "*.png"
git lfs track "*.jpg"
git lfs track "*.psd"
git lfs track "*.tga"
git lfs track "*.exr"
git lfs track "*.hdr"
git lfs track "*.mp4"
git lfs track "*.mov"
```

## 📦 Convenciones de Nombres

### Archivos 3D
- `hardware_highpoly.blend` - Modelo high-poly
- `hardware_lowpoly.blend` - Modelo low-poly
- `hardware_final.fbx` - Export final a Unity

### Texturas
- `hardware_albedo_2k.png` - Albedo 2048x2048
- `hardware_normal_2k.png` - Normal map 2048x2048
- `hardware_metallic_2k.png` - Metallic map 2048x2048
- `hardware_roughness_2k.png` - Roughness map 2048x2048

### Scripts C#
- `PascalCase` para nombres de clases y archivos
- Ejemplo: `OrbitCamera.cs`, `ObjectSelector.cs`

### Escenas Unity
- `PascalCase` para nombres de escenas
- Ejemplo: `MainScene.unity`, `TestingScene.unity`

## 🚀 Próximos Pasos

1. ✅ Crear estructura de carpetas base
2. ⏳ Inicializar proyecto Unity en `unity_project/`
3. ⏳ Configurar Git y Git LFS
4. ⏳ Comenzar modelado en Blender

---

**Última actualización:** 2025-11-30  
**Versión:** 1.0
