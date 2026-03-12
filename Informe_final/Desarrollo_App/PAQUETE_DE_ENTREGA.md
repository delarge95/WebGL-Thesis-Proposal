# Paquete de Entrega y Matriz de Coherencia

## Objetivo

Definir con una sola fuente de verdad qué se entrega realmente con el repositorio, qué partes del producto están expuestas en la versión actual del prototipo y qué elementos existen sólo como implementación parcial, soporte metodológico o trabajo pendiente.

Este documento evita tres errores comunes en la entrega:

1. Prometer en manuales o demos funcionalidades que no están expuestas en la UI actual.
2. Presentar instrumentos de validación como si ya fueran resultados cerrados.
3. Confundir artefactos de soporte técnico con entregables finales del producto.

---

## Entregables Confirmados

| Entregable                   | Ruta principal                                                                                                                                                                  | Estado            | Observación de entrega                                                                                      |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------- | ----------------------------------------------------------------------------------------------------------- |
| Prototipo WebGL reproducible | `docs/`, `docs/index.html`, `docs/Build/`, `.nojekyll`                                                                                                                          | Listo             | El build existe y puede entregarse como despliegue web o como respaldo local servido desde `docs/`.         |
| Manual técnico               | `Informe_final/Manual_tecnico/manual_tecnico.tex`, `Informe_final/Manual_tecnico/manual_tecnico.pdf`, `docs/manuals/manual_tecnico.md`, `docs/manuals/manual_tecnico.pdf`       | Listo             | Debe describir la arquitectura y el estado real del sistema, no roadmap disfrazado de funcionalidad activa. |
| Manual de usuario            | `Informe_final/Manual_de_usuario/manual_usuario.tex`, `Informe_final/Manual_de_usuario/manual_usuario.pdf`, `docs/manuals/manual_usuario.md`, `docs/manuals/manual_usuario.pdf` | Listo             | Su narrativa debe limitarse a la UI actual y separar claramente módulos futuros.                            |
| Guion y guía de demostración | `docs/manuals/DEMO_SCRIPT.md`, `Informe_final/Desarrollo_App/Documentacion_Tecnica/07_Guia_Demo.md`                                                                             | Listo             | La demo debe apoyarse sólo en flujos visibles del build actual o en respaldo local desde `docs/`.           |
| Instrumentos de validación   | `Informe_final/validacion/CUESTIONARIO_SUS.md`, `Informe_final/validacion/CUESTIONARIO_NASA_TLX.md`, espejos en `docs/manuals/`                                                 | Listo             | Son instrumentos listos para aplicación, no resultados ejecutados.                                          |
| Fuente de tesis final        | `Informe_final/informe_final.tex`, `Informe_final/chapters/`, `Informe_final/appendices/`                                                                                       | Lista como fuente | El repositorio prueba existencia de la fuente final; la aprobación académica ocurre fuera del repositorio.  |
| Trazabilidad y auditoría     | `Informe_final/Desarrollo_App/`                                                                                                                                                 | Lista             | Incluye bitácora, changelog, manifiesto de limpieza, auditorías y esta matriz de coherencia.                |

---

## Alcance Funcional Real del Prototipo Entregable

### Flujos visibles en la UI actual

#### Inspect

- Selección de piezas.
- Hoja inferior de información técnica.
- Hotspots/pines.
- Aislamiento de selección.
- Toggle de energía/encendido en estado experimental.

#### Analyze

- Corte transversal por ejes.
- Vista explosionada con slider inline.
- Filtros por categoría.

#### Studio

- Estado base `Realistic` al cargar la escena.
- Modos de render expuestos: `X-Ray`, `Solid` y `Thermal`.
- Ajustes de ambiente e iluminación del entorno.

---

## Funcionalidades Implementadas Pero No Prometibles Como UI Entregada

Los siguientes elementos existen en código o en documentación técnica, pero no deben presentarse como parte del flujo principal del build actual salvo demostración controlada y explícitamente contextualizada:

- `Blueprint`, `Wireframe` y `Ghosted`: implementados, pero no expuestos en la UI principal de esta versión.
- `MeasurementTool`: presente en el proyecto, pero fuera del prototipo final actual por baja prioridad frente a otras integraciones.
- `AssemblyGuideManager`, `AssemblyChecklist`, `BillOfMaterialsManager`, `AnnotationSystem` y `ConnectionPointsViewer`: presentes en el proyecto, pero no integrados en la navegación visible del prototipo actual.
- Simulación completa del dron como feature madura de producto: el control de energía existe, pero sigue en integración activa.

---

## Entregables Pendientes o Incompletos Que Deben Declararse Como Tales

| Elemento                                              | Estado real     | Cómo debe presentarse                                                                                                                                    |
| ----------------------------------------------------- | --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Informe de evaluación de usabilidad                   | Pendiente       | Indicar que SUS y NASA-TLX están preparados, pero que los resultados aún no se han ejecutado ni consolidado.                                             |
| Evidencia formal de KPIs                              | Pendiente       | No afirmar cumplimiento científico cerrado sin profiler capturas, métricas de carga, draw calls, VRAM y TTI documentados.                                |
| Exportaciones `.glb` y documentación de texel density | Pendiente       | No declarar entregado el paquete de optimización 3D completo mientras falten esos artefactos.                                                            |
| URL pública definitiva                                | Condicional     | Si GitHub Pages o el hosting final no están activos, entregar el build local como respaldo explícito.                                                    |
| Audio final integrado                                 | Pendiente       | Entregar la especificación técnica de audio, no presentar los clips como activos productivos ya incorporados.                                            |
| Encendido con flujo térmico progresivo                | Pendiente clave | Declararlo como desarrollo activo: el botón de encendido debe activar animación del dron y propagación gradual del calor por pieza visible en `Thermal`. |

---

## Regla de Coherencia Para Toda La Documentación

Al describir el producto, cada documento debe usar una de estas tres etiquetas implícitas, aunque no las escriba literalmente:

- **Entregado y visible en UI**
- **Implementado en código pero no integrado en UI**
- **Pendiente o preparado como instrumento/especificación**

Si una afirmación no puede ubicarse claramente en una de esas tres categorías, no debe quedar en un entregable público sin aclaración adicional.

---

## Checklist de Cierre Antes de Entregar

1. No afirmar que existe informe de usabilidad si sólo existen cuestionarios.
2. No afirmar que los 7 modos están disponibles al usuario si la UI actual expone sólo el subconjunto operativo.
3. No vender medición, assembly guide, BOM, anotaciones o connection points como flujo integrado del build actual.
4. Acompañar la demo con ruta de respaldo local desde `docs/` si la URL pública aún no está configurada.
5. Declarar el encendido con propagación térmica como pendiente prioritario hasta que quede integrado de punta a punta.
6. Mantener consistencia entre README, manuales, guía de demo y auditoría académica.

---

## Uso Recomendado de Este Documento

- Como referencia para la sustentación y la preparación del paquete final.
- Como control editorial antes de regenerar PDFs académicos.
- Como criterio para decidir qué se muestra en demo, qué se documenta como futuro y qué se declara como pendiente real.
