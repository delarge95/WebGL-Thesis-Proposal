# Análisis de Recursos para Modelo de Dron

## Contexto
**Objetivos del usuario:**
1. Trabajo de grado (tesis) - Visor 3D interactivo de dron con componentes internos
2. Portafolio de Technical Artist - Demostrar habilidades de modelado y optimización

---

## Recursos Analizados

### 📐 Modelos CAD (Gratuitos)

| Recurso | Descripción | Componentes Internos | Estética | Licencia |
|---------|-------------|---------------------|----------|----------|
| **Parrot ANAFI** | Archivos STEP oficiales de drones comerciales | ⚠️ Solo carcasa externa | ✅ Profesional | Oficial (uso educativo) |
| **GitHub km5es** | F450 y Tarot T960 para calibración de antenas | ✅ Incluye PCBs y payloads | ❌ Funcional, no estético | MIT |
| **Sketchfab T-FLEX** | Modelo UAV de software CAD | ❓ Desconocido | ⭐ Medio | CC Attribution |
| **GrabCAD Compact** | Diseño de dron compacto | ❓ Requiere descarga | ❓ Variable | Comunidad |

### 🎨 Kitbash (Modular)

| Recurso | Descripción | Para Tesis | Para Portafolio |
|---------|-------------|-----------|----------------|
| **Kitbash3D Drones** | Partes modulares: rotores, gimbals, brazos, cuerpos | ❌ No demuestra habilidad propia | ⚠️ Solo como base/inspiración |
| **CGTrader Kitbash** | FBX con materiales PBR | ❌ Mismo problema | ⚠️ Referencia visual |

### 📚 Cursos/Tutoriales

| Curso | Software | Duración | Componentes Internos | Nivel |
|-------|----------|----------|---------------------|-------|
| **Hard Surface Masters Vol 1** | Modo + UE4 (transferible) | ~7 hrs | ❌ Solo exterior | Avanzado |
| **Black Hornet Drone** | Blender | ~5 hrs | ⚠️ Nano-dron militar (pequeño) | Intermedio+ |
| **Blender Bros Sci-Fi Drone** | Blender | 7+ hrs | ❌ Diseño exterior sci-fi | Intermedio |
| **Ryan King Art Security Drone** | Blender | ~4 hrs (serie) | ❌ Sci-fi exterior | Principiante+ |

### 🔧 Herramientas

| Tool | Función | Limitación |
|------|---------|------------|
| **Import CAD Model Addon** | Importa STEP/IGES a Blender via Mayo | Solo Windows |

---

## Problema Central

> **Los modelos CAD tienen componentes internos pero no son estéticos.**
> **Los modelos artísticos son estéticos pero no tienen componentes internos.**

---

## Recomendación: Enfoque Híbrido

### Para la Tesis (Prioridad: Componentes Internos)

1. **Usar Parrot ANAFI como base estructural**
   - Descarga el STEP oficial
   - Importa a Blender con el addon CAD
   - Esto te da proporciones y forma realista

2. **Combinar con referencia técnica de km5es**
   - Estudia cómo están organizados los PCBs, motores, ESCs
   - Modela componentes internos simplificados basándote en esto

3. **Modelar componentes internos propios**
   - Motor brushless (sección cortada)
   - ESC (Electronic Speed Controller)
   - Batería LiPo
   - Flight Controller
   - GPS/Compass module
   - Cableado

### Para el Portafolio (Prioridad: Estética + Demostrar Skill)

1. **NO uses Kitbash directamente** - No demuestra tu habilidad

2. **Sigue un curso estructurado**
   - **Recomendación**: Blender Bros Sci-Fi Drone (7 hrs, Blender nativo)
   - Aprenderás técnicas de hard surface profesionales

3. **Personaliza significativamente**
   - Toma el workflow del curso
   - Aplícalo a tu propio diseño de dron
   - Añade los componentes internos de tu investigación

---

## Plan de Acción Sugerido

### Fase 1: Investigación y Blocking (2-3 días)
- [ ] Descargar Parrot ANAFI STEP
- [ ] Instalar addon CAD para Blender
- [ ] Importar y estudiar proporciones
- [ ] Recopilar referencias de componentes internos reales

### Fase 2: Modelado Exterior (1 semana)
- [ ] Seguir curso Blender Bros (o el que elijas después de análisis con Gemini)
- [ ] Aplicar técnicas a tu diseño personalizado
- [ ] Mantener topología limpia para WebGL

### Fase 3: Modelado Interior (1 semana)
- [ ] Modelar componentes internos simplificados
- [ ] Asegurar que son separables para vista explosionada
- [ ] Crear DronePartData para cada componente

### Fase 4: Optimización para WebGL (3-4 días)
- [ ] Retopología si es necesario
- [ ] Baking de normales
- [ ] Exportar a Unity
- [ ] Probar en visor

---

## Preguntas Pendientes para Gemini 3 Pro

1. ¿El curso de Blender Bros cubre componentes internos o es solo exterior?
2. ¿Hay algún tutorial que combine estética + anatomía interna de drones?
3. ¿Cuál es la complejidad real del Black Hornet tutorial vs Blender Bros?
4. ¿Existe algún recurso que muestre el workflow completo CAD → Game-Ready?

---

*Documento generado para revisión colaborativa. Actualizar después del análisis con Gemini 3 Pro.*
