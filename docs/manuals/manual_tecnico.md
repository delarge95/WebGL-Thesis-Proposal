---
tipo: entregable
area: manuales
estado: activo
trace_id: TRC-MAN-TEC-0001
entregable_ids: ["MAN-TEC-ARQ", "MAN-TEC-CONFIG", "MAN-TEC-OPT"]
script_ids: ["SCR-APP-001", "SCR-EVT-001", "SCR-SAVE-001", "SCR-PERF-001"]
bib_keys: ["hevner2004design", "peffers2007dsrm", "unity2024memory"]
resumen: "Manual tecnico del sistema WebGL y arquitectura de implementacion."
---

# Manual Técnico - WebGL Drone Viewer

## Stack Tecnológico

- **Motor**: Unity 6 (LTS)
- **Lenguaje**: C# (Scripting), HLSL (Shaders)
- **Plataforma**: WebGL 2.0 / WebAssembly
- **Frontend**: HTML5, CSS3, Vanilla JS

## Arquitectura del Proyecto

El proyecto sigue una **Clean Architecture** modificada para Unity:

### Capas Principales

1.  **Core**: Lógica de negocio pura (Stats, Data Models).
2.  **Infrastructure**: Implementación de repositorios y servicios externos.
3.  **Presentation (MonoBehaviours)**: Controladores de vista y componentes de Unity.

### Patrones de Diseño

- **Singleton**: Para Managers globales (`GameManager`, `UIManager`).
- **Observer**: Para el sistema de eventos (`EventBus`).
- **Factory**: Para la instanciación de piezas del dron.
- **Command**: Para el sistema de deshacer/rehacer acciones.

```mermaid
graph TD
    A[GameManager] --> B[UIManager]
    A --> C[DroneController]
    C --> D[PartManager]
    D --> E[PartDataRepository]
    B --> F[ViewModeSelector]
```

## Configuración de Build

Para compilar el proyecto en Unity:

1.  Abrir `Build Settings` (`Ctrl + Shift + B`).
2.  Seleccionar plataforma **WebGL**.
3.  Asegurarse de que `Code Optimization` esté en "Speed".
4.  Compresión: `Brotli` (o `Gzip` para iteración rápida).
5.  Dar clic en **Build And Run**.
