# 📚 Guía Rápida de Referencia - Proyecto WebGL

## 🎯 Información Esencial

**Proyecto:** Prototipo Web 3D Interactivo para Visualización Técnica de Hardware  
**Duración:** 6 meses (Enero - Junio 2025)  
**Metodología:** Design Science Research + Sprints de 4 semanas  
**Stack Principal:** Unity 6 WebGL + Blender + C#

---

## ⚡ KPIs Críticos

| Métrica | Target | Estado |
|---------|--------|--------|
| Polígonos | < 100,000 | ⏳ |
| FPS Móvil | > 30 FPS | ⏳ |
| Carga Shell | < 3s | ⏳ |
| Carga Completa | < 10s | ⏳ |
| Puntaje SUS | > 70 | ⏳ |

---

## 📅 Fases del Proyecto

1. **Planificación** (2 semanas) - Configuración inicial
2. **Entorno** (1 semana) - Instalación de herramientas
3. **Assets 3D** (3 semanas) - Modelado y optimización
4. **Sprint 1** (4 semanas) - Unity base + cámara
5. **Sprint 2** (4 semanas) - PBR + iluminación
6. **Sprint 3** (4 semanas) - UI + interactividad
7. **Sprint 4** (4 semanas) - Optimización
8. **Validación** (2 semanas) - Pruebas con usuarios
9. **Documentación** (1.5 semanas) - Manuales + informe
10. **Entrega** (0.5 semanas) - Deployment + presentación

---

## 🛠️ Herramientas Esenciales

### Desarrollo
- **Unity Hub + Unity 6 LTS** - Motor 3D
- **Visual Studio Code** - Editor de código
- **Git + Git LFS** - Control de versiones

### Modelado
- **Blender 4.x** - Modelado 3D y texturizado
- **Substance Painter** (opcional) - Texturizado avanzado

### Testing
- **Unity Profiler** - Análisis de rendimiento
- **Spector.js** - Debugging WebGL
- **BrowserStack** - Testing cross-browser

---

## 📂 Estructura de Carpetas

```
desarrollo/
├── unity_project/      # Proyecto Unity
├── blender_assets/     # Archivos Blender
├── textures/           # Texturas PBR
├── builds/             # Builds WebGL
├── testing/            # Resultados de pruebas
└── docs/               # Documentación técnica
```

---

## 🔑 Comandos Útiles

### Git
```bash
# Inicializar repositorio
git init
git lfs install

# Configurar LFS para archivos grandes
git lfs track "*.fbx"
git lfs track "*.blend"
git lfs track "*.png"

# Commit inicial
git add .
git commit -m "Initial project structure"
```

### Unity Build (WebGL)
```bash
# Desde Unity Editor:
# File > Build Settings > WebGL > Build
# Configurar: Compression = Brotli, Code Optimization = Size
```

### Blender Export
```bash
# Exportar FBX:
# File > Export > FBX (.fbx)
# Settings: Scale = 1.00, Forward = -Z, Up = Y
```

---

## 📊 Pipeline de Optimización

### 1. Modelado
- High-poly (500k-1M polígonos) → Low-poly (<100k)
- Retopología manual para áreas críticas
- UV Mapping optimizado

### 2. Texturizado
- Bake Normal Map desde high-poly
- Crear texturas PBR (Albedo, Metallic, Roughness, Normal)
- Texture Atlasing para reducir draw calls

### 3. Unity
- Configurar URP para móvil
- Implementar LODs (3 niveles)
- Occlusion Culling + Static Batching
- Asset Bundles para carga progresiva

### 4. WebGL
- Compresión Brotli
- WebAssembly habilitado
- Managed Stripping Level: High

---

## 🧪 Validación

### Pruebas Técnicas
- Unity Profiler (CPU, GPU, Memory)
- Testing en móviles mid-range
- Validación de KPIs

### Pruebas de Usabilidad (N=8-12)
- **SUS:** System Usability Scale (10 preguntas)
- **NASA-TLX:** Carga cognitiva (6 dimensiones)
- Entrevistas semi-estructuradas

---

## 📚 Documentos Clave

| Documento | Ubicación | Estado |
|-----------|-----------|--------|
| Hoja de Ruta | `desarrollo/HOJA_DE_RUTA.md` | ✅ |
| Estructura de Carpetas | `desarrollo/ESTRUCTURA_CARPETAS.md` | ✅ |
| Propuesta Final | `Propuesta/final_proposal.pdf` | ✅ |
| Manual Técnico | `Informe_final/Manual_tecnico/` | 🔄 |
| Manual de Usuario | `Informe_final/Manual_de_usuario/` | 🔄 |
| Informe Final | `Informe_final/informe_final.pdf` | ⏳ |

---

## 🚨 Checklist Pre-Sprint

Antes de comenzar cada Sprint, verificar:

- [ ] Objetivos del Sprint claros y medibles
- [ ] Herramientas necesarias instaladas
- [ ] Entorno de desarrollo funcional
- [ ] Backup del proyecto realizado
- [ ] Comunicación con asesor programada
- [ ] Tareas priorizadas en task.md

---

## 💡 Tips y Mejores Prácticas

### Desarrollo
- ✅ Commits frecuentes con mensajes descriptivos
- ✅ Testing temprano en dispositivos móviles
- ✅ Profiling desde Sprint 1
- ✅ Documentar decisiones técnicas importantes

### Optimización
- ✅ Medir antes de optimizar (profiling first)
- ✅ Optimizar lo que realmente impacta (80/20)
- ✅ Validar KPIs después de cada optimización
- ✅ No sacrificar calidad visual innecesariamente

### Académico
- ✅ Mantener coherencia con propuesta aprobada
- ✅ Documentar metodología aplicada
- ✅ Registrar resultados de validación
- ✅ Citar fuentes según APA 7

---

## 🔗 Enlaces Útiles

### Documentación Oficial
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Blender Manual](https://docs.blender.org/manual/en/latest/)
- [WebGL Best Practices](https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/WebGL_best_practices)

### Recursos de Aprendizaje
- [Unity Learn - WebGL](https://learn.unity.com/search?k=%5B%22q%3AWebGL%22%5D)
- [Blender Guru - PBR Texturing](https://www.youtube.com/user/AndrewPPrice)
- [Brackeys - Unity Tutorials](https://www.youtube.com/user/Brackeys)

### Herramientas Online
- [Spector.js](https://spector.babylonjs.com/) - WebGL Debugger
- [glTF Viewer](https://gltf-viewer.donmccurdy.com/) - Validar exports
- [Texture Packer](https://www.codeandweb.com/texturepacker) - Texture Atlasing

---

## 📞 Contacto

**Estudiante:** Alexander Woodcock Salomón  
**Email:** awoodcocks@unadvirtual.edu.co  
**Asesor:** Deivid Enrique Triviño Lozada  
**Programa:** Ingeniería Multimedia - UNAD

---

## 🎯 Próximos Pasos Inmediatos

1. ✅ Revisar hoja de ruta completa
2. ⏳ Instalar Unity 6 LTS + Blender 4.x
3. ⏳ Configurar Git y Git LFS
4. ⏳ Inicializar proyecto Unity
5. ⏳ Comenzar modelado high-poly

---

**Última actualización:** 2025-11-30  
**Versión:** 1.0  
**Estado:** Planificación Completa ✅
