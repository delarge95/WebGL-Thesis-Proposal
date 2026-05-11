---
tipo: columna_vertebral
estado: activo
area: sustentacion
version: 1
fecha: 2026-05-08
tags:
  - tesis
  - spine
  - sustentacion
---

# INF_EST_60 — Columna Vertebral de la Tesis

> **Propósito**: Documento de referencia rápida para recordar la esencia de la tesis, el hilo narrativo y las decisiones fundamentales en 60 segundos.

---

## Tesis en Una Frase

> Este proyecto demuestra que es posible traducir documentación técnica compleja de un dron en una **experiencia web 3D interactiva** que reduce fricción cognitiva, mejora la comprensión de relaciones espaciales entre componentes y mantiene viabilidad técnica en navegador sin sacrificar claridad funcional.

---

## Problema

**Qué falla hoy:**

- La documentación técnica de hardware complejo está fragmentada: manuales 2D, CAD pesado, planos estáticos.
- El usuario debe imaginar relaciones tridimensionales que no se presentan explícitamente.
- Esto incrementa carga cognitiva extrínseca (la que no suma a comprensión real).
- Los modelos CAD no son directamente desplegables en web sin perder viabilidad técnica.

**Por qué importa:**

- Ingenieros, técnicos y usuarios finales necesitan entender ensamblajes complejos rápidamente.
- La fricción actual genera retraso, frustración y comisión de errores.
- Una herramienta interactiva en web podría ser más accesible y útil que software especializado.

---

## Oportunidad (Gap)

**Lo que no existe hoy:**

- No hay un visor web 3D que combine:
  - Optimización de activos CAD para web
  - Interacción clara (orbitar, seleccionar, aislar)
  - Herramientas de inspección (vista explotada, corte, análisis térmico)
  - Rendimiento medible en navegador
  - Evaluación formal de usabilidad y esfuerzo percibido

---

## Propuesta

**Qué construí:**
Un prototipo web 3D interactivo del dron Holybro X500 V2 que permite:

1. **Explorar** el modelo completo con libertad orbital
2. **Seleccionar y aislar** piezas individuales o grupos
3. **Inspeccionar** mediante fichas técnicas, hotspots y metadatos
4. **Analizar** con vista explotada, corte transversal y filtros
5. **Visualizar** en múltiples modos (realista, X-Ray, análisis térmico, plano técnico)
6. **Entender relaciones** que en 2D requieren imaginación activa

**Tecnología:** Unity Web → WebGL + WebAssembly  
**Caso de estudio:** Holybro X500 V2 (hardware complejo, multicomponente)

---

## Decisiones Clave

| Decisión                                  | Alternativa rechazada                    | Razón                                                                                |
| ----------------------------------------- | ---------------------------------------- | ------------------------------------------------------------------------------------ |
| **Unity Web**                             | Three.js, Babylon.js, UE Pixel Streaming | Ecosistema integrado: editor, profiling, materiales, UI, sin infraestructura externa |
| **Optimización CAD**                      | Importar directo                         | CAD pesado no es viable en web; pipeline de limpieza es necesario                    |
| **PBR + Shaders custom**                  | Material básico                          | La legibilidad técnica requiere materiales coherentes y modos analíticos             |
| **Tres módulos (Inspect/Analyze/Studio)** | Un solo menú                             | Mobile-first design exige organización clara por tipo de pregunta                    |
| **NASA-TLX para workload**                | Carga cognitiva directa                  | Workload percibido es mensurable; teoría es interpretativa                           |
| **SUS solo para 3D**                      | Comparar 3D vs 2D                        | SUS evalúa un sistema; 2D es condición de referencia, no sistema equivalente         |

---

## Implementación Técnica (Síntesis)

**Pipeline:**

1. CAD → Blender (limpieza, retopología)
2. Blender → Materiales PBR, atlas de texturas
3. Blender → FBX exportado (Marmoset → baking)
4. FBX → Unity (importación, jerarquía, instancias)
5. Unity → Lógica (selección, modos, interacción)
6. Unity → WebGL build (Web export)

**Arquitectura:**

- Capa de datos: Piezas, categorías, hotspots, fasteners
- Capa de interacción: Selección, hover, aislamiento, cámara
- Capa visual: Shaders (realista, X-Ray, corte, térmico)
- Capa de UI: Mobile-first, bottom sheet, barra inferior

---

## Validación

**Qué se midió:**

- **KPIs técnicos:** FPS, frame time, memoria, tiempo de carga
- **Usabilidad:** SUS (System Usability Scale)
- **Esfuerzo percibido:** NASA-TLX Raw (3D vs 2D)
- **Experiencia cualitativa:** Think-Aloud protocol

