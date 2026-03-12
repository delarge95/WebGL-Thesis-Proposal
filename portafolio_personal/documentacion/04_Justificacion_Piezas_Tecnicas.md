# Justificación de Piezas Técnicas Curadas

Documento corto para explicar por qué cada pieza staged en `herramientas_fuente/` puede usarse como evidencia fuerte en portafolio, CV o entrevista técnica.

## Shaders

- `ClippableLit.shader`: demuestra adaptación de shading PBR con soporte para clipping global, útil para explicar visualización técnica y cortes transversales en WebGL.
- `XRay.shader`: sirve para mostrar lectura estructural interna con transparencia controlada y estética técnica defendible ante perfiles de rendering o UX técnica.
- `Thermal.shader`: funciona como base visual del modo térmico actual y conviene presentarlo como fundamento del sistema térmico; todavía no debe venderse como simulación completa de encendido o transferencia progresiva de calor.
- `WireframeWebGL.shader`: es vendible porque resuelve una limitación concreta de WebGL sin depender de Geometry Shaders tradicionales.
- `Blueprint.shader`: aporta valor porque conecta lenguaje visual técnico con un modo de lectura claramente orientado a ingeniería.
- `EdgeDetection.shader`: conviene incluirlo porque muestra postproceso en screen space con lectura de depth y normals, especialmente útil para explicar cómo se refuerza el modo Blueprint.
- `Ghosted.shader`: conviene incluirlo porque combina transparencia, fresnel, depth fade y compatibilidad con clipping, lo que lo vuelve una pieza más rica que un simple shader translúcido.

## UI técnica y managers

- `InputManager.cs`: sigue siendo pieza fuerte porque unifica ratón, touch y bloqueo contextual de input para una app 3D web híbrida.
- `OrbitCameraController.cs`: sirve para explicar navegación controlada, ergonomía de cámara y adaptación de interacción para exploración técnica.
- `UIDetailsSheet.cs`: aporta narrativa de producto porque conecta selección 3D con despliegue informativo y comportamiento de panel.
- `SelectionManager.cs`: conviene incluirlo porque concentra raycast, hover, selección, doble clic, bloqueo por UI y publicación de eventos; es una pieza muy útil para hablar de arquitectura de interacción.
- `ViewModeManager.cs`: conviene incluirlo porque centraliza el cambio de materiales, el fallback de shaders y la coordinación entre modos visuales; además es la pieza más cercana al trabajo en curso de encendido y vista térmica.
- `ProceduralIconBase.cs` y `ProceduralCloseIcon.cs`: son útiles como apoyo visual, pero pesan menos que los managers cuando el objetivo es vender arquitectura interactiva.

## Pipeline Holybro

- `cad_symmetry_addon_v5_ultimate.py`: aporta valor como evidencia de tooling propio para acelerar limpieza y simetría sobre geometría técnica.
- `cad_symmetry_addon_v6_batch.py`: es buena pieza para hablar de automatización por lotes y escalabilidad del pipeline CAD.
- `generate_inventory.py`: ayuda a mostrar trazabilidad e inventariado de piezas, algo muy defendible en contextos de Technical Art aplicado a hardware.

## Nota de proyección

- Cuando el sistema de encendido con animación y propagación térmica progresiva tenga una forma más estable, conviene sumar su script o manager principal como pieza candidata de portafolio.
- Hasta entonces, la forma correcta de presentarlo es como línea fuerte en desarrollo apoyada hoy por `Thermal.shader` y `ViewModeManager.cs`, no como sistema ya cerrado.