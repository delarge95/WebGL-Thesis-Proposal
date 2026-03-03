# Guía de Despliegue — WebGL Drone Viewer

> **Versión:** 1.0  
> **Última actualización:** Julio 2025  
> **Autor:** Alexander Woodcock Salomón  
> **Motor:** Unity 6000.0.62f1 · URP 17.0.3 · WebGL 2.0

---

## 1. Requisitos Previos

### 1.1 Software

| Herramienta | Versión | Propósito |
|---|---|---|
| Unity Hub | 3.x+ | Gestión de instalaciones Unity |
| Unity Editor | 6000.0.62f1 | Compilación del proyecto |
| Git | 2.40+ | Clonación del repositorio |
| Git LFS | 3.x+ | Assets binarios (texturas, modelos) |
| Node.js (opcional) | 18+ | Servidor local de pruebas |
| Navegador moderno | Chrome 110+ / Firefox 113+ / Edge 110+ | Pruebas WebGL |

### 1.2 Hardware mínimo (máquina de compilación)

- **CPU:** 4 cores / 8 threads
- **RAM:** 16 GB (recomendado 32 GB+)
- **GPU:** Compatible con DirectX 11 / Vulkan
- **Disco:** 20 GB libres (proyecto + caché de build)

---

## 2. Clonación del Repositorio

```bash
git lfs install
git clone https://github.com/delarge95/WebGL-Thesis-Proposal.git
cd WebGL-Thesis-Proposal
git checkout feature/phase2-ux-redesign
```

> **Nota:** El repositorio usa Git LFS para texturas y modelos 3D. Asegurarse de que `git lfs` esté instalado antes de clonar.

---

## 3. Apertura del Proyecto en Unity

1. Abrir **Unity Hub** → **Open** → Navegar a `desarrollo/unity_project/`
2. Si Unity solicita la versión `6000.0.62f1`, instalarla desde Hub → **Installs** → **Install Editor**
3. Al abrir el proyecto por primera vez:
   - Unity importará todos los paquetes (URP, UI Toolkit, Cinemachine, etc.)
   - La importación inicial puede tardar 5-10 minutos
4. Verificar que no haya errores en la consola (Console → Clear → verificar)

---

## 4. Configuración Pre-Build

### 4.1 Corrección Automática de Configuración

Unity provee un menú personalizado que ejecuta todos los _fixers_ necesarios:

```
Menú: WebGL > 🔧 Fix ALL WebGL Issues
```

Este comando ejecuta secuencialmente:

| # | Fixer | Función |
|---|---|---|
| 1 | URP Fixer | Configura URP Asset para WebGL (sombras, post-procesado) |
| 2 | Quality Fixer | Ajusta niveles de calidad para plataforma web |
| 3 | Shader Fixer | Pre-compila shaders, limpia variantes no utilizadas |
| 4 | Font Fixer | Asegura fuentes TMP para soporte multilínea |
| 5 | PanelSettings Fixer | Configura PanelSettings de UI Toolkit para resolución WebGL |

### 4.2 Player Settings (verificación manual)

| Parámetro | Valor Requerido |
|---|---|
| Product Name | `WebGL Drone Viewer` |
| Company Name | `AntigravityLabs` |
| Compression Format | Gzip (cambiar a Brotli para producción óptima) |
| WebGL Template | `PROJECT:Custom` |
| Initial Memory Size | 32 MB |
| Maximum Memory Size | 256 MB |
| Memory Growth Mode | Geometric (20%) |
| Linker Target | WASM |
| Name Files As Hashes | ✅ Enabled |
| Data Caching | ✅ Enabled |
| Exception Support | Explicitly Thrown Only |
| Strip Engine Code | ✅ Enabled |
| Managed Stripping Level | High |
| IL2CPP Configuration | Master |
| Power Preference | High Performance |

---

## 5. Compilación (Build)

### 5.1 Build Automatizado (Recomendado)

```
Menú: WebGL > 🚀 Fix & Build WebGL
```

Este comando:
1. Ejecuta todos los fixers (paso 4.1)
2. Compila el proyecto WebGL
3. Genera output en `../../docs/Build` (relativo al proyecto Unity → carpeta `docs/Build` en la raíz del repositorio)

### 5.2 Build Manual

1. **File** → **Build Settings**
2. Plataforma: **WebGL** → **Switch Platform** (si no está seleccionada)
3. Verificar que la escena `MainScene` esté en la lista de escenas
4. Clic en **Build**
5. Seleccionar carpeta de salida: `{repo}/docs/Build/`

### 5.3 Tiempos de Compilación Aproximados

| Máquina | Tiempo |
|---|---|
| i7-10700K / 48 GB / SSD | ~8-12 minutos |
| i5-8400 / 16 GB / HDD | ~20-30 minutos |
| Primera compilación | +5-10 min adicionales (caché IL2CPP) |

### 5.4 Archivos Generados

```
docs/Build/
├── index.html              # Punto de entrada
├── Build/
│   ├── Build.wasm.gz       # Código compilado (WASM + Gzip)
│   ├── Build.data.gz       # Assets serializados
│   ├── Build.framework.js  # Framework Unity WebGL
│   └── Build.loader.js     # Loader / bootstrap
├── TemplateData/           # Template personalizado (CSS, iconos, loading)
└── StreamingAssets/        # (si aplica)
```

### 5.5 Tamaños Objetivo

