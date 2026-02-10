# Plan Definitivo: Modelado de Dron para Tesis y Portafolio

## Síntesis de Análisis

### Conclusiones de Opus 4.5
- Identificó correctamente el conflicto central: **CAD = funcional pero feo** vs **Art = estético pero vacío**
- Propuso enfoque híbrido usando Parrot CAD como base estructural
- Recomendó curso Blender Bros para técnicas de modelado

### Correcciones de Gemini 3 Pro
- **Validó** que ningún curso (Blender Bros, Hard Surface Masters, Black Hornet) cubre componentes internos
- **Refinó** el plan: No seguir el curso ciegamente, sino aplicar las *técnicas* a un diseño propio
- **Añadió** el concepto de "restricciones funcionales" como valor diferenciador para portafolio

---

## El Plan: "Inside-Out Workflow"

### Filosofía Central
> Diseñar la piel sobre el chasis, como un diseñador industrial real.

El modelo final tendrá:
1. **Internos realistas** (satisface la tesis)
2. **Exterior sci-fi estético** (satisface el portafolio)
3. **Optimización game-ready** (satisface el perfil Technical Artist)

---

## Fase 0: Preparación (1 día)

### Descargas Requeridas
| Recurso | Link | Propósito |
|---------|------|-----------|
| Parrot ANAFI STEP | https://www.parrot.com/en/cad-modeling | Base estructural |
| CAD Import Addon | https://github.com/chenpaner/Import-CAD-Model | Importar STEP a Blender |
| Blender Bros Course | (Comprar en ArtStation) | Técnicas de modelado |

### Configuración Blender
- [ ] Instalar Blender 4.0+ (LTS recomendado)
- [ ] Instalar addon "Import CAD Model"
- [ ] Configurar unidades en metros
- [ ] Crear proyecto: `Drone_Thesis_Portfolio`

---

## Fase 1: El Esqueleto - Ingeniería Inversa (3-4 días)

### Objetivo
Extraer la anatomía interna del CAD para usarla como "chasis funcional".

### Pasos
1. **Importar Parrot ANAFI STEP**
   - Usar calidad "Medium" para balance entre detalle y rendimiento
   - Escala: probablemente 0.001 (CAD suele estar en mm)

2. **Limpiar y Aislar Componentes**
   - Borrar la carcasa exterior (no la necesitamos)
   - Mantener solo:
     - [ ] Motores (x4)
     - [ ] Batería
     - [ ] Flight Controller / PCB
     - [ ] Módulo de cámara/gimbal
     - [ ] ESCs (si existen)
   - Nombrar cada objeto claramente: `INT_Motor_FL`, `INT_Battery`, etc.

3. **Simplificar Geometría**
   - Estos internos vienen con DEMASIADO detalle de CAD
   - Usar Decimate modifier o retopología manual
   - Target: ~500-2000 tris por componente interno

4. **Documentar Posiciones**
   - Captura de pantalla de la disposición desde arriba, frente, lado
   - Estas serán tus "restricciones" de diseño

### Entregable Fase 1
- Archivo: `drone_internals_base.blend`
- 5-8 objetos limpios representando los componentes internos
- Referencias visuales guardadas

---

## Fase 2: La Piel - Diseño Hard Surface (1-2 semanas)

### Objetivo
Aplicar técnicas del curso Blender Bros para diseñar una carcasa original alrededor de los internos.

### Flujo de Trabajo
1. **Empezar el curso Blender Bros** (primeras 3 horas)
   - Aprender las técnicas: Booleanas, Cutters, Bevels
   - NO terminar su dron, solo dominar las herramientas

2. **Diseñar TU carcasa**
   - Crear "blocking" (cajas) que envuelvan los internos de Fase 1
   - Aplicar las técnicas aprendidas para agregar:
     - Paneles y cortes
     - Ventilación (que coincida con posición de motores)
     - Bahía de batería accesible
     - Housing de cámara

3. **Refinamiento**
   - Añadir detalles secundarios (tornillos, ribetes, greebles)
   - Mantener siempre los internos visibles como referencia

### Restricciones de Diseño (Tu Valor Diferenciador)
| Componente Interno | Restricción de Diseño Exterior |
|-------------------|-------------------------------|
| Motores | Brazos deben terminar exactamente sobre ellos |
| Batería | Panel removible o transparente para acceso |
| PCB | Ventilación cercana para disipación |
| Cámara | Housing que permita movimiento del gimbal |

### Entregable Fase 2
- Archivo: `drone_exterior_highpoly.blend`
- Carcasa completa estilo sci-fi
- Internos aún visibles dentro del modelo

---

## Fase 3: Optimización Technical Artist (4-5 días)

### Objetivo
Convertir el modelo en un asset game-ready para WebGL.

### 3.1 Retopología
- Crear versión low-poly (~15,000 tris máximo para todo el dron)
- Mantener silueta limpia
- Quads donde sea posible

### 3.2 UV Mapping
- Usar Texture Atlas o Trim Sheets
- Máximo 2 materiales: `MAT_Drone_Body` y `MAT_Drone_Internals`

### 3.3 Baking
- Bake normal maps del high-poly al low-poly
- Bake AO para detalles de sombra

### 3.4 Exportación
- Formato: FBX para Unity
- Separar en objetos individuales por componente (para explosión)
- Naming convention: `Drone_Body`, `Drone_Motor_FL`, `Drone_Battery`, etc.

### Entregable Fase 3
- Archivo: `drone_game_ready.blend`
- Archivo: `drone_export.fbx`
- Texturas: `Drone_Albedo.png`, `Drone_Normal.png`, `Drone_AO.png`

---

## Fase 4: Integración Unity (3-4 días)

### Objetivo
Importar a Unity y configurar para el visor WebGL.

### Pasos
1. Importar FBX a `Assets/Models/Drone/`
2. Crear `DronePartData` ScriptableObject para cada componente
3. Añadir componente `ExplodablePart` a cada pieza
4. Configurar materiales URP
5. Probar vista explosionada
6. Implementar shader X-Ray (opcional, para ver internos sin explotar)

---

## Cronograma Sugerido

| Semana | Fase | Horas Estimadas |
|--------|------|-----------------|
| 1 | Fase 0 + Fase 1 | 10-12 hrs |
| 2 | Fase 2 (Curso + Diseño) | 15-20 hrs |
| 3 | Fase 2 (Refinamiento) + Fase 3 | 12-15 hrs |
| 4 | Fase 4 (Unity) | 8-10 hrs |

**Total: ~45-55 horas de trabajo**

---

## Checklist Final

### Para la Tesis
- [ ] Modelo tiene componentes internos identificables
- [ ] Vista explosionada funciona correctamente
- [ ] Cada parte tiene información técnica (DronePartData)
- [ ] Funciona en WebGL con buen rendimiento

### Para el Portafolio
- [ ] Diseño exterior es original y estético
- [ ] Demuestra dominio de hard-surface
- [ ] Incluye renders de alta calidad
- [ ] Documentación del proceso (breakdown)
- [ ] Video turntable del modelo
- [ ] Capturas del visor WebGL funcionando

---

## Recursos Adicionales

### Referencias de Internos de Drones
- iFixit Teardowns de drones DJI/Parrot
- YouTube: "Drone disassembly" videos
- GitHub km5es: Layouts de PCB y payloads

### Inspiración Estética
- ArtStation: Buscar "Sci-Fi Drone Concept"
- Pinterest: Tablero de drones futuristas
- Kitbash3D: Solo como referencia visual, NO para usar directamente

---

*Plan generado combinando análisis de Opus 4.5 y Gemini 3 Pro. Fecha: 2025-12-11*