**Hipótesis evaluada:**

- ¿El visor 3D reduce carga percibida vs. referencia 2D?
- ¿Es usable el prototipo? (SUS ≥ threshold)
- ¿Es técnicamente viable? (FPS > 30, carga razonable)

---

## Limitaciones (Honestas)

1. **Thermal es heurística, no FEA.** Muestra tendencias, no predicciones exactas.
2. **Compatibilidad móvil no cerrada.** Esperada, no universal; depende del navegador.
3. **Muestra de validación aún pendiente.** Meta: 30 participantes; mínimo operativo: 8-12.
4. **Modelo único.** X500 V2; arquitectura preparada para extensión a otros modelos.
5. **Build aún en congelamiento.** Algunos placeholders de métricas esperan freeze final.
6. **No es simulador.** No reemplaza validación aerodinámica, térmica o mecánica real.

---

## Contribución

### Técnica

- **Pipeline documentado** de CAD → Web 3D optimizado
- **Decisiones justificadas** sobre tecnología, trade-offs, arquitectura
- **Metodología replicable** para hardware complejo futuro

### Académica

- Demuestra viabilidad de **visualización técnica web 3D interactiva**
- Integra **cognición (carga cognitiva), gráficos (PBR, shaders), UX (mobile-first) e ingeniería (CAD, optimización)**
- Aporta a **Ingeniería Multimedia** un caso aplicado con evaluación formal

### Práctica

- Herramienta accesible para inspección, capacitación, documentación
- Patrón escalable a otros drones o hardware complejo

---

## Aporte Central (Lo que Cambia)

**Antes:** Usuario + documentación 2D → reconstrucción mental → fricción + posibles errores

**Después:** Usuario + visor web 3D → exploración interactiva, contexto preservado, fichas técnicas, análisis visual → comprensión directa

**Diferencia:** No es solo "mostrar bonito". Es construir una **herramienta de comprensión técnica** que externaliza carga cognitiva extrínseca hacia la interfaz.

---

## Frases Memorables (Para Ensayo)

1. **Hook:** "El manual muestra piezas. El visor ayuda a leer relaciones."
2. **Contraste 2D/3D:** "Entender un ensamblaje no es solo mirar componentes. Es entender relaciones."
3. **Pipeline CAD:** "El reto no era abrir un archivo. Era traducir ingeniería a experiencia interactiva."
4. **Honestidad:** "La tesis no se fortalece diciendo que todo está cerrado. Se fortalece mostrando qué está verificado y qué falta medir."
5. **Cierre:** "Esa es la diferencia entre mostrar un modelo y construir una herramienta de comprensión técnica."

---

## Mapa Mental (3 Pilares)

```
                    TESIS CENTRAL
                          |
          __________________+__________________
         |                  |                  |
    PROBLEMA          SOLUCIÓN            VALIDACIÓN
         |                  |                  |
    Fricción         Visor 3D             Evaluación
    Cognitiva        Interactivo          Formal
         |                  |                  |
    Documentación   Exploración         SUS, NASA-TLX
    fragmentada     Selección           Think-Aloud
    CAD pesado      Análisis            KPIs técnicos
         |                  |                  |
    ¿Cómo reducir?  ¿Cómo construir?    ¿Funciona?
    (Why)           (What/How)          (Evidence)
```

---

## Cronograma de 30 Minutos (Distribución de Tiempo)

| Bloque           | Tiempo        | Contenido                  |
| ---------------- | ------------- | -------------------------- |
| Hook + Problema  | 0:00 - 3:00   | Ficción visual, relevancia |
| Gap + Propuesta  | 3:00 - 6:00   | Por qué falta esto         |
| Pipeline + Tech  | 6:00 - 12:00  | Cómo lo construí           |
| Demo             | 12:00 - 15:00 | Prueba viva (o video)      |
| Módulos + Modos  | 15:00 - 22:00 | Funciones, arquitectura    |
| Resultados       | 22:00 - 26:00 | Métricas, evidencia        |
| Límites + Cierre | 26:00 - 30:00 | Honestidad, aporte final   |

---

## Checklist para Memorizar

- [ ] Apertura: problema es fricción → oportunidad es herramienta
- [ ] Central: tres módulos + modos + herramientas
- [ ] Cierre: diferencia entre mostrar y construir comprensión
- [ ] Honestidad: "verificado / pendiente de medir"
- [ ] Aporte: replicable, interdisciplinario, académicamente riguroso
