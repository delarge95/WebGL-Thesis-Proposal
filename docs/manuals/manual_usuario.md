---
tipo: entregable
area: manuales
estado: activo
trace_id: TRC-MAN-USR-0001
entregable_ids: ["MAN-USR-FLUJOS", "MAN-USR-VISTAS", "MAN-USR-PANELES"]
script_ids: ["SCR-VIEW-001", "SCR-UI-001", "SCR-UI-002", "SCR-ONB-001"]
bib_keys: ["mayer2005cambridge", "paivio1986mental", "nielsen1994usability"]
resumen: "Manual de usuario con flujos de uso y modos de visualizacion."
---

# Manual de Usuario - WebGL Drone Viewer

## Introducción

Bienvenido al visor técnico de drones. Esta herramienta permite explorar el ensamblaje y funcionamiento de un dron FPV mediante un modelo 3D interactivo de alta fidelidad.

## Controles de Interacción

### Navegación 3D

- **Orbitar**: Clic Izquierdo + Arrastre
- **Panear**: Clic Derecho + Arrastre (o Rueda + Arrastre en algunos modos)
- **Zoom**: Rueda del Ratón

### Interacción con Partes

- **Seleccionar**: Clic izquierdo en cualquier pieza para ver sus detalles técnicos (Nombre, Peso, Función).
- **Aislar**: Doble clic en una pieza para centrar la vista y ocultar el resto.
- **Reset**: Presione `R` o el botón de reset para volver a la vista inicial.

## Modos de Visualización

La aplicación cuenta con 7 modos distintos de renderizado para diferentes propósitos de análisis:

1.  **Estándar (PBR)**: Visualización realista con materiales físicos.
2.  **Rayos X**: Permite ver componentes internos a través de la carcasa.
3.  **Térmico**: Simulación de distribución de calor en motores y batería.
4.  **Wireframe**: Muestra la topología de la malla 3D.
5.  **Blueprint**: Estilo de plano técnico de ingeniería.
6.  **Despiece (Exploded View)**: Separa todas las piezas para ver el ensamblaje.
7.  **Night Mode**: Visualización con iluminación nocturna.

## Requisitos del Sistema

- Navegador compatible con **WebGL 2.0** (Chrome, Firefox, Edge, Safari actualizado).
- GPU dedicada recomendada para mejor rendimiento.