| Archivo | Objetivo (comprimido) |
|---|---|
| `.wasm.gz` / `.wasm.br` | < 15 MB |
| `.data.gz` / `.data.br` | < 20 MB |
| **Total** | **< 40 MB** |

---

## 6. Despliegue en GitHub Pages

### 6.1 Configuración del Repositorio

1. Ir a **Settings** → **Pages** en el repositorio de GitHub
2. Source: **Deploy from a branch**
3. Branch: `feature/phase2-ux-redesign` (o `main` cuando se haga merge)
4. Folder: `/docs`
5. Guardar

### 6.2 Publicación

```bash
# Después de compilar exitosamente
git add docs/Build/
git commit -m "build: WebGL production build v{X.Y.Z}"
git push origin feature/phase2-ux-redesign
```

GitHub Pages detecta automáticamente los cambios en `/docs` y despliega en:

```
https://delarge95.github.io/WebGL-Thesis-Proposal/Build/
```

### 6.3 Headers del Servidor (GitHub Pages)

GitHub Pages sirve automáticamente los headers correctos para archivos `.gz`. Si se usa **Brotli** (`.br`), se requiere configuración adicional del servidor o usar un hosting que soporte `Content-Encoding: br`.

| Extensión | Content-Type | Content-Encoding |
|---|---|---|
| `.wasm.gz` | `application/wasm` | `gzip` |
| `.data.gz` | `application/octet-stream` | `gzip` |
| `.js.gz` | `application/javascript` | `gzip` |
| `.wasm.br` | `application/wasm` | `br` |

---

## 7. Pruebas Post-Despliegue

### 7.1 Servidor Local de Pruebas

```bash
# Opción 1: Python
cd docs/Build
python -m http.server 8080

# Opción 2: Node.js (con soporte de compresión)
npx serve docs/Build -l 8080

# Opción 3: Live Server (VS Code)
# Abrir docs/Build/index.html → clic derecho → Open with Live Server
```

> **Importante:** WebGL no funciona abriendo `index.html` directamente como archivo (`file://`). Requiere un servidor HTTP.

### 7.2 Checklist de Verificación

- [ ] La aplicación carga sin errores en la consola del navegador
- [ ] El modelo 3D del dron se renderiza correctamente
- [ ] La navegación orbital (clic + arrastrar) funciona
- [ ] Los 7 modos de visualización cambian correctamente
- [ ] La selección de piezas muestra el panel de información
- [ ] La vista explosionada anima correctamente
- [ ] Los cortes transversales funcionan en los 3 ejes
- [ ] El catálogo de partes lista todas las piezas
- [ ] El rendimiento es aceptable (≥ 30 FPS en desktop promedio)
- [ ] El visor es responsivo en diferentes tamaños de ventana

### 7.3 Navegadores Soportados

| Navegador | Versión Mín. | WebGL 2.0 | Estado |
|---|---|---|---|
| Google Chrome | 110+ | ✅ | Principal |
| Mozilla Firefox | 113+ | ✅ | Soportado |
| Microsoft Edge | 110+ | ✅ | Soportado |
| Safari | 16.4+ | ⚠️ | Limitado (WebGL 2.0 parcial) |
| Safari iOS | 16.4+ | ⚠️ | Requiere pruebas adicionales |
| Chrome Android | 110+ | ✅ | Compatible (touch) |

---

## 8. Solución de Problemas

| Problema | Causa | Solución |
|---|---|---|
| Pantalla negra al cargar | CORS / archivos no servidos | Usar servidor HTTP, no `file://` |
| Error "Out of Memory" | Memory limit bajo | Aumentar `WebGL Maximum Memory Size` a 512 MB |
| Shaders no compilan | Variantes faltantes | Ejecutar `WebGL > Fix ALL WebGL Issues` y recompilar |
| Build muy grande (>50 MB) | Texturas sin comprimir | Verificar texture import settings, usar Crunch compression |
| FPS bajo (<20) | GPU integrada | WebGLOptimizer ajustará automáticamente; verificar `Power Preference: High Performance` |
| Caché corrupta | Build anterior roto | Limpiar `Library/` y `docs/Build/`, recompilar |
| Fuentes no se ven | TMP assets faltantes | Ejecutar Font Fixer o importar TMP Essentials |

---

## 9. Compresión: Gzip vs. Brotli

| Característica | Gzip | Brotli |
|---|---|---|
| Compresión | ~70% | ~80-85% |
| Velocidad de compresión | Rápida | Lenta |
| Soporte servidor | Universal | Requiere configuración |
| GitHub Pages | ✅ Automático | ⚠️ Requiere workaround |
| **Recomendación** | **Desarrollo / GitHub Pages** | **Producción con servidor propio** |

Para cambiar a Brotli:
1. **Edit** → **Project Settings** → **Player** → **WebGL** → **Publishing Settings**
2. Compression Format: `Brotli`
3. Decompression Fallback: `Enabled` (si el hosting no soporta `Content-Encoding: br`)

---

## 10. Notas de Producción

- **Cache busting:** Con `Name Files As Hashes` habilitado, cada build genera nombres de archivo únicos basados en hash, evitando problemas de caché del navegador.
- **Data Caching:** Habilitado para que los assets se almacenen en IndexedDB del navegador tras la primera carga.
- **HTTPS:** GitHub Pages sirve automáticamente sobre HTTPS, requerido para algunas APIs del navegador.
- **Versión:** Incluir el número de versión en el commit de build para trazabilidad.
