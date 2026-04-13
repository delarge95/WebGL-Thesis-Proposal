# Justificación de Piezas Técnicas Curadas

Documento corto para decidir qué componentes del proyecto sí sirven como evidencia fuerte de portafolio y cuáles deben quedar como apoyo secundario o como material interno.

## Criterio de curaduría

Una pieza técnica es fuerte para portafolio si:

- resuelve un problema visible o crítico del producto;
- tiene trazabilidad clara con la app real;
- permite contar una historia técnica entendible en entrevista;
- no depende de prometer trabajo futuro como si ya estuviera terminado.

## Shaders y rendering

### Piezas fuertes

- `ClippableLit.shader`
  - Útil para explicar cómo se integró el flujo PBR de URP con clipping global, sin sobreafirmar que toda la BRDF fue escrita desde cero.
- `XRay.shader`
  - Fuerte para mostrar lectura estructural interna y modos analíticos.
- `Thermal.shader`
  - Fuerte como capa visual del sistema térmico híbrido.
- `WireframeWebGL.shader`
  - Fuerte porque resuelve una limitación real de WebGL sin depender de geometry shaders tradicionales.
- `Blueprint.shader`
  - Fuerte para mostrar lenguaje visual técnico y lectura de ingeniería.
- `EdgeDetection.shader`
  - Fuerte como apoyo para explicar realce de bordes y postproceso screen-space.

### Piezas buenas, pero secundarias

- `Ghosted.shader`
  - Técnicamente interesante, pero hoy es mejor presentarlo como profundidad adicional o modo implementado/oculto, no como feature principal visible.

## Runtime visible y arquitectura interactiva

### Piezas fuertes

- `UIManager.cs`
  - Coordina managers, paneles y estados visibles del producto.
- `SelectionManager.cs`
  - Muy buena pieza para hablar de raycast, hover, selección, eventos y relación con la UI.
- `UIDetailsSheet.cs`
  - Es la mejor evidencia de la transición entre selección 3D y lectura contextual visible al usuario.
- `ViewModeManager.cs`
  - Excelente para explicar modos visuales, fallback de shaders y separación entre capacidad implementada y capacidad publicada.
- `CrossSectionManager.cs`
  - Útil para hablar de clipping global, análisis estructural y matemática realmente implementada.
- `DroneStateController.cs`
  - Útil para explicar el vínculo entre estado operativo del dron y subsistemas visuales como térmico o power/load.
- `ImportedDroneRuntimeBinder.cs`
  - Pieza diferencial: demuestra saneamiento runtime y pensamiento sistémico sobre importación técnica.

### Piezas de apoyo

- `InputManager.cs`
  - Sigue siendo útil, aunque hoy conviene usarlo como soporte del visor más que como pieza protagonista.
- `UIHeroController.cs`
  - Importante para mostrar saneamiento del Hero y honestidad de producto, pero más como detalle de polish que como breakdown principal.

## Sistema térmico

### Piezas fuertes

- `ThermalSimulationManager.cs`
- `ThermalViewController.cs`
- `ThermalContactGraphBuilderWindow.cs`

Estas piezas juntas permiten contar una historia completa: entrada de estado operativo, resolución térmica heurística, traducción visual y tooling de autoría/validación.

## Tooling de editor y validación

### Piezas muy fuertes

- `ProjectSetupWizard.cs`
- `ImportedDroneCoverageAudit.cs`
- `ThermalContactGraphBuilderWindow.cs`

Estas herramientas son de las mejores piezas del portafolio porque prueban que el proyecto no es solo una escena bonita; también tiene utilidades para setup, auditoría y consistencia.

## Scripts y tooling de research

### Útiles como apoyo

- `cad_symmetry_addon_v5_ultimate.py`
- `cad_symmetry_addon_v6_batch.py`
- `generate_inventory.py`

Sirven para demostrar pensamiento de pipeline y automatización, pero deben usarse con una nota clara:

- son evidencia de tooling y R&D del flujo;
- no deben desplazar a las piezas más directamente conectadas con la build final visible.

## Piezas que no deben liderar el portafolio

- módulos legacy o no integrados del flujo final;
- `MeasurementTool` como si fuera UX publicada;
- scripts o ideas de ensamblaje completo no visibles en la build final;
- research sobre audio;
- tooling o módulos mencionados en documentos viejos pero no presentes en el repo activo.

## Recomendación final de curaduría

Si hay que seleccionar pocas piezas técnicas fuertes, la mejor combinación es:

1. `ImportedDroneRuntimeBinder.cs`
2. `SelectionManager.cs`
3. `ViewModeManager.cs`
4. `CrossSectionManager.cs`
5. `ThermalSimulationManager.cs`
6. `ProjectSetupWizard.cs`
7. `ImportedDroneCoverageAudit.cs`
8. `ThermalContactGraphBuilderWindow.cs`
9. `ClippableLit.shader`
10. `Thermal.shader`

Ese conjunto representa mejor el perfil buscado: Technical Artist orientado a herramientas, shaders, optimización y visualización técnica interactiva.
